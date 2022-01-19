using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Report;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using HiPA.Instrument.Motion.Dask;
using NeoWisePlatform.Sequence;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace NeoWisePlatform.Module
{
	public class EquipmentConfig
		: EquipmentConfiguration
	{
		[NonSerialized]
		private static readonly EquipmentConfig _DEFAULT = new EquipmentConfig();

		public override string Name { get; set; } = "Neowise";
		public override Type InstrumentType => typeof( MTEquipment );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
	}

	public class MTEquipment
		: Equipment
	{
		public override MachineVariant MachineVar { get; set; } = new MachineVar( true );
		public static readonly int NoOfConveyor = 2;
		#region Common Modules 
		System.Timers.Timer Hourly;
		public MultilingualErrModule MultiLangErr;
		#endregion
		#region Module
		public PNPModule PNP { get; private set; }
		public LiftModuleBase NewLift { get; private set; }
		public LiftModuleBase QICLift { get; private set; }
		public StageModule Stage { get; private set; }
		public MachineMiscSystem MachineMisc { get; private set; }
		#endregion
		#region Motion/IO Boards
		public MotionBoardBase MotionBoard { get; private set; }
		public APSIoBoard IoBoard { get; private set; }
		public DaskIoBoard IoBoard2 { get; private set; }
		#endregion
		#region IO modules
		public TowerLight TowerLight { get; private set; }
		#endregion
		#region sequences
		public AutoSeq AutoSeq { get; set; }
		#endregion
		#region IO Points
		public Dictionary<string, AdLinkIoPoint> Inputs { get; private set; } = new Dictionary<string, AdLinkIoPoint>();
		public Dictionary<string, AdLinkIoPoint> Outputs { get; private set; } = new Dictionary<string, AdLinkIoPoint>();
		#endregion
		#region Init/Shut
		public MTEquipment( EquipmentConfig config ) : base( config )
		{
		}
		public override string InitModules()
		{
			return base.InitModules();
		}

		public override void Shutdown()
		{
			base.Shutdown();
		}
		#endregion
		#region IO 
		public AdLinkIoPoint GetIOPointByEnum( Enum io )
		{
			if ( this.Inputs.ContainsKey( io.ToString() ) )
				return this.Inputs[ io.ToString() ];
			else if ( this.Outputs.ContainsKey( io.ToString() ) )
				return this.Outputs[ io.ToString() ];
			else
				return null;
		}
		private void AddIOsToDict()
		{
			foreach ( AdLinkIoPoint io in this.IoBoard.GetChildren() )
			{
				if ( io.Configuration.Type == DioType.Input )
					this.Inputs[ io.Name ] = io;
				else if ( io.Configuration.Type == DioType.Output )
					this.Outputs[ io.Name ] = io;
			}
		}

		private void InitIoModules()
		{
			try
			{
				this.NewLift.UpperLimit.IOPt = this.GetIOPointByEnum( InputIO.I_NewLiftULimit );
				this.NewLift.LowerLimit.IOPt = this.GetIOPointByEnum( InputIO.I_NewLiftLLimit );

				this.QICLift.UpperLimit.IOPt = this.GetIOPointByEnum( InputIO.I_QICLiftULimit );
				this.QICLift.LowerLimit.IOPt = this.GetIOPointByEnum( InputIO.I_QICLiftLLimit );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
		}
		private void InitEvent()
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
		}
		private void EStop_IoValueChangedEvent( object sender, DioValue value )
		{
			Equipment.ErrManager.RaiseError( this, "E-STOP Triggered. Please Check The Machine!", ErrorTitle.EStop, ErrorClass.E6 );
			var sErr = string.Empty;
			foreach ( var axis in this.MotionBoard.GetChildren() )
			{
				if ( ( sErr = ( axis as AxisBase )?.ServoOn( false ).Result ) != string.Empty )
					Equipment.ErrManager.RaiseError( this, sErr, ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
			//Stop Auto Seq.


			//OnStop();
			//Task.WaitAll( vcmt1 );
		}
		#endregion
		#region General
		protected override string OnCreate()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				this.Hourly = new System.Timers.Timer( 60 * 30 * 1000 );
				this.Hourly.Elapsed += new ElapsedEventHandler( this.OnTimedEvent );
				this.Hourly.Start();
				var something = Constructor.GetInstance().Configuration.MachineVar;

				this.MotionBoard = Constructor.GetInstance().GetInstrument( typeof( AdLinkBoardConfiguration ) ) as AdLinkMotionBoard;
				if ( this.MotionBoard == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{AdLinkBoardConfiguration.NAME}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.MotionBoard.Owner = this;

				this.IoBoard = Constructor.GetInstance().GetInstrument( IOCardList.AMP204208C.ToString(), typeof( APSIoBoardConfiguration ) ) as APSIoBoard;
				if ( this.IoBoard == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{APSIoBoardConfiguration.NAME}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.IoBoard.Owner = this;

				this.IoBoard2 = Constructor.GetInstance().GetInstrument( IOCardList.PCI7432.ToString(), typeof( DaskIoBoardConfiguration ) ) as DaskIoBoard;
				if ( this.IoBoard2 == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{DaskIoBoardConfiguration.NAME}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.IoBoard2.Owner = this;

				this.PNP = Constructor.GetInstance().GetInstrument( typeof( PNPModuleConfiguration ) ) as PNPModule;
				if ( this.PNP == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{PNPModuleConfiguration.NAME}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.PNP.Owner = this;

				this.NewLift = Constructor.GetInstance().GetInstrument( Lift.NewLift.ToString(), typeof( LiftModuleConfiguration ) ) as LiftModuleBase;
				if ( this.NewLift == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"NewLift" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.NewLift.Owner = this;

				this.QICLift = Constructor.GetInstance().GetInstrument( Lift.QICLift.ToString(), typeof( LiftModuleConfiguration ) ) as LiftModuleBase;
				if ( this.QICLift == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"QICLift" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.QICLift.Owner = this;

				this.Stage = Constructor.GetInstance().GetInstrument( "Stage", typeof( StageModuleConfiguration ) ) as StageModule;
				if ( this.Stage == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Stage" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.Stage.Owner = this;

				this.MachineMisc = Constructor.GetInstance().GetInstrument( typeof( MachineMiscSystemConfiguration ) ) as MachineMiscSystem;
				if ( this.MachineMisc == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{MachineMiscSystemConfiguration.NAME}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.MachineMisc.Owner = this;

				if ( ( sErr = this.MotionBoard.Create().Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				if ( ( sErr = this.IoBoard.Create().Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				if ( ( sErr = this.IoBoard2.Create().Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E4 );

				this.AddIOsToDict();
				this.InitIoModules();
				this.TowerLight = new TowerLight();
				var tasks = new Task<string>[]
				{
					this.PNP.Create(),
					this.NewLift.Create(),
					this.QICLift.Create(),
					this.Stage.Create(),
					this.MachineMisc.Create(),
				};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
				}

				this.PNP.Seq.PNPSeq = new PNPSeq( this.Stage.Seq, this.NewLift.Seq, this.QICLift.Seq );
				this.AutoSeq = new AutoSeq( this.PNP.Seq );


				this.MultiLangErr = new MultilingualErrModule();
				if ( ( result = this.MultiLangErr.InitLoadErrorList().Result ) != string.Empty )
					throw new Exception( result );

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
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				this.TowerLight.BypassAlarm = true;
				if ( ( sErr = this.MotionBoard.Initialize().Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.InitializeFailure, ErrorClass.E5 );
				if ( ( sErr = this.IoBoard.Initialize().Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.InitializeFailure, ErrorClass.E5 );
				if ( ( sErr = this.IoBoard2.Initialize().Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.InitializeFailure, ErrorClass.E5 );

				var tasks = new Task<string>[]
				{
					this.PNP.Initialize(),
					this.NewLift.Initialize(),
					this.QICLift.Initialize(),
					this.Stage.Initialize(),
					this.MachineMisc.Initialize(),
				};
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.InitializeFailure, ErrorClass.E5 );
				}

				this.InitEvent();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.InitializeFailure, ErrorClass.E6 );
			}
			finally
			{
				this.TowerLight.BypassAlarm = false;
			}
			return result;
		}
		protected override string OnStop()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{

				if ( this.MotionBoard is null ) return string.Empty;
				if ( this.IoBoard is null ) return string.Empty;
				if ( this.IoBoard2 is null ) return string.Empty;
				var tasks = new Task<string>[]
				{
					this.PNP.Stop(),
					this.NewLift.Stop(),
					this.QICLift.Stop(),
					this.Stage.Stop(),
					this.MachineMisc.Stop(),
				};
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
				}

				var tasks1 = new Task<string>[]
				{
					this.MotionBoard.Stop(),
					this.IoBoard.Stop(),
				};
				Task.WaitAll( tasks1 );

				foreach ( var t in tasks1 )
				{
					if ( ( sErr = t.Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
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
					this.PNP.Terminate(),
					this.NewLift.Terminate(),
					this.QICLift.Terminate(),
					this.Stage.Terminate(),
					this.MachineMisc.Terminate(),
				};
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.OperationFailure, ErrorClass.E6 );
			}

			return result;
		}
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as EquipmentConfig;
		}
		#endregion
		#region Motion
		public Task<ErrorResult> HomeAxes()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{

					MachStateMgr.MachineStatus = MachineStateType.HOMING;
					var Tasks = new Task<ErrorResult>[]
					{
						this.PNP.Home(),
					};
					this.CheckAndThrowIfError( Tasks );
					this.CheckAndThrowIfError( this.InitIO().Result );
					if ( MachineStateMng.isSimulation )
						Thread.Sleep( 2000 );
					MachStateMgr.RevertStateManualOp();
				}
				catch ( Exception ex )
				{
					MachStateMgr.RevertStateManualOp();
					this.CatchAndPromptErr( ex );
				}
				finally
				{

				}
				return this.Result;
			} );
		}
		#endregion
		#region IO
		public Task<ErrorResult> InitIO()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		#endregion
		#region StartAutoRun initialization
		#endregion
		#region Lighting control
		private bool bMachineLightOn = false;
		public bool MachineLightOn
		{
			get => this.bMachineLightOn;
			set => this.Set( ref this.bMachineLightOn, value, "MachineLightOn" );
		}
		public string MachineLights( bool On )
		{
			ErrorClass EClass = ErrorClass.OK;
			string ErrorMessage = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation )
				{
					this.MachineLightOn = On;
					return ErrorMessage;
				}
				this.MachineLightOn = On;
			}
			catch ( Exception ex )
			{
				if ( EClass == ErrorClass.OK )
					EClass = ErrorClass.E6;
				ErrorMessage = Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, EClass );
			}
			return ErrorMessage;
		}
		public string ToggleMachineLight()
		{
			ErrorClass EClass = ErrorClass.OK;
			string ErrorMessage = string.Empty;
			try
			{
				if ( ( ErrorMessage = this.MachineLights( !this.MachineLightOn ) ) != string.Empty ) throw new Exception( ErrorMessage );
			}
			catch ( Exception ex )
			{
				if ( EClass == ErrorClass.OK )
					EClass = ErrorClass.E6;
				ErrorMessage = Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, EClass );
			}
			return ErrorMessage;
		}
		#endregion
		#region Doorlock
		private bool bDoorLockOn = false;
		public bool DoorLockOn
		{
			get => this.bDoorLockOn;
			set => this.Set( ref this.bDoorLockOn, value, "DoorLockOn" );
		}
		public string DoorLock( bool On )
		{
			ErrorClass EClass = ErrorClass.OK;
			string ErrorMessage = string.Empty;
			try
			{
				if ( MachineStateMng.isSimulation )
				{
					this.DoorLockOn = On;
					return ErrorMessage;
				}
				this.DoorLockOn = On;
			}
			catch ( Exception ex )
			{
				if ( EClass == ErrorClass.OK )
					EClass = ErrorClass.E6;
				ErrorMessage = Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, EClass );
			}
			return ErrorMessage;
		}
		public string ToggleDoorLock()
		{
			ErrorClass EClass = ErrorClass.OK;
			string ErrorMessage = string.Empty;
			try
			{
				if ( ( ErrorMessage = this.DoorLock( !this.DoorLockOn ) ) != string.Empty ) throw new Exception( ErrorMessage );
			}
			catch ( Exception ex )
			{
				if ( EClass == ErrorClass.OK )
					EClass = ErrorClass.E6;
				ErrorMessage = Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, EClass );
			}
			return ErrorMessage;
		}
		#endregion
		#region GRR
		public int NoIte
		{
			get => this.GetValue( () => this.NoIte );
			set => this.SetValue( () => this.NoIte, value );
		}
		public int CurrIte
		{
			get => this.GetValue( () => this.CurrIte );
			set => this.SetValue( () => this.CurrIte, value );
		}
		public bool RunTestSeq
		{
			get => this.GetValue( () => this.RunTestSeq );
			set => this.SetValue( () => this.RunTestSeq, value );
		}
		LoggerHelper GRRLogger = new LoggerHelper( "GRRLogger", "Logger", "FOV1 RawPos,FOV1 RawOffset,FOV1 PosOffset,FOV1 PixPerMM,FOV1 PosOffsetMM,FOV1 Status,FOV2 RawPos,FOV2 RawOffset,FOV2 PosOffset,FOV2 PixPerMM,FOV2 PosOffsetMM,FOV2 Status,Result" );
		public Task<ErrorResult> GRR()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( this.RunTestSeq ) return this.Result;
					this.CurrIte = 0;
					var Repeats = this.NoIte;
					this.RunTestSeq = true;
					var Cont = Repeats == 0;
					if ( this.Stage.Stage.MatDet != false ) this.CheckAndThrowIfError( ErrorClass.E5, "Stage is not empty. Please clear the stage before starting GRR" );
					this.CheckAndThrowIfError( this.NewLift.MoveToCollectPos( false ).Result );
					while ( Cont || Repeats >= 0 )
					{
						this.CheckAndThrowIfError( this.PNP.PNPToPickPos().Result );
						this.CheckAndThrowIfError( this.PNP.LoadArm.PickUp().Result );
						this.CheckAndThrowIfError( this.PNP.PNPToLoadPos().Result );
						this.CheckAndThrowIfError( this.PNP.LoadArm.PlaceDown().Result );
						var tasks = new Task<ErrorResult>[]
						{
							this.Stage.Stage.Hold(),
							this.PNP.MoveToStandbyStatus(),
						};
						this.CheckAndThrowIfError( tasks );
						this.Stage.AutorunInfo.InspectionRes.Clear();
						this.CheckAndThrowIfError( this.Stage.SnapShot().Result );
						this.CheckAndThrowIfError( this.Stage.VisionCheck().Result );
						this.GRRLogger.WriteLog( $"{this.Stage.AutorunInfo.InspectionRes.Fov1.RawPosition},{this.Stage.AutorunInfo.InspectionRes.Fov1.RawOffset},{this.Stage.AutorunInfo.InspectionRes.Fov1.PositionOffset},{this.Stage.AutorunInfo.InspectionRes.Fov1.PixPerMM},{this.Stage.AutorunInfo.InspectionRes.Fov1.PositionOffsetMM},{this.Stage.AutorunInfo.InspectionRes.Fov1.Status}," +
							$"{this.Stage.AutorunInfo.InspectionRes.Fov2.RawPosition},{this.Stage.AutorunInfo.InspectionRes.Fov2.RawOffset},{this.Stage.AutorunInfo.InspectionRes.Fov2.PositionOffset},{this.Stage.AutorunInfo.InspectionRes.Fov2.PixPerMM},{this.Stage.AutorunInfo.InspectionRes.Fov2.PositionOffsetMM},{this.Stage.AutorunInfo.InspectionRes.Fov2.Status}," +
							$"{this.Stage.AutorunInfo.InspectionRes.Result}" );
						var tasks2 = new Task<ErrorResult>[]
						{
							this.Stage.Stage.Release(),
							this.PNP.PNPToLoadPos(),
						};
						this.CheckAndThrowIfError( tasks2 );
						this.CheckAndThrowIfError( this.PNP.LoadArm.PickUp().Result );
						this.CheckAndThrowIfError( this.PNP.PNPToPickPos().Result );
						this.CheckAndThrowIfError( this.PNP.LoadArm.PlaceDown().Result );
						this.CurrIte++;
						if ( Repeats-- == 0 || !this.RunTestSeq ) break;
					}
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				finally
				{
					this.StopTestSeq();
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> Dryrun()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( this.RunTestSeq ) return this.Result;
					this.CurrIte = 0;
					var Repeats = this.NoIte;
					this.RunTestSeq = true;
					var Cont = Repeats == 0;

					this.CheckAndThrowIfError( this.QICLift.MoveToCollectPos( false ).Result );
					this.CheckAndThrowIfError( this.NewLift.MoveToCollectPos( false ).Result );
					while ( Cont || Repeats >= 0 )
					{
						this.CheckAndThrowIfError( this.PNP.PNPToPickPos().Result );
						var tasks = new Task<ErrorResult>[]
						{
							this.PNP.LoadArm.PickUp(),
							this.PNP.UnLoadArm.PickUp(),
						};
						this.CheckAndThrowIfError( tasks );
						this.CheckAndThrowIfError( this.PNP.PNPToLoadPos().Result );
						var tasks2 = new Task<ErrorResult>[]
						{
							this.PNP.LoadArm.PlaceDown(),
							this.PNP.UnLoadArm.PlaceDown(),
						};
						this.CheckAndThrowIfError( tasks2 );
						this.CurrIte++;
						if ( Repeats-- == 0 || !this.RunTestSeq ) break;
					}
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				finally
				{
					this.StopTestSeq();
				}
				return this.Result;
			} );
		}
		public void StopTestSeq()
		{
			try
			{
				new Thread( () => System.Windows.MessageBox.Show( "The operation has completed !", "Notice" ) ).Start();
				this.RunTestSeq = false;
				this.CurrIte = 0;
			}
			catch
			{ }
		}
		#endregion
		private void OnTimedEvent( object source, ElapsedEventArgs e )
		{
			GC.Collect();
		}
	}

	[Serializable]
	public enum IOCardList
	{
		AMP204208C,
		PCI7432,
		PCI7230,
		PCI7856,
	}

	[Serializable]
	public enum IOMotionList
	{
		Clamper,
		LoadArm,
		UnLoadArm
	}

	public class StationSequences : BaseUtility
	{
		public PNPSeq PNPSeq { get; set; }

		public void Restart()
		{
			this.PNPSeq.bCycleStop = false;
			this.PNPSeq?.Restart();
		}
		public void Start()
		{
			this.PNPSeq.bCycleStop = false;
			this.PNPSeq?.StartAuto();
		}
		public void Stop()
		{
			this.PNPSeq?.StopAuto();
		}
		public void Pause()
		{
			this.PNPSeq?.PauseAuto();
		}
		public void CycleStop()
		{
			this.PNPSeq.bCycleStop = true;
		}
	}
	public class CavityWorkPosLink : BaseUtility
	{
		private string s_CavityNo = string.Empty;
		public string CavityNo
		{
			get => this.s_CavityNo;
			set => this.Set( ref this.s_CavityNo, value, "CavityNo" );
		}
		private int i_WorkPosIndex = 0;
		public int WorkPosIndex
		{
			get => this.i_WorkPosIndex;
			set => this.Set( ref this.i_WorkPosIndex, value, "WorkPosIndex" );
		}
	}
	[Serializable]
	public class EquipmentBypass : BaseUtility
	{

	}
	[Serializable]
	public class EOLHelper : BaseUtility
	{
		public event EventHandler<double> EOLReached;
		private bool EventTriggered = false;
		private double dEOLHour = 0;
		private double dLifeTimeHour = 0;
		public double EOLHour
		{
			get => this.dEOLHour;
			set => this.Set( ref this.dEOLHour, value, "EOLHour" );
		}
		public double LifeTimeHour
		{
			get => this.dLifeTimeHour;
			set
			{
				this.Set( ref this.dLifeTimeHour, value, "LifeTimeHour" );
				if ( this.dEOLHour != 0 && this.dEOLHour < this.dLifeTimeHour && !this.EventTriggered )
				{
					this.EventTriggered = true;
					this.EOLReached.Invoke( this, this.dLifeTimeHour );
				}
			}
		}
		public bool IsEOL
		{
			get
			{
				if ( this.dEOLHour == 0 ) return false;
				return this.dEOLHour >= this.dLifeTimeHour;
			}
		}
		public void ResetTimer()
		{
			this.LifeTimeHour = 0;
			this.EventTriggered = false;
		}
	}
	public enum Lift
	{
		NewLift,
		QICLift,
	}
}