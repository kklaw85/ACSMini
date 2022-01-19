using HiPA.Common;
using HiPA.Common.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace HiPA.Instrument.Motion
{

	public abstract class MotionBoardBase
		: ModuleBase
	{
		public abstract string Open();
		public abstract bool IsOpen();
		public abstract string Close();
		protected abstract string CreateAxes();
	}

	public abstract class IoBoardBase
		: ModuleBase
	{
		public abstract string Open();
		public abstract bool IsOpen();
		public abstract string Close();
		protected abstract string CreateIOs();
		public abstract uint SetOutput( int ModuleNo, int Line, DioValue value );
		public abstract uint GetInputs( int ModuleNo );
		public abstract uint GetOutputs( int ModuleNo );
	}

	internal abstract class CommonBoardBase : BaseUtility, IDisposable
	{
		public ObservableCollection<int> BoardHandler { get; protected set; } = new ObservableCollection<int>();
		public abstract int Open();
		public abstract int Close();

		bool _isDisposed = false;
		public void Dispose()
		{
			if ( this._isDisposed == false )
			{
				this._isDisposed = true;
				this.Close();
			}
		}

		#region Singleton 
		static object s_SyncRoot = new object();
		static Dictionary<Type, CommonBoardBase> s_Instances = new Dictionary<Type, CommonBoardBase>();

		internal static CommonBoardBase GetInstance( Type typeOfInstance = null )
		{
			try
			{
				CommonBoardBase board = null;

				Monitor.Enter( s_SyncRoot );
				if ( s_Instances.TryGetValue( typeOfInstance, out board ) == false )
				{
					if ( typeOfInstance == null || typeof( CommonBoardBase ).IsAssignableFrom( typeOfInstance ) == false ) return null;

					board = Activator.CreateInstance( typeOfInstance ) as CommonBoardBase;
					s_Instances[ typeOfInstance ] = board;
				}

				return board;
			}
			finally
			{
				Monitor.Exit( s_SyncRoot );
			}
		}

		internal static CommonBoardBase GetInstance<T>()
		{
			return GetInstance( typeof( T ) );
		}
		#endregion
	}
}
