using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HiPA.Instrument.Motion.APS
{
	public enum AdlinkModel
	{
		PCI7856 = 0,
		AMP204208C = 1,
		PCI7230 = 6,
		PCI7432 = 16,
	}


	[Serializable]
	public class AdLinkBoardConfiguration
		: Configuration
	{
		[NonSerialized]
		private static readonly AdLinkBoardConfiguration _DEFAULT = new AdLinkBoardConfiguration();

		[NonSerialized]
		public static readonly string NAME = "AdLink";

		public override string Name { get; set; } = "";
		public override InstrumentCategory Category => InstrumentCategory.Motion;
		public override Type InstrumentType => typeof( AdLinkMotionBoard );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();

		private int? i_StartAxisId = 0;
		public int? StartAxisId
		{
			get => this.i_StartAxisId;
			set => this.Set( ref this.i_StartAxisId, value, "StartAxisId" );
		}

		private MNETTransferRate? e_MNETTransferRate = APS.MNETTransferRate.Mbps20000;
		public MNETTransferRate? MNETTransferRate
		{
			get => this.e_MNETTransferRate;
			set => this.Set( ref this.e_MNETTransferRate, value, "MNETTransferRate" );
		}

		private int i_CardID = 0;
		public int CardID
		{
			get => this.i_CardID;
			set => this.Set( ref this.i_CardID, value, "CardID" );
		}

		private string _BoardParametersFile = Path.Combine( Constructor.GetInstance().StartupPath, "DefaultMotion.xml" );
		public string BoardParametersFile
		{
			get => this._BoardParametersFile;
			set => this.Set( ref this._BoardParametersFile, value, "BoardParametersFile" );
		}

		private AdlinkModel e_AdlinkModel = AdlinkModel.AMP204208C;
		public AdlinkModel AdlinkModel
		{
			get => this.e_AdlinkModel;
			set => this.Set( ref this.e_AdlinkModel, value, "AdlinkModel" );
		}


	}

	public class AdLinkMotionBoard
		: MotionBoardBase
	{
		public override MachineVariant MachineVar { get; set; }

		#region AxisAttribute
		public class AxisAttribute : Attribute
		{
			public string Name { get; }
			public int CardID { get; }
			public AxisAttribute( string name, int cardid )
			{
				this.Name = name;
				this.CardID = cardid;
			}
		}
		#endregion


		[Serializable]
		public enum AxesList
		{
			[AxisAttribute( "PnpAxis", 0 )]
			PnpAxis,

			[AxisAttribute( "NewLiftAxis", 0 )]
			NewLiftAxis,

			[AxisAttribute( "QICLiftAxis", 0 )]
			QICLiftAxis,
		};

		public AdLinkBoardConfiguration Configuration { get; private set; }
		public ObservableCollection<int> BoardHandler { get; set; } = new ObservableCollection<int>();
		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Motion;

		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as AdLinkBoardConfiguration;
		}

		protected override string OnCreate()
		{
			var result = string.Empty;
			try
			{
				if ( ( result = this.CreateAxes() ) != string.Empty ) throw new Exception( result );

				var tasks = this.GetChildren().Select( a => a.Create() ).ToArray();
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( result = t.Result ) != string.Empty ) throw new Exception( result );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;

		}

		protected override string OnInitialize()
		{
			var result = string.Empty;
			try
			{
				if ( ( result = this.Open() ) != string.Empty )
				{
					this.Close();
					throw new Exception( result );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		protected override string OnStop()
		{
			var result = string.Empty;
			try
			{
				var tasks = this.GetChildren().Select( a => a.Stop() ).ToArray();
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( result = t.Result ) != string.Empty ) throw new Exception( result );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		protected override string OnTerminate()
		{
			var result = string.Empty;
			try
			{
				var tasks = this.GetChildren().Select( a => a.Terminate() ).ToArray();
				Task.WaitAll( tasks );

				foreach ( var t in tasks )
				{
					if ( ( result = t.Result ) != string.Empty ) throw new Exception( result );
				}
				this.Close();
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		public override string Open()
		{
			var result = string.Empty;
			int resultint;
			if ( MachineStateMng.isSimulation ) return result;
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( this._IsOpen ) return result;

				var board = PCI7856.GetInstance<PCI7856>();

				if ( ( resultint = board.Open() ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				this.BoardHandler = board.BoardHandler;
				if ( !this.BoardHandler.Contains( this.Configuration.CardID ) )
					throw new Exception( $"^{( int )RunErrors.ERR_NoBoardIDDetectErr }^" );
				Thread.Sleep( 100 );

				if ( this.Configuration.AdlinkModel == AdlinkModel.PCI7856 )
				{
					if ( ( resultint = APS168.APS_set_field_bus_param(
						this.Configuration.CardID,
						( int )BoardPortId.MNET,
						( int )APS_Define.PRF_TRANSFER_RATE,
						( int )this.Configuration.MNETTransferRate ) ) != 0 )
						throw new Exception( PCI7856.GetErrorDesc( resultint ) );

					if ( ( resultint = APS168.APS_start_field_bus(
						this.Configuration.CardID,
						( int )BoardPortId.MNET,
						this.Configuration.StartAxisId.Value ) ) != 0 )
						throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				}
				if ( ( resultint = APS168.APS_load_param_from_file( this.Configuration.BoardParametersFile ) ) != 0 )
					throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				if ( this.Configuration.AdlinkModel == AdlinkModel.AMP204208C )
				{
					if ( ( resultint = APS168.APS_save_parameter_to_flash( this.Configuration.CardID ) ) != 0 )
						throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				}

				this._IsOpen = true;
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				if ( result != String.Empty )
					this.Close();
				Monitor.Exit( this.SyncRoot );
			}
			return result;
		}

		public override string Close()
		{
			var result = string.Empty;
			int resultint;
			try
			{
				if ( !this.BoardHandler.Contains( this.Configuration.CardID ) )
					return result;

				if ( ( resultint = APS168.APS_stop_field_bus( this.Configuration.CardID, ( int )BoardPortId.MNET ) ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
				this.BoardHandler.Clear();
				this._IsOpen = false;

				var board = PCI7856.GetInstance();
				if ( ( resultint = board.Close() ) != 0 ) throw new Exception( PCI7856.GetErrorDesc( resultint ) );
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		int _isOpen = 0;
		protected bool _IsOpen
		{
			get => Interlocked.CompareExchange( ref this._isOpen, 1, 1 ) == 1;
			set => Interlocked.Exchange( ref this._isOpen, value ? 1 : 0 );
		}

		public override bool IsOpen()
		{
			return this._IsOpen;
		}

		protected override string CreateAxes()
		{
			var result = string.Empty;
			try
			{
				var list = ReflectionTool.GetEnumAttributes<AxisAttribute>( typeof( AxesList ) );
				foreach ( var axispoint in list )
				{
					var axis = Constructor.GetInstance().GetInstrument( axispoint.Attribute.Name, typeof( AdLinkAxisConfiguration ) ) as AdLinkAxis;
					if ( axis == null ) throw new Exception( $"{axispoint.Attribute.Name} is null" );
					if ( ( int )( axispoint.Attribute.CardID ) != this.Configuration.CardID ) continue;
					axis.BoardBase = this;
					this.AddChild( axis );
				}
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}
		#region Children Operation 
		public override void OnAddChild( string name )
		{
			try
			{
				base.OnAddChild( name );
				if ( this.Configuration.Children is null )
					this.Configuration.Children = new List<string>();

				if ( this.Configuration.Children.Contains( name ) == false )
					this.Configuration.Children.Add( name );

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this, $"On Add Child failure. Error : {ex.Message}", ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
		}
		public override void OnRemoveChild( string name )
		{
			try
			{
				if ( this.Configuration.Children is null )
					return;

				this.Configuration.Children.Remove( name );
				base.OnRemoveChild( name );

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( this, $"On Remove Child failure. Error : {ex.Message}", ErrorTitle.OperationFailure, ErrorClass.E6 );
			}
		}

		public override void ApplyRecipe( RecipeBaseUtility recipeItem )
		{
			throw new NotImplementedException();
		}
		#endregion
	}


}
