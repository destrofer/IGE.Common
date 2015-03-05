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
	public struct Vector4 : IEquatable<Vector4> {
        public static readonly Vector4 Zero = new Vector4();
		
        public float X, Y, Z, W;
		
        public Vector4(float x, float y, float z, float w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
		
        public static Vector4 operator +(Vector4 left, Vector4 right) {
            return new Vector4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }
		
        public static Vector4 operator -(Vector4 left, Vector4 right) {
            return new Vector4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }
		
        public static bool operator ==(Vector4 left, Vector4 right) {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
        }
		
        public static bool operator !=(Vector4 left, Vector4 right) {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z || left.W != right.W;
        }
		
        public bool Equals(Vector4 other) {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override bool Equals(object other) {
        	return other is Vector4 && X == ((Vector4)other).X && Y == ((Vector4)other).Y && Z == ((Vector4)other).Z && W == ((Vector4)other).W;
        }

        public override int GetHashCode() {
        	return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public override string ToString() {
            return String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
        }
    }
}
