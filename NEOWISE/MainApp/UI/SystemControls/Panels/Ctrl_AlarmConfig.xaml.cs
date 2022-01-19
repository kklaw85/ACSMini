using HiPA.Common;
using HiPA.Common.UControl;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.SystemControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_BarcodeReader.xaml
	/// </summary>
	public partial class Ctrl_AlarmConfig : PanelBase
	{
		private AlarmConfig _Source = null;

		public AlarmConfig Source
		{
			get => this._Source;
			set
			{
				try
				{
					this._Source = value;
					this.OnSetupBinding();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}
		public Ctrl_AlarmConfig()
		{
			this.InitializeComponent();
			this.BindLockUI( this );
		}

		private void OnSetupBinding()
		{
			try
			{
				Binding b = new Binding() { Source = this._Source, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "EnCheck" ) };
				this.ChkEnableCheck.SetBinding( CheckBox.IsCheckedProperty, b );

				b = new Binding() { Source = this._Source, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "LSL" ) };
				this.Txt_LSL.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding() { Source = this._Source, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "USL" ) };
				this.Txt_USL.SetBinding( NumericUpDown.ValueProperty, b );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
