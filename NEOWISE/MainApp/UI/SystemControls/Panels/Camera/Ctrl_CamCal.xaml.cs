using B262_Vision_Processing;
using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Camera;
using MahApps.Metro.Controls;
using Matrox.MatroxImagingLibrary;
using Microsoft.Win32;
using N_Data_Utilities;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace NeoWisePlatform.UI.SystemControls.Panels.Camera
{
	/// <summary>
	/// </summary>
	public partial class Ctrl_CamCal : PanelBase
	{
		public Ctrl_CamCal()
		{
			this.MachineStateLockable = true;
			this.InitializeComponent();
			this.CalDir.ItemsSource = Enum.GetValues( typeof( CalibrationDirection ) ).Cast<CalibrationDirection>();
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
					this.OnSetupBinding();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private void OnSetupBinding()
		{
			try
			{
				var b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM.ScalePixperMM;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "X" );
				this.XPixMM.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM.ScalePixperMM;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Y" );
				this.YPixMM.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM.ActualLength;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "X" );
				this.XMM.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM.ActualLength;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Y" );
				this.YMM.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM.First;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "X" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.X1.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM.First;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Y" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.Y1.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM.Second;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "X" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.X2.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM.Second;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Y" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.Y2.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration.ScalePixperMM;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Direction" );
				this.CalDir.SetBinding( ComboBox.SelectedItemProperty, b );

				this.TeachROI.ROIHandler = this._Source?.Camera?.Cal;

				this.Display_MMF();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void Find_Click( object sender, RoutedEventArgs e )
		{
			try
			{

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void Btn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				string ErrorMessage = string.Empty;
				var btn = sender as Button;
				var Tags = btn.Tag.ToString();
				var Tag = Tags.Split( ',' );
				var Action = Tag[ 0 ];
				var Type = Tag[ 1 ];
				var MMFs = this._Source?.Configuration?.MMFs;
				var vision = this._Source.Vision;
				if ( MMFs == null ) return;
				if ( Action == "Import" )
				{
					OpenFileDialog FileDialog = new OpenFileDialog();
					FileDialog.Filter = Type == "MET" ? B262_Process.METFilter : B262_Process.MMFFilter;
					if ( FileDialog.ShowDialog() == false ) return;
					var filename = FileDialog.FileName;
					var Ext = Path.GetExtension( filename );
					if ( ( ErrorMessage = this._Source.SaveMMFToConfigCal( filename ) ) != "" ) throw new Exception( ErrorMessage );
					this.Display_MMF();
				}
				else if ( Action == "Export" )
				{
					SaveFileDialog FileDialog = new SaveFileDialog();
					FileDialog.Filter = Type == "MET" ? B262_Process.METFilter : B262_Process.MMFFilter;
					if ( FileDialog.ShowDialog() == false ) return;
					var filename = FileDialog.FileName;
					var Ext = Path.GetExtension( filename );
					if ( ( ErrorMessage = this._Source.ExportCalMMF( filename ) ) != "" ) throw new Exception( ErrorMessage );
				}
				else if ( Action == "Capture" )
				{
					if ( ( ErrorMessage = this._Source.SingleShotToConfigCal() ) != "" ) throw new Exception( ErrorMessage );
					this.Display_MMF();
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void Display_MMF()
		{
			try
			{
				string ErrorMessage = string.Empty;
				MIL_ID model_image = MIL.M_NULL;
				if ( this._Source?.Configuration?.MMFs?.Calibration != null )
				{
					if ( ( ErrorMessage = this._Source?.GetCalBMP( ref model_image ) ) != "" ) throw new Exception( ErrorMessage );
				}

				this.DisplayMMF.DisplayBuffer( model_image );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}


		private async void BtnClick( object sender, RoutedEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				var sErr = string.Empty;
				if ( btn == this.Find1 )
				{
					var check = this._Source.CheckCalPosition();
					await check;
					if ( check.Result.EClass == ErrorClass.OK ) this._Source.Configuration.ScalePixperMM.First.Copy( check.Result.PointRes );
				}
				else if ( btn == this.Find2 )
				{
					var check = this._Source.CheckCalPosition();
					await check;
					if ( check.Result.EClass == ErrorClass.OK ) this._Source.Configuration.ScalePixperMM.Second.Copy( check.Result.PointRes );
				}
				else if ( btn == this.Calculate )
					if ( ( sErr = this._Source.Configuration.ScalePixperMM.CalculateScale() ) != string.Empty ) throw new Exception( sErr );

				//this.Display_MMF();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}




		}
	}
}
