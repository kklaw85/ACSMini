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
		public bool DoInit { get; set; } = false;

		public Win_Wait_Home()
		{
			this.InitializeComponent();
		}
		public Task<ErrorResult> HomingTask = null;
		MTEquipment MTEquipment = Constructor.GetInstance().Equipment as MTEquipment;

		protected override void OnContentRendered( EventArgs e )
		{
			base.OnContentRendered( e );
			if ( this.DoInit )
			{
				var task = this.MTEquipment.Initialize();
				task.Wait();
			}
			if ( this.HomingTask == null )
				this.HomingTask = this.MTEquipment.HomeAxes();
			this.HomingTask.Wait();
			this.Close();
		}
	}
}
