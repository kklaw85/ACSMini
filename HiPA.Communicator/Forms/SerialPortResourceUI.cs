using HiPA.Common.Forms;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HiPA.Communicator.Forms
{
	public partial class SerialPortResourceUI
		: UserControl
		, ILockableUI
	{
		public SerialPortResourceUI()
		{
			this.InitializeComponent();
			this.SerialPort = this._serialPort;
		}
		string _serialPort = "";
		public string SerialPort
		{
			get => this._serialPort;
			set
			{
				this._serialPort = value;
				if ( this.txtPortName != null ) this.txtPortName.Text = value;
			}
		}

		public string SerialParameterSummary
		{
			get
			{
				return
					this.Parameter is null ?
					"None" :
					$"Port         : {this.Parameter.PortName}\r\n" +
					$"Baudrate     : {this.Parameter.BaudRate}\r\n" +
					$"Data Bits    : {this.Parameter.DataBits}\r\n" +
					$"Parity       : {this.Parameter.Parity}\r\n" +
					$"Stop Bits    : {this.Parameter.StopBits}\r\n" +
					$"Read Timeout : {this.Parameter.ReadTimeout} ms\r\n" +
					$"Read Buffer  : {this.Parameter.ReadBufferSize} Bytes\r\n" +
					$"Write Timeout: {this.Parameter.WriteTimeout} ms\r\n" +
					$"Write Buffer : {this.Parameter.WriteBufferSize} Bytes\r\n";
			}
		}

		SerialPortParameter _parameter;
		[Browsable( false )]
		public SerialPortParameter Parameter
		{
			get => this._parameter;
			set
			{
				if ( value is null ) return;
				this._parameter = new SerialPortParameter( value );
				this.SerialPort = this._parameter.PortName;
			}
		}

		private void btnOpenParameters_Click( object sender, EventArgs e )
		{
			using ( var dlg = new SerialPortParametersForm( this.Parameter ) )
			{
				if ( dlg.ShowDialog() == DialogResult.OK )
				{
					var newParam = dlg.Parameter;
					if ( newParam.Equals( this.Parameter ) == false )
					{
						this.Parameter = newParam as SerialPortParameter;
						this.SerialPortParameterChanged?.Invoke( this, new SerialPortParameterChangedEventArgs( newParam ) );
					}
					this.SerialPort = this.Parameter.PortName;
				}
			}
		}

		public event EventHandler<bool> LockStateChangedHandler;
		bool _locked = false;
		public bool LockUI
		{
			get => this._locked;
			set
			{
				this._locked = value;
				this.btnOpenParameters.Enabled = !this._locked;
				this.LockStateChangedHandler?.Invoke( this, value );
			}
		}

		public event SerialPortParameterChangedEventHandler SerialPortParameterChanged;
		public delegate void SerialPortParameterChangedEventHandler( object sender, SerialPortParameterChangedEventArgs e );
	}

	public class SerialPortParameterChangedEventArgs : EventArgs
	{
		public ISerialPortParameter ConnectParameter { get; set; }
		public SerialPortParameterChangedEventArgs( ISerialPortParameter parameter )
		{
			this.ConnectParameter = parameter;
		}

	}
}
