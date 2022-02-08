using HiPA.Common.Forms;
using System;
using System.Reflection;

namespace SystemInfo
{


	public class C_System_info : BaseUtility
	{

		//System Start-Up Driectory 
		public string sRoot_Dir = AppDomain.CurrentDomain.BaseDirectory + "\\";
		public const string ApplicationName = "Neowise";
		public static string DebugVersion = "_C";
		public static string Software_Version = Assembly.GetExecutingAssembly().GetName().Version + DebugVersion;

		public static string GetApplicationName()
		{
			return C_System_info.ApplicationName;
		}
		public C_System_info()
		{
		}
	}
}
