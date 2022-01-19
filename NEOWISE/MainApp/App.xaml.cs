using HiPA.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using WPFSharp.Globalizer;

namespace NeoWisePlatform
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : GlobalizedApplication
	{
		//string Err = "";
		public static MainWindow _MainWindow;
		void App_Startup( object sender, StartupEventArgs e )
		{
			// Application is running                        
			base.Init();
			Process thisProc = Process.GetCurrentProcess();
			if ( Process.GetProcessesByName( thisProc.ProcessName ).Length > 1 )
			{
				Application.Current.Shutdown();
				return;
			}

			//Only Activate in run time

			this.SetLanguageDictionary();

			Timeline.DesiredFrameRateProperty.OverrideMetadata( typeof( Timeline ),
															   new FrameworkPropertyMetadata { DefaultValue = 14 } );

			//For Main Application
			_MainWindow = new MainWindow();
			try
			{
				_MainWindow.Show();
			}
			catch
			{
				//MessageBox.Show( ex.Message, "Error In Startup" );
				App._MainWindow.Close();
			}
		}

		private void SetLanguageDictionary()
		{

			string strdir = "";
			ResourceDictionary dict = new ResourceDictionary();

			strdir = Path.Combine( Constructor.GetInstance().SystemFilesPath, "RT_Eng.xaml" );
			dict.Source = new Uri( strdir, UriKind.Absolute );
			this.Resources.MergedDictionaries.Add( dict );
		}
	}
}
