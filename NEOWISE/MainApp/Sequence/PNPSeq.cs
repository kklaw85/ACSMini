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
		private PNPModule PnpModule => this._Equipment.PNP;
		private LiftPNPComFlags NewLiftCom;
		private LiftPNPComFlags QICLiftCom;
		private eInspResult PNPInsResult => this.PnpModule.AutorunInfo.InspectionRes.InspResult;
		private StageModule StageModule => this._Equipment.Stage;
		private StageAutorunInfo StageInfo => this.StageModule.AutorunInfo;
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
				if ( this.isError( this.PnpModule.IsAllowToAutorun().Result ) ) return ( int )RunErrors.ERR_PNPModuleNotReadyForAutorun;
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
				if ( this.isError( this.PnpModule.MoveToStandbyStatus().Result ) ) return ( int )RunErrors.ERR_AxisMoveErr;
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
				if ( !this.bCycleStop ) this.MoveToPick = this.PnpModule.PNPToPickPos();
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
				if ( this.QICLiftStarted && this.NewLiftStarted && this.StageStarted ) return ( int )RunErrors.ERR_NoError;
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
				if ( this.StageModule.Stage.MatDet == true ) this.PnpModule.AutorunInfo.UnloadStage = true;
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
				var ToPickNew = !this.PnpModule.LoadArm.ObjHeld;
				var ToUnloadInspected = this.PnpModule.AutorunInfo.UnloadStage;
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
				var ToPickNew = !this.PnpModule.LoadArm.ObjHeld;
				var ToUnloadInspected = this.PnpModule.AutorunInfo.UnloadStage;
				var tasks = new List<Task<ErrorResult>>();
				if ( ToPickNew )
				{
					if ( this.isError( this.NewLiftSeq.Module.TrigPusher().Result ) ) return ( int )RunErrors.ERR_LiftPusher;
					if ( this.isError( this.NewLiftSeq.Module.StartPickPlace() ) ) return ( int )RunErrors.ERR_LiftStartPickPlace;
					tasks.Add( this.PnpModule.LoadArm.PickUp() );
				}
				if ( ToUnloadInspected ) tasks.Add( this.PnpModule.UnLoadArm.PickUp() );
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
							else return( int )RunErrors.ERR_PNPFailToPickUp;
						}
						else return( int )RunErrors.ERR_PNPFailToPickUp;
					}
					ErrIdx++;
				}
				if ( ToPickNew ) if ( this.isError( this.NewLiftSeq.Module.EndPickPlace() ) ) return ( int )RunErrors.ERR_LiftEndPickPlace;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 100 );
			return this.CycleFunction( 60000 );
		}
		private int WaitForVisionComplete()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_PNP_Seq.WaitForVisionComplete ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.StageInfo.InspectionRes.VisionOp == eVisionOperation.ResultCalculated || this.StageInfo.InspectionRes.VisionOp == eVisionOperation.Init )
				{
					if ( this.PnpModule.AutorunInfo.UnloadStage )
					{
						this.PnpModule.AutorunInfo.InspectionRes.Copy( this.StageModule.AutorunInfo.InspectionRes );
						this.StageModule.AutorunInfo.InspectionRes.Clear();
					}
					this.PnpModule.AutorunInfo.UnloadStage = this.StageModule.Stage.MatDet == true;
					if ( this.PnpModule.AutorunInfo.UnloadStage ) this.StageInfo.StageFlag = PNPToStageFlag.Stop;
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
				if ( this.isError( this.PnpModule.MovePNPByResult().Result ) ) return ( int )RunErrors.ERR_PNPMoveErr;
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
				var ToPlaceNew = this.PnpModule.LoadArm.ObjHeld && this.PnpModule.AutorunInfo.UnloadStage == false;
				var ToPlaceInspected = this.PnpModule.UnLoadArm.ObjHeld;
				if ( this.PnpModule.AutorunInfo.InspectionRes.InspResult == eInspResult.QIC && !this.FailToPickNew && ToPlaceNew )
				{
					tasks.Add( this.PnpModule.LoadArm.PlaceDown() );
				}
				if ( ToPlaceInspected )
				{
					if ( this.isError( this.QICLiftSeq.Module.StartPickPlace() ) ) return ( int )RunErrors.ERR_LiftStartPickPlace;
					tasks.Add( this.PnpModule.UnLoadArm.PlaceDown() );
				}
				Task.WaitAll( tasks.ToArray() );
				foreach ( var task in tasks )
				{
					if ( this.isError( task.Result ) ) return ( int )RunErrors.ERR_PNPFailToPlaceDown;
				}
				this.PnpModule.AutorunInfo.InspectionRes.Clear();
				if ( !this.FailToPickNew || this.PnpModule.AutorunInfo.UnloadStage )
				{
					this.StageInfo.StageFlag = PNPToStageFlag.CanClamp;
				}
				if ( ToPlaceInspected ) if ( this.isError( this.QICLiftSeq.Module.EndPickPlace() ) ) return ( int )RunErrors.ERR_LiftEndPickPlace;
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
				if ( this.isError( this.PnpModule.PNPToWaitPos().Result ) ) return ( int )RunErrors.ERR_PNPMoveErr;
				if ( !this.FailToPickNew || this.PnpModule.AutorunInfo.UnloadStage ) this.StageInfo.StageFlag = PNPToStageFlag.CanInspect;
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
				var DelayCycleStop = !this.PnpModule.AutorunInfo.UnloadStage;
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

		public new void StopAuto()
		{
			Task.Run( () =>
			{
				this.RestartSeq = false;
				this.State = SequenceState.IsNotStarted;
				this.MoveToPick?.Wait();
				this.StageSeq?.StopAuto();
				this.NewLiftSeq?.StopAuto();
				this.QICLiftSeq?.StopAuto();
				this.WorkerSeq.Stop();
				this.PnpModule.MoveToStandbyStatus().Wait();
			} );
		}
		protected override void InitFlags()
		{
			this.FailToPickNew = false;
			this.PnpModule.AutorunInfo.Clear();
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
				this.NewLiftSeq?.StartAuto();
				this.QICLiftSeq?.StartAuto();
			} );
		}
		public new void StartAuto()
		{
			Task.Run( () =>
			{
				this.RestartSeq = false;
				this.WorkerSeq.Start();
				this.StageSeq?.StartAuto();
				this.NewLiftSeq?.StartAuto();
				this.QICLiftSeq?.StartAuto();
			} );
		}
		public new void PauseAuto()
		{
			Task.Run( () =>
			{
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
			this.IsFail = true;
			this.StopAuto();
			//}
			//else
			//	this.PauseAuto();
		}
	}
	public class StationSeqBase : SequenceBase
	{
		public bool bCycleStop { get; set; } = false;
		public PNPModule Module { get; protected set; }

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
	MoveToPickPos2nd,//Load arm at new lift, unload arm at stage
	SecondPick,//Pick up 2nd time if stage still has substrate
	MoveToStandby,
	WaitAtStandbyPos,//Wait for inspection to complete
	Finish
}
