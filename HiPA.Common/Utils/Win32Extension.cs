using System.Runtime.InteropServices;

namespace Microsoft.Win32
{
	public enum WindowMessage : int
	{
		WM_CREATE = 0x0001,
		WM_DESTROY = 0x0002,
		WM_CLOSE = 0x0010,
	}

	public static class SystemFeatures
	{
		[DllImport( "kernel32.dll", SetLastError = true )]
		public static extern bool QueryPerformanceFrequency( out long lpFrequency );

		// Token: 0x06000002 RID: 2
		[DllImport( "kernel32.dll", SetLastError = true )]
		public static extern bool QueryPerformanceCounter( out long lpPerformanceCount );
	}
}
