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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using IGE;

namespace IGE.IO {
	public abstract class GameFile : IDisposable {
		private static Hashtable FileFormatExtensionRegistry = new Hashtable();
		private static List<MethodInfo> FileFormatHandlerRegistry = new List<MethodInfo>();
		private static HashSet<int> CheckedAssemblies = new HashSet<int>();
		
		static GameFile() {
			RecheckAssemblies();
		}
		
		public static bool RecheckAssemblies() {
			bool updated = false;
			foreach( Assembly asy in AppDomain.CurrentDomain.GetAssemblies() )
				updated = ScanAssemblyForFileFormats(asy) || updated;
			return updated;
		}
		
		private static bool ScanAssemblyForFileFormats(Assembly asy) {
			if( CheckedAssemblies.Contains(asy.GetHashCode()) )
				return false;
			CheckedAssemblies.Add(asy.GetHashCode());
			
			GameDebugger.EngineLog(LogLevel.Debug, "Scanning assembly '{0}' ({1}) for file formats", asy.FullName, asy.GetHashCode());

			Type formatType = typeof(GameFile);
			Type attribType = typeof(FileFormatAttribute);
			ParameterInfo[] par;
			
			foreach( Type type in asy.GetTypes() ) {
				if( !type.IsClass || (!type.IsSubclassOf(formatType) && !type.Equals(formatType)) )
					continue;
				foreach( FileFormatAttribute attr in type.GetCustomAttributes(attribType, false) ) {
					if( attr.Extension == null )
						throw new Exception(String.Format("FileFormat attribute MUST have a file extension parameter for '{0}' class", type.FullName));
					if( FileFormatExtensionRegistry.ContainsKey(attr.Extension) )
						throw new Exception(String.Format("There are more than one '{0}' file format extension handler. Classes using it: {1} and {2}", attr.Extension, ((Type)FileFormatExtensionRegistry[attr.Extension]).FullName, type.FullName));
					FileFormatExtensionRegistry.Add(attr.Extension, type);
				}
				
				foreach( MethodInfo mi in type.GetMethods() ) {
					if( !mi.IsStatic || !mi.IsPublic )
						continue;
					foreach( FileFormatAttribute attr in mi.GetCustomAttributes(attribType, false) ) {
						if( attr.Extension != null )
							throw new Exception(String.Format("FileFormat attribute MUST NOT have a file extension parameter for '{0}' method in '{1}'", mi.Name, type.FullName));
						if( FileFormatHandlerRegistry.Contains(mi) )
							continue;
						par = mi.GetParameters();
						if( par.Length != 2 || !par[0].ParameterType.Equals(typeof(string)) || !par[1].ParameterType.Equals(typeof(Stream)) || !mi.ReturnType.Equals(typeof(Type)) )
							throw new Exception(String.Format("FileFormat attribute can be assigned only to methods having (string file_name, System.IO.Stream file) parameters and returning value of class 'Type'", mi.Name, type.FullName));
						FileFormatHandlerRegistry.Add(mi);
					}
				}
			}
			return true;
		}
		
		public GameFile(Stream stream) : this() {
		}
	
		public GameFile() {
		}
	
		public static bool Exists(string filename) { return System.IO.File.Exists(filename); }

		public static Type GetFormat(string filename) {
			Type type = null;
			
			bool retry = true;
			while( true ) {
				if( FileFormatHandlerRegistry.Count > 0 && Exists(filename) ) {
					using(Stream file = new FileStream(filename, FileMode.Open)) {
						object[] dp = new object[] { filename, file };
						foreach(MethodInfo mi in FileFormatHandlerRegistry)
							if( (type = (Type)mi.Invoke(null, dp)) != null )
								break;
					}
				}
				
				if( type == null ) { // no detector method found this format so try detecting by extension ...
					Match extMatch = Regex.Match(filename, @"\.([^\.]*)$");
					if( extMatch != null && extMatch.Success && extMatch.Groups.Count == 2 ) {
						string ext = extMatch.Groups[1].Value.ToLower();
						if( FileFormatExtensionRegistry.ContainsKey(ext) )
							type = (Type)FileFormatExtensionRegistry[ext];
					}
				}
				
				if( type != null || !retry || !RecheckAssemblies() )
					break;
				
				retry = false;
			}
			
			return type;
		}

		public static bool CheckFormat(string filename, Type type) {
			Type fmt = GetFormat(filename);
			if( fmt == null )
				return false;
			return type.IsAssignableFrom(fmt);
		}
		
		public static TFileFormatType LoadFile<TFileFormatType>(string filename) where TFileFormatType:GameFile {
			Type fmt = GetFormat(filename);
			if( fmt == null )
				throw new Exception(String.Format("Could not detect the file format for '{0}'", filename));
			if( !typeof(TFileFormatType).IsAssignableFrom(fmt) )
				throw new Exception(String.Format("File format handler class for '{0}' is expected to be '{1}' or one of it's derivatives, but '{2}' is assigned currently", filename, typeof(TFileFormatType).FullName, fmt.FullName));
			
			using(Stream file = new FileStream(filename, FileMode.Open)) {
				return (TFileFormatType)Activator.CreateInstance(fmt, new object[] { file });
			}
		}
		
		public static GameFile LoadFile(string filename) {
			Type fmt = GetFormat(filename);
			if( fmt == null )
				throw new Exception(String.Format("Could not detect the file format for '{0}'", filename));
			
			using(Stream file = new FileStream(filename, FileMode.Open)) {
				return (GameFile)Activator.CreateInstance(fmt, new object[] { file });
			}
		}
		
		public virtual void Dispose() {
		}
	}
}
