using HiPA.Common;
using NeoWisePlatform.Module;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoWisePlatform.Sequence
{
	public class PNPSeq : StationSeqBase
	{
		private PNPModule Module => this._Equipment.PNP;
		private LiftPNPComFlags NewLiftCom;
		private LiftPNPComFlags QICLiftCom;
		private eInspResult PNPInsResult => this.Module.AutorunInfo.InspectionRes.InspResult;
		private StageModule StageModule => this._Equipment.Stage;
		private StageAutorunInfo StageInfo => this.StageModule.AutorunInfo;
		private Statistic Stats => this._Equipment.MachineMisc.Configuration.Stats;
		private bool NewLiftStarted = false;
		private bool QICLiftStarted = false;
		private bool StageStarted = false;
		private bool FailToPickNew = false;
		private Task<ErrorResult> MoveToPick = null;
		StageSeq StageSeq;
		LiftSeq NewLiftSeq;
		LiftSeq QICLiftSeq;
		public PNPSeq( StageSeq Stage, LiftSeq New, LiftSeq QIC )
		{
			this.StageSeq = Stage;
			this.NewLiftSeq = New;
			this.NewLiftCom = this.NewLiftSeq.Module.LiftPNPCom;
			this.QICLiftSeq = QIC;
			this.QICLiftCom = this.QICLiftSeq.Module.LiftPNPCom;
			this.WorkerSeq = new WorkerThread( "PNP Seq", this );
			this.WorkerSeq.OnThreadStoppedOnError += this.OnThreadStoppedError;
			this.InitSeqFunction();
		}

		protected override void InitSeqFunction()
		{
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.Init.ToString(), this.Init, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.CheckPNPStatus.ToString(), this.CheckPNPStatus, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.WaitLiftHomingComplete.ToString(), this.WaitLiftHomingComplete, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.MoveToStandbyPos.ToString(), this.MoveToStandbyPos, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.SelectWork.ToString(), this.SelectWork, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.IsAction.ToString(), this.IsAction, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.StartSeqs.ToString(), this.StartSeqs, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.CheckStageForSub1st.ToString(), this.CheckStageForSub1st, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.MoveToPickPos.ToString(), this.MoveToPickPos, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.WaitLiftReady.ToString(), this.WaitLiftReady, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.Pick.ToString(), this.Pick, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.WaitForVisionComplete.ToString(), this.WaitForVisionComplete, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.MoveToInspectedPos.ToString(), this.MoveToInspectedPos, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.Place.ToString(), this.Place, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.MoveToStandby.ToString(), this.MoveToStandby, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.WaitAtStandbyPos.ToString(), this.WaitAtStandbyPos, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_PNP_Seq.Finish.ToString(), this.Finish, this.GotoError ) );
			this.WorkerSeq.AssignSeqList( this.SeqList );
			this.ResetAll();
		}

		private int Init()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.Init ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( MachineStateMng.isSimulation ) return ( int )RunErrors.ERR_NoError;
				this.InitFlags();
				this.NewLiftSeq.StartAuto();
				this.QICLiftSeq.StartAuto();
				return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
		}
		private int CheckPNPStatus()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.CheckPNPStatus ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.bCycleStop )
				{
					this.StopAuto();
					return this.JumpFunctionEnum( Run_PNP_Seq.Init );
				}
				if ( this.isError( this.Module.IsAllowToAutorun().Result ) ) return ( int )RunErrors.ERR_PNPModuleNotReadyForAutorun;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int WaitLiftHomingComplete()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.WaitLiftHomingComplete ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.bCycleStop || this.NewLiftSeq.Idx == 0 || this.QICLiftSeq.Idx == 0 )
				{
					this.StopAuto();
					return this.JumpFunctionEnum( Run_PNP_Seq.Init );
				}
				if ( this.NewLiftSeq.State == SequenceState.IsWaitWork && this.QICLiftSeq.State == SequenceState.IsWaitWork ) return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 200 );
			return this.CycleFunction( 60000 );
		}
		private int MoveToStandbyPos()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.MoveToStandbyPos ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( MachineStateMng.isSimulation ) return ( int )RunErrors.ERR_NoError;
				if ( this.bCycleStop )
				{
					this.StopAuto();
					return this.JumpFunctionEnum( Run_PNP_Seq.Init );
				}
				if ( this.isError( this.Module.MoveToStandbyStatus().Result ) ) return ( int )RunErrors.ERR_AxisMoveErr;
				this.ThreadInitState();
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int SelectWork()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.SelectWork ) ) return ( int )RunErrors.ERR_Inconformity;
				this.State = SequenceState.IsWaitWork;
				if ( this.StartExecute )
				{
					this.ThreadInitState();
					return this.JumpFunction( this.StepCurrent );
				}
				else if ( this.bCycleStop )
				{
					this.StopAuto();
					return this.JumpFunctionEnum( Run_PNP_Seq.Init );
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 200 );
			return this.CycleFunction();
		}
		private int IsAction()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.IsAction ) ) return ( int )RunErrors.ERR_Inconformity;
				this.State = SequenceState.IsAction;
				this.NewLiftStarted = false;
				this.QICLiftStarted = false;
				this.StageStarted = false;
				if ( !this.bCycleStop ) this.MoveToPick = this.Module.PNPToPickPos();
				else this.MoveToPick = null;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int StartSeqs()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.StartSeqs ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( !this.QICLiftStarted && this.QICLiftSeq.State == SequenceState.IsWaitWork )
				{
					this.QICLiftStarted = true;
					this.QICLiftSeq.SetStepThread( Run_State_Lift.IsAction );
				}
				if ( !this.NewLiftStarted && this.NewLiftSeq.State == SequenceState.IsWaitWork )
				{
					this.NewLiftStarted = true;
					this.NewLiftSeq.SetStepThread( Run_State_Lift.IsAction );
				}
				if ( !this.StageStarted && this.StageSeq.State == SequenceState.IsWaitWork )
				{
					this.StageStarted = true;
					this.StageSeq.SetStepThread( Run_Stage_Seq.IsAction );
				}
				if ( ( this.QICLiftStarted || this.QICLiftSeq.Module.LiftPNPCom.IsInPos ) && ( this.NewLiftStarted || this.NewLiftSeq.Module.LiftPNPCom.IsInPos ) && this.StageStarted ) return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 100 );
			return this.CycleFunction( 60000 );
		}

		private int CheckStageForSub1st()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.CheckStageForSub1st ) ) return ( int )RunErrors.ERR_Inconformity;
				this.Module.AutorunInfo.UnloadStage = this.StageModule.Stage.MatDet == true;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int MoveToPickPos()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.MoveToPickPos ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.isError( this.MoveToPick?.Result ) )
				{
					this.MoveToPick = null;
					return ( int )RunErrors.ERR_PNPMoveErr;
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int WaitLiftReady()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.WaitLiftReady ) ) return ( int )RunErrors.ERR_Inconformity;
				var ToPickNew = !this.Module.LoadArm.SuctionSense;
				var ToUnloadInspected = this.Module.AutorunInfo.UnloadStage;
				if ( ToPickNew && ToUnloadInspected )
				{
					if ( this.NewLiftCom.IsInPos && this.QICLiftCom.IsInPos ) return ( int )RunErrors.ERR_NoError;
				}
				else if ( ToPickNew )
				{
					if ( this.NewLiftCom.IsInPos ) return ( int )RunErrors.ERR_NoError;
				}
				else if ( ToUnloadInspected ) if ( this.QICLiftCom.IsInPos ) return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 100 );
			return this.CycleFunction( 60000 );
		}
		private int Pick()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.Pick ) ) return ( int )RunErrors.ERR_Inconformity;
				var ErrIdx = 0;
				var ToPickNew = !this.Module.LoadArm.SuctionSense;
				var ToUnloadInspected = this.Module.AutorunInfo.UnloadStage;
				var tasks = new List<Task<ErrorResult>>();
				if ( ToPickNew )
				{
					this.Module.LoadArm.LinkLiftModule( this.NewLiftSeq.Module );
					tasks.Add( this.Module.LoadArm.PickUp() );
				}
				if ( ToUnloadInspected ) tasks.Add( this.Module.UnLoadArm.PickUp() );
				Task.WaitAll( tasks.ToArray() );
				foreach ( var task in tasks )
				{
					if ( this.isError( task.Result ) )
					{
						if ( ErrIdx == 0 && ToPickNew )
						{
							this.FailToPickNew = true;
							if ( ToUnloadInspected )
							{
								this.ReportWarning( RunErrors.ERR_PNPFailToPickUp, this.Result.ErrorMessage, true );
								continue;
							}
							else return ( int )RunErrors.ERR_PNPFailToPickUp;
						}
						else return ( int )RunErrors.ERR_PNPFailToPickUp;
					}
					ErrIdx++;
				}
				this.Module.LoadArm.ClearLiftModuleLink();
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 100 );
			return ( int )RunErrors.ERR_NoError;
		}
		private int WaitForVisionComplete()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.WaitForVisionComplete ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.StageInfo.InspectionRes.VisionOp == eVisionOperation.ResultCalculated || ( this.StageInfo.InspectionRes.VisionOp == eVisionOperation.Init ) )
				{
					if ( this.Module.AutorunInfo.UnloadStage )
					{
						this.Module.AutorunInfo.InspectionRes.Copy( this.StageModule.AutorunInfo.InspectionRes );
						this.StageModule.AutorunInfo.InspectionRes.Clear();
					}
					this.Module.AutorunInfo.UnloadStage = this.StageModule.Stage.MatDet == true;
					if ( this.Module.AutorunInfo.UnloadStage ) this.StageInfo.StageFlag = PNPToStageFlag.Stop;
					else this.StageInfo.StageFlag = PNPToStageFlag.Cleared;
					return ( int )RunErrors.ERR_NoError;
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 100 );
			return this.CycleFunction( 10000 );
		}
		private int MoveToInspectedPos()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.MoveToInspectedPos ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.Module.UnLoadArm.SuctionSense )
				{
					if ( this.isError( this.Module.MovePNPByResult().Result ) ) return ( int )RunErrors.ERR_PNPMoveErr;
				}
				else
				{
					if ( this.isError( this.Module.PNPToLoadPos().Result ) ) return ( int )RunErrors.ERR_PNPMoveErr;
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}

		private int Place()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.Place ) ) return ( int )RunErrors.ERR_Inconformity;
				var tasks = new List<Task<ErrorResult>>();

				if ( this.Module.LoadArm.MatDropped ) return ( int )RunErrors.ERR_PNPMatDrop;
				if ( this.Module.UnLoadArm.MatDropped ) return ( int )RunErrors.ERR_PNPMatDrop;
				var ToPlaceNew = this.Module.LoadArm.SuctionSense && this.Module.AutorunInfo.UnloadStage == false && this.Module.PNPInLoadPos();
				var ToPlaceInspected = this.Module.UnLoadArm.SuctionSense;
				if ( ToPlaceNew )
				{
					tasks.Add( this.Module.LoadArm.PlaceDown() );
				}
				if ( ToPlaceInspected )
				{
					if ( this.Module.PNPInLoadPos() ) this.Module.UnLoadArm.LinkLiftModule( this.QICLiftSeq.Module );
					tasks.Add( this.Module.UnLoadArm.PlaceDown() );
					this.Stats.Update( this.Module.AutorunInfo.InspectionRes.InspResult );
				}
				Task.WaitAll( tasks.ToArray() );
				foreach ( var task in tasks )
				{
					if ( this.isError( task.Result ) ) return ( int )RunErrors.ERR_PNPFailToPlaceDown;
				}

				this.Module.AutorunInfo.InspectionRes.Clear();
				if ( ToPlaceNew )
				{
					this.StageInfo.StageFlag = PNPToStageFlag.CanClamp;
				}
				this.Module.UnLoadArm.ClearLiftModuleLink();
				if ( !this.Module.PNPInLoadPos() ) return this.JumpFunctionEnum( Run_PNP_Seq.MoveToInspectedPos );
				return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
		}

		private int MoveToStandby()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.MoveToStandby ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.isError( this.Module.PNPToWaitPos().Result ) ) return ( int )RunErrors.ERR_PNPMoveErr;
				if ( !this.FailToPickNew || this.Module.AutorunInfo.UnloadStage ) this.StageInfo.StageFlag = PNPToStageFlag.CanInspect;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int WaitAtStandbyPos()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.WaitAtStandbyPos ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( ( int )this.StageInfo.InspectionRes.VisionOp >= ( int )eVisionOperation.ImageTaken ) return ( int )RunErrors.ERR_NoError;
				if ( !this.Module.LoadArm.SuctionSense && !this.Module.UnLoadArm.SuctionSense && this.StageSeq.Module.Stage.MatDet == false )
				{
					return ( int )RunErrors.ERR_PNPFailToPickUp;
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 100 );
			return this.CycleFunction( 10000 );
		}
		private int Finish()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.Finish ) ) return ( int )RunErrors.ERR_Inconformity;
				var DelayCycleStop = !this.Module.AutorunInfo.UnloadStage;
				this.ReportError();
				this.InitFlags();
				this.State = SequenceState.Init;
				if ( this.bCycleStop && DelayCycleStop )
				{
					this.StopAuto();
					return this.JumpFunctionEnum( Run_PNP_Seq.Init );
				}
				else return this.JumpFunctionEnum( Run_PNP_Seq.IsAction );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
		}

		protected override void InitFlags()
		{
			this.FailToPickNew = false;
			this.Module.LoadArm.ObjHeld = false;
			this.Module.UnLoadArm.ObjHeld = false;
			this.Module.AutorunInfo.Clear();
		}
		public new void StopAuto()
		{
			Task.Run( () =>
			{
				this.RestartSeq = false;
				this._Equipment.MachineMisc.Configuration.Stats.Stop();
				this.State = SequenceState.IsNotStarted;
				this.MoveToPick?.Wait();
				this.StageSeq?.StopAuto();
				this.NewLiftSeq?.StopAuto();
				this.QICLiftSeq?.StopAuto();
				this.WorkerSeq.Stop();
				//this.Module.MoveToStandbyStatus().Wait();
			} );
		}
		protected override void CycleStop()
		{
			this.bCycleStop = true;
		}
		protected override void ReportError() => this.ReportErrorOnly();

		public new void Restart()
		{
			Task.Run( () =>
			{
				if ( this.State != SequenceState.IsNotStarted ) return; // use idx!=0 if enum does not work
				base.Restart();
				this.StageSeq?.StartAuto();
			} );
		}
		public new void StartAuto()
		{
			Task.Run( () =>
			{
				this.RestartSeq = false;
				this._Equipment.MachineMisc.Configuration.Stats.Start();
				this.WorkerSeq.Start();
				this.StageSeq?.StartAuto();
			} );
		}
		public new void PauseAuto()
		{
			Task.Run( () =>
			{
				this._Equipment.MachineMisc.Configuration.Stats.Pause();
				this.StageSeq?.PauseAuto();
				this.NewLiftSeq?.PauseAuto();
				this.QICLiftSeq?.PauseAuto();
				this.WorkerSeq.Pause();
			} );
		}
		protected override void OnThreadStoppedError()
		{
			//if ( this.IsCycleStop == false )
			//{
			this.StopAuto();
			//}
			//else
			//	this.PauseAuto();
		}
	}
	public class StationSeqBase : SequenceBase
	{
		public bool bCycleStop { get; set; } = false;

		protected virtual void InitFlags()
		{
		}
		public void Restart()
		{
			if ( this.State != SequenceState.IsNotStarted ) return; // use idx!=0 if enum does not work
			Task.Run( () =>
			{
				this.RestartSeq = true;
				this.WorkerSeq.Start();
			} );
		}
		protected override void CycleStop()
		{
			this.bCycleStop = true;
		}
		protected override void ReportError() => this.ReportErrorOnly();
	}
}

public enum Run_PNP_Seq
{
	Init = 0,
	CheckPNPStatus,//home pnp axis, jog pnp arms
	WaitLiftHomingComplete,//wait lift homing done before move pnp arm
	MoveToStandbyPos,
	SelectWork,
	IsAction,
	StartSeqs,
	CheckStageForSub1st,//To decide whether stage has any substrate to clear., check for vision result at this point
	MoveToPickPos,//Load arm at new lift, unload arm at stage
	WaitLiftReady,
	Pick,//Pick from new lift and stage (if has any)
	WaitForVisionComplete,
	MoveToInspectedPos,
	Place,// place to inspected and Load stage with substrate
	MoveToStandby,
	WaitAtStandbyPos,//Wait for inspection to complete
	Finish
}
