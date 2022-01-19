using System;
using System.Collections.ObjectModel;

namespace HiPA.Communicator
{
	public class CommandItem
	{
		public const int SIZE_CMD_LEN = 1;
		public const int SIZE_CMD_ID = 2;

		public int TotalLength => SIZE_CMD_LEN + this.CommandLength;
		public int CommandLength
		{
			get
			{
				int len = SIZE_CMD_ID + ( this.CommandParameters?.Length ?? 0 );
				return len;
			}
		}

		public int CommandId { get; set; }
		public byte[] CommandParameters { get; set; }

		public byte[] Serialzie()
		{
			var result = new byte[ this.TotalLength ];
			result[ 0 ] = ( byte )this.CommandLength;

			ModbusTool.SetLittleEndainUInt16( result, 1, this.CommandId );
			if ( this.CommandParameters != null )
				Array.Copy( this.CommandParameters, 0, result, 3, this.CommandParameters.Length );

			return result;
		}
		public CommandItem( int commandId, byte[] commandParameters = null )
		{
			this.CommandId = commandId;
			this.CommandParameters = commandParameters;
		}
		public CommandItem( int commandId, int SubCmd, float value, bool isLittleEndain )
		{
			try
			{
				byte[] Data = BitConverter.GetBytes( value );
				var bv = new byte[ 5 ];
				if ( isLittleEndain )
				{
					bv[ 0 ] = Data[ 0 ];
					bv[ 1 ] = Data[ 1 ];
					bv[ 2 ] = Data[ 2 ];
					bv[ 3 ] = Data[ 3 ];
					bv[ 4 ] = ( byte )SubCmd;
					this.CommandParameters = bv;
				}
				else
				{
					bv[ 0 ] = Data[ 3 ];
					bv[ 1 ] = Data[ 2 ];
					bv[ 2 ] = Data[ 1 ];
					bv[ 3 ] = Data[ 0 ];
					bv[ 4 ] = ( byte )SubCmd;
					this.CommandParameters = bv;
				}
			}
			catch
			{ }
			this.CommandId = commandId;

		}
		public CommandItem( int commandId, ushort value, bool isLittleEndain )
		{
			if ( isLittleEndain )
			{
				var bv = new byte[ 2 ];
				bv[ 0 ] = ( byte )( ( value >> 8 ) & 0xFF );
				bv[ 1 ] = ( byte )( value & 0xFF );
				this.CommandParameters = bv;
			}
			else
			{
				this.CommandParameters = BitConverter.GetBytes( value );
			}

			this.CommandId = commandId;
		}

		public CommandItem( int commandId, float value )
		{
			this.CommandId = commandId;
			this.CommandParameters = BitConverter.GetBytes( value );
		}

		public CommandItem( int commandId, int value, bool isLittleEndain )
		{
			if ( isLittleEndain )
			{
				var bv = new byte[ 4 ];
				bv[ 0 ] = ( byte )( ( value >> 24 ) & 0xFF );
				bv[ 1 ] = ( byte )( ( value >> 16 ) & 0xFF );
				bv[ 2 ] = ( byte )( ( value >> 8 ) & 0xFF );
				bv[ 3 ] = ( byte )( value & 0xFF );
				this.CommandParameters = bv;
			}
			else
			{
				this.CommandParameters = BitConverter.GetBytes( value );
			}

			this.CommandId = commandId;
		}


		public CommandItem( byte[] data, int offset, ref int nextIndex )
		{
			int commandLen = data[ offset++ ];
			int parameterLen = commandLen - SIZE_CMD_ID;

			this.CommandId = ModbusTool.GetLittleEndainUInt16( data, offset ); offset += 2;

			if ( parameterLen > 0 )
			{
				this.CommandParameters = new byte[ parameterLen ];
				Array.Copy( data, offset, this.CommandParameters, 0, parameterLen );
				offset += parameterLen;
			}

			nextIndex = offset;
		}

		public UInt16 GetParameterUInt16( int offset, bool isLittleEndain, ref int next )
		{
			UInt16 result = 0;
			if ( isLittleEndain )
			{
				result = ( UInt16 )( this.CommandParameters[ offset++ ] << 8 );
				result |= this.CommandParameters[ offset++ ];
			}
			else
			{
				result = BitConverter.ToUInt16( this.CommandParameters, offset );
				offset += 2;
			}
			next = offset;
			return result;
		}

		public int GetParameterInt( int offset, bool isLittleEndain, ref int next )
		{
			int result = 0;
			if ( isLittleEndain )
			{
				result |= ( this.CommandParameters[ offset++ ] << 24 );
				result |= ( this.CommandParameters[ offset++ ] << 16 );
				result |= ( this.CommandParameters[ offset++ ] << 8 );
				result |= this.CommandParameters[ offset++ ];
			}
			else
			{
				result = BitConverter.ToInt32( this.CommandParameters, offset );
				offset += 4;
			}
			next = offset;
			return result;
		}
		public float GetParameterFloat( int offset, ref int next )
		{
			float result = 0;
			result = BitConverter.ToSingle( this.CommandParameters, offset );
			next = offset + 4;
			return result;
		}
		public override string ToString()
		{
			var result =
				$"\tLEN: {this.CommandLength}\r\n";

			result += "\tCMD: ";
			result += ( ( byte )( this.CommandId >> 8 ) ).ToString( "X2" ) + " ";
			result += ( ( byte )this.CommandId & 0xFF ).ToString( "X2" ) + "\r\n";
			result += "\tARG: ";

			foreach ( var v in this.CommandParameters )
				result += v.ToString( "X2" ) + " ";

			return result;
		}
	}

	public class ModbusPackage
		: Collection<CommandItem>
	{
		public const byte ADDRESS_DEFAULT = 0x01;
		public const byte FUNC_CODE_SINGLE = 0x02;
		public const byte FUNC_CODE_BATCH = 0xF2;

		public const int SIZE_ADDRESS = 1;
		public const int SIZE_FUNC = 1;
		public const int SIZE_CRC = 2;
		public const int SIZE_CMDS_LEN = 2;

		public byte Address { get; set; }
		public byte FunCode { get; set; }
		public ushort CRC { get; private set; }

		public int CommandLength
		{
			get
			{
				int cmdLen = 0;
				foreach ( var cmd in this )
				{
					cmdLen += cmd.TotalLength;
				}
				return cmdLen;
			}
		}

		public bool IsValid => this.Count > 0;

		public ModbusPackage()
		{
			this.Address = ADDRESS_DEFAULT;
			this.FunCode = FUNC_CODE_BATCH;
		}

		public byte[] Serialize()
		{
			if ( this.IsValid == false ) return null;

			int cmdLen = this.CommandLength;
			int totalLen = SIZE_ADDRESS + SIZE_FUNC + SIZE_CMDS_LEN + SIZE_CRC + cmdLen;

			var packet = new byte[ totalLen ];
			packet[ 0 ] = this.Address;
			packet[ 1 ] = this.FunCode;

			// Command Total length, Little-Endian
			ModbusTool.SetLittleEndainUInt16( packet, 2, cmdLen );

			int curIndex = SIZE_ADDRESS + SIZE_FUNC + SIZE_CMDS_LEN;
			foreach ( var cmd in this )
			{
				var data = cmd.Serialzie();
				Array.Copy( data, 0, packet, curIndex, data.Length );
				curIndex += data.Length;
			}

			this.CRC = ModbusTool.CalculateCRC( packet, 0, packet.Length - SIZE_CRC );
			ModbusTool.SetLittleEndainUInt16( packet, curIndex, this.CRC ); curIndex += 2;

			return packet;
		}

		public static ModbusPackage Deserialize( byte[] data, int length, ref int next )
		{
			ModbusPackage result = null;

			do
			{
				if ( data == null ||
					data.Length == 0 )
					break;

				int offset = 0;
				if ( data.Length < length )
					break;

				if ( length < 4 )
					break;

				byte address = data[ offset++ ];
				byte funcCode = data[ offset++ ];

				int commandLen = 0;

				// check header : 0x02 func
				if ( funcCode == FUNC_CODE_SINGLE )
				{
					commandLen = data[ offset++ ];
				}
				// check header : 0xF2 func
				else if ( funcCode == FUNC_CODE_BATCH )
				{
					commandLen = ModbusTool.GetLittleEndainUInt16( data, offset );
					offset += 2;
				}

				if ( commandLen == 0 )
					break;

				// check total size
				if ( length < offset + commandLen + SIZE_CRC )
					break;

				int crcIndex = offset + commandLen;

				result = new ModbusPackage();
				result.Address = address;
				result.FunCode = funcCode;

				if ( funcCode == FUNC_CODE_SINGLE )
				{
					result.Add( new CommandItem( data, offset - 1, ref offset ) );
				}
				else if ( funcCode == FUNC_CODE_BATCH )
				{
					while ( offset < crcIndex )
					{
						var cmd = new CommandItem( data, offset, ref offset );
						result.Add( cmd );
					}
				}

				result.CRC = ( ushort )ModbusTool.GetLittleEndainUInt16( data, crcIndex );
				//result.CRC = ( ushort )( data[ crcIndex ] << 8 );
				//result.CRC |= ( data[ crcIndex + 1 ] );

				next = offset + SIZE_CRC;
			} while ( false );

			return result;
		}

		protected void Deserialize( byte[] data )
		{
			this.Clear();

			this.Address = data[ 0 ];
			this.FunCode = data[ 1 ];

			int commandLen = ( data[ 2 ] << 8 ) | ( data[ 3 ] );

			int cmdIndex = SIZE_ADDRESS + SIZE_FUNC + SIZE_CMDS_LEN;
			int crcIndex = commandLen + cmdIndex;
			this.CRC = ( ushort )( data[ crcIndex ] << 8 );
			this.CRC |= ( data[ crcIndex + 1 ] );

			while ( cmdIndex < crcIndex )
			{
				var commandItem = new CommandItem( data, cmdIndex, ref cmdIndex );
				this.Add( commandItem );
			}
		}

		public override string ToString()
		{
			var result =
				$"HEAD: {this.Address:X2}, {this.FunCode:X2}, {this.CRC:X4}\r\n" +
				$"LEN:  {this.CommandLength}\r\n" +
				"CMDS: \r\n";

			for ( int i = 0; i < this.Count; i++ )
				result += $"[{i}]: {this[ i ]}\r\n";

			return result;
		}
	}


	public static class ModbusTool
	{
		#region Set/Get Little-Endain value
		public static int GetLittleEndainUInt16( byte[] data, int offset )
		{
			int result = 0;
			result = data[ offset ] << 8;
			result |= data[ offset + 1 ] & 0xFF;
			return result;
		}
		public static void SetLittleEndainUInt16( byte[] data, int offset, int value )
		{
			data[ offset ] = ( byte )( ( value >> 8 ) & 0xFF );
			data[ offset + 1 ] = ( byte )( value & 0xFF );
		}
		#endregion
		#region Modbus RTU CRC16 
		public static UInt16 CalculateCRC( byte[] data, int offset, int length )
		{
			int crcLow = 0xFF;
			int crcHigh = 0xFF;
			if ( offset + length > data.Length )
				length = data.Length - offset;

			for ( int i = offset; i < offset + length; i++ )
			{
				var index = crcHigh ^ data[ i ];
				crcHigh = crcLow ^ HighTable[ index ];
				crcLow = LowTable[ index ];
			}

			return ( UInt16 )( crcHigh << 8 | crcLow );
		}
		#endregion

		#region Encoding/Decoding Table 
		readonly static byte[] HighTable = new byte[]{
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
			0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
			0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
			0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
			0x80, 0x41, 0x00, 0xC1, 0x81, 0x40};

		readonly static byte[] LowTable = new byte[]{
			0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
			0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
			0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
			0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
			0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
			0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
			0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
			0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
			0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
			0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
			0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
			0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
			0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
			0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
			0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
			0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
			0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
			0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
			0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
			0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
			0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
			0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
			0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
			0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
			0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
			0x43, 0x83, 0x41, 0x81, 0x80, 0x40};

		#endregion
	}

}
