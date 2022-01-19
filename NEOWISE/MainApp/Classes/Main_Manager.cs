using HiPA.Common;
using N_Data_Utilities;


namespace NeoWisePlatform
{
	class C_Main_Manager
	{
		public static void Init_Main_Manager()
		{
			string sErr = "";
			try
			{
				Constructor.GetInstance().EquipmentConfigurationType = typeof( NeoWisePlatform.Module.EquipmentConfig );
				sErr = Constructor.GetInstance().Load();

				var taskInit = Constructor.GetInstance().Equipment.Initialize();
				taskInit.Wait();
				Equipment.ErrManager.ShowMessage( C_Shared_Data.EventMap.SYS_READY, "Initialization" );
			}
			catch
			{
				return;
			}
			return;
		}
	}
}
