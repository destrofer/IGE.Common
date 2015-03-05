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
using System.Collections;
using System.Collections.Generic;
using System.Text;
// using System.Security.Cryptography;

// using IGE.Graphics.OpenGL;

namespace IGE {
	public static class Utils {
		public const double PiOver180 = Math.PI / 180.0;
		
		public static Random m_Rnd = null;
		public static Random Rnd { get { if( m_Rnd == null ) m_Rnd = new Random(); return m_Rnd; } set { if( value != null ) m_Rnd = value; } }
		
		public static ExtRandom m_ExtRnd = null;
		public static ExtRandom ExtRnd { get { if( m_ExtRnd == null ) m_ExtRnd = new ExtRandom(); return m_ExtRnd; } set { if( value != null ) m_ExtRnd = value; } }

		// Since Perlin prepares for a very long period of time we don't create an object in utils until first access
		private static Perlin m_Perlin;
		public static Perlin Perlin { get { if( m_Perlin == null ) m_Perlin = new Perlin(); return m_Perlin; } set { if( value != null ) m_Perlin = value; } }

		public static int RandomIndex(params double[] probabilities) {
			double prob_total = 0.0, rnd;
			int i, ln = probabilities.Length;
			for( i = 0; i < ln; i++ )
				prob_total += probabilities[i];
			rnd = Rnd.NextDouble() * prob_total;
			prob_total = 0.0;
			for( i = 0; i < ln; i++ )
			{
				prob_total += probabilities[i];
				if( rnd < prob_total )
					return i;
			}
			return 0;
		}

		public delegate double RandomChanceCallback<T>(T obj);
		
		public static T RandomFrom<T>(IEnumerable<T> values, RandomChanceCallback<T> callback ) {
			double sum = 0.0, chance;
			List<double> chances = new List<double>();
			int idx = 0;
			foreach( T obj in values ) {
				chance = callback.Invoke(obj);
				sum += chance;
				chances.Add(chance);
			}
			
			if( chances.Count == 0 )
				return default(T);
			
			chance = ExtRnd.NextDouble() * sum;
			foreach( T obj in values ) {
				if( (chance -= chances[idx++]) <= 0.0 )
					return obj;
			}
			return default(T); // this will never be reached, but compiler requires to return something
		}
		
		public static int ToWrappedIndex(double v, uint limit) {
			return (v < 0.0) ? ((int)limit + ((int)Math.Floor(v) % (int)limit)) : ((int)Math.Floor(v) % (int)limit);
		}

		public static int ToWrappedIndex(double v, uint limit, out double interp) {
			double vd = Math.Floor(v);
			interp = v - vd;
			return (v < 0.0) ? ((int)limit + ((int)vd % (int)limit)) : ((int)vd % (int)limit);
		}
		
		public static double Lerp(double a, double b, double t) {
			return a + t * (b - a);
		}

		public static float Lerp(float a, float b, float t) {
			return a + t * (b - a);
		}

		public static void Clamp<T> (ref T val, T min, T max)
			where T : IComparable<T>
		{
			if( val.CompareTo(min) < 0 ) val = min;
			if( val.CompareTo(max) > 0 ) val = max;
		} 

		public static void Clamp(ref int val, int min, int max) {
			if( val < min ) val = min;
			if( val > max ) val = max;
		} 

		public static void Clamp(ref int val1, ref int val2, int min, int max) {
			if( val1 < min ) val1 = min;
			if( val1 > max ) val1 = max;
			if( val2 < min ) val2 = min;
			if( val2 > max ) val2 = max;
		}

		// triangle orientation determination
		public static int Orientation( float px, float py, float qx, float qy, float rx, float ry ) {
			float ax, ay, bx, by;
			float d;
			
			ax = qx - px;
			ay = qy - py;
			bx = rx - px;
			by = ry - py;
			d = (float)ax * (float)by - (float)ay * (float)bx;
			return (d < 0.0f) ? (-1) : ((d > 0.0f) ? 1 : 0);
		}
		
		public static byte[] Str2Bytes(string str, int count) {
			return Str2Bytes(str, count, Encoding.ASCII);
		}
		public static byte[] Str2Bytes(string str, int count, Encoding encoding) {
			byte[] bytes = new byte[count];
			encoding.GetBytes(str, 0, Math.Min(count - 1, str.Length), bytes, 0);
			bytes[count - 1] = 0; // just in case
			return bytes;
		}
		
		public static string Bytes2Str(byte[] bytes) {
			return Bytes2Str(bytes, Encoding.ASCII);
		}
		public static string Bytes2Str(byte[] bytes, Encoding encoding) {
			string str = encoding.GetString(bytes);
			int pos = str.IndexOf('\0');
			if( pos == 0 ) return "";
			return str.Substring(0, pos);
		}
		
		public static string MD5(string str) {
			return MD5(str, Encoding.UTF8);
		}
		public static string MD5(string str, Encoding encoding) {
			return MD5(encoding.GetBytes(str));
		}
		public static string MD5(byte[] data) {
			return MD5ToString(System.Security.Cryptography.MD5.Create().ComputeHash(data));
		}
		public static string MD5ToString(byte[] md5) {
			StringBuilder builder = new StringBuilder();
			foreach(byte b in md5)
				builder.Append(b.ToString("x2"));
			return builder.ToString();
		}
		
		public static string BytesToHex(byte[] data) {
			return BytesToHex(data, 16);
		}
		
		public static string BytesToHex(byte[] data, int chunkSize) {
			StringBuilder builder = new StringBuilder();
			int idx = 0;
			foreach(byte b in data) {
				builder.Append(b.ToString("X2"));
				if( ++idx == chunkSize ) {
					idx = 0;
					builder.Append('\n');
				}
				else
					builder.Append(' ');
			}
			return builder.ToString();
		}
		
		static Utils() {
		}
		
		
		public static uint ROTL(uint val, int bits) {
			return unchecked((val << bits) | (val >> (32 - bits)));
		}
		
		public static uint ROTR(uint val, int bits) {
			return unchecked((val >> bits) | (val << (32 - bits)));
		}
		
		public static string IndentedFormat(int indent, string format, params object[] p) {
			return String.Concat(new String(' ', indent), String.Format(format, p));
		}
		
		public static void IndentedOutput(int indent, string format, params object[] p) {
			Console.WriteLine("{0}{1}", new String(' ', indent), String.Format(format, p));
		}
	}
}
