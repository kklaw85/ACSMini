using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.UI.CommonControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_PNPControl : PanelBase
	{
		public Ctrl_PNPControl()
		{
			#region Panel Lockable declaration
			#endregion
			this.InitializeComponent();
			this.BindLockUI( this );
			this.UnInspResult.ItemsSource = Enum.GetValues( typeof( UninspResult ) ).Cast<UninspResult>();
		}

		private void OnSetupBinding()
		{
			try
			{
				var PnpModule = Constructor.GetInstance().GetInstrument( typeof( PNPModuleConfiguration ) ) as PNPModule;
				this.PnpAxis.Axis = PnpModule.AxisX;
				this.LoadArm.Source = PnpModule.LoadArm;
				this.UnloadArm.Source = PnpModule.UnLoadArm;

				Binding b = new Binding();
				b.Source = PnpModule?.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "UnInspResult" );
				this.UnInspResult.SetBinding( ComboBox.SelectedItemProperty, b );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
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
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private async void Btn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var btn = sender as Button;

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
