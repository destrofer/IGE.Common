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
	public class SpaceHash<TKey, TValue, TCoord> : IDictionary<TKey, TValue> where TCoord : struct, IComparable<TCoord>, IConvertible {
		private Dictionary<TKey, TValue> m_Hash;
		private Dictionary<TKey, SpaceHashObject> m_KeyHash;
		private Dictionary<TValue, SpaceHashObject> m_ValueHash;
		private Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<TKey, SpaceHashObject>>>> m_SpaceHash;
		private TCoord m_ClusterSize;
		
		public TCoord ClusterSize { get { return m_ClusterSize; } }
		public int Count { get { return m_ValueHash.Count; } }
		
		public SpaceHash(TCoord clusterSize) {
			if( clusterSize.CompareTo(default(TCoord)) <= 0 )
				throw new Exception("Cluster size must be greater than 0");
			m_ClusterSize = clusterSize;
			m_Hash = new Dictionary<TKey, TValue>();
			m_KeyHash = new Dictionary<TKey, SpaceHashObject>();
			m_ValueHash = new Dictionary<TValue, SpaceHashObject>();
			m_SpaceHash = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<TKey, SpaceHashObject>>>>();
		}

		public bool IsReadOnly { get { return false; } }
		public ICollection<TValue> Values { get { return m_Hash.Values; } }
		public ICollection<TKey> Keys { get { return m_Hash.Keys; } }

		public TValue this[TKey key] {
			get { return m_Hash[key]; }
			set {
				SpaceHashObject entry = m_KeyHash[key];
				m_ValueHash.Remove(entry.Value);
				m_ValueHash.Add(value, entry);
				entry.Value = value;
				m_Hash[key] = value;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
			return (IEnumerator<KeyValuePair<TKey, TValue>>)m_Hash.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] target, int count) {
			throw new NotImplementedException();
		}

		public bool TryGetValue(TKey key, out TValue val) {
			return m_Hash.TryGetValue(key, out val);
		}

		public virtual void Add(KeyValuePair<TKey, TValue> pair) {
			Add(pair.Key, pair.Value);
		}

		public virtual void Add(TKey key, TValue val) {
			Add(key, val, default(TCoord), default(TCoord), default(TCoord));
		}
		
		public virtual void Add(TKey key, TValue val, TCoord x, TCoord y, TCoord z) {
			SpaceHashObject entry = new SpaceHashObject(key, val);
			m_Hash.Add(key, val);
			m_ValueHash.Add(val, entry);
			m_KeyHash.Add(key, entry);
			PlaceAt(entry, x, y, z);
		}
		
		public virtual bool ContainsKey(TKey key) {
			return m_Hash.ContainsKey(key);
		}
		
		public virtual bool Contains(KeyValuePair<TKey, TValue> pair) {
			return m_Hash.ContainsKey(pair.Key);
		}
		
		public virtual bool Contains(TValue val) {
			return m_ValueHash.ContainsKey(val);
		}
		
		public virtual void MoveKey(TKey key, TCoord newX, TCoord newY, TCoord newZ) {
			PlaceAt((SpaceHashObject)m_KeyHash[key], newX, newY, newZ);
		}
		
		public virtual void Move(TValue val, TCoord newX, TCoord newY, TCoord newZ) {
			PlaceAt((SpaceHashObject)m_ValueHash[val], newX, newY, newZ);
		}

		public virtual bool Remove(KeyValuePair<TKey, TValue> pair) {
			return Remove(pair.Key);
		}
		
		public virtual void Clear() {
			m_Hash.Clear();
			m_KeyHash.Clear();
			m_ValueHash.Clear();
			m_SpaceHash.Clear();
		}
		
		public virtual bool Remove(TKey key) {
			SpaceHashObject entry = (SpaceHashObject)m_KeyHash[key];
			m_Hash.Remove(key);
			m_KeyHash.Remove(key);
			m_ValueHash.Remove(entry.Value);
			if( entry.Cluster != null ) {
				entry.Cluster.Remove(key);
				entry.Cluster = null;
				entry.Key = default(TKey);
				entry.Value = default(TValue);
			}
			return true;
		}
		
		public IEnumerable<TValue> GetAllInArea(TCoord x1, TCoord y1, TCoord z1, TCoord x2, TCoord y2, TCoord z2) {
			int cx1, cy1, cz1, cx2, cy2, cz2, cx, cy, cz;
			GetClusterCoords(x1, y1, z1, out cx1, out cy1, out cz1);
			GetClusterCoords(x2, y2, z2, out cx2, out cy2, out cz2);
			
			Dictionary<int, Dictionary<int, Dictionary<TKey, SpaceHashObject>>> ySpace;
			Dictionary<int, Dictionary<TKey, SpaceHashObject>> zSpace;
			
			for( cx = cx2; cx >= cx1; cx-- ) {
				if( !m_SpaceHash.ContainsKey(cx) )
					continue;
				ySpace = m_SpaceHash[cx];
				for( cy = cy2; cy >= cy1; cy-- ) {
					if( !ySpace.ContainsKey(cy) )
						continue;
					zSpace = ySpace[cy];
					for( cz = cz2; cz >= cz1; cz-- ) {
						if( !zSpace.ContainsKey(cz) )
							continue;
						foreach(SpaceHashObject entry in zSpace[cz].Values) {
							if( entry.X.CompareTo(x1) >= 0 && entry.X.CompareTo(x2) <= 0
							&& entry.Y.CompareTo(y1) >= 0 && entry.Y.CompareTo(y2) <= 0
							&& entry.Z.CompareTo(z1) >= 0 && entry.Z.CompareTo(z2) <= 0 )
								yield return entry.Value;
						}
					}
				}
			}
		}
		
		/// <summary>
		/// This is a faster version of GetAllInArea. It does not check if entries really are in the area. Just returns all values in clusters that come in touch with the area.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="z1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		/// <param name="z2"></param>
		/// <returns></returns>
		public IEnumerable<TValue> GetAllInClusters(TCoord x1, TCoord y1, TCoord z1, TCoord x2, TCoord y2, TCoord z2) {
			int cx1, cy1, cz1, cx2, cy2, cz2, cx, cy, cz;
			GetClusterCoords(x1, y1, z1, out cx1, out cy1, out cz1);
			GetClusterCoords(x2, y2, z2, out cx2, out cy2, out cz2);
			
			Dictionary<int, Dictionary<int, Dictionary<TKey, SpaceHashObject>>> ySpace;
			Dictionary<int, Dictionary<TKey, SpaceHashObject>> zSpace;
			
			for( cx = cx2; cx >= cx1; cx-- ) {
				if( !m_SpaceHash.ContainsKey(cx) )
					continue;
				ySpace = m_SpaceHash[cx];
				for( cy = cy2; cy >= cy1; cy-- ) {
					if( !ySpace.ContainsKey(cy) )
						continue;
					zSpace = ySpace[cy];
					for( cz = cz2; cz >= cz1; cz-- ) {
						if( !zSpace.ContainsKey(cz) )
							continue;
						foreach(SpaceHashObject entry in zSpace[cz].Values)
							yield return entry.Value;
					}
				}
			}
		}
		
		public void GetClusterCoords(TCoord x, TCoord y, TCoord z, out int cx, out int cy, out int cz) {
			double cs = Convert.ToDouble(ClusterSize);
			cx = (int)Math.Floor(Convert.ToDouble(x) / cs);
			cy = (int)Math.Floor(Convert.ToDouble(y) / cs);
			cz = (int)Math.Floor(Convert.ToDouble(z) / cs);
		}
		
		internal void PlaceAt(SpaceHashObject entry, TCoord x, TCoord y, TCoord z) {
			int cx, cy, cz;
			GetClusterCoords(x, y, z, out cx, out cy, out cz);
			entry.X = x;
			entry.Y = y;
			entry.Z = z;
			if( entry.Cluster == null || entry.ClusterX != cx || entry.ClusterY != cy || entry.ClusterZ != cz ) {
				Dictionary<int, Dictionary<int, Dictionary<TKey, SpaceHashObject>>> ySpace;
				Dictionary<int, Dictionary<TKey, SpaceHashObject>> zSpace;
				Dictionary<TKey, SpaceHashObject> objects;
				
				if( m_SpaceHash.ContainsKey(cx) )
					ySpace = m_SpaceHash[cx];
				else
					m_SpaceHash.Add(cx, ySpace = new Dictionary<int, Dictionary<int, Dictionary<TKey, SpaceHashObject>>>());
				
				if( ySpace.ContainsKey(cy) )
					zSpace = ySpace[cy];
				else
					ySpace.Add(cy, zSpace = new Dictionary<int, Dictionary<TKey, SpaceHashObject>>());
				
				if( zSpace.ContainsKey(cz) )
					objects = zSpace[cz];
				else
					zSpace.Add(cz, objects = new Dictionary<TKey, SpaceHashObject>());
	
				if( entry.Cluster != null )
					entry.Cluster.Remove(entry.Key);
				entry.Cluster = objects;

				entry.ClusterX = cx;
				entry.ClusterY = cy;
				entry.ClusterZ = cz;

				objects.Add(entry.Key, entry);
			}
		}

		internal class SpaceHashObject {
			internal TCoord X;
			internal TCoord Y;
			internal TCoord Z;
			internal TKey Key;
			internal TValue Value;
			internal Dictionary<TKey, SpaceHashObject> Cluster;
			internal int ClusterX;
			internal int ClusterY;
			internal int ClusterZ;
			
			internal SpaceHashObject(TKey key, TValue val) {
				X = Y = Z = default(TCoord);
				Key = key;
				Value = val;
				Cluster = null;
			}
		}
	}	
}
