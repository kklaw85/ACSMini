using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Camera;
using System;
using System.Windows;
using System.Windows.Controls;

namespace NeoWisePlatform.UI.CommonControls.Panels
{


	/// <summary>
	/// Interaction logic for Ctrl_AxisJog.xaml
	/// </summary>
	public partial class Ctrl_DPAD : PanelBase
	{
		public Ctrl_DPAD()
		{
			this.InitializeComponent();
			try
			{
				this.ComboX.ItemsSource = DPAD.Interval;
				this.ComboY.ItemsSource = DPAD.Interval;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private MatroxCamera _Source;

		public MatroxCamera Source
		{
			set
			{
				try
				{
					this._Source = value;
					this.DataContext = value?.Configuration?.XHairPos;
					this.Lbl.Content = $"{this._Source?.Name} {( string )this.FindResource( "XHairPositioning" )}";
					this.OnSetupBinding();
				}
				catch ( Exception ex )
				{
					Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
				}
			}
		}

		private void OnSetupBinding()
		{
			try
			{
				this.InspectROI.ROIHandler = this._Source?.Camera?.Inspect;
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{ }
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		#region button X, Y, Z - up down left right leftUp, leftDown, rightUp, rightDown
		private void Btn_Stop_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				this._Source?.Camera.MoveLaserSpotCalToCenter();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
		#endregion

		private void Btn_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				var btn = sender as Button;
				var cmd = btn.Tag as string;
				var CMDs = cmd.Split( ',' );
				var FirstAction = ( CamDir )Enum.Parse( typeof( CamDir ), CMDs[ 0 ] );
				this._Source?.Camera.MoveXHair( FirstAction );
				if ( CMDs.Length == 2 )
				{
					var SecondAction = ( CamDir )Enum.Parse( typeof( CamDir ), CMDs[ 1 ] );
					this._Source?.Camera.MoveXHair( SecondAction );
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private async void Btn_AutoPositionXHair_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				await this._Source.CheckTeachPosition();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
	}
}
