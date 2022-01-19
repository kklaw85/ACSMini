using System;
using System.Threading;

namespace HiPA.Common
{
	[Serializable]
	public enum ServiceState
	{
		None,
		InService = 0x10,
		OutOfService = 0x20,
		SelectedByUser = 0x100,
		SelectedByEquipment = 0x200,
	}

	public abstract class ModuleBase
		: InstrumentBase
	{
		public ModuleBase()
		{
			this.AlarmAgent = new AlarmAgent();
			//this.AlarmAgent.ErrorAction += ( s, a ) => this.Stop();
			this.AlarmAgent.FatalAction += ( s, a ) => this.Stop();
		}

		#region Service State 
		int _serviceState = 0;
		public ServiceState ServiceState
		{
			get => ( ServiceState )Interlocked.CompareExchange( ref this._serviceState, 0, 0 );
			set
			{
				var old = this._serviceState;
				if ( Interlocked.Exchange( ref this._serviceState, ( int )value ) != ( int )value )
					this.OnServiceStateChanged( value );
			}
		}
		protected virtual void OnServiceStateChanged( ServiceState state ) { }
		#endregion
	}
}
