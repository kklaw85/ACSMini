using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace HiPA.Common
{
	#region Config/Instance 
	[Serializable]
	public class ConfigInstance
	{
		public Configuration Config = null;

		[NonSerialized]
		[XmlIgnore]
		public InstrumentBase Instrument = null;
	}
	#endregion

	#region Equipment Configuration 
	public abstract class EquipmentConfiguration
		: Configuration
	{
		[NonSerialized]
		[System.Xml.Serialization.XmlIgnore]
		protected object SyncRoot;

		//[NonSerialized]
		//private static readonly EquipmentConfig _DEFAULT = new EquipmentConfig();

		//public override string Name { get; set; } = "Neowise";

		public override InstrumentCategory Category => InstrumentCategory.Equipment;

		public string LogPath { get; set; } = "";
		public XmlDictionary<string, ConfigInstance> ConfigInstances = new XmlDictionary<string, ConfigInstance>();
		public override void CheckDefaultValue()
		{
			this.SyncRoot = new object();

			this.LogPath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Log" );
			if ( Directory.Exists( this.LogPath ) == false )
				Directory.CreateDirectory( this.LogPath );

			base.CheckDefaultValue();
		}


		#region Config Add/Remove
		public string AddInstrument( string name, Configuration config, InstrumentBase instrument )
		{
			var title = "Add Instrument Failure";

			if ( string.IsNullOrEmpty( name ) == true ) return Equipment.ErrManager.RaiseError( null, $"Instrument name is empty", title, ErrorClass.E6 );
			if ( config == null ) return Equipment.ErrManager.RaiseError( null, $"Configuration instance is null", title, ErrorClass.E6 );
			if ( instrument == null ) return Equipment.ErrManager.RaiseError( null, $"Instrument instance is null", title, ErrorClass.E6 );

			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( this.ConfigInstances.ContainsKey( name ) == true ) return Equipment.ErrManager.RaiseError( null, $"Name[{name}] has exists", title, ErrorClass.E6 );
				this.ConfigInstances.Add( name, new ConfigInstance { Config = config, Instrument = instrument } );
			}
			catch ( Exception ex )
			{
				return Equipment.ErrManager.RaiseError( null, $"Add Instrument has occurred exception. Name[{name}], Error[{ex.Message}]", title, ErrorClass.E6, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}

			return string.Empty;
		}
		public string RenameInstrument( string oldName, string newName )
		{
			var title = "Rename Instrument Failure";

			if ( string.IsNullOrEmpty( oldName ) == true ) return Equipment.ErrManager.RaiseError( null, $"Instrument old name is empty", title, ErrorClass.E6 );

			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( this.ConfigInstances.ContainsKey( oldName ) == false ) return Equipment.ErrManager.RaiseError( null, $"Old Instrument[{oldName}] has not exists", title, ErrorClass.E6 );
				if ( this.ConfigInstances.ContainsKey( newName ) == true ) return Equipment.ErrManager.RaiseError( null, $"New Instrument[{newName}] has exists", title, ErrorClass.E6 );

				var config = this.ConfigInstances[ oldName ];
				config.Config.Name = newName;

				this.ConfigInstances.Remove( oldName );
				this.ConfigInstances.Add( newName, config );
			}
			catch ( Exception ex )
			{
				return Equipment.ErrManager.RaiseError( null, $"Add Instrument has occurred exception. Name[{oldName}], Error[{ex.Message}]", title, ErrorClass.E6, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}

			return string.Empty;
		}

		public ConfigInstance GetConfigInstance( string name, bool errorIfNotFound = false )
		{
			var title = "Add Instrument Failure";

			if ( string.IsNullOrEmpty( name ) == true )
			{
				Equipment.ErrManager.RaiseError( null, $"Instrument name is empty", title, ErrorClass.E6 );
				return null;
			}

			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( this.ConfigInstances.ContainsKey( name ) == false )
				{
					if ( errorIfNotFound ) Equipment.ErrManager.RaiseError( null, $"Name[{name}] not exists", title, ErrorClass.E6 );
					return null;
				}
				return this.ConfigInstances[ name ];
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, $"Add Instrument has occurred exception. Name[{name}], Error[{ex.Message}]", title, ErrorClass.E6, ex );
				return null;
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}


		public string RemoveInstrument( string name )
		{
			var result = string.Empty;
			var title = "Remove Instrument Failure";

			if ( string.IsNullOrEmpty( name ) == true ) return Equipment.ErrManager.RaiseError( null, $"Instrument name is empty", title, ErrorClass.E6 );

			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( this.ConfigInstances.ContainsKey( name ) == false ) return Equipment.ErrManager.RaiseError( null, $"Name[{name}] has not exists", title, ErrorClass.E6 );
				this.ConfigInstances.Remove( name );
			}
			catch ( Exception ex )
			{
				return Equipment.ErrManager.RaiseError( null, $"Add Instrument has occurred exception. Name[{name}], Error[{ex.Message}]", title, ErrorClass.E6, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}

			return result;
		}
		#endregion
	}
	#endregion

	#region Equipment Implementation
	public abstract class Equipment
		: ModuleBase
	{
		public EquipmentConfiguration Configuration { get; protected set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Equipment;
		public Equipment( Configuration configuration )
		{
			this.Configuration = configuration as EquipmentConfiguration;
		}
		public virtual string InitModules()
		{
			var result = string.Empty;
			try
			{
				foreach ( var key in this.Configuration.ConfigInstances.Keys )
				{
					var config = this.Configuration.GetConfigInstance( key );
					var instance = Activator.CreateInstance( config.Config.InstrumentType ) as InstrumentBase;

					if ( instance == null )
						throw new Exception( $"Name[{key}], Type[{config.Config.InstrumentType}] create instance failed" );

					instance.ApplyConfiguration( config.Config );
					config.Instrument = instance;
					config.Config.CheckDefaultValue();
				}
				if ( ( result = this.Create().Result ) != string.Empty ) return result;
			}
			catch ( Exception ex )
			{
				result = $"{this.Name}:InitModules:,{ex.Message}";
			}
			return result;
		}
		public virtual void Shutdown()
		{
			Constructor.GetInstance().Save();
			Constructor.GetInstance().Equipment.Stop().Wait();
			Constructor.GetInstance().Equipment.Terminate().Wait();
		}
		public override void ApplyConfiguration( Configuration configuration )
		{
		}
		public static LoginUserMgt UserMgt { get; set; } = new LoginUserMgt();
		public static MachineStateMgr MachStateMgr { get; private set; } = new MachineStateMgr();
		public static ErrorManager ErrManager { get; private set; } = new ErrorManager();
	}
	#endregion
}
