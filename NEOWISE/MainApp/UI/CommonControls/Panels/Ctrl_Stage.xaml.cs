using HiPA.Common;
using HiPA.Common.UControl;
using MahApps.Metro.Controls;
using NeoWisePlatform.Module;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.UI.CommonControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_Stage : PanelBase
	{
		public Ctrl_Stage()
		{
			#region Panel Lockable declaration
			#endregion
			this.InitializeComponent();
			this.BindLockUI( this );
			this.RegisterExpanderPnl( this.Expander );
		}
		[Browsable( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		private StageClamper _Source = null;
		public StageClamper Source
		{
			get => this._Source;
			set
			{
				try
				{
					this._Source = value;
					this.DataContext = value;
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
				Binding b = new Binding();
				//b.Source = this._Source?._VacuumSuctionSense;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Value" );
				//b.Converter = new DioValueToBrushIORec();
				//this.SucI.SetBinding( Rectangle.FillProperty, b );

				//b = new Binding();
				//b.Source = this._Source?._VacuumSuction;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Value" );
				//b.Converter = new DioValueToBrushIORec();
				//this.SucO.SetBinding( Rectangle.FillProperty, b );

				//b = new Binding();
				//b.Source = this._Source?._InPos;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Value" );
				//b.Converter = new DioValueToBrushIORec();
				//this.InInPos.SetBinding( Rectangle.FillProperty, b );

				//b = new Binding();
				//b.Source = this._Source?._ResetPos;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Value" );
				//b.Converter = new DioValueToBrushIORec();
				//this.InResetPos.SetBinding( Rectangle.FillProperty, b );

				//b = new Binding();
				//b.Source = this._Source?._ToResetCylinder;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Value" );
				//b.Converter = new DioValueToBrushIORec();
				//this.OutResetPos.SetBinding( Rectangle.FillProperty, b );

				//b = new Binding();
				//b.Source = this._Source?._ToPosCylinder;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Value" );
				//b.Converter = new DioValueToBrushIORec();
				//this.OutInPos.SetBinding( Rectangle.FillProperty, b );

				//b = new Binding();
				//b.Source = this._Source?._MatSense;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Value" );
				//b.Converter = new DioValueToBrushIORec();
				//this.MatDetI.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration?.SuctionHold;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "TimeOut" );
				this.SuctionHoldTimeout.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration?.SuctionHold;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Delay" );
				this.SuctionHoldDelay.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration?.SuctionRelease;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "TimeOut" );
				this.SuctionReleaseTimeout.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration?.SuctionRelease;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Delay" );
				this.SuctionReleaseDelay.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source?.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Timeout" );
				this.ClamperTimeout.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this._Source;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ByPassSense" );
				this.ByPassSense.SetBinding( CheckBox.IsCheckedProperty, b );

				this.BindLockAccessLevelManufacturer( this.ReleaseCfg );
				this.BindLockAccessLevelManufacturer( this.HoldCfg );
				this.BindLockAccessLevelManufacturer( this.ClampCfg );
				this.BindLockAccessLevelManufacturer( this.ByPassSense );
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

		private async void Btn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				Equipment.MachStateMgr.MachineStatus = MachineStateType.BUSY;
				var btn = sender as Button;
				if ( btn == this.BtnClampHold )
				{
					var task = this._Source.ClampClose();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnClampRelease )
				{
					var task = this._Source.ClampOpen();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnSuctionOn )
				{
					var task = this._Source.SuctionOn();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnSuctionOff )
				{
					var task = this._Source.SuctionOff();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnStageHold )
				{
					var task = this._Source.Hold();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				else if ( btn == this.BtnStageRelease )
				{
					var task = this._Source.Release();
					await task;
					if ( task.Result.EClass != ErrorClass.OK ) throw new Exception( task.Result.ErrorMessage );
				}
				Equipment.MachStateMgr.RevertStateManualOp();
			}
			catch ( Exception ex )
			{
				Equipment.MachStateMgr.RevertStateManualOp();
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
			finally
			{
			}


		}
	}
}
