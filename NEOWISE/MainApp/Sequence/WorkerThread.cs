using HiPA.Common;
using NeoWisePlatform.Sequence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NeoWisePlatform
{
	public class WorkerThread : Disposable
	{
		public delegate void OnFunctionComplete( int Idx, bool IsError );
		public event OnFunctionComplete OnFuncComplete;

		public delegate void OnFunctionStart( int Idx );
		public event OnFunctionStart OnFuncStart;

		public delegate void OnThreadStart();
		public event OnThreadStart OnThreadStarted;

		public delegate void OnThreadStop();
		public event OnThreadStop OnThreadStoppedOnError;

		public string Name { get; set; } = "";

		bool isBusy = false;
		public bool IsBusy => this.isBusy;
		public bool IsExit { get; set; } = true;
		public bool IsError { get; set; } = false;
		public bool StopOnError { get; set; } = true;

		private int idx = 0;
		public int RunIdx
		{
			get => this.idx;
			set
			{
				this.Set( ref this.idx, value, "RunIdx" );
				if ( this.Owner != null )
				{
					this.Owner.Idx = this.RunIdx;
					if ( this.functionList.Count() == 0 || this.functionList == null )
						this.Owner.SeqStep = string.Empty;
					else
						this.Owner.SeqStep = this.functionList[ this.RunIdx ].Name;
				}
			}
		}
		Thread wkThread = null;
		bool pauseSeq = false;
		int NextJumpIndex = -1;
		List<FunctionObjects> functionList = new List<FunctionObjects>();
		private SequenceBase Owner = null;

		public WorkerThread( string myName, SequenceBase seq )
		{
			this.RunIdx = 0;
			this.Name = myName;
			this.Owner = seq;
		}
		protected override void DisposeUnmanagedResources()
		{
			base.DisposeUnmanagedResources();
			this.Stop();
		}

		public void AssignSeqList( List<FunctionObjects> seqList )
		{
			this.functionList = seqList;
		}

		public void AddFunction( string FunctionName, Func<int> onExeFunction )
		{
			FunctionObjects myFun = new FunctionObjects( FunctionName, onExeFunction );
			this.functionList.Add( myFun );
		}

		public void AddFunction( string FunctionName, Func<int> onExeFunction, Func<int, int> onErrorFunction )
		{
			FunctionObjects myFun = new FunctionObjects( FunctionName, onExeFunction, onErrorFunction, false );
			this.functionList.Add( myFun );
		}
		public void AddFunction( string FunctionName, Func<int> onExeFunction, Func<int, int> onErrorFunction, bool ReqInvoke )
		{
			FunctionObjects myFun = new FunctionObjects( FunctionName, onExeFunction, onErrorFunction, ReqInvoke );
			this.functionList.Add( myFun );
		}

		private void WaitPauseOnError()
		{
			while ( this.pauseSeq == true )
			{
				this.isBusy = false;
				if ( this.IsExit ) return;
				//Application.DoEvents();
				Thread.Sleep( 10 );
			}
			this.isBusy = true;
		}

		private void StartWorkerThread()
		{
			var res = 0;
			while ( true )
			{
				this.isBusy = true;
				this.IsError = false;
				this.OnFuncStart?.Invoke( this.RunIdx );

				if ( this.functionList[ this.RunIdx ].isInvokeReq == true )
				{
					res = this.functionList[ this.RunIdx ].Execute();
					Thread.Sleep( 2 );
				}
				else
				{
					res = this.functionList[ this.RunIdx ].Execute();
				}

				if ( res < 0 )
				{
					if ( this.functionList[ this.RunIdx ].OnErrorAssigned() )
					{
						res = this.functionList[ this.RunIdx ].DoError( res ); //Alvin 2/3/17
						Thread.Sleep( 2 );
						this.IsError = true;
					}
				}


				this.OnFuncComplete?.Invoke( this.RunIdx, this.IsError );
				if ( this.StopOnError )
				{
					if ( this.IsError )
					{
						if ( OnThreadStoppedOnError != null )
						{
							OnThreadStoppedOnError();
							Thread.Sleep( 2 );
						}
						this.pauseSeq = true;
					}
				}

				this.WaitPauseOnError();
				if ( this.IsExit ) break;

				if ( this._needJump == true )
				{
					this._needJump = false;
					this.RunIdx = this.NextJumpIndex;
					continue;
				}

				if ( this.IsError == false )
				{
					this.RunIdx++;
					this.RunIdx %= this.functionList.Count;
				}
				if ( this.IsExit ) break;

				Thread.Sleep( 5 );
			}
			this.isBusy = false;
		}

		public int Start()
		{
			this.PreviousIndex = -1;
			if ( this.functionList.Count == 0 ) return -1;
			if ( this.isBusy ) return -2;
			if ( this.IsExit )
			{
				this.IsExit = false;
				this.wkThread = new Thread( new ThreadStart( this.StartWorkerThread ) );
				this.wkThread.IsBackground = true;
				this.wkThread.Start();
			}
			if ( this.pauseSeq ) this.pauseSeq = false;
			this.OnThreadStarted?.Invoke();
			return 0;
		}

		public int ResetStart()
		{
			if ( this.pauseSeq ) this.pauseSeq = false;
			this.OnThreadStarted?.Invoke();

			return 0;
		}
		public int Stop()
		{
			if ( this.functionList.Count == 0 ) return -1;
			this.IsExit = true;
			while ( this.isBusy )
			{
				Thread.Sleep( 50 );
			}
			this.Reset();
			return 0;
		}
		public int Pause()
		{
			this.pauseSeq = true;
			while ( this.isBusy )
			{
				Thread.Sleep( 50 );
			}
			return 0;
		}
		public bool IsPause => this.pauseSeq;

		public int Reset()
		{
			if ( this.functionList.Count == 0 ) return -1;
			if ( this.isBusy ) this.Stop();
			this.RunIdx = 0;
			this.NextJumpIndex = 0;
			this._needJump = false;
			return 0;
		}
		public int CurrentIndex()
		{
			return this.RunIdx;
		}

		bool _needJump = false;
		private int PreviousIndex = -1;
		public int JumpIndex( int Idx )
		{
			if ( this.PreviousIndex != Idx ) this.Owner.ResetTimeWatch();
			this.NextJumpIndex = Idx;
			this._needJump = true;
			this.PreviousIndex = Idx;
			return 0;
		}

	}

}
