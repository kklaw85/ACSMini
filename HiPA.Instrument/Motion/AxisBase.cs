using HiPA.Common;
using HiPA.Common.Forms;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace HiPA.Instrument.Motion
{
	[Serializable]
	public enum HomingDirection
	{
		Positive = 0,
		Negative = 1,
	}

	[Serializable]
	public enum HomingDirectionLTDMC
	{
		Positive = 1,
		Negative = 0,
	}

	[Serializable]
	public enum HomingVelModeLTDMC
	{
		HighSpeed = 1,
		LowSpeed = 0,
	}

	[Serializable]
	public enum NegativeDirection
	{
		None,
		Left,
		Right,
		Up,
		Down,
		Forward,
		Backward,
		CW,
		CCW
	}

	[Serializable]
	public enum IOIFSlotNumber : int
	{
		IOIF_No1 = 0,
		IOIF_No2,
		IOIF_No3,
		IOIF_No4,
	}

	[Serializable]
	public enum IOIFPortNumber : int
	{
		IOIF_PortNo1 = 1,
		IOIF_PortNo2 = 2,
	}

	[Serializable]
	public enum HomingDuringInit : int
	{
		All_Time = 1,
		Motion_Error_Only = 2,
		SVON_ALM_Error_Only = 3,
	}


	[Serializable]
	public enum IOLogicLevel
	{
		Low,
		High,
	}

	[Serializable]
	public class HomingProfile
		: BaseUtility
	{
		private int i_HOME_MODE = 0;
		public int HOME_MODE
		{
			get => this.i_HOME_MODE;
			set => this.Set( ref this.i_HOME_MODE, value, "HOME_MODE" );
		}
		private HomingDirection e_HOME_DIR = HomingDirection.Negative;
		public HomingDirection HOME_DIR
		{
			get => this.e_HOME_DIR;
			set => this.Set( ref this.e_HOME_DIR, value, "HOME_DIR" );
		}

		/// <summary>
		///	Specify the EZ count up value (default: 0)
		/// </summary>
		private int i_HOME_EZ = 0;
		public int HOME_EZ
		{
			get => this.i_HOME_EZ;
			set => this.Set( ref this.i_HOME_EZ, value, "HOME_EZ" );
		}

		/// <summary>
		/// Homing Max Velocity
		/// </summary>
		private double d_HOME_VM = 0;
		public double HOME_VM
		{
			get => this.d_HOME_VM;
			set => this.Set( ref this.d_HOME_VM, value, "HOME_VM" );
		}

		/// <summary>
		/// Homing leave ORG velocity - Specify FA speed. Unit: Pulse/Sec, (default: 100)
		/// </summary>
		private double d_HOME_VO = 0;
		public double HOME_VO
		{
			get => this.d_HOME_VO;
			set => this.Set( ref this.d_HOME_VO, value, "HOME_VO" );
		}

		/// <summary>
		/// Homing Offset
		/// </summary>
		private double d_HOME_OFFSET = 0;
		public double HOME_OFFSET
		{
			get => this.d_HOME_OFFSET;
			set => this.Set( ref this.d_HOME_OFFSET, value, "HOME_OFFSET" );
		}
	}


	[Serializable]
	public class HomingProfileLTDMC : HomingProfile
	{
		private HomingDirectionLTDMC e_HOME_DIR = HomingDirectionLTDMC.Negative;
		public new HomingDirectionLTDMC HOME_DIR
		{
			get => this.e_HOME_DIR;
			set => this.Set( ref this.e_HOME_DIR, value, "HOME_DIR" );
		}
		private HomingVelModeLTDMC e_HOME_VELMODE = HomingVelModeLTDMC.LowSpeed;
		public HomingVelModeLTDMC HOME_VELMODE
		{
			get => this.e_HOME_VELMODE;
			set => this.Set( ref this.e_HOME_VELMODE, value, "VelocityMode" );
		}

		private double d_TAcc = 10;
		public double TAcc//Time Acceleration in second
		{
			get => this.d_TAcc;
			set => this.Set( ref this.d_TAcc, value, "TAcc" );
		}

		private double d_TDec = 10;
		public double TDec//Time Deceleration in second
		{
			get => this.d_TDec;
			set => this.Set( ref this.d_TDec, value, "TDec" );
		}
	}

	[Serializable]
	public enum DisplacementUnits
	{
		EncoderCount,
		MotorStep,
		Millimeter,
		Micrometer,
		Inches,
		MilliInches,
		MicroInches,
		Degree,
		Gradian,
		Radian,
		Milliradian,
		Microradian,
	}

	[Serializable]
	public abstract class AxisConfiguration
		: Configuration
	{
		private int _AxisId = 0;
		public virtual int AxisId
		{
			get => this._AxisId;
			set => this.Set( ref this._AxisId, value, "AxisId" );
		}
		private int _ModuleNo = 0;
		public virtual int ModuleNo
		{
			get => this._ModuleNo;
			set => this.Set( ref this._ModuleNo, value, "ModuleNo" );
		}
		public abstract HomingProfile HomingProfile { get; set; }

		public abstract NegativeDirection NegativeDirection { get; set; }
		/// <summary>
		/// Pulse/mm
		/// </summary>
		private double _MotionScale = 0;
		public virtual double MotionScale
		{
			get => this._MotionScale;
			set => this.Set( ref this._MotionScale, value, "MotionScale" );
		}
		private double _DelayBeforeSetZero = 0;
		public virtual double DelayBeforeSetZero
		{
			get => this._DelayBeforeSetZero;
			set => this.Set( ref this._DelayBeforeSetZero, value, "DelayBeforeSetZero" );
		}
		private double _FineMoveVelocity = 0;
		public virtual double FineMoveVelocity
		{
			get => this._FineMoveVelocity;
			set => this.Set( ref this._FineMoveVelocity, value, "FineMoveVelocity" );
		}
		private double _CoarseMoveVelocity = 0;
		public virtual double CoarseMoveVelocity
		{
			get => this._CoarseMoveVelocity;
			set => this.Set( ref this._CoarseMoveVelocity, value, "CoarseMoveVelocity" );
		}
		private double _MaxVelocity = 0;
		public virtual double MaxVelocity
		{
			get => this._MaxVelocity;
			set => this.Set( ref this._MaxVelocity, value, "MaxVelocity" );
		}
		private double _MaxAcceleration = 0;
		public virtual double MaxAcceleration
		{
			get => this._MaxAcceleration;
			set => this.Set( ref this._MaxAcceleration, value, "MaxAcceleration" );
		}
		private double _MaxDeceleration = 0;
		public virtual double MaxDeceleration
		{
			get => this._MaxDeceleration;
			set => this.Set( ref this._MaxDeceleration, value, "MaxDeceleration" );
		}
		private double? _UpperPosLimit = 0;
		public virtual double? UpperPosLimit
		{
			get => this._UpperPosLimit;
			set => this.Set( ref this._UpperPosLimit, value, "UpperPosLimit" );
		}
		private double? _LowerPosLimit = 0;
		public virtual double? LowerPosLimit
		{
			get => this._LowerPosLimit;
			set => this.Set( ref this._LowerPosLimit, value, "LowerPosLimit" );
		}

		public abstract MotionProfile GeneralMove { get; set; }
		public abstract MotionProfile CommandedMove { get; set; }
		public abstract DisplacementUnits Units { get; set; }

		public event EventHandler IntervalChangedEvent;
		public void EmitIntervalChangedEvent() => this.IntervalChangedEvent?.Invoke( this, null );
	}

	public abstract class AxisBase
		: InstrumentBase
	{
		public static readonly ReadOnlyCollection<double> Interval =
		new ReadOnlyCollection<double>( new[]
		{
		 0.001, 0.002, 0.005,
		 0.01, 0.02, 0.05,
		 0.1, 0.2, 0.5,
		 1, 2, 5,
		 10, 20, 50
		} );
		public abstract AxisConfiguration Configuration { get; }
		public double StepMove
		{
			get => this.GetValue( () => this.StepMove );
			set => this.SetValue( () => this.StepMove, value );
		}
		public abstract int AxisId { get; }
		public abstract MotionStatusBase Status { get; protected set; }
		public MotionBoardBase BoardBase { get; set; }

		public Task<string> ServoOn( bool On ) => Task.Run( () => this.OnServoOn( On ) );
		public Task<string> Homing() => Task.Run( () => this.OnHoming() );
		public Task<string> SetAxisProfile( MotionProfile profile ) => Task.Run( () => this.OnSetAxisProfile( profile ) );
		public Task<string> AbsoluteMove( Trajectory trajectory ) => Task.Run( () => this.OnAbsoluteMove( trajectory ) );
		public Task<string> RelativeMove( Trajectory trajectory, bool WaitDone = true ) => Task.Run( () => this.OnRelativeMove( trajectory, WaitDone ) );
		public Task<string> VelocityMove( Trajectory trajectory ) => Task.Run( () => this.OnVelocityMove( trajectory ) );

		protected override string OnStop() => this.StopMove();

		protected abstract string OnServoOn( bool On );
		protected abstract string OnHoming();
		protected abstract string OnSetAxisProfile( MotionProfile profile );
		protected abstract string OnAbsoluteMove( Trajectory trajectory );
		protected abstract string OnRelativeMove( Trajectory trajectory, bool WaitDone = true );
		protected abstract string OnVelocityMove( Trajectory trajectory );
		public abstract string StopMove();
		public abstract string WaitHomingComplete();
		public abstract string WaitMoveComplete();

		public abstract string GetCommandPosition( ref double position );
		public abstract string GetActualPosition( ref double position );
		public abstract string GetErrorPosition( ref double position );
		public abstract string SetCommandPosition( double pos );
		public abstract string SetActualPosition( double pos );
		public string CheckPosition( double targetPos, double tolerance = 0.05 )
		{
			var Err = string.Empty;
			if ( MachineStateMng.isSimulation ) return Err;
			var cur = 0d;
			if ( ( Err = this.GetActualPosition( ref cur ) ) != string.Empty ) return Err;
			var PosErr = Math.Abs( cur - targetPos );
			return PosErr > tolerance ? $"Position Error higher than threshold: { PosErr}" : Err;
		}
		//public bool CheckErrorPosition( double tolerance = 0.05 )
		//{
		//	if ( MachineStateMng.isSimulation ) return true;
		//	var cur = 0d;
		//	if ( this.GetErrorPosition( ref cur ) != string.Empty ) return false;
		//	return Math.Abs( cur ) < tolerance;
		//}

		public bool isValid( double Pos )
		{
			if ( this.Configuration.LowerPosLimit == null && this.Configuration.UpperPosLimit == null ) return true;
			return Pos <= this.Configuration.UpperPosLimit && Pos >= this.Configuration.LowerPosLimit;
		}
	}

	public abstract class MotionStatusBase : BaseUtility
	{
		public abstract AxisBase Axis { get; set; }

		public abstract void UpdateStatus();
		protected abstract void UpdateProfile();

		protected virtual void OnMonitoring()
		{
			while ( this.IsEnd == false )
			{
				var axisId = this.Axis.AxisId;
				if ( this.Axis.BoardBase is MotionBoardBase board &&
					board.IsOpen() == true )
				{
					this.UpdateStatus();
					this.UpdateProfile();
				}
				Task.Delay( 500 ).Wait();
			}
		}
		#region IsEnd Property
		int _isEnd = 1;
		protected bool IsEnd
		{
			get => Interlocked.CompareExchange( ref this._isEnd, 1, 1 ) == 1;
			set => Interlocked.Exchange( ref this._isEnd, value ? 1 : 0 );
		}
		#endregion
		#region Monitoring Start/Stop
		Task _monitoring;
		public void Start()
		{
			this.IsEnd = false;
			this._monitoring = Task.Run( () => this.OnMonitoring() );
		}
		public void Stop()
		{
			this.IsEnd = true;
			this._monitoring?.Wait();
			this._monitoring?.Dispose();
			this._monitoring = null;
		}
		#endregion
		#region Motion Profile
		double _commandPosition;
		public double CommandPosition
		{
			get => Interlocked.CompareExchange( ref this._commandPosition, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this._commandPosition, value );
				this.OnPropertyChanged( "CommandPosition" );
			}
		}

		double _actualPosition;
		public double ActualPosition
		{
			get => Interlocked.CompareExchange( ref this._actualPosition, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this._actualPosition, value );
				this.OnPropertyChanged( "ActualPosition" );
			}
		}

		double _errorPosition;
		public double ErrorPosition
		{
			get => Interlocked.CompareExchange( ref this._errorPosition, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this._errorPosition, value );
				this.OnPropertyChanged( "ErrorPosition" );
			}
		}

		double _acceleration;
		public double Acceleration
		{
			get => Interlocked.CompareExchange( ref this._acceleration, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this._acceleration, value );
				this.OnPropertyChanged( "Acceleration" );
			}
		}

		double _deceleration;
		public double Deceleration
		{
			get => Interlocked.CompareExchange( ref this._deceleration, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this._deceleration, value );
				this.OnPropertyChanged( "Deceleration" );
			}
		}

		double _velocity;
		public double Velocity
		{
			get => Interlocked.CompareExchange( ref this._velocity, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this._velocity, value );
				this.OnPropertyChanged( "Velocity" );
			}
		}
		#endregion
		#region I/O Status
		// motion io status

		int _ALM = 0;
		public bool ALM
		{
			get => Interlocked.CompareExchange( ref this._ALM, 0, 0 ) != 0;
			set
			{
				int Previous_State = this._ALM;
				Interlocked.Exchange( ref this._ALM, value ? 1 : 0 );
				if ( this._ALM != Previous_State && value == true )
					this.ALM_Err_Trig = true;

				this.OnPropertyChanged( "ALM" );
			}
		}

		int _ALM_Err_Trig = 0;
		public bool ALM_Err_Trig //KK 2020/05/25
		{
			get => Interlocked.CompareExchange( ref this._ALM_Err_Trig, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._ALM_Err_Trig, value ? 1 : 0 );
				this.OnPropertyChanged( "ALM_Err_Trig" );
			}
		}

		int _SVON = 0;
		public bool SVON
		{
			get => Interlocked.CompareExchange( ref this._SVON, 0, 0 ) != 0;
			set
			{
				int Previous_State = this._SVON;
				Interlocked.Exchange( ref this._SVON, value ? 1 : 0 );
				if ( this._SVON != Previous_State && value == false )
					this.SVON_Err_Trig = true;

				this.OnPropertyChanged( "SVON" );
			}
		}

		int _SVON_Err_Trig = 0;
		public bool SVON_Err_Trig //KK 2020/05/25
		{
			get => Interlocked.CompareExchange( ref this._SVON_Err_Trig, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._SVON_Err_Trig, value ? 1 : 0 );

				this.OnPropertyChanged( "SVON_Err_Trig" );
			}
		}

		int _ORG = 0;
		public bool ORG
		{
			get => Interlocked.CompareExchange( ref this._ORG, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._ORG, value ? 1 : 0 );

				this.OnPropertyChanged( "ORG" );
			}
		}

		int _PEL = 0;
		public bool PEL
		{
			get => Interlocked.CompareExchange( ref this._PEL, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._PEL, value ? 1 : 0 );

				this.OnPropertyChanged( "PEL" );
			}
		}

		int _NEL = 0;
		public bool NEL
		{
			get => Interlocked.CompareExchange( ref this._NEL, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._NEL, value ? 1 : 0 );

				this.OnPropertyChanged( "NEL" );
			}
		}

		//int _SPEL = 0;
		//public bool SPEL => Interlocked.CompareExchange( ref this._SPEL, 0, 0 ) != 0;
		//int _SMEL = 0;
		//public bool SMEL => Interlocked.CompareExchange( ref this._SMEL, 0, 0 ) != 0;

		int _EMG = 0;
		public bool EMG
		{
			get => Interlocked.CompareExchange( ref this._EMG, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._EMG, value ? 1 : 0 );

				this.OnPropertyChanged( "EMG" );
			}
		}

		int _INP = 0;
		public bool INP
		{
			get => Interlocked.CompareExchange( ref this._INP, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._INP, value ? 1 : 0 );

				this.OnPropertyChanged( "INP" );
			}
		}

		int _RDY = 0;
		public bool RDY
		{
			get => Interlocked.CompareExchange( ref this._RDY, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._RDY, value ? 1 : 0 );

				this.OnPropertyChanged( "RDY" );
			}
		}


		// motion status
		int _HM = 0;
		public bool HM
		{
			get => Interlocked.CompareExchange( ref this._HM, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._HM, value ? 1 : 0 );

				this.OnPropertyChanged( "HM" );
			}
		}

		int _CSTP = 0;
		public bool CSTP
		{
			get => Interlocked.CompareExchange( ref this._CSTP, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._CSTP, value ? 1 : 0 );

				this.OnPropertyChanged( "CSTP" );
			}
		}

		int _NSTP = 0;
		public bool NSTP
		{
			get => Interlocked.CompareExchange( ref this._NSTP, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._NSTP, value ? 1 : 0 );

				this.OnPropertyChanged( "NSTP" );
			}
		}

		int _ASTP = 0;
		public bool ASTP
		{
			get => Interlocked.CompareExchange( ref this._ASTP, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._ASTP, value ? 1 : 0 );

				this.OnPropertyChanged( "ASTP" );
			}
		}

		int _ACC = 0;
		public bool ACC
		{
			get => Interlocked.CompareExchange( ref this._ACC, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._ACC, value ? 1 : 0 );

				this.OnPropertyChanged( "ACC" );
			}
		}

		int _DEC = 0;
		public bool DEC
		{
			get => Interlocked.CompareExchange( ref this._DEC, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._DEC, value ? 1 : 0 );

				this.OnPropertyChanged( "DEC" );
			}
		}

		int _VM = 0;
		public bool VM
		{
			get => Interlocked.CompareExchange( ref this._VM, 0, 0 ) != 0;
			set
			{
				Interlocked.Exchange( ref this._VM, value ? 1 : 0 );
				this.OnPropertyChanged( "VM" );
			}
		}
		#endregion
	}
}
