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
	public interface INativeWindow : IDisposable {
		event CloseEventHandler CloseEvent;
		event MoveEventHandler MoveEvent;
		event ResizeEventHandler ResizeEvent;
		event SizingAndMovingStateChangeEventHandler EnterSizeMoveEvent;
		event SizingAndMovingStateChangeEventHandler ExitSizeMoveEvent;
		
		int Width { get; }
		int Height { get; }
		
		bool Disposed { get; }
		bool Exists { get; }
		
		void Show();
		void Hide();
		
		/// <summary>
		/// Closes the window.
		/// </summary>
		void Close();
		
		/// <summary>
		/// Returns a rectangle occupied by window.
		/// </summary>
		/// <returns>Rectangle of window</returns>
		Rectangle GetRect();
		
		/// <summary>
		/// Gives client area rectangle. Be aware that Left and Top of returned rectangle is always 0.
		/// Use API.ClientToScreen() if you want to get position of client area on the screen.
		/// </summary>
		/// <returns>Rectangle of client area</returns>
		Rectangle GetClientRect();
		
		/// <summary>
		/// Gets requested by Application.AskWindowsIfCanQuit() when application tries to close.
		/// </summary>
		/// <returns>Should return true if window has nothing against closing the application</returns>
		bool CanQuit();
		
		/// <summary>
		/// Called when user is trying to close the window.
		/// </summary>
		bool CanClose();
	}
}
