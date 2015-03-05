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
	public abstract class DomObject<TDomObject> where TDomObject : DomObject<TDomObject> {
		private string m_Name;
		private string m_Value;
		
		private DomNode m_ParentNode;
		private TDomObject m_PreviousSibling;
		private TDomObject m_NextSibling;

		public string Name {
			get { return m_Name; }
			set {
				if( value.Equals(m_Name) )
					return;
				if( OnNameChange(m_Name, value) )
					m_Name = value;					
			}
		}
		
		public string Value { get { return m_Value; } set { m_Value = value; } }

		public DomNode ParentNode { get { return m_ParentNode; } internal set { m_ParentNode = value; } }
		public TDomObject PreviousSibling { get { return m_PreviousSibling; } internal set { m_PreviousSibling = value; } }
		public TDomObject NextSibling { get { return m_NextSibling; } internal set { m_NextSibling = value; } }

		public DomObject(string name, string val) {
			m_Name = name;
			m_Value = val;
			m_ParentNode = null;
			m_NextSibling = default(TDomObject);
			m_PreviousSibling = default(TDomObject);
		}

		public DomObject() : this("", "") {
		}
		
		public virtual bool OnNameChange(string oldName, string newName) {
			return true;
		}
	}
}
