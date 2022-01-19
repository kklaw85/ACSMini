using HiPA.Common.Forms;
using System;
using System.Threading;

namespace HiPA.Instrument.Motion
{
	[Serializable]
	public class MotionProfile
		: BaseUtility
	{
		private int i_Direction = 0;
		public int Direction// 1 positive dir, 0 negative dir
		{
			get => Interlocked.CompareExchange( ref this.i_Direction, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.i_Direction, value );
				this.OnPropertyChanged( "Direction" );
			}
		}


		private double d_velocity = 0;
		public double Velocity
		{
			get => Interlocked.CompareExchange( ref this.d_velocity, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.d_velocity, value );
				this.OnPropertyChanged( "Velocity" );
				this.Direction = this.d_velocity > 0 ? 1 : 0;
				if ( this.d_acceleration != 0 )
					this.TimeAcceleration = this.d_velocity / this.d_acceleration;
				if ( this.d_deceleration != 0 )
					this.TimeDeceleration = this.d_velocity / this.d_deceleration;
			}
		}

		private double d_acceleration = 0;
		public double Acceleration
		{
			get => Interlocked.CompareExchange( ref this.d_acceleration, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.d_acceleration, value );
				this.OnPropertyChanged( "Acceleration" );
				if ( this.d_acceleration != 0 )
					this.TimeAcceleration = this.d_velocity / this.d_acceleration;
			}
		}

		private double d_deceleration = 0;
		public double Deceleration
		{
			get => Interlocked.CompareExchange( ref this.d_deceleration, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.d_deceleration, value );
				this.OnPropertyChanged( "Deceleration" );
				if ( this.d_deceleration != 0 )
					this.TimeDeceleration = this.d_velocity / this.d_deceleration;
			}
		}

		private double d_curve = 0;
		public double Curve
		{
			get => Interlocked.CompareExchange( ref this.d_curve, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.d_curve, value );
				this.OnPropertyChanged( "Curve" );
			}
		}

		private double d_TimeAcceleration = 0;
		public double TimeAcceleration
		{
			get => Interlocked.CompareExchange( ref this.d_TimeAcceleration, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.d_TimeAcceleration, value );
				this.OnPropertyChanged( "TimeAcceleration" );
			}
		}
		private double d_TimeDeceleration = 0;
		public double TimeDeceleration
		{
			get => Interlocked.CompareExchange( ref this.d_TimeDeceleration, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.d_TimeDeceleration, value );
				this.OnPropertyChanged( "TimeDeceleration" );
			}
		}

		public readonly double PRA_ACC_SR = 100000d;
		public readonly double PRA_DEC_SR = 100000d;
	}

	[Serializable]
	public class Trajectory
		: MotionProfile
	{
		private double d_distance = 0;
		public double Distance
		{
			get => Interlocked.CompareExchange( ref this.d_distance, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.d_distance, value );
				this.OnPropertyChanged( "Distance" );
			}
		}

		private double d_position = 0;
		public double Position
		{
			get => Interlocked.CompareExchange( ref this.d_position, 0, 0 );
			set
			{
				Interlocked.Exchange( ref this.d_position, value );
				this.OnPropertyChanged( "Position" );
			}
		}

		public Trajectory( double velocity, double acceleration, double deceleration, double curve )
		{
			this.Velocity = velocity;
			this.Acceleration = acceleration;
			this.Deceleration = deceleration;
			this.Curve = curve;
		}

		public Trajectory( MotionProfile profile )
		{
			this.Velocity = profile.Velocity;
			this.Acceleration = profile.Acceleration;
			this.Deceleration = profile.Deceleration;
			this.Curve = profile.Curve;
		}

		public Trajectory()
		{
		}

		public override string ToString()
		{
			return
				$"Position[{this.Position:0.######}], " +
				$"Velocity[{this.Velocity:0.######}], " +
				$"Acceleration[{this.Acceleration:0x######}], " +
				$"Deceleration[{this.Deceleration:0.######}], " +
				$"Curve[{this.Curve:0.#}]";
		}
	}
}
