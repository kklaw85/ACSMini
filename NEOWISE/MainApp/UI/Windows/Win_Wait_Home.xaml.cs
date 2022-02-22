using HiPA.Common;
using NeoWisePlatform.Module;
using System;
using System.Threading.Tasks;
using System.Windows;


namespace NeoWisePlatform.UI.Windows
{
	/// <summary>
	/// Interaction logic for Win_Wait_Home.xaml
	/// </summary>
	public partial class Win_Wait_Home : Window
	{
		public Win_Wait_Home()
		{
			this.InitializeComponent();
		}
		public Task<ErrorResult> InitTask = null;
		protected override void OnContentRendered( EventArgs e )
		{
			base.OnContentRendered( e );
			this.InitTask?.Wait();
			this.InitTask = null;
			this.Close();
		}
	}
}
