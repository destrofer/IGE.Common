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
using System.IO;

namespace IGE {
	public class ByteQueueStream : Stream {
		private ByteQueue m_Queue;
		public ByteQueue Queue { get { return m_Queue; } }
		
		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanTimeout { get { return false; } }
		
		public override long Position { get { return 0; } set { throw new InvalidOperationException(); } }
		public override long Length { get { return m_Queue.Length; } }
		
		public override int ReadTimeout {
			get { return base.ReadTimeout; }
			set { base.ReadTimeout = value; }
		}
		
		public override int WriteTimeout {
			get { return base.WriteTimeout; }
			set { base.WriteTimeout = value; }
		}
		
		public ByteQueueStream(ByteQueue queue) {
			if( queue == null )
				throw new ArgumentNullException();
			m_Queue = queue;
		}
		
		public override void Close() {
			base.Close();
		}
		
		public override void Flush() {
		}
		
		public override int ReadByte() {
			lock(m_Queue) {
				return (int)m_Queue.Dequeue();
			}
		}
		
		public override int Read(byte[] buffer, int offset, int count) {
			lock(m_Queue) {
				if( m_Queue.Length < count )
					count = (int)m_Queue.Length;
				m_Queue.Dequeue(buffer, offset, count);
			}
			return count;
		}
		
		public override long Seek(long offset, SeekOrigin origin) {
			throw new InvalidOperationException();
		}
		
		public override void SetLength(long val) {
			throw new InvalidOperationException();
		}
		
		public override void WriteByte(byte val) {
			lock(m_Queue) {
				m_Queue.Enqueue(val);
			}
		}
		
		public override void Write(byte[] buffer, int offset, int count) {
			lock(m_Queue) {
				m_Queue.Enqueue(buffer, offset, count);
			}
		}
	}
}
