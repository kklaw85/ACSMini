using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTVCSEL.Common
{
	public delegate int IntEventHandler<T>( object sender, T t );
	public class EventArgsT<T1>
		: EventArgs
	{
		public EventArgsT( T1 t1 )
		{
			this.Arg1 = t1;
		}

		public T1 Arg1;
	}

	public class EventArgsT<T1, T2>
		: EventArgs
	{
		public EventArgsT( T1 t1, T2 t2 )
		{
			this.Arg1 = t1;
			this.Arg2 = t2;
		}

		public T1 Arg1;
		public T2 Arg2;
	}

	public class EventArgsT<T1, T2, T3>
		: EventArgs
	{
		public EventArgsT( T1 t1, T2 t2, T3 t3 )
		{
			this.Arg1 = t1;
			this.Arg2 = t2;
			this.Arg3 = t3;
		}

		public T1 Arg1;
		public T2 Arg2;
		public T3 Arg3;
	}

	public class EventArgsT<T1, T2, T3, T4>
		: EventArgs
	{
		public EventArgsT( T1 t1, T2 t2, T3 t3, T4 t4 )
		{
			this.Arg1 = t1;
			this.Arg2 = t2;
			this.Arg3 = t3;
			this.Arg4 = t4;
		}

		public T1 Arg1;
		public T2 Arg2;
		public T3 Arg3;
		public T4 Arg4;
	}

	public class EventArgsT<T1, T2, T3, T4, T5>
		: EventArgs
	{
		public EventArgsT( T1 t1, T2 t2, T3 t3, T4 t4, T5 t5 )
		{
			this.Arg1 = t1;
			this.Arg2 = t2;
			this.Arg3 = t3;
			this.Arg4 = t4;
			this.Arg5 = t5;
		}

		public T1 Arg1;
		public T2 Arg2;
		public T3 Arg3;
		public T4 Arg4;
		public T5 Arg5;
	}
}
