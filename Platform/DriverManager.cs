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
using System.Globalization;

namespace IGE.Platform {
	/// <summary>
	/// </summary>
	public static class DriverManager {
		public const string BaseNamespace = "IGE.Platform";
		
		public static string GetPlatformName() {
			// TODO: improve detection algorithm
			int p = (int) Environment.OSVersion.Platform;
			if ((p == 4) || (p == 6) || (p == 128))
				return "Unix";
			return "Win32";
		}
		
		// TODO: cache already loaded drivers so that same assemblies/drivers would not get loaded more than once.
		public static T LoadDriver<T>(string driverName, Version expectedVersion) where T : class, IDriver {
			string driverClass = String.Format("{0}.{1}.{2}", BaseNamespace, GetPlatformName(), driverName);
			string[] extensions = new string[] {"dll", "so", "o"};
			Type expectedDriverType = typeof(T), type;
			string[] names = driverClass.Split('.');
			string fileName;
			int i;
			Assembly assembly;
			Version assemblyVersion;
			T driver = default(T);
			MethodInfo getInstance;
			
			GameDebugger.EngineLog(LogLevel.Debug, "Searching for driver '{0}'", driverClass);
			for( i = names.Length; driver == null && i > 0; i-- ) {
				// Trying all possible extensions, not only dll 
				foreach( string extension in extensions ) {
					fileName = String.Format("{0}.{1}", String.Join(".", names, 0, i), extension);
					try {
						GameDebugger.EngineLog(LogLevel.Debug, "Trying: {0}", fileName);
						if( File.Exists(fileName) ) {
							// Load assembly and check it's version
							assembly = Assembly.UnsafeLoadFrom(fileName);
							assemblyVersion = assembly.GetName().Version;
							GameDebugger.EngineLog(LogLevel.Debug, "Assembly version: {0}", assemblyVersion);
							if( assemblyVersion.Major != expectedVersion.Major || assemblyVersion.Minor != expectedVersion.Minor ) {
								GameDebugger.EngineLog(LogLevel.Debug, "Major or minor version differs from expected. Expecting version: {0}", expectedVersion);
								continue;
							}
							if( assemblyVersion.Build < expectedVersion.Build ) {
								GameDebugger.EngineLog(LogLevel.Debug, "Build version is lower than expected. Expecting version: {0}", expectedVersion);
								continue;
							}

							// Find driver within loaded assembly
							type = assembly.GetType(driverClass);
							if( type != null ) {
								if( expectedDriverType.IsAssignableFrom(type) ) {
									// First try using driver's GetInstance() method in case it's a singleton like API
									getInstance = type.GetMethod("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod);
									if( getInstance != null )
										driver = getInstance.Invoke(null, BindingFlags.Static, null, null, CultureInfo.CurrentCulture) as T;
									if( driver == null )
										driver = Activator.CreateInstance(type) as T;
									if( driver != null )
										break;
									type = null;
									assembly = null; // Unload assembly ... but will it actually unload it?
									GameDebugger.EngineLog(LogLevel.Debug, "Type found, but still could not create it's instance nor via GetInstance() method nor via constructor. Maybe it's not compatible with '{0}'?", expectedDriverType.FullName);
								}
								else {
									type = null;
									assembly = null; // Unload assembly ... but will it actually unload it?
									GameDebugger.EngineLog(LogLevel.Debug, "Type found, but it is not implementing expected '{0}'", expectedDriverType.FullName);
								}
							}
							else {
								assembly = null; // Unload assembly ... but will it actually unload it?
								GameDebugger.EngineLog(LogLevel.Debug, "Assembly doesn't contain required type", driverClass);
							}
						}
					}
					catch(Exception ex) {
						GameDebugger.EngineLog(LogLevel.Debug, ex);
					}
				}
			}
			
			if( driver == null )
				GameDebugger.EngineLog(LogLevel.Error, "Driver '{0}' could not be loaded", driverClass);
			else
				GameDebugger.EngineLog(LogLevel.Info, "Driver '{0}' successfully loaded", driverClass);
			
			return driver;
		}
	}
}
