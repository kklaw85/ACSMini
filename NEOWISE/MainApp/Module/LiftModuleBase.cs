using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Utils;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using NeoWisePlatform.Sequence;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static HiPA.Instrument.Motion.APS.AdLinkMotionBoard;

namespace NeoWisePlatform.Module
{
	[Serializable]
	public class LiftModuleConfiguration
		: Configuration
	{
		[NonSerialized]
		private static readonly LiftModuleConfiguration _DEFAULT = new LiftModuleConfiguration();
		[NonSerialized]
		public static readonly string NAME = "Lift Module";

		public override string Name { get; set; }
		public override InstrumentCategory Category => InstrumentCategory.Module;
		public override Type InstrumentType => typeof( LiftModuleBase );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public int MovementTimeout
		{
			get => this.GetValue( () => this.MovementTimeout );
			set => this.SetValue( () => this.MovementTimeout, value );
		}
		public int PusherDuration
		{
			get => this.GetValue( () => this.PusherDuration );
			set => this.SetValue( () => this.PusherDuration, value );
		}
	}

	public class LiftModuleBase
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		#region General.Properties
		public LiftModuleConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Module;
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as LiftModuleConfiguration;
		}
		#endregion
		#region Sequence
		public LiftSeq Seq { get; set; }
		#endregion
		#region Parts 
		public AxisBase Lift { get; set; } = null;
		public IO UpperLimit { get; set; } = new IO();
		public IO LowerLimit { get; set; } = new IO();
		public IO Pusher { get; set; }
		public IO Blower { get; set; }
		#endregion
		#region General.Functions
		protected override string OnCreate()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{

				var axisenum = ( AxesList )Enum.Parse( typeof( AxesList ), this.Name + "Axis" );
				var axis = ReflectionTool.GetEnumAttribute<AxisAttribute>( axisenum );
				this.Lift = Constructor.GetInstance().GetInstrument( axis.Attribute.Name, null ) as AdLinkAxis;
				if ( this.Lift == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to get instance:,{axis.Attribute.Name}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.Lift.Owner = this;

				var tasks = new Task<string>[]
					{
					this.Lift.Create(),
					};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to create Obj:,{sErr}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
					//throw new Exception( $"Failed to create Obj:,{t.Result}" );	//original return value
				}
				this.Seq = new LiftSeq( this );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
			return result;
		}

		protected override string OnInitialize()
		{
			var sErr = string.Empty;
			bool InitOk = true;
			try
			{
				var tasks = new Task<string>[]
					{
					this.Lift.Initialize(),
					};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
					{
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.InitializeFailure, ErrorClass.E5 );
						InitOk = false;
					}
				}
				if ( !InitOk )
					throw new Exception( "Initialization Failed" );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, sErr, ErrorTitle.InitializeFailure, ErrorClass.E6 );
			}
			return sErr;
		}
		protected override string OnStop()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				var tasks = new Task<string>[]
					{
					this.Lift.Stop(),
					};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
			return result;
		}
		protected override string OnTerminate()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				var tasks = new Task<string>[]
				{
					this.Lift.Terminate(),
				};

				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
			return result;
		}
		#endregion
		#region Motion
		#region Absolute move
		private Task<ErrorResult> LiftMoveAbsolute( double Pos )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var trajectory = new Trajectory( this.Lift.Configuration.CommandedMove )
					{
						Position = Pos,
					};
					this.CheckAndThrowIfError( ErrorClass.E5, this.Lift.AbsoluteMove( trajectory ).Result );
					this.CheckAndThrowIfError( ErrorClass.E5, this.CheckLiftPosition( Pos ) );
				}
				catch ( Exception ex )
				{
					if ( this.Result.EClass == ErrorClass.OK )
						this.Result.EClass = ErrorClass.E6;
					this.Result.ErrorMessage = this.FormatErrMsg( this.Name, ex );
				}
				return this.Result;
			} );
		}
		#endregion
		#region Relative move
		private Task<ErrorResult> LiftMoveRel( double Pos )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var trajectory = new Trajectory( this.Lift.Configuration.CommandedMove )
					{
						Distance = Pos,
					};
					this.CheckAndThrowIfError( ErrorClass.E5, this.Lift.RelativeMove( trajectory ).Result );
					//this.CheckAndThrowIfError( ErrorClass.E5, this.CheckLiftPosition( Pos ) );
				}
				catch ( Exception ex )
				{
					if ( this.Result.EClass == ErrorClass.OK )
						this.Result.EClass = ErrorClass.E6;
					this.Result.ErrorMessage = this.FormatErrMsg( this.Name, ex );
				}
				return this.Result;
			} );
		}
		#endregion
		#region Home Axes
		public Task<ErrorResult> HomeLift()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.SingleAction = true;
					this.LiftPNPCom.ResetState();
					this.CheckAndThrowIfError( ErrorClass.E5, this.Lift.Homing().Result );
				}
				catch ( Exception ex )
				{
					if ( this.Result.EClass == ErrorClass.OK )
						this.Result.EClass = ErrorClass.E6;
					this.Result.ErrorMessage = this.FormatErrMsg( this.Name, ex );
				}
				finally
				{
					this.SingleAction = false;
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> Home()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var Tasks = new Task<ErrorResult>[]
					{
						this.HomeLift(),
					};
					this.CheckAndThrowIfError( Tasks );
				}
				catch ( Exception ex )
				{
					if ( this.Result.EClass == ErrorClass.OK )
						this.Result.EClass = ErrorClass.E6;
					this.Result.ErrorMessage = this.FormatErrMsg( this.Name, ex );
				}
				return this.Result;
			} );
		}
		#endregion
		#region CheckPosition
		public string CheckLiftPosition( double position, double tolerance = 0.02 )
		{
			try
			{
				return this.Lift.CheckPosition( position, tolerance );
			}
			catch ( Exception ex )
			{
				return Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, ErrorClass.E5 );
			}
		}
		#endregion
		#endregion
		#region StartAutoRun initialization
		public Task<ErrorResult> MoveToStandbyStatus()
		{
			return Task.Run( () =>
			 {
				 this.ClearErrorFlags();
				 try
				 {
					 if ( MachineStateMng.isSimulation ) return this.Result;
					 this.CheckAndThrowIfError( this.StartSingleAction( true ).Result );
					 this.CheckAndThrowIfError( this.StartAuto().Result );
					 this.LiftPNPCom.ResetState();
					 this.CheckAndThrowIfError( this.Pusher?.Off() );
					 this.CheckAndThrowIfError( this.BlowerOff() );
				 }
				 catch ( Exception ex )
				 {
					 this.CatchException( ex );
				 }
				 return this.Result;
			 } );
		}
		#endregion
		#region Lift action
		public bool isContinuous
		{
			get => this.GetValue( () => this.isContinuous );
			set => this.SetValue( () => this.isContinuous, value );
		}
		public bool SingleAction
		{
			get => this.GetValue( () => this.SingleAction );
			set => this.SetValue( () => this.SingleAction, value );
		}
		enum LiftMovement
		{
			Upward,
			Downward,
			Stop
		}
		public Task<ErrorResult> LiftTask { get; private set; } = null;
		private Stopwatch LiftWatchDogTimer = new Stopwatch();
		public LiftPNPComFlags LiftPNPCom { get; private set; } = new LiftPNPComFlags();
		private Task<ErrorResult> MoveToCollectPos( bool isContinuous )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					Monitor.Enter( this.SyncRoot );
					this.LiftPNPCom.ResetState();
					var Movement = LiftMovement.Stop;
					var Retry = 0;
					var HysteresisCount = 0;
					this.LiftWatchDogTimer.Restart();
					this.isContinuous = isContinuous;
					this.SingleAction = !isContinuous;
					while ( this.isContinuous || //for continuous auto mode
					( !this.LiftPNPCom.IsInPos && this.SingleAction ) )//for one time move to collection mode, but still need to loop through for stability check.
					{
						if ( this.LiftWatchDogTimer.ElapsedMilliseconds > this.Configuration.MovementTimeout ) this.CheckAndThrowIfError( ErrorClass.E5, "Movement exceeded time out." );//100second
						if ( !this.LowerLimit.State && !this.UpperLimit.State )
						{
							HysteresisCount = 0;
							Retry = 0;
							Movement = LiftMovement.Upward;
						}
						else if ( this.UpperLimit.State ) //( this.LowerLimit.State && this.UpperLimit.State )
						{
							HysteresisCount = 0;
							Retry = 0;
							Movement = LiftMovement.Downward;
						}
						else if ( !this.LowerLimit.State && this.UpperLimit.State )
						{
							HysteresisCount = 0;
							if ( Retry++ > 10 ) throw new Exception( "Faulty signalling. Please check if sensor is blocked" );
							else Thread.Sleep( 10 );
						}
						else
						{
							if ( HysteresisCount++ > 5 )
							{
								Retry = 0;
								Movement = LiftMovement.Stop;
								this.LiftPNPCom.SetInPos();
							}
							else
							{
								Thread.Sleep( 1 );
								continue;
							}
						}
						if ( Movement == LiftMovement.Upward )
						{
							if ( this.Lift.Status.PEL ) throw new Exception( "Lift has reached Positive limit. Please check on sensors alignment." );
							this.CheckAndThrowIfError( this.LiftMoveRel( 0.5 ).Result );
						}
						else if ( Movement == LiftMovement.Downward )
						{
							if ( this.Lift.Status.NEL ) throw new Exception( "Lift has reached Negative limit. Please check on sensors alignment." );
							this.CheckAndThrowIfError( this.LiftMoveRel( -0.5 ).Result );
						}
						else
						{
							this.LiftWatchDogTimer.Restart();
							Thread.Sleep( 5 );
						}
						Movement = LiftMovement.Stop;
					}
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
					this.isContinuous = false;
					this.SingleAction = false;
					this.LiftPNPCom.ResetState();
				}
				finally
				{
					this.SingleAction = false;
					Monitor.Exit( this.SyncRoot );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> StartSingleAction( bool WaitDone = false )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( this.LiftTask != null )
					{
						if ( this.isContinuous ) this.CheckAndThrowIfError( this.StopAuto().Result );
						else
						{
							this.LiftTask?.Wait();
							this.LiftTask?.Dispose();
							this.LiftTask = null;
						}
					}
					this.LiftTask = this.MoveToCollectPos( false );
					if ( WaitDone == true ) this.CheckAndThrowIfError( this.LiftTask.Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
					this.isContinuous = false;
					this.SingleAction = false;
				}
				finally
				{
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> StartAuto()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( this.LiftTask != null )
					{
						if ( !this.isContinuous ) this.LiftTask.Wait();
						else return this.Result;
					}
					this.LiftTask = this.MoveToCollectPos( true );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
					this.isContinuous = false;
					this.SingleAction = false;
				}
				finally
				{
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> OffBlowerandPusher()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.CheckAndThrowIfError( this.BlowerOff() );
					this.CheckAndThrowIfError( this.Pusher?.Off() );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				finally
				{
					this.SingleAction = false;
					//Monitor.Exit( this.SyncRoot );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> StopAuto()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					//Monitor.Enter( this.SyncRoot );
					this.isContinuous = false;
					this.LiftTask?.Wait();
					this.LiftTask?.Dispose();
					this.LiftTask = null;
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				finally
				{
					this.SingleAction = false;
					//Monitor.Exit( this.SyncRoot );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> MoveToLoadPos( bool WaitDone = false )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.CheckAndThrowIfError( this.StopAuto().Result );
					this.LiftTask = this.HomeLift();
					if ( WaitDone ) this.CheckAndThrowIfError( this.LiftTask.Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				finally
				{
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> TrigPusher()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.CheckAndThrowIfError( this.Pusher?.On() );
					Thread.Sleep( this.Configuration.PusherDuration );
					this.CheckAndThrowIfError( this.Pusher?.Off() );
				}
				catch ( Exception ex )
				{
					this.Pusher?.Off();
					this.CatchAndPromptErr( ex );
				}
				finally
				{
				}
				return this.Result;
			} );
		}
		private ErrorResult BlowerOn()
		{
			this.ClearErrorFlags();
			try
			{
				this.Blower?.On();
			}
			catch ( Exception ex )
			{
				this.Blower?.Off();
				this.CatchAndPromptErr( ex );
			}
			finally
			{
			}
			return this.Result;
		}
		private ErrorResult BlowerOff()
		{
			this.ClearErrorFlags();
			try
			{
				this.Blower?.Off();
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			finally
			{
			}
			return this.Result;
		}
		public ErrorResult StartPickPlace()
		{
			this.ClearErrorFlags();
			try
			{
				this.LiftPNPCom.SetPickPlaceStart();
				this.CheckAndThrowIfError( this.BlowerOn() );
			}
			catch ( Exception ex )
			{
				this.BlowerOff();
				this.CatchAndPromptErr( ex );
			}
			finally
			{
			}
			return this.Result;
		}
		public ErrorResult MoveDown()
		{
			this.ClearErrorFlags();
			try
			{
				this.CheckAndThrowIfError( this.LiftMoveRel( -2.5 ).Result );
			}
			catch ( Exception ex )
			{
				this.BlowerOff();
				this.CatchAndPromptErr( ex );
			}
			finally
			{
			}
			return this.Result;
		}
		public ErrorResult EndPickPlace()
		{
			this.ClearErrorFlags();
			try
			{
				this.CheckAndThrowIfError( this.BlowerOff() );
				this.LiftPNPCom.SetPickPlaceDone();
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			finally
			{
			}
			return this.Result;
		}
		#endregion

		#region Template
		public Task<ErrorResult> Template()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;

				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}
		#endregion
		#region Autorun flags
		public LiftAutorunInfo AutorunInfo { get; set; } = new LiftAutorunInfo();
		#endregion
	}
	public class LiftAutorunInfo : BaseUtility
	{

		public void Clear()
		{
		}
		public LiftAutorunInfo()
		{
		}
	}
	public enum LiftState
	{
		Reset,
		InPos,
		PickStart,
		PickDone,
	}


	public class LiftPNPComFlags : BaseUtility
	{
		public LiftState LiftState
		{
			get => this.GetValue( () => this.LiftState );
			private set => this.SetValue( () => this.LiftState, value );
		}

		public void ResetState() => this.LiftState = LiftState.Reset;// to be called by liftseq
		public void SetInPos()// to be called by lift module
		{
			if ( this.LiftState == LiftState.Reset ) this.LiftState = LiftState.InPos;
		}
		public void SetPickPlaceStart()// to be called by PNP seq
		{
			if ( this.LiftState == LiftState.InPos ) this.LiftState = LiftState.PickStart;
		}
		public void SetPickPlaceDone()// to be called by PNP seq
		{
			if ( this.LiftState == LiftState.PickStart ) this.LiftState = LiftState.PickDone;
		}
		public bool IsPickPlaceDone => this.LiftState == LiftState.PickDone;
		public bool IsInPos => this.LiftState == LiftState.InPos;
	}
}
