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
    public struct Point3 : IEquatable<Point3> {
        public static readonly Point3 Zero = new Point3();
		
        public int X, Y, Z;
		
        public Point3(int x, int y, int z) {
            X = x;
            Y = y;
            Z = z;
        }
		
        public static Point3 operator +(Point3 left, Point3 right) {
            return new Point3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }
		
        public static Point3 operator -(Point3 left, Point3 right) {
            return new Point3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }
		
        public static bool operator ==(Point3 left, Point3 right) {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }
		
        public static bool operator !=(Point3 left, Point3 right) {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }
		
        public bool Equals(Point3 other) {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object other) {
        	return other is Point3 && X == ((Point3)other).X && Y == ((Point3)other).Y && Z == ((Point3)other).Z;
        }

        public override int GetHashCode() {
            return X ^ Y ^ Z;
        }

        public override string ToString() {
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }
    }
}
