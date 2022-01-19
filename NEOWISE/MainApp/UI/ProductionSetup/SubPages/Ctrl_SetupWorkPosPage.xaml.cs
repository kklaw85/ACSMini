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
	public partial class Ctrl_SetupWorkPosPage : PageBase
	{
		public bool isMain { get; set; } = true;
		private MTEquipment equipment;
		public Ctrl_SetupWorkPosPage()
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
				this.PNPWorkPos.Source = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
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
				this.StageControl.Source = this.equipment.Stage.Stage;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
