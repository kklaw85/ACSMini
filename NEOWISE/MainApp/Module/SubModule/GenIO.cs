using HiPA.Common;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using System;

namespace NeoWisePlatform.Module
{
	[Serializable]
	public class GenIOModuleConfiguration
		: Configuration
	{
		[NonSerialized]
		public static readonly GenIOModuleConfiguration _DEFAULT = new GenIOModuleConfiguration();
		[NonSerialized]
		public static readonly string NAME = "GenIO";

		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.DOutput;
		public override Type InstrumentType => typeof( GenIOModule );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public OutputIO Output { get; set; }
	}

	public class GenIOModule
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		public GenIOModuleConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.DOutput;

		private MTEquipment Equipment = null;
		private AdLinkIoPoint Output = null;

		protected override string OnCreate()
		{
			return String.Empty;
		}

		protected override string OnInitialize()
		{
			var sErr = string.Empty;
			try
			{
				if ( ( sErr = this.Init() ) != string.Empty ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}

		protected override string OnStop()
		{
			var sErr = string.Empty;
			try
			{
				if ( ( sErr = this.Off() ) != string.Empty ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}

		protected override string OnTerminate()
		{
			return String.Empty;
		}

		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as GenIOModuleConfiguration;
		}
		private bool State
		{
			get => this.Output.Check( DioValue.On ) ? true : false;
			set
			{
				this.Output?.SetOut( value ? DioValue.On : DioValue.Off );
				this.Status = value;
			}
		}
		private bool bStatus = false;
		public bool Status
		{
			get => this.bStatus;
			protected set => this.Set( ref this.bStatus, value, "Status" );
		}
		public string On()
		{
			var sErr = string.Empty;
			try
			{
				//if ( this.ValidVariant() )
				this.State = true;
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}
		public string Off()
		{
			var sErr = string.Empty;
			try
			{
				//if ( this.ValidVariant() )
				this.State = false;
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}
		public string Toggle()
		{
			var sErr = string.Empty;
			try
			{
				//if ( this.ValidVariant() )
				this.State = !this.State;
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}
		private string Init()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				this.Equipment = Constructor.GetInstance()?.Equipment as MTEquipment;
				if ( ( sErr = this.UpdateDOAssignment() ) != string.Empty ) throw new Exception( sErr );
				if ( this.Output == null ) return "";
				this.Off();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		public string UpdateDOAssignment()
		{
			var sErr = string.Empty;
			try
			{
				this.Output = this.Equipment?.GetIOPointByEnum( this.Configuration.Output );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}
	}
}
