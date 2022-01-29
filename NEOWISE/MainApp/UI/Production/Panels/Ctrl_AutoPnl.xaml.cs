using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.Production.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_Running_Auto.xaml
	/// </summary>
	public partial class Ctrl_AutoPnl : PanelBase
	{
		private MTEquipment Eq = null;

		public Ctrl_AutoPnl()
		{
			this.InitializeComponent();
		}

		private void Btn_Start_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var button = sender as Button;
				if ( button == null ) return;
				var Action = button.Tag.ToString();
				switch ( Action )
				{
					case "Start": this.Eq.AutoSeq.StartAuto(); break;
					case "Pause": this.Eq.AutoSeq.PauseAuto(); break;
					case "Stop": this.Eq.AutoSeq.StopAuto(); break;
					case "CycleStop": this.Eq.AutoSeq.CycleStopAuto(); break;
					default:
						break;
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private void UserControl_Loaded_1( object sender, RoutedEventArgs e )
		{
			try
			{
				this.Eq = Constructor.GetInstance().Equipment as MTEquipment;
				this.OnSetupBinding();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void OnSetupBinding()
		{
			try
			{
				MTEquipment.MachStateMgr.BindToEnableStart( this.Btn_Start );
				MTEquipment.MachStateMgr.BindToEnablePause( this.Btn_Pause );
				MTEquipment.MachStateMgr.BindToEnableCycleStop( this.Btn_Cycle_Stop );
				MTEquipment.MachStateMgr.BindToEnableStop( this.Btn_Stop );
				MTEquipment.MachStateMgr.BindToStylePause( this.Btn_Pause );
				MTEquipment.MachStateMgr.BindToStyleCycleStop( this.Btn_Cycle_Stop );
				this.BindLockPrivilege( this.Btn_Start );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
