using HiPA.Common.Forms;
using HiPA.Common.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace HiPA.Common.Report
{

	[Serializable]
	public class FileSetting
	{
		public string Type { get; set; } = string.Empty;
		public string DateStamp { get; set; } = string.Empty;
		public string HourStamp { get; set; } = string.Empty;
		public string FileName { get; set; } = string.Empty;
		public string Header { get; set; } = string.Empty;
		private string _FileDirectory = string.Empty;
		public string FileDirectory
		{
			get => this._FileDirectory;
			set
			{
				this._FileDirectory = value;
				if ( !Path.IsPathRooted( this._FileDirectory ) ) this._FileDirectory = Path.Combine( Constructor.GetInstance()?.DataPath, this._FileDirectory );
			}
		}
		private void GetTimeStamp()
		{
			this.DateStamp = DateTime.Now.ToString( "yyyyMMdd" );
			this.HourStamp = DateTime.Now.ToString( "HHmmss" );
		}
		public bool ToSave { get; set; } = false;
		public FileSetting()
		{ }
		public FileSetting( FileSetting setting )
		{
			this.FileName = setting.FileName;
			this.FileDirectory = setting.FileDirectory;
			this.ToSave = setting.ToSave;
			this.DateStamp = setting.DateStamp;
			this.HourStamp = setting.HourStamp;
			this.Header = setting.Header;
		}
	}

	[Serializable]
	public class LoggerDataSetting : FileSetting
	{
		public string GetFullPath()
		{
			var _Directory = string.Empty;
			try
			{
				this.DateStamp = DateTime.Now.ToString( "yyyyMMdd" );
				var filename = $"{this.DateStamp}_{this.FileName}.csv";

				_Directory = System.IO.Path.Combine( this.GetFullDirectory(), filename );
			}
			catch ( Exception )
			{
			}
			return _Directory;
		}
		public string GetFullDirectory()
		{
			var _Directory = string.Empty;
			try
			{
				if ( this.FileDirectory == string.Empty )
					this.FileDirectory = Constructor.GetInstance()?.DataPath;
				_Directory = this.FileDirectory;
				//this.DateStamp = DateTime.Now.ToString("yyyyMMdd");

				//if ( this.DateStamp != string.Empty )
				//{
				//    _Directory = System.IO.Path.Combine( this.FileDirectory, this.DateStamp );
				//    if ( !Directory.Exists( _Directory ) )
				//        Directory.CreateDirectory( _Directory );
				//}
			}
			catch ( Exception )
			{
			}
			return _Directory;
		}

		public string GetCycleTimeLogPath()
		{
			var _Directory = string.Empty;
			try
			{
				if ( this.FileDirectory == string.Empty )
					this.FileDirectory = Constructor.GetInstance()?.DataPath;

				_Directory = System.IO.Path.Combine( this.FileDirectory, DateTime.Now.ToString( "yyyyMMdd" ) ) + ".csv";
			}
			catch ( Exception )
			{
			}
			return _Directory;
		}
		public bool OverrideDefaultFilename { get; set; } = false;
		public LoggerDataSetting()
		{ }
		public LoggerDataSetting( FileSetting setting )
		{
			this.FileName = setting.FileName;
			this.FileDirectory = setting.FileDirectory;
			this.ToSave = setting.ToSave;
			this.DateStamp = setting.DateStamp;
			this.HourStamp = setting.HourStamp;
		}
	}

	public class LoggerHelper
	{
		object SyncRoot = new object();
		object SyncRootAutoRun = new object();
		public LoggerHelper()
		{ }
		private LoggerDataSetting Reportsetting { get; set; } = new LoggerDataSetting();

		private object objLock = new object();
		public LoggerHelper( string name, string directory = null, string header = null )
		{
			if ( directory != null )
				this.Reportsetting.FileDirectory = directory;
			this.Reportsetting.FileName = System.IO.Path.GetFileName( name );
			this.Reportsetting.ToSave = true;
			this.Reportsetting.Header = header;
		}

		public void WriteLog( string content )
		{
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( !Directory.Exists( this.Reportsetting.GetFullDirectory() ) )
					Directory.CreateDirectory( this.Reportsetting.GetFullDirectory() );
				if ( !File.Exists( this.Reportsetting.GetFullPath() ) )
				{
					// Create a file to write to.
					using ( StreamWriter sw = File.CreateText( this.Reportsetting.GetFullPath() ) )
					{
						if ( this.Reportsetting.Header != null )
							sw.WriteLine( $"Date,Time,{this.Reportsetting.Header}" );
						sw.Write( $"{DateTime.Now.ToString( "dd_MM_yyyy,HH:mm:ss" )}," );
						sw.WriteLine( content );
						sw.Close();
					}
				}
				else
				{
					using ( StreamWriter sw = File.AppendText( this.Reportsetting.GetFullPath() ) )
					{
						sw.Write( $"{DateTime.Now.ToString( "dd_MM_yyyy,HH:mm:ss" )}," );
						sw.WriteLine( content );
						sw.Close();
					}
				}
			}
			catch
			{ }
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}


		public void WriteCycleTimeLog( string content )
		{
			try
			{
				if ( !MachineStateMng.isCycleTimeLog )
					return;

				Monitor.Enter( this.SyncRoot );
				if ( !Directory.Exists( this.Reportsetting.GetFullDirectory() ) )
					Directory.CreateDirectory( this.Reportsetting.GetFullDirectory() );
				if ( !File.Exists( this.Reportsetting.GetFullPath() ) )
				{
					using ( StreamWriter sw = File.CreateText( this.Reportsetting.GetFullPath() ) )
					{
						sw.Write( $"{DateTime.Now.ToString( "dd_MM_yyyy,HH:mm:ss.ffffff:" )}," );
						sw.WriteLine( content );
						sw.Close();
					}
				}
				else
				{
					using ( StreamWriter sw = File.AppendText( this.Reportsetting.GetFullPath() ) )
					{
						sw.Write( $"{DateTime.Now.ToString( "dd_MM_yyyy,HH:mm:ss.ffffff" )}:," );
						sw.WriteLine( content );
						sw.Close();
					}
				}
			}
			catch ( Exception )
			{ }
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}

		public void DeleteLog()
		{
			try
			{
				File.Delete( this.Reportsetting.GetFullPath() );
			}
			catch
			{
			}
		}
		public string[] GetLog()
		{
			try
			{
				string sFilePath = this.Reportsetting.GetFullPath();
				if ( File.Exists( sFilePath ) )
				{
					return File.ReadAllLines( sFilePath );
				}
				return null;
			}
			catch ( Exception )
			{
				return null;
			}
		}

		public string WriteAutorunLog( Dictionary<eLogItems, string> LogItems )
		{
			var s_Result = string.Empty;
			string s_Header = string.Empty;
			try
			{
				foreach ( var ele in LogItems )
				{
					s_Header += ele.Value + ",";
				}
				this.WriteReport( s_Header );
			}
			catch ( Exception )
			{

			}
			return s_Result;
		}
		private void WriteReport( string content )
		{
			try
			{
				Monitor.Enter( this.SyncRootAutoRun );
				if ( !Directory.Exists( Constructor.GetInstance().DataLoggerPath ) )
					Directory.CreateDirectory( Constructor.GetInstance().DataLoggerPath );
				if ( !File.Exists( $"{Constructor.GetInstance().DataLoggerPath}{DateTime.Now.ToString( "yyyyMMdd" )}_{this.Reportsetting.FileName}.csv" ) )
				{
					// Create a file to write to.
					using ( StreamWriter sw = File.CreateText( $"{Constructor.GetInstance().DataLoggerPath}{DateTime.Now.ToString( "yyyyMMdd" )}_{this.Reportsetting.FileName}.csv" ) )
					{
						if ( !string.IsNullOrEmpty( this.Reportsetting.Header ) )
							sw.WriteLine( $"{this.Reportsetting.Header}" );
						sw.WriteLine( content );
					}
				}
				else
				{
					using ( StreamWriter sw = File.AppendText( $"{Constructor.GetInstance().DataLoggerPath}{DateTime.Now.ToString( "yyyyMMdd" )}_{this.Reportsetting.FileName}.csv" ) )
					{
						sw.WriteLine( content );
					}
				}
			}
			catch ( Exception )
			{ }
			finally
			{
				Monitor.Exit( this.SyncRootAutoRun );
			}
		}
	}
	public class HiveFloorError
	{
		public string ErrorNo { get; set; }
		public string ErrorMessage { get; set; }

		public DateTime OccurTime { get; set; }

		public DateTime ResolvedTime { get; set; }
	}
	public enum eLogItems
	{
		//[LoggingAttr( true, false, eMachineType.All )]
		//Start,
		//[LoggingAttr( true, false, eMachineType.All )]
		//End,
		//[LoggingAttr( false, true, eMachineType.All )]
		//Counter,
		//[LoggingAttr( false, true, eMachineType.All )]
		//ProdCode,
		//[LoggingAttr( false, true, eMachineType.All )]
		//Color,
		//[LoggingAttr( false, true, eMachineType.All )]
		//SerialNo,
		//[LoggingAttr( false, true, eMachineType.All )]
		//Station,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//StopperClosedTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//NFCWorkPosTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//ReadNFCTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//NFCBottomALTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//GetImageNameTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//OpenTopCoverTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//FirstPalletToPos2Time,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//ScanBarCodeTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//FirstPalletToPos1Time,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//SendRequestToShopFloorTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//GetCavityInfoFromShopFloorTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//MoveProductPresserTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//ThreePalletsInMachineTime,
		//[LoggingAttr( true, false, eMachineType.All )]
		//MotionInPos,
		//[LoggingAttr( true, false, eMachineType.All )]
		//ScanHeightTime,
		//[LoggingAttr( false, true, eMachineType.All )]
		//ScanHeightValue,
		//[LoggingAttr( true, false, eMachineType.All )]
		//ZCompensationChkTime,
		//[LoggingAttr( false, true, eMachineType.All )]
		//ZCompensationValue,
		//[LoggingAttr( true, false, eMachineType.All )]
		//VisionAlignChkTime,
		//[LoggingAttr( false, true, eMachineType.All )]
		//VisionAlignXValue,
		//[LoggingAttr( false, true, eMachineType.All )]
		//VisionAlignYValue,
		//[LoggingAttr( false, true, eMachineType.All )]
		//VisionAlignWidthValue,
		//[LoggingAttr( false, true, eMachineType.All )]
		//VisionAlignRotationYValue,
		//[LoggingAttr( true, false, eMachineType.All )]
		//MarkingTime,
		//[LoggingAttr( true, false, eMachineType.All )]
		//ContentChkTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//SendToWMSTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//PopUpWinCloseTime,
		//[LoggingAttr( true, false, eMachineType.All )]
		//FilePostHandleTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//CloseTopCoverTime,
		//[LoggingAttr( true, false, eMachineType.B228 )]
		//ReleaseStopperTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//LiftZStandbyDoorOpenTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//StartSendResultShopFloorTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//WaitSmemaOutAvailableTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//SmemaOutFlagOnTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//StoppersResetandConveyorsOnTime,
		//[LoggingAttr( true, false, eMachineType.B262 )]
		//ConveyorClearedTime,
		//[LoggingAttr( true, false, eMachineType.All )]
		//TotalUsageTime,
	}
	public class LoggingAttr : Attribute
	{
		public bool ToRecordData { get; protected set; }
		public bool ToRecordTime { get; protected set; }
		public LoggingAttr( bool toRecordTime, bool toRecordData )
		{
			this.ToRecordTime = toRecordTime;
			this.ToRecordData = toRecordData;
		}
	}
	public class AutoRunLogging : BaseUtility
	{
		public Dictionary<eLogItems, string> LogContent = new Dictionary<eLogItems, string>();
		public AutoRunLogging()
		{
			foreach ( var ele in Enum.GetValues( typeof( eLogItems ) ).Cast<eLogItems>() )
			{
				var attr = ReflectionTool.GetEnumAttribute<LoggingAttr>( ele );
				this.LogContent[ ele ] = string.Empty;
			}
		}
		public string AutoRunLogHeader()
		{
			var s_Header = string.Empty;
			foreach ( var ele in this.LogContent.Keys )
			{
				s_Header += ele.ToString() + ( ele.ToString().Contains( "Time" ) ? " (ms)" : "" ) + ",";
			}
			return s_Header;
		}
		private Stopwatch SW = new Stopwatch();
		private Stopwatch TotalSW = new Stopwatch();

		private void RecordElapsedTime( eLogItems item )
		{
			try
			{
				this.LogContent[ item ] = this.SW.ElapsedMilliseconds.ToString();
			}
			catch
			{
			}
			finally
			{
				this.SW.Restart();
			}
		}
		private void RecordCurrentTime( eLogItems Name )
		{
			try
			{
				this.LogContent[ Name ] = DateTime.Now.ToString( "MM/dd/yyyy HH:mm:ss" );
			}
			catch
			{
			}
			finally
			{
				this.SW.Restart();
			}
		}
		private void RecordData( eLogItems item, Object Source )
		{
			try
			{
				this.LogContent[ item ] = Source?.ToString();
			}
			catch
			{
			}
		}
		private void Clear()
		{
			this.LogContent = this.LogContent.ToDictionary( p => p.Key, p => string.Empty );
		}
	}
}
