using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.UControl;
using N_Data_Utilities;
using NeoWisePlatform.Module;
using NeoWisePlatform.Recipe;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace NeoWisePlatform.UserAccess.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_User_Access.xaml
	/// </summary>
	public partial class Ctrl_User_Access : PageBase
	{
		#region PARAMETERS
		public ObservableCollection<UserAccessConfig> UserPrivilege { get; set; }
		public ObservableCollection<PrivilegeType> AccessRightList { get; set; }
		private UserMgtConfig SelectedUser { get; set; }
		#endregion

		public Ctrl_User_Access()
		{
			#region Panel Lockable declaration
			this.PrivilegeLockability = UILockability.Nonlockable;
			this.MachineStateLockable = false;
			#endregion
			Equipment.UserMgt.AccessLevelChanged += this.UserMgt_AccessLevelChanged;
			this.InitializeComponent();
		}

		private void UserMgt_AccessLevelChanged( object sender, LoginUserMgt.AccessLevelChangeEventArgs e )
		{
			if ( !this.IsLoaded ) return;
			this.Cb_Create_New_GroupID.ItemsSource = Enum.GetValues( typeof( AccessLevel ) ).Cast<AccessLevel>().ToList().Where( x => x <= MTEquipment.UserMgt.LoginUserLevel );
			this.Cb_Create_New_GroupID.SelectedItem = AccessLevel.Operator;
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
				this.UserPrivilege = new ObservableCollection<UserAccessConfig>();
				this.AccessRightList = new ObservableCollection<PrivilegeType>();
				this.DataContext = this;


				foreach ( PrivilegeType pvType in Enum.GetValues( typeof( PrivilegeType ) ) )
				{
					this.AccessRightList.Add( pvType );
				}

				#region GUI VISIBILITYY
				Binding b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsChangePasswordContent" ), Converter = new BooleanToVisibilityConverter() };
				this.Sp_ChangePasswordContent.SetBinding( StackPanel.VisibilityProperty, b );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsAddUserContent" ), Converter = new BooleanToVisibilityConverter() };
				this.Sp_AddUserContent.SetBinding( StackPanel.VisibilityProperty, b );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsAssignPrivilegeContent" ), Converter = new BooleanToVisibilityConverter() };
				this.Sp_EditDeleteContent.SetBinding( StackPanel.VisibilityProperty, b );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsUserLogin" ), Converter = new BooleanToVisibilityConverter() };
				this.Sp_CurrentAccessUser.SetBinding( StackPanel.VisibilityProperty, b );
				this.Sp_ChangePassword.SetBinding( StackPanel.VisibilityProperty, b );
				this.Sp_Logout.SetBinding( StackPanel.VisibilityProperty, b );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsUserLogin" ), Converter = new BoolToVisibilityReverse() };
				this.Sp_Login.SetBinding( StackPanel.VisibilityProperty, b );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsLoginFail" ), Converter = new BooleanToVisibilityConverter() };
				this.Lbl_Log_In_Status.SetBinding( Label.VisibilityProperty, b );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsAllowAddUser" ), Converter = new BooleanToVisibilityConverter() };
				this.Sp_AddUser.SetBinding( StackPanel.VisibilityProperty, b );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsAllowAssignPrivilege" ), Converter = new BooleanToVisibilityConverter() };
				this.Sp_EditDelUser.SetBinding( StackPanel.VisibilityProperty, b );
				#endregion

				#region LOGIN USER INFO
				//b = new Binding() { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "CurrentLoginUserID" ) };
				//this.Lbl_UserID.SetBinding( Label.ContentProperty, b );

				//b = new Binding() { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "CurrentLoginUserLevel" ) };
				//this.Lbl_UserLevel.SetBinding( Label.ContentProperty, b );
				#endregion

				//this.BindLockMachineState( this.Stk );

				this.CB_UserList.ItemsSource = MTEquipment.UserMgt.GetEditableUserList(); //Refresh Userlist
				this.CB_UserList.SelectedIndex = 0;
				this.Btn_Delete.IsEnabled = MTEquipment.UserMgt.GetEditableUserList().Count() == 0 ? false : true;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		#region BUTTON EVENT
		private void Button_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				string result = "";
				Button btn = ( Button )sender;
				switch ( btn.Name )
				{
					case "Btn_Login":
					{
						result = MTEquipment.UserMgt.Login( this.Txt_UserID.Text, this.PB_Password.Password );
						if ( result != "" )
							this.Lbl_Log_In_Status.Content = result;
						else
						{
							this.CB_UserList.SelectedItem = MTEquipment.UserMgt.CurrentLoginUser.UserID;
							this.Txt_UserID.Text = this.PB_Password.Password = "";
							this.CB_UserList.ItemsSource = MTEquipment.UserMgt.GetEditableUserList();
							this.Cb_Create_New_GroupID.ItemsSource = null;
							this.Cb_Create_New_GroupID.ItemsSource = Enum.GetValues( typeof( AccessLevel ) ).Cast<AccessLevel>().ToList().Where( x => x <= MTEquipment.UserMgt.LoginUserLevel );
							this.Cb_Create_New_GroupID.SelectedItem = AccessLevel.Operator;
						}
					}
					break;

					case "Btn_ChangePassword":
					{
						string OldPassword = this.PB_Old_Password.Password.ToString();
						string NewPassword = this.PB_New_Password.Password.ToString();
						string ComfirmPassword = this.PB_Comfirm_Password.Password.ToString();
						result = string.Empty;
						if ( OldPassword == string.Empty && NewPassword == string.Empty && ComfirmPassword == string.Empty )
							result = "Please input all information";
						else
							result = MTEquipment.UserMgt.EditUserInformation( MTEquipment.UserMgt.CurrentLoginUser.UserID, NewPassword, OldPassword );

						if ( result != string.Empty )
						{
							this.Lbl_Change_Status.Visibility = Visibility.Visible;
							this.Lbl_Change_Status.Content = result;
						}
						else
						{
							MessageBox.Show( "Change Password Successful!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information );
							this.ResetPasswordField();
						}
					}
					break;
					case "Btn_CancelChangePassword":
					{
						MTEquipment.UserMgt.IsChangePasswordContent = false;
						this.ResetPasswordField();
					}
					break;

					case "Btn_CreateUser":
					{
						string UserID = this.PB_Create_New_User.Text.ToString();
						string UserPW = this.PB_Create_New_UserPW.Password.ToString();
						result = "";
						if ( string.IsNullOrEmpty( UserID ) || string.IsNullOrEmpty( UserPW ) )
							result = "Please input all information";
						else
							result = MTEquipment.UserMgt.CreateUser( UserID, UserPW, ( AccessLevel )this.Cb_Create_New_GroupID.SelectedItem );

						if ( result != string.Empty )
						{
							this.Lbl_CreateUser_Change_Status.Visibility = Visibility.Visible;
							this.Lbl_CreateUser_Change_Status.Content = result;
						}
						else
						{
							MessageBox.Show( "Add User Successful!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information );
							this.ResetCreateUserField();
							this.CB_UserList.ItemsSource = MTEquipment.UserMgt.GetEditableUserList();   //Refresh CB_UserList
						}
					}
					break;

					case "Btn_CreateCancel":
					{
						MTEquipment.UserMgt.IsAddUserContent = false;
						this.ResetCreateUserField();
					}
					break;

					case "Btn_ApplyPrivilege":
					{
						result = MTEquipment.UserMgt.UpdateUserInfo( this.SelectedUser );
					}
					break;
					case "Btn_Delete":
					{
						result = MTEquipment.UserMgt.DeleteUser( this.SelectedUser );
						this.CB_UserList.ItemsSource = MTEquipment.UserMgt.GetEditableUserList();
						this.CB_UserList.SelectedIndex = 0;
					}
					break;
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		private void Lbl_MouseUp( object sender, MouseButtonEventArgs e )
		{
			try
			{
				Label lbl = ( Label )sender;
				switch ( lbl.Name )
				{
					case "Lbl_ChangePassword":
						MTEquipment.UserMgt.IsChangePasswordContent = !MTEquipment.UserMgt.IsChangePasswordContent;
						if ( MTEquipment.UserMgt.IsChangePasswordContent )
							MTEquipment.UserMgt.IsAssignPrivilegeContent = MTEquipment.UserMgt.IsAddUserContent = false;
						break;

					case "Lbl_Logout":
						MessageBoxResult msgBoxRslt = MessageBox.Show( "Are you sure to logout?", "LOGOUT", MessageBoxButton.YesNo, MessageBoxImage.Question );
						if ( msgBoxRslt == MessageBoxResult.Yes )
						{
							if ( MessageBox.Show( "Do you want to save Recipe File before logout?", "Save Recipe File", MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.Yes )
							{
								var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe();
								if ( recipe == null ) throw new Exception( $"^{( int )RunErrors.ERR_RecipeObjNull }^" );
								Recipes.HandlerRecipes()?.Save( recipe );
							}
							MTEquipment.UserMgt.Logout();
							//MessageBox.Show( "Logout Successful!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information );

						}
						break;

					case "Lbl_AddUser":
						MTEquipment.UserMgt.IsAddUserContent = !MTEquipment.UserMgt.IsAddUserContent;
						if ( MTEquipment.UserMgt.IsAddUserContent )
							MTEquipment.UserMgt.IsChangePasswordContent = false;
						break;

					case "Lbl_EditDelete":
						MTEquipment.UserMgt.IsAssignPrivilegeContent = !MTEquipment.UserMgt.IsAssignPrivilegeContent;
						if ( MTEquipment.UserMgt.IsAssignPrivilegeContent )
							MTEquipment.UserMgt.IsChangePasswordContent = false;
						break;
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		#endregion


		#region RESET DATA
		private void ResetPasswordField()
		{
			try
			{
				this.PB_Old_Password.Password = "";
				this.PB_New_Password.Password = "";
				this.PB_Comfirm_Password.Password = "";
				this.Lbl_Change_Status.Visibility = System.Windows.Visibility.Collapsed;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void ResetCreateUserField()
		{
			try
			{
				this.PB_Create_New_User.Text = "";
				this.PB_Create_New_UserPW.Password = "";
				this.Cb_Create_New_GroupID.SelectedItem = AccessLevel.Operator;
				this.Lbl_CreateUser_Change_Status.Content = "";
				this.Lbl_CreateUser_Change_Status.Visibility = System.Windows.Visibility.Collapsed;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		#endregion

		private void CB_UserList_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			try
			{
				if ( this.CB_UserList.SelectedIndex < 0 || this.CB_UserList.SelectedItem.ToString() == string.Empty )
				{
					this.SelectedUser = null;
					this.UserPrivilege.Clear();
				}
				else
				{
					this.SelectedUser = MTEquipment.UserMgt.GetUserInfo( this.CB_UserList.SelectedItem.ToString() );
					this.UserPrivilege.Clear();
					foreach ( UserAccessConfig uac in this.SelectedUser.UserConfig.Values )
					{
						this.UserPrivilege.Add( uac );
					}
				}
				this.Btn_Delete.IsEnabled = MTEquipment.UserMgt.CurrentLoginUser == this.SelectedUser ? false : this.CB_UserList.SelectedItem != null;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void Txt_UserID_KeyUp( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
			{
				this.PB_Password.Focus();
				this.PB_Password.Clear();
			}
		}

		private void PB_Password_KeyUp( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Enter )
				this.Btn_Login.RaiseEvent( new RoutedEventArgs( ButtonBase.ClickEvent ) );
		}
	}
}
