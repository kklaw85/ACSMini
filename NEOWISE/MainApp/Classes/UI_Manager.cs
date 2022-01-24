using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.UControl;
using N_Data_Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SystemInfo;

namespace NeoWisePlatform
{
	class C_UI_Manager
	{
		public static event PropertyChangedEventHandler StaticPropertyChanged;
		public const string MainMenu_Alarm = "Ctrl_Alarm";
		public const string MainMenu_Production = "Ctrl_Production";
		public const string MainMenu_Diagnostics = "Ctrl_Diagnostics";
		public const string MainMenu_ProcessSetup = "Ctrl_ProcessSetup";
		public const string MainMenu_UserAccess = "Ctrl_User_Access";

		public const string MainWindow = "MainWindow";

		public static string Cur_Main_Pg = "";
		public static string Cur_Sub_Pg = "";
		private const string Cur_Sub_Page = "CurrentSubPage";
		public static Dictionary<string, Dictionary<string, UI_Window>> PageStructure = new Dictionary<string, Dictionary<string, UI_Window>>();

		static Style Bg_Normal_Btn;
		static Style Bg_Btn_Hilighted;
		static StackPanel Stack_Main_Btn;
		static StackPanel Stack_SubMenu_Btn;
		public static void Init_UI_Mngr( Style Highlighted, Style Normal, StackPanel Stack_Main_Btn, StackPanel Stack_SubMenu_Btn )
		{
			C_UI_Manager.Bg_Normal_Btn = Normal;
			C_UI_Manager.Bg_Btn_Hilighted = Highlighted;
			C_UI_Manager.Stack_Main_Btn = Stack_Main_Btn;
			C_UI_Manager.Stack_SubMenu_Btn = Stack_SubMenu_Btn;
			Cur_Main_Pg = "";
		}
		private static System.Windows.Media.Brush _foregroundColor = System.Windows.Media.Brushes.LightSlateGray;
		public static System.Windows.Media.Brush ForegroundColor
		{
			get { return C_UI_Manager._foregroundColor; }
			set
			{

				C_UI_Manager._foregroundColor = value;
				StaticPropertyChanged?.Invoke( null, new PropertyChangedEventArgs( "ForegroundColor" ) );
			}
		}

		private static List<object> lstChildren;

		public static List<object> GetChildren( Visual p_vParent, int p_nLevel )
		{
			if ( p_vParent == null )
			{
				throw new ArgumentNullException( "Element {0} is null!", p_vParent.ToString() );
			}

			lstChildren = new List<object>();
			GetChildControls( p_vParent, p_nLevel );
			return lstChildren;

		}

		private static void GetChildControls( Visual p_vParent, int p_nLevel )
		{
			int nChildCount = VisualTreeHelper.GetChildrenCount( p_vParent );
			for ( int i = 0; i <= nChildCount - 1; i++ )
			{
				Visual v = ( Visual )VisualTreeHelper.GetChild( p_vParent, i );

				lstChildren.Add( ( object )v );
				if ( VisualTreeHelper.GetChildrenCount( v ) > 0 )
				{
					GetChildControls( v, p_nLevel + 1 );
				}
			}
		}

		public static childItem FindVisualChild<childItem>( DependencyObject obj ) where childItem : DependencyObject
		{
			for ( int i = 0; i < VisualTreeHelper.GetChildrenCount( obj ); i++ )
			{
				DependencyObject child = VisualTreeHelper.GetChild( obj, i );
				if ( child != null && child is childItem )
					return ( childItem )child;
				else
				{
					childItem childOfChild = FindVisualChild<childItem>( child );
					if ( childOfChild != null )
						return childOfChild;
				}
			}
			return null;
		}


		public static void Set_Last_Sub_Page( string Main_Pg, string PageName )
		{
			UI_Window Page = new UI_Window();
			Get_UI_Windows_Details( PageName, ref Page );

			if ( PageStructure.ContainsKey( Main_Pg ) )
				PageStructure[ Main_Pg ][ Cur_Sub_Page ] = Page;
		}

		public static Cursor GetWaitCursor()
		{
			try
			{
				return new Cursor( C_Shared_Data.Info_Sys.sRoot_Dir + "CW Patriotic.ani" );
			}
			catch ( Exception ) { }
			return Cursors.Wait;

		}

		public static void Get_Sub_Pages( string Main_Pg, ref string[] Sub_Pgs )
		{

			List<string> Temp_Lst_Sub_Pages = new List<string>();
			foreach ( var page in PageStructure[ Main_Pg ] )
			{
				if ( page.Value.Control == null ) continue;
				Temp_Lst_Sub_Pages.Add( page.Value.Control.Name );
			}
			Sub_Pgs = Temp_Lst_Sub_Pages.ToArray();
		}

		public static Dictionary<string, UI_Window> Lst_Page = new Dictionary<string, UI_Window>();
		public static void Register_UI_Elements( string MainMenu, StackPanel Parent, Thickness loc, ContentControl control )
		{
			try
			{
				UI_Window new_UI = new UI_Window();
				new_UI.c_Parent = Parent;
				new_UI.Control = control;
				new_UI.th_Pos = loc;

				Lst_Page[ control.Name ] = new_UI;


				if ( !PageStructure.ContainsKey( MainMenu ) )
				{
					PageStructure[ MainMenu ] = new Dictionary<string, UI_Window>();
					PageStructure[ MainMenu ][ Cur_Sub_Page ] = new UI_Window();
				}
				PageStructure[ MainMenu ][ control.Name ] = new_UI;
			}
			catch
			{
				//System.Windows.MessageBox.Show( ex.Message );
			}
		}

		public static string Get_UI_Windows_Details( string Pagename, ref UI_Window Window )
		{

			try
			{
				if ( PageStructure.ContainsKey( Pagename ) )
				{
					Window = PageStructure[ Pagename ][ Pagename ];
					return string.Empty;
				}
				foreach ( var Submenus in PageStructure.Values )
				{
					if ( !Submenus.ContainsKey( Pagename ) ) continue;
					Window = Submenus[ Pagename ];
					break;
				}

			}
			catch ( Exception ex )
			{
				return ex.Message + Environment.NewLine +
					   "Get UI Windows Details";
			}
			return C_Shared_Data.ErrorMap.NO_ERROR;

		}

		public static string Get_Page_Info( string Main, string Sub_Name )
		{
			string str_Sub = "";
			string str_Main = "";
			if ( Main != string.Empty )
				str_Main = Main.Remove( 0, 5 );
			if ( Sub_Name != string.Empty )
				str_Sub = Sub_Name.Remove( 0, 5 );

			string sTitle = C_System_info.GetApplicationName() +
							 " (" + C_System_info.Software_Version + ") ";

			if ( str_Sub == "" )
				str_Sub = sTitle + " : " + str_Main;
			else
				str_Sub = sTitle + " : " + str_Main + ">" + str_Sub;

			return str_Sub;
		}


		public static string Update_Navi_Buttons( Button Btn, ref bool Main_Win )
		{
			try
			{
				string Page_Name = Btn.Name;
				bool Click_On_Main = false;
				bool Show_Sub_Page = false;
				//Same Pages

				//Click on Main Page Navigation Button
				if ( C_UI_Manager.PageStructure.ContainsKey( Page_Name ) )
				{
					var btn = Stack_Main_Btn.Children.OfType<Button>().ToList().Find( x => x.Name == C_UI_Manager.Cur_Main_Pg );
					if ( btn != null )
						btn.Style = Bg_Normal_Btn;
					Click_On_Main = true;
				}
				Main_Win = Click_On_Main;
				string[] Sub_Pages = { "" };
				//Set Style for Current Click Button


				if ( Click_On_Main )
				{
					C_UI_Manager.Cur_Sub_Pg = "";
					C_UI_Manager.Get_Sub_Pages( Page_Name, ref Sub_Pages );
					Btn.Style = Bg_Btn_Hilighted;
				}
				else
				{
					C_UI_Manager.Get_Sub_Pages( C_UI_Manager.Cur_Main_Pg, ref Sub_Pages );
				}

				if ( Sub_Pages.Length > 1 )
				{
					var Pages = Sub_Pages.ToList().Where( x => x != string.Empty );

					foreach ( var SubPage in Pages )
					{
						Show_Sub_Page = true;
						break;
					}


					string Current_Sub_Page = Click_On_Main ? Sub_Pages[ 0 ] : Page_Name;

					//Handle Sub Navigation Buttons
					foreach ( Button btn in Stack_SubMenu_Btn.Children.OfType<Button>() )
					{
						if ( Sub_Pages.Contains( btn.Name ) )
						{
							if ( ( Lst_Page[ btn.Name ].Control as PageBase ).Privilege != PrivilegeType.Hidden )
							{
								btn.Visibility = System.Windows.Visibility.Visible;
								//Check for Last Display Sub Page
								if ( btn.Name == Current_Sub_Page )
								{
									btn.Style = Bg_Btn_Hilighted;
									C_UI_Manager.Cur_Sub_Pg = btn.Name;
									if ( !Click_On_Main ) C_UI_Manager.Set_Last_Sub_Page( C_UI_Manager.Cur_Main_Pg, Page_Name );
								}
								else btn.Style = Bg_Normal_Btn;
							}
							else
								btn.Visibility = System.Windows.Visibility.Collapsed;
						}
						else
							btn.Visibility = System.Windows.Visibility.Collapsed;
					}
				}
				else
				{
					C_UI_Manager.Cur_Sub_Pg = "";
					foreach ( Button btn in Stack_SubMenu_Btn.Children.OfType<Button>() )
					{
						btn.Visibility = System.Windows.Visibility.Collapsed;
					}
				}

				if ( Click_On_Main )
				{
					C_UI_Manager.Cur_Main_Pg = Btn.Name;
				}
				if ( Show_Sub_Page )
				{
					Main_Win = false;
				}


			}
			catch ( Exception ex )
			{
				return ex.Message + Environment.NewLine
					   + "Update Navi Button";
			}
			return C_Shared_Data.ErrorMap.NO_ERROR;
		}
	}
}
