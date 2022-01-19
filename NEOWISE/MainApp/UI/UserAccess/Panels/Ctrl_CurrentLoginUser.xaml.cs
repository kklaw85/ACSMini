using HiPA.Common;
using HiPA.Common.UControl;
using N_Data_Utilities;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.UserAccess.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_CurrentLoginUser.xaml
	/// </summary>
	public partial class Ctrl_CurrentLoginUser : PanelBase
	{
		#region PARAMETERS
		private bool _IsMainDisplay = false;
		public bool IsMainDisplay
		{
			get => this._IsMainDisplay;
			set => this.Set( ref this._IsMainDisplay, value, "IsMainDisplay" );
		}
		#endregion

		public Ctrl_CurrentLoginUser()
		{
			#region Panel Lockable declaration
			this.PrivilegeLockable = false;
			this.MachineStateLockable = false;
			#endregion
			this.InitializeComponent();
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.OnSetupBinding();
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
				#region GUI VISIBILITYY
				Binding b = new Binding() { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsMainDisplay" ), Converter = new BooleanToVisibilityConverter() };
				this.Sp_MainDisplay.SetBinding( StackPanel.VisibilityProperty, b );

				b = new Binding() { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsMainDisplay" ), Converter = new BoolToVisibilityReverse() };
				this.Sp_PageDisplay.SetBinding( StackPanel.VisibilityProperty, b );
				#endregion
				#region LOGIN USER INFO
				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "LoginUserID" ) };
				this.Lbl_UserID1.SetBinding( Label.ContentProperty, b );
				this.Lbl_UserID2.SetBinding( Label.ContentProperty, b );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "LoginUserLevel" ) };
				this.Lbl_UserLevel1.SetBinding( Label.ContentProperty, b );
				this.Lbl_UserLevel2.SetBinding( Label.ContentProperty, b );
				#endregion
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
