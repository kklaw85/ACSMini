using HiPA.Common.Forms;
using N_Data_Utilities;
using System;
using System.Windows;
using System.Windows.Data;
namespace HiPA.Common
{
	public enum StateBGColor
	{
		Ready,
		Running,
		Pause,
		Warning,
		Error,
		Homing,
	}
	public enum StateFGColor
	{
		Message,
		Warning,
	}
	public class MachineStateMgr
		: BaseUtility
	{
		public class MachineStateChangeEventArgs : EventArgs
		{
			public MachineStateType MachineState { get; }
			public StateBGColor BGColor { get; }
			public bool LockUI { get; }
			public MachineStateChangeEventArgs( MachineStateType MachineState, StateBGColor BGColor, bool LockUI )
			{
				this.MachineState = MachineState;
				this.BGColor = BGColor;
				this.LockUI = LockUI;
			}
		}
		public event MachineStateChangedEventHandler MachineStateChanged;
		public delegate void MachineStateChangedEventHandler( object sender, MachineStateChangeEventArgs e );

		private Equipment _EM = null;
		public Equipment EM
		{
			set => this._EM = value;
		}
		public MachineStateMgr()
		{
		}
		#region Autorun pnl button handler
		public bool EnablePause
		{
			get => this.GetValue( () => this.EnablePause );
			set => this.SetValue( () => this.EnablePause, value );
		}
		public bool EnableStart
		{
			get => this.GetValue( () => this.EnableStart );
			set => this.SetValue( () => this.EnableStart, value );
		}
		public bool EnableCycleStop
		{
			get => this.GetValue( () => this.EnableCycleStop );
			set => this.SetValue( () => this.EnableCycleStop, value );
		}
		public bool EnableStop
		{
			get => this.GetValue( () => this.EnableStop );
			set => this.SetValue( () => this.EnableStop, value );
		}

		public void BindToEnableStart( FrameworkElement element )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "EnableStart" );
			element.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		public void BindToEnableStop( FrameworkElement element )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "EnableStop" );
			element.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		public void BindToEnablePause( FrameworkElement element )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "EnablePause" );
			element.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		public void BindToEnableCycleStop( FrameworkElement element )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "EnableCycleStop" );
			element.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		public void BindToStylePause( FrameworkElement element )
		{
			var b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "MachineStatus" );
			b.Converter = new AutorunPausetoStyle();
			element.SetBinding( FrameworkElement.StyleProperty, b );
		}
		public void BindToStyleCycleStop( FrameworkElement element )
		{
			var b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "MachineStatus" );
			b.Converter = new AutorunCycleStoptoStyle();
			element.SetBinding( FrameworkElement.StyleProperty, b );
		}

		private void AutorunCtrlAllDisabled()
		{
			this.EnableStart = false;
			this.EnablePause = false;
			this.EnableCycleStop = false;
			this.EnableStop = false;
		}
		private void AutorunCtrlCanStart()
		{
			this.EnableStart = true;
			this.EnablePause = false;
			this.EnableCycleStop = false;
			this.EnableStop = false;
		}
		private void AutorunCtrlStarted()
		{
			this.EnableStart = false;
			this.EnablePause = true;
			this.EnableCycleStop = true;
			this.EnableStop = true;
		}
		private void AutorunCtrlPaused()
		{
			this.EnableStart = true;
			this.EnablePause = false;
			this.EnableCycleStop = true;
			this.EnableStop = true;
		}
		private void AutorunCtrlCycleStopped()
		{
			this.EnableStart = false;
			this.EnablePause = true;
			this.EnableCycleStop = false;
			this.EnableStop = true;
		}
		#endregion

		private MachineStateType e_PreviousStatus = MachineStateType.NONE;
		private MachineStateType e_ManualPreviousStatus = MachineStateType.NONE;
		private MachineStateType e_MachineStatus = MachineStateType.NONE;
		public void RevertStateManualOp()
		{
			if ( this.e_ManualPreviousStatus != MachineStateType.UNINITIALIZE && this.e_ManualPreviousStatus != MachineStateType.ERROR )
				this.MachineStatus = this.e_ManualPreviousStatus;
			else
				this.MachineStatus = MachineStateType.READY;
		}
		public MachineStateType MachineStatus
		{
			get => this.e_MachineStatus;
			set
			{
				try
				{
					if ( this.e_MachineStatus != value && this.e_MachineStatus != MachineStateType.ERROR && this.e_MachineStatus != MachineStateType.WARNING && this.e_MachineStatus != MachineStateType.HOMING )
						this.e_PreviousStatus = this.e_MachineStatus;
					if ( this.e_MachineStatus != value && this.e_MachineStatus != MachineStateType.ERROR && this.e_MachineStatus != MachineStateType.WARNING && this.e_MachineStatus != MachineStateType.HOMING && this.e_MachineStatus != MachineStateType.BUSY )
						this.e_ManualPreviousStatus = this.e_MachineStatus;
					this.Set( ref this.e_MachineStatus, value, "MachineStatus" );
					var LockUI = false;
					// UI Background Colour
					switch ( value )
					{
						case MachineStateType.NONE:
						case MachineStateType.UNINITIALIZE:
						case MachineStateType.READY:
							this.StateBG = StateBGColor.Ready;
							break;
						case MachineStateType.AUTO_CYCLESTOP:
						case MachineStateType.AUTO_RUNNING:
						case MachineStateType.BUSY:
							this.StateBG = StateBGColor.Running;
							break;
						case MachineStateType.WARNING:
							this.StateBG = StateBGColor.Warning;
							break;
						case MachineStateType.ERROR:
							this.StateBG = StateBGColor.Error;
							break;
						case MachineStateType.AUTO_PAUSE:
							this.StateBG = StateBGColor.Pause;
							break;
						case MachineStateType.HOMING:
							this.StateBG = StateBGColor.Homing;
							break;
						default:
							break;
					}

					switch ( value )
					{
						case MachineStateType.READY:
							LockUI = false;
							break;
						case MachineStateType.UNINITIALIZE:
						case MachineStateType.AUTO_RUNNING:
						case MachineStateType.AUTO_PAUSE:
						case MachineStateType.AUTO_CYCLESTOP:
						case MachineStateType.HOMING:
						case MachineStateType.BUSY:
							LockUI = true;
							break;
						default:
							break;
					}

					if ( this.e_MachineStatus == MachineStateType.UNINITIALIZE )
						this.AutorunCtrlAllDisabled();
					else if ( this.e_MachineStatus == MachineStateType.READY )
						this.AutorunCtrlCanStart();
					else if ( this.e_MachineStatus == MachineStateType.AUTO_RUNNING )
						this.AutorunCtrlStarted();
					else if ( this.e_MachineStatus == MachineStateType.AUTO_PAUSE )
						this.AutorunCtrlPaused();
					else if ( this.e_MachineStatus == MachineStateType.AUTO_CYCLESTOP )
						this.AutorunCtrlCycleStopped();

					this.MachineStateChanged?.Invoke( this, new MachineStateChangeEventArgs( this.e_MachineStatus, this.StateBG, LockUI ) );
				}
				catch
				{

				}
			}
		}
		public void ClearAbnormalStatus()
		{
			if ( this.e_MachineStatus == MachineStateType.ERROR || this.e_MachineStatus == MachineStateType.WARNING )
			{
				if ( this.e_PreviousStatus == MachineStateType.UNINITIALIZE )
					this.e_PreviousStatus = MachineStateType.READY;
				this.MachineStatus = this.e_PreviousStatus;
			}
		}
		public StateBGColor StateBG
		{
			get => this.GetValue( () => this.StateBG );
			set
			{
				this.SetValue( () => this.StateBG, value );
				switch ( this.StateBG )
				{
					case StateBGColor.Ready:
						this.StateFG = StateFGColor.Message;
						break;
					case StateBGColor.Running:
						this.StateFG = StateFGColor.Warning;
						break;
					case StateBGColor.Pause:
						this.StateFG = StateFGColor.Warning;
						break;
					case StateBGColor.Warning:
						this.StateFG = StateFGColor.Warning;
						break;
					case StateBGColor.Error:
						this.StateFG = StateFGColor.Warning;
						break;
					case StateBGColor.Homing:
						this.StateFG = StateFGColor.Warning;
						break;
				}
			}
		}
		public StateFGColor StateFG
		{
			get => this.GetValue( () => this.StateFG );
			set => this.SetValue( () => this.StateFG, value );
		}
	}
}
