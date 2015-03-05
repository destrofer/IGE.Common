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
	public sealed class RegenerateableResource<TResType> {
		private bool m_IsAlive = false;
		private bool m_MayExpire = true;
		private bool m_Disposed = false;
		private TResType m_Resource = default(TResType);
		
		public bool IsAlive { get { return m_IsAlive && !m_Disposed; } }
		public bool MayExpire {
			get { return m_MayExpire; }
			set {
				if( m_MayExpire == value )
					return;
				if( m_MayExpire = value )
					AddToChain();
				else
					RemoveFromChain();
			}
		}
		public bool Disposed { get { return m_Disposed; } }
		public TResType Resource {
			get { return m_Resource; }
			set { m_Resource = value; } // Yes, the resource may be replaced!
		}
		
		
		private static RegenerateableResource<TResType> FirstAliveResource = null;
		private static RegenerateableResource<TResType> LastAliveResource = null;
		private static int m_ChainSize = 0;
		
		private RegenerateableResource<TResType> NextAliveResource = null; // might want to use weak references
		private RegenerateableResource<TResType> PrevAliveResource = null; // might want to use weak references
		
		private CallbackInfo<ResourceRegenerationResult<TResType>> LoadCallback = CallbackInfo<ResourceRegenerationResult<TResType>>.Empty;
		private Delegate ReleaseCallback = null;

		public RegenerateableResource(CallbackInfo<ResourceRegenerationResult<TResType>> load_callback) : this(load_callback, null, true) {
		}

		public RegenerateableResource(CallbackInfo<ResourceRegenerationResult<TResType>> load_callback, bool may_expire) : this(load_callback, null, may_expire) {
		}

		public RegenerateableResource(CallbackInfo<ResourceRegenerationResult<TResType>> load_callback, Delegate release_callback) : this(load_callback, release_callback, true) {
		}
		
		public RegenerateableResource(CallbackInfo<ResourceRegenerationResult<TResType>> load_callback, Delegate release_callback, bool may_expire) {
			m_IsAlive = false;
			m_MayExpire = may_expire;
			m_Disposed = false;
			m_Resource = default(TResType);
			LoadCallback = load_callback;
			ReleaseCallback = release_callback;
		}
		
		public bool EnsureLoaded() {
			if( m_Disposed )
				return false; // it was disposed not to regenerate ever again
			if( m_IsAlive ) {
				// The resource is still alive and it seems that it is about to be used so we move it on top of the alive resources chain, so it would live longer
				if( m_MayExpire ) { // Forever lasting resources are not present in the chain
					if( NextAliveResource != null ) { // check if we are at the end of the chain already
						if( PrevAliveResource != null )
							PrevAliveResource.NextAliveResource = NextAliveResource;
						NextAliveResource.PrevAliveResource = PrevAliveResource;
						
						NextAliveResource = null;
						PrevAliveResource = LastAliveResource;
						LastAliveResource.NextAliveResource = this;
						LastAliveResource = this;
					}
				}
			}
			else do {
				ResourceRegenerationResult<TResType> result = LoadCallback.Invoke();
				switch( result.Code ) {
					case ResourceRegenerationResultCode.Success: // The resource was successfully loaded
						m_IsAlive = true;
						m_Resource = result.Resource;
						
						if( m_MayExpire ) // This resource is allowed to be released when more memory is needed so we have to add it to the chain of alive resources
							AddToChain();
						break;
						
					case ResourceRegenerationResultCode.NotEnoughMemory: // There was not enough free memory in resource space to load it ... try to release an unused resource before trying to load this one again
						if( !ReleaseUnusedResource() )
							return false;
						break;
						
					default:
						return false;
				}
			} while( !m_IsAlive );
			return true;
		}
		
		public void Release(bool final) {
			if( ReleaseCallback != null )
				ReleaseCallback.DynamicInvoke(final);
			if( m_MayExpire )
				RemoveFromChain();
			m_IsAlive = false;
			m_Resource = default(TResType);
			if( final )
				m_Disposed = true;
		}
		
		private void RemoveFromChain() {
			if( PrevAliveResource != null )
				PrevAliveResource.NextAliveResource = NextAliveResource;
			else
				FirstAliveResource = NextAliveResource;
				
			if( NextAliveResource != null )
				NextAliveResource.PrevAliveResource = PrevAliveResource;
			else
				LastAliveResource = PrevAliveResource;
			m_ChainSize--;		
		}
		
		private void AddToChain() {
			if( FirstAliveResource == null )
				FirstAliveResource = this;
			else {
				LastAliveResource.NextAliveResource = this;
				PrevAliveResource = LastAliveResource;
			}
			LastAliveResource = this;
			m_ChainSize++;
		}
		
		private static bool ReleaseUnusedResource() {
			if( m_ChainSize <= 0 )
				return false;
			if( FirstAliveResource.ReleaseCallback != null )
				FirstAliveResource.ReleaseCallback.DynamicInvoke(false);
			
			RegenerateableResource<TResType> first = FirstAliveResource;
			FirstAliveResource = first.NextAliveResource;
			
			first.m_IsAlive = false;
			first.m_Resource = default(TResType);
			first.NextAliveResource = null;
			
			if( FirstAliveResource != null )
				FirstAliveResource.PrevAliveResource = null;
			else
				LastAliveResource = null;
			m_ChainSize--;
			return true;
		}
	}
	
	public struct ResourceRegenerationResult<TResType> {
		public static readonly ResourceRegenerationResult<TResType> NotEnoughMemory = new ResourceRegenerationResult<TResType> { Code = ResourceRegenerationResultCode.NotEnoughMemory, Resource = default(TResType) };
		public static readonly ResourceRegenerationResult<TResType> LoadError = new ResourceRegenerationResult<TResType> { Code = ResourceRegenerationResultCode.LoadError, Resource = default(TResType) };
		
		public ResourceRegenerationResultCode Code;
		public TResType Resource;
		
		public ResourceRegenerationResult(TResType resource) {
			Code = ResourceRegenerationResultCode.Success;
			Resource = resource;
		}
	}
	
	public enum ResourceRegenerationResultCode {
		Success = 0,
		NotEnoughMemory = 1,
		LoadError = 2,
	}
}
