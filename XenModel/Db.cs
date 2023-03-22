/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin
{
    public class Db
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Db(XmlDocument document)
        {
            XmlNode dbNode = document.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "database") ??
                             throw new ArgumentException("Couldn't find database tag in state db");

            foreach (XmlNode n in dbNode.ChildNodes)
            {
                if (n.Name != "table")
                    continue;

                try
                {
                    Tables.Add(new DbTable(n));
                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            }
        }

        public List<DbTable> Tables { get; } = new List<DbTable>();
    }


    /// <summary>
    /// Corresponds to an API class type. Its Rows are the class instances (API objects)
    /// </summary>
    public class DbTable
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Type[] AssemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

        public DbTable(XmlNode tableNode)
        {
            var name = tableNode.Attributes?["name"].Value;

            ApiClassType = AssemblyTypes
                .Where(t => t.Namespace == nameof(XenAPI))
                .FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.CurrentCultureIgnoreCase));

            foreach (XmlNode n in tableNode.ChildNodes)
            {
                if (n.Name != "row")
                    continue;

                try
                {
                    Rows.Add(new Row(ApiClassType, n));
                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            }
        }

        public Type ApiClassType { get; }

        public List<Row> Rows { get; } = new List<Row>();
    }


    /// <summary>
    /// Corresponds to an API object
    /// </summary>
    public class Row
    {
        private readonly XmlNode _rowNode;

        public Row(Type apiClassType, XmlNode rowNode)
        {
            _rowNode = rowNode;

            HashTable = ConvertToHashtable(out var opaqueRef);
            XenObject = ConvertToXenObject(apiClassType);

            if (XenObject != null)
                XenObject.opaque_ref = opaqueRef;

            ObjectChange = new ObjectChange(apiClassType, opaqueRef, XenObject);
        }

        public Hashtable HashTable { get; }

        public IXenObject XenObject { get; }

        public ObjectChange ObjectChange { get; }

        private Hashtable ConvertToHashtable(out string opaqueRef)
        {
            opaqueRef = "OpaqueRef:NULL";

            if (_rowNode.Attributes == null)
                return null;

            var hashtable = new Hashtable();

            foreach (XmlAttribute attr in _rowNode.Attributes)
            {
                if (attr.Name.StartsWith("__"))
                    continue;

                if (attr.Name == "_ref")
                {
                    opaqueRef = attr.Value;
                    continue;
                }

                var key = attr.Name.Replace("__", "_");

                hashtable[key] = EscapeXapiChars(attr.Value);
            }

            return hashtable;
        }

        private IXenObject ConvertToXenObject(Type apiClassType)
        {
            return apiClassType?.GetConstructor(new[] { typeof(Hashtable) })?.Invoke(new object[] { HashTable }) as IXenObject;
        }

        private string EscapeXapiChars(string input)
        {
            input = input.Replace("%.", " ");
            input = input.Replace("%_", "  ");
            input = input.Replace("%n", "\n");
            input = input.Replace("%r", "\r");
            input = input.Replace("%t", "\t");
            input = input.Replace("%%", "%");
            return input;
        }
    }
}