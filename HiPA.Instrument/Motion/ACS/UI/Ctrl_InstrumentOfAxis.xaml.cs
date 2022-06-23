using HiPA.Common;
using HiPA.Common.UControl;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HiPA.Instrument.Motion.ACS.UI
{
	/// <summary>
	/// Interaction logic for Ctrl_X_Motor.xaml
	/// </summary>
	public partial class Ctrl_InstrumentOfAxis : PanelBase
	{
		public Ctrl_InstrumentOfAxis()
		{
			try
			{
				this.InitializeComponent();
				this.BindLockUI( this );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private AxisBase o_Source = null;
		public AxisBase Source
		{
			get => this.o_Source;
			set
			{
				try
				{
					if ( value == null ) return;
					this.o_Source = value;
					this.InitBar.Instrument = value;
					this.MotionProfile_GenMove.DataContext = value.Configuration.GeneralMove;
					this.MotionProfile_CmdMove.DataContext = value.Configuration.CommandedMove;
					this.AxisGenCfg.DataContext = value?.Configuration;
					this.AxisStatus.DataContext = value?.Status;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private async void Btn_Move_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var trajectory = this.UseGenProfile ? new Trajectory( this.o_Source.Configuration.GeneralMove ) : new Trajectory( this.o_Source.Configuration.CommandedMove );
				trajectory.Position = this.TargetPos;
				var TaskAbsMove = this.o_Source.AbsoluteMove( trajectory );
				await TaskAbsMove;
				if ( TaskAbsMove.Result != string.Empty )
					throw new Exception( TaskAbsMove.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		private async void Btn_Stp_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				var trajectory = this.UseGenProfile ? new Trajectory( this.o_Source.Configuration.GeneralMove ) : new Trajectory( this.o_Source.Configuration.CommandedMove );
				trajectory.Distance = ( btn.Name == "Btn_Stp_Neg" ) ? this.StpMove * -1 : this.StpMove;
				var TaskRelMove = this.o_Source.RelativeMove( trajectory );
				await TaskRelMove;
				if ( TaskRelMove.Result != string.Empty )
					throw new Exception( TaskRelMove.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private async void Btn_Vel_MouseDown( object sender, MouseButtonEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				var dir = ( btn.Name == "Btn_Vel_Neg" ) ? -1.0 : 1.0;
				var trajectory = this.UseGenProfile ? new Trajectory( this.o_Source.Configuration.GeneralMove ) : new Trajectory( this.o_Source.Configuration.CommandedMove );
				trajectory.Velocity *= dir;

				var TaskVelMove = this.o_Source.VelocityMove( trajectory );
				await TaskVelMove;
				if ( TaskVelMove.Result != string.Empty )
					throw new Exception( TaskVelMove.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		private void Btn_Vel_MouseUp( object sender, MouseButtonEventArgs e )
		{
			try
			{
				var sErr = this.o_Source.StopMove();
				if ( sErr != string.Empty )
					throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		private async void Btn_SvrHM_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var TaskHoming = this.o_Source.Homing();
				await TaskHoming;
				if ( TaskHoming.Result != string.Empty ) throw new Exception( TaskHoming.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		private async void Btn_SvrSW_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var TaskServOn = this.o_Source.ServoOn( this.o_Source.Status.SVON == false );
				await TaskServOn;
				if ( TaskServOn.Result != string.Empty )
					throw new Exception( TaskServOn.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		#region UI Components
		private double TargetPos
		{
			get => ( double )this.Txt_TgtMov.Value;
			set => this.Txt_TgtMov.Value = value;
		}
		private double StpMove
		{
			get => ( double )this.Txt_StpMov.Value;
			set => this.Txt_StpMov.Value = value;
		}

		private bool UseGenProfile
		{
			get => ( bool )this.Rdo_UseGen.IsChecked;
		}
		#endregion

		private async void Btn_Initialize_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var tsk = this.o_Source.Initialize();
				await tsk;
				if ( tsk.Result != string.Empty )
					throw new Exception( tsk.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private async void Btn_Stop_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var tsk = this.o_Source.Stop();
				await tsk;
				if ( tsk.Result != string.Empty )
					throw new Exception( tsk.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
	}
}
