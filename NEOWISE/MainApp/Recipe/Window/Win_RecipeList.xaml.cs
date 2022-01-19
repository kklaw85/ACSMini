using HiPA.Common;
using HiPA.Common.UControl;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace NeoWisePlatform.Recipe.Windows
{
	/// <summary>
	/// Interaction logic for Win_RecipeList.xaml
	/// </summary>
	public partial class Win_RecipeList : WindowBase
	{

		public Win_RecipeList()
		{
			this.InitializeComponent();
			this.DataContext = Recipes.HandlerRecipes();
			this.lvwRecipeList.ItemsSource = Recipes.HandlerRecipes()?.RecipeDir;
			Recipes.HandlerRecipes().RecipeAppliedEvent += this.Win_RecipeList_RecipeAppliedEvent;
		}

		private void Win_RecipeList_RecipeAppliedEvent( object sender, RecipeItemChangedEventArgs e )
		{
			try
			{
				Recipes.HandlerRecipes()?.RefreshList();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private bool b_IsShowExisBtn = true;
		public bool IsShowExitBtn
		{
			get => this.b_IsShowExisBtn;
			set => this.Set( ref this.b_IsShowExisBtn, value, "IsShowExitBtn" );
		}

		private void OnSetupBinding()
		{
			try
			{
				var b = new Binding() { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath( "IsShowExitBtn" ), Converter = new BooleanToVisibilityConverter() };
				this.btnExit.SetBinding( Button.VisibilityProperty, b );
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
				this.OnSetupBinding();
				Recipes.HandlerRecipes().RefreshList();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void lvwRecipeList_MouseDoubleClick( object sender, MouseButtonEventArgs e )
		{
			try
			{
				Recipes.HandlerRecipes()?.ApplySelectedRecipe();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void BtnRecipe_Clicked( object sender, RoutedEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				string tag = btn.Tag.ToString();
				if ( tag.Contains( "Apply" ) )
					Recipes.HandlerRecipes()?.ApplySelectedRecipe();
				else if ( tag.Contains( "Refresh" ) )
					Recipes.HandlerRecipes()?.RefreshList();
				else if ( tag.Contains( "New" ) )
					Recipes.HandlerRecipes()?.CreateRecipePrompt();
				else if ( tag.Contains( "Copy" ) )
					Recipes.HandlerRecipes()?.CopyRecipePrompt();
				else if ( tag.Contains( "Remove" ) )
					Recipes.HandlerRecipes()?.RemoveRecipePrompt();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				return;
			}

		}

		private bool CheckSelection()
		{
			try
			{
				if ( string.IsNullOrEmpty( Recipes.HandlerRecipes()?.RecipeName ) )
				{
					System.Windows.MessageBox.Show( "Please select Recipe for data configuration", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error );
					return false;
				}
				return true;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				return false;
			}
		}

		private void BtnSubmit_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				if ( this.CheckSelection() )
				{
					this.DialogResult = true;
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void WindowBase_Closing( object sender, CancelEventArgs e )
		{
			e.Cancel = !this.CheckSelection();
		}

		private void BtnExit_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				MessageBoxResult msgBoxRslt = System.Windows.MessageBox.Show( "Are you sure to exit from this application?", "EXIT", MessageBoxButton.YesNo, MessageBoxImage.Question );
				if ( msgBoxRslt == MessageBoxResult.Yes )
				{
					this.DialogResult = false;
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}

	public class SortAdorner : Adorner
	{
		private static Geometry ascGeometry =
			Geometry.Parse( "M 0 4 L 3.5 0 L 7 4 Z" );

		private static Geometry descGeometry =
			Geometry.Parse( "M 0 0 L 3.5 4 L 7 0 Z" );

		public ListSortDirection Direction { get; private set; }

		public SortAdorner( UIElement element, ListSortDirection dir )
			: base( element )
		{
			this.Direction = dir;
		}

		protected override void OnRender( DrawingContext drawingContext )
		{
			base.OnRender( drawingContext );

			if ( this.AdornedElement.RenderSize.Width < 20 )
				return;

			TranslateTransform transform = new TranslateTransform
				(
					this.AdornedElement.RenderSize.Width - 15,
					( this.AdornedElement.RenderSize.Height - 5 ) / 2
				);
			drawingContext.PushTransform( transform );

			Geometry geometry = ascGeometry;
			if ( this.Direction == ListSortDirection.Descending )
				geometry = descGeometry;
			drawingContext.DrawGeometry( Brushes.Cyan, null, geometry );

			drawingContext.Pop();
		}
	}
}
