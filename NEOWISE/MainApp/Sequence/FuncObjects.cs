using System;

namespace NeoWisePlatform
{
	public class FunctionObjects
	{
		private Func<int> OnExecute;
		private Func<int, int> OnError;
		public bool isInvokeReq = false;
		public string Name = "";

		public FunctionObjects( string myName, Func<int> onExeFunction )
		{
			this.Name = myName;
			this.OnExecute += onExeFunction;
		}

		public FunctionObjects( string myName, Func<int> onExeFunction, Func<int, int> onErrorFunction )
		{
			this.Name = myName;
			this.OnExecute += onExeFunction;
			this.OnError += onErrorFunction;
		}

		public FunctionObjects( string myName, Func<int> onExeFunction, Func<int, int> onErrorFunction, bool isInvoke )
		{
			this.Name = myName;
			this.OnExecute += onExeFunction;
			this.OnError += onErrorFunction;
			this.isInvokeReq = isInvoke;
		}

		public bool OnErrorAssigned()
		{
			return ( this.OnError != null );
		}

		public int Execute()
		{
			if ( this.OnExecute == null )
				return 0;
			return this.OnExecute();
		}

		public int DoError( int Res )
		{
			if ( this.OnError == null )
				return 0;
			return this.OnError( Res );
		}

	}
}
