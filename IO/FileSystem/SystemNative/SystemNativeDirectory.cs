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
	public class SystemNativeDirectory : IGEDirectory {
		protected DirectoryInfo m_DirInfo;

		protected SystemNativeDirectory(IGEDirectory parent, string internalName, string name) : base(parent, internalName, name) {
			m_DirInfo = new DirectoryInfo(internalName);
		}
		
		protected SystemNativeDirectory(IGEDirectory parent, string internalName, string name, DirectoryInfo dirInfo) : base(parent, internalName, name) {
			m_DirInfo = dirInfo;
		}

		public override IGEDirectory CreateSubdirectory(string name) {
			IGEDirectory result = base.CreateSubdirectory(name);
			if (result != null)
				return result;
			DirectoryInfo dirInfo = m_DirInfo.CreateSubdirectory(name);
			if (dirInfo != null)
				result = new SystemNativeDirectory(this, String.Format ("{0}/{1}", m_InternalPath, name), name, dirInfo);
			return result;
		}

		public override void Delete(string name) {
		}

		public override void Move(string nameFrom, string nameTo) {
		}

		public override bool Exists(string name) {
			return Directory.Exists(String.Format("{0}/{1}", m_InternalPath, name));
		}

		public override IEnumerable<string> EnumerateDirectories() {
			yield break;
		}

		public override IEnumerable<string> EnumerateFiles() {
			yield break;
		}

		public override IEnumerable<string> EnumerateFileSystemEntries() {
			yield break;
		}

		public override string[] GetDirectories() {
			return new string[0];
		}

		public override string[] GetFiles() {
			return new string[0];
		}

		public override string[] GetFileSystemEntries() {
			return new string[0];
		}
		
		public override Dictionary<string, object> GetMetaOf(string name) {
			return new Dictionary<string, object>();
		}


		
		public override void Delete() {
		}

		public override Dictionary<string, object> GetMeta() {
			return new Dictionary<string, object>();
		}
	}
}

