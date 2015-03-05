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
using System.Text.RegularExpressions;

namespace IGE {
	[StructLayout(LayoutKind.Sequential)]
    public struct Color4 : IEquatable<Color4> {
		public const float ONE_255 = 1f / 255f;
		
		public static readonly Color4 Zero = new Color4(0f, 0f, 0f, 0f);
		public static readonly Color4 Black = new Color4(0f, 0f, 0f, 1f);
		public static readonly Color4 White = new Color4(1f, 1f, 1f, 1f);

        public float R;
        public float G;
        public float B;
        public float A;
		
        public Color4(float r, float g, float b, float a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        
        public static void Lerp(ref Color4 color1, ref Color4 color2, float color2Amount, ref Color4 outColor) {
        	outColor.R = color2.R * color2Amount + (1f - color2Amount) * color1.R;
        	outColor.G = color2.G * color2Amount + (1f - color2Amount) * color1.G;
        	outColor.B = color2.B * color2Amount + (1f - color2Amount) * color1.B;
        	outColor.A = color2.A * color2Amount + (1f - color2Amount) * color1.A;
        }
        
        public static Color4 FromBytes(byte r, byte g, byte b, byte a) {
        	return new Color4((float)r * ONE_255, (float)g * ONE_255, (float)b * ONE_255, (float)a * ONE_255);
        }
        
        public static bool operator == (Color4 left, Color4 right) {
            return left.R == right.R && left.G == right.G && left.B == right.B && left.A == right.A;
        }

        public static bool operator !=(Color4 left, Color4 right) {
            return left.R != right.R || left.G != right.G || left.B != right.B || left.A != right.A;
        }
		
        public bool Equals(Color4 other) {
            return  R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override bool Equals(object other) {
        	return other is Color4 && R == ((Color4)other).R && G == ((Color4)other).G && B == ((Color4)other).B && A == ((Color4)other).A;
        }

        public override int GetHashCode() {
        	return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
        }
		
        public override string ToString() {
            return String.Format("({0:0}, {1:0}, {2:0}, {3:0})", R * 255f, G * 255f, B * 255f, A * 255f);
        }
        
        public string ToString(string format) {
        	if( format != null ) {
        		if( format.Equals("X") )
        			return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (byte)(R * 255f), (byte)(G * 255f), (byte)(B * 255f), (byte)(A * 255f));
        	}
        	return ToString();
		}

        
        public static bool TryParse(string strVal, out Color4 color) {
        	Match match = Regex.Match(strVal, "^#?([0-9A-F]{6}|[0-9A-F]{8})$", RegexOptions.IgnoreCase);
       		color = new Color4();
        	if( match != null && match.Success ) {
        		uint hexColor = Convert.ToUInt32(match.Groups[1].Value, 16);
        		if( match.Groups[1].Value.Length == 8 ) {
        			color.R = (float)((hexColor >> 24) & 0xFF) * ONE_255;
					color.G = (float)((hexColor >> 16) & 0xFF) * ONE_255;
					color.B = (float)((hexColor >> 8) & 0xFF) * ONE_255;
					color.A = (float)((hexColor) & 0xFF) * ONE_255;
        		}
        		else {
					color.R = (float)((hexColor >> 16) & 0xFF) * ONE_255;
					color.G = (float)((hexColor >> 8) & 0xFF) * ONE_255;
					color.B = (float)((hexColor) & 0xFF) * ONE_255;
					color.A = 1f;
        		}
        		return true;
        	}
        	else {
        		string[] spl = strVal.Split(',');
        		if( spl.Length >= 4 ) {
        			if( !float.TryParse(spl[3], out color.A) )
        				return false;
        		}
        		else
        			color.A = 255f;
        		if( spl.Length >= 3 ) {
        			if( !float.TryParse(spl[2], out color.B) )
        				return false;
        			if( !float.TryParse(spl[1], out color.G) )
        				return false;
        			if( !float.TryParse(spl[0], out color.R) )
        				return false;
        			color.R *= ONE_255;
					color.G *= ONE_255;
					color.B *= ONE_255;
					color.A *= ONE_255;
        			return true;
        		}
        	}
       		return false;
        }
    }
}
