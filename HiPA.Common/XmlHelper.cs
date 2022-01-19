using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

// 此处代码来源于博客【在.net中读写config文件的各种方法】的示例代码
// http://www.cnblogs.com/fish-li/archive/2011/12/18/2292037.html

namespace HiPA.Common
{
	public static class XmlHelper
	{
		private static void XmlSerializeInternal( Stream stream, object o, Type[] extraTypes = null )
		{
			if ( o == null ) throw new ArgumentNullException( "o" );

			if ( extraTypes == null ) extraTypes = ExtraTypes;
			else AddExtraTypes( extraTypes );

			XmlSerializer serializer = new XmlSerializer( o.GetType(), extraTypes );

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineChars = "\r\n";
			settings.Encoding = Encoding.UTF8;
			settings.IndentChars = "    ";

			using ( XmlWriter writer = XmlWriter.Create( stream, settings ) )
			{
				serializer.Serialize( writer, o );
				writer.Close();
			}
		}

		/// <summary>
		/// 将一个对象序列化为XML字符串
		/// </summary>
		/// <param name="o">要序列化的对象</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>序列化产生的XML字符串</returns>
		public static string XmlSerialize( object o, Type[] extraTypes = null )
		{
			if ( extraTypes == null ) extraTypes = ExtraTypes;
			else AddExtraTypes( extraTypes );

			using ( MemoryStream stream = new MemoryStream() )
			{
				XmlSerializeInternal( stream, o, extraTypes );

				stream.Position = 0;
				using ( StreamReader reader = new StreamReader( stream, Encoding.UTF8 ) )
				{
					return reader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// 将一个对象按XML序列化的方式写入到一个文件
		/// </summary>
		/// <param name="o">要序列化的对象</param>
		/// <param name="path">保存文件路径</param>
		/// <param name="encoding">编码方式</param>
		public static void XmlSerializeToFile( object o, string path, Type[] extraTypes = null )
		{
			if ( string.IsNullOrEmpty( path ) ) throw new ArgumentNullException( $"The path is invalid" );

			if ( extraTypes == null ) extraTypes = ExtraTypes;
			else AddExtraTypes( extraTypes );

			using ( FileStream file = new FileStream( path, FileMode.Create, FileAccess.Write ) )
			{
				XmlSerializeInternal( file, o, extraTypes );
			}
		}

		/// <summary>
		/// 从XML字符串中反序列化对象
		/// </summary>
		/// <typeparam name="T">结果对象类型</typeparam>
		/// <param name="s">包含对象的XML字符串</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>反序列化得到的对象</returns>
		public static T XmlDeserialize<T>( string s, Type[] extraTypes = null )
		{
			return ( T )XmlDeserialize( typeof( T ), s, extraTypes );
		}
		public static object XmlDeserialize( Type type, string s, Type[] extraTypes = null )
		{
			if ( string.IsNullOrEmpty( s ) ) throw new ArgumentNullException( "The stream is invalid" );
			var encoding = Encoding.UTF8;

			if ( extraTypes == null ) extraTypes = ExtraTypes;
			else AddExtraTypes( extraTypes );

			XmlSerializer mySerializer = new XmlSerializer( type, extraTypes );
			using ( MemoryStream ms = new MemoryStream( encoding.GetBytes( s ) ) )
			{
				using ( StreamReader sr = new StreamReader( ms, encoding ) )
				{
					return mySerializer.Deserialize( sr );
				}
			}
		}

		/// <summary>
		/// 读入一个文件，并按XML的方式反序列化对象。
		/// </summary>
		/// <typeparam name="T">结果对象类型</typeparam>
		/// <param name="path">文件路径</param>
		/// <param name="encoding">编码方式</param>
		/// <returns>反序列化得到的对象</returns>
		public static T XmlDeserializeFromFile<T>( string path, Type[] extraTypes = null )
		{
			return ( T )XmlDeserializeFromFile( typeof( T ), path, extraTypes );
		}
		public static object XmlDeserializeFromFile( Type type, string path, Type[] extraTypes = null )
		{
			if ( string.IsNullOrEmpty( path ) ) throw new ArgumentNullException( $"The path is invalid" );

			string xml = File.ReadAllText( path, Encoding.UTF8 );
			return XmlDeserialize( type, xml, extraTypes );
		}



		static List<Type> s_extraTypes = new List<Type>();
		public static Type[] ExtraTypes => s_extraTypes.ToArray();

		public static void AddExtraTypes( Type[] extraTypes )
		{
			if ( extraTypes == null ) return;
			foreach ( var type in extraTypes )
			{
				if ( s_extraTypes.Contains( type ) == false )
					s_extraTypes.Add( type );
			}
		}
	}

	public class XmlDictionary<TKey, TValue>
		: Dictionary<TKey, TValue>
		, IXmlSerializable
	{
		public XmlDictionary()
		{
		}
		public XmlDictionary( IDictionary<TKey, TValue> dictionary )
			: base( dictionary )
		{
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml( XmlReader reader )
		{
			reader.Read();
			XmlSerializer KeySerializer = new XmlSerializer( typeof( TKey ) );
			XmlSerializer ValueSerializer = new XmlSerializer( typeof( TValue ), XmlHelper.ExtraTypes );

			//name = reader.Name;
			//reader.ReadStartElement( name );
			while ( reader.NodeType != XmlNodeType.EndElement )
			{
				if ( reader.IsEmptyElement == true ) break;

				reader.ReadStartElement( "element" );   // <element>
				reader.ReadStartElement( "key" );       //	<key>
				TKey tk = ( TKey )KeySerializer.Deserialize( reader );
				reader.ReadEndElement();                //  </key>
				reader.ReadStartElement( "value" );     //  <value>
				TValue vl = ( TValue )ValueSerializer.Deserialize( reader );
				reader.ReadEndElement();                //  </value>
				reader.ReadEndElement();                // <element>

				this.Add( tk, vl );
				reader.MoveToContent();
			}
			//reader.ReadEndElement();
			//reader.ReadEndElement();
		}
		public void WriteXml( XmlWriter writer )
		{
			XmlSerializer KeySerializer = new XmlSerializer( typeof( TKey ) );
			XmlSerializer ValueSerializer = new XmlSerializer( typeof( TValue ), XmlHelper.ExtraTypes );

			//write.WriteAttributeString( "name", name );
			//write.WriteStartElement( name );
			foreach ( KeyValuePair<TKey, TValue> kv in this )
			{
				writer.WriteStartElement( "element" );
				writer.WriteStartElement( "key" );
				KeySerializer.Serialize( writer, kv.Key );
				writer.WriteEndElement();
				writer.WriteStartElement( "value" );
				ValueSerializer.Serialize( writer, kv.Value );
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
			//write.WriteEndElement();
		}

		public void ReadXmlGeneric( XmlReader reader )
		{
			reader.Read();
			XmlSerializer KeySerializer = new XmlSerializer( typeof( TKey ) );
			XmlSerializer ValueSerializer = new XmlSerializer( typeof( TValue ) );

			//name = reader.Name;
			//reader.ReadStartElement( name );
			reader.ReadStartElement( "element" );   // <element>
			while ( reader.NodeType != XmlNodeType.EndElement )
			{
				if ( reader.IsEmptyElement == true ) break;

				reader.ReadStartElement( "key" );       //	<key>
				TKey tk = ( TKey )KeySerializer.Deserialize( reader );
				reader.ReadEndElement();                //  </key>
				reader.ReadStartElement( "value" );     //  <value>
				TValue vl = ( TValue )ValueSerializer.Deserialize( reader );
				reader.ReadEndElement();                //  </value>

				this.Add( tk, vl );
				reader.MoveToContent();
			}

			reader.ReadEndElement();                // <element>
													//reader.ReadEndElement();
													//reader.ReadEndElement();
		}

		public void WriteXmlGeneric( XmlWriter writer )
		{
			XmlSerializer KeySerializer = new XmlSerializer( typeof( TKey ) );
			XmlSerializer ValueSerializer = new XmlSerializer( typeof( TValue ) );

			//write.WriteAttributeString( "name", name );
			//writer.WriteStartElement( "Test" );
			writer.WriteStartElement( "element" );
			foreach ( KeyValuePair<TKey, TValue> kv in this )
			{
				writer.WriteStartElement( "key" );
				KeySerializer.Serialize( writer, kv.Key );
				writer.WriteEndElement();
				writer.WriteStartElement( "value" );
				ValueSerializer.Serialize( writer, kv.Value );
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}


		#region Read/Write XML for at least 2 level nesting objects
		public void ReadXml_2LevelNesting( System.Xml.XmlReader reader )
		{
			XmlSerializer keySerializer = new XmlSerializer( typeof( TKey ) );
			XmlSerializer valueSerializer = new XmlSerializer( typeof( TValue ) );

			bool wasEmpty = reader.IsEmptyElement;
			reader.Read();
			if ( wasEmpty ) return;

			reader.ReadStartElement( "item" );                                          //#1
			while ( reader.NodeType != System.Xml.XmlNodeType.EndElement )
			{
				reader.ReadStartElement( "key" );                                       //#2
				TKey key = ( TKey )keySerializer.Deserialize( reader );                 //#3
				reader.ReadEndElement();                                                //#4

				reader.ReadStartElement( "value" );                                     //#5
				TValue value = ( TValue )valueSerializer.Deserialize( reader );         //#6
				reader.ReadEndElement();                                                //#7

				this.Add( key, value );

				reader.ReadEndElement();                                                //#8
				reader.MoveToContent();                                                 //#9
			}
			reader.ReadEndElement();                                                    //#10end
		}

		public void WriteXml_2LevelNesting( System.Xml.XmlWriter writer )
		{
			XmlSerializer keySerializer = new XmlSerializer( typeof( TKey ) );
			XmlSerializer valueSerializer = new XmlSerializer( typeof( TValue ) );

			writer.WriteStartElement( "item" );             //#1
			foreach ( TKey key in this.Keys )
			{
				writer.WriteStartElement( "key" );          //#2
				keySerializer.Serialize( writer, key );     //#3
				writer.WriteEndElement();                   //#4

				writer.WriteStartElement( "value" );        //#5
				TValue value = this[ key ];
				valueSerializer.Serialize( writer, value ); //#6
				writer.WriteEndElement();                   //#7
			}
			writer.WriteEndElement();                       //#8
		}
		#endregion
	}

	//public interface IXmlSerializable
	//{
	//	XmlSerializeAgent SerializeAgent { get; }
	//}

	public class XmlSerializeAgent
	{
		InstrumentBase _instrument = null;
		string _configPath = "";

		public XmlSerializeAgent( InstrumentBase instrument, string configPath )
		{
			this._instrument = instrument;
			this._configPath = configPath;
		}
		public int Load()
		{
			var result = 0;
			return result;
		}
		public int Save()
		{
			var result = 0;
			return result;
		}

	}
}