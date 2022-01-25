using ControlzEx.Theming;
using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Recipe;
using HiPA.Common.UControl;
using N_Data_Utilities;
using NeoWisePlatform.Module;
using NeoWisePlatform.Production.SubPages;
using NeoWisePlatform.ProductionSetup.SubPages;
using NeoWisePlatform.Recipe;
using NeoWisePlatform.Recipe.Windows;
using NeoWisePlatform.SystemControls.SubPages;
using NeoWisePlatform.UI.Alarm.Subpages;
using NeoWisePlatform.UI.ProductionSetup.SubPages;
using NeoWisePlatform.UI.SystemControls.SubPages;
using NeoWisePlatform.UI.Windows;
using NeoWisePlatform.UserAccess.SubPages;
using NeoWisePlatform.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace NeoWisePlatform
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : WindowBase
	{
		public static readonly DependencyProperty ColorsProperty
	= DependencyProperty.Register(
		nameof( Colors ),
		typeof( List<KeyValuePair<string, Color>> ),
		typeof( MainWindow ) );

		public List<KeyValuePair<string, Color>> Colors
		{
			get => ( List<KeyValuePair<string, Color>> )this.GetValue( ColorsProperty );
			set => this.SetValue( ColorsProperty, value );
		}

		string Err = "";
		public List<string> Lst_Loaded_UI = new List<string>();
		private double CanvasWidth = 0;
		private double CanvasHeight = 0;
		bool Exit_Without_Confirm = false;
		private MachineStateMgr _MachStateMgr = null;
		private MTEquipment _EM = null;
		public MainWindow()
		{
			this.PrivilegeLockability = UILockability.Nonlockable;
			this.MinRead = AccessLevel.Guest;
			this.MinWrite = AccessLevel.Guest;
			this.InitializeComponent();


			this.Colors = typeof( Colors )
				.GetProperties()
				.Where( prop => typeof( Color ).IsAssignableFrom( prop.PropertyType ) )
				.Select( prop => new KeyValuePair<String, Color>( prop.Name, ( Color )prop.GetValue( null ) ) )
				.ToList();
			this.ColorsSelector.ItemsSource = this.Colors;
			var theme = ThemeManager.Current.DetectTheme( Application.Current );
			ThemeManager.Current.ChangeTheme( Application.Current, ThemeManager.Current.AddTheme( RuntimeThemeGenerator.Current.GenerateRuntimeTheme( theme.BaseColorScheme, ( ( SolidColorBrush )Application.Current.Resources[ "MasterTheme" ] ).Color ) ) );
			Application.Current?.MainWindow?.Activate();
		}
		private void SetBinding()
		{
			try
			{
				this.Status_Item_Err_Msg.DataContext = Equipment.ErrManager;
				this.Status_Machine_State.DataContext = this._MachStateMgr;

				var b = new Binding();
				b.Source = this._EM.TowerLight;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "SilentBuzzer" );
				b.Converter = new BoolToStyleRed();
				this.Btn_Alarm_Off.SetBinding( Button.StyleProperty, b );

				this.BindLockUI( this.Btn_Recipe_Editor );
				this.BindLockUI( this.Btn_Save_Recipe );
				this.BindLockAccessLevelEngineer( this.Btn_InitAll );

				b = new Binding() { Source = MTEquipment.UserMgt, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsUserLogin" ), Converter = new BoolToVisibleHidden() };
				this.WP_User_Info.SetBinding( StackPanel.VisibilityProperty, b );
			}
			catch ( Exception ex )
			{
				MessageBox.Show( ex.Message + "Set Binding in Main Window UI" );
			}
		}

		private void Win_Main_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this._MachStateMgr = Equipment.MachStateMgr;
				Equipment.MachStateMgr.MachineStatus = MachineStateType.UNINITIALIZE;
				this.Visibility = System.Windows.Visibility.Collapsed;
				Splash_Sreen_Window win = new Splash_Sreen_Window();
				win.ShowDialog();
				C_UI_Manager.Init_UI_Mngr( ( Style )this.FindResource( "Button_MenuSelected" ), ( Style )this.FindResource( "Button_Menu" ), this.Stack_Main_Btn, this.SubMenu );
				this.Register_UI_Windows();
				this.LoadAllPages();
				Equipment.UserMgt.InitUserAutModule( C_UI_Manager.Lst_Page );
				Equipment.UserMgt.Logout();
				this._EM = ( Constructor.GetInstance().Equipment as MTEquipment );
				Win_Pre_Init InitOption = new Win_Pre_Init();
				bool Start = ( bool )InitOption.ShowDialog();
				if ( !Start )
				{
					if ( InitOption.IsRestart )
					{
						Constructor.GetInstance().Save();
						System.Windows.Forms.Application.Restart();
						Process.GetCurrentProcess().Kill();
					}
					else
					{
						this.Exit_Without_Confirm = true;
						Application.Current.Shutdown();
					}
				}
				else
				{
					//Register to UI Manager
					var taskhoming = this._EM.HomeAxes();
					Win_RecipeList winRecipe = new Win_RecipeList();
					winRecipe.IsShowExitBtn = true;
					Start = ( bool )winRecipe.ShowDialog();
					if ( !Start )
					{
						this.Exit_Without_Confirm = true;
						Application.Current.Shutdown();
					}
					else
						this.Visibility = System.Windows.Visibility.Visible;
					this.SetBinding();
					this.Setup_Pages( this.Ctrl_Production );
					this.Visibility = System.Windows.Visibility.Visible;
					this._MachStateMgr.EM = this._EM;
					Win_Wait_Home winHome = new Win_Wait_Home();
					winHome.HomingTask = taskhoming;
					winHome.ShowDialog();
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private void LoadAllPages()
		{
			try
			{
				foreach ( var pages in C_UI_Manager.Lst_Page.Values )
				{
					if ( !( pages.Control is PageBase ) ) continue;
					this.Add_To_Cvs_Parent_Container( pages.Control, pages.c_Parent );
					( pages.Control as PageBase ).Load();
					( pages.Control as PageBase ).Unload();
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private void Btn_Dialog_Click( object sender, RoutedEventArgs e )
		{
			Button Btn = ( Button )sender;
			this.Cursor = C_UI_Manager.GetWaitCursor();
			Btn.IsEnabled = false;
			this.Load_Dialog( Btn.Name );
			Btn.IsEnabled = true;
			this.Cursor = Cursors.Arrow;
		}

		#region BtnAction handler
		private void Btn_Action_Click( object sender, RoutedEventArgs e )
		{
			Button Btn = ( Button )sender;
			//if ( !C_Shared_Data.Info_Sys.IsMachineReady() )
			//	return;
			this.Cursor = C_UI_Manager.GetWaitCursor();
			this.Do_Actions( Btn );
		}
		public void Do_Actions( Button btn )
		{
			try
			{
				var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe();
				if ( btn == this.Btn_Save_Recipe )
				{
					if ( recipe == null ) throw new Exception( $"^{( int )RunErrors.ERR_RecipeObjNull }^" );
					Recipes.HandlerRecipes()?.Save( recipe );
					Constructor.GetInstance().Save();
					RecipeNoticeHandler.RecipeChanged = false;
				}
				else if ( btn == this.Btn_InitAll )
				{
					MessageBoxResult msgBoxRslt = System.Windows.MessageBox.Show( "The machine is about to home, please make sure the condition is safe.", "Reset Motor and Home", MessageBoxButton.YesNo, MessageBoxImage.Asterisk );
					if ( msgBoxRslt == MessageBoxResult.No )
						return;

					Win_Wait_Home win = new Win_Wait_Home();
					win.DoInit = true;
					win.ShowDialog();
				}
				else if ( btn == this.Btn_Alarm_Off )
				{
					this._EM.TowerLight.SilentBuzzer = true;
				}
			}
			catch ( Exception ex )
			{
				System.Windows.Application.Current.Dispatcher.Invoke( delegate
				{
					Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
					string msg = ex.Message + Environment.NewLine +
					"Action Handler";
				} );

			}
			finally
			{
				//C_Shared_Data.Info_Sys.Machine_State = Machine_State.MANUAL_MODE;
				System.Windows.Application.Current.Dispatcher.Invoke( delegate
				{
					this.Cursor = Cursors.Arrow;
				} );

			}
			//} );
		}

		#endregion

		private string Load_Dialog( string BtnName )
		{
			string sErr = string.Empty;
			try
			{
				C_UI_Manager.Cur_Sub_Pg = BtnName;
				string Curr_Page = C_UI_Manager.Cur_Sub_Pg;
				this.Show_DialogPage( Curr_Page );
				this.Title = C_UI_Manager.Get_Page_Info( C_UI_Manager.Cur_Main_Pg, C_UI_Manager.Cur_Sub_Pg );
				this.Cursor = Cursors.Arrow;
			}
			catch ( Exception ex )
			{
				sErr = ex.Message + Environment.NewLine +
								"Load Dialog";
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
				this.Cursor = Cursors.Arrow;
			}
			return sErr;
		}
		private void Show_DialogPage( string PageName )
		{
			try
			{
				double pad = 2;
				double x = pad;
				double y = pad;
				Thickness loc = new Thickness( x, y, 0, 0 );

				if ( PageName == this.Btn_Recipe_Editor.Name )
				{
					var name = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.Name;
					if ( name != null )
					{
						if ( MessageBox.Show( "Do you want to save Recipe File before re-select the Recipe and Product?", "Save Recipe File", MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.Yes )
						{
							var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe();
							Recipes.HandlerRecipes()?.Save( recipe );
						}
						else
						{
							Recipes.HandlerRecipes()?.Reload();
						}
					}


					Win_RecipeList win = new Win_RecipeList();
					win.IsShowExitBtn = false;
					win.ShowDialog();
				}
			}
			catch ( Exception ex )
			{
				string msg = ex.Message + Environment.NewLine +
							 "ShowDialogPage";
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}

		}

		#region Add pages here
		public void Register_UI_Windows()
		{
			try
			{
				//double pad = 2;
				//double x = pad;
				//double y = pad + 2;
				Thickness loc = new Thickness( 0, 0, 0, 0 );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_Alarm, this.Cvs_Wrapper, loc, new Ctrl_Alarm() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_Production, this.Cvs_Wrapper, loc, new Ctrl_Production() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_Diagnostics, this.Cvs_Wrapper, loc, new Ctrl_InstrumentOfMotion() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_Diagnostics, this.Cvs_Wrapper, loc, new Ctrl_InstrumentOfIO() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_Diagnostics, this.Cvs_Wrapper, loc, new Ctrl_InstrumentOfCamera() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_Diagnostics, this.Cvs_Wrapper, loc, new Ctrl_ManufacturerBypassOption() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_Diagnostics, this.Cvs_Wrapper, loc, new Ctrl_TestSequences() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_ProcessSetup, this.Cvs_Wrapper, loc, new Ctrl_SetupLoadUnload() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_ProcessSetup, this.Cvs_Wrapper, loc, new Ctrl_SetupWorkPosPage() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_ProcessSetup, this.Cvs_Wrapper, loc, new Ctrl_SetupVision() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainMenu_UserAccess, this.Cvs_Wrapper, loc, new Ctrl_User_Access() );
				C_UI_Manager.Register_UI_Elements( C_UI_Manager.MainWindow, this.Cvs_Wrapper, loc, this );
			}
			catch ( Exception ex )
			{
				string msg = ex.Message + Environment.NewLine +
								"Register UI Window";
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		#endregion
		#region BtnPageSwitch handler
		private void Btn_Navi_Click( object sender, RoutedEventArgs e )
		{
			Stopwatch timer = new Stopwatch();
			timer.Restart();
			Button Btn = ( Button )sender;
			//if ( !C_Shared_Data.Info_Sys.IsMachineReady() )
			//	return;
			this.Cursor = C_UI_Manager.GetWaitCursor();
			Btn.IsEnabled = false;
			this.Setup_Pages( Btn );
			var timepassed = timer.ElapsedMilliseconds;
			Btn.IsEnabled = true;
			this.Cursor = Cursors.Arrow;
			timepassed = timer.ElapsedMilliseconds;
		}
		private void Setup_Pages( Button Btn )
		{
			try
			{
				if ( Btn.Name == C_UI_Manager.Cur_Main_Pg ||
					Btn.Name == C_UI_Manager.Cur_Sub_Pg ) return;
				//Update and Set Current Page to setup
				bool Main_Win = false;
				this.Cursor = C_UI_Manager.GetWaitCursor();
				this.Err = C_UI_Manager.Update_Navi_Buttons( Btn, ref Main_Win );
				if ( this.Err != C_Shared_Data.ErrorMap.NO_ERROR ) throw new Exception( this.Err );
				this.Err = this.Load_Unload_UIs( Main_Win );
				if ( this.Err != C_Shared_Data.ErrorMap.NO_ERROR ) throw new Exception( this.Err );
				this.Title = C_UI_Manager.Get_Page_Info( C_UI_Manager.Cur_Main_Pg,
												   C_UI_Manager.Cur_Sub_Pg );
				Equipment.ErrManager.ShowMessage( C_Shared_Data.EventMap.SYS_READY, "UI Initialization" );
				this.Cursor = Cursors.Arrow;
			}
			catch ( Exception ex )
			{
				string msg = ex.Message + Environment.NewLine +
								"Setup Pages";

				//C_Shared_Data.Info_Sys.Machine_State = Machine_State.MANUAL_MODE;
				this.Cursor = Cursors.Arrow;
				new Thread( () => System.Windows.MessageBox.Show( msg ) ).Start();
			}


		}
		public void Unload_UI( List<string> Lst_UI_To_Unload )
		{
			try
			{
				if ( Lst_UI_To_Unload.Count == 0 ) return;
				foreach ( var name in Lst_UI_To_Unload )
				{
					this.Remove_From_Grid_Parent_Container( name, this.Cvs_Wrapper.Name );
				}
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( ex.Message + Environment.NewLine +
								"Unload UI" ) ).Start();
			}
		}
		public string Load_Unload_UIs( bool Main_Win )
		{
			try
			{
				string Curr_Page = Main_Win ? C_UI_Manager.Cur_Main_Pg : C_UI_Manager.Cur_Sub_Pg;
				//UI Windows
				var Lst_UI_To_Unload = new List<string>();
				foreach ( var name in this.Lst_Loaded_UI )
				{
					Lst_UI_To_Unload.Add( name );
				}
				var UI = new UI_Window();
				this.Err = C_UI_Manager.Get_UI_Windows_Details( Curr_Page, ref UI );
				if ( UI != null && UI.Control != null ) if ( Lst_UI_To_Unload.Contains( UI.Control.Name ) ) Lst_UI_To_Unload.Remove( UI.Control.Name );
				this.Unload_UI( Lst_UI_To_Unload );
				this.Load_UI_Elements( UI );
			}
			catch ( Exception ex )
			{
				string msg = ex.Message + Environment.NewLine +
							 "Load_Unload_UI";
			}
			return C_Shared_Data.ErrorMap.NO_ERROR;
		}
		private void Load_UI_Elements( UI_Window Lst_Windows )
		{
			try
			{
				this.CanvasWidth = this.Cvs_Wrapper.ActualWidth;
				this.CanvasHeight = this.Cvs_Wrapper.ActualHeight;
				if ( Lst_Windows == null || Lst_Windows.Control == null ) return;
				Lst_Windows.Control.Margin = Lst_Windows.th_Pos;
				this.Add_To_Cvs_Parent_Container( Lst_Windows.Control, Lst_Windows.c_Parent );
				if ( !this.Lst_Loaded_UI.Contains( Lst_Windows.Control.Name ) )
					this.Lst_Loaded_UI.Add( Lst_Windows.Control.Name );
				//Controlling of userAccess to UI Window
				( Lst_Windows.Control as PageBase ).Load();
				( Lst_Windows.Control as PageBase ).Width = this.CanvasWidth;
				( Lst_Windows.Control as PageBase ).Height = this.CanvasHeight;
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( ex.Message +
								Environment.NewLine +
								"Load UI Elements" ) ).Start();
			}
		}
		private void Add_To_Cvs_Parent_Container( UIElement Ele, StackPanel Parent )
		{
			try
			{
				if ( Ele != null )
				{
					if ( !Parent.Children.Contains( Ele ) )
					{
						Parent.Children.Add( Ele );
					}
				}
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( ex.Message +
								Environment.NewLine +
								"Add UI To Parrent " + Parent.Name ) ).Start();
			}
		}
		private void Remove_From_Grid_Parent_Container( string UI_Name, string ParentName )
		{
			try
			{
				bool Removed = false;
				var UI = new UI_Window();
				this.Err = C_UI_Manager.Get_UI_Windows_Details( UI_Name, ref UI );
				if ( this.Err != C_Shared_Data.ErrorMap.NO_ERROR )
				{
					throw new Exception( this.Err );
				}
				if ( UI?.Control != null )
				{
					UI.Control.Visibility = Visibility.Collapsed;
					Removed = true;
				}

				if ( Removed )
				{
					if ( this.Lst_Loaded_UI.Contains( UI_Name ) )
					{
						this.Lst_Loaded_UI.Remove( UI_Name );
					}
				}
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( ex.Message + Environment.NewLine +
								"Remove UI" + UI_Name + " from Parent " + ParentName ) ).Start();
			}
		}
		#endregion
		private void Win_Main_Closing( object sender, System.ComponentModel.CancelEventArgs e )
		{
			if ( ( Constructor.GetInstance()?.Equipment as MTEquipment ).AutoSeq?.State != Sequence.SequenceState.IsNotStarted )
			{
				e.Cancel = true;
				return;
			}
			Constructor.GetInstance().Save();
			if ( !this.Exit_Without_Confirm )
			{
				string Msg = this.FindResource( "ExitApplication" ).ToString();
				if ( MessageBox.Show( Msg, this.FindResource( "Exit" ).ToString(),
									 MessageBoxButton.YesNo,
									 MessageBoxImage.Question ) != MessageBoxResult.Yes )
				{
					e.Cancel = true;
					return;
				}

				if ( RecipeNoticeHandler.RecipeChanged )
				{
					if ( MessageBox.Show( C_Shared_Data.WarningMap.SAVE_RECIPE_AFTER_CHANGES, "Save Recipe File",
										 MessageBoxButton.YesNo,
										 MessageBoxImage.Question ) == MessageBoxResult.Yes )
					{
						var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe();
						if ( recipe != null )
							Recipes.HandlerRecipes()?.Save( recipe );
					}
				}
			}

			try
			{
				Constructor.GetInstance().Shutdown();
				Thread.Sleep( 50 );
				GC.Collect();
				Application.Current.Shutdown();
			}
			catch
			{
				GC.Collect();
				Dispatcher.CurrentDispatcher.Thread.Abort();
				Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				Application.Current.Shutdown();
			}
		}

		private void Btn_Recipe_Editor_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				Button Btn = ( Button )sender;
				this.Show_DialogPage( this.Btn_Recipe_Editor.Name );
			}
			catch
			{ }
		}

		private void CmbLang_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			try
			{
				var lang = e.AddedItems[ 0 ] as string;
				if ( !string.IsNullOrWhiteSpace( lang ) )
					this._EM.MultiLangErr.LoadErrLang( lang ).Wait();
			}
			catch
			{ }
		}

		private void Color_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			var selectedColor = e.AddedItems.OfType<KeyValuePair<string, Color>?>().FirstOrDefault();
			if ( selectedColor != null )
			{
				//var theme = ThemeManager.Current.DetectTheme( Application.Current );
				//ThemeManager.Current.ChangeTheme( Application.Current, ThemeManager.Current.AddTheme( RuntimeThemeGenerator.Current.GenerateRuntimeTheme( theme.BaseColorScheme, selectedColor.Value.Value ) ) );
				//Application.Current?.MainWindow?.Activate();
				//C_UI_Manager.ForegroundColor = new SolidColorBrush( selectedColor.Value.Value ); ;
			}
		}
	}//Mainmanager 
}
