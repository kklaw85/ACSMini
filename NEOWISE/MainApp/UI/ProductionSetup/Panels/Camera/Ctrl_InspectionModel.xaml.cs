using B262_Vision_Processing;
using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Camera;
using Microsoft.Win32;
using N_Data_Utilities;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.UI.ProductionSetup.Panels.Camera
{
	/// <summary>
	/// </summary>
	public partial class Ctrl_InspectionModel : PanelBase
	{
		public Ctrl_InspectionModel()
		{
			this.MachineStateLockable = true;
			this.InitializeComponent();
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
					this.LabelName.Content = this._Source?.Name;
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
				b.Source = this._Source?.InspectionResult.PositionOffsetMM;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "X" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.X1.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this._Source?.InspectionResult.PositionOffsetMM;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Y" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.Y1.SetBinding( Label.ContentProperty, b );

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
					if ( ( ErrorMessage = this._Source.SaveMMFToConfigProcess( filename ) ) != "" ) throw new Exception( ErrorMessage );
					this.Display_MMF();
				}
				else if ( Action == "Export" )
				{
					SaveFileDialog FileDialog = new SaveFileDialog();
					FileDialog.Filter = Type == "MET" ? B262_Process.METFilter : B262_Process.MMFFilter;
					if ( FileDialog.ShowDialog() == false ) return;
					var filename = FileDialog.FileName;
					var Ext = Path.GetExtension( filename );
					if ( ( ErrorMessage = this._Source.ExportProcessMMF( filename ) ) != "" ) throw new Exception( ErrorMessage );
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
				Bitmap model_image = null;
				if ( this._Source?.Configuration?.MMFs?.Calibration != null )
				{
					if ( ( ErrorMessage = this._Source?.GetCalBMP( ref model_image ) ) != "" ) throw new Exception( ErrorMessage );
				}

				this.wfHost_DispMMF.BackgroundImage = model_image;
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
				if ( btn == this.BtnInspect )
				{
					var check = this._Source.InspectPosition();
					await check;
				}
				else if ( btn == this.Clear )
				{
					this._Source.InspectionResult.Clear();
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
