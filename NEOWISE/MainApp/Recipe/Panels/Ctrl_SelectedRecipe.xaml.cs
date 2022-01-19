using HiPA.Common;
using HiPA.Common.UControl;
using System;
using System.Windows;

namespace NeoWisePlatform.Recipe.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_SelectedRecipe.xaml
	/// </summary>
	public partial class Ctrl_SelectedRecipe : PanelBase
	{
		public Ctrl_SelectedRecipe()
		{
			#region Panel Lockable declaration
			this.MachineStateLockable = false;
			this.PrivilegeLockable = false;
			#endregion
			this.InitializeComponent();
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.DataContext = Recipes.HandlerRecipes();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
