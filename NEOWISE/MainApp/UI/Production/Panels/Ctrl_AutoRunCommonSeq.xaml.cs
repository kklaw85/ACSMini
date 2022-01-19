using HiPA.Common;
using HiPA.Common.UControl;
using NeoWisePlatform.Module;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NeoWisePlatform.Production.Panels
{
	/// <summary>
	/// Interaction logic for Ctrl_AutoStatistical.xaml
	/// </summary>
	public partial class Ctrl_AutoRunCommonSeq : PanelBase
	{
		public Ctrl_AutoRunCommonSeq()
		{
			this.InitializeComponent();
		}
		private MTEquipment Eq = null;
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
			try
			{
				this.Eq = Constructor.GetInstance().Equipment as MTEquipment;
				this.OnSetupBinding();
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
				b.Source = this.Eq?.AutoSeq;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "SeqStep" );
				this.AutoSeq.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this.Eq?.PNP.Seq?.PNPSeq;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "SeqStep" );
				this.PNPSeq.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this.Eq?.Stage.Seq;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "SeqStep" );
				this.StageSeq.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this.Eq?.NewLift.Seq;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "SeqStep" );
				this.NewLiftSeq.SetBinding( Label.ContentProperty, b );

				b = new Binding();
				b.Source = this.Eq?.QICLift.Seq;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				b.Path = new PropertyPath( "SeqStep" );
				this.QICLiftSeq.SetBinding( Label.ContentProperty, b );
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
	}
}
