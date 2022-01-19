namespace Utilities
{
	class Validator
	{

		public static bool IsValid( string str, bool AllowSpace, int MaxLen )
		{
			if ( str == null || str == "" ) return false;
			if ( str.Length > MaxLen ) return false;

			//checking for conatin space
			if ( !AllowSpace )
				if ( str.Contains( " " ) )
					return false;

			//checking for all space character
			for ( int i = 0; i < str.Length; i++ )
			{
				if ( str[ i ] != ' ' )
					break;

				//All space
				if ( i == str.Length - 1 )
					return false;
			}

			return true;
		}
		public static bool IsValid( string val, decimal MinVal, decimal MaxVal )
		{
			if ( val == null || val == "" ) return false;

			decimal temp = 0;
			if ( decimal.TryParse( val, out temp ) )
			{
				if ( temp >= MinVal && temp <= MaxVal )
					return true;
				else
					return false;
			}
			else
				return false;
		}
		public static bool IsValid( string val, int MinVal, int MaxVal )
		{
			if ( val == null || val == "" ) return false;

			int temp = 0;
			if ( int.TryParse( val, out temp ) )
			{
				if ( temp >= MinVal && temp <= MaxVal )
					return true;
				else
					return false;
			}
			else
				return false;
		}
		public static bool IsValid( string val, float MinVal, float MaxVal )
		{
			if ( val == null || val == "" ) return false;

			float temp = 0;
			if ( float.TryParse( val, out temp ) )
			{
				if ( temp >= MinVal && temp <= MaxVal )
					return true;
				else
					return false;
			}
			else
				return false;
		}
		public static bool IsValid( string val, double MinVal, double MaxVal )
		{
			if ( val == null || val == "" ) return false;

			double temp = 0;
			if ( double.TryParse( val, out temp ) )
			{
				if ( temp >= MinVal && temp <= MaxVal )
					return true;
				else
					return false;
			}
			else
				return false;
		}
	}
}
