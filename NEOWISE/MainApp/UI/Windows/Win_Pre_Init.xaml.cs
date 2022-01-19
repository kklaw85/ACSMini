using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SystemInfo;

namespace NeoWisePlatform.Windows
{
	/// <summary>
	/// Interaction logic for Win_Pre_Init.xaml
	/// </summary>
	public partial class Win_Pre_Init : WindowBase
	{

		public bool IsRestart = false;
		private int InitLaserTypeIndex = -1;
		private int InitMachineTypeIndex = -1;

		private bool bTypeChanged = false;
		private bool TypeChanged
		{
			get => this.bTypeChanged;
			set
			{
				this.Set( ref this.bTypeChanged, value, "TypeChanged" );
				this.IsStartEnable = !this.bTypeChanged;
				this.IsRestartEnable = this.bTypeChanged;
			}
		}

		private bool IsRestartEnable
		{
			set
			{ //this.Btn_Restart.IsEnabled = value; 
			}
		}
		private bool IsStartEnable
		{
			set { this.Btn_Login.IsEnabled = value; }
		}
		public Win_Pre_Init()
		{
			this.InitializeComponent();
		}

		private void Btn_Exit_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = false;
		}

		private void Btn_Start_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = true;
		}

		private void MetroWindow_Loaded_1( object sender, RoutedEventArgs e )
		{
			this.Lbl_Log_In_Status.Visibility = Visibility.Collapsed;
			this.lbl_AppName.Text = C_System_info.GetApplicationName();
			this.Lbl_Software_Version.Text = "Version " + C_System_info.Software_Version;
			this.DataContext = ( Constructor.GetInstance().Equipment as MTEquipment );
			this.IsStartEnable = true;
		}

		private void Btn_Restart_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = false;
			this.IsRestart = true;
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

		private void Button_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var result = MTEquipment.UserMgt.Login( this.Txt_UserID.Text, this.PB_Password.Password );
				if ( result != "" )
				{
					this.Lbl_Log_In_Status.Visibility = Visibility.Visible;
					this.Lbl_Log_In_Status.Text = result;
				}
				else
				{
					this.DialogResult = true;
				}
			}
			catch ( Exception ex )
			{
			}
		}
	}
}
