﻿using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.UControl;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HiPA.Instrument.Motion.ACS.UI.SubPage
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_InstrumentOfMotion : PageBase
	{
		public Ctrl_InstrumentOfMotion()
		{
			#region Panel Lockable declaration
			this.MinRead = AccessLevel.Operator;
			this.MinWrite = AccessLevel.Manufacturer;
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
		private void OnSetupBinding()
		{
			try
			{ }
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private ACSMotionBoard Source { get; set; } = null;

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				if ( !this.IsLoaded ) return;
				this.Source = Constructor.GetInstance().GetInstrument( ACSBoardConfiguration.NAME, null ) as ACSMotionBoard;
				this.AxisBoard.Source = this.Source;
				foreach ( var axis in this.Source.GetChildren() )
					this.lstAxesList.AddItem( "[A] " + axis.Name, axis );
				if ( this.lstAxesList.Items.Count > 0 )
					this.lstAxesList.SelectedIndex = 0;
				else
					this.AxisConfig.Visibility = Visibility.Collapsed;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void lstAxesList_SelectionChanged( object sender, SelectionChangedEventArgs e )
		{
			try
			{
				var axis = this.lstAxesList.SelectedValue<AxisBase>();
				this.AxisConfig.Source = axis as ACSAxis;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
