using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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
				this.DataContext = value?.Configuration;
				this.Exposure.DataContext = value?.Configuration.Exposure;
				this.Gain.DataContext = value?.Configuration.Gain;
				this.Gamma.DataContext = value?.Configuration.Gamma;
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
			var slide = sender as Slider;
			if ( slide == this.Sld_Exposure )
				this.Camera.SetExposureTime( this.Sld_Exposure.Value );
			else if ( slide == this.Sld_Gain )
				this.Camera.SetGain( this.Sld_Gain.Value );
			else if ( slide == this.Sld_Gamma )
				this.Camera.SetGamma( this.Sld_Gamma.Value );
		}

		private void Sld_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
		{
			if ( !this.IsLoaded ) return;
			var slide = sender as Slider;
			if ( slide == this.Sld_Exposure )
				this.Camera.SetExposureTime( this.Sld_Exposure.Value );
			else if ( slide == this.Sld_Gain )
				this.Camera.SetGain( this.Sld_Gain.Value );
			else if ( slide == this.Sld_Gamma )
				this.Camera.SetGamma( this.Sld_Gamma.Value );
		}
	}
}
