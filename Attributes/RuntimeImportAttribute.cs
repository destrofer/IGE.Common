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
	[AttributeUsage(AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	public class RuntimeImportAttribute : Attribute {
		public RuntimeImportAttribute(string library_name) : this(library_name, null, false) {}
		
		public RuntimeImportAttribute(string library_name, bool important) : this(library_name, null, important) {}

		public RuntimeImportAttribute(string library_name, string function_name) : this(library_name, function_name, false) {}
		
		public RuntimeImportAttribute(string library_name, string function_name, bool important) {
			m_LibraryName = library_name;
			m_FunctionName = function_name;
			m_Important = important;
		}
		
		protected string m_LibraryName;
		public string LibraryName { get { return m_LibraryName; } }
		
		protected string m_FunctionName;
		public string FunctionName { get { return m_FunctionName; } }

		protected bool m_Important;
		public bool Important { get { return m_Important; } }
	}
}