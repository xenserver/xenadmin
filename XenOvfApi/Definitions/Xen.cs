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

namespace XenOvf.Definitions
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.Schema;

    #region XenServer 5.0 OVA.XML Parser
    [XmlRoot("value")]
    public class XenXva
    {
        [XmlElement("struct")]
        public XenStruct xenstruct;
    }

    [XmlIncludeAttribute(typeof(XenStruct))]
    [XmlIncludeAttribute(typeof(XenArray))]
    [XmlIncludeAttribute(typeof(string))]
    public class XenMember 
    {
        [XmlElement("name")]
        public string xenname;

        [XmlElement("value")]
        public object xenvalue;
    }

    [XmlRoot("struct")]
    public class XenStruct
    {
        [XmlElement("member")]
        public XenMember[] xenmember;
    }

    [XmlRoot("struct")]
    public class XenValue 
    {
        [XmlElement("struct")]
        public XenStruct xenstruct;
    }

    [XmlRoot("data")]
    public class XenData 
    {
        [XmlElement("value")]
        public object[] xenvalue;
    }

    [XmlRoot("array")]
    public class XenArray 
    {
        [XmlElement("data")]
        public XenData xendata;
    }
    #endregion

    #region XenConvert XVA 0.1 XML Parser
    [XmlRoot("appliance")]
    public class XcAppliance
    {
        private string _version;
        private XcVm _xvm;
        private XcVdi _xvdi;

        [XmlAttribute("version")]
        public string version
        {
            get { return _version; }
            set { _version = value; }
        }

        [XmlElement("vm")]
        public XcVm vm
        {
            get { return _xvm; }
            set { _xvm = value; }
        }

        [XmlElement("vdi")]
        public XcVdi vdi
        {
            get { return _xvdi; }
            set { _xvdi = value; }
        }

        #region FOR UNDEFINED ATTRIBUTES/ELEMENTS
        private XmlAttribute[] anyAttrField;
        [XmlAnyAttributeAttribute()]
        public XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }

        private XmlElement[] anyField;
        [XmlAnyElementAttribute()]
        public XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
        #endregion
    }

    [XmlRoot("vm")]
    public class XcVm
    {
        private string _name;
        private string _label;
        private string _shortdesc;
        private XcConfig _config;
        private XcHacks _hacks;
        private XcVbd _vbd;

        [XmlAttribute("name")]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement("label")]
        public string label
        {
            get { return _label; }
            set { _label = value; }
        }
        [XmlElement("shortdesc")]
        public string shortdesc
        {
            get { return _shortdesc; }
            set { _shortdesc = value; }
        }
        [XmlElement("config")]
        public XcConfig config
        {
            get { return _config; }
            set { _config = value; }
        }
        [XmlElement("hacks")]
        public XcHacks hacks
        {
            get { return _hacks; }
            set { _hacks = value; }
        }
        [XmlElement("vbd")]
        public XcVbd vbd
        {
            get { return _vbd; }
            set { _vbd = value; }
        }

        #region FOR UNDEFINED ATTRIBUTES/ELEMENTS
        private XmlAttribute[] anyAttrField;
        [XmlAnyAttributeAttribute()]
        public XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }

        private XmlElement[] anyField;
        [XmlAnyElementAttribute()]
        public XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }
        #endregion
    }

    [XmlRoot("config")]
    public class XcConfig
    {
        private ulong _memset;
        private ushort _vcpus;

        [XmlAttribute("mem_set")]
        public ulong memset
        {
            get { return _memset; }
            set { _memset = value; }
        }
        [XmlAttribute("vcpus")]
        public ushort vcpus
        {
            get { return _vcpus; }
            set { _vcpus = value; }
        }
    }

    [XmlRoot("hacks")]
    public class XcHacks
    {
        private bool _ishvm;

        [XmlAttribute("is_hvm")]
        public bool isHVM
        {
            get { return _ishvm; }
            set { _ishvm = value; }
        }

    }

    [XmlRoot("vbd")]
    public class XcVbd
    {
        private string _device;
        private string _function;
        private string _mode;
        private string _vdi;

        [XmlAttribute("device")]
        public string device
        {
            get { return _device; }
            set { _device = value; }
        }
        [XmlAttribute("function")]
        public string function
        {
            get { return _function; }
            set { _function = value; }
        }
        [XmlAttribute("mode")]
        public string mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        [XmlAttribute("vdi")]
        public string vdi
        {
            get { return _vdi; }
            set { _vdi = value; }
        }

    }

    [XmlRoot("vdi")]
    public class XcVdi
    {
        private string _name;
        private ulong _size;
        private string _source;
        private string _type;
        private string _variety;

        [XmlAttribute("name")]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        [XmlAttribute("size")]
        public ulong size
        {
            get { return _size; }
            set { _size = value; }
        }
        [XmlAttribute("source")]
        public string source
        {
            get { return _source; }
            set { _source = value; }
        }
        [XmlAttribute("type")]
        public string type
        {
            get { return _type; }
            set { _type = value; }
        }
        [XmlAttribute("variety")]
        public string variety
        {
            get { return _variety; }
            set { _variety = value; }
        }

    }
    #endregion
}
