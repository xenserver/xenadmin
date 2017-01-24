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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Xml;

namespace XenOvf.Utilities
{
    /// <summary>
    /// provide data mapping from CIM XML definitions to objects for validation/conversion
    /// </summary>
    public static class ValueMaps
    {
        private static bool isLoaded = false;
        private static Dictionary<string, string> MapResourceType = new Dictionary<string, string>();
        private static Dictionary<string, string> MapConsumerVisibility = new Dictionary<string, string>();
        private static Dictionary<string, string> MapMappingBehavior = new Dictionary<string, string>();
        private static Dictionary<string, string> MapOperatingSystem = new Dictionary<string, string>();


        /// <summary>
        /// Find Resource Type VALUE via string  (reverse lookup)
        /// </summary>
        /// <param name="key">Resource Value</param>
        /// <returns>UInt16</returns>
        public static UInt16 ResourceType(string key)
        {
            return ResolveMapping(MapResourceType, key);
        }
        /// <summary>
        /// Find Resource Type via Value
        /// </summary>
        /// <param name="key">value of resource type.</param>
        /// <returns>string describing resourcetype</returns>
        public static string ResourceType(UInt16 key)
        {
            return ResolveMapping(MapResourceType, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static UInt16 ConsumerVisibility(string key)
        {
            return ResolveMapping(MapConsumerVisibility, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ConsumerVisibility(UInt16 key)
        {
            return ResolveMapping(MapConsumerVisibility, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static UInt16 MappingBehavior(string key)
        {
            return ResolveMapping(MapMappingBehavior, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string MappingBehavior(UInt16 key)
        {
            return ResolveMapping(MapMappingBehavior, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static UInt16 OperatingSystem(string key)
        {
            return ResolveMapping(MapOperatingSystem, key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string OperatingSystem(UInt16 key)
        {
            return ResolveMapping(MapOperatingSystem, key);
        }
        
        
        private static void Load()
        {
            if (!isLoaded)
            {
                string assemblypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string[] files = Properties.Settings.Default.mofFiles.Split(new char[] { ',' });

                string path1 = assemblypath;
                string path2 = Path.Combine(assemblypath, "Schemas"); 

                foreach (string file in files)
                {
                    string fileinuse = string.Empty;
                    if (File.Exists(Path.Combine(path1, file.Trim())))
                    {
                        fileinuse = Path.Combine(assemblypath, file.Trim());
                    }
                    else if (File.Exists(Path.Combine(path2, file.Trim())))
                    {
                        fileinuse = Path.Combine(path2, file.Trim());
                    }
                    else
                    {
                        continue;
                    }
                    string xmlstring = Utilities.Tools.LoadFile(fileinuse);

                    XmlDocument xd = new XmlDocument();
                    xd.LoadXml(xmlstring);

                    if (file.Trim().ToUpper().Equals("CIM_OPERATINGSYSTEM.XML"))
                    {
                        LoadMap(xd, "OSTYPE", MapOperatingSystem);
                    }
                    else if (file.Trim().ToUpper().Equals("CIM_RESOURCEALLOCATIONSETTINGDATA.XML"))
                    {
                        LoadMap(xd, "RESOURCETYPE", MapResourceType);
                        LoadMap(xd, "MAPPINGBEHAVIOR", MapMappingBehavior);
                        LoadMap(xd, "CONSUMERVISIBILITY", MapConsumerVisibility);
                    }
                    isLoaded = true;

                }
            }
        }
        private static void LoadMap(XmlDocument xd, string fieldname, Dictionary<string,string> mapdictionary)
        {
            mapdictionary.Clear();
            XmlNodeList xmlOSTypeValuesMap = null;
            XmlNodeList xmlOFTypeStrings = null;
            XmlNodeList xnl = xd.SelectNodes("CIM/DECLARATION/DECLGROUP/VALUE.OBJECT/CLASS/PROPERTY");
            foreach (XmlNode xn in xnl)
            {
                foreach (XmlAttribute xa in xn.Attributes)
                {
                    if (xa.Name.ToUpper().Equals("NAME") && xa.Value.ToUpper().Equals(fieldname.ToUpper()))
                    {
                        foreach (XmlNode xn1 in xn.ChildNodes)
                        {
                            if (xn1.Name.ToUpper().Equals("QUALIFIER") && xn1.Attributes.GetNamedItem("NAME").Value.ToUpper().Equals("VALUEMAP"))
                            {
                                xmlOSTypeValuesMap = xn1.ChildNodes;
                            }
                            if (xn1.Name.ToUpper().Equals("QUALIFIER") && xn1.Attributes.GetNamedItem("NAME").Value.ToUpper().Equals("VALUES"))
                            {
                                xmlOFTypeStrings = xn1.ChildNodes;
                            }
                        }
                    }
                }
            }

            xmlOSTypeValuesMap = xmlOSTypeValuesMap[0].ChildNodes;
            xmlOFTypeStrings = xmlOFTypeStrings[0].ChildNodes;

            for (int i = 0; i < xmlOSTypeValuesMap.Count; i++)
            {
                mapdictionary.Add(xmlOSTypeValuesMap[i].InnerText, xmlOFTypeStrings[i].InnerText);
            }
        }

        private static UInt16 ResolveMapping(Dictionary<string, string> dictionary, string key)
        {
            if (!isLoaded) { Load(); }
            foreach (string _key in dictionary.Keys)
            {
                if (dictionary[_key].ToUpper().StartsWith(key.ToUpper()))
                {
                    return Convert.ToUInt16(_key);
                }
                else if (key.ToUpper().Contains(dictionary[_key].ToUpper()))
                {
                    return Convert.ToUInt16(_key);
                }
            }
            return 0xFFFF;
        }
        private static string ResolveMapping(Dictionary<string, string> dictionary, UInt16 key)
        {
            if (!isLoaded) { Load(); }
            if (dictionary.ContainsKey(Convert.ToString(key)))
            {
                return dictionary[Convert.ToString(key)];
            }
            return null;
        }
    }
}
