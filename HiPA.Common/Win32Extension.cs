using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Microsoft.Win32
{
	public enum WindowMessage : int
	{
		WM_CREATE = 0x0001,
		WM_DESTROY = 0x0002,
		WM_CLOSE = 0x0010,
	}
}
