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

namespace XenOvf.Definitions.VMX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.Schema;

    [XmlRoot("Foundry")]
    public class Vmw_Foundry_Type
    {
        [XmlElement]
        public Vmw_ValueType_Type VMId;
        [XmlElement]
        public Vmw_ClientMetaData_Type ClientMetaData;
        [XmlElement]
        public Vmw_ValueType_Type vmxPathName;
    }

    public class Vmw_ValueType_Type
    {
        [XmlAttribute]
        public string type;
        [XmlText]
        public string value;
    }

    public class Vmw_ClientMetaData_Type
    {
        [XmlElement(IsNullable= true)]
        public string[] clientMetaDataAttributes;
        [XmlElement(IsNullable = true)]
        public string[] HistoryEventList;
    }

    public class Vmw_Vmx_File
    {
        Dictionary<string, string> vmxdata = new Dictionary<string, string>();

        public Vmw_Vmx_File(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs);
            while(true)
            {
                if (sr.EndOfStream) break;
                string currentline = sr.ReadLine();
                string[] pairs = currentline.Split(new char[] { '=' });
                vmxdata.Add(pairs[0].Trim(), pairs[1].Trim().Trim(new char[] { '"' }));
            }
        }

        public string GetValueFor(string key)
        {
            if (vmxdata.ContainsKey(key))
            {
                return vmxdata[key];
            }
            return null;
        }
    }

    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.vmware.com/schema/ovf")]
    public class Vmw_IpAssignmentSection_Type : Section_Type
    {
    }
}
