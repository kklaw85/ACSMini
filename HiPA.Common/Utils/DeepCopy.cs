
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HiPA.Common.Utils
{
	/// <summary>
	/// Desc	: C# extension method for fast object cloning.
	/// Author	: Burtsev-Alexey
	/// License : MIT License
	/// Git		: https://github.com/Burtsev-Alexey/net-object-deep-copy
	/// </summary>
	public static class ObjectCopy
	{
		private static readonly MethodInfo CloneMethod = typeof( Object ).GetMethod( "MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance );

		public static bool IsPrimitive( Type type )
		{
			if ( type == typeof( String ) ) return true;
			return ( type.IsValueType & type.IsPrimitive );
		}

		public static Object Copy( this Object originalObject )
		{
			return InternalCopy( originalObject, new Dictionary<Object, Object>( new ReferenceEqualityComparer() ) );
		}
		private static Object InternalCopy( Object originalObject, IDictionary<Object, Object> visited )
		{
			if ( originalObject == null ) return null;
			var typeToReflect = originalObject.GetType();
			if ( IsPrimitive( typeToReflect ) ) return originalObject;
			if ( visited.ContainsKey( originalObject ) ) return visited[ originalObject ];
			if ( typeof( Delegate ).IsAssignableFrom( typeToReflect ) ) return null;
			var cloneObject = CloneMethod.Invoke( originalObject, null );
			if ( typeToReflect.IsArray )
			{
				var arrayType = typeToReflect.GetElementType();
				if ( IsPrimitive( arrayType ) == false )
				{
					Array clonedArray = ( Array )cloneObject;
					ArrayExtensions.ForEach( clonedArray, ( array, indices ) => array.SetValue( InternalCopy( clonedArray.GetValue( indices ), visited ), indices ) );
				}

			}
			visited.Add( originalObject, cloneObject );
			CopyFields( originalObject, visited, cloneObject, typeToReflect );
			RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect );
			return cloneObject;
		}

		private static void RecursiveCopyBaseTypePrivateFields( object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect )
		{
			if ( typeToReflect.BaseType != null )
			{
				RecursiveCopyBaseTypePrivateFields( originalObject, visited, cloneObject, typeToReflect.BaseType );
				CopyFields( originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate );
			}
		}

		private static void CopyFields( object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null )
		{
			foreach ( FieldInfo fieldInfo in typeToReflect.GetFields( bindingFlags ) )
			{
				if ( filter != null && filter( fieldInfo ) == false ) continue;
				if ( IsPrimitive( fieldInfo.FieldType ) ) continue;
				var originalFieldValue = fieldInfo.GetValue( originalObject );
				var clonedFieldValue = InternalCopy( originalFieldValue, visited );
				fieldInfo.SetValue( cloneObject, clonedFieldValue );
			}
		}
		public static T Copy<T>( this T original )
		{
			return ( T )Copy( ( Object )original );
		}
	}

	public class ReferenceEqualityComparer : EqualityComparer<Object>
	{
		public override bool Equals( object x, object y )
		{
			return ReferenceEquals( x, y );
		}
		public override int GetHashCode( object obj )
		{
			if ( obj == null ) return 0;
			return obj.GetHashCode();
		}
	}

	public static class ArrayExtensions
	{
		public static void ForEach( Array array, Action<Array, int[]> action )
		{
			if ( array.LongLength == 0 ) return;
			ArrayTraverse walker = new ArrayTraverse( array );
			do action( array, walker.Position );
			while ( walker.Step() );
		}
	}

	internal class ArrayTraverse
	{
		public int[] Position;
		private int[] maxLengths;

		public ArrayTraverse( Array array )
		{
			this.maxLengths = new int[ array.Rank ];
			for ( int i = 0; i < array.Rank; ++i )
			{
				this.maxLengths[ i ] = array.GetLength( i ) - 1;
			}
			this.Position = new int[ array.Rank ];
		}

		public bool Step()
		{
			for ( int i = 0; i < this.Position.Length; ++i )
			{
				if ( this.Position[ i ] < this.maxLengths[ i ] )
				{
					this.Position[ i ]++;
					for ( int j = 0; j < i; j++ )
					{
						this.Position[ j ] = 0;
					}
					return true;
				}
			}
			return false;
		}
	}

	public class StringTool
	{
		public static string NormalizeForCRLF( string data )
		{
			var lines = data.Split( '\n' );
			var result = "";

			foreach ( var line in lines )
			{
				var newLine = line.TrimEnd();
				result += newLine + "\r\n";
			}
			return result;
		}
	}
}



