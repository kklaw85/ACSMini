using N_Error_Event;
using SystemInfo;

namespace N_Data_Utilities
{
	class C_Shared_Data
	{
		public static ErrorMsgMap ErrorMap = new ErrorMsgMap();
		public static WarningMsgMap WarningMap = new WarningMsgMap();
		public static EventMsgMap EventMap = new EventMsgMap();
		public static C_System_info Info_Sys = new C_System_info();
	}
}
