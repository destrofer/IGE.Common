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
	public struct Point2 : IEquatable<Point2> {
        public static readonly Point2 Zero = new Point2();
		
        public int X, Y;
		
        public Point2(int x, int y) {
            X = x;
            Y = y;
        }
		
        public static Point2 operator +(Point2 point, Size2 size) {
            return new Point2(point.X + size.Width, point.Y + size.Height);
        }
		
        public static Point2 operator -(Point2 point, Size2 size) {
            return new Point2(point.X - size.Width, point.Y - size.Height);
        }
		
        public static Point2 operator +(Point2 left, Point2 right) {
            return new Point2(left.X + right.X, left.Y + right.Y);
        }
		
        public static Point2 operator -(Point2 left, Point2 right) {
            return new Point2(left.X - right.X, left.Y - right.Y);
        }
		
        public static bool operator ==(Point2 left, Point2 right) {
            return left.X == right.X && left.Y == right.Y;
        }
		
        public static bool operator !=(Point2 left, Point2 right) {
            return left.X != right.X || left.Y != right.Y;
        }
		
        public bool Equals(Point2 other) {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object other) {
        	return other is Point2 && X == ((Point2)other).X && Y == ((Point2)other).Y;
        }

        public override int GetHashCode() {
            return X ^ Y;
        }

        public override string ToString() {
            return String.Format("({0}, {1})", X, Y);
        }
    }
}
