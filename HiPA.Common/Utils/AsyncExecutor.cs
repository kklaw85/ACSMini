using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HiPA.Common
{
	public class AsyncExecutor
	{
		class DelegateAgent
		{
			public SendOrPostCallback UserFunction;
			public object UserParam;

			public Action<Exception> OnException;
			public Exception InnerException;
			ManualResetEvent _waitObj = new ManualResetEvent( false );

			public void Wait() => this._waitObj.WaitOne();
			public void Set() => this._waitObj.Set();
		}

		ConcurrentQueue<DelegateAgent> _delegateQueue = new ConcurrentQueue<DelegateAgent>();
		ManualResetEvent _waitJob = new ManualResetEvent( false );

		public void Send( SendOrPostCallback callback, object state )
		{
			var agent = new DelegateAgent
			{
				UserFunction = callback,
				UserParam = state,
			};
			this._delegateQueue.Enqueue( agent );
			this._waitJob.Set();
			agent.Wait();
			if ( agent.InnerException != null )
				throw agent.InnerException;
		}
		public void Post( SendOrPostCallback callback, object state, Action<Exception> onException = null )
		{
			var agent = new DelegateAgent
			{
				UserFunction = callback,
				UserParam = state,
				OnException = onException,
			};
			this._delegateQueue.Enqueue( agent );
			this._waitJob.Set();
		}

		bool _endWork = true;
		Task _worker = null;
		public bool Running
		{
			get => this._endWork;
			set
			{
				if ( value == false )
				{
					this._endWork = true;
					return;
				}

				if ( this._worker != null ) return;

				if ( this._worker == null )
				{
					this._endWork = false;
					this._worker = Task.Run( () =>
					{
						while ( this._endWork == false )
						{
							this._waitJob.WaitOne();
							this._waitJob.Reset();

							while ( this._delegateQueue.TryDequeue( out var item ) )
							{
								try
								{
									item.UserFunction.DynamicInvoke( item.UserParam );
								}
								catch ( Exception ex )
								{
									item.InnerException = ex;
									item.OnException?.Invoke( ex );
									Equipment.ErrManager.ShowMessage( $"AysncExecutor.cs :Running:{ex.Message}", ErrorTitle.InvalidArgument );
								}
								item.Set();
							}
						}
					} );
				}
			}
		}
	}
}
