using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Camera;
using System;
using System.Linq;
using System.Windows;
using static JptCamera.MatroxDigitizer;

namespace NeoWisePlatform.UI.SystemControls.Panels.Camera
{
	/// <summary>
	/// </summary>
	public partial class Ctrl_CamConfigPnl : PanelBase
	{
		public Ctrl_CamConfigPnl()
		{
			this.MachineStateLockable = true;
			this.InitializeComponent();
			this.Cb_ImageType.ItemsSource = Enum.GetValues( typeof( FileFormat ) ).Cast<FileFormat>();
			this.cmbAngleList.ItemsSource = Enum.GetValues( typeof( ImageRotate ) ).Cast<ImageRotate>();
		}

		private MatroxCamera _Source = null;
		public MatroxCamera Source
		{
			get => this._Source;
			set
			{
				try
				{
					this._Source = value;
					this.DataContext = value;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private void Btn_FilePath_Click( object sender, RoutedEventArgs e )
		{
			this._Source.Configuration.ImageSave.BrowseFolder();
		}
	}
}
