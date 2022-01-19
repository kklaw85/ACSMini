using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JptCamera
{
	/// <summary>
	/// Interaction logic for CameraSettingsWPF.xaml
	/// </summary>
	public partial class CameraSettingsWPF : Window
	{

		private string TitleWnd = "Camera Settings - ";

		private JptCamera.Camera _camera;

		public CameraSettingsWPF()
		{
			this.InitializeComponent();
		}

		public CameraSettingsWPF( JptCamera.Camera Camera )
		{
			this.InitializeComponent();

			this._camera = Camera;

			string sErr = this.InitialiseCameraSettings();
			if ( sErr.Length > 0 ) this.tB_Comment.AppendText( sErr + "\r\n" );
		}

		private void Btn_CloseSettings_Click( object sender, RoutedEventArgs e )
		{
			this.Close();
		}

		public string InitialiseCameraSettings()
		{
			string sErr = string.Empty;
			try
			{
				if ( this._camera == null )
				{ throw new Exception( "Camera is null" ); }

				sErr = this.UpdateCameraInfoUI();
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.UpdatePixelFormatUI();
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.UpdateExposureUI();
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.UpdateGainUI();
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.UpdateGammaUI();
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.UpdateTriggerModeUI();
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.UpdateTriggerDelayUI();
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.UpdateROIOptionUI();
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - InitialiseCameraSettings error: " + ex.Message );
			}
			return sErr;
		}


		#region Events handling - button events


		private void numericTextBox_PreviewTextInput( object sender, TextCompositionEventArgs e )
		{
			if ( !char.IsDigit( e.Text, e.Text.Length - 1 ) )
				e.Handled = true;
		}

		private void Btn_PixelFormat_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				var _pFormat = ( string )this.cB_PixelFormat.SelectedItem;

				sErr = this.SetPixelFormat( _pFormat );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( _pFormat + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_PixelFormat_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void Btn_Exposure_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				string valueEx = this.tB_Exposure.Text;
				double _exposureValue = 0;
				_exposureValue = Convert.ToDouble( valueEx );

				sErr = this.SetExposure( _exposureValue );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "ExposureTime set to " + _exposureValue.ToString() + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_Exposure_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void Btn_Gain_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				string valueEx = this.tB_Gain.Text;
				int _gainValue = 0;
				_gainValue = Convert.ToInt32( valueEx );

				sErr = this.SetGainValue( _gainValue );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "Gain value set to " + _gainValue.ToString() + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_Gain_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void CB_GammaEnable_Checked( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this.SetGammaEnable( this.cB_GammaEnable.IsChecked == true );
				if ( sErr.Length > 0 ) this.tB_Comment.AppendText( sErr + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - CB_GammaEnable_Checked error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void Btn_Gamma_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				string valueEx = this.tB_Gamma.Text;
				double _gammaValue = 0;
				_gammaValue = Convert.ToDouble( valueEx );

				sErr = this.SetGammaValue( _gammaValue );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "Gamma value set to " + _gammaValue.ToString() + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_Gamma_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void CB_ImageReverse_Checked( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				if ( ( ( CheckBox )sender ).Name.Contains( "ReverseX" ) )
				{ sErr = this.SetReverseX( ( ( CheckBox )sender ).IsChecked == true ); }
				else if ( ( ( CheckBox )sender ).Name.Contains( "ReverseY" ) )
				{ sErr = this.SetReverseY( ( ( CheckBox )sender ).IsChecked == true ); }

				if ( sErr.Length > 0 ) this.tB_Comment.AppendText( sErr + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - CB_ImageReverse_Checked error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void TriggerMode_Checked( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				// ... Get RadioButton reference.
				var rButton = sender as RadioButton;
				string bContent = rButton.Content.ToString();

				sErr = this.SetTriggerMode( bContent );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "TriggerMode changed to " + bContent + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - TriggerMode_Checked error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void Btn_TriggerDelay_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				string valueEx = this.tB_TriggerDelay.Text;
				int _trigDelay = 0;
				_trigDelay = Convert.ToInt32( valueEx );

				sErr = this.SetTriggerDelay( _trigDelay );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "Trigger delay(microseconds) set to " + _trigDelay.ToString() + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_Gain_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void Btn_Width_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				string _sWid = this.tB_Width.Text;
				int _width = 0;
				_width = Convert.ToInt32( _sWid );

				string _sHei = this.tB_Height.Text;
				int _height = 0;
				_height = Convert.ToInt32( _sHei );

				string _sOffX = this.tB_OffsetX.Text;
				int _offX = 0;
				_offX = Convert.ToInt32( _sOffX );

				string _sOffY = this.tB_OffsetY.Text;
				int _offY = 0;
				_offY = Convert.ToInt32( _sOffY );

				sErr = this.ValidateROISettings( _width, _height, _offX, _offY );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.SetROIWidth( _width );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "ROI Width set to " + _width.ToString() + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_Width_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void Btn_Height_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				string _sWid = this.tB_Width.Text;
				int _width = 0;
				_width = Convert.ToInt32( _sWid );

				string _sHei = this.tB_Height.Text;
				int _height = 0;
				_height = Convert.ToInt32( _sHei );

				string _sOffX = this.tB_OffsetX.Text;
				int _offX = 0;
				_offX = Convert.ToInt32( _sOffX );

				string _sOffY = this.tB_OffsetY.Text;
				int _offY = 0;
				_offY = Convert.ToInt32( _sOffY );

				sErr = this.ValidateROISettings( _width, _height, _offX, _offY );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.SetROIHeight( _height );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "ROI Height set to " + _height.ToString() + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_Height_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void Btn_OffsetX_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				string _sWid = this.tB_Width.Text;
				int _width = 0;
				_width = Convert.ToInt32( _sWid );

				string _sHei = this.tB_Height.Text;
				int _height = 0;
				_height = Convert.ToInt32( _sHei );

				string _sOffX = this.tB_OffsetX.Text;
				int _offX = 0;
				_offX = Convert.ToInt32( _sOffX );

				string _sOffY = this.tB_OffsetY.Text;
				int _offY = 0;
				_offY = Convert.ToInt32( _sOffY );

				sErr = this.ValidateROISettings( _width, _height, _offX, _offY );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.SetOffsetX( _offX );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "OffsetX set to " + _offX.ToString() + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_OffsetX_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		private void Btn_OffsetY_Click( object sender, RoutedEventArgs e )
		{
			string sErr = string.Empty;
			try
			{
				string _sWid = this.tB_Width.Text;
				int _width = 0;
				_width = Convert.ToInt32( _sWid );

				string _sHei = this.tB_Height.Text;
				int _height = 0;
				_height = Convert.ToInt32( _sHei );

				string _sOffX = this.tB_OffsetX.Text;
				int _offX = 0;
				_offX = Convert.ToInt32( _sOffX );

				string _sOffY = this.tB_OffsetY.Text;
				int _offY = 0;
				_offY = Convert.ToInt32( _sOffY );

				sErr = this.ValidateROISettings( _width, _height, _offX, _offY );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.SetOffsetY( _offY );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this.tB_Comment.AppendText( "OffsetY set to " + _offY.ToString() + "\r\n" );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - Btn_OffsetY_Click error: " + ex.Message );
				this.tB_Comment.AppendText( sErr + "\r\n" );
			}
		}

		#endregion //Event handling

		#region Camera Settings

		/// <summary>
		/// Set camera pixel format
		/// </summary>
		/// <param name="InputPixelFormat">string: pixel format</param>
		/// <returns></returns>
		public string SetPixelFormat( string InputPixelFormat )
		{
			string sErr = string.Empty;
			try
			{
				var data = ( PixelFormatEnum )Enum.Parse( typeof( PixelFormatEnum ), InputPixelFormat, true );
				this._camera.PixelFormat = data;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - SetPixelFormat error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Validate if the exposure time is within the supported range for this camera
		/// </summary>
		/// <param name="ExposureTime">double: exposure time in us</param>
		/// <returns></returns>
		public string ValidateExposureValue( double ExposureTime )
		{
			string sErr = string.Empty;
			try
			{
				double minExp = 0;
				double maxExp = 50000;

				minExp = this._camera.ExposureMinimum;
				maxExp = this._camera.ExposureMaximum;

				if ( ExposureTime < minExp || ExposureTime > maxExp )
				{
					sErr = string.Format( "Exposure must be within " + minExp.ToString() + " to " + maxExp.ToString() );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - ValidateExposureValue error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Get the current exposure time for this camera
		/// </summary>
		/// <param name="RefExposure">ref double: exposure time in us</param>
		/// <returns></returns>
		public string GetExposure( ref double RefExposure )
		{
			string sErr = string.Empty;
			try
			{
				RefExposure = this._camera.Exposure;
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera GetExposure error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Set camera exposure time in us(microsecond)
		/// </summary>
		/// <param name="ExposureTimeInUS">double: New exposure time in us</param>
		/// <returns></returns>
		public string SetExposure( double ExposureTimeInUS )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this.ValidateExposureValue( ExposureTimeInUS );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this._camera.Exposure = ExposureTimeInUS;
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera SetExposure error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Validate if the gain value is within the supported range for this camera
		/// </summary>
		/// <param name="GainValue">int: Gain</param>
		/// <returns></returns>
		public string ValidateGainValue( int GainValue )
		{
			string sErr = string.Empty;
			try
			{
				int minGain = 0;
				int maxGain = 0;

				minGain = ( int )this._camera.GainMinimum;
				maxGain = ( int )this._camera.GainMaximum;

				if ( GainValue < minGain || GainValue > maxGain )
				{
					sErr = string.Format( "Gain must be within " + minGain.ToString() + " to " + maxGain.ToString() );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - ValidateGainValue error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Read the gain value from camera
		/// </summary>
		/// <param name="RefGain">Int Gain value</param>
		/// <returns></returns>
		public string GetGainValue( ref int RefGain )
		{
			string sErr = string.Empty;
			try
			{
				RefGain = Convert.ToInt32( this._camera.Gain );
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera GetGainValue error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Set the gain value to camera
		/// </summary>
		/// <param name="GainValue">Int: new gain value to be set</param>
		/// <returns></returns>
		public string SetGainValue( int GainValue )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this.ValidateGainValue( GainValue );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this._camera.Gain = GainValue;
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera SetGainValue error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Enable or disable the gamma feature
		/// </summary>
		/// <param name="Enable">Boolean: True for enable and false for disable</param>
		/// <returns></returns>
		public string SetGammaEnable( bool Enable )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.GammaEnable = Enable;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera SetGammaEnable error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Validate the gamma value against the camera supported range
		/// </summary>
		/// <param name="GammaValue">Double: Gamma value</param>
		/// <returns></returns>
		public string ValidateGammaValue( double GammaValue )
		{
			string sErr = string.Empty;
			try
			{
				double minExp = 0;
				double maxExp = 10;

				minExp = this._camera.GammaMinimum;
				maxExp = this._camera.GammaMaximum;

				if ( GammaValue < minExp || GammaValue > maxExp )
				{
					sErr = string.Format( "Gamma must be within " + minExp.ToString() + " to " + maxExp.ToString() );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - ValidateGammaValue error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Set the camera gamma value
		/// </summary>
		/// <param name="GammaValue">Double: New gamma value</param>
		/// <returns></returns>
		public string SetGammaValue( double GammaValue )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this.ValidateGammaValue( GammaValue );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				this._camera.Gamma = GammaValue;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera SetGammaValue error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Get the current camera trigger mode
		/// </summary>
		/// <param name="RefTriggerMode">String: current trigger mode</param>
		/// <returns></returns>
		public string GetTriggerMode( ref string RefTriggerMode )
		{
			string sErr = string.Empty;
			try
			{
				if ( !this._camera.IsCameraConnected )
					return "No camera connected";

				var trigMode = this._camera.TriggerType;

				switch ( trigMode )
				{
					case TriggerTypeEnum.TRIGGER_HW_RISING:
					case TriggerTypeEnum.TRIGGER_HW_FALLING:
						RefTriggerMode = "Hardware";
						break;
					case TriggerTypeEnum.TRIGGER_SOFTWARE:
						RefTriggerMode = "Software";
						break;
					case TriggerTypeEnum.TRIGGER_CONTINUOUS:
					default:
						RefTriggerMode = "Continuous";
						break;
				}

			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - GetTriggerMode error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Set camera trigger mode
		/// </summary>
		/// <param name="sTriggerType">String: Trigger mode</param>
		/// <returns></returns>
		public string SetTriggerMode( string sTriggerType )
		{
			string sErr = string.Empty;
			try
			{
				if ( !this._camera.IsCameraConnected )
				{
					throw new Exception( "No camera connected" );
				}

				TriggerTypeEnum trigModeSelected = TriggerTypeEnum.TRIGGER_OFF;

				if ( sTriggerType.Contains( "Continuous" ) )
				{
					trigModeSelected = TriggerTypeEnum.TRIGGER_CONTINUOUS;
				}
				else if ( sTriggerType.Contains( "Hardware" ) )
				{
					trigModeSelected = TriggerTypeEnum.TRIGGER_HW_RISING;
				}
				else if ( sTriggerType.Contains( "Software" ) )
				{
					trigModeSelected = TriggerTypeEnum.TRIGGER_SOFTWARE;
				}

				sErr = this.SetTriggerModeMil( trigModeSelected );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - SetTriggerMode error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Set camera trigger mode using Matrox Gige system
		/// </summary>
		/// <param name="NewTriggerType">Matrox Trigger Mode</param>
		/// <returns></returns>
		public string SetTriggerModeMil( TriggerTypeEnum NewTriggerType )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.TriggerType = NewTriggerType;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - SetTriggerModeMil error: " + ex.Message );
			}
			return sErr;
		}


		/// <summary>
		/// Validate camera trigger delay against camera accepted ranges
		/// </summary>
		/// <param name="TriggerDelay">Int: Trigger Delay value in us</param>
		/// <returns></returns>
		public string ValidateTriggerDelayValue( int TriggerDelay )
		{
			string sErr = string.Empty;
			try
			{
				int minDelay = 0;
				int maxDelay = 0;

				minDelay = ( int )this._camera.TriggerDelayMin;
				maxDelay = ( int )this._camera.TriggerDelayMax;

				if ( TriggerDelay < minDelay || TriggerDelay > maxDelay )
				{
					sErr = string.Format( "Gain must be within " + minDelay.ToString() + " to " + maxDelay.ToString() );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - ValidateGainValue error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Read current camera trigger delay value
		/// </summary>
		/// <param name="RefTriggerDelay">Int: Trigger delay</param>
		/// <returns></returns>
		public string GetTriggerDelay( ref int RefTriggerDelay )
		{
			string sErr = string.Empty;
			try
			{
				RefTriggerDelay = Convert.ToInt32( this._camera.TriggerDelay );
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera - GetTriggerDelay error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Set new camera trigger delay time in us(microsecond)
		/// </summary>
		/// <param name="TriggerDelayInUS">Int: Trigger delay in us</param>
		/// <returns></returns>
		public string SetTriggerDelay( int TriggerDelayInUS )
		{
			string sErr = string.Empty;
			try
			{
				sErr = this.ValidateTriggerDelayValue( TriggerDelayInUS );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }

				sErr = this.SetTriggerDelayMil( TriggerDelayInUS );
				if ( sErr.Length > 0 ) { throw new Exception( sErr ); }
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera - SetTriggerDelay error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Set camera trigger delay in us(microsecond) using Matrox MIL
		/// </summary>
		/// <param name="TriggerDelayInUS">Int: Trigger delay</param>
		/// <returns></returns>
		public string SetTriggerDelayMil( int TriggerDelayInUS )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.TriggerDelay = TriggerDelayInUS;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - SetTriggerDelayMil error: " + ex.Message );
			}
			return sErr;
		}

		/// <summary>
		/// Set new camera ROI width
		/// </summary>
		/// <param name="WidthValue">Int: ROI Width</param>
		/// <returns></returns>
		public string SetROIWidth( int WidthValue )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.Width = WidthValue;
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera - SetROIWidth error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Set new camera ROI Height
		/// </summary>
		/// <param name="HeightValue">Int: ROI Height</param>
		/// <returns></returns>
		public string SetROIHeight( int HeightValue )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.Height = HeightValue;
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera - SetROIHeight error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Set new camera Offset X value
		/// </summary>
		/// <param name="OffsetXValue">Int: OffsetX</param>
		/// <returns></returns>
		public string SetOffsetX( int OffsetXValue )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.OffsetX = OffsetXValue;
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera - SetOffsetX error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Set new camera Offset Y value
		/// </summary>
		/// <param name="OffsetYValue">Int: OffsetY</param>
		/// <returns></returns>
		public string SetOffsetY( int OffsetYValue )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.OffsetY = OffsetYValue;
			}
			catch ( Exception ex )
			{ sErr = string.Format( "FXRAOICamera - SetOffsetY error: " + ex.Message ); }
			return sErr;
		}

		/// <summary>
		/// Validate the canera ROI settings before implementation
		/// </summary>
		/// <param name="iWidth">Int: New width</param>
		/// <param name="iHeight">Int: New height</param>
		/// <param name="iOffX">Int: New offsetX</param>
		/// <param name="iOffY">Int: New offsetY</param>
		/// <returns></returns>
		public string ValidateROISettings( int iWidth, int iHeight, int iOffX, int iOffY )
		{
			string sErr = string.Empty;
			try
			{
				int _minWidth = 0;
				int _maxWidth = 0;
				int _minHeight = 0;
				int _maxHeight = 0;

				_minWidth = this._camera.WidthMinimum;
				_maxWidth = this._camera.WidthMaximum;
				_minHeight = this._camera.HeightMinimum;
				_maxHeight = this._camera.HeightMaximum;

				if ( iWidth < _minWidth || iWidth > _maxWidth )
				{
					sErr = string.Format( "Width must be within " + _minWidth.ToString() + " to " + _maxWidth.ToString() );
				}

				if ( iHeight < _minHeight || iHeight > _maxHeight )
				{
					sErr = string.Format( "Height must be within " + _minHeight.ToString() + " to " + _maxHeight.ToString() );
				}

				if ( _maxWidth < iWidth )
				{
					sErr = string.Format( "Width + OffsetX exceeded the maximum width " + _maxWidth.ToString() );
				}

				if ( _maxHeight < iHeight )
				{
					sErr = string.Format( "Height + OffsetY exceeded the maximum height " + _maxHeight.ToString() );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "FXRAOICamera - ValidateROISettings error: " + ex.Message );
			}
			return sErr;
		}



		#endregion Camera Settings

		#region Control Logic



		public string SetReverseX( bool NewValue )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.ReverseX = NewValue;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF SetReverseX error: " + ex.Message );
			}
			return sErr;
		}

		public string SetReverseY( bool NewValue )
		{
			string sErr = string.Empty;
			try
			{
				this._camera.ReverseY = NewValue;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF SetReverseY error: " + ex.Message );
			}
			return sErr;
		}



		/// <summary>
		/// Return list of strings of camera pixel format options.
		/// </summary>
		public IEnumerable<string> GetPixelFormatEnum
		{
			get
			{
				return Enum.GetNames( typeof( PixelFormatEnum ) );
			}
		}

		#endregion //Control Logic


		#region Update UI

		public string UpdateCameraInfoUI()
		{
			string sErr = string.Empty;
			try
			{
				string serialNo = string.Empty;
				string camBrand = string.Empty;
				string camModel = string.Empty;
				string camUserID = string.Empty;

				serialNo = this._camera.CameraSerialName;
				camBrand = this._camera.CameraBrand;
				camModel = this._camera.CameraModel;
				camUserID = this._camera.CameraUserID;

				this.Title = this.TitleWnd + serialNo;

				this.tB_CameraBrand.Text = camBrand;
				this.tB_CameraModel.Text = camModel;
				this.tB_CameraUserID.Text = camUserID;
				this.tB_CameraSerialNumber.Text = serialNo;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - UpdateCameraInfoUI error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}



		public string UpdatePixelFormatUI()
		{
			string sErr = string.Empty;
			try
			{
				string sRes = string.Empty;

				var colorModeEnum = this.GetPixelFormatEnum;
				this.cB_PixelFormat.ItemsSource = colorModeEnum;

				var getPixelFormat = this._camera.PixelFormat;
				sRes = getPixelFormat.ToString();

				this.cB_PixelFormat.Text = sRes;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - UpdatePixelFormatUI error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string UpdateExposureUI()
		{
			string sErr = string.Empty;
			try
			{
				double exposure = this._camera.Exposure;

				this.tB_Exposure.Text = exposure.ToString();
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - UpdateExposureUI error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string UpdateGainUI()
		{
			string sErr = string.Empty;
			try
			{
				double gain = this._camera.Gain;

				this.tB_Gain.Text = gain.ToString();
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - UpdateGainUI error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string UpdateGammaUI()
		{
			string sErr = string.Empty;
			try
			{
				bool gammaEnabled = false;
				double gamma = 0.0;

				gammaEnabled = this._camera.GammaEnable;

				gamma = this._camera.Gamma;

				this.tB_Gamma.Text = gamma.ToString( "F2" );

				if ( gammaEnabled )
				{
					this.btn_Gamma.IsEnabled = true;
					this.cB_GammaEnable.IsChecked = true;
				}
				else
				{
					this.btn_Gamma.IsEnabled = false;
					this.cB_GammaEnable.IsChecked = false;
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - UpdateGammaUI error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}


		public string UpdateTriggerModeUI()
		{
			string sErr = string.Empty;
			try
			{
				string sTrigMode = "Unknown";

				sErr = this.GetTriggerMode( ref sTrigMode );
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				if ( sTrigMode.Contains( "Continuous" ) )
				{
					this.rb_Continuous.IsChecked = true;
				}
				else if ( sTrigMode.Contains( "Hardware" ) )
				{
					this.rb_HardTrigger.IsChecked = true;
				}
				else if ( sTrigMode.Contains( "Software" ) )
				{
					this.rb_SoftTrigger.IsChecked = true;
				}

			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - UpdateGammaUI error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string UpdateTriggerDelayUI()
		{
			string sErr = string.Empty;
			try
			{
				int trigDelay = 0;

				sErr = this.GetTriggerDelay( ref trigDelay );
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				this.tB_TriggerDelay.Text = trigDelay.ToString();
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - UpdateTriggerDelayUI error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string UpdateROIOptionUI()
		{
			string sErr = string.Empty;
			try
			{
				int iWidth = 0;
				int iHeight = 0;
				int iOffsetX = 0;
				int iOffsetY = 0;

				iWidth = this._camera.Width;
				iHeight = this._camera.Height;
				iOffsetX = this._camera.OffsetX;
				iOffsetY = this._camera.OffsetY;


				this.tB_Width.Text = iWidth.ToString();
				this.tB_Height.Text = iHeight.ToString();
				this.tB_OffsetX.Text = iOffsetX.ToString();
				this.tB_OffsetY.Text = iOffsetY.ToString();
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "CameraSettingsWPF - UpdateTriggerDelayUI error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}



		#endregion //Update UI


	}
}
