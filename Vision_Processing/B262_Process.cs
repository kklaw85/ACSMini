using HiPA.Common;
using HiPA.Common.Forms;
using JptMatroxSystem;
using Matrox.MatroxImagingLibrary;
using netDxf;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace B262_Vision_Processing
{
	public class B262_Process
	{
		public static Bitmap BlankBmp = new Bitmap( 256, 256 );
		public const string METFilter = "files (*.met) | *.met";
		public const string MMFFilter = "files (*.mmf) | *.mmf";
		private static MatroxSystem _appID = MatroxSystem.Instance;
		public static MIL_ID m_system = _appID.HostSystemID;
		public static PointD cameraCentre = new PointD( 2736, 1829, 0 );
		private static DxfDocument dxfDoc = new DxfDocument();
		#region Static/Dynamic Test
		public static string FindModel( string mmffile, CameraObj CamObj, out C_PointD modelPos, out double modelAngle )
		{
			string sErr = string.Empty;
			MIL_ID m_ModContextResult = MIL.M_NULL;
			MIL_ID m_ModContext = MIL.M_NULL;
			modelPos = new C_PointD();
			modelAngle = 0;
			try
			{
				MIL.MmodRestore( mmffile, m_system, MIL.M_DEFAULT, ref m_ModContext );
				m_ModContextResult = MIL.MmodAllocResult( m_system, MIL.M_DEFAULT, MIL.M_NULL );
				if ( MIL.MmodInquire( m_ModContext, MIL.M_CONTEXT, MIL.M_PREPROCESSED ) == 0 )
					MIL.MmodPreprocess( m_ModContext, MIL.M_DEFAULT );
				MIL.MmodFind( m_ModContext, CamObj.MilImage, m_ModContextResult );
				var Height = 0;
				var Width = 0;
				MIL.MbufInquire( CamObj.MilImage, MIL.M_SIZE_Y, ref Height );
				MIL.MbufInquire( CamObj.MilImage, MIL.M_SIZE_X, ref Width );
				int iNumberPoint = 0;
				MIL.MmodGetResult( m_ModContextResult, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref iNumberPoint );
				var modelPosX = 0d;
				var modelPosY = 0d;
				var XPos = 0d;
				var YPos = 0d;
				if ( iNumberPoint == 1 )
				{
					var ImgCenterX = Width / 2;
					var ImgCenterY = Height / 2;

					MIL.MmodGetResult( m_ModContextResult, MIL.M_ALL, MIL.M_POSITION_X, ref modelPosX );
					MIL.MmodGetResult( m_ModContextResult, MIL.M_ALL, MIL.M_POSITION_Y, ref modelPosY );
					MIL.MmodGetResult( m_ModContextResult, MIL.M_ALL, MIL.M_ANGLE, ref modelAngle );
					modelPos = new C_PointD()
					{
						X = -XPos + ImgCenterX,
						Y = YPos - ImgCenterY,
					};
				}
			}
			catch ( Exception ex )
			{
				sErr = "Find model fail" + ex.Message;
			}
			finally
			{
				if ( m_ModContextResult != MIL.M_NULL )
				{
					MIL.MmodFree( m_ModContextResult );
				}
				if ( m_ModContext != MIL.M_NULL )
				{
					MIL.MmodFree( m_ModContext );
				}
			}
			return sErr;
		}
		public static string DefineMMF( MIL_ID MilImage, out MIL_ID m_ModContext )
		{
			string sErr = string.Empty;
			m_ModContext = MIL.M_NULL;
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );

				MIL.MmodAlloc( m_system, MIL.M_GEOMETRIC, MIL.M_DEFAULT, ref m_ModContext );

				MIL.MmodDefine( m_ModContext, MIL.M_IMAGE, MilImage, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT );

				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_ACCURACY, MIL.M_HIGH );

				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_SPEED, MIL.M_VERY_HIGH );

				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_SEARCH_ANGLE_RANGE, MIL.M_DISABLE );

				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_SEARCH_SCALE_RANGE, MIL.M_DISABLE ); //210804 disabled, it somehow messed up the score

				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_SEARCH_POSITION_RANGE, MIL.M_ENABLE );

				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_NUMBER, MIL.M_ALL );

				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_SMOOTHNESS, 100 );
				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_DETAIL_LEVEL, MIL.M_MEDIUM );
				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_ACCEPTANCE, 70 );
				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_CERTAINTY, 90 );

				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_ACCEPTANCE_TARGET, 10 );
				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_CERTAINTY_TARGET, 10 );

				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_ANGLE_DELTA_NEG, 0 );
				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_ANGLE_DELTA_POS, 0 );

				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_SCALE_MIN_FACTOR, 1 );
				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_SCALE_MAX_FACTOR, 1 );

				MIL.MmodControl( m_ModContext, MIL.M_ALL, MIL.M_POLARITY, MIL.M_SAME );
				MIL.MmodControl( m_ModContext, MIL.M_CONTEXT, MIL.M_TIMEOUT, 300 );
				//  MIL.MmodSave(MIL.M_INTERACTIVE, m_ModContext, MIL.M_DEFAULT);
			}
			catch ( Exception ex )
			{
				sErr = "Define Image to MMF failed." + ex.Message;
			}
			return sErr;
		}
		public static ErrorResult FindModel( byte[] mmffile, CameraObj CamObj, ROIRectangle ROI, out C_PointD RawPosition, out C_PointD RawOffsetCenter, out C_PointD OffsetPosition )
		{
			var Result = new ErrorResult();
			MIL_ID m_ModContextResult = MIL.M_NULL;
			MIL_ID m_ModContext = MIL.M_NULL;
			MIL_ID m_roiImage = MIL.M_NULL;
			OffsetPosition = new C_PointD( 999, 999, 999 );
			RawPosition = new C_PointD( 999, 999, 999 );
			RawOffsetCenter = new C_PointD( 999, 999, 999 );
			try
			{
				if ( mmffile == null )
					throw new Exception( $"^{( int )RunErrors.ERR_ByteArrIsNull }^" );
				//var IMAGE_FILE = "C:\\grid.bmp";
				//MIL.MbufImport( IMAGE_FILE, MIL.M_DEFAULT, MIL.M_RESTORE + MIL.M_NO_GRAB + MIL.M_NO_COMPRESS, m_system, ref CamObj.MilImage );
				MIL.MbufChild2d( CamObj.MilImage, ROI.X, ROI.Y, ROI.Width, ROI.Height, ref m_roiImage );

				MIL.MmodStream( mmffile, m_system, MIL.M_RESTORE, MIL.M_MEMORY, MIL.M_DEFAULT, MIL.M_DEFAULT, ref m_ModContext, MIL.M_NULL );
				m_ModContextResult = MIL.MmodAllocResult( m_system, MIL.M_DEFAULT, MIL.M_NULL );
				if ( MIL.MmodInquire( m_ModContext, MIL.M_CONTEXT, MIL.M_PREPROCESSED ) == 0 )
					MIL.MmodPreprocess( m_ModContext, MIL.M_DEFAULT );
				MIL.MmodFind( m_ModContext, m_roiImage, m_ModContextResult );
				var Height = 0;
				var Width = 0;
				MIL.MbufInquire( CamObj.MilImage, MIL.M_SIZE_Y, ref Height );
				MIL.MbufInquire( CamObj.MilImage, MIL.M_SIZE_X, ref Width );
				int iNumberPoint = 0;
				MIL.MmodGetResult( m_ModContextResult, MIL.M_DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref iNumberPoint );
				var modelPosX = 0d;
				var modelPosY = 0d;
				var modelAngle = 0d;
				if ( iNumberPoint == 1 )
				{
					var ImgCenterX = Width / 2;
					var ImgCenterY = Height / 2;
					MIL.MmodGetResult( m_ModContextResult, MIL.M_ALL, MIL.M_POSITION_X, ref modelPosX );
					MIL.MmodGetResult( m_ModContextResult, MIL.M_ALL, MIL.M_POSITION_Y, ref modelPosY );
					MIL.MmodGetResult( m_ModContextResult, MIL.M_ALL, MIL.M_ANGLE, ref modelAngle );
					OffsetPosition = new C_PointD()
					{
						X = modelPosX + ROI.X - ( ImgCenterX + CamObj.XHairPosX ),
						Y = modelPosY + ROI.Y - ( ImgCenterY + CamObj.XHairPosY ),
						Theta = modelAngle,
					};
					RawPosition = new C_PointD()
					{
						X = modelPosX + ROI.X,
						Y = modelPosY + ROI.Y,
						Theta = modelAngle,
					};
					RawOffsetCenter = new C_PointD()
					{
						X = modelPosX + ROI.X - ImgCenterX,
						Y = modelPosY + ROI.Y - ImgCenterY,
						Theta = modelAngle,
					};
				}
				else if ( iNumberPoint > 1 )
				{
					Result.EClass = ErrorClass.E3;
					throw new Exception( "More than 1 points detected" );
				}
				else
				{
					Result.EClass = ErrorClass.E3;
					throw new Exception( "No point detected" );
				}
			}
			catch ( Exception ex )
			{
				if ( Result.EClass == ErrorClass.OK ) Result.EClass = ErrorClass.E6;
				Result.ErrorMessage = "Find model fail" + ex.Message;
			}
			finally
			{
				if ( m_ModContextResult != MIL.M_NULL )
				{
					MIL.MmodFree( m_ModContextResult );
				}
				if ( m_ModContext != MIL.M_NULL )
				{
					MIL.MmodFree( m_ModContext );
				}
			}
			return Result;
		}
		#endregion
		/// <summary>
		/// Convert image from bitmap format to 1D byte array
		/// </summary>
		/// <param name="image">Input image in bitmap format</param>
		/// <param name="OutArray">Output image in 1D byte array</param>
		/// <returns></returns>
		private static string ConvertBitmapTo1DByteArray( Bitmap image, ref byte[] OutArray )
		{
			string sErr = string.Empty;
			try
			{
				byte[] result = new byte[ image.Width * image.Height ];

				BitmapData bmpData = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
					ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb );
				unsafe
				{
					byte* ptr = ( byte* )bmpData.Scan0;

					int remain = bmpData.Stride - bmpData.Width * 3;

					for ( int i = 0, iShift = 0; i < bmpData.Height; i++, iShift = i * bmpData.Width )
					{
						for ( int j = 0; j < bmpData.Width; j++, iShift++ )
						{
							byte n1 = ptr[ 0 ];

							result[ iShift ] = n1;

							ptr += 3;
						}
						ptr += remain;
					}

				}
				image.UnlockBits( bmpData );

				OutArray = result;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ConvertBitmapTo1DByteArray error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}

			return sErr;
		}
		/// <summary>
		/// Convert Image data byte array to MIL Image
		/// </summary>
		/// <param name="data">(IN) Image data array</param>
		/// <param name="width">(IN) Image Width</param>
		/// <param name="height">(IN) Image Height</param>
		/// <param name="MilImage">(OUT) MIL Image</param>
		private static string ByteArray2MilImage( byte[] data, int width, int height, ref MIL_ID MilImage, out GCHandle UserDataHandle )
		{
			string sErr = string.Empty;
			MilImage = MIL.M_NULL;
			UserDataHandle = new GCHandle();
			try
			{
				if ( m_system == MIL.M_NULL )
					throw new NullReferenceException( "MIL System is null. Please set it using SetMatroxSystemID(MIL_ID SystemId)" );

				UserDataHandle = GCHandle.Alloc( data, GCHandleType.Pinned );

				MIL.MbufCreate2d( m_system, width, height, 8 + MIL.M_UNSIGNED,
				MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, MIL.M_HOST_ADDRESS + MIL.M_PITCH,
				MIL.M_DEFAULT, ( ulong )UserDataHandle.AddrOfPinnedObject(), ref MilImage );

				//UserDataHandle.Free();
			}
			catch ( Exception ex )
			{
				sErr = "Converting Image to MIL Image fail :" + ex.Message;
			}
			return sErr;
		}
		public static string LoadImageFromFileName( string Filename, ref MIL_ID MilImage, out GCHandle handle )
		{
			string sErr = string.Empty;
			Bitmap _bitmapLoaded = null;
			byte[] _array8Loaded = null;
			handle = new GCHandle();
			try
			{
				if ( !File.Exists( Filename ) )
				{ throw new Exception( "File not exist: " + Filename ); }


				if ( MilImage != MIL.M_NULL )
				{
					MIL.MbufFree( MilImage );
					MilImage = MIL.M_NULL;
				}

				System.Drawing.Image img = System.Drawing.Image.FromFile( Filename );

				_bitmapLoaded = ( Bitmap )img;

				int _width = img.Width;
				int _height = img.Height;

				sErr = ConvertBitmapTo1DByteArray( ( Bitmap )img, ref _array8Loaded );
				if ( sErr.Length > 0 ) throw new Exception( sErr );

				ByteArray2MilImage( _array8Loaded, _width, _height, ref MilImage, out handle );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Load Image to MIL_ID error: " + ex.Message );
			}
			return sErr;
		}
		public static string Save( string Filename, MIL_ID milimg )
		{
			try
			{
				if ( milimg == MIL.M_NULL ) return string.Format( "MilImg is null" );

				if ( !Directory.Exists( Path.GetDirectoryName( Filename ) ) )
					Directory.CreateDirectory( Path.GetDirectoryName( Filename ) );
				MIL.MbufExport( Filename, ( long )64, milimg );
			}
			catch ( Exception ex ) { return string.Format( "Save error: " + ex.Message ); }
			return string.Empty;
		}
	}
	public class VisionProcessing : BaseUtility
	{
		private MIL_ID m_system => B262_Process.m_system;
		private PointD cameraCentre = B262_Process.cameraCentre;
		#region General Functions
		/// <summary>
		/// Apply ROI on image data array
		/// </summary>
		/// <param name="Array">(IN) Image data array</param>
		/// <param name="width">(IN) Image Width</param>
		/// <param name="roiLeft">(IN) ROI Rect left</param>
		/// <param name="roiTop">(IN) ROI Rect top</param>
		/// <param name="roiWidth">(IN) ROI Rect size width</param>
		/// <param name="roiHeight">(IN) ROI Rect size height</param>
		/// <param name="NewArray">(OUT) Result Image data array</param>
		public void ROIArray( byte[] Array, int width, int roiLeft, int roiTop, int roiWidth, int roiHeight, out byte[] NewArray )
		{
			NewArray = new byte[ roiWidth * roiHeight ];
			int arrayPos = 0;

			for ( int i = roiTop; i < roiTop + roiHeight; i++ )
			{
				for ( int j = roiLeft; j < roiLeft + roiWidth; j++ )
				{
					NewArray[ arrayPos ] = ( j + i * width >= Array.Length ) ? ( byte )0 : Array[ j + i * width ];
					arrayPos++;
				}
			}
		}
		private string RotateImagebyImageCenter( double degreeToRotate, ref MIL_ID MilImage )
		{
			string sErr = string.Empty;

			try
			{
				MIL.MimRotate( MilImage, MilImage, degreeToRotate, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT );
			}
			catch ( Exception ex )
			{
				sErr = "Image Rotation failed" + ex.Message;
			}
			return sErr;
		}
		#endregion
	}
	public class CameraObj
	{
		public int XHairPosX { get; set; } = 0;
		public int XHairPosY { get; set; } = 0;
		public MIL_ID MilImage = MIL.M_NULL;
		public CameraObj( MIL_ID MilImage, int XHairPosX, int XHairPosY )
		{
			this.MilImage = MilImage;
			this.XHairPosX = XHairPosX;
			this.XHairPosY = XHairPosY;
		}
		public CameraObj()
		{ }
		public void Copy( CameraObj source, int XHairPosX, int XHairPosY )
		{
			try
			{
				MIL.MbufAlloc2d( B262_Process.m_system,
								MIL.MbufInquire( source.MilImage, MIL.M_SIZE_X, MIL.M_NULL ),
								MIL.MbufInquire( source.MilImage, MIL.M_SIZE_Y, MIL.M_NULL ),
								MIL.MbufInquire( source.MilImage, MIL.M_TYPE, MIL.M_NULL ) + MIL.M_UNSIGNED,
								MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC,
								ref this.MilImage );
				MIL.MbufCopy( source.MilImage, this.MilImage );
				this.XHairPosX = XHairPosX;
				this.XHairPosY = XHairPosY;
			}
			catch
			{ }
		}
		public CameraObj( CameraObj source, int XHairPosX, int XHairPosY )
		{
			try
			{
				MIL.MbufAlloc2d( B262_Process.m_system,
								MIL.MbufInquire( source.MilImage, MIL.M_SIZE_X, MIL.M_NULL ),
								MIL.MbufInquire( source.MilImage, MIL.M_SIZE_Y, MIL.M_NULL ),
								MIL.MbufInquire( source.MilImage, MIL.M_TYPE, MIL.M_NULL ) + MIL.M_UNSIGNED,
								MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC,
								ref this.MilImage );
				MIL.MbufCopy( source.MilImage, this.MilImage );
				this.XHairPosX = XHairPosX;
				this.XHairPosY = XHairPosY;
			}
			catch
			{ }
		}
	}
}
