using System.Windows;

namespace NeoWisePlatform.Recipe.Windows
{
	/// <summary>
	/// Interaction logic for Win_Pre_Init.xaml
	/// </summary>
	public partial class Win_DialogProductName : MahApps.Metro.Controls.MetroWindow
	{
		public enum DialogType
		{
			Rename,
			New,
			Copy,
		}
		public Win_DialogProductName()
		{
			this.InitializeComponent();
		}

		public DialogType UseType { get; set; } = DialogType.New;
		public string ProductName { get; set; } = "";

		private void MetroWindow_Loaded_1( object sender, RoutedEventArgs e )
		{
			if ( this.UseType == DialogType.New )
				this.TitleBar.Content = "New Product";
			else if ( this.UseType == DialogType.Rename )
				this.TitleBar.Content = "Product Rename";
			else if ( this.UseType == DialogType.Copy )
				this.TitleBar.Content = "Copy Product";

			this.txtRecipeName.Text = this.ProductName;
			//this.Cb_ProductType.ItemsSource = Enum.GetValues( typeof( ProductType ) ).Cast<ProductType>();
			//this.Cb_ProductType.SelectedIndex = 0;
		}

		private void Btn_Ok_Click( object sender, RoutedEventArgs e )
		{
			var container = Recipes.HandlerRecipes();
			var Recipe = container?.GetAppliedRecipe() as RecipeItemHandler;
			var name = this.txtRecipeName.Text.Trim();
			if ( !container.CheckNameValidation( name ) )
			{
				var chars = "";
				foreach ( var ch in container._invalidNameChars )
				{
					chars += ch + " ";
				}

				System.Windows.MessageBox.Show(
					this,
					$"Name: [{name}]\r\nHave some invalid chars\r\nInvalid: {chars}",
					"Product Name",
					MessageBoxButton.OK );
				this.txtRecipeName.Focus();
				return;
			}
			//this.ProductType = ( ProductType )this.Cb_ProductType.SelectedItem;
			this.ProductName = name;
			this.DialogResult = true;
		}

		private void Btn_Cancel_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = false;
		}
	}
}
