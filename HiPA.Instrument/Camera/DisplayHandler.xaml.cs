using HiPA.Common;
using HiPA.Common.UControl;
using N_Data_Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace HiPA.Instrument.Camera
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class DisplayHandler : PanelBase
	{
		Win_CamSetting Win_CamSetting_Dlg = null;
		public DisplayHandler()
		{
			this.InitializeComponent();
		}
		MatroxCamera _CAM = null;
		public InstrumentBase Instrument
		{
			get => this._CAM;
			set
			{
				if ( value == null ) return;
				//if ( this._CAM == value ) return;
				this._CAM = value as MatroxCamera;
				this.OnSetupBinding();
			}
		}
		private Thickness _DisplayMargin = new Thickness();
		public Thickness SetDisplayMargin
		{
			get => this._DisplayMargin;
			set => this._DisplayMargin = value;
		}
		private ROI eROItype = ROI.Cal;
		public ROI ROItype
		{
			get => this.eROItype;
			set
			{
				this.eROItype = value;
				if ( this._CAM != null )
				{
					this._CAM.Camera.Cal.ShowROI = this.eROItype == ROI.Cal ? ( bool )this.Chk_ROI.IsChecked : false;
					this._CAM.Camera.Inspect.ShowROI = this.eROItype == ROI.Inspect ? ( bool )this.Chk_ROI.IsChecked : false;
				}
			}
		}

		private void OnSetupBinding()
		{
			try
			{
				Binding b = new Binding();
				b.Source = this._CAM;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Name" );
				this.Title.SetBinding( Label.ContentProperty, b );
				if ( this._CAM != null )
				{
					var AspectRatio = ( ( double )this._CAM.Resolution.MaxWidth / ( double )this._CAM.Resolution.MaxHeight );
					this.Display.Height = this.Width / AspectRatio;
				}
				b = new Binding();
				b.Source = this._CAM;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ZoomScale" );
				this.Sld_Zoom.SetBinding( Slider.ValueProperty, b );

				this.Sld_Zoom.Maximum = 5;
				this.Sld_Zoom.Minimum = 1;

				b = new Binding();
				b.Source = this._CAM;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ZoomScale" );
				b.Converter = new DisplayTwoDecPlacesDouble();
				this.Lbl_Zoom_Level.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this._CAM.Camera;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "IsShowCrossLine" );
				this.Chk_xHair.SetBinding( CheckBox.IsCheckedProperty, b );
			}
			catch
			{
			}
		}
		private bool b_EnableROI = false;
		public bool EnableROI
		{
			get => this.b_EnableROI;
			set
			{
				this.b_EnableROI = value;
				this.Chk_ROI.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				this.Rec_ROI.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				if ( !value )
					this.Chk_ROI.IsChecked = false;
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			this.Chk_ROI.IsChecked = false;
			this.Chk_ROI.Visibility = this.b_EnableROI ? Visibility.Visible : Visibility.Collapsed;
			this.Rec_ROI.Visibility = this.b_EnableROI ? Visibility.Visible : Visibility.Collapsed;
		}


		private void Btn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				if ( btn.Name.Contains( "Live" ) )
				{
					this._CAM?.ContinuousGrab();
				}
				else if ( btn.Name.Contains( "OneShot" ) )
				{
					this._CAM?.SingleGrab();
				}
				else if ( btn.Name.Contains( "CamSetting" ) )
				{
					if ( this.Win_CamSetting_Dlg != null )
					{
						if ( !this.Win_CamSetting_Dlg.IsVisible )
							this.Win_CamSetting_Dlg = null;
					}
					if ( this.Win_CamSetting_Dlg == null )
					{
						this.Win_CamSetting_Dlg = new Win_CamSetting();
						this.Win_CamSetting_Dlg.Camera = this._CAM;
						this.Win_CamSetting_Dlg.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
						//Mouse Exact Position
						this.Win_CamSetting_Dlg.Top = System.Windows.Forms.Cursor.Position.Y;
						this.Win_CamSetting_Dlg.Left = System.Windows.Forms.Cursor.Position.X;

						this.Win_CamSetting_Dlg.Topmost = true;

						this.Win_CamSetting_Dlg.Visibility = System.Windows.Visibility.Visible;
						this.Win_CamSetting_Dlg.Show();
					}
				}
				else if ( btn.Name.Contains( "Save" ) )
				{
					this._CAM?.SingleGrab();
					this._CAM.SaveIMG( true ).Wait();
				}
			}
			catch
			{
				//System.Windows.MessageBox.Show( ex.Message );
			}
		}

		private void UserControl_IsVisibleChanged( object sender, DependencyPropertyChangedEventArgs e )
		{
			if ( this.Win_CamSetting_Dlg != null )
				this.Win_CamSetting_Dlg.Close();
			if ( this._CAM?.Camera == null ) return;
			if ( this.IsLoaded )
			{
				this.Dispatcher.BeginInvoke( DispatcherPriority.Input,
					new Action( delegate ()
					{
						if ( this._CAM != null )
						{
							this._CAM.Camera.Cal.ShowROI = this.eROItype == ROI.Cal && this.b_EnableROI ? ( bool )this.Chk_ROI.IsChecked : false;
							this._CAM.Camera.Inspect.ShowROI = this.eROItype == ROI.Inspect && this.b_EnableROI ? ( bool )this.Chk_ROI.IsChecked : false;
						}
						this._CAM.UpdateDisplayZoom();
					} ) );
			}
		}

		private void Chk_xHair_Checked( object sender, RoutedEventArgs e )
		{
			this._CAM.Camera.IsShowCrossLine = ( bool )( sender as CheckBox ).IsChecked;
		}

		private void Slider_DragCompleted( object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e )
		{
			this._CAM.UpdateDisplayZoom();
		}

		private void Chk_ROI_Checked( object sender, RoutedEventArgs e )
		{
			if ( this.b_EnableROI )
			{
				this._CAM.Camera.Cal.ShowROI = this.eROItype == ROI.Cal ? ( bool )( sender as CheckBox ).IsChecked : false;
				this._CAM.Camera.Inspect.ShowROI = this.eROItype == ROI.Inspect ? ( bool )( sender as CheckBox ).IsChecked : false;
			}
			else
			{
				this._CAM.Camera.Cal.ShowROI = false;
				this._CAM.Camera.Inspect.ShowROI = false;
			}
		}
	}
}
