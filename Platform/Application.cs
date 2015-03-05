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
	public static class Application {
		public static event IdleEventHandler PreIdleEvent;
		public static event IdleEventHandler IdleEvent;
		public static event IdleEventHandler PostIdleEvent;
		
		public static event ActivateAppEventHandler ActivateAppEvent;
		public static event ActivateAppEventHandler DeactivateAppEvent;

		private static IApplication Instance;
		
		public static bool KeepAliveWithoutWindows {
			get { return Instance.KeepAliveWithoutWindows; }
			set { Instance.KeepAliveWithoutWindows = value; }
		}
		
		public static bool Exits { get { return Instance.Exits; } }
		public static bool Active { get { return Instance.Active; } }

		public static INativeWindow MainWindow { get { return Instance.MainWindow; } }
		
		/// <summary>
		/// Global game timer, that could be paused/continued to control the game time.
		/// </summary>
		public static GameTimer Timer { get { return m_Timer; } }
		private static GameTimer m_Timer = null;

		/// <summary>
		/// Engine internal timer. Starts off when the engine loads and never stops afterwards.
		/// </summary>
		// public static double InternalTime { get { return m_InternalTimer.Time; } }
		public static GameTimer InternalTimer { get { return m_InternalTimer; } }
		private static GameTimer m_InternalTimer = null;
		
		static Application() {
			Instance = API.GetApplication();
			Instance.PreIdleEvent += OnPreIdle;
			Instance.IdleEvent += OnIdle;
			Instance.PostIdleEvent += OnPostIdle;
			Instance.ActivateAppEvent += OnActivate;
			Instance.DeactivateAppEvent += OnDeactivate;
			
			m_Timer = new GameTimer();
			m_InternalTimer = new GameTimer();
		}
		
		private static void OnPreIdle() {
			m_Timer.OnFrame();
			m_InternalTimer.OnFrame();
			if( PreIdleEvent != null )
				PreIdleEvent();
		}

		private static void OnIdle() {
			if( IdleEvent != null )
				IdleEvent();
		}

		private static void OnPostIdle() {
			if( PostIdleEvent != null )
				PostIdleEvent();
		}
		
		private static void OnActivate() {
			if( ActivateAppEvent != null )
				ActivateAppEvent();
		}
		
		private static void OnDeactivate() {
			if( DeactivateAppEvent != null )
				DeactivateAppEvent();
		}
		
		public static bool DoLoop() {
			return Instance.DoLoop();
		}
		
		public static void Run() {
			Instance.Run();
		}
		
		public static void Exit() {
			Exit(0);
		}
		
		public static void Exit(int exitCode) {
			Instance.Exit(exitCode);
		}
		
		public static bool AskWindowsIfCanQuit() {
			return Instance.AskWindowsIfCanQuit();
		}
	}
}
