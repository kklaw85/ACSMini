using HiPA.Common;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using NeoWisePlatform.Module;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NeoWisePlatform.Sequence
{
	public class AutoSeq : SequenceBase
	{
		public bool Start_Confirm = false;
		Stopwatch _cycleTime = null;

		private StationSequences Station = null;
		public AutoSeq( StationSequences Station )
		{
			this.Station = Station;
			this.WorkerSeq = new WorkerThread( "AutoSeqThread", this );
			this.WorkerSeq.OnThreadStoppedOnError += this.OnThreadStoppedError;
			this.InitSeqFunction();
			this._cycleTime = new Stopwatch();

			//this._equipment.IoBoard.GetIoPoint( InputIOlist1.In_Start ).IoValueChangedEvent += this.AutoSeq_IoValueChangedEvent;
		}

		protected override void InitSeqFunction()
		{
			this.SeqList.Add( new FunctionObjects( RUN_AUTO.Init.ToString(), this.Init, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( RUN_AUTO.WaitStartButton.ToString(), this.WaitStartButton, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( RUN_AUTO.StationIsAction.ToString(), this.StationIsAction, this.GotoError ) );
			this.SeqList.Add( new FunctionObjects( RUN_AUTO.CheckThreadsForError.ToString(), this.CheckThreadsForError, this.GotoError ) );
			this.WorkerSeq.AssignSeqList( this.SeqList );
			this.ResetAll();
		}

		#region  AutoRunStep
		private int Init()
		{
			this.ThreadInitState();
			this._Equipment.MachineLights( false );
			return ( int )RunErrors.ERR_NoError;
		}
		private int WaitStartButton()
		{
			try
			{
				var bOn = this._Equipment.GetIOPointByEnum( InputIO.I_AutoSwitch ).Check( DioValue.On );
				this._cycleTime.Restart();
				if ( bOn || MachineStateMng.isSimulation )
				{
					this.Start_Confirm = false;
					return ( int )RunErrors.ERR_NoError;
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_StartBtnIOErr;
			}
			return this.CycleFunction();
		}
		private int StationIsAction()
		{
			try
			{
				if ( this.Station.PNPSeq.State == SequenceState.IsWaitWork ) this.Station.PNPSeq.SetStepThread( Run_PNP_Seq.IsAction );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				return ( int )RunErrors.ERR_SetAllStationReadyErr;
			}
			Thread.Sleep( 200 );
			return this.CycleFunction();
		}
		private int CheckThreadsForError()
		{
			if ( this.Station.PNPSeq.Idx == 0 )
				this.StopAuto();
			Thread.Sleep( 200 );
			return this.CycleFunction();
		}
		#endregion
		public new void StartAuto()
		{
			Task.Run( () =>
			{
				this.State = SequenceState.IsAction;
				this.WorkerSeq?.Start();
				this.Station?.Start();
				Equipment.MachStateMgr.MachineStatus = MachineStateType.AUTO_RUNNING;
			} );
		}

		public new void StopAuto()
		{
			Task.Run( () =>
			{
				this.Station?.Stop();
				this.WorkerSeq?.Stop();
				Equipment.MachStateMgr.MachineStatus = MachineStateType.READY;
				this.State = SequenceState.IsNotStarted;
			} );
		}
		public void CycleStopAuto()
		{
			Task.Run( () =>
			{
				this.Station?.CycleStop();
				Equipment.MachStateMgr.MachineStatus = MachineStateType.AUTO_CYCLESTOP;
			} );
		}
		public new void PauseAuto()
		{
			Task.Run( () =>
			{
				this.Station?.Pause();
				this.WorkerSeq?.Pause();
				Equipment.MachStateMgr.MachineStatus = MachineStateType.AUTO_PAUSE;
			} );
		}
		protected override void OnThreadStoppedError()
		{
			this.IsFail = true;
			this.StopAuto();
		}
		protected override void CycleStop()
		{
		}
		protected override void ReportError() => this.ReportErrorOnly();
	}
}

public enum RUN_AUTO
{
	Init = 0,
	WaitStartButton,
	StationIsAction,
	CheckThreadsForError,
}

