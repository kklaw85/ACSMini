using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Report;
using System;

namespace HiPA.Communicator
{
	public interface IConnectParameter
	{
		int ConnectTimeout { get; set; }
		int WriteTimeout { get; set; }
		int WriteBufferSize { get; set; }
		int ReadTimeout { get; set; }
		int ReadBufferSize { get; set; }
	}

	[Serializable]
	public enum ChannelIds : int
	{
		None = -1,
		CH1 = 0,
		CH2,
		CH3,
		CH4,
		CH5,
		CH6,
		CH7,
		CH8,
		CH9,
	}

	[Serializable]
	public abstract class ConnectParameter : BaseUtility, IConnectParameter
	{
		private int _ConnectTimeout = 2000;
		public virtual int ConnectTimeout
		{
			get => this._ConnectTimeout;
			set
			{
				this._ConnectTimeout = value;
				this.OnPropertyChanged( "ConnectTimeout" );
			}
		}
		protected int _WriteTimeout = 1000;
		public virtual int WriteTimeout
		{
			get => this._WriteTimeout;
			set
			{
				this._WriteTimeout = value;
				this.OnPropertyChanged( "WriteTimeout" );
			}
		}
		protected int _WriteBufferSize = 256;
		public virtual int WriteBufferSize
		{
			get => this._WriteBufferSize;
			set
			{
				this._WriteBufferSize = value;
				this.OnPropertyChanged( "WriteBufferSize" );
			}
		}
		protected int _ReadTimeout = 1000;
		public virtual int ReadTimeout
		{
			get => this._ReadTimeout;
			set
			{
				this._ReadTimeout = value;
				this.OnPropertyChanged( "ReadTimeout" );
			}
		}
		protected int _ReadBufferSize = 256;
		public virtual int ReadBufferSize
		{
			get => this._ReadBufferSize;
			set
			{
				this._ReadBufferSize = value;
				this.OnPropertyChanged( "ReadBufferSize" );
			}
		}

		public ConnectParameter()
		{
		}
		public ConnectParameter( IConnectParameter parameter )
		{
			this.ConnectTimeout = parameter.ConnectTimeout;
			this.WriteTimeout = parameter.WriteTimeout;
			this.WriteBufferSize = parameter.WriteBufferSize;
			this.ReadTimeout = parameter.ReadTimeout;
			this.ReadBufferSize = parameter.ReadBufferSize;
		}

		public override bool Equals( object obj )
		{
			if ( obj is ConnectParameter param )
			{
				if ( param.ConnectTimeout != this.ConnectTimeout ) return false;
				if ( param.WriteTimeout != this.WriteTimeout ) return false;
				if ( param.WriteBufferSize != this.WriteBufferSize ) return false;
				if ( param.ReadTimeout != this.ReadTimeout ) return false;
				if ( param.ReadBufferSize != this.ReadBufferSize ) return false;

				return true;
			}
			return false;
		}
	}

	[Serializable]
	public abstract class CommunicatorBase : Disposable
	{
		public object SyncRoot { get; } = new object();
		public string LineSeparator { get; set; } = "";
		public abstract void Connect( IConnectParameter token );
		public abstract bool IsConnected { get; }
		public abstract int Write( byte[] data, int offset, int size );
		public abstract int Read( byte[] buffer, int offset, int size );
		public abstract string Read( ref Byte[] ReadBytes, ref int ReadLen );
		public abstract int WriteLine( string text );
		public abstract string ReadLine();
		public abstract string Query( string text, bool syncMode );
		public abstract void Close();
		public abstract int WriteTimeout { get; set; }
		public abstract int ReadTimeout { get; set; }
		public LoggerHelper _Logger { get; set; }

		protected override void DisposeUnmanagedResources()
		{
			base.DisposeUnmanagedResources();
			this.Close();
		}
	}

	public class CommunicatorException : Exception
	{
		public CommunicatorException( string message )
			: base( message )
		{
		}
		public CommunicatorException( string message, Exception innerException )
			: base( message, innerException )
		{
		}
	}

	public class CommunicatorConnectionException : CommunicatorException
	{
		public CommunicatorConnectionException( string message )
			: base( message )
		{
		}
		public CommunicatorConnectionException( string message, Exception innerException )
			: base( message, innerException )
		{
		}
	}

	public class CommunicatorTimeoutException : CommunicatorException
	{
		public CommunicatorTimeoutException( string message )
			: base( message )
		{
		}
		public CommunicatorTimeoutException( string message, Exception innerException )
			: base( message, innerException )
		{
		}
	}

	public class CommunicatorTransferException : CommunicatorException
	{
		public CommunicatorTransferException( string message )
			: base( message )
		{
		}
		public CommunicatorTransferException( string message, Exception innerException )
			: base( message, innerException )
		{
		}
	}

	public class CommunicatorParsingException : CommunicatorException
	{
		public CommunicatorParsingException( string message )
			: base( message )
		{
		}
		public CommunicatorParsingException( string message, Exception innerException )
			: base( message, innerException )
		{
		}
	}
}
