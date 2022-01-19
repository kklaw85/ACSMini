using Matrox.MatroxImagingLibrary;
using System;

namespace JptCamera
{
	public class MtxGigeCamera : MtxGigeBase
	{

		#region Private members

		private CameraSettings.TriggerModeEnum _currentTriggerMode = CameraSettings.TriggerModeEnum.TRIGGER_CONTINUOUS;
		private TriggerTypeEnum _currentTriggerType = TriggerTypeEnum.TRIGGER_CONTINUOUS;

		#endregion

		public MtxGigeCamera()
		{
			JptMatroxSystem.MatroxSystem _mtxSystem = JptMatroxSystem.MatroxSystem.Instance;

			this.InitialiseMtxSystem( _mtxSystem.GigeSystemID );
		}

		//public string Initialise

		public override string CameraBrand
		{
			get
			{
				try { return this.Settings.M_CameraVendor; }
				catch { return ""; }
			}
		}

		public override string CameraModel
		{
			get
			{
				try { return this.Settings.M_CameraModel; }
				catch { return ""; }
			}
		}

		public override string CameraUserID
		{
			get
			{
				try { return this.Settings.M_UserName; }
				catch { return ""; }
			}
		}

		public override string CameraSerialName
		{
			get
			{
				try { return this.Settings.M_SerialNumber; }
				catch { return ""; }
			}
		}


		/// <summary>
		/// Exposure time in micro seconds
		/// </summary>
		public override double Exposure
		{
			get
			{
				try
				{
					double expInNanoSec = this.Settings.M_Exposure;
					return expInNanoSec / 1000;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					double expInNanoSec = value * 1000;
					JPTUtility.Logger.doLog( "ExposureInNanoSec(SET) " + expInNanoSec.ToString() );
					this.Settings.M_Exposure = expInNanoSec;
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "Exposure(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override double ExposureMinimum
		{
			get
			{
				try { return this.Settings.M_ExposureMin; }
				catch { return -1; }
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override double ExposureMaximum
		{
			get
			{
				try { return this.Settings.M_ExposureMax; }
				catch { return -1; }
			}
		}

		public override ushort Binning
		{
			get
			{
				try
				{
					int binning = this.Settings.BinningHorizontal;
					return ( ushort )binning;
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "Binning(GET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
			set
			{
				JPTUtility.Logger.doLog( "Binning(SET) " + value.ToString() );
				try
				{
					ushort binning = value;
					if ( ( binning < this.Settings.BinningMinimum ) ||
						( binning > this.Settings.BinningMaximum ) )
					{
						throw new Exception( "Value exceed range (" + this.Settings.BinningMinimum.ToString()
						+ "-" + this.Settings.BinningMaximum.ToString() + ")" );
					}

					// Set both horizontal and vertical binning the same value
					this.Settings.BinningHorizontal = ( int )binning;
					this.Settings.BinningVertical = ( int )binning;

				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "Binning(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		public override PixelFormatEnum PixelFormat
		{
			get
			{
				try
				{
					string value = this.Settings.M_PixelFormat;
					PixelFormatEnum result = PixelFormatEnum.UNDEFINED;
					if ( value.Length > 0 )
					{
						switch ( value )
						{
							case "Mono8": result = PixelFormatEnum.MONO8; break;
							case "Mono10": result = PixelFormatEnum.MONO10; break;
							case "Mono12": result = PixelFormatEnum.MONO12; break;
							case "RGB8Packed": result = PixelFormatEnum.RGB8PACKED; break;
							default: break;
						}
					}
					return result;
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "BitMode(GET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
			set
			{
				JPTUtility.Logger.doLog( "BitMode(SET) " + value.ToString() );
				try
				{
					string result = "";
					bool beforeApply = this.IsCameraInColorMode;
					switch ( value )
					{
						case PixelFormatEnum.MONO8: result = "Mono8"; break;
						case PixelFormatEnum.MONO10: result = "Mono10"; break;
						case PixelFormatEnum.MONO12: result = "Mono12"; break;
						case PixelFormatEnum.RGB8PACKED: result = "RGB8Packed"; break;
						default: break;
					}
					if ( result.Length > 0 )
					{
						this.Settings.M_PixelFormat = result;

						bool afterApplied = this.IsCameraInColorMode;
						if ( beforeApply != afterApplied )
						{
							//Allocate new buffer
							this.AllocateNewBufferResource();
						}
					}
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "BitMode(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}



		///// <summary>
		///// Preset value to 1 for Adimec S-50A30 camera
		///// </summary>
		//public double ExposureIncrement
		//{
		//    get { return 1; } //return cam.Settings.ExposureTimeIncrement; }
		//}

		public override double Gain
		{
			get
			{
				try { return this.Settings.Gain; }
				catch { return -1; }
			}
			set
			{
				try
				{
					JPTUtility.Logger.doLog( "Gain(SET) " + value.ToString() );
					this.Settings.Gain = value;
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "Gain(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		public override double GainMinimum
		{
			get
			{
				try { return Convert.ToDouble( this.Settings.M_GainMin ); }
				catch { return -1; }
			}
		}

		public override double GainMaximum
		{
			get
			{
				try { return Convert.ToDouble( this.Settings.M_GainMax ); }
				catch { return -1; }
			}
		}

		/// <summary>
		/// Gamma enabled
		/// </summary>
		public override bool GammaEnable
		{
			get { return this.Settings.GammaEnable; }
			set
			{
				try
				{
					this.Settings.GammaEnable = value;
				}
				catch ( Exception ex )
				{ throw ex; }
			}
		}

		/// <summary>
		/// Gamma 
		/// </summary>
		public override double Gamma
		{
			get
			{
				try { return this.Settings.Gamma; }
				catch { return -1; }
			}
			set
			{
				try
				{
					JPTUtility.Logger.doLog( "Gamma(SET) " + value.ToString() );
					this.Settings.Gamma = value;
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "Gamma(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		/// <summary>
		/// Gamma minimum allowable
		/// return -1 if error
		/// </summary>
		public override double GammaMinimum
		{
			get
			{
				try { return this.Settings.GammaMinimum; }
				catch { return -1; }
			}
		}

		/// <summary>
		/// Gamma Maximum allowable
		/// return -1 if error
		/// </summary>
		public override double GammaMaximum
		{
			get
			{
				try { return this.Settings.GammaMaximum; }
				catch { return -1; }
			}
		}

		public bool ExternalTriggerIn
		{
			get
			{
				try { return this.TriggerType != TriggerTypeEnum.TRIGGER_CONTINUOUS; }
				catch { return false; }
			}
			set
			{
				JPTUtility.Logger.doLog( "ExternalTriggerIn(SET) " + value.ToString() );
				try
				{
					if ( value )
					{
						this.TriggerType = TriggerTypeEnum.TRIGGER_HW_RISING;
					}
					else
					{
						this.TriggerType = TriggerTypeEnum.TRIGGER_CONTINUOUS;
					}
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "ExternalTriggerIn(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		public override TriggerTypeEnum TriggerType
		{
			get
			{
				try
				{
					if ( this.Settings.M_GrabTriggerState == true )
					{
						if ( this.Settings.M_GrabTriggerSource == CameraSettings.TriggerSourceEnum.M_SOFTWARE )
						{ this._currentTriggerType = TriggerTypeEnum.TRIGGER_SOFTWARE; }
						else if ( this.Settings.M_GrabTriggerSource == CameraSettings.TriggerSourceEnum.M_AUX_IO0 ||
							this.Settings.M_GrabTriggerSource == CameraSettings.TriggerSourceEnum.M_AUX_IO1 )
						{
							if ( this.Settings.M_GrabTriggerActivation == CameraSettings.TriggerActivationEnum.M_EDGE_RISING )
							{ this._currentTriggerType = TriggerTypeEnum.TRIGGER_HW_RISING; }
							else if ( this.Settings.M_GrabTriggerActivation == CameraSettings.TriggerActivationEnum.M_EDGE_FALLING )
							{ this._currentTriggerType = TriggerTypeEnum.TRIGGER_HW_FALLING; }
						}
						else
						{ this._currentTriggerType = TriggerTypeEnum.TRIGGER_OFF; }
					}
					else { this._currentTriggerType = TriggerTypeEnum.TRIGGER_OFF; }
					return this._currentTriggerType;
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "TriggerType(GET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
			set
			{
				JPTUtility.Logger.doLog( "TriggerType(SET) " + value.ToString() );
				try
				{
					if ( this._currentTriggerType == value )
					{
						return;
					}

					//bool tempStop = false;
					//if (_isInContinuousMode)
					//{
					//    Stop();
					//    tempStop = true;
					//}

					switch ( value )
					{
						case TriggerTypeEnum.TRIGGER_CONTINUOUS:
							this.Settings.M_GrabTriggerState = false;
							break;
						case TriggerTypeEnum.TRIGGER_HW_FALLING:
							this.Settings.M_GrabTriggerState = true;
							this.Settings.M_GrabTriggerSource = CameraSettings.TriggerSourceEnum.M_AUX_IO0;
							this.Settings.M_GrabTriggerActivation = CameraSettings.TriggerActivationEnum.M_EDGE_FALLING;
							break;
						case TriggerTypeEnum.TRIGGER_HW_RISING:
							this.Settings.M_GrabTriggerState = true;
							this.Settings.M_GrabTriggerSource = CameraSettings.TriggerSourceEnum.M_AUX_IO0;
							this.Settings.M_GrabTriggerActivation = CameraSettings.TriggerActivationEnum.M_EDGE_RISING;
							break;
						case TriggerTypeEnum.TRIGGER_SOFTWARE:
							this.Settings.M_GrabTriggerState = true;
							this.Settings.M_GrabTriggerSource = CameraSettings.TriggerSourceEnum.M_SOFTWARE;
							break;
						default:
							this.Settings.M_GrabTriggerState = false;
							break;
					}
					this._currentTriggerType = value;

					JPTUtility.Logger.doLog( "TriggerType(SET) " + value.ToString() + " successfully completed" );

					//if (tempStop)
					//{
					//    Start();
					//}
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "TriggerType(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		public CameraSettings.TriggerModeEnum TriggerMode
		{
			get
			{
				try
				{
					if ( this.Settings.M_GrabTriggerState == true )
					{
						if ( this.Settings.M_GrabTriggerSource == CameraSettings.TriggerSourceEnum.M_SOFTWARE )
						{ this._currentTriggerMode = CameraSettings.TriggerModeEnum.TRIGGER_SOFTWARE; }
						else if ( this.Settings.M_GrabTriggerSource == CameraSettings.TriggerSourceEnum.M_AUX_IO0 ||
							this.Settings.M_GrabTriggerSource == CameraSettings.TriggerSourceEnum.M_AUX_IO1 )
						{
							if ( this.Settings.M_GrabTriggerActivation == CameraSettings.TriggerActivationEnum.M_EDGE_RISING )
							{ this._currentTriggerMode = CameraSettings.TriggerModeEnum.TRIGGER_LO_HI; }
							else if ( this.Settings.M_GrabTriggerActivation == CameraSettings.TriggerActivationEnum.M_EDGE_FALLING )
							{ this._currentTriggerMode = CameraSettings.TriggerModeEnum.TRIGGER_HI_LO; }
						}
						else
						{ this._currentTriggerMode = CameraSettings.TriggerModeEnum.TRIGGER_OFF; }
					}
					else { this._currentTriggerMode = CameraSettings.TriggerModeEnum.TRIGGER_OFF; }
					return this._currentTriggerMode;
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "TriggerType(GET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
			set
			{
				JPTUtility.Logger.doLog( "TriggerType(SET) " + value.ToString() );
				try
				{
					if ( this._currentTriggerMode == value )
					{
						return;
					}

					//bool tempStop = false;
					//if (_isInContinuousMode)
					//{
					//    Stop();
					//    tempStop = true;
					//}

					switch ( value )
					{
						case CameraSettings.TriggerModeEnum.TRIGGER_CONTINUOUS:
							this.Settings.M_GrabTriggerState = false;
							break;
						case CameraSettings.TriggerModeEnum.TRIGGER_HI_LO:
							this.Settings.M_GrabTriggerState = true;
							this.Settings.M_GrabTriggerSource = CameraSettings.TriggerSourceEnum.M_AUX_IO0;
							this.Settings.M_GrabTriggerActivation = CameraSettings.TriggerActivationEnum.M_EDGE_FALLING;
							break;
						case CameraSettings.TriggerModeEnum.TRIGGER_LO_HI:
							this.Settings.M_GrabTriggerState = true;
							this.Settings.M_GrabTriggerSource = CameraSettings.TriggerSourceEnum.M_AUX_IO0;
							this.Settings.M_GrabTriggerActivation = CameraSettings.TriggerActivationEnum.M_EDGE_RISING;
							break;
						case CameraSettings.TriggerModeEnum.TRIGGER_SOFTWARE:
							this.Settings.M_GrabTriggerState = true;
							this.Settings.M_GrabTriggerSource = CameraSettings.TriggerSourceEnum.M_SOFTWARE;
							break;
						default:
							this.Settings.M_GrabTriggerState = false;
							break;
					}
					this._currentTriggerMode = value;

					JPTUtility.Logger.doLog( "TriggerType(SET) " + value.ToString() + " successfully completed" );

					//if (tempStop)
					//{
					//    Start();
					//}
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "TriggerType(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		public override int TriggerDelay
		{
			get { return ( int )this.Settings.M_GrabTriggerDelay; }
			set
			{
				try
				{
					JPTUtility.Logger.doLog( "TriggerDelay(SET) " + value.ToString() );
					this.Settings.M_GrabTriggerDelay = value;
				}
				catch ( Exception ex )
				{
					string sErr = string.Format( "TriggerDelay(SET) error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		public override int TriggerDelayMin
		{
			get
			{
				return 0;
				//try { return (int)_mtxCamera.Settings.TriggerDelayMin; }
				//catch { return -1; }
			}
		}

		public override int TriggerDelayMax
		{
			get
			{
				return 1000000;
				//try { return (int)_mtxCamera.Settings.TriggerDelayMax; }
				//catch { return -1; }
			}
		}

		//public TriggerMode TriggerModeSupported
		//{
		//    get { return (TriggerMode)(0x0F); }
		//}

		public override int Width
		{
			get
			{
				try
				{
					double width = this.Settings.M_SourceSizeX;
					return Convert.ToInt32( width );
				}
				catch
				{ return -1; }
			}
			set
			{
				try { this.Settings.M_SourceSizeX = value; }
				catch ( Exception ex )
				{
					string sErr = string.Format( "Width error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );
				}
			}
		}

		public override int WidthMinimum
		{
			get
			{
				try
				{
					double width = this.Settings.M_SizeXMin;
					return Convert.ToInt32( width );
				}
				catch
				{
					return -1;
				}
			}
		}

		public override int WidthMaximum
		{
			get
			{
				try
				{
					double width = this.Settings.M_SizeXMax;
					return Convert.ToInt32( width );
				}
				catch
				{
					return -1;
				}
			}
		}

		public override int Height
		{
			get
			{
				try
				{
					double height = this.Settings.M_SourceSizeY;
					return Convert.ToInt32( height );
				}
				catch
				{ return -1; }
			}
			set
			{
				try { this.Settings.M_SourceSizeY = value; }
				catch ( Exception ex )
				{
					string sErr = string.Format( "Height error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );

				}
			}
		}

		public override int HeightMinimum
		{
			get
			{
				try
				{
					double hei = this.Settings.M_SizeYMin;
					return Convert.ToInt32( hei );
				}
				catch
				{
					return -1;
				}
			}
		}

		public override int HeightMaximum
		{
			get
			{
				try
				{
					double hei = this.Settings.M_SizeYMax;
					return Convert.ToInt32( hei );
				}
				catch
				{
					return -1;
				}
			}
		}

		public override int OffsetX
		{
			get
			{
				try
				{
					int offX = this.Settings.M_SOURCEOFFSETX;
					return Convert.ToInt32( offX );
				}
				catch
				{ return -1; }
			}
			set
			{
				try { this.Settings.M_SOURCEOFFSETX = value; }
				catch ( Exception ex )
				{
					string sErr = string.Format( "OffsetX error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );

				}
			}
		}

		public override int OffsetY
		{
			get
			{
				try
				{
					int offY = this.Settings.OffsetY;
					return Convert.ToInt32( offY );
				}
				catch
				{ return -1; }
			}
			set
			{
				try { this.Settings.OffsetY = value; }
				catch ( Exception ex )
				{
					string sErr = string.Format( "OffsetY error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );

				}
			}
		}

		public override bool ReverseX
		{
			get
			{
				try
				{
					int value = this.Settings.M_GRABDIRECTIONX;
					if ( value == MIL.M_FORWARD ) return false;
					else if ( value == MIL.M_REVERSE ) return true;

					return false;
				}
				catch
				{ return false; }
			}
			set
			{
				try { this.Settings.M_GRABDIRECTIONX = value == true ? MIL.M_REVERSE : MIL.M_FORWARD; }
				catch ( Exception ex )
				{
					string sErr = string.Format( "ReverseX error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );

				}
			}
		}

		public override bool ReverseY
		{
			get
			{
				try
				{
					int value = this.Settings.M_GrabDirectionY;
					if ( value == MIL.M_FORWARD ) return false;
					else if ( value == MIL.M_REVERSE ) return true;

					return false;
				}
				catch
				{ return false; }
			}
			set
			{
				try { this.Settings.M_GrabDirectionY = value == true ? MIL.M_REVERSE : MIL.M_FORWARD; }
				catch ( Exception ex )
				{
					string sErr = string.Format( "ReverseY error: " + ex.Message );
					JPTUtility.Logger.doLog( sErr );
					throw new Exception( sErr );

				}
			}
		}

		public double FrameRate
		{
			get
			{
				try { return this.Settings.M_FrameRate; }
				catch { return -1; }
			}
			//set
			//{
			//    try
			//    {
			//        JPTUtility.Logger.doLog("FrameRate(SET) " + value.ToString());
			//        Settings.M_FrameRate = value;
			//    }
			//    catch (Exception ex)
			//    {
			//        string sErr = string.Format("FrameRate(SET) error: " + ex.Message);
			//        JPTUtility.Logger.doLog(sErr);
			//        throw new Exception(sErr);
			//    }
			//}
		}

	}


}
