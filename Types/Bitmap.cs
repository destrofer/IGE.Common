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

namespace IGE {
	public class Bitmap {
		protected int m_Width = 0;
		protected int m_Height = 0;
		protected int m_BPP = 0;
		protected byte[] m_Pixels = null;
		protected BitmapFormat m_Format = BitmapFormat.Unknown;
		
		public BitmapFormat Format { get { return m_Format; } set { m_Format = value; } }
		
		public int Width { get { return m_Width; } }
		public int Height { get { return m_Height; } }
		public int BytesPerPixel { get { return m_BPP; } }
		public byte[] Pixels { get { return m_Pixels; } }

		public Bitmap(int width, int height, int bytesPerPixel, byte[] pixels) : this(width, height, bytesPerPixel) {
			if( pixels == null || pixels.Length == 0 )
				throw new Exception("Cannot create bitmap without pixels");
			if( pixels.Length != width * height * bytesPerPixel )
				throw new Exception("Size of pixel array does not match specified dimensions");
			pixels.CopyTo(m_Pixels, 0);
		}
		
		public Bitmap(int width, int height, int bytesPerPixel) {
			Create(width, height, bytesPerPixel);
		}

		public Bitmap() {
		}
		
		public Bitmap(System.Drawing.Bitmap systemBitmap) {
			LoadSystemBitmap(systemBitmap);
		}
		
		public void LoadSystemBitmap(System.Drawing.Bitmap systemBitmap) {
			if( systemBitmap.Width != m_Width || systemBitmap.Height != m_Height )
				Create(systemBitmap.Width, systemBitmap.Height, 4);
			System.Drawing.Imaging.BitmapData bmd = systemBitmap.LockBits(
				new System.Drawing.Rectangle(0, 0, m_Width, m_Height),
				System.Drawing.Imaging.ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb
			);
			if( bmd != null ) {
				int i, j, idx, pad;
				pad = bmd.Stride - m_Width * 4;
				idx = m_Width * m_Height * 4;
				unsafe {
					byte *row = (byte *)bmd.Scan0 + bmd.Stride * m_Height;
					for( j = m_Height - 1; j >= 0; j-- ) {
						row -= pad;
						for( i = m_Width * 4 - 1; i >= 0; i-- ) {
							row--;
							idx--;
							m_Pixels[idx] = *row;
						}
					}
				}
				m_Format = BitmapFormat.BGRA;
				systemBitmap.UnlockBits(bmd);
			}
			else
				throw new OutOfMemoryException();
		}
		
		public void Create(int width, int height, int bytesPerPixel) {
			int size = width * height * bytesPerPixel;
			if( size <= 0 )
				throw new Exception("Cannot create a bitmap with one of dimensions equal to zero");
			m_Width = width;
			m_Height = height;
			m_BPP = bytesPerPixel;
			m_Pixels = new byte[size];
			m_Format = BitmapFormat.Unknown;
		}
		
		public void Fill(byte val) {
			for( int i = m_Pixels.Length - 1; i >= 0; i-- )
				m_Pixels[i] = val;
		}
		
		public void CopyPixels(Bitmap from, int targetX, int targetY, int sourceX, int sourceY, int sourceWidth, int sourceHeight) {
			if( from.Format != Format )
				throw new Exception("Cannot copy pixels from a bitmap with a different format.");
			if( targetX < 0 ) {
				sourceX -= targetX;
				sourceWidth += targetX;
				targetX = 0;
			}
			if( targetY < 0 ) {
				sourceY -= targetY;
				sourceHeight += targetY;
				targetY = 0;
			}
			if( targetX + sourceWidth > Width )
				sourceWidth = Width - targetX;
			if( targetY + sourceHeight > Height )
				sourceHeight = Height - targetY;
			
			if( sourceX < 0 ) {
				targetX -= sourceX;
				sourceWidth += sourceX;
				sourceX = 0;
			}
			if( sourceY < 0 ) {
				targetY -= sourceY;
				sourceHeight += sourceY;
				sourceY = 0;
			}
			if( sourceX + sourceWidth > from.Width )
				sourceWidth = from.Width - sourceX;
			if( sourceY + sourceHeight > from.Height )
				sourceHeight = from.Height - sourceY;
			if( sourceWidth <= 0 || sourceHeight <= 0 )
				return;
			int i, bytesPerLine = BytesPerPixel * sourceWidth;
			int dstIndex = BytesPerPixel * (targetX + targetY * Width), dstStep = BytesPerPixel * (Width - sourceWidth);
			int srcIndex = BytesPerPixel * (sourceX + sourceY * from.Width), srcStep = BytesPerPixel * (from.Width - sourceWidth);
			for( int j = sourceHeight; j > 0; j-- ) {
				for( i = bytesPerLine; i > 0; i-- )
					m_Pixels[dstIndex++] = from.m_Pixels[srcIndex++];
				
				dstIndex += dstStep;
				srcIndex += srcStep;
			}
		}
	}
	
	public enum BitmapFormat {
		Unknown,
		RGBA,
		BGRA,
		ARGB,
		ABGR,
		RGB,
		BGR,
		R,
		G,
		B,
		A,
		Grayscale,
		HeightMap,
		NormalMap
	}
}
