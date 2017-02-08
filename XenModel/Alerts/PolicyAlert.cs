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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Alerts
{
    public class PolicyAlert : IEquatable<PolicyAlert>
    {
        public readonly string Type;
        public readonly string Text;
        public readonly DateTime Time;
        public readonly string PolicyType;
        public readonly int numberOfVMsFailed;

        public PolicyAlert(long priority, string name, DateTime _time, string body, string policyName)
        {
            Type = (priority == 4 ? "info": "error");
            Time = _time;
            PolicyType = "VMSS";

            if(Type == "info")
            {
                Text = Message.FriendlyBody(name);
                return;
            }

            /* We reach here when message type is an error hence we need 
             * to parse the message body accordingly */

            numberOfVMsFailed = Regex.Matches(body, "VM:").Count;              
            if(numberOfVMsFailed == 0)
            {
                Text = Message.FriendlyBody(name);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(Message.FriendlyBody(name), policyName);
            string[] messageHeader = Regex.Split(body.Replace("\n", " "), "Details:");

             /* get a list of all VMs that have encountered an error */
            string[] vmList = Regex.Split(messageHeader[1], "VM:");

            if(vmList.Length > 1)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(Messages.VMSS_ALERT_DETAILS);
            }
            /* for each VM entry parse the data and get VM name and corresponding error message */
            for (int vmIterator = 1; vmIterator < vmList.Length; vmIterator++)
            {
                vmList[vmIterator].Trim();
                string[] tmp = Regex.Split(vmList[vmIterator], "Error:");
                string vmName = Regex.Split(tmp[0], "UUID:")[0];
              
                string[] errorCode = Regex.Split(tmp[1].Replace("[", "").Replace("],", "").Replace("]", ""), "\',");
                for (int errorCodeIterator = 0; errorCodeIterator < errorCode.Length; errorCodeIterator++)
                    errorCode[errorCodeIterator] = errorCode[errorCodeIterator].Replace("\'", "").Trim();

                string errorMessage = new Failure(errorCode).Message;
                sb.AppendLine();
                sb.AppendFormat(Messages.VMSS_ALERT_VM_ERROR_FORMAT, vmIterator, vmName.Trim(), errorMessage);
            }
            Text = sb.ToString();
        }

        public PolicyAlert(IXenConnection connection, string body)
        {
            PolicyType = "VMPP";
            var sb = new StringBuilder();
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(body);
                XmlNodeList _fnames = xmlDocument.GetElementsByTagName("time");
                Time = DateTime.Parse(_fnames[0].InnerText, CultureInfo.InvariantCulture);
                _fnames = xmlDocument.GetElementsByTagName("messagetype");
                if (_fnames[0].InnerText == "error")
                {
                    Type = "error";
                    numberOfVMsFailed = 0;
                    _fnames = xmlDocument.GetElementsByTagName("error");
                    for (int i = 0; i < _fnames.Count; i++)
                    {
                        XmlNode item = _fnames[i];
                        foreach (XmlNode child in item.ChildNodes)
                        {
                            if (child.Name == "vm")
                            {
                                var vm = connection.Cache.Find_By_Uuid<VM>(child.InnerText);
                                if (vm == null)
                                    continue;
                                sb.AppendFormat("VM '{0}': ", vm.Name);
                                numberOfVMsFailed++;
                            }
                            if (child.Name == "errorcode")
                            {
                                string text = FriendlyErrorNames.ResourceManager.GetString(child.InnerText);
                                if (string.IsNullOrEmpty(text))
                                    text = Message.FriendlyBody(child.InnerText);
                                sb.Append(string.IsNullOrEmpty(text) ? child.InnerText : text);
                            }
                        }
                        if (i + 1 < _fnames.Count)
                            sb.AppendLine();
                    }
                    Text = sb.ToString();
                }
                else if (_fnames[0].InnerText == "info" || _fnames[0].InnerText == "warn")
                {
                    Type = _fnames[0].InnerText;
                    XmlNodeList _messages = xmlDocument.GetElementsByTagName("message");

                    foreach (XmlNode node in _messages)
                    {
                        if (node.InnerText == node.InnerXml)
                        {
                            Text = Message.FriendlyBody(node.InnerText);
                            break;
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        public string ShortFormatBody
        {
            get
            {
                {
                    if (Type == "error")
                    {   
                        if (PolicyType == "VMSS")
                        {  
                            if (numberOfVMsFailed == 0)
                            {
                                return Message.FriendlyName(XenAPI.Message.MessageType.VMSS_SNAPSHOT_FAILED.ToString());
                            }
                            else
                            {
                                return string.Format(Messages.VM_SNAPSHOT_SCHEDULE_FAILED, Message.FriendlyName(XenAPI.Message.MessageType.VMSS_SNAPSHOT_FAILED.ToString()), numberOfVMsFailed);
                            }
                                
                        }
                        else
                        {
                            return string.Format(Messages.VM_PROTECTION_POLICY_FAILED, Message.FriendlyName(XenAPI.Message.MessageType.VMPP_SNAPSHOT_FAILED.ToString()), numberOfVMsFailed);
                        }  
                    }
                    else return Text;
                }
            }
        }

        #region IEquatable<PolicyAlert> Members

        public bool Equals(PolicyAlert other)
        {
            if (Text != other.Text)
                return false;
            if (Type != other.Type)
                return false;
            if (Time != other.Time)
                return false;
            return true;
        }

        #endregion


    }
}

