using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;

namespace NeoWisePlatform.CommonControls
{
	/// <summary>
	/// Interaction logic for Ctrl_AxisJogPage.xaml
	/// </summary>
	public partial class Ctrl_LiftsControl : PanelBase
	{
		public Ctrl_LiftsControl()
		{

			this.InitializeComponent();
		}

		private void OnSetupBinding()
		{
			try
			{
				this.New.Source = Constructor.GetInstance().GetInstrument( Lift.NewLift.ToString(), typeof( LiftModuleConfiguration ) ) as LiftModuleBase;
				this.QIC.Source = Constructor.GetInstance().GetInstrument( Lift.QICLift.ToString(), typeof( LiftModuleConfiguration ) ) as LiftModuleBase;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.OnSetupBinding();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

	}
}
