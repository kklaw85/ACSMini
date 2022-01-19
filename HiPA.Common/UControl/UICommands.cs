using System;
using System.Windows.Input;

namespace HiPA.Common
{
	public class RelayCommand : ICommand
	{
		Action _TargetExecuteMethod;
		Func<bool> _TargetCanExecuteMethod;

		public RelayCommand( Action executeMethod )
		{
			this._TargetExecuteMethod = executeMethod;
		}

		public RelayCommand( Action executeMethod, Func<bool> canExecuteMethod )
		{
			if ( executeMethod == null )
				throw new ArgumentNullException( "execute" );
			this._TargetExecuteMethod = executeMethod;
			this._TargetCanExecuteMethod = canExecuteMethod;
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged( this, EventArgs.Empty );
		}
		#region ICommand Members

		bool ICommand.CanExecute( object parameter )
		{
			if ( this._TargetCanExecuteMethod != null )
			{
				return this._TargetCanExecuteMethod();
			}
			else
			{
				return true;
			}
			//if (_TargetExecuteMethod != null)
			//{
			//    return true;
			//}
			//return false;
		}

		// Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command
		// Prism commands solve this in their implementation
		public event EventHandler CanExecuteChanged = delegate { };

		//public event EventHandler CanExecuteChanged
		//{
		//    add { CommandManager.RequerySuggested += value; }
		//    remove { CommandManager.RequerySuggested -= value; }
		//}

		void ICommand.Execute( object parameter )
		{
			if ( this._TargetExecuteMethod != null )
			{
				this._TargetExecuteMethod();
			}
		}
		#endregion
	}

	public class RelayCommand<T> : ICommand
	{
		Action<T> _TargetExecuteMethod;
		Func<T, bool> _TargetCanExecuteMethod;

		public RelayCommand( Action<T> executeMethod )
		{
			this._TargetExecuteMethod = executeMethod;
		}

		public RelayCommand( Action<T> executeMethod, Func<T, bool> canExecuteMethod )
		{
			this._TargetExecuteMethod = executeMethod;
			this._TargetCanExecuteMethod = canExecuteMethod;
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged( this, EventArgs.Empty );
		}
		#region ICommand Members

		bool ICommand.CanExecute( object parameter )
		{
			if ( this._TargetCanExecuteMethod != null )
			{
				T tparm = ( T )parameter;
				return this._TargetCanExecuteMethod( tparm );
			}
			if ( this._TargetExecuteMethod != null )
			{
				return true;
			}
			return false;
		}

		// Beware - should use weak references if command instance lifetime is longer than lifetime of UI objects that get hooked up to command
		// Prism commands solve this in their implementation
		public event EventHandler CanExecuteChanged = delegate { };

		void ICommand.Execute( object parameter )
		{
			if ( this._TargetExecuteMethod != null )
			{
				this._TargetExecuteMethod( ( T )parameter );
			}
		}
		#endregion
	}

}
