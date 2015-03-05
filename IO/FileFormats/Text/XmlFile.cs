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
using System.Xml;

using IGE;

namespace IGE.IO {
	[FileFormat("xml")]
	public class XmlFile : StructuredTextFile {
		public XmlFile(Stream file) : base(file) {
			DomNode current = null;
			DomNode newNode;
			
			XmlTextReader reader = new XmlTextReader(file);
			XmlNodeType nodeType = XmlNodeType.EndElement;
			
			while( reader.Read() ) {
				nodeType = reader.NodeType;
				if( nodeType == XmlNodeType.EndElement ) {
					if( current == null )
						break;
					current = current.ParentNode;
				}
				else if( nodeType == XmlNodeType.XmlDeclaration || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.Comment ) {
					// just ignore comments and whitespaces
				}
				else if( nodeType == XmlNodeType.Text || nodeType == XmlNodeType.CDATA ) {
					current.Value += reader.Value;
				}
				else {
					newNode = new DomNode(reader.Name, reader.Value);
					if( current == null ) {
						if( Root != null )
							throw new Exception("XML file must not contain several root nodes.");
						Root = newNode;
					}
					else
						current.Add(newNode);
					current = newNode;
					
					if( reader.HasAttributes ) {
						while( reader.MoveToNextAttribute() )
							current.Add(new DomAttribute(reader.Name, reader.Value));
						reader.MoveToElement();
					}
					
					if( reader.IsStartElement() ) {
						if( reader.IsEmptyElement )
							current = current.ParentNode;
					}
				}
			}
		}
	}
}
