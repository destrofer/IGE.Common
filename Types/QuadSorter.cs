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
using System.Collections.Generic;

using IGE;

namespace IGE {
	/// <summary>
	/// Used to pack many small quads containers.
	/// There may be one or many containers where all quads will be packed and will not intersect.
	/// If max width or max height is 0 then only one container will be created with all quads.
	/// Otherwise new containers will be created if some quads cannot fit into already existing.
	/// </summary>
	public class QuadSorter {
		/// <summary>
		/// This constant balances speed/precision of sorting.
		/// Lower value gives more precision, but less speed. However going with
		/// too great value will start generating too many containers which will
		/// slow down.
		/// 61f is for optimal speed (test sorting finished averagely in 2.6 seconds with 14 containers)
		/// 0f is maximum precision (test sorting finished averagely in 40 seconds with 13 containers)
		/// Tested with 2000 quads of random size from 4x4 to 32x32
		/// </summary>
		public const float ExpectedAreaMultiplier = 61f;
		
		public List<QuadSorter.QuadContainer> Containers = new List<QuadSorter.QuadContainer>();
		public List<QuadSorter.Quad> Quads;
		public float MaxWidth;
		public float MaxHeight;
		
		public QuadSorter(List<QuadSorter.Quad> quads, float maxWidth, float maxHeight) {
			Quads = quads;
			MaxWidth = maxWidth;
			MaxHeight = maxHeight;
			Sort();
		}
		
		protected virtual void Sort() {
			// Sort quads by size
			Quads.Sort();
			
			// Now fit them
			bool fit;
			QuadSorter.QuadContainer newContainer;
			foreach( QuadSorter.Quad quad in Quads ) {
				fit = false;
				foreach( QuadSorter.QuadContainer container in Containers ) {
					if( TryFitQuad(container, quad) ) {
						fit = true;
						break;
					}
				}
				if( !fit ) {
					newContainer = new QuadSorter.QuadContainer();
					if( !TryFitQuad(newContainer, quad) )
						throw new Exception("Something unexpected: could not fit an item in an empty container");
					Containers.Add(newContainer);
				}
			}
		}
		
		protected virtual bool TryFitQuad(QuadSorter.QuadContainer container, QuadSorter.Quad quad) {
			float nextX = 0f, nextY = 0f, x11, y11, x12, y12, x21, y21, x22, y22;
			quad.X = 0f;
			quad.Y = 0f;
			bool collided;
			// try to predict if container is full
			if( container.Width + quad.Width > MaxWidth && container.Height + quad.Height > MaxHeight && container.Area - container.TakenArea < quad.Width * quad.Height * ExpectedAreaMultiplier )
				return false;
			
			do {
				x11 = quad.X;
				y11 = quad.Y;
				x12 = x11 + quad.Width;
				y12 = y11 + quad.Height;
				nextX = x11;
				collided = false;
				foreach(QuadSorter.Quad subQuad in container.Quads) {
					x21 = subQuad.X;
					y21 = subQuad.Y;
					x22 = x21 + subQuad.Width;
					y22 = y21 + subQuad.Height;
					
					if( x11 < x22 && x12 > x21 && y11 < y22 && y12 > y21 ) {
						if( nextX < x22 )
							nextX = x22;
						if( nextY == 0f || y22 < nextY )
							nextY = y22;
						collided = true;
					}
				}

				quad.X = nextX;
				if( quad.X + quad.Width > container.Width && !TryStretchContainer(container, quad.X + quad.Width - container.Width, 0f) ) {
					if( quad.X == 0f )
						throw new Exception("Quad width is greater than maximum allowed container width.");
					collided = true;
					quad.X = 0f;
					quad.Y = nextY;
					nextY = 0f;
				}

				if( quad.X == 0f && quad.Y + quad.Height > container.Height && !TryStretchContainer(container, 0f, quad.Y + quad.Height - container.Height) ) {
					if( quad.Y == 0f )
						throw new Exception("Quad height is greater than maximum allowed container height.");
					break;
				}
				
				if( !collided ) {
					// We found an empty space which doesn't collide with others
					container.Quads.Add(quad);
					container.TakenArea += quad.Width * quad.Height;
					return true;
				}
			} while(true);
			
			return false;
		}
		
		protected virtual bool TryStretchContainer(QuadSorter.QuadContainer container, float incrementWidth, float incrementHeight) {
			if( MaxWidth > 0f && container.Width + incrementWidth > MaxWidth )
				return false;
			if( MaxHeight > 0f && container.Height + incrementHeight > MaxHeight )
				return false;
			container.Width += incrementWidth;
			container.Height += incrementHeight;
			container.Area = container.Width * container.Height;  
			return true;
		}
		
		public class QuadContainer {
			public float Width = 0f;
			public float Height = 0f;
			public List<QuadSorter.Quad> Quads = new List<QuadSorter.Quad>();
			public float TakenArea = 0f;
			public float Area = 0f;
		}
		
		public class Quad : IComparable<Quad> {
			public float X;
			public float Y;
			public float Width;
			public float Height;
			public object Data = null;
			
			/// <summary>
			/// Sorts objects by their area from biggest to smallest
			/// </summary>
			/// <param name="other"></param>
			/// <returns></returns>
			public int CompareTo(Quad other) {
				if( other.Width == Width && other.Height == Height )
					return 0;
				float area1 = Width * Height;
				float area2 = other.Width * other.Height;
				return (area1 > area2) ? -1 : 1;
			}
		}
	}
}
