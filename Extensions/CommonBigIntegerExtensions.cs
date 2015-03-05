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
 
using IGE;

namespace System.Numerics {
	public static class CommonBigIntegerExtensions {
		public static ushort[] lowPrimes;
		private static byte[] lowPrimeIndex = new byte[65536];
		private static ushort maxLowPrime;
		private static int lowPrimeCount = 0;
		private static ExtRandom seqRnd;
		
		static CommonBigIntegerExtensions() {
			seqRnd = new ExtRandom(GameTimer.UTime);
			
			// build an array and index of prime numbers that are not greater than 65535
			uint[] tmp = new uint[65536];
			uint i;
			for( i = 0; i < 65536; i++ ) {
				tmp[i] = 0;
				lowPrimeIndex[i] = 0;
			}
			tmp[0] = 2;
			while( tmp[lowPrimeCount] < 65536 ) {
				for( i = tmp[lowPrimeCount] * tmp[lowPrimeCount]; i < 65536; i += tmp[lowPrimeCount] )
					tmp[i] = 1;
				lowPrimeCount++;
				tmp[lowPrimeCount] = tmp[lowPrimeCount - 1] + 1;
				while( tmp[lowPrimeCount] < 65536 && tmp[tmp[lowPrimeCount]] != 0 )
					tmp[lowPrimeCount]++;
			}
			lowPrimes = new ushort[lowPrimeCount];
			maxLowPrime = (ushort)tmp[lowPrimeCount - 1];
			for( i = 0; i < lowPrimeCount; i++ ) {
				lowPrimes[i] = (ushort)tmp[i];
				lowPrimeIndex[tmp[i]] = 1;
			}
		}
		
		/// <summary>
		/// http://en.wikipedia.org/wiki/Modular_multiplicative_inverse
		/// </summary>
		/// <param name="x">number</param>
		/// <param name="m">Modulo</param>
		/// <returns>x^(-1) mod m</returns>
		public static BigInteger MultiplicativeInverse(this BigInteger x, BigInteger m) {
			BigInteger two = new BigInteger(2);
			BigInteger A = BigInteger.One;
			BigInteger B = BigInteger.Zero;
			BigInteger C = BigInteger.Zero;
			BigInteger D = BigInteger.One;
			BigInteger u = x, v = m;
			BigInteger a;
			
			for(;;) {
				while( u.IsEven ) {
					u = BigInteger.Divide(u, two);
					if( !A.IsEven || !B.IsEven ) {
						A = BigInteger.Add(A, m);
						B = BigInteger.Subtract(B, x);
					}
					A = BigInteger.Divide(A, two);
					B = BigInteger.Divide(B, two);
				}

				while( v.IsEven ) {
					v = BigInteger.Divide(v, two);
					if( !C.IsEven || !D.IsEven ) {
						C = BigInteger.Add(C, m);
						D = BigInteger.Subtract(D, x);
					}
					C = BigInteger.Divide(C, two);
					D = BigInteger.Divide(D, two);
				}
				
				if( BigInteger.Compare(v, u) <= 0 ) {
					u = BigInteger.Subtract(u, v);
					A = BigInteger.Subtract(A, C);
					B = BigInteger.Subtract(B, D);
				}
				else {
					v = BigInteger.Subtract(v, u);
					C = BigInteger.Subtract(C, A);
					D = BigInteger.Subtract(D, B);
				}
				
				if( u.IsZero ) {
					while( C.Sign < 0 )
						C = BigInteger.Add(C, m);
					a = v.IsOne ? C : BigInteger.Zero;
					break;
				}
			}
			return a;
		}
		
		public static bool IsProbablePrime(this BigInteger self) {
			return IsProbablePrime(self, 5);
		}
		
		public static bool IsProbablePrime(this BigInteger self, int testDepth) {
			int i, j;
			long m;
			if( self.Sign <= 0 )
				return false;
			if( self.CompareTo(maxLowPrime) <= 0 ) {
				ushort smallSelf = (ushort)self;
				if( smallSelf <= 1 )
					return false;
				if( smallSelf == 2 )
					return true;
				return lowPrimeIndex[smallSelf] == 1;
			}
			
			for( i = 1; i < lowPrimeCount; ) {
				m = (long)lowPrimes[i];
				j = i + 1;
				while( j < lowPrimeCount && m < 65536 )
					m *= (long)lowPrimes[j++];
				m = (long)(self % (BigInteger)m);
				while( i < j )
					if( m % lowPrimes[i++] == 0 )
						return false;
			}
			
			return DoMillerRabinTest(ref self, testDepth);
		}
		
		private static bool DoMillerRabinTest(ref BigInteger x, int testDepth) {
			BigInteger x1 = BigInteger.Subtract(x, BigInteger.One);
			BigInteger r = x1;
			
			int bit = 0, i, j;
			bool nequals;
			
			while ((r & BigInteger.One).IsZero) {
				bit++;
				r >>= 1;
			}
			if( bit == 0 )
				return false; // it was an even number

			BigInteger a;
			BigInteger two = new BigInteger(2);

			for( i = testDepth; i > 0; i-- ) {
				a = new BigInteger(lowPrimes[seqRnd.NextUInt32() % lowPrimeCount]);
				a = BigInteger.ModPow(a, r, x);
				
				if( !a.IsOne ) {
					nequals = (a != x1);
					if( nequals ) {
						j = 1;
						for( j = bit; nequals && j > 1; j-- ) {
							a = BigInteger.ModPow(a, two, x);
							if( a.IsOne )
								return false;
							nequals = (a != x1);
						}
						if( nequals )
							return false;
					}
				}
			}
			
			return true;
		}
	}
}
