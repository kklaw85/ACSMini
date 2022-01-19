using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HiPA.Communicator.Forms
{
	/// <summary>
	/// Interaction logic for Ctrl_SerialComSet.xaml
	/// </summary>
	public partial class Ctrl_SerialComSet : UserControl
	{
		public Ctrl_SerialComSet()
		{
			this.InitializeComponent();
		}

		private Win_SerialComSet Win_SerialComSet_Dlg = null;

		private SerialPortParameter _SerialComParam = null;
		public SerialPortParameter SerialComParam
		{
			get => this._SerialComParam;
			set
			{
				if ( value == null ) return;
				this._SerialComParam = value;
				this.OnSetupBinding();
			}
		}
		private void OnSetupBinding()
		{
			Binding b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "PortName" );
			this.Lbl_SerialCom.SetBinding( Label.ContentProperty, b );
		}

		private void UserControl_IsVisibleChanged( object sender, DependencyPropertyChangedEventArgs e )
		{
			if ( this.Win_SerialComSet_Dlg != null )
			{
				this.Win_SerialComSet_Dlg.Close();
			}
		}

		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
		}

		private void Btn_SerialCom_Click( object sender, RoutedEventArgs e )
		{
			if ( this.Win_SerialComSet_Dlg != null )
			{
				if ( !this.Win_SerialComSet_Dlg.IsVisible )
					this.Win_SerialComSet_Dlg = null;
			}
			if ( this.Win_SerialComSet_Dlg == null )
			{
				var MousePos = ( sender as Button ).PointToScreen( Mouse.GetPosition( ( sender as Button ) ) );

				this.Win_SerialComSet_Dlg = new Win_SerialComSet();
				this.Win_SerialComSet_Dlg.SerialComParam = this._SerialComParam;
				this.Win_SerialComSet_Dlg.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
				this.Win_SerialComSet_Dlg.Top = MousePos.Y;
				this.Win_SerialComSet_Dlg.Left = MousePos.X;
				this.Win_SerialComSet_Dlg.Topmost = true;
				this.Win_SerialComSet_Dlg.Visibility = System.Windows.Visibility.Visible;
				this.Win_SerialComSet_Dlg.Show();
				this.Win_SerialComSet_Dlg.Topmost = true;
			}
			else if ( this.Win_SerialComSet_Dlg != null )
				this.Win_SerialComSet_Dlg.Close();
		}
	}
}
