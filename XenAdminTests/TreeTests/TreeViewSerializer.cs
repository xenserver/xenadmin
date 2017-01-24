/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Xml;
using System.Windows.Forms;
using System.Text;
using System.IO;
using XenAdmin.Controls;
using XenAdmin;

namespace XenAdminTests.TreeTests
{
	/// <summary>
    /// A class used for serializing and deserializing treeviews. Currently used in MainWindowTreeBuilderTests.
	/// </summary>
	internal class TreeViewSerializer
	{
		// Xml tag for node, e.g. 'node' in case of <node></node>
		private const string XmlNodeTag = "node";
		
		// Xml attributes for node e.g. <node text="Asia" tag="" imageindex="1"></node>
		private const string XmlNodeTextAtt = "text";
		private const string XmlNodeTagAtt = "tag";
        private const string XmlNodeImageIndexAtt = "imageindex";

		public string SerializeTreeView(VirtualTreeView treeView) 
		{
            Util.ThrowIfParameterNull(treeView, "treeView");

            StringWriter sw = new StringWriter();

			XmlTextWriter textWriter = new XmlTextWriter(sw);
			// writing the xml declaration tag
			textWriter.WriteStartDocument();
			//textWriter.WriteRaw("\r\n");
			// writing the main tag that encloses all node tags
			textWriter.WriteStartElement("VirtualTreeView");
			
			// save the nodes, recursive method
			SaveNodes(treeView.Nodes, textWriter);
			
			textWriter.WriteEndElement();
					
			textWriter.Close();

            return sw.ToString();
		}

		private void SaveNodes(VirtualTreeView.VirtualTreeNodeCollection nodesCollection, XmlTextWriter textWriter)
		{
			for(int i = 0; i < nodesCollection.Count; i++)
			{
				VirtualTreeNode node = nodesCollection[i];
				textWriter.WriteStartElement(XmlNodeTag);
				textWriter.WriteAttributeString(XmlNodeTextAtt, node.Text);
                textWriter.WriteAttributeString(XmlNodeImageIndexAtt, ((Icons)(node.ImageIndex)).ToString());
                if (node.Tag != null) 
					textWriter.WriteAttributeString(XmlNodeTagAtt, node.Tag.ToString());
				
				// add other node properties to serialize here
				
				if (node.Nodes.Count > 0)
				{
					
					SaveNodes(node.Nodes, textWriter);
					    
				} 				
				textWriter.WriteEndElement();
			}
		}		

		public void DeserializeTreeView(VirtualTreeView treeView, string fileName)
		{
            Util.ThrowIfParameterNull(treeView, "treeView");
            Util.ThrowIfStringParameterNullOrEmpty(fileName, "fileName");

			XmlTextReader reader = null;
			try
			{
                // disabling re-drawing of treeview till all nodes are added
				treeView.BeginUpdate();				
				reader =
					new XmlTextReader(fileName);

				VirtualTreeNode parentNode = null;
				
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{						
						if (reader.Name == XmlNodeTag)
						{
							VirtualTreeNode newNode = new VirtualTreeNode("");
							bool isEmptyElement = reader.IsEmptyElement;

                            // loading node attributes
							int attributeCount = reader.AttributeCount;
							if (attributeCount > 0)
							{
								for (int i = 0; i < attributeCount; i++)
								{
									reader.MoveToAttribute(i);
									
									SetAttributeValue(newNode, reader.Name, reader.Value);
								}								
							}

                            // add new node to Parent Node or VirtualTreeView
                            if(parentNode != null)
                                parentNode.Nodes.Add(newNode);
                            else
                                treeView.Nodes.Add(newNode);

                            // making current node 'ParentNode' if its not empty
							if (!isEmptyElement)
							{
                                parentNode = newNode;
							}
														
						}						                    
					}
                    // moving up to in VirtualTreeView if end tag is encountered
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (reader.Name == XmlNodeTag)
						{
							parentNode = parentNode.Parent;
						}
					}
					else if (reader.NodeType == XmlNodeType.XmlDeclaration)
					{ //Ignore Xml Declaration                    
					}
					else if (reader.NodeType == XmlNodeType.None)
					{
						return;
					}
					else if (reader.NodeType == XmlNodeType.Text)
					{
						parentNode.Nodes.Add(new VirtualTreeNode(reader.Value));
					}

				}
			}
			finally
			{
                // enabling redrawing of treeview after all nodes are added
				treeView.EndUpdate();      
                reader.Close();	
			}
		}

		/// <summary>
		/// Used by Deserialize method for setting properties of VirtualTreeNode from xml node attributes
		/// </summary>
		/// <param name="node"></param>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		private void SetAttributeValue(VirtualTreeNode node, string propertyName, string value)
		{
			if (propertyName == XmlNodeTextAtt)
			{                
				node.Text = value;				
			}
            else if (propertyName == XmlNodeImageIndexAtt)
            {
                Icons icon = (Icons)Enum.Parse(typeof(Icons), value);

                node.ImageIndex = (int)icon;
            }
            else if (propertyName == XmlNodeTagAtt)
			{
				node.Tag = value;
			}		
		}
	}
}
