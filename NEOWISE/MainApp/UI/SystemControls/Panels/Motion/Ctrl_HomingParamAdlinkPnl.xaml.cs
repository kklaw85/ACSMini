using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion;
using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Validations;

namespace NeoWisePlatform.SystemControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_InstrumentOfLTDMC.xaml
	/// </summary>
	public partial class Ctrl_HomingParamAdlinkPnl : PanelBase
	{
		public Ctrl_HomingParamAdlinkPnl()
		{
			this.InitializeComponent();


			this.Cb_HomeDir.ItemsSource = Enum.GetValues( typeof( HomingDirection ) ).Cast<HomingDirection>();
			this.Cb_HomeDir.SelectedItem = HomingDirection.Negative;
		}

		private HomingProfile _source = null;
		public HomingProfile Source
		{
			get => this._source;
			set
			{
				try
				{
					this._source = value;
					this.DataContext = value;
					this.OnSetupBinding();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}
		private void OnSetupBinding()
		{
			try
			{
				#region Motion Profile: Home Move
				var b = new Binding();
				//b.Source = this._source;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "HOME_MODE" );
				var RU = new Valid_Rule_INT();
				RU.Min = 0;
				RU.Max = 40;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( RU );
				this.Txt_HomeMode.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				//b.Source = ( this._Axis as AdLinkAxis ).Configuration.HomingProfile;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "HOME_DIR" );
				this.Cb_HomeDir.SetBinding( ComboBox.SelectedItemProperty, b );

				b = new Binding();
				//b.Source = ( this._Axis as AdLinkAxis ).Configuration.HomingProfile;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "HOME_EZ" );
				var d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 10000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_HomeEZ.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				//b.Source = ( this._Axis as AdLinkAxis ).Configuration.HomingProfile;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "HOME_VM" );
				d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 10000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_HomeMaxVel.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				//b.Source = ( this._Axis as AdLinkAxis ).Configuration.HomingProfile;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "HOME_VO" );
				d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 10000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_HomeOrgVel.SetBinding( NumericUpDown.ValueProperty, b );
				#endregion
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
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
