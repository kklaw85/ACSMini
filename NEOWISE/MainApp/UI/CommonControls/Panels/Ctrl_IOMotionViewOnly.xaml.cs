using HiPA.Common;
using HiPA.Common.UControl;
using N_Data_Utilities;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace NeoWisePlatform.UI.CommonControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_IOMotionDisplayPnl.xaml
	/// </summary>
	public partial class Ctrl_IOMotionViewOnly : PanelBase
	{
		public Ctrl_IOMotionViewOnly()
		{
			this.InitializeComponent();
		}

		private IOMotion _Source = null;
		public IOMotion Source
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

		private void OnSetupBinding()
		{
			try
			{
				Binding b = new Binding();
				//b.Source = this._Source?._ResetPos;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Value" );
				//b.Converter = new DioValueToBrushResetIORec();
				//this.ResetPos.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this._Source?._InPos;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Value" );
				b.Converter = new DioValueToBrushIORec();
				this.InPos.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this._Source?._ToPosCylinder;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Value" );
				b.Converter = new DioValueToBrushIORec();
				this.ToPosCylinder.SetBinding( Rectangle.FillProperty, b );

				if ( this._Source?._ToResetCylinder == null )
					this.ResetCylinder.Visibility = Visibility.Collapsed;
				else
				{
					this.ResetCylinder.Visibility = Visibility.Visible;
					b = new Binding();
					b.Source = this._Source?._ToResetCylinder;
					b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
					b.Path = new PropertyPath( "Value" );
					b.Converter = new DioValueToBrushIORec();
					this.ResetCylinder.SetBinding( Rectangle.FillProperty, b );
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
