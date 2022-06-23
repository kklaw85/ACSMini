using HiPA.Common.Recipe;
using HiPA.Common.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HiPA.Common.Forms
{
	public class B_Utility : System.ICloneable
	{
		#region Errorformatting
		protected string FormatErrMsg( string Name, Exception ex )
		{
			var TypeName = this.GetType().Name;
			var st = new StackTrace();
			var sf = st.GetFrame( 1 );
			var MethodNameArr = sf.GetMethod().Name.Split( '<', '>' );
			var MethodName = MethodNameArr.Length == 3 ? MethodNameArr[ 1 ] : MethodNameArr[ 0 ];
			string fullMethodName = $"{TypeName}:{MethodName}:,";
			return $"{Name}:{fullMethodName}{ex.Message}";
		}
		protected string FormatErrMsg2( string Name, Exception ex )
		{
			var TypeName = this.GetType().Name;
			var st = new StackTrace();
			var sf = st.GetFrame( 2 );
			var MethodNameArr = sf.GetMethod().Name.Split( '<', '>' );
			var MethodName = MethodNameArr.Length == 3 ? MethodNameArr[ 1 ] : MethodNameArr[ 0 ];
			string fullMethodName = $"{TypeName}:{MethodName}:,";
			return $"{Name}:{fullMethodName}{ex.Message}";
		}
		#endregion
		#region Shared error handling
		protected ErrorResult Result { get; private set; } = new ErrorResult();
		protected void ThrowError( string ErrorMessage )
		{
			if ( string.IsNullOrEmpty( ErrorMessage ) ) this.ClearErrorFlags();
			this.Result.Set( ErrorClass.E6, ErrorMessage );
			throw new Exception( this.Result.ErrorMessage );
		}
		protected void CatchException( Exception ex )
		{
			this.Result.Set( this.Result.EClass == ErrorClass.OK ? ErrorClass.E6 : this.Result.EClass, this.FormatErrMsg2( null, ex ) );
		}
		protected void CatchException( ErrorClass eclass, string err )
		{
			this.Result.Set( eclass, err );
		}
		protected void CatchAndPromptErr( Exception ex )
		{
			this.CatchException( ex );
			Equipment.ErrManager.RaiseError( null, this.Result.ErrorMessage, ErrorTitle.OperationFailure, this.Result.EClass );
		}
		protected void ClearErrorFlags()
		{
			this.Result.Reset();
		}
		protected void ThrowError( ErrorClass EClass, string ErrorMessage )
		{
			this.Result.Set( EClass, ErrorMessage );
			throw new Exception( ErrorMessage );
		}
		private void ThrowError( ErrorResult Result )
		{
			this.Result.Set( Result );
			throw new Exception( Result.ErrorMessage );
		}
		protected void CheckAndThrowIfError( string Result )
		{
			if ( string.IsNullOrEmpty( Result ) )
			{
				this.ClearErrorFlags();
				return;
			}
			this.Result.Set( ErrorClass.E6, Result );
			if ( this.Result.EClass != ErrorClass.OK ) this.ThrowError( this.Result );
			else this.ClearErrorFlags();
		}
		protected void CheckAndThrowIfError( ErrorResult Result )
		{
			if ( Result == null )
			{
				this.ClearErrorFlags();
				return;
			}
			this.Result.Set( Result );
			if ( this.Result.EClass != ErrorClass.OK ) this.ThrowError( this.Result );
			else this.ClearErrorFlags();
		}
		protected void CheckAndThrowIfError( ErrorClass EClassIfFail, string ErrorMessage )
		{
			this.Result.Set( EClassIfFail, ErrorMessage );
			if ( this.Result.ErrorMessage != string.Empty ) this.ThrowError( this.Result );
			else this.ClearErrorFlags();
		}
		protected void CheckAndThrowIfError( Task<ErrorResult>[] tasks )
		{
			Task.WaitAll( tasks );
			foreach ( var task in tasks )
			{
				this.CheckAndThrowIfError( task.Result );
			}
			this.ClearErrorFlags();
		}
		protected void CheckAndThrowIfError( ErrorClass EClass, Task<string>[] tasks )
		{
			Task.WaitAll( tasks );
			foreach ( var task in tasks )
			{
				this.CheckAndThrowIfError( EClass, task.Result );
			}
			this.ClearErrorFlags();
		}
		#endregion
		#region Cloneable
		public object Clone() { return this.MemberwiseClone(); }
		#endregion
		#region Getname
		protected string GetName( [CallerMemberName] string name = null )
		{
			return name;
		}
		#endregion
	}

	public class TicToc
	{
		private Stopwatch SW = new Stopwatch();
		public void Tic()
		{
			this.SW.Restart();
		}
		public Stopwatch Toc()
		{
			this.SW.Stop();
			return this.SW;
		}
		public double LapseTime()
		{
			return this.SW.ElapsedMilliseconds;
		}
		public void Clear()
		{
			this.SW.Reset();
		}
	}


	public class BaseUtility : B_Utility, INotifyPropertyChanged
	{
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
	public class RecipeBaseUtility : B_Utility, INotifyPropertyChanged, System.ICloneable
	{
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
			RecipeNoticeHandler.RecipeChanged = true;
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
	public interface ILockableUI
	{
		event EventHandler<bool> LockStateChangedHandler;
		bool LockUI { get; set; }
	}
	public interface ILockablePage
	{
		UILockability PrivilegeLockability { get; set; }
		bool MachineStateLockable { get; set; }
		AccessLevel MinRead { get; set; }
		AccessLevel MinWrite { get; set; }
		PrivilegeType Privilege { get; set; }
		AccessLevel AccessLevel { get; set; }
		void Unload();
		void Load();
		PrivilegeType CheckPrivilege( AccessLevel Level );
		bool PagePrivLockable();
		bool PagePrivConfigurable();
	}
	public enum UILockability
	{
		Nonlockable,
		LockableNotConfigurable,
		LockableAndConfigurable,
	}
	public interface IPollingUpdatable
	{
		void OnDisplayData();
	}

	//public interface ISetBinding
	//{
	//	void OnSetupBinding();
	//}

	public interface IArchivableUI
	{
		void SaveModification();
		void CancelModification();
		void UpdateToUI();
		void UpdateToConfig();
	}

	public enum ArchiveAction
	{
		Save,
		Cancel,
		Exit,
	}
	public interface IArchiveActionNotifier
	{
		event EventHandler<ArchiveAction> ArchiveActionEvent;
	}

	public enum PrivilegeType
	{
		ReadWrite,
		ReadOnly,
		Hidden,
	}

	public interface IHierarchicalLayer
	{
		IHierarchicalLayer Super { get; set; }
		IHierarchicalLayer SelectedLayer { get; set; }

		string LayerName { get; }
		string Path { get; }
		string PathSaparator { get; }

		bool HasChild { get; }
		IList<IHierarchicalLayer> GetChildren();

		void OnPageActived( bool isActived );
		void OnDisplayData();
		PrivilegeType Privilege { get; set; }
	}

	public interface IPagePanelBase
	{
		void OnPageLoad();
		void OnPageDestroy();
	}

	public class ROISynchronizeEventArgs
		: EventArgs
	{
		public string ElementName;
		public RectangleD? Area;
		public bool? ReadOnly = null;
		public bool? IsVisible = null;
	}

	public interface IROISynchronizerProvider
	{
		IROISynchronizer ROISynchronizer { get; set; }
	}

	public interface IROISynchronizer
	{
		IList<string> ElementNames { get; }
		string CurrentElement { get; }
		RectangleD Area { get; }

		event EventHandler<ROISynchronizeEventArgs> AreaStateChanged;
		void AreaStateChangedFromDisplay( string elementName, RectangleD area );

		bool IsVisibleElements { get; set; }
	}

	public interface ISyncFinderResult
	{
		RectangleD Area { get; set; }
	}

	public interface ISplashWindow
	{
		void Progress( string message );
		void Error( string error );
	}

	public static class HierarchicalLayerExtension
	{
		public static void ForEachChildren( this IHierarchicalLayer layer, Action<IHierarchicalLayer> action )
		{
			if ( action == null ) return;
			if ( layer == null || layer.HasChild == false ) return;

			foreach ( var child in layer.GetChildren() )
			{
				if ( child != null )
				{
					action( child );
					child.ForEachChildren( action );
				}
			}
		}
		public static void ForEachSupers( this IHierarchicalLayer layer, Action<IHierarchicalLayer> action )
		{
			if ( action == null ) return;
			if ( layer == null || layer.Super == null ) return;

			var super = layer.Super;
			action( super );
			super.ForEachSupers( action );
		}

		public static string GetPath( this IHierarchicalLayer layer, string pathSaparator = null )
		{
			if ( layer == null ) return "";
			//if ( layer.Super == null ) return layer.LayerName;
			if ( pathSaparator is null ) pathSaparator = layer.PathSaparator;

			var front = layer.Super?.GetPath( pathSaparator );
			if ( string.IsNullOrEmpty( front ) == true ) return layer.LayerName;
			return string.Join( pathSaparator, front, layer.LayerName );
		}

		public static IHierarchicalLayer GetAncestor( this IHierarchicalLayer layer )
		{
			if ( layer == null ) return null;
			if ( layer.Super == null ) return layer;
			return layer.Super.GetAncestor();
		}
	}
}
