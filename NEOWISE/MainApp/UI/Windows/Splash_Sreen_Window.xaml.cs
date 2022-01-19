using System.ComponentModel;
using System.Windows;
using SystemInfo;

namespace NeoWisePlatform.Windows
{
	/// <summary>
	/// Interaction logic for Splash_Sreen_Window.xaml
	/// </summary>
	public partial class Splash_Sreen_Window : Window
	{

		BackgroundWorker BckWorker;
		public Splash_Sreen_Window()
		{
			this.InitializeComponent();
		}



		private void Window_Loaded( object sender, RoutedEventArgs e )
		{
			this.lbl_AppName.Text = C_System_info.GetApplicationName();
			this.Lbl_Software_Version.Text = "Version " + C_System_info.Software_Version;
			this.BckWorker = new BackgroundWorker();
			this.BckWorker.DoWork += this.worker_DoWork;
			this.BckWorker.RunWorkerCompleted += this.worker_RunWorkerCompleted;
			this.BckWorker.RunWorkerAsync();
		}

		private void worker_DoWork( object sender, DoWorkEventArgs e )
		{
			C_Main_Manager.Init_Main_Manager();
			//if (sErr != C_Shared_Data.ErrorMap.NO_ERROR)
			//    MsgHandler.Process_Error(sErr + Environment.NewLine + C_Shared_Data.ErrorMap.SYS_INIT_MAIN_MANAGER_ERR);

		}

		private void worker_RunWorkerCompleted( object sender,
											   RunWorkerCompletedEventArgs e )
		{
			//update ui once worker complete his work
			this.DialogResult = true;
		}




	}
}
