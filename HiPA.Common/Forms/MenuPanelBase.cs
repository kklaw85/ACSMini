using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HiPA.Common.Forms
{
	public class MenuPanelBase
		: UserControl
		, IArchiveActionNotifier
	{
		#region Menu Checked/Unchecked Color
		public Color MenuCheckedColor { get; set; } = Color.FromArgb( 255, 128, 128, 255 );
		public Color MenuUncheckedColor { get; set; } = Color.FromArgb( 255, 224, 224, 224 );
		#endregion

		#region Menu Events
		public event MenuItemEventHandler MenuItemAdd;
		public event MenuItemEventHandler MenuItemRemove;
		public event MenuItemEventHandler MenuItemSelectChanged;
		#endregion

		#region Initialize Component
		public MenuPanelBase()
		{
			this.InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// MenuPanelBase
			// 
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Font = new System.Drawing.Font( "Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
			this.Name = "MenuPanelBase";
			this.Padding = new System.Windows.Forms.Padding( 6 );
			this.Size = new System.Drawing.Size( 299, 162 );
			this.ResumeLayout( false );
		}
		#endregion

		#region Menu Selection Operation
		string _selectedItem = "";
		[Browsable( false )]
		public string SelectedItem
		{
			get => this._selectedItem;
			set
			{
				if ( this._selectedItem == value ) return;
				if ( string.IsNullOrEmpty( value ) == false && this._menuItems.ContainsKey( value ) == false ) return;

				var older = this._selectedItem ?? "";
				this._selectedItem = value ?? "";

				var arg = new MenuItemEventArgs( older, this._selectedItem );

				this._menuItems.TryGetValue( older, out var previous );
				arg.PreviousControl = previous;

				this._menuItems.TryGetValue( this._selectedItem, out var current );
				arg.CurrentControl = current;

				this.OnMenuItemSelectChanged( arg );
				//Debug.WriteLine( $"{this.Name}.SelectedItem: [{older}] => [{this._selectedItem}]" );
			}
		}
		protected virtual void OnMenuItemSelectChanged( MenuItemEventArgs e )
		{
			this.MenuItemSelectChanged?.Invoke( this, e );

			var previous = e.PreviousControl as RadioButton;
			var current = e.CurrentControl as RadioButton;

			if ( previous != null )
			{
				previous.BackColor = this.MenuUncheckedColor;
			}
			if ( current != null )
			{
				current.BackColor = this.MenuCheckedColor;
				current.Checked = true;
			}
		}
		#endregion

		#region Menu Other Action
		public event EventHandler<ArchiveAction> ArchiveActionEvent;
		public void EmitArchiveActionEvent( object sender, ArchiveAction action )
		{
			this.ArchiveActionEvent?.Invoke( sender, action );
		}
		#endregion

		#region Menu Item Add/Remove/Set Operation
		//protected List<string> _menuItems = new List<string>();
		Dictionary<string, Control> _menuItems = new Dictionary<string, Control>();
		protected void AddMenuItem( Control menuItem )
		{
			if ( menuItem == null ) return;
			this.AddMenuItem( menuItem.Text, menuItem );
		}
		protected void AddMenuItem( string layerName, Control menuItem )
		{
			if ( string.IsNullOrEmpty( layerName ) == true ) return;
			if ( menuItem == null ) return;
			if ( this._menuItems.ContainsKey( layerName ) == true ) return;
			this._menuItems[ layerName ] = menuItem;
			this.MenuItemAdd?.Invoke( this, new MenuItemEventArgs( null, null, layerName, menuItem ) );
			if ( menuItem is RadioButton menu ) menu.CheckedChanged += this.EventMenuItemCheckedChanged;
			//Debug.WriteLine( $"{this.Name}.AddMenuItem: {layerName}" );
		}
		protected void RemoveMenuItem( string layerName )
		{
			if ( this._menuItems.TryGetValue( layerName, out var target ) == true )
			{
				if ( this.SelectedItem == layerName ) this.SelectedItem = "";
				if ( target is RadioButton menu ) menu.CheckedChanged -= this.EventMenuItemCheckedChanged;
				this.MenuItemRemove?.Invoke( this, new MenuItemEventArgs( null, null, layerName, target ) );
				this._menuItems.Remove( layerName );
				//Debug.WriteLine( $"{this.Name}.RemoveMenuItem: {layerName}" );
			}
		}
		public void SetMenuItemEnabled( string layerName, bool enabled )
		{
			this.BeginInvoke( new Action( () =>
			{
				if ( string.IsNullOrEmpty( layerName ) == true ) return;

				if ( this._menuItems.TryGetValue( layerName, out var target ) == true )
				{
					target.Enabled = enabled;
					//Debug.WriteLine( $"{this.GetType().Name}.SetMenuItemEnabled, [{layerName}], enabled[{enabled}]" );
				}
			} ) );
		}
		protected virtual void EventMenuItemCheckedChanged( object sender, EventArgs e )
		{
			if ( sender is RadioButton button && button.Checked )
			{
				this.SelectedItem = button.Text;
			}
		}
		#endregion
	}

	#region MenuItem Event & Handler
	public class MenuItemEventArgs : EventArgs
	{
		public string Previous { get; } = null;
		public Control PreviousControl { get; set; } = null;

		public string Current { get; } = null;
		public Control CurrentControl { get; set; } = null;

		public MenuItemEventArgs( string current )
			: this( null, null, current, null )
		{
		}
		public MenuItemEventArgs( string previous, string current )
			: this( previous, null, current, null )
		{
		}
		public MenuItemEventArgs( string pKey, Control previous, string cKey, Control current )
		{
			this.Previous = pKey;
			this.PreviousControl = previous;
			this.Current = cKey;
			this.CurrentControl = current;
		}
	}

	public delegate void MenuItemEventHandler( object sender, MenuItemEventArgs e );
	#endregion
}
