using HiPA.Common.Forms;
using HiPA.Common.Report;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace HiPA.Common
{
	[Serializable]
	public enum InstrumentCategory
	{
		Motion,
		Axis,
		DIO,
		Camera,
		SourceMeter,
		Spectormeter,
		Oscilloscope,
		OpticalPowerMeter,
		PowerSupply,
		HeightSensor,
		BarcodeReader,
		DriverBoard,
		TempController,
		LightSource,
		LightPoint,
		NFCReader,
		ThermoHyDro,
		Laser,
		Module,
		Equipment,
		TemperatureSensor,
		Machine,
		ShopFloor,
		IOMotion,
		SuctionIO,
		StageClamper,
		DOutput,
	}

	[Serializable]
	public enum InstrumentLifeState
	{
		None = 0,

		Creating,
		Created,

		Terminating,
		Terminated,
	}

	[Serializable]
	public enum InstrumentBehaviorState
	{
		None = 0,

		Initializing = 0x01,
		Ready = 0x02,

		Pause = 0xF0,
		Pausing = 0x10,
		Paused = 0x20,

		Stopping = 0x100,
		Stopped = 0x200,
	}

	public abstract class InstrumentBase
		: Disposable, INotifyPropertyChanged
	{
		#region interlock
		public InterLock InternalProvider { get; set; } = new InterLock();
		public CollectiveInterlock Interlocks { get; set; } = new CollectiveInterlock();


		#endregion

		protected object SyncRoot { get; } = new object();

		public abstract string Name { get; }
		[XmlIgnore]
		public abstract string Location { get; protected set; }
		public abstract InstrumentCategory Category { get; }
		//public ILogger Logger { get; set; }
		public AlarmAgent AlarmAgent { get; set; } = null;
		public abstract MachineVariant MachineVar { get; set; }
		public LoggerHelper _Logger { get; set; }
		protected bool b_Enable = true;
		public bool Enable
		{
			get => this.b_Enable;
			set => this.Set( ref this.b_Enable, value, "Enable" );
		}
		#region Behavior State 
		private bool bIsValid = false;
		public bool IsValid
		{
			get => this.bIsValid;
			set => this.Set( ref this.bIsValid, value, "IsValid" );
		}
		int _behaviorState = ( int )InstrumentBehaviorState.None;
		[XmlIgnore]
		public InstrumentBehaviorState BehaviorState
		{
			get => ( InstrumentBehaviorState )Interlocked.CompareExchange( ref this._behaviorState, 0, 0 );
			protected set
			{
				Interlocked.Exchange( ref this._behaviorState, ( int )value );
				this.IsValid = this.BehaviorState == InstrumentBehaviorState.Ready;
			}
		}
		public event EventHandler<InstrumentBehaviorStateChangedEventArgs> BehaviorStateChanged;
		protected virtual string OnBehaviorStateChanged( InstrumentBehaviorState previous, InstrumentBehaviorState current ) => string.Empty;
		protected virtual string SetBehaviorState( InstrumentBehaviorState state, out bool bypass )
		{
			bypass = false;
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( state == this.BehaviorState )
				{
					bypass = true;
					return string.Empty;
				}

				var previous = this.BehaviorState;

				if ( state == InstrumentBehaviorState.Initializing )
				{
					if ( this.LifeState != InstrumentLifeState.Created )
						throw new InvalidOperationException( $"The Behavior state can not change to {state}. Because Life State is not Created" );
				}
				this.BehaviorState = state;
				this.BehaviorStateChanged?.Invoke( this, new InstrumentBehaviorStateChangedEventArgs( previous, state ) );
				return this.OnBehaviorStateChanged( previous, state );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}
		#endregion


		public Visibility AssignVisVari()
		{
			return this.ValidVariant() ? Visibility.Visible : Visibility.Collapsed;
		}
		public bool ValidVariant() => this.b_Enable && this.MachineVar.ValidVariant();

		public Task<string> Initialize()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				if ( !this.ValidVariant() ) return result;
				if ( ( result = this.SetBehaviorState( InstrumentBehaviorState.Initializing, out var bypass ) ) != String.Empty ) return result;
				if ( bypass == true ) return String.Empty;
				if ( ( result = this.OnInitialize() ) != string.Empty )
				{
					this.SetBehaviorState( InstrumentBehaviorState.None, out bypass );
					return result;
				}
				if ( ( result = this.SetBehaviorState( InstrumentBehaviorState.Ready, out bypass ) ) != string.Empty ) return result;

				return result;
			} );
		}
		protected abstract string OnInitialize();

		int _previousPauseState = ( int )InstrumentBehaviorState.None;
		[XmlIgnore]
		public InstrumentBehaviorState PreviousPauseState
		{
			get => ( InstrumentBehaviorState )Interlocked.CompareExchange( ref this._previousPauseState, 0, 0 );
			protected set => Interlocked.Exchange( ref this._previousPauseState, ( int )value );
		}
		public Task<string> Pause()
		{
			return Task<int>.Run( () =>
			{
				var result = string.Empty;

				var previous = this.BehaviorState;
				if ( ( result = this.SetBehaviorState( InstrumentBehaviorState.Pausing, out var bypass ) ) != string.Empty ) return result;
				if ( bypass == true ) return string.Empty;
				if ( ( result = this.OnPause() ) != string.Empty ) return result;
				if ( ( result = this.SetBehaviorState( InstrumentBehaviorState.Paused, out bypass ) ) != string.Empty ) return result;
				this.PreviousPauseState = previous;
				return result;
			} );
		}
		protected virtual string OnPause() => string.Empty;

		public Task<string> Resume()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;

				if ( ( result = this.OnResume() ) != string.Empty ) return result;
				if ( ( result = this.SetBehaviorState( this.PreviousPauseState, out var bypass ) ) != string.Empty ) return result;

				return result;
			} );
		}
		protected virtual string OnResume() => string.Empty;

		public Task<string> Stop()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				if ( !this.ValidVariant() ) return result;
				if ( ( result = this.SetBehaviorState( InstrumentBehaviorState.Stopping, out var bypass ) ) != string.Empty ) return result;
				if ( bypass == true ) return string.Empty;
				if ( ( result = this.OnStop() ) != string.Empty ) return result;
				if ( ( result = this.SetBehaviorState( InstrumentBehaviorState.Stopped, out bypass ) ) != string.Empty ) return result;

				return result;
			} );
		}
		protected abstract string OnStop();


		#region Instrument Life State 
		int _lifeState = ( int )InstrumentLifeState.None;
		[XmlIgnore]
		public InstrumentLifeState LifeState
		{
			get => ( InstrumentLifeState )Interlocked.CompareExchange( ref this._lifeState, 0, 0 );
			protected set => Interlocked.Exchange( ref this._lifeState, ( int )value );
		}
		public event EventHandler<InstrumentLifeStateChangedEventArgs> LifeStateChanged;
		protected virtual void OnLifeStateChanged( InstrumentLifeState Previous, InstrumentLifeState Current ) { }
		private void SetLifeState( InstrumentLifeState state )
		{
			if ( state == this.LifeState ) return;

			var previous = this.LifeState;
			var successed = true;
			switch ( state )
			{
				case InstrumentLifeState.Creating: successed = previous == InstrumentLifeState.None; break;
				case InstrumentLifeState.Created: successed = previous == InstrumentLifeState.Creating; break;
			}

			if ( successed == false )
			{
				throw new InvalidOperationException( $"The Life State can not change to {state} from {this.LifeState}" );
			}

			this.LifeState = state;
			this.LifeStateChanged?.Invoke( this, new InstrumentLifeStateChangedEventArgs( previous, state ) );
			this.OnLifeStateChanged( previous, state );
		}
		#endregion

		#region Life Operation 
		public Task<string> Create()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					Monitor.Enter( this.SyncRoot );
					if ( !this.ValidVariant() ) return result;
					if ( this.LifeState == InstrumentLifeState.Created ) return string.Empty;
					this.SetLifeState( InstrumentLifeState.Creating );

					//this.SerializeAgent = new XmlSerializeAgent( this, Constructor.GetInstance().ConfigPath );
					//if ( ( result = this.SerializeAgent.Load() ) != 0 ) return result;

					result = this.OnCreate();

					this.SetLifeState( InstrumentLifeState.Created );
					return result;
				}
				finally
				{
					Monitor.Exit( this.SyncRoot );
				}
			} );
		}
		protected abstract string OnCreate();

		public Task<string> Terminate()
		{
			return Task<string>.Run( () =>
			{
				var result = string.Empty;
				try
				{
					Monitor.Enter( this.SyncRoot );
					if ( !this.ValidVariant() ) return result;
					if ( this.LifeState == InstrumentLifeState.Terminated ) return string.Empty;
					if ( !this.b_Enable ) return result;
					this.SetLifeState( InstrumentLifeState.Terminating );
					result = this.OnTerminate();
					this.SetLifeState( InstrumentLifeState.Terminated );
					return result;
				}
				finally
				{
					Monitor.Exit( this.SyncRoot );
				}
			} );
		}
		protected abstract string OnTerminate();

		#endregion

		public abstract void ApplyConfiguration( Configuration configuration );
		public abstract void ApplyRecipe( RecipeBaseUtility recipeItem );

		int _isRunning = 0;
		public virtual bool IsRunning
		{
			get => Interlocked.CompareExchange( ref this._isRunning, 1, 1 ) == 1;
			set => Interlocked.Exchange( ref this._isRunning, value ? 1 : 0 );
		}

		#region Owner Operation
		InstrumentBase _owner = null;
		public InstrumentBase Owner
		{
			get => this._owner;
			set
			{
				if ( value == this._owner ) return;
				this._owner = value;
				this.Location = this._owner != null ? this._owner.Location + "/" + this.Name : this.Name;
			}
		}

		List<InstrumentBase> _children;
		public string AddChild( InstrumentBase child )
		{
			var result = string.Empty;
			try
			{
				if ( child == null )
					throw new Exception( "Argument is null" );

				if ( this._children == null )
					this._children = new List<InstrumentBase>();

				if ( this._children.Contains( child, new InstrumentBaseCompare() ) == true )
					return string.Empty;

				this._children.Add( child );
				this.OnAddChild( child.Name );
			}
			catch ( Exception ex )
			{
				result = $"AddChild:,{ex.Message}";
			}
			return result;
		}


		public void RemoveChild( InstrumentBase child )
		{
			if ( child == null ) return;
			if ( this._children == null ) return;
			this.OnRemoveChild( child.Name );
			this._children.Remove( child );
		}

		public IEnumerable<InstrumentBase> GetChildren()
		{
			if ( this._children == null )
				return new List<InstrumentBase>();
			return this._children;
		}

		public virtual void OnAddChild( string name )
		{
		}
		public virtual void OnRemoveChild( string name )
		{
		}
		#endregion


		#region Override Object Function 
		public override bool Equals( object obj )
		{
			if ( obj == null ) return false;
			if ( typeof( InstrumentBase ).IsAssignableFrom( obj.GetType() ) == false ) return false;
			return this.Name == ( obj as InstrumentBase ).Name;
		}
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return
				$"Name :     {this.Name}\r\n" +
				$"Location : {this.Location}\r\n" +
				$"Life :     {this.LifeState}" +
				$"Behavior : {this.BehaviorState}" +
				$"Class :    {this.GetType()}";
		}
		#endregion


		public ModuleBase GetModule()
		{
			for ( var owner = this.Owner; owner != null; owner = owner.Owner )
			{
				if ( owner is ModuleBase ) return owner as ModuleBase;
			}
			return null;
		}
		public AlarmAgent GetAlarmAgent()
		{
			for ( var owner = this; owner != null; owner = owner.Owner )
			{
				if ( owner.AlarmAgent != null ) return owner.AlarmAgent;
			}
			return null;
		}

		public string FormatErrMsg( string Name, string ex )
		{
			var TypeName = this.GetType().Name;
			var st = new StackTrace();
			var sf = st.GetFrame( 1 );
			var MethodName = sf.GetMethod().Name;
			string fullMethodName = $"{TypeName}:{MethodName}:,";
			return $"{Name}:{fullMethodName}{ex}";
		}
		public string GetMethodName()
		{
			var st = new StackTrace();
			var sf = st.GetFrame( 1 );
			var NameSplit = sf.GetMethod().Name.Split( '<', '>' );
			var Name = string.Empty;
			foreach ( var segment in NameSplit )
			{
				if ( Name.Length < segment.Length )
					Name = segment;
			}
			return Name;
		}

		public string GetPropertyName<T>( Expression<Func<T>> propertyExpression )
		{
			return ( propertyExpression.Body as MemberExpression ).Member.Name;
		}
	}

	#region Instrument Life State Changed Event Args 
	public class InstrumentLifeStateChangedEventArgs
		: EventArgs
	{
		public InstrumentLifeStateChangedEventArgs( InstrumentLifeState previous, InstrumentLifeState current )
		{
			this.PreviousState = previous;
			this.CurrentState = current;
		}
		public InstrumentLifeState PreviousState { get; private set; }
		public InstrumentLifeState CurrentState { get; private set; }
	}
	#endregion

	#region Instrument Behavior State Changed Event Args
	public class InstrumentBehaviorStateChangedEventArgs
		: EventArgs
	{
		public InstrumentBehaviorStateChangedEventArgs( InstrumentBehaviorState previous, InstrumentBehaviorState current )
		{
			this.PreviousState = previous;
			this.CurrentState = current;
		}
		public InstrumentBehaviorState PreviousState { get; private set; }
		public InstrumentBehaviorState CurrentState { get; private set; }
	}
	#endregion

	#region EqualityCompare 
	public class InstrumentBaseCompare
		: IEqualityComparer<InstrumentBase>
	{
		public bool Equals( InstrumentBase x, InstrumentBase y )
		{
			return x.Name == y.Name;
		}

		public int GetHashCode( InstrumentBase obj )
		{
			return obj.Name.GetHashCode();
		}
	}
	#endregion

	[Serializable]
	public class AlarmConfig : RecipeBaseUtility
	{
		public event EventHandler<double> OutofSpec;
		private double d_USL = 0;
		public double USL
		{
			get => this.d_USL;
			set => this.Set( ref this.d_USL, value, "USL" );
		}
		private double d_LSL = 0;
		public double LSL
		{
			get => this.d_LSL;
			set => this.Set( ref this.d_LSL, value, "LSL" );
		}
		private bool b_EnCheck = false;
		public bool EnCheck
		{
			get => this.b_EnCheck;
			set => this.Set( ref this.b_EnCheck, value, "EnCheck" );
		}
		private double dReading = 0;
		[XmlIgnore]
		public double Reading
		{
			get => this.dReading;
			set
			{
				this.Set( ref this.dReading, value, "Reading" );
				if ( this.b_EnCheck && !this.Triggered && ( this.dReading > this.USL || this.dReading < this.LSL ) )
				{
					this.OutofSpec.Invoke( this, this.dReading );
					this.Triggered = true;
				}
				else if ( this.dReading <= this.USL && this.dReading >= this.LSL )
					this.Triggered = false;
			}
		}
		private bool Triggered = false;
	}
	[Serializable]
	public abstract class MachineVariant : BaseUtility
	{
		public bool ValidVariant()
		{
			bool Valid = true;
			var Properties = this.GetType().GetProperties();
			foreach ( var prop in Properties )
			{
				if ( !( prop.PropertyType == typeof( Variant ) ) ) continue;
				var variant = prop.GetValue( this, null ) as Variant;
				if ( variant.isValid == false )
				{
					Valid = false;
					break;
				}
			}
			return Valid;
		}
		public MachineVariant()
		{ }
	}
	[Serializable]
	public class Variant : BaseUtility
	{
		[XmlIgnore]
		private Variant EquipmentVar { get; set; }
		private int iType = 0;
		public int Type
		{
			get => this.iType;
			set => this.Set( ref this.iType, value, "Type" );
		}
		[XmlIgnore]
		public bool isValid => ( this.Type & this.EquipmentVar?.Type ) != 0 || this.EquipmentVar == null || this.Type == 0;
		public Variant()
		{ }
		public Variant( Variant Source )
		{
			this.EquipmentVar = Source;
		}
		public Variant( Variant Source, int Type )
		{
			this.EquipmentVar = Source;
			this.Type = Type;
		}
	}
	[Serializable]
	public class MachineVar : MachineVariant
	{
		public Variant MachineVariant { get; set; } = new Variant();
		public MachineVar()
		{ }
		public MachineVar( eMachineType MacType )
		{
			//var macvar = Constructor.GetInstance().Equipment.Configuration.MachineVar as MachineVar;
			this.MachineVariant = new Variant( ( Constructor.GetInstance()?.Equipment?.Configuration?.MachineVar as MachineVar )?.MachineVariant, ( int )MacType );
		}
		public MachineVar( MachineVar source )
		{
			this.MachineVariant = new Variant( ( Constructor.GetInstance()?.Equipment?.Configuration?.MachineVar as MachineVar )?.MachineVariant, source.MachineVariant.Type );
		}

		public MachineVar( bool All )
		{
			if ( All )
			{
				this.MachineVariant = new Variant( ( Constructor.GetInstance()?.Equipment?.Configuration?.MachineVar as MachineVar )?.MachineVariant, ( int )eMachineType.All );
			}
		}
	}


	public class InterLock : BaseUtility
	{
		public bool IsSafeToOperate
		{
			get => this.GetValue( () => this.IsSafeToOperate );
			set => this.SetValue( () => this.IsSafeToOperate, value );
		}
		public void SetSafe()
		{
			this.IsSafeToOperate = true;
		}
		public void SetUnsafe()
		{
			this.IsSafeToOperate = false;
		}
	}
	public class CollectiveInterlock : BaseUtility
	{
		private List<InterLock> SubscribedInterlock { get; set; } = new List<InterLock>();
		public void Subscribe( InstrumentBase source )
		{
			this.SubscribedInterlock.Add( source.InternalProvider );
		}
		public void ClearSubscription()
		{
			this.SubscribedInterlock.Clear();
		}
		public bool IsSafeToOperate()
		{
			if ( this.SubscribedInterlock.Count == 0 ) return true;
			foreach ( var interlock in this.SubscribedInterlock )
			{
				if ( !interlock.IsSafeToOperate ) return false;
			}
			return true;
		}
	}
}
