﻿using HiPA.Common.Forms;
using MahApps.Metro.Controls;
using N_Data_Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace HiPA.Common.UControl
{
	public abstract class PanelBase : UserControl, INotifyPropertyChanged
	{
		protected List<PanelBase> Panels = new List<PanelBase>();
		public PanelBase()
		{
			this.Loaded += this.PanelBase_Loaded;
			Equipment.MachStateMgr.MachineStateChanged += this.MachStateMgr_MachineStateChanged;
		}
		#region Eventhandler
		private void PanelBase_Loaded( object sender, EventArgs e )
		{
			this.Panels = this.FindChild<PanelBase>();
		}
		private void MachStateMgr_MachineStateChanged( object sender, MachineStateMgr.MachineStateChangeEventArgs e )
		{
			this.LockUI_MachineState = e.LockUI;
		}
		#endregion
		#region Lock UI 
		#region Configuration flags
		public bool MachineStateLockable { get; set; } = true;
		public bool PrivilegeLockable { get; set; } = true;
		private PrivilegeType _privilege = PrivilegeType.ReadOnly; //Change this to hidden to start User Locking Feature.
		#endregion
		#region Locking interface flags
		public PrivilegeType Privilege
		{
			get => this._privilege;
			set
			{
				this._privilege = value;
				if ( this.eAccessLevel == AccessLevel.Manufacturer )
					this._privilege = PrivilegeType.ReadWrite;
				this.LockUI_Privilege = this._privilege == PrivilegeType.ReadWrite ? false : true;
				foreach ( var panel in this.Panels )
					panel.Privilege = this._privilege;
			}
		}
		private AccessLevel eAccessLevel = AccessLevel.Guest;
		public AccessLevel AccessLevel
		{
			get => this.eAccessLevel;
			set
			{
				this.Set( ref this.eAccessLevel, value, "AccessLevel" );
				foreach ( var panel in this.Panels )
				{
					panel.AccessLevel = this.eAccessLevel;
				}
			}
		}
		#endregion
		#region LockFlags
		private bool _LockUIMachineState = false;
		public virtual bool LockUI_MachineState
		{
			get => this._LockUIMachineState;
			set
			{
				this.Set( ref this._LockUIMachineState, value && this.MachineStateLockable, "LockUI_MachineState" );
				this.LockUI = this._LockUIPrivilege || this._LockUIMachineState;
			}
		}
		private bool _LockUIPrivilege = false;
		public virtual bool LockUI_Privilege
		{
			get => this._LockUIPrivilege;
			set
			{

				this.Set( ref this._LockUIPrivilege, value && this.PrivilegeLockable, "LockUI_Privilege" );
				this.LockUI = this._LockUIPrivilege || this._LockUIMachineState;
			}
		}
		private bool _LockUI = false;
		public virtual bool LockUI
		{
			get => this._LockUI;
			set
			{
				if ( this.eAccessLevel == AccessLevel.Manufacturer )
					this.Set( ref this._LockUI, false, "LockUI" );
				else
					this.Set( ref this._LockUI, value, "LockUI" );
			}
		}
		#endregion
		#region Bindlock
		protected void BindLockUI( FrameworkElement Object )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Converter = new InverseBool();
			b.Path = new PropertyPath( "LockUI" );
			Object.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		protected void BindLockUI( IEnumerable<FrameworkElement> Object )
		{
			foreach ( FrameworkElement element in Object )
			{
				this.BindLockUI( element );
			}
		}
		protected void BindLockPrivilege( FrameworkElement Object )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Converter = new InverseBool();
			b.Path = new PropertyPath( "LockUI_Privilege" );
			Object.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		protected void BindLockPrivilege( IEnumerable<FrameworkElement> Object )
		{
			foreach ( FrameworkElement element in Object )
			{
				this.BindLockPrivilege( element );
			}
		}
		protected void BindLockMachineState( FrameworkElement Object )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Converter = new InverseBool();
			b.Path = new PropertyPath( "LockUI_MachineState" );
			Object.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		protected void BindLockMachineState( IEnumerable<FrameworkElement> Object )
		{
			foreach ( FrameworkElement element in Object )
			{
				this.BindLockMachineState( element );
			}
		}
		protected void BindLockAccessLevelManufacturer( FrameworkElement Object )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Converter = new AccessLevelManufacturerToBool();
			b.Path = new PropertyPath( "AccessLevel" );
			Object.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		protected void BindLockAccessLevelManufacturer( IEnumerable<FrameworkElement> Object )
		{
			foreach ( FrameworkElement element in Object )
			{
				this.BindLockAccessLevelManufacturer( element );
			}
		}
		protected void BindLockAccessLevelEngineer( FrameworkElement Object )
		{
			Binding b = new Binding();
			b.Source = this;
			b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			b.Converter = new AccessLevelEngAboveToBool();
			b.Path = new PropertyPath( "AccessLevel" );
			Object.SetBinding( FrameworkElement.IsEnabledProperty, b );
		}
		protected void BindLockAccessLevelEngineer( IEnumerable<FrameworkElement> Object )
		{
			foreach ( FrameworkElement element in Object )
			{
				this.BindLockAccessLevelEngineer( element );
			}
		}
		#endregion
		#endregion
		#region Entry generator
		protected Dictionary<string, StackPanel> Entries = new Dictionary<string, StackPanel>();
		#endregion
		#region Setupbinding
		protected void OnSetupBinding( object _Source, StackPanel Content )
		{
			try
			{
				if ( _Source == null ) return;
				this.Entries.Clear();
				Content.Children.Clear();
				var Properties = _Source.GetType().GetProperties();
				foreach ( var prop in Properties )
				{
					if ( prop.PropertyType == typeof( double ) )
					{
						var stk = new StackPanel();
						stk.Name = prop.Name;
						stk.Orientation = Orientation.Horizontal;
						stk.Margin = new Thickness( 0, 0, 0, 5 );
						var lbl = new Label();
						lbl.Content = ( string )this.FindResource( prop.Name );
						lbl.Style = ( Style )this.FindResource( "LB_ItemContent_Right" );
						lbl.Width = 100;
						stk.Children.Add( lbl );

						var numericupdown = new NumericUpDown();

						numericupdown.Width = 80;
						numericupdown.StringFormat = "F3";
						stk.Children.Add( numericupdown );
						Binding b = new Binding();
						b.Source = _Source;
						b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
						b.Path = new PropertyPath( prop.Name );
						numericupdown.SetBinding( NumericUpDown.ValueProperty, b );
						if ( !this.Entries.ContainsKey( prop.Name ) )
						{
							this.Entries[ prop.Name ] = stk;
							Content.Children.Add( stk );
						}
					}
				}
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseWarning( this.FormatErrMsg( this.Name, ex ), ErrorTitle.InvalidOperation );
			}
		}
		#endregion
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
		#region Expanderhandler
		protected void HandleExpanderExpanded( object sender, RoutedEventArgs e )
		{
			this.ExpandExculsively( sender as Expander );
		}
		private Panel Expanderpnl;
		protected void RegisterExpanderPnl( Panel source )
		{
			this.Expanderpnl = source;
		}
		private void ExpandExculsively( Expander expander )
		{
			foreach ( var child in this.Expanderpnl.Children )
			{
				if ( child is Expander && child != expander )
					( ( Expander )child ).IsExpanded = false;
			}
		}
		#endregion




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
		#region MIT binding
		private Dictionary<string, object> propertyValueStorage;
		#region GetProperty
		protected T GetValue<T>( Expression<Func<T>> property )
		{
			var lambdaExpression = property as LambdaExpression;

			if ( lambdaExpression == null )
			{
				throw new ArgumentException( @"Lambda expression return value can't be null", "property" );
			}

			string propertyName = GetPropertyName( lambdaExpression );
			return this.GetValue<T>( propertyName );
		}

		private static string GetPropertyName( LambdaExpression lambdaExpression )
		{
			MemberExpression memberExpression;

			if ( lambdaExpression.Body is UnaryExpression )
			{
				var unaryExpression = lambdaExpression.Body as UnaryExpression;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}
			else
			{
				memberExpression = lambdaExpression.Body as MemberExpression;
			}

			return memberExpression == null ? null : memberExpression.Member.Name;
		}

		private T GetValue<T>( string propertyName )
		{
			object value;
			if ( this.propertyValueStorage == null )
				this.propertyValueStorage = new Dictionary<string, object>();
			if ( this.propertyValueStorage.TryGetValue( propertyName, out value ) )
			{
				return ( T )value;
			}

			return default( T );
		}
		#endregion
		#region SetProperty
		[SuppressMessage( "StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
				Justification = "Required as Equals can handle null refs." )]
		protected bool SetValue<T>( Expression<Func<T>> property, T value, bool compareBeforeTrigger = true )
		{
			var lambdaExpression = property as LambdaExpression;

			if ( lambdaExpression == null )
			{
				throw new ArgumentException( @"Lambda expression return value can't be null", "property" );
			}

			string propertyName = GetPropertyName( lambdaExpression );
			var storedValue = this.GetValue<T>( propertyName );

			if ( compareBeforeTrigger )
			{
				if ( typeof( T ) == typeof( Uri ) && storedValue != null )
				{
					if ( Equals( storedValue.ToString(), value.ToString() ) )
						return false;
				}
				else
				{
					if ( Equals( storedValue, value ) )
						return false;
				}
			}
			this.propertyValueStorage[ propertyName ] = value;
			this.OnPropertyChanged( propertyName );

			return true;
		}
		#endregion
		#endregion
		#region inotifypropertychanged
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		protected virtual void OnPropertyChanged<T>( Expression<Func<T>> raiser )
		{
			var propName = ( ( MemberExpression )raiser.Body ).Member.Name;
			this.OnPropertyChanged( propName );
		}

		protected bool Set<T>( ref T field, T value, [CallerMemberName] string name = null )
		{
			//if ( !EqualityComparer<T>.Default.Equals( field, value ) )
			{
				field = value;
				this.OnPropertyChanged( name );
				return true;
			}
			//return false;
		}
		#endregion
	}
}