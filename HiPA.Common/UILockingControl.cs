using HiPA.Common.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace HiPA.Common
{
	public class UILockingControl
	{
		public static void GetLogicalChildCollection<T>( DependencyObject parent, List<T> logicalCollection ) where T : DependencyObject
		{
			IEnumerable children = LogicalTreeHelper.GetChildren( parent );
			foreach ( object child in children )
			{
				if ( child is DependencyObject )
				{
					DependencyObject depChild = child as DependencyObject;
					if ( child is T )
					{
						logicalCollection.Add( child as T );
					}
					GetLogicalChildCollection( depChild, logicalCollection );
				}
			}
		}

		public static IEnumerable<Control> GetAll( Control control, Type type )
		{
			var controls = control.Controls.Cast<Control>();

			return controls.SelectMany( ctrl => GetAll( ctrl, type ) )
									  .Concat( controls )
									  .Where( c => c.GetType() == type );
		}

		public static IEnumerable<Control> GetAllByGroup( Control control, Type type, string GroupName )
		{
			var controls = control.Controls.Find( GroupName, true );
			//var controls = control.Controls.Cast<Control>();
			return controls.SelectMany( ctrl => GetAll( ctrl, type ) )
									  .Concat( controls )
									  .Where( c => c.GetType() == type );
		}

		public static IEnumerable<Control> GetAllInType( Control control, Type type, Type type2 )
		{
			var controls = control.Controls.Cast<Control>();
			var control1 = controls.SelectMany( ctrl => GetAll( ctrl, type ) )
									  .Concat( controls )
									  .Where( c => c.GetType() == type );
			return control1.SelectMany( ctrl => GetAll( ctrl, type2 ) )
						  .Concat( control1 )
						  .Where( c => c.GetType() == type2 );
		}


		public static void LockPanelButtons( Control control, bool Lock )
		{
			var enable = !Lock;
			var Buttons = UILockingControl.GetAll( control, typeof( Button ) );
			var ButtonsPagePanelTitle = UILockingControl.GetAllInType( control, typeof( PagePanelTitle ), typeof( Button ) );
			var ButtonsPnlSaveCancel = UILockingControl.GetAllByGroup( control, typeof( Button ), "pnlSaveOrCancel" );
			foreach ( Button B in Buttons )
			{
				if ( ButtonsPagePanelTitle.Contains( B ) )
					continue;
				if ( ButtonsPnlSaveCancel.Contains( B ) )
					continue;
				//if ( B.Tag == "LockCheck" && ( UserManagementConfiguration.isHighAuthorization ) )
				//{
				//	//Console.WriteLine( $"UserManagementConfiguration.GroupIDState: {UserManagementConfiguration.GroupIDState}" );
				//	B.Enabled = true;
				//}
				//else
				//	B.Enabled = enable;
			};
		}
		public static void LockSaveCancelButtons( Control control, bool Lock )
		{
			var enable = !Lock;
			var ButtonsPnlSaveCancel = UILockingControl.GetAllByGroup( control, typeof( Button ), "pnlSaveOrCancel" );
			foreach ( Button B in ButtonsPnlSaveCancel )
			{
				B.Enabled = enable;
			};
		}
		public static void LockAllTextBox( Control control, bool Lock )
		{
			var enable = !Lock;
			var NumericUpDownCanReadOnly = UILockingControl.GetAll( control, typeof( NumericUpDownCanReadOnly ) );
			//foreach ( NumericUpDownCanReadOnly B in NumericUpDownCanReadOnly )
			//{
			//	if ( B.Tag == "LockCheck" && ( UserManagementConfiguration.isHighAuthorization ) )
			//		B.ReadOnly = false;
			//	else
			//		B.ReadOnly = Lock;
			//};
			var TextBoxCanReadOnly = UILockingControl.GetAll( control, typeof( TextBoxCanReadOnly ) );
			foreach ( TextBoxCanReadOnly B in TextBoxCanReadOnly )
			{
				B.ReadOnly = Lock;
			};
			var TextBox = UILockingControl.GetAll( control, typeof( TextBox ) );
			foreach ( TextBox B in TextBox )
			{
				B.ReadOnly = Lock;
			};
			var NumericUpDown = UILockingControl.GetAll( control, typeof( NumericUpDown ) );
			foreach ( NumericUpDown B in NumericUpDown )
			{
				B.ReadOnly = Lock;
			};
		}
		public static void LockAllCheckBox( Control control, bool Lock )
		{
			var enable = !Lock;
			var Checkbox = UILockingControl.GetAll( control, typeof( CheckBox ) );
			foreach ( CheckBox B in Checkbox )
			{
				B.Enabled = enable;
			};
		}
		public static void LockAllTrackbar( Control control, bool Lock )
		{
			var enable = !Lock;
			var TrackBar = UILockingControl.GetAll( control, typeof( TrackBar ) );
			foreach ( TrackBar B in TrackBar )
			{
				B.Enabled = enable;
			};
		}
		public static void LockAllComboBox( Control control, bool Lock )
		{
			var enable = !Lock;
			var Combobox = UILockingControl.GetAll( control, typeof( ComboBox ) );
			foreach ( ComboBox B in Combobox )
			{
				//if ( B.Tag == "LockCheck" && ( UserManagementConfiguration.isHighAuthorization ) )
				//	B.Enabled = true;
				//else
				//	B.Enabled = enable;
			};
		}
		public static void LockAllListBox( Control control, bool Lock )
		{
			var enable = !Lock;
			var ListBox = UILockingControl.GetAll( control, typeof( ListBox ) );
			foreach ( ListBox B in ListBox )
			{
				B.Enabled = enable;
			};
		}
		public static void LockAllDataGridView( Control control, bool Lock )
		{
			try
			{
				var enable = !Lock;
				var DataGridView = UILockingControl.GetAll( control, typeof( DataGridView ) );
				foreach ( DataGridView B in DataGridView )
				{
					B.ReadOnly = Lock;
				};
			}
			catch
			{ }
		}
		public static void LockAllToolStripButton( Control control, bool Lock )
		{
			var enable = !Lock;
			var toolstrips = UILockingControl.GetAll( control, typeof( ToolStrip ) );
			foreach ( ToolStrip ts in toolstrips )
			{
				foreach ( ToolStripItem tsi in ts.Items )
				{
					if ( ts.Items.Count > 0 )
					{
						if ( tsi is ToolStripButton )
						{
							ToolStripButton tb = tsi as ToolStripButton;
							tb.Enabled = enable;
						}
					}
				}
			}
		}
		public static void LockAllUI( Control control, bool Lock )
		{
			UILockingControl.LockPanelButtons( control, Lock );
			UILockingControl.LockAllTextBox( control, Lock );
			UILockingControl.LockAllCheckBox( control, Lock );
			UILockingControl.LockAllTrackbar( control, Lock );
			UILockingControl.LockAllComboBox( control, Lock );
			UILockingControl.LockAllListBox( control, Lock );
			UILockingControl.LockAllDataGridView( control, Lock );
			UILockingControl.LockAllToolStripButton( control, Lock );
		}

	}

}
