namespace HiPA.Communicator.Forms
{
	partial class SerialPortParametersUI
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
			this.label1 = new System.Windows.Forms.Label();
			this.cmbPortName = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cmbBaudrate = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cmbParity = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cmbDataBits = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cmbStopBits = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.numReadTimeout = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.numReadBufferSize = new System.Windows.Forms.NumericUpDown();
			this.label12 = new System.Windows.Forms.Label();
			this.numWriteTimeout = new System.Windows.Forms.NumericUpDown();
			this.label13 = new System.Windows.Forms.Label();
			this.numWriteBufferSize = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numReadTimeout)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numReadBufferSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numWriteTimeout)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numWriteBufferSize)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Port Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbPortName
			// 
			this.cmbPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPortName.DropDownWidth = 400;
			this.cmbPortName.FormattingEnabled = true;
			this.cmbPortName.Location = new System.Drawing.Point(84, 3);
			this.cmbPortName.Name = "cmbPortName";
			this.cmbPortName.Size = new System.Drawing.Size(220, 25);
			this.cmbPortName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 23);
			this.label2.TabIndex = 0;
			this.label2.Text = "Baudrate";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbBaudrate
			// 
			this.cmbBaudrate.FormattingEnabled = true;
			this.cmbBaudrate.Location = new System.Drawing.Point(84, 34);
			this.cmbBaudrate.Name = "cmbBaudrate";
			this.cmbBaudrate.Size = new System.Drawing.Size(220, 25);
			this.cmbBaudrate.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 23);
			this.label3.TabIndex = 0;
			this.label3.Text = "Parity";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbParity
			// 
			this.cmbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbParity.FormattingEnabled = true;
			this.cmbParity.Location = new System.Drawing.Point(84, 96);
			this.cmbParity.Name = "cmbParity";
			this.cmbParity.Size = new System.Drawing.Size(220, 25);
			this.cmbParity.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(3, 65);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(75, 23);
			this.label4.TabIndex = 0;
			this.label4.Text = "Data Bits";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbDataBits
			// 
			this.cmbDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDataBits.FormattingEnabled = true;
			this.cmbDataBits.Location = new System.Drawing.Point(84, 65);
			this.cmbDataBits.Name = "cmbDataBits";
			this.cmbDataBits.Size = new System.Drawing.Size(220, 25);
			this.cmbDataBits.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 127);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(75, 23);
			this.label5.TabIndex = 0;
			this.label5.Text = "Stop Bits";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cmbStopBits
			// 
			this.cmbStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbStopBits.FormattingEnabled = true;
			this.cmbStopBits.Location = new System.Drawing.Point(84, 127);
			this.cmbStopBits.Name = "cmbStopBits";
			this.cmbStopBits.Size = new System.Drawing.Size(220, 25);
			this.cmbStopBits.TabIndex = 5;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 161);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(108, 23);
			this.label6.TabIndex = 0;
			this.label6.Text = "Read Timeout";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numReadTimeout
			// 
			this.numReadTimeout.Location = new System.Drawing.Point(118, 161);
			this.numReadTimeout.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
			this.numReadTimeout.Name = "numReadTimeout";
			this.numReadTimeout.Size = new System.Drawing.Size(92, 25);
			this.numReadTimeout.TabIndex = 6;
			this.numReadTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numReadTimeout.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(216, 160);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(37, 23);
			this.label7.TabIndex = 0;
			this.label7.Text = "ms";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(4, 192);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(108, 23);
			this.label8.TabIndex = 0;
			this.label8.Text = "Read Buffer Size";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(4, 223);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(108, 23);
			this.label9.TabIndex = 0;
			this.label9.Text = "Write Timeout";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(4, 254);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(108, 23);
			this.label10.TabIndex = 0;
			this.label10.Text = "Write Buffer Size";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(216, 191);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(50, 23);
			this.label11.TabIndex = 0;
			this.label11.Text = "Bytes";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numReadBufferSize
			// 
			this.numReadBufferSize.Location = new System.Drawing.Point(118, 192);
			this.numReadBufferSize.Maximum = new decimal(new int[] {
            4096000,
            0,
            0,
            0});
			this.numReadBufferSize.Name = "numReadBufferSize";
			this.numReadBufferSize.Size = new System.Drawing.Size(92, 25);
			this.numReadBufferSize.TabIndex = 7;
			this.numReadBufferSize.Tag = "";
			this.numReadBufferSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numReadBufferSize.Value = new decimal(new int[] {
            409600,
            0,
            0,
            0});
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(216, 222);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(37, 23);
			this.label12.TabIndex = 0;
			this.label12.Text = "ms";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numWriteTimeout
			// 
			this.numWriteTimeout.Location = new System.Drawing.Point(118, 223);
			this.numWriteTimeout.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
			this.numWriteTimeout.Name = "numWriteTimeout";
			this.numWriteTimeout.Size = new System.Drawing.Size(92, 25);
			this.numWriteTimeout.TabIndex = 8;
			this.numWriteTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numWriteTimeout.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(216, 253);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(50, 23);
			this.label13.TabIndex = 0;
			this.label13.Text = "Bytes";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numWriteBufferSize
			// 
			this.numWriteBufferSize.Location = new System.Drawing.Point(118, 254);
			this.numWriteBufferSize.Maximum = new decimal(new int[] {
            4096000,
            0,
            0,
            0});
			this.numWriteBufferSize.Name = "numWriteBufferSize";
			this.numWriteBufferSize.Size = new System.Drawing.Size(92, 25);
			this.numWriteBufferSize.TabIndex = 9;
			this.numWriteBufferSize.Tag = "";
			this.numWriteBufferSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numWriteBufferSize.Value = new decimal(new int[] {
            409600,
            0,
            0,
            0});
			// 
			// SerialPortParametersUI
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.numWriteBufferSize);
			this.Controls.Add(this.numWriteTimeout);
			this.Controls.Add(this.numReadBufferSize);
			this.Controls.Add(this.numReadTimeout);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.cmbStopBits);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.cmbDataBits);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.cmbParity);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cmbBaudrate);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.cmbPortName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "SerialPortParametersUI";
			this.Padding = new System.Windows.Forms.Padding(3);
			this.Size = new System.Drawing.Size(307, 283);
			((System.ComponentModel.ISupportInitialize)(this.numReadTimeout)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numReadBufferSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numWriteTimeout)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numWriteBufferSize)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cmbPortName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cmbBaudrate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cmbParity;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cmbDataBits;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cmbStopBits;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numReadTimeout;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.NumericUpDown numReadBufferSize;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.NumericUpDown numWriteTimeout;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.NumericUpDown numWriteBufferSize;
	}
}
