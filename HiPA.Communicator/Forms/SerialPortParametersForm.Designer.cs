namespace HiPA.Communicator.Forms
{
	partial class SerialPortParametersForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnConnect = new System.Windows.Forms.Button();
			this.grbTesting = new System.Windows.Forms.GroupBox();
			this.SerialPortResourceTestUI = new HiPA.Communicator.Forms.SerialPortResourceTestUI();
			this.SerialPortConnectParameters = new HiPA.Communicator.Forms.SerialPortParametersUI();
			this.groupBox1.SuspendLayout();
			this.grbTesting.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnCancel);
			this.groupBox1.Controls.Add(this.btnOK);
			this.groupBox1.Controls.Add(this.btnConnect);
			this.groupBox1.Controls.Add(this.SerialPortConnectParameters);
			this.groupBox1.Location = new System.Drawing.Point(2, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 3, 6, 6);
			this.groupBox1.Size = new System.Drawing.Size(325, 363);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Connect Parameters";
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.Silver;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(152, 329);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(79, 30);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Close";
			this.btnCancel.UseVisualStyleBackColor = false;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.Silver;
			this.btnOK.Location = new System.Drawing.Point(237, 329);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(79, 30);
			this.btnOK.TabIndex = 4;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnConnect
			// 
			this.btnConnect.Location = new System.Drawing.Point(6, 329);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(99, 30);
			this.btnConnect.TabIndex = 2;
			this.btnConnect.Text = "Connect";
			this.btnConnect.UseVisualStyleBackColor = true;
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// grbTesting
			// 
			this.grbTesting.Controls.Add(this.SerialPortResourceTestUI);
			this.grbTesting.Location = new System.Drawing.Point(334, 2);
			this.grbTesting.Name = "grbTesting";
			this.grbTesting.Padding = new System.Windows.Forms.Padding(6, 3, 6, 6);
			this.grbTesting.Size = new System.Drawing.Size(463, 363);
			this.grbTesting.TabIndex = 2;
			this.grbTesting.TabStop = false;
			this.grbTesting.Text = "Testing";
			// 
			// SerialPortResourceTestUI
			// 
			this.SerialPortResourceTestUI.Dock = System.Windows.Forms.DockStyle.Fill;
			this.SerialPortResourceTestUI.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SerialPortResourceTestUI.Location = new System.Drawing.Point(6, 21);
			this.SerialPortResourceTestUI.Name = "SerialPortResourceTestUI";
			this.SerialPortResourceTestUI.Size = new System.Drawing.Size(451, 336);
			this.SerialPortResourceTestUI.TabIndex = 0;
			// 
			// SerialPortConnectParameters
			// 
			this.SerialPortConnectParameters.DataBits = ComDataBits.Eight;
			this.SerialPortConnectParameters.Dock = System.Windows.Forms.DockStyle.Top;
			this.SerialPortConnectParameters.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SerialPortConnectParameters.Location = new System.Drawing.Point(6, 21);
			this.SerialPortConnectParameters.Name = "SerialPortConnectParameters";
			this.SerialPortConnectParameters.Padding = new System.Windows.Forms.Padding(3);
			this.SerialPortConnectParameters.Parity = System.IO.Ports.Parity.None;
			this.SerialPortConnectParameters.ReadBufferSize = 40960;
			this.SerialPortConnectParameters.SerialPort = null;
			this.SerialPortConnectParameters.Size = new System.Drawing.Size(313, 280);
			this.SerialPortConnectParameters.StopBits = System.IO.Ports.StopBits.One;
			this.SerialPortConnectParameters.TabIndex = 0;
			// 
			// SerialPortParametersForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(803, 368);
			this.Controls.Add(this.grbTesting);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SerialPortParametersForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Serial Port Parameters";
			this.groupBox1.ResumeLayout(false);
			this.grbTesting.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SerialPortParametersUI SerialPortConnectParameters;
		private SerialPortResourceTestUI SerialPortResourceTestUI;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox grbTesting;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}