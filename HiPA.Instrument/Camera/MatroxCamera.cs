using B262_Vision_Processing;
using HiPA.Common;
using HiPA.Common.Forms;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using static JptCamera.MatroxDigitizer;

namespace HiPA.Instrument.Camera
{
	public enum ImageRotate
	{
		Deg000 = 0,
		Deg090 = 90,
		Deg180 = 180,
		Deg270 = 270,
	}

	[Serializable]
	public enum CameraList
	{
		FOV1,
		FOV2,
	}
	[Serializable]
	public enum CameraType
	{
		GigE,
		Rapixo,
	}

	[Serializable]
	public class MatroxCameraConfiguration
		: Configuration
	{
		private string s_Name = string.Empty;
		public override string Name
		{
			get => this.s_Name;
			set
			{
				this.s_Name = value;
				this.OnPropertyChanged( "Name" );
			}
		}
		public override InstrumentCategory Category => InstrumentCategory.Camera;
		public override Type InstrumentType => typeof( MatroxCamera );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public int DeviceId
		{
			get => this.GetValue( () => this.DeviceId );
			set => this.SetValue( () => this.DeviceId, value );
		}
		public ImageRotate ImageAngle
		{
			get => this.GetValue( () => this.ImageAngle );
			set => this.SetValue( () => this.ImageAngle, value );
		}

		public ImageSave ImageSave { get; set; } = new ImageSave();
		public Setting Exposure { get; set; } = new Setting();
		public Setting Gain { get; set; } = new Setting();
		public Setting Gamma { get; set; } = new Setting();
		private bool b_ReverseX = false;
		public bool ReverseX
		{
			get => this.b_ReverseX;
			set => this.Set( ref this.b_ReverseX, value, "ReverseX" );
		}

		private bool b_ReverseY = false;
		public bool ReverseY
		{
			get => this.b_ReverseY;
			set => this.Set( ref this.b_ReverseY, value, "ReverseY" );
		}
		public DPAD XHairPos { get; set; } = new DPAD();
		public MMFCollection MMFs { get; set; } = new MMFCollection();
		public ROIRectangle CalROI { get; set; } = new ROIRectangle();
		public ROIRectangle InspectROI { get; set; } = new ROIRectangle();
		public CalibrationData ScalePixperMM { get; set; } = new CalibrationData();
	}

	[Serializable]
	public class Resolution : BaseUtility
	{
		private int i_MaxHeight = 1200;
		public int MaxHeight
		{
			get => this.i_MaxHeight;
			set => this.Set( ref this.i_MaxHeight, value, "MaxHeight" );
		}

		private int i_MaxWidth = 1920;
		public int MaxWidth
		{
			get => this.i_MaxWidth;
			set => this.Set( ref this.i_MaxWidth, value, "MaxWidth" );
		}
	}
	[Serializable]
	public class Setting : BaseUtility
	{
		protected double d_Value = 1;
		public double Value
		{
			get => this.d_Value;
			set => this.Set( ref this.d_Value, value, "Value" );
		}
		protected double d_Min = 1;
		public double Min
		{
			get => this.d_Min;
			set => this.Set( ref this.d_Min, value, "Min" );
		}
		protected double d_Max = 1;
		public double Max
		{
			get => this.d_Max;
			set => this.Set( ref this.d_Max, value, "Max" );
		}
	}
	[Serializable]
	public enum TriggerMode
	{
		Continuous,
		Hardware,
		Software,
	}
	public class MatroxCamera
		: InstrumentBase
	{

		public override MachineVariant MachineVar { get; set; }
		#region Instrument Members 
		public MatroxCameraConfiguration Configuration { get; protected set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Camera;

		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as MatroxCameraConfiguration;
		}
		public CameraManager Camera = null;
		public MatroxProcessing Vision { get; set; }
		public string SystemName => this.Name + ",system";
		public string DigitizerName => this.Name + ",digitizer";
		public string BufferName => this.Name + ",buffer";
		#endregion
		#region Camera Initialize/Terminate 
		protected override string OnCreate()
		{
			var result = string.Empty;
			try
			{
				this.ZoomScale = 1;
				this.Camera = new CameraManager();
				this.Camera.XHairPos = this.Configuration.XHairPos;
				this.Configuration.XHairPos.ScalePixperMM = this.Configuration.ScalePixperMM.ScalePixperMM;
				this.InspectionResult.PixPerMM = this.Configuration.ScalePixperMM.ScalePixperMM;
				this.Vision = Constructor.GetInstance().GetInstrument( this.Name + MatroxProcessingConfiguration.NAME, typeof( MatroxProcessingConfiguration ), new MachineVar( true ) ) as MatroxProcessing;
				if ( this.Vision == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to get instance:,{this.Name + MatroxProcessingConfiguration.NAME}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.Vision.Owner = this;

				var tasks = new Task<string>[]
				{
					this.Vision.Create(),
				};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( result = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to create Obj:,{result}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnInitialize()
		{
			var Result = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
				var tasks = new Task<string>[]
{
					this.Vision.Initialize(),
};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( Result = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to Initialize Obj:,{Result}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				}

				if ( ( Result = this.CreateSystemDigitizer() ) != string.Empty )
				{
					if ( !MachineStateMng.isSimulation )
						throw new Exception( Result );
				}
				this.GetConfiguration();
				this.ApplyConfiguration();
				this.Camera.Inspect.SetPosition( this.Configuration.InspectROI );
				this.Camera.Cal.SetPosition( this.Configuration.CalROI );
			}
			catch ( Exception ex )
			{
				//this.CloseCamera();
				Result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				//				if ( this.MtxSys != null && this._displayViewer != null && !Error ) this.CreateDisplay( null );
				Monitor.Exit( this.SyncRoot );
			}

			return Result;
		}
		protected override string OnTerminate()
		{
			var Result = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
				var tasks = new Task<string>[]
{
					this.Vision.Terminate(),
};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( Result = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to Terminate Obj:,{Result}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				}
				this.CloseCamera();
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				//				if ( this.MtxSys != null && this._displayViewer != null && !Error ) this.CreateDisplay( null );
				Monitor.Exit( this.SyncRoot );
			}

			return Result;
		}
		protected override string OnStop()
		{
			var Result = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
				var tasks = new Task<string>[]
{
					this.Vision.Stop(),
};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( Result = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to Stop Obj:,{Result}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				}
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				//				if ( this.MtxSys != null && this._displayViewer != null && !Error ) this.CreateDisplay( null );
				Monitor.Exit( this.SyncRoot );
			}

			return Result;
		}
		#endregion
		public Resolution Resolution { get; set; } = new Resolution();
		public double ZoomScale
		{
			get => this.GetValue( () => this.ZoomScale );
			set => this.SetValue( () => this.ZoomScale, value );
		}
		public InspectionResult InspectionResult { get; set; } = new InspectionResult();

		protected string CloseCamera()
		{
			var Result = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( ( Result = this.Camera.DisconnectCamera() ) != string.Empty ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return Result;
		}
		protected string CreateSystemDigitizer()
		{
			var Result = "";
			try
			{
				if ( ( Result = this.Camera.CameraConnect( this.Configuration.DeviceId ) ) != string.Empty ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		protected void GetConfiguration()
		{
			this.Configuration.Exposure.Min = 1000;
			this.Configuration.Exposure.Max = 100000;
			this.Configuration.Gain.Min = MachineStateMng.isSimulation ? 1000 : this.Camera.MinGain;
			this.Configuration.Gain.Max = 5;
			this.Configuration.Gamma.Min = MachineStateMng.isSimulation ? 0.5 : this.Camera.GammaMin;
			this.Configuration.Gamma.Max = MachineStateMng.isSimulation ? 5 : this.Camera.GammaMax;
			this.Resolution.MaxWidth = MachineStateMng.isSimulation ? 640 : this.Camera.WidthMaximum;
			this.Resolution.MaxHeight = MachineStateMng.isSimulation ? 480 : this.Camera.HeightMaximum;
		}
		protected void ApplyConfiguration()
		{
			if ( MachineStateMng.isSimulation ) return;
			this.UpdateImageRotation();
			//this.Camera.GammaEnable = true;
			this.SetExposureTime( this.Configuration.Exposure.Value );
			this.SetGain( this.Configuration.Gain.Value );
			//this.SetGain( 1 );
		}
		public string UpdateImageRotation()
		{
			var Result = string.Empty;
			try
			{
				this.Camera.ImageRotation = ( double )this.Configuration.ImageAngle;
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public bool SetReverseX( bool reverse )
		{
			try
			{
				if ( this.Name == "Station1Cam" || this.Name == "Station2Cam" ) return true;
				this.Camera.ReverseX = reverse;
				this.Configuration.ReverseX = reverse;
			}
			catch
			{
				//new Thread( () => MessageBox.Show( $"MatroxCamera.cs :ReverseX: {ex.Message}" ) ).Start();
				return false;
			}
			return true;
		}
		public bool SetReverseY( bool reverse )
		{
			try
			{
				if ( this.Name == "Station1Cam" || this.Name == "Station2Cam" ) return true;
				this.Camera.ReverseY = reverse;
				this.Configuration.ReverseY = reverse;
			}
			catch
			{
				//new Thread( () => MessageBox.Show( $"MatroxCamera.cs :ReverseY: {ex.Message}" ) ).Start();
				return false;
			}
			return true;
		}
		public Task<string> TSetExposureTime( double exposureTime ) => Task.Run( () => this.SetExposureTime( exposureTime ) );
		public string SetExposureTime( double exposureTime )
		{
			var sErr = string.Empty;
			try
			{
				if ( !this.ValidVariant() ) return sErr;
				var Exp = exposureTime;
				if ( Exp < this.Configuration.Exposure.Min )
					Exp = this.Configuration.Exposure.Min;
				if ( Exp > this.Configuration.Exposure.Max )
					Exp = this.Configuration.Exposure.Max;
				this.Camera.Exposure = Exp;
				this.Configuration.Exposure.Value = Exp;
			}
			catch ( Exception ex )
			{
				//new Thread( () => MessageBox.Show( $"MatroxCamera.cs :SetExposureTime: {ex.Message}" ) ).Start();
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}
		public Task<string> TSetGamma( double Gamma ) => Task.Run( () => this.SetGamma( Gamma ) );
		public string SetGamma( double Gamma )
		{
			var sErr = string.Empty;
			try
			{
				if ( !this.ValidVariant() ) return sErr;
				var gamma = Gamma;
				if ( gamma < this.Configuration.Gamma.Min )
					gamma = this.Configuration.Gamma.Min;
				if ( gamma > this.Configuration.Gamma.Max )
					gamma = this.Configuration.Gamma.Max;
				this.Camera.Gamma = gamma;
				this.Configuration.Gamma.Value = gamma;
			}
			catch ( Exception ex )
			{
				//new Thread( () => MessageBox.Show( $"MatroxCamera.cs :SetExposureTime: {ex.Message}" ) ).Start();
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}

		public string GetCalBMP( ref Bitmap Calmodel )
		{
			var Result = "";
			try
			{
				Calmodel = B262_Process.BlankBmp;
				if ( ( Result = this.Vision.ArrayMMFToBMP( this.Configuration.MMFs.Calibration, ref Calmodel ) ) != string.Empty ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public string SaveMMFToConfigCal( string Path )
		{
			var Result = "";
			try
			{
				Result = this.Vision.LoadMMFPathToByteArr( Path, out byte[] mmf_Arr );
				if ( Result != "" )
				{
					throw new Exception( Result );
				}
				this.Configuration.MMFs.Calibration = mmf_Arr;
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public string GetProcessBMP( ref Bitmap Calmodel )
		{
			var Result = "";
			try
			{
				Calmodel = B262_Process.BlankBmp;
				if ( ( Result = this.Vision.ArrayMMFToBMP( this.Configuration.MMFs.Process, ref Calmodel ) ) != string.Empty ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public string SaveMMFToConfigProcess( string Path )
		{
			var Result = "";
			try
			{
				Result = this.Vision.LoadMMFPathToByteArr( Path, out byte[] mmf_Arr );
				if ( Result != "" )
				{
					throw new Exception( Result );
				}
				this.Configuration.MMFs.Process = mmf_Arr;
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public string ExportCalMMF( string Path )
		{
			var Result = "";
			try
			{
				if ( ( Result = this.Vision.MMFExportFromArr( this.Configuration.MMFs.Calibration, Path ) ) != "" ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public string ExportProcessMMF( string Path )
		{
			var Result = "";
			try
			{
				if ( ( Result = this.Vision.MMFExportFromArr( this.Configuration.MMFs.Process, Path ) ) != "" ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}

		public bool SetGain( double Gain )
		{
			try
			{
				if ( this.Name == "Station1Cam" || this.Name == "Station2Cam" ) return true;
				if ( Gain < this.Configuration.Gain.Min )
					Gain = this.Configuration.Gain.Min;
				if ( Gain > this.Configuration.Gain.Max )
					Gain = this.Configuration.Gain.Max;
				this.Camera.Gain = Gain;
				this.Configuration.Gain.Value = Gain;
			}
			catch
			{
				//new Thread( () => MessageBox.Show( $"MatroxCamera.cs :SetGain: {ex.Message}" ) ).Start();
				return false;
			}
			return true;
		}
		public string SetDisplay( DisplayHandler Window )
		{
			var Result = "";
			try
			{
				//if ( !this.Camera.IsCameraConnected ) return string.Empty;
				if ( ( Result = this.Camera.SetCameraDisplayWPF( Window.Display ) ) != string.Empty ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public string SetDisplay( WindowsFormsHost Window )
		{
			var Result = "";
			try
			{
				if ( ( Result = this.Camera.SetCameraDisplayWPF( Window ) ) != string.Empty ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public string DisplayZoom( double Scale )
		{
			var Result = "";
			try
			{
				if ( ( Result = this.Camera.DisplayZoom( Scale ) ) != string.Empty ) throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public void UpdateDisplayZoom()
		{
			try
			{
				this.CheckAndThrowIfError( ErrorClass.E5, this.DisplayZoom( this.ZoomScale ) );
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
		}
		public Task<ErrorResult> SaveIMG( bool Force = false )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( !this.Configuration.ImageSave.SaveAllImage && !Force ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					var datetime = DateTime.Now.ToString( "_yyyyMMdd_HHmmssff" );
					if ( this.Configuration.ImageSave.SavePath == string.Empty )
						this.Configuration.ImageSave.SavePath = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Log\\Images" );
					var name = Path.Combine( this.Configuration.ImageSave.SavePath, this.Name );
					var path = name + datetime;
					this.CheckAndThrowIfError( ErrorClass.E4, this.Camera.SaveIMG( path, this.Configuration.ImageSave.FileFormat ) );
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}

		#region processing
		private Task<(ErrorClass EClass, string ErrorMessage, C_PointD RawPos, C_PointD RawOffsetCenter, C_PointD OffPos)> CheckModelPosition( ROIRectangle ROI )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var RawPos = new C_PointD();
				var OffPos = new C_PointD();
				var RawOffsetCenter = new C_PointD();
				try
				{
					this.CheckAndThrowIfError( ErrorClass.E5, this.SingleGrab().Result );
					this.CheckAndThrowIfError( ErrorClass.E5, this.GrabSuccessful ? string.Empty : "Grab unsuccessful" );
					var Res = this.Vision.CheckModelPosition( this.Configuration.MMFs.Calibration, new CameraObj( this.Camera.MilImage, this.Camera.XHairPos.XOffsetPix, this.Camera.XHairPos.YOffsetPix ), ROI ).Result;
					this.CheckAndThrowIfError( Res.EClass, Res.ErrorMessage );
					RawPos = Res.RawPos;
					OffPos = Res.OffPos;
					RawOffsetCenter = Res.RawOffsetCenter;
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return (this.Result.EClass, this.Result.ErrorMessage, RawPos, RawOffsetCenter, OffPos);
			} );
		}
		public Task<(ErrorClass EClass, string ErrorMessage, C_PointD PointRes)> CheckCalPosition()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var PointRes = new C_PointD();
				try
				{
					var Res = this.CheckModelPosition( this.Configuration.CalROI ).Result;
					this.CheckAndThrowIfError( Res.EClass, Res.ErrorMessage );
					PointRes = Res.RawPos;
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return (this.Result.EClass, this.Result.ErrorMessage, PointRes);
			} );
		}
		public Task<ErrorResult> InspectPosition()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.InspectionResult.Clear();
					var Res = this.CheckModelPosition( this.Configuration.InspectROI ).Result;
					this.CheckAndThrowIfError( Res.EClass, Res.ErrorMessage );
					this.InspectionResult.SetResult( Res.RawPos, Res.RawOffsetCenter, Res.OffPos );
				}
				catch ( Exception ex )
				{
					this.InspectionResult.Status = eInspStatusSingle.Fail;
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		public Task<(ErrorClass EClass, string ErrorMessage)> CheckTeachPosition()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var PointRes = new C_PointD();
				try
				{
					var Res = this.CheckModelPosition( this.Configuration.InspectROI ).Result;
					this.CheckAndThrowIfError( Res.EClass, Res.ErrorMessage );
					PointRes = Res.RawOffsetCenter;
					this.Configuration.XHairPos.SetCrossHairPos( PointRes );
					this.Camera.IsShowCrossLine = true;
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return (this.Result.EClass, this.Result.ErrorMessage);
			} );
		}
		#endregion
		#region Grab Operation 
		public Task<string> ContinuousGrab()
		{
			return Task.Run( () =>
			{
				var Result = string.Empty;
				if ( MachineStateMng.isSimulation ) return Result;
				try
				{
					if ( this.Camera.MtxSys == null ) throw new Exception( $"^{( int )RunErrors.ERR_CamNotInitializedErr }^" );
					//Logger.Debug( "Start to StartGrabProcess" );
					if ( ( Result = this.Camera.CameraGrabContinuos() ) != string.Empty )
					{
						throw new Exception( $"{Result}" );
					}
				}
				catch ( Exception ex )
				{
					Result = this.FormatErrMsg( this.Name, ex );
				}
				return Result;
			} );
		}
		public bool GrabSuccessful { get; set; } = false;
		public Task<string> SingleGrab( int retryCount = 1 )
		{
			return Task.Run( () =>
			{
				var Result = string.Empty;
				var RetryCounter = 0;
				while ( --retryCount >= 0 )
				{
					try
					{
						if ( !this.ValidVariant() ) return Result;
						if ( this.Camera.MtxSys == null && !MachineStateMng.isSimulation ) throw new Exception( $"^{( int )RunErrors.ERR_CamNotInitializedErr }^" );
						this.GrabSuccessful = false;
						this.InspectionResult.Clear();
						Retry:
						this.HaltGrab().Wait();
						Result = this.Camera.CameraGrabSingle();
						if ( Result != string.Empty )
						{
							if ( RetryCounter++ < 5 )
								goto Retry;
							else
								throw new Exception( Result );
						}
						this.GrabSuccessful = true;
					}
					catch ( Exception ex )
					{
						Result = this.FormatErrMsg( this.Name, ex );
						break;
					}
				}
				return Result;
			} );
		}
		public Task<string> HaltGrab()
		{
			return Task.Run( () =>
			{
				var Result = string.Empty;
				try
				{
					if ( MachineStateMng.isSimulation ) return Result;
					if ( this.Camera.MtxSys == null ) throw new Exception( $"^{( int )RunErrors.ERR_CamNotInitializedErr }^" );
					this.Camera.CameraStopContinuos();
				}
				catch ( Exception ex )
				{
					Result = this.FormatErrMsg( this.Name, ex );
				}
				return Result;
			} );
		}
		#endregion

		private unsafe Bitmap ToBitmap( byte[,] rawImage )
		{
			try
			{
				int width = rawImage.GetLength( 1 );
				int height = rawImage.GetLength( 0 );

				Bitmap Image = new Bitmap( width, height );
				BitmapData bitmapData = Image.LockBits(
					new Rectangle( 0, 0, width, height ),
					ImageLockMode.ReadWrite,
					PixelFormat.Format32bppArgb
				);
				ColorARGB* startingPosition = ( ColorARGB* )bitmapData.Scan0;


				for ( int i = 0; i < height; i++ )
					for ( int j = 0; j < width; j++ )
					{
						double color = rawImage[ i, j ];
						byte rgb = ( byte )( color * 255 );

						ColorARGB* position = startingPosition + j + i * width;
						position->A = 255;
						position->R = rgb;
						position->G = rgb;
						position->B = rgb;
					}

				Image.UnlockBits( bitmapData );
				return Image;
			}
			catch
			{ }
			return null;
		}
	}
	[Serializable]
	public class ImageSave : BaseUtility
	{
		private string s_SavePath = string.Empty;
		public string SavePath
		{
			get => this.s_SavePath;
			set => this.Set( ref this.s_SavePath, value, "SavePath" );
		}
		private FileFormat e_FileFormat = FileFormat.PNG;
		public FileFormat FileFormat
		{
			get => this.e_FileFormat;
			set => this.Set( ref this.e_FileFormat, value, "FileFormat" );
		}
		private bool b_SaveAllImage = false;
		public bool SaveAllImage
		{
			get => this.b_SaveAllImage;
			set => this.Set( ref this.b_SaveAllImage, value, "SaveAllImage" );
		}
		public void BrowseFolder()
		{
			try
			{
				System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
				var result = openFileDlg.ShowDialog();
				if ( result.ToString() != string.Empty )
				{
					this.SavePath = openFileDlg.SelectedPath;
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( null, ex ), ErrorTitle.InvalidOperation );
			}
		}
		public ImageSave()
		{ }
	}

	public struct ColorARGB
	{
		public byte B;
		public byte G;
		public byte R;
		public byte A;

		public ColorARGB( Color color )
		{
			this.A = color.A;
			this.R = color.R;
			this.G = color.G;
			this.B = color.B;
		}

		public ColorARGB( byte a, byte r, byte g, byte b )
		{
			this.A = a;
			this.R = r;
			this.G = g;
			this.B = b;
		}

		public Color ToColor()
		{
			return Color.FromArgb( this.A, this.R, this.G, this.B );
		}
	}
	public class MMFCollection : BaseUtility
	{
		public byte[] Calibration { get; set; }
		public byte[] Process { get; set; }
	}
	public enum CalibrationDirection
	{
		Horizontal,
		Vertical,
	}
	public class CalibrationData : BaseUtility
	{
		int MaxTol = 5;
		public C_PointD First { get; set; } = new C_PointD();
		public C_PointD Second { get; set; } = new C_PointD();
		public CalibrationDirection Direction
		{
			get => this.GetValue( () => this.Direction );
			set => this.SetValue( () => this.Direction, value );
		}
		public C_PointD ScalePixperMM { get; set; } = new C_PointD();
		public C_PointD ActualLength { get; set; } = new C_PointD();
		public void Clear()
		{
			this.First.Clear();
			this.Second.Clear();
		}
		public string CalculateScale()
		{
			var sErr = string.Empty;
			try
			{
				if ( this.Direction == CalibrationDirection.Horizontal )
				{
					if ( Math.Abs( this.First.Y - this.Second.Y ) > this.MaxTol ) throw new Exception( $"Difference in Y must be less than or equal {this.MaxTol} pixels" );
					this.ScalePixperMM.X = Math.Abs( this.First.X - this.Second.X ) / this.ActualLength.X;
				}
				else
				{
					if ( Math.Abs( this.First.X - this.Second.X ) > this.MaxTol ) throw new Exception( $"Difference in X must be less than or equal {this.MaxTol} pixels" );
					this.ScalePixperMM.Y = Math.Abs( this.First.Y - this.Second.Y ) / this.ActualLength.Y;
				}
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( null, ex );
			}
			return sErr;
		}
	}

	public class InspectionResult : BaseUtility
	{
		public C_PointD RawPosition { get; set; } = new C_PointD();//pixels
		public C_PointD RawOffset { get; set; } = new C_PointD();//pixels
		public C_PointD PositionOffset { get; set; } = new C_PointD();//pixels
		public C_PointD PixPerMM { get; set; }
		public C_PointD PositionOffsetMM { get; set; } = new C_PointD();//mm
		public eInspStatusSingle Status
		{
			get => this.GetValue( () => this.Status );
			set => this.SetValue( () => this.Status, value );

		}

		public void SetResult( C_PointD RawPosition, C_PointD RawOffset, C_PointD PositionOffset )
		{
			this.RawPosition.Copy( RawPosition );
			this.RawOffset.Copy( RawOffset );
			this.PositionOffset.Copy( PositionOffset );
			this.PositionOffsetMM.X = this.PositionOffset.X / this.PixPerMM.X;
			this.PositionOffsetMM.Y = this.PositionOffset.Y / this.PixPerMM.Y;
			this.Status = eInspStatusSingle.Done;
		}
		public void Clear()
		{
			this.RawPosition.Clear();
			this.RawOffset.Clear();
			this.PositionOffset.Clear();
			this.PositionOffsetMM.Clear();
			this.Status = eInspStatusSingle.NotStarted;
		}
		public void Copy( InspectionResult Source )
		{
			this.RawPosition.Copy( Source.RawPosition );
			this.RawOffset.Copy( Source.RawOffset );
			this.PositionOffset.Copy( Source.PositionOffset );
			this.PixPerMM.Copy( Source.PixPerMM );
			this.PositionOffsetMM.Copy( Source.PositionOffsetMM );
		}
	}
	public enum eInspStatusSingle
	{
		NotStarted,
		Done,
		Fail,
	}

}
