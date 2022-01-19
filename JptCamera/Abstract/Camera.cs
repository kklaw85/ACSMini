using System;
using System.Drawing;

namespace JptCamera
{
	public abstract class Camera : ICameraSettings
	{
		public abstract string ConnectCamera();

		public abstract string ConnectCamera( string CameraName );
		public abstract string ConnectCamera( int DeviceNumber );
		public abstract string ConnectCamera( string CameraName, string DCFFilename );

		//public virtual string InitialiseMtxSystem(MIL_ID MtxSystemID)
		//{ throw new NotImplementedException(); }

		//public virtual string ReconnectCamera()
		//{ throw new NotImplementedException(); }

		//public virtual string StartCameraEvent()
		//{ throw new NotImplementedException(); }

		public abstract string DisconnectCamera();

		public abstract string GrabSingle();

		//public abstract string GrabContinuous();

		public abstract string StartGrab();

		public abstract string StopGrab();

		//public abstract string ExecuteSoftwareTrigger();

		//public abstract string FreeResources();

		//public abstract bool IsCameraInitialised { get; }

		public virtual string SetDisplayWinForm( IntPtr DisplayHandler )
		{ throw new NotImplementedException(); }

		public abstract bool IsCameraConnected { get; }

		public virtual CameraSettings Settings { get; }

		public abstract string CameraBrand { get; }

		public abstract string CameraModel { get; }

		public abstract string CameraUserID { get; }

		public abstract string CameraSerialName { get; }

		public abstract ushort Binning { get; set; }

		public abstract PixelFormatEnum PixelFormat { get; set; }

		public abstract double Exposure { get; set; }

		public abstract double ExposureMinimum { get; }

		public abstract double ExposureMaximum { get; }

		public abstract double Gain { get; set; }

		public abstract double GainMinimum { get; }

		public abstract double GainMaximum { get; }

		public abstract bool GammaEnable { get; set; }

		public abstract double Gamma { get; set; }

		public abstract double GammaMinimum { get; }

		public abstract double GammaMaximum { get; }

		public abstract TriggerTypeEnum TriggerType { get; set; }

		public abstract int TriggerDelay { get; set; }

		public abstract int TriggerDelayMin { get; }

		public abstract int TriggerDelayMax { get; }

		public abstract int Width { get; set; }

		public abstract int WidthMinimum { get; }

		public abstract int WidthMaximum { get; }

		public abstract int Height { get; set; }

		public abstract int HeightMinimum { get; }

		public abstract int HeightMaximum { get; }

		public abstract int OffsetX { get; set; }

		public abstract int OffsetY { get; set; }

		public abstract bool ReverseX { get; set; }

		public abstract bool ReverseY { get; set; }

		public class NewImageEventArgs : EventArgs
		{
			public byte[] bImageArray { get; set; } = null;
			public short[] ImageArray { get; set; } = null;
			public int Width { get; set; } = 0;
			public int Height { get; set; } = 0;
			public Bitmap ImageBMP { get; set; } = null;
		}

		public delegate void OnNewImageProcessedHandler( object sender, NewImageEventArgs e );

		public event OnNewImageProcessedHandler OnProcessedImageReceived;

		protected virtual void OnNewImageReceived( NewImageEventArgs e )
		{
			if ( OnProcessedImageReceived != null ) OnProcessedImageReceived( this, e );
		}

		private void OnNewImageProcessed( NewImageEventArgs e )
		{
			this.OnNewImageReceived( e );
		}

		public virtual void OnProcessedImageReceive( EventHandler<NewImageEventArgs> addEvent )
		{
			OnProcessedImageReceived += new OnNewImageProcessedHandler( addEvent );
		}


		public virtual void OnProcessedImageReceivedRemove( EventHandler<NewImageEventArgs> removeEvent )
		{
			OnProcessedImageReceived -= new OnNewImageProcessedHandler( removeEvent );
		}
	}


}
