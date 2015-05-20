/*
 * Author: Viacheslav Soroka
 * 
 * This file is part of IGE <https://github.com/destrofer/IGE>.
 * 
 * IGE is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * IGE is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with IGE.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Text;

using IGE;

namespace System.IO {
	public static class CommonBinaryReaderExtensions {
		public static string ReadFixedSizeString(this BinaryReader reader, int byte_count) {
			return ReadFixedSizeString(reader, byte_count, Encoding.Default);
		}
		
		public static string ReadFixedSizeString(this BinaryReader reader, int byte_count, Encoding encoding) {
			if( byte_count == 0 ) return "";
			string str = encoding.GetString(reader.ReadBytes(byte_count));
			int pos = str.IndexOf('\0');
			if( pos < 0 ) return str;
			if( pos == 0 ) return "";
			return str.Substring(0, pos);
		}
		
		public static string ReadZeroTerminatedString(this BinaryReader reader, Encoding encoding) {
			byte c;
			byte[] zero = encoding.GetBytes("\0");
			int zeroLength = zero.Length, zeroMatch = 0;
			ByteQueue bytes = new ByteQueue();

			do {
				c = reader.ReadByte();
				bytes.Enqueue(c);
				if( c == 0 )
					zeroMatch++;
				else
					zeroMatch = 0;
			} while(zeroMatch < zeroLength);
			
			return encoding.GetString(bytes.Dequeue(bytes.Length - zeroLength));
		}
		
		public static Vector2 ReadVector2(this BinaryReader reader) {
			return new Vector2(reader.ReadSingle(), reader.ReadSingle());
		}

		public static Vector3 ReadVector3(this BinaryReader reader) {
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Quaternion ReadQuaternion(this BinaryReader reader) {
			return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Color4 ReadColor4(this BinaryReader reader) {
			return new Color4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static AABR ReadAABR(this BinaryReader reader) {
			return new AABR(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Matrix4 ReadMatrix4(this BinaryReader reader) {
			return new Matrix4(
				reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
				reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
				reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
				reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()
			);
		}

		public static ushort ReadUInt16(this BinaryReader reader, Endian endian) {
			if( endian == Endian.Little )
				return reader.ReadUInt16();
			ushort val = reader.ReadUInt16();
			val = unchecked((ushort)((val >> 8) | (val << 8)));
			return val;
		}

		public static short ReadInt16(this BinaryReader reader, Endian endian) {
			if( endian == Endian.Little )
				return reader.ReadInt16();
			short val = reader.ReadInt16();
			val = unchecked((short)((val >> 8) | (val << 8)));
			return val;
		}

		public static uint ReadUInt32(this BinaryReader reader, Endian endian) {
			if( endian == Endian.Little )
				return reader.ReadUInt32();
			uint val = reader.ReadUInt32();
			val = unchecked(((val >> 24) | ((val & 0xFF0000) >> 8) | ((val & 0xFF00) << 8) | (val << 24)));
			return val;
		}
		
		public static int ReadInt32(this BinaryReader reader, Endian endian) {
			if( endian == Endian.Little )
				return reader.ReadInt32();
			int val = reader.ReadInt32();
			val = unchecked(((val >> 24) | ((val & 0xFF0000) >> 8) | ((val & 0xFF00) << 8) | (val << 24)));
			return val;
		}

		public static ulong ReadUInt64(this BinaryReader reader, Endian endian) {
			if( endian == Endian.Little )
				return reader.ReadUInt64();
			ulong val = reader.ReadUInt64();
			val = unchecked(((val >> 56) | ((val & 0xFF000000000000UL) >> 40) | ((val & 0xFF0000000000UL) >> 24) | ((val & 0xFF00000000UL) >> 8) | ((val & 0xFF000000UL) << 8) | ((val & 0xFF0000UL) << 24) | ((val & 0xFF00UL) << 40) | (val << 56)));
			return val;
		}

		public static long ReadInt64(this BinaryReader reader, Endian endian) {
			if( endian == Endian.Little )
				return reader.ReadInt64();
			long val = reader.ReadInt64();
			val = unchecked(((val >> 56) | ((val & 0xFF000000000000L) >> 40) | ((val & 0xFF0000000000L) >> 24) | ((val & 0xFF00000000L) >> 8) | ((val & 0xFF000000L) << 8) | ((val & 0xFF0000L) << 24) | ((val & 0xFF00L) << 40) | (val << 56)));
			return val;
		}
		
		public static float ReadSingle(this BinaryReader reader, Endian endian) {
			if( endian == Endian.Little )
				return reader.ReadSingle();
			byte[] data = reader.ReadBytes(4);
			Array.Reverse(data);
			return BitConverter.ToSingle(data, 0);
		}
		
		public static double ReadDouble(this BinaryReader reader, Endian endian) {
			if( endian == Endian.Little )
				return reader.ReadDouble();
			byte[] data = reader.ReadBytes(8);
			Array.Reverse(data);
			return BitConverter.ToDouble(data, 0);
		}
	}

	public enum Endian : byte {
		Little,
		Big
	}
}
