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

using IGE;

namespace IGE.Platform {
	/// <summary>
	/// </summary>
	public interface IApiDriver : IDriver {
		IApplication GetApplication();
		
		void RuntimeImport(Type type, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param);
		void RuntimeImport(object instance, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param);
		
		INativeWindow CreateWindow(INativeWindow parent, int x, int y, int width, int height);
		
		IDisplayDevice GetDisplayDevice(string id);
		void ResetDisplayMode();
		
		Size2 AdjustWindowSize(int clientAreaWidth, int clientAreaHeight);
	}
	
	public delegate IntPtr GetProcAddressDelegate(string lpszProc, object param);
}
