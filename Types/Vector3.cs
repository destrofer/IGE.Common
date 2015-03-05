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
	public struct Vector3 : IEquatable<Vector3> {
        public static readonly Vector3 Zero = new Vector3();
		
        public float X, Y, Z;
		
        public Vector3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }
		
        public static Vector3 operator +(Vector3 left, Vector3 right) {
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }
		
        public static Vector3 operator -(Vector3 left, Vector3 right) {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }
		
        public static bool operator ==(Vector3 left, Vector3 right) {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }
		
        public static bool operator !=(Vector3 left, Vector3 right) {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }
		
        public bool Equals(Vector3 other) {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object other) {
        	return other is Vector3 && X == ((Vector3)other).X && Y == ((Vector3)other).Y && Z == ((Vector3)other).Z;
        }

        public override int GetHashCode() {
        	return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override string ToString() {
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }
        
		public float Length { get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); } }
		public float LengthSquared { get { return X * X + Y * Y + Z * Z; } }
				
		public static void Multiply(ref Vector3 v, float scale, ref Vector3 result) {
        	result.X = v.X * scale;
        	result.Y = v.Y * scale;
        	result.Z = v.Z * scale;
		}
		
        public float Dot(ref Vector3 other) {
        	return X * other.X + Y * other.Y + Z * other.Z;
        }
        
        public float Dot(Vector3 other) {
        	return X * other.X + Y * other.Y + Z * other.Z;
        }
 
        public static void CrossProduct(ref Vector3 left, ref Vector3 right, ref Vector3 result) {
        	result.X = left.Y * right.Z - left.Z * right.Y;
        	result.Y = left.Z * right.X - left.X * right.Z;
        	result.Z = left.X * right.Y - left.Y * right.X;
        }

		public void Normalize() {
        	float l = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        	if( l > 0f ) {
        		l = 1f / l;
        		X *= l;
        		Y *= l;
				Z *= l;
        	}
        }
        
        public void Normalize(float scale) {
        	float l = (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        	if( l > 0f ) {
        		l = scale / l;
        		X *= l;
        		Y *= l;
				Z *= l;
        	}
        }
		
		public static Vector3 Subtract(ref Vector3 left, ref Vector3 right) {
			return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}
		
		public static Vector3 AddScaled(ref Vector3 left, float scale, ref Vector3 right) {
			return new Vector3(left.X + scale * right.X, left.Y + scale * right.Y, left.Z + scale * right.Z);
		}
		public void AddScaled(float scale, ref Vector3 right) {
			X += scale * right.X;
			Y += scale * right.Y;
			Z += scale * right.Z;
		}
		
    }
}
