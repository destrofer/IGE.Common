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
using System.Text;
using System.Collections.Generic;

namespace IGE {
	/// <summary>
	/// Description of ScopeStack.
	/// </summary>
	public class ScopeStack<T> {
		protected List<Scope<T>> ScopeList = new List<Scope<T>>();
		protected Scope<T> CurrentScope = null;
		
		public ScopeStack() {
		}
		
		public void Clear() {
			ScopeList.Clear();
			CurrentScope = null;
		}
		
		public void IncrementLevel(bool isRoot) {
			lock(ScopeList) {
				ScopeList.Add(CurrentScope);
				CurrentScope = new Scope<T>(isRoot);
			}
		}
		
		public void DecrementLevel(bool isRoot) {
			if( CurrentScope == null )
				throw new Exception("Cannot decrement scope level: scope list is empty");
			if( CurrentScope.IsRoot != isRoot )
				throw new Exception("Scope stack consistency failure: trying to remove a scope from the stack with different IsRoot flag set. Maybe something did not decrement level after incrementing?");
			lock(ScopeList) {
				int idx = ScopeList.Count - 1;
				CurrentScope = ScopeList[idx];
				ScopeList.RemoveAt(idx);
			}
		}
		
		public IEnumerable<Scope<T>> EnumerateCurrentScopes() {
			if( CurrentScope == null )
				yield break;
			yield return CurrentScope;
			int i;
			Scope<T> scope = CurrentScope;
			for( i = ScopeList.Count - 1; i > 0 && !scope.IsRoot; i-- ) { // skip index 0 since it always contains null
				scope = ScopeList[i];
				yield return scope;
			}
			if( i > 1 && ScopeList.Count > 1 )
				yield return ScopeList[1]; // also enumerate global scope
		}
		
		public bool ContainsKey(string key) {
			lock(ScopeList) {
				foreach( Scope<T> scope in EnumerateCurrentScopes() ) {
					if( scope.ContainsKey(key) )
						return true;
				}
			}
			return false;
		}
		
		public bool TryGetValue(string key, out T val) {
			lock(ScopeList) {
				val = default(T);
				foreach( Scope<T> scope in EnumerateCurrentScopes() ) {
					if( scope.TryGetValue(key, out val) )
						return true;
				}
			}
			return false;
		}

		public T Get(string key) {
			lock(ScopeList) {
				T val;
				foreach( Scope<T> scope in EnumerateCurrentScopes() ) {
					if( scope.TryGetValue(key, out val) )
						return val;
				}
			}
			throw new Exception("No active scopes contain value with a given key");
		}
		
		public void Add(string key, T val) {
			lock(ScopeList) {
				if( CurrentScope == null )
					throw new Exception("Cannot add a value without entering scope first");
				CurrentScope.Add(key, val);
			}
		}
		
		public void Set(string key, T val) {
			lock(ScopeList) {
				foreach( Scope<T> scope in EnumerateCurrentScopes() ) {
					if( scope.ContainsKey(key) ) {
						scope.Set(key, val);
						return;
					}
				}
			}
			throw new Exception("No active scopes contain value with a given key");
		}
		
		public void Output() {
			foreach( Scope<T> scope in ScopeList )
				Console.WriteLine("---- {0}", (scope == null) ? "NULL" : scope.ToString());
			Console.WriteLine("----- {0}", (CurrentScope == null) ? "NULL" : CurrentScope.ToString());
		}
		
		public class Scope<DT> {
			protected Dictionary<string, DT> Data = new Dictionary<string, DT>();
			protected bool m_IsRoot;
			
			public bool IsRoot { get { return m_IsRoot; } }
			
			public Scope(bool isRoot) {
				m_IsRoot = isRoot;
			}
			
			public bool ContainsKey(string key) {
				return Data.ContainsKey(key);
			}
			
			public bool TryGetValue(string key, out DT val) {
				return Data.TryGetValue(key, out val);
			}
			
			public void Add(string key, DT val) {
				Data.Add(key, val);
			}
			
			public void Set(string key, DT val) {
				Data[key] = val;
			}
			
			public override string ToString() {
				StringBuilder str = new StringBuilder();
				str.Append(String.Format("Scope({0}):", m_IsRoot));
				foreach( string key in Data.Keys ) {
					str.Append(' ');
					str.Append(key);
				}
				return str.ToString();
			}
		}
	}
}
