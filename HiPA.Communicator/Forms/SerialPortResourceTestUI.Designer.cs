namespace HiPA.Communicator.Forms
{
	partial class SerialPortResourceTestUI
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.chkShowAsHex = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.txtSuffix = new System.Windows.Forms.TextBox();
			this.txtPrefix = new System.Windows.Forms.TextBox();
			this.txtSendData = new System.Windows.Forms.TextBox();
			this.btnClear = new System.Windows.Forms.Button();
			this.btnSend = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.txtRecvData = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.chkShowAsHex);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.txtSuffix);
			this.panel1.Controls.Add(this.txtPrefix);
			this.panel1.Controls.Add(this.txtSendData);
			this.panel1.Controls.Add(this.btnClear);
			this.panel1.Controls.Add(this.btnSend);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(441, 62);
			this.panel1.TabIndex = 0;
			// 
			// chkShowAsHex
			// 
			this.chkShowAsHex.AutoSize = true;
			this.chkShowAsHex.Location = new System.Drawing.Point(249, 37);
			this.chkShowAsHex.Name = "chkShowAsHex";
			this.chkShowAsHex.Size = new System.Drawing.Size(103, 21);
			this.chkShowAsHex.TabIndex = 5;
			this.chkShowAsHex.Text = "Show as HEX";
			this.chkShowAsHex.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(129, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 17);
			this.label2.TabIndex = 10;
			this.label2.Text = "Suffix";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 17);
			this.label1.TabIndex = 11;
			this.label1.Text = "Prefix";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtSuffix
			// 
			this.txtSuffix.Location = new System.Drawing.Point(174, 35);
			this.txtSuffix.Name = "txtSuffix";
			this.txtSuffix.Size = new System.Drawing.Size(69, 25);
			this.txtSuffix.TabIndex = 4;
			// 
			// txtPrefix
			// 
			this.txtPrefix.Location = new System.Drawing.Point(49, 35);
			this.txtPrefix.Name = "txtPrefix";
			this.txtPrefix.Size = new System.Drawing.Size(74, 25);
			this.txtPrefix.TabIndex = 3;
			// 
			// txtSendData
			// 
			this.txtSendData.Location = new System.Drawing.Point(3, 3);
			this.txtSendData.Name = "txtSendData";
			this.txtSendData.Size = new System.Drawing.Size(350, 25);
			this.txtSendData.TabIndex = 1;
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(359, 33);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(79, 25);
			this.btnClear.TabIndex = 6;
			this.btnClear.Text = "&Clear";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// btnSend
			// 
			this.btnSend.Location = new System.Drawing.Point(359, 3);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(79, 25);
			this.btnSend.TabIndex = 2;
			this.btnSend.Text = "&Send";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.txtRecvData);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 62);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(3);
			this.panel2.Size = new System.Drawing.Size(441, 220);
			this.panel2.TabIndex = 0;
			// 
			// txtRecvData
			// 
			this.txtRecvData.AcceptsReturn = true;
			this.txtRecvData.AcceptsTab = true;
			this.txtRecvData.BackColor = System.Drawing.Color.White;
			this.txtRecvData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtRecvData.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtRecvData.Location = new System.Drawing.Point(3, 3);
			this.txtRecvData.Multiline = true;
			this.txtRecvData.Name = "txtRecvData";
			this.txtRecvData.ReadOnly = true;
			this.txtRecvData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtRecvData.Size = new System.Drawing.Size(435, 214);
			this.txtRecvData.TabIndex = 7;
			// 
			// SerialPortResourceTestUI
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "SerialPortResourceTestUI";
			this.Size = new System.Drawing.Size(441, 282);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox chkShowAsHex;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtSuffix;
		private System.Windows.Forms.TextBox txtPrefix;
		private System.Windows.Forms.TextBox txtSendData;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox txtRecvData;
	}
}
