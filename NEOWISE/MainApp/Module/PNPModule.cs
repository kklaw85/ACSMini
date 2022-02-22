using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Utils;
using HiPA.Instrument.Motion;
using HiPA.Instrument.Motion.APS;
using NeoWisePlatform.Recipe;
using System;
using System.Threading.Tasks;
using static HiPA.Instrument.Motion.APS.AdLinkMotionBoard;

namespace NeoWisePlatform.Module
{
	[Serializable]
	public class PNPModuleConfiguration
		: Configuration
	{
		[NonSerialized]
		private static readonly PNPModuleConfiguration _DEFAULT = new PNPModuleConfiguration();
		[NonSerialized]
		public static readonly string NAME = "PNPHead Module";

		public override string Name { get; set; }
		public override InstrumentCategory Category => InstrumentCategory.Module;
		public override Type InstrumentType => typeof( PNPModule );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public UninspResult UnInspResult
		{
			get => this.GetValue( () => this.UnInspResult );
			set => this.SetValue( () => this.UnInspResult, value );
		}
	}

	public class PNPModule
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		#region General.Properties
		public PNPModuleConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Module;
		#endregion
		#region Sequence
		public StationSequences Seq { get; set; }
		#endregion
		#region Parts 
		public AxisBase AxisX { get; private set; } = null;
		public IOMotion LoadArm { get; private set; } = null;
		public IOMotion UnLoadArm { get; private set; } = null;
		#endregion
		#region General.Functions
		protected override string OnCreate()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				var axis = ReflectionTool.GetEnumAttribute<AxisAttribute>( AxesList.PnpAxis );
				this.AxisX = Constructor.GetInstance().GetInstrument( axis.Attribute.Name, null ) as AdLinkAxis;
				if ( this.AxisX == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to get instance:,{axis.Attribute.Name}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.AxisX.Owner = this;

				this.LoadArm = Constructor.GetInstance().GetInstrument( IOMotionList.LoadArm.ToString(), typeof( IOMotionConfiguration ) ) as IOMotion;
				if ( this.LoadArm == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, IOMotionList.LoadArm.ToString() ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.LoadArm.Owner = this;

				this.UnLoadArm = Constructor.GetInstance().GetInstrument( IOMotionList.UnLoadArm.ToString(), typeof( IOMotionConfiguration ) ) as IOMotion;
				if ( this.UnLoadArm == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, IOMotionList.LoadArm.ToString() ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.UnLoadArm.Owner = this;

				var tasks = new Task<string>[]
				{
					this.AxisX.Create(),
					this.LoadArm.Create(),
					this.UnLoadArm.Create(),
				};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to create Obj:,{sErr}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				}
				if ( ( result = this.InitIO() ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				if ( ( result = this.InitInterlock() ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				this.Seq = new StationSequences();
			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
			return result;
		}

		private string InitIO()
		{
			var sErr = string.Empty;
			try
			{
				var Equipment = Constructor.GetInstance().Equipment as MTEquipment;
				this.LoadArm._ResetPos = Equipment.GetIOPointByEnum( InputIO.I_LoadArmUp );
				this.LoadArm._InPos = Equipment.GetIOPointByEnum( InputIO.I_LoadArmDown );
				this.LoadArm._VacuumSuctionSense = Equipment.GetIOPointByEnum( InputIO.I_LoadArmVacDet );
				this.LoadArm._ToResetCylinder = Equipment.GetIOPointByEnum( OutputIO.O_LoadArmUp );
				this.LoadArm._ToPosCylinder = Equipment.GetIOPointByEnum( OutputIO.O_LoadArmDown );
				this.LoadArm._VacuumSuction = Equipment.GetIOPointByEnum( OutputIO.O_LoadArmVac );

				this.UnLoadArm._ResetPos = Equipment.GetIOPointByEnum( InputIO.I_UnloadArmUp );
				this.UnLoadArm._InPos = Equipment.GetIOPointByEnum( InputIO.I_UnloadArmDown );
				this.UnLoadArm._VacuumSuctionSense = Equipment.GetIOPointByEnum( InputIO.I_UnloadArmVacDet );
				this.UnLoadArm._ToResetCylinder = Equipment.GetIOPointByEnum( OutputIO.O_UnLoadArmUp );
				this.UnLoadArm._ToPosCylinder = Equipment.GetIOPointByEnum( OutputIO.O_UnLoadArmDown );
				this.UnLoadArm._VacuumSuction = Equipment.GetIOPointByEnum( OutputIO.O_UnLoadArmVac );

			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}
		private string InitInterlock()
		{
			var sErr = string.Empty;
			try
			{
				this.AxisX.Interlocks.Subscribe( this.LoadArm );
				this.AxisX.Interlocks.Subscribe( this.UnLoadArm );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}


		protected override string OnInitialize()
		{
			var sErr = string.Empty;
			bool InitOk = true;
			try
			{
				var tasks = new Task<string>[]
				{
					this.AxisX.Initialize(),
					this.LoadArm.Initialize(),
					this.UnLoadArm.Initialize(),
				};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
					{
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.InitializeFailure, ErrorClass.E5 );
						InitOk = false;
					}
				}

				if ( !InitOk )
					throw new Exception( "Initialization Failed" );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
				Equipment.ErrManager.RaiseError( this, sErr, ErrorTitle.InitializeFailure, ErrorClass.E6 );
			}
			return sErr;
		}
		protected override string OnStop()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				var tasks = new Task<string>[]
				{
					this.AxisX.Stop(),
					this.LoadArm.Stop(),
					this.UnLoadArm.Stop(),
				};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
				}
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
				var tasks = new Task<string>[]
				{
					this.AxisX.Terminate(),
					this.LoadArm.Terminate(),
					this.UnLoadArm.Terminate(),
				};

				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E5 );
				}
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
			this.Configuration = configuration as PNPModuleConfiguration;
		}
		#endregion
		#region Motion
		#region internal
		#region Absolute move
		public Task<ErrorResult> PNPMoveAbsolute( double Pos )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var trajectory = new Trajectory( this.AxisX.Configuration.CommandedMove )
					{
						Position = Pos,
					};
					this.CheckAndThrowIfError( ErrorClass.E5, this.AxisX.AbsoluteMove( trajectory ).Result );
					this.CheckAndThrowIfError( ErrorClass.E5, this.CheckXPosition( Pos ) );
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return this.Result;
			} );
		}

		#endregion
		#region Arms

		#endregion
		#region public move
		public Task<ErrorResult> PNPToPickPos()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
					if ( recipe == null ) this.ThrowError( ErrorClass.E4, "Recipe not loaded properly" );
					this.CheckAndThrowIfError( this.PNPMoveAbsolute( recipe.PickPos ).Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> PNPToWaitPos()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
					if ( recipe == null ) this.ThrowError( ErrorClass.E4, "Recipe not loaded properly" );
					this.CheckAndThrowIfError( this.PNPMoveAbsolute( recipe.WaitPos ).Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> PNPToLoadPos()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
					this.CheckAndThrowIfError( this.PNPMoveAbsolute( recipe.LoadPos ).Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> PNPToPlaceKIV()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
					this.CheckAndThrowIfError( this.PNPMoveAbsolute( recipe.PlaceKIVPos ).Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> PNPToPlaceNG()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
					this.CheckAndThrowIfError( this.PNPMoveAbsolute( recipe.PlaceNGPos ).Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> MovePNPByResult()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( this.AutorunInfo.InspectionRes.InspResult == eInspResult.KIV ) this.CheckAndThrowIfError( this.PNPToPlaceKIV().Result );
					else if ( this.AutorunInfo.InspectionRes.InspResult == eInspResult.NG ) this.CheckAndThrowIfError( this.PNPToPlaceNG().Result );
					else if ( this.AutorunInfo.InspectionRes.InspResult == eInspResult.QIC ) this.CheckAndThrowIfError( this.PNPToLoadPos().Result );
					else
					{
						if ( this.Configuration.UnInspResult == UninspResult.KIV ) this.CheckAndThrowIfError( this.PNPToPlaceKIV().Result );
						else if ( this.Configuration.UnInspResult == UninspResult.NG ) this.CheckAndThrowIfError( this.PNPToPlaceNG().Result );
					}
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		#endregion
		#region Home Axes
		public Task<ErrorResult> HomeX()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.CheckAndThrowIfError( ErrorClass.E5, this.AxisX.Homing().Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> Home()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.CheckAndThrowIfError( ErrorClass.E5, this.LoadArm.ThrowEXIfMatDet() );
					this.CheckAndThrowIfError( ErrorClass.E5, this.UnLoadArm.ThrowEXIfMatDet() );
					var Tasks = new Task<ErrorResult>[]
					{
						this.LoadArm.Reset(),
						this.LoadArm.SuctionOff(),
						this.UnLoadArm.Reset(),
						this.UnLoadArm.SuctionOff(),
					};
					this.CheckAndThrowIfError( Tasks );
					this.CheckAndThrowIfError( this.HomeX().Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		#endregion
		#region CheckPosition
		public string CheckXPosition( double position, double tolerance = 0.02 )
		{
			try
			{
				return this.AxisX.CheckPosition( position, tolerance );
			}
			catch ( Exception ex )
			{
				return Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, ex ), ErrorTitle.OperationFailure, ErrorClass.E5 );
			}
		}
		#endregion
		#region Getposition
		private bool PositionOK( double Pos )
		{
			var Res = false;
			try
			{
				Res = this.AxisX.CheckPosition( Pos ) == string.Empty;
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			return Res;
		}
		public bool PNPInLoadPos()
		{
			var Res = false;
			try
			{
				var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
				Res = this.PositionOK( recipe.LoadPos );
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			return Res;
		}
		public bool PNPInPickPos()
		{
			var Res = false;
			try
			{
				var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
				Res = this.PositionOK( recipe.PickPos );
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			return Res;
		}
		public bool PNPInKIVPos()
		{
			var Res = false;
			try
			{
				var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
				Res = this.PositionOK( recipe.PlaceKIVPos );
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			return Res;
		}
		public bool PNPInNGPos()
		{
			var Res = false;
			try
			{
				var recipe = Recipes.HandlerRecipes()?.GetAppliedRecipe()?.PNPWorkPos;
				Res = this.PositionOK( recipe.PlaceNGPos );
			}
			catch ( Exception ex )
			{
				this.CatchAndPromptErr( ex );
			}
			return Res;
		}
		#endregion
		#endregion
		#endregion
		#region StartAutoRun initialization
		public Task<ErrorResult> MoveToStandbyStatus()
		{
			return Task.Run( () =>
			 {
				 this.ClearErrorFlags();
				 try
				 {
					 if ( MachineStateMng.isSimulation ) return this.Result;
					 this.CheckAndThrowIfError( this.Home().Result );
					 this.CheckAndThrowIfError( this.PNPToWaitPos().Result );
					 this.LoadArm.ClearLiftModuleLink();
					 this.UnLoadArm.ClearLiftModuleLink();
				 }
				 catch ( Exception ex )
				 {
					 this.CatchAndPromptErr( ex );
				 }
				 return this.Result;
			 } );
		}

		public Task<ErrorResult> IsAllowToAutorun()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					if ( MachineStateMng.isSimulation ) return this.Result;
					if ( this.LoadArm.Suction == true && this.LoadArm.SuctionSense ) this.CheckAndThrowIfError( ErrorClass.E5, "Substrate detected on Load Arm. Please remove substrate and restart autorun." );
					else if ( this.LoadArm.Suction == false && this.LoadArm.SuctionSense ) this.CheckAndThrowIfError( ErrorClass.E5, "Malfunction on vacuum sensor in Load Arm. Please check vacuum sensor." );
					else if ( this.UnLoadArm.Suction == true && this.UnLoadArm.SuctionSense ) this.CheckAndThrowIfError( ErrorClass.E5, "Substrate detected on Unload Arm. Please remove substrate and restart autorun." );
					else if ( this.UnLoadArm.Suction == false && this.UnLoadArm.SuctionSense ) this.CheckAndThrowIfError( ErrorClass.E5, "Malfunction on vacuum sensor in Unload Arm. Please check vacuum sensor." );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}

		#endregion
		#region Autorun flags
		public PNPAutorunInfo AutorunInfo { get; private set; } = new PNPAutorunInfo();
		#endregion
	}

	#region JogAxis Class
	public class JogAxis : BaseUtility
	{
		private double d_AbsPosX = 0.001;
		public double AbsPosX
		{
			get => this.d_AbsPosX;
			set => this.Set( ref this.d_AbsPosX, value, "AbsPosX" );
		}

		private double d_AbsPosY = 0.001;
		public double AbsPosY
		{
			get => this.d_AbsPosY;
			set => this.Set( ref this.d_AbsPosY, value, "AbsPosY" );
		}

		private double d_AbsPosZ = 0.001;
		public double AbsPosZ
		{
			get => this.d_AbsPosZ;
			set => this.Set( ref this.d_AbsPosZ, value, "AbsPosZ" );
		}

		private double d_IntPosX = 0.001;
		public double IntrPosX
		{
			get => this.d_IntPosX;
			set => this.Set( ref this.d_IntPosX, value, "IntrPosX" );
		}

		private double d_IntPosY = 0.001;
		public double IntrPosY
		{
			get => this.d_IntPosY;
			set => this.Set( ref this.d_IntPosY, value, "IntrPosY" );
		}

		private double d_IntPosZ = 0.001;
		public double IntrPosZ
		{
			get => this.d_IntPosZ;
			set => this.Set( ref this.d_IntPosZ, value, "IntrPosZ" );
		}
	}

	#endregion

	public class PNPAutorunInfo : BaseUtility
	{
		public bool UnloadStage
		{
			get => this.GetValue( () => this.UnloadStage );
			set => this.SetValue( () => this.UnloadStage, value );
		}
		public InspectionRes InspectionRes { get; set; } = new InspectionRes();
		public void Clear()
		{
			this.UnloadStage = false;
			this.InspectionRes.Clear();
		}
	}
	public enum UninspResult
	{
		KIV,
		NG
	}
}
