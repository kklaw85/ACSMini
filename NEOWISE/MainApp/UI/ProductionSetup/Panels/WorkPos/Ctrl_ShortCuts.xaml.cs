using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.ProductionSetup.Panels.WorkPos
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_ShortCuts : PanelBase
	{
		public Ctrl_ShortCuts()
		{
			#region Panel Lockable declaration
			#endregion
			this.InitializeComponent();
			this.BindLockUI( this );
			this.BindLockMachineState( this.Content );
		}
		MTEquipment Eq;
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.Eq = ( Constructor.GetInstance().Equipment as MTEquipment );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}

		}

		private async void Button_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				Equipment.MachStateMgr.MachineStatus = MachineStateType.BUSY;
				var btn = sender as Button;
				if ( btn == this.Load )
				{
					var task = this.Eq.MoveToLoadPos();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.Work )
				{
					var task = this.Eq.MoveToWorkPos();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				Equipment.MachStateMgr.RevertStateManualOp();
			}
			catch ( Exception ex )
			{
				Equipment.MachStateMgr.RevertStateManualOp();
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
