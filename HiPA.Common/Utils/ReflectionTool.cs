using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HiPA.Common.Utils
{
	public static class ReflectionTool
	{
		public static string GetEnumDescription( Enum value )
		{
			var ev = value.GetHashCode();
			var type = value.GetType();
			foreach ( var v in GetEnumValueDesc( type ) )
				if ( v.Value.CompareTo( ev ) == 0 )
					return v.Desc;
			return value.ToString();
		}
		//public static string GetEnumDescription( Enum value )
		//{
		//	var fi = value.GetType().GetField( value.ToString() );
		//	var attributes = ( DescriptionAttribute[] )fi.GetCustomAttributes( typeof( DescriptionAttribute ), false );

		//	return ( attributes != null && attributes.Length > 0 ) ? 
		//		attributes[ 0 ].Description : 
		//		value.ToString();
		//}

		public static (int Address, T Attribute) GetEnumAttribute<T>( Enum point ) where T : Attribute
		{
			var type = point.GetType();
			var str = point.ToString();
			foreach ( var name in Enum.GetNames( type ) )
			{
				if ( name == str )
				{
					var value = Enum.Parse( type, name );
					var v = Convert.ToInt32( value );
					var field = type.GetField( name );
					var fds = field.GetCustomAttributes<T>( true );
					if ( fds.Count() > 0 )
						return (v, fds.First());
				}
			}
			return (-1, null);
		}

		public static IEnumerable<string> GetEnumDescriptions( Type type )
		{
			foreach ( var name in Enum.GetNames( type ) )
			{
				var field = type.GetField( name );
				var fds = field.GetCustomAttributes<DescriptionAttribute>( true );
				if ( fds.Count() > 0 )
					yield return fds.First().Description;
			}
		}

		public static IEnumerable<(int Value, string Desc)> GetEnumValueDesc( Type type )
		{
			foreach ( var name in Enum.GetNames( type ) )
			{
				var value = Enum.Parse( type, name );
				var v = Convert.ToInt32( value );

				var field = type.GetField( value.ToString() );
				var fds = field.GetCustomAttributes<DescriptionAttribute>( true );
				if ( fds.Count() > 0 )
					yield return (v, fds.First().Description);
			}
		}
		public static IEnumerable<(int Value, T Attribute)> GetEnumAttributes<T>( Type type ) where T : Attribute
		{
			foreach ( var name in Enum.GetNames( type ) )
			{
				var value = Enum.Parse( type, name );
				var v = Convert.ToInt32( value );
				var field = type.GetField( name );
				var fds = field.GetCustomAttributes<T>( true );
				if ( fds.Count() > 0 )
					yield return (v, fds.First());
			}
		}

		public static void SetPropertyValue( object instance, string name, object value )
		{
			if ( instance == null ) return;
			var type = instance.GetType();
			var prop = type.GetProperty( name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );
			prop?.SetValue( instance, value );
		}

		public static string GetPropertyName<T>( Expression<Func<T>> propertyExpression )
		{
			return ( propertyExpression.Body as MemberExpression ).Member.Name;
		}

		public static Dictionary<Type, List<Type>> QueryConfigurationTypes( IEnumerable<Type> validTypes )
		{
			var typePairs = new Dictionary<Type, List<Type>>();

			foreach ( var assembly in AppDomain.CurrentDomain.GetAssemblies() )
			{
				foreach ( var valid in validTypes )
				{
					if ( assembly.IsDynamic == true ) continue;

					var pair = FetchTypes( assembly, valid );
					if ( pair.Any() )
					{
						if ( typePairs.TryGetValue( valid, out var list ) == true )
						{
							list.AddRange( pair );
						}
						else
						{
							list = new List<Type>();
							list.AddRange( pair );
							typePairs.Add( valid, list );
						}
					}
				}
			}
			return typePairs;
		}

		public static IEnumerable<Type> FetchTypes( Assembly assembly, Type validType )
		{
			foreach ( var type in assembly.ExportedTypes )
			{
				if ( validType.IsAssignableFrom( type ) &&
					type.IsInterface == false &&
					type.IsAbstract == false )
				{
					yield return type;
				}
			}
		}
		public static T GetPropertyValue<T>( object obj, string propName ) { return ( T )obj.GetType().GetProperty( propName ).GetValue( obj, null ); }
	}
}
