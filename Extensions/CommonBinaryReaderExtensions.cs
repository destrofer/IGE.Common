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
			string str = encoding.GetString(reader.ReadBytes(byte_count));
			int pos = str.IndexOf('\0');
			if( pos == 0 ) return "";
			return str.Substring(0, pos);
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
	}
}
