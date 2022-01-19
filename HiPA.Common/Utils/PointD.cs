using System;

using System.Drawing;

namespace HiPA.Common.Utils
{
	[Serializable]
	public struct PointD
	{
		double x;
		public double X { get => this.x; set => this.x = value; }
		double y;
		public double Y { get => this.y; set => this.y = value; }

		public bool IsEmpty => this.x == 0d && this.y == 0d;

		public PointD( double x, double y )
		{
			this.x = x;
			this.y = y;
		}
		public PointD( PointD point )
			: this( point.x, point.y )
		{
		}
		public PointD( PointF point )
			: this( point.X, point.Y )
		{
		}

		/// <summary>Translates a <see cref="T:System.Drawing.PointD" /> by a given <see cref="T:System.Drawing.Size" />.</summary>
		/// <param name="pt">The <see cref="T:System.Drawing.PointD" /> to translate.</param>
		/// <param name="sz">A <see cref="T:System.Drawing.Size" /> that specifies the pair of numbers to add to the coordinates of <paramref name="pt" />.</param>
		/// <returns>The translated <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x0600065B RID: 1627 RVA: 0x0001A9EA File Offset: 0x00018BEA
		public static PointD operator +( PointD pt, Size sz )
		{
			return PointD.Add( pt, sz );
		}

		/// <summary>Translates a <see cref="T:System.Drawing.PointD" /> by the negative of a given <see cref="T:System.Drawing.Size" />.</summary>
		/// <param name="pt">The <see cref="T:System.Drawing.PointD" /> to translate.</param>
		/// <param name="sz">The <see cref="T:System.Drawing.Size" /> that specifies the numbers to subtract from the coordinates of <paramref name="pt" />.</param>
		/// <returns>The translated <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x0600065C RID: 1628 RVA: 0x0001A9F3 File Offset: 0x00018BF3
		public static PointD operator -( PointD pt, Size sz )
		{
			return PointD.Subtract( pt, sz );
		}

		/// <summary>Translates the <see cref="T:System.Drawing.PointD" /> by the specified <see cref="T:System.Drawing.SizeF" />.</summary>
		/// <param name="pt">The <see cref="T:System.Drawing.PointD" /> to translate.</param>
		/// <param name="sz">The <see cref="T:System.Drawing.SizeF" /> that specifies the numbers to add to the x- and y-coordinates of the <see cref="T:System.Drawing.PointD" />.</param>
		/// <returns>The translated <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x0600065D RID: 1629 RVA: 0x0001A9FC File Offset: 0x00018BFC
		public static PointD operator +( PointD pt, SizeF sz )
		{
			return PointD.Add( pt, sz );
		}

		/// <summary>Translates a <see cref="T:System.Drawing.PointD" /> by the negative of a specified <see cref="T:System.Drawing.SizeF" />.</summary>
		/// <param name="pt">The <see cref="T:System.Drawing.PointD" /> to translate.</param>
		/// <param name="sz">The <see cref="T:System.Drawing.SizeF" /> that specifies the numbers to subtract from the coordinates of <paramref name="pt" />.</param>
		/// <returns>The translated <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x0600065E RID: 1630 RVA: 0x0001AA05 File Offset: 0x00018C05
		public static PointD operator -( PointD pt, SizeF sz )
		{
			return PointD.Subtract( pt, sz );
		}

		/// <summary>Compares two <see cref="T:System.Drawing.PointD" /> structures. The result specifies whether the values of the <see cref="P:System.Drawing.PointD.X" /> and <see cref="P:System.Drawing.PointD.Y" /> properties of the two <see cref="T:System.Drawing.PointD" /> structures are equal.</summary>
		/// <param name="left">A <see cref="T:System.Drawing.PointD" /> to compare.</param>
		/// <param name="right">A <see cref="T:System.Drawing.PointD" /> to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Drawing.PointD.X" /> and <see cref="P:System.Drawing.PointD.Y" /> values of the left and right <see cref="T:System.Drawing.PointD" /> structures are equal; otherwise, <see langword="false" />.</returns>
		// Token: 0x0600065F RID: 1631 RVA: 0x0001AA0E File Offset: 0x00018C0E
		public static bool operator ==( PointD left, PointD right )
		{
			return left.X == right.X && left.Y == right.Y;
		}

		/// <summary>Determines whether the coordinates of the specified points are not equal.</summary>
		/// <param name="left">A <see cref="T:System.Drawing.PointD" /> to compare.</param>
		/// <param name="right">A <see cref="T:System.Drawing.PointD" /> to compare.</param>
		/// <returns>
		///   <see langword="true" /> to indicate the <see cref="P:System.Drawing.PointD.X" /> and <see cref="P:System.Drawing.PointD.Y" /> values of <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
		// Token: 0x06000660 RID: 1632 RVA: 0x0001AA32 File Offset: 0x00018C32
		public static bool operator !=( PointD left, PointD right )
		{
			return !( left == right );
		}

		/// <summary>Translates a given <see cref="T:System.Drawing.PointD" /> by the specified <see cref="T:System.Drawing.Size" />.</summary>
		/// <param name="pt">The <see cref="T:System.Drawing.PointD" /> to translate.</param>
		/// <param name="sz">The <see cref="T:System.Drawing.Size" /> that specifies the numbers to add to the coordinates of <paramref name="pt" />.</param>
		/// <returns>The translated <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x06000661 RID: 1633 RVA: 0x0001AA3E File Offset: 0x00018C3E
		public static PointD Add( PointD pt, Size sz )
		{
			return new PointD( pt.X + ( float )sz.Width, pt.Y + ( float )sz.Height );
		}

		/// <summary>Translates a <see cref="T:System.Drawing.PointD" /> by the negative of a specified size.</summary>
		/// <param name="pt">The <see cref="T:System.Drawing.PointD" /> to translate.</param>
		/// <param name="sz">The <see cref="T:System.Drawing.Size" /> that specifies the numbers to subtract from the coordinates of <paramref name="pt" />.</param>
		/// <returns>The translated <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x06000662 RID: 1634 RVA: 0x0001AA65 File Offset: 0x00018C65
		public static PointD Subtract( PointD pt, Size sz )
		{
			return new PointD( pt.X - ( float )sz.Width, pt.Y - ( float )sz.Height );
		}

		/// <summary>Translates a given <see cref="T:System.Drawing.PointD" /> by a specified <see cref="T:System.Drawing.SizeF" />.</summary>
		/// <param name="pt">The <see cref="T:System.Drawing.PointD" /> to translate.</param>
		/// <param name="sz">The <see cref="T:System.Drawing.SizeF" /> that specifies the numbers to add to the coordinates of <paramref name="pt" />.</param>
		/// <returns>The translated <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x06000663 RID: 1635 RVA: 0x0001AA8C File Offset: 0x00018C8C
		public static PointD Add( PointD pt, SizeF sz )
		{
			return new PointD( pt.X + sz.Width, pt.Y + sz.Height );
		}

		/// <summary>Translates a <see cref="T:System.Drawing.PointD" /> by the negative of a specified size.</summary>
		/// <param name="pt">The <see cref="T:System.Drawing.PointD" /> to translate.</param>
		/// <param name="sz">The <see cref="T:System.Drawing.SizeF" /> that specifies the numbers to subtract from the coordinates of <paramref name="pt" />.</param>
		/// <returns>The translated <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x06000664 RID: 1636 RVA: 0x0001AAB1 File Offset: 0x00018CB1
		public static PointD Subtract( PointD pt, SizeF sz )
		{
			return new PointD( pt.X - sz.Width, pt.Y - sz.Height );
		}

		/// <summary>Specifies whether this <see cref="T:System.Drawing.PointD" /> contains the same coordinates as the specified <see cref="T:System.Object" />.</summary>
		/// <param name="obj">The <see cref="T:System.Object" /> to test.</param>
		/// <returns>This method returns <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.Drawing.PointD" /> and has the same coordinates as this <see cref="T:System.Drawing.Point" />.</returns>
		// Token: 0x06000665 RID: 1637 RVA: 0x0001AAD8 File Offset: 0x00018CD8
		public override bool Equals( object obj )
		{
			if ( !( obj is PointD ) )
			{
				return false;
			}
			PointD pointF = ( PointD )obj;
			return pointF.X == this.X && pointF.Y == this.Y && pointF.GetType().Equals( base.GetType() );
		}
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


		/// <summary>Converts this <see cref="T:System.Drawing.PointD" /> to a human readable string.</summary>
		/// <returns>A string that represents this <see cref="T:System.Drawing.PointD" />.</returns>
		// Token: 0x06000667 RID: 1639 RVA: 0x0001AB48 File Offset: 0x00018D48
		public override string ToString()
		{
			return string.Format( System.Globalization.CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[]
			{
				this.x,
				this.y
			} );
		}

		/// <summary>Represents a new instance of the <see cref="T:System.Drawing.PointD" /> class with member data left uninitialized.</summary>
		// Token: 0x0400056C RID: 1388
		public static readonly PointD Empty;
	}



}
