using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HiPA.Common
{
	public class MathExt
	{
		public static List<double> GetValues( double a, double b, int count )
		{
			double d = ( b - a ) / ( double )( count - 1 );

			return Enumerable.Range( 0, count )
			   .Select( i => ( a + d * i ) )
			   .ToList();
		}
		public static IList<int> FindPeaks( IList<double> values, int rangeOfPeaks )
		{
			List<int> peaks = new List<int>();
			double current;
			IEnumerable<double> range;

			int checksOnEachSide = rangeOfPeaks / 2;
			for ( int i = 0; i < values.Count; i++ )
			{
				current = values[ i ];
				range = values;

				if ( i > checksOnEachSide )
				{
					range = range.Skip( i - checksOnEachSide );
				}

				range = range.Take( rangeOfPeaks );
				if ( ( range.Count() > 0 ) && ( current == range.Max() ) )
				{
					peaks.Add( i );
				}
			}

			return peaks;
		}

		public static IEnumerable<(int Index, double Intensity)> LocalMaxima( IEnumerable<double> source, int windowSize )
		{
			// Round up to nearest odd value
			windowSize = windowSize - windowSize % 2 + 1;
			int halfWindow = windowSize / 2;

			int index = 0;
			var before = new Queue<double>( Enumerable.Repeat( double.NegativeInfinity, halfWindow ) );
			var after = new Queue<double>( source.Take( halfWindow + 1 ) );

			foreach ( double d in source.Skip( halfWindow + 1 ).Concat( Enumerable.Repeat( double.NegativeInfinity, halfWindow + 1 ) ) )
			{
				double curVal = after.Dequeue();
				if ( before.All( x => curVal > x ) && after.All( x => curVal >= x ) )
				{
					yield return (index, curVal);
				}

				before.Dequeue();
				before.Enqueue( curVal );
				after.Enqueue( d );
				index++;
			}
		}

		public static bool LinearRegression( double LSL, double USL, List<double> y, List<double> x, out double Slope, out double y_intercept )
		{
			var minPercentage = 0.02;
			int MaxIndex = 0;
			double Max = y.Max();
			MaxIndex = y.IndexOf( y.Max() );
			var minThreshold = Max * minPercentage;
			var sumSquaresX = 0d;
			var sumSquaresY = 0d;
			var sumX = 0d;
			var sumY = 0d;
			var sumXY = 0d;
			var count = 0d;

			for ( int i = 0; i < y.Count; i++ )
			{
				if ( x[ i ] > USL || x[ i ] < LSL )
					continue;
				sumSquaresY += y[ i ] * y[ i ];
				sumSquaresX += x[ i ] * x[ i ];
				sumY += y[ i ];
				sumX += x[ i ];
				sumXY += x[ i ] * y[ i ];
				count++;
			}
			Slope = 0;
			y_intercept = 0;
			if ( sumX == 0 ) return true;

			var squareSumX = sumX * sumX;
			var denominator = ( count * sumSquaresX - squareSumX );
			if ( denominator == 0 ) return true;

			Slope = ( count * sumXY - sumX * sumY ) / denominator;
			y_intercept = ( sumSquaresX * sumY - sumX * sumXY ) / denominator;
			return true;
		}
		public static bool LinearRegression( List<PointD> xy, out double Slope, out double y_intercept )
		{
			var minPercentage = 0.02;
			int MaxIndex = 0;
			double Max = xy.Max( x => x.Y );
			MaxIndex = xy.FindIndex( x => x.Y == Max );
			var minThreshold = Max * minPercentage;
			var sumSquaresX = 0d;
			var sumSquaresY = 0d;
			var sumX = 0d;
			var sumY = 0d;
			var sumXY = 0d;
			var count = 0d;

			foreach ( var point in xy )
			{
				sumSquaresY += point.Y * point.Y;
				sumSquaresX += point.X * point.X;
				sumY += point.Y;
				sumX += point.X;
				sumXY += point.X * point.Y;
				count++;
			}

			Slope = 0;
			y_intercept = 0;
			if ( sumX == 0 ) return true;

			var squareSumX = sumX * sumX;
			var denominator = ( count * sumSquaresX - squareSumX );
			if ( denominator == 0 ) return true;

			Slope = ( count * sumXY - sumX * sumY ) / denominator;
			y_intercept = ( sumSquaresX * sumY - sumX * sumXY ) / denominator;
			return true;
		}

		public static PointD Rotate( PointD point, PointD pivot, double angleDegree )
		{
			double angle = angleDegree * Math.PI / 180;
			double cos = Math.Cos( angle );
			double sin = Math.Sin( angle );
			double dx = point.X - pivot.X;
			double dy = point.Y - pivot.Y;
			double x = cos * dx - sin * dy + pivot.X;
			double y = sin * dx + cos * dy + pivot.X;

			PointD rotated = new PointD( x, y, 0 );
			return rotated;
		}

		public static double Mean( double totalValue, int totalNumbers )
		{
			if ( totalValue == 0 || totalNumbers == 0 ) return 0;

			var mean = totalValue / totalNumbers;
			return mean;
		}

		public static double Mean( double[] value )
		{
			if ( value.Length == 0 ) return 0;
			double totalValue = 0;
			foreach ( var element in value )
				totalValue += element;
			return totalValue / value.Length;
		}
		public static double Mean( IEnumerable<double> Data )
		{
			return Statistics.Mean( Data );
		}
		public static double StandardDeviation( IEnumerable<double> Data )
		{
			return Statistics.StandardDeviation( Data );
		}
		public static double Median( double[] value )
		{
			if ( value.Length == 0 ) return 0;

			int count = value.Length;
			double[] tempValue = value;
			Array.Sort( tempValue );

			double medianValue = 0;
			if ( count % 2 == 0 ) // if count is even, select the both elements in the middle and divide by 2
			{
				double middleElement1 = tempValue[ ( count / 2 ) - 1 ];
				double middleElement2 = tempValue[ ( count / 2 ) ];
				medianValue = ( middleElement1 + middleElement2 ) / 2;
			}
			else // if odd, get the middle element.
				medianValue = tempValue[ ( count / 2 ) ];

			return medianValue;
		}
		public static bool IsPointInPolygon( PointD[] polygon, PointD point )
		{
			bool isInside = false;

			for ( int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++ )
			{
				if ( ( ( polygon[ i ].Y > point.Y ) != ( polygon[ j ].Y > point.Y ) ) &&
					( point.X < ( polygon[ j ].X - polygon[ i ].X ) * ( point.Y - polygon[ i ].Y ) / ( polygon[ j ].Y - polygon[ i ].Y ) + polygon[ i ].X ) )
				{
					isInside = !isInside;
				}
			}
			return isInside;
		}
		public static int ReadShorts( string path, int length, out ushort[] fileuShort )
		{
			fileuShort = null;
			try
			{
				using ( FileStream fs = new FileStream( path, FileMode.Open, FileAccess.Read ) )
				{
					fileuShort = new ushort[ length ];
					using ( BinaryReader bw = new BinaryReader( fs ) )
					{
						for ( long i = 0; i < ( length ); i++ )
							fileuShort[ i ] = bw.ReadUInt16();
					}
					fs.Close();
				}
			}
			catch
			{
			}
			return 0;
		}

		public static Task<double> SignalRadiusByCrossSec( List<(int, int)> Intensity, double PixelPitch, double threshold, int Angle )//pixel pitch in um, output width in mm
		{
			return Task.Run( () =>
			{
				if ( Intensity == null ) return 0;

				double calPixelPitch = 0;
				int StartIndex = 0, EndIndex = 0;

				var MaxIntensity = Intensity.Max( x => x.Item2 );


				for ( int i = 0; i < Intensity.Count() / 2; i++ )
				{
					if ( ( Convert.ToDouble( Intensity[ i ].Item2 ) / Convert.ToDouble( MaxIntensity ) ) > threshold )
					{
						StartIndex = i;
						break;
					}
				}
				if ( StartIndex == 0 )
					StartIndex = 1;
				var MirrorPoint = Intensity.Find( x => x.Item1 == -( Intensity[ StartIndex ].Item1 ) );
				var IndexofMirrorPoint = Intensity.IndexOf( MirrorPoint );
				if ( Intensity[ StartIndex ].Item2 > MirrorPoint.Item2 )
				{
					for ( int i = IndexofMirrorPoint; i > ( Intensity.Count() / 2 ); i-- )
					{
						if ( ( Convert.ToDouble( Intensity[ i ].Item2 ) / Convert.ToDouble( MaxIntensity ) ) > threshold )
						{
							EndIndex = i;
							break;
						}
					}
				}
				else if ( Intensity[ StartIndex ].Item2 < MirrorPoint.Item2 )
				{
					for ( int i = IndexofMirrorPoint; i < Intensity.Count; i++ )
					{
						if ( ( Convert.ToDouble( Intensity[ i ].Item2 ) / Convert.ToDouble( MaxIntensity ) ) < threshold )
						{
							EndIndex = i;
							break;
						}
					}
				}
				else
					EndIndex = Intensity.IndexOf( MirrorPoint );
				if ( EndIndex == 0 )
					EndIndex = Intensity.Count - 1;

				var AngleCheck = Math.Sin( ( double )( Angle * Math.PI / 180 ) );
				switch ( AngleCheck )
				{
					case 0:
					case 1:
					case -1:
						calPixelPitch = PixelPitch;
						break;
					default:
						calPixelPitch = Math.Sqrt( Math.Pow( PixelPitch, 2 ) + Math.Pow( PixelPitch, 2 ) );
						break;
				}
				calPixelPitch = PixelPitch;
				var SignalRadius = Math.Abs( ( Intensity[ EndIndex ].Item1 - Intensity[ StartIndex ].Item1 ) * calPixelPitch / ( 1000 * 2 ) );
				return SignalRadius;
			} );
		}
		public static Task<int[,]> UShortto2DArrayConverter( ushort[] Buff, int iw, int ih )
		{
			return Task.Run( () =>
			{
				int[,] array2d = new int[ iw, ih ];
				for ( int i = 0; i < iw; i++ )
					for ( int j = 0; j < ih; j++ )
						array2d[ i, j ] = Buff[ ( j * iw ) + i ];
				return array2d;
			} );
		}

		public static int[,] Byteto2DArrayConverter( byte[] Buff, int iw, int ih )
		{
			int[,] array2d = new int[ iw, ih ];
			for ( int i = 0; i < iw; i++ )
				for ( int j = 0; j < ih; j++ )
					array2d[ i, j ] = Buff[ ( j * iw ) + i ];
			return array2d;
		}


		#region decrement method
		public static List<double> IntWidthDecmm( int Radius, List<(int, int, int)> ImgArray, Point PointI, double Summation, List<double> Levels, double PixelWidth )
		{
			Dictionary<(int, int), int> FirstScanCheckPoints = new Dictionary<(int, int), int>();
			int FirstScan = 0;
			long FirstScanSum = 0;
			int FirstStart = Radius * 9 / 10;
			int FirstLimit = 0;
			int FirstStep = Radius / 10;
			var ScanLevelLimit = Levels.Max();
			List<(int, long, double)> FirstStepScanResult = new List<(int, long, double)>();
			for ( int i = FirstStart; i > FirstLimit; i -= FirstStep )
			{
				FirstScan = i;

				foreach ( var point in ImgArray )
				{
					if ( ( ( Math.Pow( point.Item1 - PointI.X, 2 ) + Math.Pow( point.Item2 - PointI.Y, 2 ) ) > Math.Pow( i, 2 ) ) && !FirstScanCheckPoints.ContainsKey( (point.Item1, point.Item2) ) )
					{
						FirstScanSum += point.Item3;
						FirstScanCheckPoints[ (point.Item1, point.Item2) ] = ( point.Item3 );
					}
				}
				FirstStepScanResult.Add( (FirstScan, FirstScanSum, FirstScanSum / Summation) );

				if ( ( FirstScanSum / Summation ) > ScanLevelLimit )//0.5054
					break;
			}




			List<double> ListOfWidth = new List<double>();

			foreach ( var Level in Levels )
			{

				#region SecondStep
				var ScanRadius = FirstStepScanResult.Where( x => x.Item3 >= Level ).First();

				int SecondScan = 0;
				long SecondScanSum = ScanRadius.Item2;
				int SecondStart = ScanRadius.Item1;
				int SecondLimit = ScanRadius.Item1 - FirstStep;
				var SecondStep = FirstStep / 10;
				if ( SecondStep < 1 )
					SecondStep = 1;
				var LevelLimit = Level;

				List<(int, long, double)> SecondStepScanResult = new List<(int, long, double)>();
				for ( int i = SecondStart; i > SecondLimit; i -= SecondStep )
				{
					SecondScan = i;//1027

					foreach ( var point in FirstScanCheckPoints )
					{
						if ( ( ( Math.Pow( point.Key.Item1 - PointI.X, 2 ) + Math.Pow( point.Key.Item2 - PointI.Y, 2 ) ) > Math.Pow( i, 2 ) ) && FirstScanCheckPoints.ContainsKey( (point.Key.Item1, point.Key.Item2) ) )
						{
							SecondScanSum -= point.Value;
							if ( ( SecondScanSum / Summation ) < LevelLimit )//0.441
								break;
						}
					}
					SecondStepScanResult.Add( (SecondScan, SecondScanSum, SecondScanSum / Summation) );
					if ( ( SecondScanSum / Summation ) < LevelLimit )//0.441
						break;
				}
				#endregion
				#region ThirdStep
				var ScanRadius2 = SecondStepScanResult.Where( x => x.Item3 <= Level ).Last();

				int ThirdScan = 0;
				long ThirdScanSum = 0;
				int ThirdStart = ScanRadius2.Item1;
				int ThirdLimit = ScanRadius2.Item1 + SecondStep;
				var ThirdStep = SecondStep / 10;
				if ( ThirdStep < 1 )
					ThirdStep = 1;
				//var LevelLimit = Level[ 1 ];

				Dictionary<(int, int), int> SecondScanCheckPoints = new Dictionary<(int, int), int>();
				for ( int i = ThirdStart - 1; i < ThirdLimit; i += ThirdStep )
				{
					ThirdScan = i;//1021

					foreach ( var point in FirstScanCheckPoints )
					{
						if ( ( ( Math.Pow( point.Key.Item1 - PointI.X, 2 ) + Math.Pow( point.Key.Item2 - PointI.Y, 2 ) ) <= Math.Pow( i, 2 ) ) && !SecondScanCheckPoints.ContainsKey( (point.Key.Item1, point.Key.Item2) ) )
						{
							SecondScanCheckPoints[ (point.Key.Item1, point.Key.Item2) ] = point.Value;
							ThirdScanSum += point.Value;
							if ( ( ThirdScanSum / Summation ) > Level )//0.5054
								break;
						}
					}
					if ( ( ThirdScanSum / Summation ) > Level )//0.5054
						break;
				}
				ListOfWidth.Add( ThirdScan * PixelWidth / 1000 );
				#endregion
			}
			return ListOfWidth;
		}
		#endregion

		#region incremental method
		public static Task<(ErrorClass EClass, string ErrorMessage, List<double> ListOfWidth)> IntWidthmm( int Radius, List<(int, int, int)> ImgArray, Point PointI, double Summation, List<double> Levels, double PixelWidth )
		{
			return Task.Run( () =>
			{
				var EClass = ErrorClass.OK;
				string ErrorMessage = string.Empty;
				var ListOfWidth = new List<double>();
				try
				{
					var FirstScanCheckPoints = new Dictionary<(int, int), (int, int)>();
					int FirstScan = 0;
					long FirstScanSum = 0;
					int FirstStart = Radius * 2 / 10;
					int FirstLimit = Radius;
					int FirstStep = Radius / 10;
					var ScanLevelLimit = Levels.Max();
					var FirstStepScanResult = new List<(int, int, long, double)>();
					int Band = 0;
					for ( int i = FirstStart; i < FirstLimit; i += FirstStep )
					{
						FirstScan = i;
						Band++;
						var Radius1 = Math.Pow( i, 2 );
						foreach ( var point in ImgArray )
						{
							if ( Math.Abs( point.Item1 - PointI.X ) > i ||
								Math.Abs( point.Item2 - PointI.Y ) > i ||
								FirstScanCheckPoints.ContainsKey( (point.Item1, point.Item2) ) ) continue;
							if ( ( Math.Pow( point.Item1 - PointI.X, 2 ) + Math.Pow( point.Item2 - PointI.Y, 2 ) ) <= Radius1 )
							{
								FirstScanSum += point.Item3;
								FirstScanCheckPoints[ (point.Item1, point.Item2) ] = (Band, point.Item3);
							}
						}
						FirstStepScanResult.Add( (Band, FirstScan, FirstScanSum, FirstScanSum / Summation) );

						if ( ( FirstScanSum / Summation ) > ScanLevelLimit )//0.5054
							break;
					}
					var Tasks = new Task<(ErrorClass EClass, string ErrorMessage, double Width)>[]
					{
						Second_ThirdStep(Levels[0],FirstStep,FirstStepScanResult,FirstScanCheckPoints,PointI,Summation,PixelWidth),
						Second_ThirdStep(Levels[1],FirstStep,FirstStepScanResult,FirstScanCheckPoints,PointI,Summation,PixelWidth),
					};
					Task.WaitAll( Tasks );
					foreach ( var t in Tasks )
					{
						if ( t.Result.EClass != ErrorClass.OK )
						{
							EClass = t.Result.EClass;
							throw new Exception( t.Result.ErrorMessage );
						}
						ListOfWidth.Add( t.Result.Width );
					}
				}
				catch ( Exception ex )
				{
					if ( EClass == ErrorClass.OK )
						EClass = ErrorClass.E6;
					ErrorMessage = $"MathExt.cs:IntWidthmm:,{ex.Message}";
				}

				return (EClass, ErrorMessage, ListOfWidth);
			} );
		}

		public static Task<(ErrorClass EClass, string ErrorMessage, double Width)> Second_ThirdStep( double Level, int FirstStep, List<(int, int, long, double)> FirstStepScanResult, Dictionary<(int, int), (int, int)> FirstScanCheckPoints, Point PointI, double Summation, double PixelWidth )
		{
			return Task.Run( () =>
			{
				ErrorClass EClass = ErrorClass.OK;
				string ErrorMessage = string.Empty;
				double Width = 0;
				try
				{
					#region SecondStep
					var ScanRadius = FirstStepScanResult.Where( x => x.Item4 <= Level ).Last();

					int SecondScan = 0;
					long SecondScanSum = ScanRadius.Item3;
					//long SecondScanSum = 0;
					int SecondStart = ScanRadius.Item2 + 1;
					int SecondLimit = ScanRadius.Item2 + FirstStep + 1;
					var SecondStep = FirstStep / 5;
					if ( SecondStep < 1 )
						SecondStep = 1;
					var LevelLimit = Level;

					List<(int, int, long, double)> SecondStepScanResult = new List<(int, int, long, double)>();
					Dictionary<(int, int), (int, int)> SecondScanCheckPoints = new Dictionary<(int, int), (int, int)>();
					int Band2 = 0;
					SecondStepScanResult.Add( (Band2, ScanRadius.Item2, ScanRadius.Item3, ScanRadius.Item4) );

					for ( int i = SecondStart; i < SecondLimit; i += SecondStep )
					{
						SecondScan = i;//1027
						Band2++;
						var Radius2 = Math.Pow( i, 2 );
						foreach ( var point in FirstScanCheckPoints )
						{
							if ( Math.Abs( point.Key.Item1 - PointI.X ) > i ||
								Math.Abs( point.Key.Item2 - PointI.Y ) > i ||
								SecondScanCheckPoints.ContainsKey( (point.Key.Item1, point.Key.Item2) ) ) continue;
							if ( point.Value.Item1 != ScanRadius.Item1 + 1 )
								continue;
							if ( ( Math.Pow( point.Key.Item1 - PointI.X, 2 ) + Math.Pow( point.Key.Item2 - PointI.Y, 2 ) ) <= Radius2 )
							{
								SecondScanCheckPoints[ (point.Key.Item1, point.Key.Item2) ] = (Band2, point.Value.Item2);
								SecondScanSum += point.Value.Item2;
							}
						}
						SecondStepScanResult.Add( (Band2, SecondScan, SecondScanSum, SecondScanSum / Summation) );
						if ( ( SecondScanSum / Summation ) > LevelLimit )//0.441
							break;
					}
					#endregion
					#region ThirdStep
					var ScanRadius2 = SecondStepScanResult.Where( x => x.Item4 <= Level ).Last();

					int ThirdScan = 0;
					long ThirdScanSum = ScanRadius2.Item3;
					int ThirdStart = ScanRadius2.Item2 + 1;
					int ThirdLimit = ScanRadius2.Item2 + SecondStep + 1;
					var ThirdStep = SecondStep / 5;
					if ( ThirdStep < 1 )
						ThirdStep = 1;

					List<(int, int, long, double)> ThirdStepScanResult = new List<(int, int, long, double)>();
					Dictionary<(int, int), (int, int)> ThirdScanCheckPoints = new Dictionary<(int, int), (int, int)>();
					int Band3 = 0;
					ThirdStepScanResult.Add( (Band3, ScanRadius2.Item2, ScanRadius2.Item3, ScanRadius2.Item4) );
					for ( int i = ThirdStart - 1; i < ThirdLimit; i += ThirdStep )
					{
						ThirdScan = i;//1021
						Band3++;
						var Radius2 = Math.Pow( i, 2 );


						foreach ( var point in SecondScanCheckPoints )
						{
							if ( Math.Abs( point.Key.Item1 - PointI.X ) > i ||
								Math.Abs( point.Key.Item2 - PointI.Y ) > i ||
								ThirdScanCheckPoints.ContainsKey( (point.Key.Item1, point.Key.Item2) ) ) continue;
							if ( point.Value.Item1 != ScanRadius2.Item1 + 1 )
								continue;
							if ( ( ( Math.Pow( point.Key.Item1 - PointI.X, 2 ) + Math.Pow( point.Key.Item2 - PointI.Y, 2 ) ) <= Radius2 ) )
							{
								ThirdScanCheckPoints[ (point.Key.Item1, point.Key.Item2) ] = (Band3, point.Value.Item2);
								ThirdScanSum += point.Value.Item2;
								if ( ( ThirdScanSum / Summation ) > Level )//0.5054
									break;
							}
						}
						if ( ( ThirdScanSum / Summation ) > Level )//0.5054
							break;
					}
					Width = ThirdScan * PixelWidth / 1000;
					#endregion
					//#region FourthStep
					//var ScanRadius3 = ThirdStepScanResult.Where( x => x.Item4 <= Level ).Last();

					//int FourthScan = 0;
					//long FourthScanSum = ScanRadius3.Item3;
					//int FourthStart = ScanRadius3.Item2 + 1;
					//int FourthLimit = ScanRadius3.Item2 + ThirdStep + 1;
					//var FourthStep = ThirdStep / 3;
					//if ( FourthStep < 1 )
					//	FourthStep = 1;

					//List<(int, int, long, double)> FourthStepScanResult = new List<(int, int, long, double)>();
					//Dictionary<(int, int), (int, int)> FourthScanCheckPoints = new Dictionary<(int, int), (int, int)>();
					//int Band4 = 0;
					//FourthStepScanResult.Add( (Band4, ScanRadius3.Item2, ScanRadius3.Item3, ScanRadius3.Item4) );
					//for ( int i = FourthStart - 1; i < FourthLimit; i += FourthStep )
					//{
					//	FourthScan = i;//1021
					//	Band4++;
					//	var Radius3 = Math.Pow( i, 2 );

					//	foreach ( var point in ThirdScanCheckPoints )
					//	{
					//		if ( Math.Abs( point.Key.Item1 - PointI.X ) > i ||
					//			Math.Abs( point.Key.Item2 - PointI.Y ) > i ||
					//			FourthScanCheckPoints.ContainsKey( (point.Key.Item1, point.Key.Item2) ) ) continue;
					//		if ( point.Value.Item1 != ScanRadius3.Item1 + 1 )
					//			continue;
					//		if ( ( ( Math.Pow( point.Key.Item1 - PointI.X, 2 ) + Math.Pow( point.Key.Item2 - PointI.Y, 2 ) ) <= Radius3 ) )
					//		{
					//			FourthScanCheckPoints[ (point.Key.Item1, point.Key.Item2) ] = (Band4, point.Value.Item2);
					//			FourthScanSum += point.Value.Item2;
					//			if ( ( FourthScanSum / Summation ) > Level )//0.5054
					//				break;
					//		}
					//	}
					//	if ( ( FourthScanSum / Summation ) > Level )//0.5054
					//		break;
					//}
					//ListOfWidth.Add( FourthScan * PixelWidth / 1000 );
					//#endregion
				}
				catch ( Exception ex )
				{
					if ( EClass == ErrorClass.OK )
					{
						EClass = ErrorClass.E6;
					}
					ErrorMessage = $"ShortPulseModule.cs:SetVoltageOut:,{ex.Message}";
				}
				return (EClass, ErrorMessage, Width);
			} );
		}
		public static List<int> IntWidthPxl( int Radius, Dictionary<(int, int), int> Array2D, Point PointI, double Summation, List<double> Levels )
		{
			Dictionary<(int, int), int> FirstScanCheckPoints = new Dictionary<(int, int), int>();
			int FirstScan = 0;
			long FirstScanSum = 0;
			int FirstStart = Radius / 10;
			int FirstLimit = Radius;
			int FirstStep = Radius / 10;
			var ScanLevelLimit = Levels.Max();
			List<(int, long, double)> FirstStepScanResult = new List<(int, long, double)>();
			for ( int i = FirstStart; i < FirstLimit; i += FirstStep )
			{
				FirstScan = i;
				var RadSqr = Math.Pow( i, 2 );
				for ( int x = PointI.X - i + 2; x <= PointI.X + i - 2; x++ )
				{
					var XSquare = Math.Pow( x - PointI.X, 2 );
					for ( int y = PointI.Y - i + 2; x <= PointI.Y + i - 2; y++ )
					{
						if ( FirstScanCheckPoints.ContainsKey( (x, y) ) )
							continue;
						if ( ( ( XSquare + Math.Pow( y - PointI.Y, 2 ) ) <= RadSqr ) )
						{
							FirstScanSum += Array2D[ (x, y) ];
							FirstScanCheckPoints[ (x, y) ] = Array2D[ (x, y) ];
							if ( ( FirstScanSum / Summation ) > ScanLevelLimit )//0.5054
								break;
						}
					}
				}

				FirstStepScanResult.Add( (FirstScan, FirstScanSum, FirstScanSum / Summation) );

				if ( ( FirstScanSum / Summation ) > ScanLevelLimit )//0.5054
					break;
			}




			List<int> ListOfWidth = new List<int>();

			foreach ( var Level in Levels )
			{

				#region SecondStep
				var ScanRadius = FirstStepScanResult.Where( x => x.Item3 >= Level ).First();

				int SecondScan = 0;
				long SecondScanSum = ScanRadius.Item2;
				int SecondStart = ScanRadius.Item1;
				int SecondLimit = ScanRadius.Item1 - FirstStep;
				var SecondStep = FirstStep / 10;
				if ( SecondStep < 1 )
					SecondStep = 1;
				var LevelLimit = Level;

				List<(int, long, double)> SecondStepScanResult = new List<(int, long, double)>();
				for ( int i = SecondStart; i > SecondLimit; i -= SecondStep )
				{
					SecondScan = i;//1027

					var RadSqr = Math.Pow( i, 2 );
					for ( int x = PointI.X - i + 2; x <= PointI.X + i - 2; x++ )
					{
						var XSquare = Math.Pow( x - PointI.X, 2 );
						for ( int y = PointI.Y - i + 2; x <= PointI.Y + i - 2; y++ )
						{
							if ( !FirstScanCheckPoints.ContainsKey( (x, y) ) )
								continue;
							if ( ( ( XSquare + Math.Pow( y - PointI.Y, 2 ) ) > RadSqr ) )
							{
								SecondScanSum -= Array2D[ (x, y) ];
								if ( ( SecondScanSum / Summation ) < LevelLimit )//0.441
									break;
							}
						}
					}
					SecondStepScanResult.Add( (SecondScan, SecondScanSum, SecondScanSum / Summation) );
					if ( ( SecondScanSum / Summation ) < LevelLimit )//0.441
						break;
				}
				#endregion
				#region ThirdStep
				var ScanRadius2 = SecondStepScanResult.Where( x => x.Item3 <= Level ).Last();

				int ThirdScan = 0;
				long ThirdScanSum = 0;
				int ThirdStart = ScanRadius2.Item1;
				int ThirdLimit = ScanRadius2.Item1 + SecondStep;
				var ThirdStep = SecondStep / 10;
				if ( ThirdStep < 1 )
					ThirdStep = 1;

				Dictionary<(int, int), int> SecondScanCheckPoints = new Dictionary<(int, int), int>();
				for ( int i = ThirdStart - 1; i < ThirdLimit; i += ThirdStep )
				{
					ThirdScan = i;//1021

					var RadSqr = Math.Pow( i, 2 );
					for ( int x = PointI.X - i + 2; x <= PointI.X + i - 2; x++ )
					{
						var XSquare = Math.Pow( x - PointI.X, 2 );
						for ( int y = PointI.Y - i + 2; x <= PointI.Y + i - 2; y++ )
						{
							if ( SecondScanCheckPoints.ContainsKey( (x, y) ) )
								continue;
							if ( ( ( XSquare + Math.Pow( y - PointI.Y, 2 ) ) <= RadSqr ) )
							{
								SecondScanCheckPoints[ (x, y) ] = Array2D[ (x, y) ];
								ThirdScanSum += Array2D[ (x, y) ];
								if ( ( ThirdScanSum / Summation ) > Level )//0.5054
									break;
							}
						}
					}
					if ( ( ThirdScanSum / Summation ) > Level )//0.5054
						break;
				}
				ListOfWidth.Add( ThirdScan );
				#endregion
			}
			return ListOfWidth;
		}
		#endregion



		public static Task<int> MinOfDip( Point MaxLoc, Point CenterLoc, List<(int, int, int)> ArrayIMG )
		{
			return Task.Run( () =>
			{
				var MaxRingRadius = Math.Sqrt( Math.Pow( MaxLoc.X - CenterLoc.X, 2 ) + Math.Pow( MaxLoc.Y - CenterLoc.Y, 2 ) );
				var Cropped = ArrayIMG.Where( x => x.Item1 >= ( CenterLoc.X - MaxRingRadius ) &&
													x.Item1 <= ( CenterLoc.X + MaxRingRadius ) &&
													x.Item2 >= ( CenterLoc.Y - MaxRingRadius ) &&
													x.Item2 <= ( CenterLoc.Y + MaxRingRadius ) );
				return Cropped.Where( x => ( Math.Pow( x.Item1 - CenterLoc.X, 2 ) + Math.Pow( x.Item2 - CenterLoc.Y, 2 ) ) <= MaxRingRadius ).Min( x => x.Item3 );
			} );
		}

		public static Task<(ErrorClass EClass, string ErrorMessage, List<(int, int, int)> ListImg, Point MaxLoc, int Max, long Summation)> ConverttoList( ushort[] ListOfArray, int iw, int ih )
		{
			return Task.Run( () =>
			{
				long Summation = 0;
				int Max = 0;
				Point MaxLoc = new Point();
				List<(int, int, int)> ListImg = new List<(int, int, int)>();
				for ( int i = 0; i < iw; i++ )
					for ( int j = 0; j < ih; j++ )
						if ( ( ListOfArray[ ( j * iw ) + i ] ) != 0 )
						{
							ListImg.Add( (i, j, ListOfArray[ ( j * iw ) + i ]) );
							Summation += ListOfArray[ ( j * iw ) + i ];
							if ( Max < ListOfArray[ ( j * iw ) + i ] )
							{
								Max = ListOfArray[ ( j * iw ) + i ];
								MaxLoc.X = i;
								MaxLoc.Y = j;
							}
						}
				return (ErrorClass.OK, "", ListImg, MaxLoc, Max, Summation);
			} );
		}
	}
}
