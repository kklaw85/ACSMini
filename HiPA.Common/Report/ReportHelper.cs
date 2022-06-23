using System;
using System.Collections.Generic;
using System.IO;
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
		//bool toRecordTime, bool toRecordData
		[LoggingAttr( true, false )]
		Start,
		[LoggingAttr( true, false )]
		End,
		[LoggingAttr( true, false )]
		CycleDuration,


		[LoggingAttr( false, false )]
		Vision,
		[LoggingAttr( false, true )]
		FOV1_VisionRawPos_X,
		[LoggingAttr( false, true )]
		FOV1_VisionRawPos_Y,
		[LoggingAttr( false, true )]
		FOV1_VisionRawOffset_X,
		[LoggingAttr( false, true )]
		FOV1_VisionRawOffset_Y,
		[LoggingAttr( false, true )]
		FOV1_PositionOffset_X,
		[LoggingAttr( false, true )]
		FOV1_PositionOffset_Y,
		[LoggingAttr( false, true )]
		FOV1_PositionOffsetMM_X,
		[LoggingAttr( false, true )]
		FOV1_PositionOffsetMM_Y,
		[LoggingAttr( false, true )]
		FOV2_VisionRawPos_X,
		[LoggingAttr( false, true )]
		FOV2_VisionRawPos_Y,
		[LoggingAttr( false, true )]
		FOV2_VisionRawOffset_X,
		[LoggingAttr( false, true )]
		FOV2_VisionRawOffset_Y,
		[LoggingAttr( false, true )]
		FOV2_PositionOffset_X,
		[LoggingAttr( false, true )]
		FOV2_PositionOffset_Y,
		[LoggingAttr( false, true )]
		FOV2_PositionOffsetMM_X,
		[LoggingAttr( false, true )]
		FOV2_PositionOffsetMM_Y,
		[LoggingAttr( false, true )]
		VisionResult,
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
}
