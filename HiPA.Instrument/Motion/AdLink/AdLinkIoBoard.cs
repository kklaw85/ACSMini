using HiPA.Common;
using HiPA.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HiPA.Instrument.Motion.APS
{
	[Serializable]
	public class IoBoardConfigurationBase
		: Configuration
	{
		public override string Name { get; set; }

		public override InstrumentCategory Category => InstrumentCategory.DIO;

		public override Type InstrumentType => typeof( IoBoardBase );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();

		private int i_Polling = 10;
		public int PollingRate
		{
			get => this.i_Polling;
			set
			{
				this.i_Polling = value;
				if ( this.i_Polling < 10 )
					this.i_Polling = 10;
				this.OnPropertyChanged( "PollingRate" );
			}
		}

		private AdlinkModel e_CardType = AdlinkModel.AMP204208C;
		public AdlinkModel CardType
		{
			get => this.e_CardType;
			set => this.Set( ref this.e_CardType, value, "CardType" );
		}

		private ushort i_CardID = 0;
		public ushort CardID
		{
			get => this.i_CardID;
			set => this.Set( ref this.i_CardID, value, "CardID" );
		}
	}

	[Serializable]
	public class APSIoBoardConfiguration
		: IoBoardConfigurationBase
	{
		[NonSerialized]
		private static readonly APSIoBoardConfiguration _DEFAULT = new APSIoBoardConfiguration();

		[NonSerialized]
		public static readonly string NAME = "AdLink-HSL-DIO";

		public override string Name { get; set; }

		public override InstrumentCategory Category => InstrumentCategory.DIO;

		public override Type InstrumentType => typeof( APSIoBoard );
	}

	public class APSIoBoard
		: IoBoardBase
	{
		public override MachineVariant MachineVar { get; set; }
		[XmlIgnore]
		public APSIoBoardConfiguration Configuration { get; protected set; }
		protected bool BoardFound = false;
		public override string Name => this.Configuration.Name;
		[XmlIgnore]
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		[XmlIgnore]
		public override InstrumentCategory Category => InstrumentCategory.Motion;
		public override string Open()
		{
			var result = string.Empty;
			var resultint = 0;
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( !MachineStateMng.isSimulation )
				{
					var board = PCI7856.GetInstance<PCI7856>();
					if ( ( resultint = board.Open() ) != 0 )
						throw new Exception( $"{PCI7856.GetErrorDesc( resultint )}" );
					if ( !board.BoardHandler.Contains( ( int )this.Configuration.CardID ) )
						throw new Exception( $"^{( int )RunErrors.ERR_NoBoardDetectErr }^" );
					this.BoardFound = true;
					Thread.Sleep( 300 );
					if ( this.Configuration.CardType == AdlinkModel.PCI7856 )
						if ( ( resultint = APS168.APS_start_field_bus(
								this.Configuration.CardID,
								( int )BoardPortId.HSL,
								0 ) ) != 0 )
							throw new Exception( $"{PCI7856.GetErrorDesc( resultint )}" );
				}
				this.OnIOListMonitoring();
				return result;
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return result;
		}

		public override string Close()
		{
			var result = string.Empty;
			var resultint = 0;
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( this.Configuration.CardID >= 0 )
				{
					this._exitMonitoring = true;
					Thread.Sleep( 100 );

					if ( ( resultint = APS168.APS_stop_field_bus( this.Configuration.CardID, ( int )BoardPortId.HSL ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
					PCI7856.GetInstance().Close();
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return result;
		}

		public override bool IsOpen() => this.BoardFound;


		protected override string CreateIOs()
		{
			var result = string.Empty;
			var resultint = 0;
			try
			{
				this._inputBuffer = new Dictionary<int, uint>();
				this._outputBuffer = new Dictionary<int, uint>();
				string line;
				Dictionary<string, IoPointAttribute> iolist = new Dictionary<string, IoPointAttribute>();

				var reader = File.OpenText( AppDomain.CurrentDomain.BaseDirectory + "\\Config\\IO_Para.csv" );
				while ( ( line = reader.ReadLine() ) != null )
				{
					if ( !line.Contains( "=" ) ) continue;
					string[] items = line.Split( '=', ',' );

					var IOType = items[ 0 ].StartsWith( "I" ) ? DioType.Input : DioType.Output;
					int CardID = int.Parse( items[ 2 ] );
					int ModuleNo = int.Parse( items[ 3 ] );
					int Line = int.Parse( items[ 4 ] );

					var point = new IoPointAttribute( items[ 0 ], AdlinkModel.AMP204208C, CardID, ModuleNo, Line, IOType );
					iolist[ point.Name ] = point;
				}

				foreach ( var type in iolist.Values )
				{
					if ( this.Configuration.CardID != type.CardID ) continue;
					if ( this.Configuration.CardType != type.Model ) continue;
					var point = Constructor.GetInstance().GetInstrument( type.Name, typeof( AdLinkIoPointConfiguration ) ) as AdLinkIoPoint;
					if ( point == null ) throw new Exception( $"{type.Name} is null" );
					point.Configuration.Line = type.LineNo;
					point.Configuration.Type = type.Type;
					point.Configuration.ModuleNo = type.ModuleNo;
					point.Configuration.CardID = type.CardID;
					point.Board = this;
					this.AddChild( point );

					var moduleNo = point.Configuration.ModuleNo;//pair.Attribute.ModuleNo;
					if ( point.Configuration.Type == DioType.Input )
					{
						if ( this._inputBuffer.ContainsKey( moduleNo ) == false ) this._inputBuffer[ moduleNo ] = 0;
					}
					else
						if ( this._outputBuffer.ContainsKey( moduleNo ) == false ) this._outputBuffer[ moduleNo ] = 0;
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		public AdLinkIoPoint GetIoPoint( Enum io )
		{
			var result = ReflectionTool.GetEnumAttribute<IoPointAttribute>( io );
			if ( result.Address == -1 ) return null;

			return this.GetIoPoint( result.Attribute.ModuleNo, result.Address, result.Attribute.Type );
		}
		public AdLinkIoPoint GetIoPoint( int moduleNo, int address, DioType type )
		{
			foreach ( AdLinkIoPoint point in this.GetChildren() )
			{
				if ( point.Configuration.ModuleNo == moduleNo &&
					point.Configuration.Line == address &&
					point.Configuration.Type == type )
					return point;
			}
			return null;
		}

		protected void UpdateIO()
		{
			this.GetOutputs();
			this.GetInputs();
			this.Dispatch();
		}

		bool _exitMonitoring = true;

		protected Task OnIOListMonitoring()
		{
			if ( this._exitMonitoring == false ) return null;

			return Task.Run( () =>
			{
				this._exitMonitoring = false;
				while ( this._exitMonitoring == false )
				{
					this.UpdateIO();
					Thread.Sleep( this.Configuration.PollingRate );
				}
			} );
		}

		protected void Dispatch()
		{
			var inputLocked = false;
			var outputLocked = false;
			try
			{
				Monitor.Enter( this._inputLock, ref inputLocked );
				Monitor.Enter( this._outputLock, ref outputLocked );

				foreach ( AdLinkIoPoint child in this.GetChildren() )
				{
					if ( child.Configuration.Type == DioType.Input )
						child._Update( this._inputBuffer[ child.Configuration.ModuleNo ] );
					else
						child._Update( this._outputBuffer[ child.Configuration.ModuleNo ] );
				}
			}
			finally
			{
				if ( inputLocked ) Monitor.Exit( this._inputLock );
				if ( outputLocked ) Monitor.Exit( this._outputLock );
			}
		}
		protected void DispatchOut()
		{
			var outputLocked = false;
			try
			{
				Monitor.Enter( this._outputLock, ref outputLocked );

				foreach ( AdLinkIoPoint child in this.GetChildren() )
				{
					if ( child.Configuration.Type == DioType.Output )
						child._Update( this._outputBuffer[ child.Configuration.ModuleNo ] );
				}
			}
			finally
			{
				if ( outputLocked ) Monitor.Exit( this._outputLock );
			}
		}
		protected void DispatchIn()
		{
			var inputLocked = false;
			try
			{
				Monitor.Enter( this._inputLock, ref inputLocked );

				foreach ( AdLinkIoPoint child in this.GetChildren() )
				{
					if ( child.Configuration.Type == DioType.Input )
						child._Update( this._inputBuffer[ child.Configuration.ModuleNo ] );
				}
			}
			finally
			{
				if ( inputLocked ) Monitor.Exit( this._inputLock );
			}
		}

		#region IN/OUT Buffers
		protected object _inputLock = new object();
		protected Dictionary<int, uint> _inputBuffer = null;
		protected object _outputLock = new object();
		protected Dictionary<int, uint> _outputBuffer = null;
		public override uint GetInputs( int moduleNo = -1 )
		{
			uint result = 0;
			var resultint = 0;
			try
			{
				while ( this._AccessingInputBuffer )
				{
					Thread.Sleep( 10 );
				}
				var boardHandle = this.Configuration.CardID;
				Monitor.Enter( this._inputLock );
				if ( !this.IsOpen() ) return 0;
				foreach ( var key in this._inputBuffer.Keys.ToArray() )
				{
					var value = 0;
					uint value2 = 0;
					if ( MachineStateMng.isSimulation ) continue;
					if ( this.Configuration.CardType == AdlinkModel.PCI7856 )
						resultint = APS168.APS_field_bus_d_get_input( boardHandle, ( int )BoardPortId.HSL, key, ref value );
					else if ( this.Configuration.CardType == AdlinkModel.AMP204208C )
						resultint = APS168.APS_read_d_input( boardHandle, 0, ref value );
					if ( resultint != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
					if ( this.Configuration.CardType == AdlinkModel.PCI7856 )
					{
						value2 = ( uint )value;
						this._inputBuffer[ key ] = value2;
					}
					else if ( this.Configuration.CardType == AdlinkModel.AMP204208C )
					{
						value2 = ( ( uint )value ) >> 8;
						this._inputBuffer[ key ] = value2;
					}
					if ( moduleNo == key ) result = value2;
				}
				return result;
			}
			catch
			{
				//new Thread(() => MessageBox.Show(this.FormatErrMsg(this.Name, ex))).Start();
				return 0;
			}
			finally
			{
				Monitor.Exit( this._inputLock );
				this.DispatchIn();
			}
		}
		public override uint GetOutputs( int moduleNo = -1 )
		{
			try
			{
				uint result = 0;
				var resultint = 0;
				while ( this._AccessingOutputBuffer )
				{
					Thread.Sleep( 10 );
				}
				var boardHandle = this.Configuration.CardID;
				Monitor.Enter( this._outputLock );
				if ( !this.IsOpen() ) return 0;
				foreach ( var key in this._outputBuffer.Keys.ToArray() )
				{
					var value = 0;
					if ( MachineStateMng.isSimulation ) continue;
					if ( this.Configuration.CardType == AdlinkModel.PCI7856 )
						resultint = APS168.APS_field_bus_d_get_output( boardHandle, ( int )BoardPortId.HSL, key, ref value );
					else if ( this.Configuration.CardType == AdlinkModel.AMP204208C )
						resultint = APS168.APS_read_d_output( boardHandle, 0, ref value );
					if ( resultint != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );

					if ( this.Configuration.CardType == AdlinkModel.PCI7856 )
						this._outputBuffer[ key ] = ( uint )value;
					else if ( this.Configuration.CardType == AdlinkModel.AMP204208C )
						this._outputBuffer[ key ] = ( ( uint )value ) >> 8;
					if ( moduleNo == key ) result = ( uint )value;
				}
				return result;
			}
			catch
			{
				//new Thread( () => MessageBox.Show( this.FormatErrMsg( this.Name, ex ) ) ).Start();
				return 0;
			}
			finally
			{
				Monitor.Exit( this._outputLock );
				this.DispatchOut();
			}
		}
		public override uint SetOutput( int moduleNo, int addr, DioValue value )
		{
			var resultint = 0;
			try
			{
				var boardHandle = this.Configuration.CardID;
				Monitor.Enter( this._outputLock );

				if ( this._outputBuffer.ContainsKey( moduleNo ) == false ) return 0;
				if ( MachineStateMng.isSimulation ) return 0;
				uint buff = 0;
				if ( !this.IsOpen() ) return buff;
				buff = this.GetOutputs( moduleNo );
				if ( this.Configuration.CardType == AdlinkModel.PCI7856 )
				{
					if ( value == DioValue.On ) buff |= ( uint )( 1 << addr );
					else buff &= ~( ( uint )( 1 << addr ) );
					resultint = APS168.APS_field_bus_d_set_output( boardHandle, ( int )BoardPortId.HSL, moduleNo, ( int )buff );
				}
				else if ( this.Configuration.CardType == AdlinkModel.AMP204208C )
				{
					resultint = APS168.APS_write_d_channel_output( boardHandle, 0, addr + 8, ( value == DioValue.On ? 1 : 0 ) );
				}
				if ( resultint != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				return buff;
			}
			catch ( Exception ex )
			{
				if ( !MachineStateMng.isSimulation )
					Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E5 );
				//new Thread( () => MessageBox.Show( this.FormatErrMsg( this.Name, ex ) ) ).Start();
				return 0;
			}
			finally
			{
				Monitor.Exit( this._outputLock );
			}
		}
		private bool _AccessingInputBuffer = false;
		private bool _AccessingOutputBuffer = false;
		#endregion

		#region General
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as APSIoBoardConfiguration;
		}

		protected override string OnCreate()
		{
			var result = string.Empty;

			try
			{
				PCI7856.GetErrorDesc( 0 );

				if ( ( result = this.CreateIOs() ) != string.Empty ) throw new Exception( result );

				var tasks = this.GetChildren().Select( a => a.Create() ).ToArray();
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( result = t.Result ) != string.Empty ) throw new Exception( result );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnInitialize()
		{
			var result = string.Empty;
			try
			{
				if ( ( result = this.Open() ) != string.Empty )
				{
					this.Close();
					throw new Exception( result );
				}
				var tasks = this.GetChildren().Select( a => a.Initialize() ).ToArray();
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( result = t.Result ) != string.Empty ) throw new Exception( result );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnStop()
		{
			var result = string.Empty;

			try
			{
				var tasks = this.GetChildren().Select( a => a.Stop() ).ToArray();
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( result = t.Result ) != string.Empty ) throw new Exception( result );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnTerminate()
		{
			var result = string.Empty;
			try
			{
				var tasks = this.GetChildren().Select( a => a.Terminate() ).ToArray();
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( result = t.Result ) != string.Empty ) throw new Exception( result );
				}
				this.Close();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		#endregion
	}

	#region IoPointAttribute
	public class IoPointAttribute : Attribute
	{
		public string Name { get; }
		public int CardID { get; }
		public int ModuleNo { get; }
		public int LineNo { get; }
		public DioType Type { get; }
		public AdlinkModel Model { get; }
		public NormalState NormalState { get; }
		public IoPointAttribute( string name, AdlinkModel model, int cardID, int moduleNo, int LineNo, DioType type, NormalState normalState )
		{
			this.CardID = cardID;
			this.ModuleNo = moduleNo;
			this.Type = type;
			this.Model = model;
			this.LineNo = LineNo;
			//this.Name = $"{model}_{type}_CID{ cardID }_MNO{moduleNo}_LineNo{LineNo}";
			this.Name = name;
			this.NormalState = normalState;
		}
		public IoPointAttribute( string name, AdlinkModel model, int cardID, int moduleNo, int LineNo, DioType type )
		{
			this.CardID = cardID;
			this.ModuleNo = moduleNo;
			this.Type = type;
			this.Model = model;
			this.LineNo = LineNo;
			//this.Name = $"{model}_{type}_CID{ cardID }_MNO{moduleNo}_LineNo{LineNo}";
			this.Name = name;
			this.NormalState = NormalState.Low;
		}
	}
	#endregion

	#region Input List
	[Serializable]
	public enum InputIO
	{
		I_ManualSwitch,
		I_AutoSwitch,
		I_DI2,
		I_DI3,
		I_DI4,
		I_DI5,
		I_DI6,
		I_DI7,
		I_NewLiftLLimit,
		I_NewLiftULimit,
		I_QICLiftLLimit,
		I_QICLiftULimit,
		I_LoadArmUp,
		I_LoadArmDown,
		I_LoadArmVacDet,
		I_UnloadArmUp,
		I_UnloadArmDown,
		I_UnloadArmVacDet,
		I_ClamperOpen,
		I_ClamperClose,
		I_MachinePressure,
		I_StageMatDet,
		I_EStop,
		I_StageVacDet,
	}
	#endregion

	#region Output List
	[Serializable]
	public enum OutputIO
	{
		O_DO0,
		O_DO1,
		O_DO2,
		O_DO3,
		O_DO4,
		O_DO5,
		O_DO6,
		O_DO7,
		O_Green,
		O_Yellow,
		O_Red,
		O_Buzzer,
		O_NewLiftBlower,
		O_LoadArmUp,
		O_LoadArmDown,
		O_LoadArmVac,
		O_UnLoadArmUp,
		O_UnLoadArmDown,
		O_UnLoadArmVac,
		O_Clamp,
		O_StageVac,
		O_Lighting,
	}
	#endregion
}
