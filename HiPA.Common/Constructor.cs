using HiPA.Common.Forms;
using HiPA.Common.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HiPA.Common
{
	public class Constructor : BaseUtility
	{
		public static string AppTitle { get; set; } = "";

		object SyncRoot = new object();
		public string StartupPath { get; set; } = "";
		public string DataPath { get; set; } = "";
		public string DataLoggerPath { get; set; } = "";
		public string ConfigPath { get; set; } = "";
		public string RecipePath { get; set; } = "";
		public string ResourceFilePath { get; set; } = "";
		public string SystemFilesPath { get; set; } = "";
		public string ActivityLogPath { get; set; } = "";
		public string ConfigFile { get; set; } = "";
		public string AppVersion { get; set; } = "";

		#region Equipment 
		public Type EquipmentConfigurationType { get; set; }
		public EquipmentConfiguration Configuration { get; private set; }
		public Equipment Equipment { get; private set; }
		#endregion

		#region Config File Locker
		FileStream _configStream = null;
		#endregion

		protected Constructor()
		{
			this.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.StartupPath = AppDomain.CurrentDomain.BaseDirectory;
			this.ConfigPath = Path.Combine( this.StartupPath, "Config" );
			this.DataPath = Path.Combine( this.StartupPath, "Data" );
			this.DataLoggerPath = Path.Combine( this.DataPath, "Logger" );
			this.ActivityLogPath = Path.Combine( this.StartupPath, "Globalization\\en-US\\License" );
			this.SystemFilesPath = Path.Combine( this.StartupPath, "SystemFiles" );
			this.ResourceFilePath = Path.Combine( this.StartupPath, "ResourceFiles" );
			this.ConfigFile = Path.Combine( this.ConfigPath, "Configuration.xml" );
			this.RecipePath = Path.Combine( this.StartupPath, "RecipeHandler" );

			if ( Directory.Exists( this.ConfigPath ) == false )
				Directory.CreateDirectory( this.ConfigPath );

			if ( Directory.Exists( this.RecipePath ) == false )
				Directory.CreateDirectory( this.RecipePath );
		}

		#region Load/Save/Shutdown
		int _isLoaded = 0;
		public bool IsLoaded
		{
			get => Interlocked.CompareExchange( ref this._isLoaded, 1, 1 ) == 1;
			set => Interlocked.Exchange( ref this._isLoaded, value ? 1 : 0 );
		}

		public string Load()
		{
			if ( this.IsLoaded == true ) return string.Empty;

			var result = string.Empty;
			try
			{
				if ( this.EquipmentConfigurationType == null )
					throw new Exception( "EquipmentConfigurationType is null" );

				if ( ( result = this.LoadConfigurationTypes() ) != String.Empty ) throw new Exception( result );

				if ( File.Exists( this.ConfigFile ) )
				{
					try
					{
						this.Configuration = ( EquipmentConfiguration )XmlHelper.XmlDeserializeFromFile( this.EquipmentConfigurationType, this.ConfigFile, this._xmlExtraTypes );

						//foreach ( var obj in this.Configuration.ConfigInstances )   //Update all SubModules
						//{
						//	obj.Value.Config.MachineType = this.Configuration.MachineType;
						//	obj.Value.Config.LaserType = this.Configuration.LaserType;
						//}
					}
					catch ( Exception ex )
					{
						this.Configuration = null;
					}
				}

				if ( this.Configuration == null )
				{
					this.Configuration = Activator.CreateInstance( this.EquipmentConfigurationType ) as EquipmentConfiguration;
				}

				this._configStream = this.GetConfigStream();
				this.Configuration.CheckDefaultValue();

				this.Equipment = Activator.CreateInstance( this.Configuration.InstrumentType, new object[] { this.Configuration } ) as Equipment;
				//this.Equipment.Logger = LogManager.GetLogger( "Equipment" );
				this.Equipment._Logger = new LoggerHelper( "Equipment.Log", "Logger" );

				if ( ( result = this.Equipment.InitModules() ) != string.Empty ) throw new Exception( result );

			}
			catch ( Exception ex )
			{
				result = Equipment.ErrManager.RaiseError( null, this.FormatErrMsg( "Constructor", ex ), "Load Configuration", ErrorClass.E6, ex );
			}
			return result;
		}
		public string Save()
		{
			try
			{
				//foreach ( var obj in this.Configuration.ConfigInstances )   //Update all SubModules
				//{
				//	obj.Value.Config.MachineType = this.Configuration.MachineType;
				//	obj.Value.Config.LaserType = this.Configuration.LaserType;
				//}

				var stream = XmlHelper.XmlSerialize( this.Configuration, this._xmlExtraTypes );
				var data = Encoding.UTF8.GetBytes( stream );
				this._configStream.Position = 0;
				this._configStream.Write( data, 0, data.Length );
				this._configStream.Flush();
				this._configStream.SetLength( data.Length );

				//var configData = Serialize( this._equipmentConfig );
				//var sw = new BinaryWriter( this._configStream );
				//sw.Write( configData );
				//sw.Flush();
				return String.Empty;
			}
			catch ( Exception ex )
			{
				//this.Logger.Error( $"Save Config failure.", ex );
				return Equipment.ErrManager.RaiseError( null, $"Save config failure. Error[{ex.Message}]", "Save Configuration", ErrorClass.E6, ex );
			}
			finally
			{
				this._configStream.Close();
				this._configStream = null;
				this._configStream = this.GetConfigStream();
			}
		}
		protected FileStream GetConfigStream()
		{
			var path = Path.GetDirectoryName( this.ConfigFile );
			if ( Directory.Exists( path ) == false )
				Directory.CreateDirectory( path );
			return new FileStream( this.ConfigFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read );
		}

		public int Shutdown()
		{
			_unique?.ReleaseMutex();
			_unique?.Close();
			_unique = null;

			this.Equipment?.Shutdown();

			return 0;
		}
		#endregion

		#region Configuration Serialize/Deserialize
		public static byte[] Serialize( object instance, SerializationBinder sb = null )
		{
			byte[] ret = null;
			using ( MemoryStream ms = new MemoryStream() )
			{
				BinaryFormatter bf = new BinaryFormatter();

				if ( sb != null ) bf.Binder = sb;
				//else bf.Binder = new HiPAModuleBinder();

				bf.Serialize( ms, instance );
				ms.Seek( 0, SeekOrigin.Begin );
				ret = ms.ToArray();
			}
			return ret;
		}

		public static object Deserialize( byte[] data, SerializationBinder sb = null )
		{
			object ret = null;
			using ( MemoryStream ms = new MemoryStream() )
			{
				ms.Write( data, 0, data.Length );
				ms.Seek( 0, SeekOrigin.Begin );

				BinaryFormatter bf = new BinaryFormatter();

				if ( sb != null ) bf.Binder = sb;
				//else bf.Binder = new HiPAModuleBinder();

				ret = bf.Deserialize( ms );
			}

			return ret;
		}
		#endregion

		#region Singlton Instance 
		static Constructor _instance = null;
		public static Constructor GetInstance()
		{
			if ( _instance == null )
				_instance = new Constructor();
			return _instance;
		}

		static Mutex _unique = null;
		public static bool CheckProcessUnique()
		{
			try
			{
				if ( Mutex.TryOpenExisting( System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, out _unique ) == false )
					_unique = new Mutex( true, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name );
				else
					return false;
			}
			catch ( WaitHandleCannotBeOpenedException ex )
			{
				_unique = null;
				new Thread( () => MessageBox.Show( $"Constructor.cs :CheckProcessUnique:+:WaitHandle:{ex.Message}" ) ).Start();
				return false;
			}
			catch ( UnauthorizedAccessException ex )
			{
				_unique = null;
				new Thread( () => MessageBox.Show( $"Constructor.cs :CheckProcessUnique:+:Unauthorized:{ex.Message}" ) ).Start();
				return false;
			}
			catch ( Exception ex )
			{
				_unique = null;
				new Thread( () => MessageBox.Show( $"Constructor.cs :CheckProcessUnique:+:Exception:{ex.Message}" ) ).Start();
				return false;
			}
			return true;
		}
		#endregion

		#region GetInstrument
		public InstrumentBase GetInstrument( string name, Type type, MachineVar MachineVar )
		{
			ConfigInstance configInstance = null;
			try
			{
				Monitor.Enter( this.SyncRoot );
				configInstance = this.Configuration.GetConfigInstance( name, type == null );
				if ( configInstance != null && configInstance.Instrument != null )
				{
					configInstance.Instrument.MachineVar = new MachineVar( MachineVar );
					return configInstance.Instrument;
				}

				if ( configInstance == null )
					configInstance = new ConfigInstance();

				if ( configInstance.Config == null )
				{
					var config = Activator.CreateInstance( type ) as Configuration;
					if ( config == null )
					{
						Equipment.ErrManager.RaiseError( null, "Constructor.cs :GetInstrument:+:Config CreateInstance: ", "", ErrorClass.E6 );
						return null;
					}
					config.Name = name;
					configInstance.Config = config;
					config = null;
				}
				configInstance.Config.CheckDefaultValue();

				if ( configInstance.Instrument == null )
				{
					var instType = type.GetProperty( "InstrumentType" )?.GetValue( configInstance.Config ) as Type;
					if ( instType == null )
					{
						Equipment.ErrManager.RaiseError( null, "Constructor.cs :GetInstrument:+:Config GetProperty: ", "", ErrorClass.E6 );
						return null;
					}

					var instrument = Activator.CreateInstance( instType ) as InstrumentBase;
					if ( instrument == null )
					{
						Equipment.ErrManager.RaiseError( null, "Constructor.cs :GetInstrument:+:Instrument CreateInstance: ", "", ErrorClass.E6 );
						return null;
					}
					instrument.ApplyConfiguration( configInstance.Config );
					configInstance.Instrument = instrument;
					instrument = null;
				}
				configInstance.Instrument.MachineVar = new MachineVar( MachineVar );

				if ( this.Configuration.AddInstrument( name, configInstance.Config, configInstance.Instrument ) != string.Empty )
				{
					return null;
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, $"GetInstrument occurred exception. Error:{ex.Message}", ErrorTitle.OperationFailure, ErrorClass.E6 );
				return null;
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}

			return configInstance.Instrument;
		}
		public InstrumentBase GetInstrument( string name, Type config )
		{
			return this.GetInstrument( name, config, new MachineVar( true ) );
		}
		public InstrumentBase GetInstrument( Type config, MachineVar machinevar )
		{
			var fName = config.GetField( "NAME", BindingFlags.Static | BindingFlags.Public );
			var name = ( string )fName.GetValue( null );
			return this.GetInstrument( name, config, machinevar );
		}
		public InstrumentBase GetInstrument( Type config )
		{
			var fName = config.GetField( "NAME", BindingFlags.Static | BindingFlags.Public );
			var name = ( string )fName.GetValue( null );
			return this.GetInstrument( name, config, new MachineVar( true ) );
		}
		public InstrumentBase GetInstrument( string instrumentName )
		{
			try
			{
				Monitor.Enter( this.SyncRoot );
				var configInstance = this.Configuration.GetConfigInstance( instrumentName, false );
				return configInstance == null ? null : configInstance.Instrument;
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}
		#endregion

		#region Check Configuration Types 
		Type[] _xmlExtraTypes = null;
		string LoadConfigurationTypes()
		{
			var result = string.Empty;
			try
			{
				var type = typeof( HiPA.Common.Configuration );

				this.LoadAssemblies( new Type[] { type } );

				var types = HiPA.Common.Utils.ReflectionTool.QueryConfigurationTypes( new Type[] { type } );
				if ( types.TryGetValue( type, out var pair ) == true )
				{
					pair.Add( typeof( MachineVar ) );
					this._xmlExtraTypes = pair.ToArray();
				}
			}
			catch ( Exception ex )
			{
				result = $",Constructor:,LoadConfigurationTypes:,{ex.Message}";
			}
			return result;
		}
		#endregion


		void LoadAssemblies( IEnumerable<Type> validType )
		{
			var files = Directory.GetFiles( this.StartupPath, "HiPA.*.dll" );

			foreach ( var file in files )
				Assembly.LoadFile( file );
		}
	}



}
