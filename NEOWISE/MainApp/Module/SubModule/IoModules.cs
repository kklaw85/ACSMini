using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
namespace NeoWisePlatform.Module
{
	#region TowerLight
	public enum TowerLights
	{
		RED = 0,
		GREEN,
		AMBER,
		BUZZER
	}

	public enum LightStatus
	{
		Off = 0,
		Static = 1,
		Blink = 2,
	}

	public class TowerLight : BaseUtility
	{
		private SolidColorBrush Black = new SolidColorBrush( Color.FromRgb( 0, 0, 0 ) );
		// { Red, Green, Amber, Buzzer}//multi segment light
		LightStatus[] default_Auto = new LightStatus[] { LightStatus.Off, LightStatus.Static, LightStatus.Off, LightStatus.Off };   // Static Green
		LightStatus[] default_Manual = new LightStatus[] { LightStatus.Off, LightStatus.Off, LightStatus.Static, LightStatus.Off }; // Static Amber
		LightStatus[] default_Homing = new LightStatus[] { LightStatus.Off, LightStatus.Off, LightStatus.Blink, LightStatus.Off }; // Blinking Amber
		LightStatus[] default_Error = new LightStatus[] { LightStatus.Blink, LightStatus.Off, LightStatus.Off, LightStatus.Off };  //Blinking Red

		LightStatus[] Ready_Mode = new LightStatus[] { LightStatus.Off, LightStatus.Static, LightStatus.Off, LightStatus.Off }; // Static Green

		LightStatus[] Auto_Mode = new LightStatus[] { LightStatus.Off, LightStatus.Static, LightStatus.Off, LightStatus.Off }; // Static Green
		LightStatus[] Auto_Laser_Marking = new LightStatus[] { LightStatus.Off, LightStatus.Static, LightStatus.Static, LightStatus.Off }; // Static Green, Static Amber
		LightStatus[] Auto_Pause_Mode = new LightStatus[] { LightStatus.Off, LightStatus.Blink, LightStatus.Off, LightStatus.Off }; //Blinking Green
		LightStatus[] Manual_Mode = new LightStatus[] { LightStatus.Off, LightStatus.Off, LightStatus.Static, LightStatus.Off }; // Static Amber
		LightStatus[] Homing_Mode = new LightStatus[] { LightStatus.Off, LightStatus.Off, LightStatus.Blink, LightStatus.Off }; // Blinking Amber
		LightStatus[] Error_Mode = new LightStatus[] { LightStatus.Blink, LightStatus.Off, LightStatus.Off, LightStatus.Blink }; // Blinking Red  "BRZ";
		LightStatus[] Warning_Mode = new LightStatus[] { LightStatus.Off, LightStatus.Static, LightStatus.Blink, LightStatus.Blink }; // Blinking Amber, Static Green;
		LightStatus[] HitLimit_Mode = new LightStatus[] { LightStatus.Off, LightStatus.Off, LightStatus.Blink, LightStatus.Blink }; // Blinking Amber  "BAZ";

		const bool b_ON = true;
		const bool b_OFF = false;

		private LightStatus[] BLINK_LightStatus = new LightStatus[ 4 ];

		private StateBGColor Original_Bru;
		private Dictionary<TowerLights, AdLinkIoPoint> Towerlight = new Dictionary<TowerLights, AdLinkIoPoint>();
		private bool b_SilentBuzzer = false;
		public bool SilentBuzzer
		{
			get => this.b_SilentBuzzer;
			set => this.Set( ref this.b_SilentBuzzer, value, "SilentBuzzer" );
		}
		private bool b_BypassAlarm = true;
		public bool BypassAlarm
		{
			get => this.b_BypassAlarm;
			set => this.Set( ref this.b_BypassAlarm, value, "BypassAlarm" );
		}

		private MTEquipment _Equipment = null;
		public TowerLight()
		{
			this._Equipment = ( Constructor.GetInstance().Equipment as MTEquipment );
			this.Towerlight[ TowerLights.AMBER ] = this._Equipment.GetIOPointByEnum( OutputIO.O_Yellow );
			this.Towerlight[ TowerLights.BUZZER ] = this._Equipment.GetIOPointByEnum( OutputIO.O_Buzzer );
			this.Towerlight[ TowerLights.GREEN ] = this._Equipment.GetIOPointByEnum( OutputIO.O_Green );
			this.Towerlight[ TowerLights.RED ] = this._Equipment.GetIOPointByEnum( OutputIO.O_Red );
			this.LightBuzzer = Enum.GetValues( typeof( TowerLights ) ).Cast<TowerLights>();
			Equipment.MachStateMgr.MachineStateChanged += this.MachStateMgr_MachineStateChanged;
		}

		private void MachStateMgr_MachineStateChanged( object sender, MachineStateMgr.MachineStateChangeEventArgs e )
		{
			this.Original_Bru = e.BGColor;
			this.Get_State_And_Update( e.MachineState );
		}

		private void UpdateStatus( TowerLights L, bool on )
		{
			try
			{
				var IOValue = on ? DioValue.On : DioValue.Off;
				if ( L != TowerLights.BUZZER )
					this.Towerlight[ L ]?.SetOut( IOValue );
				else
				{
					if ( !this.b_SilentBuzzer )
						this.Towerlight[ L ]?.SetOut( IOValue );
					Task.Delay( 20 ).Wait();
					this.Towerlight[ L ]?.SetOut( DioValue.Off );
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( "TowerLight_UpdateStatus", ex ), ErrorTitle.TowerLightBlinking, ErrorClass.E4 );
			}
		}

		private void Get_State_And_Update( MachineStateType st )
		{
			LightStatus[] TowerLightStr = new LightStatus[] { LightStatus.Off, LightStatus.Off, LightStatus.Off, LightStatus.Off };
			if ( this.b_BypassAlarm ) return;
			this.Stop();
			switch ( st )
			{
				case MachineStateType.READY:
				case MachineStateType.UNINITIALIZE:
					TowerLightStr = this.Ready_Mode;
					break;
				case MachineStateType.AUTO_CYCLESTOP:
				case MachineStateType.AUTO_RUNNING:
				case MachineStateType.BUSY:
					TowerLightStr = this.Auto_Mode;
					break;
				case MachineStateType.AUTO_PAUSE:
					TowerLightStr = this.Auto_Pause_Mode;
					break;
				case MachineStateType.ERROR:
					this.SilentBuzzer = false;
					TowerLightStr = this.Error_Mode;
					break;
				case MachineStateType.WARNING:
					TowerLightStr = this.Warning_Mode;
					break;
				case MachineStateType.HOMING:
					TowerLightStr = this.Homing_Mode;
					break;
			}
			this.AssignsOutPuts( TowerLightStr );
		}
		private void AssignsOutPuts( LightStatus[] TowerLightArray )
		{
			try
			{
				foreach ( var num in this.LightBuzzer )
				{
					if ( TowerLightArray[ ( int )num ] == LightStatus.Off )
						this.UpdateStatus( num, b_OFF );
					else if ( TowerLightArray[ ( int )num ] == LightStatus.Static )
						this.UpdateStatus( num, b_ON );
				}
				this.BLINK_LightStatus = TowerLightArray;
				var liststatus = TowerLightArray.ToList();
				var ResultBlink = liststatus.Where( x => x == LightStatus.Blink );
				if ( ResultBlink.Count() > 0 )
					this.Start();
				else
					Equipment.ErrManager.StateBG = this.Original_Bru;

			}
			catch
			{
			}
		}

		#region IsEnd Property
		int _isContinuous = 0;
		private bool isContinuous
		{
			get => Interlocked.CompareExchange( ref this._isContinuous, 1, 1 ) == 1;
			set
			{
				Interlocked.Exchange( ref this._isContinuous, value ? 1 : 0 );
				this.OnPropertyChanged( "isContinuous" );
			}
		}
		#endregion
		#region Blinking Start/Stop
		Task _Blinking;
		bool BlinkState = false;
		private void Start()
		{
			if ( this.Stopping ) return;
			this.isContinuous = true;
			this.BlinkState = false;
			this._Blinking = Task.Run( () => this.OnBlinking() );
		}
		bool Stopping = false;
		private void Stop()
		{
			this.Stopping = true;
			this.isContinuous = false;
			this.BlinkState = false;
			this._Blinking?.Wait();
			this._Blinking?.Dispose();
			this._Blinking = null;
			this.OffAll();
			this.Stopping = false;
		}
		#endregion
		private IEnumerable<TowerLights> LightBuzzer = null;
		private void OnBlinking()
		{
			while ( this.isContinuous )
			{
				this.BlinkState = !this.BlinkState;
				foreach ( var num in this.LightBuzzer )
				{
					if ( this.BLINK_LightStatus[ ( int )num ] == LightStatus.Blink )
						this.UpdateStatus( num, this.BlinkState );
				}
				Equipment.ErrManager.StateBG = this.BlinkState ? this.Original_Bru : StateBGColor.Ready;
				Task.Delay( 350 ).Wait();
			}
		}
		private void OffAll()
		{
			try
			{
				foreach ( var num in this.LightBuzzer )
				{
					this.UpdateStatus( num, false );
				}
				Equipment.ErrManager.StateBG = StateBGColor.Ready;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( "TowerLight", ex ), ErrorTitle.TowerLightBlinking, ErrorClass.E4 );
			}
		}
	}
	#endregion
	#region Door Locker
	public class DoorLockModule
	{
		public enum LockState : int
		{
			UnlockAll = 0x0,
			LockAll = 0x3,
			LockAllTray = 0xE,
			LockAllGlobal = 0xf,
			Front = 0x1,
			Back = 0x2,
			Left = 0x4,
			Right = 0x8,
			LockFront = LockState.UnlockAll | LockState.Front,
			UnlockFront = LockState.LockAll ^ LockState.Front,
		}

		public AdLinkIoPoint InFront { get; set; }
		public AdLinkIoPoint InRear { get; set; }
		public AdLinkIoPoint InRight { get; set; }
		public AdLinkIoPoint InLeft { get; set; }
		public AdLinkIoPoint OutFront { get; set; }
		public AdLinkIoPoint OutRear { get; set; }
		public AdLinkIoPoint OutRight { get; set; }
		public AdLinkIoPoint OutLeft { get; set; }
		public LockState State
		{
			get
			{
				var state = LockState.UnlockAll;
				if ( this.IsValid == false ) return state;
				if ( this.InFront.Check( DioValue.On ) ) state |= LockState.Front;
				if ( this.InRear.Check( DioValue.On ) ) state |= LockState.Back;
				return state;
			}
			set
			{
				if ( this.IsValid == false ) return;
				this.OutFront.SetOut( ( value & LockState.Front ) != 0 ? DioValue.On : DioValue.Off );
				this.OutRear.SetOut( ( value & LockState.Back ) != 0 ? DioValue.On : DioValue.Off );
			}
		}

		public bool IsValid
		{
			get
			{
				if ( this.InFront == null || this.InRear == null || this.OutFront == null || this.OutRear == null ) return false;
				return true;
			}
		}
		public bool IsValidTray
		{
			get
			{
				if ( this.InLeft == null || this.InRight == null || this.InRear == null || this.OutLeft == null || this.OutRight == null || this.OutRear == null ) return false;
				return true;
			}
		}
	}


	#endregion
	#region Machine Light
	public class MachineLight
	{
		public enum LightState : int
		{
			Off = 0,
			On = 1,
		}

		public AdLinkIoPoint LightSwitch { get; set; }

		public bool State
		{
			get => this.LightSwitch.Check( DioValue.On ) ? true : false;
			set => this.LightSwitch.SetOut( value ? DioValue.On : DioValue.Off );
		}

	}
	#endregion
	#region IO Motion Base

	[Serializable]
	public class IOMotionConfiguration
	: Configuration
	{
		[NonSerialized]
		public static readonly IOMotionConfiguration _DEFAULT = new IOMotionConfiguration();
		[NonSerialized]
		public static readonly string NAME = "IOMotion";

		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.IOMotion;
		public override Type InstrumentType => typeof( IOMotion );
		public Timing Pick { get; set; } = new Timing();
		public Timing Place { get; set; } = new Timing();
		public int TimeOut
		{
			get => this.GetValue( () => this.TimeOut );
			set => this.SetValue( () => this.TimeOut, value );
		}

		public override MachineVariant MachineVar { get; set; } = new MachineVar();
	}

	public class IOMotion : InstrumentBase
	{
		public LiftModuleBase Lift { get; protected set; }
		public override MachineVariant MachineVar { get; set; }
		public double AxisPositionCheck { get; set; } = 0;
		public IOMotionConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.IOMotion;
		protected override string OnCreate()
		{
			base.Create();
			return String.Empty;
		}

		protected override string OnInitialize()
		{
			var result = String.Empty;
			try
			{

			}
			catch ( Exception ex )
			{
				result = Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InitializeFailure, ErrorClass.E5 );
			}
			return result;
		}

		protected override string OnStop()
		{
			return String.Empty;
		}

		protected override string OnTerminate()
		{
			return String.Empty;
		}
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as IOMotionConfiguration;
		}

		private Stopwatch Cylinderstopwatch = new Stopwatch();
		private Stopwatch Suctionstopwatch = new Stopwatch();
		public AdLinkIoPoint _InPos { get; set; }
		public AdLinkIoPoint _ResetPos { get; set; }
		public AdLinkIoPoint _ToPosCylinder { get; set; }
		public AdLinkIoPoint _ToResetCylinder { get; set; }
		public AdLinkIoPoint _VacuumSuction { get; set; }
		public AdLinkIoPoint _VacuumSuctionSense { get; set; }
		public bool? InPos
		{
			get => this._InPos?.Check( DioValue.On );
		}
		public bool? ResetPos
		{
			get => this._ResetPos?.Check( DioValue.On );
		}
		private bool? Out
		{
			get
			{
				if ( this._ToResetCylinder == null )
					return this._ToPosCylinder?.Check( DioValue.On );
				else
					return this._ToPosCylinder?.Check( DioValue.On ) == true && this._ToResetCylinder?.Check( DioValue.Off ) == true;
			}
			set
			{
				this._ToPosCylinder?.SetOut( value == true ? DioValue.On : DioValue.Off );
				this._ToResetCylinder?.SetOut( value == true ? DioValue.Off : DioValue.On );
			}
		}
		public bool? Suction
		{
			get => this._VacuumSuction?.Check( DioValue.On );
			set => this._VacuumSuction?.SetOut( value == true ? DioValue.On : DioValue.Off );
		}
		public bool SuctionSense
		{
			get => this._VacuumSuctionSense.Check( DioValue.On );
		}
		public string ThrowEXIfMatDet()
		{
			if ( this.SuctionSense ) return $"Material detected in {this.Name}. Please remove material manually.";
			else return string.Empty;
		}
		public Task<ErrorResult> MoveToPos()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var OriginalState = false;
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					OriginalState = ( bool )this.Out;
					if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
					if ( !this._ToPosCylinder.Board.IsValid ) throw new Exception( $"^{( int )RunErrors.ERR_IONotInit}^" );
					if ( this._ToResetCylinder != null ) if ( !this._ToResetCylinder.Board.IsValid ) throw new Exception( $"^{( int )RunErrors.ERR_IONotInit}^" );
					if ( this.InPos == true )
					{
						this.InternalProvider.SetUnsafe();
						return this.Result;
					}
					Thread.Sleep( 10 );
					this.Out = true;
					if ( this._InPos == null ) return this.Result;
					this.Cylinderstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( this.InPos == true || MachineStateMng.isSimulation )
						{
							Done = true;
							break;
						}
						else if ( this.Cylinderstopwatch.ElapsedMilliseconds >= this.Configuration.TimeOut )
							throw new Exception( $"^{( int )RunErrors.ERR_IOTimeout }^" );
						Thread.Sleep( 50 );
					}
					this.InternalProvider.SetUnsafe();
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
					this.Out = OriginalState;
					if ( this.ResetPos == true && this.InPos == false )
						this.InternalProvider.SetSafe();
					else
						this.InternalProvider.SetUnsafe();
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> Reset()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var OriginalState = false;
				try
				{
					if ( !this.ValidVariant() ) return this.Result;
					OriginalState = ( bool )this.Out;
					if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
					if ( !this._ToPosCylinder.Board.IsValid ) throw new Exception( $"^{( int )RunErrors.ERR_IONotInit }^" );
					if ( this._ToResetCylinder != null ) if ( !this._ToResetCylinder.Board.IsValid ) throw new Exception( $"^{( int )RunErrors.ERR_IONotInit }^" );
					if ( this.ResetPos == true )
					{
						this.InternalProvider.SetSafe();
						return this.Result;
					}
					Thread.Sleep( 10 );
					this.Out = false;
					if ( this._ResetPos == null ) return this.Result;
					this.Cylinderstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( this.ResetPos == true || MachineStateMng.isSimulation )
						{
							Done = true;
							break;
						}
						else if ( this.Cylinderstopwatch.ElapsedMilliseconds >= this.Configuration.TimeOut )
							throw new Exception( $"^{( int )RunErrors.ERR_IOTimeout}^" );
						Thread.Sleep( 50 );
					}
					this.InternalProvider.SetSafe();
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
					this.Out = OriginalState;
					if ( this.ResetPos == true && this.InPos == false )
						this.InternalProvider.SetSafe();
					else
						this.InternalProvider.SetUnsafe();
				}
				return this.Result;
			} );
		}
		public bool ObjHeld
		{
			get => this.GetValue( () => this.ObjHeld );
			set => this.SetValue( () => this.ObjHeld, value );
		}
		public Task<ErrorResult> PickUp( bool? BypassVac = null )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var SuctionErrMsg = string.Empty;
				ErrorResult SuctionResult = new ErrorResult();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this._VacuumSuction == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum object" );
					if ( this._VacuumSuctionSense == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum sense object" );
					if ( BypassVac != true ) if ( this.SuctionSense ) return this.Result;
					this.CheckAndThrowIfError( this.MoveToPos().Result );
					this.CheckAndThrowIfError( this.Lift?.StartPickPlace() );
					if ( BypassVac != true )
					{
						if ( ( this.Result = this.SuctionOn().Result ).EClass != ErrorClass.OK )
						{
							SuctionResult.ErrorMessage = this.Result.ErrorMessage;
							SuctionResult.EClass = this.Result.EClass;
						}
						if ( SuctionResult.EClass == ErrorClass.OK )
						{
							this.CheckAndThrowIfError( this.Lift?.MoveDown() );
							Thread.Sleep( this.Configuration.Pick.Delay );
							this.CheckAndThrowIfError( this.Lift?.EndPickPlace() );
						}
					}
					this.CheckAndThrowIfError( this.Reset().Result );
					if ( BypassVac != true )
					{
						this.CheckAndThrowIfError( SuctionResult );
						if ( this.Suction != true ) this.ThrowError( ErrorClass.E5, "Output turned off." );
						if ( this.SuctionSense != true ) this.ThrowError( ErrorClass.E5, "Suction pressure dropped." );
					}
				}
				catch ( Exception ex )
				{
					this.SuctionOff().Wait();
					this.CatchException( ex );
				}
				finally
				{
					this.ObjHeld = this.SuctionSense;
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> PlaceDown( bool? BypassVac = null )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var SuctionErrMsg = string.Empty;
				ErrorResult SuctionResult = new ErrorResult();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this._VacuumSuctionSense == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum sense object" );
					if ( BypassVac != true ) if ( !this.SuctionSense ) this.ThrowError( ErrorClass.E6, "Arm is empty or air leakage." );
					this.CheckAndThrowIfError( this.MoveToPos().Result );
					if ( BypassVac != true )
					{
						if ( ( this.Result = this.SuctionOff().Result ).EClass != ErrorClass.OK )
						{
							SuctionResult.ErrorMessage = this.Result.ErrorMessage;
							SuctionResult.EClass = this.Result.EClass;
						}
						if ( SuctionResult.EClass == ErrorClass.OK )
							Thread.Sleep( this.Configuration.Place.Delay );
					}
					this.CheckAndThrowIfError( this.Reset().Result );
					if ( BypassVac != true )
					{
						this.CheckAndThrowIfError( SuctionResult );
						if ( this.Suction != false ) this.ThrowError( ErrorClass.E5, "Output turned On." );
						if ( this.SuctionSense != false ) this.ThrowError( ErrorClass.E5, "Suction pressure still high." );
					}
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				finally
				{
					this.ObjHeld = this.SuctionSense;
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> SuctionOn()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this._VacuumSuction == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum object" );
					if ( this._VacuumSuctionSense == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum sense object" );
					this.Suction = true;
					this.Suctionstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( this.SuctionSense )
						{
							Done = true;
							break;
						}
						else if ( this.Suctionstopwatch.ElapsedMilliseconds >= this.Configuration.Pick.TimeOut ) this.ThrowError( ErrorClass.E4, $"^{( int )RunErrors.ERR_PickExceededTimeoutErr }^" );
						Thread.Sleep( 50 );
					}
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
					this.Suction = false;
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> SuctionOff()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this._VacuumSuction == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum object" );
					if ( this._VacuumSuctionSense == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum sense object" );
					this.Suction = false;
					this.Suctionstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( !this.SuctionSense )
						{
							Done = true;
							break;
						}
						else if ( this.Suctionstopwatch.ElapsedMilliseconds >= this.Configuration.Place.TimeOut ) this.ThrowError( ErrorClass.E4, $"^{( int )RunErrors.ERR_PlaceExceededTimeoutErr }^" );
						Thread.Sleep( 50 );
					}
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}
		public bool MatDropped => !this.SuctionSense && this.ObjHeld;

		public void LinkLiftModule( LiftModuleBase source )
		{
			this.Lift = source;
		}
		public void ClearLiftModuleLink()
		{
			this.Lift = null;
		}
	}

	#endregion
	#region SimpleIO
	public class IO : BaseUtility
	{
		public AdLinkIoPoint IOPt { get; set; }

		public bool State
		{
			get => this.IOPt.Check( DioValue.On ) ? true : false;
			set => this.IOPt.SetOut( value ? DioValue.On : DioValue.Off );
		}
		public string On()
		{
			this.ClearErrorFlags();
			try
			{
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.State = true;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result;
		}
		public string Off()
		{
			this.ClearErrorFlags();
			try
			{
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.State = false;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result;
		}
	}
	#endregion
	#region StageClamperBase

	[Serializable]
	public class StageClamperConfiguration
	: Configuration
	{
		[NonSerialized]
		public static readonly StageClamperConfiguration _DEFAULT = new StageClamperConfiguration();
		[NonSerialized]
		public static readonly string NAME = "StageClamperVac";

		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.StageClamper;
		public override Type InstrumentType => typeof( StageClamper );
		public Timing SuctionHold { get; set; } = new Timing();
		public Timing SuctionRelease { get; set; } = new Timing();
		public int Timeout
		{
			get => this.GetValue( () => this.Timeout );
			set => this.SetValue( () => this.Timeout, value );
		}
		public bool ReverseLogic
		{
			get => this.GetValue( () => this.ReverseLogic );
			set => this.SetValue( () => this.ReverseLogic, value );
		}
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
	}

	public class StageClamper : InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		public StageClamperConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.StageClamper;
		protected override string OnCreate()
		{
			base.Create();
			return String.Empty;
		}

		protected override string OnInitialize()
		{
			var result = String.Empty;
			try
			{

			}
			catch ( Exception ex )
			{
				result = Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InitializeFailure, ErrorClass.E5 );
			}
			return result;
		}

		protected override string OnStop()
		{
			return String.Empty;
		}

		protected override string OnTerminate()
		{
			return String.Empty;
		}
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as StageClamperConfiguration;
		}

		private Stopwatch Cylinderstopwatch = new Stopwatch();
		private Stopwatch Suctionstopwatch = new Stopwatch();
		public AdLinkIoPoint _InPos { get; set; }//close pos
		public AdLinkIoPoint _ResetPos { get; set; }//open pos
		public AdLinkIoPoint _ToPosCylinder { get; set; }
		public AdLinkIoPoint _ToResetCylinder { get; set; }
		public AdLinkIoPoint _VacuumSuction { get; set; }
		public AdLinkIoPoint _VacuumSuctionSense { get; set; }
		public AdLinkIoPoint _MatSense { get; set; }
		public bool ByPassSense
		{
			get => this.GetValue( () => this.ByPassSense );
			set => this.SetValue( () => this.ByPassSense, value );
		}
		public bool? InPos
		{
			get => this._InPos?.Check( DioValue.On );
		}
		public bool? ResetPos
		{
			get => this._ResetPos?.Check( DioValue.On );
		}
		public bool? MatDet
		{
			get => this._MatSense?.Check( DioValue.On );
		}
		private bool? Out
		{
			get
			{
				if ( !this.Configuration.ReverseLogic )
				{
					if ( this._ToResetCylinder == null )
						return this._ToPosCylinder?.Check( DioValue.On );
					else
						return this._ToPosCylinder?.Check( DioValue.On ) == true && this._ToResetCylinder?.Check( DioValue.Off ) == true;
				}
				else
				{
					if ( this._ToResetCylinder == null )
						return this._ToPosCylinder?.Check( DioValue.Off );
					else
						return this._ToPosCylinder?.Check( DioValue.Off ) == true && this._ToResetCylinder?.Check( DioValue.On ) == true;
				}
			}
			set
			{
				if ( !this.Configuration.ReverseLogic )
				{
					this._ToPosCylinder?.SetOut( value == true ? DioValue.On : DioValue.Off );
					this._ToResetCylinder?.SetOut( value == true ? DioValue.Off : DioValue.On );
				}
				else
				{
					this._ToPosCylinder?.SetOut( value == true ? DioValue.Off : DioValue.On );
					this._ToResetCylinder?.SetOut( value == true ? DioValue.On : DioValue.Off );
				}
			}
		}
		public bool Suction
		{
			get => this._VacuumSuction.Check( DioValue.On );
			set => this._VacuumSuction?.SetOut( value ? DioValue.On : DioValue.Off );
		}
		public bool SuctionSense
		{
			get => this._VacuumSuctionSense.Check( DioValue.On );
		}
		public Task<ErrorResult> ClampClose()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var OriginalState = false;
				try
				{
					//if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					OriginalState = ( bool )this.Out;
					if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
					if ( !this._ToPosCylinder.Board.IsValid ) throw new Exception( $"^{( int )RunErrors.ERR_IONotInit}^" );
					if ( this._ToResetCylinder != null ) if ( !this._ToResetCylinder.Board.IsValid ) throw new Exception( $"^{( int )RunErrors.ERR_IONotInit}^" );
					if ( this.InPos == true )
					{
						this.InternalProvider.SetUnsafe();
						return this.Result;
					}
					Thread.Sleep( 10 );
					this.Out = true;
					if ( this._InPos == null ) return this.Result;
					this.Cylinderstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( this.InPos == true || MachineStateMng.isSimulation )
						{
							Done = true;
							break;
						}
						else if ( this.ByPassSense )
						{
							Thread.Sleep( 500 );
							Done = true;
							break;
						}
						else if ( this.Cylinderstopwatch.ElapsedMilliseconds >= this.Configuration.Timeout ) throw new Exception( $"^{( int )RunErrors.ERR_ClampCloseMovementTimeoutErr }^" );
						Thread.Sleep( 5 );
					}
					this.InternalProvider.SetUnsafe();
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
					this.Out = OriginalState;
					if ( this.ResetPos == true && this.InPos == false )
						this.InternalProvider.SetSafe();
					else
						this.InternalProvider.SetUnsafe();
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> ClampOpen()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var OriginalState = false;
				try
				{
					if ( !this.ValidVariant() ) return this.Result;
					OriginalState = ( bool )this.Out;
					if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
					if ( !this._ToPosCylinder.Board.IsValid ) throw new Exception( $"^{( int )RunErrors.ERR_IONotInit }^" );
					if ( this._ToResetCylinder != null ) if ( !this._ToResetCylinder.Board.IsValid ) throw new Exception( $"^{( int )RunErrors.ERR_IONotInit }^" );
					if ( this.ResetPos == true )
					{
						this.InternalProvider.SetSafe();
						return this.Result;
					}
					Thread.Sleep( 10 );
					this.Out = false;
					if ( this._ResetPos == null ) return this.Result;
					this.Cylinderstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( this.ResetPos == true || MachineStateMng.isSimulation )
						{
							Done = true;
							break;
						}
						else if ( this.ByPassSense )
						{
							Thread.Sleep( 500 );
							Done = true;
							break;
						}
						else if ( this.Cylinderstopwatch.ElapsedMilliseconds >= this.Configuration.Timeout ) throw new Exception( $"^{( int )RunErrors.ERR_ClampOpenMovementTimeoutErr}^" );
						Thread.Sleep( 5 );
					}
					this.InternalProvider.SetSafe();
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
					this.Out = OriginalState;
					if ( this.ResetPos == true && this.InPos == false )
						this.InternalProvider.SetSafe();
					else
						this.InternalProvider.SetUnsafe();
				}
				return this.Result;
			} );
		}

		public Task<ErrorResult> SuctionOn()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					//if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this._VacuumSuction == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum object" );
					if ( this._VacuumSuctionSense == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum sense object" );
					this.Suction = true;
					this.Suctionstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( this.SuctionSense )
						{
							Done = true;
							break;
						}
						else if ( this.ByPassSense )
						{
							Thread.Sleep( this.Configuration.SuctionHold.Delay );
							Done = true;
							break;
						}
						else if ( this.Suctionstopwatch.ElapsedMilliseconds >= this.Configuration.SuctionHold.TimeOut ) this.ThrowError( ErrorClass.E4, $"^{( int )RunErrors.ERR_SuctionHoldExceededTimeoutErr }^" );
						Thread.Sleep( 5 );
					}
					Thread.Sleep( this.Configuration.SuctionHold.Delay );
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
					this.Suction = false;
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> SuctionOff()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					//if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this._VacuumSuction == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum object" );
					if ( this._VacuumSuctionSense == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum sense object" );
					this.Suction = false;
					this.Suctionstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( !this.SuctionSense )
						{
							Done = true;
							break;
						}
						else if ( this.ByPassSense )
						{
							Thread.Sleep( this.Configuration.SuctionRelease.Delay );
							Done = true;
							break;
						}
						else if ( this.Suctionstopwatch.ElapsedMilliseconds >= this.Configuration.SuctionRelease.TimeOut ) this.ThrowError( ErrorClass.E4, $"^{( int )RunErrors.ERR_SuctionReleaseExceededTimeoutErr }^" );
						Thread.Sleep( 5 );
					}
					Thread.Sleep( this.Configuration.SuctionRelease.Delay );
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}

		public Task<ErrorResult> Hold()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var SuctionErrMsg = string.Empty;
				ErrorResult SuctionResult = new ErrorResult();
				this.ByPassSense = false;
				try
				{
					//if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this.MatDet != true ) throw new Exception( "MatDet sense: Material not detected." );
					this.CheckAndThrowIfError( this.SuctionOn().Result );
					this.CheckAndThrowIfError( this.ClampClose().Result );
					this.CheckAndThrowIfError( this.SuctionOff().Result );
					this.CheckAndThrowIfError( this.SuctionOn().Result );
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> Release()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var SuctionErrMsg = string.Empty;
				ErrorResult SuctionResult = new ErrorResult();
				this.ByPassSense = false;
				try
				{
					//if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					var Tasks = new Task<ErrorResult>[]
					{
						this.SuctionOff(),
						this.ClampOpen(),
					};
					this.CheckAndThrowIfError( Tasks );
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}
		public void ResetBypass()
		{
			this.ByPassSense = false;
		}
	}

	#endregion
	#region SuctionIo
	[Serializable]
	public class SuctionIOConfiguration
	: Configuration
	{
		[NonSerialized]
		public static readonly SuctionIOConfiguration _DEFAULT = new SuctionIOConfiguration();
		[NonSerialized]
		public static readonly string NAME = "IOMotion";

		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.IOMotion;
		public override Type InstrumentType => typeof( SuctionIO );
		public Timing Pick { get; set; } = new Timing();
		public Timing Place { get; set; } = new Timing();
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
	}

	public class SuctionIO : InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		public double AxisPositionCheck { get; set; } = 0;
		public SuctionIOConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.SuctionIO;
		protected override string OnCreate()
		{
			base.Create();
			return String.Empty;
		}

		protected override string OnInitialize()
		{
			var result = String.Empty;
			try
			{

			}
			catch ( Exception ex )
			{
				result = Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InitializeFailure, ErrorClass.E5 );
			}
			return result;
		}

		protected override string OnStop()
		{
			return String.Empty;
		}

		protected override string OnTerminate()
		{
			return String.Empty;
		}
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as SuctionIOConfiguration;
		}

		private Stopwatch Suctionstopwatch = new Stopwatch();
		public AdLinkIoPoint _VacuumSuction { get; set; }
		public AdLinkIoPoint _VacuumSuctionSense { get; set; }
		public bool Suction
		{
			get => this._VacuumSuction.Check( DioValue.On );
			set => this._VacuumSuction?.SetOut( value ? DioValue.On : DioValue.Off );
		}
		public bool SuctionSense
		{
			get => this._VacuumSuctionSense.Check( DioValue.On );
		}
		public Task<ErrorResult> SuctionOn()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this._VacuumSuction == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum object" );
					if ( this._VacuumSuctionSense == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum sense object" );
					this.Suction = true;
					this.Suctionstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( this.SuctionSense )
						{
							Done = true;
							break;
						}
						else if ( this.Suctionstopwatch.ElapsedMilliseconds >= this.Configuration.Pick.TimeOut ) this.ThrowError( ErrorClass.E4, $"^{( int )RunErrors.ERR_PickExceededTimeoutErr }^" );
						Thread.Sleep( 50 );
					}
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
					this.Suction = false;
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> SuctionOff()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( !this.ValidVariant() ) return this.Result;
					if ( this._VacuumSuction == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum object" );
					if ( this._VacuumSuctionSense == null ) this.ThrowError( ErrorClass.E6, "Null Vacuum sense object" );
					this.Suction = false;
					this.Suctionstopwatch.Restart();
					bool Done = false;
					while ( !Done )
					{
						if ( !this.SuctionSense )
						{
							Done = true;
							break;
						}
						else if ( this.Suctionstopwatch.ElapsedMilliseconds >= this.Configuration.Place.TimeOut ) this.ThrowError( ErrorClass.E4, $"^{( int )RunErrors.ERR_PlaceExceededTimeoutErr }^" );
						Thread.Sleep( 50 );
					}
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}
	}
	#endregion
	public class Timing : RecipeBaseUtility
	{
		public int Delay
		{
			get => this.GetValue( () => this.Delay );
			set => this.SetValue( () => this.Delay, value );
		}
		public int TimeOut
		{
			get => this.GetValue( () => this.TimeOut );
			set => this.SetValue( () => this.TimeOut, value );
		}
	}


}



