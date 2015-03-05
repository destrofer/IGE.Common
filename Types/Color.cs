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
    public struct Color : IEquatable<Color> {
		public static readonly Color Zero = new Color();

        public byte R;
        public byte G;
        public byte B;
        public byte A;
		
        public Color(int r, int g, int b, int a) {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = (byte)a;
        }

        public static bool operator == (Color left, Color right) {
            return left.R == right.R && left.G == right.G && left.B == right.B && left.A == right.A;
        }

        public static bool operator !=(Color left, Color right) {
            return left.R != right.R || left.G != right.G || left.B != right.B || left.A != right.A;
        }
		
        public bool Equals(Color other) {
            return  R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override bool Equals(object other) {
        	return other is Color && R == ((Color)other).R && G == ((Color)other).G && B == ((Color)other).B && A == ((Color)other).A;
        }

        public override int GetHashCode() {
            return unchecked((int)(((uint)R << 24) | ((uint)G << 16) | ((uint)B << 8) | (uint)A));
        }
		
        public override string ToString() {
            return String.Format("({0}, {1}, {2}, {3})", R, G, B, A);
        }
    }
}