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
using System.IO;
using System.Collections.Generic;

namespace IGE.Data {
	public abstract class IGEFileStream : Stream, IFileSystemObject {
		protected IGEDirectory m_Parent;
		public IGEDirectory Parent { get { return m_Parent; } }

		protected string m_InternalPath;
		public string InternalPath { get { return m_InternalPath; } }

		protected string m_Name;
		public string Name { get { return m_Name; } }

		protected FileAccess m_Access;
		public FileAccess Access { get { return m_Access; } }

		protected FileMode m_Mode;
		public FileMode Mode { get { return m_Mode; } }

		protected IGEFileStream(IGEDirectory parent, string internalPath, string name, FileAccess access, FileMode mode) {
			m_Parent = parent;
			m_InternalPath = internalPath;
			m_Name = name;
			m_Access = access;
			m_Mode = mode;
		}
		
		public abstract Dictionary<string, object> GetMeta();

		public static IGEFileStream Open(string path, FileAccess access, FileMode mode) {
			return null;
		}
	}
}

