using HiPA.Common;
using HiPA.Common.Forms;
using JptCamera;
using JptMatroxSystem;
using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;

namespace HiPA.Instrument.Camera
{
	public enum ROI
	{
		Cal,
		Inspect,
		None,
	}
	public enum CamDir
	{
		_X,//left
		_Y,//up
		X,//right
		Y,//down
	}
	public class CameraManager : BaseUtility
	{
		public static readonly ReadOnlyCollection<int> PixelStep =
new ReadOnlyCollection<int>( new[]
{
		 1, 2, 5,
		 10, 20, 50,
		 100, 200, 500,
} );
		private MatroxSystem _mtxSys;
		public MtxGigeCamera _camera;

		private byte[] _array8Loaded;
		private short[] _array16Loaded;
		private Bitmap _bitmapLoaded = null;

		private int _width;
		private int _height;
		private bool _isNewImage = false;

		Object _lockable = new object();
		Object _lockableAuto = new object();

		private static MatroxSystem _appID = MatroxSystem.Instance;
		private static MIL_ID m_system = _appID.HostSystemID;
		private MIL_ID LoadedMilImage;
		public CameraManager()
		{
			string sErr = string.Empty;
			try
			{
				this._camera = new MtxGigeCamera();
			}
			catch ( Exception ex )
			{ sErr = "Camera Manager loading error: " + ex.Message; }
			if ( sErr.Length > 0 ) JPTUtility.Logger.doLog( sErr );
		}

		public string CameraFreeResources()
		{
			JPTUtility.Logger.doLog( "FreeResources() ... " );
			string sErr = string.Empty;
			try
			{

				if ( this._camera != null )
				{
					// Check if camera in trigger mode before FreeResource for camera
					// Camera will wait for last trigger before closing
					// hence quicker to change mode to continuous before close
					if ( this._camera.IsCameraConnected )
					{
						if ( this._camera.TriggerType != TriggerTypeEnum.TRIGGER_OFF )
						{
							this._camera.TriggerType = TriggerTypeEnum.TRIGGER_OFF;
						}
					}

					this._camera.OnProcessedImageReceivedRemove( this.NewImageReceived );

					sErr = this._camera.DisconnectCamera();
				}

			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FreeResources error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		public string CameraConnect( string CameraName )
		{
			JPTUtility.Logger.doLog( "CameraConnect() CameraName... " );
			string sErr = string.Empty;
			try
			{
				if ( this._camera.MILSystem == MIL.M_NULL )
				{
					this._camera.InitialiseMtxSystem( this.MtxSys.GigeSystemID );
				}

				sErr = this._camera.ConnectCamera( CameraName );
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this.CameraStartEvent();
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraConnect error : " + ex.Message );
			}
			return sErr;
		}
		public string CameraConnect( int DeviceID )
		{
			JPTUtility.Logger.doLog( "CameraConnect() CameraName... " );
			string sErr = string.Empty;
			try
			{
				if ( this._camera.MILSystem == MIL.M_NULL )
					this._camera.InitialiseMtxSystem( this.MtxSys.GigeSystemID );

				sErr = this._camera.ConnectCamera( DeviceID );
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this.CameraStartEvent();
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraConnect error : " + ex.Message );
			}
			return sErr;
		}

		public double ImageRotation
		{
			get => this._camera.ImageRotation;
			set => this._camera.ImageRotation = value;
		}

		public string DisconnectCamera()
		{
			JPTUtility.Logger.doLog( "CameraConnect() CameraName... " );
			string sErr = string.Empty;
			try
			{
				sErr = this.CameraStopEvent();
				sErr = this._camera.FreeResources();
				sErr = this._camera.DisconnectCamera();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraConnect error : " + ex.Message );
			}
			return sErr;
		}

		public string SetCameraDisplayForm( IntPtr DisplayHandler )
		{
			JPTUtility.Logger.doLog( "SetCameraDisplayForm() ... " );
			string sErr = string.Empty;
			try
			{
				if ( DisplayHandler == IntPtr.Zero )
				{ sErr = this._camera.SetDisplayWinFormOff( DisplayHandler ); }
				else
				{ sErr = this._camera.SetDisplayWinForm( DisplayHandler ); }
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetCameraDisplayForm error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		public string SetCameraDisplayWPF( WindowsFormsHost DisplayHandler )
		{
			JPTUtility.Logger.doLog( "SetCameraDisplayWPF() ... " );
			string sErr = string.Empty;
			try
			{
				{ sErr = this._camera.SetDisplayWPF( DisplayHandler ); }
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetCameraDisplayWPF error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}


		public string CameraGrabSingle()
		{
			JPTUtility.Logger.doLog( "CameraGrabSingle() ... " );
			string sErr = string.Empty;
			try
			{
				if ( this._camera.IsInContinuousGrabMode )
				{ throw new Exception( $"^{( int )RunErrors.ERR_CamInContinuousMode }^" ); }

				// Set trigger off first before single grab
				if ( !MachineStateMng.isSimulation )
					this._camera.TriggerMode = JptCamera.CameraSettings.TriggerModeEnum.TRIGGER_OFF;
				this._camera.ResultCross.Clear();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
				sErr = this._camera.GrabSingle();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraGrabSingle error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string CameraGrabContinuos()
		{
			JPTUtility.Logger.doLog( "CameraGrabContinuos() ... " );
			string sErr = string.Empty;
			try
			{
				this._camera.ResultCross.Clear();
				if ( this._camera.IsInContinuousGrabMode ) return string.Empty;

				sErr = this._camera.StartGrab();
				if ( sErr != string.Empty )
					throw new Exception( $"^{( int )RunErrors.ERR_ContinuousGrabErr }^" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraGrabContinuos error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string CameraStopContinuos()
		{
			JPTUtility.Logger.doLog( "CameraStopContinuos() ... " );
			string sErr = string.Empty;
			try
			{
				if ( !this._camera.IsInContinuousGrabMode )
				{ throw new Exception( $"^{( int )RunErrors.ERR_CamNotInContinuousMode }^" ); }

				sErr = this._camera.StopGrab();
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraStopContinuos error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string DisplayZoom( double Scale )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this._camera.DisplayZoom( Scale );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "TrimmerVisionInterface - DisplayZoom error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		private string CameraStartEvent()
		{
			JPTUtility.Logger.doLog( "CameraStartEvent() ... " );
			string sErr = string.Empty;
			try
			{
				this._camera.OnProcessedImageReceive( this.NewImageReceived );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraStartEvent error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		private string CameraStopEvent()
		{
			JPTUtility.Logger.doLog( "CameraStopEvent() ... " );
			string sErr = string.Empty;
			try
			{
				this._camera.OnProcessedImageReceivedRemove( this.NewImageReceived );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraStopEvent error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string CameraExecuteCommand()
		{
			JPTUtility.Logger.doLog( "CameraExecuteCommand() ... " );
			string sErr = string.Empty;
			try
			{

				sErr = this._camera.ExecuteSoftwareTrigger();
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraExecuteCommand error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		private void NewImageReceived( object sender, JptCamera.Camera.NewImageEventArgs e )
		{
			try
			{
				lock ( this._lockable )
				{
					this._array8Loaded = e.bImageArray != null ? ( byte[] )e.bImageArray.Clone() : null;

					this._array16Loaded = e.ImageArray != null ? ( short[] )e.ImageArray.Clone() : null;

					//_bitmapLoaded  = e.ImageBMP != null ? (Bitmap)e.ImageBMP.Clone() : null;

					this.Width = e.Width;

					this.Height = e.Height;

					this._isNewImage = true;

					// New delegate to GUI
					NewImageEventArgs arg = new NewImageEventArgs();

					arg.bImageArray = this._array8Loaded;
					arg.ImageArray = this._array16Loaded;
					arg.Width = this.Width;
					arg.Height = this.Height;

					this.OnNewImageReceived( arg );
				}

			}
			catch
			{ JPTUtility.Logger.doLog( "NewImageReceived error" ); }
		}

		public string LoadMilImageFromFile( string Filename )
		{
			string sErr = string.Empty;
			try
			{
				if ( !File.Exists( Filename ) )
				{ throw new Exception( "File not exist: " + Filename ); }

				sErr = this._camera.Load2DImageFromFile( Filename );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "LoadImageFromFile error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		public ROIHandler Cal => this._camera.Cal;
		public ROIHandler Inspect => this._camera.Inspect;
		public ResultDisplay ResultCross => this._camera.ResultCross;
		public string LoadImageFromFileSdk( string Filename )
		{
			JPTUtility.Logger.doLog( "FXRAOI_LoadImageFromFileSdk(Filename) ... " + Filename );
			string sErr = string.Empty;
			try
			{
				if ( !File.Exists( Filename ) )
				{ throw new Exception( "File not exist: " + Filename ); }


				if ( this.LoadedMilImage != null )
				{
					MIL.MbufFree( this.LoadedMilImage );
					this.LoadedMilImage = MIL.M_NULL;
				}

				System.Drawing.Image img = System.Drawing.Image.FromFile( Filename );

				this._width = img.Width;
				this._height = img.Height;

				this._bitmapLoaded = ( Bitmap )img;

				sErr = MngrUtility.ConvertBitmapTo1DByteArray( ( Bitmap )img, ref this._array8Loaded );
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				ByteArray2MilImage( this._array8Loaded, this._width, this._height, out this.LoadedMilImage );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "LoadImageFromFileSdk error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public DPAD XHairPos { get; set; } = new DPAD();


		public bool IsShowCrossLine
		{
			get => this.GetValue( () => this.IsShowCrossLine );
			set
			{
				this._camera.ShowDisplayCross( value, MIL.M_COLOR_RED, this.XHairPos.XOffsetPix, this.XHairPos.YOffsetPix );
				this.SetValue( () => this.IsShowCrossLine, value );
			}
		}

		public string MoveLaserSpotCalToCenter()
		{
			string sErr = string.Empty;
			try
			{
				this.XHairPos.ClearOffset();
				this.IsShowCrossLine = true;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "MoveLaserSpotCalToCenter error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string MoveXHair( CamDir Direction )
		{
			string sErr = string.Empty;
			try
			{
				if ( Direction == CamDir.X )
					this.XHairPos.IncrementX();
				else if ( Direction == CamDir.Y )
					this.XHairPos.IncrementY();
				else if ( Direction == CamDir._Y )
					this.XHairPos.DecrementY();
				else if ( Direction == CamDir._X )
					this.XHairPos.DecrementX();
				this.IsShowCrossLine = true;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "MoveXHair error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		private static string ByteArray2MilImage( byte[] data, int width, int height, out MIL_ID LoadedMilImage )
		{
			string sErr = string.Empty;
			LoadedMilImage = MIL.M_NULL;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );

				var UserDataHandle = GCHandle.Alloc( data, GCHandleType.Pinned );

				MIL.MbufCreate2d( m_system, width, height, 8 + MIL.M_UNSIGNED,
				MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, MIL.M_HOST_ADDRESS + MIL.M_PITCH,
				MIL.M_DEFAULT, ( ulong )UserDataHandle.AddrOfPinnedObject(), ref LoadedMilImage );

				UserDataHandle.Free();
			}
			catch ( Exception ex )
			{
				sErr = "Converting Image to MIL Image fail :" + ex.Message;
			}
			return sErr;
		}
		public Task<(ErrorClass EClass, string ErrorMessage, byte[,] ImageArray2D)> Get2DArray()
		{
			return Task.Run( () =>
			{
				ErrorClass EClass = ErrorClass.OK;
				var ErrorMessage = string.Empty;
				byte[,] ImageArray2D = new byte[,] { };

				try
				{
					if ( m_system == MIL.M_NULL ) throw new Exception( $"^{( int )RunErrors.ERR_CamNotInitializedErr }^" );
					ErrorMessage = this._camera.Get2DArrayFromMIL( ref ImageArray2D );
					if ( ErrorMessage != string.Empty )
					{
						EClass = ErrorClass.E4;
						throw new Exception( ErrorMessage );
					}
				}
				catch ( Exception ex )
				{
					if ( EClass != ErrorClass.OK )
						EClass = ErrorClass.E6;

					ErrorMessage = this.FormatErrMsg( "CameraManager", ex );
				}
				return (EClass, ErrorMessage, ImageArray2D);
			} );
		}
		public string SaveIMG( string path, MatroxDigitizer.FileFormat FileFormat )
		{
			var ErrorMessage = string.Empty;
			try
			{
				this._camera.SaveImageToFile( path, FileFormat );
			}
			catch ( Exception ex )
			{
				ErrorMessage = this.FormatErrMsg( "SaveIMG", ex );
			}
			return ErrorMessage;
		}

		public bool IsCameraConnected
		{
			get { return this._camera.IsCameraConnected; }
		}

		public int Width
		{
			get { return this._width; }
			set { this._width = value; }
		}

		public int Height
		{
			get { return this._height; }
			set { this._height = value; }
		}

		public MIL_ID MilImage
		{
			get => this._camera.MILImage;
		}

		public Bitmap ImageBitmap
		{
			get { return this._bitmapLoaded; }
			set { this._bitmapLoaded = value; }
		}

		public byte[] ImageArray8
		{
			get { return this._array8Loaded; }
			set { this._array8Loaded = value; }
		}

		public short[] ImageArray16
		{
			get { return this._array16Loaded; }
			set { this._array16Loaded = value; }
		}

		public bool IsNewImageReceived
		{
			get { return this._isNewImage; }
			set { this._isNewImage = value; }
		}

		public MatroxSystem MtxSys
		{
			get
			{
				if ( this._mtxSys == null )
				{ this._mtxSys = MatroxSystem.Instance; }

				return this._mtxSys;
			}
		}

		#region Camera Settings


		/// <summary>
		/// String returns of the camera serial number
		/// </summary>
		public string CameraSerialName { get { return this._camera.CameraSerialName; } }

		/// <summary>
		/// String returns of camera manufacturer name/brand
		/// </summary>
		public string CameraBrand { get { return this._camera.CameraBrand; } }

		/// <summary>
		/// String returns of camera model name
		/// </summary>
		public string CameraModel { get { return this._camera.CameraModel; } }

		/// <summary>
		/// String returns of camera user-defined name/ID
		/// </summary>
		public string CameraUserID { get { return this._camera.CameraUserID; } }

		/// <summary>
		/// GET/SET of camera pixel format 
		/// </summary>
		public string PixelFormat
		{
			get
			{
				return this._camera.PixelFormat.ToString();
			}
			set
			{
				var data = ( PixelFormatEnum )Enum.Parse( typeof( PixelFormatEnum ), value, true );
				this._camera.PixelFormat = data;
			}
		}

		/// <summary>
		/// Return list of strings of camera pixel format options.
		/// </summary>
		public IEnumerable<string> GetPixelFormatEnum
		{
			get
			{
				return Enum.GetNames( typeof( PixelFormatEnum ) );
			}
		}

		/// <summary>
		/// GET INT return of camera offset X value.
		/// SET camera image offset X value in pixel.
		/// </summary>
		public int OffsetX
		{
			get
			{
				return this._camera.OffsetX;
			}
			set
			{
				this._camera.OffsetX = value;
			}
		}

		/// <summary>
		/// GET INT returns of the camera image offset Y.
		/// SET camera image offset Y value in pixel
		/// </summary>
		public int OffsetY
		{
			get
			{
				return this._camera.OffsetY;
			}
			set
			{
				this._camera.OffsetY = value;
			}
		}

		/// <summary>
		/// GET|SET Bool returns of camera image in reverse X orientation
		/// </summary>
		public bool ReverseX
		{
			get
			{
				return this._camera.ReverseX;
			}
			set
			{
				this._camera.ReverseX = value;
			}
		}

		/// <summary>
		/// GET|SET Bool returns of camera image in reverse Y orientation
		/// </summary>
		public bool ReverseY
		{
			get
			{
				return this._camera.ReverseY;
			}
			set
			{
				this._camera.ReverseY = value;
			}
		}

		/// <summary>
		/// GET|SET double returns of the camera exposure time in us
		/// </summary>
		public double Exposure
		{
			get
			{
				return this._camera.Exposure;
			}
			set
			{
				this._camera.Exposure = value;
			}
		}
		public double MinExposure
		{
			get
			{
				return this._camera.ExposureMinimum;
			}
		}
		public double MaxExposure
		{
			get
			{
				return this._camera.ExposureMaximum;
			}
		}
		public int WidthMaximum
		{
			get
			{
				return this._camera.WidthMaximum;
			}
		}

		public int HeightMaximum
		{
			get
			{
				return this._camera.HeightMaximum;
			}
		}

		/// <summary>
		/// GET|SET int returns of camera gain value
		/// </summary>
		public double Gain
		{
			get
			{
				return this._camera.Gain;
			}
			set
			{
				this._camera.Gain = value;
			}
		}
		public double MinGain
		{
			get
			{
				return this._camera.GainMinimum;
			}
		}
		public double MaxGain
		{
			get
			{
				return this._camera.GainMaximum;
			}
		}
		/// <summary>
		/// GET|SET Bool returns of the camera to enable Gamma settings
		/// </summary>
		public bool GammaEnable
		{
			get
			{
				return this._camera.GammaEnable;
			}
			set
			{
				this._camera.GammaEnable = value;
			}
		}

		/// <summary>
		/// GET|SET double returns of camera gamma value
		/// </summary>
		public double Gamma
		{
			get
			{
				return this._camera.Gamma;
			}
			set
			{
				this._camera.Gamma = value;
			}
		}

		/// <summary>
		/// GET|SET double returns of camera gamma value
		/// </summary>
		public double GammaMin
		{
			get
			{
				return this._camera.GammaMinimum;
			}
		}
		/// <summary>
		/// GET|SET double returns of camera gamma value
		/// </summary>
		public double GammaMax
		{
			get
			{
				return this._camera.GammaMaximum;
			}
		}
		/// <summary>
		/// GET|SET double returns of camera trigger delay value in us(microsecond)
		/// </summary>
		public double TriggerDelay
		{
			get
			{
				return this._camera.TriggerDelay;
			}
			set
			{
				this._camera.TriggerDelay = Convert.ToInt32( value );
			}
		}

		public ushort Binning
		{
			get { return this._camera.Binning; }
			set { this._camera.Binning = value; }
		}

		/// <summary>
		/// Get the current camera trigger mode
		/// </summary>
		/// <param name="RefTriggerMode">String: current trigger mode</param>
		/// <returns></returns>
		public string GetTriggerMode( ref string RefTriggerMode )
		{
			string sErr = string.Empty;
			try
			{
				if ( !this._camera.IsCameraConnected )
					return "No camera connected";

				var trigMode = this._camera.TriggerType;

				switch ( trigMode )
				{
					case TriggerTypeEnum.TRIGGER_HW_RISING:
					case TriggerTypeEnum.TRIGGER_HW_FALLING:
						RefTriggerMode = "Hardware";
						break;
					case TriggerTypeEnum.TRIGGER_SOFTWARE:
						RefTriggerMode = "Software";
						break;
					case TriggerTypeEnum.TRIGGER_CONTINUOUS:
					default:
						RefTriggerMode = "Continuous";
						break;
				}

			}
			catch ( Exception ex )
			{
				sErr = string.Format( "GetTriggerMode error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Set camera trigger mode
		/// </summary>
		/// <param name="sTriggerType">String: Trigger mode</param>
		/// <returns></returns>
		public string SetTriggerMode( string sTriggerType )
		{
			string sErr = string.Empty;
			try
			{
				if ( !this._camera.IsCameraConnected )
				{
					throw new Exception( $"^{( int )RunErrors.ERR_NoCamDetectedErr }^" );
				}

				TriggerTypeEnum trigModeSelected = TriggerTypeEnum.TRIGGER_OFF;

				if ( sTriggerType.Contains( "Continuous" ) )
				{
					trigModeSelected = TriggerTypeEnum.TRIGGER_CONTINUOUS;
				}
				else if ( sTriggerType.Contains( "Hardware" ) )
				{
					trigModeSelected = TriggerTypeEnum.TRIGGER_HW_RISING;
				}
				else if ( sTriggerType.Contains( "Software" ) )
				{
					trigModeSelected = TriggerTypeEnum.TRIGGER_SOFTWARE;
				}

				sErr = this.SetTriggerModeMil( trigModeSelected );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetTriggerMode error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Set camera trigger mode using Matrox Gige system
		/// </summary>
		/// <param name="NewTriggerType">Matrox Trigger Mode</param>
		/// <returns></returns>
		public string SetTriggerModeMil( TriggerTypeEnum NewTriggerType )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.TriggerType = NewTriggerType;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetTriggerModeMil error: " + ex.Message );
			}
			return sErr;
		}


		#endregion Camera Settings

		#region Callback functions

		/// <summary>
		/// Event arguments class for new image received event
		/// </summary>
		public class NewImageEventArgs : EventArgs
		{
			public byte[] bImageArray { get; set; } = null;
			public short[] ImageArray { get; set; } = null;
			public int Width { get; set; } = 0;
			public int Height { get; set; } = 0;
			public Bitmap ImageBMP { get; set; } = null;
		}

		NewImageEventArgs imgEventArgs = new NewImageEventArgs();

		public delegate void OnNewImageProcessedHandler( object sender, NewImageEventArgs e );

		/// <summary>
		/// Define event for new image received and processed 
		/// </summary>
		public event OnNewImageProcessedHandler OnProcessedImageReceived;

		protected virtual void OnNewImageReceived( NewImageEventArgs e )
		{
			if ( OnProcessedImageReceived != null ) OnProcessedImageReceived( this, e );
		}

		private void OnNewImageProcessed( NewImageEventArgs e )
		{
			this.OnNewImageReceived( e );
		}

		/// <summary>
		/// To add function to listen to new image received event
		/// </summary>
		/// <param name="addEvent"></param>
		public void OnProcessedImageReceive( EventHandler<NewImageEventArgs> addEvent )
		{
			OnProcessedImageReceived += new OnNewImageProcessedHandler( addEvent );

			JPTUtility.Logger.doLog( "OnProcessedImageReceive event added" );
		}

		/// <summary>
		/// To remove function from listen to new image received event
		/// </summary>
		/// <param name="removeEvent"></param>
		public void OnProcessedImageReceivedRemove( EventHandler<NewImageEventArgs> removeEvent )
		{
			OnProcessedImageReceived -= new OnNewImageProcessedHandler( removeEvent );

			JPTUtility.Logger.doLog( "OnProcessedImageReceivedRemove event removed" );
		}

		#endregion Callback functions
	}
}
