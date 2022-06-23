using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Report;
using HiPA.Instrument.Camera;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.ACS;
using HiPA.Instrument.Motion.APS;
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
		public MachineMiscSystem MachineMisc { get; private set; }
		#endregion
		#region Motion/IO Boards
		public MotionBoardBase MotionBoard { get; private set; }
		#endregion
		#region IO modules
		public TowerLight TowerLight { get; private set; }
		#endregion
		#region sequences
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
		}

		private void InitIoModules()
		{
			try
			{
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

				this.MotionBoard = Constructor.GetInstance().GetInstrument( typeof( ACSBoardConfiguration ) ) as ACSMotionBoard;
				if ( this.MotionBoard == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{ACSBoardConfiguration.NAME}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.MotionBoard.Owner = this;

				this.MachineMisc = Constructor.GetInstance().GetInstrument( typeof( MachineMiscSystemConfiguration ) ) as MachineMiscSystem;
				if ( this.MachineMisc == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{MachineMiscSystemConfiguration.NAME}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.MachineMisc.Owner = this;

				if ( ( sErr = this.MotionBoard.Create().Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				//if ( ( sErr = this.IoBoard2.Create().Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E4 );

				this.AddIOsToDict();
				this.InitIoModules();
				this.TowerLight = new TowerLight();
				var tasks = new Task<string>[]
				{
					this.MachineMisc.Create(),
				};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
				}
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

				var tasks = new Task<string>[]
				{
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
				//if ( this.IoBoard2 is null ) return string.Empty;
				var tasks = new Task<string>[]
				{
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
		public Task<ErrorResult> InitAndHome()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{

					MachStateMgr.MachineStatus = MachineStateType.HOMING;
					this.CheckAndThrowIfError( this.OnInitialize() );
					var Tasks = new Task<ErrorResult>[]
					{
						this.HomeAxes(),
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
		#region Motion
		public Task<ErrorResult> HomeAxes()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{

					MachStateMgr.MachineStatus = MachineStateType.HOMING;
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
		LoggerHelper GRRLogger = new LoggerHelper( "GRRLogger", "Logger", "FOV1 RawPos,FOV1 RawOffset,FOV1 PosOffset,FOV1 PosOffsetMM,FOV1 Status,FOV2 RawPos,FOV2 RawOffset,FOV2 PosOffset,FOV2 PosOffsetMM,FOV2 Status,Result" );

		public DryRunBypass DryrunBypass { get; set; } = new DryRunBypass();

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
			//Constructor.GetInstance().Save();
			GC.Collect();
		}
		public override void ApplyRecipe( RecipeBaseUtility recipeItem )
		{
			throw new NotImplementedException();
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
	[Serializable]
	public enum ePNPPos
	{
		Pick,
		Wait,
		Load,
		PlaceKIV,
		PlaceNG,
	}

	public class StationSequences : BaseUtility
	{
		public void Restart()
		{

		}
		public void Start()
		{

		}
		public void Stop()
		{

		}
		public void Pause()
		{

		}
		public void CycleStop()
		{

		}
	}
	public class DryRunBypass : BaseUtility
	{
		public bool NewLift
		{
			get => this.GetValue( () => this.NewLift );
			set => this.SetValue( () => this.NewLift, value );
		}
		public bool QICLift
		{
			get => this.GetValue( () => this.QICLift );
			set => this.SetValue( () => this.QICLift, value );
		}
		public List<DryRunArmOpt> DryRunOp = new List<DryRunArmOpt>();
		public DryRunBypass()
		{
			this.DryRunOp.Clear();
			this.DryRunOp.Add( new DryRunArmOpt() { TravelOption = ePNPPos.Pick } );
			this.DryRunOp.Add( new DryRunArmOpt() { TravelOption = ePNPPos.Wait } );
			this.DryRunOp.Add( new DryRunArmOpt() { TravelOption = ePNPPos.Load } );
			this.DryRunOp.Add( new DryRunArmOpt() { TravelOption = ePNPPos.PlaceNG } );
			this.DryRunOp.Add( new DryRunArmOpt() { TravelOption = ePNPPos.PlaceKIV } );
		}
	}
	public class DryRunArmOpt : BaseUtility
	{
		public bool LoadArm
		{
			get => this.GetValue( () => this.LoadArm );
			set => this.SetValue( () => this.LoadArm, value );
		}
		public bool UnloadArm
		{
			get => this.GetValue( () => this.UnloadArm );
			set => this.SetValue( () => this.UnloadArm, value );
		}
		public ePNPPos TravelOption { get; set; }
	}

	[Serializable]
	public class EquipmentBypass : RecipeBaseUtility
	{
		public VisionBypass Vision { get; set; } = new VisionBypass();

	}

	[Serializable]
	public class EOLHelper : BaseUtility
	{
		public event EventHandler<double> EOLReached;
		private bool EventTriggered = false;
		public double EOLHour
		{
			get => this.GetValue( () => this.EOLHour );
			set
			{
				this.SetValue( () => this.EOLHour, value );
				if ( this.EOLHour != 0 && this.EOLHour < this.LifeTimeHour && !this.EventTriggered )
				{
					this.EventTriggered = true;
					this.EOLReached.Invoke( this, this.LifeTimeHour );
				}
			}
		}
		public double LifeTimeHour
		{
			get => this.GetValue( () => this.LifeTimeHour );
			set
			{
				this.SetValue( () => this.LifeTimeHour, value );
				if ( this.EOLHour != 0 && this.EOLHour < this.LifeTimeHour && !this.EventTriggered )
				{
					this.EventTriggered = true;
					this.EOLReached.Invoke( this, this.LifeTimeHour );
				}
			}
		}
		public bool IsEOL
		{
			get
			{
				if ( this.EOLHour == 0 ) return false;
				return this.EOLHour >= this.LifeTimeHour;
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