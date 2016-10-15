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
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using IGE;
using IGE.IO;

namespace IGE {
	public static class GameConfig {
		public static string GameWindowTitle = "IGE Game Window";
		public static int IconResourceId = 101;

		// configurable variables, which get saved to the file
		public static int ResX = -1;						// -1 for keeping resolution
		public static int ResY = -1;						// -1 for keeping resolution
		public static int BitsPerPixel = -1;				// -1 for keeping resolution
		public static int RefreshRate = -1;					// -1 for keeping refresh rate
		public static string DisplayDevice = null; 			// null for using primary, "\\\\.\\DISPLAY1", "\\\\.\\DISPLAY2", ... (DisplayDevice.Id)
		
		private static bool m_VSyncEnabled = true;			//  true to use vertical synchronization
		private static bool m_FullScreen = true;			//  true to use full screen
		private static bool m_AutoSwapBuffers = true;		//  true to swap buffers automatically after RenderFrame event. 
		
		//public static int BufferCount = 2;				// number of screen buffers (1, 2 or 3)
		//public static int FullScreenAntialiasing = 0;	// level of full screen antialiasing (0, 1, 2, 4, 8, 16)
		//public static bool StereoSupport = false;

		// configurable variables, but they don't get saved
		public static int ColorBits = 32;
		public static int DepthBufferBits = 24;
		public static int StencilBufferBits = 0;
		public static int AccumBufferBits = 0;
		//public static bool MouseCursorVisible = false;
		public static bool RecalcViewportOnResize = true;
		public static bool ClearEachFrame = true;
		//public static bool AlphaBuffer = false;
		
		public static bool ShowHardwareCursor = false;
		public static bool ClipMouseCursor = true;
		
		public static string DataDirectory = "../data";
		public static string DefaultFont = "/ui/fonts/arial-12.xml";
		public static string ConfigFile = "/gameconf.xml";
		public static string UIDefaultsFile = "/ui/defaults.xml";
		
		public static string InputDriver = "NativeInput";
		public static string GraphicsDriver = "OpenGL";
		public static string SFXDriver = "OpenAL";
		public static string SoundRecordingDriver = "OpenAL";
		public static string MusicDriver = "NativeMultimedia";
		public static string VideoDriver = "NativeMultimedia";
		public static string CameraDriver = "NativeCamera";
		
		// internal unconfigurable variables

		// custom variables go into the hashtable
		private static Dictionary<string, string> m_CustomVars = new Dictionary<string, string>();
		public static Dictionary<string, string> CustomVars { get { return m_CustomVars; } }

		// events
		public delegate void VSyncChangedEventHandler(bool vsyncValue);
		public static event VSyncChangedEventHandler VSyncChangedEvent;
		
		public static bool VSync
		{
			get { return m_VSyncEnabled; }
			set {
				if( m_VSyncEnabled == value )
					return;
				m_VSyncEnabled = value;
				if( VSyncChangedEvent != null )
					VSyncChangedEvent(m_VSyncEnabled);
			}
		}

		public static bool FullScreen
		{
			get { return m_FullScreen; }
			set { m_FullScreen = value; }
		}
		
		public static bool AutoSwapBuffers
		{
			get { return m_AutoSwapBuffers; }
			set { m_AutoSwapBuffers = value; }
		}
		
		public static string GetCustomValue(string id, string defval) {
			if( m_CustomVars.ContainsKey(id) )
				return m_CustomVars[id];
			m_CustomVars.Add(id, defval);
			return defval;
		}
		
		public static int GetCustomValue(string id, int defval) {
			if( m_CustomVars.ContainsKey(id) )
				return m_CustomVars[id].ToInt32(defval);
			m_CustomVars.Add(id, defval.ToString());
			return defval;
		}
		
		public static float GetCustomValue(string id, float defval) {
			if( m_CustomVars.ContainsKey(id) )
				return m_CustomVars[id].ToSingle(defval);
			m_CustomVars.Add(id, defval.ToString());
			return defval;
		}
		
		public static double GetCustomValue(string id, double defval) {
			if( m_CustomVars.ContainsKey(id) )
				return m_CustomVars[id].ToDouble(defval);
			m_CustomVars.Add(id, defval.ToString());
			return defval;
		}
		
		public static void Load() {
			GameDebugger.EngineLog(LogLevel.Debug, "Loading configuration from '{0}'", ConfigFile.ToPath());
			try {
				m_CustomVars.Clear();
				
				using(StructuredTextFile xml = GameFile.LoadFile<StructuredTextFile>(ConfigFile.ToPath())) {
					foreach(DomNode node in xml.Root) {
						switch (node.Name.ToLower()) {
								case "resolutionx": ResX = node.Value.ToInt32(-1); GameDebugger.EngineLog(LogLevel.Debug, "Config X resolution: {0}", ResX); break;
								case "resolutiony": ResY = node.Value.ToInt32(-1); GameDebugger.EngineLog(LogLevel.Debug, "Config Y resolution: {0}", ResY); break;
								case "bitsperpixel": BitsPerPixel = node.Value.ToInt32(-1); GameDebugger.EngineLog(LogLevel.Debug, "Config BitsPerPixel: {0}", BitsPerPixel); break;
								case "refreshrate": RefreshRate = node.Value.ToInt32(-1); GameDebugger.EngineLog(LogLevel.Debug, "Config RefreshRate: {0}", RefreshRate);break;
								case "displaydevice": {
									int deviceIndex = node.Value.ToInt32(-1);
									if( deviceIndex == 0 || node.Value.Equals("") && node.Value.ToLower().Equals("primary") )
										DisplayDevice = null;
									else if( deviceIndex < 0 ) // could not parse the index
										DisplayDevice = (node.Value.Length > 32) ? node.Value.Substring(0, 32) : node.Value;
									else
										DisplayDevice = String.Format(@"\\.\DISPLAY{0}", deviceIndex);
									GameDebugger.EngineLog(LogLevel.Debug, "Config DisplayDevice: {0}", DisplayDevice);
									break;
								}
								
								default: {
									if( m_CustomVars.ContainsKey(node.Name) ) {
										GameDebugger.EngineLog(LogLevel.Notice, "Config has more than one custom var node '{0}'. The variable value is overwritten by latest: '{1}'", node.Name, node.Value);
										m_CustomVars[node.Name] = node.Value;
									}
									else {
										GameDebugger.EngineLog(LogLevel.Debug, "Config custom var '{0}': '{1}'", node.Name, node.Value);
										m_CustomVars.Add(node.Name, node.Value);
									}
									break;
								}
						}
					}
				}				
			}
			catch {
			}
		}

		public static void Save() {
			GameDebugger.EngineLog(LogLevel.Debug, "Saving configuration to '{0}'", ConfigFile.ToPath());
			using(TextWriter file = new StreamWriter(ConfigFile.ToPath(), false)) {
				file.WriteLine("<GameConfig>");
				file.WriteLine("\t<DisplayDevice>{0}</DisplayDevice>", ((DisplayDevice == null) ? "primary" : DisplayDevice.RegReplace(@"^\\\\\.\\DISPLAY([0-9]+)$", "$1")));
				file.WriteLine("\t<ResolutionX>{0}</ResolutionX>", ((ResX <= 0) ? "desktop" : ResX.ToString()) );
				file.WriteLine("\t<ResolutionY>{0}</ResolutionY>", ((ResY <= 0) ? "desktop" : ResY.ToString()));
				file.WriteLine("\t<BitsPerPixel>{0}</BitsPerPixel>", ((BitsPerPixel <= 0) ? "desktop" : BitsPerPixel.ToString()));
				file.WriteLine("\t<RefreshRate>{0}</RefreshRate>", ((RefreshRate <= 0) ? "desktop" : RefreshRate.ToString()));
				foreach (KeyValuePair<string, string> pair in m_CustomVars)
					file.WriteLine("\t<{0}><![CDATA[{1}]]></{0}>", pair.Key, pair.Value);
				file.WriteLine("</GameConfig>");
			}
		}
		
		/// <summary>
		/// String class extension to convert strings from path relative to data directory into path relative to bin directory.
		/// For instance when GameConfig.DataDirectory equals to "../data", then "ui/defaults.xml".ToPath() would result in "../data/ui/defaults.xml"
		/// </summary>
		/// <param name="relativePath"></param>
		/// <returns></returns>
		public static string ToPath(this string relativePath) {
			return DataDirectory.Replace('\\', '/').TrimEnd('/') + "/" + relativePath.Replace('\\', '/').TrimStart('/');
		}
		
		/// <summary>
		/// String class extension to convert strings from path relative to a specified file or directory into a single path string.
		/// Samples:
		/// "../tex/wood.png".RelativeTo("../data/models/log.xml") would result in "../data/models/../tex/wood.png" or "..\data\models\../tex/wood.png" in windows
		/// "../tex/wood.png".RelativeTo("../data/models/") would result in "../data/models/../tex/wood.png"
		/// "../tex/wood.png".RelativeTo("../data/models") would result in "../data/../tex/wood.png"
		/// </summary>
		/// <param name="relativePath"></param>
		/// <param name="basePath"></param>
		/// <returns></returns>
		public static string RelativeTo(this string relativePath, string basePath) {
			try {
				string baseDir = Path.GetDirectoryName(basePath);
				if( baseDir == null || baseDir.Equals("") )
					return relativePath;
				return Path.Combine(baseDir, relativePath).Replace('\\', '/');
			}
			catch {
				return relativePath;
			}
		}
		
		public static string ToAbsolutePath(this string relativePath) {
			return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativePath));
			// return Path.GetFullPath((new Uri(relativePath)).LocalPath);
		}
	}
}
