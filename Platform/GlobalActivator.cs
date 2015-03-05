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
using System.Reflection;
using System.Globalization;

namespace IGE.Platform {
	/// <summary>
	/// </summary>
	public static class GlobalActivator {
		private static Dictionary<string, Type> m_TypeCache = new Dictionary<string, Type>();
		public static Type FindType(string typeName) {
			Type type;
			if( m_TypeCache.TryGetValue(typeName, out type) )
				return type;
			foreach( Assembly asy in AppDomain.CurrentDomain.GetAssemblies() ) {
				type = asy.GetType(typeName);
				if( type != null ) {
					m_TypeCache.Add(typeName, type);
					return type;
				}
			}
			return null;
		}
		
		public static void ClearTypeCache() {
			m_TypeCache.Clear();
		}
		
		public static object CreateInstance(string typeName, params object[] constructorParams) {
			return CreateInstance(FindType(typeName), constructorParams);
		}
		
		public static object CreateInstance(Type type, params object[] constructorParams) {
			return Activator.CreateInstance(type, BindingFlags.Default, null, constructorParams, CultureInfo.CurrentCulture);
		}
	}
}
