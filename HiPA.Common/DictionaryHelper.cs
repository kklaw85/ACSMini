using System;
using System.Reflection;

namespace HiPA.Common
{
	public static class DictionaryHelper
	{
		static readonly Type _PureXmlDictionaryType = typeof( XmlDictionary<,> );

		public static void Filter( object instance )
		{
			if ( instance == null ) return;

			var type = instance.GetType();
			var fields = type.GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance );

			foreach ( var field in fields )
			{
				var value = field.GetValue( instance );
				if ( value == null ) continue;

				var fType = value.GetType();
				if ( fType.IsValueType == true ) continue;
				if ( fType.IsClass == false ) continue;
				if ( fType.Name.StartsWith( "Dictionary" ) == true )
				{
					var gType = _PureXmlDictionaryType.MakeGenericType( fType.GenericTypeArguments );
					var xmldict = Activator.CreateInstance( gType, new object[] { value } );
					field.SetValue( instance, xmldict );
				}

				Filter( value );
			}
		}

		public static object ConvertToXmlDictionary( object dictionary )
		{
			if ( dictionary == null ) return null;
			var fType = dictionary.GetType();
			if ( fType.Name.StartsWith( "Dictionary" ) == false ) return null;

			var gType = _PureXmlDictionaryType.MakeGenericType( fType.GenericTypeArguments );
			var xmldict = Activator.CreateInstance( gType );

			//var propValues = fType.GetProperty( "Values", BindingFlags.Public | BindingFlags.Instance );
			//var srcValues = propValues.GetValue( dictionary );
			//var srcValuesType = srcValues.GetType();
			//var methodGetEnum = srcValuesType.GetMethod( "GetEnumerator", BindingFlags.Public | BindingFlags.Instance );
			//var enumter = methodGetEnum.Invoke( srcValues, null ) as Enumerator;



			//var methodAdd = gType.GetMethod( "Add", BindingFlags.Public | BindingFlags.Instance );
			//methodAdd.Invoke( xmldict, new object[] );

			//var dict = new Dictionary<int, string>();
			//dict.Values

			return xmldict;
		}
	}
}
