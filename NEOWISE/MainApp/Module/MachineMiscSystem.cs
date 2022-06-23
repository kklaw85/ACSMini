using HiPA.Common;
using HiPA.Common.Forms;
using System;
using System.Diagnostics;
using System.Timers;

namespace NeoWisePlatform.Module
{
	[Serializable]
	public class MachineMiscSystemConfiguration
		: Configuration
	{
		[NonSerialized]
		private static readonly MachineMiscSystemConfiguration _DEFAULT = new MachineMiscSystemConfiguration();
		[NonSerialized]
		public static readonly string NAME = "Machine Misc System";

		public override string Name { get; set; }
		public override InstrumentCategory Category => InstrumentCategory.Machine;
		public override Type InstrumentType => typeof( MachineMiscSystem );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public EquipmentBypass ByPassConfig { get; set; } = new EquipmentBypass();
	}

	public class MachineMiscSystem
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		#region General.Properties
		public MachineMiscSystemConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Machine;
		#endregion
		System.Timers.Timer TenSecs;
		Stopwatch SW = new Stopwatch();
		#region General.Functions
		protected override string OnCreate()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				this.TenSecs = new System.Timers.Timer( 10000 );
				this.TenSecs.Elapsed += new ElapsedEventHandler( this.OnTimedEvent );
				this.TenSecs.Start();
				this.SW.Restart();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
			return result;
		}
		protected override string OnInitialize()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			bool InitOk = true;
			try
			{

			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.InitializeFailure, ErrorClass.E6 );
			}
			return result;
		}
		protected override string OnStop()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{

			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
			return result;
		}
		protected override string OnTerminate()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, result, ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
			return result;
		}
		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as MachineMiscSystemConfiguration;
		}
		#endregion

		private void OnTimedEvent( object source, ElapsedEventArgs e )
		{
			var TimePassed = ( double )this.SW.ElapsedMilliseconds / ( 1000 * 60 * 60 );
			this.SW.Restart();
		}
		public override void ApplyRecipe( RecipeBaseUtility recipeItem )
		{
			throw new NotImplementedException();
		}
	}
}
