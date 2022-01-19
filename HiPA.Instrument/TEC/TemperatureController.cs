using MTVCSEL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using System.Net.Sockets;
using System.Globalization;
using System.IO.Ports;

namespace MTVCSEL.Instrument.TEC
{
	[Serializable]
	public class TECConfiguration
	   : Configuration
	{
		[NonSerialized]
		private static readonly TECConfiguration _DEFAULT = new TECConfiguration();

		[NonSerialized]
		public static readonly string NAME = "EZ_Zone";

		public override string Name { get; set; } = "";

		public override InstrumentCategory Category => InstrumentCategory.TempController;
		public override Type InstrumentType => typeof( TemperatureController );

		public string ServerIpAddress { get; set; } = "169.254.1.1";
		public int ServerIpPort { get; set; } = 502;

		public string SerialPortName { get; set; } = "";
		public int BaudRate { get; set; } = 115200;
		public int DelayOfReceive { get; set; } = 200;
	}

	public class TemperatureController
		: InstrumentBase
	{
		public TECConfiguration Configuration { get; private set; }

		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}

		public override InstrumentCategory Category => InstrumentCategory.SourceMeter;

		private ModbusIpMaster master;

		private TcpClient tcpClient;

		private SerialPort _com;

		protected override int OnCreate()
		{
			return 0;
		}

		protected override int OnInitialize()
		{
			var result = 0;
			if ( this._com == null ) this._com = new SerialPort();
			if ( ( result = this.Open() ) != 0 ) return result;

			return result;
		}

		public int Open()
		{
			var result = 0;
			try
			{
				this.tcpClient = new TcpClient( string.Concat( new string[] { this.Configuration.ServerIpAddress } ), 502 );

			}
			catch ( Exception ex )
			{
				this.tcpClient = null;
				return ErrorManager.RaiseError(
					this,
					$"Unable to do transfer.  Check port: {ex.Message}",
					ErrorTitle.OperationFailure );
			}

			try
			{
				this.master = ModbusIpMaster.CreateIp( this.tcpClient );
			}
			catch ( Exception ex )
			{
				this.tcpClient?.Dispose();
				this.tcpClient = null;
				this.master?.Dispose();
				this.master = null;

				return ErrorManager.RaiseError(
					this,
					$"Unable to do transfer.  Check port: {ex.Message}",
					ErrorTitle.OperationFailure );
			}
			try
			{
				this._com.PortName = this.Configuration.SerialPortName;
				this._com.BaudRate = this.Configuration.BaudRate;
				this._com.DataBits = 8;
				this._com.Parity = Parity.None;
				this._com.StopBits = StopBits.One;
				this._com.Open();
				this._com.ReadTimeout = this.Configuration.DelayOfReceive;
				this._com.WriteTimeout = this.Configuration.DelayOfReceive;
			}
			catch ( Exception ex )
			{
				return ErrorManager.RaiseError( this,
					 $"Temperature Sensor[Serial COM] Open Device Exception.{ex.Message},Port[{this._com.PortName}], BaudRate[{this._com.BaudRate}]. Error : {1}",
					 ErrorTitle.InvalidOperation );
			}
			return result;
		}



		protected override int OnStop()
		{
			this.master?.Dispose();
			this.master = null;
			this.tcpClient?.Close();
			this.tcpClient = null;
			this._com.Close();
			return 0;
		}

		protected override int OnTerminate()
		{
			return this.OnStop();
		}


		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as TECConfiguration;
		}

		public int GetSettingTemperature( out double settingTemperature )
		{
			var result = 0;
			settingTemperature = 0.000d;
			string TempData = "";
			ushort startAddress = ushort.Parse( "202" );
			ushort registerNum = ushort.Parse( "2" );
			ushort[] registers = this.master.ReadHoldingRegisters( startAddress, registerNum );
			for ( int i = 0; i < ( int )registerNum; i++ )
			{
				if ( registerNum > 1 && i % 2 == 1 )
				{
					object obj3 = TempData;
					TempData = string.Concat( new object[]
					{
											obj3,
											(int)startAddress + i,
											" = ",
											registers[i].ToString().PadLeft(5, '0'),
											"  0x",
											registers[i].ToString("X").PadLeft(4, '0'),
											"  ",
											CalcFloat(registers[i].ToString("X").PadLeft(4, '0') + registers[i - 1].ToString("X").PadLeft(4, '0')),
											"\r\n"
					} );
					TempData = CalcFloat( registers[ i ].ToString( "X" ).PadLeft( 4, '0' ) + registers[ i - 1 ].ToString( "X" ).PadLeft( 4, '0' ) );
					settingTemperature = Math.Round( ( double.Parse( TempData ) - 32 ) * 5 / 9, 3 );
				}
				else
				{
					object obj5 = TempData;
					TempData = string.Concat( new object[]
					{
										obj5,
										(int)startAddress + i,
										" = ",
										registers[i].ToString().PadLeft(5, '0'),
										"  0x",
										registers[i].ToString("X").PadLeft(4, '0'),
										"\r\n"
					} );
				}

			}
			return result;
		}

		public int GetCurrentTemperature( out double currentTemperature )
		{
			var result = 0;
			currentTemperature = 0.000d;
			string TempData = "";
			ushort startAddress = ushort.Parse( "240" );
			ushort registerNum = ushort.Parse( "2" );
			ushort[] registers = this.master.ReadHoldingRegisters( startAddress, registerNum );
			for ( int i = 0; i < ( int )registerNum; i++ )
			{
				if ( registerNum > 1 && i % 2 == 1 )
				{
					object obj3 = TempData;
					TempData = string.Concat( new object[]
					{
											obj3,
											(int)startAddress + i,
											" = ",
											registers[i].ToString().PadLeft(5, '0'),
											"  0x",
											registers[i].ToString("X").PadLeft(4, '0'),
											"  ",
											CalcFloat(registers[i].ToString("X").PadLeft(4, '0') + registers[i - 1].ToString("X").PadLeft(4, '0')),
											"\r\n"
					} );
					TempData = CalcFloat( registers[ i ].ToString( "X" ).PadLeft( 4, '0' ) + registers[ i - 1 ].ToString( "X" ).PadLeft( 4, '0' ) );
					currentTemperature = Math.Round( ( double.Parse( TempData ) - 32 ) * 5 / 9, 3 );
				}
				else
				{
					object obj5 = TempData;
					TempData = string.Concat( new object[]
					{
										obj5,
										(int)startAddress + i,
										" = ",
										registers[i].ToString().PadLeft(5, '0'),
										"  0x",
										registers[i].ToString("X").PadLeft(4, '0'),
										"\r\n"
					} );
				}

			}
			return result;
		}

		public int SetTemperature( float setTemperature )
		{
			var result = 0;
			setTemperature = setTemperature * 9 / 5 + 32;
			CalcHex( setTemperature, out ushort HighWord, out ushort LowWord );
			ushort startAddress = ushort.Parse( "202" );
			ushort[] WriteRegs = new ushort[ 2 ];
			WriteRegs[ 0 ] = LowWord;
			WriteRegs[ 1 ] = HighWord;
			this.master.WriteMultipleRegisters( startAddress, WriteRegs );
			return result;
		}

		private string CalcFloat( string HexIn )
		{
			int i = 0;
			byte[] bArray = new byte[ 4 ];
			for ( int j = 0; j <= HexIn.Length - 1; j += 2 )
			{
				bArray[ i ] = byte.Parse( HexIn[ j ].ToString() + HexIn[ j + 1 ].ToString(), NumberStyles.HexNumber );
				i++;
			}
			Array.Reverse( bArray );
			return BitConverter.ToSingle( bArray, 0 ).ToString();
		}

		private int CalcHex( float FloatIn, out ushort HighWord, out ushort LowWord )
		{
			try
			{
				byte[] HexValue = BitConverter.GetBytes( FloatIn );
				HighWord = ( ushort )( ( int )HexValue[ 3 ] << 8 | ( int )HexValue[ 2 ] );
				LowWord = ( ushort )( ( int )HexValue[ 1 ] << 8 | ( int )HexValue[ 0 ] );
				return 0;
			}
			catch
			{
				HighWord = 0;
				LowWord = 0;
				return ErrorManager.RaiseError( this, "Error format of Setting temperature", ErrorTitle.OperationFailure );
			}
		}

		byte[] bytestosend = new byte[ 1 ] { 0x01 };
		byte[] bytestoread;

		public double ReadTemperatureDUT()
		{
			var result = 0;
			try
			{
				this._com.Write( bytestosend, 0, 1 );

				bytestoread = new byte[ this._com.BytesToRead ];

				var readResult = this._com.Read( bytestoread, 0, this._com.BytesToRead );
				if ( readResult != null )
				{
					return ResultTemperatureDUT( bytestoread );
				}
				return result;
			}
			catch ( Exception ex )
			{
				System.Windows.MessageBox.Show( $"Serial Communication Failed. {ex.Message}" );
				return -1;
			}

		}
		double tempResult = 0.0;
		private double ResultTemperatureDUT( byte[] serialTempResult )
		{
			try
			{
				tempResult = ( ( ( serialTempResult[ 0 ] * 256 ) + serialTempResult[ 1 ] ) - 1000 ) / 10.0;
				return tempResult;
			}
			catch (Exception ex)
			{
				Console.WriteLine( $"Result Temperature DUT Error: {ex.Message}" );
				return -1;
			}
			
		}
	}
}
