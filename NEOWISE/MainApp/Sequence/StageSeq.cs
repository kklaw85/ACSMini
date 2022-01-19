using HiPA.Common;
using NeoWisePlatform.Module;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NeoWisePlatform.Sequence
{
	public class StageSeq : SequenceBase
	{
		public StageModule Module { get; set; }
		public StageSeq( StageModule module )
		{
			this.Module = module;
			this.WorkerSeq = new WorkerThread( "Stage Seq", this );
			this.WorkerSeq.OnThreadStoppedOnError += this.OnThreadStoppedError;
			this.InitSeqFunction();
		}

		protected override void InitSeqFunction()
		{
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.Init.ToString(), this.Init, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.MoveToStandbyPos.ToString(), this.MoveToStandbyPos, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.SelectWork.ToString(), this.SelectWork, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.IsAction.ToString(), this.IsAction, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.WaitForClampFlag.ToString(), this.WaitForClampFlag, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.StageHold.ToString(), this.StageHold, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.WaitForImageTakingFlag.ToString(), this.WaitForImageTakingFlag, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.CamSingleShot.ToString(), this.CamSingleShot, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.StageReleaseAndImageProcess.ToString(), this.StageReleaseAndImageProcess, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( Run_Stage_Seq.Finish.ToString(), this.Finish, this.GotoError ) );
			this.WorkerSeq.AssignSeqList( this.SeqList );
			this.ResetAll();
		}

		private int Init()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_Stage_Seq.Init ) ) return ( int )RunErrors.ERR_Inconformity;
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
				if ( this.CompareThreadIndex( Run_Stage_Seq.MoveToStandbyPos ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( MachineStateMng.isSimulation ) return ( int )RunErrors.ERR_NoError;
				if ( this.isError( this.Module.MoveToStandbyStatus().Result ) ) return ( int )RunErrors.ERR_StageMoveToStandby;
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
				if ( this.CompareThreadIndex( Run_Stage_Seq.SelectWork ) ) return ( int )RunErrors.ERR_Inconformity;
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
				if ( this.CompareThreadIndex( Run_Stage_Seq.IsAction ) ) return ( int )RunErrors.ERR_Inconformity;
				this.State = SequenceState.IsAction;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int WaitForClampFlag()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_Stage_Seq.WaitForClampFlag ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( ( int )this.Module.AutorunInfo.StageFlag >= ( int )PNPToStageFlag.CanClamp ) return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 100 );
			return this.CycleFunction();
		}
		private int StageHold()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_Stage_Seq.StageHold ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.isError( this.Module.Stage.Hold().Result ) ) return ( int )RunErrors.ERR_StageHold;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int WaitForImageTakingFlag()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_Stage_Seq.WaitForImageTakingFlag ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( ( int )this.Module.AutorunInfo.StageFlag >= ( int )PNPToStageFlag.CanInspect ) return ( int )RunErrors.ERR_NoError;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			Thread.Sleep( 100 );
			return this.CycleFunction();
		}
		private int CamSingleShot()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_Stage_Seq.CamSingleShot ) ) return ( int )RunErrors.ERR_Inconformity;
				if ( this.isError( this.Module.SnapShot().Result ) ) return ( int )RunErrors.ERR_StageImageShot;
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int StageReleaseAndImageProcess()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_Stage_Seq.StageReleaseAndImageProcess ) ) return ( int )RunErrors.ERR_Inconformity;
				var ErrCn = 0;
				var tasks = new Task<ErrorResult>[]
				{
					this.Module.VisionCheck(),
					this.Module.Stage.Release(),
				};
				Task.WaitAll( tasks );
				foreach ( var task in tasks )
				{
					if ( this.isError( task.Result ) )
					{
						if ( ErrCn == 0 ) return ( int )RunErrors.ERR_StageVisionProcessing;
						else if ( ErrCn == 1 ) return ( int )RunErrors.ERR_StageRelease;
					}
					ErrCn++;
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_UnexpectedException;
			}
			return ( int )RunErrors.ERR_NoError;
		}
		private int Finish()
		{
			try
			{
				if ( this.CompareThreadIndex( Run_Stage_Seq.Finish ) ) return ( int )RunErrors.ERR_Inconformity;
				this.ReportError();
				this.State = SequenceState.Init;
				return this.JumpFunctionEnum( Run_Stage_Seq.SelectWork );
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
				this.Module.MoveToStandbyStatus().Wait();
				this.ResetAll();
			} );
		}
		private void InitFlags()
		{
			this.Module.AutorunInfo.Clear();
		}
		protected override void ReportError() => this.ReportErrorOnly();
		protected override void CycleStop()
		{

		}
	}
}

public enum Run_Stage_Seq
{
	Init = 0,//
	MoveToStandbyPos,//open clamper, off vacuum
	SelectWork,
	IsAction,
	WaitForClampFlag,
	StageHold,//Vacuum on to hold onto sub
	WaitForImageTakingFlag,
	CamSingleShot,//push vision processing to another thread
	StageReleaseAndImageProcess,//Stage clamp release and image processing
	Finish,
}
