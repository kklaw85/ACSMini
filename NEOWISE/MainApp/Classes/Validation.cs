using System;
using System.ComponentModel;
using System.Globalization;
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


	internal class Valid_Rule_INT : ValidationRule
	{
		int _min = 1;
		public int Min
		{
			get { return this._min; }
			set { this._min = value; }
		}

		int _max = 1;
		public int Max
		{
			get { return this._max; }
			set { this._max = value; }
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

				if ( !Validator.IsValid( str,
									   this.Min,
									   this.Max ) )
				{
					return new ValidationResult( false, "Invalid Value" );
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
	internal class Valid_Rule_Bool : ValidationRule
	{
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
				if ( str.Length > 5 )
					return new ValidationResult( false,
					   "Too long String" );
				if ( str.ToUpper() != "FALSE" &&
				   str.ToUpper() != "TRUE" )
					return new ValidationResult( false,
						"Unknown String" );
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

	internal class Valid_Rule_Float : ValidationRule
	{
		float _min = 1;
		public float Min
		{
			get { return this._min; }
			set { this._min = value; }
		}

		float _max = 1;
		public float Max
		{
			get { return this._max; }
			set { this._max = value; }
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


				if ( !Validator.IsValid( str,
									   this.Min,
									   this.Max ) )
				{
					return new ValidationResult( false, "Invalid Value" );
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

	internal class Valid_Rule_Double : ValidationRule
	{
		double _min = 1;
		public double Min
		{
			get { return this._min; }
			set { this._min = value; }
		}

		double _max = 1;
		public double Max
		{
			get { return this._max; }
			set { this._max = value; }
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


				if ( !Validator.IsValid( str,
									   this.Min,
									   this.Max ) )
				{
					return new ValidationResult( false, "Invalid Value" );
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
