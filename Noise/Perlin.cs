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
	public class Perlin : INoiseGenerator {
		public const uint DefaultFrequency = 256;
		
		private ulong m_Seed;
		public ulong Seed {
			get { return m_Seed; }
			set { m_Seed = value; RebuildArrays(); }
		}
		
		private double m_FrequencyD;
		private uint m_Frequency;
		public uint Frequency {
			get { return m_Frequency; }
			set { m_InvFrequencyD = 1.0 / (m_FrequencyD = (double)(m_Frequency = value)); RebuildArrays(); }
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

		private double[] m_Values;
		private int[,] m_Indexes;
		
		public Perlin() : this(GameTimer.UTime, DefaultFrequency) {
		}

		public Perlin(ulong seed) : this(seed, DefaultFrequency) {
		}
		
		public Perlin(ulong seed, uint freqency) {
			m_Seed = seed;
			m_InvFrequencyD = 1.0 / (m_FrequencyD = (double)(m_Frequency = freqency));
			RebuildArrays();
		}

		private void RebuildArrays() {
			if( m_Frequency < 1 )
				throw new Exception("Perlin noise frequency cannot be less than 1");
			
			int i, j, k, r, t;
			ExtRandom rnd = new ExtRandom(m_Seed);
			
			m_Values = new double[m_Frequency];
			m_Indexes = new int[4, m_Frequency * 2]; // for optimization we take 2 times more memory, but in the end we'll not need to "mod" a lot.
			
			for( i = 0; i < m_Frequency; i++ ) {
				m_Values[i] = rnd.NextDouble();
				m_Indexes[0, i] = m_Indexes[1, i] = m_Indexes[2, i] = m_Indexes[3, i] = i;
			}
			
			for( j = 0; j < 4; j++ ) {
				// for more reliability
				for( k = 0; k < 5; k++ ) {
					for( i = 0; i < m_Frequency; i++ ) {
						r = (int)(rnd.NextUInt64() % (ulong)m_Frequency);
						t = m_Indexes[j, i];
						m_Indexes[j, i] = m_Indexes[j, r];
						m_Indexes[j, r] = t;
						//if( r != i )
						//	m_Indexes[j, i] ^= m_Indexes[j, r] ^= m_Indexes[j, i] ^= m_Indexes[j, r]; // swap values of two elements (commented, because it was somehow buggy)
					}
				}
				for( i = 0; i < m_Frequency; i++ )
					m_Indexes[j, m_Frequency + i] = m_Indexes[j, i]; // as was said before - needed for optimization
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
			double xt;
			int xi = Utils.ToWrappedIndex(x, m_Frequency, out xt);
			xt *= xt * xt * (xt * (xt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation
			
			int vi1 = m_Indexes[0, xi];
			int vi2 = m_Indexes[0, xi + 1];
			
			double v1 = m_Values[vi1];
			double v2 = m_Values[vi2];
			
			return v1 + xt * (v2 - v1);
		}
		
		public double Noise(double x, double y) {
			double xt;
			int xi = Utils.ToWrappedIndex(x, m_Frequency, out xt);
			xt *= xt * xt * (xt * (xt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			double yt;
			int yi = Utils.ToWrappedIndex(y, m_Frequency, out yt);
			yt *= yt * yt * (yt * (yt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			int xi1 = m_Indexes[1, yi];
			int xi2 = m_Indexes[1, yi + 1];

			int vi11 = m_Indexes[0, xi + xi1];
			int vi12 = m_Indexes[0, xi + xi1 + 1];
			int vi21 = m_Indexes[0, xi + xi2];
			int vi22 = m_Indexes[0, xi + xi2 + 1];

			double v11 = m_Values[vi11];
			double v12 = m_Values[vi12];
			double v21 = m_Values[vi21];
			double v22 = m_Values[vi22];
			
			double v1 = v11 + xt * (v12 - v11);
			double v2 = v21 + xt * (v22 - v21);
			
			return v1 + yt * (v2 - v1);
		}
		
		public double Noise(double x, double y, double z) {
			double xt;
			int xi = Utils.ToWrappedIndex(x, m_Frequency, out xt);
			xt *= xt * xt * (xt * (xt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			double yt;
			int yi = Utils.ToWrappedIndex(y, m_Frequency, out yt);
			yt *= yt * yt * (yt * (yt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			double zt;
			int zi = Utils.ToWrappedIndex(z, m_Frequency, out zt);
			zt *= zt * zt * (zt * (zt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			int yi1 = m_Indexes[2, zi];
			int yi2 = m_Indexes[2, zi + 1];

			int xi11 = m_Indexes[1, yi + yi1];
			int xi12 = m_Indexes[1, yi + yi1 + 1];
			int xi21 = m_Indexes[1, yi + yi2];
			int xi22 = m_Indexes[1, yi + yi2 + 1];

			int vi111 = m_Indexes[0, xi + xi11];
			int vi112 = m_Indexes[0, xi + xi11 + 1];
			int vi121 = m_Indexes[0, xi + xi12];
			int vi122 = m_Indexes[0, xi + xi12 + 1];
			int vi211 = m_Indexes[0, xi + xi21];
			int vi212 = m_Indexes[0, xi + xi21 + 1];
			int vi221 = m_Indexes[0, xi + xi22];
			int vi222 = m_Indexes[0, xi + xi22 + 1];

			double v111 = m_Values[vi111];
			double v112 = m_Values[vi112];
			double v121 = m_Values[vi121];
			double v122 = m_Values[vi122];
			double v211 = m_Values[vi211];
			double v212 = m_Values[vi212];
			double v221 = m_Values[vi221];
			double v222 = m_Values[vi222];

			double v11 = v111 + xt * (v112 - v111);
			double v12 = v121 + xt * (v122 - v121);
			double v21 = v211 + xt * (v212 - v211);
			double v22 = v221 + xt * (v222 - v221);

			double v1 = v11 + yt * (v12 - v11);
			double v2 = v21 + yt * (v22 - v21);
			
			return v1 + zt * (v2 - v1);
		}		
		
		public double Noise(double x, double y, double z, double t) {
			double xt;
			int xi = Utils.ToWrappedIndex(x, m_Frequency, out xt);
			xt *= xt * xt * (xt * (xt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			double yt;
			int yi = Utils.ToWrappedIndex(y, m_Frequency, out yt);
			yt *= yt * yt * (yt * (yt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			double zt;
			int zi = Utils.ToWrappedIndex(z, m_Frequency, out zt);
			zt *= zt * zt * (zt * (zt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			double tt;
			int ti = Utils.ToWrappedIndex(t, m_Frequency, out tt);
			tt *= tt * tt * (tt * (tt * 6.0 - 15.0) + 10.0); // "fade" by Ken Perlin, which is aproximately very close to cosine interpolation

			int zi1 = m_Indexes[3, ti];
			int zi2 = m_Indexes[3, ti + 1];

			int yi11 = m_Indexes[2, zi + zi1];
			int yi12 = m_Indexes[2, zi + zi1 + 1];
			int yi21 = m_Indexes[2, zi + zi2];
			int yi22 = m_Indexes[2, zi + zi2 + 1];

			int xi111 = m_Indexes[1, yi + yi11];
			int xi112 = m_Indexes[1, yi + yi11 + 1];
			int xi121 = m_Indexes[1, yi + yi12];
			int xi122 = m_Indexes[1, yi + yi12 + 1];
			int xi211 = m_Indexes[1, yi + yi21];
			int xi212 = m_Indexes[1, yi + yi21 + 1];
			int xi221 = m_Indexes[1, yi + yi22];
			int xi222 = m_Indexes[1, yi + yi22 + 1];

			int vi1111 = m_Indexes[0, xi + xi111];
			int vi1112 = m_Indexes[0, xi + xi111 + 1];
			int vi1121 = m_Indexes[0, xi + xi112];
			int vi1122 = m_Indexes[0, xi + xi112 + 1];
			int vi1211 = m_Indexes[0, xi + xi121];
			int vi1212 = m_Indexes[0, xi + xi121 + 1];
			int vi1221 = m_Indexes[0, xi + xi122];
			int vi1222 = m_Indexes[0, xi + xi122 + 1];
			int vi2111 = m_Indexes[0, xi + xi211];
			int vi2112 = m_Indexes[0, xi + xi211 + 1];
			int vi2121 = m_Indexes[0, xi + xi212];
			int vi2122 = m_Indexes[0, xi + xi212 + 1];
			int vi2211 = m_Indexes[0, xi + xi221];
			int vi2212 = m_Indexes[0, xi + xi221 + 1];
			int vi2221 = m_Indexes[0, xi + xi222];
			int vi2222 = m_Indexes[0, xi + xi222 + 1];

			double v1111 = m_Values[vi1111];
			double v1112 = m_Values[vi1112];
			double v1121 = m_Values[vi1121];
			double v1122 = m_Values[vi1122];
			double v1211 = m_Values[vi1211];
			double v1212 = m_Values[vi1212];
			double v1221 = m_Values[vi1221];
			double v1222 = m_Values[vi1222];
			double v2111 = m_Values[vi2111];
			double v2112 = m_Values[vi2112];
			double v2121 = m_Values[vi2121];
			double v2122 = m_Values[vi2122];
			double v2211 = m_Values[vi2211];
			double v2212 = m_Values[vi2212];
			double v2221 = m_Values[vi2221];
			double v2222 = m_Values[vi2222];

			double v111 = v1111 + xt * (v1112 - v1111);
			double v112 = v1121 + xt * (v1122 - v1121);
			double v121 = v1211 + xt * (v1212 - v1211);
			double v122 = v1221 + xt * (v1222 - v1221);
			double v211 = v2111 + xt * (v2112 - v2111);
			double v212 = v2121 + xt * (v2122 - v2121);
			double v221 = v2211 + xt * (v2212 - v2211);
			double v222 = v2221 + xt * (v2222 - v2221);

			double v11 = v111 + yt * (v112 - v111);
			double v12 = v121 + yt * (v122 - v121);
			double v21 = v211 + yt * (v212 - v211);
			double v22 = v221 + yt * (v222 - v221);

			double v1 = v11 + zt * (v12 - v11);
			double v2 = v21 + zt * (v22 - v21);
			
			return v1 + tt * (v2 - v1);
		}		
	}
}