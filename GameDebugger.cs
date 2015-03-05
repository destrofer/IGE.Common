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
using System.Threading;
using System.Collections.Generic;

namespace IGE {
	public static class GameDebugger {
		public static string EngineLogFile = "ige.log";
		public static string GameLogFile = "game.log";
		public static string NetLogFile = "network.log";
		
		#if DEBUG
		public static LogLevel MinLogLevel = LogLevel.Debug;
		#else
		public static LogLevel MinLogLevel = LogLevel.Error;
		#endif

		public static LogLevel MinNetLogLevel = LogLevel.LogNothing; // this must be set explicitly, because network log gets huge very fast.
		
		public static bool OutputToConsole = false;
		public static bool PersistentLogFile = false;
		
		private static StreamWriter[] LogFiles = new StreamWriter[3];
		
		public static object DebugValue = null;
		
		private static void LogToFile(int logFile, string text) {
			try {
				lock(LogFiles) {
					if( OutputToConsole )
						Console.WriteLine(text);
					if( PersistentLogFile ) {
						if( LogFiles[logFile] == null )
							LogFiles[logFile] = new StreamWriter((logFile == 0) ? EngineLogFile : ((logFile == 2) ? NetLogFile : GameLogFile), true);
						
						// using(StreamWriter file = new StreamWriter(fileName, true)) { file.WriteLine(text); }
						try {
							LogFiles[logFile].WriteLine(text);
							LogFiles[logFile].Flush();
						}
						catch {}
					}
					else {
						try {
							using(StreamWriter w = new StreamWriter((logFile == 0) ? EngineLogFile : ((logFile == 2) ? NetLogFile : GameLogFile), true)) {
								w.WriteLine(text);
							}
						}
						catch {}
					}
				}
			}
			catch(ThreadInterruptedException) {
				// damn it! thread was interrupted while trying to write to the file.
				// log entry will be lost unless we do something
				// TODO: make something so that log entry would not get lost. maybe use multithreading? :/
			}
		}

		private static void LogException(int logType, Exception ex, int level) {
			if( level == 0 )
				LogToFile(logType, String.Format("=== {0} has thrown an exception ===", (logType == 0) ? "IGE" : "Application"));
			else
				LogToFile(logType, "=== Inner exception ===");
			LogToFile(logType, String.Format("{2} Message: \n{0}\n\nStackTrace: \n{1}", ex.Message, ex.StackTrace, ex.GetType().FullName));
			if( ex.InnerException != null )
				LogException(logType, ex.InnerException, level + 1);
			if( level == 0 )
				LogToFile(logType, "=== End of exception ===");
		}
		
		public static void EngineLog(LogLevel level, string text) {
			if( level < MinLogLevel )
				return;
			LogToFile(0, text);
		}
		public static void EngineLog(LogLevel level, string format, params object[] args) {
			if( level < MinLogLevel )
				return;
			LogToFile(0, String.Format(format, args));
		}
		public static void EngineLog(LogLevel level, object obj) {
			if( level < MinLogLevel )
				return;
			if( obj is Exception )
				LogException(0, (Exception)obj, 0);
			else
				LogToFile(0, obj.ToString());
		}
		
		public static void Log(LogLevel level, string text) {
			if( level < MinLogLevel )
				return;
			LogToFile(1, text);
		}
		public static void Log(LogLevel level, string format, params object[] args) {
			if( level < MinLogLevel )
				return;
			LogToFile(1, String.Format(format, args));
		}
		public static void Log(LogLevel level, object obj) {
			if( level < MinLogLevel )
				return;
			if( obj is Exception )
				LogException(1, (Exception)obj, 0);
			else
				LogToFile(1, obj.ToString());
		}
		
		
		public static void NetLog(LogLevel level, string text) {
			if( level < MinNetLogLevel )
				return;
			LogToFile(2, text);
		}
		public static void NetLog(LogLevel level, string format, params object[] args) {
			if( level < MinNetLogLevel )
				return;
			LogToFile(2, String.Format(format, args));
		}
		public static void NetLog(LogLevel level, object obj) {
			if( level < MinNetLogLevel )
				return;
			if( obj is Exception )
				LogException(2, (Exception)obj, 0);
			else
				LogToFile(2, obj.ToString());
		}
		
		public static void Trace(string format, params object[] args) {
			#if TRACE
			Console.WriteLine(format, args);
			#endif
		}
		
		public static void Add(object obj) {
			if( DebugValue == null )
				DebugValue = new List<object>();
			if( DebugValue is List<object> )
				((List<object>)DebugValue).Add(obj);
			else
				DebugValue = obj;				
		}
		
		public static void Clear() {
			if( DebugValue is List<object> )
				((List<object>)DebugValue).Clear();
			else
				DebugValue = null;
		}
	}
	
	public enum LogLevel : byte {
		VeryVerboseDebug, // used to write all raw data sent over the network
		VerboseDebug, // used to debug network packet contents
		Debug,
		Info,
		Notice,
		Warning,
		Error,
		LogNothing,
	}
}
