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

// ============================================================================
// Description:   Utilitiy functions built on top of libxen for use in all
//                providers.
// ============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using XenOvf.Definitions;


namespace XenOvf.Utilities
{
    /// <summary>
    /// Common TOOLS used within XenOvf.
    /// </summary>
    public sealed class Tools
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const long KB = 1024;
        private const long MB = (KB * 1024);
        private const long GB = (MB * 1024);

        private Tools() { }

        #region MISC TOOLS
        /// <summary>
        /// Load default namespaces required to define OVF.
        /// Can be overriden in app.config:  KnownNamespaces
        /// </summary>
        /// <returns>Collection: XmlSerializerNamespces</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static XmlSerializerNamespaces LoadNamespaces()
        {
            string[] namespaces = Properties.Settings.Default.KnownNamespaces.Split(new char[] { ',' });

            XmlSerializerNamespaces ns;
            ns = new XmlSerializerNamespaces();
            foreach (string name in namespaces)
            {
                string[] sep = name.Split(new char[] { '=' });
                ns.Add(sep[0].Trim(), sep[1].Trim());
            }
            return ns;
        }
        /// <summary>
        /// Load a file into a string
        /// </summary>
        /// <param name="filename">fullpath/filename</param>
        /// <returns>string containing file contents</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static string LoadFile(string filename)
        {
            FileStream fs = null;
            StreamReader sr = null;
            string xmldoc = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                sr = new StreamReader(fs);
                xmldoc = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Utilities.LoadFile: failed: {0}", ex.Message);
                throw;
            }
            finally
            {
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }
            return xmldoc;
        }
        #endregion

        #region SERIALIZATION TOOLS
        /// <summary>
        /// Create an object from a string given the type to deserialize as.
        /// </summary>
        /// <param name="xmlString">xml string content</param>
        /// <param name="objectType">Type object definition</param>
        /// <returns>an object of Type objectType.</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static object Deserialize(string xmlString, Type objectType)
        {
            object returnClass = null;

            MemoryStream ms = new MemoryStream();          
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(xmlString);
            sw.Flush();
            ms.Position = 0;            
            StreamReader sr = new StreamReader(ms, true);
            
            try
            {
                XmlSerializer xs = new XmlSerializer(objectType);
                returnClass = xs.Deserialize(sr);
            }
            catch (Exception ex)
            {
                log.DebugFormat("Tools.Deserialize failed attempt (may get retried) {0}", ex.Message);
                if (ex.InnerException != null && ex.InnerException.Message.ToLower().Contains("hexadecimal value 0x00"))
                {
                    throw new InvalidDataException(Messages.INVALID_DATA_IN_OVF, ex);
                }
            }
            finally
            {
                if (sw != null) sw.Close();
                if (ms != null) ms.Close();
            }
            return returnClass;
        }
        /// <summary>
        /// Transform an object into a xml string.
        /// </summary>
        /// <param name="tbs">object To-Be-Serialized</param>
        /// <param name="objectType">Object T value</param>
        /// <returns>xml string</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static string Serialize(object tbs, Type objectType)
        {
            return Tools.Serialize(tbs, objectType, null);
        }
        /// <summary>
        /// Transform an object into a xml string, adding in known namespaces.
        /// </summary>
        /// <param name="tbs">object To-Be-Serialized</param>
        /// <param name="objectType">Object T value</param>
        /// <param name="ns">Namespace definitions.</param>
        /// <returns>xml string</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static string Serialize(object tbs, Type objectType, XmlSerializerNamespaces ns)
        {
            XmlSerializer xs = new XmlSerializer(objectType);
            MemoryStream ms = new MemoryStream();
            XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode);
            StreamReader sr = null;
            string returnXml = null;
            try
            {
                if (ns != null)
                {
                    xs.Serialize(xtw, tbs, ns);
                }
                else
                {
                    xs.Serialize(xtw, tbs);
                }
                ms.Position = 0;
                sr = new StreamReader(ms, true);
                returnXml = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("OVF.Tools.Serialize failed {0}", ex.Message);
                throw new CtxUtilitiesException(ex.Message,ex);
            }
            finally
            {
                if (ms != null) ms.Close();
            }
            return returnXml;
        }
        #endregion

        #region OVA.XML TOOLS
        /// <summary>
        /// This deserializes the OVA.XML File.  The issue here is the form of the XML, 
        /// Though it is valid XML generic parsing tools cannot handle the same element name
        /// in embedded elements (value in this case, it is the 'root' plus a member of member)
        /// with out the use of name spaces, also the 'value' field can take on one of 3 different
        /// formats:
        ///     1. text (string)
        ///     2. struct
        ///     3. array
        /// Therefore the deserializer will need some help in the processing. The first pass will
        /// give the basic form up to the points of ambiquity, then we can take the xml nodes in their 
        /// current context and provide further deserialization.
        /// </summary>
        /// <param name="filename">pathto\\ova.xml</param>
        /// <returns>XenXva Data Structure</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        internal static XenXva LoadOvaXml(string filename)
        {
            return DeserializeOvaXml(LoadFile(filename));
        }
        /// <summary>
        /// Load an old style OVA.XML file (version 0.1)
        /// </summary>
        /// <param name="filename">fullpath/filename</param>
        /// <returns>object of type XcAppliance</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        internal static XcAppliance LoadOldOvaXml(string filename)
        {
            return LoadFileXml<XcAppliance>(filename);
        }
        /// <summary>
        /// Load an OVF xml File and convert to an EnvelopeType
        /// </summary>
        /// <param name="filename">fullpath/filename</param>
        /// <returns>object EnvelopeType or NULL</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        internal static EnvelopeType LoadOvfXml(string appliancePath)
        {
            string ovfFilePath = appliancePath;

            string extension = Path.GetExtension(appliancePath);

            if ((String.Compare(extension, ".gz", true) == 0) ||
                (String.Compare(extension, ".bz2", true) == 0) ||
                (String.Compare(extension, ".ova", true) == 0) ||
                (String.Compare(extension, ".tar", true) == 0))
            {
                // Extract the contents of the archive.
                OpenArchive(appliancePath);

                // Get the OVF name from within the archive because it may not share the base name of the archive.
                Package package = Package.Create(appliancePath);

                string ovfFileName = package.DescriptorFileName;

                if (string.IsNullOrEmpty(ovfFileName))
                {
                    // An OVF file was not found.
                    // Default to the base file name of the archive.
                    // Remove the last extension that could be .gz, .bz2, .ova, or .tar.
                    ovfFileName = Path.GetFileNameWithoutExtension(appliancePath);

                    extension = Path.GetFileNameWithoutExtension(ovfFileName);

                    if ((String.Compare(extension, ".ova", true) == 0) ||
                        (String.Compare(extension, ".tar", true) == 0))
                    {
                        // Remove the second to last extension that could be ..ova, or .tar.
                        ovfFileName = Path.GetFileNameWithoutExtension(ovfFileName);
                    }

                    // Add the .ovf extension.
                    ovfFileName = ovfFileName + Properties.Settings.Default.ovfFileExtension;
                }

                string directory = Path.GetDirectoryName(appliancePath);

                ovfFilePath = Path.Combine(directory, ovfFileName);
            }

            if (!File.Exists(ovfFilePath))
            {
                log.ErrorFormat("Utilities.LoadOvfXml: File not found: {0}", ovfFilePath);
                throw new FileNotFoundException(string.Format(Messages.FILE_MISSING, ovfFilePath));
            }

            return DeserializeOvfXml(LoadFile(ovfFilePath));
        }
        /// <summary>
        /// Give a fullpath/filename validate the OVF XML against DMTF v1.0 OVF Schema
        /// </summary>
        /// <param name="ovffilename">fullpath/filename</param>
        /// <returns>true: validates  false: failed validation</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static EnvelopeType DeserializeOvfXml(string ovfxml)
        {
            bool isVmware = false;
            EnvelopeType ovfEnv = null;
            try
            {
                ovfEnv = (EnvelopeType)Deserialize(ovfxml, typeof(EnvelopeType));
            }
            catch (Exception ex)
            {
                log.InfoFormat("Attempt reading xml failed. {0}", ex.Message);
            }

            if (ovfEnv != null && ovfEnv.AnyAttr != null)
            {
                foreach (XmlAttribute xa in ovfEnv.AnyAttr)
                {
                    if (xa.Prefix.ToLower().Equals(Properties.Settings.Default.vmwNamespacePrefix) ||
                        xa.NamespaceURI.Equals(Properties.Settings.Default.vmwNameSpace) ||
                        xa.Prefix.ToLower() == Properties.Settings.Default.VMwareNamespacePrefix ||
                        xa.NamespaceURI == Properties.Settings.Default.VMwareNamespace)
                    {
                        isVmware = true;
                        break;
                    }
                }

                //always call the vc4 conversion; even if the ovf is newer the
                //replacements in the method (if they happen) are harmless
                //and the only cost is the re-deserialization of the xml
                if (isVmware)
                    ovfEnv = LoadVmw40OvfXml(ovfxml);
            }
            if (ovfEnv == null)
            {
                ovfEnv = LoadVmw35OvfXml(ovfxml);
                log.Error("Last Change Convert died.");
            }

            return ovfEnv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ovffilename"></param>
        /// <returns></returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static bool ValidateXmlToSchema(string ovffilename)
        {
            bool isValid = false;
            ValidationEventHandler eventHandler = new ValidationEventHandler(ShowSchemaValidationCompileErrors);
            string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string xmlNamespaceSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.xmlNamespaceSchemaLocation);
            string cimCommonSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.cimCommonSchemaLocation);
            string cimRASDSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.cimRASDSchemaLocation);
            string cimVSSDSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.cimVSSDSchemaLocation);
            string ovfSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.ovfEnvelopeSchemaLocation);
            string wsseSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.wsseSchemaLocation);
            string xencSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.xencSchemaLocation);
            string wsuSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.wsuSchemaLocation);
            string xmldsigSchemaFilename = Path.Combine(currentPath, Properties.Settings.Default.xmldsigSchemaLocation);

            // Allow use of xmlStream in finally block.
            FileStream xmlStream = null;

            try
            {
                string ovfname = ovffilename;
                string ext = Path.GetExtension(ovffilename);

                if (!string.IsNullOrEmpty(ext) && (ext.ToLower().EndsWith("gz") || ext.ToLower().EndsWith("bz2")))
                {
                    ovfname = Path.GetFileNameWithoutExtension(ovffilename);
                }

                ext = Path.GetExtension(ovfname);

                if (!string.IsNullOrEmpty(ext) && ext.ToLower().EndsWith("ova"))
                {
                    ovfname = Path.GetFileNameWithoutExtension(ovfname) + ".ovf";
                }

                ext = Path.GetExtension(ovfname);

                if (!ext.ToLower().EndsWith("ovf"))
                {
                    throw new InvalidDataException("OVF.ValidateXmlToSchema: Incorrect filename: " + ovfname);
                }

                xmlStream = new FileStream(ovfname, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse; //Upgrading to .Net 4.0: ProhibitDtd=false is obsolete, this line is the replacement according to: http://msdn.microsoft.com/en-us/library/system.xml.xmlreadersettings.prohibitdtd%28v=vs.100%29.aspx
                XmlSchema schema = new XmlSchema();
                bool useOnlineSchema = Convert.ToBoolean(Properties.Settings.Default.useOnlineSchema);
                if (useOnlineSchema)
                {
                    settings.Schemas.Add(null, Properties.Settings.Default.dsp8023OnlineSchema);
                }
                else
                {
                    settings.Schemas.Add("http://www.w3.org/XML/1998/namespace", xmlNamespaceSchemaFilename);
                    settings.Schemas.Add("http://schemas.dmtf.org/wbem/wscim/1/common", cimCommonSchemaFilename);
                    settings.Schemas.Add("http://schemas.dmtf.org/wbem/wscim/1/cim-schema/2/CIM_VirtualSystemSettingData", cimVSSDSchemaFilename);
                    settings.Schemas.Add("http://schemas.dmtf.org/wbem/wscim/1/cim-schema/2/CIM_ResourceAllocationSettingData", cimRASDSchemaFilename);
                    settings.Schemas.Add("http://schemas.dmtf.org/ovf/envelope/1", ovfSchemaFilename);
                    settings.Schemas.Add("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", wsseSchemaFilename);
                    settings.Schemas.Add("http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", wsuSchemaFilename);
                    //settings.Schemas.Add("http://www.w3.org/2001/04/xmlenc#", xencSchemaFilename);
                    //settings.Schemas.Add("http://www.w3.org/2000/09/xmldsig#", xmldsigSchemaFilename);
                }
                settings.ValidationType = ValidationType.Schema;
                XmlReader reader = XmlReader.Create(xmlStream,settings);
                while (reader.Read()){}
                isValid = true;

            }
            catch (XmlException xmlex)
            {
                log.ErrorFormat("ValidateXmlToSchema XML Exception: {0}", xmlex.Message);
                throw new Exception(xmlex.Message, xmlex);
            }
            catch (XmlSchemaException schemaex)
            {
                log.ErrorFormat("ValidateXmlToSchema Schema Exception: {0}", schemaex.Message);
                throw new Exception(schemaex.Message, schemaex);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("ValidateXmlToSchema Exception: {0}", ex.Message);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                xmlStream.Close();
            }
            return isValid;
        }
        /// <summary>
        /// Attempt to validate an object's property to ensure it is not null and contains a value
        /// uses reflection into the provided object to find the named property.
        /// </summary>
        /// <param name="name">Name of property to test</param>
        /// <param name="target">object containing property</param>
        /// <returns>true: appears valid, false: something failed</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static bool ValidateProperty(string name, object target)
        {
            bool isValid = true;
            PropertyInfo[] properties = target.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name.Equals(name))
                {
                    object outervalue = property.GetValue(target, null);
                    if (outervalue != null)
                    {
                        if (outervalue is cimString[])
                        {
                            cimString[] strvalue = (cimString[])property.GetValue(target, null);
                            if (!(strvalue != null &&
                                  strvalue.Length > 0 &&
                                  strvalue[0] != null &&
                                  !string.IsNullOrEmpty(strvalue[0].Value)))
                            {
                                isValid = false;
                            }
                        }
                        else if (outervalue is Msg_Type[])
                        {
                            Msg_Type[] msgtype = (Msg_Type[])property.GetValue(target, null);
                            if (!(msgtype != null &&
                                  msgtype.Length > 0 &&
                                  msgtype[0] != null &&
                                  !string.IsNullOrEmpty(msgtype[0].Value)))
                            {
                                isValid = false;
                            }
                        }
                        else if (!outervalue.GetType().IsPrimitive)
                        {
                            // The purpose of this code is still unclear.
                            // It seems meant to handle complex types like XenOvf.Definitions.CimString.
                            object innervalue = property.GetValue(target, null);
                            Type innertype = innervalue.GetType();
                            PropertyInfo innerproperty = innertype.GetProperty("Value");
                            object targetvalue = null;
                            if (innerproperty == null)
                            {
                                // There is no property.
                                // The outertype is likely a standard .Net type like String and not an alias for a type like XenOvf.Definitions.CimString.
                                // A case that causes a failure is when the target type is XenOvfEnvelope and the property name is "Name".
                                targetvalue = innervalue;
                            }
                            else
                            {
                                // The outertype is alias for a type like XenOvf.Definitions.CimString.
                                targetvalue = innerproperty.GetValue(outervalue, null);
                            }

                            if (targetvalue != null)
                            {
                                if (targetvalue is string)
                                {
                                    if (((string)targetvalue).Length <= 0)
                                    {
                                        isValid = false;
                                    }
                                }
                            }
                            else
                            {
                                isValid = false;
                            }
                        }
                    }
                    else
                    {
                        isValid = false;
                    }
                }
            }
            return isValid;
        }
        /// <summary>
        /// create an XenXva object give the OVA.XML version 2 string.
        /// </summary>
        /// <param name="ovaxml">xml string</param>
        /// <returns>XenXva object</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static XenXva DeserializeOvaXml(string ovaxml)
        {
            XenXva xenobj = (XenXva)Tools.Deserialize(ovaxml, typeof(XenXva));
            xenobj.xenstruct.xenmember = DeserializeXenMembers(xenobj.xenstruct.xenmember);
            return xenobj;
        }
        /// <summary>
        /// Generic Method Attempt:  Load and XML file and create 
        /// an object of type T
        /// </summary>
        /// <typeparam name="T">Type of object to create</typeparam>
        /// <param name="filename">fullpath/filename</param>
        /// <returns>object of type T</returns>
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static T LoadFileXml<T>(string filename)
        {
            return (T)Deserialize(LoadFile(filename), typeof(T));
        }

        #endregion

        #region STREAM
        public static bool CancelStreamCopy = false;
        public static void StreamCopy(Stream Input, Stream Output)
        {
            long bufsize = 2 * MB;
            byte[] block = new byte[bufsize];
            ulong p = 0;
            int n = 0;
            while (true)
            {
                n = Input.Read(block, 0, block.Length);
                if (n <= 0) break;
                Output.Write(block, 0, n);
                p += (ulong)n;
                if (CancelStreamCopy) break;
                ClearBuffer(block);
            }
            Output.Flush();
            CancelStreamCopy = false;  // Reset so next call will start.
        }
        private static void ClearBuffer(byte[] block)
        {
            for (int i = 0; i < block.Length; i++) block[i] = 0;
        }
        #endregion


        private static void ShowSchemaValidationCompileErrors(object sender, ValidationEventArgs args)
        {
            log.ErrorFormat("ShowSchemaValidationCompileErrors: {0}", args.Message);
        }
        private static bool ValidateField(string name, object target)
        {
            bool isValid = true;

            return isValid;
        }
        private static EnvelopeType LoadVmw40OvfXml(string ovfxml)
        {
            if (string.IsNullOrEmpty(ovfxml))
                return null;

            // With what VMWare currently publishes with VC4, we need to update the XML

            ovfxml = ovfxml.Replace("<References", "<ovf:References").Replace("</References", "</ovf:References")
                           .Replace("<Section", "<ovf:Section").Replace("</Section", "</ovf:Section")
                           .Replace("<Content", "<ovf:Content").Replace("</Content", "</ovf:Content")
                           .Replace("<File", "<ovf:File").Replace("</File", "</ovf:File")
                           .Replace("<Disk", "<ovf:Disk").Replace("</Disk", "</ovf:Disk")
                           .Replace("<Info", "<ovf:Info").Replace("</Info", "</ovf:Info")
                           .Replace("<Network", "<ovf:Network").Replace("</Network", "</ovf:Network")
                           .Replace("<Description", "<ovf:Description").Replace("</Description", "</ovf:Description")
                           .Replace("<License", "<ovf:License").Replace("</License", "</ovf:License")
                           .Replace("<System", "<ovf:System").Replace("</System", "</ovf:System")
                           .Replace("<rasd:InstanceId", "<rasd:InstanceID").Replace("</rasd:InstanceId", "</rasd:InstanceID")
                           .Replace("<Item", "<ovf:Item").Replace("</Item", "</ovf:Item");

            EnvelopeType ovfEnv = (EnvelopeType)Deserialize(ovfxml, typeof(EnvelopeType));
            log.Debug("Finished LoadVmw40OvfXml");
            return ovfEnv;
        }
        private static EnvelopeType LoadVmw35OvfXml(string ovfxml)
        {
            if (string.IsNullOrEmpty(ovfxml))
                return null;

            // With what VMWare currently publishes with VC35, we need to update the XML

            ovfxml = ovfxml.Replace(Properties.Settings.Default.vmwEnvelopeNamespace, Properties.Settings.Default.cimEnvelopeURI)
                           .Replace("<References", "<ovf:References").Replace("</References", "</ovf:References")
                           .Replace("<Section", "<ovf:Section").Replace("</Section", "</ovf:Section")
                           .Replace("<Content", "<ovf:Content").Replace("</Content", "</ovf:Content")
                           .Replace("<File", "<ovf:File").Replace("</File", "</ovf:File")
                           .Replace("<Disk", "<ovf:Disk").Replace("</Disk", "</ovf:Disk")
                           .Replace("<Info", "<ovf:Info").Replace("</Info", "</ovf:Info")
                           .Replace("<Network", "<ovf:Network").Replace("</Network", "</ovf:Network")
                           .Replace("<Description", "<ovf:Description").Replace("</Description", "</ovf:Description")
                           .Replace("<License", "<ovf:License").Replace("</License", "</ovf:License")
                           .Replace("<System", "<ovf:System").Replace("</System", "</ovf:System")
                           .Replace("<rasd:InstanceId", "<rasd:InstanceID").Replace("</rasd:InstanceId", "</rasd:InstanceID")
                           .Replace("<Item", "<ovf:Item").Replace("</Item", "</ovf:Item");

            EnvelopeType ovfEnv = (EnvelopeType)Deserialize(ovfxml, typeof(EnvelopeType));
            log.Debug("Finished LoadVmw35OvfXml");
            return ovfEnv;
        }
        private static XenMember[] DeserializeXenMembers(XenMember[] members)
        {
            foreach (XenMember xm in members)
            {
                if (xm.xenvalue is XmlNode[])
                {
                    foreach (XmlNode xn in (XmlNode[])xm.xenvalue)
                    {
                        xm.xenvalue = DeserializeContent(xn);
                    }
                }
                else
                {
                    xm.xenvalue = "";
                }
            }
            return members;
        }
        private static object DeserializeContent(XmlNode node)
        {
            object xenobj = null;
            if (node.NodeType == XmlNodeType.Element && node.Name.ToLower().Equals("struct"))
            {
                xenobj = Tools.Deserialize(node.OuterXml, typeof(XenStruct));
                XenStruct xs = (XenStruct)xenobj;
                if (xs.xenmember != null && xs.xenmember.Length > 0)
                {
                    DeserializeXenMembers(xs.xenmember);
                }
            }
            else if (node.NodeType == XmlNodeType.Element && node.Name.ToLower().Equals("data"))
            {
                xenobj = Tools.Deserialize(node.OuterXml, typeof(XenData));
            }
            else if (node.NodeType == XmlNodeType.Element && node.Name.ToLower().Equals("array"))
            {
                xenobj = Tools.Deserialize(node.OuterXml, typeof(XenArray));
                XenArray xa = (XenArray)xenobj;
                if (xa.xendata.xenvalue != null && xa.xendata.xenvalue[0] is XmlNode[])
                {
                    List<object> values = new List<object>();
                    for (int i = 0; i < xa.xendata.xenvalue.Length; i++ )
                    {
                        values.Add(DeserializeContent((XmlNode)((XmlNode[])xa.xendata.xenvalue[i])[0]));
                    }
                    xa.xendata.xenvalue = values.ToArray();
                }
                else if (xa.xendata != null && xa.xendata.xenvalue != null && xa.xendata.xenvalue.Length > 0)
                {
                    foreach (XenValue xv in xa.xendata.xenvalue)
                    {
                        if (xv.xenstruct != null)
                        {
                            DeserializeXenMembers(xv.xenstruct.xenmember);
                        }
                    }
                }
                else
                {
                    xa.xendata.xenvalue = null;
                }
            }
            else if (node.NodeType == XmlNodeType.Text )
            {
                xenobj = node.Value;
            }
            
            return xenobj;
        }
        private static T DeserializeNode<T>(XmlNode node)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            try
            {
                return (T)xs.Deserialize(new XmlNodeReader(node));
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Tools.Deserialize FAILED {0}", ex.Message);
            }
            return (T) new object();
        }
        private static void OpenArchive(string filename)
        {
            log.InfoFormat("Utilities.OpenArchive: Opening OVF Archive: {0}", filename);
            if (!File.Exists(filename))
            {
                log.ErrorFormat("Utilities.OpenArchive: Cannot find file: {0}", filename);
                throw new FileNotFoundException(string.Format(Messages.FILE_MISSING, filename));
            }
            OVF.OpenOva(Path.GetDirectoryName(filename), Path.GetFileName(filename));
       }
        /// <summary>
        /// Specific Exception starting from XenOvf.Utilities
        /// </summary>
        public class CtxUtilitiesException : Exception
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public CtxUtilitiesException() : base() { }
            /// <summary>
            /// Constructor
            /// </summary>
            public CtxUtilitiesException(string message) : base(message) { }
            /// <summary>
            /// Constructor
            /// </summary>
            public CtxUtilitiesException(string message, Exception exception) : base(message, exception) { }
            /// <summary>
            /// Constructor
            /// </summary>
            public CtxUtilitiesException(SerializationInfo serialinfo, StreamingContext context) : base(serialinfo, context) { }
        }
    }
}
