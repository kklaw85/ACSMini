using HiPA.Common;
using NeoWisePlatform.Module;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NeoWisePlatform.Sequence
{
	public class LiftSeq : SequenceBase
	{
		public LiftModuleBase Module { get; private set; }
		public LiftSeq( LiftModuleBase liftmodule )
		{
			this.Module = liftmodule;
			this.WorkerSeq = new WorkerThread( "Stage Seq", this );
			this.WorkerSeq.OnThreadStoppedOnError += this.OnThreadStoppedError;
			this.InitSeqFunction();
		}
		public LiftAutorunInfo AutorunInfo => this.Module.AutorunInfo;
		protected override void InitSeqFunction()
		{
			this.SeqList.Add( new FunctionObjects( Run_State_Lift.Init.ToString(), this.Init, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_State_Lift.MoveToStandbyPos.ToString(), this.MoveToStandbyPos, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_State_Lift.SelectWork.ToString(), this.SelectWork, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_State_Lift.IsAction.ToString(), this.IsAction, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_State_Lift.StopToPickPlace.ToString(), this.StopToPickPlace, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_State_Lift.WaitPickPlaceDone.ToString(), this.WaitPickPlaceDone, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_State_Lift.Finish.ToString(), this.Finish, this.GotoError ) );
			this.WorkerSeq.AssignSeqList( this.SeqList );
			this.ResetAll();
		}

		private int Init()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_State_Lift.Init ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( MachineStateMng.isSimulation ) return ( int )RunErrors.ERR_NoError;
				this.InitFlags();
				return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
		}
		private int MoveToStandbyPos()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_State_Lift.MoveToStandbyPos ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( MachineStateMng.isSimulation ) return ( int )RunErrors.ERR_NoError;
				if ( this.isError( this.Module.MoveToStandbyStatus().Result ) ) return ( int )RunErrors.ERR_LiftMoveToStandby;
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
				if ( this.CompareThreadIndex( Run_State_Lift.SelectWork ) ) return ( int )RunErrors.ERR_Inconformity;
				this.State = SequenceState.IsWaitWork;
				if ( this.StartExecute )
				{
					this.ThreadInitState();
					return this.JumpFunction( this.StepCurrent );
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
				if ( this.CompareThreadIndex( Run_State_Lift.IsAction ) ) return ( int )RunErrors.ERR_Inconformity;
				this.State = SequenceState.IsAction;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int StopToPickPlace()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_State_Lift.StopToPickPlace ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.isError( this.Module.MoveToCollectPos( false ).Result ) ) return ( int )RunErrors.ERR_LiftToPickPlace;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int WaitPickPlaceDone()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_State_Lift.WaitPickPlaceDone ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.Module.AutorunInfo.LiftPNPCom.PickPlaceDone ) return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 200 );
			return this.CycleFunction();
		}
		private int Finish()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_State_Lift.Finish ) ) return ( int )RunErrors.ERR_Inconformity;
				this.ReportError();
				this.Module.StartAuto();
				this.State = SequenceState.Init;
				return this.JumpFunctionEnum( Run_State_Lift.SelectWork );
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
				this.State = SequenceState.IsNotStarted;
				this.WorkerSeq.Stop();
				this.Module.StopAuto().Wait();
				this.ResetAll();
			} );
		}
		private void InitFlags()
		{

		}
		protected override void CycleStop()
		{
			//this.Station1.StationModule.StationSeq.CycleStop();
			//this.Station2.StationModule.StationSeq.CycleStop();
		}
		protected override void ReportError() => this.ReportErrorOnly();
	}
}

public enum Run_State_Lift
{
	Init = 0,//
	MoveToStandbyPos,//Start continuous mode, after that, move to selectwork state
	SelectWork,
	IsAction,
	StopToPickPlace,
	WaitPickPlaceDone,
	Finish//go to movetostandbypos state
}
