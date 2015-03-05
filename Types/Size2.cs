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
using System.Runtime.InteropServices;

namespace IGE {
	[StructLayout(LayoutKind.Sequential)]
	public struct Size2 : IEquatable<Size2> {
        public static readonly Size2 Empty = new Size2();
        public static readonly Size2 Zero = new Size2();
        
        public int Width, Height;
        
        public Size2(int width, int height) : this() {
            Width = width;
            Height = height;
        }

        public bool IsEmpty {
            get { return Width == 0 && Height == 0; }
        }

        public static bool operator ==(Size2 left, Size2 right) {
            return left.Width == right.Width && left.Height == right.Height;
        }

		public static bool operator !=(Size2 left, Size2 right) {
            return left.Width != right.Width || left.Height != right.Height;
        }

		public bool Equals(Size2 other) {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object other) {
        	return other is Size2 && Width == ((Size2)other).Width && Height == ((Size2)other).Height;
        }

		public override int GetHashCode() {
            return Width ^ Height;
        }

		public override string ToString() {
            return String.Format("({0}, {1})", Width, Height);
        }
    }
}
