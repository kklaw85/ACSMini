namespace HiPA.Communicator.Forms
{
	partial class SerialPortResourceUI
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
			this.txtPortName = new System.Windows.Forms.TextBox();
			this.btnOpenParameters = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtPortName
			// 
			this.txtPortName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.txtPortName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtPortName.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPortName.ForeColor = System.Drawing.Color.Lime;
			this.txtPortName.Location = new System.Drawing.Point(0, 0);
			this.txtPortName.Name = "txtPortName";
			this.txtPortName.ReadOnly = true;
			this.txtPortName.Size = new System.Drawing.Size(177, 27);
			this.txtPortName.TabIndex = 1;
			// 
			// btnOpenParameters
			// 
			this.btnOpenParameters.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnOpenParameters.Location = new System.Drawing.Point(177, 0);
			this.btnOpenParameters.Name = "btnOpenParameters";
			this.btnOpenParameters.Size = new System.Drawing.Size(42, 27);
			this.btnOpenParameters.TabIndex = 2;
			this.btnOpenParameters.Tag = "LockCheck";
			this.btnOpenParameters.Text = "...";
			this.btnOpenParameters.UseVisualStyleBackColor = true;
			this.btnOpenParameters.Click += new System.EventHandler(this.btnOpenParameters_Click);
			// 
			// SerialPortResourceUI
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.txtPortName);
			this.Controls.Add(this.btnOpenParameters);
			this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "SerialPortResourceUI";
			this.Size = new System.Drawing.Size(219, 27);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txtPortName;
		private System.Windows.Forms.Button btnOpenParameters;
	}
}
