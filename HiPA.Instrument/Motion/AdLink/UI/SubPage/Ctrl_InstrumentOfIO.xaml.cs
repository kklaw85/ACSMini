using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion.APS;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HiPA.Instrument.Motion.Adlink.UI.SubPage
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
		private IoBoardBase Board = null;
		public IoBoardBase Source
		{
			get => this.Board;
			set
			{
				this.Board = value;
				this.IOBoard.Source = this.Board;
				this.OnSetupBinding();
			}
		}
		private void OnSetupBinding()
		{
			try
			{
				this.InputGrid.ItemsSource = this.Board.GetChildren().Cast<AdLinkIoPoint>().Where( x => x.Configuration.Type == DioType.Input );
				this.OutputGrid.ItemsSource = this.Board.GetChildren().Cast<AdLinkIoPoint>().Where( x => x.Configuration.Type == DioType.Output );
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
