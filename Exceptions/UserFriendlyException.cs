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
using System.Runtime.Serialization;

namespace IGE {
	/// <summary>
	/// </summary>
	public class UserFriendlyException : Exception, System.Runtime.Serialization.ISerializable {
		protected string m_UserFriendlyMessage;
		public string UserFriendlyMessage { get { return m_UserFriendlyMessage; } }
	
		public UserFriendlyException() {
			m_UserFriendlyMessage = "";
		}

	 	public UserFriendlyException(string message) : this(message, message) {
		}
		
	 	public UserFriendlyException(string message, string userFriendlyMessage) : base(message) {
			m_UserFriendlyMessage = userFriendlyMessage;
		}

		public UserFriendlyException(string message, string userFriendlyMessage, Exception innerException) : base(message, innerException) {
			m_UserFriendlyMessage = userFriendlyMessage;
		}

		// This constructor is needed for serialization.
		protected UserFriendlyException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}