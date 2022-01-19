using System;
using System.Windows.Forms;

namespace HiPA.Common.Forms
{
	public class TextBoxCanReadOnly : TextBox
	{
		//private string OldText;

		public TextBoxCanReadOnly()
		{
			this.InitializeComponent();
		}

		protected override void OnTextChanged( EventArgs e )
		{
			base.OnTextChanged( e );
			//if ( this.OldText == this.Text )
			//	this.BackColor = Color.White;
			//else
			//	this.BackColor = Color.Yellow;
		}

		public virtual void OnSave()
		{
			//this.Select( 0, 0 );
			//this.OldText = this.Text;
			//this.BackColor = Color.White;
		}

		public virtual void OnCancel()
		{
			//this.Select( 0, 0 );
			//this.Text = this.OldText;
			//this.BackColor = Color.White;
		}

		#region Designer
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
			if ( disposing && ( this.components != null ) )
			{
				this.components.Dispose();
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
			this.components = new System.ComponentModel.Container();
		}

		#endregion
		#endregion
	}
}
