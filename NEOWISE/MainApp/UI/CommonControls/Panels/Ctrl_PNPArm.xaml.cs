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
	public partial class Ctrl_PNPArm : PanelBase
	{
		public Ctrl_PNPArm()
		{
			#region Panel Lockable declaration
			#endregion
			this.InitializeComponent();
			this.BindLockUI( this );
			this.RegisterExpanderPnl( this.Expanderpnl );
		}
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		private IOMotion _Source = null;
		public IOMotion Source
		{
			get => this._Source;
			set
			{
				try
				{
					this._Source = value;
					this.DataContext = value;
					this.NameLbl.Content = this._Source?.Name;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.BindLockAccessLevelManufacturer( this.PickCfg );
				this.BindLockAccessLevelManufacturer( this.PlaceCfg );
				this.BindLockAccessLevelManufacturer( this.ArmCfg );
				this.BindLockMachineState( this.ManualCtrl );
				this.BindLockMachineState( this.AutoCtrl );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private async void Btn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				Equipment.MachStateMgr.MachineStatus = MachineStateType.BUSY;
				var btn = sender as Button;
				if ( btn == this.BtnArmDown )
				{
					var task = this._Source?.MoveToPos();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnArmUp )
				{
					var task = this._Source?.Reset();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnSuctionOn )
				{
					var task = this._Source?.SuctionOn();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnSuctionOff )
				{
					var task = this._Source?.SuctionOff();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnPick )
				{
					var task = this._Source?.PickUp();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnPlace )
				{
					var task = this._Source?.PlaceDown();
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
