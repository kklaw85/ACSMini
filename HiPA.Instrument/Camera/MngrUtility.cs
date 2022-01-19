using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace HiPA.Instrument.Camera
{
	public static class MngrUtility
	{
		/// <summary>
		/// Convert image from bitmap format to 1D byte array
		/// </summary>
		/// <param name="image">Input image in bitmap format</param>
		/// <param name="OutArray">Output image in 1D byte array</param>
		/// <returns></returns>
		public static string ConvertBitmapTo1DByteArray( Bitmap image, ref byte[] OutArray )
		{
			string sErr = string.Empty;
			try
			{
				byte[] result = new byte[ image.Width * image.Height ];

				BitmapData bmpData = image.LockBits( new Rectangle( 0, 0, image.Width, image.Height ),
					ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format24bppRgb );
				unsafe
				{
					byte* ptr = ( byte* )bmpData.Scan0;

					int remain = bmpData.Stride - bmpData.Width * 3;

					for ( int i = 0, iShift = 0; i < bmpData.Height; i++, iShift = i * bmpData.Width )
					{
						for ( int j = 0; j < bmpData.Width; j++, iShift++ )
						{
							byte n1 = ptr[ 0 ];

							result[ iShift ] = n1;

							ptr += 3;
						}
						ptr += remain;
					}

				}
				image.UnlockBits( bmpData );

				OutArray = result;
			}
			catch ( Exception ex )
			{
				sErr = string.Format( "ConvertBitmapTo1DByteArray error: " + ex.Message );
				JPTUtility.Logger.doLog( sErr );
			}

			return sErr;
		}

		public static Bitmap ConvertByteArrayToImageMonoFormat( byte[] array, int width, int height )
		{
			try
			{
				Bitmap bmp = new Bitmap( width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed );
				BitmapData bmpData = bmp.LockBits( new Rectangle( 0, 0, width, height ),
							  ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed );
				int stride = bmpData.Stride;
				int offset = stride - width;
				IntPtr iptr = bmpData.Scan0;
				int scanBytes = stride * height;

				int posScan = 0, posReal = 0;
				byte[] pixelValues = new byte[ scanBytes ];

				for ( int x = 0; x < height; x++ )
				{
					for ( int y = 0; y < width; y++ )
					{
						pixelValues[ posScan++ ] = array[ posReal++ ];
					}
					posScan += offset;
				}
				Marshal.Copy( pixelValues, 0, iptr, scanBytes );
				bmp.UnlockBits( bmpData );

				ColorPalette tempPalette;
				using ( Bitmap tempBmp = new Bitmap( 1, 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed ) )
				{
					tempPalette = tempBmp.Palette;
				}
				for ( int i = 0; i < 256; i++ )
				{
					tempPalette.Entries[ i ] = System.Drawing.Color.FromArgb( i, i, i );
				}

				bmp.Palette = tempPalette;

				return bmp;
			}
			catch
			{
				JPTUtility.Logger.doLog( "ConvertByteArrayToImageMonoFormat error " );
				return null;
			}
		}

		public static string LaunchMtxCodeReader( string MCOFile )
		{
			string sErr = string.Empty;
			try
			{
				JptMtxCodeReader.CodeReader code = new JptMtxCodeReader.CodeReader();
				code.LoadCodeReaderWindow();
			}
			catch { }
			return sErr;

		}




	}
}
