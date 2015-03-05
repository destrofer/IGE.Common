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
	public static class CommonBinaryWriterExtensions {
		/*public static string ReadFixedSizeString(this BinaryReader reader, int byte_count) {
			return ReadFixedSizeString(reader, byte_count, Encoding.Default);
		}
		
		public static string ReadFixedSizeString(this BinaryReader reader, int byte_count, Encoding encoding) {
			string str = encoding.GetString(reader.ReadBytes(byte_count));
			int pos = str.IndexOf('\0');
			if( pos == 0 ) return "";
			return str.Substring(0, pos);
		}
		*/
		public static void Write(this BinaryWriter writer, Vector2 vector) {
			writer.Write(vector.X);
			writer.Write(vector.Y);
		}

		public static void Write(this BinaryWriter writer, Vector3 vector) {
			writer.Write(vector.X);
			writer.Write(vector.Y);
			writer.Write(vector.Z);
		}

		public static void Write(this BinaryWriter writer, Quaternion quaternion) {
			writer.Write(quaternion.X);
			writer.Write(quaternion.Y);
			writer.Write(quaternion.Z);
			writer.Write(quaternion.W);
		}

		public static void Write(this BinaryWriter writer, Color4 color) {
			writer.Write(color.R);
			writer.Write(color.G);
			writer.Write(color.B);
			writer.Write(color.A);
		}

		public static void Write(this BinaryWriter writer, AABR box) {
			writer.Write(box.X1);
			writer.Write(box.Y1);
			writer.Write(box.X2);
			writer.Write(box.Y2);
		}

		public static void Write(this BinaryWriter writer, Matrix4 matrix) {
			writer.Write(matrix.M11); writer.Write(matrix.M12); writer.Write(matrix.M13); writer.Write(matrix.M14);
			writer.Write(matrix.M21); writer.Write(matrix.M22); writer.Write(matrix.M23); writer.Write(matrix.M24);
			writer.Write(matrix.M31); writer.Write(matrix.M32); writer.Write(matrix.M33); writer.Write(matrix.M34);
			writer.Write(matrix.M41); writer.Write(matrix.M42); writer.Write(matrix.M43); writer.Write(matrix.M44);
		}
		
		public static void Write(this BinaryWriter writer, ISerializable obj) {
			obj.Serialize(writer);
		}
	}
}
