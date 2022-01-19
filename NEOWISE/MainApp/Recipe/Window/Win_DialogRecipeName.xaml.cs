using System.Windows;

namespace NeoWisePlatform.Recipe.Windows
{
	/// <summary>
	/// Interaction logic for Win_Pre_Init.xaml
	/// </summary>
	public partial class Win_DialogRecipeName : MahApps.Metro.Controls.MetroWindow
	{
		public enum DialogType
		{
			Rename,
			New,
			Copy,
		}
		public Win_DialogRecipeName()
		{
			this.InitializeComponent();
		}

		public DialogType UseType { get; set; } = DialogType.New;
		public string RecipeName { get; set; } = "";


		private void MetroWindow_Loaded_1( object sender, RoutedEventArgs e )
		{
			if ( this.UseType == DialogType.New )
				this.TitleBar.Content = "New Recipe";
			else if ( this.UseType == DialogType.Rename )
				this.TitleBar.Content = "Recipe Rename";
			else if ( this.UseType == DialogType.Copy )
				this.TitleBar.Content = "Copy Recipe";

			this.txtRecipeName.Text = this.RecipeName;
		}

		private void Btn_Ok_Click( object sender, RoutedEventArgs e )
		{
			var container = Recipes.HandlerRecipes();

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
					"Recipe Name",
					MessageBoxButton.OK );
				this.txtRecipeName.Focus();
				return;
			}
			if ( container.IsExists( name ) == true )
			{
				System.Windows.MessageBox.Show(
					this,
					$"The name [{name}] already exists",
					"Recipe Name",
					MessageBoxButton.OK );
				this.txtRecipeName.Focus();
				return;
			}

			this.RecipeName = name;
			this.DialogResult = true;
		}

		private void Btn_Cancel_Click( object sender, RoutedEventArgs e )
		{
			this.DialogResult = false;
		}
	}
}
