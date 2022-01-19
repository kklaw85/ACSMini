using HiPA.Common;
using System;

namespace HiPA.Instrument.Motion
{
	[Serializable]
	public enum DioType
	{
		Input,
		Output,
	}

	[Serializable]
	public enum DioValue
	{
		Off = 0,
		On = 1,
	}

	[Serializable]
	public enum NormalState
	{
		Low = 0,
		High = 1,
	}

	[Serializable]
	public class IoPointBaseConfiguration
		: Configuration
	{
		public override string Name { get; set; } = "";
		public override Type InstrumentType => typeof( IoPointBase );
		public override InstrumentCategory Category => InstrumentCategory.DIO;
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		private int i_Address = -1;
		public int Address
		{
			get => this.i_Address;
			set => this.Set( ref this.i_Address, value, "Address" );
		}
		private int i_ModuleNo = 0;
		public int ModuleNo
		{
			get => this.i_ModuleNo;
			set => this.Set( ref this.i_ModuleNo, value, "ModuleNo" );
		}
		private DioType e_Type = DioType.Input;
		public DioType Type
		{
			get => this.e_Type;
			set => this.Set( ref this.e_Type, value, "Type" );
		}
		private NormalState e_NormalState = NormalState.Low;
		public NormalState NormalState
		{
			get => this.e_NormalState;
			set => this.Set( ref this.e_NormalState, value, "NormalState" );
		}
	}


	public abstract class IoPointBase
		: InstrumentBase
	{

		public IoPointBaseConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.DIO;
		private DioValue e_Value = DioValue.Off;
		public DioValue Value
		{
			get => this.e_Value;
			set => this.Set( ref this.e_Value, value, "Value" );
		}
		public abstract bool Check( DioValue value );
		public abstract void SetOut( DioValue value );
		public abstract void _Update( uint value );
		#region General
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as IoPointBaseConfiguration;
		}

		protected override string OnCreate()
		{
			return string.Empty;
		}

		protected override string OnInitialize()
		{
			return string.Empty;
		}

		protected override string OnStop()
		{
			return string.Empty;
		}

		protected override string OnTerminate()
		{
			return string.Empty;
		}
		#endregion
	}
}
