using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.Production.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_Production : PageBase
	{
		private MTEquipment Eq = null;
		public Ctrl_Production()
		{
			this.PrivilegeLockability = HiPA.Common.Forms.UILockability.LockableNotConfigurable;
			this.MinRead = AccessLevel.Guest;
			this.MinWrite = AccessLevel.Operator;
			this.InitializeComponent();
		}
		protected override void RecipeChanged( object sender, TextChangedEventArgs e )
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private void OnSetupBinding()
		{
			try
			{

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				if ( !this.IsLoaded ) return;
				this.Eq = Constructor.GetInstance().Equipment as MTEquipment;
				this.StageInsp.Source = this.Eq?.Stage?.AutorunInfo?.InspectionRes;
				this.PNPInsp.Source = this.Eq?.PNP?.AutorunInfo?.InspectionRes;
				this.FOV1.Instrument = this.Eq.Stage.Fov1;
				this.FOV2.Instrument = this.Eq.Stage.Fov2;
				this.FOV1.Controllable = false;
				this.FOV2.Controllable = false;
				this.OnSetupBinding();
				#region Lock UI Binding
				this.BindLockUI( this.StageInsp );
				this.BindLockUI( this.PNPInsp );
				#endregion
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
