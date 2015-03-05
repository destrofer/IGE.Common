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
	public abstract class IGEDirectory : IFileSystemObject {
		protected IGEDirectory m_Parent;
		public IGEDirectory Parent { get { return m_Parent; } }

		protected string m_InternalPath;
		public string InternalPath { get { return m_InternalPath; } }

		protected string m_Name;
		public string Name { get { return m_Name; } }

		protected IGEDirectory(IGEDirectory parent, string internalPath, string name) {
			m_Parent = parent;
			m_InternalPath = internalPath;
			m_Name = name;
		}

		public virtual IGEDirectory CreateSubdirectory(string name) {
			return null;
		}

		public abstract void Delete(string name);
		public abstract void Move(string nameFrom, string nameTo);
		public abstract bool Exists(string name);

		public abstract IEnumerable<string> EnumerateDirectories();
		public abstract IEnumerable<string> EnumerateFiles();
		public abstract IEnumerable<string> EnumerateFileSystemEntries();

		public abstract string[] GetDirectories();
		public abstract string[] GetFiles();
		public abstract string[] GetFileSystemEntries();

		public abstract Dictionary<string, object> GetMetaOf(string name);

		public abstract void Delete();
		public abstract Dictionary<string, object> GetMeta();

		public static IGEDirectory Open(string path) {
			return null;
		}
	}
}

