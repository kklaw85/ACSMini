using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion;
using N_Data_Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.UI.CommonControls
{
	/// <summary>
	/// Interaction logic for Ctrl_AxisJogPage.xaml
	/// </summary>
	public partial class Ctrl_SingleAxisJog : PanelBase
	{
		public Ctrl_SingleAxisJog()
		{
			this.InitializeComponent();
			this.BindLockMachineState( this.Ctrl );
		}

		AxisBase _axis = null;
		public AxisBase Axis
		{
			get => this._axis;
			set
			{
				try
				{
					if ( this._axis == value || value == null ) return;
					this._axis = value;
					var config = value.Configuration;
					this.OnSetupBinding();
					this.LblAxisName.Content = $"{value.Name}";
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		public double[] Steps { get; set; }

		private void OnSetupBinding()
		{
			try
			{
				#region Configurations
				Binding b = new Binding();
				b.Source = this._axis.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ActualPosition" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				b.Mode = BindingMode.OneWay;
				this.Txt_Curr_Pos.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this._axis;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "StepMove" );
				this.Cb_StpMove.SetBinding( ComboBox.SelectedItemProperty, b );
				#endregion
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.Cb_StpMove.ItemsSource = AxisBase.Interval;
				this.Cb_StpMove.SelectedItem = AxisBase.Interval[ 0 ];
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}


		private async void Btn_Step_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				Equipment.MachStateMgr.MachineStatus = MachineStateType.BUSY;
				var btn = sender as Button;
				var dir = btn.Name.Contains( "Neg" ) ? -1.0 : 1.0;
				var TaskRelMove = this._axis.RelativeMove( new Trajectory( this._axis.Configuration.GeneralMove ) { Distance = dir * this._axis.StepMove } );
				await TaskRelMove;
				if ( TaskRelMove.Result != string.Empty )
					throw new Exception( TaskRelMove.Result );
				Equipment.MachStateMgr.RevertStateManualOp();
			}
			catch ( Exception ex )
			{
				Equipment.MachStateMgr.RevertStateManualOp();
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

	}
}
