using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using HiPA.Instrument.Motion.Dask;
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
	/// Interaction logic for Ctrl_InstrumentOfAxisBoard.xaml
	/// </summary>
	public partial class Ctrl_InstrumentOfIOBoard : PanelBase
	{
		public Ctrl_InstrumentOfIOBoard()
		{
			try
			{
				this.InitializeComponent();
				this.BindLockUI( this );
				this.Cb_AdlinkType.ItemsSource = Enum.GetValues( typeof( AdlinkModel ) ).Cast<AdlinkModel>();
				this.Cb_AdlinkType.SelectedItem = AdlinkModel.PCI7432;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		private IoBoardBase o_Source = null;
		public IoBoardBase Source
		{
			get => this.o_Source;
			set
			{
				try
				{
					this.o_Source = value;
					this.InitBar.Instrument = value;
					this.OnSetupBinding();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private string _TitleName = "";
		public string TitleName
		{
			set
			{
				try
				{
					this.Set( ref this._TitleName, value, "TitleName" );
					this.lbl_TitleName.Content = value;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
				}
			}
		}
		private void OnSetupBinding()
		{
			try
			{
				if ( this.o_Source is APSIoBoard )
				{
					var source = this.o_Source as APSIoBoard;
					Binding b = new Binding();
					b = new Binding();
					b.Source = source.Configuration;
					b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					b.Path = new PropertyPath( "CardType" );
					this.Cb_AdlinkType.SetBinding( ComboBox.SelectedItemProperty, b );

					b = new Binding();
					b.Source = source.Configuration;
					b.ValidatesOnDataErrors = true;
					b.ValidatesOnExceptions = true;
					b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					b.Path = new PropertyPath( "PollingRate" );
					Valid_Rule_INT RI = new Valid_Rule_INT();
					RI.Min = 10;
					RI.Max = 1000;
					b.ValidationRules.Clear();
					b.ValidationRules.Add( RI );
					this.txtPollingRate.SetBinding( NumericUpDown.ValueProperty, b );


					b = new Binding();
					b.Source = source.Configuration;
					b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					b.Path = new PropertyPath( "CardID" );
					RI = new Valid_Rule_INT();
					RI.Min = 0;
					RI.Max = 16;
					b.ValidationRules.Clear();
					b.ValidationRules.Add( RI );
					this.Txt_CardID.SetBinding( NumericUpDown.ValueProperty, b );
				}
				else
				{
					var source = this.o_Source as DaskIoBoard;
					Binding b = new Binding();
					b = new Binding();
					b.Source = source.Configuration;
					b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					b.Path = new PropertyPath( "CardType" );
					this.Cb_AdlinkType.SetBinding( ComboBox.SelectedItemProperty, b );

					b = new Binding();
					b.Source = source.Configuration;
					b.ValidatesOnDataErrors = true;
					b.ValidatesOnExceptions = true;
					b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					b.Path = new PropertyPath( "PollingRate" );
					Valid_Rule_INT RI = new Valid_Rule_INT();
					RI.Min = 10;
					RI.Max = 1000;
					b.ValidationRules.Clear();
					b.ValidationRules.Add( RI );
					this.txtPollingRate.SetBinding( NumericUpDown.ValueProperty, b );


					b = new Binding();
					b.Source = source.Configuration;
					b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					b.Path = new PropertyPath( "CardID" );
					RI = new Valid_Rule_INT();
					RI.Min = 0;
					RI.Max = 16;
					b.ValidationRules.Clear();
					b.ValidationRules.Add( RI );
					this.Txt_CardID.SetBinding( NumericUpDown.ValueProperty, b );
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
