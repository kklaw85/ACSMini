using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace HiPA.Common.Forms
{
	public class PagePanelBase
		: UserControl
		, IPagePanelBase
		, IArchivableUI
		, IHierarchicalLayer
		, ILockableUI
		, IPollingUpdatable
	{
		public static event LayerActiveChangedEventHandler LayerActiveChangedEvent;
		public static event EventHandler<ArchiveAction> ArchiveActionEvent;
		#region Errorformatting
		protected string FormatErrMsg( string Name, Exception ex )
		{
			var TypeName = this.GetType().Name;
			var st = new StackTrace();
			var sf = st.GetFrame( 1 );
			var MethodName = sf.GetMethod().Name;
			string fullMethodName = $"{TypeName}:{MethodName}:,";
			return $"{Name}:{fullMethodName}{ex.Message}";
		}
		#endregion
		#region Menu/Content Controls
		[Browsable( true )]
		[DefaultValue( null )]
		public virtual MenuPanelBase MenuPanel { get; set; } = null;

		[Browsable( true )]
		[DefaultValue( null )]
		public virtual Panel ContentPanel { get; set; } = null;
		#endregion

		#region IHierarchicalLayer.Properties
		[Browsable( false )]
		public virtual string LayerName { get; protected set; } = null;

		[Browsable( false )]
		public virtual string Path { get; protected set; }

		[Browsable( false )]
		public virtual string PathSaparator { get; protected set; } = " > ";
		#endregion

		#region Page Events
		public virtual void OnPageLoad()
		{
			//Debug.WriteLine( $"{this.Name}.OnPageLoad, [{this.LayerName}]" );
			if ( this.MenuPanel != null )
			{
				this.MenuPanel.MenuItemSelectChanged += this._MenuItemSelectChanged;
				this.MenuPanel.ArchiveActionEvent += this._ArchiveActionEvent;
			}
			Equipment.MachStateMgr.MachineStateChanged += this.MachineStatusChangedUpdateUI;
		}

		private void _ArchiveActionEvent( object sender, ArchiveAction e )
		{
			if ( e == ArchiveAction.Save ) ( this.SelectedLayer as IArchivableUI )?.SaveModification();
			else if ( e == ArchiveAction.Cancel ) ( this.SelectedLayer as IArchivableUI )?.CancelModification();

			ArchiveActionEvent?.Invoke( this, e );
		}

		private void _MenuItemSelectChanged( object sender, MenuItemEventArgs e )
		{
			var layer = this.GetLayerByName( e.Current );
			this.SelectedLayer = layer;
		}
		public virtual void OnPageDestroy()
		{
			//Debug.WriteLine( $"{this.Name}.OnPageDestroy, [{this.LayerName}]" );
			if ( this.HasChild == true )
			{
				this._children.Value.RemoveAll( c =>
				{
					c.Super = null;
					return true;
				} );
			}
			//this.SelectedLayer?.OnDisplayData(); //#howie todo

			if ( this.MenuPanel != null ) this.MenuPanel.MenuItemSelectChanged -= this._MenuItemSelectChanged;
			if ( this.MenuPanel is IArchiveActionNotifier notifier ) notifier.ArchiveActionEvent -= this._ArchiveActionEvent;
		}

		public virtual void OnPageActived( bool isActived )
		{
			//Debug.WriteLine( $"{this.Name}.OnPageActived, [{this.LayerName}] {isActived}" );
			//this.SelectedLayer?.OnPageActived( isActived ); //#KK not in used.
			if ( isActived )
			{
				this.BringToFront();
				this.Privilege = this._privilege;
				Equipment.MachStateMgr.MachineStatus = Equipment.MachStateMgr.MachineStatus;
			}
			if ( this.SelectedLayer == null ) LayerActiveChangedEvent?.Invoke( this, new LayerActiveChangedEventArgs( this, isActived ) );
		}
		public virtual void OnDisplayData()
		{
			this.SelectedLayer?.OnDisplayData();
		}
		#endregion

		#region IArchivableUI.Impl
		public virtual void SaveModification()
		{
			//Howie's
			if ( this.SelectedLayer is IArchivableUI archive )
			{
				archive.SaveModification();
				archive.UpdateToConfig();
			}
			else if ( this is IArchivableUI archiveLayer )
			{
				this.UpdateToConfig();
				this.UpdateToUI();
			}
			//this.SaveAllTextBox();
		}

		public virtual void CancelModification()
		{
			//Howie
			if ( this.SelectedLayer is IArchivableUI archive )
			{
				archive.CancelModification();
				archive.UpdateToUI();
			}
			//#JW's. #Howie to double confirm on why?
			if ( this is IArchivableUI archiveLayer )
			{
				this.UpdateToUI();
			}
			//this.CancelAllTextBox();
		}

		public virtual void UpdateToUI()
		{
			//this.SaveAllTextBox();
		}

		public virtual void UpdateToConfig()
		{
			//this.SaveAllTextBox();
		}
		#endregion

		#region IHierarchicalLayer.Super 
		public event LayerSelectChangedEventHandler SuperLayerChangedEvent;
		IHierarchicalLayer _super = null;
		[Browsable( false )]
		public virtual IHierarchicalLayer Super
		{
			get => this._super;
			set
			{
				if ( this._super == value ) return;
				var older = this._super;
				this._super = value;
				if ( this._super == null ) this.Path = "";
				else this.Path = this.GetPath( this.PathSaparator );
				this.OnSuperChanged( older, value );
			}
		}
		protected virtual void OnSuperChanged( IHierarchicalLayer previous, IHierarchicalLayer current )
		{
			this.SuperLayerChangedEvent?.Invoke( this, new LayerSelectChangedEventArgs( previous, current ) );
		}
		#endregion

		#region IHierarchicalLayer.Children.Add/Remove/Get
		Lazy<List<IHierarchicalLayer>> _children = new Lazy<List<IHierarchicalLayer>>();
		public bool HasChild => this._children.IsValueCreated;
		public IList<IHierarchicalLayer> GetChildren() => this._children.IsValueCreated ? this._children.Value : null;
		[Browsable( false )]
		public void AddChildLayer( IHierarchicalLayer child )
		{
			if ( this._children.Value.Contains( child ) == false )
			{
				this._children.Value.Add( child );
				child.Super = this;

				if ( this.ContentPanel != null &&
					child is Control control )
				{
					control.Dock = DockStyle.Fill;
					this.ContentPanel.Controls.Add( control );
				}
			}
		}
		public void RemoveChildLayer( string layerName )
		{
			this.RemoveChildLayer( this.GetLayerByName( layerName ) );
		}
		public void RemoveChildLayer( IHierarchicalLayer child )
		{
			if ( child is null ) return;
			child.Super = null;

			if ( this._children.IsValueCreated == true ) this._children.Value.Remove( child );
		}
		public IHierarchicalLayer GetLayerByName( string layerName )
		{
			if ( string.IsNullOrEmpty( layerName ) == true ) return null;
			if ( this._children.IsValueCreated == false ) return null;
			return this._children.Value.Find( c => c.LayerName == layerName );
		}
		public IEnumerable<string> GetLayerNames()
		{
			foreach ( var Layer in this._children.Value )
			{
				yield return Layer.LayerName;
			}
		}
		#endregion

		#region IHierarchicalLayer.Selection
		public event LayerSelectChangedEventHandler LayerSelectChangedEvent;
		IHierarchicalLayer _selectedLayer = null;
		[Browsable( false )]
		public virtual IHierarchicalLayer SelectedLayer
		{
			get => this._selectedLayer;
			set
			{
				if ( this._selectedLayer == value ) return;

				var older = this._selectedLayer;
				this._selectedLayer = value;
				this.OnSelectChanged( older, value );
			}
		}
		protected virtual void OnSelectChanged( IHierarchicalLayer previous, IHierarchicalLayer current )
		{
			//Debug.WriteLine( $"{this.Name}.OnSelectChanged, [{previous?.LayerName}] => [{current?.LayerName}]" );
			previous?.OnPageActived( false );
			current?.OnPageActived( true );
			this.LayerSelectChangedEvent?.Invoke( this, new LayerSelectChangedEventArgs( previous, current ) );
		}
		#endregion

		#region ILockableUI 
		public event EventHandler<bool> LockStateChangedHandler;
		public event EventHandler<bool> LockStateChangedLockOnlyHandler;
		public event EventHandler<bool> LockStateChangedPrivilegeHandler;
		public event EventHandler<bool> LockStateChangedPrivilegeLockOnlyHandler;
		public event EventHandler<bool> LockStateChangedMachineStatusHandler;
		public event EventHandler<bool> LockStateChangedMachineStatusLockOnlyHandler;
		bool _locked_MachineStatus = false;
		public virtual bool LockUI_MachineStatus
		{
			get => this._locked_MachineStatus;
			set
			{
				if ( this._locked_MachineStatus == value ) return;
				this._locked_MachineStatus = value;
				this.OnLockStateChangedMachineStatus( value );
				this.LockUI = value || this._locked_Privilege;
				if ( value )
					this.OnLockStateChangedMachineStatusLockOnly( value );
			}
		}

		bool _locked_Privilege = false;
		public virtual bool LockUI_Privilege
		{
			get => this._locked_Privilege;
			set
			{
				if ( this._locked_Privilege == value ) return;
				this._locked_Privilege = value;
				this.OnLockStateChangedPrivilege( value );
				this.LockUI = value || this._locked_MachineStatus;
				if ( value )
					this.OnLockStateChangedPrivilegeLockOnly( value );
			}
		}

		bool _locked = false;
		public virtual bool LockUI
		{
			get => this._locked;
			set
			{
				if ( this._locked == value ) return;
				this._locked = value;
				this.OnLockStateChanged( value );
				if ( value )
					this.OnLockStateChangedLockOnly( value );
			}
		}
		public virtual void OnLockStateChanged( bool locked )
		{
			this.LockStateChangedHandler?.Invoke( this, locked );
		}
		public virtual void OnLockStateChangedPrivilege( bool locked )
		{
			this.LockStateChangedPrivilegeHandler?.Invoke( this, locked );
		}
		public virtual void OnLockStateChangedLockOnly( bool locked )
		{
			this.LockStateChangedLockOnlyHandler?.Invoke( this, locked );
		}
		public virtual void OnLockStateChangedPrivilegeLockOnly( bool locked )
		{
			this.LockStateChangedPrivilegeLockOnlyHandler?.Invoke( this, locked );
		}
		public virtual void OnLockStateChangedMachineStatus( bool locked )
		{
			this.LockStateChangedMachineStatusHandler?.Invoke( this, locked );
		}
		public virtual void OnLockStateChangedMachineStatusLockOnly( bool locked )
		{
			this.LockStateChangedMachineStatusLockOnlyHandler?.Invoke( this, locked );
		}
		#endregion

		#region Privilege
		[Browsable( false )]
		PrivilegeType _privilege = PrivilegeType.ReadOnly; //#JW changed to readonly on startup.
		public PrivilegeType Privilege
		{
			get => this._privilege;
			set
			{
				this._privilege = value;
				this.LockUI_Privilege = value != PrivilegeType.ReadWrite;
				this.Visible = value != PrivilegeType.Hidden;

				var layer = this as IHierarchicalLayer;
				var menu = this.MenuPanel;
				while ( menu == null && layer != null )
				{
					menu = ( layer?.Super as PagePanelBase )?.MenuPanel;
					layer = layer?.Super;
				}
				menu?.SetMenuItemEnabled( this.LayerName, this.Visible );
			}
		}
		#endregion
		#region UILocking
		public virtual void MachineStatusChangedUpdateUI( object sender, EventArgs e )
		{
			//if ( this.MenuPanel != null )
			//	if ( this.MenuPanel.Name == "mainMenu1" )
			//		this.MenuPanel?.SetMenuItemEnabled( "System", !Equipment.MachStateMgr.isMachineRunningSeq() );
		}

		public void LockPanelButtons( bool Lock )
		{
			UILockingControl.LockPanelButtons( this, Lock );
		}
		public void LockSaveCancelButtons( bool Lock )
		{
			UILockingControl.LockSaveCancelButtons( this, Lock );
		}
		public void LockAllTextBox( bool Lock )
		{
			UILockingControl.LockAllTextBox( this, Lock );
		}
		public void LockAllCheckBox( bool Lock )
		{
			UILockingControl.LockAllCheckBox( this, Lock );
		}
		public void LockAllTrackbar( bool Lock )
		{
			UILockingControl.LockAllTrackbar( this, Lock );
		}
		public void LockAllComboBox( bool Lock )
		{
			UILockingControl.LockAllComboBox( this, Lock );
		}
		public void LockAllListBox( bool Lock )
		{
			UILockingControl.LockAllListBox( this, Lock );
		}
		public void LockAllDataGridView( bool Lock )
		{
			UILockingControl.LockAllDataGridView( this, Lock );
		}
		public void LockAllToolStripButton( bool Lock )
		{
			UILockingControl.LockAllToolStripButton( this, Lock );
		}
		public void LockAllPanel( bool Lock )
		{
			this.LockPanelButtons( Lock );
			this.LockAllCheckBox( Lock );
			this.LockAllComboBox( Lock );
			this.LockAllDataGridView( Lock );
			this.LockAllListBox( Lock );
			this.LockAllTextBox( Lock );
			this.LockAllToolStripButton( Lock );
			this.LockAllTrackbar( Lock );
		}

		#endregion
		#region UIColorChange
		//public void SaveAllTextBox()
		//{
		//	var NumericUpDownCanReadOnly = this.GetAll( this, typeof( NumericUpDownCanReadOnly ) );
		//	foreach ( NumericUpDownCanReadOnly B in NumericUpDownCanReadOnly )
		//	{
		//		B.OnSave();
		//	};

		//	var TextBoxCanReadOnly = this.GetAll( this, typeof( TextBoxCanReadOnly ) );
		//	foreach ( TextBoxCanReadOnly B in TextBoxCanReadOnly )
		//	{
		//		B.OnSave();
		//	};
		//}

		//public void CancelAllTextBox()
		//{
		//	var NumericUpDownCanReadOnly = this.GetAll( this, typeof( NumericUpDownCanReadOnly ) );
		//	foreach ( NumericUpDownCanReadOnly B in NumericUpDownCanReadOnly )
		//	{
		//		B.OnCancel();
		//	};

		//	var TextBoxCanReadOnly = this.GetAll( this, typeof( TextBoxCanReadOnly ) );
		//	foreach ( TextBoxCanReadOnly B in TextBoxCanReadOnly )
		//	{
		//		B.OnSave();
		//	};
		//}


		#endregion
		#region Watch received windows message
		protected override void WndProc( ref Message m )
		{
			if ( m.Msg == ( int )WindowMessage.WM_CREATE )
			{
				this.OnPageLoad();
			}
			else if ( m.Msg == ( int )WindowMessage.WM_DESTROY ) this.OnPageDestroy();

			base.WndProc( ref m );
		}
		#endregion
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// PagePanelBase
			// 
			this.Name = "PagePanelBase";
			this.ResumeLayout( false );

		}
	}

	#region Layer Select Event & Handler 
	public class LayerSelectChangedEventArgs : EventArgs
	{
		public IHierarchicalLayer Previous { get; }
		public IHierarchicalLayer Current { get; }
		public LayerSelectChangedEventArgs( IHierarchicalLayer previous, IHierarchicalLayer current )
		{
			this.Previous = previous;
			this.Current = current;
		}
	}
	public delegate void LayerSelectChangedEventHandler( object sender, LayerSelectChangedEventArgs e );

	public class LayerActiveChangedEventArgs : EventArgs
	{
		public IHierarchicalLayer Layer { get; }
		public bool IsActived { get; }
		public LayerActiveChangedEventArgs( IHierarchicalLayer layer, bool isActived )
		{
			this.Layer = layer;
			this.IsActived = isActived;
		}
	}
	public delegate void LayerActiveChangedEventHandler( object sender, LayerActiveChangedEventArgs e );
	#endregion

	#region FormExtension
	public static class FormExtension
	{
		public static void FindControls( this Control control, Func<Control, bool> finder, int depth, ref IList<Control> result )
		{
			if ( depth <= 0 ) return;

			foreach ( Control item in control.Controls )
			{
				if ( finder( item ) == true )
					result.Add( item );
				item.FindControls( finder, depth - 1, ref result );
			}
		}

		public static Control FindControlByName( this Control control, string name )
		{
			if ( control == null ) return null;
			if ( string.IsNullOrEmpty( name ) == true ) return null;

			foreach ( Control item in control.Controls )
			{
				if ( item != null && item.Name == name ) return item;
				var res = item.FindControlByName( name );
				if ( res != null ) return res;
			}
			return null;
		}
	}
	#endregion
}
