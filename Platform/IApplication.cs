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

namespace IGE.Platform {
	/// <summary>
	/// </summary>
	public interface IApplication {
		// Three idle events are executed consecutively: PreIdle, Idle and then PostIdle.
		// PreIdle is usually used by mouse/controller/pen/touch input devices to update their states between rendered frames right before rendering next frame.
		// Idle event is usually used by graphics module to swap rendering buffers.
		// PostIdle event is usually used by keyboard input devices to update their states between rendered frames right after rendering last frame.
		event IdleEventHandler PreIdleEvent;
		event IdleEventHandler IdleEvent;
		event IdleEventHandler PostIdleEvent;
		
		event ActivateAppEventHandler ActivateAppEvent;
		event ActivateAppEventHandler DeactivateAppEvent;

		bool KeepAliveWithoutWindows { get; set; }
		bool Exits { get; }
		bool Active { get; }
		INativeWindow MainWindow { get; }
		
		void Run();
		bool DoLoop();
		void Exit(int exitCode);
		bool AskWindowsIfCanQuit();
	}
}
