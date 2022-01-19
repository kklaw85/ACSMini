using HiPA.Instrument.Motion;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace N_Data_Utilities
{
	[ValueConversion( typeof( bool ), typeof( Brush ) )]
	public class BoolToBrushIORec : IValueConverter
	{
		private Brush Br_Have_Signal = new SolidColorBrush( Color.FromRgb( 0, 255, 0 ) );
		private Brush Br_Miss_Signal = new SolidColorBrush( Color.FromRgb( 0, 0, 0 ) );
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( bool )value ? this.Br_Have_Signal : this.Br_Miss_Signal;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( Brush )value == this.Br_Have_Signal;
		}
	}
	[ValueConversion( typeof( DioValue ), typeof( Brush ) )]
	public class DioValueToBrushIORec : IValueConverter
	{
		private Brush Br_Have_Signal = new SolidColorBrush( Color.FromRgb( 0, 255, 0 ) );
		private Brush Br_Miss_Signal = new SolidColorBrush( Color.FromRgb( 0, 0, 0 ) );
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( value as DioValue? ) == DioValue.On ? this.Br_Have_Signal : this.Br_Miss_Signal;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( ( Brush )value == this.Br_Have_Signal ) ? DioValue.On : DioValue.Off;
		}
	}
	[ValueConversion( typeof( DioValue ), typeof( Style ) )]
	public class DioValueToStyle : IValueConverter
	{
		private Brush Br_Have_Signal = new SolidColorBrush( Color.FromRgb( 255, 0, 0 ) );
		private Brush Br_Miss_Signal = new SolidColorBrush( Color.FromRgb( 0, 0, 0 ) );
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( value as DioValue? ) == DioValue.On ? Application.Current.Resources[ "Button_CyanContent_Lime_Highlight" ] : Application.Current.Resources[ "Button_CyanContent" ];
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return null;
		}
	}


	[ValueConversion( typeof( DioValue ), typeof( Brush ) )]
	public class DioValueToBrushResetIORec : IValueConverter
	{
		private Brush Br_Have_Signal = new SolidColorBrush( Color.FromRgb( 255, 0, 0 ) );
		private Brush Br_Miss_Signal = new SolidColorBrush( Color.FromRgb( 0, 0, 0 ) );
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( value as DioValue? ) == DioValue.On ? this.Br_Have_Signal : this.Br_Miss_Signal;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( ( Brush )value == this.Br_Have_Signal ) ? DioValue.On : DioValue.Off;
		}
	}
	[ValueConversion( typeof( DioValue ), typeof( Brush ) )]
	public class InvsDioValueToBrushIORec : IValueConverter
	{
		private Brush Br_Have_Signal = new SolidColorBrush( Color.FromRgb( 0, 255, 0 ) );
		private Brush Br_Miss_Signal = new SolidColorBrush( Color.FromRgb( 0, 0, 0 ) );
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( value as DioValue? ) == DioValue.Off ? this.Br_Have_Signal : this.Br_Miss_Signal;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( ( Brush )value == this.Br_Miss_Signal ) ? DioValue.On : DioValue.Off;
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
	[ValueConversion( typeof( DioValue ), typeof( DioValue ) )]
	public class DioValueInverter : IValueConverter
	{
		public object Convert( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{

			return ( value as DioValue? ) == DioValue.On ? DioValue.Off : DioValue.On;
		}
		public object ConvertBack( object value, Type targetType,
		object parameter, System.Globalization.CultureInfo culture )
		{
			return ( value as DioValue? ) == DioValue.On ? DioValue.Off : DioValue.On;
		}
	}
}
