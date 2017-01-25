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
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using System.Xml;


namespace XenAdmin.CustomFields
{
    /// <summary>
    /// Provide custom fields management support for VMs.  The master list of custom fields will be
    /// maintained in the pool class using the same conventions as the tags implementation (see
    /// XenAdmin.XenSearch.Tags).  When persisting the label-value pairs in the VMs, the
    /// following key/value convention will be used:
    ///     "XenCenter.CustomFields.foo1" value
    ///     "XenCenter.CustomFields.foo2" value
    /// </summary>
    public class CustomFieldsManager
    {
        #region These functions deal with caching the list of custom fields

        private static readonly CustomFieldsCache customFieldsCache = new CustomFieldsCache();
        private const String CUSTOM_FIELD_DELIM = ".";
        public const String CUSTOM_FIELD_BASE_KEY = "XenCenter.CustomFields";

        public const String CUSTOM_FIELD = "CustomField:";

        public static event Action CustomFieldsChanged;

        static CustomFieldsManager()
        {
            OtherConfigAndTagsWatcher.GuiConfigChanged += OtherConfigAndTagsWatcher_GuiConfigChanged;
        }

        private static void OtherConfigAndTagsWatcher_GuiConfigChanged()
        {
            InvokeHelper.AssertOnEventThread();

            customFieldsCache.RecalculateCustomFields();
            OnCustomFieldsChanged();
        }

        private static void OnCustomFieldsChanged()
        {
            Action handler = CustomFieldsChanged;

            if (handler != null)
            {
                handler();
            }
        }

        #endregion

        #region These functions deal with custom field definitions on the pool object

        public static List<CustomFieldDefinition> GetCustomFields()
        {
            return customFieldsCache.GetCustomFields();
        }

        public static List<CustomFieldDefinition> GetCustomFields(IXenConnection connection)
        {
            return customFieldsCache.GetCustomFields(connection);
        }

        /// <returns>The CustomFieldDefinition with the given name, or null if none is found.</returns>
        public static CustomFieldDefinition GetCustomFieldDefinition(string name)
        {
            foreach (CustomFieldDefinition d in GetCustomFields())
            {
                if (d.Name == name)
                    return d;
            }
            return null;
        }

        public static void RemoveCustomField(Session session, IXenConnection connection, CustomFieldDefinition definition)
        {
            List<CustomFieldDefinition> customFields = customFieldsCache.GetCustomFields(connection);
            if (customFields.Remove(definition))
            {
                SaveCustomFields(session, connection, customFields);

                // Remove from all Objects
                RemoveCustomFieldsFrom(session, connection.Cache.VMs, definition);
                RemoveCustomFieldsFrom(session, connection.Cache.Hosts, definition);
                RemoveCustomFieldsFrom(session, connection.Cache.Pools, definition);
                RemoveCustomFieldsFrom(session, connection.Cache.SRs, definition);
            }
        }

        public static void AddCustomField(Session session, IXenConnection connection, CustomFieldDefinition customField)
        {
            List<CustomFieldDefinition> customFields = customFieldsCache.GetCustomFields(connection);
            if (!customFields.Contains(customField))
            {
                customFields.Add(customField);
                SaveCustomFields(session, connection, customFields);
            }
        }

        private static String GetCustomFieldDefinitionXML(List<CustomFieldDefinition> customFieldDefinitions)
        {
            XmlDocument doc = new XmlDocument();

            XmlNode parentNode = doc.CreateElement("CustomFieldDefinitions");
            doc.AppendChild(parentNode);

            foreach (CustomFieldDefinition customFieldDefinition in customFieldDefinitions)
            {
                parentNode.AppendChild(customFieldDefinition.ToXmlNode(doc));
            }

            return doc.OuterXml;
        }

        #endregion

        #region These functions deal with the custom fields themselves

        public static string GetCustomFieldKey(CustomFieldDefinition customFieldDefinition)
        {
            return CUSTOM_FIELD_BASE_KEY + CUSTOM_FIELD_DELIM + customFieldDefinition.Name;
        }

        private static void RemoveCustomFieldsFrom(Session session, IEnumerable<IXenObject> os, CustomFieldDefinition customFieldDefinition)
        {
            InvokeHelper.AssertOffEventThread();

            string customFieldKey = GetCustomFieldKey(customFieldDefinition);

            foreach (IXenObject o in os)
            {
                Helpers.RemoveFromOtherConfig(session, o, customFieldKey);
            }
        }

        private static void SaveCustomFields(Session session, IXenConnection connection, List<CustomFieldDefinition> customFields)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool != null)
            {
                String customFieldXML = GetCustomFieldDefinitionXML(customFields);
                Helpers.SetGuiConfig(session, pool, CUSTOM_FIELD_BASE_KEY, customFieldXML);
            }
        }

        public static List<CustomField> CustomFieldValues(IXenObject o)
        {
            //Program.AssertOnEventThread();

            List<CustomField> customFields = new List<CustomField>();
            Dictionary<String, String> otherConfig = GetOtherConfigCopy(o);

            if (otherConfig != null)
            {
                foreach (CustomFieldDefinition customFieldDefinition in customFieldsCache.GetCustomFields(o.Connection))
                {
                    string customFieldKey = GetCustomFieldKey(customFieldDefinition);
                    if (!otherConfig.ContainsKey(customFieldKey) || otherConfig[customFieldKey] == String.Empty)
                    {
                        continue;
                    }

                    object value = ParseValue(customFieldDefinition.Type, otherConfig[customFieldKey]);
                    if (value != null)
                    {
                        customFields.Add(new CustomField(customFieldDefinition, value));
                    }
                }
            }

            return customFields;
        }

        // The same as CustomFieldValues(), but with each custom field unwound into an array
        public static List<object[]> CustomFieldArrays(IXenObject o)
        {
            List<object[]> ans = new List<object[]>();
            foreach (CustomField cf in CustomFieldValues(o))
            {
                ans.Add(cf.ToArray());
            }

            return ans;
        }

        // Whether the object has any custom fields defined
        public static bool HasCustomFields(IXenObject o)
        {
            Dictionary<String, String> otherConfig = GetOtherConfigCopy(o);
            if (otherConfig != null)
            {
                foreach (CustomFieldDefinition customFieldDefinition in GetCustomFields(o.Connection))
                {
                    string customFieldKey = GetCustomFieldKey(customFieldDefinition);
                    if (otherConfig.ContainsKey(customFieldKey) && otherConfig[customFieldKey] != String.Empty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static Object GetCustomFieldValue(IXenObject o, CustomFieldDefinition customFieldDefinition)
        {
            Dictionary<String, String> otherConfig = GetOtherConfigCopy(o);
            if (otherConfig == null)
                return null;

            String key = GetCustomFieldKey(customFieldDefinition);
            if (!otherConfig.ContainsKey(key))
                return null;

            String value = otherConfig[key];
            if (value == String.Empty)
                return null;

            return ParseValue(customFieldDefinition.Type, value);
        }

        private static object ParseValue(CustomFieldDefinition.Types type, string value)
        {
            switch (type)
            {
                case CustomFieldDefinition.Types.Date:
                    DateTime datetime;
                    if (DateTime.TryParse(value, out datetime))
                        return datetime;
                    return null;

                case CustomFieldDefinition.Types.String:
                    return value;

                default:
                    return null;
            }
        }

        private static Dictionary<string, string> GetOtherConfigCopy(IXenObject o)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            InvokeHelper.Invoke(delegate()
                                    {
                                        Dictionary<String, String> otherConfig = Helpers.GetOtherConfig(o);

                                        if (otherConfig == null)
                                        {
                                            output = null;
                                        }
                                        else
                                        {
                                            output = new Dictionary<string, string>(otherConfig);
                                        }
                                    });
            return output;

        }

        #endregion
    }
}