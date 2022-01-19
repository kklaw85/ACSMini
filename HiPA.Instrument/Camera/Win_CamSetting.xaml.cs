using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace HiPA.Instrument.Camera
{
	/// <summary>
	/// Interaction logic for Win_Pre_Init.xaml
	/// </summary>
	public partial class Win_CamSetting : MahApps.Metro.Controls.MetroWindow
	{
		public Win_CamSetting()
		{
			this.InitializeComponent();
		}
		private MatroxCamera _Camera = null;
		public MatroxCamera Camera
		{
			get => this._Camera;
			set
			{
				this._Camera = value;
				this.OnSetupBinding();
			}
		}

		private void OnSetupBinding()
		{
			try
			{
				#region Configurations
				//Set Exposure Bindings
				Binding b = new Binding();
				b.Source = this._Camera.Configuration.Exposure;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Min" );
				this.Sld_Exposure.SetBinding( Slider.MinimumProperty, b );

				b = new Binding();
				b.Source = this._Camera.Configuration.Exposure;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Max" );
				this.Sld_Exposure.SetBinding( Slider.MaximumProperty, b );

				b = new Binding();
				b.Source = this._Camera.Configuration.Exposure;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Value" );
				this.Sld_Exposure.SetBinding( Slider.ValueProperty, b );

				//Display to LblExposureDisplay
				b = new Binding();
				b.Source = this._Camera.Configuration.Exposure;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Value" );
				this.Lbl_Exposure_Val.SetBinding( Label.ContentProperty, b );

				//Set Gain Bindings
				b = new Binding();
				b.Source = this._Camera.Configuration.Gain;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Min" );
				this.Sld_Gain.SetBinding( Slider.MinimumProperty, b );

				b = new Binding();
				b.Source = this._Camera.Configuration.Gain;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Max" );
				this.Sld_Gain.SetBinding( Slider.MaximumProperty, b );

				b = new Binding();
				b.Source = this._Camera.Configuration.Gain;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Value" );
				this.Sld_Gain.SetBinding( Slider.ValueProperty, b );

				//Display to LblGainDisplay
				b = new Binding();
				b.Source = this._Camera.Configuration.Gain;
				b.Mode = BindingMode.OneWay;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Value" );
				this.Lbl_Gain_Val.SetBinding( Label.ContentProperty, b );
				#endregion
			}
			catch
			{
			}
		}

		private void MetroWindow_Loaded_1( object sender, RoutedEventArgs e )
		{

		}

		private void Btn_Close_Cam_Sett_Click( object sender, RoutedEventArgs e )
		{
			this.Close();
		}


		private void Slider_DragCompleted( object sender, DragCompletedEventArgs e )
		{
			if ( ( sender as Slider ).Name == "Sld_Exposure" )
				this.Camera.SetExposureTime( this.Sld_Exposure.Value );
			else
				this.Camera.SetGain( this.Sld_Gain.Value );
		}

		private void Sld_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
		{
			if ( !this.IsLoaded ) return;
			if ( ( sender as Slider ).Name == "Sld_Exposure" )
				this.Camera.SetExposureTime( this.Sld_Exposure.Value );
			else
				this.Camera.SetGain( this.Sld_Gain.Value );
		}
	}
}
