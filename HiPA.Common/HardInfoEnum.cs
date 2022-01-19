using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Management;

namespace MTVCSEL.Common
{
	/// <summary>
	/// 枚举win32 api
	/// </summary>
	public enum HardwareEnum
	{
		// 硬件
		Win32_Processor,				// CPU 处理器
		Win32_PhysicalMemory,			// 物理内存条
		Win32_Keyboard,					// 键盘
		Win32_PointingDevice,			// 点输入设备，包括鼠标。
		Win32_FloppyDrive,				// 软盘驱动器
		Win32_DiskDrive,				// 硬盘驱动器
		Win32_CDROMDrive,				// 光盘驱动器
		Win32_BaseBoard,				// 主板
		Win32_BIOS,						// BIOS 芯片
		Win32_ParallelPort,				// 并口
		Win32_SerialPort,				// 串口
		Win32_SerialPortConfiguration,	// 串口配置
		Win32_SoundDevice,				// 多媒体设置，一般指声卡。
		Win32_SystemSlot,				// 主板插槽 (ISA & PCI & AGP)
		Win32_USBController,			// USB 控制器
		Win32_NetworkAdapter,			// 网络适配器
		Win32_NetworkAdapterConfiguration, // 网络适配器设置
		Win32_Printer,					// 打印机
		Win32_PrinterConfiguration,		// 打印机设置
		Win32_PrintJob,					// 打印机任务
		Win32_TCPIPPrinterPort,			// 打印机端口
		Win32_POTSModem,				// MODEM
		Win32_POTSModemToSerialPort,	// MODEM 端口
		Win32_DesktopMonitor,			// 显示器
		Win32_DisplayConfiguration,		// 显卡
		Win32_DisplayControllerConfiguration, // 显卡设置
		Win32_VideoController,			// 显卡细节。
		Win32_VideoSettings,			// 显卡支持的显示模式。

		// 操作系统
		Win32_TimeZone,					// 时区
		Win32_SystemDriver,				// 驱动程序
		Win32_DiskPartition,			// 磁盘分区
		Win32_LogicalDisk,				// 逻辑磁盘
		Win32_LogicalDiskToPartition,	// 逻辑磁盘所在分区及始末位置。
		Win32_LogicalMemoryConfiguration, // 逻辑内存配置
		Win32_PageFile,					// 系统页文件信息
		Win32_PageFileSetting,			// 页文件设置
		Win32_BootConfiguration,		// 系统启动配置
		Win32_ComputerSystem,			// 计算机信息简要
		Win32_OperatingSystem,			// 操作系统信息
		Win32_StartupCommand,			// 系统自动启动程序
		Win32_Service,					// 系统安装的服务
		Win32_Group,					// 系统管理组
		Win32_GroupUser,				// 系统组帐号
		Win32_UserAccount,				// 用户帐号
		Win32_Process,					// 系统进程
		Win32_Thread,					// 系统线程
		Win32_Share,					// 共享
		Win32_NetworkClient,			// 已安装的网络客户端
		Win32_NetworkProtocol,			// 已安装的网络协议
		Win32_PnPEntity,				//all device
	}

	public class HardInfoEnum
	{
		/// <summary>
		/// WMI取硬件信息
		/// </summary>
		/// <param name="hardType"></param>
		/// <param name="propKey"></param>
		/// <returns></returns>
		public static List<string> MulGetHardwareInfo( HardwareEnum hardType, string propKey )
		{
			List<string> result = new List<string>();
			try
			{
				using ( ManagementObjectSearcher searcher = new ManagementObjectSearcher( "select * from " + hardType ) )
				{
					var hardInfos = searcher.Get();
					foreach ( var hardInfo in hardInfos )
					{
						var value = hardInfo.Properties[ propKey ].Value;
						if ( value != null && 
							value.ToString().Contains( "COM" ) == true )
						{
							result.Add( value.ToString() );
						}
					}
					searcher.Dispose();
				}
				return result;
			}
			catch
			{
				return null;
			}
		}
		//通过WMI获取COM端口
		/// <summary>
		/// 串口信息
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<SerialPortNameItem> GetSerialPort()
		{
			var ports =  MulGetHardwareInfo( HardwareEnum.Win32_PnPEntity, "Name" );
			foreach ( var port in ports )
			{
				yield return new SerialPortNameItem( port );
			}
		}
	}

	public class SerialPortNameItem
	{
		public readonly string PortName = "";
		public readonly string Describe = "";

		public SerialPortNameItem( string portDescribe )
		{
			var match = System.Text.RegularExpressions.Regex.Match( portDescribe, @"^(?<NAME>.*?)\(COM(?<COM>\d+)\).*?$", RegexOptions.IgnoreCase );

			var name = match.Groups[ "NAME" ].Value;
			var com = match.Groups[ "COM" ].Value;

			this.PortName = $"COM{com}";
			this.Describe = $"(COM{com}) {name}";
		}

		public override string ToString()
		{
			return this.Describe;
		}
	}
}
