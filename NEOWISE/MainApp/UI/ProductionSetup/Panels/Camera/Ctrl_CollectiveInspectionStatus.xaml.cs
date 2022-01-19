using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;

namespace NeoWisePlatform.UI.ProductionSetup.Panels.Camera
{
	/// <summary>
	/// </summary>
	public partial class Ctrl_CollectiveInspectionStatus : PanelBase
	{
		public Ctrl_CollectiveInspectionStatus()
		{
			this.MachineStateLockable = true;
			this.InitializeComponent();
		}

		public InspectionRes Source
		{
			set
			{
				try
				{
					this.DataContext = value;
					this.Fov1.DataContext = ( this.DataContext as InspectionRes )?.Fov1;
					this.Fov2.DataContext = ( this.DataContext as InspectionRes )?.Fov2;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}
	}
}
