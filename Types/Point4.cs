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
	public struct Point4 : IEquatable<Point4> {
        public static readonly Point4 Zero = new Point4();
		
        public int X, Y, Z, W;
		
        public Point4(int x, int y, int z, int w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
		
        public static Point4 operator +(Point4 left, Point4 right) {
            return new Point4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }
		
        public static Point4 operator -(Point4 left, Point4 right) {
            return new Point4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }
		
        public static bool operator ==(Point4 left, Point4 right) {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
        }
		
        public static bool operator !=(Point4 left, Point4 right) {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z || left.W != right.W;
        }
		
        public bool Equals(Point4 other) {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override bool Equals(object other) {
        	return other is Point4 && X == ((Point4)other).X && Y == ((Point4)other).Y && Z == ((Point4)other).Z && W == ((Point4)other).W;
        }

        public override int GetHashCode() {
        	return X ^ Y ^ Z ^ W;
        }

        public override string ToString() {
            return String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
        }
    }
}
