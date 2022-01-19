using HiPA.Common.UControl;

namespace NeoWisePlatform.UI.ProductionSetup.Panels.Camera
{
	/// <summary>
	/// </summary>
	public partial class Ctrl_SingleInspectionStatus : PanelBase
	{
		public Ctrl_SingleInspectionStatus()
		{
			this.MachineStateLockable = true;
			this.InitializeComponent();
		}
	}
}
