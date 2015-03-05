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
using System.Reflection;

namespace IGE {
	public abstract class CacheableObject : IDisposable {
		private static Dictionary<string, WeakReference> m_Cache;
		
		static CacheableObject() {
			m_Cache = new Dictionary<string, WeakReference>();
		}
		
		private bool m_Disposed = false;
		public bool Disposed { get { return m_Disposed; } }

		private string m_CachedName = null;
		public string CachedName { get { return m_CachedName; } }

		public CacheableObject() {
		}

		~CacheableObject() {
			if( m_Disposed )
				return;
			RemoveFromCache();
			m_Disposed = true;
			OnDispose(false);
			// GameDebugger.Log("{0} ({1}) was not properly disposed", GetType().FullName, m_CachedName);
		}

		public void Dispose() {
			if( m_Disposed )
				return;
			RemoveFromCache();
			m_Disposed = true;
			OnDispose(true);
			GC.SuppressFinalize(this);
		}
		
		public static void DisposeAll() {
			List<CacheableObject> disposeList = new List<CacheableObject>();
			CacheableObject obj;
			int i;
			
			lock( m_Cache ) {
				foreach( WeakReference obj_ref in m_Cache.Values ) {
					obj = (CacheableObject)obj_ref.Target;
					if( !obj_ref.IsAlive || obj == null )
						continue;
					disposeList.Add(obj);
				}
				m_Cache.Clear();
			}
			
			for( i = disposeList.Count - 1; i >= 0; i-- )
				disposeList[i].Dispose();
		}
		
		private void RemoveFromCache() {
			if( m_CachedName == null )
				return;
			lock( m_Cache ) {
				if( m_Cache.ContainsKey(m_CachedName) )
					m_Cache.Remove(m_CachedName);
			}
		}

		/// <summary>
		/// This method is called when object is being removed from the memory. It will be called only once so no need to check if it was already disposed or not.
		/// </summary>
		/// <param name="manual">Will have value of True in case if disposal was made using Dispose() method or auto disposal happened before GameEngine.UnloadEvent was invoked.</param>
		public virtual void OnDispose(bool manual) {
		}
		
		public static TObject Cache<TObject>(string name) where TObject:CacheableObject {
			return (TObject)Cache(typeof(TObject), name);
		}

		public static TObject Cache<TObject>(string name, params object[] parm) where TObject:CacheableObject {
			return (TObject)Cache(typeof(TObject), name, parm);
		}

		public static CacheableObject Cache(Type obj_type, string name) {
			return Cache(obj_type, name, null);
		}
		
		public static CacheableObject Cache(Type obj_type, string name, params object[] parm) {
			CacheableObject obj;
			if( name == null )
				throw new Exception("cache_name must not be null");
			
			lock(m_Cache) {
				if( m_Cache.ContainsKey(name) ) {
					WeakReference obj_ref = m_Cache[name];
					obj = (CacheableObject)obj_ref.Target;
					if( obj_ref.IsAlive && obj != null )
						return obj; // cache hit
					m_Cache.Remove(name); // // no longer in cache
				}
				// cache miss
				
				obj = (CacheableObject)Activator.CreateInstance(obj_type);
				obj.m_CachedName = null;
				obj.m_Disposed = false;
				
				int i, j, ln = 1 + ((parm == null) ? 0 : parm.Length);
				Type[] types = new Type[ln];
				object[] load_params = new object[ln];

				types[0] = typeof(string);
				load_params[0] = name;

				for( i = 1, j = 0; i < ln; i++, j++ ) {
					types[i] = parm[j].GetType();
					load_params[i] = parm[j];
				}
				
				MethodInfo mi = obj_type.GetMethod("Load", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public, null, types, null);
				if( mi == null )
					throw new Exception(String.Format("'{0}' does not contain a 'Load' method, suitable to accept all arguments, passed to 'Cache' static method", obj_type.FullName));
				if( mi.ReturnType == null || mi.ReturnType != typeof(bool) )
					throw new Exception(String.Format("'{0}.Load' method must return a boolean value to indicate wether resource was successfully loaded or not", obj_type.FullName));
				
				if( (bool)mi.Invoke(obj, load_params) ) {
					obj.m_CachedName = name;
					m_Cache.Add(name, new WeakReference(obj));
					return obj;
				}
				
				obj.Dispose();
			}
			return null;
		}
		
		public abstract bool Load(string fileName);
	}

	public abstract class CacheableObject<TObject> : CacheableObject where TObject:CacheableObject<TObject> {
		public static TObject Cache(string name) {
			return (TObject)CacheableObject.Cache(typeof(TObject), name);
		}
		
		public static TObject Cache(string name, params object[] parm) {
			return (TObject)CacheableObject.Cache(typeof(TObject), name, parm);
		}
	}
}