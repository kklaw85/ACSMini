using System;
using System.Diagnostics;
using System.Threading;

namespace HiPA.Communicator
{
	public class ModbusCommunicator : SerialCommunicator
	{
		public void SendPacket( ModbusPackage packet )
		{
			byte[] data = null;
			try
			{
				data = packet.Serialize();
			}
			catch ( Exception ex )
			{
				throw new CommunicatorParsingException( "ModbusCommunicator.cs :SendPacket: Unpack received data failure", ex );
			}

			this.Write( data, 0, data.Length );
			//if ( this.Logger != null )
			//{
			//	var byteLog = string.Join( " ", data.Select<byte, string>( d => d.ToString( "X2" ) ) );
			//	this.Logger?.Debug( $"COM[{this.PortName}] Size[{data.Length}] : {byteLog}" );
			//}
		}

		public void ReceivePacket( out ModbusPackage packet )
		{
			packet = null;

			var header = new byte[ 16 ];
			var headerSize = this.Read( header, 0, 4 );

			int desiredSize = 0;
			if ( header[ 1 ] == 0x02 )
				desiredSize = header[ 2 ] - 1;
			else if ( header[ 1 ] == 0xF2 )
			{
				desiredSize = header[ 2 ] << 8;
				desiredSize |= header[ 3 ];
			}
			else
				throw new CommunicatorParsingException( "ModbusCommunicator.cs :ReceivePacket: header data is invalid" );

			var body = new byte[ desiredSize + 2 ];
			var bodySize = this.Read( body, 0, body.Length );

			var buffer = new byte[ headerSize + bodySize ];
			Array.Copy( header, 0, buffer, 0, headerSize );
			Array.Copy( body, 0, buffer, headerSize, bodySize );

			try
			{
				int next = 0;
				packet = ModbusPackage.Deserialize( buffer, buffer.Length, ref next );
			}
			catch ( Exception ex )
			{
				throw new CommunicatorParsingException( "ModbusCommunicator.cs :ReceivePacket: Parsing received package failure.", ex );
			}
		}

		public void SendAndResponse( ModbusPackage toSend, out ModbusPackage read, int delay = 10 )
		{
			this.SendPacket( toSend );
			Thread.Sleep( ( int )( delay < 10 ? 10 : delay ) );
			this.ReceivePacket( out read );
		}

		protected byte[] Read( int size )
		{
			var buffer = new byte[ size ];
			int readSize = 0;

			var sw = new Stopwatch();
			sw.Restart();

			while ( readSize < size )
			{
				int ret = this.Read( buffer, readSize, size - readSize );
				readSize += ret;

				if ( sw.ElapsedMilliseconds > this.ReadTimeout )
					return null;
			}

			//if ( this.Logger != null )
			//{
			//	var byteLog = string.Join( " ", buffer.Select<byte, string>( d => d.ToString( "X2" ) ) );
			//	this.Logger?.Debug( $"COM[{this.PortName}] Size[{buffer.Length}] : {byteLog}" );
			//}
			return buffer;
		}


		#region 228 Communication protocal
		public int[] CRC_TB ={
		  0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
		  0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
		  0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40,
		  0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
		  0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,
		  0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
		  0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
		  0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
		  0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
		  0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,
		  0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
		  0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
		  0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
		  0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
		  0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,
		  0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
		  0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
		  0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
		  0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
		  0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,
		  0x7800, 0xB8C1, 0xb981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
		  0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
		  0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
		  0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
		  0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,
		  0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
		  0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
		  0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
		  0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
		  0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,
		  0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
		  0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040};

		public int GetModbusValue( byte[] txData, ref float fValue )
		{
			fValue = 0;
			// int iReadLen = 1024;
			int uiIndex = 0;
			int TxCheckCRC = 0xFFFF;
			byte[] bValue = new byte[ 4 ];
			// byte[] txData = new byte[9] { 1, 2, 0, 3, 2, 0, 35, 0, 0 };
			byte[] ReadData = new byte[ 1024 ];
			for ( int i = 0; i < 7; i++ )
			{
				uiIndex = ( TxCheckCRC ^ txData[ i ] ) & 0xFF;
				TxCheckCRC = ( TxCheckCRC >> 8 ) ^ this.CRC_TB[ uiIndex ];
			}
			txData[ 7 ] = ( byte )( TxCheckCRC & 0x00FF );
			txData[ 8 ] = ( byte )( TxCheckCRC >> 8 );

			//   string aa = WriteBytes(txData);
			//   ReadBytes(ref ReadData);

			if ( this.QueryDataBytes( txData, ref ReadData ) > 0 )
			{
				bValue[ 0 ] = ReadData[ 5 ];
				bValue[ 1 ] = ReadData[ 6 ];
				bValue[ 2 ] = ReadData[ 7 ];
				bValue[ 3 ] = ReadData[ 8 ];
				fValue = BitConverter.ToSingle( bValue, 0 );

			}
			else
			{
				Console.WriteLine( "获取数据失败！" );
				return -1;
			}
			return 0;
		}

		//Not Used in 228
		//public int SetModbusValue( byte[] txData, float[] fValues )
		//{

		//	// int iReadLen = 1024;
		//	int uiIndex = 0;
		//	int TxCheckCRC = 0xFFFF;

		//	// byte[] txData = new byte[9] { 1, 2, 0, 3, 2, 0, 35, 0, 0 };
		//	int iStart = txData.Length;
		//	byte[] SendData = new byte[ 1024 ];
		//	for ( int i = 0; i < iStart; i++ )
		//	{
		//		SendData[ i ] = txData[ i ];
		//	}
		//	SendData[ 3 ] = Convert.ToByte( 3 + fValues.Length * 4 );
		//	SendData[ 4 ] = Convert.ToByte( SendData[ 3 ] - 1 );
		//	byte[] ReadData = new byte[ 1024 ];
		//	for ( int i = 0; i < fValues.Length; i++ )
		//	{
		//		byte[] bValue = BitConverter.GetBytes( fValues[ i ] );
		//		int index = iStart + i * 4;
		//		SendData[ index ] = bValue[ 0 ];
		//		SendData[ index + 1 ] = bValue[ 1 ];
		//		SendData[ index + 2 ] = bValue[ 2 ];
		//		SendData[ index + 3 ] = bValue[ 3 ];
		//	}
		//	iStart = iStart + fValues.Length * 4;
		//	for ( int i = 0; i < iStart; i++ )
		//	{
		//		uiIndex = ( TxCheckCRC ^ SendData[ i ] ) & 0xFF;
		//		TxCheckCRC = ( TxCheckCRC >> 8 ) ^ this.CRC_TB[ uiIndex ];
		//	}
		//	SendData[ iStart ] = ( byte )( TxCheckCRC & 0x00FF );
		//	SendData[ iStart + 1 ] = ( byte )( TxCheckCRC >> 8 );

		//	int iSendLen = 9 + fValues.Length * 4;
		//	int iReadLen = ReadData.Length;
		//	this.QueryDataBytes( SendData, iSendLen, ref ReadData, ref iReadLen );
		//	if ( iReadLen > 0 )
		//	{
		//		//bValue[0] = ReadData[5];
		//		//bValue[1] = ReadData[6];
		//		//bValue[2] = ReadData[7];
		//		//bValue[3] = ReadData[8];
		//		//fValue = BitConverter.ToSingle(bValue, 0);
		//	}
		//	else
		//	{
		//		Console.WriteLine( "获取数据失败！" );
		//		return -1;
		//	}
		//	return 0;
		//}
		#endregion
	}
}
