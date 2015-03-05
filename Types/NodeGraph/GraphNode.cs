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
	public class GraphNode : GraphObject {
		protected GraphNode m_Parent = null;
		protected List<GraphPin> m_Inputs = new List<GraphPin>();
		protected List<GraphPin> m_Outputs = new List<GraphPin>();
		protected List<GraphNode> m_Children = new List<GraphNode>(); // might want to use hashset, but GetChildrenEnumerator

		public IList<GraphPin> Inputs { get { return m_Inputs.AsReadOnly(); } }
		public IList<GraphPin> Outputs { get { return m_Outputs.AsReadOnly(); } }
		public IList<GraphNode> Children { get { return m_Children.AsReadOnly(); } }
		
		public GraphNode Parent {
			get { return m_Parent; }
			set {
				if( m_Parent != value ) {
					if( m_Parent != null ) {
						GraphNode node = m_Parent;
						m_Parent = null;
						node.RemoveChild(this);
					}
					m_Parent = value;
					if( m_Parent != null )
						m_Parent.AddChild(this);
				}
			}
		}
		
		public GraphNode() : base() {
		}
		
		public GraphPin GetInput(int index) {
			return m_Inputs[index];
		}
		
		public void AddInput(GraphPin pin) {
			if( pin.Type != GraphPin.PinType.Input )
				throw new UserFriendlyException("Cannot add output pin to node intput", "There was an error in graph node usage");
			if( !m_Inputs.Contains(pin) ) {
				m_Inputs.Add(pin);
				pin.Node = this;
			}
		}
		
		public void RemoveInput(GraphPin pin) {
			if( m_Inputs.Contains(pin) ) {
				m_Inputs.Remove(pin);
				pin.Node = null;
			}
		}
		
		public GraphPin GetOutput(int index) {
			return m_Outputs[index];
		}
		
		public void AddOutput(GraphPin pin) {
			if( pin.Type != GraphPin.PinType.Output )
				throw new UserFriendlyException("Cannot add input pin to node output", "There was an error in graph node usage");
			if( !m_Outputs.Contains(pin) ) {
				m_Outputs.Add(pin);
				pin.Node = this;
			}
		}
		
		public void RemoveOutput(GraphPin pin) {
			if( m_Outputs.Contains(pin) ) {
				m_Outputs.Remove(pin);
				pin.Node = null;
			}
		}
		
		public void AddChild(GraphNode node) {
			if( !m_Children.Contains(node) ) {
				m_Children.Add(node);
				node.Parent = this;
			}
		}
		
		public void RemoveChild(GraphNode node) {
			if( m_Children.Contains(node) ) {
				m_Children.Remove(node);
				node.Parent = null;
			}
		}
	}
}