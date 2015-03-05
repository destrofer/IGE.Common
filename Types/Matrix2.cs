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
    public struct Matrix2 : IEquatable<Matrix2> {
    	public float
    		M11, M12,
    		M21, M22;

        public static Matrix2 Identity = new Matrix2(
        	1, 0,
        	0, 1
        );

        public Matrix2(Vector2 right, Vector2 up) {
        	M11 = right.X;		M12 = right.Y;
        	M21 = up.X;			M22 = up.Y;
        }

        public Matrix2(
            float m11, float m12,
            float m21, float m22
        ) {
        	M11 = m11; M12 = m12;
        	M21 = m21; M22 = m22;
        }

        public float Determinant {
            get {
                return
                    M11 * M22 - M12 * M21;
            }
        }

        public Vector2 Row1 { get { return new Vector2(M11, M12); } }
        public Vector2 Row2 { get { return new Vector2(M21, M22); } }

        public Vector2 Column1 { get { return new Vector2(M11, M21); } }
        public Vector2 Column2 { get { return new Vector2(M12, M22); } }

        public static Matrix2 Mult(Matrix2 left, Matrix2 right)
        {
            Matrix2 result;
            Mult(ref left, ref right, out result);
            return result;
        }

        public static void Mult(ref Matrix2 left, ref Matrix2 right, out Matrix2 result) {
            float
            	a11 = left.M11, a12 = left.M12,
                a21 = left.M21, a22 = left.M22,
                
                b11 = right.M11, b12 = right.M12,
                b21 = right.M21, b22 = right.M22;

            result.M11 = a11 * b11 + a12 * b21;
            result.M12 = a11 * b12 + a12 * b22;
            result.M21 = a21 * b11 + a22 * b21;
            result.M22 = a21 * b12 + a22 * b22;
        }

        public static void Mult(ref Matrix2 matrix, ref Vector2 vector) {
            float x = matrix.M11 * vector.X + matrix.M21 * vector.Y;
            float y = matrix.M12 * vector.X + matrix.M22 * vector.Y;
            vector.X = x;
            vector.Y = y;
        }

        public static Matrix2 Transpose(Matrix2 mat) {
            return new Matrix2(
            	mat.M11, mat.M21,
            	mat.M12, mat.M22
            );
        }
        
        public static void Transpose(ref Matrix2 inMatr, ref Matrix2 outMatr) {
        	float t;
        	// using temp in case when same matrix object is used to fetch output of the method
        	t = inMatr.M21; outMatr.M21 = inMatr.M12; outMatr.M12 = t;
        }

        public static Matrix2 operator *(Matrix2 left, Matrix2 right) {
            return Matrix2.Mult(left, right);
        }
        
        public static bool operator ==(Matrix2 left, Matrix2 right) {
            return	left.M11 == right.M11 && left.M12 == right.M12
            	&&	left.M21 == right.M21 && left.M22 == right.M22;
        }

        public static bool operator !=(Matrix2 left, Matrix2 right) {
            return	left.M11 != right.M11 || left.M12 != right.M12
            	&&	left.M21 != right.M21 || left.M22 != right.M22;
        }

        public bool Equals(Matrix2 other) {
            return
            		M11 == other.M11 && M12 == other.M12
            	&&	M21 == other.M21 && M22 == other.M22;
        }
        
        public override bool Equals(object other) {
            return
            	other is Matrix2
            	&&	M11 == ((Matrix2)other).M11 && M12 == ((Matrix2)other).M12
            	&&	M21 == ((Matrix2)other).M21 && M22 == ((Matrix2)other).M22;
        }
        
        public override int GetHashCode() {
            return
            		M11.GetHashCode() ^ M12.GetHashCode()
            	^	M21.GetHashCode() ^ M22.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}\n{2}, {3}",
            	M11, M12,
            	M21, M22
            );
        }
    }
}
