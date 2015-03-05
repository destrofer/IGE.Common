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
using System.ComponentModel;
using System.Xml.Serialization;

namespace IGE {
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion> {
		public float X;
		public float Y;
		public float Z;
		public float W;

		public static Quaternion Identity = new Quaternion(0, 0, 0, 1);

		public Quaternion(Vector3 v, float w) : this(v.X, v.Y, v.Z, w) {
		}
		
		public Quaternion(float x, float y, float z, float w) {
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector3 XYZ {
			get {
				return new Vector3(X, Y, Z);
			}
			set {
				X = value.X;
				Y = value.Y;
				Z = value.Z;
			}
		}

		public void ToAxisAndAngle(out Vector3 axis, out float angle) {
			if( Math.Abs(W) > 1f )
				Normalize();
			angle = 2f * (float)Math.Acos(W);
			float t = (float)Math.Sqrt(1.0 - W * W);
			if( t > 0.0001f ) {
				t = 1f / t;
				axis = new Vector3(X * t, Y * t, Z * t);
			}
			else
				axis = new Vector3(1f, 0f, 0f);
		}

		public float Length { get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W); } }
		public float LengthSquared { get { return X * X + Y * Y + Z * Z + W * W; } }

		public void Normalize() {
			float t = 1.0f / (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
			if( t != 0f ) {
				X *= t;
				Y *= t;
				Z *= t;
				W *= t;
			}
		}

		public static void Add(ref Quaternion left, ref Quaternion right, ref Quaternion result) {
			result.X = left.X + right.X;
			result.Y = left.Y + right.Y;
			result.Z = left.Z + right.Z;
			result.W = left.W + right.W;
		}

		public static void Subtract(ref Quaternion left, ref Quaternion right, ref Quaternion result) {
			result.X = left.X - right.X;
			result.Y = left.Y - right.Y;
			result.Z = left.Z - right.Z;
			result.W = left.W - right.W;
		}

		public static void Multiply(ref Quaternion left, ref Quaternion right, ref Quaternion result) {
			// V = V1 * W2 + V2 * W1 + cross(V1, V2)
			// W = W1 * W2 - dot(V1. V2)
			
        	result.X = left.X * right.W + right.X * left.W + left.Y * right.Z - left.Z * right.Y;
        	result.Y = left.Y * right.W + right.Y * left.W + left.Z * right.X - left.X * right.Z;
        	result.Z = left.Z * right.W + right.Z * left.W + left.X * right.Y - left.Y * right.X;
        	result.W = left.W * right.W - (left.X * right.X + left.Y * right.Y + left.Z * right.Z); 
		}
		
		public static void Multiply(ref Quaternion q, float scale, ref Quaternion result) {
        	result.X = q.X * scale;
        	result.Y = q.Y * scale;
        	result.Z = q.Z * scale;
        	result.W = q.W * scale;
		}

		public void Conjugate() {
			X = -X;
			Y = -Y;
			Z = -Z;
		}

		public static void Invert(ref Quaternion qIn, ref Quaternion qOut) {
			float l = qIn.LengthSquared;
			if( l == 0f ) {
				qOut.X = qIn.X;
				qOut.Y = qIn.Y;
				qOut.Z = qIn.Z;
				qOut.W = qIn.W;
			}
			else {
				l = -1f / l;
				qOut.X = qIn.X * l;
				qOut.Y = qIn.Y * l;
				qOut.Z = qIn.Z * l;
				qOut.W = qIn.W * -l;
			}
		}
        
		public void FromAxisAndAngle(Vector3 axis, float angle) {
			float l = axis.LengthSquared;
			if (l == 0.0f) {
				X = 0f;
				Y = 0f;
				Z = 0f;
				W = 1f;
				return;
			}
			
			angle *= 0.5f;
			l = (float)Math.Sin(angle) / (float)Math.Sqrt(l);
			X = axis.X * l;
			Y = axis.Y * l;
			Z = axis.Z * l;
			W = (float)Math.Cos(angle);
		}

		public static bool operator ==(Quaternion left, Quaternion right) {
			return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
		}
		
		public static bool operator !=(Quaternion left, Quaternion right) {
			return left.X != right.X || left.Y != right.Y || left.Z != right.Z || left.W != right.W;
		}
		
		public override bool Equals(object other) {
			return (other is Quaternion) && this == (Quaternion)other;
		}

		public bool Equals(Quaternion other) {
			return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
		}

		public override int GetHashCode() {
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
		}
		
		public override string ToString() {
			return String.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
		}
    }
}
