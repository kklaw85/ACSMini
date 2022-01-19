namespace JptCamera
{
	public enum PixelFormatEnum
	{
		UNDEFINED,
		MONO8,
		MONO10,
		MONO12,
		RGB8PACKED,
	}

	public enum TriggerTypeEnum
	{
		TRIGGER_OFF = 0,
		TRIGGER_CONTINUOUS = 1,
		TRIGGER_HW_RISING = 2,
		TRIGGER_HW_FALLING = 4,
		TRIGGER_SOFTWARE = 8,
	}

}
