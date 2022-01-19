using HiPA.Common;
using HiPA.Communicator;
using System;
using System.Threading;

namespace HiPA.Instrument.HeightSensor
{
	[Serializable]
	public class KeyenceHighSensorConfiguration
		: Configuration
	{
		[NonSerialized]
		public static readonly KeyenceHighSensorConfiguration _DEFAULT = new KeyenceHighSensorConfiguration();
		[NonSerialized]
		public static readonly string NAME = "DL-RS1A";

		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.HeightSensor;
		public override Type InstrumentType => typeof( KeyenceHighSensor );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public SerialPortParameter ConnectParameter { get; set; }
	}

	public class KeyenceHighSensor
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		public KeyenceHighSensorConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.HeightSensor;

		SerialCommunicator _session = null;

		protected override string OnCreate()
		{
			return String.Empty;
		}

		protected override string OnInitialize()
		{
			var result = String.Empty;
			if ( this.Configuration.ConnectParameter == null )
				this.Configuration.ConnectParameter = new SerialPortParameter();
			if ( this.IsOpen == false )
			{
				if ( ( result = this.Open() ) != String.Empty ) return result;
			}
			//this.GetDisplacement( out double displacement, out string errstring );
			return String.Empty;
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
			var result = String.Empty;
			try
			{
				if ( MachineStateMng.isSimulation ) return string.Empty;
				if ( this._session == null ) this._session = new SerialCommunicator();
				this._session.Connect( this.Configuration.ConnectParameter );
				this._session.Separator = "\r\n";
			}
			catch ( CommunicatorConnectionException cex )
			{
				var portName = this.Configuration.ConnectParameter?.PortName;
				var baudRate = this.Configuration.ConnectParameter?.BaudRate;
				result = $"{this.Name}:,Open:,{cex.Message}";
			}
			catch ( Exception ex )
			{
				result = $"{this.Name}:,Open:,{ex.Message}";
			}
			return result;
		}
		public bool IsOpen => this._session?.IsConnected == true;
		public int Close()
		{
			try
			{
				this._session?.Dispose();
				this._session = null;
				return 0;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
				this._session = null;
				return 0;
			}
		}

		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as KeyenceHighSensorConfiguration;
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
		public string GetDisplacement( out double value, out string error )
		{
			value = 0;
			error = "";

			try
			{
				Monitor.Enter( this.SyncRoot );

				var response = this._session.Query( $"M0\r\n", false ).Trim();
				var parts = this.ParseResponse( response );
				if ( parts.Error != 0 )
				{
					error = parts.Desc;
					return Equipment.ErrManager.RaiseError( this, $"HighSensor[{this.Name}] Get High Value Failure: {parts.Desc}", ErrorTitle.OperationFailure, ErrorClass.E4 );
				}
				value = parts.Value;
				return string.Empty;
			}
			catch ( Exception ex )
			{
				return Equipment.ErrManager.RaiseError( this, $"KeyenceHighSensor.cs :GetDisplacement:HighSensor[{this.Name}] Get High Value Exception: [{ex.Message}]", ErrorTitle.OperationFailure, ErrorClass.E4, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}

		public string SetZero( out string error )
		{
			error = "";
			try
			{
				Monitor.Enter( this.SyncRoot );

				var response = "";
				var first = "SW,00,001,0\r\n";
				var second = "SW,00,001,1\r\n";

				response = this._session.Query( first, false );
				var firstResult = this.ParseResponse( response );
				if ( firstResult.Error != 0 )
				{
					error = firstResult.Desc;
					return Equipment.ErrManager.RaiseError( this, $"HighSensor[{this.Name}] Set Zero Failure: {firstResult.Desc}", ErrorTitle.OperationFailure, ErrorClass.E4 );
				}

				response = this._session.Query( second, false );
				var secondResult = this.ParseResponse( response );
				if ( secondResult.Error != 0 )
				{
					error = secondResult.Desc;
					return Equipment.ErrManager.RaiseError( this, $"HighSensor[{this.Name}] Set Zero Failure: {secondResult.Desc}", ErrorTitle.OperationFailure, ErrorClass.E4 );
				}
				return string.Empty;
			}
			catch ( Exception ex )
			{
				return Equipment.ErrManager.RaiseError( this, $"KeyenceHighSensor.cs :SetZero:HighSensor[{this.Name}] Set Zero Exception: [{ex.Message}]", ErrorTitle.OperationFailure, ErrorClass.E4, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}
	}
}
