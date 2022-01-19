using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.SystemControls.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_ManufacturerBypassOption.xaml
	/// </summary>
	public partial class Ctrl_ManufacturerBypassOption : PageBase
	{
		public Ctrl_ManufacturerBypassOption()
		{
			#region Panel Lockable declaration
			this.MachineStateLockable = false;
			this.PrivilegeLockability = HiPA.Common.Forms.UILockability.LockableNotConfigurable;
			this.MinRead = AccessLevel.Manufacturer;
			this.MinWrite = AccessLevel.Manufacturer;
			#endregion
			this.InitializeComponent();
			this.DataContext = ( Constructor.GetInstance().Equipment as MTEquipment ).MachineMisc?.Configuration.ByPassConfig;
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
	}
}
