using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.CommonControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_LiftControl : PanelBase
	{
		public Ctrl_LiftControl()
		{
			#region Panel Lockable declaration
			this.MachineStateLockable = true;
			#endregion
			this.InitializeComponent();
			this.BindLockUI(this);
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		private LiftModuleBase _Source = null;
		public LiftModuleBase Source
		{
			get => this._Source;
			set
			{
				try
				{
					this._Source = value;
					this.DataContext = value;
					this.NameLbl.Content = this._Source?.Name;
					this.PusherExp.Visibility = this._Source?.Pusher?.IOPt == null ? Visibility.Collapsed : Visibility.Visible;
					this.PusherCtrl.Visibility = this._Source?.Pusher?.IOPt == null ? Visibility.Collapsed : Visibility.Visible;
					this.Blower.Visibility = this._Source?.Blower?.IOPt == null ? Visibility.Collapsed : Visibility.Visible;
					this.BlowerIO.Visibility = this._Source?.Blower?.IOPt == null ? Visibility.Collapsed : Visibility.Visible;
					this.PusherIO.Visibility = this._Source?.Pusher?.IOPt == null ? Visibility.Collapsed : Visibility.Visible;
				}
				catch (Exception ex)
				{
					Equipment.ErrManager.RaiseWarning(this.FormatErrMsg(this.Name, ex), ErrorTitle.InvalidOperation);
				}
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				this.BindLockAccessLevelManufacturer(this.Configs);
				this.BindLockMachineState(this.Ctrls);
			}
			catch (Exception ex)
			{
				Equipment.ErrManager.RaiseWarning(this.FormatErrMsg(this.Name, ex), ErrorTitle.InvalidOperation);
			}
		}

		private async void Btn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//Equipment.MachStateMgr.MachineStatus = MachineStateType.BUSY;
				var btn = sender as Button;
				if (btn == this.BtnLoadPos)
				{
					var task = this._Source.MoveToLoadPos();
					await task;
					if (task.Result.EClass != ErrorClass.OK) throw new Exception(task.Result.ErrorMessage);
				}
				else if (btn == this.BtnCollectionPos)
				{
					var task = this._Source.StartSingleAction(true);
					await task;
					if (task.Result.EClass != ErrorClass.OK) throw new Exception(task.Result.ErrorMessage);
				}
				else if (btn == this.BtnAutomated)
				{
					var task = this._Source.StartAuto();
					await task;
					if (task.Result.EClass != ErrorClass.OK) throw new Exception(task.Result.ErrorMessage);
				}
				else if (btn == this.BtnStopAutomation)
				{
					var task = this._Source.StopAuto();
					await task;
					if (task.Result.EClass != ErrorClass.OK) throw new Exception(task.Result.ErrorMessage);
				}
				else if ( btn == this.BtnPickUpIP )
				{
					var res = this._Source.StartPickup();
					if ( res.EClass != ErrorClass.OK ) throw new Exception( res.ErrorMessage );
				}
				else if ( btn == this.BtnPickUpDone )
				{
					var res = this._Source.EndPickup();
					if ( res.EClass != ErrorClass.OK ) throw new Exception( res.ErrorMessage );
				}
				else if ( btn == this.BtnTriggerPusher )
				{
					var task = this._Source.TrigPusher();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				//Equipment.MachStateMgr.RevertStateManualOp();
			}
			catch (Exception ex)
			{
				//Equipment.MachStateMgr.RevertStateManualOp();
				Equipment.ErrManager.RaiseWarning(this.FormatErrMsg(this.Name, ex), ErrorTitle.InvalidOperation);
			}
			finally
			{
			}
		}
	}
}
