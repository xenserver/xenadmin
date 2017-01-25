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

namespace XenOvf.Definitions.VMC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.Schema;

    #region VPC/VMC
    [XmlRoot("preferences")]
    public class Ms_Vmc_Type
    {
        [XmlElement]
        public Ms_ValueType_Type version;
        [XmlElement]
        public Ms_Alerts_Type alerts;
        [XmlElement]
        public Ms_Hardware_Type hardware;
        [XmlElement]
        public Ms_Integration_Type integration;
        [XmlElement]
        public Ms_Properties_Type properties;
        [XmlElement]
        public Ms_Settings_Type settings;
        [XmlElement]
        public Ms_Virtual_Machines_Type virtual_machines;
        [XmlElement]
        public Ms_ValueType_Type multi_channel;
        [XmlElement]
        public Ms_Ui_Options_Type ui_options;
    }

    public class Ms_Alerts_Type
    {
        [XmlElement]
        public Ms_Notice_Type notfications;
    }
    public class Ms_Hardware_Type
    {
        [XmlElement]
        public Ms_Memory_Type memory;
        [XmlElement]
        public Ms_Pci_Bus_Type pci_bus;
        [XmlElement]
        public Ms_Standard_Type standard;
        [XmlElement]
        public Ms_Super_Io_Type super_io;
        [XmlElement]
        public Ms_Bios_Type bios;
        [XmlElement]
        public Ms_Integration_Type integration;
    }

    public class Ms_ValueType_Type
    {
        [XmlAttribute]
        public string type;
        [XmlText]
        public string value;
    }
    public class Ms_PathReference_Type
    {
        [XmlElement]
        public Ms_ValueType_Type absolute;
        [XmlElement]
        public Ms_ValueType_Type relative;
    }
    public class Ms_BuildName_Type
    {
        [XmlElement]
        public Ms_ValueType_Type build;
        [XmlElement]
        public Ms_ValueType_Type name;
    }
    public class Ms_Enable_Type
    {
        [XmlElement]
        public Ms_ValueType_Type enable;
    }

    public class Ms_Ui_Options_Type
    {
        [XmlElement]
        public Ms_ValueType_Type dvd_none;
        [XmlElement]
        public Ms_ValueType_Type guest_rail_enabled;
        [XmlElement]
        public Ms_ValueType_Type window_xpos;
        [XmlElement]
        public Ms_ValueType_Type window_ypos;
        [XmlElement]
        public Ms_ValueType_Type full_screen;
        [XmlElement]
        public Ms_ValueType_Type resolution_height;
        [XmlElement]
        public Ms_ValueType_Type resolution_width;
    }
    public class Ms_Virtual_Machines_Type
    {
        [XmlElement]
        public Ms_Hw_Assist_Type hw_assist;
        [XmlElement]
        public Ms_ValueType_Type allow_packet_filtering;
        [XmlElement]
        public Ms_ValueType_Type allow_promiscuous_mode;
    }
    public class Ms_Hw_Assist_Type
    {
        [XmlElement]
        public Ms_ValueType_Type enable_hw_assist;
    }
    public class Ms_Notice_Type
    {
        [XmlElement]
        public Ms_ValueType_Type no_boot_disk;
    }
    public class Ms_Memory_Type
    {
        [XmlElement]
        public Ms_ValueType_Type ram_size;
    }
    public class Ms_Pci_Bus_Type
    {
        [XmlElement]
        public Ms_Ethernet_Adapter_Type ethernet_adapter;
        [XmlElement]
        public Ms_Video_Adapter_Type video_adapter;
        [XmlElement]
        public Ms_Ide_Adapter_Type ide_adapter;
    }
    public class Ms_Ethernet_Adapter_Type
    {
        [XmlElement]
        public Ms_ValueType_Type controller_count;
        [XmlElement]
        public Ms_Ethernet_Controller_Type[] ethernet_controller;
    }
    public class Ms_Ethernet_Controller_Type
    {
        [XmlAttribute]
        public string id;
        [XmlElement]
        public Ms_Virtual_Network_Type virtual_network;
        [XmlElement]
        public Ms_ValueType_Type ethernet_card_address;
    }
    public class Ms_Virtual_Network_Type
    {
        [XmlElement]
        public Ms_ValueType_Type id;
        [XmlElement]
        public Ms_ValueType_Type name;
    }
    public class Ms_Video_Adapter_Type
    {
        [XmlElement]
        public Ms_ValueType_Type vram_size;
    }
    public class Ms_Ide_Adapter_Type
    {
        [XmlElement]
        public Ms_Ide_Controller_Type[] ide_controller;
    }
    public class Ms_Ide_Controller_Type
    {
        [XmlAttribute]
        public string id;
        [XmlElement]
        public Ms_Location_Type[] location;
    }
    public class Ms_Location_Type
    {
        [XmlAttribute]
        public string id;
        [XmlElement]
        public Ms_ValueType_Type drive_type;
        [XmlElement]
        public Ms_PathReference_Type pathname;
        [XmlElement]
        public Ms_PathReference_Type undo_pathname;
    }
    public class Ms_Standard_Type
    {
        [XmlElement]
        public Ms_ValueType_Type name;
        [XmlElement]
        public Ms_ValueType_Type version;
    }
    public class Ms_Super_Io_Type
    {
        [XmlElement]
        public Ms_Floppy_Type[] floppy;
        [XmlElement]
        public Ms_Parallel_Port_Type[] parallel_port;
        [XmlElement]
        public Ms_Serial_Port_Type[] serial_port;
    }
    public class Ms_Floppy_Type
    {
        [XmlAttribute]
        public string id;
        [XmlElement]
        public Ms_ValueType_Type auto_detect;
        [XmlElement]
        public Ms_PathReference_Type pathname;
    }
    public class Ms_Parallel_Port_Type
    {
        [XmlElement]
        public Ms_ValueType_Type port_shared;
        [XmlElement]
        public Ms_ValueType_Type port_type;
    }
    public class Ms_Serial_Port_Type
    {
        [XmlElement]
        public Ms_ValueType_Type connect_immediately;
    }
    public class Ms_Bios_Type
    {
        [XmlElement]
        public Ms_Base_Board_Type base_board;
        [XmlElement]
        public Ms_ValueType_Type bios_guid;
        [XmlElement]
        public Ms_ValueType_Type bios_serial_number;
        [XmlElement]
        public Ms_Chassis_Type chassis;
        [XmlElement]
        public Ms_ValueType_Type cmos;
        [XmlElement]
        public Ms_ValueType_Type time_bytes;
    }
    public class Ms_Base_Board_Type
    {
        [XmlElement]
        public Ms_ValueType_Type serial_number;
    }
    public class Ms_Chassis_Type
    {
        [XmlElement]
        public Ms_ValueType_Type asset_tag;
        [XmlElement]
        public Ms_ValueType_Type serial_number;
    }
    public class Ms_Integration_Type
    {
        [XmlElement]
        public Ms_Microsoft_Type microsoft;
    }
    public class Ms_Microsoft_Type
    {
        [XmlElement]
        public Ms_Mouse_Type mouse;
        [XmlElement]
        public Ms_Integration_Version_Type version;
        [XmlElement]
        public Ms_Video_Type video;
        [XmlElement]
        public Ms_Folder_Sharing_Type folder_sharing;
        [XmlElement]
        public Ms_Heartbeat_Type heartbeat;
        [XmlElement]
        public Ms_Host_Time_Sync_Type host_time_sync;
    }
    public class Ms_Folder_Sharing_Type
    {
        [XmlElement]
        public Ms_ValueType_Type enabled;
        [XmlElement]
        public Ms_ValueType_Type load_allowed;
    }
    public class Ms_Heartbeat_Type
    {
        [XmlElement]
        public Ms_ValueType_Type failure_attempts;
        [XmlElement]
        public Ms_ValueType_Type failure_interval;
        [XmlElement]
        public Ms_ValueType_Type rate;
        [XmlElement]
        public Ms_ValueType_Type time;
    }
    public class Ms_Host_Time_Sync_Type
    {
        [XmlElement]
        public Ms_ValueType_Type enabled;
        [XmlElement]
        public Ms_ValueType_Type frequency;
        [XmlElement]
        public Ms_ValueType_Type threshold;
    }
    public class Ms_Mouse_Type
    {
        [XmlElement]
        public Ms_ValueType_Type allow;
    }
    public class Ms_Integration_Version_Type
    {
        [XmlElement]
        public Ms_ValueType_Type additions_number;
        [XmlElement]
        public Ms_Guest_Os_Type guest_os;

    }
    public class Ms_Guest_Os_Type
    {
        [XmlElement]
        public Ms_ValueType_Type build_number;
        [XmlElement]
        public Ms_ValueType_Type long_name;
        [XmlElement]
        public Ms_ValueType_Type revision_number;
        [XmlElement]
        public Ms_ValueType_Type short_name;
        [XmlElement]
        public Ms_ValueType_Type suite_name;
    }
    public class Ms_Video_Type
    {
        [XmlElement]
        public Ms_User_Selected_Type user_selected;
        [XmlElement]
        public Ms_ValueType_Type disable_resize;
        [XmlElement]
        public Ms_ValueType_Type full_screen;
        [XmlElement]
        public Ms_Mode_Type mode;
        [XmlElement]
        public Ms_Resolutions_Type resolutions;
        [XmlElement]
        public Ms_ValueType_Type height;
        [XmlElement]
        public Ms_ValueType_Type left_position;
        [XmlElement]
        public Ms_ValueType_Type max_height;
        [XmlElement]
        public Ms_ValueType_Type max_width;
        [XmlElement]
        public Ms_ValueType_Type top_position;
        [XmlElement]
        public Ms_ValueType_Type width;
    }
    public class Ms_Mode_Type
    {
        [XmlElement]
        public Ms_Full_Screen_Type full_screen;
    }
    public class Ms_Full_Screen_Type
    {
        [XmlElement]
        public Ms_ValueType_Type startup;
    }
    public class Ms_Resolutions_Type
    {
        [XmlElement]
        public Ms_ValueType_Type standard_only;
    }
    public class Ms_User_Selected_Type
    {
        [XmlElement]
        public Ms_ValueType_Type depth;
        [XmlElement]
        public Ms_ValueType_Type height;
        [XmlElement]
        public Ms_ValueType_Type width;
    }
    public class Ms_Properties_Type
    {
        [XmlElement]
        public Ms_BuildName_Type creator;
        [XmlElement]
        public Ms_BuildName_Type modifier;
    }
    public class Ms_Settings_Type
    {
        [XmlElement]
        public Ms_Configuration_Type configuration;
        [XmlElement]
        public Ms_Disks_Type disks;
        [XmlElement]
        public Ms_ValueType_Type globalconfigid;
        [XmlElement]
        public Ms_Host_Type host;
        [XmlElement]
        public Ms_ValueType_Type notes;
        [XmlElement]
        public Ms_Shutdown_Type shutdown;
        [XmlElement]
        public Ms_Sound_Type sound;
        [XmlElement]
        public Ms_Startup_Type startup;
        [XmlElement]
        public Ms_Undo_Drives_Type undo_drives;
        [XmlElement]
        public Ms_Video_Type video;
        [XmlElement]
        public Ms_ValueType_Type guest_os;
    }
    public class Ms_Host_Type
    {
        [XmlElement]
        public Ms_Resource_Control_Type resource_control;
    }
    public class Ms_Resource_Control_Type
    {
        [XmlElement]
        public Ms_Cpu_Type cpu;
    }
    public class Ms_Cpu_Type
    {
        [XmlElement]
        public Ms_ValueType_Type host_processors;
    }
    public class Ms_Disks_Type
    {
        [XmlElement]
        public Ms_Enable_Type track_disk_cache;
    }
    public class Ms_Configuration_Type
    {
        [XmlElement]
        public Ms_Saved_State_Type saved_state;
    }
    public class Ms_Saved_State_Type
    {
        [XmlElement]
        public Ms_PathReference_Type path;
    }
    public class Ms_Shutdown_Type
    {
        [XmlElement]
        public Ms_ValueType_Type prompt;
        [XmlElement]
        public Ms_Quit_Type quit;
        [XmlElement]
        public Ms_Enable_Type save;
        [XmlElement]
        public Ms_Enable_Type shutdown;
        [XmlElement]
        public Ms_Enable_Type turn_off;
        [XmlElement]
        public Ms_Last_Shutdown_Type last_shutdown;
    }
    public class Ms_Quit_Type
    {
        [XmlElement]
        public Ms_ValueType_Type action;
        [XmlElement]
        public Ms_ValueType_Type was_running;
    }
    public class Ms_Last_Shutdown_Type
    {
        [XmlElement]
        public Ms_ValueType_Type choice;
        [XmlElement]
        public Ms_ValueType_Type commit;
    }
    public class Ms_Sound_Type
    {
        [XmlElement]
        public Ms_Sound_Adapter_Type sound;
    }
    public class Ms_Sound_Adapter_Type
    {
        [XmlElement]
        public Ms_Enable_Type enable;
    }
    public class Ms_Type_Type
    {
        [XmlElement]
        public Ms_ValueType_Type type;
    }
    public class Ms_Startup_Type
    {
        [XmlElement]
        public Ms_Type_Type automatic;
    }
    public class Ms_Undo_Drives_Type
    {
        [XmlElement]
        public Ms_ValueType_Type always;
        [XmlElement]
        public Ms_ValueType_Type default_action;
        [XmlElement]
        public Ms_Enable_Type enabled;
        [XmlElement]
        public Ms_ValueType_Type purposely_kept;
        [XmlElement]
        public Ms_ValueType_Type use_default;
    }
    #endregion

    #region HYPER-V / CIMXML
    [XmlIncludeAttribute(typeof(Ms_ParameterValue_Type))]
    [XmlIncludeAttribute(typeof(Ms_ParameterValueArray_Type))]
    [XmlRoot(ElementName = "DECLARATIONS")]
    public class Ms_Declarations_Type
    {
        [XmlElement(ElementName = "DECLGROUP")]
        public List<Ms_DeclGroup_Type> declgroups = new List<Ms_DeclGroup_Type>();
    }
    [XmlRoot(ElementName = "DECLGROUP")]
    public class Ms_DeclGroup_Type
    {
        [XmlElement(ElementName = "VALUE.OBJECT")]
        public List<Ms_WrapperInstance_Type> values = new List<Ms_WrapperInstance_Type>();
    }
    [XmlRoot(ElementName = "PROPERTY")]
    public class Ms_ParameterValue_Type : Ms_Property_Base_Type
    {
        public Ms_ParameterValue_Type()
        {
        }
        public Ms_ParameterValue_Type(string name, string prop, string type, string value, Ms_QualifierInfo_Type qualifier)
        {
            Name = name;
            Propagated = prop;
            Type = type;
            Value = value;
            Qualifier = qualifier;
        }
        public Ms_ParameterValue_Type(string name, string prop, string type, string value)
        {
            Name = name;
            Propagated = prop;
            Type = type;
            Value = value;
        }
        public Ms_ParameterValue_Type(string name, string type, string value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
        public Ms_ParameterValue_Type(string name, string value)
        {
            Name = name;
            Value = value;
        }


        [XmlElement(ElementName = "VALUE")]
        public string Value;
    }
    [XmlRoot(ElementName = "PROPERTY.ARRAY")]
    public class Ms_ParameterValueArray_Type : Ms_Property_Base_Type
    {
        public Ms_ParameterValueArray_Type()
        {
        }
        public Ms_ParameterValueArray_Type(string name, string prop, string type, string[] values)
        {
            initialize(name, prop, type, values);
        }
        public Ms_ParameterValueArray_Type(string name, string type, string[] values)
        {
            initialize(name, null, type, values);
        }

        private void initialize(string name, string prop, string type, string[] values)
        {
            Name = name;
            Propagated = prop;
            Type = type;
            Values = new Ms_ParameterValue_Type[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                Values[i].Value = values[i];
            }
        }

        [XmlElement(ElementName = "VALUE.ARRAY")]
        public Ms_ParameterValue_Type[] Values;
    }
    public class Ms_ParameterValueArrayClass_Type
    {
        [XmlElement(ElementName = "VALUE")]
        public object[] Value;
    }   
    public abstract class Ms_Property_Base_Type
    {
        [XmlAttribute(AttributeName = "NAME")]
        public string Name;
        [XmlAttribute(AttributeName = "CLASSORIGIN")]
        public string ClassOrigin;
        [XmlAttribute(AttributeName = "PROPAGATED")]
        public string Propagated;
        [XmlAttribute(AttributeName = "TYPE")]
        public string Type;
        [XmlElement(ElementName = "QUALIFIER")]
        public Ms_QualifierInfo_Type Qualifier;
    }
    public class Ms_QualifierInfo_Type
    {
        [XmlAttribute(AttributeName = "NAME")]
        public string Name;
        [XmlAttribute(AttributeName = "PROPAGATED")]
        public string Propagated;
        [XmlAttribute(AttributeName = "TYPE")]
        public string Type;
        [XmlAttribute(AttributeName = "TOSUBCLASS")]
        public string ToSubClass;
        [XmlAttribute(AttributeName = "TOINSTANCE")]
        public string ToInstance;
        [XmlElement(ElementName = "VALUE")]
        public string Value;

        public Ms_QualifierInfo_Type()
        {
        }
        public Ms_QualifierInfo_Type(string name, string prop, string type, string toinstance, string value)
        {
            Name = name;
            Propagated = prop;
            Type = type;
            ToInstance = toinstance;
            Value = value;
        }
    }
    [XmlRoot(ElementName = "VALUE.OBJECT")]
    public class Ms_WrapperInstance_Type
    {
        [XmlElement(ElementName = "INSTANCE")]
        public Ms_Instance_Type instance;
    }
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Ms_ParameterValue_Type))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Ms_ParameterValueArray_Type))]
    [XmlRoot(ElementName = "INSTANCE")]
    public class Ms_Instance_Type
    {
        [XmlAttribute(AttributeName = "CLASSNAME", Form = System.Xml.Schema.XmlSchemaForm.None)]
        public string className;
        [XmlElement(ElementName = "QUALIFIER")]
        public Ms_QualifierInfo_Type Qualifier;
        [XmlElementAttribute("PROPERTY.ARRAY", typeof(Ms_ParameterValueArray_Type))]
        [XmlElementAttribute("PROPERTY", typeof(Ms_ParameterValue_Type))]
        public List<Ms_Property_Base_Type> Properties = new List<Ms_Property_Base_Type>();
    }
    [XmlRoot(ElementName = "INSTANCE")]
    public class Ms_Instance2_Type
    {
        [XmlAttribute(AttributeName = "CLASSNAME", Form = System.Xml.Schema.XmlSchemaForm.None)]
        public string className;
        [XmlElement(ElementName = "QUALIFIER")]
        public Ms_QualifierInfo_Type Qualifier;
        [XmlElement(ElementName = "PROPERTY", Form = System.Xml.Schema.XmlSchemaForm.None)]
        public List<Ms_ParameterValue_Type> Properties = new List<Ms_ParameterValue_Type>();
        [XmlElement(ElementName = "PROPERTY.ARRAY", Form = System.Xml.Schema.XmlSchemaForm.None)]
        public List<Ms_ParameterValueArrayClass_Type> PropertyArray = new List<Ms_ParameterValueArrayClass_Type>();
    }
    #endregion

    #region WIM MANIFEST
    [XmlRoot(ElementName="WIM")]
    public class Wim_Manifest
    {
        private ulong _totalbytes;
        private Wim_Image[] _image;
        private XmlAttribute[] _anyAttr;
        private XmlElement[] _anyField;

        [XmlElement(ElementName="TOTALBYTES")]
        public ulong TotalBytes
        {
            get { return _totalbytes; }
            set { _totalbytes = value; }
        }
        
        [XmlElement(ElementName = "IMAGE")]
        public Wim_Image[] Image
        {
            get { return _image; }
            set { _image = value; }
        }

        [XmlAnyAttributeAttribute()]
        public XmlAttribute[] AnyAttr
        {
            get { return this._anyAttr; }
            set { this._anyAttr = value; }
        }
 
        [XmlAnyElementAttribute()]
        public XmlElement[] Any
        {
            get
            {
                return this._anyField;
            }
            set
            {
                this._anyField = value;
            }
        }
    }
    public class Wim_Image
    {
        private ushort _index;
        private string _name;
        private string _description;
        private Wim_Windows_Info _windows;
        private ulong _dircount;
        private ulong _filecount;
        private ulong _totalbytes;
        private Wim_Image_CreationTime _creationtime;
        private Wim_Image_LastModificationTime _lastmodificationtime;
        private XmlAttribute[] _anyAttr;
        private XmlElement[] _anyField;

        [XmlAttribute(AttributeName="INDEX")]
        public ushort Index
        {
            get { return _index; }
            set { _index = value; }
        }
        [XmlElement(ElementName = "NAME")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [XmlElement(ElementName = "DESCRIPTION")]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        [XmlElement(ElementName = "WINDOWS")]
        public Wim_Windows_Info Windows
        {
            get { return _windows; }
            set { _windows = value; }
        }
        [XmlElement(ElementName = "DIRCOUNT")]
        public ulong DirCount
        {
            get { return _dircount; }
            set { _dircount = value; }
        }
        [XmlElement(ElementName = "FILECOUNT")]
        public ulong FileCount
        {
            get { return _filecount; }
            set { _filecount = value; }
        }
        [XmlElement(ElementName = "TOTALBYTES")]
        public ulong TotalBytes
        {
            get { return _totalbytes; }
            set { _totalbytes = value; }
        }
        [XmlElement(ElementName = "CREATIONTIME")]
        public Wim_Image_CreationTime CreationTime
        {
            get { return _creationtime; }
            set { _creationtime = value; }
        }
        [XmlElement(ElementName = "LASTMODIFICATIONTIME")]
        public Wim_Image_LastModificationTime LastModificationTime
        {
            get { return _lastmodificationtime; }
            set { _lastmodificationtime = value; }
        }

        [XmlAnyAttributeAttribute()]
        public XmlAttribute[] AnyAttr
        {
            get { return this._anyAttr; }
            set { this._anyAttr = value; }
        }

        [XmlAnyElementAttribute()]
        public XmlElement[] Any
        {
            get
            {
                return this._anyField;
            }
            set
            {
                this._anyField = value;
            }
        }
    }
    public class Wim_Windows_Info
    {
        private int _arch;
        private string _productname;
        private string _editionid;
        private string _installationtype;
        private string _hal;
        private string _producttype;
        private string _productsuite;
        private Wim_Windows_Languages _languages;
        private Wim_Windows_Version _version;
        private string _systemroot;
        private XmlAttribute[] _anyAttr;
        private XmlElement[] _anyField;

        [XmlElement(ElementName = "ARCH")]
        public int Architecture
        {
            get { return _arch; }
            set { _arch = value; }
        }
        [XmlElement(ElementName = "PRODUCTNAME")]
        public string ProductName
        {
            get { return _productname; }
            set { _productname = value; }
        }
        [XmlElement(ElementName = "EDITIIONID")]
        public string EditionID
        {
            get { return _editionid; }
            set { _editionid = value; }
        }
        [XmlElement(ElementName = "INSTALLATIONTYPE")]
        public string InstallationType
        {
            get { return _installationtype; }
            set { _installationtype = value; }
        }
        [XmlElement(ElementName = "HAL")]
        public string Hal
        {
            get { return _hal; }
            set { _hal = value; }
        }
        [XmlElement(ElementName = "PRODUCTTYPE")]
        public string ProductType
        {
            get { return _producttype; }
            set { _producttype = value; }
        }
        [XmlElement(ElementName = "PRODUCTSUITE")]
        public string ProductSuite
        {
            get { return _productsuite; }
            set { _productsuite = value; }
        }
        [XmlElement(ElementName = "LANGUAGES")]
        public Wim_Windows_Languages Languages
        {
            get { return _languages; }
            set { _languages = value; }
        }
        [XmlElement(ElementName = "VERSION")]
        public Wim_Windows_Version Version
        {
            get { return _version; }
            set { _version = value; }
        }
        [XmlElement(ElementName = "SYSTEMROOT")]
        public string SystemRoot
        {
            get { return _systemroot; }
            set { _systemroot = value; }
        }
 
        [XmlAnyAttributeAttribute()]
        public XmlAttribute[] AnyAttr
        {
            get { return this._anyAttr; }
            set { this._anyAttr = value; }
        }

        [XmlAnyElementAttribute()]
        public XmlElement[] Any
        {
            get
            {
                return this._anyField;
            }
            set
            {
                this._anyField = value;
            }
        }

    }
    public class Wim_Windows_Languages
    {
        private string[] _language;
        private string _default;
        [XmlElement(ElementName = "LANGUAGE")]
        public string[] Language
        {
            get { return _language; }
            set { _language = value; }
        }
        [XmlElement(ElementName = "DEFAULT")]
        public string Default
        {
            get { return _default; }
            set { _default = value; }
        }
    }
    public class Wim_Windows_Version
    {
        private string _major;
        private string _minor;
        private string _build;
        private string _spbuild;

        [XmlElement(ElementName = "MAJOR")]
        public string Major
        {
            get { return _major; }
            set { _major = value; }
        }
        [XmlElement(ElementName = "MINOR")]
        public string Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }
        [XmlElement(ElementName = "BUILD")]
        public string Build
        {
            get { return _build; }
            set { _build = value; }
        }
        [XmlElement(ElementName = "SPBUILD")]
        public string SpBuild
        {
            get { return _spbuild; }
            set { _spbuild = value; }
        }

    }
    public class Wim_Image_CreationTime : Wim_Image_Time
    {
    }
    public class Wim_Image_LastModificationTime : Wim_Image_Time
    {
    }
    public class Wim_Image_Time
    {
        private string _highpart;
        private string _lowpart;

        [XmlElement(ElementName = "HIGHPART")]
        public string HighPart
        {
            get { return _highpart; }
            set { _highpart = value; }
        }
        [XmlElement(ElementName = "LOWPART")]
        public string LowPart
        {
            get { return _lowpart; }
            set { _lowpart = value; }
        }
    }
    #endregion
}
