using HiPA.Common.Forms;
using HiPA.Common.UControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace HiPA.Common
{
	public class UserMgtConfig : BaseUtility
	{
		private string _UserID = "";
		public string UserID
		{
			get => this._UserID;
			set => this.Set( ref this._UserID, value, "UserID" );
		}
		public string UserPassword { get; set; }
		private AccessLevel _GroupID = AccessLevel.Guest;
		public AccessLevel GroupID
		{
			get => this._GroupID;
			set => this.Set( ref this._GroupID, value, "GroupID" );
		}
		public XmlDictionary<string, UserAccessConfig> UserConfig { get; set; } = new XmlDictionary<string, UserAccessConfig>();

		public bool isAllowChangeRecipe
		{
			get
			{
				try
				{
					foreach ( var config in this.UserConfig.Values )
					{
						if ( config.Privilege == PrivilegeType.ReadWrite )
							return true;
					}
				}
				catch
				{
				}
				return false;
			}
		}
	}
	public class LoginUserMgt : UserAccessConfig
	{
		public class AccessLevelChangeEventArgs : EventArgs
		{
			public AccessLevel AccessLevel { get; }
			public AccessLevelChangeEventArgs( AccessLevel AccessLevel )
			{
				this.AccessLevel = AccessLevel;
			}
		}
		public event AccessLevelChangedEventHandler AccessLevelChanged;
		public delegate void AccessLevelChangedEventHandler( object sender, AccessLevelChangeEventArgs e );

		public LoginUserMgt()
		{
		}

		public void InitUserAutModule( Dictionary<string, UI_Window> Lst_Page )
		{
			try
			{
				this.Lst_Page = Lst_Page;
				List<UserAccessConfig> list = new List<UserAccessConfig>();
				foreach ( var page in this.Lst_Page )
				{
					if ( page.Value.Control == null ) continue;
					var privilege = page.Value.Control is PageBase ? ( page.Value.Control as PageBase ).Privilege : ( page.Value.Control as WindowBase ).Privilege;
					list.Add( new UserAccessConfig() { Page = page.Value.Control.Name, Privilege = privilege } );
				}
				this.Lst_UserAccess = list;
				var sErr = this.UserDBDeserialize();
				if ( sErr != string.Empty )
					throw new Exception( sErr );
			}
			catch
			{ }
		}

		private Dictionary<string, UI_Window> Lst_Page = new Dictionary<string, UI_Window>();
		private UserMgtConfig _CurrentLoginUser = new UserMgtConfig();
		public UserMgtConfig CurrentLoginUser
		{
			get => this._CurrentLoginUser;
			set
			{
				this.Set( ref this._CurrentLoginUser, value, "CurrentLoginUser" );

				if ( value != null )
				{
					this.IsUserLogin = this._CurrentLoginUser.GroupID != AccessLevel.Guest;
					this.LoginUserID = value.UserID;
					this.LoginUserLevel = value.GroupID;
					this.SetPageVisibility( this._CurrentLoginUser );
				}
				else
				{
					this.SetPageVisibility( this.Guest );
					this.IsUserLogin = false;
				}
			}
		}
		private string _LoginUserID = "";
		public string LoginUserID
		{
			get => this._LoginUserID;
			set => this.Set( ref this._LoginUserID, value, "LoginUserID" );
		}

		private AccessLevel _LoginUserLevel = AccessLevel.Guest;
		public AccessLevel LoginUserLevel
		{
			get => this._LoginUserLevel;
			set
			{
				this.Set( ref this._LoginUserLevel, value, "LoginUserLevel" );
				this.AccessLevelChanged?.Invoke( this, new AccessLevelChangeEventArgs( this._LoginUserLevel ) );
			}
		}

		private bool _IsUserLogin = false;
		public bool IsUserLogin
		{
			get => this._IsUserLogin;
			set
			{
				this.Set( ref this._IsUserLogin, value, "IsUserLogin" );
				this.IsAssignPrivilegeContent = this.IsChangePasswordContent = this.IsAddUserContent = false;
				this.IsAllowAddUser = value && ( this.CurrentLoginUser.GroupID >= AccessLevel.Admin );
				this.IsAllowAssignPrivilege = value && ( this.CurrentLoginUser.GroupID >= AccessLevel.Admin );
			}
		}

		public List<UserAccessConfig> Lst_UserAccess { get; set; } = new List<UserAccessConfig>();
		private XmlDictionary<string, UserMgtConfig> Lst_UserMgtDB { get; set; } = new XmlDictionary<string, UserMgtConfig>();
		#region ui FLAGS
		private bool _IsAssignPrivilegeContent = false;
		public bool IsAssignPrivilegeContent
		{
			get => this._IsAssignPrivilegeContent;
			set => this.Set( ref this._IsAssignPrivilegeContent, value, "IsAssignPrivilegeContent" );
		}

		private bool _IsAddUserContent = false;
		public bool IsAddUserContent
		{
			get => this._IsAddUserContent;
			set => this.Set( ref this._IsAddUserContent, value, "IsAddUserContent" );
		}

		private bool _IsChangePasswordContent = false;
		public bool IsChangePasswordContent
		{
			get => this._IsChangePasswordContent;
			set => this.Set( ref this._IsChangePasswordContent, value, "IsChangePasswordContent" );
		}

		private bool _IsLoginFail = false;
		public bool IsLoginFail
		{
			get => this._IsLoginFail;
			set
			{
				this.Set( ref this._IsLoginFail, value, "IsLoginFail" );
			}
		}

		private bool _IsAllowAddUser = false;
		public bool IsAllowAddUser
		{
			get => this._IsAllowAddUser;
			set => this.Set( ref this._IsAllowAddUser, value, "IsAllowAddUser" );
		}

		private bool _IsAllowAssignPrivilege = false;
		public bool IsAllowAssignPrivilege
		{
			get => this._IsAllowAssignPrivilege;
			set => this.Set( ref this._IsAllowAssignPrivilege, value, "IsAllowAssignPrivilege" );
		}
		#endregion
		#region User Database Configuration: "Create User, Edit User Informations"
		public string CreateUser( string UserID, string UserPassword, AccessLevel GroupID )
		{
			var result = String.Empty;
			try
			{
				// Check for unique name
				if ( this.Lst_UserMgtDB.ContainsKey( UserID ) )
					return result = $"{UserID}\" existed.";

				// Set Parameters
				UserMgtConfig NewUser = new UserMgtConfig();
				NewUser.UserID = UserID;
				NewUser.UserPassword = ComputeSha256Hash( UserPassword );
				NewUser.GroupID = GroupID;

				foreach ( var element in this.Lst_UserAccess )
				{
					var page = this.Lst_Page[ element.Page ];
					if ( page.Control is PageBase )
					{
						var Page = page.Control as PageBase;
						if ( !Page.PagePrivConfigurable() ) continue;
						element.Privilege = Page.CheckPrivilege( GroupID );
					}
					else if ( page.Control is WindowBase )
					{
						var Page = page.Control as WindowBase;
						if ( !Page.PagePrivConfigurable() ) continue;
						element.Privilege = Page.CheckPrivilege( GroupID );
					}
					NewUser.UserConfig[ element.Page ] = new UserAccessConfig() //To fix C# Referencing issue
					{
						Page = element.Page,
						Privilege = element.Privilege
					};
				}
				this.Lst_UserMgtDB[ NewUser.UserID ] = NewUser;
				this.RefreshUserList();
				this.RefreshEditableUserList();
				this.RefreshCreatableUserLevel();
				this.SaveUserData();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( "LoginUserMgt", ex );
				return result;
			}
			return result;
		}
		public string EditUserInformation( string UserID, string NewPW, string OldPW )
		{
			var result = string.Empty;

			if ( this.Lst_UserMgtDB[ UserID ].UserPassword != ComputeSha256Hash( OldPW ) )
				result = "Incorrect Old Password";
			else
			{
				this.Lst_UserMgtDB[ UserID ].UserPassword = ComputeSha256Hash( NewPW );
				this.SaveUserData();
			}

			return result;
		}
		#endregion
		#region User Database Verification Check
		private string CheckUserDataBase()
		{
			var result = String.Empty;
			try
			{
				foreach ( var User in this.Lst_UserMgtDB )
				{
					//#JW See if there's any good ways to optimise this area.

					//Add Pages that is not existed in Saved UserDB
					foreach ( var element in this.Lst_Page.Values )
					{
						if ( element.Control == null ) continue;
						if ( element.Control is WindowBase ) if ( !( element.Control as WindowBase ).PagePrivConfigurable() ) continue;
						if ( element.Control is PageBase ) if ( !( element.Control as PageBase ).PagePrivConfigurable() ) continue;
						var privilege = element.Control is PageBase ? ( element.Control as PageBase ).Privilege : ( element.Control as WindowBase ).Privilege;
						if ( !User.Value.UserConfig.ContainsKey( element.Control.Name ) )
							User.Value.UserConfig[ element.Control.Name ] = new UserAccessConfig() { Page = element.Control.Name, Privilege = privilege };
						if ( !this.Guest.UserConfig.ContainsKey( element.Control.Name ) )
							this.Guest.UserConfig[ element.Control.Name ] = new UserAccessConfig() { Page = element.Control.Name, Privilege = PrivilegeType.ReadOnly };
					}
					var ExistedPages = User.Value.UserConfig.Where( pages => this.Lst_Page.ContainsKey( pages.Key ) );
					//Removing old page if any
					if ( User.Value.UserConfig.Count() != ExistedPages.Count() )
					{
						User.Value.UserConfig = new XmlDictionary<string, UserAccessConfig>();
						foreach ( var ele in ExistedPages )
							User.Value.UserConfig.Add( ele.Key, ele.Value );
					}
				}
				if ( ( result = this.SaveUserData() ) != string.Empty )
					throw new Exception( result );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( "LoginUserMgt", ex );
				return result;
			}
			return result;
		}
		#endregion
		#region User Login Function


		public string Login( string UserID, string UserPassword )
		{
			this.IsLoginFail = true;
			if ( string.IsNullOrEmpty( UserID ) || string.IsNullOrEmpty( UserPassword ) )
				return $"Invalid User ID or Password. Please try again!";
			if ( !this.Lst_UserMgtDB.ContainsKey( UserID ) )
				return $"Incorrect UserID.";
			if ( this.Lst_UserMgtDB[ UserID ].UserPassword != ComputeSha256Hash( UserPassword ) )
				return $"Incorrect Password.";
			this.IsLoginFail = false;
			this.CurrentLoginUser = this.Lst_UserMgtDB[ UserID ];
			this.RefreshCreatableUserLevel();
			this.RefreshEditableUserList();
			this.RefreshUserList();
			return "";
		}
		#endregion
		#region User Logout Function
		private UserMgtConfig Guest = new UserMgtConfig()
		{
			GroupID = AccessLevel.Guest,
			UserID = "Guest",
			UserPassword = "",
			UserConfig = new XmlDictionary<string, UserAccessConfig>(),
		};
		private UserMgtConfig Manu = new UserMgtConfig()
		{
			GroupID = AccessLevel.Manufacturer,
			UserID = "Manu",
			UserPassword = "",
			UserConfig = new XmlDictionary<string, UserAccessConfig>(),
		};
		public void Logout()
		{
			this.CurrentLoginUser = this.Manu;
		}
		#endregion
		#region Set Page Visibility
		private void SetPageVisibility( UserMgtConfig UserMgtCfg )
		{
			try
			{
				foreach ( var page in this.Lst_Page.Values )
				{
					if ( UserMgtCfg.UserConfig.ContainsKey( page.Control.Name ) )
					{
						if ( page.Control is PageBase )
						{
							( page.Control as PageBase ).AccessLevel = UserMgtCfg.GroupID;
							( page.Control as PageBase ).Privilege = UserMgtCfg.UserConfig[ page.Control.Name ].Privilege;
						}
						else if ( page.Control is WindowBase )
						{
							( page.Control as WindowBase ).AccessLevel = UserMgtCfg.GroupID;
							( page.Control as WindowBase ).Privilege = UserMgtCfg.isAllowChangeRecipe ? PrivilegeType.ReadWrite : PrivilegeType.ReadOnly;
						}
					}
					else
					{
						if ( page.Control is PageBase )
							( page.Control as PageBase ).AccessLevel = UserMgtCfg.GroupID;
						else if ( page.Control is WindowBase )
							( page.Control as WindowBase ).AccessLevel = UserMgtCfg.GroupID;
					}
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( "UserLogin", ex ), ErrorTitle.UserLoginError );
			}
		}
		public AccessLevel Is_UIWindowsAccess
		{
			set
			{
				foreach ( var page in this.Lst_Page.Values )
				{
					if ( page.Control == null ) continue;
					if ( page.Control is PageBase )
					{
						var Page = page.Control as PageBase;
						Page.Privilege = value >= Page.MinRead ? ( value >= Page.MinWrite ? PrivilegeType.ReadWrite : PrivilegeType.ReadOnly ) : PrivilegeType.Hidden;
						Page.AccessLevel = value;
					}
					else if ( page.Control is WindowBase )
					{
						var Page = page.Control as WindowBase;
						Page.Privilege = value >= Page.MinRead ? ( value >= Page.MinWrite ? PrivilegeType.ReadWrite : PrivilegeType.ReadOnly ) : PrivilegeType.Hidden;
						Page.AccessLevel = value;
					}
				}
			}
		}
		#endregion
		#region Get Username List
		public ObservableCollection<string> UserList { get; private set; } = new ObservableCollection<string>();
		private void RefreshUserList()
		{
			System.Windows.Application.Current.Dispatcher.Invoke( delegate
			{
				this.UserList.Clear();
				foreach ( var user in this.Lst_UserMgtDB )
					this.UserList.Add( user.Key );
			} );

		}
		public ObservableCollection<string> EditableUserList { get; private set; } = new ObservableCollection<string>();
		private void RefreshEditableUserList()
		{
			System.Windows.Application.Current.Dispatcher.Invoke( delegate
			{
				this.EditableUserList.Clear();
				foreach ( var user in this.Lst_UserMgtDB )
				{
					if ( user.Value.GroupID >= this.LoginUserLevel ) continue;
					this.EditableUserList.Add( user.Key );
				}
			} );
		}
		#endregion
		#region Creatable user level
		public ObservableCollection<AccessLevel> CreatableUserLevel { get; private set; } = new ObservableCollection<AccessLevel>();
		private void RefreshCreatableUserLevel()
		{
			var Creatable = Enum.GetValues( typeof( AccessLevel ) ).Cast<AccessLevel>().ToList().Where( x => x <= this.LoginUserLevel && x > AccessLevel.Guest );
			System.Windows.Application.Current.Dispatcher.Invoke( delegate
			{
				this.CreatableUserLevel.Clear();
				foreach ( var lvl in Creatable )
				{
					this.CreatableUserLevel.Add( lvl );
				}
			} );



		}
		#endregion
		#region Get User Informations
		public UserMgtConfig GetUserInfo( string name )
		{
			return this.Lst_UserMgtDB[ name ];
		}
		#endregion
		#region Update User Informations
		public string UpdateUserInfo( UserMgtConfig user )
		{
			var result = string.Empty;
			try
			{
				this.Lst_UserMgtDB[ user.UserID ] = user;
				this.SaveUserData();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( "LoginUserMgt", ex );
			}

			return result;
		}
		#endregion
		#region Delete User
		public string DeleteUser( UserMgtConfig user )
		{
			var result = string.Empty;
			if ( this.Lst_UserMgtDB.ContainsKey( user.UserID ) )
			{
				this.Lst_UserMgtDB.Remove( user.UserID );
				this.SaveUserData();
			}
			else
				result = $"Invalid User";
			this.RefreshEditableUserList();
			this.RefreshUserList();
			this.RefreshCreatableUserLevel();
			return result;
		}
		public string DeleteUser( string user )
		{
			var result = string.Empty;
			if ( this.Lst_UserMgtDB.ContainsKey( user ) )
			{
				this.Lst_UserMgtDB.Remove( user );
				this.SaveUserData();
			}
			else
				result = $"Invalid User";

			return result;
		}
		#endregion
		#region Saving into UserDatabase
		public string SaveUserData()
		{
			var result = String.Empty;
			XmlWriter writer = null;
			try
			{
				var fpSaveUserData = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "UserDataBase.xml" );

				// Check for UserDatabase's existance
				if ( File.Exists( fpSaveUserData ) )
				{
					// Set UserDatabase file to unhidden
					var attributes = File.GetAttributes( fpSaveUserData );
					if ( ( attributes & FileAttributes.Hidden ) == FileAttributes.Hidden )
					{
						attributes &= ~FileAttributes.Hidden;
						File.SetAttributes( fpSaveUserData, attributes );
					}
				}
				#region Serialization
				var settings = new XmlWriterSettings();
				settings.ConformanceLevel = ConformanceLevel.Fragment;
				settings.Indent = true;
				settings.NewLineOnAttributes = true;
				writer = XmlWriter.Create( fpSaveUserData, settings );
				//this.Lst_UserMgtDB.WriteXml( writer );
				this.Lst_UserMgtDB.WriteXml_2LevelNesting( writer );
				writer.Close();
				#endregion

				// Set UserDatabase file to hidden
				File.SetAttributes( fpSaveUserData, File.GetAttributes( fpSaveUserData ) | FileAttributes.Hidden );
			}
			catch ( Exception ex )
			{
				writer?.Close();
				result = this.FormatErrMsg( "LoginUserMgt", ex );
				return result;
			}
			return result;
		}
		#endregion
		#region User Database Deserialization
		private string UserDBDeserialize()
		{
			var result = String.Empty;
			try
			{
				#region Retrieve User's Database
				var fpSaveUserData = string.Empty;

				// Backup UserDB if Customer's User Management DataBase was deleted.
				if ( File.Exists( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "Manufacturer.xml" ) ) )
					fpSaveUserData = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "Manufacturer.xml" );
				else if ( File.Exists( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "AMSMaster.xml" ) ) )
					fpSaveUserData = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "AMSMaster.xml" );
				else if ( File.Exists( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "UserDataBase.xml" ) ) )
					fpSaveUserData = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "config", "UserDataBase.xml" );
				#endregion
				#region DeSerialization
				if ( fpSaveUserData != string.Empty )
				{
					var settings = new XmlReaderSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;
					XmlReader reader = XmlReader.Create( fpSaveUserData, settings );
					this.Lst_UserMgtDB.ReadXml_2LevelNesting( reader );
					reader.Close();
					//this.DeleteUser( "JPT2020" );
				}
				else
				{
					var UserMngCfg = new UserMgtConfig()
					{
						UserID = "ENERGEO",
						UserPassword = ComputeSha256Hash( "ENERGEO123" ),
						GroupID = AccessLevel.Manufacturer,
						UserConfig = new XmlDictionary<string, UserAccessConfig>(),
					};
					UserMngCfg.UserConfig[ "DefaultUser" ] = new UserAccessConfig();
					this.Lst_UserMgtDB[ UserMngCfg.UserID ] = UserMngCfg;
				}
				#endregion
				result = this.CheckUserDataBase();
				return result;
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( "LoginUserMgt", ex );
				return result;
			}
		}
		#endregion
		#region Encryption of User's Password
		static string ComputeSha256Hash( string Password )
		{
			try
			{
				// Create a SHA256   
				using ( SHA256 sha256Hash = SHA256.Create() )
				{
					// ComputeHash - returns byte array  
					byte[] bytes = sha256Hash.ComputeHash( Encoding.UTF8.GetBytes( Password + "Neowise" ) );

					// Convert byte array to a string   
					StringBuilder builder = new StringBuilder();
					for ( int i = 0; i < bytes.Length; i++ )
					{
						builder.Append( bytes[ i ].ToString( "x2" ) );
					}
					return builder.ToString();
				}
			}
			catch ( Exception )
			{
				return string.Empty;
			}
		}
		#endregion
	}
	public class UserAccessConfig : BaseUtility
	{
		public string Page
		{
			get => this.GetValue( () => this.Page );
			set => this.SetValue( () => this.Page, value );
		}
		public PrivilegeType Privilege
		{
			get => this.GetValue( () => this.Privilege );
			set => this.SetValue( () => this.Privilege, value );
		}
		public UserAccessConfig( string Page, UserControl Control, PrivilegeType Privilege )
		{
			this.Page = Page;
			this.Privilege = Privilege;
		}
		public UserAccessConfig()
		{
		}
	}
	public class UI_Window
	{
		public ContentControl Control = null;
		public StackPanel c_Parent = null;
		public Thickness th_Pos = new Thickness( 0, 0, 0, 0 );
		public UI_Window()
		{

		}

		public UI_Window( UI_Window src )
		{
			this.c_Parent = src.c_Parent;
			this.th_Pos = src.th_Pos;
			this.Control = src.Control;
		}
	}
}
