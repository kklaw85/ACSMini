using HiPA.Common.Forms;
using System;
using System.Threading;

namespace HiPA.Common
{
	[Serializable]
	public class Disposable : BaseUtility, IDisposable
	{
		protected Disposable()
		{
			this._disposed = 0;
		}
		~Disposable()
		{
			this.Dispose( false );
		}

		public bool IsDisposed
		{
			get
			{
				return this._disposed == 1;
			}
		}

		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}

		private void Dispose( bool disposing )
		{
			if ( Interlocked.CompareExchange( ref this._disposed, 1, 0 ) != 0 )
			{
				return;
			}
			if ( disposing )
			{
				this.DisposeManagedResources();
			}
			this.DisposeUnmanagedResources();
		}

		internal void CheckDisposedAndThrow()
		{
			if ( this.IsDisposed )
			{
				throw new ObjectDisposedException( base.GetType().FullName );
			}
		}

		protected virtual void DisposeManagedResources()
		{
		}

		protected virtual void DisposeUnmanagedResources()
		{
		}

		private int _disposed;
	}
}
