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
	public class GraphConnection : GraphObject {
		protected GraphPin m_Source = null;
		protected GraphPin m_Target = null;

		public GraphPin Source {
			get { return m_Source; }
			set {
				if( m_Source != value ) {
					if( m_Source != null ) {
						GraphPin pin = m_Source;
						m_Source = null;
						pin.RemoveOutputConnection(this);
					}
					m_Source = value;
					if( m_Source != null )
						m_Source.AddOutputConnection(this);
				}
			}
		}

		public GraphPin Target {
			get { return m_Target; }
			set {
				if( m_Target != value ) {
					if( m_Target != null ) {
						GraphPin pin = m_Target;
						m_Target = null;
						pin.RemoveInputConnection(this);
					}
					m_Target = value;
					if( m_Target != null )
						m_Target.AddInputConnection(this);
				}
			}
		}		

		public GraphConnection() : base() {
		}
	}
}