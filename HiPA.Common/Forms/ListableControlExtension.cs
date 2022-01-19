using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HiPA.Common.Forms
{
	#region NameValueItem
	public class NameValueItem
	{
		public string Name { get; private set; }
		public object Value { get; private set; }

		public NameValueItem( string name, object value )
		{
			this.Name = name;
			this.Value = value;
		}

		public override string ToString()
		{
			return this.Name;
		}
		public override bool Equals( object obj )
		{
			if ( obj is NameValueItem other )
				return this.Name == other.Name;
			return false;
		}
		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}
	}
	#endregion
	#region ListableControlExtension
	public static class ListableControlExtension
	{
		#region AddItem
		public static void AddItem( this System.Windows.Controls.ComboBox list, object value )
		{
			if ( value.GetType().IsValueType == true || value is string )
				list.AddItem( value.ToString(), value );
		}
		public static void AddItem( this System.Windows.Controls.ListBox list, object value )
		{
			if ( value.GetType().IsValueType == true )
				list.AddItem( value.ToString(), value );
		}
		public static void AddItem( this System.Windows.Controls.ComboBox list, string name, object value )
		{
			if ( list.IsExists( name ) == true ) return;
			list.Items.Add( new NameValueItem( name, value ) );
		}
		public static void AddItem( this System.Windows.Controls.ListBox list, string name, object value )
		{
			if ( list.IsExists( name ) == true ) return;
			list.Items.Add( new NameValueItem( name, value ) );
		}
		public static void AddItem( this System.Windows.Forms.ComboBox list, object value )
		{
			if ( value.GetType().IsValueType == true || value is string )
				list.AddItem( value.ToString(), value );
		}
		public static void AddItem( this System.Windows.Forms.ListBox list, object value )
		{
			if ( value.GetType().IsValueType == true )
				list.AddItem( value.ToString(), value );
		}
		public static void AddItem( this System.Windows.Forms.ComboBox list, string name, object value )
		{
			if ( list.IsExists( name ) == true ) return;
			list.Items.Add( new NameValueItem( name, value ) );
		}
		public static void AddItem( this System.Windows.Forms.ListBox list, string name, object value )
		{
			if ( list.IsExists( name ) == true ) return;
			list.Items.Add( new NameValueItem( name, value ) );
		}

		#endregion

		#region UpdateItem 
		public static void UpdateItem( this System.Windows.Forms.ComboBox list, string name, object value )
		{
			if ( list.IsExists( name ) == true )
			{
				for ( int i = 0; i < list.Items.Count; i++ )
				{
					if ( list.Items[ i ] is NameValueItem item && item.Name == name )
					{
						list.Items[ i ] = new NameValueItem( name, value );
					}
				}
			}
			else
			{
				list.AddItem( name, value );
			}
		}
		public static void UpdateItem( this System.Windows.Forms.ListBox list, string name, object value )
		{
			if ( list.IsExists( name ) == true )
			{
				for ( int i = 0; i < list.Items.Count; i++ )
				{
					if ( list.Items[ i ] is NameValueItem item && item.Name == name )
					{
						list.Items[ i ] = new NameValueItem( name, value );
					}
				}
			}
			else
			{
				list.AddItem( name, value );
			}
		}
		public static void UpdateItem( this System.Windows.Controls.ComboBox list, string name, object value )
		{
			if ( list.IsExists( name ) == true )
			{
				for ( int i = 0; i < list.Items.Count; i++ )
				{
					if ( list.Items[ i ] is NameValueItem item && item.Name == name )
					{
						list.Items[ i ] = new NameValueItem( name, value );
					}
				}
			}
			else
			{
				list.AddItem( name, value );
			}
		}
		public static void UpdateItem( this System.Windows.Controls.ListBox list, string name, object value )
		{
			if ( list.IsExists( name ) == true )
			{
				for ( int i = 0; i < list.Items.Count; i++ )
				{
					if ( list.Items[ i ] is NameValueItem item && item.Name == name )
					{
						list.Items[ i ] = new NameValueItem( name, value );
					}
				}
			}
			else
			{
				list.AddItem( name, value );
			}
		}
		#endregion

		#region IsExists
		public static bool IsExists( this System.Windows.Forms.ComboBox list, string name )
		{
			foreach ( var item in list.Items )
			{
				if ( item is NameValueItem i )
					if ( i.Name == name )
						return true;
			}
			return false;
		}
		public static bool IsExists( this System.Windows.Forms.ListBox list, string name )
		{
			foreach ( var item in list.Items )
			{
				if ( item is NameValueItem i )
					if ( i.Name == name )
						return true;
			}
			return false;
		}
		public static bool IsExists( this System.Windows.Controls.ComboBox list, string name )
		{
			foreach ( var item in list.Items )
			{
				if ( item is NameValueItem i )
					if ( i.Name == name )
						return true;
			}
			return false;
		}
		public static bool IsExists( this System.Windows.Controls.ListBox list, string name )
		{
			foreach ( var item in list.Items )
			{
				if ( item is NameValueItem i )
					if ( i.Name == name )
						return true;
			}
			return false;
		}
		#endregion

		#region SelectedValue
		public static T SelectedValue<T>( this System.Windows.Forms.ComboBox list )
		{
			if ( list.SelectedItem is NameValueItem item )
				return ( T )item.Value;
			return default( T );
		}
		public static T SelectedValue<T>( this System.Windows.Forms.ListBox list )
		{
			if ( list.SelectedItem is NameValueItem item )
				return ( T )item.Value;
			return default( T );
		}

		public static IEnumerable<T> Values<T>( this System.Windows.Forms.ComboBox list )
		{
			return list.Items.Cast<T>();
		}
		public static IEnumerable<T> Values<T>( this System.Windows.Forms.ListBox list )
		{
			return list.Items.Cast<T>();
		}

		public static T SelectedValue<T>( this System.Windows.Controls.ComboBox list )
		{
			if ( list.SelectedItem is NameValueItem item )
				return ( T )item.Value;
			return default( T );
		}
		public static T SelectedValue<T>( this System.Windows.Controls.ListBox list )
		{
			if ( list.SelectedItem is NameValueItem item )
				return ( T )item.Value;
			return default( T );
		}

		public static IEnumerable<T> Values<T>( this System.Windows.Controls.ComboBox list )
		{
			return list.Items.Cast<T>();
		}
		public static IEnumerable<T> Values<T>( this System.Windows.Controls.ListBox list )
		{
			return list.Items.Cast<T>();
		}
		#endregion

		#region SelectByName
		public static void SelectByName( this System.Windows.Forms.ComboBox list, string name )
		{
			for ( int i = 0; i < list.Items.Count; i++ )
			{
				if ( list.Items[ i ] is NameValueItem item &&
					item.Name == name )
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		public static void SelectByName( this System.Windows.Forms.ListBox list, string name )
		{
			for ( int i = 0; i < list.Items.Count; i++ )
			{
				if ( list.Items[ i ] is NameValueItem item &&
					item.Name == name )
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		public static void SelectByName( this System.Windows.Controls.ComboBox list, string name )
		{
			for ( int i = 0; i < list.Items.Count; i++ )
			{
				if ( list.Items[ i ] is NameValueItem item &&
					item.Name == name )
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		public static void SelectByName( this System.Windows.Controls.ListBox list, string name )
		{
			for ( int i = 0; i < list.Items.Count; i++ )
			{
				if ( list.Items[ i ] is NameValueItem item &&
					item.Name == name )
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		#endregion

		#region SelectByValue
		public static void SelectByValue( this System.Windows.Forms.ComboBox list, object value )
		{
			for ( int i = 0; i < list.Items.Count; i++ )
			{
				if ( list.Items[ i ] is NameValueItem item &&
					item.Value.Equals( value ) )
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		public static void SelectByValue( this System.Windows.Forms.ListBox list, object value )
		{
			for ( int i = 0; i < list.Items.Count; i++ )
			{
				if ( list.Items[ i ] is NameValueItem item &&
					item.Value.Equals( value ) )
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		public static void SelectByValue( this System.Windows.Controls.ComboBox list, object value )
		{
			for ( int i = 0; i < list.Items.Count; i++ )
			{
				if ( list.Items[ i ] is NameValueItem item &&
					item.Value.Equals( value ) )
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		public static void SelectByValue( this System.Windows.Controls.ListBox list, object value )
		{
			for ( int i = 0; i < list.Items.Count; i++ )
			{
				if ( list.Items[ i ] is NameValueItem item &&
					item.Value.Equals( value ) )
				{
					list.SelectedIndex = i;
					break;
				}
			}
		}
		#endregion

		#region Names Property
		public static IEnumerable<string> Names( this System.Windows.Forms.ComboBox list )
		{
			foreach ( var item in list.Items )
			{
				if ( item is NameValueItem i )
					yield return i.Name;
			}
		}
		public static IEnumerable<string> Names( this System.Windows.Forms.ListBox list )
		{
			foreach ( var item in list.Items )
			{
				if ( item is NameValueItem i )
					yield return i.Name;
			}
		}
		public static IEnumerable<string> Names( this System.Windows.Controls.ComboBox list )
		{
			foreach ( var item in list.Items )
			{
				if ( item is NameValueItem i )
					yield return i.Name;
			}
		}
		public static IEnumerable<string> Names( this System.Windows.Controls.ListBox list )
		{
			foreach ( var item in list.Items )
			{
				if ( item is NameValueItem i )
					yield return i.Name;
			}
		}
		#endregion
	}

	#endregion
	#region Matrixextension
	public static class MatrixExtensions
	{
		/// <summary>
		/// Returns the row with number 'row' of this matrix as a 1D-Array.
		/// </summary>
		public static T[] GetRow<T>( this T[,] matrix, int row )
		{
			var rowLength = matrix.GetLength( 1 );
			var rowVector = new T[ rowLength ];

			for ( var i = 0; i < rowLength; i++ )
				rowVector[ i ] = matrix[ row, i ];

			return rowVector;
		}



		/// <summary>
		/// Sets the row with number 'row' of this 2D-matrix to the parameter 'rowVector'.
		/// </summary>
		public static void SetRow<T>( this T[,] matrix, int row, T[] rowVector )
		{
			var rowLength = matrix.GetLength( 1 );

			for ( var i = 0; i < rowLength; i++ )
				matrix[ row, i ] = rowVector[ i ];
		}



		/// <summary>
		/// Returns the column with number 'col' of this matrix as a 1D-Array.
		/// </summary>
		public static T[] GetCol<T>( this T[,] matrix, int col )
		{
			var colLength = matrix.GetLength( 0 );
			var colVector = new T[ colLength ];

			for ( var i = 0; i < colLength; i++ )
				colVector[ i ] = matrix[ i, col ];

			return colVector;
		}



		/// <summary>
		/// Sets the column with number 'col' of this 2D-matrix to the parameter 'colVector'.
		/// </summary>
		public static void SetCol<T>( this T[,] matrix, int col, T[] colVector )
		{
			var colLength = matrix.GetLength( 0 );

			for ( var i = 0; i < colLength; i++ )
				matrix[ i, col ] = colVector[ i ];
		}
	}
	#endregion
	#region MathExtension
	public static class Mathematics
	{
		public static bool Approximate( this double value, double other, double tolerance = 0.000000001 )
		{
			return Math.Abs( value - other ) <= tolerance;
		}

		public static bool IsValidValue( this double value )
		{
			if ( double.IsNaN( value ) == true ) return false;
			if ( double.IsInfinity( value ) == true ) return false;
			if ( double.IsNegativeInfinity( value ) == true ) return false;
			if ( double.IsPositiveInfinity( value ) == true ) return false;
			return true;
		}

		public static bool InRange<T>( this T value, T from, T to ) where T : IComparable<T>
		{
			return value.CompareTo( from ) >= 0 && value.CompareTo( to ) <= 0;
		}
	}
	#endregion
	#region string
	public static class StringExt
	{
		public static string RemoveWhitespace( this string value )
		{
			return new string( value.ToCharArray()
			.Where( c => !Char.IsWhiteSpace( c ) )
			.ToArray() );
		}

	}
	#endregion
	public static class ControlFinder
	{
		/// <summary>
		/// Finds a Child of a given item in the visual tree. 
		/// </summary>
		/// <param name="parent">A direct parent of the queried item.</param>
		/// <typeparam name="T">The type of the queried item.</typeparam>
		/// <param name="childName">x:Name or Name of child. </param>
		/// <returns>The first parent item that matches the submitted type parameter. 
		/// If not matching item can be found, 
		/// a null parent is being returned.</returns>
		public static T FindChild<T>( DependencyObject parent, string childName )
		   where T : DependencyObject
		{
			// Confirm parent and childName are valid. 
			if ( parent == null ) return null;

			T foundChild = null;

			int childrenCount = VisualTreeHelper.GetChildrenCount( parent );
			for ( int i = 0; i < childrenCount; i++ )
			{
				var child = VisualTreeHelper.GetChild( parent, i );
				// If the child is not of the request child type child
				T childType = child as T;
				if ( childType == null )
				{
					// recursively drill down the tree
					foundChild = FindChild<T>( child, childName );

					// If the child is found, break so we do not overwrite the found child. 
					if ( foundChild != null ) break;
				}
				else if ( !string.IsNullOrEmpty( childName ) )
				{
					var frameworkElement = child as FrameworkElement;
					// If the child's name is set for search
					if ( frameworkElement != null && frameworkElement.Name == childName )
					{
						// if the child's name is of the request name
						foundChild = ( T )child;
						break;
					}
				}
				else
				{
					// child element found.
					foundChild = ( T )child;
					break;
				}
			}

			return foundChild;
		}
		public static List<T> FindChild<T>( this DependencyObject parent )
   where T : DependencyObject
		{
			// Confirm parent and childName are valid. 
			if ( parent == null ) return null;

			var foundChild = new List<T>();
			foreach ( var child in LogicalTreeHelper.GetChildren( parent ) )
			{
				if ( !( child is FrameworkElement ) )
					continue;
				else if ( child is T )
					foundChild.Add( child as T );
				else
					foundChild.AddRange( FindChild<T>( child as DependencyObject ) );
			}
			return foundChild;
		}
	}
	public static class MethodsFinder
	{
		public static void ShowMethods( Type type )
		{
			foreach ( var method in type.GetMethods() )
			{
				var parameters = method.GetParameters();
				var parameterDescriptions = string.Join
					( ", ", method.GetParameters()
								 .Select( x => x.ParameterType + " " + x.Name )
								 .ToArray() );

				Console.WriteLine( "{0} {1} ({2})",
								  method.ReturnType,
								  method.Name,
								  parameterDescriptions );
			}
		}
	}
}
