using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HiPA.Common.Forms
{

	public partial class PagePanelInstrument
		: PagePanelBase
	{
		public PagePanelInstrument()
		{
			this.InitializeComponent();
		}

		public override void OnPageLoad()
		{
			base.OnPageLoad();
			this.LockUI_MachineStatus = true;
			if ( this.TitleBar != null )
			{
				this.TitleBar.InitializeEvent += this.InitializeEventFromTitle;
				this.TitleBar.StopEvent += this.StopEventFromTitle;
			}
		}

		public override void OnDisplayData()
		{
			base.OnDisplayData();
		}

		public override void OnPageActived( bool isActived )
		{
			base.OnPageActived( isActived );
		}

		public override void OnLockStateChangedPrivilege( bool locked )
		{
			base.OnLockStateChangedPrivilege( locked );
			if ( this.TitleBar != null )
			{
				//this.TitleBar.LockUI = locked || MachineStateMng.isMachineBusy();
			}
		}

		public override void OnLockStateChanged( bool locked )
		{
			base.OnLockStateChanged( locked );
			this.LockAllPanel( locked );
		}

		#region Instrument Operation
		InstrumentBase _instrument = null;
		[Browsable( false )]
		public virtual InstrumentBase Instrument
		{
			get => this._instrument;
			set
			{
				if ( this._instrument != null )
				{
					this._instrument.LifeStateChanged -= this.InstrumentLifeStateChangedEvent;
					this._instrument.BehaviorStateChanged -= this.InstrumentBehaviorStateChangedEvent;
				}
				if ( value == null ) return;

				this._instrument = value;
				this._instrument.LifeStateChanged += this.InstrumentLifeStateChangedEvent;
				this._instrument.BehaviorStateChanged += this.InstrumentBehaviorStateChangedEvent;
				this.TitleBar?.UpdateTitle( this.Instrument );
				if ( this.LockUI_MachineStatus == this.IsValid )
					this.LockUI_MachineStatus = !this.IsValid;
				this.UpdateToUI();
			}
		}
		public override void MachineStatusChangedUpdateUI( object sender, EventArgs e )
		{
			base.MachineStatusChangedUpdateUI( sender, e );
			this._BeginInvoke( new Action( () =>
			{
				if ( this.TitleBar != null )
				{
					//this.TitleBar.LockUI = this.LockUI_Privilege || MachineStateMng.isMachineBusy();
				}
			} ) );
		}

		private void InstrumentLifeStateChangedEvent( object sender, InstrumentLifeStateChangedEventArgs e )
		{
			this.OnInstrumentLifeStateChanged( sender as InstrumentBase, e.PreviousState, e.CurrentState );
			//this._BeginInvoke( new Action( () => this.OnInstrumentLifeStateChanged( sender as InstrumentBase, e.PreviousState, e.CurrentState ) ) );
		}

		private void InstrumentBehaviorStateChangedEvent( object sender, InstrumentBehaviorStateChangedEventArgs e )
		{
			this.OnInstrumentBehaviorStateChanged( sender as InstrumentBase, e.PreviousState, e.CurrentState );
			//this._BeginInvoke( new Action( () => this.OnInstrumentBehaviorStateChanged( sender as InstrumentBase, e.PreviousState, e.CurrentState ) ) );
		}
		public virtual void OnInstrumentBehaviorStateChanged( InstrumentBase instrument, InstrumentBehaviorState previous, InstrumentBehaviorState current )
		{
			this._BeginInvoke( new Action( () =>
			{
				this.TitleBar?.UpdateTitle( this.Instrument );
				this.LockUI_MachineStatus = !this.IsValid;
			} ) );
		}
		protected virtual void OnInstrumentLifeStateChanged( InstrumentBase instrument, InstrumentLifeState previous, InstrumentLifeState current ) { }
		public bool IsValid
		{
			get
			{
				if ( this.Instrument == null )
					return false;
				return this.Instrument.BehaviorState == InstrumentBehaviorState.Ready;
			}
		}

		public virtual async void RunTaskWithValidState( Task work, Func<bool> checkBefore = null )
		{
			if ( this.IsValid == false ) return;
			if ( checkBefore != null && checkBefore() == false ) return;
			this.LockUI_MachineStatus = true;
			await work;
			this.LockUI_MachineStatus = false;
		}
		public void _BeginInvoke( Action action )
		{
			if ( this.IsHandleCreated ) this.BeginInvoke( action );
		}
		#endregion

		#region TitleBar Event 
		private async void InitializeEventFromTitle( object sender, EventArgs e )
		{
			this.LockUI_MachineStatus = true;
			this.TitleBar.InInit = true;
			var result = 0;

			var task = this.OnInitialize();
			if ( task != null )
			{
				await task;
				result = task.Result;
			}
			Application.DoEvents();
			this.TitleBar.InInit = false;
			this.TitleBar.IsInitialized = result == 0;
			this.LockUI_MachineStatus = result != 0;
		}

		private async void StopEventFromTitle( object sender, EventArgs e )
		{
			this.TitleBar.InInit = true;
			var result = 0;
			var task = this.OnStop();
			if ( task != null )
			{
				await task;
				result = task.Result;
			}
			Application.DoEvents();
			this.LockUI_MachineStatus = true;
			this.TitleBar.InInit = false;
			this.TitleBar.IsInitialized = false;
		}


		[DefaultValue( null )]
		public PagePanelTitle TitleBar { get; set; } = null;

		protected virtual Task<int> OnInitialize() => null;
		protected virtual Task<int> OnStop() => null;
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( this.components != null ) )
			{
				this.components.Dispose();
			}
			base.Dispose( disposing );
		}


		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}

		#endregion
	}
}
