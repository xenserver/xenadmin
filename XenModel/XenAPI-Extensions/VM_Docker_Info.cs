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

using System.Linq;
using System.Xml;

namespace XenAPI
{
    public class VM_Docker_Info
    {
        private string _NGoroutines;
        public string NGoroutines
        {
            get { return _NGoroutines; }
            set {
                if (value != _NGoroutines)
                    _NGoroutines = value;
            }
        }

        private string _DockerRootDir;
        public string DockerRootDir
        {
            get { return _DockerRootDir; }
            set {
                if (value != _DockerRootDir)
                    _DockerRootDir = value;
            }
        }

        private string _DriverStatus;
        public string DriverStatus
        {
            get { return _DriverStatus; }
            set {
                if (value != _DriverStatus)
                    _DriverStatus = value;
            }
        }

        private string _OperatingSystem;
        public string OperatingSystem
        {
            get { return _OperatingSystem; }
            set {
                if (value != _OperatingSystem)
                    _OperatingSystem = value;
            }
        }

        private string _Containers;
        public string Containers
        {
            get { return _Containers; }
            set {
                if (value != _Containers)
                    _Containers = value;
            }
        }

        private string _MemTotal;
        public string MemTotal
        {
            get { return _MemTotal; }
            set {
                if (value != _MemTotal)
                    _MemTotal = value;
            }
        }

        private string _Driver;
        public string Driver
        {
            get { return _Driver; }
            set {
                if (value != _Driver)
                    _Driver = value;
            }
        }

        private string _IndexServerAddress;
        public string IndexServerAddress
        {
            get { return _IndexServerAddress; }
            set {
                if (value != _IndexServerAddress)
                    _IndexServerAddress = value;
            }
        }

        private string _InitPath;
        public string InitPath
        {
            get { return _InitPath; }
            set {
                if (value != _InitPath)
                    _InitPath = value;
            }
        }

        private string _ExecutionDriver;
        public string ExecutionDriver
        {
            get { return _ExecutionDriver; }
            set {
                if (value != _ExecutionDriver)
                    _ExecutionDriver = value;
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set {
                if (value != _Name)
                    _Name = value;
            }
        }

        private string _NCPU;
        public string NCPU
        {
            get { return _NCPU; }
            set {
                if (value != _NCPU)
                    _NCPU = value;
            }
        }

        private string _Debug;
        public string Debug
        {
            get { return _Debug; }
            set {
                if (value != _Debug)
                    _Debug = value;
            }
        }

        private string _ID;
        public string ID
        {
            get { return _ID; }
            set {
                if (value != _ID)
                    _ID = value;
            }
        }

        private string _IPv4Forwarding;
        public string IPv4Forwarding
        {
            get { return _IPv4Forwarding; }
            set {
                if (value != _IPv4Forwarding)
                    _IPv4Forwarding = value;
            }
        }

        private string _KernelVersion;
        public string KernelVersion
        {
            get { return _KernelVersion; }
            set {
                if (value != _KernelVersion)
                    _KernelVersion = value;
            }
        }

        private string _NFd;
        public string NFd
        {
            get { return _NFd; }
            set {
                if (value != _NFd)
                    _NFd = value;
            }
        }

        private string _InitSha1;
        public string InitSha1
        {
            get { return _InitSha1; }
            set {
                if (value != _InitSha1)
                    _InitSha1 = value;
            }
        }

        private string _Labels;
        public string Labels
        {
            get { return _Labels; }
            set {
                if (value != _Labels)
                    _Labels = value;
            }
        }

        private string _MemoryLimit;
        public string MemoryLimit
        {
            get { return _MemoryLimit; }
            set {
                if (value != _MemoryLimit)
                    _MemoryLimit = value;
            }
        }

        private string _SwapLimit;
        public string SwapLimit
        {
            get { return _SwapLimit; }
            set {
                if (value != _SwapLimit)
                    _SwapLimit = value;
            }
        }

        private string _Images;
        public string Images
        {
            get { return _Images; }
            set {
                if (value != _Images)
                    _Images = value;
            }
        }

        private string _NEventsListener;
        public string NEventsListener
        {
            get { return _NEventsListener; }
            set {
                if (value != _NEventsListener)
                    _NEventsListener = value;
            }
        }

        public VM_Docker_Info(string dockerInfo)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dockerInfo);
            foreach (XmlNode docker_info in doc.GetElementsByTagName("docker_info"))
            {
                var propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "NGoroutines");
                if (propertyNode != null)
                    this.NGoroutines = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "DockerRootDir");
                if (propertyNode != null)
                    DockerRootDir = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "DriverStatus");
                if (propertyNode != null)
                    DriverStatus = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "OperatingSystem");
                if (propertyNode != null)
                    OperatingSystem = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Containers");
                if (propertyNode != null)
                    Containers = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "MemTotal");
                if (propertyNode != null)
                    MemTotal = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Driver");
                if (propertyNode != null)
                    Driver = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "IndexServerAddress");
                if (propertyNode != null)
                    IndexServerAddress = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "InitPath");
                if (propertyNode != null)
                    InitPath = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "ExecutionDriver");
                if (propertyNode != null)
                    ExecutionDriver = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Name");
                if (propertyNode != null)
                    Name = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "NCPU");
                if (propertyNode != null)
                    NCPU = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Debug");
                if (propertyNode != null)
                    Debug = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "ID");
                if (propertyNode != null)
                    ID = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "IPv4Forwarding");
                if (propertyNode != null)
                    IPv4Forwarding = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "KernelVersion");
                if (propertyNode != null)
                    KernelVersion = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "NFd");
                if (propertyNode != null)
                    NFd = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "InitSha1");
                if (propertyNode != null)
                    InitSha1 = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Labels");
                if (propertyNode != null)
                    Labels = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "MemoryLimit");
                if (propertyNode != null)
                    MemoryLimit = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "SwapLimit");
                if (propertyNode != null)
                    SwapLimit = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Images");
                if (propertyNode != null)
                    Images = propertyNode.InnerText;

                propertyNode = docker_info.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "NEventsListener");
                if (propertyNode != null)
                    NEventsListener = propertyNode.InnerText;
            }
        }
    }
}
