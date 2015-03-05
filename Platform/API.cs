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
	public static partial class API {
		public const string ApiDriverName = "API";
		public static readonly Version ExpectedDriverVersion = new Version(9, 0, 0);
		
		/// <summary>
		/// We are leaving this public for those crazy people, who no matter
		/// what want to have low level access to native functions.
		/// </summary>
		public static IApiDriver Driver = null;
		
		static API() {
			Driver = DriverManager.LoadDriver<IApiDriver>(ApiDriverName, ExpectedDriverVersion);
			if( Driver == null )
				throw new UserFriendlyException(String.Format("Failed to load platform specific API driver compatible with required version {0}", ExpectedDriverVersion));
			Driver.Initialize();
		}
		
		public static IApplication GetApplication() {
			return Driver.GetApplication();
		}
		
		public static INativeWindow CreateWindow(INativeWindow parent, int x, int y, int width, int height) {
			return Driver.CreateWindow(parent, x, y, width, height);
		}
		
		public static void ResetDisplayMode() {
			Driver.ResetDisplayMode();
		}
		
		public static IDisplayDevice GetDisplayDevice(string id) {
			return Driver.GetDisplayDevice(id);
		}
		
		public static Size2 AdjustWindowSize(int clientAreaWidth, int clientAreaHeight) {
			return Driver.AdjustWindowSize(clientAreaWidth, clientAreaHeight);
		}
		
		public static void RuntimeImport(Type type, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param) {
			Driver.RuntimeImport(type, proc_address_get_func, proc_address_get_func_param);
		}
		
		public static void RuntimeImport(object instance, GetProcAddressDelegate proc_address_get_func, object proc_address_get_func_param) {
			Driver.RuntimeImport(instance, proc_address_get_func, proc_address_get_func_param);
		}
	}
}
