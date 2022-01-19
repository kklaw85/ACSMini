using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;

namespace NeoWisePlatform.UI.CommonControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_IOMotionDisplayPnl.xaml
	/// </summary>
	public partial class Ctrl_IOMotionViewOnlyPnl : PanelBase
	{
		public Ctrl_IOMotionViewOnlyPnl()
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
					this.IOsStatus.Source = value;
					this.LblAxisName.Content = $"{value?.Name}";
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}
	}
}
