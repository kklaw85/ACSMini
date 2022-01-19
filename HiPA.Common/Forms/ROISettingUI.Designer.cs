namespace HiPA.Common.Forms
{
	partial class ROISettingUI
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
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.numROI_Height = new NumericUpDownCanReadOnly();
			this.numROI_OffsetY = new NumericUpDownCanReadOnly();
			this.numROI_Width = new NumericUpDownCanReadOnly();
			this.numROI_OffsetX = new NumericUpDownCanReadOnly();
			this.chkIsEditable = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numROI_Height)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numROI_OffsetY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numROI_Width)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numROI_OffsetX)).BeginInit();
			this.SuspendLayout();
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(2, 103);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(69, 23);
			this.label7.TabIndex = 10;
			this.label7.Text = "Height";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(2, 37);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(69, 23);
			this.label5.TabIndex = 11;
			this.label5.Text = "Offset Y";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(2, 70);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(69, 23);
			this.label6.TabIndex = 12;
			this.label6.Text = "Width";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(2, 4);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 23);
			this.label4.TabIndex = 13;
			this.label4.Text = "Offset X";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numROI_Height
			// 
			this.numROI_Height.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numROI_Height.Location = new System.Drawing.Point(77, 102);
			this.numROI_Height.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numROI_Height.Name = "numROI_Height";
			this.numROI_Height.Size = new System.Drawing.Size(81, 27);
			this.numROI_Height.TabIndex = 14;
			this.numROI_Height.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// numROI_OffsetY
			// 
			this.numROI_OffsetY.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numROI_OffsetY.Location = new System.Drawing.Point(77, 36);
			this.numROI_OffsetY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numROI_OffsetY.Name = "numROI_OffsetY";
			this.numROI_OffsetY.Size = new System.Drawing.Size(81, 27);
			this.numROI_OffsetY.TabIndex = 15;
			this.numROI_OffsetY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// numROI_Width
			// 
			this.numROI_Width.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numROI_Width.Location = new System.Drawing.Point(77, 69);
			this.numROI_Width.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numROI_Width.Name = "numROI_Width";
			this.numROI_Width.Size = new System.Drawing.Size(81, 27);
			this.numROI_Width.TabIndex = 16;
			this.numROI_Width.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// numROI_OffsetX
			// 
			this.numROI_OffsetX.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.numROI_OffsetX.Location = new System.Drawing.Point(77, 3);
			this.numROI_OffsetX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.numROI_OffsetX.Name = "numROI_OffsetX";
			this.numROI_OffsetX.Size = new System.Drawing.Size(81, 27);
			this.numROI_OffsetX.TabIndex = 17;
			this.numROI_OffsetX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// chkIsEditable
			// 
			this.chkIsEditable.Location = new System.Drawing.Point(18, 135);
			this.chkIsEditable.Name = "chkIsEditable";
			this.chkIsEditable.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.chkIsEditable.Size = new System.Drawing.Size(140, 24);
			this.chkIsEditable.TabIndex = 18;
			this.chkIsEditable.Text = "Editable";
			this.chkIsEditable.UseVisualStyleBackColor = true;
			this.chkIsEditable.CheckedChanged += new System.EventHandler(this.chkIsEditable_CheckedChanged);
			// 
			// ROISettingUI
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.chkIsEditable);
			this.Controls.Add(this.numROI_Height);
			this.Controls.Add(this.numROI_OffsetY);
			this.Controls.Add(this.numROI_Width);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.numROI_OffsetX);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ROISettingUI";
			this.Size = new System.Drawing.Size(175, 167);
			((System.ComponentModel.ISupportInitialize)(this.numROI_Height)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numROI_OffsetY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numROI_Width)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numROI_OffsetX)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private NumericUpDownCanReadOnly numROI_OffsetX;
		private NumericUpDownCanReadOnly numROI_OffsetY;
		private NumericUpDownCanReadOnly numROI_Width;
		private NumericUpDownCanReadOnly numROI_Height;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox chkIsEditable;
	}
}
