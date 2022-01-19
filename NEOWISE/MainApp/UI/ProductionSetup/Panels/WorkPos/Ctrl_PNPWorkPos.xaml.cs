using HiPA.Common;
using HiPA.Common.UControl;
using MahApps.Metro.Controls;
using NeoWisePlatform.Module;
using NeoWisePlatform.Recipe;
using NeoWisePlatform.UI.CommonControls.Panels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.UI.ProductionSetup.Panels.WorkPos
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_PNPWorkPos : PanelBase
	{
		public Ctrl_PNPWorkPos()
		{
			#region Panel Lockable declaration
			#endregion
			this.InitializeComponent();
			this.BindLockUI( this );
		}
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		private PNPPos _Source = null;
		public PNPPos Source
		{
			get => this._Source;
			set
			{
				try
				{
					if ( value == null ) return;
					this._Source = value;
					this.OnSetupBinding();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}



		private void OnSetupBinding()
		{
			try
			{
				//this.OnSetupBinding( this._Source, this.Content );
				this.Entries.Clear();
				this.Content.Children.Clear();
				if ( this._Source == null ) return;
				var Properties = this._Source?.GetType().GetProperties();
				var pnpmodule = ( Constructor.GetInstance().Equipment as MTEquipment ).PNP;
				foreach ( var prop in Properties )
				{
					if ( prop.PropertyType == typeof( double ) )
					{
						var ctrl = new Ctrl_EntryMotion();
						ctrl.Name = prop.Name;
						ctrl.Label.Content = ( string )this.FindResource( prop.Name );
						ctrl.Label.Style = ( Style )this.FindResource( "STYL_GENERAL_LBL_Right" );
						ctrl.Label.Width = 100;
						ctrl.Entry.Width = 80;
						ctrl.Entry.StringFormat = "F3";
						ctrl.Margin = new Thickness( 0, 0, 0, 5 );
						ctrl.PNPModule = pnpmodule;
						Binding b = new Binding();
						b.Source = this._Source;
						b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
						b.Path = new PropertyPath( prop.Name );
						ctrl.Entry.SetBinding( NumericUpDown.ValueProperty, b );
						if ( !this.Entries.ContainsKey( prop.Name ) )
						{
							this.Entries[ prop.Name ] = null;
							this.Content.Children.Add( ctrl );
						}
					}
				}
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
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}

		}

		private void BtnMoveClick( object sender, RoutedEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				double X = 0, Y = 0, Z = 0, ZConveyor = 0;

				//var Pos = this._LHModule.GetLHPosition();
				//var Saved = ( this.Grd_Positions.CurrentItem as Pos );

				//if ( btn.Name.Contains( "Apply" ) )
				//{
				//	if ( btn.Name.Contains( "Laser" ) )
				//	{
				//		Saved.LMPos.X = Pos.X;
				//		Saved.LMPos.Y = Pos.Y;
				//		Saved.LMPos.Z = Pos.Z;
				//	}
				//}
				//else if ( btn.Name.Contains( "Move" ) )
				//{
				//	if ( btn.Name.Contains( "Laser" ) )
				//	{
				//		var TrajectX = new Trajectory( this._LHModule.AxisX.Configuration.CommandedMove ) { Position = Saved.LMPos.X };
				//		var tsk1 = this._LHModule.AxisX.AbsoluteMove( TrajectX );
				//		var TrajectY = new Trajectory( this._LHModule.AxisY.Configuration.CommandedMove ) { Position = Saved.LMPos.Y };
				//		var tsk2 = this._LHModule.AxisY.AbsoluteMove( TrajectY );
				//		var TrajectZ = new Trajectory( this._LHModule.AxisZ.Configuration.CommandedMove ) { Position = Saved.LMPos.Z };
				//		var tsk3 = this._LHModule.AxisZ.AbsoluteMove( TrajectZ );
				//		await tsk1;
				//		if ( tsk1.Result != string.Empty )
				//			throw new Exception( tsk1.Result );
				//		await tsk2;
				//		if ( tsk2.Result != string.Empty )
				//			throw new Exception( tsk2.Result );
				//		await tsk3;
				//		if ( tsk3.Result != string.Empty )
				//			throw new Exception( tsk3.Result );
				//		( Constructor.GetInstance().Equipment as MTEquipment ).WorkingStation = this._StationModule;
				//	}
				//}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
			finally
			{ }
		}

		private void Grd_Positions_CellEditEnding( object sender, DataGridCellEditEndingEventArgs e )
		{
			try
			{
				var rowno = e.Row.GetIndex();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private void btnAdd_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				//check product type
				var Recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe();

				//var Pos = this._LHModule.GetLHPosition();
				//var existing = this.o_WorkPosStation.Where( y => y.LMPos.X == Pos.X && y.LMPos.Y == Pos.Y && y.LMPos.Z == Pos.Z );

				//if ( existing.Count() > 0 )
				//	throw new Exception( "Existing teached position found" );

				//int iiD = 1;
				//if ( this.o_WorkPosStation.Count > 0 )
				//	iiD = this.o_WorkPosStation[ this.o_WorkPosStation.Count - 1 ].ID + 1;

				//this.o_WorkPosStation.Add( new Pos()
				//{
				//	ID = iiD,
				//	LMPos = new LMPos()
				//	{
				//		X = Pos.X,
				//		Y = Pos.Y,
				//		Z = Pos.Z,
				//	},
				//} );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}


		private void btnDelete_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				int index = -1;
				//foreach ( DataGridCellInfo cell in this.Grd_Positions.SelectedCells )
				//{
				//	index = this.Grd_Positions.Items.IndexOf( cell.Item );
				//}

				//if ( index > -1 )
				//	this.o_WorkPosStation.RemoveAt( index );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
