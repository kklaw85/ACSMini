using HiPA.Common.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace Validations
{
	public enum ValidationEnum
	{
		[Description( "Result is OK" )] // allow to submit
		ok,
		[Description( "Data has changed" )] // allow to submit
		DataChanged,
		[Description( "Result not OK" )]
		NotOk,
		[Description( "Input cannot be empty" )]
		InputEmpty,
		[Description( "Input should be type of Int32" )]
		InputInt32,
		[Description( "Input should be type of Int" )]
		InputInt,
		[Description( "Invalid format" )]
		InvalidFormat,
		[Description( "Input out of range" )]
		OutOfRange,
	}

	public class BindingProxy : System.Windows.Freezable
	{
		protected override Freezable CreateInstanceCore()
		{
			return new BindingProxy();
		}

		public object Data
		{
			get { return ( object )GetValue( DataProperty ); }
			set { SetValue( DataProperty, value ); }
		}

		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register( "Data", typeof( object ), typeof( BindingProxy ), new PropertyMetadata( null ) );
	}

	public class Wrapper : DependencyObject
	{
		public static readonly DependencyProperty MaxProperty =
			 DependencyProperty.Register( "Max", typeof( double ),
			 typeof( Wrapper ), new FrameworkPropertyMetadata( double.MaxValue ) );

		public double Max
		{
			get { return ( double )this.GetValue( MaxProperty ); }
			set { this.SetValue( MaxProperty, value ); }
		}

		public static readonly DependencyProperty MinProperty =
	 DependencyProperty.Register( "Min", typeof( double ),
	 typeof( Wrapper ), new FrameworkPropertyMetadata( double.MinValue ) );

		public double Min
		{
			get { return ( double )this.GetValue( MinProperty ); }
			set { this.SetValue( MinProperty, value ); }
		}
	}

	public class Valid_Rule_Base : ValidationRule
	{
		public object Min
		{
			get => this.GetValue( () => this.Min );
			set => this.SetValue( () => this.Min, value );
		}
		public object Max
		{
			get => this.GetValue( () => this.Max );
			set => this.SetValue( () => this.Max, value );
		}
		public override ValidationResult Validate( object value, CultureInfo cultureInfo )
		{
			try
			{
				string str = value.ToString();
				if ( String.IsNullOrEmpty( str ) )
				{
					return new ValidationResult( false,
						"Empty String" );
				}

				if ( value is double )
				{
					if ( !( ( double )value ).InRange( Convert.ToDouble( this.Min ), Convert.ToDouble( this.Max ) ) ) return new ValidationResult( false, $"Out of range. Must be within {this.Min} and {this.Max}." );
				}
				else if ( value is int )
				{
					if ( !( ( int )value ).InRange( Convert.ToInt32( this.Min ), Convert.ToInt32( this.Max ) ) ) return new ValidationResult( false, $"Out of range. Must be within {this.Min} and {this.Max}." );
				}
				else if ( value is float )
				{
					if ( !( ( float )value ).InRange( Convert.ToSingle( this.Min ), Convert.ToSingle( this.Max ) ) ) return new ValidationResult( false, $"Out of range. Must be within {this.Min} and {this.Max}." );
				}
				else if ( value is decimal )
				{
					if ( !( ( decimal )value ).InRange( Convert.ToDecimal( this.Min ), Convert.ToDecimal( this.Max ) ) ) return new ValidationResult( false, $"Out of range. Must be within {this.Min} and {this.Max}." );
				}
			}
			catch ( ArgumentNullException ex )
			{
				return new ValidationResult( false, ex.Message );
			}
			catch ( Exception ex )
			{
				return new ValidationResult( false, ex.Message );
			}
			return new ValidationResult( true, null );
		}
		#region MIT binding
		private Dictionary<string, object> propertyValueStorage;
		#region GetProperty
		protected T GetValue<T>( Expression<Func<T>> property )
		{
			var lambdaExpression = property as LambdaExpression;

			if ( lambdaExpression == null )
			{
				throw new ArgumentException( @"Lambda expression return value can't be null", "property" );
			}

			string propertyName = GetPropertyName( lambdaExpression );
			return this.GetValue<T>( propertyName );
		}

		private static string GetPropertyName( LambdaExpression lambdaExpression )
		{
			MemberExpression memberExpression;

			if ( lambdaExpression.Body is UnaryExpression )
			{
				var unaryExpression = lambdaExpression.Body as UnaryExpression;
				memberExpression = unaryExpression.Operand as MemberExpression;
			}
			else
			{
				memberExpression = lambdaExpression.Body as MemberExpression;
			}

			return memberExpression == null ? null : memberExpression.Member.Name;
		}

		private T GetValue<T>( string propertyName )
		{
			object value;
			if ( this.propertyValueStorage == null )
				this.propertyValueStorage = new Dictionary<string, object>();
			if ( this.propertyValueStorage.TryGetValue( propertyName, out value ) )
			{
				return ( T )value;
			}

			return default( T );
		}
		#endregion
		#region SetProperty
		[SuppressMessage( "StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
				Justification = "Required as Equals can handle null refs." )]
		protected bool SetValue<T>( Expression<Func<T>> property, T value, bool compareBeforeTrigger = true )
		{
			var lambdaExpression = property as LambdaExpression;

			if ( lambdaExpression == null )
			{
				throw new ArgumentException( @"Lambda expression return value can't be null", "property" );
			}

			string propertyName = GetPropertyName( lambdaExpression );
			var storedValue = this.GetValue<T>( propertyName );

			if ( compareBeforeTrigger )
			{
				if ( typeof( T ) == typeof( Uri ) && storedValue != null )
				{
					if ( Equals( storedValue.ToString(), value.ToString() ) )
						return false;
				}
				else
				{
					if ( Equals( storedValue, value ) )
						return false;
				}
			}
			this.propertyValueStorage[ propertyName ] = value;
			this.OnPropertyChanged( propertyName );

			return true;
		}
		#endregion
		#endregion
		#region inotifypropertychanged
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged( [CallerMemberName] string propertyName = null )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		protected virtual void OnPropertyChanged<T>( Expression<Func<T>> raiser )
		{
			var propName = ( ( MemberExpression )raiser.Body ).Member.Name;
			this.OnPropertyChanged( propName );
		}

		protected bool Set<T>( ref T field, T value, [CallerMemberName] string name = null )
		{
			//if ( !EqualityComparer<T>.Default.Equals( field, value ) )
			{
				field = value;
				this.OnPropertyChanged( name );
				return true;
			}
			//return false;
		}
		#endregion
	}




	//public class Valid_Rule_INT : ValidationRule
	//{
	//	public Wrapper Wrapper { get; set; }
	//	public override ValidationResult Validate( object value, CultureInfo cultureInfo )
	//	{

	//		try
	//		{
	//			string str = value.ToString();
	//			if ( String.IsNullOrEmpty( str ) )
	//			{
	//				return new ValidationResult( false,
	//					"Empty String" );
	//			}

	//			if ( !Validator.IsValid( str,
	//								   this.Min,
	//								   this.Max ) )
	//			{
	//				return new ValidationResult( false, "Invalid Value" );
	//			}
	//		}
	//		catch ( ArgumentNullException ex )
	//		{
	//			return new ValidationResult( false, ex.Message );
	//		}
	//		catch ( Exception ex )
	//		{
	//			return new ValidationResult( false, ex.Message );
	//		}
	//		return new ValidationResult( true, null );
	//	}
	//}
	//public class Valid_Rule_Bool : ValidationRule
	//{
	//	public override ValidationResult Validate( object value, CultureInfo cultureInfo )
	//	{
	//		try
	//		{
	//			string str = value.ToString();
	//			if ( String.IsNullOrEmpty( str ) )
	//			{
	//				return new ValidationResult( false,
	//					"Empty String" );
	//			}
	//			if ( str.Length > 5 )
	//				return new ValidationResult( false,
	//				   "Too long String" );
	//			if ( str.ToUpper() != "FALSE" &&
	//			   str.ToUpper() != "TRUE" )
	//				return new ValidationResult( false,
	//					"Unknown String" );
	//		}
	//		catch ( ArgumentNullException ex )
	//		{
	//			return new ValidationResult( false, ex.Message );
	//		}
	//		catch ( Exception ex )
	//		{
	//			return new ValidationResult( false, ex.Message );
	//		}
	//		return new ValidationResult( true, null );
	//	}
	//}

	//public class Valid_Rule_Float : ValidationRule
	//{
	//	public float Min
	//	{
	//		get => this.GetValue( () => this.Min );
	//		set => this.SetValue( () => this.Min, value );
	//	}
	//	public float Max
	//	{
	//		get => this.GetValue( () => this.Max );
	//		set => this.SetValue( () => this.Max, value );
	//	}
	//	public override ValidationResult Validate( object value, CultureInfo cultureInfo )
	//	{

	//		try
	//		{
	//			string str = value.ToString();
	//			if ( String.IsNullOrEmpty( str ) )
	//			{
	//				return new ValidationResult( false,
	//					"Empty String" );
	//			}


	//			if ( !Validator.IsValid( str,
	//								   this.Min,
	//								   this.Max ) )
	//			{
	//				return new ValidationResult( false, "Invalid Value" );
	//			}
	//		}
	//		catch ( ArgumentNullException ex )
	//		{
	//			return new ValidationResult( false, ex.Message );
	//		}
	//		catch ( Exception ex )
	//		{
	//			return new ValidationResult( false, ex.Message );
	//		}

	//		return new ValidationResult( true, null );
	//	}
	//}

	public class Valid_Rule : ValidationRule
	{
		public Wrapper Wrapper { get; set; }
		public override ValidationResult Validate( object value, CultureInfo cultureInfo )
		{

			try
			{
				string str = value.ToString();
				if ( String.IsNullOrEmpty( str ) )
				{
					return new ValidationResult( false,
						"Empty String" );
				}


				if ( !Validator.IsValid( str,
									   this.Wrapper.Min,
									   this.Wrapper.Max ) )
				{
					return new ValidationResult( false, "Out of range" );
				}
			}
			catch ( ArgumentNullException ex )
			{
				return new ValidationResult( false, ex.Message );
			}
			catch ( Exception ex )
			{
				return new ValidationResult( false, ex.Message );
			}

			return new ValidationResult( true, null );
		}
	}
	public class Valid_RuleExclude : ValidationRule
	{
		public Wrapper Wrapper { get; set; }
		public override ValidationResult Validate( object value, CultureInfo cultureInfo )
		{

			try
			{
				string str = value.ToString();
				if ( String.IsNullOrEmpty( str ) )
				{
					return new ValidationResult( false,
						"Empty String" );
				}


				if ( !Validator.IsValidEx( str,
									   this.Wrapper.Min,
									   this.Wrapper.Max ) )
				{
					return new ValidationResult( false, "Out of range" );
				}
			}
			catch ( ArgumentNullException ex )
			{
				return new ValidationResult( false, ex.Message );
			}
			catch ( Exception ex )
			{
				return new ValidationResult( false, ex.Message );
			}

			return new ValidationResult( true, null );
		}
	}
}
