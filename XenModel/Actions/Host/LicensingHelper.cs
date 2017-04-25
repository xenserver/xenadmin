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
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using CookComputing.XmlRpc;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions.HostActions
{
    public static class LicensingHelper
    {
        #region Send license data to activation server

        /// <summary>
        /// Send license information to the activation server after assigning or releasing a paid license.
        /// This task will run on a separate thread.
        /// </summary>
        /// <param name="hosts">Pass in a Dictionary containing the hosts and their previous license data</param>
        /// <param name="currentEdition">Pass in the current license edition</param>
        public static void SendLicenseEditionData(Dictionary<XenAPI.Host, LicenseDataStruct> hosts, string currentEdition)
        {
            // Supply the state information required by the task.
            SendLicenseDataHelper sendLicenseDataHelper = new SendLicenseDataHelper(hosts, "apply_license", currentEdition, true);

            // start a separate thread
            Thread thread = new Thread(new ThreadStart(sendLicenseDataHelper.ThreadProc));

            thread.Name = "Process licensing data for assigning or releasing a license";
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Send license information to the activation server after activating a free license.
        /// In this case both previous and current editions are "free"
        /// This task will run on a separate thread.
        /// </summary>
        /// <param name="hosts">Pass in a Dictionary containing the hosts and their previous license data</param>
        public static void SendActivationData(Dictionary<XenAPI.Host, LicenseDataStruct> hosts)
        {
            // Supply the state information required by the task.
            SendLicenseDataHelper sendLicenseDataHelper = new SendLicenseDataHelper(hosts, "activation", XenAPI.Host.GetEditionText(XenAPI.Host.Edition.Free), true);

            // start a separate thread
            Thread thread = new Thread(new ThreadStart(sendLicenseDataHelper.ThreadProc));

            thread.Name = "Process licensing data for activating a free license)";
            thread.IsBackground = true;
            thread.Start();
        }

        public struct LicenseDataStruct
        {
            public string Edition;
            public string ExpiryDate;

            public LicenseDataStruct(string edition, string expiryDate)
            {
                Edition = edition;
                ExpiryDate = expiryDate;
            }

            public LicenseDataStruct(XenAPI.Host host)
            {
                Edition = host.edition;
                ExpiryDate = host.license_params.ContainsKey("expiry") ? host.license_params["expiry"] : "";
            }
        }

        private class SendLicenseDataHelper
        {
            public Dictionary<XenAPI.Host, LicenseDataStruct> Hosts;
            public string LicensingAction;
            public string CurrentEdition;
            public bool IncludeSKU;

            public SendLicenseDataHelper(Dictionary<XenAPI.Host, LicenseDataStruct> hosts, string licensingAction, string currentEdition, bool includeSKU)
            {
                Hosts = hosts;
                LicensingAction = licensingAction;
                CurrentEdition = currentEdition;
                IncludeSKU = includeSKU;
            }

            public void ThreadProc()
            {
                string licensingData;
                try
                {
                    // build the xml
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Encoding Utf8 = new UTF8Encoding(false);
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.OmitXmlDeclaration = true;
                        settings.Encoding = Utf8;
                        XmlWriter writer = XmlWriter.Create(ms, settings);

                        writer.WriteStartDocument();
                        writer.WriteStartElement(LicensingAction);
                        foreach (XenAPI.Host host in Hosts.Keys)
                        {
                            ProduceXmlForHost(host, Hosts[host], writer);
                        }
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                        writer.Close();

                        licensingData = Utf8.GetString(ms.ToArray());
                    }

                    // send the xml
                    ActivationProxy proxy = XmlRpcProxyGen.Create<ActivationProxy>();
                    proxy.Url = string.Format(InvisibleMessages.ACTIVATION_URL, InvisibleMessages.ACTIVATION_SERVER);
                    proxy.Timeout = 30 * 1000;
                    proxy.UseIndentation = false;
                    proxy.UserAgent = Session.UserAgent;
                    proxy.KeepAlive = true;
                    proxy.Proxy = Session.Proxy;

                    // response is the transaction id of this call to the activation service
                    string response = proxy.process_reactivation_request(licensingData);
                }
                catch (Exception)
                {
                }
            }

            private void ProduceXmlForHost(XenAPI.Host host, LicenseDataStruct previousLicenseData, XmlWriter writer)
            {

                // wait for host to be updated 
                for (int i = 0; i < 100 &&
                        Helper.AreEqual2(host.license_params["expiry"], previousLicenseData.ExpiryDate) &&
                        Helper.AreEqual2(host.edition, previousLicenseData.Edition); i++)
                {
                    Thread.Sleep(100);
                }
                writer.WriteStartElement("host");
                writer.WriteAttributeString("uuid", host.uuid);
                foreach (KeyValuePair<String, String> kvp in host.software_version)
                {
                    writer.WriteStartElement("software_version_element");
                    writer.WriteAttributeString("key", kvp.Key);
                    writer.WriteAttributeString("value", kvp.Value);
                    writer.WriteEndElement();
                }
                // edition information
                if (IncludeSKU)
                {
                    string newEdition;
                    if (String.IsNullOrEmpty(CurrentEdition))
                    {
                        newEdition = host.edition;
                    }
                    else
                    {
                        newEdition = CurrentEdition;
                    }
                    writer.WriteStartElement("SKU");
                    writer.WriteAttributeString("from", previousLicenseData.Edition);
                    writer.WriteAttributeString("to", newEdition);
                    writer.WriteEndElement();
                }
                // license expiry information
                writer.WriteStartElement("license_expiry_date");
                writer.WriteAttributeString("from", previousLicenseData.ExpiryDate);
                writer.WriteAttributeString("to", host.license_params["expiry"]);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
