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
    public class VM_Docker_Version
    {
        private string _KernelVersion;
        public string KernelVersion
        {
            get { return _KernelVersion; }
            set
            {
                if (value != _KernelVersion)
                    _KernelVersion = value;
            }
        }

        private string _Arch;
        public string Arch
        {
            get { return _Arch; }
            set
            {
                if (value != _Arch)
                    _Arch = value;
            }
        }

        private string _ApiVersion;
        public string ApiVersion
        {
            get { return _ApiVersion; }
            set
            {
                if (value != _ApiVersion)
                    _ApiVersion = value;
            }
        }

        private string _Version;
        public string Version
        {
            get { return _Version; }
            set
            {
                if (value != _Version)
                    _Version = value;
            }
        }

        private string _GitCommit;
        public string GitCommit
        {
            get { return _GitCommit; }
            set
            {
                if (value != _GitCommit)
                    _GitCommit = value;
            }
        }

        private string _Os;
        public string Os
        {
            get { return _Os; }
            set
            {
                if (value != _Os)
                    _Os = value;
            }
        }

        private string _GoVersion;
        public string GoVersion
        {
            get { return _GoVersion; }
            set
            {
                if (value != _GoVersion)
                    _GoVersion = value;
            }
        }

        public VM_Docker_Version(string dockerVersion)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(dockerVersion);
            foreach (XmlNode docker_Version in doc.GetElementsByTagName("docker_version"))
            {
                var propertyNode = docker_Version.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "KernelVersion");
                if (propertyNode != null)
                    this.KernelVersion = propertyNode.InnerText;

                propertyNode = docker_Version.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Arch");
                if (propertyNode != null)
                    this.Arch = propertyNode.InnerText;

                propertyNode = docker_Version.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "ApiVersion");
                if (propertyNode != null)
                    this.ApiVersion = propertyNode.InnerText;

                propertyNode = docker_Version.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Version");
                if (propertyNode != null)
                    this.Version = propertyNode.InnerText;

                propertyNode = docker_Version.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "GitCommit");
                if (propertyNode != null)
                    this.GitCommit = propertyNode.InnerText;

                propertyNode = docker_Version.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "Os");
                if (propertyNode != null)
                    this.Os = propertyNode.InnerText;

                propertyNode = docker_Version.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "GoVersion");
                if (propertyNode != null)
                    this.GoVersion = propertyNode.InnerText;
            }
        }
    }
}
