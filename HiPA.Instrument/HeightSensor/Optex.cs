using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Report;
using HiPA.Communicator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HiPA.Instrument.HeightSensor
{
	public enum ReadingOut
	{
		StopContinuousRead = 0x30,
		StartContinuousRead = 0x31,
		ReadOnce = 0x3F
	}

	public enum InputType
	{
		PNP = 0x30,
		NPN = 0x31
	}

	public enum SamplingPeriod
	{
		P100us = 0x30,
		P200us = 0x31,
		P400us = 0x32,
		P800us = 0x33,
		P1600us = 0x34,
		P3200us = 0x35,
		Auto = 0x36
	}

	public enum BaudRate
	{
		R96K = 0x30, //9.6k
		R192K = 0x31,//19.2k
		R384K = 0x32,//38.4k
		R576K = 0x33,//57.6K
		R1152K = 0x34,//
		R2304K = 0x35,
		R4608K = 0x36,
		R921K = 0x37,
		R18432K = 0x38,
		R3125K = 0x39,
		R6250K = 0x41,
		R12500K = 0x42,
	}

	public enum Sensitivity
	{
		Min = 0x30,
		One = 0x31,
		Two = 0x32,
		Three = 0x33,
		Four = 0x34,
		Five = 0x35,
		Six = 0x36,
		Seven = 0x37,
		Eight = 0x38,
		Nine = 0x39,
		Max = 0x41,
		Auto = 0x42,
	}

	public enum LaserPower
	{
		OFF = 0x30,
		One = 0x31,
		Two = 0x32,
		Three = 0x33,
		Four = 0x34,
		Five = 0x35,
	}
	public enum NoOfAveraging
	{
		Once = 0x30,
		Twice = 0x31,
		Times4 = 0x32,
		Times8 = 0x33,
		Times16 = 0x34,
		Times32 = 0x35,
		Times64 = 0x36,
		Times128 = 0x37,
		Times256 = 0x38,
		Times512 = 0x39,
		Times1024 = 0x41,
		Times2048 = 0x42,
		Times4096 = 0x43
	}

	public enum CalShift
	{
		CalShiftHighByte = 0x48,
		CalShiftMiddleByte = 0x47,
		CalShiftLowByte = 0x46,
	}

	public enum CalSpan
	{
		CalSpanHighByte = 0x4F,
		CalSpanMiddleByte = 0x50,
		CalSpanLowByte = 0x51,

	}
	public enum OptexCommand
	{
		Read = 0x4D,
		InputType = 0x4E,
		SamplingTime = 0x43,
		SetBaudRate = 0x42,
		SetSensitivity = 0x53,
		SetLaserPower = 0x4C,
		CalShiftHighByte = 0x48,
		CalShiftMiddleByte = 0x47,
		CalShiftLowByte = 0x46,
		CalSpanHighByte = 0x4F,
		CalSpanMiddleByte = 0x50,
		CalSpanLowByte = 0x51,
		SetAvgNo = 0x41,
	}
	public enum ReadCmd
	{
		StopCont = 0x30,
		StartCont = 0x31,
		ReadOnce = 0x3F,
	}
	[Serializable]
	public class OptexHeightSensorConfiguration
		: Configuration
	{
		[NonSerialized]
		public static readonly OptexHeightSensorConfiguration _DEFAULT = new OptexHeightSensorConfiguration();
		[NonSerialized]
		public static readonly string NAME = "CD5";

		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.HeightSensor;
		public override Type InstrumentType => typeof( OptexHeightSensor );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public SerialPortParameter ConnectParameter { get; set; }
		private SamplingPeriod e_SamplingPeriod = SamplingPeriod.Auto;
		private double d_Factor = 0.06;
		public double Factor
		{
			get => this.d_Factor;
			set => this.Set( ref this.d_Factor, value, "Factor" );
		}
		private double d_Offset = 90.234;
		public double Offset
		{
			get => this.d_Offset;
			set => this.Set( ref this.d_Offset, value, "Offset" );
		}
		public SamplingPeriod SamplingPeriod
		{
			get => this.e_SamplingPeriod;
			set => this.Set( ref this.e_SamplingPeriod, value, "SamplingPeriod" );
		}
		private LaserPower e_LaserPower = LaserPower.Five;
		public LaserPower LaserPower
		{
			get => this.e_LaserPower;
			set => this.Set( ref this.e_LaserPower, value, "LaserPower" );
		}
		private NoOfAveraging e_NoOfAveraging = NoOfAveraging.Once;
		public NoOfAveraging NoOfAveraging
		{
			get => this.e_NoOfAveraging;
			set => this.Set( ref this.e_NoOfAveraging, value, "NoOfAveraging" );
		}
		private Sensitivity e_Sensitivity = Sensitivity.Eight;
		public Sensitivity Sensitivity
		{
			get => this.e_Sensitivity;
			set => this.Set( ref this.e_Sensitivity, value, "Sensitivity" );
		}
		private bool b_EnHeightLogging = false;
		public bool EnHeightLogging
		{
			get => this.b_EnHeightLogging;
			set => this.Set( ref this.b_EnHeightLogging, value, "EnHeightLogging" );
		}
	}

	public class OptexHeightSensor
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		public OptexHeightSensorConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.HeightSensor;
		SerialCommunicator _session = new SerialCommunicator();

		public bool bStartMonitoring = false;

		const byte Stx = 0x02;
		const byte Etx = 0x03;
		public OptexStatus Status { get; private set; }
		protected override string OnCreate()
		{
			base.Create();
			if ( this.Status == null )
				this.Status = new OptexStatus( this );
			this._Logger = new LoggerHelper( this.Name, "Logger" );
			return String.Empty;
		}
		protected override string OnInitialize()
		{
			var Result = string.Empty;
			try
			{
				if ( this.Configuration.ConnectParameter == null )
					this.Configuration.ConnectParameter = new SerialPortParameter();
				if ( this.IsOpen == false )
				{
					if ( ( Result = this.Open() ) != String.Empty )
					{
						Result = this.OffLaserSpot();
						if ( Result != string.Empty )
							throw new Exception( Result );
						return Result;
					}
				}
				Result = this.OffLaserSpot();
				if ( Result != string.Empty )
					throw new Exception( Result );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		protected override string OnStop()
		{
			this.Close();
			return String.Empty;
		}
		protected override string OnTerminate()
		{
			this.Close();
			return String.Empty;
		}

		public string Open()
		{
			var Result = String.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( this._session == null ) this._session = new SerialCommunicator();
				this.Configuration.ConnectParameter.UseEvent = false;
				this._session.Connect( this.Configuration.ConnectParameter );
				this._session.Separator = "";
			}
			catch ( CommunicatorConnectionException cex )
			{
				var portName = this.Configuration.ConnectParameter?.PortName;
				var baudRate = this.Configuration.ConnectParameter?.BaudRate;
				Result = this.FormatErrMsg( this.Name, cex );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}
		public bool IsOpen => this._session?.IsConnected == true;
		public string Close()
		{
			var Result = string.Empty;
			try
			{
				this._session?.Dispose();
				this._session = null;
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			return Result;
		}

		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as OptexHeightSensorConfiguration;
		}
		private (int Error, double Value, string Desc) ParseResponse( string response )
		{
			var parts = response.Split( ',' );
			if ( parts == null || parts.Length < 2 ) return (-1, 0, "Response packet format invalid");
			if ( parts[ 0 ].ToUpper() == "ER" )
			{
				return (-2, 0, $"Response error code: {parts[ 1 ]}");
			}
			if ( double.TryParse( parts[ 1 ], out var value ) == true )
				return (0, value, "");
			return (-3, 0, "Response value invalid");
		}
		public string GetDisplacement( out double value )
		{
			value = 0;
			int retries = 0;
			var Result = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
				string sError = string.Empty;
				Retry:
				sError = this.OnLaserSpot();
				if ( sError != string.Empty )
				{
					if ( retries++ < 5 )
					{
						Thread.Sleep( 10 );
						goto Retry;
					}
					else
						throw new Exception( sError );
				}
				Thread.Sleep( 10 );
				Result = this.Status.GetHeight();
				if ( Result != string.Empty )
					throw new Exception( Result );
				value = this.Status.CurrentHeight;
			}
			catch ( Exception ex )
			{
				value = -99;
				Result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return Result;
		}
		public string SetReadingOut( ReadingOut hex )
		{
			var Error = string.Empty;
			try
			{
				Error = this.SetParameter( ( byte )OptexCommand.Read, ( byte )hex );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string SetInputType( InputType hex )
		{
			var Error = string.Empty;
			try
			{
				Error = this.SetParameter( ( byte )OptexCommand.InputType, ( byte )hex );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string GetInputType( out InputType? hexResult )
		{
			var Error = string.Empty;
			hexResult = null;
			try
			{
				byte Result = 0;
				if ( ( Error = this.GetParameter( ( byte )OptexCommand.InputType, out Result ) ) != String.Empty )
					throw new Exception( Error );
				hexResult = ( InputType )Result;
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string SetSamplingPeriods( SamplingPeriod hex )
		{
			var Error = string.Empty;
			try
			{
				Error = this.SetParameter( ( byte )OptexCommand.SamplingTime, ( byte )hex );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string GetSamplingPeriods( out SamplingPeriod? hexResult )
		{
			var Error = string.Empty;
			hexResult = null;
			try
			{
				byte Result = 0;
				if ( ( Error = this.GetParameter( ( byte )OptexCommand.SamplingTime, out Result ) ) != String.Empty )
					throw new Exception( Error );
				hexResult = ( SamplingPeriod )Result;
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string SetAvgNo( NoOfAveraging hex )
		{
			var Error = string.Empty;
			try
			{
				Error = this.SetParameter( ( byte )OptexCommand.SetAvgNo, ( byte )hex );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string UpdateAvgNo()
		{
			//var TEST = this.GetAvgNo ( out var test );
			return this.SetAvgNo( this.Configuration.NoOfAveraging );
		}

		public string UpdateSamplingPeriods()
		{
			//var TEST = this.GetSamplingPeriods ( out var test );
			return this.SetSamplingPeriods( this.Configuration.SamplingPeriod );
		}

		public string UpdateLaserPower()
		{
			//var TEST = this.GetLaserPower ( out var laserpower );
			return this.SetLaserPower( this.Configuration.LaserPower );
		}

		public string UpdateSensitivity()
		{
			//var TEST = this.GetSensitivity ( out var test );
			return this.SetSensitivity( this.Configuration.Sensitivity );
		}

		public string GetAvgNo( out NoOfAveraging? hexResult )
		{
			var Error = string.Empty;
			hexResult = null;
			try
			{
				byte Result = 0;
				if ( ( Error = this.GetParameter( ( byte )OptexCommand.SetAvgNo, out Result ) ) != String.Empty )
					throw new Exception( Error );
				hexResult = ( NoOfAveraging )Result;
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string SetLaserPower( LaserPower hex )
		{
			var Error = string.Empty;
			try
			{
				Error = this.SetParameter( ( byte )OptexCommand.SetLaserPower, ( byte )hex );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string OffLaserSpot()
		{
			return this.SetLaserPower( LaserPower.OFF );
		}
		public string OnLaserSpot()
		{
			return this.SetLaserPower( this.Configuration.LaserPower );
		}
		public string GetLaserPower( out LaserPower? hexResult )
		{
			var Error = string.Empty;
			hexResult = null;
			try
			{
				byte Result = 0;
				if ( ( Error = this.GetParameter( ( byte )OptexCommand.SetLaserPower, out Result ) ) != String.Empty )
					throw new Exception( Error );
				hexResult = ( LaserPower )Result;
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}

		public string SetSensitivity( Sensitivity hex )
		{
			var Error = string.Empty;
			try
			{
				Error = this.SetParameter( ( byte )OptexCommand.SetSensitivity, ( byte )hex );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string GetSensitivity( out Sensitivity? hexResult )
		{
			var Error = string.Empty;
			hexResult = null;
			try
			{
				byte Result = 0;
				if ( ( Error = this.GetParameter( ( byte )OptexCommand.SetSensitivity, out Result ) ) != String.Empty )
					throw new Exception( Error );
				hexResult = ( Sensitivity )Result;
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string SetBaudRate( BaudRate hex )
		{
			var Error = string.Empty;
			try
			{
				Error = this.SetParameter( ( byte )OptexCommand.SetBaudRate, ( byte )hex );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string GetBaudRate( out BaudRate? hexResult )
		{
			var Error = string.Empty;
			hexResult = null;
			try
			{
				byte Result = 0;
				if ( ( Error = this.GetParameter( ( byte )OptexCommand.SetBaudRate, out Result ) ) != String.Empty )
					throw new Exception( Error );
				hexResult = ( BaudRate )Result;
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string SetCalShif( CalShift hex )
		{
			var Error = "";
			try
			{
				var result = this.SetParameter( ( byte )hex, ( byte )0x00 );
				if ( result != string.Empty )
					throw new Exception( result );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string SetCalSpan( CalSpan hex )
		{
			var Error = "";
			try
			{
				byte b_data = 0x00;
				if ( hex == CalSpan.CalSpanMiddleByte )
					b_data = 0x80;
				Error = this.SetParameter( ( byte )hex, b_data );
				if ( Error != string.Empty )
					throw new Exception( Error );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			return Error;
		}
		public string SetParameter( byte Command, byte Data )
		{
			var Error = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
				var command = new byte[] { Command, Data };
				var b_Message = this.MessageFormatting( command, true );
				this._session.Write( b_Message, 0, b_Message.Length );
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return Error;
		}
		public string GetParameter( byte Command, out byte Data )
		{
			int iError = 0;
			Data = 0;
			var Error = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
				var command = new byte[] { Command, ( byte )ReadCmd.ReadOnce };
				var b_Message = this.MessageFormatting( command, true );
				this._session.Write( b_Message, 0, b_Message.Length );
				//Thread.Sleep ( 100 );
				byte[] b_Result = new byte[ 6 ];
				iError = this._session.Read( b_Result, 0, b_Result.Length );
				if ( iError < 0 )
					throw new Exception( $"^{( int )RunErrors.ERR_GetCMDError }^" );

				if ( !this.CheckReceivingData( b_Result ) )
					throw new Exception( $"^{( int )RunErrors.ERR_CheckSumErr }^" );

				Data = b_Result[ 1 ];
			}
			catch ( Exception ex )
			{
				Error = this.FormatErrMsg( this.Name, ex );
			}

			finally
			{
				Monitor.Exit( this.SyncRoot );

			}
			return Error;
		}
		private byte[] GetByteArray( string hex, char Splitter )
		{
			string[] ssArray = hex.Split( Splitter );
			List<byte> bytList = new List<byte>();
			foreach ( var s in ssArray )
			{
				//½«Ê®Áù½øÖÆµÄ×Ö·û´®×ª»»³ÉÊýÖµ
				bytList.Add( Convert.ToByte( s, 16 ) );
			}
			//·µ»Ø×Ö½ÚÊý×é
			return bytList.ToArray();
		}
		private byte[] MessageFormatting( byte[] Message, bool bAddCheckSumValue )
		{
			//byte[] packet = Encoding.ASCII.GetBytes( "MEASURE" );//new byte[] { (byte)OptexCommand.Read, (byte) ReadCmd.ReadOnce , Etx};
			var Result = Message.ToList();
			Result.Insert( 0, Stx );
			if ( bAddCheckSumValue && Result.Count() > 2 && Message.Length >= 2 )
			{
				Result.Insert( Result.Count(), Etx );
				byte CheckSumValue = ( byte )( Result[ 1 ] ^ Result[ 2 ] ^ Result[ 3 ] );
				Result.Insert( Result.Count(), CheckSumValue );
			}
			else
				Result.Insert( Result.Count(), Etx );
			return Result.ToArray();
		}
		bool CheckReceivingData( byte[] b_Data )
		{
			return ( b_Data[ 1 ] ^ b_Data[ 2 ] ^ b_Data[ 3 ] ^ b_Data[ 4 ] ) == b_Data[ 5 ] ? true : false;
		}


		public string GetHeight( out int HeightRaw )
		{
			Monitor.Enter( this.SyncRoot );
			int iError = 0;
			var sErr = string.Empty;
			HeightRaw = -99;
			try
			{
				byte[] b_Result = new byte[ 6 ];
				iError = this._session.Read( b_Result, 0, b_Result.Length );
				if ( iError < 0 )
					throw new Exception( $"^{( int )RunErrors.ERR_GetDataErr }^" );
				if ( !this.CheckReceivingData( b_Result ) )
					throw new Exception( $"^{( int )RunErrors.ERR_CheckCRCErr }^" );
				HeightRaw = ( b_Result[ 1 ] << 16 ) + ( b_Result[ 2 ] << 8 ) + ( b_Result[ 3 ] );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return sErr;
		}


		public void ZeroOffset()
		{
			try
			{
				this.Configuration.Offset -= this.Status.CurrentHeight * 1000;
				this.Status.GetHeight();
			}
			catch
			{
			}
			finally
			{
			}
		}


		public double GetHeightNew()
		{
			Monitor.Enter( this.SyncRoot );
			float DistanceMm = -99;
			try
			{
				this.Open();

				byte[] OffCMD = new byte[] { 0x02, 0x4f, 0x46, 0x46, 0x03 };
				byte[] ONNCMD = new byte[] { 0x02, 0x4f, 0x4e, 0x03 };
				if ( !this.FocalLenghSetData( OffCMD ) )
				{
					Thread.Sleep( 200 );
					this.FocalLenghSetData( OffCMD );
				}
				Thread.Sleep( 10 );
				bool bRet = this.GetFocalLengh( ref DistanceMm );
				if ( !this.FocalLenghSetData( ONNCMD ) )
				{
					Thread.Sleep( 200 );
					this.FocalLenghSetData( ONNCMD );
				}
				return DistanceMm;
			}
			catch
			{
				return -99;
			}
			finally
			{
				this._session.Close();
				Monitor.Exit( this.SyncRoot );
			}

		}

		public bool FocalLenghSetData( byte[] txData )
		{
			byte[] ReadData = new byte[ 1024 ];
			try
			{
				if ( this._session.QueryDataBytes( txData, ref ReadData ) == 0 )
					return false;
			}
			catch
			{
				return false;
			}
			return true;
		}


		public bool GetFocalLengh( ref float DistanceMm )
		{
			byte[] bValue = new byte[ 4 ];
			byte[] MEASURECMD = new byte[] { 0x02, 0x4D, 0x45, 0x41, 0x53, 0x55, 0x52, 0x45, 0x03 };
			byte[] ReadData = new byte[ 1024 ];
			try
			{
				var ReadLength = this._session.QueryDataBytes( MEASURECMD, ref ReadData );
				if ( ReadLength == 0 )
					return false;
				string sReadData = System.Text.Encoding.Default.GetString( ReadData );
				string sDistance = sReadData.Substring( 1, ReadLength - 2 );
				DistanceMm = float.Parse( sDistance );
			}
			catch
			{
				return false;
			}
			return true;
		}

		public Task<ErrorResult> SetOptexValues()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( this.UpdateAvgNo() != string.Empty )
						throw new Exception( $"^{( int )RunErrors.ERR_UpdateAvgNumErr }^" );
					Thread.Sleep( 100 );
					if ( this.UpdateLaserPower() != string.Empty )
						throw new Exception( $"^{( int )RunErrors.ERR_UpdateLaserPwrErr }^" );
					Thread.Sleep( 100 );
					if ( this.UpdateSamplingPeriods() != string.Empty )
						throw new Exception( $"^{( int )RunErrors.ERR_UpdateSamplingPeriodErr }^" );
					Thread.Sleep( 100 );
					if ( this.UpdateSensitivity() != string.Empty )
						throw new Exception( $"^{( int )RunErrors.ERR_UpdateSenErr }^" );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
	}

	public class OptexStatus : BaseUtility
	{
		private double _CurrentHeight = -99;
		public double CurrentHeight
		{
			get => this._CurrentHeight;
			set => this.Set( ref this._CurrentHeight, value, "CurrentHeight" );
		}
		private double _CurrentRawHeight = -99;
		public double CurrentRawHeight
		{
			get => this._CurrentRawHeight;
			set => this.Set( ref this._CurrentRawHeight, value, "CurrentRawHeight" );
		}
		private OptexHeightSensor OptexHeightSensor { get; set; }
		public OptexStatus( OptexHeightSensor highsensor )
		{
			this.OptexHeightSensor = highsensor;
		}
		private Stopwatch swRetries = new Stopwatch(); //#JW for HeightSensor Tracking

		public string GetHeight( bool bypassretry = false )
		{
			var sErr = string.Empty;
			try
			{
				if ( !this.OptexHeightSensor.IsValid ) return string.Empty;
				sErr = this.GetDisplacement( bypassretry );
				if ( sErr != string.Empty ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.OptexHeightSensor.Name, ex );
			}
			return sErr;
		}

		//public bool SetContinuous
		//{
		//	set
		//	{
		//		if ( value )
		//			this.OptexHeightSensor.SetReadingOut( ReadingOut.StartContinuousRead );
		//		else
		//			this.OptexHeightSensor.SetReadingOut( ReadingOut.StopContinuousRead );
		//	}
		//}

		private string GetDisplacement( bool bypassretry )
		{
			var sErr = string.Empty;
			int retry = 0;
			int RetryStability = 0;
			int PreviousHeight = -99;
			int Height = -99;
			try
			{
				this.swRetries.Restart();
				Retry:
				string sError = string.Empty;
				sError = this.OptexHeightSensor.SetReadingOut( ReadingOut.ReadOnce );
				if ( sError != string.Empty )
					throw new Exception( sError );
				PreviousHeight = Height;
				sErr = this.OptexHeightSensor.GetHeight( out Height );
				if ( this.OptexHeightSensor.Configuration.EnHeightLogging )
					this.OptexHeightSensor._Logger.WriteLog( Height.ToString() );
				if ( bypassretry )
				{
					if ( sErr != string.Empty )
						throw new Exception( sErr );
				}
				else
				{
					retry++;
					if ( retry > 15 )
						throw new Exception( $"^{( int )RunErrors.ERR_RetriesExceeded }^" );
					if ( sErr != string.Empty || Math.Abs( Height - PreviousHeight ) > 350 )
					{
						if ( this.OptexHeightSensor.Configuration.EnHeightLogging )
						{
							this.OptexHeightSensor._Logger.WriteLog( $"Error: {sErr}" );
							this.OptexHeightSensor._Logger.WriteLog( $"Height(Raw):{Height}, PrevHeight(Raw):{PreviousHeight}," );
							this.OptexHeightSensor._Logger.WriteLog( $"Height:{( ( this.OptexHeightSensor.Configuration.Factor * Height ) + this.OptexHeightSensor.Configuration.Offset ) * 0.001}, " +
								$"PrevHeight:{( ( this.OptexHeightSensor.Configuration.Factor * PreviousHeight ) + this.OptexHeightSensor.Configuration.Offset ) * 0.001}," );
						}
						RetryStability = 0;
						Thread.Sleep( 10 );
						goto Retry;
					}
					else
					{
						if ( Math.Abs( Height - PreviousHeight ) <= 350 )
						{
							if ( RetryStability++ < 3 )
							{
								Thread.Sleep( 10 );
								goto Retry;
							}
						}
						else
							RetryStability = 0;
					}
				}
				if ( Height == 0x1FFFFF )
					this.CurrentHeight = -999;
				else
					this.CurrentHeight = ( ( this.OptexHeightSensor.Configuration.Factor * Height ) + this.OptexHeightSensor.Configuration.Offset ) * 0.001;
				this.CurrentRawHeight = Height;

				if ( this.OptexHeightSensor.Configuration.EnHeightLogging )
					this.OptexHeightSensor._Logger.WriteLog( $"Total Scan Height Retries: {retry}, time: {this.swRetries.ElapsedMilliseconds}ms" );
			}
			catch ( Exception ex )
			{
				this.CurrentHeight = -99;
				this.CurrentRawHeight = -99;
				sErr = this.FormatErrMsg( this.OptexHeightSensor.Name, ex );
			}
			return sErr;
		}

		#region IsEnd Property
		int _isContinuous = 0;
		public bool isContinuous
		{
			get => Interlocked.CompareExchange( ref this._isContinuous, 1, 1 ) == 1;
			set
			{
				Interlocked.Exchange( ref this._isContinuous, value ? 1 : 0 );
				this.OnPropertyChanged( "isContinuous" );
			}
		}
		#endregion
		#region Monitoring Start/Stop
		Task _monitoring;
		public void Start()
		{
			this.isContinuous = true;
			this._monitoring = Task.Run( () => this.OnMonitoring() );
		}
		public void Stop()
		{
			this.isContinuous = false;
			this._monitoring?.Wait();
			this._monitoring?.Dispose();
			this._monitoring = null;
		}
		#endregion
		protected void OnMonitoring()
		{
			while ( this.isContinuous )
			{
				this.GetHeight( true );
				Thread.Sleep( 100 );
			}
		}
	}
	public enum HeightRead
	{
		Pass,
		NeedCompensation,
		OutOfHeightSensorRange,
		TiltError,
		Fail,
	}
}
