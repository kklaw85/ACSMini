using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using NeoWisePlatform.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.SystemControls.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_InstrumentOfIO : PageBase
	{
		public bool isMain { get; set; } = false;
		public Ctrl_InstrumentOfIO()
		{
			try
			{
				#region Panel Lockable declaration
				this.MinRead = AccessLevel.Operator;
				this.MinWrite = AccessLevel.Manufacturer;
				#endregion
				this.InitializeComponent();
				#region Lock UI Binding
				this.BindLockUI( this.IOBoard );
				this.BindLockUI( this.InputGrid );
				this.BindLockUI( this.OutputGrid );
				#endregion
				this.Board = ( Constructor.GetInstance().Equipment as MTEquipment ).IoBoard as APSIoBoard;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
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
		private List<AdLinkIoPoint> InPoints = new List<AdLinkIoPoint>();
		private List<AdLinkIoPoint> OutPoints = new List<AdLinkIoPoint>();
		private APSIoBoard Board { get; set; } = null;
		private void OnSetupBinding()
		{
			try
			{
				this.InPoints.Clear();
				foreach ( AdLinkIoPoint point in this.Board.GetChildren().Cast<AdLinkIoPoint>().Where( x => x.Configuration.Type == DioType.Input ) )
				{
					this.InPoints.Add( point );
				}
				this.OutPoints.Clear();
				foreach ( AdLinkIoPoint point in this.Board.GetChildren().Cast<AdLinkIoPoint>().Where( x => x.Configuration.Type == DioType.Output ) )
				{
					this.OutPoints.Add( point );
				}
				this.InputGrid.ItemsSource = this.InPoints;
				this.OutputGrid.ItemsSource = this.OutPoints;

				this.IOBoard.Source = this.Board;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
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
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void Btn_ToggleOutput_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var CurrentState = ( this.OutputGrid.CurrentItem as AdLinkIoPoint ).Value;
				( this.OutputGrid.CurrentItem as AdLinkIoPoint ).SetOut( CurrentState == DioValue.On ? DioValue.Off : DioValue.On );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.Board, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
	}
}
