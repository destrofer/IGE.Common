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

using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace System {
	public static class CommonStringExtensions {
		#region Parsing
		public static bool TryParseSByte(this string val, out sbyte outVal) {
			return sbyte.TryParse(val, out outVal);
		}

		public static bool TryParseByte(this string val, out byte outVal) {
			return byte.TryParse(val, out outVal);
		}

		public static bool TryParseInt16(this string val, out short outVal) {
			return short.TryParse(val, out outVal);
		}

		public static bool TryParseUInt16(this string val, out ushort outVal) {
			return ushort.TryParse(val, out outVal);
		}

		public static bool TryParseInt32(this string val, out int outVal) {
			return int.TryParse(val, out outVal);
		}

		public static bool TryParseUInt32(this string val, out uint outVal) {
			return uint.TryParse(val, out outVal);
		}

		public static bool TryParseInt64(this string val, out long outVal) {
			return long.TryParse(val, out outVal);
		}

		public static bool TryParseUInt64(this string val, out ulong outVal) {
			return ulong.TryParse(val, out outVal);
		}

		public static bool TryParseSingle(this string val, out float outVal) {
			return float.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out outVal);
		}

		public static bool TryParseDouble(this string val, out double outVal) {
			return double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out outVal);
		}

		public static bool TryParseDecimal(this string val, out decimal outVal) {
			return decimal.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out outVal);
		}

		public static bool TryParseDateTime(this string val, out DateTime outVal) {
			return DateTime.TryParse(val, out outVal);
		}
		
		public static sbyte ToSByte(this string val, sbyte defaultValue) {
			sbyte outVal;
			return TryParseSByte(val, out outVal) ? outVal : defaultValue;
		}

		public static byte ToByte(this string val, byte defaultValue) {
			byte outVal;
			return TryParseByte(val, out outVal) ? outVal : defaultValue;
		}

		public static short ToInt16(this string val, short defaultValue) {
			short outVal;
			return TryParseInt16(val, out outVal) ? outVal : defaultValue;
		}

		public static ushort ToUInt16(this string val, ushort defaultValue) {
			ushort outVal;
			return TryParseUInt16(val, out outVal) ? outVal : defaultValue;
		}

		public static int ToInt32(this string val, int defaultValue) {
			int outVal;
			return TryParseInt32(val, out outVal) ? outVal : defaultValue;
		}

		public static uint ToUInt32(this string val, uint defaultValue) {
			uint outVal;
			return TryParseUInt32(val, out outVal) ? outVal : defaultValue;
		}

		public static long ToInt64(this string val, long defaultValue) {
			long outVal;
			return TryParseInt64(val, out outVal) ? outVal : defaultValue;
		}

		public static ulong ToUInt64(this string val, ulong defaultValue) {
			ulong outVal;
			return TryParseUInt64(val, out outVal) ? outVal : defaultValue;
		}

		public static float ToSingle(this string val, float defaultValue) {
			float outVal;
			return TryParseSingle(val, out outVal) ? outVal : defaultValue;
		}

		public static double ToDouble(this string val, double defaultValue) {
			double outVal;
			return TryParseDouble(val, out outVal) ? outVal : defaultValue;
		}
		
		public static decimal ToDecimal(this string val, decimal defaultValue) {
			decimal outVal;
			return TryParseDecimal(val, out outVal) ? outVal : defaultValue;
		}

		public static DateTime ToDateTime(this string val, DateTime defaultValue) {
			DateTime outVal;
			return TryParseDateTime(val, out outVal) ? outVal : defaultValue;
		}
		
		public static bool ToBoolean(this string val, bool defaultValue) {
			if( val.Equals("1") || val.Equals("yes", StringComparison.OrdinalIgnoreCase) || val.Equals("true", StringComparison.OrdinalIgnoreCase) )
				return true;
			if( val.Equals("0") || val.Equals("no", StringComparison.OrdinalIgnoreCase) || val.Equals("false", StringComparison.OrdinalIgnoreCase) )
				return false;
			return defaultValue;
		}
		#endregion
		
		#region RegularExpressions

		public static string RegReplace(this string val, string pattern, string replacement) {
			return RegReplace(val, pattern, replacement, false);
		}
		
		public static string RegReplace(this string val, string pattern, string replacement, bool ignoreCase) {
			return RegReplace(val, pattern, replacement, ignoreCase, false);
		}
		
		public static string RegReplace(this string val, string pattern, string replacement, bool ignoreCase, bool multiline) {
			return RegReplace(val, pattern, replacement, (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None) | (multiline ? RegexOptions.Multiline : RegexOptions.Singleline));
		}
		
		public static string RegReplace(this string val, string pattern, string replacement, RegexOptions options) {
			Regex rx = new Regex(pattern, options);
			return rx.Replace(val, replacement);
		}
		
		#endregion
		
		#region Converting
		public static byte[] ToASCIIZByteArray(this string val) {
			byte[] bytes = new byte[Encoding.ASCII.GetByteCount(val) + 1];
			Encoding.ASCII.GetBytes(val, 0, val.Length, bytes, 0);
			bytes[bytes.Length - 1] = 0;
			return bytes;
		}
		#endregion
	}
}
