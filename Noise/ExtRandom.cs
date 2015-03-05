/*
 * Original 64 bit random number generator algorithm copyright belongs to
 * Makoto Matsumoto and Takuji Nishimura.
 * http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
 * 
 * Author: Viacheslav Soroka.
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
using System.IO;
using System.Numerics;

namespace IGE {
	public class ExtRandom : ISerializable {
		private const double DoubleMultiplier = 1.0 / 9007199254740991.0;
		private const double TruncatedDoubleMultiplier = 1.0 / 9007199254740992.0; // multiplying by this constant should never give a 1.0. might need to use decimal (128 bit float) in case of bad results.
		private const decimal DecimalMultiplier = 1.0m / 18446744073709551615.0m;
		private const decimal TruncatedDecimalMultiplier = 1.0m / 18446744073709551616.0m;
		
		protected const int NN = 312;
		protected const int MM = 156;
		protected const ulong UM = 0xFFFFFFFF80000000UL;
		protected const ulong LM = 0x7FFFFFFFUL;
		protected static ulong[] MAG = new ulong[2] { 0UL, 0xB5026F5AA96619E9UL };
		
		protected ulong[] m_StateVector;
		protected int m_VectorIndex;
		
		public ExtRandom() {
			m_StateVector = new ulong[NN];
			m_VectorIndex = 1;
			Reseed(GameTimer.UTime);
		}
		
		public ExtRandom(ulong seed) {
			m_StateVector = new ulong[NN];
			m_VectorIndex = 1;
			Reseed(seed);
		}
		
		public ExtRandom(ulong[] seed) {
			m_StateVector = new ulong[NN];
			m_VectorIndex = 1;
			Reseed(seed);
		}
		
		public void Reseed(ulong seed) {
		    m_StateVector[0] = seed;
		    unchecked {
		    	for (m_VectorIndex = 1; m_VectorIndex < NN; m_VectorIndex++) 
		    		m_StateVector[m_VectorIndex] =  (6364136223846793005UL * (m_StateVector[m_VectorIndex - 1] ^ (m_StateVector[m_VectorIndex - 1] >> 62)) + (ulong)m_VectorIndex);
		    }
		}
		
		public void Reseed(ulong[] seed) {
			ulong i, j, k;
			Reseed(19650218UL);
			i = 1;
			j = 0;
			k = (ulong)((NN > seed.Length) ? NN : seed.Length);
			
			unchecked {
				for(; k > 0; k--) {
					m_StateVector[i] = (m_StateVector[i] ^ ((m_StateVector[i - 1] ^ (m_StateVector[i - 1] >> 62)) * 3935559000370003845UL))
						+ seed[j] + j;
					i++; j++;
					if( i >= NN ) { m_StateVector[0] = m_StateVector[NN - 1]; i = 1; }
					if( (int)j >= seed.Length ) j = 0;
				}
				for(k = NN - 1; k > 0; k--) {
					m_StateVector[i] = (m_StateVector[i] ^ ((m_StateVector[i - 1] ^ (m_StateVector[i - 1] >> 62)) * 2862933555777941757UL))
						- i;
					i++;
					if( i >= NN ) { m_StateVector[0] = m_StateVector[NN - 1]; i = 1; }
				}
				
				m_StateVector[0] = 1UL << 63;
			}
		}
		
		public ulong NextUInt64() {
			int i;
			ulong x;
			
			unchecked {
				if( m_VectorIndex >= NN ) {
					for(i = 0; i < NN - MM; i++) {
						x = (m_StateVector[i] & UM) | (m_StateVector[i + 1] & LM);
						m_StateVector[i] = m_StateVector[i + MM] ^ (x >> 1) ^ MAG[(int)(x & 1UL)];
					}
					for(; i < NN - 1; i++) {
						x = (m_StateVector[i] & UM) | (m_StateVector[i + 1] & LM);
						m_StateVector[i] = m_StateVector[i + MM - NN] ^ (x >> 1) ^ MAG[(int)(x & 1UL)];
					}
					x = (m_StateVector[NN - 1] & UM) | (m_StateVector[0] & LM);
					m_StateVector[NN - 1] = m_StateVector[MM - 1] ^ (x >> 1) ^ MAG[(int)(x & 1UL)];
					
					m_VectorIndex = 0;
				}
				
				x = m_StateVector[m_VectorIndex++];
				x ^= (x >> 29) & 0x5555555555555555UL;
				x ^= (x << 17) & 0x71D67FFFEDA60000UL;
				x ^= (x << 37) & 0xFFF7EEE000000000UL;
				x ^= (x >> 43);
			}
			
			return x;
		}
		
		public ulong NextUInt64(ulong max) {
			return unchecked( (ulong)NextTruncatedDecimal((decimal)max + 1.0m) );
		}
		
		public ulong NextUInt64(ulong min, ulong max) {
			return unchecked( (ulong)NextTruncatedDecimal((decimal)min, (decimal)max + 1.0m) ); // double does not have enough precision so using decimal
		}
		
		public long NextInt64() {
			return unchecked( (long)NextUInt64() );
		}
		
		public long NextInt64(long max) {
			return unchecked( (long)NextTruncatedDecimal((decimal)max + 1.0m) );
		}
		
		public long NextInt64(long min, long max) {
			return unchecked( (long)NextTruncatedDecimal((decimal)min, (decimal)max + 1.0m) ); // double does not have enough precision so using decimal
		}
		
		public uint NextUInt32() {
			return unchecked( (uint)NextUInt64() );
		}
		
		public uint NextUInt32(uint max) {
			return unchecked( (uint)NextTruncatedDouble((double)max + 1.0) );
		}
		
		public uint NextUInt32(uint min, uint max) {
			return unchecked( (uint)NextTruncatedDouble((double)min, (double)max + 1.0) );
		}
		
		public int NextInt32() {
			return unchecked( (int)NextUInt64() );
		}
		
		public int NextInt32(int max) {
			return unchecked( (int)NextTruncatedDouble((double)max + 1.0) );
		}
		
		public int NextInt32(int min, int max) {
			return unchecked( (int)NextTruncatedDouble((double)min, (double)max + 1.0) );
		}
		
		/// <summary>
		/// Generates a double value in range of [0; 1] (has a chance of returning 1.0)
		/// </summary>
		/// <returns>value in range of [0; 1]</returns>
		public double NextDouble() {
			return (NextUInt64() >> 11) * DoubleMultiplier;
		}

		/// <summary>
		/// Generates a double value in range of [0; max] (has a chance of returning max)
		/// </summary>
		/// <param name="max">Maximum value to return</param>
		/// <returns>value in range of [0; max]</returns>
		public double NextDouble(double max) {
			return (NextUInt64() >> 11) * max * DoubleMultiplier;
		}

		/// <summary>
		/// Generates a double value in range of [min; max] (has a chance of returning max)
		/// </summary>
		/// <param name="min">Maximum value to return</param>
		/// <param name="max">Maximum value to return</param>
		/// <returns>value in range of [min; max]</returns>
		public double NextDouble(double min, double max) {
			return min + (NextUInt64() >> 11) * (max - min) * DoubleMultiplier;
		}
		
		/// <summary>
		/// Generates a double value in range of [0; 1) (never returns 1.0)
		/// </summary>
		/// <returns>value in range of [0; 1)</returns>
		public double NextTruncatedDouble() {
			return (NextUInt64() >> 11) * TruncatedDoubleMultiplier;
		}

		/// <summary>
		/// Generates a double value in range of [0; max) (never returns max)
		/// </summary>
		/// <param name="max">Maximum value to return</param>
		/// <returns>value in range of [0; max)</returns>
		public double NextTruncatedDouble(double max) {
			return (NextUInt64() >> 11) * max * TruncatedDoubleMultiplier;
		}

		/// <summary>
		/// Generates a double value in range of [min; max) (never returns max)
		/// </summary>
		/// <param name="min">Maximum value to return</param>
		/// <param name="max">Maximum value to return</param>
		/// <returns>value in range of [min; max)</returns>
		public double NextTruncatedDouble(double min, double max) {
			return min + (NextUInt64() >> 11) * (max - min) * TruncatedDoubleMultiplier;
		}
		
		/// <summary>
		/// Generates a decimal (128 bit) value in range of [0; 1] (has a chance of returning 1.0)
		/// </summary>
		/// <returns>value in range of [0; 1]</returns>
		public decimal NextDecimal() {
			return NextUInt64() * DecimalMultiplier;
		}

		/// <summary>
		/// Generates a decimal (128 bit) value in range of [0; max] (has a chance of returning max)
		/// </summary>
		/// <returns>value in range of [0; 1]</returns>
		public decimal NextDecimal(decimal max) {
			return NextUInt64() * max * DecimalMultiplier;
		}

		/// <summary>
		/// Generates a decimal (128 bit) value in range of [min; max] (has a chance of returning max)
		/// </summary>
		/// <returns>value in range of [min; max]</returns>
		public decimal NextDecimal(decimal min, decimal max) {
			return min + NextUInt64() * (max - min) * DecimalMultiplier;
		}
		
		/// <summary>
		/// Generates a decimal (128 bit) value in range of [0; 1) (never returns 1.0)
		/// </summary>
		/// <returns>value in range of [0; 1)</returns>
		public decimal NextTruncatedDecimal() {
			return NextUInt64() * TruncatedDecimalMultiplier;
		}

		/// <summary>
		/// Generates a decimal (128 bit) value in range of [0; max) (never returns max)
		/// </summary>
		/// <param name="max">Maximum value to return</param>
		/// <returns>value in range of [0; max)</returns>
		public decimal NextTruncatedDecimal(decimal max) {
			return NextUInt64() * max * TruncatedDecimalMultiplier;
		}

		/// <summary>
		/// Generates a decimal (128 bit) value in range of [min; max) (never returns max)
		/// </summary>
		/// <param name="min">Maximum value to return</param>
		/// <param name="max">Maximum value to return</param>
		/// <returns>value in range of [min; max)</returns>
		public decimal NextTruncatedDecimal(decimal min, decimal max) {
			return min + NextUInt64() * (max - min) * TruncatedDecimalMultiplier;
		}
		
		public BigInteger NextBigInteger(int bits) {
			if( bits < 8 )
				return new BigInteger(0);
			return new BigInteger(NextBytesZ(bits / 8));
		}
		
		public BigInteger NextPrimeBigInteger(int bits) {
			if( bits < 8 )
				return new BigInteger(1);
			BigInteger x;
			do {
				x = NextBigInteger(bits);
			} while( !x.IsProbablePrime() );
			return x;
		}
		
		protected void FillBytes(byte[] bytes, int count) {
			ulong r = NextUInt64();
			int shift = 0;
			unchecked {
				while( --count >= 0 ) {
					if( shift >= 64 ) {
						r = NextUInt64();
						shift = 0;
					}
					bytes[count] = (byte)((r >> shift) & 0xFF);
					shift += 8;
				}
			}
		}
		
		public byte[] NextBytes(int count) {
			byte[] bytes = new byte[count];
			FillBytes(bytes, count);
			return bytes;
		}
		
		public byte[] NextBytesZ(int count) {
			byte[] bytes = new byte[count + 1];
			FillBytes(bytes, count);
			bytes[count] = 0;
			return bytes;
		}
		
		public void Serialize(BinaryWriter w) {
			int i;
			w.Write(NN);
			w.Write(MM);
			w.Write(UM);
			w.Write(LM);
			for( i = 0; i < MAG.Length; i++ )
				w.Write(MAG[i]);
			w.Write(m_VectorIndex);
			for( i = 0; i < m_StateVector.Length; i++ )
				w.Write(m_StateVector[i]);
		}
		
		public void Deserialize(BinaryReader r) {
			int i;
			
			if( r.ReadInt32() != NN || r.ReadInt32() != MM || r.ReadUInt64() != UM || r.ReadUInt64() != LM )
				throw new UserFriendlyException("Invalid or different version of ExtRandom serialization encountered in saved data", "Saved data is corupt or incompatible");
			for( i = 0; i < MAG.Length; i++ )
				if( MAG[i] != r.ReadUInt64() )
					throw new UserFriendlyException("Invalid or different version of ExtRandom serialization encountered in saved data", "Saved data is corupt or incompatible");
			
			m_VectorIndex = r.ReadInt32();
			for( i = 0; i < m_StateVector.Length; i++ )
				m_StateVector[i] = r.ReadUInt64();
		}
	}
}
