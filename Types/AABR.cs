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
	/// <summary>
	/// </summary>
	public struct AABR : IEquatable<AABR> {
		public float X1;
		public float Y1;
		public float X2;
		public float Y2;
		
		public AABR(float x1, float y1, float x2, float y2) {
			X1 = x1;
			Y1 = y1;
			X2 = x2;
			Y2 = y2;
		}
		
		public void Add(ref Vector2 vec) {
			X1 += vec.X;
			Y1 += vec.Y;
			X2 += vec.X;
			Y2 += vec.Y;
		}

		public void Add(Vector2 vec) {
			X1 += vec.X;
			Y1 += vec.Y;
			X2 += vec.X;
			Y2 += vec.Y;
		}
		
		#region Equals and GetHashCode implementation
		// The code in this region is useful if you want to use this structure in collections.
		// If you don't need it, you can just remove the region and the ": IEquatable<AABR>" declaration.
		
		public override bool Equals(object obj) {
			if( obj is AABR )
				return Equals((AABR)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(AABR other) {
			// add comparisions for all members here
			return this.X1 == other.X1 && this.Y1 == other.Y1 && this.X2 == other.X2 && this.Y2 == other.Y2;
		}
		
		public override int GetHashCode() {
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return X1.GetHashCode() ^ Y1.GetHashCode() ^ X2.GetHashCode() ^ Y2.GetHashCode();
		}
		
		public static bool operator ==(AABR left, AABR right) {
			return left.Equals(right);
		}
		
		public static bool operator !=(AABR left, AABR right) {
			return !left.Equals(right);
		}
		#endregion
	}
}
