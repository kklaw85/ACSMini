using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion.APS;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using N_Data_Utilities;
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
	public partial class Ctrl_InstrumentOfAxisBoard : PanelBase
	{
		public Ctrl_InstrumentOfAxisBoard()
		{
			try
			{
				this.InitializeComponent();
				this.BindLockUI( this );
				this.Cb_AxisMNETTxRate.ItemsSource = Enum.GetValues( typeof( MNETTransferRate ) ).Cast<MNETTransferRate>();
				this.Cb_AxisMNETTxRate.SelectedItem = MNETTransferRate.Mbps20000;
				this.Cb_AdlinkType.ItemsSource = Enum.GetValues( typeof( AdlinkModel ) ).Cast<AdlinkModel>();
				this.Cb_AdlinkType.SelectedItem = AdlinkModel.AMP204208C;
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
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}

		}
		private AdLinkMotionBoard o_Source = null;
		public AdLinkMotionBoard Source
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
		private bool b_is204C = false;
		public bool is204C
		{
			get => this.b_is204C;
			set => this.Set( ref this.b_is204C, value, "is204C" );
		}
		private void OnSetupBinding()
		{
			try
			{
				//this.lvwCardIDList.ItemsSource = this.o_Source.BoardHandler;

				Binding b = new Binding();
				b.Source = this;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "is204C" );
				b.Converter = new BoolToVisibilityReverse();
				b.Mode = BindingMode.OneWay;
				this.MnetStackPanel.SetBinding( StackPanel.VisibilityProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "StartAxisId" );
				Valid_Rule_INT RU = new Valid_Rule_INT();
				RU.Min = 0;
				RU.Max = 2000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( RU );
				this.Txt_AxisBoardStartID.SetBinding( NumericUpDown.ValueProperty, b );

				// Combo Box
				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "MNETTransferRate" );
				this.Cb_AxisMNETTxRate.SetBinding( ComboBox.SelectedItemProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "AdlinkModel" );
				this.Cb_AdlinkType.SetBinding( ComboBox.SelectedItemProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "BoardParametersFile" );
				this.txt_AxisCfgFilePath.SetBinding( TextBox.TextProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "CardID" );
				RU = new Valid_Rule_INT();
				RU.Min = 0;
				RU.Max = 100;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( RU );
				this.Txt_CardID.SetBinding( NumericUpDown.ValueProperty, b );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void Btn_AxisBoardCfgFilePath_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.InitialDirectory = Constructor.GetInstance().ConfigPath;
				openFileDialog.Filter = "eXtensible Markup Language files (*.xml)|*.xml";
				if ( openFileDialog.ShowDialog() == true )
					this.txt_AxisCfgFilePath.Text = openFileDialog.FileName;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void Cb_AxisMNETTxRate_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			try
			{

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private async void Btn_Initialize_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var tsk = this.Source.Initialize();
				await tsk;
				if ( tsk.Result != string.Empty )
					throw new Exception( tsk.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private async void Btn_Stop_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var tsk = this.Source.Stop();
				await tsk;
				if ( tsk.Result != string.Empty )
					throw new Exception( tsk.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
	}
}
