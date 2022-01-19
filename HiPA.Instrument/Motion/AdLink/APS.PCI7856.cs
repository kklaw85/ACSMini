using HiPA.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace HiPA.Instrument.Motion.APS
{
	[Serializable]
	public enum MNETTransferRate
	{
		Mbps2500 = 0,   // 2.5 Mbps
		Mbps5000 = 1,   //   5 Mbps
		Mbps10000 = 2,  //  10 Mbps
		Mbps20000 = 3,  //	20 Mbps
	}

	[Serializable]
	public enum HSLTransferRate
	{
		Mbps3000 = 1,
		Mbps6000 = 2,
		Mbps12000 = 3,
	}

	[Serializable]
	public enum BoardPortId
	{
		HSL = 0,
		MNET = 1,
	}

	internal class PCI7856 : CommonBoardBase
	{
		int _openCount = 0;
		bool _isOpen = false;
		public override int Open()
		{
			var error = 0;
			if ( this._isOpen == false )
			{
				int boardHandle = 0;
				if ( ( error = APS168.APS_initial( ref boardHandle, 0 ) ) != 0 ) return error;
				if ( boardHandle == 0 ) return 1;

				this._isOpen = true;

				for ( int i = 0; i < 16; i++ )
				{
					if ( ( ( boardHandle >> i ) & 1 ) == 1 )
					{
						this.BoardHandler.Add( i );
					}
				}
				Interlocked.Increment( ref this._openCount );
			}

			return error;
		}

		public override int Close()
		{
			if ( this._isOpen == false ) return 0;

			if ( Interlocked.Decrement( ref this._openCount ) <= 0 )
			{
				this._isOpen = false;
				this._openCount = 0;
				this.BoardHandler.Clear();
				return APS168.APS_close();
			}
			return 0;
		}


		static Dictionary<int, string> _errorsSet;
		public static string GetErrorDesc( int errorCode )
		{
			try
			{
				if ( _errorsSet == null )
				{
					_errorsSet = new Dictionary<int, string>();
					foreach ( var pair in HiPA.Common.Utils.ReflectionTool.GetEnumValueDesc( typeof( AdLinkErrors ) ) )
						_errorsSet.Add( pair.Item1, pair.Item2 );
				}

				if ( errorCode == 0 ) return "";

				if ( _errorsSet.TryGetValue( errorCode, out var result ) ) return result;

			}
			catch ( Exception ex )
			{
				Equipment.ErrManager.RaiseError( $"PS.PCI7856.cs :GetErrorDesc:{ex.Message}", ErrorTitle.InvalidOperation, ErrorClass.E6 );
				//new Thread( () => MessageBox.Show( $"PS.PCI7856.cs :GetErrorDesc:{ex.Message}" ) ).Start();
				throw;
			}
			return "Unknown Error";
		}
	}

	#region AdLink Errors
	public enum AdLinkErrors
	{
		ERR_NoError = ( 0 ),        //No Error	

		[Description( "Operation System type mismatched" )]
		ERR_OSVersion = ( -1 ),       // 

		[Description( "Open device driver failed - Create driver interface failed" )]
		ERR_OpenDriverFailed = ( -2 ),  // 

		[Description( "System memory insufficiently" )]
		ERR_InsufficientMemory = ( -3 ),    // 

		[Description( "Cards not be initialized" )]
		ERR_DeviceNotInitial = ( -4 ),  // 

		[Description( "Cards not found(No card in your system)" )]
		ERR_NoDeviceFound = ( -5 ), // 

		[Description( "Cards' ID is duplicated. " )]
		ERR_CardIdDuplicate = ( -6 ),   // 

		[Description( "Cards have been initialed " )]
		ERR_DeviceAlreadyInitialed = ( -7 ),    // 

		[Description( "Cards' interrupt events not enable or not be initialized" )]
		ERR_InterruptNotEnable = ( -8 ),    // 

		[Description( "Function time out" )]
		ERR_TimeOut = ( -9 ),   // 

		[Description( "Function input parameters are invalid" )]
		ERR_ParametersInvalid = ( -10 ),    // 

		[Description( "Set data to EEPROM (or nonvolatile memory) failed" )]
		ERR_SetEEPROM = ( -11 ),    // 

		[Description( "Get data from EEPROM (or nonvolatile memory) failed" )]
		ERR_GetEEPROM = ( -12 ),    // 

		[Description( "Function is not available in this step, The device is not support this function or Internal process failed" )]
		ERR_FunctionNotAvailable = ( -13 ), // 

		[Description( "Firmware error, please reboot the system" )]
		ERR_FirmwareError = ( -14 ),   // 

		[Description( "Previous command is in process" )]
		ERR_CommandInProcess = ( -15 ), // 

		[Description( "Axes' ID is duplicated" )]
		ERR_AxisIdDuplicate = ( -16 ),  // .

		[Description( "Slave module not found" )]
		ERR_ModuleNotFound = ( -17 ),   // .

		[Description( "System ModuleNo insufficiently" )]
		ERR_InsufficientModuleNo = ( -18 ), // 

		[Description( "HandSake with the DSP out of time" )]
		ERR_HandShakeFailed = ( -19 ),   // .

		[Description( "Config file format error.(cannot be parsed)" )]
		ERR_FILE_FORMAT = ( -20 ),  // 

		[Description( "Function parameters read only" )]
		ERR_ParametersReadOnly = ( -21 ),   // .

		[Description( "Distant is not enough for motion" )]
		ERR_DistantNotEnough = ( -22 ), // .

		[Description( "Function is not enabled" )]
		ERR_FunctionNotEnable = ( -23 ),    // .

		[Description( "Server already closed" )]
		ERR_ServerAlreadyClose = ( -24 ),   // .

		[Description( "Related dll is not found, not in correct path" )]
		ERR_DllNotFound = ( -25 ),  // .

		[Description( "(indeterminacy)TrimDCA Channel" )]
		ERR_TrimDAC_Channel = ( -26 ),

		[Description( "(indeterminacy)Satellite Type" )]
		ERR_Satellite_Type = ( -27 ),

		[Description( "(indeterminacy)Over Voltage Spec" )]
		ERR_Over_Voltage_Spec = ( -28 ),

		[Description( "(indeterminacy)Over Current Spec" )]
		ERR_Over_Current_Spec = ( -29 ),

		[Description( "(indeterminacy) Slave Is Not AI" )]
		ERR_SlaveIsNotAI = ( -30 ),

		[Description( "(indeterminacy)Over AO Channel Scope" )]
		ERR_Over_AO_Channel_Scope = ( -31 ),

		[Description( "Failed to invoke dll function. Extension Dll version is wrong" )]
		ERR_DllFuncFailed = ( -32 ),    // .

		[Description( "Feeder abnormal stop, External stop or feeding stop" )]
		ERR_FeederAbnormalStop = ( -33 ), //

		[Description( "(indeterminacy)Read Module Type Dismatch" )]
		ERR_Read_ModuleType_Dismatch = ( -34 ),

		[Description( "No such INT number, or WIN32_API error, contact with ADLINK's FAE staff" )]
		ERR_Win32Error = ( -1000 ), // .

		[Description( "The base for DSP error" )]
		ERR_DspStart = ( -2000 ), // 
	}
	#endregion
}
