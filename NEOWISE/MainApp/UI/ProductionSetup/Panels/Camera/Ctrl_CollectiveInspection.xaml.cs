using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.ProductionSetup.Panels.Camera
{
	/// <summary>
	/// </summary>
	public partial class Ctrl_CollectiveInspection : PanelBase
	{
		public Ctrl_CollectiveInspection()
		{
			this.MachineStateLockable = true;
			this.InitializeComponent();
		}

		private StageModule _Source = null;
		public StageModule Source
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
				this.Insp.Source = this._Source?.AutorunInfo.InspectionRes;
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

		private async void BtnClick( object sender, RoutedEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				var sErr = string.Empty;
				if ( btn == this.BtnInspect )
				{
					var snap = this._Source?.SnapShot();
					await snap;
					if ( snap.Result.EClass != ErrorClass.OK ) return;
					var check = this._Source?.VisionCheck();
					await check;
				}
				else if ( btn == this.Clear )
				{
					this._Source?.AutorunInfo?.InspectionRes.Clear();
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
