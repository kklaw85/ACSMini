using HiPA.Common;
using System;
using System.Xml.Serialization;

namespace HiPA.Instrument.Motion.APS
{
	[Serializable]
	public class AdLinkIoPointConfiguration
		: Configuration
	{
		public override string Name { get; set; } = "";
		public override Type InstrumentType => typeof( AdLinkIoPoint );
		public override InstrumentCategory Category => InstrumentCategory.DIO;
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		private int i_Line = -1;
		public int Line
		{
			get => this.i_Line;
			set => this.Set( ref this.i_Line, value, "Line" );
		}
		private int i_ModuleNo = -1;
		public int ModuleNo
		{
			get => this.i_ModuleNo;
			set => this.Set( ref this.i_ModuleNo, value, "ModuleNo" );
		}
		private int i_CardID = -1;
		public int CardID
		{
			get => this.i_CardID;
			set => this.Set( ref this.i_CardID, value, "CardID" );
		}
		private DioType? e_Type = null;
		public DioType? Type
		{
			get => this.e_Type;
			set => this.Set( ref this.e_Type, value, "Type" );
		}

		private bool b_NormalHigh = false;
		public bool NormalHigh
		{
			get => this.b_NormalHigh;
			set => this.Set( ref this.b_NormalHigh, value, "NormalHigh" );
		}
	}


	public class AdLinkIoPoint
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		DateTime Start = DateTime.Now;
		[XmlIgnore]
		public AdLinkIoPointConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		[XmlIgnore]
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}

		public IoBoardBase Board { get; set; }
		[XmlIgnore]
		public override InstrumentCategory Category => InstrumentCategory.Motion;


		uint _value = 0;
		private DioValue e_Value = DioValue.Off;
		[XmlIgnore]
		public DioValue Value
		{
			get => this.e_Value;
			private set => this.Set( ref this.e_Value, value, "Value" );
		}
		public bool Check( DioValue value )
		{
			if ( this.Configuration.Type == DioType.Input )
				this._Update( this.Board.GetInputs( this.Configuration.ModuleNo ) );
			else
				this._Update( this.Board.GetOutputs( this.Configuration.ModuleNo ) );

			return this.Value == value;
		}
		public void SetOut( DioValue value )
		{
			try
			{
				if ( !this.Board.IsValid || !this.Board.IsOpen() ) return;
				if ( this.Configuration.Type != DioType.Output ) return;
				this.Board.SetOutput( this.Configuration.ModuleNo, this.Configuration.Line, value );
				this._Update( ( uint )value << this.Configuration.Line );
			}
			catch
			{

			}
		}
		public void ToggleOut()
		{
			try
			{
				if ( !this.Board.IsValid ) return;
				if ( this.Configuration.Type != DioType.Output ) return;
				var value = this.Value == DioValue.On ? DioValue.Off : DioValue.On;
				this.Board.SetOutput( this.Configuration.ModuleNo, this.Configuration.Line, value );
				this._Update( ( uint )value << this.Configuration.Line );
			}
			catch
			{

			}
		}
		public void _Update( uint value )
		{
			try
			{
				var previous = this._value;
				this._value = ( uint )( value & ( 1 << this.Configuration.Line ) );
				if ( this.Configuration.Type == DioType.Input )
					this.Value = ( this._value > 0 && !this.Configuration.NormalHigh ) ||
						( this._value == 0 && this.Configuration.NormalHigh )
						? DioValue.On : DioValue.Off;
				else
					this.Value = this._value == 0 ? DioValue.Off : DioValue.On;
				if ( previous != this._value )
				{
					this.Start = DateTime.Now;
					this.IoValueChangedEvent?.Invoke( this, this.Value );
					if ( this.Value == DioValue.Off )
						this.IoValueChangedOn2OffEvent?.Invoke( this, this.Value );
					else
						this.IoValueChangedOff2OnEvent?.Invoke( this, this.Value );
				}
			}
			catch
			{

			}
		}

		public event EventHandler<DioValue> IoValueChangedEvent;
		public event EventHandler<DioValue> IoValueChangedOff2OnEvent;
		public event EventHandler<DioValue> IoValueChangedOn2OffEvent;
		#region General
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as AdLinkIoPointConfiguration;
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
