using HiPA.Common;
using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace N_Data_Utilities
{
	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayMinValDouble : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempfloat = 0;
			if ( double.TryParse( value.ToString(), out tempfloat ) )
			{
				double fValue = ( double )value;
				string Val = fValue == 100000 ? "NAN" : fValue.ToString( "F3" );
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayMaxValDouble : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempfloat = 0;
			if ( double.TryParse( value.ToString(), out tempfloat ) )
			{
				double fValue = ( double )value;
				string Val = fValue == -100000 ? "NAN" : fValue.ToString( "F3" );
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayThreeDecPlacesDouble : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempfloat = 0;
			if ( double.TryParse( value.ToString(), out tempfloat ) )
			{
				double fValue = ( double )value;
				string Val = fValue.ToString( "F3" );
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplaySixDecPlacesDouble : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempfloat = 0;
			if ( double.TryParse( value.ToString(), out tempfloat ) )
			{
				double fValue = ( double )value;
				string Val = fValue.ToString( "F6" );
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}
	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayDiodeCurrentAmp : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				string Val = fValue == -10 ? "UNKNOWN" : fValue.ToString( "F2" ) + " A";
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}


	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayDiodeCurrent : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				string Val = fValue == -10 ? "UNKNOWN" : fValue.ToString( "F3" ) + " %";
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayDiodeUsage : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				string Val = fValue == -10 ? "UNKNOWN" : fValue.ToString( "F2" ) + " Hours";
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayDiodeTemp : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				string Val = fValue == -10 ? "UNKNOWN" : fValue.ToString( "F2" ) + " Degree";
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayTwoDecPlacesDouble : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempfloat = 0;
			if ( double.TryParse( value.ToString(), out tempfloat ) )
			{
				double fValue = ( double )value;
				string Val = fValue.ToString( "F2" );
				return Val;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double dValue;
			double.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}


	[ValueConversion( typeof( float ), typeof( string ) )]
	public class Sec_Per_Sub : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				if ( fValue < 2 )
				{
					fValue = 0;
				}
				return fValue.ToString( "F2" ) + " Sec";
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float dValue;
			float.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayTwoDecPlaces : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				return fValue.ToString( "F2" );
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float dValue;
			float.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DataGrid_Edit : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				return ( string )value;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float dValue;
			float.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}


	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayPower_Val_WithUnit : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				return fValue.ToString( "F3" ) + " Watt";
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			float dValue;
			float.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayCom_Off_Val_WithUnit : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				string str = fValue.ToString( "F2" ) + " %";
				return str;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			float dValue;
			float.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayOneDecPlacesAndPcnt : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				return fValue.ToString( "F1" ) + "%";
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float dValue;
			float.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayTwoDecPlacesAndPcnt : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				return fValue.ToString( "F2" ) + "%";
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float dValue;
			float.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayThreeDecPlacesAndPcnt : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				return fValue.ToString( "F3" ) + "%";
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float dValue;
			float.TryParse( ( string )value, out dValue );
			return dValue;
		}
	}

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayNoDecimalPlaces : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				return fValue.ToString( "F0" );
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float fValue;
			float.TryParse( ( string )value, out fValue );
			return fValue;
		}
	}

	[ValueConversion( typeof( int ), typeof( string ) )]
	public class DisplayIntToString : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			int tempint = 0;
			if ( int.TryParse( value.ToString(), out tempint ) )
			{
				int fValue = ( int )value;
				return fValue.ToString();
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			int iValue;
			int.TryParse( ( string )value, out iValue );
			return iValue;
		}
	}
	[ValueConversion( typeof( double ), typeof( string ) )]
	public class DisplayNoDecimalPlacesDouble : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempdouble = 0;
			if ( double.TryParse( value.ToString(), out tempdouble ) )
			{
				double fValue = ( double )value;
				return fValue.ToString( "F0" );
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double fValue;
			double.TryParse( ( string )value, out fValue );
			return fValue;
		}
	}

	//[ValueConversion( typeof( Machine_State ), typeof( bool ) )]
	//public class MachineStateToBoolInterLock : IValueConverter
	//{
	//	public object Convert( object value, Type targetType,
	//	object parameter, System.Globalization.CultureInfo culture )
	//	{
	//		Machine_State s = ( Machine_State )value;
	//		if ( s == Machine_State.AUTO_RUNNING ||
	//		   s == Machine_State.SEMI_AUTO_RUNNING ||
	//		   s == Machine_State.WORK_IN_PROGRESS ||
	//		   s == Machine_State.AUTO_RUN_ERROR ||
	//		   s == Machine_State.AUTO_COMMUNICATION_W_S_CORE_INPROGRESS ||
	//		   s == Machine_State.SEMI_AUTO_COMMUNICATION_W_S_CORE_INPROGRESS ||
	//		   s == Machine_State.SYSTEM_HOMING )
	//			return false;

	//		return true;
	//	}
	//	public object ConvertBack( object value, Type targetType,
	//	object parameter, System.Globalization.CultureInfo culture )
	//	{
	//		throw new Exception( "Not Implemented" );
	//	}
	//}
	[ValueConversion( typeof( float ), typeof( string ) )]
	public class DisplayThreeDecPlaces : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float tempfloat = 0;
			if ( float.TryParse( value.ToString(), out tempfloat ) )
			{
				float fValue = ( float )value;
				return fValue.ToString( "F3" );
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float fValue;
			float.TryParse( ( string )value, out fValue );
			return fValue;
		}
	}

	[ValueConversion( typeof( bool ), typeof( Style ) )]
	public class Theta_Trim_Pos_Bool_To_Style : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )( value );
			if ( b )
				return Application.Current.Resources[ "Button_CyanContent" ];
			else
				return Application.Current.Resources[ "Button_CyanContent_Red_Highlight" ];
			//return Brushes.Gray;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			float fValue;
			float.TryParse( ( string )value, out fValue );
			return fValue;
		}
	}
	[ValueConversion( typeof( bool ), typeof( Visibility ) )]
	public class BoolToVisibility : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
				return tempBool ? Visibility.Visible : Visibility.Collapsed;
			return Visibility.Visible;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			Visibility vs = ( Visibility )value;
			return vs == Visibility.Visible ? true : false;
		}
	}

	[ValueConversion( typeof( bool ), typeof( Visibility ) )]
	public class BoolToVisibilityReverse : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
				return tempBool ? Visibility.Collapsed : Visibility.Visible;
			return Visibility.Collapsed;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			Visibility vs = ( Visibility )value;
			return vs == Visibility.Collapsed ? true : false;
		}
	}


	[ValueConversion( typeof( bool ), typeof( Visibility ) )]
	public class BoolToVisibleHidden : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
			{
				Visibility vs = Visibility.Visible;
				if ( !tempBool )
				{
					vs = Visibility.Hidden;
				}
				return vs;
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			Visibility vs;
			vs = ( Visibility )value;
			if ( vs == Visibility.Visible )
				return true;
			else
				return false;
		}
	}
	[ValueConversion( typeof( bool ), typeof( string ) )]
	public class BoolToStringReadyBusy : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
			{
				return tempBool ? "Ready" : "Busy";
			}
			return "Busy";
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( ( string )value == "Ready" );
		}
	}
	[ValueConversion( typeof( AccessLevel ), typeof( bool ) )]
	public class AccessLevelEngAboveToBool : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( AccessLevel )value >= AccessLevel.Engineer;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return !( ( bool )value );
		}
	}
	[ValueConversion( typeof( AccessLevel ), typeof( bool ) )]
	public class AccessLevelManufacturerToBool : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( AccessLevel )value >= AccessLevel.Manufacturer;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return !( ( bool )value );
		}
	}
	[ValueConversion( typeof( bool ), typeof( bool ) )]
	public class InverseBool : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			if ( bool.TryParse( value.ToString(), out bool tempBool ) )
				return !tempBool;
			return false;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return !( ( bool )value );
		}
	}

	[ValueConversion( typeof( bool ), typeof( Brush ) )]
	public class BoolToBrushIORecPassFail : IValueConverter
	{
		private Brush Br_Have_Signal = new SolidColorBrush( Color.FromRgb( 0, 255, 0 ) );
		private Brush Br_Miss_Signal = new SolidColorBrush( Color.FromRgb( 255, 0, 0 ) );
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
			{
				return tempBool ? this.Br_Have_Signal : this.Br_Miss_Signal;
			}
			return this.Br_Miss_Signal;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( ( Brush )value == this.Br_Have_Signal );
		}
	}

	[ValueConversion( typeof( bool ), typeof( Brush ) )]
	public class BoolToBrushIOGreenRedRec : IValueConverter
	{
		private Brush Br_Have_Signal = new SolidColorBrush( Color.FromRgb( 0, 255, 0 ) );
		private Brush Br_Miss_Signal = new SolidColorBrush( Color.FromRgb( 255, 0, 0 ) );
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
			{
				return tempBool ? this.Br_Have_Signal : this.Br_Miss_Signal;
			}
			return this.Br_Miss_Signal;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( ( Brush )value == this.Br_Have_Signal );
		}
	}

	[ValueConversion( typeof( bool ), typeof( Brush ) )]
	public class BoolToBrushIORec : IValueConverter
	{
		private Brush Br_Have_Signal = new SolidColorBrush( Color.FromRgb( 0, 255, 0 ) );
		private Brush Br_Miss_Signal = new SolidColorBrush( Color.FromRgb( 0, 0, 0 ) );
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
			{
				return tempBool ? this.Br_Have_Signal : this.Br_Miss_Signal;
			}
			return this.Br_Miss_Signal;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( ( Brush )value == this.Br_Have_Signal );
		}
	}

	[ValueConversion( typeof( int ), typeof( bool ) )]
	public class IntToBool : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			int tempInt = 0;
			bool tempBool = false;
			if ( int.TryParse( value.ToString(), out tempInt ) )
			{

				if ( tempInt == 0 )
					tempBool = false;
				else if ( tempInt == 1 )
					tempBool = true;
			}
			return tempBool;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool tempBool = ( bool )value;
			if ( tempBool == true )
				return 1;
			else
				return 0;
		}
	}
	[ValueConversion( typeof( bool ), typeof( Style ) )]
	public class BoolToStyleGreen : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
			{
				return tempBool ? Application.Current.Resources[ "Button_CyanContent_Lime_Highlight" ] : Application.Current.Resources[ "Button_CyanContent" ];
			}
			return Application.Current.Resources[ "Button_CyanContent" ];
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return null;
		}
	}
	[ValueConversion( typeof( bool ), typeof( Style ) )]
	public class BoolToStyleRed : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool tempBool = false;
			if ( bool.TryParse( value.ToString(), out tempBool ) )
			{
				return tempBool ? Application.Current.Resources[ "Button_LimeContent_Red_Highlight" ] : Application.Current.Resources[ "Button_MainMenu" ];
			}
			return Application.Current.Resources[ "Button_CyanContent" ];
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return null;
		}
	}

	[ValueConversion( typeof( MachineStateType ), typeof( Style ) )]
	public class AutorunStarttoStyle : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			if ( ( ( MachineStateType )value ) == MachineStateType.AUTO_RUNNING )
				return Application.Current.Resources[ "Button_CyanContent_Lime_Highlight" ];
			else
				return Application.Current.Resources[ "Button_CyanContent" ];
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return null;
		}
	}
	[ValueConversion( typeof( MachineStateType ), typeof( Style ) )]
	public class AutorunPausetoStyle : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			if ( ( ( MachineStateType )value ) == MachineStateType.AUTO_PAUSE )
				return Application.Current.Resources[ "Button_CyanContent_Lime_Highlight" ];
			else
				return Application.Current.Resources[ "Button_CyanContent" ];
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return null;
		}
	}
	[ValueConversion( typeof( MachineStateType ), typeof( Style ) )]
	public class AutorunCycleStoptoStyle : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			if ( ( ( MachineStateType )value ) == MachineStateType.AUTO_CYCLESTOP )
				return Application.Current.Resources[ "Button_CyanContent_Lime_Highlight" ];
			else
				return Application.Current.Resources[ "Button_CyanContent" ];
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return null;
		}
	}
	[ValueConversion( typeof( bool ), typeof( SolidColorBrush ) )]
	public class SaveFile_ToColor : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )( value );
			if ( b )
				return Application.Current.Resources[ "BR_NEED_TO_SAVE" ];
			else
				return Application.Current.Resources[ "BackgroundNormal" ];

			//return Brushes.Gray;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return null;
		}
	}

	[ValueConversion( typeof( bool ), typeof( Style ) )]
	public class SaveFile_ToStyle : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )( value );
			if ( b )
				return Application.Current.Resources[ "Button_CyanContent_Red_Highlight" ];
			else
				return Application.Current.Resources[ "Button_CyanContent" ];

			//return Brushes.Gray;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return null;
		}
	}

	[ValueConversion( typeof( double ), typeof( string ) )]
	public class D_Trim_Result_Val_Pcnt : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempfloat = 0;
			if ( double.TryParse( value.ToString(), out tempfloat ) )
			{
				double fValue = ( double )value;
				return fValue.ToString( "F3" ) + " %";
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			double fValue;
			double.TryParse( ( string )value, out fValue );
			return fValue;
		}
	}

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

		public static bool IsValid( string str, bool AllowAlternatingComma, int MaxLen, int MinVal, int MaxVal )
		{
			int tempint = 0;

			if ( str == null || str == "" ) return false;
			if ( str.Length > MaxLen ) return false;

			for ( int i = 1; i < str.Length; i = i + 2 )
			{
				if ( str.ElementAt( i ) != ',' )
					return false;
			}
			for ( int i = 0; i < str.Length; i = i + 2 )
			{
				if ( int.TryParse( str.ElementAt( i ).ToString(), out tempint ) )
				{
					if ( tempint < MinVal || tempint > MaxVal )
						return false;
				}
				else
				{
					return false;
				}

			}

			return true;
		}

		public static bool IsValid( string val, int MinVal, int MaxVal )
		{
			if ( val == null || val == "" ) return false;

			int tempint = 0;
			if ( int.TryParse( val, out tempint ) )
			{
				if ( tempint >= MinVal && tempint <= MaxVal )
					return true;
				else
					return false;
			}
			else
			{
				return false;
			}
		}
		public static bool IsValid( int val, int MinVal, int MaxVal )
		{
			if ( val >= MinVal && val <= MaxVal )
				return true;
			else
				return false;
		}
		public static bool IsValid( string val, float MinVal, float MaxVal )
		{
			if ( val == null || val == "" ) return false;

			float tempfloat = 0;
			if ( float.TryParse( val, out tempfloat ) )
			{
				if ( tempfloat >= MinVal && tempfloat <= MaxVal )
					return true;
				else
					return false;
			}
			else
			{
				return false;
			}
		}
		public static bool IsValid( float val, float MinVal, float MaxVal )
		{
			if ( val >= MinVal && val <= MaxVal )
				return true;
			else
				return false;
		}
		public static string ToReadableString( TimeSpan span )
		{
			string formatted = string.Format( "{0}{1}{2}{3}",
				span.Duration().Days > 0 ? string.Format( "{0:0} d: ", span.Days ) : string.Empty,
				span.Duration().Hours > 0 ? string.Format( "{0:0} h: ", span.Hours ) : string.Empty,
				span.Duration().Minutes > 0 ? string.Format( "{0:0} m: ", span.Minutes ) : string.Empty,
				span.Duration().Seconds > 0 ? string.Format( "{0:0} s", span.Seconds ) : string.Empty );

			if ( formatted.EndsWith( ", " ) ) formatted = formatted.Substring( 0, formatted.Length - 2 );

			if ( string.IsNullOrEmpty( formatted ) ) formatted = "0 s";

			return formatted;
		}

	}

	[ValueConversion( typeof( System.Drawing.Image ), typeof( System.Windows.Media.ImageSource ) )]
	public class ImageConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			// empty images are empty...
			if ( value == null ) { return null; }

			var image = ( System.Drawing.Image )value;
			// Winforms Image we want to get the WPF Image from...
			var bitmap = new System.Windows.Media.Imaging.BitmapImage();
			bitmap.BeginInit();
			MemoryStream memoryStream = new MemoryStream();
			// Save to a memory stream...
			image.Save( memoryStream, ImageFormat.Bmp );
			// Rewind the stream...
			memoryStream.Seek( 0, System.IO.SeekOrigin.Begin );
			bitmap.StreamSource = memoryStream;
			bitmap.EndInit();
			return bitmap;
		}

		public object ConvertBack( object value, Type targetType,
			object parameter, CultureInfo culture )
		{
			return null;
		}
	}
	[ValueConversion( typeof( TestResult ), typeof( SolidColorBrush ) )]
	public class TestResultToStateColor : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			TestResult b = ( TestResult )value;
			Color c = Colors.Gray;
			if ( b == TestResult.Untested )
				c = Colors.Gray;
			else if ( b == TestResult.Pass )
				c = Color.FromRgb( 0, 255, 0 );
			else if ( b == TestResult.Fail )
				c = Color.FromRgb( 255, 0, 0 );

			return new SolidColorBrush( c );
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}
