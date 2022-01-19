using HiPA.Common;
using HiPA.Common.UControl;
using HiPA.Instrument.Motion;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Data;
using Validations;

namespace NeoWisePlatform.SystemControls.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_MotionProfile.xaml
	/// </summary>
	public partial class Ctrl_MotionProfile : PanelBase
	{
		public Ctrl_MotionProfile()
		{
			this.InitializeComponent();
			this.BindLockUI( this );
		}

		public string MotionProfileTitle
		{
			set { this.Lbl_MotionProfileTitle.Content = value; }
		}

		public MotionProfile Source
		{
			get => this.DataContext as MotionProfile;
			set
			{
				try
				{
					if ( value == null ) return;
					this.DataContext = value;
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
				#region Motion Profile
				Binding b = new Binding();
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Velocity" );
				Valid_Rule_Double d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 1000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_GenVel.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Acceleration" );
				d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 10000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_GenAcc.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "Deceleration" );
				d_RU = new Valid_Rule_Double();
				d_RU.Min = 0;
				d_RU.Max = 10000;
				b.ValidationRules.Clear();
				b.ValidationRules.Add( d_RU );
				this.Txt_GenDec.SetBinding( NumericUpDown.ValueProperty, b );
				#endregion

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{ }
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
