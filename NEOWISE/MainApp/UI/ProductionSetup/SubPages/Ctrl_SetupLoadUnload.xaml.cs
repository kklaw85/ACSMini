using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using NeoWisePlatform.Recipe;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.ProductionSetup.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_SetupLoadUnload : PageBase
	{
		public bool isMain { get; set; } = true;
		private MTEquipment equipment;
		public Ctrl_SetupLoadUnload()
		{
			#region Panel Lockable declaration
			this.MinRead = AccessLevel.Operator;
			this.MinWrite = AccessLevel.Engineer;
			#endregion
			this.InitializeComponent();
			this.BindRecipeIndex( Recipes.HandlerRecipes() );
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

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.equipment = ( Constructor.GetInstance()?.Equipment as MTEquipment );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
