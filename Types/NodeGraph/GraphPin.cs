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

namespace IGE {
	public abstract class GraphPin : GraphObject {
		protected List<GraphConnection> m_Connections = new List<GraphConnection>();
		protected GraphNode m_Node = null;
		protected PinType m_Type = PinType.Input;
		
		public IList<GraphConnection> Connections { get { return m_Connections.AsReadOnly(); } }
		public GraphConnection this[int index] { get { return m_Connections[index]; } }
		
		public PinType Type { get { return m_Type; } }
		
		public GraphNode Node {
			get { return m_Node; }
			set {
				if( m_Node != value ) {
					if( m_Node != null ) {
						GraphNode node = m_Node;
						m_Node = null;
						if( m_Type == PinType.Input )
							node.RemoveInput(this);
						else
							node.RemoveOutput(this);
					}
					m_Node = value;
					if( m_Node != null ) {
						if( m_Type == PinType.Input )
							m_Node.AddInput(this);
						else
							m_Node.AddOutput(this);
					}
				}
			}
		}
		
		protected GraphPin(PinType type) : base() {
			m_Type = type;
		}
		
		private GraphPin() {
		}
		
		internal void AddOutputConnection(GraphConnection conn) {
			if( !m_Connections.Contains(conn) ) {
				m_Connections.Add(conn);
				conn.Source = this;
			}
		}
		
		internal void RemoveOutputConnection(GraphConnection conn) {
			if( m_Connections.Contains(conn) ) {
				m_Connections.Remove(conn);
				conn.Source = null;
			}
		}
		
		internal void AddInputConnection(GraphConnection conn) {
			if( !m_Connections.Contains(conn) ) {
				m_Connections.Add(conn);
				conn.Target = this;
			}
		}
		
		internal void RemoveInputConnection(GraphConnection conn) {
			if( m_Connections.Contains(conn) ) {
				m_Connections.Remove(conn);
				conn.Target = null;
			}
		}
		
		public enum PinType : byte {
			Input,
			Output
		}
	}
}