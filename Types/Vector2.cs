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
	public struct Vector2 : IEquatable<Vector2> {
        public static readonly Vector2 Zero = new Vector2();
		
        public float X, Y;
		
        public Vector2(float x, float y) {
            X = x;
            Y = y;
        }
        
        public float Length { get { return (float)Math.Sqrt(X * X + Y * Y); } }
        public float Length2 { get { return X * X + Y * Y; } }
        
        public float Dot(ref Vector2 other) {
        	return X * other.X + Y * other.Y;
        }
        
        public float Dot(Vector2 other) {
        	return X * other.X + Y * other.Y;
        }
        
        /// <summary>
        /// 2D Cross multiplication: zScale = thisVec * otherVec
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float Cross(ref Vector2 other) {
        	return X * other.Y - Y * other.X;
        }
        
        /// <summary>
        /// 2D Cross multiplication: outVec = thisVec * zScale 
        /// </summary>
        /// <param name="zScale"></param>
        /// <returns></returns>
        public Vector2 Cross(float zScale) {
        	return new Vector2(zScale * Y, -zScale * X);
        }
        
        /// <summary>
        /// 2D Cross multiplication: outVec = zScale * thisVec
        /// </summary>
        /// <param name="zScale"></param>
        /// <returns></returns>
        public Vector2 CrossInverse(float zScale) {
        	return Cross(-zScale);
        }
        
        public void CopyFrom(ref Vector2 other) {
        	X = other.X;
        	Y = other.Y;
        }

        public void CopyTo(ref Vector2 other) {
        	other.X = X;
        	other.Y = Y;
        }
        
        public static void Subtract(ref Vector2 left, ref Vector2 right, ref Vector2 result) {
        	result.X = left.X - right.X;
        	result.Y = left.Y - right.Y;
        }
        
        public static Vector2 Subtract(ref Vector2 left, ref Vector2 right) {
        	return new Vector2(left.X - right.X, left.Y - right.Y);
        }
        
        public void Subtract(ref Vector2 right) {
        	X -= right.X;
        	Y -= right.Y;
        }

        public static Vector2 Subtract(Vector2 left, Vector2 right) {
        	return new Vector2(left.X - right.X, left.Y - right.Y);
        }
        
        public static void Add(ref Vector2 left, ref Vector2 right, ref Vector2 result) {
        	result.X = left.X + right.X;
        	result.Y = left.Y + right.Y;
        }
        
        public static Vector2 Add(ref Vector2 left, ref Vector2 right) {
        	return new Vector2(left.X + right.X, left.Y + right.Y);
        }
        
        public static Vector2 Add(Vector2 left, Vector2 right) {
        	return new Vector2(left.X + right.X, left.Y + right.Y);
        }
        
        public void Add(Vector2 right) {
        	X += right.X;
        	Y += right.Y;
        }

        public void Add(ref Vector2 right) {
        	X += right.X;
        	Y += right.Y;
        }
        
        public static void AddScaled(ref Vector2 left, float scale, ref Vector2 right, ref Vector2 result) {
        	result.X = left.X + scale * right.X;
        	result.Y = left.Y + scale * right.Y;
        }
        
        public static Vector2 AddScaled(ref Vector2 left, float scale, ref Vector2 right) {
        	return new Vector2(left.X + scale * right.X, left.Y + scale * right.Y);
        }
        
        public void AddScaled(float scale, ref Vector2 right) {
        	X += scale * right.X;
        	Y += scale * right.Y;
        }
        
        public void AddDelta(ref Vector2 left, ref Vector2 right) {
        	X += left.X - right.X;
        	Y += left.Y - right.Y;
        }
        
        public void AddScaledDelta(float scale, ref Vector2 left, ref Vector2 right) {
        	X += scale * (left.X - right.X);
        	Y += scale * (left.Y - right.Y);
        }

        public void AddScaledSum(float scale, ref Vector2 left, ref Vector2 right) {
        	X += scale * (left.X + right.X);
        	Y += scale * (left.Y + right.Y);
        }
        
        public void Normalize() {
        	float l = (float)Math.Sqrt(X * X + Y * Y);
        	if( l > 0f ) {
        		l = 1f / l;
        		X *= l;
        		Y *= l;
        	}
        }
        
        public void Normalize(float scale) {
        	float l = (float)Math.Sqrt(X * X + Y * Y);
        	if( l > 0f ) {
        		l = scale / l;
        		X *= l;
        		Y *= l;
        	}
        }
        
        public static void Scale(ref Vector2 inVec, float scale, ref Vector2 outVec) {
        	outVec.X = scale * inVec.X;
        	outVec.Y = scale * inVec.Y;
        }
		
        public void Scale(float scale) {
        	X *= scale;
        	Y *= scale;
        }
		
        public static Vector2 operator +(Vector2 left, Vector2 right) {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }
		
        public static Vector2 operator -(Vector2 left, Vector2 right) {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }
        
        public static Vector2 operator *(Vector2 left, Vector2 right) {
        	return new Vector2(left.Y - right.Y, right.X - left.X);
        }
		
        public static bool operator ==(Vector2 left, Vector2 right) {
            return left.X == right.X && left.Y == right.Y;
        }
		
        public static bool operator !=(Vector2 left, Vector2 right) {
            return left.X != right.X || left.Y != right.Y;
        }
		
        public bool Equals(Vector2 other) {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object other) {
        	return other is Vector2 && X == ((Vector2)other).X && Y == ((Vector2)other).Y;
        }

        public override int GetHashCode() {
        	return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString() {
            return String.Format("({0}, {1})", X, Y);
        }
    }
}
