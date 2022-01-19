using Matrox.MatroxImagingLibrary;
using System;
using System.Text;

namespace JptCamera
{
	public class CameraSettings
	{

		public enum ColorModeEnum
		{
			Undefined,
			Mono8,
			Mono10,
			Mono12,
			RGB8Packed,
		}

		public enum TriggerModeEnum
		{
			TRIGGER_OFF = 0,
			TRIGGER_CONTINUOUS = 1,
			TRIGGER_HI_LO = 2,
			TRIGGER_LO_HI = 4,
			TRIGGER_SOFTWARE = 8,
		}

		public enum ColorBit
		{
			Mono8,
			Mono10,
			Mono12,
		}

		public enum TriggerActivationEnum
		{
			M_DEFAULT,
			M_EDGE_FALLING,
			M_EDGE_RISING,
			M_LEVEL_HIGH,
			M_LEVEL_LOW,
		}

		public enum TriggerSourceEnum
		{
			M_AUX_IO0,  // Specifies to use auxiliary input signal n as the trigger source, where n is the number of the auxiliary signal.
			M_AUX_IO1,
			M_AUX_IO2,
			M_AUX_IO3,
			M_AUX_IO4,
			M_AUX_IO5,
			M_AUX_IO6,
			M_AUX_IO7,
			M_AUX_IO8,
			M_AUX_IO9,
			M_AUX_IO10,
			M_AUX_IO11,
			M_AUX_IO12,
			M_AUX_IO13,
			M_AUX_IO14,
			M_AUX_IO15,
			M_AUX_IO16,
			M_AUX_IO17,
			M_AUX_IO18,
			M_AUX_IO19,
			M_AUX_IO20,
			M_AUX_IO21,
			M_AUX_IO22,
			M_AUX_IO23,
			M_AUX_IO24,
			M_AUX_IO25,
			M_AUX_IO26,
			M_AUX_IO27,
			M_AUX_IO28,
			M_AUX_IO29,
			M_AUX_IO30,
			M_AUX_IO31,
			M_DEFAULT,  // Same as the one specified by the DCF.
			M_ROTARY_ENCODER,  // Specifies to use the output of the default rotary decoder (set with M_ROTARY_ENCODER_OUTPUT_MODE) as the trigger source.
			M_ROTARY_ENCODER1,  // Specifies to use rotary decoder n as the trigger source, where n is a number between 1 and 4.
			M_ROTARY_ENCODER2,
			M_ROTARY_ENCODER3,
			M_ROTARY_ENCODER4,
			M_SOFTWARE,  // Specifies to use a software trigger as the trigger source.
			M_SOFTWARE1,  // Specifies to use the software trigger being used for the grab of another digitizer (on the same board) as the trigger source.
			M_SOFTWARE2,
			M_SOFTWARE3,
			M_SOFTWARE4,
			M_TIMER0,  // Specifies to use the output signal of the specified timer as the trigger source.
			M_TIMER1,
			M_TIMER2,
			M_TIMER3,
			M_TIMER4,
			M_TIMER5,
			M_TIMER6,
			M_TIMER7,
			M_TIMER8,
			M_TL_TRIGGER,
		}

		public enum AdimecTriggerSource
		{
			Trigger,
			IO_Connector,
			Unknown
		}

		public enum AdimecTriggerActivation
		{
			FallingEdge,
			RisingEdge
		}

		public enum AdimecExposureMode
		{
			Timed,
			TriggerWidth,
			SyncControlMode,
			TimedTriggerControl
		}

		public enum State
		{
			M_DEFAULT,
			M_ENABLE,
			M_DISABLE,
		}

		//double _exposureMode;
		//double _exposureTime;

		MIL_ID _digID;

		public CameraSettings()
		{ }

		public CameraSettings( MIL_ID DigitizerID )
		{
			this._digID = DigitizerID;
		}


		public string DeviceVendorName
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "DeviceVendorName", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "DeviceVendorName", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					return result.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "DeviceModelName error: " + ex.Message ); }
			}
		}

		public string DeviceModelName
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "DeviceModelName", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "DeviceModelName", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					return result.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "DeviceModelName error: " + ex.Message ); }
			}
		}

		public string DeviceSerialNumber
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "DeviceSerialNumber", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "DeviceSerialNumber", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					return result.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "DeviceSerialNumber error: " + ex.Message ); }
			}
		}

		public string DeviceVersion
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "DeviceVersion", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "DeviceVersion", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					return result.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "DeviceSerialNumber error: " + ex.Message ); }
			}
		}

		public string DeviceUserID
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "DeviceUserID", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "DeviceUserID", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					return result.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "DeviceSerialNumber error: " + ex.Message ); }
			}
		}

		public double Width
		{
			get
			{
				try
				{
					long width = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "Width", MIL.M_TYPE_INT64, ref width );
					return ( double )width;
				}
				catch ( Exception ex ) { throw new Exception( "Width(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					long width = ( long )value;
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "Width", MIL.M_TYPE_INT64, ref width );
				}
				catch ( Exception ex ) { throw new Exception( "Width(SET) error: " + ex.Message ); }
			}
		}

		public double WidthMinimum
		{
			get
			{
				try
				{
					long width = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MIN, "Width", MIL.M_TYPE_INT64, ref width );
					return ( double )width;
				}
				catch ( Exception ex ) { throw new Exception( "WidthMinimum(GET) error: " + ex.Message ); }
			}
		}

		public double WidthMaximum
		{
			get
			{
				try
				{
					long width = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MAX, "Width", MIL.M_TYPE_INT64, ref width );
					return ( double )width;
				}
				catch ( Exception ex ) { throw new Exception( "WidthMaximum(GET) error: " + ex.Message ); }
			}
		}

		public double Height
		{
			get
			{
				try
				{
					long height = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "Height", MIL.M_TYPE_INT64, ref height );
					return ( double )height;
				}
				catch ( Exception ex ) { throw new Exception( "Height(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					long height = ( long )value;
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "Height", MIL.M_TYPE_INT64, ref height );
				}
				catch ( Exception ex ) { throw new Exception( "Height(SET) error: " + ex.Message ); }
			}
		}

		public double HeightMinimum
		{
			get
			{
				try
				{
					long height = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MIN, "Height", MIL.M_TYPE_INT64, ref height );
					return ( double )height;
				}
				catch ( Exception ex ) { throw new Exception( "HeightMinimum(GET) error: " + ex.Message ); }
			}
		}

		public double HeightMaximum
		{
			get
			{
				try
				{
					long height = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MAX, "Height", MIL.M_TYPE_INT64, ref height );
					return ( double )height;
				}
				catch ( Exception ex ) { throw new Exception( "HeightMaximum(GET) error: " + ex.Message ); }
			}
		}

		public int OffsetX
		{
			get
			{
				try
				{
					long offX = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "OffsetX", MIL.M_TYPE_INT64, ref offX );
					return ( int )offX;
				}
				catch ( Exception ex ) { throw new Exception( "OffsetX(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					long offX = ( long )value;
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "OffsetX", MIL.M_TYPE_INT64, ref offX );
				}
				catch ( Exception ex ) { throw new Exception( "OffsetX(SET) error: " + ex.Message ); }
			}
		}

		public int OffsetY
		{
			get
			{
				try
				{
					long offY = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "OffsetY", MIL.M_TYPE_INT64, ref offY );
					return ( int )offY;
				}
				catch ( Exception ex ) { throw new Exception( "OffsetY(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					long offY = ( long )value;
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "OffsetY", MIL.M_TYPE_INT64, ref offY );
				}
				catch ( Exception ex ) { throw new Exception( "OffsetY(SET) error: " + ex.Message ); }
			}
		}

		public string PixelFormat
		{
			get
			{
				try
				{
					long PixelFormatStringSize = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "PixelFormat", MIL.M_TYPE_MIL_INT, ref PixelFormatStringSize );
					StringBuilder PixelFormatPtr = new StringBuilder( ( int )PixelFormatStringSize );
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "PixelFormat", MIL.M_TYPE_STRING, PixelFormatPtr );

					return PixelFormatPtr.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "PixelFormat(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "PixelFormat", MIL.M_TYPE_STRING, value );
				}
				catch ( Exception ex ) { throw new Exception( "PixelFormat(SET) error: " + ex.Message ); }
			}
		}

		public double AcquisitionFrameRate
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "AcquisitionFrameRate", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "AcquisitionFrameRate", MIL.M_TYPE_DOUBLE, ref value );
				}
				catch ( Exception ex ) { throw new Exception( "AcquisitionFrameRate(SET) error: " + ex.Message ); }
			}
		}

		public double AcquisitionFrameRateMinimum
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MIN, "AcquisitionFrameRate", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public double AcquisitionFrameRateMaximum
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MAX, "AcquisitionFrameRate", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public bool LongExposure
		{
			get { throw new NotImplementedException(); }
			set { }
		}

		public int BinningHorizontal
		{
			get
			{
				try
				{
					long BinningHorizontal = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "BinningHorizontal", MIL.M_TYPE_INT64, ref BinningHorizontal );
					return ( int )BinningHorizontal;
				}
				catch ( Exception ex ) { throw new Exception( "BinningHorizontal(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					long BinningHorizontal = value;
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "BinningHorizontal", MIL.M_TYPE_INT64, ref BinningHorizontal );
				}
				catch ( Exception ex ) { throw new Exception( "BinningHorizontal(SET) error: " + ex.Message ); }
			}
		}

		public int BinningVertical
		{
			get
			{
				try
				{
					long BinningVertical = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "BinningVertical", MIL.M_TYPE_INT64, ref BinningVertical );
					return ( int )BinningVertical;
				}
				catch ( Exception ex ) { throw new Exception( "BinningVertical(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					long BinningVertical = value;
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "BinningVertical", MIL.M_TYPE_INT64, ref BinningVertical );
				}
				catch ( Exception ex ) { throw new Exception( "BinningVertical(SET) error: " + ex.Message ); }
			}
		}

		/// <summary>
		/// Binning minimum settings. Hardcoded to 1
		/// </summary>
		public int BinningMinimum
		{ get { return 1; } }

		/// <summary>
		/// Binning maximum settings. Hardcoded to 4
		/// </summary>
		public int BinningMaximum
		{ get { return 4; } }

		/// <summary>
		/// Specifies the delay (in us) to apply after the trigger reception before activating it.
		/// </summary>
		public double TriggerDelay
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerDelay", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerDelay", MIL.M_TYPE_DOUBLE, ref value );
				}
				catch ( Exception ex ) { throw new Exception( "TriggerDelay(SET) error: " + ex.Message ); }
			}
		}

		public double TriggerDelayMin
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MIN, "TriggerDelay", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public double TriggerDelayMax
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MAX, "TriggerDelay", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

#if PLATFORM_X64
		/// <summary>
		/// GrabTriggerSource from framegrabber Rapixo
		/// M_AUX_IO0, M_AUX_IO1, ... M_SOFTWARE
		/// </summary>
		public TriggerSourceEnum GrabTriggerSource
		{
			get
			{
				try
				{
					MIL_INT GrabTriggerSource = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_TRIGGER_SOURCE, ref GrabTriggerSource );
					switch ( ( int )GrabTriggerSource )
					{
						case MIL.M_AUX_IO0: return TriggerSourceEnum.M_AUX_IO0;  // Specifies to use auxiliary input signal n as the trigger source, where n is the number of the auxiliary signal.
						case MIL.M_AUX_IO1: return TriggerSourceEnum.M_AUX_IO1;
						case MIL.M_AUX_IO2: break;
						case MIL.M_AUX_IO3: break;
						case MIL.M_AUX_IO4: break;
						case MIL.M_AUX_IO5: break;
						case MIL.M_AUX_IO6: break;
						case MIL.M_AUX_IO7: break;
						case MIL.M_AUX_IO8: break;
						case MIL.M_AUX_IO9: break;
						case MIL.M_AUX_IO10: break;
						case MIL.M_AUX_IO11: break;
						case MIL.M_AUX_IO12: break;
						case MIL.M_AUX_IO13: break;
						case MIL.M_AUX_IO14: break;
						case MIL.M_AUX_IO15: break;
						case MIL.M_AUX_IO16: break;
						case MIL.M_AUX_IO17: break;
						case MIL.M_AUX_IO18: break;
						case MIL.M_AUX_IO19: break;
						case MIL.M_AUX_IO20: break;
						case MIL.M_AUX_IO21: break;
						case MIL.M_AUX_IO22: break;
						case MIL.M_AUX_IO23: break;
						case MIL.M_AUX_IO24: break;
						case MIL.M_AUX_IO25: break;
						case MIL.M_AUX_IO26: break;
						case MIL.M_AUX_IO27: break;
						case MIL.M_AUX_IO28: break;
						case MIL.M_AUX_IO29: break;
						case MIL.M_AUX_IO30: break;
						case MIL.M_AUX_IO31: break;
						case MIL.M_DEFAULT: return TriggerSourceEnum.M_DEFAULT;  // Same as the one specified by the DCF.
						case MIL.M_ROTARY_ENCODER: break;  // Specifies to use the output of the default rotary decoder (set with M_ROTARY_ENCODER_OUTPUT_MODE) as the trigger source.
						case MIL.M_ROTARY_ENCODER1: break;  // Specifies to use rotary decoder n as the trigger source, where n is a number between 1 and 4.
						case MIL.M_ROTARY_ENCODER2: break;
						case MIL.M_ROTARY_ENCODER3: break;
						case MIL.M_ROTARY_ENCODER4: break;
						case MIL.M_SOFTWARE: return TriggerSourceEnum.M_SOFTWARE;  // Specifies to use a software trigger as the trigger source.
						case MIL.M_SOFTWARE1: break;  // Specifies to use the software trigger being used for the grab of another digitizer (on the same board) as the trigger source.
						case MIL.M_SOFTWARE2: break;
						case MIL.M_SOFTWARE3: break;
						case MIL.M_SOFTWARE4: break;
						case MIL.M_TIMER0: break;  // Specifies to use the output signal of the specified timer as the trigger source.
						case MIL.M_TIMER1: break;
						case MIL.M_TIMER2: break;
						case MIL.M_TIMER3: break;
						case MIL.M_TIMER4: break;
						case MIL.M_TIMER5: break;
						case MIL.M_TIMER6: break;
						case MIL.M_TIMER7: break;
						case MIL.M_TIMER8: break;
						case MIL.M_TL_TRIGGER: break;  // Specifies to use the transport layer trigger signal.
					}
					return TriggerSourceEnum.M_SOFTWARE;
				}
				catch ( Exception ex ) { throw new Exception( "GrabTriggerSource(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					switch ( value )
					{
						case TriggerSourceEnum.M_AUX_IO0:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_SOURCE, MIL.M_AUX_IO0 );
							break;
						case TriggerSourceEnum.M_AUX_IO1:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_SOURCE, MIL.M_AUX_IO1 );
							break;
						case TriggerSourceEnum.M_SOFTWARE:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_SOURCE, MIL.M_SOFTWARE );
							break;
						default:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_SOURCE, MIL.M_SOFTWARE );
							break;
					}
				}
				catch ( Exception ex ) { throw new Exception( "GrabTriggerSource(SET) error: " + ex.Message ); }
			}
		}

		/// <summary>
		/// GrabTriggerActivation from framegrabber Rapixo
		/// M_DEFAULT, M_EDGE_FALLING, M_EDGE_RISING, M_LEVEL_HIGH, M_LEVEL_LOW
		/// </summary>
		public TriggerActivationEnum GrabTriggerActivation
		{
			get
			{
				try
				{
					MIL_INT GrabTriggerActivation = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_TRIGGER_ACTIVATION, ref GrabTriggerActivation );
					switch ( ( int )GrabTriggerActivation )
					{
						case MIL.M_DEFAULT: return TriggerActivationEnum.M_DEFAULT;  // Same as the signal transition specified by the DCF.
						case MIL.M_EDGE_FALLING: return TriggerActivationEnum.M_EDGE_FALLING;  // Specifies that a trigger will be generated upon a high-to-low signal transition.
						case MIL.M_EDGE_RISING: return TriggerActivationEnum.M_EDGE_RISING;  // Specifies that a trigger will be generated upon a low-to-high signal transition.
						case MIL.M_LEVEL_HIGH: return TriggerActivationEnum.M_LEVEL_HIGH;  // Specifies that a trigger is continuously issued during a high signal polarity.
						case MIL.M_LEVEL_LOW: return TriggerActivationEnum.M_LEVEL_LOW;  // Specifies that a trigger is continuously issued during a low signal polarity.
						default: return TriggerActivationEnum.M_EDGE_RISING;
					}
				}
				catch ( Exception ex ) { throw new Exception( "GrabTriggerActivation(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					switch ( value )
					{
						case TriggerActivationEnum.M_DEFAULT:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_ACTIVATION, MIL.M_DEFAULT );
							break;  // Same as the signal transition specified by the DCF.
						case TriggerActivationEnum.M_EDGE_FALLING:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_ACTIVATION, MIL.M_EDGE_FALLING );
							break;   // Specifies that a trigger will be generated upon a high-to-low signal transition.
						case TriggerActivationEnum.M_EDGE_RISING:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_ACTIVATION, MIL.M_EDGE_RISING );
							break;// Specifies that a trigger will be generated upon a low-to-high signal transition.
						case TriggerActivationEnum.M_LEVEL_HIGH:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_ACTIVATION, MIL.M_LEVEL_HIGH );
							break;// Specifies that a trigger is continuously issued during a high signal polarity.
						case TriggerActivationEnum.M_LEVEL_LOW:
							MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_ACTIVATION, MIL.M_LEVEL_LOW );
							break;// Specifies that a trigger is continuously issued during a low signal polarity.
					}
				}
				catch ( Exception ex ) { throw new Exception( "GrabTriggerActivation(SET) error: " + ex.Message ); }
			}
		}

		/// <summary>
		/// GrabTriggerState for frame grabber state
		/// MIL.M_DEFAULT,MIL.M_DISABLE = false
		/// MIL.M_ENABLE = true
		/// </summary>
		public State GrabTriggerState
		{
			get
			{
				MIL_INT GrabTriggerState = 0;
				MIL.MdigInquire( this._digID, MIL.M_GRAB_TRIGGER_STATE, ref GrabTriggerState );
				if ( GrabTriggerState == MIL.M_DEFAULT ) // Same as the state specified by the DCF or, if none specified, M_DISABLE.
				{ return State.M_DEFAULT; }
				else if ( GrabTriggerState == MIL.M_DISABLE ) // Specifies that, when a grab command is issued, the grab occurs without waiting for a trigger.
				{ return State.M_DISABLE; }
				else if ( GrabTriggerState == MIL.M_ENABLE ) // Specifies that, when a grab command is issued, the grab waits for a trigger before occurring.
				{ return State.M_ENABLE; }
				return State.M_DEFAULT;
			}
			set
			{
				if ( State.M_ENABLE == value ) //M_ENABLE
				{ MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_STATE, MIL.M_ENABLE ); }// Specifies that, when a grab command is issued, the grab occurs without waiting for a trigger.
				else { MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_STATE, MIL.M_DISABLE ); } // M_DEFAULT and M_DISABLE
			}
		}

		public AdimecTriggerSource ATriggerSource
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "TriggerSource", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "TriggerSource", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					switch ( result.ToString() )
					{
						case "Trigger": return AdimecTriggerSource.Trigger;
						case "IO_Connector": return AdimecTriggerSource.IO_Connector;
					}
					return AdimecTriggerSource.Unknown;
				}
				catch ( Exception ex ) { throw new Exception( "AdimecTriggerSource(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					switch ( value )
					{
						case AdimecTriggerSource.Trigger:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, "Trigger" );
							break;
						case AdimecTriggerSource.IO_Connector:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, "IO_Connector" );
							break;
						default:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, "IO_Connector" );
							break;
					}
				}
				catch ( Exception ex ) { throw new Exception( "AdimecTriggerSource(SET) error: " + ex.Message ); }
			}
		}

		public AdimecTriggerActivation ATriggerActivation
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "TriggerActivation", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "TriggerActivation", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					switch ( result.ToString() )
					{
						case "FallingEdge": return AdimecTriggerActivation.FallingEdge;
						case "RisingEdge": return AdimecTriggerActivation.RisingEdge;
					}
					return AdimecTriggerActivation.RisingEdge;
				}
				catch ( Exception ex ) { throw new Exception( "ATriggerActivation(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					switch ( value )
					{
						case AdimecTriggerActivation.FallingEdge:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerActivation", MIL.M_TYPE_STRING, "FallingEdge" );
							break;
						case AdimecTriggerActivation.RisingEdge:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerActivation", MIL.M_TYPE_STRING, "RisingEdge" );
							break;
						default:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerActivation", MIL.M_TYPE_STRING, "RisingEdge" );
							break;
					}
				}
				catch ( Exception ex ) { throw new Exception( "ATriggerActivation(SET) error: " + ex.Message ); }
			}
		}

		public AdimecExposureMode AExposureMode
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "ExposureMode", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "ExposureMode", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					switch ( result.ToString() )
					{
						case "Timed": return AdimecExposureMode.Timed;
						case "TriggerWidth": return AdimecExposureMode.TriggerWidth;
						case "SyncControlMode": return AdimecExposureMode.SyncControlMode;
						case "TimedTriggerControl": return AdimecExposureMode.TimedTriggerControl;
					}
					return AdimecExposureMode.Timed;
				}
				catch ( Exception ex ) { throw new Exception( "AExposureMode(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					switch ( value )
					{
						case AdimecExposureMode.Timed:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "ExposureMode", MIL.M_TYPE_STRING, "Timed" );
							break;
						case AdimecExposureMode.TriggerWidth:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "ExposureMode", MIL.M_TYPE_STRING, "TriggerWidth" );
							break;
						case AdimecExposureMode.SyncControlMode:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "ExposureMode", MIL.M_TYPE_STRING, "SyncControlMode" );
							break;
						case AdimecExposureMode.TimedTriggerControl:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "ExposureMode", MIL.M_TYPE_STRING, "TimedTriggerControl" );
							break;
						default:
							MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "ExposureMode", MIL.M_TYPE_STRING, "Timed" );
							break;
					}
				}
				catch ( Exception ex ) { throw new Exception( "AExposureMode(SET) error: " + ex.Message ); }
			}
		}

#endif

		public string TriggerMode
		{
			get
			{
				try
				{
					long length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "TriggerMode", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "TriggerMode", MIL.M_TYPE_STRING, result );

					return result.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "TriggerMode(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerMode", MIL.M_TYPE_STRING, value );
				}
				catch ( Exception ex ) { throw new Exception( "TriggerMode(SET) error: " + ex.Message ); }
			}
		}

		public string TriggerSource
		{
			get
			{
				try
				{
					MIL_INT length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "TriggerSource", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					if ( length > 0 )
					{
						MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "TriggerSource", MIL.M_TYPE_STRING + MIL.M_FEATURE_USER_ARRAY_SIZE( length ), result );
					}
					return result.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "TriggerSource(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerSource", MIL.M_TYPE_STRING, value );
				}
				catch ( Exception ex ) { throw new Exception( "TriggerSource(SET) error: " + ex.Message ); }
			}
		}

		public string TriggerActivation
		{
			get
			{
				try
				{
					long length = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE + MIL.M_STRING_SIZE, "TriggerActivation", MIL.M_TYPE_MIL_INT, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE_AS_STRING, "TriggerActivation", MIL.M_TYPE_STRING, result );

					return result.ToString();
				}
				catch ( Exception ex ) { throw new Exception( "TriggerActivation(GET) error: " + ex.Message ); }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "TriggerActivation", MIL.M_TYPE_STRING, value );
				}
				catch ( Exception ex ) { throw new Exception( "TriggerActivation(SET) error: " + ex.Message ); }
			}
		}

		public double Gain
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "Gain", MIL.M_TYPE_DOUBLE, ref value );
				}
				catch ( Exception ex ) { throw new Exception( "Gain(SET) error: " + ex.Message ); }
			}
		}

		public double GainMinimum
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MIN, "Gain", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public double GainMaximum
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MAX, "Gain", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public double ExposureTime
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "ExposureTime", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "ExposureTime", MIL.M_TYPE_DOUBLE, ref value );
				}
				catch ( Exception ex ) { throw new Exception( "ExposureTime(SET) error: " + ex.Message ); }
			}
		}

		public double ExposureTimeMinimum
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MIN, "ExposureTime", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public double ExposureTimeMaximum
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MAX, "ExposureTime", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public long ExposureTimeIncrement
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_INCREMENT, "ExposureTime", MIL.M_TYPE_INT64, ref result );
					return result;
				}
				catch { return 1; }
			}
		}

		public bool GammaEnable
		{
			get
			{
				try
				{
					bool result = false;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "GammaEnable", MIL.M_TYPE_BOOLEAN, ref result );
					return result;
				}
				catch { return false; }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "GammaEnable", MIL.M_TYPE_BOOLEAN, ref value );
				}
				catch ( Exception ex ) { throw new Exception( "GammaEnable(SET) error: " + ex.Message ); }
			}
		}

		public double Gamma
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "Gamma", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "Gamma", MIL.M_TYPE_DOUBLE, ref value );
				}
				catch ( Exception ex ) { throw new Exception( "Gamma(SET) error: " + ex.Message ); }
			}
		}

		public double GammaMinimum
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MIN, "Gamma", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public double GammaMaximum
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_MAX, "Gamma", MIL.M_TYPE_DOUBLE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public long GammaIncrement
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_INCREMENT, "Gamma", MIL.M_TYPE_INT64, ref result );
					return result;
				}
				catch { return 1; }
			}
		}

		public bool ReverseX
		{
			get
			{
				try
				{
					bool _revX = false;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "ReverseX", MIL.M_TYPE_BOOLEAN, ref _revX );
					return _revX;
				}
				catch { return false; }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "ReverseX", MIL.M_TYPE_BOOLEAN, ref value );
				}
				catch ( Exception ex ) { throw new Exception( "ReverseX(SET) error: " + ex.Message ); }
			}
		}

		public bool ReverseY
		{
			get
			{
				try
				{
					bool _revY = false;
					MIL.MdigInquireFeature( this._digID, MIL.M_FEATURE_VALUE, "ReverseY", MIL.M_TYPE_BOOLEAN, ref _revY );
					return _revY;
				}
				catch { return false; }
			}
			set
			{
				try
				{
					MIL.MdigControlFeature( this._digID, MIL.M_FEATURE_VALUE, "ReverseY", MIL.M_TYPE_BOOLEAN, ref value );
				}
				catch ( Exception ex ) { throw new Exception( "ReverseY(SET) error: " + ex.Message ); }
			}
		}

		#region Matrox Digitizer Settings Access

		public string M_CameraVendor
		{
			get
			{
				try
				{
					long length = 0;
					MIL.MdigInquire( this._digID, MIL.M_CAMERA_VENDOR_SIZE, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					MIL.MdigInquire( this._digID, MIL.M_CAMERA_VENDOR, result );

					return result.ToString();
				}
				catch { return "NA"; }
			}
		}

		public string M_CameraModel
		{
			get
			{
				try
				{
					long length = 0;
					MIL.MdigInquire( this._digID, MIL.M_CAMERA_MODEL_SIZE, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					MIL.MdigInquire( this._digID, MIL.M_CAMERA_MODEL, result );

					return result.ToString();
				}
				catch { return "NA"; }
			}
		}

		public string M_UserName
		{
			get
			{
				try
				{
					long length = 0;
					MIL.MdigInquire( this._digID, MIL.M_GC_USER_NAME_SIZE, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					MIL.MdigInquire( this._digID, MIL.M_GC_USER_NAME, result );

					return result.ToString();
				}
				catch { return "NA"; }
			}
		}

		public string M_SerialNumber
		{
			get
			{
				try
				{
					long length = 0;
					MIL.MdigInquire( this._digID, MIL.M_GC_SERIAL_NUMBER_SIZE, ref length );
					StringBuilder result = new StringBuilder( ( int )length );
					MIL.MdigInquire( this._digID, MIL.M_GC_SERIAL_NUMBER, result );

					return result.ToString();
				}
				catch { return "NA"; }
			}
		}

		public double M_FrameRate
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SELECTED_FRAME_RATE, ref result );
					return result;
				}
				catch { return 0; }
			}
		}

		/// <summary>
		/// Exposure time in NANO seconds
		/// </summary>
		public double M_Exposure
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquire( this._digID, MIL.M_EXPOSURE_TIME, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_EXPOSURE_TIME, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_Exposure(SET) error: " + ex.Message ); }
			}
		}

		public double M_ExposureMin
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquire( this._digID, MIL.M_EXPOSURE_TIME + MIL.M_MIN_VALUE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public double M_ExposureMax
		{
			get
			{
				try
				{
					double result = 0;
					MIL.MdigInquire( this._digID, MIL.M_EXPOSURE_TIME + MIL.M_MAX_VALUE, ref result );
					return result;
				}
				catch { return -1; }
			}
		}

		public double M_Gain
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_GAIN, ref result );
					return ( double )result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_GAIN, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_GAIN(SET) error: " + ex.Message ); }
			}
		}

		public double M_GainMin
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_GAIN + MIL.M_MIN_VALUE, ref result );
					return ( double )result;
				}
				catch { return -1; }
			}
		}

		public double M_GainMax
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_GAIN + MIL.M_MAX_VALUE, ref result );
					return ( double )result;
				}
				catch { return -1; }
			}
		}

		public string M_PixelFormat
		{
			get
			{
				try
				{
					MIL_INT pixFormat = 0;
					string result = string.Empty;
					MIL.MdigInquire( this._digID, MIL.M_GC_PIXEL_FORMAT, ref pixFormat );
					switch ( ( int )pixFormat )
					{
						case 17301505: result = "Mono8"; break;
						case 17825795: result = "Mono10"; break;
						case 17825797: result = "Mono12"; break;
						case 17563652: result = "Mono10Packed"; break;
						case 17563654: result = "Mono12Packed"; break;
						default: result = "Undefined"; break;
					}
					return result;
				}
				catch { return ""; }
			}
			set
			{
				try
				{
					MIL_INT newPxFormat = 17301505; //Mono8
					switch ( value )
					{
						case "Mono8": newPxFormat = 17301505; break;
						case "Mono10": newPxFormat = 17825795; break;
						case "Mono12": newPxFormat = 17825797; break;
						case "Mono10Packed": newPxFormat = 17563652; break;
						case "Mono12Packed": newPxFormat = 17563654; break;
						default: newPxFormat = 17301505; break;
					}
					MIL.MdigControl( this._digID, MIL.M_GC_PIXEL_FORMAT, newPxFormat );
				}
				catch ( Exception ex ) { throw new Exception( "M_GC_PIXEL_FORMAT(SET) error: " + ex.Message ); }
			}
		}

		public double M_SizeX
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SIZE_X, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_SIZE_X, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_SIZE_X(SET) error: " + ex.Message ); }
			}
		}

		public int M_SizeXMin
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SIZE_X + MIL.M_MIN_VALUE, ref result );
					return ( int )result;
				}
				catch { return -1; }
			}
		}

		public int M_SizeXMax
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SIZE_X + MIL.M_MAX_VALUE, ref result );
					return ( int )result;
				}
				catch { return -1; }
			}
		}

		public double M_SizeY
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SIZE_Y, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_SIZE_Y, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_SIZE_Y(SET) error: " + ex.Message ); }
			}
		}

		public int M_SizeYMin
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SIZE_Y + MIL.M_MIN_VALUE, ref result );
					return ( int )result;
				}
				catch { return -1; }
			}
		}

		public int M_SizeYMax
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SIZE_Y + MIL.M_MAX_VALUE, ref result );
					return ( int )result;
				}
				catch { return -1; }
			}
		}

		public double M_SourceSizeX
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SOURCE_SIZE_X, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_SOURCE_SIZE_X, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_SOURCE_SIZE_X(SET) error: " + ex.Message ); }
			}
		}

		public double M_SourceSizeY
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SOURCE_SIZE_Y, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_SOURCE_SIZE_Y, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_SOURCE_SIZE_Y(SET) error: " + ex.Message ); }
			}
		}

		public int M_SOURCEOFFSETX
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SOURCE_OFFSET_X, ref result );
					return ( int )result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_SOURCE_OFFSET_X, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_SOURCE_OFFSET_X (SET) error: " + ex.Message ); }
			}
		}

		public int M_SOURCEOFFSETY
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquire( this._digID, MIL.M_SOURCE_OFFSET_Y, ref result );
					return ( int )result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_SOURCE_OFFSET_Y, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_SOURCE_OFFSET_X (SET) error: " + ex.Message ); }
			}
		}

		public int M_GRABDIRECTIONX
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_DIRECTION_X, ref result );
					return ( int )result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_GRAB_DIRECTION_X, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_GRAB_DIRECTION_X (SET) error: " + ex.Message ); }
			}
		}

		public int M_GrabDirectionY
		{
			get
			{
				try
				{
					MIL_INT result = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_DIRECTION_Y, ref result );
					return ( int )result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_GRAB_DIRECTION_Y, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_GRAB_DIRECTION_Y (SET) error: " + ex.Message ); }
			}
		}

		public bool M_GrabContinuousEnabled
		{
			get
			{
				try
				{
					bool result = true;
					MIL_INT iRes = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_CONTINUOUS_END_TRIGGER, ref iRes );
					if ( MIL.M_DISABLE == iRes ) result = false;

					return result;
				}
				catch { return true; }
			}
			set
			{
				try
				{
					MIL_INT inputValue = MIL.M_ENABLE;
					if ( value == false ) inputValue = MIL.M_DISABLE;
					MIL.MdigControl( this._digID, MIL.M_GRAB_CONTINUOUS_END_TRIGGER, inputValue );
				}
				catch ( Exception ex ) { throw new Exception( "M_GRAB_CONTINUOUS_END_TRIGGER (SET) error: " + ex.Message ); }
			}
		}

		public TriggerActivationEnum M_GrabTriggerActivation
		{
			get
			{
				try
				{
					TriggerActivationEnum result = TriggerActivationEnum.M_DEFAULT;
					MIL_INT iRes = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_TRIGGER_ACTIVATION, ref iRes );
					switch ( ( int )iRes )
					{

						case MIL.M_EDGE_FALLING: result = TriggerActivationEnum.M_EDGE_FALLING; break;
						case MIL.M_EDGE_RISING: result = TriggerActivationEnum.M_EDGE_RISING; break;
						case MIL.M_LEVEL_HIGH: result = TriggerActivationEnum.M_LEVEL_HIGH; break;
						case MIL.M_LEVEL_LOW: result = TriggerActivationEnum.M_LEVEL_LOW; break;
						case MIL.M_ANY_EDGE:
						default: result = TriggerActivationEnum.M_DEFAULT; break;
					}
					return result;
				}
				catch { return TriggerActivationEnum.M_DEFAULT; }
			}
			set
			{
				try
				{
					MIL_INT inputValue = MIL.M_ANY_EDGE;
					switch ( value )
					{
						case TriggerActivationEnum.M_EDGE_FALLING: inputValue = MIL.M_EDGE_FALLING; break;
						case TriggerActivationEnum.M_EDGE_RISING: inputValue = MIL.M_EDGE_RISING; break;
						case TriggerActivationEnum.M_LEVEL_HIGH: inputValue = MIL.M_LEVEL_HIGH; break;
						case TriggerActivationEnum.M_LEVEL_LOW: inputValue = MIL.M_LEVEL_LOW; break;
					}
					MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_ACTIVATION, inputValue );
				}
				catch ( Exception ex ) { throw new Exception( "M_GRAB_TRIGGER_ACTIVATION (SET) error: " + ex.Message ); }
			}
		}

		public double M_GrabTriggerDelay
		{
			get
			{
				try
				{
					long result = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_TRIGGER_DELAY, ref result );
					return result;
				}
				catch { return -1; }
			}
			set
			{
				try
				{
					MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_DELAY, value );
				}
				catch ( Exception ex ) { throw new Exception( "M_GRAB_TRIGGER_DELAY(SET) error: " + ex.Message ); }
			}
		}


		public bool M_GrabTriggerOverlap
		{
			get
			{
				try
				{
					bool result = false;
					MIL_INT iRes = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_TRIGGER_OVERLAP, ref iRes );
					if ( MIL.M_ENABLE == iRes ) result = true;

					return result;
				}
				catch { return false; }
			}
			set
			{
				try
				{
					MIL_INT inputValue = MIL.M_DISABLE;
					if ( value == true ) inputValue = MIL.M_ENABLE;
					MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_OVERLAP, inputValue );
				}
				catch ( Exception ex ) { throw new Exception( "M_GRAB_TRIGGER_OVERLAP (SET) error: " + ex.Message ); }
			}
		}

		public TriggerSourceEnum M_GrabTriggerSource
		{
			get
			{
				try
				{
					TriggerSourceEnum result = TriggerSourceEnum.M_DEFAULT;
					MIL_INT iRes = MIL.M_NULL;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_TRIGGER_SOURCE, ref iRes );
					switch ( ( int )iRes )
					{
						case MIL.M_AUX_IO0: result = TriggerSourceEnum.M_AUX_IO0; break;
						case MIL.M_AUX_IO1: result = TriggerSourceEnum.M_AUX_IO1; break;
						case MIL.M_AUX_IO2: result = TriggerSourceEnum.M_AUX_IO2; break;
						case MIL.M_TIMER0: result = TriggerSourceEnum.M_TIMER0; break;
						case MIL.M_SOFTWARE: result = TriggerSourceEnum.M_SOFTWARE; break;
						default: break;
					}
					return result;
				}
				catch { return TriggerSourceEnum.M_DEFAULT; }
			}
			set
			{
				try
				{
					MIL_INT inputValue = MIL.M_SOFTWARE;
					switch ( value )
					{
						case TriggerSourceEnum.M_AUX_IO0: inputValue = MIL.M_AUX_IO0; break;
						case TriggerSourceEnum.M_AUX_IO1: inputValue = MIL.M_AUX_IO1; break;
						case TriggerSourceEnum.M_AUX_IO2: inputValue = MIL.M_AUX_IO2; break;
						case TriggerSourceEnum.M_TIMER0: inputValue = MIL.M_TIMER0; break;
						case TriggerSourceEnum.M_SOFTWARE: inputValue = MIL.M_SOFTWARE; break;
						default: break;
					}
					MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_SOURCE, inputValue );
				}
				catch ( Exception ex ) { throw new Exception( "M_GRAB_TRIGGER_SOURCE(SET) error: " + ex.Message ); }
			}
		}


		public bool M_GrabTriggerState
		{
			get
			{
				try
				{
					bool result = false;
					MIL_INT iRes = 0;
					MIL.MdigInquire( this._digID, MIL.M_GRAB_TRIGGER_STATE, ref iRes );
					if ( MIL.M_ENABLE == iRes ) result = true;

					return result;
				}
				catch { return false; }
			}
			set
			{
				try
				{
					MIL_INT inputValue = MIL.M_DISABLE;
					if ( value == true ) inputValue = MIL.M_ENABLE;
					MIL.MdigControl( this._digID, MIL.M_GRAB_TRIGGER_STATE, inputValue );
				}
				catch ( Exception ex ) { throw new Exception( "M_GRAB_TRIGGER_STATE (SET) error: " + ex.Message ); }
			}
		}

		#endregion

	}
}
