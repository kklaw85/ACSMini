using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HiPA.Common.Forms
{
	public class PagePanelTitle
		: UserControl
	{
		public event EventHandler InitializeEvent;
		public event EventHandler StopEvent;
		public static event EventHandler<bool> TriggerLightEvent;

		public PagePanelTitle()
		{
			this.InitializeComponent();
		}
		private void btnMachineLight_Click( object sender, EventArgs e )
		{
			TriggerLightEvent?.Invoke( this, true );
		}
		private void btnInitialize_Click( object sender, EventArgs e )
		{
			this.InInit = true;
			this.InitializeEvent?.Invoke( this, null );
		}

		private void btnStop_Click( object sender, EventArgs e )
		{
			this.StopEvent?.Invoke( this, null );
		}

		[DefaultValue( false )]

		bool _isInInit = false;
		private Button btnMachineLight;

		public bool InInit
		{
			get => this._isInInit;
			set
			{
				if ( this._isInitialized ) return;
				this._isInInit = value;
				if ( this._locked == false ) this.btnInitialize.Enabled = !value;
				if ( this._isInitialized == false )
					if ( this._locked == false ) this.btnStop.Enabled = value;
			}
		}

		[DefaultValue( false )]
		bool _isInitialized = false;
		public bool IsInitialized
		{
			get => this._isInitialized;
			set
			{
				if ( this._isInInit ) return;
				this.lblInitialized.BackColor = value ? Color.Lime : Color.Red;
				this._isInitialized = value;

				if ( this._locked == false )
				{
					this.btnInitialize.Enabled = !value;
					this.btnStop.Enabled = value;
					this.btnMachineLight.Enabled = value;
				}
			}
		}

		public void UpdateTitle( InstrumentBase instrument )
		{
			if ( instrument is null )
				this.UpdateTitle( null, null );
			else
				this.UpdateTitle( instrument.Category.ToString(), instrument.Name );

			if ( this.IsInitialized != ( instrument?.BehaviorState == InstrumentBehaviorState.Ready ) ) //#KK Do only when there is status change
				this.IsInitialized = instrument?.BehaviorState == InstrumentBehaviorState.Ready;
		}
		public void UpdateTitle( string category, string name )
		{
			if ( string.IsNullOrEmpty( category ) == true )
				this.lblInstrumentInfo.Text = "Unknown";
			else
				this.lblInstrumentInfo.Text = $"{category} - {name}";
		}

		[Browsable( false )]
		bool _locked = false;
		public bool LockUI
		{
			get => this._locked;
			set
			{
				//if ( this._locked == value ) return;
				this._locked = value;

				if ( value )
				{
					this.btnInitialize.Enabled = false;
					this.btnStop.Enabled = false;
					this.btnMachineLight.Enabled = false;
				}
				else
				{
					this.IsInitialized = this._isInitialized;
					this.InInit = this._isInInit;
				}
			}
		}

		#region Component Designer generated code
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



		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lblInstrumentInfo = new System.Windows.Forms.Label();
			this.btnStop = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnInitialize = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.lblInitialized = new System.Windows.Forms.Label();
			this.btnMachineLight = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblInstrumentInfo
			// 
			this.lblInstrumentInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblInstrumentInfo.Font = new System.Drawing.Font( "Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold );
			this.lblInstrumentInfo.Location = new System.Drawing.Point( 170, 4 );
			this.lblInstrumentInfo.Name = "lblInstrumentInfo";
			this.lblInstrumentInfo.Padding = new System.Windows.Forms.Padding( 10, 0, 0, 0 );
			this.lblInstrumentInfo.Size = new System.Drawing.Size( 429, 28 );
			this.lblInstrumentInfo.TabIndex = 42;
			this.lblInstrumentInfo.Text = "Instrument Name";
			this.lblInstrumentInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnStop
			// 
			this.btnStop.BackColor = System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 224 ) ) ) ), ( ( int )( ( ( byte )( 224 ) ) ) ), ( ( int )( ( ( byte )( 224 ) ) ) ) );
			this.btnStop.Dock = System.Windows.Forms.DockStyle.Left;
			this.btnStop.Enabled = false;
			this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnStop.Font = new System.Drawing.Font( "Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold );
			this.btnStop.Location = new System.Drawing.Point( 112, 4 );
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size( 58, 28 );
			this.btnStop.TabIndex = 43;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = false;
			this.btnStop.Click += new System.EventHandler( this.btnStop_Click );
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Dock = System.Windows.Forms.DockStyle.Left;
			this.label1.Location = new System.Drawing.Point( 108, 4 );
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size( 4, 28 );
			this.label1.TabIndex = 40;
			// 
			// btnInitialize
			// 
			this.btnInitialize.BackColor = System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 224 ) ) ) ), ( ( int )( ( ( byte )( 224 ) ) ) ), ( ( int )( ( ( byte )( 224 ) ) ) ) );
			this.btnInitialize.Dock = System.Windows.Forms.DockStyle.Left;
			this.btnInitialize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnInitialize.Font = new System.Drawing.Font( "Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold );
			this.btnInitialize.Location = new System.Drawing.Point( 23, 4 );
			this.btnInitialize.Name = "btnInitialize";
			this.btnInitialize.Size = new System.Drawing.Size( 85, 28 );
			this.btnInitialize.TabIndex = 41;
			this.btnInitialize.Text = "Initialize";
			this.btnInitialize.UseVisualStyleBackColor = false;
			this.btnInitialize.Click += new System.EventHandler( this.btnInitialize_Click );
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Dock = System.Windows.Forms.DockStyle.Left;
			this.label2.Location = new System.Drawing.Point( 19, 4 );
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size( 4, 28 );
			this.label2.TabIndex = 39;
			// 
			// lblInitialized
			// 
			this.lblInitialized.BackColor = System.Drawing.Color.Red;
			this.lblInitialized.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblInitialized.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblInitialized.Location = new System.Drawing.Point( 4, 4 );
			this.lblInitialized.Name = "lblInitialized";
			this.lblInitialized.Size = new System.Drawing.Size( 15, 28 );
			this.lblInitialized.TabIndex = 38;
			// 
			// btnMachineLight
			// 
			this.btnMachineLight.Anchor = ( ( System.Windows.Forms.AnchorStyles )( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
			this.btnMachineLight.BackColor = System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 224 ) ) ) ), ( ( int )( ( ( byte )( 224 ) ) ) ), ( ( int )( ( ( byte )( 224 ) ) ) ) );
			this.btnMachineLight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnMachineLight.Enabled = false;
			this.btnMachineLight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMachineLight.Font = new System.Drawing.Font( "Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold );
			this.btnMachineLight.Location = new System.Drawing.Point( 570, 4 );
			this.btnMachineLight.Name = "btnMachineLight";
			this.btnMachineLight.Size = new System.Drawing.Size( 29, 28 );
			this.btnMachineLight.TabIndex = 44;
			this.btnMachineLight.UseVisualStyleBackColor = false;
			this.btnMachineLight.Click += new System.EventHandler( this.btnMachineLight_Click );
			// 
			// PagePanelTitle
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Silver;
			this.Controls.Add( this.btnMachineLight );
			this.Controls.Add( this.lblInstrumentInfo );
			this.Controls.Add( this.btnStop );
			this.Controls.Add( this.label1 );
			this.Controls.Add( this.btnInitialize );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.lblInitialized );
			this.Font = new System.Drawing.Font( "Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.Name = "PagePanelTitle";
			this.Padding = new System.Windows.Forms.Padding( 4 );
			this.Size = new System.Drawing.Size( 603, 36 );
			this.ResumeLayout( false );

		}
		private System.Windows.Forms.Label lblInstrumentInfo;
		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnInitialize;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lblInitialized;
		#endregion
	}
}
