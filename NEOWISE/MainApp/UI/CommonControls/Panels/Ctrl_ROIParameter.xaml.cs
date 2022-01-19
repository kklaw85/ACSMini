using HiPA.Common;
using HiPA.Common.UControl;
using JptCamera;
using System;
using System.ComponentModel;
using System.Windows;

namespace NeoWisePlatform.UI.CommonControls.Panels
{
	public partial class Ctrl_ROIParameter : PanelBase
	{
		public Ctrl_ROIParameter()
		{
			this.InitializeComponent();
		}

		private ROIHandler _ROIHandler = null;
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ROIHandler ROIHandler
		{
			get => this._ROIHandler;
			set
			{
				try
				{
					this._ROIHandler = value;
					this.DataContext = value;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{ }
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void Button_Click( object sender, RoutedEventArgs e )
		{
			//do log
			try
			{
				this._ROIHandler?.ResetROI();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
	}
}
