using HiPA.Common;
using HiPA.Common.Forms;
using System;
using System.Diagnostics;
using System.Threading;


namespace HiPA.Instrument.Motion.APS
{
	[Serializable]
	public class AdLinkAxisConfiguration
		: AxisConfiguration
	{
		public static int MIO_ALM = 0;      // Servo alarm.
		public static int MIO_PEL = 1;       // Positive end limit.
		public static int MIO_MEL = 2;      // Negative end limit.
		public static int MIO_ORG = 3;       // ORG (Home)
		public static int MIO_EMG = 4;       // Emergency stop
		public static int MIO_EZ = 5;        // EZ.
		public static int MIO_INP = 6;       // In position.
		public static int MIO_SVON = 7;      // Servo on signal.
		public static int MIO_RDY = 8;       // Ready.
		public static int MIO_WARN = 9;     // Warning.
		public static int MIO_ZSP = 10;      // Zero speed.
		public static int MIO_SPEL = 11;     // Soft positive end limit.
		public static int MIO_SMEL = 12;    // Soft negative end limit.
		public static int MIO_TLC = 13;      // Torque is limited by torque limit value.
		public static int MIO_ABSL = 14;     // Absolute position lost.
		public static int MIO_STA = 15;     // External start signal.

		// Motion status bit number define.
		public static int MTS_CSTP = 0;     // Command stop signal. 
		public static int MTS_VM = 1;   // At maximum velocity.
		public static int MTS_ACC = 2;   // In acceleration.
		public static int MTS_DEC = 3;   // In deceleration.
		public static int MTS_DIR = 4;   // (Last)Moving direction.
		public static int MTS_NSTP = 5; // Normal stop(Motion done).
		public static int MTS_HMV = 6;    // In home operation.
		public static int MTS_SMV = 7;   // Single axis move( relative, absolute, velocity move).
		public static int MTS_LIP = 8;   // Linear interpolation.
		public static int MTS_CIP = 9;   // Circular interpolation.
		public static int MTS_VS = 10;   // At start velocity.
		public static int MTS_PMV = 11;   // Point table move.
		public static int MTS_PDW = 12;   // Point table dwell move.
		public static int MTS_PPS = 13;   // Point table pause state.
		public static int MTS_SLV = 14;   // Slave axis move.
		public static int MTS_JOG = 15;   // Jog move.
		public static int MTS_ASTP = 16;  // Abnormal stop.

		public static int MTS_SVONS = 17;  // Servo off stopped.
		public static int MTS_EMGS = 18;  // EMG / SEMG stopped.
		public static int MTS_ALMS = 19;  // Alarm stop.
		public static int MTS_WANS = 20;  // Warning stopped.
		public static int MTS_PELS = 21;  // PEL stopped.
		public static int MTS_MELS = 22;  // MEL stopped.
		public static int MTS_ECES = 23;  // Error counter check level reaches and stopped.
		public static int MTS_SPELS = 24;  // Soft PEL stopped.
		public static int MTS_SMELS = 25;  // Soft MEL stopped.
		public static int MTS_STPOA = 26;  // Stop by others axes.
		public static int MTS_GDCES = 27;  //Gantry deviation error level reaches and stopped.
		public static int MTS_GTM = 28; // Gantry mode turn on.
		public static int MTS_PAPB = 29;  // Pulsar mode turn on.



		[NonSerialized]
		private static readonly AdLinkAxisConfiguration _DEFAULT = new AdLinkAxisConfiguration();

		private string s_Name = string.Empty;
		public override string Name
		{
			get => this.s_Name;
			set => this.Set( ref this.s_Name, value, "Name" );
		}
		public override InstrumentCategory Category => InstrumentCategory.Axis;
		public override Type InstrumentType => typeof( AdLinkAxis );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();

		private int i_AxisId = 0;
		public override int AxisId
		{
			get => this.i_AxisId;
			set => this.Set( ref this.i_AxisId, value, "AxisId" );
		}
		private int i_ModuleNo = 0;
		public override int ModuleNo
		{
			get => this.i_ModuleNo;
			set => this.Set( ref this.i_ModuleNo, value, "ModuleNo" );
		}
		private HomingProfile c_HomingProfile = new HomingProfile();
		public override HomingProfile HomingProfile
		{
			get => this.c_HomingProfile;
			set => this.Set( ref this.c_HomingProfile, value, "HomingProfile" );
		}

		private NegativeDirection e_NegativeDirection = NegativeDirection.Left;
		public override NegativeDirection NegativeDirection
		{
			get => this.e_NegativeDirection;
			set => this.Set( ref this.e_NegativeDirection, value, "NegativeDirection" );
		}
		/// <summary>
		/// Pulse/mm
		/// </summary>
		/// 
		private double d_MotionScale = 0;
		public override double MotionScale
		{
			get => this.d_MotionScale;
			set => this.Set( ref this.d_MotionScale, value, "MotionScale" );
		}
		private double d_DelayBeforeSetZero = 500d;
		public override double DelayBeforeSetZero
		{
			get => this.d_DelayBeforeSetZero;
			set => this.Set( ref this.d_DelayBeforeSetZero, value, "DelayBeforeSetZero" );
		}
		private double d_FineMoveVelocity = 5d;
		public override double FineMoveVelocity
		{
			get => this.d_FineMoveVelocity;
			set => this.Set( ref this.d_FineMoveVelocity, value, "FineMoveVelocity" );
		}
		private double d_CoarseMoveVelocity = 50d;
		public override double CoarseMoveVelocity
		{
			get => this.d_CoarseMoveVelocity;
			set => this.Set( ref this.d_CoarseMoveVelocity, value, "CoarseMoveVelocity" );
		}
		private double d_MaxVelocity = 0;
		public override double MaxVelocity
		{
			get => this.d_MaxVelocity;
			set => this.Set( ref this.d_MaxVelocity, value, "MaxVelocity" );
		}
		private double d_MaxAcceleration = 0;
		public override double MaxAcceleration
		{
			get => this.d_MaxAcceleration;
			set => this.Set( ref this.d_MaxAcceleration, value, "MaxAcceleration" );
		}
		private double d_MaxDeceleration = 0;
		public override double MaxDeceleration
		{
			get => this.d_MaxDeceleration;
			set => this.Set( ref this.d_MaxDeceleration, value, "MaxDeceleration" );
		}
		private double? d_UpperPosLimit = 0;
		public override double? UpperPosLimit
		{
			get => this.d_UpperPosLimit;
			set => this.Set( ref this.d_UpperPosLimit, value, "UpperPosLimit" );
		}
		private double? d_LowerPosLimit = 0;
		public override double? LowerPosLimit
		{
			get => this.d_LowerPosLimit;
			set => this.Set( ref this.d_LowerPosLimit, value, "LowerPosLimit" );
		}
		private MotionProfile c_GeneralMove = new MotionProfile();
		public override MotionProfile GeneralMove
		{
			get => this.c_GeneralMove;
			set => this.Set( ref this.c_GeneralMove, value, "GeneralMove" );
		}
		private MotionProfile c_CommandedMove = new MotionProfile();
		public override MotionProfile CommandedMove
		{
			get => this.c_CommandedMove;
			set => this.Set( ref this.c_CommandedMove, value, "CommandedMove" );
		}
		private DisplacementUnits e_Units = DisplacementUnits.Millimeter;
		public override DisplacementUnits Units
		{
			get => this.e_Units;
			set => this.Set( ref this.e_Units, value, "Units" );
		}
	}


	public class AdLinkAxis
		: AxisBase
	{
		public override MachineVariant MachineVar { get; set; }
		AdLinkAxisConfiguration _configuration = null;
		public override AxisConfiguration Configuration => this._configuration;

		public override string Name => this.Configuration?.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Axis;

		public AdLinkMotionBoard Board => this.BoardBase as AdLinkMotionBoard;
		public override int AxisId => this.Configuration.AxisId;

		public override MotionStatusBase Status { get; protected set; }
		protected override string OnCreate()
		{
			if ( this.Status == null )
				this.Status = new AdLinkMotionStatus( this );
			this.Status.Start();
			return string.Empty;
		}

		bool isFirstInit = true;
		protected override string OnInitialize()
		{
			var result = string.Empty;
			try
			{
				this.CheckAndThrowIfError( this.ServoOn( true ).Result );
				if ( this.isFirstInit )
				{
					this.isFirstInit = false;

					this.ReadHomeProfile( this.Configuration.MotionScale, out var profile );
					this.Configuration.HomingProfile = profile;
				}

				//t = this.Homing();
				//if ( ( result = t.Result ) != string.Empty ) throw new Exception( result );
				//if ( this._homingAbort == true ) throw new Exception( "Homing Aborted" );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnTerminate()
		{
			var result = string.Empty;
			try
			{
				this.Status.Stop();
				if ( this.Board.IsOpen() == false ) return string.Empty;
				if ( ( result = this.StopMove() ) != string.Empty ) return result;

				var t = this.ServoOn( false );
				if ( ( result = t.Result ) != string.Empty ) return result;

				this.Status?.Stop();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override void ApplyConfiguration( Configuration configuration )
		{
			this._configuration = configuration as AdLinkAxisConfiguration;
		}

		#region Axis Operation 

		protected override string OnServoOn( bool On )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return String.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				//this.UpdateStatus();
				int num;
				if ( ( num = APS168.APS_set_servo_on( this.AxisId, On ? 1 : 0 ) ) != 0 )
					throw new Exception( $"{PCI7856.GetErrorDesc( num )}" );
				Thread.Sleep( 1 );
				this.Status.UpdateStatus();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		protected override string OnHoming()
		{
			var result = string.Empty;
			if ( MachineStateMng.isSimulation ) return result;
			try
			{
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
				this.OnServoOn( true );
				this.Status.UpdateStatus();
				if ( !this.Status.SVON )
					throw new Exception( $"Axis[{this.AxisId}] is disabled" );
				if ( this.Status.ALM )
					throw new Exception( $"Axis[{this.AxisId}] is in Alarm State." );
				var profile = this.Configuration.HomingProfile;
				var scale = this.Configuration.MotionScale;
				int resultint;
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_MODE, ( int )profile.HOME_MODE ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_DIR, ( int )profile.HOME_DIR ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_VM, ( int )( profile.HOME_VM * scale ) ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_EZA, ( int )( profile.HOME_EZ ) ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_VO, ( int )( profile.HOME_VO * scale ) ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( ( resultint = APS168.APS_home_move( this.AxisId ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );

				if ( ( result = this.WaitHomingComplete() ) != String.Empty )
					throw new Exception( result );
				if ( this.Configuration.DelayBeforeSetZero > 0 )
					Thread.Sleep( ( int )this.Configuration.DelayBeforeSetZero );

				this.Status.SVON_Err_Trig = false;
				this.Status.ALM_Err_Trig = false;

				this.SetZero();
				this.InternalProvider.SetSafe();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected void ReadHomeProfile( double scale, out HomingProfile profile )
		{
			var value = 0;
			profile = new HomingProfile();
			if ( MachineStateMng.isSimulation ) return;
			APS168.APS_get_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_MODE, ref value ); profile.HOME_MODE = value;
			APS168.APS_get_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_DIR, ref value ); profile.HOME_DIR = ( HomingDirection )value;
			APS168.APS_get_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_VM, ref value ); profile.HOME_VM = value / scale;
			APS168.APS_get_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_EZA, ref value ); profile.HOME_EZ = value;
			APS168.APS_get_axis_param( this.AxisId, ( int )APS_Define.PRA_HOME_VO, ref value ); profile.HOME_VO = value / scale;
		}

		protected override string OnSetAxisProfile( MotionProfile profile )
		{
			var result = string.Empty;
			try
			{
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				var scale = this.Configuration.MotionScale;
				var acc = profile.Acceleration * scale;
				var dec = profile.Deceleration * scale;

				int resultint;
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_CURVE, 1 ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_ACC, ( int )acc ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_DEC, ( int )dec ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( this.Board.Configuration.AdlinkModel == AdlinkModel.PCI7856 )
				{
					if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_ACC_SR, ( int )profile.PRA_ACC_SR ) ) != 0 )
						throw new Exception( PCI7856.GetErrorDesc( resultint ) );
					if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_DEC_SR, ( int )profile.PRA_DEC_SR ) ) != 0 )
						throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				}
				if ( ( resultint = APS168.APS_set_axis_param( this.AxisId, ( int )APS_Define.PRA_VS, 0 ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnAbsoluteMove( Trajectory trajectory )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
				this.Status.UpdateStatus();
				if ( !this.Status.SVON )
					throw new Exception( $"Axis[{this.AxisId}] is disabled" );
				if ( this.Status.ALM )
					throw new Exception( $"Axis[{this.AxisId}] is in Alarm State." );
				var scale = this.Configuration.MotionScale;

				int pos = ( int )( trajectory.Position * scale );
				int speed = ( int )( trajectory.Velocity * scale );

				if ( !this.isValid( trajectory.Position ) )
					throw new Exception( $"Axis[{this.AxisId}] trajectory pos of {trajectory.Position} over limit" );
				int resultint;
				if ( ( result = this.OnSetAxisProfile( trajectory ) ) != string.Empty ) throw new Exception( result );
				this.InternalProvider.SetUnsafe();
				if ( ( resultint = APS168.APS_absolute_move( this.AxisId, pos, speed ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( ( result = this.WaitMoveComplete() ) != string.Empty ) throw new Exception( result );

			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnRelativeMove( Trajectory trajectory, bool WaitDone = true )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
				this.Status.UpdateStatus();
				if ( !this.Status.SVON )
					throw new Exception( $"Axis[{this.AxisId}] is disabled" );
				if ( this.Status.ALM )
					throw new Exception( $"Axis[{this.AxisId}] is in Alarm State." );
				double Command_Pos = 0;
				if ( ( result = this.GetCommandPosition( ref Command_Pos ) ) != string.Empty )
					throw new Exception( result );
				var scale = this.Configuration.MotionScale;

				int dist = ( int )( trajectory.Distance * scale );
				int speed = ( int )( trajectory.Velocity * scale );
				var ScaledCommandPos = Command_Pos / scale;
				if ( !this.isValid( ScaledCommandPos + trajectory.Distance ) )
					throw new Exception( $"Axis[{this.AxisId}] trajectory pos of {( ScaledCommandPos + trajectory.Distance )} over limit" );
				int resultint;
				if ( ( result = this.OnSetAxisProfile( trajectory ) ) != string.Empty ) throw new Exception( result );
				this.InternalProvider.SetUnsafe();
				if ( ( resultint = APS168.APS_relative_move( this.AxisId, dist, speed ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( WaitDone ) if ( ( result = this.WaitMoveComplete() ) != string.Empty ) throw new Exception( result );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnVelocityMove( Trajectory trajectory )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
				this.Status.UpdateStatus();
				if ( !this.Status.SVON )
					throw new Exception( $"Axis[{this.AxisId}] is disabled" );
				if ( this.Status.ALM )
					throw new Exception( $"Axis[{this.AxisId}] is in Alarm State." );

				var scale = this.Configuration.MotionScale;
				int speed = ( int )( trajectory.Velocity * scale );

				int resultint;
				if ( ( result = this.OnSetAxisProfile( trajectory ) ) != string.Empty ) throw new Exception( result );
				this.InternalProvider.SetUnsafe();
				if ( ( resultint = APS168.APS_velocity_move( this.AxisId, speed ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				//if ( ( result = this.WaitMoveComplete() ) != string.Empty ) throw new Exception( result );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		int _isStopMove = 0;
		public override string StopMove()
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				this.Status.UpdateStatus();
				if ( !this.Status.SVON )
					throw new Exception( $"Axis[{this.AxisId}] is disabled" );
				Interlocked.Exchange( ref this._isStopMove, 1 );
				int resultint;
				if ( ( resultint = APS168.APS_stop_move( this.AxisId ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				Interlocked.Exchange( ref this._isStopMove, 0 );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override string GetCommandPosition( ref double position )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				var pos = 0d;
				int resultint;
				if ( ( resultint = APS168.APS_get_command_f( this.AxisId, ref pos ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				position = ( double )pos / this.Configuration.MotionScale;
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override string GetActualPosition( ref double position )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				var pos = 0d;
				int resultint;
				if ( ( resultint = APS168.APS_get_position_f( this.AxisId, ref pos ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				position = ( double )pos / this.Configuration.MotionScale;
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override string GetErrorPosition( ref double position )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				var pos = 0d;
				int resultint;
				if ( ( resultint = APS168.APS_get_error_position_f( this.AxisId, ref pos ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				position = ( double )pos / this.Configuration.MotionScale;
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public int SetZero()
		{
			this.SetCommandPosition( 0 );
			this.SetActualPosition( 0 );
			return 0;
		}

		public override string SetCommandPosition( double pos )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				pos *= this.Configuration.MotionScale;
				int resultint;
				if ( ( resultint = APS168.APS_set_command_f( this.AxisId, pos ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		public override string SetActualPosition( double pos )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				pos *= this.Configuration.MotionScale;
				int resultint;
				if ( ( resultint = APS168.APS_set_position_f( this.AxisId, pos ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}


		public override string WaitHomingComplete()
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				var axisId = this.AxisId;
				while ( Interlocked.CompareExchange( ref this._isStopMove, 0, 0 ) == 0 )
				{
					var ioStatus = APS168.APS_motion_io_status( axisId );
					var status = APS168.APS_motion_status( axisId );

					var ALM = ( ioStatus & 0x01 ) != 0;
					if ( ALM )
						throw new Exception( $"{this.AxisId} in ALM state." );

					var INP = ( ioStatus & ( 0x01 << 6 ) ) != 0;

					var CSTP = ( status & 0x01 ) != 0;
					var HM = ( status & ( 0x01 << 6 ) ) != 0;

					if ( ALM ) break;
					if ( CSTP && HM == false ) break;
					Thread.Sleep( 10 );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override string WaitMoveComplete( double? Position = null )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				var axisId = this.AxisId;

				var ErrorPos = 1d;
				Stopwatch SW = new Stopwatch();
				SW.Restart();
				while ( Interlocked.CompareExchange( ref this._isStopMove, 0, 0 ) == 0 && SW.ElapsedMilliseconds < 15000 )
				{
					var ioStatus = APS168.APS_motion_io_status( axisId );
					var status = APS168.APS_motion_status( axisId );
					var CSTP = ( status & 0x01 ) != 0;
					var HM = ( status & ( 0x01 << 6 ) ) != 0;

					var ALM = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_ALM ) ) != 0;
					var PEL = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_PEL ) ) != 0;
					var NEL = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_MEL ) ) != 0;
					var ORG = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_ORG ) ) != 0;
					var EMG = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_EMG ) ) != 0;
					var INP = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_INP ) ) != 0;
					var RDY = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_RDY ) ) != 0;
					var SVN = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_SVON ) ) != 0;

					if ( ALM ) break;
					if ( INP && CSTP ) break;
					Thread.Sleep( 10 );
				}
				SW.Restart();
				Stopwatch settlingTimer = new Stopwatch();

				while ( SW.ElapsedMilliseconds < 300 )
				{
					this.GetErrorPosition( ref ErrorPos );
					var ErrAmplitude = Math.Abs( ErrorPos );
					//checking Err < 10um for a continuous 50ms
					if ( ErrAmplitude < 0.01 && !settlingTimer.IsRunning )
						settlingTimer.Restart();
					else if ( ErrAmplitude >= 0.01 )
						settlingTimer.Stop();
					else if ( settlingTimer.ElapsedMilliseconds >= 50 )
						break;

					//if (/*this.AxisId == 2*/ true)
					//    Console.WriteLine($"{ this.Name} { this.AxisId}:, Time:,{SW.ElapsedMilliseconds},Errorpos:,{ErrorPos}");

					Thread.Sleep( 10 );
				}
				Console.WriteLine( $"{ this.Name} { this.AxisId}: Settling Time: {SW.ElapsedMilliseconds}, Errorpos:{ErrorPos}" );
				//Console.WriteLine("   ");
				if ( ErrorPos > 0.01 )
					throw new Exception( $"{this.Name} {this.AxisId}:,ErrorPosition:,{ErrorPos}" );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		#endregion
		#region Motion I/O control
		public string IOIF_Output( int slot, int port, bool on )
		{
			if ( this.Board.IsOpen() == false )
				return Equipment.ErrManager.RaiseError( this,
						$"MotionBoard[{this.Board.Name}] has not open. OP : WaitMoveComplete",
						ErrorTitle.InvalidOperation, ErrorClass.E4 );

			if ( slot < 0 || slot > 3 )
				return Equipment.ErrManager.RaiseError( this,
						$"MotionBoard[{this.Board.Name}] Motion IO slot number is invalid",
						ErrorTitle.InvalidArgument, ErrorClass.E4 );

			var resultint = 0;

			var boardId = this.Board.Configuration.CardID;
			var modBus = ( int )BoardPortId.MNET;
			var modNo = this.Configuration.ModuleNo;
			var value = 0;

			if ( ( resultint = APS168.APS_field_bus_d_get_output(
				boardId,
				modBus,
				modNo,
				ref value ) ) != 0 )
				return Equipment.ErrManager.RaiseError( this,
					PCI7856.GetErrorDesc( resultint ),
					ErrorTitle.OperationFailure, ErrorClass.E4 );

			var slotBit = 1 << slot;

			if ( port == 2 ) slotBit <<= 4;

			if ( on == true ) value |= slotBit;
			else value &= ~slotBit;

			if ( ( resultint = APS168.APS_field_bus_d_set_output(
				boardId,
				modBus,
				modNo,
				value ) ) != 0 )
				return Equipment.ErrManager.RaiseError( this,
					PCI7856.GetErrorDesc( resultint ),
					ErrorTitle.OperationFailure, ErrorClass.E4 );

			return string.Empty;
		}
		public string IOIF_Input( int slot, int port, ref bool on )
		{
			if ( this.Board.IsOpen() == false )
				return Equipment.ErrManager.RaiseError( this,
						$"MotionBoard[{this.Board.Name}] has not open. OP : WaitMoveComplete",
						ErrorTitle.InvalidOperation, ErrorClass.E4 );

			if ( slot < 0 || slot > 3 )
				return Equipment.ErrManager.RaiseError( this,
						$"MotionBoard[{this.Board.Name}] Motion IO slot number is invalid",
						ErrorTitle.InvalidArgument, ErrorClass.E4 );

			var result = 0;

			var boardId = this.Board.Configuration.CardID;
			var modBus = ( int )BoardPortId.MNET;
			var modNo = this.Configuration.ModuleNo;
			var value = 0;

			if ( ( result = APS168.APS_field_bus_d_get_output(
				boardId,
				modBus,
				modNo,
				ref value ) ) != 0 )
				return Equipment.ErrManager.RaiseError( this,
					PCI7856.GetErrorDesc( result ),
					ErrorTitle.OperationFailure, ErrorClass.E4 );

			var slotBit = 1 << slot;
			if ( port == 2 ) slotBit <<= 4;

			on = ( ( value & slotBit ) == slotBit );

			return string.Empty;
		}

		public override void ApplyRecipe( RecipeBaseUtility recipeItem )
		{
			throw new NotImplementedException();
		}
		#endregion
	}


	public class AdLinkMotionStatus
		: MotionStatusBase
	{
		AdLinkAxis _axis = null;
		public override AxisBase Axis { get => this._axis; set => this._axis = value as AdLinkAxis; }

		public AdLinkMotionStatus( AdLinkAxis axis )
		{
			this._axis = axis;
		}

		public override void UpdateStatus() //KK 2020/05/25
		{
			if ( this._axis.Board.IsOpen() == false ) return;

			var axisId = this.Axis.AxisId;
			var ioStatus = 0;
			var status = 0;

			if ( axisId >= 0 )
			{
				ioStatus = APS168.APS_motion_io_status( axisId );
				status = APS168.APS_motion_status( axisId );
			}

			// motion io status 
			this.ALM = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_ALM ) ) != 0;
			this.PEL = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_PEL ) ) != 0;
			this.NEL = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_MEL ) ) != 0;
			this.ORG = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_ORG ) ) != 0;
			this.EMG = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_EMG ) ) != 0;
			this.INP = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_INP ) ) != 0;
			this.RDY = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_RDY ) ) != 0;
			this.SVON = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_SVON ) ) != 0;
			//Interlocked.Exchange( ref this._SPEL,	ioStatus & ( 0x01 <<  ) );
			//Interlocked.Exchange( ref this._SMEL,	ioStatus & ( 0x01 <<  ) );

			// motion status
			this.CSTP = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_CSTP ) ) != 0;
			this.VM = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_VM ) ) != 0;
			this.ACC = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_ACC ) ) != 0;
			this.DEC = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_DEC ) ) != 0;
			this.NSTP = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_NSTP ) ) != 0;
			this.HM = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_HMV ) ) != 0;
			this.ASTP = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_ASTP ) ) != 0;
		}

		protected override void UpdateProfile()
		{
			if ( this._axis.Board.IsOpen() == false ) return;

			var axisId = this.Axis.AxisId;
			var scale = this.Axis.Configuration.MotionScale;
			//bool temp;
			var value = 0;
			if ( APS168.APS_get_command( axisId, ref value ) == 0 )
				this.CommandPosition = value / scale;
			if ( APS168.APS_get_position( axisId, ref value ) == 0 )
				this.ActualPosition = value / scale;
			if ( APS168.APS_get_error_position( axisId, ref value ) == 0 )
				this.ErrorPosition = value / scale;
		}
	}
}
