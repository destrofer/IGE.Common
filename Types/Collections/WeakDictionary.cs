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
 using System.Runtime.InteropServices;
 
 namespace IGE {
	/// <summary>
	/// An implementation of a dictionary containing weak references to objects.
	/// Unlike normal IDictionary implementations it does not have methods
	/// that have anything to do with keys (except Add and this[]) because
	/// the value may be garbage-collected in berween calls to ContainsKey
	/// method and this[] property.
	/// </summary>
	public class WeakDictionary<TKey, TValue> : IDisposable {
		private Dictionary<TKey, GCHandle> m_Items = new Dictionary<TKey, GCHandle>();
		private int m_UseCount = 0;
		
		/// <summary>
		/// Allows to get amount of keys stored in the dictionary. The amount of alive objects may differ from this amount so use this property just for statistics.
		/// </summary>
		public int Count { get { return m_Items.Count; } }
		
		public IEnumerable<TValue> Values {
			get {
				foreach(GCHandle handle in m_Items.Values) {
					object val = handle.Target;
					if( val != null )
						yield return (TValue)val;
				}
			}
		}
		
		public TValue this[TKey key] {
			get {
				lock(m_Items) {
					// Cleanup unused items every 100 requests.
					// This may be bad so maybe cleanup algorithm should
					// use a separate thread handling GC notifications.
					if( ++m_UseCount >= 100 ) {
						m_UseCount = 0;
						List<TKey> invalidKeys = new List<TKey>();
						foreach( KeyValuePair<TKey, GCHandle> pair in m_Items ) {
							if( pair.Value.Target == null ) {
								pair.Value.Free();
								invalidKeys.Add(pair.Key);
							}
						}
						foreach( TKey ikey in invalidKeys )
							m_Items.Remove(ikey);
					}
					
					if( m_Items.ContainsKey(key) ) {
						GCHandle handle = m_Items[key];
						object val = handle.Target;
						if( val != null )
							return (TValue)val;
						m_Items.Remove(key);
						handle.Free();
					}
				}
				return default(TValue);
			}
			
			set { Add(key, value); }
		}
		
		public void Add(TKey key, TValue val) {
			lock(m_Items) {
				if( m_Items.ContainsKey(key) ) {
					GCHandle handle = m_Items[key];
					handle.Target = val;
					m_Items[key] = handle;
				}
				else
					m_Items.Add(key, GCHandle.Alloc(val, GCHandleType.Weak));
			}
		}
		
		public void Remove(TKey key) {
			lock(m_Items) {
				if( m_Items.ContainsKey(key) ) {
					GCHandle handle = m_Items[key];
					m_Items.Remove(key);
					handle.Free();
				}
			}
		}
		
		public void Clear() {
			lock(m_Items) {
				foreach(GCHandle handle in m_Items.Values)
					handle.Free();
				m_Items.Clear();
			}
		}
		
		~WeakDictionary() {
			Dispose();
		}
		
		public void Dispose() {
			foreach( GCHandle handle in m_Items.Values )
				handle.Free();
			GC.SuppressFinalize(true);
		}
	}
 }