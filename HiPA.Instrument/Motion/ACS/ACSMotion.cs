using HiPA.Common;
using HiPA.Common.Forms;
using SPIIPLUSCOM660Lib;
using System;
using System.Diagnostics;
using System.Threading;


namespace HiPA.Instrument.Motion.ACS
{
	[Serializable]
	public class ACSAxisConfiguration
		: AxisConfiguration
	{

		[NonSerialized]
		private static readonly ACSAxisConfiguration _DEFAULT = new ACSAxisConfiguration();

		private string s_Name = string.Empty;
		public override string Name
		{
			get => this.s_Name;
			set => this.Set( ref this.s_Name, value, "Name" );
		}
		public override InstrumentCategory Category => InstrumentCategory.Axis;
		public override Type InstrumentType => typeof( ACSAxis );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();

		private int i_AxisId = 0;
		public override int AxisId
		{
			get => this.i_AxisId;
			set => this.Set( ref this.i_AxisId, value, "AxisId" );
		}
		/// <summary>
		/// Pulse/mm
		/// </summary>
		/// 
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

		public int TOHome
		{
			get => this.GetValue( () => this.TOHome );
			set => this.SetValue( () => this.TOHome, value );
		}
		public int TOServoOn
		{
			get => this.GetValue( () => this.TOServoOn );
			set => this.SetValue( () => this.TOServoOn, value );
		}
		public override HomingProfile HomingProfile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public override NegativeDirection NegativeDirection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
	public class ACSAxis
		: AxisBase
	{
		public override MachineVariant MachineVar { get; set; }
		ACSAxisConfiguration _configuration = null;
		public override AxisConfiguration Configuration => this._configuration;

		public override string Name => this.Configuration?.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Axis;

		public ACSMotionBoard Board => this.BoardBase as ACSMotionBoard;
		private Channel Comm => this.Board?.Comm;
		public override int AxisId => this.Configuration.AxisId;

		public override MotionStatusBase Status { get; protected set; }
		protected override string OnCreate()
		{
			this.ClearErrorFlags();
			try
			{
				if ( this.Status == null )
					this.Status = new ACSMotionStatus( this );
				this.Status?.Start();
				return string.Empty;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		bool isFirstInit = true;
		protected override string OnInitialize()
		{
			this.ClearErrorFlags();
			try
			{
				this.CheckAndThrowIfError( this.ServoOn( true ).Result );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		protected override string OnTerminate()
		{
			this.ClearErrorFlags();
			try
			{
				this.Status?.Stop();
				if ( this.Board.IsOpen() == false ) return string.Empty;
				this.CheckAndThrowIfError( this.StopMove() );
				this.CheckAndThrowIfError( this.ServoOn( false ).Result );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		public override void ApplyConfiguration( Configuration configuration )
		{
			this._configuration = configuration as ACSAxisConfiguration;
		}

		#region Axis Operation 

		protected override string OnServoOn( bool On )
		{
			this.ClearErrorFlags();
			try
			{
				if ( MachineStateMng.isSimulation ) return String.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				//this.UpdateStatus();
				this.CheckAndThrowIfError( On ? this.Board.ServoOn( this._configuration.AxisId, this._configuration.TOServoOn ) : this.Board.ServoOff( this._configuration.AxisId ) );
				Thread.Sleep( 1 );
				this.Status.UpdateStatus();
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}
		protected override string OnHoming()
		{
			this.ClearErrorFlags();
			if ( MachineStateMng.isSimulation ) return this.Result.ErrorMessage;
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
				this.CheckAndThrowIfError( this.Board.Home( this._configuration.AxisId, this._configuration.TOHome ) );
				this.Status.SVON_Err_Trig = false;
				this.Status.ALM_Err_Trig = false;
				this.InternalProvider.SetSafe();
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		protected override string OnSetAxisProfile( MotionProfile profile )
		{
			this.ClearErrorFlags();
			try
			{
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				var scale = this.Configuration.MotionScale;
				var acc = profile.Acceleration * scale;
				var dec = profile.Deceleration * scale;
				var killdec = profile.KillDeceleration * scale;
				var vel = profile.Velocity * scale;
				this.CheckAndThrowIfError( this.Board.SetVelocity( this.Configuration.AxisId, vel ) );
				this.CheckAndThrowIfError( this.Board.SetAcceleration( this.Configuration.AxisId, acc ) );
				this.CheckAndThrowIfError( this.Board.SetDeceleration( this.Configuration.AxisId, dec ) );
				this.CheckAndThrowIfError( this.Board.SetJerk( this.Configuration.AxisId, profile.Jerk ) );
				this.CheckAndThrowIfError( this.Board.SetKillDeceleration( this.Configuration.AxisId, killdec ) );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		protected override string OnAbsoluteMove( Trajectory trajectory )
		{
			this.ClearErrorFlags();
			try
			{
				if ( MachineStateMng.isSimulation ) return this.Result.ErrorMessage;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				if ( !this.Interlocks.IsSafeToOperate() ) throw new Exception( $"^{( int )RunErrors.ERR_InterlockInEffect}^" );
				this.Status.UpdateStatus();
				if ( !this.Status.SVON )
					throw new Exception( $"Axis[{this.AxisId}] is disabled" );
				if ( this.Status.ALM )
					throw new Exception( $"Axis[{this.AxisId}] is in Alarm State." );
				var scale = this.Configuration.MotionScale;

				var pos = trajectory.Position * scale;

				if ( !this.isValid( trajectory.Position ) ) this.CheckAndThrowIfError( ErrorClass.E5, $"Axis[{this.AxisId}] trajectory pos of {trajectory.Position} over limit" );
				this.CheckAndThrowIfError( this.OnSetAxisProfile( trajectory ) );
				this.InternalProvider.SetUnsafe();
				this.CheckAndThrowIfError( this.Board.AbsoluteMove( this._configuration.AxisId, pos ) );
				this.CheckAndThrowIfError( this.WaitMoveComplete( pos ) );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		protected override string OnRelativeMove( Trajectory trajectory, bool WaitDone = true )
		{
			this.ClearErrorFlags();
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
				this.CheckAndThrowIfError( this.GetCommandPosition( ref Command_Pos ) );
				var scale = this.Configuration.MotionScale;
				int dist = ( int )( trajectory.Distance * scale );
				int speed = ( int )( trajectory.Velocity * scale );

				var ScaledCommandPos = Command_Pos / scale;
				if ( !this.isValid( ScaledCommandPos + trajectory.Distance ) ) this.CheckAndThrowIfError( ErrorClass.E5, $"Axis[{this.AxisId}] trajectory pos of {( ScaledCommandPos + trajectory.Distance )} over limit" );
				this.CheckAndThrowIfError( this.OnSetAxisProfile( trajectory ) );
				this.InternalProvider.SetUnsafe();
				this.CheckAndThrowIfError( this.Board.RelativeMove( this._configuration.AxisId, dist ) );
				if ( WaitDone ) this.CheckAndThrowIfError( this.WaitMoveComplete( Command_Pos + dist ) );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		protected override string OnVelocityMove( Trajectory trajectory )
		{
			this.ClearErrorFlags();
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
				this.CheckAndThrowIfError( this.OnSetAxisProfile( trajectory ) );
				this.InternalProvider.SetUnsafe();
				this.CheckAndThrowIfError( this.Board.VelocityMove( this._configuration.AxisId, speed ) );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		int _isStopMove = 0;
		public override string StopMove()
		{
			this.ClearErrorFlags();
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				this.Status.UpdateStatus();
				if ( !this.Status.SVON )
					throw new Exception( $"Axis[{this.AxisId}] is disabled" );
				Interlocked.Exchange( ref this._isStopMove, 1 );
				this.CheckAndThrowIfError( this.Board.KillMotor( this.Configuration.AxisId ) );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Interlocked.Exchange( ref this._isStopMove, 0 );
			}
			return this.Result.ErrorMessage;
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
				this.CheckAndThrowIfError( this.Board.GetPosition( this.Configuration.AxisId, ref pos ) );
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
				this.CheckAndThrowIfError( this.Board.GetPosition( this.Configuration.AxisId, ref pos ) );
				position = ( double )pos / this.Configuration.MotionScale;
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override string GetErrorPosition( ref double position )// not used in ACS
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
				position = 0;
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
				this.CheckAndThrowIfError( this.Board.SetPosition( this.AxisId, pos ) );
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
				this.CheckAndThrowIfError( this.Board.SetPosition( this.AxisId, pos ) );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}


		public override string WaitHomingComplete()//not required in ACS
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override string WaitMoveComplete( double? pos = null )
		{
			var result = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( !this.Board.IsOpen() )
					throw new Exception( $"MotionBoard[{this.Board.Name}] has not open." );

				var axisId = this.AxisId;
				Stopwatch SW = new Stopwatch();
				SW.Restart();
				while ( Interlocked.CompareExchange( ref this._isStopMove, 0, 0 ) == 0 && SW.ElapsedMilliseconds < 15000 )
				{
					this.Status.UpdateStatus();
					if ( this.Status.ALM ) this.CheckAndThrowIfError( ErrorClass.E5, "Axis in Alarm state." );
					if ( !this.Status.NSTP && this.Status.INP ) break;
					Thread.Sleep( 10 );
				}
				if ( pos != null )
				{
					SW.Restart();
					Stopwatch settlingTimer = new Stopwatch();
					var Position = 0d;
					var ErrorPos = 1d;
					while ( SW.ElapsedMilliseconds < 300 )
					{
						this.GetActualPosition( ref Position );
						var ErrAmplitude = Math.Abs( Position - ( double )pos );
						//checking Err < 10um for a continuous 50ms
						if ( ErrAmplitude < 0.01 && !settlingTimer.IsRunning )
							settlingTimer.Restart();
						else if ( ErrAmplitude >= 0.01 )
							settlingTimer.Stop();
						else if ( settlingTimer.ElapsedMilliseconds >= 50 )
							break;
						Thread.Sleep( 10 );
					}
					if ( ErrorPos > 0.01 )
						throw new Exception( $"{this.Name} {this.AxisId}:,ErrorPosition:,{ErrorPos}" );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		#endregion
		#region Motion I/O control

		public override void ApplyRecipe( RecipeBaseUtility recipeItem )
		{
			throw new NotImplementedException();
		}
		#endregion
	}


	public class ACSMotionStatus
		: MotionStatusBase
	{
		ACSAxis _axis = null;
		public override AxisBase Axis { get => this._axis; set => this._axis = value as ACSAxis; }

		public ACSMotionStatus( ACSAxis axis )
		{
			this._axis = axis;
		}

		public override void UpdateStatus() //KK 2020/05/25
		{
			if ( this._axis.Board.IsOpen() == false ) return;

			var axisId = this.Axis.AxisId;
			var m_state = 0;
			var fault = 0;

			if ( axisId >= 0 )
			{
				this._axis.Board.GetMState( axisId, ref m_state );
				this._axis.Board.GetFault( axisId, ref fault );
			}

			// motion io status 
			this.ALM = fault != 0;
			this.PEL = Convert.ToBoolean( fault & this._axis.Board.Comm.ACSC_SAFETY_RL );
			this.NEL = Convert.ToBoolean( fault & this._axis.Board.Comm.ACSC_SAFETY_LL );
			//this.ORG = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_ORG ) ) != 0;
			//this.EMG = ( ioStatus & ( 0x01 << AdLinkAxisConfiguration.MIO_EMG ) ) != 0;
			this.INP = Convert.ToBoolean( m_state & this._axis.Board.Comm.ACSC_MST_INPOS );
			this.RDY = Convert.ToBoolean( m_state & this._axis.Board.Comm.ACSC_MST_ENABLE );
			this.SVON = Convert.ToBoolean( m_state & this._axis.Board.Comm.ACSC_MST_ENABLE );

			// motion status
			//this.CSTP = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_CSTP ) ) != 0;
			//this.VM = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_VM ) ) != 0;
			this.ACC = Convert.ToBoolean( m_state & this._axis.Board.Comm.ACSC_MST_ACC );
			//this.DEC = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_DEC ) ) != 0;
			this.NSTP = Convert.ToBoolean( m_state & this._axis.Board.Comm.ACSC_MST_MOVE );
			//this.HM = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_HMV ) ) != 0;
			//this.ASTP = ( status & ( 0x01 << AdLinkAxisConfiguration.MTS_ASTP ) ) != 0;
		}

		protected override void UpdateProfile()
		{
			if ( this._axis.Board.IsOpen() == false ) return;

			var axisId = this.Axis.AxisId;
			var scale = this.Axis.Configuration.MotionScale;
			//bool temp;
			var value = 0d;
			this._axis.Board.GetPosition( axisId, ref value );
			this.CommandPosition = value / scale;
			this.ActualPosition = value / scale;
		}
	}
}
