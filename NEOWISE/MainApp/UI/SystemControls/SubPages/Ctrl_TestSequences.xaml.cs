using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.SystemControls.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_ManufacturerBypassOption.xaml
	/// </summary>
	public partial class Ctrl_TestSequences : PageBase
	{
		public Ctrl_TestSequences()
		{
			#region Panel Lockable declaration
			this.MachineStateLockable = false;
			this.PrivilegeLockability = HiPA.Common.Forms.UILockability.LockableNotConfigurable;
			this.MinRead = AccessLevel.Manufacturer;
			this.MinWrite = AccessLevel.Manufacturer;
			#endregion
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
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		MTEquipment Eq = null;
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				#region Lock UI Binding
				this.BindLockUI( this.StkPnlCtrlLst );
				#endregion
				this.Eq = Constructor.GetInstance().Equipment as MTEquipment;
				this.DataContext = this.Eq;
				this.PNPOptions.ItemsSource = this.Eq.DryrunBypass.DryRunOp;
				this.OnSetupBinding();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}





		private async void Button_Click( object sender, RoutedEventArgs e )
		{
			var btn = sender as Button;
			try
			{
				if ( btn == this.GRRTest )
				{
					await this.Eq.GRR();
				}
				else if ( btn == this.Dryrun )
				{
					await this.Eq.Dryrun();
				}
			}
			catch ( Exception ex )
			{
				this.Eq.StopTestSeq();
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private void Stop_Click( object sender, RoutedEventArgs e )
		{
			var btn = sender as Button;
			try
			{
				this.Eq.StopTestSeq();
			}
			catch ( Exception ex )
			{
				this.Eq.StopTestSeq();
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
	}
}
