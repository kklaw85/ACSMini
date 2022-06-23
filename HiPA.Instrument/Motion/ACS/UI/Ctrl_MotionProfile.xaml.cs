using HiPA.Common.UControl;

namespace HiPA.Instrument.Motion.ACS.UI
{
	/// <summary>
	/// Interaction logic for Ctrl_MotionProfile.xaml
	/// </summary>
	public partial class Ctrl_MotionProfile : PanelBase
	{
		public Ctrl_MotionProfile()
		{
			this.InitializeComponent();
			this.BindLockUI( this );
		}

		public string MotionProfileTitle
		{
			set { this.Lbl_MotionProfileTitle.Content = value; }
		}
	}
}
