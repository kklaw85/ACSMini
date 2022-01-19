namespace JptCamera
{
	public interface ICameraSettings
	{

		string CameraBrand { get; }

		string CameraModel { get; }

		string CameraUserID { get; }

		string CameraSerialName { get; }

		ushort Binning { get; set; }

		PixelFormatEnum PixelFormat { get; set; }

		double Exposure { get; set; }

		double ExposureMinimum { get; }

		double ExposureMaximum { get; }

		double Gain { get; set; }

		double GainMinimum { get; }

		double GainMaximum { get; }

		bool GammaEnable { get; set; }

		double Gamma { get; set; }

		double GammaMinimum { get; }

		double GammaMaximum { get; }

		//bool TriggerMode { get; set; }

		TriggerTypeEnum TriggerType { get; set; }

		int TriggerDelay { get; set; }

		int TriggerDelayMin { get; }

		int TriggerDelayMax { get; }

		int Width { get; set; }

		int WidthMinimum { get; }

		int WidthMaximum { get; }

		int Height { get; set; }

		int HeightMinimum { get; }

		int HeightMaximum { get; }

		int OffsetX { get; set; }

		int OffsetY { get; set; }

		bool ReverseX { get; set; }

		bool ReverseY { get; set; }

	}
}
