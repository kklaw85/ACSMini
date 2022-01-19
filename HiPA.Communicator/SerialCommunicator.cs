using HiPA.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HiPA.Communicator
{
	public enum ComBaudRate
	{
		//BR1200 = 1200,
		//BR2400 = 2400,
		//BR4800 = 4800,
		BR9600 = 9600,
		BR19200 = 19200,
		BR38400 = 38400,
		BR57600 = 57600,
		BR115200 = 115200,
		//BR230400 = 230400,
		//BR460800 = 460800,
		//BR921600 = 9216100
	}
	public enum ComDataBits
	{
		Five = 5,
		Six,
		Seven,
		Eight
	}
	public enum ComParity
	{
		None = 0,
		Odd,
		Even,
		Mark,
		Space
	}
	public enum ComStopBits
	{
		None = 0,
		One,
		Two,
		OnePointFive,
	}
	public interface ISerialPortParameter : IConnectParameter
	{
		string PortName { get; set; }
		string SerialNumber { get; set; }
		ComBaudRate BaudRate { get; set; }
		Parity Parity { get; set; }
		ComDataBits DataBits { get; set; }
		StopBits StopBits { get; set; }
		bool DtrEnable { get; set; }
		bool RtsEnable { get; set; }
		bool UseEvent { get; set; }
	}

	[Serializable]
	public class SerialPortParameter : ConnectParameter, ISerialPortParameter
	{
		private string s_PortName = string.Empty;
		public string PortName
		{
			get => this.s_PortName;
			set => this.Set( ref this.s_PortName, value, "PortName" );
		}

		private ComBaudRate e_BaudRate = ComBaudRate.BR9600;
		[DefaultValue( 9600 )]
		public ComBaudRate BaudRate
		{
			get => this.e_BaudRate;
			set => this.Set( ref this.e_BaudRate, value, "BaudRate" );
		}

		private Parity e_Parity = Parity.None;
		public Parity Parity
		{
			get => this.e_Parity;
			set => this.Set( ref this.e_Parity, value, "Parity" );
		}
		private ComDataBits e_DataBits = ComDataBits.Eight;
		public ComDataBits DataBits
		{
			get => this.e_DataBits;
			set => this.Set( ref this.e_DataBits, value, "DataBits" );
		}
		private StopBits e_StopBits = StopBits.One;
		public StopBits StopBits
		{
			get => this.e_StopBits;
			set => this.Set( ref this.e_StopBits, value, "StopBits" );
		}
		private string s_SerialNumber = string.Empty;
		public string SerialNumber
		{
			get => this.s_SerialNumber;
			set => this.Set( ref this.s_SerialNumber, value, "SerialNumber" );
		}
		private bool b_DtrEnable = false;
		public bool DtrEnable
		{
			get => this.b_DtrEnable;
			set => this.Set( ref this.b_DtrEnable, value, "DtrEnable" );
		}
		private bool b_RtsEnable = false;
		public bool RtsEnable
		{
			get => this.b_RtsEnable;
			set => this.Set( ref this.b_RtsEnable, value, "RtsEnable" );
		}
		private bool b_UseEvent = true;
		public bool UseEvent
		{
			get => this.b_UseEvent;
			set => this.Set( ref this.b_UseEvent, value, "UseEvent" );
		}

		public SerialPortParameter()
		{
		}
		public SerialPortParameter( ISerialPortParameter parameter ) : base( parameter )
		{
			this.SerialNumber = parameter.SerialNumber;
			this.PortName = parameter.PortName;
			this.BaudRate = parameter.BaudRate;
			this.Parity = parameter.Parity;
			this.DataBits = parameter.DataBits;
			this.StopBits = parameter.StopBits;
		}

		public override bool Equals( object obj )
		{
			if ( base.Equals( obj ) == false ) return false;

			if ( obj is ISerialPortParameter param )
			{
				if ( param.SerialNumber != this.SerialNumber ) return false;
				if ( param.PortName != this.PortName ) return false;
				if ( param.BaudRate != this.BaudRate ) return false;
				if ( param.Parity != this.Parity ) return false;
				if ( param.DataBits != this.DataBits ) return false;
				if ( param.StopBits != this.StopBits ) return false;

				return true;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class SerialCommunicator
		: CommunicatorBase
	{
		public string PortName => this._COM != null ? this._COM.PortName : "";
		SerialPort _COM = new SerialPort();
		public override bool IsConnected => this._COM.IsOpen;
		public string Separator { get; set; } = "\n\r";

		public override int WriteTimeout { get => this._COM.WriteTimeout; set => this._COM.WriteTimeout = value; }
		public override int ReadTimeout { get => this._COM.ReadTimeout; set => this._COM.ReadTimeout = value; }
		public override void Close()
		{
			this._COM?.Close();
		}
		private string sDataRecv = "";
		private int iRecvLen = 0;
		private byte[] DataRecv;

		public override void Connect( IConnectParameter token )
		{
			if ( token is ISerialPortParameter parameter )
			{
				this.Close();
				try
				{
					this._COM.PortName = parameter.PortName;
					this._COM.BaudRate = ( int )parameter.BaudRate;
					this._COM.Parity = parameter.Parity;
					this._COM.DataBits = ( int )parameter.DataBits;
					this._COM.StopBits = parameter.StopBits;
					this._COM.WriteBufferSize = parameter.WriteBufferSize;
					this._COM.ReadBufferSize = parameter.ReadBufferSize;
					this._COM.DtrEnable = parameter.DtrEnable;
					this._COM.RtsEnable = parameter.RtsEnable;

					//this._COM.Handshake = Handshake.XOnXOff;
					//this._COM.DiscardNull = false;
					//this._COM.NewLine = "\n";

					this.ReadTimeout = parameter.ReadTimeout;
					this.WriteTimeout = parameter.WriteTimeout;
					if ( MachineStateMng.isSimulation ) return;
					if ( this._COM.IsOpen )
						this._COM.Close();
					this._COM.Open();
					if ( parameter.UseEvent )
						this._COM.DataReceived += new SerialDataReceivedEventHandler( this.OnComm_DataReceived );
				}
				catch ( Exception ex )
				{
					throw new CommunicatorConnectionException( "SerialCommunicator.cs :Connect: Connect failed", ex );
				}
			}
			else
				throw new CommunicatorConnectionException( "SerialCommunicator.cs :Connect: The token type MUST be a ISerialPortParameter" );
		}

		public override int WriteLine( string text )
		{
			text += this.Separator;

			var data = Encoding.ASCII.GetBytes( text );
			var toSend = data.Length;
			var sentSize = 0;
			while ( toSend > 0 )
			{
				var size = this.Write( data, sentSize, toSend );
				sentSize += size;
				toSend -= toSend;
			}
			return sentSize;
		}
		public string ReadExisting()
		{
			return this._COM.ReadExisting();
		}
		public string ReadString( string error = "" )
		{
			var result = "";
			var sw = new Stopwatch();
			sw.Restart();
			while ( sw.ElapsedMilliseconds <= this.ReadTimeout )
			{
				var read = this._COM.ReadExisting();
				if ( string.IsNullOrEmpty( read ) == false )
				{
					result += read;
					if ( result.LastIndexOf( this.Separator ) >= 0 )
						break;
					if ( error != "" && result.IndexOf( error ) >= 0 )
					{
						result = "";
						break;
					}
				}
				Thread.Sleep( 10 );
			}
			return result;
		}
		public override string ReadLine()
		{
			return this.ReadString();
		}
		public override string Query( string text, bool syncMode )
		{
			try
			{
				if ( syncMode == true ) Monitor.Enter( this.SyncRoot );
				if ( this.WriteLine( text ) == 0 ) return "";
				return this.ReadLine();
			}
			finally
			{
				if ( syncMode == true ) Monitor.Exit( this.SyncRoot );
			}
		}

		public override int Read( byte[] buffer, int offset, int size )
		{
			try
			{
				var sw = new Stopwatch();
				sw.Restart();
				while ( sw.ElapsedMilliseconds <= this.ReadTimeout )
				{
					if ( this._COM.BytesToRead >= size )
						return this._COM.Read( buffer, offset, size );
					Thread.Sleep( 10 );
				}
				sw.Stop();
				throw new TimeoutException();
			}
			catch
			{
				//throw new CommunicatorConnectionException( "SerialCommunicator.cs :Read: port is not open", ioex );
			}
			return -99;
		}

		public override string Read( ref Byte[] ReadBytes, ref int ReadLen )
		{
			var sErr = string.Empty;

			try
			{
				var sw = new Stopwatch();
				sw.Restart();
				while ( sw.ElapsedMilliseconds <= this.ReadTimeout )
				{
					if ( this.iRecvLen > 0 )
					{
						ReadBytes = new byte[ this.iRecvLen ];
						Array.Copy( this.DataRecv, ReadBytes, this.iRecvLen );
						ReadLen = this.iRecvLen;
						this.DataRecv = null;
						return sErr;
					}
					Thread.Sleep( 10 );
				}
			}
			catch ( InvalidOperationException ioex )
			{
				sErr = $"SerialCommunicator.cs :Read: port is not open: {ioex.Message}";
			}
			catch ( TimeoutException tex )
			{
				sErr = $"SerialCommunicator.cs :Read: Transfer Timeout: {tex.Message}";
			}
			catch ( Exception ex )
			{
				sErr = $"SerialCommunicator.cs :Read: {ex.Message}";
			}
			return sErr;
		}

		public override int Write( byte[] data, int offset, int size )
		{
			try
			{
				this.ClearInOutBuff();
				this._COM.Write( data, offset, size );
				return size;
			}
			catch ( InvalidOperationException ioex )
			{
				throw new CommunicatorConnectionException( "SerialCommunicator.cs :Write: port is not open", ioex );
			}
			catch ( TimeoutException tex )
			{
				throw new CommunicatorTimeoutException( "SerialCommunicator.cs :Write: Transfer Timeout", tex );
			}
			catch ( Exception ex )
			{
				throw new CommunicatorException( "SerialCommunicator.cs :Write: Argument is invalid", ex );
			}
		}


		public void OnComm_DataReceived( object sender, SerialDataReceivedEventArgs e )
		{
			try
			{
				Thread.Sleep( 100 );
				this.DataRecv = new byte[ this._COM.ReadBufferSize ];
				int Len = this._COM.BytesToRead;
				byte[] recv = new byte[ Len ];
				int ReadLen = this._COM.Read( recv, 0, Len );
				if ( ( this.iRecvLen + ReadLen ) > 1000 )   // ³¬³öbuffer¾Í²»ÔÙ½ÓÊÕ
					return;
				recv.CopyTo( this.DataRecv, this.iRecvLen );
				string str = System.Text.Encoding.Default.GetString( recv );
				this.sDataRecv += str;
				this.iRecvLen += ReadLen;
			}
			catch
			{ }
		}
		public int QueryDataBytes( Byte[] SendBytes, ref Byte[] ReadBytes )
		{
			int ReadLen = ReadBytes.Length;
			if ( this.QueryDataBytes( SendBytes, SendBytes.Length, ref ReadBytes, ref ReadLen ) == 0 )
				return ReadLen;
			return 0;
		}

		//public int QueryDataBytes( Byte[] SendBytes, int SendLen, ref Byte[] ReadBytes, ref int ReadLen, int TimeoutMs = 1000 )
		//{
		//	try
		//	{
		//		//int iReadLen = ReadLen;
		//		ReadLen = 0;
		//		this.Write( SendBytes, 0, SendLen );
		//		DateTime st = DateTime.Now;
		//		while ( true )
		//		{
		//			TimeSpan ts = DateTime.Now - st;
		//			if ( ts.TotalMilliseconds > TimeoutMs )
		//				break;
		//			if ( iRecvLen > 0 )
		//			{
		//				Array.Copy( DataRecv, ReadBytes, iRecvLen );
		//				//      DataRecv.CopyTo(ReadBytes, 0);
		//				ReadLen = iRecvLen;
		//				break;
		//			}

		//		}
		//		if ( ReadLen > 0 )
		//			return 0;
		//	}
		//	catch ( Exception ex )
		//	{
		//		return -2;
		//	}
		//	return -3;   // timeout
		//}
		//#Howie backup
		public int QueryDataBytes( Byte[] SendBytes, int SendLen, ref Byte[] ReadBytes, ref int ReadLen )
		{
			try
			{
				ReadLen = 0;
				this.Write( SendBytes, 0, SendLen );
				this.Read( ref ReadBytes, ref ReadLen );
				if ( ReadLen > 0 )
					return 0;
			}
			catch
			{
				return -2;
			}
			return -3;   // timeout
		}

		public string QueryDataString( string SendStr )
		{
			try
			{
				Byte[] ReadBytes = null;

				int ReadLen = 0;
				this.ClearInOutBuff();
				this.WriteLine( SendStr );
				this.Read( ref ReadBytes, ref ReadLen );
				if ( ReadBytes != null )
					return Encoding.ASCII.GetString( ReadBytes );
				return string.Empty;
			}
			catch
			{
				return "";
			}
		}

		public void ClearInOutBuff()
		{
			this._COM.DiscardInBuffer();
			this._COM.DiscardOutBuffer();
			this.sDataRecv = "";
			this.iRecvLen = 0;
			this.DataRecv = null;
		}
		public static (int PortNo, string PortName, string Describe) ParsePortName( string portDescribe )
		{
			var match = System.Text.RegularExpressions.Regex.Match( portDescribe, @"^(?<NAME>.*)\(COM(?<COM>\d+).*?\).*?$", RegexOptions.IgnoreCase );

			var name = match.Groups[ "NAME" ].Value;
			var com = match.Groups[ "COM" ].Value;
			int.TryParse( com, out var portNo );

			var portName = $"COM{com}";
			var describe = $"(COM{com}) {name}";
			return (portNo, portName, describe);
		}
		public static IEnumerable<(int PortNo, string PortName, string Describe)> GetSerialPorts()
		{
			var ports = HardInfoEnum.MulGetHardwareInfo( HardwareEnum.Win32_PnPEntity, "Name" );
			foreach ( var port in ports )
			{
				yield return ParsePortName( port );
			}
		}

		public static SerialPort CreatePortFromParameter( ISerialPortParameter parameter )
		{
			var port = new SerialPort();
			port.PortName = parameter.PortName;
			port.BaudRate = ( int )parameter.BaudRate;
			port.Parity = parameter.Parity;
			port.DataBits = ( int )parameter.DataBits;
			port.StopBits = parameter.StopBits;
			port.WriteBufferSize = parameter.WriteBufferSize;
			port.ReadBufferSize = parameter.ReadBufferSize;
			port.Open();
			return port;
		}
		public static SerialPort CreatePortFromParameter( SerialPortParameter parameter )
		{
			var port = new SerialPort();
			port.PortName = parameter.PortName;
			port.BaudRate = ( int )parameter.BaudRate;
			port.Parity = parameter.Parity;
			port.DataBits = ( int )parameter.DataBits;
			port.StopBits = parameter.StopBits;
			port.WriteBufferSize = parameter.WriteBufferSize;
			port.ReadBufferSize = parameter.ReadBufferSize;
			port.Open();
			return port;
		}
	}
}
