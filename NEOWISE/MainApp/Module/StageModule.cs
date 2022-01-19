using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Instrument.Camera;
using HiPA.Instrument.Motion.APS;
using NeoWisePlatform.Sequence;
using System;
using System.Threading.Tasks;

namespace NeoWisePlatform.Module
{
	[Serializable]
	public class StageModuleConfiguration
		: Configuration
	{
		[NonSerialized]
		private static readonly StageModuleConfiguration _DEFAULT = new StageModuleConfiguration();
		[NonSerialized]
		public static readonly string NAME = "Stage Module";

		public override string Name { get; set; }
		public override InstrumentCategory Category => InstrumentCategory.Module;
		public override Type InstrumentType => typeof( StageModule );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		public InspectionCriteria InspectionCriteria { get; set; } = new InspectionCriteria();
	}
	public class StageModule
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; }
		#region General.Properties
		public StageModuleConfiguration Configuration { get; private set; }
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Module;
		#endregion
		#region Parts 
		public MatroxCamera Fov1 { get; private set; } = null;
		public MatroxCamera Fov2 { get; private set; } = null;
		public StageClamper Stage { get; private set; } = null;
		public static bool ToSkipPwrVfyChk { get; set; } = false;
		#endregion
		#region Sequence
		public StageSeq Seq { get; set; }
		#endregion
		#region General.Functions
		protected override string OnCreate()
		{
			var result = string.Empty;
			var sErr = string.Empty;
			try
			{
				this.AutorunInfo.InspectionRes.InspectionCriteria = this.Configuration.InspectionCriteria;
				this.Fov1 = Constructor.GetInstance().GetInstrument( CameraList.FOV1.ToString(), typeof( MatroxCameraConfiguration ) ) as MatroxCamera;
				if ( this.Fov1 == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{CameraList.FOV1.ToString()}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.Fov1.Owner = this;

				this.Fov2 = Constructor.GetInstance().GetInstrument( CameraList.FOV2.ToString(), typeof( MatroxCameraConfiguration ) ) as MatroxCamera;
				if ( this.Fov2 == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"{CameraList.FOV2.ToString()}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.Fov2.Owner = this;

				this.Stage = Constructor.GetInstance().GetInstrument( typeof( StageClamperConfiguration ) ) as StageClamper;
				if ( this.Stage == null ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, StageClamperConfiguration.NAME ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				else this.Stage.Owner = this;

				var tasks = new Task<string>[]
				{
					this.Fov1.Create(),
					this.Fov2.Create(),
					this.Stage.Create(),
				};
				Task.WaitAll( tasks );
				foreach ( var t in tasks )
				{
					if ( ( sErr = t.Result ) != string.Empty )
						Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, $"Failed to create Obj:,{sErr}" ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				}
				if ( ( result = this.InitIO() ) != string.Empty ) Equipment.ErrManager.RaiseError( this, this.FormatErrMsg( this.Name, sErr ), ErrorTitle.OperationFailure, ErrorClass.E4 );
				this.Seq = new StageSeq( this );
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
			var sErr = string.Empty;
			bool InitOk = true;
			try
			{
				var tasks = new Task<string>[]
				{
					this.Fov1.Initialize(),
					this.Fov2.Initialize(),
					this.Stage.Initialize(),
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
					this.Fov1.Stop(),
					this.Fov2.Stop(),
					this.Stage.Stop(),
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
					this.Fov1.Terminate(),
					this.Fov2.Terminate(),
					this.Stage.Terminate(),
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
			this.Configuration = configuration as StageModuleConfiguration;
		}
		#endregion
		#region General used.
		private string InitIO()
		{
			var sErr = string.Empty;
			try
			{
				var Equipment = Constructor.GetInstance().Equipment as MTEquipment;
				this.Stage._InPos = Equipment.GetIOPointByEnum( InputIO.I_ClamperClose );
				this.Stage._ResetPos = Equipment.GetIOPointByEnum( InputIO.I_ClamperOpen );
				this.Stage._ToPosCylinder = Equipment.GetIOPointByEnum( OutputIO.O_Clamp );
				this.Stage._MatSense = Equipment.GetIOPointByEnum( InputIO.I_StageMatDet );
				this.Stage._VacuumSuction = Equipment.GetIOPointByEnum( OutputIO.O_StageVac );
				this.Stage._VacuumSuctionSense = Equipment.GetIOPointByEnum( InputIO.I_StageVacDet );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			return sErr;
		}

		#endregion


		#region Motion
		#region Clamper movement
		#endregion
		#region substrate hold/release
		#endregion




		#region camera and vision
		public Task<ErrorResult> SnapShot()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var Tasks = new Task<string>[]
					{
						this.Fov1.SingleGrab(),
						this.Fov2.SingleGrab(),
					};
					this.CheckAndThrowIfError( ErrorClass.E5, Tasks );
					this.AutorunInfo.InspectionRes.VisionOp = eVisionOperation.ImageTaken;
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		public Task<ErrorResult> VisionCheck()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					var tasks = new Task<ErrorResult>[]
					{
						this.Fov1.InspectPosition(),
						this.Fov2.InspectPosition(),
					};
					this.CheckAndThrowIfError( tasks );
					this.AutorunInfo.InspectionRes.VisionOp = eVisionOperation.SingleVisionCalculated;
					this.AutorunInfo.InspectionRes.CalculateResult( this.Fov1.InspectionResult, this.Fov2.InspectionResult );
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

		public Task<ErrorResult> Home()
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				try
				{
					this.CheckAndThrowIfError( this.Stage.Release().Result );
				}
				catch ( Exception ex )
				{
					this.CatchAndPromptErr( ex );
				}
				return this.Result;
			} );
		}
		#endregion

		#endregion
		#region Laser Current Jog Settings
		public JogAxis StepCurrSettings { get; set; } = new JogAxis();
		public JogAxis VelCurrSettings { get; set; } = new JogAxis();
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
					 var Tasks = new Task<ErrorResult>[]
					 {
						this.Home(),
					 };
					 this.CheckAndThrowIfError( Tasks );
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
		public StageAutorunInfo AutorunInfo { get; set; } = new StageAutorunInfo();
		#endregion
	}
	public class StageAutorunInfo : BaseUtility
	{
		public InspectionRes InspectionRes { get; set; } = new InspectionRes();
		public PNPToStageFlag StageFlag
		{
			get => this.GetValue( () => this.StageFlag );
			set => this.SetValue( () => this.StageFlag, value );
		}
		public void Clear()
		{
			this.InspectionRes.Clear();
			this.StageFlag = PNPToStageFlag.Stop;
		}
	}
	public class InspectionRes : BaseUtility
	{
		public eVisionOperation VisionOp
		{
			get => this.GetValue( () => this.VisionOp );
			set => this.SetValue( () => this.VisionOp, value );
		}
		public InspectionResult Fov1 { get; private set; } = new InspectionResult();
		public InspectionResult Fov2 { get; private set; } = new InspectionResult();
		public InspectionCriteria InspectionCriteria { get; set; } = new InspectionCriteria();
		public eInspResult Result
		{
			get => this.GetValue( () => this.Result );
			set => this.SetValue( () => this.Result, value );
		}
		public void Clear()
		{
			this.Fov1.Clear();
			this.Fov2.Clear();
			this.Result = eInspResult.Uninspected;
			this.VisionOp = eVisionOperation.Init;
		}
		public void CalculateResult( InspectionResult Fov1, InspectionResult Fov2 )
		{
			try
			{
				this.Fov1.Copy( Fov1 );
				this.Fov2.Copy( Fov2 );
				if ( this.InspectionCriteria.Criteria == CompCriteria.BothFOVsWithinSpec )
				{
					if ( !this.Fov1.PositionOffsetMM.InRange( -this.InspectionCriteria.NGThres, this.InspectionCriteria.NGThres ) ||
						!this.Fov2.PositionOffsetMM.InRange( -this.InspectionCriteria.NGThres, this.InspectionCriteria.NGThres ) )
						this.Result = eInspResult.NG;
					else if ( !this.Fov1.PositionOffsetMM.InRange( -this.InspectionCriteria.KIVThres, this.InspectionCriteria.KIVThres ) ||
						!this.Fov2.PositionOffsetMM.InRange( -this.InspectionCriteria.KIVThres, this.InspectionCriteria.KIVThres ) )
						this.Result = eInspResult.KIV;
					else
						this.Result = eInspResult.QIC;
				}
				else if ( this.InspectionCriteria.Criteria == CompCriteria.RelativeOffsetBetween2FOVs )
				{
					if ( ( this.Fov1.PositionOffsetMM.X - this.Fov2.PositionOffsetMM.X ).InRange( -this.InspectionCriteria.NGThres, this.InspectionCriteria.NGThres ) ||
						( this.Fov1.PositionOffsetMM.Y - this.Fov2.PositionOffsetMM.Y ).InRange( -this.InspectionCriteria.NGThres, this.InspectionCriteria.NGThres ) )
						this.Result = eInspResult.NG;
					else if ( ( this.Fov1.PositionOffsetMM.X - this.Fov2.PositionOffsetMM.X ).InRange( -this.InspectionCriteria.KIVThres, this.InspectionCriteria.KIVThres ) ||
						( this.Fov1.PositionOffsetMM.Y - this.Fov2.PositionOffsetMM.Y ).InRange( -this.InspectionCriteria.KIVThres, this.InspectionCriteria.KIVThres ) )
						this.Result = eInspResult.KIV;
					else
						this.Result = eInspResult.QIC;
				}
				this.VisionOp = eVisionOperation.ResultCalculated;
			}
			catch ( Exception ex )
			{
				this.Result = eInspResult.CalcError;
			}
		}
		public void Copy( InspectionRes Source )
		{
			this.Fov1.Copy( Source.Fov1 );
			this.Fov2.Copy( Source.Fov2 );
			this.InspectionCriteria.Copy( Source.InspectionCriteria );
			this.Result = Source.Result;
		}
	}

	public enum CompCriteria
	{
		BothFOVsWithinSpec,
		RelativeOffsetBetween2FOVs,
	}
	public enum eInspResult
	{
		Uninspected,
		CalcError,
		NG,
		KIV,
		QIC,
	}
	public enum eCamera
	{
		Uninspected,
		CalcError,
		NG,
		KIV,
		QIC,
	}
	public enum PNPToStageFlag
	{
		Stop,
		Cleared,
		CanClamp,
		CanInspect,
	}
	public class InspectionCriteria : RecipeBaseUtility
	{
		private CompCriteria eCriteria = CompCriteria.BothFOVsWithinSpec;
		public CompCriteria Criteria
		{
			get => this.eCriteria;
			set => this.Set( ref this.eCriteria, value );
		}
		private double d_NGThres = 0.01;
		public double NGThres
		{
			get => this.d_NGThres;
			set => this.Set( ref this.d_NGThres, value );
		}
		private double d_KIVThres = 0.005;
		public double KIVThres
		{
			get => this.d_KIVThres;
			set => this.Set( ref this.d_KIVThres, value );
		}
		public void Copy( InspectionCriteria Source )
		{
			this.Criteria = Source.Criteria;
			this.NGThres = Source.NGThres;
			this.KIVThres = Source.KIVThres;
		}
	}
	public enum eVisionOperation
	{
		Init,
		ImageTaken,
		SingleVisionCalculated,
		ResultCalculated,
	}
}
