using Matrox.MatroxImagingLibrary;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms.Integration;
using static JptCamera.MatroxDigitizer;

namespace JptCamera
{
	public class MtxGigeBase : Camera
	{
		//public enum ColorMode
		//{
		//    Undefined,
		//    Mono8,
		//    Mono10,
		//    Mono12,
		//    RGB8Packed,
		//}

		//public enum TriggerMode
		//{
		//    TRIGGER_OFF = 0,
		//    TRIGGER_CONTINUOUS = 1,
		//    TRIGGER_HI_LO = 2,
		//    TRIGGER_LO_HI = 4,
		//    TRIGGER_SOFTWARE = 8,
		//}

		#region Private members

		private MIL_ID _gigeSys = MIL.M_NULL;
		private MIL_ID _gigeBuffer = MIL.M_NULL;
		private MIL_ID _gigeCamera = MIL.M_NULL;
		private MIL_ID _gigeDisp = MIL.M_NULL;

		protected JptCamera.MatroxDigitizer _mtxCamera;
		//private JptMtxCamera.CameraSettings _settings;

		// Default image dimensions.
		private const int DEFAULT_IMAGE_SIZE_X = 640;
		private const int DEFAULT_IMAGE_SIZE_Y = 480;
		private const int DEFAULT_IMAGE_SIZE_BAND = 1;

		private bool _isInContinuousMode = false;
		//private bool _isWpfDisplay = false;

		NewImageEventArgs imgEventArgs = new NewImageEventArgs();

		Object _lockable = new object();
		Object _lockableAuto = new object();

		#endregion

		public MtxGigeBase()
		{
			this._mtxCamera = new JptCamera.MatroxDigitizer();
		}

		public string InitialiseMtxSystem( MIL_ID MtxSystemID )
		{
			return this.InitialiseMatrox( MtxSystemID );
		}

		public string InitialiseMatrox( MIL_ID MILSystem )
		{
			JPTUtility.Logger.doLog( "Initialise ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.InitialiseSystem( MILSystem );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Initialise error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public void EnableLogging( bool IsEnabled )
		{
			try
			{
				JPTUtility.Logger.doLog( "EnableLogging ..." );
				JPTUtility.Logger.IsDoLog = IsEnabled;
			}
			catch { }
		}

		public MIL_ID MILSystem
		{
			get
			{
				try { return this._mtxCamera.MILSystem; }
				catch { return MIL.M_NULL; }
			}
		}

		public MIL_ID MILImage
		{
			get
			{
				try { return this._mtxCamera.MILImage; }
				catch { return MIL.M_NULL; }
			}
		}

		public MIL_ID MILCamera
		{
			get
			{
				try { return this._mtxCamera.MILDigitizer; }
				catch { return MIL.M_NULL; }
			}
		}

		public string SaveImageToFile( string Filename, MatroxDigitizer.FileFormat fileFormat )
		{
			string sErr = string.Empty;
			try
			{
				var ext = string.Empty;
				if ( fileFormat == FileFormat.BMP )
					ext = ".bmp";
				else if ( fileFormat == FileFormat.PNG )
					ext = ".png";
				else if ( fileFormat == FileFormat.TIFF )
					ext = ".tiff";

				var Dir = Path.GetDirectoryName( Filename );
				if ( !Directory.Exists( Dir ) )
					Directory.CreateDirectory( Dir );
				sErr = this._mtxCamera.Save( Filename + ext, fileFormat );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SaveImageToFile error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string GetImageAsBitmap( ref Bitmap ResultBMP )
		{
			string sErr = string.Empty;
			try
			{
				ResultBMP = ( Bitmap )this._mtxCamera.CreateBitmapFromMILBuffer( this._mtxCamera.MILImage );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "GetImageAsBitmap error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string Get1DArrayFromMIL( ref byte[] ResultArr )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.Get1DArray( ref ResultArr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Get1DArrayFromMIL error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string Get2DArrayFromMIL( ref byte[,] ResultArr )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.Get2DArray( ref ResultArr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Get2DArrayFromMIL error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string Load2DImageFromFile( string Filename )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.Load2DImageFromFile( Filename );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Load2DImageFromFile error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		public ROIHandler Cal => this._mtxCamera.Cal;
		public ROIHandler Inspect => this._mtxCamera.Inspect;
		public ResultDisplay ResultCross => this._mtxCamera.ResultCross;
		public string FreeResources()
		{
			JPTUtility.Logger.doLog( "FreeResources ..." );
			string sErr = string.Empty;
			try
			{
				if ( this._mtxCamera != null )
				{
					this._mtxCamera.FreeResources();
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FreeResources error" + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public override string DisconnectCamera()
		{
			JPTUtility.Logger.doLog( "RemoveCamera ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.DisconnectCamera();
				this._isInContinuousMode = false;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "RemoveCamera error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
				return sErr;
			}
			return sErr;
		}

		public override string ConnectCamera( string Camera_Name )
		{
			JPTUtility.Logger.doLog( "ConnectCamera CameraNm..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.AllocateCamera( Camera_Name );
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this._mtxCamera.AllocateBuffer();
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this.CameraStartEvent();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ConnectCamera error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		public override string ConnectCamera( int DeviceNumber )
		{
			JPTUtility.Logger.doLog( "ConnectCamera CameraNm..." );
			string sErr = string.Empty;
			string sCamErr = string.Empty;
			try
			{
				bool isConnected = false;
				sErr = this._mtxCamera.IsCameraConnected( ref isConnected );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
				if ( isConnected )
					return string.Empty;
				sErr = this._mtxCamera.AllocateCamera( DeviceNumber );
				if ( sErr != string.Empty ) sCamErr = sErr;
				//if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this._mtxCamera.AllocateBuffer();
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this.CameraStartEvent();
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				if ( sCamErr.Length > 0 ) throw new Exception( sCamErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ConnectCamera error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		public override string ConnectCamera( string Camera_Name, string dcf_file )
		{
			JPTUtility.Logger.doLog( "ConnectCamera CameraNm..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.AllocateCamera( Camera_Name, dcf_file );
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this._mtxCamera.AllocateBuffer();
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this.CameraStartEvent();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ConnectCamera error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public double ImageRotation
		{
			get => this._mtxCamera.ImageRotateAngle;
			set => this._mtxCamera.ImageRotateAngle = value;
		}

		public override string ConnectCamera()
		{
			JPTUtility.Logger.doLog( "ConnectCamera ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.AllocateCamera();
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this._mtxCamera.AllocateBuffer();
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				sErr = this.CameraStartEvent();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ConnectCamera error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string CameraStartEvent()
		{
			string sErr = string.Empty;
			try
			{
				this._mtxCamera.OnImageReceived += new MatroxDigitizer.OnNewImageHandler( this.NewImageReceived );

			}
			catch ( Exception ex )
			{
				sErr = string.Format( "MtxGigeBase CameraStartEvent error: " + ex.Message );
			}
			return string.Empty;
		}

		private void NewImageReceived( object sender, MatroxDigitizer.NewDataEventArg e )
		{
			try
			{
				lock ( this._lockable )
				{
					this.imgEventArgs.bImageArray = e.bMatrixArray != null ? ( byte[] )e.bMatrixArray.Clone() : null;

					this.imgEventArgs.ImageArray = e.MatrixArray != null ? ( short[] )e.MatrixArray.Clone() : null;

					this.imgEventArgs.ImageBMP = e.Image != null ? ( Bitmap )e.Image.Clone() : null;

					this.imgEventArgs.Width = e.Width;

					this.imgEventArgs.Height = e.Height;

					// Delegate
					this.OnNewImageReceived( this.imgEventArgs );
				}

			}
			catch
			{ JPTUtility.Logger.doLog( "MtxGigeBase NewImageReceived error" ); }
		}

		public string SetDisplayOption( IntPtr DisplayHandle )
		{
			JPTUtility.Logger.doLog( "SetDisplayOption ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.SetDisplayWinHnd( DisplayHandle );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetDisplayOption error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public override string SetDisplayWinForm( IntPtr displayHost )
		{
			JPTUtility.Logger.doLog( "SetDisplayWinForm ..." );
			string sErr = string.Empty;
			try
			{
				if ( this.Settings.M_PixelFormat == "Mono12" )
				{
					sErr = this._mtxCamera.DisplayBitShift( 4 );
				}

				sErr = this._mtxCamera.SetDisplayWinHnd( displayHost );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetDisplayWinForm error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string SetDisplayWPF( WindowsFormsHost displayHost )
		{
			JPTUtility.Logger.doLog( "SetDisplayWPF ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.SetDisplayWPFHost( displayHost );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetDisplayWPF error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string SetDisplayWinFormOff( IntPtr displayHost )
		{
			JPTUtility.Logger.doLog( "SetDisplayWinForm ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.SetDisplayWinHndOff( displayHost );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetDisplayWinForm error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string SetDisplayWPFOFF()
		{
			JPTUtility.Logger.doLog( "SetDisplayWPFOFF ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.SetDisplayWPFOff();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetDisplayWPFOFF error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string SetDisplayBitShift( int BitShift )
		{
			JPTUtility.Logger.doLog( "SetDisplayBitShift ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.DisplayBitShift( BitShift );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetDisplayBitShift error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		public string DisplayZoom( double Scale )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.DisplayZoom( Scale );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "TrimmerVisionInterface - DisplayZoom error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string ShowDisplayCross( bool Enabled, int Color, int? XPos = null, int? YPos = null )
		{
			JPTUtility.Logger.doLog( "ShowDisplayCross ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.XHairDraw( Enabled, Color, XPos, YPos );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ShowDisplayCross error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string ShowDisplayRedCrossByCoordinate( int X, int Y )
		{
			JPTUtility.Logger.doLog( "ShowDisplayRedCrossByCoordinate ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.ShowBlueCrossCoordinate( X, Y );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ShowDisplayRedCrossByCoordinate error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public override string StartGrab()
		{
			JPTUtility.Logger.doLog( "Start ..." );
			string sErr = string.Empty;
			try
			{
				if ( this._mtxCamera.IsCameraColor )
				{
					sErr = this.GrabContinuous();
				}
				else
				{
					sErr = this._mtxCamera.StartGrabProcess();
				}
				if ( sErr.Length > 0 ) throw new Exception( sErr );
				this._isInContinuousMode = this._mtxCamera.isContinousMode;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Start error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public override string GrabSingle()
		{
			JPTUtility.Logger.doLog( "GrabSingle ..." );
			string sErr = string.Empty;
			try
			{
				this._isInContinuousMode = this._mtxCamera.isContinousMode;
				if ( this._isInContinuousMode )
					throw new Exception( "Camera is in continuous mode" );

				sErr = this._mtxCamera.GrabSingle();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "GrabSingle error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string ExecuteSoftwareTrigger()
		{
			JPTUtility.Logger.doLog( "ExecuteSoftwareTrigger ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.ExecuteSoftwareTrigger();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ExecuteSoftwareTrigger error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public override string StopGrab()
		{
			JPTUtility.Logger.doLog( "Stop ..." );
			string sErr = string.Empty;
			try
			{
				if ( this._mtxCamera.IsCameraColor )
				{
					sErr = this.HaltContinuous();
				}
				else
				{
					sErr = this._mtxCamera.StopGrabProcess();
				}
				if ( sErr.Length > 0 ) throw new Exception( sErr );
				this._isInContinuousMode = this._mtxCamera.isContinousMode;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Stop error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string GrabContinuous()
		{
			JPTUtility.Logger.doLog( "GrabContinuous ..." );
			string sErr = string.Empty;
			try
			{
				this._isInContinuousMode = this._mtxCamera.isContinousMode;
				if ( this._isInContinuousMode )
					throw new Exception( "Camera is in continuous mode" );

				sErr = this._mtxCamera.GrabContinuous();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
				this._isInContinuousMode = this._mtxCamera.isContinousMode;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "GrabContinuous error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string HaltContinuous()
		{
			JPTUtility.Logger.doLog( "HaltContinuous ..." );
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.HaltContinuous();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
				this._isInContinuousMode = this._mtxCamera.isContinousMode;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "HaltContinuous error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string ResetCamera()
		{
			JPTUtility.Logger.doLog( "ResetCamera ..." );
			string sErr = string.Empty;
			try
			{
				if ( this._mtxCamera != null )
				{
					bool connected = false;
					sErr = this._mtxCamera.IsCameraConnected( ref connected );
					if ( sErr.Length > 0 ) return sErr;

					if ( connected )
					{
						this.DisconnectCamera();
					}
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ResetCamera error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string AllocateNewBufferResource()
		{
			string sErr = string.Empty;
			try
			{
				sErr = this._mtxCamera.AllocateBuffer();
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "AllocateNewBufferResource error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public override CameraSettings Settings
		{
			get { return this._mtxCamera.Settings; }
		}

		public override bool IsCameraConnected
		{
			get
			{
				try
				{
					bool result = false;
					if ( this._mtxCamera == null ) return result;
					string sErr = this._mtxCamera.IsCameraConnected( ref result );
					if ( sErr.Length > 0 ) return false;
					return result;
				}
				catch ( Exception ex )
				{
					JPTUtility.Logger.doLog( "IsInstanceAvailable error " + ex.Message );
					return false;
				}
			}
		}

		public bool IsCameraInitialised
		{
			get
			{
				try
				{
					bool result = false;
					if ( this._mtxCamera != null ) result = true;
					return result;
				}
				catch ( Exception ex )
				{
					JPTUtility.Logger.doLog( "IsCameraInitialised error " + ex.Message );
					return false;
				}
			}
		}

		public bool IsInContinuousGrabMode
		{
			get { return this._isInContinuousMode; }
		}

		public bool IsCameraInColorMode
		{
			get { return this._mtxCamera.IsCameraColor; }
		}


		public CameraSettings.TriggerModeEnum TriggerModeSupported
		{
			get { return ( CameraSettings.TriggerModeEnum )( 0x0F ); }
		}

		//public void OnImageReceive(EventHandler<JptCamera.MatroxDigitizer.NewDataEventArg> addEvent)
		//{
		//    _mtxCamera.OnImageReceived += new JptCamera.MatroxDigitizer.OnNewImageHandler(addEvent);

		//    JPTUtility.Logger.doLog("OnImageReceive event added");
		//}

		//public void OnImageReceivedRemove(EventHandler<JptCamera.MatroxDigitizer.NewDataEventArg> removeEvent)
		//{
		//    _mtxCamera.OnImageReceived -= new JptCamera.MatroxDigitizer.OnNewImageHandler(removeEvent);

		//    JPTUtility.Logger.doLog("OnImageReceive event removed");
		//}


		public override string CameraBrand => throw new NotImplementedException();

		public override string CameraModel => throw new NotImplementedException();

		public override string CameraUserID => throw new NotImplementedException();

		public override string CameraSerialName => throw new NotImplementedException();

		public override ushort Binning { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override PixelFormatEnum PixelFormat { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override double Exposure { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override double ExposureMinimum => throw new NotImplementedException();

		public override double ExposureMaximum => throw new NotImplementedException();

		public override double Gain { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override double GainMinimum => throw new NotImplementedException();

		public override double GainMaximum => throw new NotImplementedException();

		public override bool GammaEnable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override double Gamma { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override double GammaMinimum => throw new NotImplementedException();

		public override double GammaMaximum => throw new NotImplementedException();

		public override TriggerTypeEnum TriggerType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override int TriggerDelay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override int TriggerDelayMin => throw new NotImplementedException();

		public override int TriggerDelayMax => throw new NotImplementedException();

		public override int Width { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override int WidthMinimum => throw new NotImplementedException();

		public override int WidthMaximum => throw new NotImplementedException();

		public override int Height { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override int HeightMinimum => throw new NotImplementedException();

		public override int HeightMaximum => throw new NotImplementedException();

		public override int OffsetX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override int OffsetY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override bool ReverseX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public override bool ReverseY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	}


}
