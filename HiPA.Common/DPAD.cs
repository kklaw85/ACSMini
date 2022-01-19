using HiPA.Common.Forms;
using System;
using System.Collections.ObjectModel;

namespace HiPA.Common
{
	[Serializable]
	public class DPAD : RecipeBaseUtility
	{
		public static readonly ReadOnlyCollection<double> Interval =
new ReadOnlyCollection<double>( new[]
{
		 0.001, 0.002, 0.005,
		 0.01, 0.02, 0.05,
		 0.1, 0.2, 0.5,
		 1, 2, 5,
		 10, 20, 50
} );
		public int BuffWidth { get; set; } = 640;
		public int BuffHeight { get; set; } = 480;
		public double X_Interval
		{
			get => this.GetValue( () => this.X_Interval );
			set => this.SetValue( () => this.X_Interval, value );
		}
		public double Y_Interval
		{
			get => this.GetValue( () => this.Y_Interval );
			set => this.SetValue( () => this.Y_Interval, value );
		}
		public double XOffsetmm
		{
			get => this.GetValue( () => this.XOffsetmm );
			set => this.SetValue( () => this.XOffsetmm, value );
		}
		public double YOffsetmm
		{
			get => this.GetValue( () => this.YOffsetmm );
			set => this.SetValue( () => this.YOffsetmm, value );
		}
		public int XOffsetPix //offset from center
		{
			get => this.GetValue( () => this.XOffsetPix );
			set
			{
				this.SetValue( () => this.XOffsetPix, value );
				if ( this.ScalePixperMM.X == 0 ) this.ScalePixperMM.X = 1;
				this.XOffsetmm = this.XOffsetPix / this.ScalePixperMM.X;
			}
		}
		public int YOffsetPix //offset from center
		{
			get => this.GetValue( () => this.YOffsetPix );
			set
			{
				this.SetValue( () => this.YOffsetPix, value );
				if ( this.ScalePixperMM.Y == 0 ) this.ScalePixperMM.Y = 1;
				this.YOffsetmm = this.YOffsetPix / this.ScalePixperMM.Y;
			}
		}
		public C_PointD ScalePixperMM { get; set; } = new C_PointD();
		public void IncrementX()
		{
			if ( this.ScalePixperMM.X == 0 ) this.ScalePixperMM.X = 1;
			this.XOffsetPix += ( int )Math.Round( this.X_Interval * this.ScalePixperMM.X, 0 );
		}
		public void DecrementX()
		{
			if ( this.ScalePixperMM.X == 0 ) this.ScalePixperMM.X = 1;
			this.XOffsetPix -= ( int )Math.Round( this.X_Interval * this.ScalePixperMM.X, 0 );
		}
		public void IncrementY()
		{
			if ( this.ScalePixperMM.Y == 0 ) this.ScalePixperMM.Y = 1;
			this.YOffsetPix += ( int )Math.Round( this.Y_Interval * this.ScalePixperMM.Y, 0 );
		}
		public void DecrementY()
		{
			if ( this.ScalePixperMM.Y == 0 ) this.ScalePixperMM.Y = 1;
			this.YOffsetPix -= ( int )Math.Round( this.Y_Interval * this.ScalePixperMM.Y, 0 );
		}
		public void ClearOffset()
		{
			this.XOffsetPix = 0;
			this.YOffsetPix = 0;
		}
		public void SetCrossHairPos( C_PointD Offset )
		{
			this.XOffsetPix = ( int )Math.Round( Offset.X, 0 );
			this.YOffsetPix = ( int )Math.Round( Offset.Y, 0 );
		}
	}
	public class ROIRectangle : RecipeBaseUtility
	{
		public int X
		{
			get => this.GetValue( () => this.X );
			set => this.SetValue( () => this.X, value );
		}
		public int Y
		{
			get => this.GetValue( () => this.Y );
			set => this.SetValue( () => this.Y, value );
		}
		public int Width
		{
			get => this.GetValue( () => this.Width );
			set => this.SetValue( () => this.Width, value );
		}
		public int Height
		{
			get => this.GetValue( () => this.Height );
			set => this.SetValue( () => this.Height, value );
		}
		public ROIRectangle( int x, int y, int width, int height )
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}
		public ROIRectangle( ROIRectangle source )
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Width = source.Width;
			this.Height = source.Height;
		}
		public void Update( ROIRectangle source )
		{
			this.X = source.X;
			this.Y = source.Y;
			this.Width = source.Width;
			this.Height = source.Height;
		}
		public void Update( int x, int y, int width, int height )
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}
		public ROIRectangle()
		{ }
	}
}
