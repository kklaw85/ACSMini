using HiPA.Common;
using HiPA.Common.UControl;
using System;
using System.Windows;
//using UNP_HASP_DLL;

namespace NeoWisePlatform.Production.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_Running_Auto.xaml
	/// </summary>
	public partial class Ctrl_AutoRunCommon : PanelBase
	{
		public Ctrl_AutoRunCommon()
		{
			this.InitializeComponent();
		}
		private void UserControl_Loaded_1( object sender, RoutedEventArgs e )
		{
			try
			{
				this.OnSetupBinding();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void OnSetupBinding()
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
