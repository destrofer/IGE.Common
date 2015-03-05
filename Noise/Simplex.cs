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

namespace IGE {
	public class Simplex : INoiseGenerator {
		public const uint DefaultFrequency = 256;
		
		protected const double F2 = 0.5 * (1.7320508075688772935274463415059 - 1.0);
		protected const double G2 = (3.0 - 1.7320508075688772935274463415059) / 6.0;
		protected const double G22 = G2 * 2.0 - 1.0;
		
		protected const double F3 = 1.0 / 3.0;
		protected const double G3 = 1.0 / 6.0;
		
		protected const double F4 = (2.2360679774997896964091736687313 - 1.0) / 3.0;
		protected const double G4 = (5.0 - 2.2360679774997896964091736687313) / 20.0;
		protected const double G42 = G4 * 2.0;
		protected const double G43 = G4 * 3.0;
		protected const double G44 = G4 * 4.0 - 1.0;
		
		protected static readonly double[][] grad3 = new double[][] {
			new double[] { 1, 1, 0 }, new double[] { -1, 1, 0 }, new double[] { 1, -1, 0 }, new double[] { -1, -1, 0 },
			new double[] { 1, 0, 1 }, new double[] { -1, 0, 1 }, new double[] { 1, 0, -1 }, new double[] { -1, 0, -1 },
			new double[] { 0, 1, 1 }, new double[] { 0, -1, 1 }, new double[] { 0, 1, -1 }, new double[] { 0, -1, -1 }
		};
		
		protected static readonly double[][] grad4 = new double[][] {
			new double[] { 0, 1, 1, 1 }, new double[] { 0, 1, 1, -1 }, new double[] { 0, 1, -1, 1 }, new double[] { 0, 1, -1, -1 }, new double[] { 0, -1, 1, 1 }, new double[] { 0, -1, 1, -1 }, new double[] { 0, -1, -1, 1 }, new double[] { 0, -1, -1, -1 },
			new double[] { 1, 0, 1, 1 }, new double[] { 1, 0, 1, -1 }, new double[] { 1, 0, -1, 1 }, new double[] { 1, 0, -1, -1 }, new double[] { -1, 0, 1, 1 }, new double[] { -1, 0, 1, -1 }, new double[] { -1, 0, -1, 1 }, new double[] { -1, 0, -1, -1 },
			new double[] { 1, 1, 0, 1 }, new double[] { 1, 1, 0, -1 }, new double[] { 1, -1, 0, 1 }, new double[] { 1, -1, 0, -1 }, new double[] { -1, 1, 0, 1 }, new double[] { -1, 1, 0, -1 }, new double[] { -1, -1, 0, 1 }, new double[] { -1, -1, 0, -1 },
			new double[] { 1, 1, 1, 0 }, new double[] { 1, 1, -1, 0 }, new double[] { 1, -1, 1, 0 }, new double[] { 1, -1, -1, 0 }, new double[] { -1, 1, 1, 0 }, new double[] { -1, 1, -1, 0 }, new double[] { -1, -1, 1, 0 }, new double[] { -1, -1, -1, 0 },
		};
		
		protected static readonly int[][] simplex = new int[][] {
			new int[] { 0, 1, 2, 3 }, new int[] { 0, 1, 3, 2 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 2, 3, 1 },
			new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 1, 2, 3, 0 },
			new int[] { 0, 2, 1, 3 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 3, 1, 2 }, new int[] { 0, 3, 2, 1 },
			new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 1, 3, 2, 0 },
			new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 },
			new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 },
			new int[] { 1, 2, 0, 3 }, new int[] { 0, 0, 0, 0 }, new int[] { 1, 3, 0, 2 }, new int[] { 0, 0, 0, 0 },
			new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 2, 3, 0, 1 }, new int[] { 2, 3, 1, 0 },
			new int[] { 1, 0, 2, 3 }, new int[] { 1, 0, 3, 2 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 },
			new int[] { 0, 0, 0, 0 }, new int[] { 2, 0, 3, 1 }, new int[] { 0, 0, 0, 0 }, new int[] { 2, 1, 3, 0 },
			new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 },
			new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 },
			new int[] { 2, 0, 1, 3 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 },
			new int[] { 3, 0, 1, 2 }, new int[] { 3, 0, 2, 1 }, new int[] { 0, 0, 0, 0 }, new int[] { 3, 1, 2, 0 },
			new int[] { 2, 1, 0, 3 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 },
			new int[] { 3, 1, 0, 2 }, new int[] { 0, 0, 0, 0 }, new int[] { 3, 2, 0, 1 }, new int[] { 3, 2, 1, 0 }
		};
		
		private ulong m_Seed;
		public ulong Seed {
			get { return m_Seed; }
			set { m_Seed = value; RebuildArrays(); }
		}
		
		private double m_FrequencyD;
		private uint m_Frequency;
		public uint Frequency {
			get { return m_Frequency; }
			set { /* m_InvFrequencyD = 1.0 / (m_FrequencyD = (double)(m_Frequency = value)); RebuildArrays(); */ }
		}
		
		private double m_InvFrequencyD;
		public double InvFrequencyD { get { return m_InvFrequencyD; } }

		protected double m_ScaleX = 1.0;
		protected double m_ScaleY = 1.0;
		protected double m_ScaleZ = 1.0;
		protected double m_ScaleT = 1.0;
		protected double m_OffsetX = 0.0;
		protected double m_OffsetY = 0.0;
		protected double m_OffsetZ = 0.0;
		protected double m_OffsetT = 0.0;
		protected int m_Octaves = 1;
		
		public double ScaleX { get { return m_ScaleX; } set { m_ScaleX = value; } }
		public double ScaleY { get { return m_ScaleY; } set { m_ScaleY = value; } }
		public double ScaleZ { get { return m_ScaleZ; } set { m_ScaleZ = value; } }
		public double ScaleT { get { return m_ScaleT; } set { m_ScaleT = value; } }
		
		public double OffsetX { get { return m_OffsetX; } set { m_OffsetX = value; } }
		public double OffsetY { get { return m_OffsetY; } set { m_OffsetY = value; } }
		public double OffsetZ { get { return m_OffsetZ; } set { m_OffsetZ = value; } }
		public double OffsetT { get { return m_OffsetT; } set { m_OffsetT = value; } }
		
		public int Octaves {
			get { return m_Octaves; }
			set { m_Octaves = value; }
		}

		private int[] m_Indexes = new int[] { 151, 160, 137, 91, 90, 15, 131, 13, 201,
            95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37,
            240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
            94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56,
            87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139,
            48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133,
            230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25,
            63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200,
            196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3,
            64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255,
            82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
            223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
            101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79,
            113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242,
            193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249,
            14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204,
            176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222,
            114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
		private int[] m_IndexesMod12;
		private int[] m_IndexesMod32;
		
		public Simplex() : this(GameTimer.UTime, DefaultFrequency) {
		}

		public Simplex(ulong seed) : this(seed, DefaultFrequency) {
		}
		
		public Simplex(ulong seed, uint frequency) {
			frequency = 256; // it must be fixed for now
			
			m_Seed = seed;
			m_InvFrequencyD = 1.0 / (m_FrequencyD = (double)(m_Frequency = frequency));
			RebuildArrays();
		}

		protected double Dot(double[] v, double x, double y) {
			return v[0] * x + v[1] * y;
		}

		protected double Dot(double[] v, double x, double y, double z) {
			return v[0] * x + v[1] * y + v[2] * z;
		}

		protected double Dot(double[] v, double x, double y, double z, double t) {
			return v[0] * x + v[1] * y + v[2] * z + v[3] * t;
		}

		private void RebuildArrays() {
			int i, r;
			ExtRandom rnd = new ExtRandom(m_Seed);
			
			m_Indexes = new int[m_Frequency * 2];
			m_IndexesMod12 = new int[m_Frequency * 2];
			m_IndexesMod32 = new int[m_Frequency * 2];
			
			for( i = 0; i < m_Frequency; i++ ) {
				r = (int)(rnd.NextUInt32() & 0xFF);
				m_Indexes[i + m_Frequency] = m_Indexes[i] = r;
				m_IndexesMod12[i + m_Frequency] = m_IndexesMod12[i] = r % 12;
				m_IndexesMod32[i + m_Frequency] = m_IndexesMod32[i] = r % 32;
			}
		}
		
		public double this[double x] { get {
			if( m_Octaves <= 0 )
				return 0.0;
			x += m_OffsetX; x *= m_ScaleX;
			double amp = 2.0, pamp = 0.0, sum = 0.0;
			int i = m_Octaves;
			while( i-- > 0 ) {
				sum += Noise(x) * (amp *= 0.5);
				pamp += amp;
				x *= 2.0;
			}
			return sum / pamp;
		} }
		
		public double this[double x, double y] { get {
			if( m_Octaves <= 0 )
				return 0.0;
			x += m_OffsetX; x *= m_ScaleX;
			y += m_OffsetY; y *= m_ScaleY;
			double amp = 2.0, pamp = 0.0, sum = 0.0;
			int i = m_Octaves;
			while( i-- > 0 ) {
				sum += Noise(x, y) * (amp *= 0.5);
				pamp += amp;
				x *= 2.0; y *= 2.0;
			}
			return sum / pamp;
		} }
		
		public double this[double x, double y, double z] { get {
			if( m_Octaves <= 0 )
				return 0.0;
			x += m_OffsetX; x *= m_ScaleX;
			y += m_OffsetY; y *= m_ScaleY;
			z += m_OffsetZ; z *= m_ScaleZ;
			double amp = 2.0, pamp = 0.0, sum = 0.0;
			int i = m_Octaves;
			while( i-- > 0 ) {
				sum += Noise(x, y, z) * (amp *= 0.5);
				pamp += amp;
				x *= 2.0; y *= 2.0; z *= 2.0;
			}
			return sum / pamp;
		} }
		
		public double this[double x, double y, double z, double t] { get {
			if( m_Octaves <= 0 )
				return 0.0;
			x += m_OffsetX; x *= m_ScaleX;
			y += m_OffsetY; y *= m_ScaleY;
			z += m_OffsetZ; z *= m_ScaleZ;
			t += m_OffsetT; t *= m_ScaleT;
			double amp = 2.0, pamp = 0.0, sum = 0.0;
			int i = m_Octaves;
			while( i-- > 0 ) {
				sum += Noise(x, y, z, t) * (amp *= 0.5);
				pamp += amp;
				x *= 2.0; y *= 2.0; z *= 2.0; t *= 2.0;
			}
			return sum / pamp;
		} }
				
		public double Noise(double x) {
			return 0.0;
		}
		
		public double Noise(double x, double y) {
			double n0 = 0.0, n1 = 0.0, n2 = 0.0;
            double s = (x + y) * F2; // Hairy factor for 2D
            int i = (int)Math.Floor(x + s);
            int j = (int)Math.Floor(y + s);
            double d = (i + j) * G2;
            double x0 = x - (i - d);
            double y0 = y - (j - d);
            int i1, j1;
            if (x0 > y0) {
                    i1 = 1;
                    j1 = 0;
            }
            else {
                    i1 = 0;
                    j1 = 1;
            }
            double x1 = x0 - i1 + G2;
            double y1 = y0 - j1 + G2;
            double x2 = x0 + G22;
            double y2 = y0 + G22;
            int ii = i & 0xff;
            int jj = j & 0xff;
            double t0 = 0.5 - x0 * x0 - y0 * y0;
            if (t0 > 0) {
                    t0 *= t0;
                    int gi0 = m_IndexesMod12[ii + m_Indexes[jj]];
                    n0 = t0 * t0 * Dot(grad3[gi0], x0, y0); // (x,y) of grad3 used for
            }
            double t1 = 0.5 - x1 * x1 - y1 * y1;
            if (t1 > 0) {
                    t1 *= t1;
                    int gi1 = m_IndexesMod12[ii + i1 + m_Indexes[jj + j1]];
                    n1 = t1 * t1 * Dot(grad3[gi1], x1, y1);
            }
            double t2 = 0.5 - x2 * x2 - y2 * y2;
            if (t2 > 0) {
                    t2 *= t2;
                    int gi2 = m_IndexesMod12[ii + 1 + m_Indexes[jj + 1]];
                    n2 = t2 * t2 * Dot(grad3[gi2], x2, y2);
            }
            return 0.5 + 35.0 * (n0 + n1 + n2);
		}
		
		public double Noise(double x, double y, double z) {
			double n0 = 0.0, n1 = 0.0, n2 = 0.0, n3 = 0.0;
            double s = (x + y + z) * F3;
            int i = (int)Math.Floor(x + s);
            int j = (int)Math.Floor(y + s);
            int k = (int)Math.Floor(z + s);
            double d = (i + j + k) * G3;
            double x0 = x - (i - d);
            double y0 = y - (j - d);
            double z0 = z - (k - d);
            int i1, j1, k1;
            int i2, j2, k2;
            if (x0 >= y0) {
                    if (y0 >= z0) {
                            i1 = 1;
                            j1 = 0;
                            k1 = 0;
                            i2 = 1;
                            j2 = 1;
                            k2 = 0;
                    }
                    else if (x0 >= z0) {
                            i1 = 1;
                            j1 = 0;
                            k1 = 0;
                            i2 = 1;
                            j2 = 0;
                            k2 = 1;
                    }
                    else {
                            i1 = 0;
                            j1 = 0;
                            k1 = 1;
                            i2 = 1;
                            j2 = 0;
                            k2 = 1;
                    }
            } else {
                    if (y0 < z0) {
                            i1 = 0;
                            j1 = 0;
                            k1 = 1;
                            i2 = 0;
                            j2 = 1;
                            k2 = 1;
                    }
                    else if (x0 < z0) {
                            i1 = 0;
                            j1 = 1;
                            k1 = 0;
                            i2 = 0;
                            j2 = 1;
                            k2 = 1;
                    }
                    else {
                            i1 = 0;
                            j1 = 1;
                            k1 = 0;
                            i2 = 1;
                            j2 = 1;
                            k2 = 0;
                    }
            }
            double x1 = x0 - i1 + G3;
            double y1 = y0 - j1 + G3;
            double z1 = z0 - k1 + G3;

            double x2 = x0 - i2 + F3;
            double y2 = y0 - j2 + F3;
            double z2 = z0 - k2 + F3;

            double x3 = x0 - 0.5;
            double y3 = y0 - 0.5;
            double z3 = z0 - 0.5;
            int ii = i & 0xff;
            int jj = j & 0xff;
            int kk = k & 0xff;

            double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 > 0) {
                    t0 *= t0;
                    int gi0 = m_IndexesMod12[ii + m_Indexes[jj + m_Indexes[kk]]];
                    n0 = t0 * t0 * Dot(grad3[gi0], x0, y0, z0);
            }
            double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 > 0) {
                    t1 *= t1;
                    int gi1 = m_IndexesMod12[ii + i1 + m_Indexes[jj + j1 + m_Indexes[kk + k1]]];
                    n1 = t1 * t1 * Dot(grad3[gi1], x1, y1, z1);
            }
            double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 > 0) {
                    t2 *= t2;
                    int gi2 = m_IndexesMod12[ii + i2 + m_Indexes[jj + j2 + m_Indexes[kk + k2]]];
                    n2 = t2 * t2 * Dot(grad3[gi2], x2, y2, z2);
            }
            double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 > 0) {
                    t3 *= t3;
                    int gi3 = m_IndexesMod12[ii + 1 + m_Indexes[jj + 1 + m_Indexes[kk + 1]]];
                    n3 = t3 * t3 * Dot(grad3[gi3], x3, y3, z3);
            }
            // return 32.0 * (n0 + n1 + n2 + n3);
            return 0.5 + 16.0 * (n0 + n1 + n2 + n3);
		}
		
		public double Noise(double x, double y, double z, double w) {
            double n0 = 0.0, n1 = 0.0, n2 = 0.0, n3 = 0.0, n4 = 0.0;
            double s = (x + y + z + w) * F4;
            int i = (int)Math.Floor(x + s);
            int j = (int)Math.Floor(y + s);
            int k = (int)Math.Floor(z + s);
            int l = (int)Math.Floor(w + s);
            double d = (i + j + k + l) * G4;
            double x0 = x - (i - d);
            double y0 = y - (j - d);
            double z0 = z - (k - d);
            double w0 = w - (l - d);
            int c = 0;
            if (x0 > y0) {
                    c = 0x20;
            }
            if (x0 > z0) {
                    c |= 0x10;
            }
            if (y0 > z0) {
                    c |= 0x08;
            }
            if (x0 > w0) {
                    c |= 0x04;
            }
            if (y0 > w0) {
                    c |= 0x02;
            }
            if (z0 > w0) {
                    c |= 0x01;
            }
            int i1, j1, k1, l1;
            int i2, j2, k2, l2;
            int i3, j3, k3, l3;
            int[] sc = simplex[c];
            i1 = sc[0] >= 3 ? 1 : 0;
            j1 = sc[1] >= 3 ? 1 : 0;
            k1 = sc[2] >= 3 ? 1 : 0;
            l1 = sc[3] >= 3 ? 1 : 0;
            i2 = sc[0] >= 2 ? 1 : 0;
            j2 = sc[1] >= 2 ? 1 : 0;
            k2 = sc[2] >= 2 ? 1 : 0;
            l2 = sc[3] >= 2 ? 1 : 0;
            i3 = sc[0] >= 1 ? 1 : 0;
            j3 = sc[1] >= 1 ? 1 : 0;
            k3 = sc[2] >= 1 ? 1 : 0;
            l3 = sc[3] >= 1 ? 1 : 0;
            double x1 = x0 - i1 + G4;
            double y1 = y0 - j1 + G4;
            double z1 = z0 - k1 + G4;
            double w1 = w0 - l1 + G4;

            double x2 = x0 - i2 + G42;
            double y2 = y0 - j2 + G42;
            double z2 = z0 - k2 + G42;
            double w2 = w0 - l2 + G42;

            double x3 = x0 - i3 + G43;
            double y3 = y0 - j3 + G43;
            double z3 = z0 - k3 + G43;
            double w3 = w0 - l3 + G43;

            double x4 = x0 + G44;
            double y4 = y0 + G44;
            double z4 = z0 + G44;
            double w4 = w0 + G44;

            int ii = i & 0xff;
            int jj = j & 0xff;
            int kk = k & 0xff;
            int ll = l & 0xff;

            double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t0 > 0) {
                    t0 *= t0;
                    int gi0 = m_IndexesMod32[ii + m_Indexes[jj + m_Indexes[kk + m_Indexes[ll]]]];
                    n0 = t0 * t0 * Dot(grad4[gi0], x0, y0, z0, w0);
            }
            double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 > 0) {
                    t1 *= t1;
                    int gi1 = m_IndexesMod32[ii + i1 + m_Indexes[jj + j1 + m_Indexes[kk + k1 + m_Indexes[ll + l1]]]];
                    n1 = t1 * t1 * Dot(grad4[gi1], x1, y1, z1, w1);
            }
            double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 > 0) {
                    t2 *= t2;
                    int gi2 = m_IndexesMod32[ii + i2 + m_Indexes[jj + j2 + m_Indexes[kk + k2 + m_Indexes[ll + l2]]]];
                    n2 = t2 * t2 * Dot(grad4[gi2], x2, y2, z2, w2);
            }
            double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 > 0) {
                    t3 *= t3;
                    int gi3 = m_IndexesMod32[ii + i3 + m_Indexes[jj + j3 + m_Indexes[kk + k3 + m_Indexes[ll + l3]]]];
                    n3 = t3 * t3 * Dot(grad4[gi3], x3, y3, z3, w3);
            }
            double t4 = 0.6 - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 > 0) {
                    t4 *= t4;
                    int gi4 = m_IndexesMod32[ii + 1 + m_Indexes[jj + 1 + m_Indexes[kk + 1 + m_Indexes[ll + 1]]]];
                    n4 = t4 * t4 * Dot(grad4[gi4], x4, y4, z4, w4);
            }
            // return 27.0 * (n0 + n1 + n2 + n3 + n4);
            return 0.5 + 13.5 * (n0 + n1 + n2 + n3 + n4);
		}
		
		// TODO: Add octaves
		public double SphericalNoise(double size, double lon, double lat) {
			return SphericalNoise(size, Math.Cos(lon), Math.Sin(lon), Math.Cos(lat), Math.Sin(lat));
		}
		
		public double SphericalNoise(double size, double lonCos, double lonSin, double latCos, double latSin) {
			double val, initialSize = size;
			double x = (latCos * lonCos + 1.0) * 0.5 * size, y = (latSin + 1.0) * 0.5 * size, z = (latCos * lonSin + 1.0) * 0.5 * size;
			for( val = 0.0; size >= 1.0; size *= 0.5 )
				val += Noise(x / size, y / size, z / size) * size;
			return val / initialSize;
		}
	}
}