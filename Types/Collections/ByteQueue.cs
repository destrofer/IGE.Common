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

namespace IGE {
	public class ByteQueue {
		public const int DefaultChunkSize = 256;
		
		private int ChunkSize = DefaultChunkSize;
		
		private ByteChunk ReadChunk = null;
		private int ReadPointer = 0;
		
		private ByteChunk WriteChunk = null;
		private int WritePointer = 0;
		
		// private List<ByteChunk> Chunks = new List<ByteChunk>();
		
		private int m_Length = 0;
		
		public int Length { get { return m_Length; } }
		
		public ByteQueue() {
		}
		
		public ByteQueue(int chunkSize) : this() {
			if( chunkSize < 1 )
				throw new Exception("Chunk size cannot be smaller than 1");
			ChunkSize = chunkSize;
		}

		public void Clear() {
			ReadChunk = null;
			ReadPointer = 0;
			WriteChunk = null;
			WritePointer = 0;
			m_Length = 0;
		}
		
		public static object ConvertFromBytes<T>(byte[] bytes) where T : struct {
			if( !BitConverter.IsLittleEndian ) {
				byte[] tmp = new byte[bytes.Length];
				bytes.CopyTo(tmp, 0);
				Array.Reverse(tmp);
				bytes = tmp;
			}
			Type t = typeof(T);
			if( t.Equals(typeof(bool)) ) return BitConverter.ToBoolean(bytes, 0);
			else if( t.Equals(typeof(char)) ) return BitConverter.ToChar(bytes, 0);
			else if( t.Equals(typeof(short)) ) return BitConverter.ToInt16(bytes, 0);
			else if( t.Equals(typeof(ushort)) ) return BitConverter.ToUInt16(bytes, 0);
			else if( t.Equals(typeof(int)) ) return BitConverter.ToInt32(bytes, 0);
			else if( t.Equals(typeof(uint)) ) return BitConverter.ToUInt32(bytes, 0);
			else if( t.Equals(typeof(long)) ) return BitConverter.ToInt64(bytes, 0);
			else if( t.Equals(typeof(ulong)) ) return BitConverter.ToUInt64(bytes, 0);
			else if( t.Equals(typeof(float)) ) return BitConverter.ToSingle(bytes, 0);
			else if( t.Equals(typeof(double)) ) return BitConverter.ToDouble(bytes, 0);
			else if( t.Equals(typeof(byte)) ) return bytes[0];
			else if( t.Equals(typeof(sbyte)) ) return unchecked((sbyte)bytes[0]);
			
			return default(T);
		}

		public static byte[] ConvertToBytes(object val) {
			byte[] bytes = null;
			
			if( val is bool ) bytes = BitConverter.GetBytes((bool)val);
			else if( val is char ) bytes = BitConverter.GetBytes((char)val);
			else if( val is short ) bytes = BitConverter.GetBytes((short)val);
			else if( val is ushort ) bytes = BitConverter.GetBytes((ushort)val);
			else if( val is int ) bytes = BitConverter.GetBytes((int)val);
			else if( val is uint ) bytes = BitConverter.GetBytes((uint)val);
			else if( val is long ) bytes = BitConverter.GetBytes((long)val);
			else if( val is ulong ) bytes = BitConverter.GetBytes((ulong)val);
			else if( val is float ) bytes = BitConverter.GetBytes((float)val);
			else if( val is double ) bytes = BitConverter.GetBytes((double)val);
			else if( val is byte ) bytes = new byte[] { (byte)val };
			else if( val is sbyte ) bytes = new byte[] { unchecked((byte)val) };
			
			if( bytes != null && !BitConverter.IsLittleEndian )
				Array.Reverse(bytes);
			
			return bytes;
		}
		
		public void Enqueue(byte[] bytes, int offset, int length) {
			if( WriteChunk == null )
				AddChunk();
			if( WritePointer >= ChunkSize ) {
				AddChunk();
				WritePointer = 0;
				WriteChunk = WriteChunk.Next;
			}
			int size;
			// m_Length += length; // increasing length with each chunk copying is slower, but in case of OutOfMemoryException the queue length will not be corupt
			while( length > 0 ) {
				size = ChunkSize - WritePointer;
				if( length <= size ) {
					size = length;
					Array.Copy(bytes, offset, WriteChunk.Bytes, WritePointer, size);
					m_Length += size;
					length = 0;
					WritePointer += size;
				}
				else {
					Array.Copy(bytes, offset, WriteChunk.Bytes, WritePointer, size);
					m_Length += size;
					WritePointer = ChunkSize; // just in case of an OutOfMemoryException
					AddChunk();
					WriteChunk = WriteChunk.Next;
					WritePointer = 0;
					length -= size;
					offset += size;
				}
			}
		}
		
		public void Enqueue(byte[] bytes) {
			Enqueue(bytes, 0, bytes.Length);
		}
		
		public void Enqueue(byte b) {
			if( WriteChunk == null )
				AddChunk();
			if( WritePointer >= ChunkSize ) {
				AddChunk();
				WritePointer = 0;
				WriteChunk = WriteChunk.Next;
			}
			m_Length++;
			WriteChunk.Bytes[WritePointer++] = b;
		}

		public void Enqueue(object val) {
			Enqueue(ConvertToBytes(val));
		}
		
		public bool DequeueBoolean() {
			return (bool)ConvertFromBytes<bool>(Dequeue(sizeof(bool)));
		}
		
		public char DequeueChar() {
			return (char)ConvertFromBytes<char>(Dequeue(sizeof(char)));
		}
		
		public short DequeueInt16() {
			return (short)ConvertFromBytes<short>(Dequeue(sizeof(short)));
		}
		
		public ushort DequeueUInt16() {
			return (ushort)ConvertFromBytes<ushort>(Dequeue(sizeof(ushort)));
		}
		
		public int DequeueInt32() {
			return (int)ConvertFromBytes<int>(Dequeue(sizeof(int)));
		}
		
		public uint DequeueUInt32() {
			return (uint)ConvertFromBytes<uint>(Dequeue(sizeof(uint)));
		}
		
		public long DequeueInt64() {
			return (long)ConvertFromBytes<long>(Dequeue(sizeof(long)));
		}
		
		public ulong DequeueUInt64() {
			return (ulong)ConvertFromBytes<ulong>(Dequeue(sizeof(ulong)));
		}
		
		public float DequeueSingle() {
			return (float)ConvertFromBytes<float>(Dequeue(sizeof(float)));
		}
		
		public double DequeueDouble() {
			return (double)ConvertFromBytes<double>(Dequeue(sizeof(double)));
		}
		
		public byte Dequeue() {
			if( ReadPointer >= ChunkSize ) {
				ReadChunk = ReadChunk.Next;
				ReadPointer = 0;
				if( ReadChunk == null ) {
					WriteChunk = null;
					WritePointer = 0;
				}
			}
			if( m_Length == 0 )
				throw new Exception("ByteQueue is empty");
			m_Length--;
			return ReadChunk.Bytes[ReadPointer++];
		}
		
		public void Dequeue(byte[] bytes, int offset, int length) {
			if( ReadPointer >= ChunkSize ) {
				ReadChunk = ReadChunk.Next;
				ReadPointer = 0;
				if( ReadChunk == null ) {
					WriteChunk = null;
					WritePointer = 0;
				}
			}
			if( m_Length < length )
				throw new Exception("ByteQueue is smaller than requested length");
			int size;
			while( length > 0 ) {
				size = ChunkSize - ReadPointer;
				if( length <= size ) {
					size = length;
					Array.Copy(ReadChunk.Bytes, ReadPointer, bytes, offset, size);
					length = 0;
					ReadPointer += size;
				}
				else {
					Array.Copy(ReadChunk.Bytes, ReadPointer, bytes, offset, size);
					ReadChunk = ReadChunk.Next;
					ReadPointer = 0;
					length -= size;
					offset += size;
				}
				m_Length -= size;
			}
		}
		
		public byte[] Dequeue(int length) {
			if( m_Length < length )
				throw new Exception("ByteQueue is smaller than requested length");
			byte[] array = new byte[length];
			Dequeue(array, 0, length);
			return array;
		}
		
		public byte[] ToArray() {
			return Dequeue(m_Length);
		}
		
		public int IndexOf(byte searchByte) {
			ByteChunk currentChunk = ReadChunk;
			int currentPointer = ReadPointer, pos = 0;
			for(int i = m_Length; i > 0; i--, pos++ ) {
				if( currentPointer >= ChunkSize ) {
					currentChunk = currentChunk.Next;
					currentPointer = 0;
				}
				if( currentChunk.Bytes[currentPointer++] == searchByte )
					return pos;
			}
			return -1;
		}
		
		private void AddChunk() {
			ByteChunk newChunk = new ByteChunk(ChunkSize);
			if( WriteChunk != null )
				WriteChunk.Next = newChunk;
			else
				ReadChunk = WriteChunk = newChunk;
		}
		
		internal class ByteChunk {
			internal byte[] Bytes;
			internal ByteChunk Next = null;
			
			public ByteChunk(int size) {
				Bytes = new byte[size];
			}
		}
	}	
}