using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace HiPA.Communicator.Forms
{
	/// <summary>
	/// Interaction logic for Win_SerialComSet.xaml
	/// </summary>
	public partial class Win_SerialComSet : MahApps.Metro.Controls.MetroWindow
	{
		public Win_SerialComSet()
		{
			this.InitializeComponent();
			this.CB_BaudRate.ItemsSource = Enum.GetValues( typeof( ComBaudRate ) ).Cast<ComBaudRate>();
			this.CB_BaudRate.SelectedItem = ComBaudRate.BR9600;

			this.Cb_DataBits.ItemsSource = Enum.GetValues( typeof( ComDataBits ) ).Cast<ComDataBits>();
			this.Cb_DataBits.SelectedItem = ComDataBits.Eight;

			this.Cb_Parity.ItemsSource = Enum.GetValues( typeof( Parity ) ).Cast<Parity>();
			this.Cb_Parity.SelectedItem = Parity.None;

			this.Cb_StopBits.ItemsSource = Enum.GetValues( typeof( StopBits ) ).Cast<StopBits>();
			this.Cb_StopBits.SelectedItem = StopBits.One;
		}

		private async void RefreshPorts()
		{
			try
			{
				var selectedPort = this.Cb_ComPort.SelectedItem?.ToString();
				this.Cb_ComPort.Items.Clear();

				var ports = await this._RefreshPorts();

				if ( ports != null )
				{
					var sorted = ports.ToList();
					sorted.Sort( ( x, y ) => x.PortNo - y.PortNo );
					foreach ( var port in sorted )
						this.Cb_ComPort.Items.Add( port.PortName );

					if ( string.IsNullOrEmpty( selectedPort ) == false )
					{
						this.Cb_ComPort.SelectedItem = selectedPort;
					}
				}
			}
			catch ( Exception ex )
			{
				new Thread( () => System.Windows.MessageBox.Show( ex.Message ) ).Start();
			}
		}
		private Task<IEnumerable<(int PortNo, string PortName, string Describe)>> _RefreshPorts()
		{
			return Task.Run( () =>
			{
				try
				{
					return SerialCommunicator.GetSerialPorts();
				}
				catch ( Exception ex )
				{
					new Thread( () => System.Windows.MessageBox.Show( ex.Message ) ).Start();
					return null;
				}
			} );
		}
		private SerialPortParameter _SerialComParam = null;
		public SerialPortParameter SerialComParam
		{
			get => this._SerialComParam;
			set
			{
				//if ( value == null ) return;
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
			this.Cb_ComPort.SetBinding( ComboBox.SelectedItemProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "BaudRate" );
			this.CB_BaudRate.SetBinding( ComboBox.SelectedItemProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "Parity" );
			this.Cb_Parity.SetBinding( ComboBox.SelectedItemProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "DataBits" );
			this.Cb_DataBits.SetBinding( ComboBox.SelectedItemProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "StopBits" );
			this.Cb_StopBits.SetBinding( ComboBox.SelectedItemProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "WriteBufferSize" );
			this.Txt_WriteBuffer.SetBinding( NumericUpDown.ValueProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "ReadBufferSize" );
			this.Txt_ReadBuffer.SetBinding( NumericUpDown.ValueProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "WriteTimeout" );
			this.Txt_WriteTimeout.SetBinding( NumericUpDown.ValueProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "ReadTimeout" );
			this.Txt_ReadTimeOut.SetBinding( NumericUpDown.ValueProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "DtrEnable" );
			this.Chk_DtrEn.SetBinding( CheckBox.IsCheckedProperty, b );

			b = new Binding();
			b.Source = this._SerialComParam;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Path = new PropertyPath( "RtsEnable" );
			this.Chk_RtsEn.SetBinding( CheckBox.IsCheckedProperty, b );

			this.RefreshPorts();
		}
		private void UserControl_Loaded( object sender, RoutedEventArgs e )
		{
		}

		private bool _ComConnect = false;
		private void Btn_ComConnect_Click( object sender, RoutedEventArgs e )
		{
			if ( !this._ComConnect )
			{
				var error = string.Empty;
				if ( !string.IsNullOrEmpty( error = this.Connect( this.SerialComParam ).Result ) )
					new Thread( () => System.Windows.MessageBox.Show( error ) ).Start();
				else
				{
					this._ComConnect = true;
				}
			}
			else
			{
				this.Disconnect();
			}
		}
		private SerialPort _session = new SerialPort();
		public Task<string> Connect( SerialPortParameter param )
		{
			return Task.Run( () =>
			{
				try
				{
					this._session = SerialCommunicator.CreatePortFromParameter( param );
					this._session.DataReceived += ( s, e ) =>
					{
						try
						{
							var buffer = new byte[ 4096 ];
							var size = this._session.Read( buffer, 0, buffer.Length );
						}
						catch ( Exception ex )
						{
							new Thread( () => System.Windows.MessageBox.Show( ex.Message ) ).Start();
						}
					};
					return "";
				}
				catch ( Exception ex )
				{
					this.Disconnect();
					return ex.Message;
				}
			} );
		}
		public void Disconnect()
		{
			try
			{
				this._session?.Close();
				this._session = null;
			}
			catch
			{

			}
		}
		private void Btn_ComClose_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				this.Disconnect();
				this.Close();
			}
			catch
			{

			}
		}

		private void Btn_ComOk_Click( object sender, RoutedEventArgs e )
		{
			try
			{
				this.Disconnect();
				this.Close();
			}
			catch
			{

			}
		}
	}
}
