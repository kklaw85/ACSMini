using HiPA.Common;
using HiPA.Common.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiPA.Communicator.Forms
{
	public partial class SerialPortParametersUI : UserControl
	{
		public SerialPortParametersUI( ISerialPortParameter parameter = null )
		{
			this.InitializeComponent();

			this.cmbBaudrate.AddItem( 1200 );
			this.cmbBaudrate.AddItem( 2400 );
			this.cmbBaudrate.AddItem( 4800 );
			this.cmbBaudrate.AddItem( 9600 );
			this.cmbBaudrate.AddItem( 19200 );
			this.cmbBaudrate.AddItem( 38400 );
			this.cmbBaudrate.AddItem( 57600 );
			this.cmbBaudrate.AddItem( 115200 );
			this.cmbBaudrate.AddItem( 230400 );
			this.cmbBaudrate.AddItem( 460800 );
			this.cmbBaudrate.AddItem( 921600 );

			this.cmbDataBits.AddItem( 5 );
			this.cmbDataBits.AddItem( 6 );
			this.cmbDataBits.AddItem( 7 );
			this.cmbDataBits.AddItem( 8 );

			this.cmbParity.AddItem( Parity.None );
			this.cmbParity.AddItem( Parity.Odd );
			this.cmbParity.AddItem( Parity.Even );
			this.cmbParity.AddItem( Parity.Mark );
			this.cmbParity.AddItem( Parity.Space );

			this.cmbStopBits.AddItem( "1", StopBits.One );
			this.cmbStopBits.AddItem( "1.5", StopBits.OnePointFive );
			this.cmbStopBits.AddItem( "2", StopBits.Two );

			this.RefreshPorts();

			if ( parameter != null )
			{
				this.Parameter = parameter;
			}
			else
			{
				this.Baudrate = ComBaudRate.BR9600;
				this.DataBits = ComDataBits.Eight;
				this.Parity = Parity.None;
				this.StopBits = StopBits.One;
			}
		}

		public async void RefreshPorts()
		{
			var selectedPort = this.cmbPortName.SelectedValue<string>();
			this.cmbPortName.Items.Clear();

			var ports = await this._RefreshPorts();

			if ( ports != null )
			{
				var sorted = ports.ToList();
				sorted.Sort( ( x, y ) => x.PortNo - y.PortNo );
				foreach ( var port in sorted )
					this.cmbPortName.AddItem( port.Describe, port.PortName );

				if ( string.IsNullOrEmpty( selectedPort ) == false )
				{
					this.SerialPort = selectedPort;
				}
			}
		}

		private Task<IEnumerable<(int PortNo, string PortName, string Describe)>> _RefreshPorts()
		{
			return Task.Run( () =>
			{
				try
				{
					return SerialCommunicator.GetSerialPorts();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.ShowMessage( $"SerialPortParametersUI.cs :_RefreshPorts:{ex.Message}", ErrorTitle.InvalidArgument );
					return null;
				}
			} );
		}

		[Browsable( false )]
		public ISerialPortParameter Parameter
		{
			get
			{
				return new SerialPortParameter()
				{
					PortName = this.SerialPort,
					BaudRate = this.Baudrate,
					DataBits = this.DataBits,
					Parity = this.Parity,
					StopBits = this.StopBits,
					ReadTimeout = this.ReadTimeout,
					ReadBufferSize = this.ReadBufferSize,
					WriteTimeout = this.WriteTimeout,
					WriteBufferSize = this.WriteBufferSize,
				};
			}

			set
			{
				if ( value is null ) return;

				this.SerialPort = value.PortName;
				this.Baudrate = value.BaudRate;
				this.DataBits = value.DataBits;
				this.Parity = this.Parity;
				this.StopBits = this.StopBits;
				this.ReadTimeout = this.ReadTimeout;
				this.ReadBufferSize = this.ReadBufferSize;
				this.WriteTimeout = this.WriteTimeout;
				this.WriteBufferSize = this.WriteBufferSize;
			}
		}


		string _serialPort = "";
		public string SerialPort
		{
			get => this.cmbPortName.SelectedValue<string>();
			set
			{
				if ( this._serialPort != value )
				{
					this._serialPort = value;
					this.cmbPortName.SelectByValue( value );
				}
			}
		}

		ComBaudRate _baudrate = ComBaudRate.BR9600;
		[DefaultValue( 9600 )]
		public ComBaudRate Baudrate
		{
			get => this.cmbBaudrate.SelectedValue<ComBaudRate>();
			set
			{
				if ( this._baudrate != value )
				{
					this._baudrate = value;
					this.cmbBaudrate.SelectByValue( value );
				}
			}
		}

		ComDataBits _dataBits = ComDataBits.Eight;
		public ComDataBits DataBits
		{
			get => this.cmbDataBits.SelectedValue<ComDataBits>();
			set
			{
				if ( this._dataBits != value )
				{
					this._dataBits = value;
					this.cmbDataBits.SelectByValue( value );
				}
			}
		}

		public Parity Parity
		{
			get => this.cmbParity.SelectedValue<Parity>();
			set => this.cmbParity.SelectByValue( value );
		}

		StopBits _stopBits;
		public StopBits StopBits
		{
			get => this.cmbStopBits.SelectedValue<StopBits>();
			set
			{
				if ( this._stopBits != value )
				{
					this._stopBits = value;
					this.cmbStopBits.SelectByValue( value );
				}
			}
		}

		[DefaultValue( 5000 )]
		public int ReadTimeout
		{
			get => ( int )this.numReadTimeout.Value;
			set => this.numReadTimeout.Value = value;
		}
		[DefaultValue( 409600 )]
		public int ReadBufferSize
		{
			get => ( int )this.numReadBufferSize.Value;
			set => this.numReadBufferSize.Value = value;
		}
		[DefaultValue( 5000 )]
		public int WriteTimeout
		{
			get => ( int )this.numWriteTimeout.Value;
			set => this.numWriteTimeout.Value = value;
		}
		[DefaultValue( 409600 )]
		public int WriteBufferSize
		{
			get => ( int )this.numWriteBufferSize.Value;
			set => this.numWriteBufferSize.Value = value;
		}
	}
}
