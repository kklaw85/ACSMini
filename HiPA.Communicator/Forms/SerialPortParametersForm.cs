using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace HiPA.Communicator.Forms
{
	public partial class SerialPortParametersForm : Form
	{
		public SerialPortParametersForm( ISerialPortParameter parameter = null )
		{
			this.InitializeComponent();
			if ( parameter != null ) this.Parameter = parameter;
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			//Console.WriteLine( "SerialPortParametersForm.OnLoad" );
		}

		[Browsable( false )]
		public ISerialPortParameter Parameter
		{
			get => this.SerialPortConnectParameters.Parameter;
			set
			{
				if ( value != null )
					this.SerialPortConnectParameters.Parameter = value;


			}
		}


		private async void btnConnect_Click( object sender, EventArgs e )
		{

			if ( this.btnConnect.Text == "Connect" )
			{
				this.btnConnect.Enabled = false;
				var param = this.SerialPortConnectParameters.Parameter;
				var error = await this.SerialPortResourceTestUI.Connect( param );
				if ( string.IsNullOrEmpty( error ) == false )
				{
					new Thread( () => MessageBox.Show( this, $"Connect Port[{this.SerialPortConnectParameters.SerialPort}] Failed\r\nError:\r\n{error}",
						"Connect Failed", MessageBoxButtons.OK, MessageBoxIcon.Error ) ).Start();
				}
				else
				{
					this.grbTesting.Enabled = true;
					this.btnConnect.Text = "Disconnect";
				}

				Application.DoEvents();
				this.btnConnect.Enabled = true;
			}
			else
			{
				this.SerialPortResourceTestUI.Disconnect();
				this.grbTesting.Enabled = false;
				this.btnConnect.Text = "Connect";
			}
		}

		private void btnOK_Click( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}
