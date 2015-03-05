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
    public struct Matrix4 : IEquatable<Matrix4> {
    	public float
    		M11, M12, M13, M14,
    		M21, M22, M23, M24,
    		M31, M32, M33, M34,
    		M41, M42, M43, M44;

        public static Matrix4 Identity = new Matrix4(
        	1, 0, 0, 0,
        	0, 1, 0, 0,
        	0, 0, 1, 0,
        	0, 0, 0, 1
        );

        public Matrix4(Vector3 right, Vector3 up, Vector3 forward, Vector3 transfer) {
        	M11 = right.X;		M12 = right.Y;		M13 = right.Z;		M14 = transfer.X;
        	M21 = up.X;			M22 = up.Y;			M23 = up.Z;			M24 = transfer.Y;
        	M31 = forward.X;	M32 = forward.Y;	M33 = forward.Z;	M34 = transfer.Z;
        	M41 = 0;			M42 = 0;			M43 = 0;			M44 = 1;
        }

        public Matrix4(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44
        ) {
        	M11 = m11; M12 = m12; M13 = m13; M14 = m14;
        	M21 = m21; M22 = m22; M23 = m23; M24 = m24;
        	M31 = m31; M32 = m32; M33 = m33; M34 = m34;
        	M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }

        public float Determinant {
            get {
                return
                    M11 * M22 * M33 * M44 - M11 * M22 * M34 * M43 + M11 * M23 * M34 * M42 - M11 * M23 * M32 * M44
                  + M11 * M24 * M32 * M43 - M11 * M24 * M33 * M42 - M12 * M23 * M34 * M41 + M12 * M23 * M31 * M44
                  - M12 * M24 * M31 * M43 + M12 * M24 * M33 * M41 - M12 * M21 * M33 * M44 + M12 * M21 * M34 * M43
                  + M13 * M24 * M31 * M42 - M13 * M24 * M32 * M41 + M13 * M21 * M32 * M44 - M13 * M21 * M34 * M42
                  + M13 * M22 * M34 * M41 - M13 * M22 * M31 * M44 - M14 * M21 * M32 * M43 + M14 * M21 * M33 * M42
                  - M14 * M22 * M33 * M41 + M14 * M22 * M31 * M43 - M14 * M23 * M31 * M42 + M14 * M23 * M32 * M41;
            }
        }

        public Vector4 Row1 { get { return new Vector4(M11, M12, M13, M14); } }
        public Vector4 Row2 { get { return new Vector4(M21, M22, M23, M24); } }
        public Vector4 Row3 { get { return new Vector4(M31, M32, M33, M34); } }
        public Vector4 Row4 { get { return new Vector4(M41, M42, M43, M44); } }

        public Vector4 Column1 { get { return new Vector4(M11, M21, M31, M41); } }
        public Vector4 Column2 { get { return new Vector4(M12, M22, M32, M42); } }
        public Vector4 Column3 { get { return new Vector4(M13, M23, M33, M43); } }
        public Vector4 Column4 { get { return new Vector4(M14, M24, M34, M44); } }

        public static Matrix4 Mult(Matrix4 left, Matrix4 right)
        {
            Matrix4 result;
            Mult(ref left, ref right, out result);
            return result;
        }

        public static void Mult(ref Matrix4 left, ref Matrix4 right, out Matrix4 result) {
            float
            	a11 = left.M11, a12 = left.M12, a13 = left.M13, a14 = left.M14,
                a21 = left.M21, a22 = left.M22, a23 = left.M23, a24 = left.M24,
                a31 = left.M31, a32 = left.M32, a33 = left.M33, a34 = left.M34,
                a41 = left.M41, a42 = left.M42, a43 = left.M43, a44 = left.M44,
                
                b11 = right.M11, b12 = right.M12, b13 = right.M13, b14 = right.M14,
                b21 = right.M21, b22 = right.M22, b23 = right.M23, b24 = right.M24,
                b31 = right.M31, b32 = right.M32, b33 = right.M33, b34 = right.M34,
                b41 = right.M41, b42 = right.M42, b43 = right.M43, b44 = right.M44;

            result.M11 = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
            result.M12 = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
            result.M13 = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
            result.M14 = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;
            result.M21 = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
            result.M22 = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
            result.M23 = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
            result.M24 = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;
            result.M31 = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
            result.M32 = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
            result.M33 = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
            result.M34 = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;
            result.M41 = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
            result.M42 = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
            result.M43 = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
            result.M44 = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;
        }

        public static void Mult(ref Matrix4 matrix, ref Vector3 vector) {
            float x = matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z + matrix.M41;
            float y = matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z + matrix.M42;
            vector.Z = matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z + matrix.M43;
            vector.X = x;
            vector.Y = y;
        }

        public static void Rotate(ref Matrix4 matrix, ref Vector3 vector) {
            float x = matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z;
            float y = matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z;
            vector.Z = matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z;
            vector.X = x;
            vector.Y = y;
        }

        public static Matrix4 Transpose(Matrix4 mat) {
            return new Matrix4(
            	mat.M11, mat.M21, mat.M31, mat.M41,
            	mat.M12, mat.M22, mat.M32, mat.M42,
            	mat.M13, mat.M23, mat.M33, mat.M43,
            	mat.M14, mat.M24, mat.M34, mat.M44
            );
        }
        
        public static void Transpose(ref Matrix4 inMatr, ref Matrix4 outMatr) {
        	float t;
        	// using temp in case when same matrix object is used to fetch output of the method
        	t = inMatr.M21; outMatr.M21 = inMatr.M12; outMatr.M12 = t;
        	t = inMatr.M31; outMatr.M31 = inMatr.M13; outMatr.M13 = t;
        	t = inMatr.M41; outMatr.M41 = inMatr.M14; outMatr.M14 = t;
        	t = inMatr.M32; outMatr.M32 = inMatr.M23; outMatr.M23 = t;
        	t = inMatr.M42; outMatr.M42 = inMatr.M24; outMatr.M24 = t;
        	t = inMatr.M43; outMatr.M43 = inMatr.M34; outMatr.M34 = t;
        }

        public static Matrix4 Perspective(float fov, float aspectRatio, float zNear, float zFar) {
            float yMax = zNear * (float)Math.Tan(0.5f * fov);
            float yMin = -yMax;
            float xMin = yMin * aspectRatio;
            float xMax = yMax * aspectRatio;
            
            float width = 1f / (xMax - xMin);
            float height = 1f / (yMax - yMin);
            float depth = 1f / (zFar - zNear);
            
            float near = 2f * zNear;
        
            float x = near * width;
            float y = near * height;
            float a = (xMax + xMin) * width;
            float b = (yMax + yMin) * height;
            float c = -(zFar + zNear) * depth;
            float d = -(zFar * near) * depth;
            
            return new Matrix4(
				x, 0, 0,  0,
				0, y, 0,  0,
				a, b, c, -1,
				0, 0, d,  0
			);
        }

        public static Matrix4 operator *(Matrix4 left, Matrix4 right) {
            return Matrix4.Mult(left, right);
        }
        
        public static bool operator ==(Matrix4 left, Matrix4 right) {
            return	left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14
            	&&	left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24
            	&&	left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M34 == right.M34
            	&&	left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43 && left.M44 == right.M44;
        }

        public static bool operator !=(Matrix4 left, Matrix4 right) {
            return	left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14
            	&&	left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24
            	&&	left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M34 != right.M34
            	&&	left.M41 != right.M41 || left.M42 != right.M42 || left.M43 != right.M43 || left.M44 != right.M44;
        }

        public bool Equals(Matrix4 other) {
            return
            		M11 == other.M11 && M12 == other.M12 && M13 == other.M13 && M14 == other.M14
            	&&	M21 == other.M21 && M22 == other.M22 && M23 == other.M23 && M24 == other.M24
            	&&	M31 == other.M31 && M32 == other.M32 && M33 == other.M33 && M34 == other.M34
            	&&	M41 == other.M41 && M42 == other.M42 && M43 == other.M43 && M44 == other.M44;
        }
        
        public override bool Equals(object other) {
            return
            	other is Matrix4
            	&&	M11 == ((Matrix4)other).M11 && M12 == ((Matrix4)other).M12 && M13 == ((Matrix4)other).M13 && M14 == ((Matrix4)other).M14
            	&&	M21 == ((Matrix4)other).M21 && M22 == ((Matrix4)other).M22 && M23 == ((Matrix4)other).M23 && M24 == ((Matrix4)other).M24
            	&&	M31 == ((Matrix4)other).M31 && M32 == ((Matrix4)other).M32 && M33 == ((Matrix4)other).M33 && M34 == ((Matrix4)other).M34
            	&&	M41 == ((Matrix4)other).M41 && M42 == ((Matrix4)other).M42 && M43 == ((Matrix4)other).M43 && M44 == ((Matrix4)other).M44;
        }
        
        public override int GetHashCode() {
            return
            		M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^ M14.GetHashCode()
            	^	M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^ M24.GetHashCode()
            	^	M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode() ^ M34.GetHashCode()
            	^	M41.GetHashCode() ^ M42.GetHashCode() ^ M43.GetHashCode() ^ M44.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}\n{4}, {5}, {6}, {7}\n{8}, {9}, {10}, {11}\n{12}, {13}, {14}, {15}",
            	M11, M12, M13, M14,
            	M21, M22, M23, M24,
            	M31, M32, M33, M34,
            	M41, M42, M43, M44
            );
        }
    }
}
