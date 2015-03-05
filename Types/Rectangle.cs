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
    public struct Rectangle : IEquatable<Rectangle> {
        public static readonly Rectangle Zero = new Rectangle();
		
        public int X, Y, Width, Height;
		
        public int Left { get { return X; } set { X = value; } }
        public int Top { get { return Y; } set { Y = value; } }
        public int Right { get { return X + Width; } set { Width = value - X; } }
        public int Bottom { get { return Y + Height; } set { Height = value - Y; } }
		
        public Point2 Location { get { return new Point2(X, Y); } set { X = value.X; Y = value.Y; } }
        public Size2 Size { get { return new Size2(Width, Height); } set { Width = value.Width; Height = value.Height; } }
		
        public Rectangle(int x, int y, int width, int height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
		
        public static bool operator ==(Rectangle left, Rectangle right) {
            return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        }
		
        public static bool operator !=(Rectangle left, Rectangle right) {
            return left.X != right.X || left.Y != right.Y || left.Width != right.Width || left.Height != right.Height;
        }
		
        public bool Equals(Rectangle other) {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object other) {
        	return other is Rectangle && X == ((Rectangle)other).X && Y == ((Rectangle)other).Y && Width == ((Rectangle)other).Width && Height == ((Rectangle)other).Height;
        }

        public override int GetHashCode() {
            return X ^ Y ^ Width ^ Height;
        }

        public override string ToString() {
            return String.Format("({0}, {1}: {2}, {3})", X, Y, Width, Height);
        }

		public static Rectangle FromLTRB(int left, int top, int right, int bottom) {
            return new Rectangle(left, top, right - left, bottom - top);
        }

        public void FixLTRB() {
        	Width -= X;
        	Height -= Y;
        }
    }
}
