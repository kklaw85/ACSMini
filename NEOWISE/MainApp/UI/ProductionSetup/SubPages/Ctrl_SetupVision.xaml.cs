using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using NeoWisePlatform.Recipe;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.ProductionSetup.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_SetupVision : PageBase
	{
		public Ctrl_SetupVision()
		{
			#region Panel Lockable declaration
			this.MinRead = AccessLevel.Operator;
			this.MinWrite = AccessLevel.Engineer;
			#endregion
			this.InitializeComponent();
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
		private void OnLoadBinding()
		{
			try
			{
				Binding b = new Binding();
				b.Source = Recipes.HandlerRecipes();
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "RecipeName" );
				this.Recipename.SetBinding( TextBox.TextProperty, b );

				this.XHairFOV1.Source = this.Eq.Stage.Fov1;
				this.XHairFOV2.Source = this.Eq.Stage.Fov2;
				this.InspectFOV1.Source = this.Eq.Stage.Fov1;
				this.InspectFOV2.Source = this.Eq.Stage.Fov2;

				this.DisplayFOV1.Instrument = this.Eq.Stage.Fov1;
				this.DisplayFOV2.Instrument = this.Eq.Stage.Fov2;
				this.DisplayFOV1.Controllable = false;
				this.DisplayFOV2.Controllable = false;
				this.DisplayFOV1.ROIType = HiPA.Instrument.Camera.ROI.Inspect;
				this.DisplayFOV2.ROIType = HiPA.Instrument.Camera.ROI.Inspect;

				this.CollectiveInsp.Source = this.Eq.Stage;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private MTEquipment Eq = null;
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.Eq = Constructor.GetInstance().Equipment as MTEquipment;
				this.OnLoadBinding();
				//var MachineType = Constructor.GetInstance().MachineType;
				#region Lock UI Binding
				this.BindLockUI( this.DisplayFOV1 );
				this.BindLockUI( this.DisplayFOV2 );
				#endregion
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void ProductIdx_TextChanged( object sender, TextChangedEventArgs e )
		{
			this.OnLoadBinding();
		}
	}
}
