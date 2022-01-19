using HiPA.Instrument.Motion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


namespace Converter
{

	[ValueConversion( typeof( int ), typeof( string ) )]
	public class IntegerToString : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			int tempint = 0;
			if ( int.TryParse( value.ToString(), out tempint ) )
			{
				int ivalue = tempint;
				return ivalue.ToString( "000" );
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

	[ValueConversion( typeof( uint ), typeof( string ) )]
	public class UIntegerToString : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			uint tempint = 0;
			if ( uint.TryParse( value.ToString(), out tempint ) )
			{
				uint ivalue = tempint;
				return ivalue.ToString();
			}
			return value.ToString();
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			uint iValue;
			uint.TryParse( ( string )value, out iValue );
			return iValue;
		}
	}

	[ValueConversion( typeof( double ), typeof( string ) )]
	public class DoubleTo2DecimalDisplay : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempfloat = 0;
			if ( double.TryParse( value.ToString(), out tempfloat ) )
			{
				double fValue = ( double )value;
				return fValue.ToString( "F2" );
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

	[ValueConversion( typeof( double ), typeof( string ) )]
	public class DoubleTo3DecimalDisplay : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			double tempfloat = 0;
			if ( double.TryParse( value.ToString(), out tempfloat ) )
			{
				double fValue = ( double )value;
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

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class FloatTo3DecimalDisplay : IValueConverter
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

	[ValueConversion( typeof( float ), typeof( string ) )]
	public class FloatTo2DecimalDisplay : IValueConverter
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
			float fValue;
			float.TryParse( ( string )value, out fValue );
			return fValue;
		}
	}

	public class EnumDescriptionSourceConverter : IValueConverter
	{
		private readonly Dictionary<Type, List<string>> _localLists =
		  new Dictionary<Type, List<string>>();

		public object Convert( object value, Type targetType, object parameter,
		  System.Globalization.CultureInfo culture )
		{
			bool Isenum = value.GetType().IsEnum;
			if ( value == null || !value.GetType().IsEnum )
				return value;
			if ( !this._localLists.ContainsKey( value.GetType() ) )
				this.CreateList( value.GetType() );
			return this._localLists[ value.GetType() ];
		}

		public object ConvertBack( object value, Type targetType, object parameter,
		  System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException( "There is no backward conversion" );
		}

		private void CreateList( Type e )
		{
			var list = new List<string>();

			foreach ( var value in Enum.GetValues( e ) )
			{
				FieldInfo info = value.GetType().GetField( value.ToString() );
				var valueDescription = ( DescriptionAttribute[] )info.GetCustomAttributes
					  ( typeof( DescriptionAttribute ), false );
				list.Add( valueDescription.Length == 1 ?
						valueDescription[ 0 ].Description : value.ToString() );
			}
			this._localLists.Add( e, list );
		}
	}

	public class EnumDescriptionConverter : IValueConverter
	{
		private class LocalDictionaries
		{
			public readonly Dictionary<int, string> EnumDescriptions =
				new Dictionary<int, string>();
			public readonly Dictionary<string, int> EnumIntValues =
				new Dictionary<string, int>();
		}

		private readonly Dictionary<Type, LocalDictionaries> _localDictionaries =
		  new Dictionary<Type, LocalDictionaries>();

		public object Convert( object value, Type targetType,
		  object parameter, System.Globalization.CultureInfo culture )
		{
			if ( value == null || !value.GetType().IsEnum )
				return value;
			if ( !this._localDictionaries.ContainsKey( value.GetType() ) )
				this.CreateDictionaries( value.GetType() );
			return this._localDictionaries[ value.GetType() ].EnumDescriptions[ ( int )value ];
		}

		public object ConvertBack( object value, Type targetType,
		  object parameter, System.Globalization.CultureInfo culture )
		{
			if ( value == null || !targetType.IsEnum )
				return value;
			if ( !this._localDictionaries.ContainsKey( targetType ) )
				this.CreateDictionaries( targetType );
			int enumInt = this._localDictionaries[ targetType ].EnumIntValues[ value.ToString() ];
			return Enum.ToObject( targetType, enumInt );
		}

		private void CreateDictionaries( Type e )
		{
			var dictionaries = new LocalDictionaries();

			foreach ( var value in Enum.GetValues( e ) )
			{
				FieldInfo info = value.GetType().GetField( value.ToString() );
				var valueDescription = ( DescriptionAttribute[] )info.GetCustomAttributes
				  ( typeof( DescriptionAttribute ), false );
				if ( valueDescription.Length == 1 )
				{
					dictionaries.EnumDescriptions.Add( ( int )value,
					  valueDescription[ 0 ].Description );
					dictionaries.EnumIntValues.Add( valueDescription[ 0 ].Description,
					  ( int )value );
				}
				else //Use the value for display if not concrete result
				{
					dictionaries.EnumDescriptions.Add( ( int )value, value.ToString() );
					dictionaries.EnumIntValues.Add( value.ToString(), ( int )value );
				}
			}
			this._localDictionaries.Add( e, dictionaries );
		}
	}

	[ValueConversion( typeof( bool ), typeof( bool ) )]
	public class BoolToInvertBoolConveter : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return !( bool )value;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}


	[ValueConversion( typeof( bool ), typeof( SolidColorBrush ) )]
	public class OnlineBoolToColorConveter : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )value;
			SolidColorBrush br = b ? Brushes.Green : Brushes.Gray;
			return br;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion( typeof( DateTime ), typeof( String ) )]
	public class DateTimeToTimeString : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			DateTime d = ( DateTime )value;
			return d.ToString( "HH:mm:ss" );
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion( typeof( DateTime ), typeof( String ) )]
	public class DateTimeToDateString : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			DateTime d = ( DateTime )value;
			return d.ToString( "dd/MMM/yyyy" );
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}


	[ValueConversion( typeof( bool ), typeof( SolidColorBrush ) )]
	public class BoolToUpdateButtonColor : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )( value );
			if ( b )
				return Application.Current.Resources[ "BR_NEED_TO_SAVE" ];
			else
				return Application.Current.Resources[ "BackgroundNormal" ];
			//return Brushes.DimGray;
			//return Brushes.Gray;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion( typeof( bool ), typeof( Style ) )]
	public class BoolToUpdateButtonStyle : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )( value );
			if ( b )
				return Application.Current.Resources[ "Button_CyanContent_Lime_Highlight" ];
			else
				return Application.Current.Resources[ "Button_CyanContent" ];
			//return Brushes.DimGray;
			//return Brushes.Gray;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion( typeof( int ), typeof( SolidColorBrush ) )]
	public class IntToUpdateButtonColor : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			int b = ( int )( value );
			if ( b == 2 )
				return Application.Current.Resources[ "BR_NEED_TO_SAVE" ];
			else if ( b == 1 )
				return Application.Current.Resources[ "BackgroundNormal" ];
			else
				return Brushes.DimGray;
			//return Brushes.DimGray;
			//return Brushes.Gray;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion( typeof( int ), typeof( Style ) )]
	public class IntToUpdateButtonStyle : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			int b = ( int )( value );
			if ( b == 2 )
				return Application.Current.Resources[ "Button_CyanContent_Lime_Highlight" ];
			else if ( b == 1 )
				return Application.Current.Resources[ "Button_CyanContent" ];
			else
				return Application.Current.Resources[ "Button_CyanContent" ];
			//return Brushes.DimGray;
			//return Brushes.Gray;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion( typeof( int ), typeof( SolidColorBrush ) )]
	public class IntToUpdateTemperatureLabelColor : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			int b = ( int )( value );
			if ( b == 4 )
				return Brushes.Red;
			else if ( b == 3 )
				return Brushes.Green;
			else if ( b == 2 )
				return Brushes.LightGray;
			else if ( b == 1 )
				return Brushes.Green;
			else
				return Brushes.LightGray;
			//return Brushes.Gray;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion( typeof( int ), typeof( double ) )]
	public class ValueToAngleConverter : IValueConverter
	{
		public object Convert( object value, Type t, object parameter, System.Globalization.CultureInfo culture )
		{
			return ( double )( ( ( int )value * 0.01 ) * 360 );
		}

		public object ConvertBack( object value, Type t, object parameter, System.Globalization.CultureInfo culture )
		{
			//return (double)((double)value / 360) * 100;
			throw new NotImplementedException();
		}
	}

	[ValueConversion( typeof( bool ), typeof( SolidColorBrush ) )]
	public class BoolToUpdateDataColor : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )value;
			string sKey = b ? "BR_UNP_GRAY" : "BR_UNP_GRAY";
			SolidColorBrush br = ( SolidColorBrush )Application.Current.Resources[ sKey ];
			return br;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}


	[ValueConversion( typeof( bool ), typeof( SolidColorBrush ) )]
	public class BoolToStateColor : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )value;
			Color c = b ? Colors.Green : Colors.Gray;
			return new SolidColorBrush( c );
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
	[ValueConversion( typeof( DioValue ), typeof( bool ) )]
	public class DioValueToBool : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			return ( value as DioValue? ) == DioValue.On;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( bool )value ? DioValue.On : DioValue.Off;
		}
	}
	[ValueConversion( typeof( bool ), typeof( SolidColorBrush ) )]
	public class LimeWhiteBoolToStateColor : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )value;
			Color c = b ? Colors.Lime : Colors.LightGray;
			return new SolidColorBrush( c );
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
	[ValueConversion( typeof( bool ), typeof( SolidColorBrush ) )]
	public class BoolToTextBoxBg : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			bool b = ( bool )value;
			Color c = b ? Colors.White : Colors.Gray;
			return new SolidColorBrush( c );
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotImplementedException();
		}
	}
}
