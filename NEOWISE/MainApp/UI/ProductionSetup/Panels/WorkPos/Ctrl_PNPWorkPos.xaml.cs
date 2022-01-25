using HiPA.Common;
using HiPA.Common.UControl;
using MahApps.Metro.Controls;
using NeoWisePlatform.Module;
using NeoWisePlatform.Recipe;
using NeoWisePlatform.UI.CommonControls.Panels;
using System;
using System.ComponentModel;
using System.Windows;
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
						ctrl.Label.Style = ( Style )this.FindResource( "LB_ItemContent_Right" );
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
	}
}
