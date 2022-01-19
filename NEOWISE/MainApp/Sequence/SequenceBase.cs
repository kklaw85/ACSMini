using HiPA.Common;
using HiPA.Common.Forms;
using HiPA.Common.Report;
using NeoWisePlatform.Module;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NeoWisePlatform.Sequence
{
	public enum SequenceState
	{
		Init,
		IsWaitWork,//indicates thread is waiting for task
		IsAction,//indicates thread is working
		IsDone,//indicates thread has completed all the steps
		IsFail,
		IsNotStarted,
	}
	public abstract class SequenceBase : BaseUtility
	{
		public double CycleTime { get; set; } = 0d;
		protected int WaitIoTime { get; set; } = 30000;
		protected int WaitMoveTime { get; set; } = 30000;
		protected int WaitThreadOutTime { get; set; } = 30000;

		private Stopwatch Stopwatch = new Stopwatch();

		protected ErrorResult Result = new ErrorResult();
		protected void CatchException( Exception ex )
		{
			if ( this.Result.EClass == ErrorClass.OK )
				this.Result.EClass = ErrorClass.E6;
			this.Result.ErrorMessage = this.FormatErrMsg2( this.WorkerSeq.Name, ex );
		}

		//public bool IsCycleStop { get; set; } = false;
		//public bool IsReady { get; set; } = false;//indicates thread is ready for instruction
		public bool StartExecute { get; set; } = false;
		public bool RestartSeq { get; set; } = false;
		public bool IsFail { get; set; } = false;
		//public bool IsDone { get; set; } = false;//indicates thread has completed all the steps
		protected MTEquipment _Equipment => Constructor.GetInstance().Equipment as MTEquipment;
		protected EquipmentBypass ByPass => this._Equipment.MachineMisc.Configuration.ByPassConfig;
		protected Issuehandler IssueHandler { get; private set; } = new Issuehandler();
		private SequenceState _State = SequenceState.IsNotStarted;
		public SequenceState State
		{
			get => this._State;
			set => this.Set( ref this._State, value, "State" );
		}
		public int StepCurrent = 0;
		public int StepPrevious = 0;
		protected string PrvStatus = string.Empty;

		protected object SyncRoot = new object();
		public LoggerHelper CycleTimeLog { get; protected set; }

		protected WorkerThread WorkerSeq { get; set; }
		protected List<FunctionObjects> SeqList = new List<FunctionObjects>();

		private string s_SeqStep = string.Empty;
		public string SeqStep
		{
			get => this.s_SeqStep;
			set => this.Set( ref this.s_SeqStep, value, "SeqStep" );
		}
		private int i_Idx = 0;
		public int Idx
		{
			get => this.i_Idx;
			set => this.Set( ref this.i_Idx, value, "Idx" );
		}
		protected virtual void InitSeqFunction()
		{
		}

		protected virtual void OnThreadStoppedError()
		{
			//if ( this.IsCycleStop == false )
			//{
			this.IsFail = true;
			this.StopAuto();
			//}
			//else
			//{
			//	this.PauseAuto();
			//}
		}
		/// <summary>
		/// 线程状态初始化
		/// </summary>
		public void ThreadInitState()
		{
			this.StartExecute = false;
			this.IsFail = false;
			this.State = SequenceState.Init;
			this.Result.Reset();
		}

		public void SetStepThread( Enum index )
		{
			this.StepCurrent = Convert.ToInt32( index );
			this.StartExecute = true;
		}
		/// <summary>
		/// 线程启动
		/// </summary>
		public void StartAuto()
		{
			//this.IsCycleStop = false;
			Task.Run( () =>
			{
				this.WorkerSeq.Start();
				this.Stopwatch.Reset();
			} );
		}
		/// <summary>
		/// 线程停止
		/// </summary>
		public void StopAuto()
		{
			Task.Run( () =>
			{
				this.State = SequenceState.IsNotStarted;
				this.WorkerSeq.Stop();
				this.ResetAll();
			} );

		}
		/// <summary>
		/// 线程暂停
		/// </summary>
		public void PauseAuto()
		{
			Task.Run( () =>
			{
				this.WorkerSeq.Pause();
				this.Stopwatch.Reset();
			} );
		}
		/// <summary>
		/// 线程复位
		/// </summary>
		public void ResetAll()
		{
			this.WorkerSeq.Reset();
		}
		/// <summary>
		/// 复位启动
		/// </summary>
		public void ResetStart()
		{
			this.WorkerSeq.ResetStart();
		}
		/// <summary>
		/// 循环函数 运动超时判断
		/// </summary>
		public int MoveCycleFunction( double time = 0 )
		{
			if ( time != 0 && this.Stopwatch.ElapsedMilliseconds > time )
			{
				this.ResetTimeWatch();
				return ( int )RunErrors.ERR_MoveTimeout;
			}
			//if ( time != 0 && !this.Stopwatch.IsRunning ) this.StartTimeWatch();
			this.WorkerSeq.JumpIndex( this.WorkerSeq.CurrentIndex() );
			return ( int )RunStatusIndex.Status_Circle;
		}
		/// <summary>
		/// 循环函数 IO超时判断
		/// </summary>
		public int IoCycleFunction( double time = 0 )
		{
			if ( time != 0 && !this.Stopwatch.IsRunning ) this.StartTimeWatch();
			if ( time != 0 && this.Stopwatch.ElapsedMilliseconds > time )
			{
				this.ResetTimeWatch();
				return ( int )RunErrors.ERR_IOTimeout;
			}
			this.WorkerSeq.JumpIndex( this.WorkerSeq.CurrentIndex() );
			return ( int )RunStatusIndex.Status_Circle;
		}
		/// <summary>
		/// 循环函数 函数超时
		/// </summary>

		public int CycleFunction( double time = 0 )
		{
			if ( time != 0 && !this.Stopwatch.IsRunning ) this.StartTimeWatch();
			if ( time != 0 && this.Stopwatch.ElapsedMilliseconds > time )
			{
				this.ResetTimeWatch();
				return ( int )RunErrors.ERR_FunctionTimeout;
			}
			this.WorkerSeq.JumpIndex( this.WorkerSeq.CurrentIndex() );
			return ( int )RunStatusIndex.Status_Circle;
		}
		/// <summary>
		/// 跳转函数 
		/// </summary>
		public int JumpFunction( int index )
		{
			this.StepPrevious = this.WorkerSeq.CurrentIndex();
			this.StepCurrent = Convert.ToInt32( index );
			this.WorkerSeq.JumpIndex( index );
			return ( int )RunStatusIndex.Status_Circle;
		}
		/// <summary>
		/// 线程计时器
		/// </summary>
		public void StartTimeWatch()
		{
			this.Stopwatch.Restart();
		}
		/// <summary>
		/// 线程计时器
		/// </summary>
		public void ResetTimeWatch()
		{
			this.Stopwatch.Reset();
		}
		/// <summary>
		/// 获取当前状态
		/// </summary>
		public int GetThreadStatus()
		{
			return this.WorkerSeq.CurrentIndex();
		}
		/// <summary>
		/// 跳转函数 使用枚举
		/// </summary>
		/// <param name="threadStatus"></param>
		/// <returns></returns>
		public int JumpFunctionEnum( Enum index )
		{
			this.StepPrevious = this.WorkerSeq.CurrentIndex();
			this.StepCurrent = Convert.ToInt32( index );
			this.WorkerSeq.JumpIndex( Convert.ToInt32( index ) );
			return ( int )RunStatusIndex.Status_Circle;
		}

		/// <summary>
		/// 比较当前线程状态和枚举是否对应
		/// </summary>
		/// <param name="threadStatus"></param>
		/// <returns></returns>
		public bool CompareThreadIndex( Enum threadStatus )
		{
			var result = this.WorkerSeq.CurrentIndex();
			this.Logging( threadStatus );
			return result != Convert.ToInt32( threadStatus );
		}

		/// <summary>
		/// 获取报错异常描述
		/// </summary>
		static Dictionary<int, string> _errorsSet;
		public static string GetErrorDesc( int errorCode )
		{
			if ( _errorsSet == null )
			{
				_errorsSet = new Dictionary<int, string>();
				foreach ( var pair in HiPA.Common.Utils.ReflectionTool.GetEnumValueDesc( typeof( RunErrors ) ) )
					_errorsSet.Add( pair.Value, pair.Desc );
			}
			if ( errorCode == 0 ) return "";

			if ( _errorsSet.TryGetValue( errorCode, out var result ) ) return result;
			return "Unknown Error";
		}
		/// <summary>
		/// 获取当前函数名字
		/// </summary>
		/// <returns></returns>
		public static string GetMethodName()
		{
			var method = new StackFrame( 1 ).GetMethod();
			var property =
				( from p in method.DeclaringType.GetProperties(
				  System.Reflection.BindingFlags.Instance |
				  System.Reflection.BindingFlags.Static |
				  System.Reflection.BindingFlags.Public |
				  System.Reflection.BindingFlags.NonPublic )
				  where p.GetGetMethod( true ) == method || p.GetSetMethod( true ) == method
				  select p ).FirstOrDefault();
			return property == null ? method.Name : property.Name;
		}
		public string GetCurrentFunctionName()
		{
			return this.SeqList[ this.WorkerSeq.CurrentIndex() ].Name;
		}
		private void Logging( Enum threadStatus )
		{
			if ( this.PrvStatus != threadStatus.ToString() )
			{
				//if ( this.PrvStatus != string.Empty && this.AdditionSeqInfo.Count > 0 && this.AdditionSeqInfo[ this.PrvStatus.ToString() ] != string.Empty )
				//	this.CycleTimeLog?.WriteCycleTimeLog( this.PrvStatus + "-> Additional Info:," + this.AdditionSeqInfo[ this.PrvStatus ] );
				//this.CycleTimeLog?.WriteCycleTimeLog( threadStatus.ToString() );
				//this.PrvStatus = threadStatus.ToString();
				//this.s_AdditionalLoggingInfo = string.Empty;
			}
		}
		protected bool isError( ErrorResult source )
		{
			if ( source == null )
			{
				this.Result.EClass = ErrorClass.OK;
				this.Result.ErrorMessage = string.Empty;
				return false;
			}

			this.Result.EClass = source.EClass;
			this.Result.ErrorMessage = source.ErrorMessage;
			return this.Result.EClass != ErrorClass.OK;
		}
		protected bool isStoppableError()
		{
			return ( int )this.Result.EClass >= ( int )ErrorClass.E4;
		}

		protected virtual int GotoError( int Res )
		{
			var eRes = ( RunErrors )Res;
			this.ReportIssue( eRes, this.Result.ErrorMessage, false );
			return 0;
		}
		protected void ReportWarning( RunErrors eRes, string errmsg, bool CycleStop )
		{
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( CycleStop ) this.CycleStop();
				Equipment.ErrManager.RaiseWarning( $"{this.WorkerSeq.Name} {this.SeqList[ this.WorkerSeq.CurrentIndex() ].Name} [{GetErrorDesc( ( int )eRes )}] [{errmsg}]", GetErrorDesc( ( int )eRes ) );
			}
			catch
			{ }
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}
		protected void ReportIssue( RunErrors eRes, string errmsg, bool CycleStop )
		{
			try
			{
				Monitor.Enter( this.SyncRoot );
				if ( CycleStop ) this.CycleStop();
				this.IssueHandler.Add( this.WorkerSeq.Name, this.SeqList[ this.WorkerSeq.CurrentIndex() ].Name, eRes, errmsg );
				this.ReportError();
			}
			catch
			{ }
			finally
			{
				Monitor.Exit( this.SyncRoot );
			}
		}

		protected abstract void ReportError();
		protected void ReportErrorOnly()
		{
			foreach ( var issue in this.IssueHandler.Fetch() )
			{
				Equipment.ErrManager.RaiseError( $"{issue.WorkerName} {issue.IndexName} [{issue.ErrorString}]", ErrorTitle.OperationFailure, this.Result.EClass );
			}
			this.IssueHandler.Clear();
			this.Result.Reset();
		}
		protected abstract void CycleStop();
	}
	public class IssueTracker
	{
		public string WorkerName { get; set; }
		public string IndexName { get; set; }
		public string ErrorString { get; set; }
		public IssueTracker( string sourcename, string indexsourcename, string errstring )
		{
			this.WorkerName = sourcename;
			this.IndexName = indexsourcename;
			this.ErrorString = errstring;
		}
	}
	public class Issuehandler
	{
		private List<IssueTracker> Issues { get; set; } = new List<IssueTracker>();
		public void Clear()
		{
			this.Issues.Clear();
		}
		public void Add( string WorkerName, string IndexName, RunErrors ErrCode, string ErrMsg )
		{
			var errstr = SequenceBase.GetErrorDesc( ( int )ErrCode );
			this.Issues.Add( new IssueTracker( WorkerName, IndexName, $"{errstr}_{ErrMsg}" ) );
		}
		public List<IssueTracker> Fetch()
		{
			return this.Issues;
		}
	}
	public enum ErrorAction
	{
		Continue,
		CycleStop,
		Stop,
	}
}
#region Sequence Status
public enum RunStatusIndex
{
	[Description( "Operation System type mismatched" )]
	Status_Circle = ( 99 ),
}
#endregion Sequence Status