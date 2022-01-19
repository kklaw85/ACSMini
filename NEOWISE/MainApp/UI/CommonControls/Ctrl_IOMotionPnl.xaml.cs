using HiPA.Common;
using HiPA.Common.UControl;
using MahApps.Metro.Controls;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.CommonControls
{
	/// <summary>
	/// Interaction logic for Ctrl_AxisJogPage.xaml
	/// </summary>
	public partial class Ctrl_IOMotionPnl : PanelBase
	{
		public Ctrl_IOMotionPnl()
		{
			this.InitializeComponent();
			this.BindLockUI( this );
		}

		private IOMotion _Source = null;
		public IOMotion Source
		{
			get => this._Source;
			set
			{
				try
				{
					if ( this._Source == value || value == null ) return;
					this._Source = value;
					this.IOsStatus.Source = value;
					//Set Binding
					this.OnSetupBinding();

					//Set Axis Label Name
					this.LblAxisName.Content = $"{value.Name}";
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private void OnSetupBinding()
		{
			try
			{
				Binding b = new Binding();
				b.Source = this._Source.Configuration;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Timeout" );
				this.Timeout.SetBinding( NumericUpDown.ValueProperty, b );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private async void Btn_Move_Click( object sender, RoutedEventArgs e )
		{
			ErrorClass EClass = ErrorClass.OK;
			try
			{
				var btn = sender as Button;
				if ( btn == this.Btn_Move )
				{
					var tsk = this._Source.MoveToPos();
					await tsk;
					if ( ( EClass = tsk.Result.EClass ) != ErrorClass.OK )
						throw new Exception( tsk.Result.ErrorMessage );
				}
				else if ( btn == this.Btn_Reset )
				{
					var tsk = this._Source.Reset();
					await tsk;
					if ( ( EClass = tsk.Result.EClass ) != ErrorClass.OK )
						throw new Exception( tsk.Result.ErrorMessage );
				}
			}
			catch ( Exception ex )
			{
				if ( EClass == ErrorClass.OK )
					EClass = ErrorClass.E6;
				Equipment.ErrManager.RaiseError( this._Source, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, EClass );
			}
		}
	}
}
