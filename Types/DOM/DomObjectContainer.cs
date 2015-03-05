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
	public abstract class DomObjectContainer<TDomObject> : IEnumerable<TDomObject> where TDomObject: DomObject<TDomObject> {
		private DomNode m_Owner;
		private List<TDomObject> m_Items;
		private Dictionary<string, DomObject<TDomObject>> m_Index;
		private TDomObject m_FirstItem;
		private TDomObject m_LastItem;
	
		public DomNode Owner { get { return m_Owner; } }
		public int Count { get { return (m_Items == null) ? 0 : m_Items.Count; } }
		
		public TDomObject First { get { return m_FirstItem; } }
		public TDomObject Last { get { return m_LastItem; } }
		
		public TDomObject this[int itemIndex] { get { return (m_Items != null) ? m_Items[itemIndex] : default(TDomObject); } }
		public string this[string itemName] {
			get {
				String name = itemName.ToLower();
				if( m_Index != null && m_Index.ContainsKey(name) )
					return m_Index[name].Value;
				return "";
			}
			set {
				String name = itemName.ToLower();
				if( m_Index != null && m_Index.ContainsKey(name) )
					m_Index[name].Value = value;
				else
					Add((TDomObject)Activator.CreateInstance(typeof(TDomObject), new object[] { itemName, value }));
			}
		}
		
		public IEnumerator<TDomObject> GetEnumerator() {
			return (m_Items == null) ? GetEmptyEnumerator() : m_Items.GetEnumerator();
		}
		
		private IEnumerator<TDomObject> GetEmptyEnumerator() {
			yield break;
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		
		public DomObjectContainer(DomNode owner) {
			m_Owner = owner;
			m_Items = null;
			m_Index = null;
			m_FirstItem = default(TDomObject);
			m_LastItem = default(TDomObject);
		}
		
		public abstract DomObjectContainer<TDomObject> GetContainerFor(TDomObject obj); // needed to remove item from another container if some item changes it's parent
		
		public void Add(TDomObject obj) {
			if( m_LastItem == null ) {
				m_Items = new List<TDomObject>();
				m_Index = new Dictionary<string, DomObject<TDomObject>>();
				
				m_FirstItem = obj;
				obj.PreviousSibling = null;
			}
			else {
				m_LastItem.NextSibling = obj;
				obj.PreviousSibling = m_LastItem;
			}
			
			m_LastItem = obj;
			obj.ParentNode = Owner;
			
			// TODO: remove from another container if object is already added somewhere else
			
			m_Items.Add(obj);
			string name = obj.Name.ToLower();
			if( !m_Index.ContainsKey(name) )
				m_Index.Add(name, obj);
		}
	}
}
