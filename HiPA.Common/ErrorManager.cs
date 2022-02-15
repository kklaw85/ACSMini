using HiPA.Common.Forms;
using HiPA.Common.Report;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HiPA.Common
{
	[Serializable]
	public enum AlarmGrade
	{
		Message,
		Warning,
		Error,
		Fatal,
	}
	[Serializable]
	public enum ErrorClass
	{
		OK,
		E1,//a note to user, no need action, proceeding to work. not an error
		E2,//an error has occur, no need action, proceeding to work on same dut
		E3,//an error has occur, no need action, skip to next dut
		E4,//an error has occur, need human intervention to check settings, machine stop
		E5,//an error has occur, need human intervention to check hardware, machine stop
		E6,//an error has occur, unclassified error, machine stop
		F,//speak to manufacturer
	}

	public class ErrorResult : BaseUtility
	{
		public ErrorClass EClass
		{
			get => this.GetValue( () => this.EClass );
			set => this.SetValue( () => this.EClass, value );
		}
		public string ErrorMessage
		{
			get => this.GetValue( () => this.ErrorMessage );
			set => this.SetValue( () => this.ErrorMessage, value );
		}
		public void Reset()
		{
			this.EClass = ErrorClass.OK;
			this.ErrorMessage = string.Empty;
		}
		public ErrorResult()
		{
			this.EClass = ErrorClass.OK;
			this.ErrorMessage = string.Empty;
		}
		public void Set( ErrorClass EClass, string ErrorMessage )
		{
			this.EClass = EClass;
			this.ErrorMessage = ErrorMessage;
		}
		public void Set( ErrorResult Source )
		{
			this.EClass = Source.EClass;
			this.ErrorMessage = Source.ErrorMessage;
		}
	}

	[Serializable]
	public class Alarm : BaseUtility
	{
		public DateTime TimeStamp
		{
			get => this.GetValue( () => this.TimeStamp );
			set
			{
				this.SetValue( () => this.TimeStamp, value );
				this.sTimeStamp = this.TimeStamp.ToString( "MM/dd/yyyy hh:mm:ss tt" );
			}
		}
		public string sTimeStamp
		{
			get => this.GetValue( () => this.sTimeStamp );
			private set => this.SetValue( () => this.sTimeStamp, value );
		}
		public AlarmGrade Grade
		{
			get => this.GetValue( () => this.Grade );
			set
			{
				this.SetValue( () => this.Grade, value );
				this.sGrade = this.Grade.ToString();
			}
		}
		public string sGrade
		{
			get => this.GetValue( () => this.sGrade );
			private set => this.SetValue( () => this.sGrade, value );
		}
		public ErrorClass EClass
		{
			get => this.GetValue( () => this.EClass );
			set
			{
				this.SetValue( () => this.EClass, value );
				this.sEClass = this.EClass.ToString();
			}
		}
		public string sEClass
		{
			get => this.GetValue( () => this.sEClass );
			private set => this.SetValue( () => this.sEClass, value );
		}
		public int Code
		{
			get => this.GetValue( () => this.Code );
			set
			{
				this.SetValue( () => this.Code, value );
				this.sCode = this.Code.ToString();
			}
		}
		public string sCode
		{
			get => this.GetValue( () => this.sCode );
			private set => this.SetValue( () => this.sCode, value );
		}
		public string Type
		{
			get => this.GetValue( () => this.Type );
			set => this.SetValue( () => this.Type, value );
		}
		public string Message
		{
			get => this.GetValue( () => this.Message );
			set => this.SetValue( () => this.Message, value );
		}

		[NonSerialized]
		[System.Xml.Serialization.XmlIgnore]
		public readonly InstrumentBase Source;

		[NonSerialized]
		[System.Xml.Serialization.XmlIgnore]
		public readonly AlarmAgent Agent;

		public Alarm( InstrumentBase source, string type, string message, AlarmGrade grade, ErrorClass errorClass )
		{
			this.TimeStamp = DateTime.Now;
			this.Source = source;
			this.Type = type;
			this.Grade = grade;
			this.EClass = errorClass;
			this.Message = message;
		}
		public override string ToString()
		{
			return $"[{this.TimeStamp}] Alarm: Message[{this.Message}], Title[{this.Type}], Source[{this.Source}], Code[{this.Code}]";
		}
		public override int GetHashCode()
		{
			return this.Type.GetHashCode();
		}
		public override bool Equals( object obj )
		{
			if ( obj is Alarm alarm ) return alarm.Type == this.Type;
			if ( obj is string title ) return title == this.Type;
			return false;
		}

		public Alarm()
		{
		}
		public void Emit()
		{
			this.Agent.EmitAlarm( this );
		}
		//public void Clear()
		//{
		//	this.Agent.Clear( this );
		//}
		public void Clear()
		{
			this.sTimeStamp = string.Empty;
			this.Type = string.Empty;
			this.sGrade = string.Empty;
			this.sEClass = string.Empty;
			this.sCode = string.Empty;
			this.Message = string.Empty;
		}
		public void Copy( Alarm source )
		{
			this.TimeStamp = source.TimeStamp;
			this.Type = source.Type;
			this.Grade = source.Grade;
			this.EClass = source.EClass;
			this.Code = source.Code;
			this.Message = source.Message;
		}
	}
	public class AlarmAgent
		: IDisposable
	{
		public List<Alarm> EmittedAlarm { get; set; } = new List<Alarm>();

		public event EventHandler<Alarm> BeforeEmitActionEvent;
		public event EventHandler<Alarm> MessageAction;
		public event EventHandler<Alarm> WarningAction;
		public event EventHandler<Alarm> ErrorAction;
		public event EventHandler<Alarm> FatalAction;

		public bool IsEmitted( Alarm alarm ) => this.EmittedAlarm.Contains( alarm );


		public void EmitAlarm( Alarm alarm )
		{
			try
			{
				this.BeforeEmitActionEvent?.Invoke( this, alarm );

				switch ( alarm.Grade )
				{
					case AlarmGrade.Message: this.MessageAction?.Invoke( this, alarm ); break;
					case AlarmGrade.Warning: this.WarningAction?.Invoke( this, alarm ); break;
					case AlarmGrade.Error: this.ErrorAction?.Invoke( this, alarm ); break;
					case AlarmGrade.Fatal: this.FatalAction?.Invoke( this, alarm ); break;
				}

				this.EmittedAlarm.Add( alarm );
				Task.Run( () =>
				{
					Equipment.ErrManager.AlarmReceiver?.Invoke( alarm );
					Equipment.ErrManager.AlarmLog?.Invoke( this, alarm );
				} );
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( $"ErrorManager.cs :EmitAlarm:{ex.Message}" ) ).Start();
			}
		}

		public void Clear( Alarm alarm )
		{
			this.EmittedAlarm.Remove( alarm );
		}

		public bool IsDisposed { get; set; } = false;
		public void Dispose()
		{
			if ( this.IsDisposed == false )
			{
				this.EmittedAlarm.Clear();
				this.IsDisposed = true;
			}
		}
	}
	public class ErrorManager : BaseUtility
	{
		public ErrorManager()
		{
		}
		public ObservableCollection<Alarm> AlarmList = new ObservableCollection<Alarm>();
		public int AlarmIdx
		{
			get => this.GetValue( () => this.AlarmIdx );
			set
			{
				if ( this.AlarmList.Count < value || value == 0 )
				{
					this.SetValue( () => this.AlarmIdx, 0 );
					this.CurrentAlarm.Clear();
				}
				else
				{
					this.SetValue( () => this.AlarmIdx, value );
					this.CurrentAlarm.Copy( this.AlarmList[ this.AlarmIdx - 1 ] );
				}

			}
		}
		public Alarm CurrentAlarm { get; private set; } = new Alarm();
		public Action<Alarm> AlarmReceiver = null;
		public Action<AlarmAgent, Alarm> AlarmLog = null;
		private LoggerHelper ENGAlarmErrLog = new LoggerHelper( "ENGAlarmErrLog", "Alarm" );
		private LoggerHelper AlarmErrLog = new LoggerHelper( "AlarmErrLog", "Alarm" );
		private LoggerHelper AlarmWarnLog = new LoggerHelper( "AlarmWarnLog", "Alarm" );
		private LoggerHelper AlarmShowLog = new LoggerHelper( "AlarmShowLog", "Alarm" );


		public string RaiseError( InstrumentBase source, string message, string title, ErrorClass errorClass, Exception exception = null )
		{
			var agent = source?.GetAlarmAgent();
			var DecodedMsg = this.DecodeMsg( message );
			var alarm = new Alarm( source, title, DecodedMsg[ 0 ], AlarmGrade.Error, errorClass );
			if ( agent != null ) agent.EmitAlarm( alarm );
			else Task.Run( () => this.AlarmReceiver?.Invoke( alarm ) );
			if ( message != "Ready" )
			{
				this.TrigWindowInfo( DecodedMsg[ 0 ], AlarmGrade.Error );
				this.ENGAlarmErrLog.WriteLog( $"[RaiseError]: {DecodedMsg[ 1 ]}" );
				this.AlarmErrLog.WriteLog( DecodedMsg[ 0 ] );
				Application.Current.Dispatcher.Invoke( ( Action )delegate // <--- HERE
				{
					this.AlarmList.Add( alarm );
					this.AlarmIdx = this.AlarmList.Count;
				} );
			}
			return message;
		}
		public string RaiseError( string message, string title, ErrorClass errorClass, Exception exception = null )
		{
			var DecodedMsg = this.DecodeMsg( message );
			var alarm = new Alarm( null, title, DecodedMsg[ 0 ], AlarmGrade.Error, errorClass );
			Task.Run( () => this.AlarmReceiver?.Invoke( alarm ) );
			if ( message != "Ready" )
			{
				this.TrigWindowInfo( DecodedMsg[ 0 ], AlarmGrade.Error );
				this.ENGAlarmErrLog.WriteLog( $"[RaiseError]: {DecodedMsg[ 1 ]}" );
				this.AlarmErrLog.WriteLog( DecodedMsg[ 0 ] );
				Application.Current.Dispatcher.Invoke( ( Action )delegate // <--- HERE
				{
					this.AlarmList.Add( alarm );
					this.AlarmIdx = this.AlarmList.Count;
				} );
			}
			return message;
		}
		public string RaiseWarning( InstrumentBase source, string message, string title )
		{
			var DecodedMsg = this.DecodeMsg( message );
			var alarm = new Alarm( source, title, DecodedMsg[ 0 ], AlarmGrade.Warning, ErrorClass.E2 );
			Task.Run( () => this.AlarmReceiver?.Invoke( alarm ) );
			if ( message != "Ready" )
			{
				this.TrigWindowInfo( DecodedMsg[ 0 ], AlarmGrade.Warning );
				this.ENGAlarmErrLog.WriteLog( $"[RaiseWarning]: {DecodedMsg[ 1 ]}" );
				this.AlarmWarnLog.WriteLog( DecodedMsg[ 0 ] );
				Application.Current.Dispatcher.Invoke( ( Action )delegate // <--- HERE
				{
					this.AlarmList.Add( alarm );
					this.AlarmIdx = this.AlarmList.Count;
				} );
			}
			return message;
		}
		public string RaiseWarning( string message, string title )
		{
			var DecodedMsg = this.DecodeMsg( message );
			var alarm = new Alarm( null, title, DecodedMsg[ 0 ], AlarmGrade.Warning, ErrorClass.E2 );
			Task.Run( () => this.AlarmReceiver?.Invoke( alarm ) );
			if ( message != "Ready" )
			{
				this.TrigWindowInfo( DecodedMsg[ 0 ], AlarmGrade.Warning );
				this.ENGAlarmErrLog.WriteLog( $"[RaiseWarning]: {DecodedMsg[ 1 ]}" );
				this.AlarmWarnLog.WriteLog( DecodedMsg[ 0 ] );
				Application.Current.Dispatcher.Invoke( ( Action )delegate // <--- HERE
				{
					this.AlarmList.Add( alarm );
					this.AlarmIdx = this.AlarmList.Count;
				} );
			}
			return message;
		}
		public void ShowMessage( string message, string title )
		{
			var DecodedMsg = this.DecodeMsg( message );
			Task.Run( () => this.AlarmReceiver?.Invoke( new Alarm( null, title, DecodedMsg[ 0 ], AlarmGrade.Message, ErrorClass.OK ) ) );
			if ( message != "Ready" )
			{
				this.TrigWindowInfo( DecodedMsg[ 0 ], AlarmGrade.Message );
				this.ENGAlarmErrLog.WriteLog( $"[ShowMessage]: {DecodedMsg[ 1 ]}" );
				this.AlarmShowLog.WriteLog( DecodedMsg[ 0 ] );
			}
		}
		private void TrigWindowInfo( string msg, AlarmGrade alrmgrade )
		{
			this.AlrmGrade = alrmgrade;
			this.Event_Err_Msg = msg;
		}
		private string[] DecodeMsg( string message )
		{
			//[0] = Selected Language, [1] = Eng
			var returnMsg = new string[] { "", "" };
			try
			{
				int msgIndex = 0;
				var msgParts = message.Split( '^' );
				var msgEng = string.Empty;
				var msg = string.Empty;
				bool isNumeric = false;
				if ( msgParts.Length == 1 )
				{
					//[0] = Selected Language, [1] = Eng
					returnMsg[ 0 ] = message;
					returnMsg[ 1 ] = message;
				}
				else
				{
					foreach ( var part in msgParts )
					{
						isNumeric = int.TryParse( part, out msgIndex );
						if ( isNumeric )
						{
							if ( MultilingualErrModule.RuntimeErrENGList.ContainsKey( msgIndex ) &&
								MultilingualErrModule.RuntimeErrList.ContainsKey( msgIndex ) )
							{
								msg += $"[ {Math.Abs( msgIndex )} ] " + MultilingualErrModule.RuntimeErrList[ msgIndex ];
								msgEng += $"[ {Math.Abs( msgIndex )} ] " + MultilingualErrModule.RuntimeErrENGList[ msgIndex ];
							}
							else
							{
								msg += $"[ {Math.Abs( msgIndex )} ] ";
								msgEng += $"[ {Math.Abs( msgIndex )} ] ";
							}
						}
						else
						{
							msg += part;
							msgEng += part;
						}
					}

					//[0] = Selected Language, [1] = Eng
					returnMsg[ 0 ] = msg;
					returnMsg[ 1 ] = msgEng;
				}
			}
			catch ( Exception ex )
			{
				returnMsg = new string[] { message, message + $",[Manufacturer Take Note!] {ex.Message}" };
			}
			return returnMsg;
		}
		public void ClearAlarm()
		{
			try
			{
				#region Clear Existing Binded Informations
				this.AlarmList.Clear();
				this.CurrentAlarm.Clear();
				this.AlarmIdx = 0;
				this.TrigWindowInfo( "", AlarmGrade.Message );
				#endregion
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( ex.Message ) ).Start();
			}
		}
		public void SelectErr( int Idx )
		{
			try
			{
				#region Clear Existing Binded Informations
				this.AlarmIdx = Idx + 1;
				#endregion
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( ex.Message ) ).Start();
			}
		}

		#region Window info bar
		public AlarmGrade AlrmGrade
		{
			get => this.GetValue( () => this.AlrmGrade );
			set
			{
				this.SetValue( () => this.AlrmGrade, value );
				switch ( this.AlrmGrade )
				{
					case AlarmGrade.Error:
						Equipment.MachStateMgr.MachineStatus = MachineStateType.ERROR;
						break;
					case AlarmGrade.Warning:
						Equipment.MachStateMgr.MachineStatus = MachineStateType.WARNING;
						break;
					case AlarmGrade.Message:
						Equipment.MachStateMgr.ClearAbnormalStatus();
						break;
				}
			}
		}
		public string Event_Err_Msg
		{
			get => this.GetValue( () => this.Event_Err_Msg );
			set => this.SetValue( () => this.Event_Err_Msg, value );
		}
		public StateBGColor StateBG
		{
			get => this.GetValue( () => this.StateBG );
			set
			{
				this.SetValue( () => this.StateBG, value );
				switch ( this.StateBG )
				{
					case StateBGColor.Ready:
						this.StateFG = StateFGColor.Message;
						break;
					case StateBGColor.Running:
						this.StateFG = StateFGColor.Warning;
						break;
					case StateBGColor.Pause:
						this.StateFG = StateFGColor.Warning;
						break;
					case StateBGColor.Warning:
						this.StateFG = StateFGColor.Warning;
						break;
					case StateBGColor.Error:
						this.StateFG = StateFGColor.Warning;
						break;
					case StateBGColor.Homing:
						this.StateFG = StateFGColor.Warning;
						break;
				}
			}
		}
		public StateFGColor StateFG
		{
			get => this.GetValue( () => this.StateFG );
			set => this.SetValue( () => this.StateFG, value );
		}
		#endregion
	}

	public class ErrorTitle
	{
		public static readonly string Ready = "Ready";  //For ShowMessage Clearing.
		public static readonly string UserLoginError = "User Login Error";
		public static readonly string InitializeFailure = "Instrument Initializing Failure";
		public static readonly string InvalidInstrumentState = "Invalid instrument state";
		public static readonly string InvalidServiceState = "Invalid service state";
		public static readonly string InvalidArgument = "Invalid arguments";
		public static readonly string NullObject = "Object is Null";
		public static readonly string InvalidOperation = "Invalid operation";
		public static readonly string NotFoundFile = "Not found file";
		public static readonly string KeyHasExists = "Key has not exits";
		public static readonly string OperationFailure = "Operation failure";
		public static readonly string UnhandledException = "Occurred Exception";
		public static readonly string VisionError = "Vision Error Occurred";
		public static readonly string ProgramFatal = "Have to Check the Program";
		public static readonly string EStop = "Emergency Stop Triggered";
		public static readonly string AirPressure = "Caution Air Pressure";
		public static readonly string TowerLightThreadBlinking = "TowerLight Blinking Thread Error";
		public static readonly string TowerLightBlinking = "TowerLight Blinking Thread Error";
		public static readonly string TowerLightUpdateStatus = "TowerLight Blinking Update Status Error";
		public static readonly string EzCadDisplay = "EzCad Display Img Failure";
		public static readonly string CamCalGenFile = "Camera Calibration Generation Failure";
		public static readonly string MultiLanguageErr = "Load selected Language Failure";
		public static readonly string ExportFileErr = "Export File Failure";
	}

	#region Sequence Errors
	public enum RunErrors
	{
		//0- no error
		//-1 to -100 for timeouts
		//-101 to -200 for thread inconformity error
		//-201 to -300 for input
		//-301 to -400 for output
		//-401 to -500 for cylinder
		//-501 to -600 for adlinkmotion
		//-601 to -700 for vision
		//-701 to -800 for servercom
		//-801 to -900 for conveyor
		//-901 to -1000 for heightsensor
		//-1001 to -1100 for laser marking
		//-1101 to -1200 for barcode
		//-1201 to -1300 for recipelink
		//-1301 to -1400 for image handling
		//-4001 to -5000 for checking of other thread error by Conveyorout.
		//-9001 to -10000 for autorunseq exception
		//-10001 to -20000 for laserhead exception
		//-20001 to -30000 for ConveyorIn exception
		//-30001 to -40000 for ConveyorOut exception
		//-40001 to -50000 for Smemainlet exception
		//-50001 to -60000 for Smemaoutlet exception
		//-60001 to -70000 for Common exception
		//-70001 to -80000 for Tray exception
		//-80001 to -90000 for Shopfloor exception
		//-10000001 to -20000000 B262 Vision Processing
		//-20000001 to -30000000 HiPA Common
		//-30000001 to -40000000 HiPA Instrument
		//-40000001 to -50000000 JPT Camera
		//-50000001 to -60000000 JPT Matrox System
		//-60000001 to -70000000 Neowise
		//....and other exception

		[Description( "No Error" )]
		ERR_NoError = ( 0 ),      //No Error	

		#region timeouts
		[Description( "Motion Timeout" )]
		ERR_MoveTimeout = ( -1 ),   // 

		[Description( "IO Timeout" )]
		ERR_IOTimeout = ( -2 ),   // 

		[Description( "Function Timeout" )]
		ERR_FunctionTimeout = ( -3 ),   // 
		#endregion
		#region Threadstatus inconformity
		[Description( "Thread status inconformity" )]
		ERR_Inconformity = ( -101 ),
		[Description( "Unexpected Exception Error" )]
		ERR_UnexpectedException = ( -102 ),
		//[Description( "Shop floor communication thread status inconformity" )]
		//ERR_Inconformity_ShopFloorSeq = ( -101 ),   //

		//[Description( "Laser head seq thread status inconformity" )]
		//ERR_Inconformity = ( -102 ),

		//[Description( "Conveyor Out thread status inconformity" )]
		//ERR_Inconformity_ConveyorOutSeq = ( -103 ),   // 

		//[Description( "Conveyor In thread status inconformity" )]
		//ERR_Inconformity_ConveyorInSeq = ( -104 ),   // 

		//[Description( "Smema thread status inconformity" )]
		//ERR_Inconformity_SmemaSeq = ( -105 ),   // 

		//[Description( "Tray thread status inconformity" )]
		//ERR_Inconformity_TraySeq = ( -106 ),
		#endregion
		#region Input failure
		[Description( "Read Adlink In Error" )]
		ERR_ReadInputIOErr = ( -201 ),   // 
		#endregion
		#region Output failure
		[Description( "Set Adlink Out Error" )]
		ERR_SetOutputIOErr = ( -301 ),   // 
		#endregion
		#region IO Motion
		[Description( "Cylinder Motion Error" )]
		ERR_CylinderMotionErr = ( -401 ),   // 
		[Description( "Cylinder Door Motion Error" )]
		ERR_CylinderDrMotionErr = ( -402 ),   // 
		[Description( "Cylinder Product Presser Motion Error" )]
		ERR_CylinderProdPressMotionErr = ( -403 ),   // 
		[Description( "Place exceeded Timeout Error. Please check if valve is stucked." )]
		ERR_PlaceExceededTimeoutErr = ( -404 ),   // 
		[Description( "Pick exceeded Timeout Error. Please check if vacuum pressure is sufficient." )]
		ERR_PickExceededTimeoutErr = ( -405 ),   // 
		#endregion
		#region StageIO
		[Description( "Clamper Cylinder Close Timeout Error. Please check if Material is on stage or air pressure is sufficient or sensor position is correct." )]
		ERR_ClampCloseMovementTimeoutErr = ( -451 ),   // 
		[Description( "Clamper Cylinder Open Timeout Error. Please check if air pressure is sufficient or sensor position is correct." )]
		ERR_ClampOpenMovementTimeoutErr = ( -452 ),   // 
		[Description( "Suction Release exceeded Timeout Error. Please check if valve is stucked or pressure release is sufficient." )]
		ERR_SuctionReleaseExceededTimeoutErr = ( -453 ),   // 
		[Description( "Suction Hold Timeout Error. Please check if vacuum pressure is sufficient or if there is leakage." )]
		ERR_SuctionHoldExceededTimeoutErr = ( -454 ),   // 




		#endregion
		#region adlink motion
		[Description( "Axis Move Error" )]
		ERR_AxisMoveErr = ( -501 ),   // 
									  //[Description( "Conveyor Outlet Start Slow Failed" )]
									  //ERR_ConveyorOutStartSlowFail = ( -3001 ),   // 
									  //[Description( "Conveyor Outlet Start Fast Failed" )]
									  //ERR_ConveyorOutStartFastFail = ( -3002 ),   // 
									  //[Description( "Conveyor Outlet Stop Failed" )]
									  //ERR_ConveyorOutStopFail = ( -3003 ),   // 
									  //[Description( "LiftZ Move To Standby Pos Failed" )]
									  //ERR_LiftZMoveToStandbyPosFailed = ( -3004 ),   // 
									  //[Description( "LiftZ Move To Work Pos Failed" )]
									  //ERR_LiftZMoveToWorkPosFailed = ( -3005 ),   // 
									  //[Description( "Laser Head Move XYZ Failed" )]
									  //ERR_LHMoveXYZFailed = ( -3006 ),   // 
									  //[Description( "Laser Head Move X Failed" )]
									  //ERR_LHMoveXFailed = ( -3007 ),   // 
									  //[Description( "Laser Head Move Y Failed" )]
									  //ERR_LHMoveYFailed = ( -3008 ),   // 
									  //[Description( "Laser Head Move Z Failed" )]
									  //ERR_LHMoveZFailed = ( -3009 ),   // 
		#endregion
		#region Vision
		[Description( "Camera Grab All error" )]
		ERR_CameraGrabAllError = ( -601 ),   // 
		[Description( "Orientation check error" )]
		ERR_VisionOrientationCheckErr = ( -602 ),
		[Description( "Orientation: Not Found" )]
		ERR_VisionOrientationNotFoundErr = ( -603 ),
		[Description( "Alignment check error" )]
		ERR_VisionAlignmentCheckErr = ( -604 ),
		[Description( "Incorrect Product Width" )]
		ERR_ProductWidthErr = ( -605 ),
		[Description( "Content Check Error" )]
		ERR_ContentChkErr = ( -606 ),
		[Description( "Exceeded Y Rotation Limit" )]
		ERR_ExceedRotYLimitErr = ( -607 ),
		[Description( "Incorrect Product Angle" )]
		ERR_ProductAngleErr = ( -608 ),
		[Description( "Clean Checking Error" )]
		ERR_ProductCleanCheckErr = ( -609 ),
		[Description( "Content Check Position Failed" )]
		ERR_ContentCheckPositionFail = ( -610 ),
		[Description( "Model Score Failed" )]
		ERR_ContentCheckModelScore = ( -611 ),
		[Description( "Model Not Found" )]
		ERR_ContentCheckNotFound = ( -612 ),
		[Description( "Content check with mixed failure" )]
		ERR_ContentCheckMixed = ( -613 ),
		#endregion
		#region ServerComm
		//ShopFloor Communication Seq.
		[Description( "Error occured in retrieving image from Shop Floor" )]
		ERR_ShopFloor_Retrieve_Img = ( -701 ),   // 
		[Description( "Error occured in retrieving Cavity No" )]
		ERR_ShopFloor_Retrieve_CavityNo = ( -702 ),   // 
		[Description( "Error occured in posting result to Shop Floor" )]
		ERR_ShopFloor_Post_Result = ( -703 ),   // 
		[Description( "No cavity data to engrave error" )]
		ERR_NoCavityDataToEngrave = ( -704 ),//only occur during simulation mode
		#endregion
		#region PNP
		[Description( "PNP not ready for autorun." )]
		ERR_PNPModuleNotReadyForAutorun = ( -801 ),   // 
		[Description( "PNP fail to pick up." )]
		ERR_PNPFailToPickUp = ( -802 ),   // 
		[Description( "PNP fail to place down." )]
		ERR_PNPFailToPlaceDown = ( -803 ),   // 
		[Description( "PNP move to KIV Pos Error." )]
		ERR_PNPMoveToKIVErr = ( -804 ),   // 
		[Description( "PNP move to NG Pos Error." )]
		ERR_PNPMoveToNGErr = ( -805 ),   // 
		[Description( "PNP move to Load Pos Error." )]
		ERR_PNPMoveToLoadErr = ( -806 ),   // 
		[Description( "PNP move to Pick Pos Error." )]
		ERR_PNPMoveToPickErr = ( -807 ),   // 
		[Description( "PNP move to Wait Pos Error." )]
		ERR_PNPMoveToWaitErr = ( -808 ),   // 
		#endregion
		#region Stage
		[Description( "Stage To Standby State Error" )]
		ERR_StageMoveToStandby = ( -901 ),   // 
		[Description( "Stage Holding substrate Error" )]
		ERR_StageHold = ( -902 ),   // 
		[Description( "Image acquisition Error" )]
		ERR_StageImageShot = ( -903 ),   //
		[Description( "Vision Processing Error" )]
		ERR_StageVisionProcessing = ( -904 ),   //
		[Description( "Stage Releasing substrate Error" )]
		ERR_StageRelease = ( -905 ),   //
		#endregion
		#region Lift
		[Description( "Lift To Standby State Error" )]
		ERR_LiftMoveToStandby = ( -1001 ),
		[Description( "Lift To Continuous State Error" )]
		ERR_LiftToCont = ( -1002 ),
		[Description( "Lift To Pick/Place Pos. Error" )]
		ERR_LiftToPickPlace = ( -1003 ),
		[Description( "Lift Pusher Error" )]
		ERR_LiftPusher = ( -1004 ),
		#endregion
		#region Barcode
		[Description( "Barcode Reader Error" )]
		ERR_BarCodeReaderErr = ( -1101 ),   // 
		#endregion
		#region Recipesetting
		[Description( "Cavity No. not found in Cavity/WorkPos Idx. link" )]
		ERR_CavityNoNotFound = ( -1201 ),
		[Description( "Recipe not found in Color/Product Type link" )]
		ERR_RecipeNotFound = ( -1202 ),
		#endregion
		#region Imagehandling
		[Description( "Image File Post Action Error" )]
		ERR_ImageFilePostActionErr = ( -1301 ),
		#endregion




		[Description( "Conveyor Inlet Contains Error" )]
		ERR_InletContainsErr = ( -4001 ),//extension, no need to mention
		[Description( "SMEMA Outlet Contains Error" )]
		ERR_SMEMAOutContainsErr = ( -4002 ),//extension, no need to mention
		[Description( "SMEMA Inlet Contains Error" )]
		ERR_SMEMAInContainsErr = ( -4003 ),//extension, no need to mention
		[Description( "ShopFloor Contains Error" )]
		ERR_ShopFloorContainsErr = ( -4004 ),//extension, no need to mention
		#region autoseq
		// Auto Seq
		[Description( "Wait for Start Button IO Error" )]
		ERR_StartBtnIOErr = ( -9001 ),
		[Description( "Set Door Lock IO Error" )]
		ERR_SetDoorLockErr = ( -9002 ),
		[Description( "Check Door Lock IO Error" )]
		ERR_CheckDoorLockErr = ( -9003 ),
		[Description( "Set All Station to Ready Error" )]
		ERR_SetAllStationReadyErr = ( -9004 ),
		[Description( "Bypass In Effect." )]
		ERR_BypassInEffectErr = ( -9005 ),
		#endregion
		#region laserheadseqexception
		//Laser Head Seq.
		[Description( "Waiting for Work Error" )]
		ERR_WaitForWorkErr = ( -10001 ),
		[Description( "Wait Lift in Pos. Error" )]
		ERR_WaitLiftInPosErr = ( -10002 ),
		[Description( "Error occured in checking height" )]
		ERR_ScanHeight = ( -10003 ),
		[Description( "Error occured in during vision check in Laser Head sequence" )]
		ERR_VisionCheckError = ( -10004 ),
		[Description( "Error occured while waiting door closed in laser head sequence" )]
		ERR_WaitDoorClosedError = ( -10005 ),
		[Description( "Do Marking Error" )]
		ERR_DoMarkingErr = ( -10006 ),
		[Description( "Wait For CC Done Error" )]
		ERR_WaitForCCDoneErr = ( -10007 ),
		[Description( "Do CC Error" )]
		ERR_DoCCErr = ( -10008 ),
		[Description( "Continue Next Cavity" )]
		ERR_ContinueNextCavityErr = ( -10009 ),
		[Description( "Wait For All Pallets" )]
		ERR_WaitForALLPalletsErr = ( -10010 ),
		[Description( "Shutter Door IO Error; Please check Shutter Door IO" )]
		ERR_ShutterDoorIOError = ( -10011 ),
		#endregion
		#region ConveyorInSeqexception
		//ConveyorInlet Seq
		[Description( "Inlet SMEMA Work Error" )]
		ERR_InletSMEMAWorkErr = ( -20002 ),
		[Description( "Inlet Check Run Conveyor Flag Error" )]
		ERR_InletCheckRunConveyorFlagErr = ( -20003 ),
		[Description( "Wait for Pallet Sensor 2 Error" )]
		ERR_WaitPalletSen2Err = ( -20004 ),
		[Description( "Bar Code Checking Error" )]
		ERR_BarCodeCheckErr = ( -20005 ),
		[Description( "Move to Pallet Sensor 1 Error" )]
		ERR_MovePalletSen1Err = ( -20006 ),
		[Description( "Check Pallet Sensor 1 Error" )]
		ERR_CheckPalletSen1Err = ( -20007 ),
		[Description( "Wait for Inlet SMEMA Ready Error" )]
		ERR_WaitForSMEMAReadyErr = ( -20008 ),
		[Description( "Inlet Check Pallet All Sensors Error" )]
		ERR_InletCheckPalletAllSenErr = ( -20009 ),
		#endregion
		#region ConveyorOutSeqexception
		[Description( "Outlet Check Status Error" )]
		ERR_OutletCheckStateErr = ( -30002 ),
		[Description( "Conveyor In Action Error" )]
		ERR_ConveyorInActionErr = ( -30003 ),
		[Description( "Wait For Pallet 1 Error" )]
		ERR_WaitForPallet1Err = ( -30004 ),
		[Description( "Retrieve Bar Code Error" )]
		ERR_RetrieveBarCodeErr = ( -30005 ),
		[Description( "Start Server Communication Error" )]
		ERR_StartServerCommErr = ( -30006 ),
		[Description( "Wait for Product Serial Error" )]
		ERR_WaitProdSNErr = ( -30007 ),
		[Description( "Lift Z Move to Working Pos. Error" )]
		ERR_ZWorkPosErr = ( -30008 ),
		[Description( "Wait For Mark Completion Error" )]
		ERR_WaitMarkCompleteErr = ( -30009 ),
		[Description( "ShopFloor Send Result Error" )]
		ERR_ShopFloorSendResultErr = ( -30010 ),
		[Description( "Lift Z Move to Standby Pos. Error" )]
		ERR_ZStandbyPosErr = ( -30011 ),
		[Description( "Outlet Check Pallet All Sensors Error" )]
		ERR_OutletCheckPalletAllSenErr = ( -30012 ),
		[Description( "Outlet Start Smema to Action Error" )]
		ERR_OutletSMEMAIsActionErr = ( -30013 ),
		[Description( "Release all 3 Pallets Error" )]
		ERR_Release3PalletsErr = ( -30014 ),
		[Description( "Release 1 by 1 Pallet Error" )]
		ERR_Release1By1PalletErr = ( -30015 ),
		[Description( "Release 1 Pallet Error" )]
		ERR_ReleasePallet1Err = ( -30016 ),
		[Description( "Wait Board Move To Outlet Sensor Error" )]
		ERR_BoardToOutletSenErr = ( -30017 ),
		[Description( "Start Releasing Error" )]
		ERR_StartReleasingErr = ( -30018 ),
		[Description( "Shifting Pallet 2 to 1 Error" )]
		ERR_ShiftPallet2To1Err = ( -30019 ),
		[Description( "Wait for Pallet 3 Sensor Off Error" )]
		ERR_WaitPallet3SenseOffErr = ( -30020 ),
		[Description( "Wait for Smema Out flag error" )]
		ERR_WaitSmemaFlagErr = ( -30021 ),
		[Description( "Bottom Alignment Error" )]
		ERR_BottomALErr = ( -30022 ),

		#endregion
		#region Smemainletexception
		//Smema Inlet Seq.
		[Description( "Smema Waiting To Accept Error" )]
		ERR_WaitingToAcceptErr = ( -40001 ),
		[Description( "Accepting Error" )]
		ERR_AcceptErr = ( -40002 ),
		#endregion
		#region Smemaoutletexception
		//Smema Outlet Seq.
		[Description( "Waiting to Release Error" )]
		ERR_WaitingToReleaseErr = ( -50001 ),
		[Description( "Smema Outlet Releasing Error" )]
		ERR_SmemaReleasingErr = ( -50002 ),
		#endregion
		#region Commonexception
		//Smema Outlet Seq.
		[Description( "Initialise Error" )]
		ERR_InitErr = ( -60001 ),
		[Description( "Post Initialise Error" )]
		ERR_PostInitErr = ( -60002 ),
		[Description( "IsAction Error" )]
		ERR_IsActionErr = ( -60003 ),
		[Description( "Finish Error" )]
		ERR_FinishErr = ( -60004 ),
		#endregion
		#region TraySeqException
		//Tray Seq.
		[Description( "Tray Proximity Sensor OUT Error" )]
		ERR_ProxSenOutErr = ( -70001 ),
		[Description( "Tray Proximity Sensor In Error" )]
		ERR_ProxSenInErr = ( -70002 ),
		[Description( "Tray Activate Stopper Error" )]
		ERR_ActStopperErr = ( -70003 ),
		[Description( "Tray Check FileName Error" )]
		ERR_ChkFileNameErr = ( -70004 ),
		[Description( "Tray Save Image Error" )]
		ERR_SaveImageqErr = ( -70005 ),
		[Description( "Tray Setup Cavity Settings Error" )]
		ERR_SetCavityErr = ( -70006 ),
		[Description( "Tray Check Bar Code Error" )]
		ERR_ChkbarCodeErr = ( -70007 ),
		[Description( "Tray Server Comm Is Action Error" )]
		ERR_SvrCommIsActionErr = ( -70008 ),
		[Description( "Tray Receive Product Serial Error" )]
		ERR_RecvProdSerialErr = ( -70009 ),
		[Description( "Tray Move To NFC Pos. Error" )]
		ERR_ToNPCPosErr = ( -70010 ),
		[Description( "Tray Execute NFC Error" )]
		ERR_ExecuteNFCErr = ( -70011 ),
		[Description( "Tray NFC Scanning Error" )]
		ERR_NFCScanningErr = ( -70012 ),
		[Description( "Tray Move To Laser Pos. Error" )]
		ERR_ToLaserPosErr = ( -70013 ),
		[Description( "Tray Signal Laser Marking Error" )]
		ERR_SignalLaserMarkingErr = ( -70014 ),
		[Description( "Tray Bottom Alignment Error" )]
		ERR_TrayBottomALErr = ( -70015 ),
		#endregion
		#region Shopfloor
		[Description( "Waiting For ShopFloor Comm Error" )]
		ERR_WaitShopFloorCommErr = ( -80001 ),
		[Description( "Send Data ShopFloor Error" )]
		ERR_SendToShopFloorErr = ( -80002 ),

		#endregion



		//-10000001 to -20000000 B262 Vision Processing
		//-20000001 to -30000000 HiPA Common
		//-30000001 to -40000000 HiPA Instrument
		//-40000001 to -50000000 JPT Camera
		//-50000001 to -60000000 JPT Matrox System
		#region -10000001 to -20000000 B262 Vision Processing
		[Description( "Golden PNG file name is empty" )]
		ERR_PNGNameEmptyErr = ( -10000001 ),
		[Description( "Etched height or width in mm is empty" )]
		ERR_EtchedHeightWidthIsEmpty = ( -10000002 ),
		[Description( "Byte Array is null" )]
		ERR_ByteArrIsNull = ( -10000003 ),
		[Description( "Invalid Cavity No. or Bottom AL. result" )]
		ERR_InvalidCavityNoBtmALResult = ( -10000004 ),


		#endregion
		#region -30000001 to -40000000 HiPA Instrument
		//Barcode Scanner Cognex
		[Description( "Device discovery takes too long" )]
		ERR_DeviceDiscoveryErr = ( -30000001 ),
		[Description( "Cognex Send Command Trigger Error" )]
		ERR_CongexBarSendTriggerErr = ( -30000002 ),

		//Camera - CameraManager
		[Description( "Camera is Not Initialized" )]
		ERR_CamNotInitializedErr = ( -30010001 ),
		[Description( "Camera in continuous mode" )]
		ERR_CamInContinuousMode = ( -30010002 ),
		[Description( "Continuous Grab Error" )]
		ERR_ContinuousGrabErr = ( -30010003 ),
		[Description( "Camera is NOT in continuous mode" )]
		ERR_CamNotInContinuousMode = ( -30010004 ),
		[Description( "No Camera Detected" )]
		ERR_NoCamDetectedErr = ( -30010005 ),
		//Camera - MatroxProcessing
		[Description( "Invalid or Incorrect Product Recipe Selected" )]
		ERR_InvalidProdRecipe = ( -30011001 ),

		//Height Sensor - Optex
		[Description( "Write Session Error" )]
		ERR_SetCMDError = ( -30020001 ),
		[Description( "Read Session Error" )]
		ERR_GetCMDError = ( -30020002 ),
		[Description( "Checksum Error" )]
		ERR_CheckSumErr = ( -30020003 ),
		[Description( "Fetch Data Failure" )]
		ERR_GetDataErr = ( -30020004 ),
		[Description( "CRC Error" )]
		ERR_CheckCRCErr = ( -30020005 ),
		[Description( "Update Average Number Failure" )]
		ERR_UpdateAvgNumErr = ( -30020006 ),
		[Description( "Update Laser Power Failure" )]
		ERR_UpdateLaserPwrErr = ( -30020007 ),
		[Description( "Update Sampling Periods Failure" )]
		ERR_UpdateSamplingPeriodErr = ( -30020008 ),
		[Description( "Update Sensitivity Failure" )]
		ERR_UpdateSenErr = ( -30020009 ),
		[Description( "Exceeded The Number of Retries" )]
		ERR_RetriesExceeded = ( -30020010 ),

		//LaserController - EzCadCtrl
		[Description( "No EzCad Card detected" )] //Original => "Could not search any EzCad card"
		ERR_NoEzCadCardErr = ( -30030001 ),
		[Description( "Invalid mark count" )]
		ERR_InvalidMarkCountErr = ( -30030002 ),
		[Description( "EzCard is busy" )]
		ERR_EzCardBusy = ( -30030003 ),
		[Description( "File does not exist" )]
		ERR_FileNotExistErr = ( -30030004 ),
		[Description( "Invalid Marker ID" )]
		ERR_InvalidMarkID = ( -30030005 ),
		[Description( "Invalid Marker or EzCad ID" )]
		ERR_InvalidID = ( -30030006 ),
		[Description( "Invalid HatchParam count" )]
		ERR_InvalidHatchParamCount = ( -30030007 ),
		[Description( "Entity does not exist" )]
		ERR_EntityNotExistErr = ( -30030008 ),
		[Description( "Invalid MarkerIDList" )]
		ERR_InvalidMarkIDList = ( -30030009 ),

		//LaserController - LaserEzCad
		[Description( "Get PowerLevel Perc Failed" )]
		ERR_GetPwrLvlPercErr = ( -30031001 ),
		[Description( "Get PowerLevel Failed, Please Enter Watt that within Calibrated Range" )]
		ERR_GetPwrLvlErr = ( -30031002 ),
		[Description( "Commencing Marking Fail" )]
		ERR_MarkingErr = ( -30031003 ),

		//LaserModule - LaserAOC
		[Description( "No Reply Received" )]
		ERR_ReadCMDErr = ( -30040001 ),
		[Description( "Command Set Fail" )]
		ERR_SendCMDErr = ( -30040002 ),
		[Description( "No Reply from Device" )]
		ERR_NoResponseErr = ( -30040003 ),

		//LightSource - CST
		[Description( "Brightness Setting out of range" )]
		ERR_BrightnessOutOfRangeErr = ( -30050001 ),
		[Description( "Invalid return value" )]
		ERR_InvalidReturnValErr = ( -30050002 ),

		//Motion - ADLinkIoBoard
		[Description( "Board not found" )]
		ERR_NoBoardDetectErr = ( -30060001 ),

		//Motion - ADLinkMotionBoard
		[Description( "BoardID not found" )]
		ERR_NoBoardIDDetectErr = ( -30061001 ),

		//NFC - Amphenol
		[Description( "No Response Received" )]
		ERR_NoResponseRecvErr = ( -30070001 ),
		[Description( "Incorrect Data Format" )]
		ERR_InvalidDataFormat = ( -30070002 ),
		[Description( "Unable to find Serial Number" )]
		ERR_FindSNErr = ( -30070003 ),
		[Description( "NFC device not detected" )]
		ERR_NoNFCDetectErr = ( -30070004 ),
		[Description( "NFC Data not Ready" )]
		ERR_NFCDataNotReady = ( -30070005 ),

		//OpticalPowerMeter - LaserPowerMeter
		[Description( "Device not connected" )]
		ERR_NoDeviceErr = ( -30080001 ),

		//ThermoHygroSensor - ThermoHygro
		[Description( "No result received" )]
		ERR_NoResultErr = ( -30090001 ),


		#endregion
		#region -60000001 to -70000000 Neowise Modules 
		// Alarm Page
		[Description( "Please close Conveyor Inlet Cover First." )]
		ERR_CloseConveyorInletCover = ( -60000001 ),
		[Description( "Please close Conveyor Outlet Cover First." )]
		ERR_CloseConveyorOutletCover = ( -60000002 ),

		//MainWindow
		[Description( "Recipe Object null" )]
		ERR_RecipeObjNull = ( -60100001 ),
		[Description( "Failed to Import Product Recipe, The imported recipe already exist." )]
		ERR_RecipeImportFail = ( -60100002 ),

		//ConveyorModuleBase

		[Description( "Invalid Position" )]
		ERR_InvalidAxisPos = ( -60200001 ),
		[Description( "Board detected in InletDoor" )]
		ERR_BoardOnInletDr = ( -60200002 ),
		[Description( "Board detected in OutletDoor" )]
		ERR_BoardOnOutletDr = ( -60200003 ),

		//LaserHeadModule
		[Description( "Initialization Failed" )]
		ERR_InitFailed = ( -60300001 ),
		[Description( "Unexpected height difference. Please check if HeightSensor is pointing at correct position." )]
		ERR_HeightDiff = ( -60300002 ),
		[Description( "Exceeded number of compensation. Please check acceptance tolerance." )]
		ERR_ExceedNoOfCompensationRetries = ( -60300003 ),
		[Description( "Invalid Input Range" )]
		ERR_InvalidRange = ( -60300004 ),
		[Description( "Get PowerLevel Perc Failed" )]
		ERR_GetPwrPercFail = ( -60300005 ),
		[Description( "Get PowerLevel Failed, Please Enter Watt that within Calibrated Range" )]
		ERR_GetPwrPercFailEnterWatt = ( -60300006 ),
		[Description( "Inspection file does not exist" )]
		ERR_InspectFileNotExist = ( -60300007 ),
		[Description( "Model file does not exist" )]
		ERR_ModelFileNotExist = ( -60300008 ),

		//StationModuleBase
		[Description( "Source path is undefined" )]
		ERR_PathIsUndefined = ( -60400001 ),
		[Description( "More than 1 image in folder" )]
		ERR_ExceedOneImage = ( -60400002 ),
		[Description( "GetImageFileName Error." )]
		ERR_GetImgFileNameErr = ( -60400003 ),
		[Description( "Invalid Filename format for noEtch" )]
		ERR_InvalidnoEtchImgFileFormat = ( -60400004 ),
		[Description( "Invalid Filename format" )]
		ERR_InvalidFilenameFormat = ( -60400005 ),

		//IoModules
		[Description( "IO Board not Initialized" )]
		ERR_IONotInit = ( -60500001 ),
		[Description( "Axis not in Safe range" )]
		ERR_AxisNotSafe = ( -60500002 ),
		[Description( "Interlock in effect" )]
		ERR_InterlockInEffect = ( -60500003 ),

		//TrayModuleBase
		[Description( "Tray not closed" )]
		ERR_TrayNotClose = ( -60600001 ),
		[Description( "NFC Z not in reset pos" )]
		ERR_NFCZNotInResetPos = ( -60600002 ),
		[Description( "NFC X not in work pos" )]
		ERR_NFCXNotInWorkPos = ( -60600003 ),
		[Description( "Stopper not in reset pos" )]
		ERR_TrayStopperNotInResetPos = ( -60600004 ),
		[Description( "Input Image File Not Exists" )]
		ERR_ImgFileNotExist = ( -60600005 ),

		//Ctrl_Ezcad
		[Description( "Load File Fail" )]
		ERR_LoadFileFail = ( -60700001 ),
		[Description( "Apply Pen Settings Error" )]
		ERR_ApplyPenErr = ( -60700002 ),
		[Description( "Apply BMP Settings Error" )]
		ERR_ApplyBMPErr = ( -60700003 ),
		[Description( "Apply Pen and BMP Settings Error" )]
		ERR_ApplyPenBMPErr = ( -60700004 ),
		[Description( "MoveRotate Error" )]
		ERR_MovRotateErr = ( -60700005 ),

		//Ctrl_WorkPos
		[Description( "Reached the limit of number of Working Position allowed" )]
		ERR_ExceedNumOfWorkPos = ( -60800001 ),
		[Description( "Existing teached position found" )]
		ERR_TeachPosFound = ( -60800002 ),

		//Recipe
		[Description( "The content of the file is incorrect" )]
		ERR_IncorrectFileContent = ( -60900001 ),

		//MachineStateMng
		[Description( "Timeout: Fail to acknowledge to Pop Up Window (Laser Module Disconnected)" )]
		ERR_AcknowledgeTimeoutEzcad = ( -61000001 ),
		[Description( "Timeout: Fail to acknowledge to Pop Up Window (Manual Marking)" )]
		ERR_AcknowledgeTimeoutMan = ( -61000002 ),

		#endregion

		[Description( "Incorrect Language List Found, Loading backup Selected Language" )]
		ERR_LoadLangErr = ( -99999998 ),
		[Description( "Unknown Exception Error" )]
		ERR_UnknownExceptionErr = ( -99999999 ),
	}
	#endregion
}
