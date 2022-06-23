using B262_Vision_Processing;
using HiPA.Common;
using HiPA.Common.Forms;
using JptMatroxSystem;
using Matrox.MatroxImagingLibrary;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace HiPA.Instrument.Camera
{

	[Serializable]
	public class MatroxProcessingConfiguration
		: Configuration
	{
		[NonSerialized]
		private static readonly MatroxProcessingConfiguration _DEFAULT = new MatroxProcessingConfiguration();

		[NonSerialized]
		public static readonly string NAME = "MatroxProcessing";

		private string s_Name = string.Empty;
		public override string Name
		{
			get => this.s_Name;
			set
			{
				this.s_Name = value;
				this.OnPropertyChanged( "Name" );
			}
		}
		public override InstrumentCategory Category => InstrumentCategory.Camera;
		public override Type InstrumentType => typeof( MatroxProcessing );
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
	}

	public class MatroxProcessing
		: InstrumentBase
	{
		public override MachineVariant MachineVar { get; set; } = new MachineVar();
		#region Instrument Members 
		public MatroxProcessingConfiguration Configuration { get; protected set; }

		public override string Name => this.Configuration.Name;
		public override string Location
		{
			get => this.Configuration.Location;
			protected set => this.Configuration.Location = value;
		}
		public override InstrumentCategory Category => InstrumentCategory.Camera;

		public override void ApplyConfiguration( Configuration configuration )
		{
			this.Configuration = configuration as MatroxProcessingConfiguration;
		}
		#endregion
		private static MatroxSystem _appID = MatroxSystem.Instance;
		private static MIL_ID m_system = _appID.HostSystemID;
		#region Camera Initialize/Terminate 
		protected override string OnCreate()
		{
			var result = string.Empty;
			try
			{
				var temp = MIL.M_NULL;
			}
			catch ( Exception ex )
			{
				result = this.FormatErrMsg( this.Name, ex );
			}
			return result;
		}

		protected override string OnInitialize()
		{
			var Result = string.Empty;
			try
			{
				Monitor.Enter( this.SyncRoot );
			}
			catch ( Exception ex )
			{
				Result = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}

			return Result;
		}
		protected override string OnTerminate()
		{
			return "";
		}
		protected override string OnStop()
		{
			return "";
		}
		#endregion
		#region processing
		public Task<(ErrorResult EResult, C_PointD RawPos, C_PointD RawOffsetCenter, C_PointD OffPos)> CheckModelPosition( byte[] mmf, CameraObj MilImg, ROIRectangle ROI )
		{
			return Task.Run( () =>
			{
				this.ClearErrorFlags();
				var OffPos = new C_PointD();
				var RawPos = new C_PointD();
				var RawOffsetCenter = new C_PointD();
				try
				{
					this.CheckAndThrowIfError( B262_Process.FindModel( mmf, MilImg, ROI, out RawPos, out RawOffsetCenter, out OffPos ) );
				}
				catch ( Exception ex )
				{
					this.CatchException( ex );
				}
				return (this.Result, RawPos, RawOffsetCenter, OffPos);
			} );
		}
		#endregion
		#region Load/Save file
		#region Load mmf to byte arr
		private string LoadMMF( string filename, ref MIL_ID m_modcontext )
		{
			string sErr = string.Empty;
			MIL_INT sizeRequired = 0;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );
				MIL.MmodRestore( filename, m_system, MIL.M_DEFAULT, ref m_modcontext );
			}
			catch ( Exception ex )
			{
				sErr = "Loading of MMF failed" + ex.Message;
			}
			finally
			{
			}
			return sErr;
		}
		public string MMFToByteArr( MIL_ID m_modcontext, out byte[] mmf_Arr )
		{
			string sErr = string.Empty;
			MIL_INT sizeRequired = 0;
			mmf_Arr = null;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );
				MIL.MmodStream( MIL.M_NULL, MIL.M_NULL, MIL.M_INQUIRE_SIZE_BYTE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_modcontext, ref sizeRequired );
				mmf_Arr = new byte[ sizeRequired ];
				MIL.MmodStream( mmf_Arr, m_system, MIL.M_SAVE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_modcontext, MIL.M_NULL );
			}
			catch ( Exception ex )
			{
				sErr = "Conversion from MMF to byte Array Failed" + ex.Message;
			}
			finally
			{
				if ( m_modcontext != MIL.M_NULL ) MIL.MmodFree( m_modcontext );
			}
			return sErr;
		}
		public string LoadMMFPathToByteArr( string filename, out byte[] mmf_Arr )
		{
			string sErr = string.Empty;
			MIL_ID mmfcontext = 0;
			mmf_Arr = null;
			try
			{
				this.CheckAndThrowIfError( ErrorClass.E5, this.LoadMMF( filename, ref mmfcontext ) );
				this.CheckAndThrowIfError( ErrorClass.E5, this.MMFToByteArr( mmfcontext, out mmf_Arr ) );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
			}
			return sErr;
		}
		#endregion
		#region Export mmf from Arr
		private string MMFLoadByteArr( byte[] mmf_Arr, ref MIL_ID m_modcontext )
		{
			string sErr = string.Empty;
			MIL_INT sizeRequired = 0;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );
				MIL.MmodStream( mmf_Arr, m_system, MIL.M_RESTORE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_modcontext, MIL.M_NULL );
			}
			catch ( Exception ex )
			{
				sErr = "Conversion from MMF to byte Array Failed" + ex.Message;
			}
			finally
			{
			}
			return sErr;
		}
		private string SaveMMFModel( string filename, MIL_ID m_modcontext )
		{
			try
			{
				if ( m_modcontext == MIL.M_NULL ) { return "Model Context is null"; }

				MIL.MmodSave( filename, m_modcontext, MIL.M_DEFAULT );
			}
			catch ( Exception ex )
			{
				return this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				if ( m_modcontext != MIL.M_NULL ) MIL.MmodFree( m_modcontext );
			}
			return string.Empty;
		}
		public string MMFExportFromArr( byte[] MMFArr, string filename )
		{
			string sErr = string.Empty;
			MIL_ID MMFContext = MIL.M_NULL;
			try
			{
				sErr = this.MMFLoadByteArr( MMFArr, ref MMFContext );
				if ( sErr != string.Empty ) throw new Exception( sErr );
				sErr = this.SaveMMFModel( filename, MMFContext );
				if ( sErr != string.Empty ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
			}
			return sErr;
		}
		#endregion
		#region Arr mmf to bmp
		private string GetMMFModelBitmap( MIL_ID m_modcontext, ref Bitmap ModelBitmap )
		{
			MIL_ID modelBuf = MIL.M_NULL;
			try
			{
				if ( m_modcontext == MIL.M_NULL ) { return "Model Context is null"; }

				ModelBitmap = null;

				MIL.MbufAlloc2d( m_system,
						MIL.MmodInquire( m_modcontext, MIL.M_DEFAULT, MIL.M_ALLOC_SIZE_X, MIL.M_NULL ),
						MIL.MmodInquire( m_modcontext, MIL.M_DEFAULT, MIL.M_ALLOC_SIZE_Y, MIL.M_NULL ),
						8 + MIL.M_UNSIGNED,
						MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
						ref modelBuf );
				//MIL.MbufAllocColor( m_system,
				//		3,
				//		MIL.MmodInquire( m_modcontext, MIL.M_DEFAULT, MIL.M_ALLOC_SIZE_X, MIL.M_NULL ),
				//		MIL.MmodInquire( m_modcontext, MIL.M_DEFAULT, MIL.M_ALLOC_SIZE_Y, MIL.M_NULL ),
				//		8 + MIL.M_UNSIGNED,E:\NeowiseWhite\NEOWISE\MainApp\Sequence\PNPSeq.cs
				//		MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
				//		ref modelBuf );
				MIL.MgraColor( MIL.M_DEFAULT, MIL.M_COLOR_RED );
				MIL.MmodDraw( MIL.M_DEFAULT, m_modcontext, modelBuf, MIL.M_DRAW_IMAGE + MIL.M_DRAW_EDGES, MIL.M_DEFAULT, MIL.M_DEFAULT );
				//B262_Process.Save( "c:\\test.bmp", modelBuf );
				ModelBitmap = this.CreateBitmapFromMILBuffer( modelBuf );
			}
			catch ( Exception ex )
			{
				return this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				if ( modelBuf != MIL.M_NULL ) MIL.MbufFree( modelBuf );
				if ( m_modcontext != MIL.M_NULL ) MIL.MmodFree( m_modcontext );
			}

			return string.Empty;
		}
		private string GetMMFModelMilImg( MIL_ID m_modcontext, ref MIL_ID ModelBitmap )
		{
			//modelBuf = MIL.M_NULL;
			try
			{
				if ( m_modcontext == MIL.M_NULL ) { return "Model Context is null"; }
				//MIL.MbufAlloc2d( m_system,
				//		MIL.MmodInquire( m_modcontext, MIL.M_DEFAULT, MIL.M_ALLOC_SIZE_X, MIL.M_NULL ),
				//		MIL.MmodInquire( m_modcontext, MIL.M_DEFAULT, MIL.M_ALLOC_SIZE_Y, MIL.M_NULL ),
				//		8 + MIL.M_UNSIGNED,
				//		MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
				//		ref ModelBitmap );
				MIL.MbufAllocColor( m_system,
						3,
						MIL.MmodInquire( m_modcontext, MIL.M_DEFAULT, MIL.M_ALLOC_SIZE_X, MIL.M_NULL ),
						MIL.MmodInquire( m_modcontext, MIL.M_DEFAULT, MIL.M_ALLOC_SIZE_Y, MIL.M_NULL ),
						8 + MIL.M_UNSIGNED,
						MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
						ref ModelBitmap );
				MIL.MgraColor( MIL.M_DEFAULT, MIL.M_COLOR_RED );
				MIL.MmodDraw( MIL.M_DEFAULT, m_modcontext, ModelBitmap, MIL.M_DRAW_IMAGE + MIL.M_DRAW_EDGES, MIL.M_DEFAULT, MIL.M_DEFAULT );
				//B262_Process.Save( "c:\\test.bmp", modelBuf );
				//ModelBitmap = this.CreateBitmapFromMILBuffer( modelBuf );
			}
			catch ( Exception ex )
			{
				return this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				//if ( modelBuf != MIL.M_NULL ) MIL.MbufFree( modelBuf );
				if ( m_modcontext != MIL.M_NULL ) MIL.MmodFree( m_modcontext );
			}

			return string.Empty;
		}
		public string ArrayMMFToBMP( byte[] mmf_Arr, ref Bitmap ModelBitmap )
		{
			MIL_ID mmfContext = MIL.M_NULL;
			ModelBitmap = null;
			var sErr = string.Empty;
			try
			{
				if ( mmf_Arr == null ) return sErr;
				sErr = this.MMFLoadByteArr( mmf_Arr, ref mmfContext );
				if ( sErr != string.Empty )
					throw new Exception( sErr );
				sErr = this.GetMMFModelBitmap( mmfContext, ref ModelBitmap );
				if ( sErr != string.Empty )
					throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				return this.FormatErrMsg( this.Name, ex );
			}
			finally
			{

			}
			return string.Empty;
		}
		public string ArrayMMFToMilIMG( byte[] mmf_Arr, ref MIL_ID ModelBitmap )
		{
			MIL_ID mmfContext = MIL.M_NULL;
			var sErr = string.Empty;
			try
			{
				if ( mmf_Arr == null ) return sErr;
				sErr = this.MMFLoadByteArr( mmf_Arr, ref mmfContext );
				if ( sErr != string.Empty )
					throw new Exception( sErr );
				sErr = this.GetMMFModelMilImg( mmfContext, ref ModelBitmap );
				if ( sErr != string.Empty )
					throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				return this.FormatErrMsg( this.Name, ex );
			}
			finally
			{

			}
			return string.Empty;
		}
		#endregion
		#region MET to Byte Arr
		private string LoadMET( string filename, ref MIL_ID metcontext )
		{
			string sErr = string.Empty;
			MIL_INT sizeRequired = 0;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );
				MIL.MmetRestore( filename, m_system, MIL.M_DEFAULT, ref metcontext );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
				if ( metcontext != null )
				{
					MIL.MmetFree( metcontext );
				}
			}
			finally
			{
			}
			return sErr;
		}
		private string MetToByteArr( out byte[] met_Arr, ref MIL_ID metcontext )
		{
			string sErr = string.Empty;
			MIL_INT sizeRequired = 0;
			met_Arr = null;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );
				MIL.MmetStream( MIL.M_NULL, MIL.M_NULL, MIL.M_INQUIRE_SIZE_BYTE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref metcontext, ref sizeRequired );
				met_Arr = new byte[ sizeRequired ];
				MIL.MmetStream( met_Arr, m_system, MIL.M_SAVE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref metcontext, MIL.M_NULL );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				if ( metcontext != MIL.M_NULL ) MIL.MmetFree( metcontext );
			}
			return sErr;
		}
		public string LoadMETPathToByteArr( string filename, out byte[] met_Arr )
		{
			string sErr = string.Empty;
			MIL_ID metcontext = 0;
			met_Arr = null;
			try
			{
				sErr = this.LoadMET( filename, ref metcontext );
				if ( sErr != string.Empty ) throw new Exception( sErr );
				sErr = this.MetToByteArr( out met_Arr, ref metcontext );
				if ( sErr != string.Empty ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
			}
			return sErr;
		}
		#endregion
		#region export met from arr
		private string METLoadByteArr( byte[] met_Arr, ref MIL_ID metcontext )
		{
			string sErr = string.Empty;
			MIL_INT sizeRequired = 0;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );
				MIL.MmetStream( met_Arr, m_system, MIL.M_RESTORE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref metcontext, MIL.M_NULL );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
			}
			return sErr;
		}
		private string SaveMETModel( string filename, MIL_ID metcontext )
		{
			try
			{
				if ( metcontext == MIL.M_NULL ) { return "MET Context is null"; }
				MIL.MmetSave( filename, metcontext, MIL.M_DEFAULT );
			}
			catch ( Exception ex )
			{
				return this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				if ( metcontext != MIL.M_NULL ) { MIL.MmetFree( metcontext ); }
			}
			return string.Empty;
		}
		public string METExportFromArr( byte[] METArr, string filename )
		{
			string sErr = string.Empty;
			MIL_ID METContext = MIL.M_NULL;
			try
			{
				sErr = this.METLoadByteArr( METArr, ref METContext );
				if ( sErr != string.Empty ) throw new Exception( sErr );
				sErr = this.SaveMETModel( filename, METContext );
				if ( sErr != string.Empty ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
			}
			return sErr;
		}
		#endregion
		private string ConvertMETFileToStream( string fileName, out byte[] met_Arr, ref MIL_ID metcontext )
		{
			string sErr = string.Empty;
			MIL_INT sizeRequired = 0;
			met_Arr = null;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );
				MIL.MmetRestore( fileName, m_system, MIL.M_DEFAULT, ref metcontext );
				MIL.MmetStream( MIL.M_NULL, MIL.M_NULL, MIL.M_INQUIRE_SIZE_BYTE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref metcontext, ref sizeRequired );
				met_Arr = new byte[ sizeRequired ];
				MIL.MmetStream( met_Arr, m_system, MIL.M_SAVE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref metcontext, MIL.M_NULL );
			}
			catch ( Exception ex )
			{
				sErr = "Conversion from MET to byte Array Failed" + ex.Message;
			}
			finally
			{
			}
			return sErr;
		}
		#region arr met to bmp
		private string GetMETModelBitmap( MIL_ID metcontext, ref Bitmap ModelBitmap, int _Width, int _Height )
		{
			MIL_ID modelBuf = MIL.M_NULL;
			MIL_ID graContext = MIL.M_NULL;
			MIL_ID graList = MIL.M_NULL;
			try
			{
				if ( metcontext == MIL.M_NULL ) { return "Model Context is null"; }
				var Width = MIL.MmetInquire( metcontext, MIL.M_DEFAULT, MIL.M_TEMPLATE_REFERENCE_SIZE_X );
				var Height = MIL.MmetInquire( metcontext, MIL.M_DEFAULT, MIL.M_TEMPLATE_REFERENCE_SIZE_Y );
				if ( Width == 0 )
					Width = _Width;
				if ( Height == 0 )
					Height = _Height;
				MIL.MbufAlloc2d( m_system,
					Width,
					Height,
					8 + MIL.M_UNSIGNED,
					MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
					ref modelBuf );

				MIL.MbufClear( modelBuf, MIL.M_COLOR_GRAY );
				MIL.MmetControl( metcontext, MIL.M_DEFAULT, MIL.M_TEMPLATE_REFERENCE_ID, modelBuf );

				MIL.MgraAlloc( m_system, ref graContext );
				MIL.MgraControl( graContext, MIL.M_COLOR, MIL.M_COLOR_BLACK );
				MIL.MgraAllocList( m_system, MIL.M_DEFAULT, ref graList );
				MIL.MmetDraw( graContext, metcontext, graList, MIL.M_DRAW_TOLERANCE, MIL.M_ALL_TOLERANCES, MIL.M_DEFAULT );
				MIL.MgraDraw( graList, modelBuf, MIL.M_DEFAULT );
				MIL.MmetDraw( graContext, metcontext, graList, MIL.M_DRAW_REGION, MIL.M_ALL_FEATURES, MIL.M_DEFAULT );
				MIL.MgraDraw( graList, modelBuf, MIL.M_DEFAULT );

				ModelBitmap = this.CreateBitmapFromMILBuffer( modelBuf );
			}
			catch ( Exception ex )
			{
				return this.FormatErrMsg( this.Name, ex );
			}
			finally
			{
				if ( modelBuf != MIL.M_NULL ) { MIL.MbufFree( modelBuf ); }
				if ( metcontext != MIL.M_NULL ) { MIL.MmetFree( metcontext ); }
				if ( graContext != MIL.M_NULL ) { MIL.MgraFree( graContext ); }
				if ( graList != MIL.M_NULL ) { MIL.MgraFree( graList ); }
			}

			return string.Empty;
		}
		public string ArrayMETToBMP( byte[] met_Arr, ref Bitmap ModelBitmap, int Width, int Height )
		{
			MIL_ID metcontext = MIL.M_NULL;
			ModelBitmap = null;
			var sErr = string.Empty;
			try
			{
				if ( met_Arr == null ) return sErr;
				sErr = this.METLoadByteArr( met_Arr, ref metcontext );
				if ( sErr != string.Empty )
					throw new Exception( sErr );
				sErr = this.GetMETModelBitmap( metcontext, ref ModelBitmap, Width, Height );
				if ( sErr != string.Empty )
					throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				return this.FormatErrMsg( this.Name, ex );
			}
			finally
			{

			}
			return string.Empty;
		}
		#endregion
		#endregion
		#region Utility
		private Bitmap CreateBitmapFromMILBuffer( MIL_ID MilBuffer )
		{
			int sizeX = 0, sizeY = 0, stride = 0, numBands = 0;
			MIL_INT DataFormat = 0;
			MIL.MbufInquire( MilBuffer, MIL.M_SIZE_X, ref sizeX );
			MIL.MbufInquire( MilBuffer, MIL.M_SIZE_Y, ref sizeY );
			MIL.MbufInquire( MilBuffer, MIL.M_PITCH_BYTE, ref stride );
			MIL.MbufInquire( MilBuffer, MIL.M_SIZE_BAND, ref numBands );
			MIL.MbufInquire( MilBuffer, MIL.M_DATA_FORMAT, ref DataFormat );
			PixelFormat format = PixelFormat.Format8bppIndexed;
			if ( numBands >= 3 )
			{
				if ( DataFormat == MIL.M_BGR32 )
					format = PixelFormat.Format32bppRgb;
				else //if (DataFormat == MIL.M_BGR24_PACKED)
					format = PixelFormat.Format24bppRgb;
			}

			//Here create the Bitmap with the known height, width and format
			//PixelFormat format = (numBands == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb);
			//PixelFormat format = (numBands == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format32bppRgb);
			try
			{
				Bitmap tempBitmap = new Bitmap( sizeX, sizeY, format );
				BitmapData data = tempBitmap.LockBits( new Rectangle( 0, 0, sizeX, sizeY ), ImageLockMode.WriteOnly, format );
				IntPtr ptr = ( IntPtr )MIL.MbufInquire( MilBuffer, MIL.M_HOST_ADDRESS, MIL.M_NULL );
				unsafe
				{
					System.Diagnostics.Debug.Assert( data.Width == sizeX );
					System.Diagnostics.Debug.Assert( data.Height == sizeY );

					byte* src = ( byte* )ptr.ToPointer();
					byte* dest = ( byte* )data.Scan0.ToPointer();
					//int pixelsPerRow = sizeX * numBands;
					int pixelsPerRow = 0;
					if ( numBands == 1 )
						pixelsPerRow = sizeX * numBands;
					else
					{
						if ( DataFormat == MIL.M_BGR32 )
							pixelsPerRow = sizeX * ( numBands + 1 );
						else
							pixelsPerRow = sizeX * numBands;
					}
					int scrOffset = stride - pixelsPerRow;
					int destOffset = data.Stride - pixelsPerRow;
					for ( int j = 0; j < sizeY; j++ )
					{
						// Copy every pixel of a row.
						for ( int i = 0; i < pixelsPerRow; i++ )
						{
							*dest++ = *src++;
						}

						// At the end of the row, make the pointers point to the next row.
						src += scrOffset;
						dest += destOffset;
					}
				}

				tempBitmap.UnlockBits( data );
				if ( numBands == 1 )
				{
					ColorPalette palette = tempBitmap.Palette;
					for ( int i = 0; i < 256; i++ )
					{
						palette.Entries[ i ] = Color.FromArgb( i, i, i );
					}
					tempBitmap.Palette = palette;
				}

				return tempBitmap;
				//tempBitmap.Save("Temp bitmap" + numBands);
			}
			catch
			{
				return null;
			}
			//return tempBitmap;    
		}

		public override void ApplyRecipe( RecipeBaseUtility recipeItem )
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}



public enum OrientationResult
{
	NotFound,
	Checking,
	Found,
}
