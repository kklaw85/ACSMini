using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion;
using N_Data_Utilities;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace NeoWisePlatform.UI.CommonControls
{
	/// <summary>
	/// Interaction logic for Ctrl_AxisJogPage.xaml
	/// </summary>
	public partial class Ctrl_AxisJogPage : PanelBase
	{
		public Ctrl_AxisJogPage()
		{
			this.InitializeComponent();
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

					//Set Binding
					this.OnSetupBinding();

					//Set Axis Label Name
					this.LblAxisName.Content = $"{value.Name}";
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

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
				this.Cb_VelMove.ItemsSource = AxisBase.Interval;
				this.Cb_VelMove.SelectedItem = AxisBase.Interval[ 0 ];
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}


		private int _isVelocityMoving = 0;
		public bool IsVelocityMoving
		{
			get => Interlocked.CompareExchange( ref this._isVelocityMoving, 1, 1 ) == 1;
			set => Interlocked.Exchange( ref this._isVelocityMoving, value ? 1 : 0 );
		}

		private void Btn_Vel_MouseUp( object sender, MouseButtonEventArgs e )
		{
			try
			{
				string sErr = string.Empty;
				sErr = this._axis.StopMove();
				if ( sErr != string.Empty )
					throw new Exception( sErr );

				this.IsVelocityMoving = false;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private async void Btn_Vel_MouseDown( object sender, MouseButtonEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				var dir = btn.Name.Contains( "Neg" ) ? -1.0 : 1.0;

				if ( ( double )this.Cb_VelMove.SelectedItem == 0 )
					return;
				this.IsVelocityMoving = true;
				var speed = ( double )this.Cb_VelMove.SelectedItem * dir;

				var TaskVelMove = this._axis.VelocityMove( new Trajectory( this._axis.Configuration.CommandedMove ) { Velocity = speed } );
				await TaskVelMove;
				if ( TaskVelMove.Result != string.Empty )
					throw new Exception( TaskVelMove.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private async void Btn_Move_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var trajectory = new Trajectory( this._axis.Configuration.GeneralMove )
				{
					Position = ( double )this.Txt_Tgt_Pos.Value,
				};

				var TaskAbsMove = this._axis.AbsoluteMove( trajectory );
				await TaskAbsMove;
				if ( TaskAbsMove.Result != string.Empty )
					throw new Exception( TaskAbsMove.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private void Rdo_Step_Checked( object sender, RoutedEventArgs e )
		{
			try
			{
				if ( this.Cb_StpMove == null || this.Cb_VelMove == null ) return;
				this.Cb_StpMove.IsEnabled = ( bool )this.Rdo_Step.IsChecked;
				this.Cb_VelMove.IsEnabled = ( bool )!this.Rdo_Step.IsChecked;
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
				var btn = sender as Button;
				var dir = btn.Name.Contains( "Neg" ) ? -1.0 : 1.0;
				if ( ( double )this.Cb_StpMove.SelectedItem == 0 )
					return;
				var step = ( double )this.Cb_StpMove.SelectedItem * dir;

				var TaskRelMove = this._axis.RelativeMove( new Trajectory( this._axis.Configuration.GeneralMove ) { Distance = step } );
				await TaskRelMove;
				if ( TaskRelMove.Result != string.Empty )
					throw new Exception( TaskRelMove.Result );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this._axis, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

	}
}
