using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.UControl;
using HiPA.Instrument.Camera;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.CommonControls
{
	/// <summary>
	/// </summary>
	public partial class Ctrl_DisplayPanels : PanelBase
	{
		public bool isMain { get; set; } = false;

		bool bIsLoaded = false;
		bool bEnableCameraCameraSelection = true;
		public bool EnableCameraSelection
		{
			get => this.bEnableCameraCameraSelection;
			set
			{
				this.bEnableCameraCameraSelection = value;
				this.CamListCmb.IsEnabled = value;
			}

		}
		private ROI eROI = ROI.None;
		public ROI ROIType
		{
			get => this.eROI;
			set
			{
				this.eROI = value;
				foreach ( DisplayHandler display in this.Displays.Values )
				{
					( display as DisplayHandler ).ROItype = this.eROI;
				}
				this.UpdateROIEnable();
			}
		}
		public Ctrl_DisplayPanels()
		{
			this.InitializeComponent();
		}
		private void AssignCamToDisplay( string CamName )
		{
			try
			{
				if ( !this.Displays.ContainsKey( CamName ) ) return;
				var control = this.Displays[ CamName ];
				if ( control == null ) return;
				control.Instrument = this.Cameras[ CamName ];
				var sErr = this.Cameras[ CamName ]?.SetDisplay( control );
				if ( sErr != string.Empty )
					throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}


		private void OnSetupBinding()
		{
			try
			{
				//foreach (var camera in this.Cameras.Values)
				{
					this.AssignCamToDisplay( this._CAM?.Name );
				}
				this.GetDisplayControl( this._CAM?.Name );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		MatroxCamera _CAM = null;
		public InstrumentBase Instrument
		{
			get => this._CAM;
			set
			{
				try
				{
					if ( value == null ) return;
					this.AddCams();
					this._CAM = value as MatroxCamera;
					this.cmbCameraList.SelectByValue( this._CAM );
					this.OnSetupBinding();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}
		[Browsable( true ), DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
		public bool Controllable
		{
			set => this.CamListCmb.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
		}
		private bool b_EnableROI = false;
		public bool EnableROI
		{
			get => this.b_EnableROI;
			set => this.b_EnableROI = value;
		}
		private void UpdateROIEnable()
		{
			try
			{
				foreach ( var display in this.Displays.Values )
				{
					display.EnableROI = this.eROI != ROI.None ? this.b_EnableROI : false;
				}
			}
			catch ( Exception ex )
			{
				MessageBox.Show( ex.Message );
			}
		}

		private DisplayHandler GetDisplayControl( string cameraName )
		{
			try
			{
				if ( cameraName == null )
					return null;
				var displays = this.Displays.Values.Where( x => x.Visibility == Visibility.Visible );
				foreach ( var display in displays )
				{
					display.Visibility = Visibility.Collapsed;
				}
				if ( !this.Displays.ContainsKey( cameraName ) ) return null;
				this.Displays[ cameraName ].Visibility = Visibility.Visible;
				return this.Displays[ cameraName ];
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
			return null;
		}
		Dictionary<string, DisplayHandler> Displays { get; set; } = new Dictionary<string, DisplayHandler>();
		Dictionary<string, MatroxCamera> Cameras { get; set; } = new Dictionary<string, MatroxCamera>();
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				if ( this.bIsLoaded ) return;
				if ( !this.IsLoaded ) return;
				this.AddCams();
				if ( this.CamListCmb.Visibility == Visibility.Visible )
					this.cmbCameraList.SelectedIndex = 0;
				this.SetDisplayMargin();
				this.UpdateROIEnable();
				this.bIsLoaded = true;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}


		private bool CamAdded = false;
		private void AddCams()
		{
			try
			{
				if ( this.CamAdded ) return;
				this.Displays.Clear();
				foreach ( var cam in Enum.GetValues( typeof( CameraList ) ) )
				{
					var camObj = Constructor.GetInstance().GetInstrument( cam.ToString(), typeof( MatroxCameraConfiguration ) ) as MatroxCamera;
					if ( camObj == null ) continue;
					if ( !camObj.ValidVariant() ) continue;
					this.cmbCameraList.AddItem( cam.ToString(), camObj );
					var display = new DisplayHandler();
					display.HorizontalAlignment = HorizontalAlignment.Stretch;
					display.VerticalAlignment = VerticalAlignment.Stretch;
					this.Cameras[ cam.ToString() ] = camObj;
					this.Displays[ cam.ToString() ] = display;
					this.StkPnlDisplays.Children.Add( display );
				}
				this.CamAdded = true;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}


		protected Task<string> OnInitialize()
		{
			return Task.Run( () =>
			{
				var result = string.Empty;
				if ( ( result = this._CAM.Initialize().Result ) != string.Empty ) return result;
				return result;
			} );
		}
		private void SetDisplayMargin()
		{
			try
			{
				foreach ( var display in this.Displays.Values )
				{
					display.SetDisplayMargin = this.Margin;
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
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
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void PageBase_IsVisibleChanged( object sender, DependencyPropertyChangedEventArgs e )
		{
			try
			{
				if ( ( bool )e.NewValue && this.Instrument != null && this.IsLoaded )
					this.OnSetupBinding();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
