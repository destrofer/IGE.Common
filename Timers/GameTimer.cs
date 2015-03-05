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
using System.Diagnostics;

namespace IGE {
	public class GameTimer : ISerializable {
		protected bool m_Paused;
		public virtual bool Paused { get { return m_Paused; } set { m_Paused = value; } }
		
		protected long m_TicksCounted;

		protected double m_Frequency;
		protected long m_PreviousTicks;
		protected long m_CurrentTicks;
		protected long m_LastFrameTicks;
		
		protected double m_LastFrameTime;
		public virtual double LastFrameTime { get { return m_LastFrameTime; } }
		protected double m_Time;
		public virtual double Time { get { return m_Time; } }
		
		public static ulong UTime { get { return (ulong)Stopwatch.GetTimestamp(); } }
		public static bool IsHighResolution { get { return Stopwatch.IsHighResolution; } }
		
		public GameTimer() {
			m_Frequency = (double)Stopwatch.Frequency;
			Reset();
		}
		
		public virtual void OnFrame() {
			m_CurrentTicks = Stopwatch.GetTimestamp();
			if( !m_Paused ) {
				m_LastFrameTicks = m_CurrentTicks - m_PreviousTicks;
				m_TicksCounted += m_LastFrameTicks;
				m_Time = (double)m_TicksCounted / m_Frequency;
				m_LastFrameTime = (double)m_LastFrameTicks / m_Frequency;
			}
			m_PreviousTicks = m_CurrentTicks;
		}
		
		public virtual void Reset() {
			m_TicksCounted = 0;
			m_LastFrameTicks = 1;
			m_Time = 0;
			m_LastFrameTime = (1.0 / m_Frequency);
			m_PreviousTicks = m_CurrentTicks = Stopwatch.GetTimestamp();
		}
		
		/// <summary>
		/// Ignores time that has passed since last OnFrame() method call. Can be used when timer gets initialized during loading and you need to skip that load time.
		/// </summary>
		public virtual void SkipFrames() {
			m_PreviousTicks = m_CurrentTicks = Stopwatch.GetTimestamp();
		}
		
		public virtual void Serialize(BinaryWriter w) {
			w.Write(m_TicksCounted);
			w.Write(m_Paused);
		}
		
		public virtual void Deserialize(BinaryReader r) {
			Reset();
			m_TicksCounted = r.ReadInt64();
			m_Paused = r.ReadBoolean();
			m_Time = (double)m_TicksCounted / m_Frequency;
		}
	}
}
