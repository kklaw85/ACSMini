using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiPA.Communicator.Forms
{
	public partial class SerialPortResourceTestUI : UserControl
	{
		public SerialPortResourceTestUI()
		{
			this.InitializeComponent();
			this.Disposed += ( s, e ) => this.Disconnect();
		}

		System.IO.Ports.SerialPort _session = null;
		public Task<string> Connect( ISerialPortParameter param )
		{
			return Task.Run( () =>
			{
				try
				{
					this._session = SerialCommunicator.CreatePortFromParameter( param );
					this._session.DataReceived += ( s, e ) =>
					{
						try
						{
							var buffer = new byte[ 4096 ];
							var size = this._session.Read( buffer, 0, buffer.Length );

							this.BeginInvoke( new Action( () =>
							{
								this.AppendLog( buffer, size, LogType.Received );
							} ) );
						}
						catch ( Exception ex )
						{
							new Thread( () => MessageBox.Show( $"SerialPortResourceTestUI.cs :Connect:{ex.Message}" ) ).Start();
							this.BeginInvoke( new Action( () =>
							{
								this.AppendLog( $"Receive error:{ex.Message}", LogType.Error );
							} ) );
						}
					};
					return "";
				}
				catch ( Exception ex )
				{
					this.Disconnect();
					new Thread( () => MessageBox.Show( $"SerialPortResourceTestUI.cs :Connect:{ex.Message}" ) ).Start();
					return ex.Message;
				}
			} );
		}

		public void Disconnect()
		{
			this._session?.Close();
			this._session = null;
		}

		private void btnSend_Click( object sender, EventArgs e )
		{
			var send = this.txtSendData.Text.Trim();
			if ( string.IsNullOrEmpty( send ) == true ) return;
			if ( this._session == null || this._session.IsOpen == false ) return;

			try
			{
				var final = this.txtPrefix.Text.Trim() + send + this.txtSuffix.Text.Trim();
				final = final.Replace( "\\n", "\n" ).Replace( "\\r", "\r" );

				var data = Encoding.ASCII.GetBytes( final );
				this._session.Write( data, 0, data.Length );

				this.AppendLog( send, LogType.Sent );
				this.txtSendData.Select( 0, -1 );
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( $"SerialPortResourceTestUI.cs :btnSend_Click:{ex.Message}" ) ).Start();
				this.AppendLog( $"Send data occurred error:{ex.Message}", LogType.Error );
			}
		}

		private void btnClear_Click( object sender, EventArgs e )
		{
			this.txtRecvData.Clear();
		}

		enum LogType
		{
			Sent,
			Received,
			Error,
		}

		private void AppendLog( byte[] message, int size, LogType logType )
		{
			var log = "";
			if ( this.chkShowAsHex.Checked == true )
			{
				log = string.Join( " ", message.Take( size ).Select<byte, string>( b => $"{b:X2}" ) );
			}
			else
			{
				log = Encoding.ASCII.GetString( message, 0, size );
			}
			this.AppendLog( log, logType );
		}
		private void AppendLog( string message, LogType logType )
		{
			var flag =
				logType == LogType.Sent ? "=>" :
				logType == LogType.Received ? "<=" :
				"><";

			this.txtRecvData.AppendText( string.Format( "{0:HH:mm:ss.fff} {1} {2}\r\n", DateTime.Now, flag, message ) );
		}

	}
}
