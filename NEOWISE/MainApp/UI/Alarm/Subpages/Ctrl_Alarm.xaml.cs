using HiPA.Common;
using HiPA.Common.UControl;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.Alarm.Subpages
{
	/// <summary>
	/// Interaction logic for Ctrl_Alarm.xaml
	/// </summary>
	public partial class Ctrl_Alarm : PageBase
	{
		public Ctrl_Alarm()
		{
			#region Panel Lockable declaration
			this.MinRead = AccessLevel.Guest;
			this.MinWrite = AccessLevel.Engineer;
			#endregion
			this.InitializeComponent();
			this.BindLockPrivilege( this.Btn_Clear );
			this.DataContext = Equipment.ErrManager;
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

		private void Grd_AlarmHistory_SelectedCellsChanged( object sender, SelectedCellsChangedEventArgs e )
		{
			try
			{
				var dg = sender as DataGrid;
				if ( dg == null || dg.SelectedIndex < 0 ) return;
				Equipment.ErrManager.SelectErr( dg.SelectedIndex );
			}
			catch ( Exception ex )
			{
			}
		}
		private void Btn_Clear_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				#region Clear Existing Binded Informations
				Equipment.ErrManager.ClearAlarm();
				#endregion
			}
			catch ( Exception ex )
			{
				new Thread( () => MessageBox.Show( ex.Message ) ).Start();
			}
		}

		private void PageBase_Loaded( object sender, RoutedEventArgs e )
		{
			this.Grd_AlarmHistory.ItemsSource = ( this.DataContext as ErrorManager ).AlarmList;
		}
	}
}
