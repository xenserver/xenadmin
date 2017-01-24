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
using XenAdmin.XenSearch;


namespace XenAdmin.CustomFields
{
    public class CustomFieldDefinition
    {
        public const String TAG_NAME = "CustomFieldDefinition";

        public enum Types { String, Date };

        private readonly String name;
        private readonly Types type;

        public CustomFieldDefinition(string name, Types type)
        {
            this.name = name;
            this.type = type;
        }

        public Types Type
        {
            get
            {
                return type;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public override string ToString()
        {
            String typeString = type == Types.Date ? Messages.DATE_AND_TIME : Messages.TEXT;

            return String.Format(Messages.CUSTOM_FIELD_NAME_AND_TYPE, name, typeString);
        }

        public override bool Equals(object obj)
        {
            CustomFieldDefinition other = obj as CustomFieldDefinition;

            return other != null && other.name == name && other.type == type;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public CustomFieldDefinition(XmlNode node)
        {
            this.name = node.Attributes["name"].Value;
            this.type = (Types)Enum.Parse(typeof(Types), node.Attributes["type"].Value);
        }

        public XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(TAG_NAME);

            SearchMarshalling.AddAttribute(doc, node, "name", name);
            SearchMarshalling.AddAttribute(doc, node, "type", type.ToString());
            SearchMarshalling.AddAttribute(doc, node, "defaultValue", "");  // always unused, but needed until George is retired because of CA-37473

            return node;
        }
    }
}