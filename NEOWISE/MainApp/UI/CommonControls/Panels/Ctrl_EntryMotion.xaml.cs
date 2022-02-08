using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;

namespace NeoWisePlatform.UI.CommonControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_IOMotionDisplayPnl.xaml
	/// </summary>
	public partial class Ctrl_EntryMotion : PanelBase
	{
		public Ctrl_EntryMotion()
		{
			this.InitializeComponent();
		}

		private PNPModule _PnpModule = null;
		public PNPModule PNPModule
		{
			get => this._PnpModule;
			set => this._PnpModule = value;
		}

		private void Apply_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var pos = 0d;
				this.PNPModule.AxisX.GetActualPosition( ref pos );
				this.Entry.Value = pos;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private async void Goto_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				Equipment.MachStateMgr.MachineStatus = MachineStateType.BUSY;
				var res = this._PnpModule.PNPMoveAbsolute( ( double )this.Entry.Value );
				await res;
				if ( res.Result.EClass != ErrorClass.OK )
					throw new Exception( res.Result.ErrorMessage );
				Equipment.MachStateMgr.RevertStateManualOp();
			}
			catch ( Exception ex )
			{
				Equipment.MachStateMgr.RevertStateManualOp();
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void PanelBase_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.BindLockMachineState( this.Ctrl );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}

		}
	}
}
