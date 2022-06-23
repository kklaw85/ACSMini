using HiPA.Common;
using HiPA.Common.UControl;
using System;
using System.Windows;

namespace HiPA.Instrument.Motion.ACS.UI
{
	/// <summary>
	/// Interaction logic for Ctrl_InstrumentOfAxisBoard.xaml
	/// </summary>
	public partial class Ctrl_InstrumentOfAxisBoard : PanelBase
	{
		public Ctrl_InstrumentOfAxisBoard()
		{
			try
			{
				this.InitializeComponent();
				this.BindLockUI( this );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}

		}
		private ACSMotionBoard o_Source = null;
		public ACSMotionBoard Source
		{
			get => this.o_Source;
			set
			{
				try
				{
					this.o_Source = value;
					this.InitBar.Instrument = value;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}
	}
}
