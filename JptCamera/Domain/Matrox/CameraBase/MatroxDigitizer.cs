using HiPA.Common;
using HiPA.Common.Forms;
using Matrox.MatroxImagingLibrary;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace JptCamera
{
	public class MatroxDigitizer
	{

		// Number of images in the buffering grab queue.
		// Generally, increasing this number gives a better real-time grab.
		private const int BUFFERING_SIZE_MAX = 5;
		public ROIHandler Cal { get; set; } = new ROIHandler();
		public ROIHandler Inspect { get; set; } = new ROIHandler();
		const int MAX_CAM = 32;
		const int MAX_ADAPTERS = 16;
		/*
        public struct DigHookDataStruct
        {
            MIL_ID MilSystem;
            MIL_ID MilDigitizer;
            MIL_ID MilDisplay;
            MIL_ID MilImageDisp;
            MIL_ID[] MilGrabBufferList;
            MIL_INT MilGrabBufferListSize;
            MIL_INT ProcessedImageCount;
            double FrameRate;
            MIL_INT ResendRequests;
            MIL_INT PacketSize;
            MIL_INT PacketsMissed;
            MIL_INT CorruptImageCount;
            MIL_INT GrabInProgress;
            MIL_INT PayloadSize;
            string CamVendor;
            string CamModel;
            string CamUniqueId;
            string pAdapterName;
            bool IsConnected;
            MIL_INT BoardType;
        }

        public struct SysHookDataStruct
        {
            MIL_ID MilSystem;
            MIL_INT NbCameras;
            DigHookDataStruct CameraStruct;
            bool PrintAdapterInfo;
            string[] Adapters;
            MIL_INT BoardType;
        }
        */

		// User's processing function hook data object.
		public class HookDataStruct : EventArgs
		{

			public MIL_ID MilDigitizer { get; set; }
			public MIL_ID MilImageDisp { get; set; }
			public int ProcessedImageCount { get; set; }
			public string PixelFormat { get; set; }
			public double BufferRotDegree { get; set; }
			public WindowsFormsHost winHost { get; set; }

			public IntPtr winHandler { get; set; }
		};

		#region Private members

		private MIL_ID _mApp;
		private MIL_ID _mSys;
		private MIL_ID _mBuffer;
		private MIL_ID _mCamera;
		private MIL_ID _mDisp;
		private MIL_ID _mOverlayDisp;
		private MIL_ID _mBufferForDisp = MIL.M_NULL;
		private MIL_ID _mgraphicList = MIL.M_NULL;
		//private MIL_ID MilImageDisp = MIL.M_NULL;

		private MIL_ID[] MilGrabBufferList = new MIL_ID[ BUFFERING_SIZE_MAX ];
		int MilGrabBufferListSize = 0;
		MIL_INT ProcessFrameCount = 0;

		HookDataStruct UserHookData = new HookDataStruct();
		GCHandle hUserData;
		MIL_DIG_HOOK_FUNCTION_PTR ProcessingFunctionPtr;
		MIL_GRA_HOOK_FUNCTION_PTR graphicFunctionPtr;
		private JptCamera.CameraSettings _settings;

		// Default image dimensions.
		private const int DEFAULT_IMAGE_SIZE_X = 640;
		private const int DEFAULT_IMAGE_SIZE_Y = 480;
		private const int DEFAULT_IMAGE_SIZE_BAND = 1;
		private const int DEFAULT_IMAGE_TYPE = 8 + MIL.M_UNSIGNED;

		private bool isCameraColorMode = false;

		private bool isInContinuousMode = false;
		private bool isInProcessMode = false;
		private bool isWpfDisplay = false;

		public bool isContinousMode => this.isInContinuousMode;
		#endregion

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// File format of the image. Usually BMP, TIFF
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public enum FileFormat : long
		{
			BMP = 64,
			RAW = 2,
			PNG = 8192,
			TIFF = 4096
		};

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Predefinition Camera connection type. Usually GigE, USB, Host
		/// System_Default using Matrox Config Default settings
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public enum CameraFormat
		{
			M_SYSTEM_USB3_VISION,
			M_SYSTEM_GIGE_VISION,
			M_SYSTEM_HOST,
			M_SYSTEM_DEFAULT
		};

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Creates a new unallocated MILApplication object.
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public MatroxDigitizer()
		{
			this._mApp = MIL.M_NULL;
			this._mSys = MIL.M_NULL;
			this._mBuffer = MIL.M_NULL;
			this._mCamera = MIL.M_NULL;
			this._mDisp = MIL.M_NULL;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Frees the MIL Application and its associated resources.
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public void FreeResources()
		{
			try
			{
				JPTUtility.Logger.doLog( "MtxCamera - FreeResources ..." );

				string sErr = string.Empty;

				if ( this.isInContinuousMode )
				{
					sErr = this.HaltContinuous();
					if ( sErr.Length > 0 ) { }
				}

				if ( this.isInProcessMode )
				{
					sErr = this.StopGrabProcess();
					if ( sErr.Length > 0 ) { }
				}

				if ( this._mOverlayDisp != MIL.M_NULL )
				{
					//MIL.MgraFree(_mOverlayDisp);
					this._mOverlayDisp = MIL.M_NULL;
				}

				// Free the display.
				if ( this._mDisp != MIL.M_NULL )
				{
					// If an image buffer is selected on the display, deselect it before freeing the display.
					MIL_ID selectedBufferId = ( MIL_ID )MIL.MdispInquire( this._mDisp, MIL.M_SELECTED, MIL.M_NULL );
					if ( selectedBufferId != MIL.M_NULL )
					{
						if ( this.isWpfDisplay )
						{
							MIL.MdispSelectWPF( this._mDisp, MIL.M_NULL, null );
						}
						else
						{
							MIL.MdispSelectWindow( this._mDisp, MIL.M_NULL, IntPtr.Zero );
						}
					}
					MIL.MdispFree( this._mDisp );
					this._mDisp = MIL.M_NULL;
				}

				// Free the GCHandle when no longer used
				if ( this.hUserData != null && this.hUserData.IsAllocated )
				{
					this.hUserData.Free();
				}

				if ( this.MilGrabBufferList.Length > 0 )
				{
					// Free the grab buffers.
					while ( this.MilGrabBufferListSize > 0 )
					{
						MIL.MbufFree( this.MilGrabBufferList[ --this.MilGrabBufferListSize ] );
					}
				}
				MIL.MgraHookFunction( this._mgraphicList, MIL.M_GRAPHIC_MODIFIED + MIL.M_UNHOOK, this.graphicFunctionPtr, MIL.M_NULL );
				if ( this._mgraphicList != MIL.M_NULL )
					MIL.MgraFree( this._mgraphicList );
				this.Cal.FreeResources();
				this.Inspect.FreeResources();
				if ( this._mBuffer != MIL.M_NULL )
				{
					MIL.MbufFree( this._mBuffer );
					this._mBuffer = MIL.M_NULL;
				}
				if ( this._mBufferForDisp != MIL.M_NULL )
				{
					MIL.MbufFree( this._mBufferForDisp );
					this._mBufferForDisp = MIL.M_NULL;
				}
				if ( this._mCamera != MIL.M_NULL )
				{
					MIL.MdigFree( this._mCamera );
					this._mCamera = MIL.M_NULL;
				}

				if ( this._mSys != MIL.M_NULL )
				{
					//MIL.MsysFree(_mSys);
					this._mSys = MIL.M_NULL;
				}

				if ( this._mApp != MIL.M_NULL )
				{
					//MIL.MappFree(_mApp);
					//_mApp = MIL.M_NULL;
				}

				// The object has been cleaned up.
				// This call removes the object from the finalization queue and 
				// prevent finalization code object from executing a second time.
				GC.SuppressFinalize( this );
			}
			catch
			{

			}
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Initialise resources before connection to camera
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string InitialiseSystem( MIL_ID MILSystem )
		{
			JPTUtility.Logger.doLog( "MtxCamera - InitialiseSystem ..." );
			try
			{

				if ( this._mSys == MIL.M_NULL )
				{
					this._mSys = MILSystem;
				}

				if ( this._mDisp == MIL.M_NULL )
				{
					MIL.MdispAlloc( this._mSys, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_WINDOWED, ref this._mDisp );
				}
				if ( this._mgraphicList == MIL.M_NULL )
				{
					MIL.MgraAllocList( this._mSys, MIL.M_DEFAULT, ref this._mgraphicList );
				}
				MIL.MdispControl( this._mDisp, MIL.M_CENTER_DISPLAY, MIL.M_ENABLE );
				this.graphicFunctionPtr = new MIL_GRA_HOOK_FUNCTION_PTR( this.GraphicHookHandler );
				MIL.MgraHookFunction( this._mgraphicList, MIL.M_GRAPHIC_MODIFIED, this.graphicFunctionPtr, MIL.M_NULL );
			}
			catch ( Exception ex )
			{
				string error = string.Format( "MtxCamera_InitialiseSystem error: " + ex.Message );
				JPTUtility.Logger.doLog( error );
				return error;
			}
			return string.Empty;
		}

		public string DisplayBitShift( int BitShift )
		{
			string sErr = string.Empty;
			try
			{
				if ( this._mDisp == MIL.M_NULL )
				{ return string.Format( "DisplayBitShift -  Display buffer not allocated" ); }

				MIL.MdispControl( this._mDisp, MIL.M_VIEW_MODE, MIL.M_BIT_SHIFT );
				MIL.MdispControl( this._mDisp, MIL.M_VIEW_BIT_SHIFT, BitShift );
			}
			catch ( Exception ex )
			{ sErr = string.Format( "MtxCamera_DisplayBitShift error " + ex.Message ); }
			return sErr;
		}

		public string InitialiseProcessBuffer()
		{
			try
			{
				for ( this.MilGrabBufferListSize = 0; this.MilGrabBufferListSize < BUFFERING_SIZE_MAX; this.MilGrabBufferListSize++ )
				{
					MIL.MbufAlloc2d( this._mSys,
									MIL.MdigInquire( this._mCamera, MIL.M_SIZE_X, MIL.M_NULL ),
									MIL.MdigInquire( this._mCamera, MIL.M_SIZE_Y, MIL.M_NULL ),
									MIL.MdigInquire( this._mCamera, MIL.M_TYPE, MIL.M_NULL ) + MIL.M_UNSIGNED,
									MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC,
									ref this.MilGrabBufferList[ this.MilGrabBufferListSize ] );

					if ( this.MilGrabBufferList[ this.MilGrabBufferListSize ] != MIL.M_NULL )
					{
						MIL.MbufClear( this.MilGrabBufferList[ this.MilGrabBufferListSize ], 0 );
					}
					else
					{
						break;
					}
				}

				// Free buffers to leave space for possible temporary buffers.
				for ( int n = 0; n < 2 && this.MilGrabBufferListSize > 0; n++ )
				{
					this.MilGrabBufferListSize--;
					MIL.MbufFree( this.MilGrabBufferList[ this.MilGrabBufferListSize ] );
				}
			}
			catch ( Exception ex ) { return string.Format( "InitialiseProcessBuffer error: " + ex.Message ); }
			return string.Empty;
		}

		public double ImageRotateAngle
		{
			get => this.UserHookData.BufferRotDegree;
			set => this.UserHookData.BufferRotDegree = value;
		}

		public string InitialiseHookFunction()
		{
			try
			{
				// Initialize the user's processing function data structure.
				this.UserHookData.MilDigitizer = this._mCamera;
				this.UserHookData.MilImageDisp = this._mBuffer;
				this.UserHookData.ProcessedImageCount = 0;
				this.UserHookData.PixelFormat = "";
				this.UserHookData.BufferRotDegree = 180; //Initialize here
														 // get a handle to the HookDataStruct object in the managed heap, we will use this 
														 // handle to get the object back in the callback function
				this.hUserData = GCHandle.Alloc( this.UserHookData );
				this.ProcessingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR( this.ProcessingFunction );
			}
			catch ( Exception ex ) { return string.Format( "InitialiseHookFunction error: " + ex.Message ); }
			return string.Empty;
		}

		public string AllocateCamera()
		{
			string sErr = string.Empty;
			try
			{
				if ( MIL.MsysInquire( this._mSys, MIL.M_DIGITIZER_NUM, MIL.M_NULL ) > 0 )
				{
					MIL.MdigAlloc( this._mSys, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_DEFAULT, ref this._mCamera );
					if ( this._mCamera == MIL.M_NULL )
						return string.Format( "Camera allocation failed" );
				}
				else
				{
					return string.Format( "No camera detected in the system" );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Camera_AllocateCamera error: " + ex.Message );
			}
			return sErr;
		}

		public string AllocateCamera( string CameraName, string DCF_Filename = "M_DEFAULT" )
		{
			string sErr = string.Empty;
			try
			{
				MIL_INT _boardType = MIL.M_NULL;

				MIL.MsysInquire( this._mSys, MIL.M_BOARD_TYPE, ref _boardType );

				if ( MIL.MsysInquire( this._mSys, MIL.M_DIGITIZER_NUM, MIL.M_NULL ) > 0 )
				{
					if ( ( _boardType & MIL.M_BOARD_TYPE_MASK ) == MIL.M_GIGE_VISION )
					{
						if ( MIL.MsysInquire( this._mSys, MIL.M_NUM_CAMERA_PRESENT, MIL.M_NULL ) > 0 )
						{
							MIL.MdigAlloc( this._mSys, CameraName, DCF_Filename, MIL.M_GC_DEVICE_USER_NAME, ref this._mCamera );
						}
					}
					else
					{
						MIL.MdigAlloc( this._mSys, CameraName, DCF_Filename, MIL.M_GC_DEVICE_USER_NAME, ref this._mCamera );
					}
					if ( this._mCamera == MIL.M_NULL )
						return string.Format( "Camera allocation failed" );
				}
				else
				{
					return string.Format( "No camera detected in the system" );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Camera_AllocateCamera error: " + ex.Message );
			}
			return sErr;
		}
		public string AllocateCamera( MIL_INT DeviceNumber, string DCF_Filename = "M_DEFAULT" )
		{
			string sErr = string.Empty;
			try
			{
				MIL_INT _boardType = MIL.M_NULL;

				MIL.MsysInquire( this._mSys, MIL.M_BOARD_TYPE, ref _boardType );

				if ( MIL.MsysInquire( this._mSys, MIL.M_DIGITIZER_NUM, MIL.M_NULL ) > 0 )
				{
					if ( ( _boardType & MIL.M_BOARD_TYPE_MASK ) == MIL.M_GIGE_VISION )
					{
						if ( MIL.MsysInquire( this._mSys, MIL.M_NUM_CAMERA_PRESENT, MIL.M_NULL ) > 0 )
						{
							MIL.MdigAlloc( this._mSys, DeviceNumber, DCF_Filename, MIL.M_DEV_NUMBER, ref this._mCamera );
						}
					}
					else
					{
						MIL.MdigAlloc( this._mSys, DeviceNumber, DCF_Filename, MIL.M_DEV_NUMBER, ref this._mCamera );
					}
					if ( this._mCamera == MIL.M_NULL )
						return string.Format( "Camera allocation failed" );
				}
				else
				{
					return string.Format( "No camera detected in the system" );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Camera_AllocateCamera error: " + ex.Message );
			}
			return sErr;
		}
		//public string AllocateCamera(MIL_INT DeviceNumber, string DCF_Filename = "M_DEFAULT")
		//{
		//    string sErr = string.Empty;
		//    try
		//    {
		//        if (MIL.MsysInquire(_mSys, MIL.M_DIGITIZER_NUM, MIL.M_NULL) > 0)
		//        {
		//            MIL.MdigAlloc(_mSys, DeviceNumber, DCF_Filename, MIL.M_DEV_NUMBER, ref _mCamera);
		//            if (_mCamera == MIL.M_NULL)
		//                return string.Format("Camera allocation failed");
		//        }
		//        else
		//        {
		//            return string.Format("No camera detected in the system");
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        sErr = string.Format("Camera_AllocateCamera error: " + ex.Message);
		//    }
		//    return sErr;
		//}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Check for camera and connect. Also allocate appropriate buffer
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		///        
		int CameraHeight = 0;
		int CameraWidth = 0;
		private MIL_ID _mGraphicLinesLaserspot = MIL.M_NULL;
		public string AllocateBuffer()
		{
			try
			{
				if ( this._mSys == MIL.M_NULL )
					return string.Format( "System is NULL" );

				//if ( this._mCamera == MIL.M_NULL )
				//	return string.Format( "Camera not allocated" );

				MIL_INT BufSizeX = DEFAULT_IMAGE_SIZE_X;
				MIL_INT BufSizeY = DEFAULT_IMAGE_SIZE_Y;
				MIL_INT BufSizeBand = DEFAULT_IMAGE_SIZE_BAND;
				MIL_INT BufType = DEFAULT_IMAGE_TYPE;
				long Attributes = MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC;

				if ( this._mCamera != MIL.M_NULL )
				{
					MIL.MdigInquire( this._mCamera, MIL.M_SIZE_X, ref BufSizeX );
					MIL.MdigInquire( this._mCamera, MIL.M_SIZE_Y, ref BufSizeY );
					MIL.MdigInquire( this._mCamera, MIL.M_SIZE_BAND, ref BufSizeBand );
					MIL.MdigInquire( this._mCamera, MIL.M_TYPE, ref BufType );
					// Add M_GRAB attribute if a digitizer is allocated.
					Attributes |= MIL.M_GRAB;
				}

				// Release the buffer before allocation
				if ( this._mBuffer != MIL.M_NULL )
				{
					MIL.MbufFree( this._mBuffer );
					this._mBuffer = MIL.M_NULL;
				}
				this.CameraHeight = ( int )BufSizeY;
				this.CameraWidth = ( int )BufSizeX;
				MIL.MbufAllocColor( this._mSys, BufSizeBand, BufSizeX, BufSizeY, BufType + MIL.M_UNSIGNED, Attributes, ref this._mBuffer );

				// Clear the buffer.
				MIL.MbufClear( this._mBuffer, 0 );

				//Release buffer before allocation.
				if ( this._mBufferForDisp != MIL.M_NULL )
				{
					MIL.MbufFree( this._mBufferForDisp );
					this._mBufferForDisp = MIL.M_NULL;
				}
				MIL.MbufAllocColor( this._mSys, BufSizeBand, BufSizeX, BufSizeY, BufType + MIL.M_UNSIGNED, Attributes, ref this._mBufferForDisp );
				MIL.MbufClear( this._mBufferForDisp, 0 );

				if ( this._mCamera != MIL.M_NULL )
					if ( !this.IsCameraColor )
					{
						// For processing event
						this.InitialiseProcessBuffer();

						this.InitialiseHookFunction();
					}
				this.Cal = new ROIHandler( this._mSys, this._mDisp, this._mBuffer, this._mgraphicList, ( int )BufSizeX, ( int )BufSizeY, MIL.M_COLOR_GREEN );
				this.Inspect = new ROIHandler( this._mSys, this._mDisp, this._mBuffer, this._mgraphicList, ( int )BufSizeX, ( int )BufSizeY, MIL.M_COLOR_CYAN );

				MIL.MgraAlloc( this._mSys, ref this._mGraphicLinesLaserspot );
			}
			catch ( Exception ex )
			{
				return string.Format( "MtxCamera_ConnectCamera error: " + ex.Message );
			}
			return string.Empty;
		}

		public bool IsCameraColor
		{
			get
			{
				var result = MIL.MdigInquire( this._mCamera, MIL.M_SIZE_BAND );
				if ( result > 1 )
				{ this.isCameraColorMode = true; }
				else { this.isCameraColorMode = false; }
				return this.isCameraColorMode;
			}
			set { this.isCameraColorMode = value; }
		}


		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Sets the WHND Handler control that will be used to host the MIL display.
		/// </summary>
		/// <param name="displayHost">A <see cref="WindowsFormsHost"/> control on which to display an image buffer.</param>
		/// <remarks>This method will select the image buffer to display in the <see cref="WindowsFormsHost"/> specified by <paramref name="displayHost"/>.</remarks>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string SetDisplayWinHnd( IntPtr winHandler )
		{
			try
			{
				if ( winHandler == null )
					throw new ArgumentNullException( "winHandler", "Must be a valid object." );

				// Set Display to scaled mode
				MIL.MdispControl( this._mDisp, MIL.M_SCALE_DISPLAY, MIL.M_ENABLE );

				// Select the image buffer on the display.
				MIL.MdispSelectWindow( this._mDisp, this._mBufferForDisp, winHandler );
				this.UserHookData.winHandler = IntPtr.Zero;
				this.UserHookData.winHandler = winHandler;
				this.isWpfDisplay = false;

				// Initialize the overlay.  It won't be visible by default
				//InitializeOverlay();
			}
			catch ( Exception ex ) { return string.Format( "SetDisplayHost error : " + ex.Message ); }
			return string.Empty;
		}

		public string SetDisplayWinHndOff( IntPtr winHandler )
		{
			try
			{
				// Select the image buffer on the display.
				MIL.MdispSelectWindow( this._mDisp, MIL.M_NULL, winHandler );

				this.isWpfDisplay = false;
			}
			catch ( Exception ex ) { return string.Format( "SetDisplayWinHndOff error : " + ex.Message ); }
			return string.Empty;
		}

		public string SetDisplayWinHndOff()
		{
			try
			{
				// Select the image buffer on the display.
				MIL.MdispSelectWindow( this._mDisp, MIL.M_NULL, MIL_WINDOW_HANDLE.Zero );
			}
			catch ( Exception ex ) { return string.Format( "SetDisplayWinHndOff error : " + ex.Message ); }
			return string.Empty;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Sets the <see cref="WindowsFormsHost"/> control that will be used to host the MIL display.
		/// </summary>
		/// <param name="displayHost">A <see cref="WindowsFormsHost"/> control on which to display an image buffer.</param>
		/// <remarks>This method will select the image buffer to display in the <see cref="WindowsFormsHost"/> specified by <paramref name="displayHost"/>.</remarks>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="displayHost"/> is null.</exception>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string SetDisplayWPFHost( WindowsFormsHost displayHost )
		{
			try
			{
				if ( displayHost == null )
					throw new ArgumentNullException( "displayHost", "Must be a valid object." );

				// Set Display to scaled mode
				MIL.MdispControl( this._mDisp, MIL.M_SCALE_DISPLAY, MIL.M_ENABLE );

				// Select the image buffer on the display.
				//MIL.MdispSelectWPF( this._mDisp, this._mBufferForDisp, displayHost );
				MIL.MdispSelectWPF( this._mDisp, this._mBuffer, displayHost );
				this.isWpfDisplay = true;
				// Initialize the overlay.  It won't be visible by default
				//InitializeOverlay();
			}
			catch ( Exception ex ) { return string.Format( "SetDisplayHost error : " + ex.Message ); }
			return string.Empty;
		}

		public string SetDisplayWPFOff()
		{
			try
			{
				// Select the image buffer on the display.
				MIL.MdispSelectWPF( this._mDisp, MIL.M_NULL, null );

				this.isWpfDisplay = false;
			}
			catch ( Exception ex ) { return string.Format( "SetDisplayWPFOff error : " + ex.Message ); }
			return string.Empty;
		}

		int laserLabel = -1;
		int laserCoorX = 0;
		int laserCoorY = 0;
		int laserColor = MIL.M_COLOR_RED;
		public string XHairDraw( bool Enabled, int color, int? XPos = null, int? YPos = null )
		{
			string sErr = string.Empty;
			try
			{
				if ( this.laserLabel != -1 )
				{
					//int test = -1;
					//MIL.MgraInquireList( this._mgraphicList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_LAST_LABEL + MIL.M_TYPE_MIL_INT32, ref test );
					MIL.MgraControlList( this._mgraphicList, MIL.M_GRAPHIC_LABEL( this.laserLabel ), MIL.M_DEFAULT, MIL.M_DELETE, MIL.M_DEFAULT );
					this.laserLabel = -1;
				}
				if ( Enabled )
				{
					var xpos = ( int )( this.CameraWidth / 2 );
					var ypos = ( int )( this.CameraHeight / 2 );

					if ( XPos != null )
						xpos += ( int )XPos;
					if ( YPos != null )
						ypos += ( int )YPos;

					this.laserCoorX = ( int )xpos;
					this.laserCoorY = ( int )ypos;

					double[] x = { -100 + this.laserCoorX, 0 + this.laserCoorX };
					double[] y = { 0 + this.laserCoorY, -100 + this.laserCoorY };
					double[] x2 = { 100 + this.laserCoorX, 0 + this.laserCoorX };
					double[] y2 = { 0 + this.laserCoorY, 100 + this.laserCoorY };

					MIL.MgraColor( this._mGraphicLinesLaserspot, color );
					MIL.MgraLines( this._mGraphicLinesLaserspot, this._mgraphicList, 2, x, y, x2, y2, MIL.M_INFINITE_LINES );
					MIL.MgraInquireList( this._mgraphicList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_LAST_LABEL + MIL.M_TYPE_MIL_INT32, ref this.laserLabel );
					MIL.MgraControlList( this._mgraphicList, MIL.M_GRAPHIC_LABEL( this.laserLabel ), MIL.M_DEFAULT, MIL.M_SELECTABLE, MIL.M_DISABLE );
				}

				if ( sErr.Length > 0 ) throw new Exception( sErr );

			}
			catch ( Exception ex )
			{
				sErr = string.Format( "LaserSpotDraw error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string ShowOverlayCrossCenterDisplay( bool Enabled, System.Drawing.Color PenColor, float PenSize, int? XPos = null, int? YPos = null )
		{
			string sErr = string.Empty;
			try
			{
				MIL_INT width = 0;
				MIL_INT height = 0;
				// double AnnotationColor = MIL.M_COLOR_GREEN;

				IntPtr hCustomDC = IntPtr.Zero;

				if ( this._mDisp == MIL.M_NULL )
				{ throw new Exception( "No display allocated" ); }

				if ( this._mBuffer == MIL.M_NULL )
				{ throw new Exception( "No buffer allocated" ); }

				if ( Enabled )
				{
					//if (_mOverlayDisp == MIL.M_NULL)
					//{
					//    MIL.MgraAllocList(_mSys, MIL.M_DEFAULT, ref _mOverlayDisp);

					//}

					//MIL.MdispControl(_mDisp, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, _mOverlayDisp);

					//MIL.MbufInquire(_mBuffer, MIL.M_SIZE_X, ref width);
					//MIL.MbufInquire(_mBuffer, MIL.M_SIZE_Y, ref height);

					//int lineX = Convert.ToInt16(width / 2);
					//int lineY = Convert.ToInt16(height / 2);

					//MIL.MgraColor(MIL.M_DEFAULT, AnnotationColor);
					//MIL.MgraLine(MIL.M_DEFAULT, _mOverlayDisp, lineX, 0, lineX, height);
					//MIL.MgraLine(MIL.M_DEFAULT, _mOverlayDisp, 0, lineY, width, lineY);

					MIL.MdispControl( this._mDisp, MIL.M_OVERLAY, MIL.M_ENABLE );

					// Inquire the overlay buffer associated with the display.
					MIL.MdispInquire( this._mDisp, MIL.M_OVERLAY_ID, ref this._mOverlayDisp );

					// Clear the overlay to transparent.
					MIL.MdispControl( this._mDisp, MIL.M_OVERLAY_CLEAR, MIL.M_DEFAULT );

					// Disable the overlay display update to accelerate annotations.
					MIL.MdispControl( this._mDisp, MIL.M_OVERLAY_SHOW, MIL.M_DISABLE );

					// Inquire overlay size.
					width = MIL.MbufInquire( this._mOverlayDisp, MIL.M_SIZE_X, MIL.M_NULL );
					height = MIL.MbufInquire( this._mOverlayDisp, MIL.M_SIZE_Y, MIL.M_NULL );

					MIL.MgraControl( MIL.M_DEFAULT, MIL.M_BACKGROUND_MODE, MIL.M_TRANSPARENT );

					// Re-enable the overlay display after all annotations are done.
					MIL.MdispControl( this._mDisp, MIL.M_OVERLAY_SHOW, MIL.M_ENABLE );

					// Draw GDI color overlay annotation.
					//***********************************

					// The inquire might not be supported
					//MIL.MappControl(MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_PRINT_DISABLE);

					// Create a device context to draw in the overlay buffer with GDI.
					MIL.MbufControl( this._mOverlayDisp, MIL.M_DC_ALLOC, MIL.M_DEFAULT );

					// Inquire the device context.
					hCustomDC = ( IntPtr )MIL.MbufInquire( this._mOverlayDisp, MIL.M_DC_HANDLE, MIL.M_NULL );

					MIL.MappControl( MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_PRINT_ENABLE );

					// Perform operation if GDI drawing is supported.
					if ( !hCustomDC.Equals( IntPtr.Zero ) )
					{
						// NOTE : The using blocks will automatically call the Dipose method on the GDI objects.
						//        This ensures that resources are freed even if an exception occurs.

						// Create a System.Drawing.Graphics object from the Device context
						using ( Graphics DrawingGraphics = Graphics.FromHdc( hCustomDC ) )
						{
							// Draw a blue cross.
							using ( System.Drawing.Pen DrawingPen = new System.Drawing.Pen( PenColor ) )
							{
								DrawingPen.Width = PenSize;

								// Draw a blue cross in the overlay image

								var xpos = ( int )( width / 2 );
								var ypos = ( int )( height / 2 );

								if ( XPos != null )
									xpos = ( int )XPos;
								if ( YPos != null )
									ypos = ( int )YPos;
								DrawingGraphics.DrawLine( DrawingPen, 0, ypos, width, ypos );
								DrawingGraphics.DrawLine( DrawingPen, xpos, 0, xpos, height );

							}
						}

						//   // Delete device context.
						MIL.MbufControl( this._mOverlayDisp, MIL.M_DC_FREE, MIL.M_DEFAULT );

						//   // Signal MIL that the overlay buffer was modified.
						MIL.MbufControl( this._mOverlayDisp, MIL.M_MODIFIED, MIL.M_DEFAULT );
					}
				}
				else
				{
					//MIL.MdispControl(_mDisp, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, MIL.M_NULL);

					//if (_mOverlayDisp != MIL.M_NULL)
					//{
					//    MIL.MgraFree(_mOverlayDisp);
					//    _mOverlayDisp = MIL.M_NULL;
					//}

					MIL.MdispControl( this._mDisp, MIL.M_OVERLAY, MIL.M_DISABLE );
				}

			}
			catch ( Exception ex ) { return string.Format( "ShowOverlayCrossCenterDisplay error : " + ex.Message ); }
			return sErr;
		}
		bool bFirsttimeZoom = true;
		double defaultZoomFactorX = 0;
		double defaultZoomFactorY = 0;
		public string DisplayZoom( double ratio )
		{
			string sErr = string.Empty;
			try
			{
				if ( this._mDisp == MIL.M_NULL ) throw new Exception( "Display is not initialized" );
				else if ( this._mBuffer == MIL.M_NULL ) throw new Exception( "Image not available" );
				else if ( ratio <= 0 ) throw new Exception( "Invalid parameter ratio" );

				if ( this.bFirsttimeZoom )
				{
					MIL.MdispInquire( this._mDisp, MIL.M_ZOOM_FACTOR_X, ref this.defaultZoomFactorX );
					MIL.MdispInquire( this._mDisp, MIL.M_ZOOM_FACTOR_Y, ref this.defaultZoomFactorY );
					this.bFirsttimeZoom = false;
					//defaultZoomFactorX = 0.25;
					//defaultZoomFactorY = 0.25;
					//defaultZoomFactorX = 0.13;
					//defaultZoomFactorY = 0.13;
				}
				//defaultZoomFactorX = 1;
				//defaultZoomFactorY = 1;
				double factorX = ratio * this.defaultZoomFactorX;
				double factorY = ratio * this.defaultZoomFactorY;
				MIL.MdispZoom( this._mDisp, factorX, factorY );
				//MIL.MdispControl( _mDisp, MIL.M_CENTER_DISPLAY, MIL.M_ENABLE );

				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "TrimmerVisionInterface - DisplayZoom error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}

		public string ShowBlueCrossCoordinate( int CoordinateX, int CoordinateY )
		{
			string sErr = string.Empty;
			try
			{
				MIL_INT width = 0;
				MIL_INT height = 0;
				// double AnnotationColor = MIL.M_COLOR_GREEN;

				IntPtr hCustomDC = IntPtr.Zero;

				if ( this._mDisp == MIL.M_NULL )
				{ throw new Exception( "No display allocated" ); }

				if ( this._mBuffer == MIL.M_NULL )
				{ throw new Exception( "No buffer allocated" ); }

				if ( true )
				{
					MIL.MdispControl( this._mDisp, MIL.M_OVERLAY, MIL.M_ENABLE );

					//if (_mOverlayDisp == MIL.M_NULL)
					{
						// Inquire the overlay buffer associated with the display.
						MIL.MdispInquire( this._mDisp, MIL.M_OVERLAY_ID, ref this._mOverlayDisp );

						// Clear the overlay to transparent.
						MIL.MdispControl( this._mDisp, MIL.M_OVERLAY_CLEAR, MIL.M_DEFAULT );

						// Disable the overlay display update to accelerate annotations.
						MIL.MdispControl( this._mDisp, MIL.M_OVERLAY_SHOW, MIL.M_DISABLE );

						// Inquire overlay size.
						width = MIL.MbufInquire( this._mOverlayDisp, MIL.M_SIZE_X, MIL.M_NULL );
						height = MIL.MbufInquire( this._mOverlayDisp, MIL.M_SIZE_Y, MIL.M_NULL );

						MIL.MgraControl( MIL.M_DEFAULT, MIL.M_BACKGROUND_MODE, MIL.M_TRANSPARENT );

						// Re-enable the overlay display after all annotations are done.
						MIL.MdispControl( this._mDisp, MIL.M_OVERLAY_SHOW, MIL.M_ENABLE );
					}

					MIL_INT dispW;

					dispW = MIL.MdispInquire( this._mDisp, MIL.M_SIZE_X, MIL.M_NULL );

					double scaled = dispW == 0 ? 1 : ( double )width / dispW;

					// Draw GDI color overlay annotation.
					//***********************************

					// The inquire might not be supported
					MIL.MappControl( MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_PRINT_DISABLE );

					// Create a device context to draw in the overlay buffer with GDI.
					MIL.MbufControl( this._mOverlayDisp, MIL.M_DC_ALLOC, MIL.M_DEFAULT );

					// Inquire the device context.
					hCustomDC = ( IntPtr )MIL.MbufInquire( this._mOverlayDisp, MIL.M_DC_HANDLE, MIL.M_NULL );

					MIL.MappControl( MIL.M_DEFAULT, MIL.M_ERROR, MIL.M_PRINT_ENABLE );

					// Perform operation if GDI drawing is supported.
					if ( !hCustomDC.Equals( IntPtr.Zero ) )
					{
						// NOTE : The using blocks will automatically call the Dipose method on the GDI objects.
						//        This ensures that resources are freed even if an exception occurs.

						// Create a System.Drawing.Graphics object from the Device context
						using ( Graphics DrawingGraphics = Graphics.FromHdc( hCustomDC ) )
						{
							// Draw a blue cross.
							using ( System.Drawing.Pen DrawingPen = new System.Drawing.Pen( System.Drawing.Color.Blue ) )
							{
								DrawingPen.Width = ( float )scaled;

								int len = Convert.ToInt16( width * 0.01 );

								if ( len < 5 ) len = 5;

								// Draw a blue cross in the overlay image
								DrawingGraphics.DrawLine( DrawingPen, CoordinateX - len, CoordinateY, CoordinateX + len, CoordinateY );
								DrawingGraphics.DrawLine( DrawingPen, CoordinateX, CoordinateY - len, CoordinateX, CoordinateY + len );

							}
						}

						//   // Delete device context.
						MIL.MbufControl( this._mOverlayDisp, MIL.M_DC_FREE, MIL.M_DEFAULT );

						//   // Signal MIL that the overlay buffer was modified.
						MIL.MbufControl( this._mOverlayDisp, MIL.M_MODIFIED, MIL.M_DEFAULT );
					}
				}
			}
			catch ( Exception ex ) { return string.Format( "ShowOverlayCrossCenterDisplay error : " + ex.Message ); }
			return sErr;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Disconnect camera and free some resources
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string DisconnectCamera()
		{
			try
			{
				if ( this.isInContinuousMode )
				{
					this.HaltContinuous();
				}
				if ( this.isInProcessMode )
				{
					this.StopGrabProcess();
				}

				// Free allocated objects.
				if ( this._mBuffer != MIL.M_NULL )
				{
					MIL.MbufFree( this._mBuffer );
					this._mBuffer = MIL.M_NULL;
				}

				if ( this._mCamera != MIL.M_NULL )
				{
					MIL.MdigFree( this._mCamera );
					this._mCamera = MIL.M_NULL;
					this._settings = null;
				}

			}
			catch ( Exception ex ) { return string.Format( "DisconnectCamera error: " + ex.Message ); }
			return string.Empty;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Starts the continuous grab mode for camera
		/// </summary>
		/// <remarks>
		/// This method is called from the StartGrab_Click method of the main window when the 
		/// user clicks the Start Grab button in the UI.
		/// </remarks>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string GrabContinuous()
		{
			try
			{
				if ( this._mCamera == MIL.M_NULL )
				{ return string.Format( "No camera connected" ); }

				if ( !this.isInContinuousMode || !this.isInProcessMode )
				{
					MIL.MdigGrabContinuous( this._mCamera, this._mBuffer );

					this.isInContinuousMode = true;
				}
				else { return string.Format( "Camera is already grabbing in continuous" ); }
			}
			catch ( Exception ex ) { return string.Format( "GrabContinuous error: " + ex.Message ); }
			return string.Empty;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Stops the grab on the camera. 
		/// </summary>
		/// <remarks>
		/// This method is called from the StopGrab_Click method of the main window when the 
		/// user clicks the Stop Grab button in the UI.
		/// This function will wait for the end of the current frame before returning, to ensure the last frame is always valid
		/// </remarks>
		/////////////////////////////////////////////////////////////////////////////////////////////// 
		public string HaltContinuous()
		{
			try
			{
				if ( this._mCamera == MIL.M_NULL )
				{ return string.Format( "No camera connected" ); }

				if ( !this.isInContinuousMode )
				{ return string.Format( "Camera not in conitnuous mode" ); }

				//This function will wait for the end of the current frame before returning, to ensure the last frame is always valid
				//Hence in trigger mode, it will wait for last trigger before returning
				MIL.MdigHalt( this._mCamera );

				this.isInContinuousMode = false;
			}
			catch ( Exception ex ) { return string.Format( "HaltContinuous error: " + ex.Message ); }
			return string.Empty;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Starts a single grab mode for camera
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string GrabSingle()
		{
			MIL_ID rotateBuf = MIL.M_NULL;

			try
			{
				if ( this._mCamera == MIL.M_NULL && !MachineStateMng.isSimulation )
				{ return string.Format( "No camera connected" ); }

				if ( !MachineStateMng.isSimulation )
				{
					if ( this.isInContinuousMode || this.isInProcessMode ) return string.Format( "Camera in continuous mode" );
					rotateBuf = MIL.MbufClone( this._mBuffer, this._mSys, MIL.M_DEFAULT, MIL.M_DEFAULT,
								MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_NULL );
					MIL.MdigGrab( this._mCamera, rotateBuf );
					MIL.MimRotate( rotateBuf, this._mBuffer, this.UserHookData.BufferRotDegree, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_BILINEAR );
				}
				else
				{
					var IMAGE_FILE = "C:\\grid.bmp";
					MIL.MbufImport( IMAGE_FILE, MIL.M_DEFAULT, MIL.M_RESTORE + MIL.M_NO_GRAB + MIL.M_NO_COMPRESS, this.MILSystem, ref this._mBuffer );
				}


				var width = 0;
				var height = 0;
				MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_X, ref width );
				MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_Y, ref height );

				if ( !MachineStateMng.isSimulation )
				{
					if ( this.Settings.PixelFormat == "Mono8" )
					{
						byte[] bMatrixArray = new byte[ width * height ];

						MIL.MbufGet2d(
							this._mBuffer,
							0,
							0,
							width,
							height,
							bMatrixArray
							);
						MIL.MbufPut( this._mBufferForDisp, bMatrixArray );
					}
				}
				else
				{
					byte[] bMatrixArray = new byte[ width * height ];

					MIL.MbufGet2d(
						this._mBuffer,
						0,
						0,
						width,
						height,
						bMatrixArray
						);
					MIL.MbufPut( this._mBufferForDisp, bMatrixArray );
				}
				//MIL.MbufFree( rotateBuf );
			}
			catch ( Exception ex )
			{ return string.Format( "GrabSingle error: " + ex.Message ); }
			finally
			{
				if ( rotateBuf != MIL.M_NULL ) MIL.MbufFree( rotateBuf );
			}
			return string.Empty;
		}

		public string ExecuteSoftwareTrigger()
		{
			try
			{
				if ( this._mCamera == MIL.M_NULL )
				{ return string.Format( "No camera connected" ); }

				MIL.MdigControlFeature( this._mCamera, MIL.M_FEATURE_VALUE, "TriggerSelector", MIL.M_TYPE_STRING, "FrameStart" );
				MIL.MdigControlFeature( this._mCamera, MIL.M_FEATURE_EXECUTE, "TriggerSoftware", MIL.M_DEFAULT, MIL.M_NULL );
			}
			catch ( Exception ex ) { return string.Format( "ExecuteSoftwareTrigger error: " + ex.Message ); }
			return string.Empty;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Starts the continuous grab process with HookFunction
		/// </summary>
		/// <remarks>
		/// This method enabled hookfunction to be triggerred when new image arrived.
		/// </remarks>
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string StartGrabProcess()
		{
			try
			{
				string sErr = "";

				if ( this._mCamera == MIL.M_NULL )
				{ return string.Format( "No camera connected" ); }

				if ( !this.isInContinuousMode || !this.isInProcessMode )
				{
					sErr = this.GetUpdatePixelFormat();
					if ( sErr.Length > 0 ) { }

					// Start the processing. The processing function is called with every frame grabbed.
					MIL.MdigProcess( this._mCamera, this.MilGrabBufferList, this.MilGrabBufferListSize, MIL.M_START, MIL.M_DEFAULT, this.ProcessingFunctionPtr, GCHandle.ToIntPtr( this.hUserData ) );
					this.isInContinuousMode = true;
					this.isInProcessMode = true;
				}
				else { return string.Format( "Camera is already grabbing in continuous" ); }
			}
			catch ( Exception ex ) { return string.Format( "GrabContinuous error: " + ex.Message ); }
			return string.Empty;
		}


		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Stops the StartGrabProcess(). Removed the hookfunction call
		/// </summary>
		/// <remarks>
		/// This method removed the hookfunction calls
		/// </remarks>
		/////////////////////////////////////////////////////////////////////////////////////////////// 
		public string StopGrabProcess()
		{
			try
			{
				if ( this._mCamera == MIL.M_NULL )
				{ return string.Format( "No camera connected" ); }

				if ( !this.isInProcessMode )
				{ return string.Format( "Camera not in grab process" ); }

				// Stop the processing.
				MIL.MdigProcess( this._mCamera, this.MilGrabBufferList, this.MilGrabBufferListSize, MIL.M_STOP, MIL.M_DEFAULT, this.ProcessingFunctionPtr, GCHandle.ToIntPtr( this.hUserData ) );
				this.isInContinuousMode = false;
				this.isInProcessMode = false;
			}
			catch ( Exception ex ) { return string.Format( "HaltContinuous error: " + ex.Message ); }
			return string.Empty;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Check status of the camera
		/// </summary>
		/// <param name="IsConnected">A boolean, passed by reference, 
		/// that return the status true if camera is connected or Alive.</param>        
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string IsCameraConnected( ref bool IsConnected )
		{
			try
			{
				IsConnected = false;

				if ( this._mCamera == MIL.M_NULL ) return string.Empty;

				MIL_INT exposure = 0;
				MIL.MdigInquire( this._mCamera, MIL.M_EXPOSURE_TIME, ref exposure );

				if ( exposure > 0 ) IsConnected = true;
			}
			catch { return string.Format( "this error" ); }
			return string.Empty;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Reconnect the camera by Disconnect and then Connect again
		/// </summary>      
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string ReconnectCamera()
		{
			try
			{
				this.DisconnectCamera();
				//ConnectCamera();
			}
			catch { return string.Format( "ReconnectCamera error" ); }
			return string.Empty;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Save the image buffer in external file 
		/// </summary>
		/// <param name="Filename">A string, filename of the external file.</param>       
		/// <param name="fileFormat">A Camera.FileFormat, format of the saved file(BMP,TIFF,...</param> 
		///////////////////////////////////////////////////////////////////////////////////////////////
		public string Save( string Filename, FileFormat fileFormat )
		{
			try
			{
				if ( this._mBuffer == MIL.M_NULL ) return string.Format( "Buffer is null" );

				MIL.MbufExport( Filename, ( long )fileFormat, this._mBuffer );
			}
			catch ( Exception ex ) { return string.Format( "Save error: " + ex.Message ); }
			return string.Empty;
		}

		public string Load2DImageFromFile( string Filename )
		{
			try
			{

				// Release the buffer before allocation
				if ( this._mBuffer != MIL.M_NULL )
				{
					MIL.MbufFree( this._mBuffer );
					this._mBuffer = MIL.M_NULL;
				}

				MIL.MbufAlloc2d( this._mSys,
					MIL.MbufDiskInquire( Filename, MIL.M_SIZE_X ),
					MIL.MbufDiskInquire( Filename, MIL.M_SIZE_Y ),
					8 + MIL.M_UNSIGNED,
					MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
					ref this._mBuffer );

				MIL_INT sizeBand = MIL.M_NULL;

				MIL.MbufDiskInquire( Filename, MIL.M_SIZE_BAND, ref sizeBand );

				if ( sizeBand > 1 )
				{
					MIL_ID tempColor = MIL.M_NULL;

					MIL.MbufAllocColor( this._mSys,
						sizeBand,
						MIL.MbufDiskInquire( Filename, MIL.M_SIZE_X ),
						MIL.MbufDiskInquire( Filename, MIL.M_SIZE_Y ),
						MIL.MbufDiskInquire( Filename, MIL.M_TYPE ),
						MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
						ref tempColor );

					MIL.MbufLoad( Filename, tempColor );

					MIL.MimConvert( tempColor, this._mBuffer, MIL.M_RGB_TO_L );

					MIL.MbufFree( tempColor );
				}
				else
				{
					MIL.MbufLoad( Filename, this._mBuffer );
				}
			}
			catch ( Exception ex ) { return string.Format( "Load2DImageFromFile error: " + ex.Message ); }
			return string.Empty;
		}

		public string ImportRAWImageFromFile( string Filename, int Width, int Height, int DataDepth )
		{
			try
			{
				// Release the buffer before allocation
				if ( this._mBuffer != MIL.M_NULL )
				{
					MIL.MbufFree( this._mBuffer );
					this._mBuffer = MIL.M_NULL;
				}

				MIL.MbufAlloc2d( this._mSys,
					Width,
					Height,
					DataDepth + MIL.M_UNSIGNED,
					MIL.M_IMAGE + MIL.M_DISP + MIL.M_PROC,
					ref this._mBuffer );

				MIL_INT format = MIL.MbufDiskInquire( Filename, MIL.M_FILE_FORMAT );

				MIL.MbufImport( Filename, format, MIL.M_LOAD, this._mSys, ref this._mBuffer );

			}
			catch ( Exception ex ) { return string.Format( "ImportImageFromFile error: " + ex.Message ); }
			return string.Empty;
		}

		public string Get1DArray( ref byte[] ResultArray )
		{
			string sErr = string.Empty;
			try
			{
				// Release the buffer before allocation
				if ( this._mBuffer == MIL.M_NULL )
				{
					throw new Exception( "No buffer available" );
				}

				var w = MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_X );
				var h = MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_Y );
				ResultArray = new byte[ ( int )h * ( int )w ];

				MIL.MbufGet( this._mBuffer, ResultArray );

			}
			catch ( Exception ex )
			{ sErr = string.Format( "Get1DArray error: " + ex.Message ); }
			return sErr;
		}

		public string Get1DArray( ref short[] ResultArray )
		{
			string sErr = string.Empty;
			try
			{
				// Release the buffer before allocation
				if ( this._mBuffer == MIL.M_NULL )
				{
					throw new Exception( "No buffer available" );
				}

				var w = MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_X );
				var h = MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_Y );
				ResultArray = new short[ ( int )h * ( int )w ];

				MIL.MbufGet( this._mBuffer, ResultArray );

			}
			catch ( Exception ex )
			{ sErr = string.Format( "Get1DArray error: " + ex.Message ); }
			return sErr;
		}

		public string Get2DArray( ref byte[,] ResultArray )
		{
			string sErr = string.Empty;
			try
			{
				// Release the buffer before allocation
				if ( this._mBuffer == MIL.M_NULL )
				{
					throw new Exception( "No buffer available" );
				}

				var w = MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_X );
				var h = MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_Y );
				ResultArray = new byte[ ( int )h, ( int )w ];

				MIL.MbufGet( this._mBuffer, ResultArray );

			}
			catch ( Exception ex )
			{ sErr = string.Format( "Get2DArray error: " + ex.Message ); }
			return sErr;
		}

		public string Get2DArray( ref short[,] ResultArray )
		{
			string sErr = string.Empty;
			try
			{
				// Release the buffer before allocation
				if ( this._mBuffer == MIL.M_NULL )
				{
					throw new Exception( "No buffer available" );
				}

				var w = MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_X );
				var h = MIL.MbufInquire( this._mBuffer, MIL.M_SIZE_Y );
				ResultArray = new short[ ( int )h, ( int )w ];

				MIL.MbufGet( this._mBuffer, ResultArray );

			}
			catch ( Exception ex )
			{ sErr = string.Format( "Get2DArray error: " + ex.Message ); }
			return sErr;
		}

		public CameraSettings Settings
		{
			get
			{
				try
				{
					if ( this._settings == null ) this._settings = new CameraSettings( this._mCamera );
					return this._settings;
				}
				catch { return null; }
			}
		}

		public MIL_ID MILSystem
		{
			get { return this._mSys; }
		}

		public MIL_ID MILImage
		{
			get { return this._mBuffer; }
		}

		public MIL_ID MILDigitizer
		{
			get { return this._mCamera; }
		}

		public MIL_ID NULL_ID
		{ get { return MIL.M_NULL; } }


		public void BufferFlip()
		{
			try
			{
				if ( this._mBuffer != MIL.M_NULL )
				{ }
				//MIL.MimFlip()
			}
			catch { }
		}


		// User's processing function called every time a grab buffer is ready.
		// -----------------------------------------------------------------------

		// Local defines.
		private const int STRING_LENGTH_MAX = 20;
		private const int STRING_POS_X = 20;
		private const int STRING_POS_Y = 20;
		private int counter = 0;
		private MIL_INT ProcessingFunction( MIL_INT HookType, MIL_ID HookId, IntPtr HookDataPtr )
		{
			//	MIL_ID ModifiedBufferId = MIL.M_NULL;
			MIL_ID OriBuff = MIL.M_NULL;

			MIL_ID rotateBuf = MIL.M_NULL;
			try
			{
				if ( this.counter++ == 100 )
				{
					this.counter = 0;
					GC.Collect();
				}
				// this is how to check if the user data is null, the IntPtr class
				// contains a member, Zero, which exists solely for this purpose
				if ( !IntPtr.Zero.Equals( HookDataPtr ) )
				{
					// get the handle to the DigHookUserData object back from the IntPtr
					GCHandle hUserData = GCHandle.FromIntPtr( HookDataPtr );

					// get a reference to the DigHookUserData object
					HookDataStruct UserData = hUserData.Target as HookDataStruct;

					// Retrieve the MIL_ID of the grabbed buffer.
					MIL.MdigGetHookInfo( HookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref OriBuff );
					//Rotate the grabbed buffer.

					// Increment the frame counter.
					UserData.ProcessedImageCount++;

					if ( OriBuff != MIL.M_NULL )
					{
						rotateBuf = MIL.MbufClone( OriBuff, this._mSys, MIL.M_DEFAULT, MIL.M_DEFAULT,
							MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_NULL );
						MIL.MimRotate( OriBuff, rotateBuf, UserData.BufferRotDegree, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_BILINEAR );
						this.eventArgs.ProcessedImageCount = UserData.ProcessedImageCount;

						int width = 0;
						int height = 0;
						MIL.MbufInquire( OriBuff, MIL.M_SIZE_X, ref width );
						MIL.MbufInquire( OriBuff, MIL.M_SIZE_Y, ref height );

						this.eventArgs.Width = width;
						this.eventArgs.Height = height;

						if ( UserData.PixelFormat == "Mono8" )
						{
							this.eventArgs.bMatrixArray = new byte[ width * height ];

							MIL.MbufGet2d(
								rotateBuf,
								0,
								0,
								width,
								height,
								this.eventArgs.bMatrixArray
								);
							MIL.MbufPut( this._mBufferForDisp, this.eventArgs.bMatrixArray );

						}
						else
						{
							this.eventArgs.MatrixArray = new short[ width * height ];

							MIL.MbufGet2d(
								OriBuff,
								0,
								0,
								width,
								height,
								this.eventArgs.MatrixArray
								);
						}

						//MIL_INT bmpPtr = MIL.M_NULL;
						//MIL.MbufInquire(ModifiedBufferId, MIL.M_BITMAPINFO, ref bmpPtr);

						//eventArgs.Image = new Bitmap(width, height);
						// eventArgs.Image = Image.FromHbitmap(bmpPtr);

						//Call delegate
						this.OnNewImageActivate( this.eventArgs );
						MIL.MbufCopy( rotateBuf, UserData.MilImageDisp );

					}

					// Update the display

					//if ( _mBufferForDisp != MIL.M_NULL )
					//{
					//    MIL.MbufFree ( _mBufferForDisp );
					//    _mBufferForDisp = MIL.M_NULL;
					//    MIL.MbufClone ( OriBuff , _mSys , MIL.M_DEFAULT , MIL.M_DEFAULT ,MIL.M_DEFAULT, MIL.M_DEFAULT ,MIL.M_COPY_SOURCE_DATA , ref _mBufferForDisp );
					//    if(UserHookData.winHost!=null)
					//    {
					//        MIL.MdispSelectWPF ( _mDisp , _mBufferForDisp , UserData.winHost );
					//    }
					//    if(UserHookData.winHandler!=IntPtr.Zero)
					//    {
					//        MIL.MdispSelectWindow ( _mDisp , _mBufferForDisp , UserHookData.winHandler );
					//    }

					//}

					// MIL.MbufCopy ( OriBuff , _mBufferForDisp );
					//MIL.MbufFree( OriBuff );

				}
			}
			catch ( Exception ex ) { JPTUtility.Logger.doLog( "ProcessingFunction error: " + ex.Message ); }
			finally
			{
				if ( rotateBuf != MIL.M_NULL ) MIL.MbufFree( rotateBuf );
			}
			return 0;
		}

		public class NewDataEventArg : EventArgs
		{
			public byte[] bMatrixArray { get; set; }
			public short[] MatrixArray { get; set; }
			public int Width { get; set; }
			public int Height { get; set; }
			public Bitmap Image { get; set; }
			public int ProcessedImageCount { get; set; }
			public bool IsDisplayAvailable { get; set; }
		}

		NewDataEventArg eventArgs = new NewDataEventArg();

		public delegate void OnNewImageHandler( object sender, NewDataEventArg e );

		public event OnNewImageHandler OnImageReceived;

		protected virtual void OnNewImageReceived( NewDataEventArg e )
		{
			if ( OnImageReceived != null ) OnImageReceived( this, e );
		}

		private void OnNewImageActivate( NewDataEventArg e )
		{
			this.OnNewImageReceived( e );
		}

		private string GetUpdatePixelFormat()
		{
			try
			{
				this.UserHookData.PixelFormat = this.Settings.PixelFormat;
			}
			catch ( Exception ex ) { return string.Format( "GetUpdatePixelFormat error: " + ex.Message ); }
			return string.Empty;
		}

		public System.Drawing.Image CreateBitmapFromMILBuffer( MIL_ID buffer )
		{
			int sizeX = 0, sizeY = 0, stride = 0, numBands = 0;
			MIL_INT DataFormat = 0;
			MIL.MbufInquire( buffer, MIL.M_SIZE_X, ref sizeX );
			MIL.MbufInquire( buffer, MIL.M_SIZE_Y, ref sizeY );
			MIL.MbufInquire( buffer, MIL.M_PITCH_BYTE, ref stride );
			MIL.MbufInquire( buffer, MIL.M_SIZE_BAND, ref numBands );
			MIL.MbufInquire( buffer, MIL.M_DATA_FORMAT, ref DataFormat );
			System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
			if ( numBands >= 3 )
			{
				if ( DataFormat == MIL.M_BGR32 )
					format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
				else //if (DataFormat == MIL.M_BGR24_PACKED)
					format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
			}

			//Here create the Bitmap with the known height, width and format
			//PixelFormat format = (numBands == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format24bppRgb);
			//PixelFormat format = (numBands == 1 ? PixelFormat.Format8bppIndexed : PixelFormat.Format32bppRgb);
			try
			{
				Bitmap tempBitmap = new Bitmap( sizeX, sizeY, format );
				BitmapData data = tempBitmap.LockBits( new Rectangle( 0, 0, sizeX, sizeY ), ImageLockMode.WriteOnly, format );
				IntPtr ptr = ( IntPtr )MIL.MbufInquire( buffer, MIL.M_HOST_ADDRESS, MIL.M_NULL );
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
					System.Drawing.Imaging.ColorPalette palette = tempBitmap.Palette;
					for ( int i = 0; i < 256; i++ )
					{
						palette.Entries[ i ] = System.Drawing.Color.FromArgb( i, i, i );
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
		MIL_INT GraphicHookHandler( MIL_INT HookType, MIL_ID EventId, IntPtr UserDataPtr )
		{
			this.Cal.GetPosition();
			this.Inspect.GetPosition();
			return MIL.M_NULL;
		}
	}

	public class ROIHandler : BaseUtility
	{
		MIL_ID m_bufferROI = MIL.M_NULL;
		MIL_ID m_graphic = MIL.M_NULL;
		MIL_ID m_graphicList = MIL.M_NULL;
		MIL_ID _mSys = MIL.M_NULL;
		MIL_ID _mDisp = MIL.M_NULL;
		MIL_ID _mBuffer = MIL.M_NULL;

		public int roiX
		{
			get => this.GetValue( () => this.roiX );
			set
			{
				this.SetValue( () => this.roiX, value );
				this.maxW = this.ImgWidth - this.roiX - 1;
				this.BrushX = ( this.roiX < 0 || this.roiX > this.ImgWidth ) ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Transparent;
				this.BrushW = ( this.roiW <= 0 || this.roiW > this.ImgWidth - this.roiX ) ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Transparent;
				try
				{
					if ( !this.SkipUpdate )
						MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_POSITION_X, ( double )( this.roiX ) );
					if ( this.Config != null )
						this.Config.X = this.roiX;
				}
				catch ( Exception ex )
				{
					new Thread( () => MessageBox.Show( $"{ex.Message}" ) ).Start();
				}
			}
		}
		public int roiY
		{
			get => this.GetValue( () => this.roiY );
			set
			{
				this.SetValue( () => this.roiY, value );
				this.maxH = this.ImgHeight - this.roiY - 1;
				this.BrushY = ( this.roiY < 0 || this.roiY > this.ImgHeight ) ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Transparent;
				this.BrushH = ( this.roiH <= 0 || this.roiH > this.ImgHeight - this.roiY ) ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Transparent;
				try
				{
					if ( !this.SkipUpdate )
						MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_POSITION_Y, ( double )( this.roiY ) );
					if ( this.Config != null )
						this.Config.Y = this.roiY;
				}
				catch ( Exception ex )
				{
					new Thread( () => MessageBox.Show( $"{ex.Message}" ) ).Start();
				}
			}
		}
		public int roiW
		{
			get => this.GetValue( () => this.roiW );
			set
			{
				this.SetValue( () => this.roiW, value );
				this.BrushW = ( this.roiW <= 0 || this.roiW > this.ImgWidth - this.roiX ) ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Transparent;
				try
				{
					if ( !this.SkipUpdate )
						MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_RECTANGLE_WIDTH, ( double )this.roiW );
					if ( this.Config != null )
						this.Config.Width = this.roiW;
				}
				catch ( Exception ex )
				{
					new Thread( () => MessageBox.Show( $"{ex.Message}" ) ).Start();
				}
			}
		}
		public int roiH
		{
			get => this.GetValue( () => this.roiH );
			set
			{
				this.SetValue( () => this.roiH, value );
				this.BrushH = ( this.roiH <= 0 || this.roiH > this.ImgHeight - this.roiY ) ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Transparent;
				try
				{
					if ( !this.SkipUpdate )
						MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_RECTANGLE_HEIGHT, ( double )this.roiH );
					if ( this.Config != null )
						this.Config.Height = this.roiH;
				}
				catch ( Exception ex )
				{
					new Thread( () => MessageBox.Show( $"{ex.Message}" ) ).Start();
				}
			}
		}
		private ROIRectangle Config { get; set; }
		public int ImgWidth = 0, ImgHeight = 0;
		int roiLabel = -1;
		private bool UpdateInProgress = false;
		private bool SkipUpdate = false;
		public ROIHandler()
		{

		}
		public ROIHandler( MIL_ID _sys, MIL_ID _Disp, MIL_ID _Buffer, MIL_ID _graphicList, int Width, int Height, MIL_INT color )
		{
			string sErr = string.Empty;
			try
			{
				this._mSys = _sys;
				this._mDisp = _Disp;
				this._mBuffer = _Buffer;
				this.m_graphicList = _graphicList;
				this.ImgWidth = Width;
				this.ImgHeight = Height;
				this.minX = 0;
				this.maxX = this.ImgWidth - 1;
				this.minY = 0;
				this.maxY = this.ImgHeight - 1;
				this.minW = 1;
				this.minH = 1;
				// Alloc Graphics

				MIL.MgraAlloc( this._mSys, ref this.m_graphic );
				MIL.MdispControl( this._mDisp, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, this.m_graphicList );
				MIL.MdispControl( this._mDisp, MIL.M_GRAPHIC_LIST_INTERACTIVE, MIL.M_ENABLE );
				MIL.MgraRectAngle( this.m_graphic, this.m_graphicList, this.roiX, this.roiY, this.roiW, this.roiH, 0, MIL.M_CORNER_AND_DIMENSION );
				MIL.MgraInquireList( this.m_graphicList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_LAST_LABEL + MIL.M_TYPE_MIL_INT32, ref this.roiLabel );
				MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_ROTATABLE, MIL.M_DISABLE );
				MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_COLOR, color );
				MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_GRAPHIC_SELECTED, MIL.M_TRUE );
				MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_POSITION_TYPE, MIL.M_CORNER_AND_DIMENSION );
				//MIL.MgraControlList( m_graphicList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_INTERACTIVE_ANNOTATIONS_COLOR, color );
				//MIL.MgraControlList( m_graphicList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_SELECTED_COLOR, color );
				MIL.MgraControlList( this.m_graphicList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_MULTIPLE_SELECTION, MIL.M_DISABLE );
				this.ShowROI = this.ShowROI;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Create error: " + ex.Message );
			}
		}
		public string SetPosition( ROIRectangle source )
		{
			var sErr = string.Empty;
			try
			{
				if ( this.roiLabel != -1 )
				{
					this.Config = source;
					this.CheckAndThrowIfError( this.SetPosition( this.Config.X, this.Config.Y, this.Config.Width, this.Config.Height ) );
				}
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "SetPosition error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			finally
			{
				this.UpdateInProgress = false;
			}
			return sErr;
		}
		public string SetPosition( int X, int Y, int Width, int Height )
		{
			this.ClearErrorFlags();
			try
			{
				if ( this.roiLabel != -1 )
				{
					if ( this.Config == null ) this.CheckAndThrowIfError( "ROI Config is null" );
					this.UpdateInProgress = true;
					this.roiX = X;
					this.roiY = Y;
					this.roiW = Width;
					this.roiH = Height;
					this.UpdateInProgress = false;
					this.ShowROI = this.ShowROI;
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				JPTUtility.Logger.doLog( this.Result );
			}
			finally
			{
				this.UpdateInProgress = false;
			}
			return this.Result;
		}
		public string GetPosition()
		{
			this.ClearErrorFlags();
			try
			{
				if ( this.UpdateInProgress ) return string.Empty;
				if ( this.roiLabel != -1 )
				{
					int roiX, roiY, roiW, roiH;
					roiX = roiY = roiW = roiH = 0;
					MIL.MgraInquireList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_POSITION_X + MIL.M_TYPE_MIL_INT32, ref roiX );
					MIL.MgraInquireList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_POSITION_Y + MIL.M_TYPE_MIL_INT32, ref roiY );
					MIL.MgraInquireList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_RECTANGLE_WIDTH + MIL.M_TYPE_MIL_INT32, ref roiW );
					MIL.MgraInquireList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_RECTANGLE_HEIGHT + MIL.M_TYPE_MIL_INT32, ref roiH );
					this.SkipUpdate = true;
					this.roiX = roiX;
					this.roiY = roiY;
					this.roiW = roiW;
					this.roiH = roiH;
				}
			}
			catch ( Exception ex )
			{
				this.CatchException( ex );
				JPTUtility.Logger.doLog( this.Result );
			}
			finally
			{
				this.SkipUpdate = false;
			}
			return this.Result;
		}
		private bool b_ShowROI = false;
		public bool ShowROI
		{
			get => this.b_ShowROI;
			set
			{
				this.b_ShowROI = value;
				if ( this.b_ShowROI )
					this.Show();
				else
					this.Hide();
			}
		}
		private string Hide()
		{
			string sErr = string.Empty;
			try
			{
				if ( this.roiLabel != -1 ) MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_VISIBLE, MIL.M_FALSE );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Hide error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		private string Show()
		{
			string sErr = string.Empty;
			try
			{
				if ( this.roiLabel != -1 ) MIL.MgraControlList( this.m_graphicList, MIL.M_GRAPHIC_LABEL( this.roiLabel ), MIL.M_DEFAULT, MIL.M_VISIBLE, MIL.M_TRUE );
				if ( sErr.Length > 0 ) throw new Exception( sErr );
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "Show error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}
			return sErr;
		}
		public void Unlink()
		{
			try
			{
				this.Config = null;
			}
			catch ( Exception ex )
			{
				throw new Exception( "Unlink() error: " + ex.Message );
			}
		}

		public void ResetROI()
		{
			this.SetPosition( 0, 0, this.ImgWidth, this.ImgHeight );
		}

		public SolidColorBrush BrushX
		{
			get => this.GetValue( () => this.BrushX );
			set => this.SetValue( () => this.BrushX, value );
		}
		public SolidColorBrush BrushY
		{
			get => this.GetValue( () => this.BrushY );
			set => this.SetValue( () => this.BrushY, value );
		}
		public SolidColorBrush BrushW
		{
			get => this.GetValue( () => this.BrushW );
			set => this.SetValue( () => this.BrushW, value );
		}
		public SolidColorBrush BrushH
		{
			get => this.GetValue( () => this.BrushH );
			set => this.SetValue( () => this.BrushH, value );
		}

		public int minX
		{
			get => this.GetValue( () => this.minX );
			set => this.SetValue( () => this.minX, value );
		}
		public int minY
		{
			get => this.GetValue( () => this.minY );
			set => this.SetValue( () => this.minY, value );
		}
		public int maxX
		{
			get => this.GetValue( () => this.maxX );
			set => this.SetValue( () => this.maxX, value );
		}
		public int maxY
		{
			get => this.GetValue( () => this.maxY );
			set => this.SetValue( () => this.maxY, value );
		}
		public int minW
		{
			get => this.GetValue( () => this.minW );
			set => this.SetValue( () => this.minW, value );
		}
		public int minH
		{
			get => this.GetValue( () => this.minH );
			set => this.SetValue( () => this.minH, value );
		}
		public int maxW
		{
			get => this.GetValue( () => this.maxW );
			set => this.SetValue( () => this.maxW, value );
		}
		public int maxH
		{
			get => this.GetValue( () => this.maxH );
			set => this.SetValue( () => this.maxH, value );
		}
		public void FreeResources()
		{
			try
			{
				if ( this.m_graphic != MIL.M_NULL )
					MIL.MgraFree( this.m_graphic );
			}
			catch
			{

			}
		}
	}

}
