using HiPA.Common.Forms;

namespace HiPA.Common
{
	public enum eMachineType
	{
		All = 0,
	}

	public enum ModelType
	{
		MMF,
		MET,
	}
	public enum TestResult
	{
		Untested,
		Fail,
		Pass,
	}
	public struct PointD
	{
		public double X;
		public double Y;
		public double Theta;
		public PointD( double x, double y, double theta )
		{
			this.X = x;
			this.Y = y;
			this.Theta = theta;
		}
		public PointD( PointD Point )
		{
			this.X = Point.X;
			this.Y = Point.Y;
			this.Theta = Point.Theta;
		}
	}
	public class C_PointD : BaseUtility
	{
		public double X
		{
			get => this.GetValue( () => this.X );
			set => this.SetValue( () => this.X, value );
		}
		public double Y
		{
			get => this.GetValue( () => this.Y );
			set => this.SetValue( () => this.Y, value );
		}
		public double Theta
		{
			get => this.GetValue( () => this.Theta );
			set => this.SetValue( () => this.Theta, value );
		}

		public C_PointD()
		{
		}
		public C_PointD( double x, double y, double theta )
		{
			this.X = x;
			this.Y = y;
			this.Theta = theta;
		}
		public void Copy( double x, double y, double theta )
		{
			this.X = x;
			this.Y = y;
			this.Theta = theta;
		}
		public C_PointD( PointD Point )
		{
			this.X = Point.X;
			this.Y = Point.Y;
			this.Theta = Point.Theta;
		}
		public void Copy( C_PointD Point )
		{
			this.X = Point.X;
			this.Y = Point.Y;
			this.Theta = Point.Theta;
		}
		public void Clear()
		{
			this.X = 999;
			this.Y = 999;
			this.Theta = 999;
		}

		public bool InRange( double from, double to )
		{
			return this.X.InRange( from, to ) && this.Y.InRange( from, to );
		}
	}
	public enum AccessLevel
	{
		Guest,
		Operator,
		Engineer,
		Admin,
		Manufacturer,
	}
	public enum MachineStateType
	{
		NONE,
		UNINITIALIZE,
		READY,
		BUSY,
		AUTO_RUNNING,
		AUTO_PAUSE,
		AUTO_CYCLESTOP,
		ERROR,
		WARNING,
		HOMING,
	}
	public class MachineStateMng : BaseUtility
	{
		public static bool isSimulation { get; set; } = false;
		public static bool isCycleTimeLog { get; set; } = true;
	}
}
