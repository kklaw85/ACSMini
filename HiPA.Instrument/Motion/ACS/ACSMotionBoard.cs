using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Utils;
using SPIIPLUSCOM660Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Attribute = System.Attribute;

namespace HiPA.Instrument.Motion.ACS
{
	[Serializable]
	public class ACSBoardConfiguration
		: Configuration
	{
		[NonSerialized]
		private static readonly ACSBoardConfiguration _DEFAULT = new ACSBoardConfiguration();

		[NonSerialized]
		public static readonly string NAME = "ACS";

		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.Motion;
		public override Type InstrumentType => typeof( ACSMotionBoard );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public string IPAddress
		{
			get => this.GetValue( () => this.IPAddress );
			set => this.SetValue( () => this.IPAddress, value );
		}

	}

	public class ACSMotionBoard
		: MotionBoardBase
	{
		public override MachineVariant MachineVar { get; set; }
		#region AxisAttribute
		public class AxisAttribute : Attribute
		{
			public string Name { get; }
			public AxisAttribute( string name )
			{
				this.Name = name;
			}
		}
		#endregion
		[Serializable]
		public enum AxesList
		{
			[AxisAttribute( "X" )]
			StageX,

			[AxisAttribute( "Y" )]
			StageY,
		};
		#region internal var
		public Channel Comm { get; private set; }

		#endregion
		public ACSBoardConfiguration Configuration { get; private set; }
		public ObservableCollection<int> BoardHandler { get; set; } = new ObservableCollection<int>();
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Motion;

		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as ACSBoardConfiguration;
		}

		protected override string OnCreate()
		{
			this.ClearErrorFlags();
			try
			{
				if ( this.Comm == null ) this.Comm = new Channel();
				this.CheckAndThrowIfError( this.CreateAxes() );
				var tasks = this.GetChildren().Select( a => a.Create() ).ToArray();
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					this.CheckAndThrowIfError( t.Result );
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;

		}

		protected override string OnInitialize()
		{
			this.ClearErrorFlags();
			try
			{
				this.CheckAndThrowIfError( this.Open() );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
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
					this.CheckAndThrowIfError( t.Result );
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
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
					this.CheckAndThrowIfError( t.Result );
				}
				this.Close();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override string Open()
		{
			this.ClearErrorFlags();

			try
			{
				this.CheckAndThrowIfError( this.Connect() );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				this.Close();
			}
			return this.Result.ErrorMessage;
		}

		public override string Close()
		{
			this.ClearErrorFlags();
			try
			{
				this.CheckAndThrowIfError( this.Disconnect() );
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
			}
			return this.Result.ErrorMessage;
		}

		int _isOpen = 0;
		protected bool _IsOpen
		{
			get => Interlocked.CompareExchange( ref this._isOpen, 1, 1 ) == 1;
			set => Interlocked.Exchange( ref this._isOpen, value ? 1 : 0 );
		}

		public override bool IsOpen()
		{
			return this._IsOpen && this.Comm != null;
		}

		protected override string CreateAxes()
		{
			var result = string.Empty;
			try
			{
				var list = ReflectionTool.GetEnumAttributes<AxisAttribute>( typeof( AxesList ) );
				foreach ( var axispoint in list )
				{
					var axis = Constructor.GetInstance().GetInstrument( axispoint.Attribute.Name, typeof( ACSAxisConfiguration ) ) as ACSAxis;
					if ( axis == null ) throw new Exception( $"{axispoint.Attribute.Name} is null" );
					axis.BoardBase = this;
					this.AddChild( axis );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		#region Children Operation 
		public override void OnAddChild( string name )
		{
			try
			{
				base.OnAddChild( name );
				if ( this.Configuration.Children is null )
					this.Configuration.Children = new List<string>();

				if ( this.Configuration.Children.Contains( name ) == false )
					this.Configuration.Children.Add( name );

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this, $"On Add Child failure. Error : {ex.Message}", ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
		}
		public override void OnRemoveChild( string name )
		{
			try
			{
				if ( this.Configuration.Children is null )
					return;

				this.Configuration.Children.Remove( name );
				base.OnRemoveChild( name );

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this, $"On Remove Child failure. Error : {ex.Message}", ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
		}
		#endregion

		#region ACS operation
		private ErrorResult Connect()
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				if ( !this._IsOpen )
				{
					this.Comm.OpenCommEthernet( this.Configuration.IPAddress, this.Comm.ACSC_SOCKET_STREAM_PORT );
					this._IsOpen = true;
				}
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		private ErrorResult Disconnect()
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				if ( this._IsOpen )
				{
					this.Comm.CloseComm();
					this._IsOpen = false;
				}
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}

		public ErrorResult AbsoluteMove( int Axis, double dPos )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.ToPoint( 0, Axis, dPos );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult RelativeMove( int Axis, double distance )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.ToPoint( this.Comm.ACSC_AMF_RELATIVE, Axis, distance );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult VelocityMove( int Axis, double speed )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.Jog( this.Comm.ACSC_AMF_VELOCITY, Axis, speed );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult Home( int Axis, int Timeout )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.RunBuffer( Axis, "" );
				this.Comm.WaitProgramEnd( Axis, Timeout );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult Stop( int Axis )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.Kill( Axis );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult SetZero( int Axis )
		{
			this.ClearErrorFlags();
			try
			{
				this.CheckAndThrowIfError( this.SetPosition( Axis, 0 ) );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
			}
			return this.Result;
		}
		public ErrorResult SetPosition( int Axis, double Position )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.SetFPosition( Axis, Position );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult ServoOn( int Axis, int TimeOut )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.Enable( Axis ); // Enable motor
				this.Comm.WaitMotorEnabled( Axis, 1, TimeOut );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult ServoOff( int Axis )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.Disable( Axis );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult GetFault( int Axis, ref int Fault )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				Fault = this.Comm.GetFault( Axis );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult GetMState( int Axis, ref int State )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				State = this.Comm.GetMotorState( Axis );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult GetPosition( int Axis, ref double pos )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				pos = this.Comm.GetFPosition( Axis );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult MultiAbsoluteMove( int[] Axis, double[] dPos )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.ToPointM( 0, Axis, dPos );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult SetVelocity( int Axis, double vel )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.SetVelocity( Axis, vel );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult SetAcceleration( int Axis, double acc )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.SetAcceleration( Axis, acc );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult SetDeceleration( int Axis, double dec )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.SetDeceleration( Axis, dec );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult SetJerk( int Axis, double Jerk )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.SetJerk( Axis, Jerk );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult SetKillDeceleration( int Axis, double killdec )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.SetKillDeceleration( Axis, killdec );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult KillMotor( int Axis )
		{
			this.ClearErrorFlags();
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.Kill( Axis );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public ErrorResult SetCurrentLimit( int Axis, double Value )
		{
			this.ClearErrorFlags();
			try
			{
				var cmd = $"XCURI({Axis})={Value}";
				Monitor.Enter( this.SyncRoot );
				if ( MachineStateMng.isSimulation ) return this.Result;
				this.Comm.Send( cmd );
			}
			catch ( COMException ex )
			{
				this.CatchException( ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
			return this.Result;
		}
		public override void ApplyRecipe( RecipeBaseUtility recipeItem )
		{
			throw new NotImplementedException();
		}

		#endregion
	}


}
