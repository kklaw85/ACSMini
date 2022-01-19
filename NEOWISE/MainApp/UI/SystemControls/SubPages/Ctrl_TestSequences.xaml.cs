using HiPA.Common;
using HiPA.Common.UControl;
using MahApps.Metro.Controls;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.SystemControls.SubPages
{
	/// <summary>
	/// Interaction logic for Ctrl_ManufacturerBypassOption.xaml
	/// </summary>
	public partial class Ctrl_TestSequences : PageBase
	{
		public Ctrl_TestSequences()
		{
			#region Panel Lockable declaration
			this.MachineStateLockable = false;
			this.MinRead = AccessLevel.Manufacturer;
			this.MinWrite = AccessLevel.Manufacturer;
			#endregion
			this.InitializeComponent();
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
		private void OnSetupBinding()
		{
			try
			{
				var b = new Binding();
				b.Source = this.Eq;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "NoIte" );
				this.Txt_Seq.SetBinding( NumericUpDown.ValueProperty, b );

				b = new Binding();
				b.Source = this.Eq;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "CurrIte" );
				this.Txt_CurSeq.SetBinding( NumericUpDown.ValueProperty, b );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}

		MTEquipment Eq = null;
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				#region Lock UI Binding
				this.BindLockUI( this.StkPnlCtrlLst );
				#endregion
				this.Eq = Constructor.GetInstance().Equipment as MTEquipment;
				this.OnSetupBinding();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( null, this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}





		private async void Button_Click( object sender, RoutedEventArgs e )
		{
			var btn = sender as Button;
			try
			{
				if ( btn == this.GRRTest )
				{
					await this.Eq.GRR();
				}
				else if ( btn == this.Dryrun )
				{
					await this.Eq.Dryrun();
				}
			}
			catch ( Exception ex )
			{
				this.Eq.StopTestSeq();
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}

		private void Stop_Click( object sender, RoutedEventArgs e )
		{
			var btn = sender as Button;
			try
			{
				this.Eq.StopTestSeq();
			}
			catch ( Exception ex )
			{
				this.Eq.StopTestSeq();
				Equipment.ErrManager.RaiseError( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation, ErrorClass.E6 );
			}
		}
	}
}
