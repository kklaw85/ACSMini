using HiPA.Common;
using HiPA.Common.Utils;
using HiPA.Instrument.Motion.APS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HiPA.Instrument.Motion.Dask
{
	[Serializable]
	public class DaskIoBoardConfiguration
		: IoBoardConfigurationBase
	{
		[NonSerialized]
		private static readonly DaskIoBoardConfiguration _DEFAULT = new DaskIoBoardConfiguration();

		[NonSerialized]
		public static readonly string NAME = "DASK";

		public override string Name { get; set; }

		public override InstrumentCategory Category => InstrumentCategory.DIO;

		public override Type InstrumentType => typeof( DaskIoBoard );
	}

	public class DaskIoBoard
		: IoBoardBase
	{
		public override MachineVariant MachineVar { get; set; }
		public DaskIoBoardConfiguration Configuration { get; protected set; }
		short Dev = -1;
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Motion;
		protected bool BoardFound = false;
		public override string Open()
		{
			var result = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( !MachineStateMng.isSimulation )
				{
					this.Dev = DASK.Register_Card( ( ushort )this.Configuration.CardType, this.Configuration.CardID );
					if ( this.Dev < 0 )
						throw new Exception( Dask.GetErrorDesc( this.Dev ) );
					this.BoardFound = true;
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
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( this.IsOpen() )
				{
					this._exitMonitoring = true;
					Thread.Sleep( 100 );
					DASK.Release_Card( ( ushort )this.Dev );
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
			try
			{
				this._inputBuffer = new Dictionary<int, uint>();
				this._outputBuffer = new Dictionary<int, uint>();

				foreach ( var type in new Type[] { typeof( InputIO ), typeof( OutputIO ) } )
				{
					var list = ReflectionTool.GetEnumAttributes<IoPointAttribute>( type );
					foreach ( var pair in list )
					{
						if ( this.Configuration.CardID != pair.Attribute.CardID ) continue;
						if ( this.Configuration.CardType != pair.Attribute.Model ) continue;
						var point = Constructor.GetInstance().GetInstrument( pair.Attribute.Name, typeof( AdLinkIoPointConfiguration ) ) as AdLinkIoPoint;
						if ( point == null ) throw new Exception( $"{pair.Attribute.Name} is null" );

						point.Configuration.Line = pair.Attribute.LineNo;
						point.Configuration.Type = pair.Attribute.Type;
						point.Configuration.ModuleNo = pair.Attribute.ModuleNo;
						point.Configuration.CardID = pair.Attribute.CardID;
						point.Board = this;
						this.AddChild( point );

						var CardID = point.Configuration.CardID;//pair.Attribute.ModuleNo;
						if ( point.Configuration.Type == DioType.Input )
						{
							if ( this._inputBuffer.ContainsKey( CardID ) == false ) this._inputBuffer[ CardID ] = 0;
						}
						else
							if ( this._outputBuffer.ContainsKey( CardID ) == false ) this._outputBuffer[ CardID ] = 0;
					}
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

			return this.GetIoPoint( result.Attribute.CardID, result.Address, result.Attribute.Type );
		}
		public AdLinkIoPoint GetIoPoint( int CardID, int Line, DioType type )
		{
			foreach ( AdLinkIoPoint point in this.GetChildren() )
			{
				if ( point.Configuration.CardID == CardID &&
					point.Configuration.Line == Line &&
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
						child._Update( this._inputBuffer[ child.Configuration.CardID ] );
					else
						child._Update( this._outputBuffer[ child.Configuration.CardID ] );
				}
			}
			finally
			{
				if ( inputLocked ) Monitor.Exit( this._inputLock );
				if ( outputLocked ) Monitor.Exit( this._outputLock );
			}
		}

		#region IN/OUT Buffers
		protected object _inputLock = new object();
		protected Dictionary<int, uint> _inputBuffer = null;

		protected object _outputLock = new object();
		protected Dictionary<int, uint> _outputBuffer = null;

		protected bool _UpdatingInputBuffer = true;
		public override uint GetInputs( int moduleNo = -1 )
		{
			uint result = 0;
			short resultint = 0;
			try
			{
				while ( this._AccessingInputBuffer )
				{
					Thread.Sleep( 10 );
				}
				this._UpdatingInputBuffer = true;
				Monitor.Enter( this._inputLock );
				if ( !this.IsOpen() ) return 0;
				foreach ( var key in this._inputBuffer.Keys.ToArray() )
				{
					if ( MachineStateMng.isSimulation ) continue;
					resultint = DASK.DI_ReadPort( ( ushort )this.Dev, 0, out uint value );
					if ( resultint < 0 ) throw new Exception( Dask.GetErrorDesc( resultint ) );

					this._inputBuffer[ key ] = value;
					if ( moduleNo == key ) result = value;
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
				this._UpdatingInputBuffer = false;
				Monitor.Exit( this._inputLock );
			}
		}

		protected bool _UpdatingOutputBuffer = true;
		public override uint GetOutputs( int moduleNo = -1 )
		{
			try
			{
				uint result = 0;
				short resultint = 0;
				while ( this._AccessingOutputBuffer )
				{
					Thread.Sleep( 10 );
				}
				this._UpdatingOutputBuffer = true;
				Monitor.Enter( this._outputLock );
				if ( !this.IsOpen() ) return 0;
				foreach ( var key in this._outputBuffer.Keys.ToArray() )
				{
					if ( MachineStateMng.isSimulation ) continue;
					resultint = DASK.DO_ReadPort( ( ushort )this.Dev, 0, out uint value );
					if ( resultint != 0 ) throw new Exception( Dask.GetErrorDesc( resultint ) );
					this._outputBuffer[ key ] = value;
					if ( moduleNo == key ) result = value;
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
				this._UpdatingOutputBuffer = false;
				Monitor.Exit( this._outputLock );
			}
		}
		public override uint SetOutput( int moduleNo, int Line, DioValue value )
		{
			short resultint = 0;
			try
			{
				Monitor.Enter( this._outputLock );
				resultint = DASK.DO_WriteLine( ( ushort )this.Dev, 0, ( ushort )Line, ( ushort )( value == DioValue.On ? 1 : 0 ) );
				if ( resultint != 0 ) throw new Exception( Dask.GetErrorDesc( resultint ) );
				resultint = DASK.DO_ReadPort( ( ushort )this.Dev, 0, out uint val );
				if ( resultint != 0 ) throw new Exception( Dask.GetErrorDesc( resultint ) );
				return val;
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
		public virtual uint SetOutputs( int moduleNo )
		{
			try
			{
				uint result = 0;
				short resultint = 0;
				Monitor.Enter( this._outputLock );
				foreach ( var pair in this._outputBuffer )
				{
					resultint = DASK.DO_WritePort( ( ushort )this.Dev, 0, ( uint )pair.Value );
					if ( resultint != 0 ) throw new Exception( Dask.GetErrorDesc( resultint ) );
					if ( moduleNo == pair.Key ) result = pair.Value;
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
			}
		}

		const int IO_POINT_COUNT = 16;
		public bool _AccessingInputBuffer = false;
		public Dictionary<int, DioValue[]> _GetInputs()
		{
			try
			{
				this._AccessingInputBuffer = true;
				while ( this._UpdatingInputBuffer )
				{
					Thread.Sleep( 10 );
				}
				Monitor.Enter( this._inputBuffer );
				var result = new Dictionary<int, DioValue[]>();
				DateTime Now = DateTime.Now;
				foreach ( var key in this._inputBuffer.Keys.ToArray() )
				{
					var values = new DioValue[ IO_POINT_COUNT ];
					var bits = this._inputBuffer[ key ];
					for ( int i = 0; i < IO_POINT_COUNT; i++ )
						values[ i ] = ( bits & ( 1 << i ) ) == ( 1 << i ) ? DioValue.On : DioValue.Off;
					result.Add( key, values );
				}
				return result;
			}
			finally
			{
				Monitor.Exit( this._inputBuffer );
				this._AccessingInputBuffer = false;
			}
		}

		public bool _AccessingOutputBuffer = false;
		public Dictionary<int, DioValue[]> _GetOutputs()
		{
			try
			{
				this._AccessingOutputBuffer = true;
				while ( this._UpdatingOutputBuffer )
				{
					Thread.Sleep( 10 );
				}
				Monitor.Enter( this._outputLock );
				var result = new Dictionary<int, DioValue[]>();
				foreach ( var key in this._outputBuffer.Keys.ToArray() )
				{
					var values = new DioValue[ IO_POINT_COUNT ];
					var bits = this._outputBuffer[ key ];
					for ( int i = 0; i < IO_POINT_COUNT; i++ )
						values[ i ] = ( bits & ( 1 << i ) ) == ( 1 << i ) ? DioValue.On : DioValue.Off;
					result.Add( key, values );
				}
				return result;
			}
			finally
			{
				Monitor.Exit( this._outputLock );
				this._AccessingOutputBuffer = false;
			}
		}

		#endregion

		#region General
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as DaskIoBoardConfiguration;
		}

		protected override string OnCreate()
		{
			var result = string.Empty;

			try
			{
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


	//#region Output List
	//[Serializable]
	//public enum OutputIO7432
	//{
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 0, DioType.Output )]
	//	Out_00 = 00,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 1, DioType.Output )]
	//	Out_01 = 01,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 2, DioType.Output )]
	//	Out_02 = 02,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 3, DioType.Output )]
	//	Out_03 = 03,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 4, DioType.Output )]
	//	Out_04 = 04,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 5, DioType.Output )]
	//	Out_05 = 05,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 6, DioType.Output )]
	//	Out_06 = 06,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 7, DioType.Output )]
	//	Out_07 = 07,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 8, DioType.Output )]
	//	Out_08 = 08,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 9, DioType.Output )]
	//	Out_09 = 09,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 10, DioType.Output )]
	//	Out_10 = 10,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 11, DioType.Output )]
	//	Out_11 = 11,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 12, DioType.Output )]
	//	Out_12 = 12,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 13, DioType.Output )]
	//	Out_13 = 13,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 14, DioType.Output )]
	//	Out_14 = 14,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 15, DioType.Output )]
	//	Out_15 = 15,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 16, DioType.Output )]
	//	Out_16 = 16,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 17, DioType.Output )]
	//	Out_17 = 17,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 18, DioType.Output )]
	//	Out_18 = 18,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 19, DioType.Output )]
	//	Out_19 = 19,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 20, DioType.Output )]
	//	Out_20 = 20,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 21, DioType.Output )]
	//	Out_21 = 21,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 22, DioType.Output )]
	//	Out_22 = 22,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 23, DioType.Output )]
	//	Out_23 = 23,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 24, DioType.Output )]
	//	Out_24 = 24,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 25, DioType.Output )]
	//	Out_25 = 25,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 26, DioType.Output )]
	//	Out_26 = 26,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 27, DioType.Output )]
	//	Out_27 = 27,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 28, DioType.Output )]
	//	Out_28 = 28,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 29, DioType.Output )]
	//	Out_29 = 29,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 30, DioType.Output )]
	//	Out_30 = 30,
	//	[IoPointAttribute( AdlinkModel.PCI7432, 0, 0, 31, DioType.Output )]
	//	Out_31 = 31,
	//}
	//#endregion



	//#region Output List
	//[Serializable]
	//public enum OutputIO7230
	//{
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 0, DioType.Output )]
	//	Out_00 = 00,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 1, DioType.Output )]
	//	Out_01 = 01,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 2, DioType.Output )]
	//	Out_02 = 02,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 3, DioType.Output )]
	//	Out_03 = 03,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 4, DioType.Output )]
	//	Out_04 = 04,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 5, DioType.Output )]
	//	Out_05 = 05,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 6, DioType.Output )]
	//	Out_06 = 06,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 7, DioType.Output )]
	//	Out_07 = 07,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 8, DioType.Output )]
	//	Out_08 = 08,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 9, DioType.Output )]
	//	Out_09 = 09,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 10, DioType.Output )]
	//	Out_10 = 10,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 11, DioType.Output )]
	//	Out_11 = 11,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 12, DioType.Output )]
	//	Out_12 = 12,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 13, DioType.Output )]
	//	Out_13 = 13,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 14, DioType.Output )]
	//	Out_14 = 14,
	//	[IoPointAttribute( AdlinkModel.PCI7230, 0, 0, 15, DioType.Output )]
	//	Out_15 = 15,
	//}
	//#endregion
}
//#region Input List
//[Serializable]
//public enum InputIO7230
//{
//	[DaskIoPointAttribute( "In_00", 1, DioType.Input )]
//	In_00 = 00,
//	[DaskIoPointAttribute( "In_01", 1, DioType.Input )]
//	In_01 = 01,
//	[DaskIoPointAttribute( "In_02", 1, DioType.Input )]
//	In_02 = 02,
//	[DaskIoPointAttribute( "In_03", 1, DioType.Input )]
//	In_03 = 03,
//	[DaskIoPointAttribute( "In_04", 1, DioType.Input )]
//	In_04 = 04,
//	[DaskIoPointAttribute( "In_05", 1, DioType.Input )]
//	In_05 = 05,
//	[DaskIoPointAttribute( "In_06", 1, DioType.Input )]
//	In_06 = 06,
//	[DaskIoPointAttribute( "In_07", 1, DioType.Input )]
//	In_07 = 07,
//	[DaskIoPointAttribute( "In_08", 1, DioType.Input )]
//	In_08 = 08,
//	[DaskIoPointAttribute( "In_09", 1, DioType.Input )]
//	In_09 = 09,
//	[DaskIoPointAttribute( "In_10", 1, DioType.Input )]
//	In_10 = 10,
//	[DaskIoPointAttribute( "In_11", 1, DioType.Input )]
//	In_11 = 11,
//	[DaskIoPointAttribute( "In_12", 1, DioType.Input )]
//	In_12 = 12,
//	[DaskIoPointAttribute( "In_13", 1, DioType.Input )]
//	In_13 = 13,
//	[DaskIoPointAttribute( "In_14", 1, DioType.Input )]
//	In_14 = 14,
//	[DaskIoPointAttribute( "In_15", 1, DioType.Input )]
//	In_15 = 15,
//	[DaskIoPointAttribute( "In_16", 1, DioType.Input )]
//	In_16 = 16,
//	[DaskIoPointAttribute( "In_17", 1, DioType.Input )]
//	In_17 = 17,
//	[DaskIoPointAttribute( "In_18", 1, DioType.Input )]
//	In_18 = 18,
//	[DaskIoPointAttribute( "In_19", 1, DioType.Input )]
//	In_19 = 19,
//	[DaskIoPointAttribute( "In_20", 1, DioType.Input )]
//	In_20 = 20,
//	[DaskIoPointAttribute( "In_21", 1, DioType.Input )]
//	In_21 = 21,
//	[DaskIoPointAttribute( "In_22", 1, DioType.Input )]
//	In_22 = 22,
//	[DaskIoPointAttribute( "In_23", 1, DioType.Input )]
//	In_23 = 23,
//	[DaskIoPointAttribute( "In_24", 1, DioType.Input )]
//	In_24 = 24,
//	[DaskIoPointAttribute( "In_25", 1, DioType.Input )]
//	In_25 = 25,
//	[DaskIoPointAttribute( "In_26", 1, DioType.Input )]
//	In_26 = 26,
//	[DaskIoPointAttribute( "In_27", 1, DioType.Input )]
//	In_27 = 27,
//	[DaskIoPointAttribute( "In_28", 1, DioType.Input )]
//	In_28 = 28,
//	[DaskIoPointAttribute( "In_29", 1, DioType.Input )]
//	In_29 = 29,
//	[DaskIoPointAttribute( "In_30", 1, DioType.Input )]
//	In_30 = 30,
//	[DaskIoPointAttribute( "In_31", 1, DioType.Input )]
//	In_31 = 31,
//}
//#endregion

//#region Output List
//[Serializable]
//public enum OutputIO7230
//{
//	[DaskIoPointAttribute( "Out_00", 0, DioType.Output )]
//	Out_00 = 00,
//	[DaskIoPointAttribute( "Out_01", 0, DioType.Output )]
//	Out_01 = 01,
//	[DaskIoPointAttribute( "Out_02", 0, DioType.Output )]
//	Out_02 = 02,
//	[DaskIoPointAttribute( "Out_03", 0, DioType.Output )]
//	Out_03 = 03,
//	[DaskIoPointAttribute( "Out_04", 0, DioType.Output )]
//	Out_04 = 04,
//	[DaskIoPointAttribute( "Out_05", 0, DioType.Output )]
//	Out_05 = 05,
//	[DaskIoPointAttribute( "Out_06", 0, DioType.Output )]
//	Out_06 = 06,
//	[DaskIoPointAttribute( "Out_07", 0, DioType.Output )]
//	Out_07 = 07,
//	[DaskIoPointAttribute( "Out_08", 0, DioType.Output )]
//	Out_08 = 08,
//	[DaskIoPointAttribute( "Out_09", 0, DioType.Output )]
//	Out_09 = 09,
//	[DaskIoPointAttribute( "Out_10", 0, DioType.Output )]
//	Out_10 = 10,
//	[DaskIoPointAttribute( "Out_11", 0, DioType.Output )]
//	Out_11 = 11,
//	[DaskIoPointAttribute( "Out_12", 0, DioType.Output )]
//	Out_12 = 12,
//	[DaskIoPointAttribute( "Out_13", 0, DioType.Output )]
//	Out_13 = 13,
//	[DaskIoPointAttribute( "Out_14", 0, DioType.Output )]
//	Out_14 = 14,
//	[DaskIoPointAttribute( "Out_15", 0, DioType.Output )]
//	Out_15 = 15,
//	[DaskIoPointAttribute( "Out_16", 0, DioType.Output )]
//	Out_16 = 16,
//	[DaskIoPointAttribute( "Out_17", 0, DioType.Output )]
//	Out_17 = 17,
//	[DaskIoPointAttribute( "Out_18", 0, DioType.Output )]
//	Out_18 = 18,
//	[DaskIoPointAttribute( "Out_19", 0, DioType.Output )]
//	Out_19 = 19,
//	[DaskIoPointAttribute( "Out_20", 0, DioType.Output )]
//	Out_20 = 20,
//	[DaskIoPointAttribute( "Out_21", 0, DioType.Output )]
//	Out_21 = 21,
//	[DaskIoPointAttribute( "Out_22", 0, DioType.Output )]
//	Out_22 = 22,
//	[DaskIoPointAttribute( "Out_23", 0, DioType.Output )]
//	Out_23 = 23,
//	[DaskIoPointAttribute( "Out_24", 0, DioType.Output )]
//	Out_24 = 24,
//	[DaskIoPointAttribute( "Out_25", 0, DioType.Output )]
//	Out_25 = 25,
//	[DaskIoPointAttribute( "Out_26", 0, DioType.Output )]
//	Out_26 = 26,
//	[DaskIoPointAttribute( "Out_27", 0, DioType.Output )]
//	Out_27 = 27,
//	[DaskIoPointAttribute( "Out_28", 0, DioType.Output )]
//	Out_28 = 28,
//	[DaskIoPointAttribute( "Out_29", 0, DioType.Output )]
//	Out_29 = 29,
//	[DaskIoPointAttribute( "Out_30", 0, DioType.Output )]
//	Out_30 = 30,
//	[DaskIoPointAttribute( "Out_31", 0, DioType.Output )]
//	Out_31 = 31,
//}
//#endregion
