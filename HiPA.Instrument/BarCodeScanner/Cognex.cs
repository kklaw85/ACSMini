using Cognex.DataMan.SDK;
using Cognex.DataMan.SDK.Discovery;
using Cognex.DataMan.SDK.Utils;
using HiPA.Common;
using HiPA.Common.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;

namespace HiPA.Instrument.BarCodeScanner
{
	[Serializable]
	public class Dataman70xConfiguration
		: Configuration
	{
		[NonSerialized]
		public static readonly Dataman70xConfiguration _DEFAULT = new Dataman70xConfiguration();
		[NonSerialized]
		public static readonly string NAME = "70x";
		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.BarcodeReader;
		public override Type InstrumentType => typeof( Dataman70x );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public ResultType ResultType { get; set; } = new ResultType();
		private string sComPort = string.Empty;
		public string Comport
		{
			get => this.sComPort;
			set => this.Set( ref this.sComPort, value, "Comport" );
		}
		private int _BarcodeRetryLimits = 5;
		public int BarcodeRetryLimits
		{
			get => this._BarcodeRetryLimits;
			set => this.Set( ref this._BarcodeRetryLimits, value, "BarcodeRetryLimits" );
		}
	}
	public class Dataman70x
		: InstrumentBase
	{





		public override MachineVariant MachineVar { get; set; }
		private const int BaudRate = 115200;
		private DispatcherSynchronizationContext _syncContext = new DispatcherSynchronizationContext();
		private SerSystemDiscoverer _serSystemDiscoverer = null;
		private ISystemConnector _connector = null;
		private DataManSystem _system = null;
		private ResultCollector _results;
		public ObservableCollection<SerSystemDiscoverer.SystemInfo> Systeminfos = new ObservableCollection<SerSystemDiscoverer.SystemInfo>();
		private object _currentResultInfoSyncLock = new object();
		public Dataman70xConfiguration Configuration { get; private set; }
		public SerSystemDiscoverer.SystemInfo TempDevice = null;
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.BarcodeReader;


		protected override string OnCreate()
		{
			return String.Empty;
		}

		protected override string OnInitialize()
		{
			var result = String.Empty;
			if ( this.IsOpen == false )
			{
				if ( !this.Configuration.Comport.Contains( "COM" ) )
					if ( ( result = this.TDiscoverDevices().Result ) != String.Empty ) return result;
				if ( ( result = this.Open() ) != String.Empty ) return result;
			}
			return String.Empty;
		}

		protected override string OnStop()
		{
			this.Close();
			return String.Empty;
		}

		protected override string OnTerminate()
		{
			this.Close();
			return String.Empty;
		}

		public string Open()
		{
			var Result = String.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				//var device = this.Systeminfos.ToList().Find(x => x.PortName == this.Configuration.Comport);
				//if (device == null)
				//    throw new Exception("Device not found");
				SerSystemConnector conn = new SerSystemConnector( this.Configuration.Comport, BaudRate );
				this._connector = conn;
				this._system = new DataManSystem( this._connector );
				ResultTypes requested_result_types = this.Configuration.ResultType.GetResultTypes();
				this._results = new ResultCollector( this._system, requested_result_types );
				this._results.ComplexResultCompleted += this.Results_ComplexResultCompleted;
				this._results.SimpleResultDropped += this.Results_SimpleResultDropped;
				this._system.DefaultTimeout = 5000;
				this._system.SetKeepAliveOptions( true, 3000, 1000 );
				if ( this._system.State != ConnectionState.Connected )
					this._system.Connect();
				try
				{
					this._system.SetResultTypes( requested_result_types );
				}
				catch
				{ }
			}
			catch ( Exception ex )
			{
				this.CleanupConnection();
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		private void CleanupConnection()
		{
			this._connector = null;
			this._system = null;
		}

		private void Results_ComplexResultCompleted( object sender, ComplexResult e )
		{
			this.ShowResult( e );
		}

		private void Results_SimpleResultDropped( object sender, SimpleResult e )
		{
			//_syncContext.Post(
			//	delegate
			//	{
			//		ReportDroppedResult( e );
			//	},
			//	null );
		}
		public bool IsOpen => this._system == null ? false : this._system.State == ConnectionState.Connected;
		public string Close()
		{
			var Result = string.Empty;
			try
			{
				if ( null != this._system && this._system.State == ConnectionState.Connected )
					this._system.Disconnect();
				if ( this._serSystemDiscoverer == null ) return string.Empty;
				this._serSystemDiscoverer.Dispose();
				this._serSystemDiscoverer = null;
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}

		private void OnLiveImageArrived( IAsyncResult result )
		{
			try
			{
				// SAMPLE
				// do not need to resize image, set "Stretch=Fill" in GUI, image will automatically fit into Image
				Image image = this._system.EndGetLiveImage( result );
				this.Display = image;
				if ( this.EnLiveDisplay )
					this._system.BeginGetLiveImage(
						ImageFormat.jpeg,
						ImageSize.Sixteenth,
						ImageQuality.Medium,
						this.OnLiveImageArrived,
						null );
			}
			catch
			{
			}
		}

		private bool EnLiveDisplay = false;
		public void StartLiveDisplay()
		{
			//For simulation purpose
			//IAsyncResult result = null;
			//this.OnLiveImageArrived( result );

			if ( this._system == null ) return;
			this._system.SendCommand( "SET LIVEIMG.MODE 2" );
			this._system.BeginGetLiveImage(
				ImageFormat.jpeg,
				ImageSize.Sixteenth,
				ImageQuality.Medium,
				this.OnLiveImageArrived,
				null );
			this.EnLiveDisplay = true;
		}
		public void StopLiveDisplay()
		{
			if ( this._system == null ) return;
			this._system.SendCommand( "SET LIVEIMG.MODE 0" );
			this.EnLiveDisplay = false;
		}

		string s_Result = string.Empty;
		public string sResult
		{
			get => this.s_Result;
			set => this.Set( ref this.s_Result, value, "sResult" );
		}

		private Image _Display = null;
		public Image Display
		{
			get => this._Display;
			set => this.Set( ref this._Display, value, "Display" );
		}
		private string ShowResult( ComplexResult complexResult )
		{
			List<Image> images = new List<Image>();
			List<string> image_graphics = new List<string>();

			int result_id = -1;
			ResultTypes collected_results = ResultTypes.None;

			// Take a reference or copy values from the locked result info object. This is done
			// so that the lock is used only for a short period of time.
			lock ( this._currentResultInfoSyncLock )
			{
				foreach ( var simple_result in complexResult.SimpleResults )
				{
					collected_results |= simple_result.Id.Type;

					switch ( simple_result.Id.Type )
					{
						case ResultTypes.Image:
							Image image = ImageArrivedEventArgs.GetImageFromImageBytes( simple_result.Data );
							if ( image != null )
								images.Add( image );
							break;

						case ResultTypes.ImageGraphics:
							image_graphics.Add( simple_result.GetDataAsString() );
							break;

						case ResultTypes.ReadXml:
							this.sResult = this.GetReadStringFromResultXml( simple_result.GetDataAsString() );
							result_id = simple_result.Id.Id;
							break;

						case ResultTypes.ReadString:
							this.sResult = simple_result.GetDataAsString();
							result_id = simple_result.Id.Id;
							break;
					}
				}
			}
			return this.sResult;
		}
		private string GetReadStringFromResultXml( string resultXml )
		{
			try
			{
				XmlDocument doc = new XmlDocument();

				doc.LoadXml( resultXml );

				XmlNode full_string_node = doc.SelectSingleNode( "result/general/full_string" );

				if ( full_string_node != null && this._system != null && this._system.State == ConnectionState.Connected )
				{
					XmlAttribute encoding = full_string_node.Attributes[ "encoding" ];
					if ( encoding != null && encoding.InnerText == "base64" )
					{
						if ( !string.IsNullOrEmpty( full_string_node.InnerText ) )
						{
							byte[] code = Convert.FromBase64String( full_string_node.InnerText );
							return this._system.Encoding.GetString( code, 0, code.Length );
						}
						else
						{
							return "";
						}
					}

					return full_string_node.InnerText;
				}
			}
			catch
			{
			}

			return "";
		}
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as Dataman70xConfiguration;
		}
		private void OnSerSystemDiscovered( SerSystemDiscoverer.SystemInfo systemInfo )
		{
			Application.Current.Dispatcher.Invoke( ( Action )delegate // <--- HERE
			{
				if ( this.Systeminfos.ToList().Where( x => x.PortName == systemInfo.PortName ).Count() == 0 )
					this.Systeminfos.Add( systemInfo );
			} );

		}
		public string SendCommand( string CMD )
		{
			var Error = string.Empty;
			try
			{
				var Reply = this._system.SendCommand( CMD );

			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		private string SendTrigger( bool On )
		{
			var Error = string.Empty;
			try
			{
				string CMD = "TRIGGER ";
				CMD += On ? "ON" : "OFF";
				var Reply = this._system.SendCommand( CMD );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string Trigger()
		{
			var sErr = string.Empty;
			try
			{
				this.sResult = string.Empty;
				if ( ( this.sResult = this.SendTrigger( true ) ) != string.Empty ) throw new Exception( $"{( int )RunErrors.ERR_CongexBarSendTriggerErr}" );
				Thread.Sleep( 10 );
				if ( ( this.sResult = this.SendTrigger( false ) ) != string.Empty ) throw new Exception( $"{( int )RunErrors.ERR_CongexBarSendTriggerErr}" );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}
		public Task<(ErrorClass EClass, string ErrorMessage, string QRResult)> GetQRCode()
		{
			return Task.Run( () =>
			{
				ErrorClass EClass = ErrorClass.OK;
				string ErrorMessage = string.Empty;
				try
				{
					ErrorMessage = this.Trigger();
					if ( ErrorMessage != string.Empty )
						throw new Exception( ErrorMessage );

					Stopwatch sw = new Stopwatch();
					sw.Restart();
					while ( this.sResult == string.Empty && sw.ElapsedMilliseconds < 2000 )
					{
						Thread.Sleep( 10 );
					}
					sw.Reset();
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return (EClass, ErrorMessage, this.sResult);
			} );
		}

		public SerSystemDiscoverer.SystemInfo GetSerialInfoByName( string Comport )
		{
			return this.Systeminfos.ToList().Find( x => x.PortName == Comport );
		}
		public bool ComportExist()
		{
			if ( this.Systeminfos == null ) return false;
			if ( this.Systeminfos.Count == 0 ) return false;
			var found = this.Systeminfos.Where( x => x.PortName == this.Configuration.Comport );
			return found.Count() > 0;
		}

		public SerSystemDiscoverer.SystemInfo GetCurrentSystemInfo()
		{
			if ( this.Systeminfos == null ) return null;
			if ( this.Systeminfos.Count == 0 ) return null;
			return this.Systeminfos.First( x => x.PortName == this.Configuration.Comport );
		}

		public Task<string> TForceDiscoverDevices() => Task.Run( () => this.ForceDiscoverDevices() );
		private string ForceDiscoverDevices()
		{

			var sErr = string.Empty;
			try
			{
				Application.Current.Dispatcher.Invoke( ( Action )delegate // <--- HERE
				{
					this.Systeminfos.Clear();
				} );
				this.DiscoverDevices();
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}

		public Task<string> TDiscoverDevices() => Task.Run( () => this.DiscoverDevices() );
		private string DiscoverDevices()
		{
			var sErr = string.Empty;
			try
			{
				if ( this._serSystemDiscoverer == null )
				{
					this._serSystemDiscoverer = new SerSystemDiscoverer();
					this._serSystemDiscoverer.SystemDiscovered += new SerSystemDiscoverer.SystemDiscoveredHandler( this.OnSerSystemDiscovered );
				}
				if ( this.Systeminfos.Count == 0 )
					this._serSystemDiscoverer.Discover();
				Stopwatch sw = new Stopwatch();
				sw.Restart();
				while ( this._serSystemDiscoverer.IsDiscoveryInProgress )
				{
					Thread.Sleep( 100 );
					if ( sw.ElapsedMilliseconds > 30000 )
					{
						if ( this.Systeminfos.Count == 2 ) break;
						throw new Exception( $"^{( int )RunErrors.ERR_DeviceDiscoveryErr }^" );
					}
				}
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}

		public override void ApplyRecipe( RecipeBaseUtility recipeItem )
		{
			throw new NotImplementedException();
		}
	}
	public class ResultType : BaseUtility
	{
		private bool _string = true;
		private bool _image = false;
		private bool _imagegraphics = false;
		public bool EnString
		{
			get => this._string;
			set => this.Set( ref this._string, value, "EnString" );
		}
		public bool EnImage
		{
			get => this._image;
			set => this.Set( ref this._image, value, "EnImage" );
		}
		public bool EnImageGraphics
		{
			get => this._imagegraphics;
			set => this.Set( ref this._imagegraphics, value, "EnImageGraphics" );
		}

		public ResultTypes GetResultTypes()
		{
			return ( this.EnString ? ResultTypes.ReadString : ResultTypes.None ) | ( this.EnImage ? ResultTypes.Image : ResultTypes.None ) | ( this.EnImageGraphics ? ResultTypes.ImageGraphics : ResultTypes.None );
		}
	}
}

