using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.UControl;
using HiPA.Instrument.Camera;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.SystemControls.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_InstrumentOfCamera : PageBase
	{
		public bool isMain { get; set; } = false;
		public Ctrl_InstrumentOfCamera()
		{
			#region Panel Lockable declaration
			this.MinRead = AccessLevel.Operator;
			this.MinWrite = AccessLevel.Manufacturer;
			#endregion
			this.InitializeComponent();
			#region UI Binding
			this.BindLockUI( this.Grid_CamLst );
			this.BindLockUI( this.CamConfig );
			this.BindLockUI( this.CalConfig );
			#endregion
		}
		protected override void RecipeChanged( object sender, TextChangedEventArgs e )
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private void OnSetupBinding()
		{
			try
			{

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._CAM, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		MatroxCamera _CAM = null;
		private InstrumentBase Instrument
		{
			get => this._CAM;
			set
			{
				try
				{
					if ( value == null ) return;
					this._CAM = value as MatroxCamera;
					this.InitBar.Instrument = value;
					this.Displays.Instrument = value;
					this.CamConfig.Source = value as MatroxCamera;
					this.CalConfig.Source = value as MatroxCamera;
					this.OnSetupBinding();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this._CAM, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				if ( !this.IsLoaded ) return;
				this.Displays.Controllable = false;
				foreach ( var cam in ( CameraList[] )Enum.GetValues( typeof( CameraList ) ) )
				{
					var camObj = Constructor.GetInstance().GetInstrument( cam.ToString(), typeof( MatroxCameraConfiguration ) ) as MatroxCamera;
					if ( camObj == null ) continue;
					if ( !camObj.ValidVariant() ) continue;
					this.cmbCameraList.AddItem( cam.ToString(), camObj );
				}
				this.cmbCameraList.SelectedIndex = 0;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._CAM, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void cmbCameraList_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			try
			{
				this.Instrument = this.cmbCameraList.SelectedValue<InstrumentBase>();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._CAM, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void MetroTabControl_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			if ( this.CamCfg.SelectedIndex == 1 )
				this.Displays.ROIType = ROI.Cal;
			else
				this.Displays.ROIType = ROI.None;
		}
	}
}
