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
using System.Text;

namespace IGE {
	public class DomNode : DomObject<DomNode>, IEnumerable<DomNode> {
		private DomNodeContainer m_Children;
		private DomAttributeContainer m_Attributes;
		
		public DomNodeContainer Children { get { return m_Children; } }
		public DomNode FirstChild { get { return m_Children.First; } }
		public DomNode LastChild { get { return m_Children.Last; } }

		public DomAttributeContainer Attributes { get { return m_Attributes; } }
		public DomAttribute FirstAttribute { get { return m_Attributes.First; } }
		public DomAttribute LastAttribute { get { return m_Attributes.Last; } }

		public string this[string attributeName] { get { return m_Attributes[attributeName]; } set { m_Attributes[attributeName] = value; } }

		public DomNode(string name, string val) : base(name, val) {
			m_Children = new DomNodeContainer(this);
			m_Attributes = new DomAttributeContainer(this);
		}

		public DomNode() : this("", "") {
		}

		public IEnumerator<DomNode> GetEnumerator() {
			return m_Children.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public virtual bool Add(DomNode node) {
			m_Children.Add(node);
			return true;
		}
		
		public virtual bool Add(DomAttribute attrib) {
			m_Attributes.Add(attrib);
			return true;
		}
		
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append('<');
			sb.Append(Name);
			foreach(DomAttribute attr in m_Attributes) {
				sb.Append(' ');
				sb.Append(attr.ToString());
			}
			if( m_Children.Count > 0 ) {
				sb.Append(">\n");
				
				foreach(DomNode child in m_Children) {
					sb.Append(child.ToString());
				}
				
				if( Value.Length != 0 )
					sb.Append(String.Format("<![CDATA[{0}]]>\n", Value));
				
				sb.Append("</");
				sb.Append(Name);
				sb.Append(">\n");
			}
			else if( Value.Length != 0 ) {
				sb.Append(String.Format(">\n<![CDATA[{0}]]>\n</{1}>\n", Value, Name));
			}
			else {
				sb.Append(" />\n");
			}
			return sb.ToString();
		}
	}
}
