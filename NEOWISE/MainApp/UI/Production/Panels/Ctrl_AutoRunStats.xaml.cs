using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.Production.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_AutoStatistical.xaml
	/// </summary>
	public partial class Ctrl_AutoRunStats : PanelBase
	{
		public Ctrl_AutoRunStats()
		{
			this.InitializeComponent();
		}
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private Statistic _Source = null;
		public Statistic Source
		{
			set
			{
				this._Source = value;
				this.DataContext = this._Source;
			}
		}
		private void Button_Click( object sender, RoutedEventArgs e )
		{
			var btn = sender as Button;
			if ( btn == this.Reset )
				this._Source.Reset();
		}
	}
}
