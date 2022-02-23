using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using MahApps.Metro.Controls;
using N_Data_Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using Validations;

namespace NeoWisePlatform.UI.SystemControls.Panels.Motion
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
				this.Cb_AxisNegDir.ItemsSource = Enum.GetValues( typeof( NegativeDirection ) ).Cast<NegativeDirection>();
				this.Cb_AxisNegDir.SelectedItem = NegativeDirection.Left;
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
					this.OnSetupBinding();
					this.HMProfileAD.Source = ( this.o_Source as AdLinkAxis ).Configuration.HomingProfile;
					this.MotionProfile_GenMove.Source = value.Configuration.GeneralMove;
					this.MotionProfile_CmdMove.Source = value.Configuration.CommandedMove;
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private void OnSetupBinding()
		{
			try
			{
				#region Configurations
				Binding b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "AxisId" );
				Valid_Rule_INT RU = new Valid_Rule_INT();
				RU.Min = 0;
				RU.Max = 1600;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( RU );
				this.AxisID.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ModuleNo" );
				RU = new Valid_Rule_INT();
				RU.Min = 0;
				RU.Max = 255;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( RU );
				this.Txt_ModuleNo.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "MotionScale" );
				var RD = new Valid_Rule_Double();
				RD.Min = 0;
				RD.Max = 50000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( RD );
				this.Txt_MotionScale.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "MaxAcceleration" );
				Valid_Rule_Double d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 20000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_MaxAcc.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "MaxDeceleration" );
				d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 20000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_MaxDec.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "MaxVelocity" );
				d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 20000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_MaxVel.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "UpperPosLimit" );
				this.Txt_PosUSL.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "LowerPosLimit" );
				this.Txt_PosLSL.SetBinding( NumericUpDown.ValueProperty, b );

				//Cmb Box
				b = new Binding();
				b.Source = this.o_Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "NegativeDirection" );
				this.Cb_AxisNegDir.SetBinding( ComboBox.SelectedItemProperty, b );

				//b = new Binding();
				//b.Source = this.o_Source.Configuration;
				//b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				//b.Path = new PropertyPath( "Units" );
				//this.Cb_MotionUnit.SetBinding( ComboBox.SelectedItemProperty, b );
				#endregion

				#region Axis Status
				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "CommandPosition" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.Txt_Cmd_Pos.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ActualPosition" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.Txt_FeedBack_Pos.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ErrorPosition" );
				b.Converter = new DisplayThreeDecPlacesDouble();
				this.Txt_Err_Pos.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "PEL" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_PEL.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "HM" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_HM.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "NEL" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_MEL.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "INP" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_INP.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "NSTP" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_NST.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "CSTP" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_CSTP.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ASTP" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_ASTP.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ACC" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_ACC.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "DEC" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_DEC.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "VM" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_VM.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "ALM" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_SVRALM.SetBinding( Rectangle.FillProperty, b );

				b = new Binding();
				b.Source = this.o_Source.Status;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "SVON" );
				b.Converter = new BoolToBrushIORec();
				this.Rec_SVRSW.SetBinding( Rectangle.FillProperty, b );
				#endregion
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.o_Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
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
