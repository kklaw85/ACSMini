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
		#region Home Axes
		public Task<ErrorResult> HomeLift()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.CheckAndThrowIfError( ErrorClass.E5, this.Lift.Homing().Result );
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
					 var tasks = new Task<ErrorResult>[]
					 {
						this.MoveToCollectPos(true),
					 };
					 this.CheckAndThrowIfError( tasks );
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
		private Task<ErrorResult> MoveUp()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( this.LowerLimit.State && !this.UpperLimit.State ) return this.Result;
					while ( !this.LowerLimit.State )
					{
						this.CheckAndThrowIfError( this.LiftMoveRel( 2 ).Result );
						if ( this.Lift.Status.PEL ) break;
						Thread.Sleep( 5 );
					}
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}
		private Task<ErrorResult> MoveDown()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( this.LowerLimit.State && !this.UpperLimit.State ) return this.Result;
					while ( this.UpperLimit.State )
					{
						this.CheckAndThrowIfError( this.LiftMoveRel( -2 ).Result );
						if ( this.Lift.Status.NEL ) break;
						Thread.Sleep( 5 );
					}
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}
		public bool isContinuous
		{
			get => this.GetValue( () => this.isContinuous );
			set => this.SetValue( () => this.isContinuous, value );
		}
		enum LiftMovement
		{
			Upward,
			Downward,
			Stop
		}
		public Task<ErrorResult> LiftTask { get; private set; } = null;
		public bool ReadyForCollection
		{
			get => this.GetValue( () => this.ReadyForCollection );
			private set => this.SetValue( () => this.ReadyForCollection, value );
		}

		private Stopwatch LiftWatchDogTimer = new Stopwatch();

		public Task<ErrorResult> MoveToCollectPos( bool isContinuous )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					Monitor.Enter( this.SyncRoot );
					this.ReadyForCollection = false;
					var Movement = LiftMovement.Stop;
					var Retry = 0;
					var HysteresisCount = 0;
					this.LiftWatchDogTimer.Restart();
					this.isContinuous = isContinuous;
					while ( this.isContinuous || //for continuous auto mode
					( !this.ReadyForCollection && !this.isContinuous ) )//for one time move to collection mode, but still need to loop through for stability check.
					{
						if ( this.LiftWatchDogTimer.ElapsedMilliseconds > this.Configuration.MovementTimeout ) this.CheckAndThrowIfError( ErrorClass.E5, "Movement exceeded time out." );//100second
						if ( !this.LowerLimit.State && !this.UpperLimit.State )
						{
							HysteresisCount = 0;
							Retry = 0;
							Movement = LiftMovement.Upward;
						}
						else if ( this.LowerLimit.State && this.UpperLimit.State )
						{
							HysteresisCount = 0;
							Retry = 0;
							Movement = LiftMovement.Downward;
						}
						else if ( !this.LowerLimit.State && this.UpperLimit.State )
						{
							HysteresisCount = 0;
							if ( Retry++ > 10 ) throw new Exception( "Faulty signalling. Please check if sensor is blocked" );
							else Thread.Sleep( 100 );
						}
						else
						{
							if ( HysteresisCount++ > 5 )
							{
								Retry = 0;
								Movement = LiftMovement.Stop;
								this.ReadyForCollection = true;
							}
							else
							{
								HysteresisCount = 0;
								Thread.Sleep( 10 );
								continue;
							}
						}
						if ( Movement == LiftMovement.Upward ) this.CheckAndThrowIfError( this.MoveUp().Result );
						else if ( Movement == LiftMovement.Downward ) this.CheckAndThrowIfError( this.MoveDown().Result );
						else
						{
							this.LiftWatchDogTimer.Restart();
							Thread.Sleep( 5 );
						}
					}
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
					this.isContinuous = false;
					this.ReadyForCollection = false;
				}
				finally
				{
					Monitor.Exit( this.SyncRoot );
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
					Monitor.Enter( this.SyncRoot );
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
				}
				finally
				{
					Monitor.Exit( this.SyncRoot );
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
					Monitor.Enter( this.SyncRoot );
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
					Monitor.Exit( this.SyncRoot );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> MoveToLoadPos()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.CheckAndThrowIfError( this.StopAuto().Result );
					this.CheckAndThrowIfError( this.HomeLift().Result );
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
		public LiftPNPComFlags LiftPNPCom { get; private set; } = new LiftPNPComFlags();
		public void Clear()
		{
			this.LiftPNPCom.ClearReady();
			this.LiftPNPCom.ClearPickPlaceDone();
		}
		public LiftAutorunInfo()
		{
		}
	}

	public class LiftPNPComFlags : BaseUtility
	{
		public bool Ready
		{
			get => this.GetValue( () => this.Ready );
			private set => this.SetValue( () => this.Ready, value );
		}
		public bool PickPlaceDone
		{
			get => this.GetValue( () => this.PickPlaceDone );
			private set => this.SetValue( () => this.PickPlaceDone, value );
		}
		public void SetReady() => this.Ready = true;
		public void ClearReady() => this.Ready = false;
		public void SetPickPlaceDone() => this.PickPlaceDone = true;
		public void ClearPickPlaceDone() => this.PickPlaceDone = false;

	}
}
