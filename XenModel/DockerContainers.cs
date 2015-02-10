/* Copyright (c) Citrix Systems Inc. 
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
using System.ComponentModel;
using System.Diagnostics;
using XenAPI;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using System.Xml;
using System.Linq;


namespace XenAdmin.Model
{
    public class DockerContainers
    {
        static DockerContainers()
        {
        }

        public static void InitDockerContainers()
        {
            Trace.Assert(InvokeHelper.Synchronizer != null);
            CollectionChangedWithInvoke = InvokeHelper.InvokeHandler(CollectionChanged);

            ConnectionsManager.XenConnections.CollectionChanged += XenConnections_CollectionChanged;

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                AddConnection(connection);
        }

        private static void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            InvokeHelper.BeginInvoke(() =>
                                                        {
                                                            IXenConnection connection = e.Element as IXenConnection;
                                                            if (connection == null)
                                                                return;

                                                            switch (e.Action)
                                                            {
                                                                case CollectionChangeAction.Add:
                                                                    AddConnection(connection);
                                                                    break;

                                                                case CollectionChangeAction.Remove:
                                                                    RemoveConnection(connection);
                                                                    break;
                                                            }
                                                        });
        }

        private static CollectionChangeEventHandler CollectionChangedWithInvoke;
        private static void AddConnection(IXenConnection connection)
        {
            connection.Cache.RegisterCollectionChanged<VM>(CollectionChangedWithInvoke);

            connection.XenObjectsUpdated += connection_XenObjectsUpdated;

            InvokeHelper.Invoke(delegate()
            {
                UpdateAll(connection.Cache.VMs);
            });
        }

        private static void RemoveConnection(IXenConnection connection)
        {
            connection.Cache.DeregisterCollectionChanged<VM>(CollectionChangedWithInvoke);

            connection.XenObjectsUpdated -= connection_XenObjectsUpdated;
        }

        private static void CollectionChanged(Object sender, CollectionChangeEventArgs e)
        {
            InvokeHelper.AssertOnEventThread();

            IXenObject ixmo = e.Element as IXenObject;
            if (ixmo == null)
                return;

            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    ixmo.PropertyChanged += ServerXenObject_PropertyChanged;
                    UpdateDockerContainer(ixmo);
                    break;

                case CollectionChangeAction.Remove:
                    ixmo.PropertyChanged -= ServerXenObject_PropertyChanged;
                    RemoveObject(ixmo);
                    break;

                default:
                    System.Diagnostics.Trace.Assert(false);
                    break;
            }
        }

        static void connection_XenObjectsUpdated(object sender, EventArgs e)
        {
            IXenConnection connection = sender as IXenConnection;
            if (connection == null)
                return;

            connection.Cache.CheckDockerContainersBatchChange();
        }

        private static void ServerXenObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IXenObject xenObject = (IXenObject) sender;
            if (e.PropertyName == "other_config")
            {
                UpdateDockerContainer(xenObject);
            }
        }

        private static void RemoveObject(IXenObject ixmo)
        {
            InvokeHelper.AssertOnEventThread();

            if (ixmo as VM == null)
                return;

            IXenConnection connection = ixmo.Connection;
            connection.Cache.UpdateDockerContainersForVM(new List<DockerContainer>(), (VM)ixmo);
        }

        private static void UpdateAll(IXenObject[] ixmos)
        {
            foreach (IXenObject ixmo in ixmos)
            {
                ixmo.PropertyChanged -= ServerXenObject_PropertyChanged;
                ixmo.PropertyChanged += ServerXenObject_PropertyChanged;
                UpdateDockerContainer(ixmo);
            }
        }

        private static void UpdateDockerContainer(IXenObject ixmo)
        {
            InvokeHelper.AssertOnEventThread();
            
            if (ixmo as VM == null)
                return;
            
            IXenConnection connection = ixmo.Connection;

            var dockerVMs = GetDockerVMs(ixmo);
            connection.Cache.UpdateDockerContainersForVM(dockerVMs, (VM)ixmo);
        }

        public static List<DockerContainer> GetContainersFromOtherConfig(VM vm)
        {
            var containers = new List<DockerContainer>();
            var other_config = vm.other_config;
            if (other_config.ContainsKey("docker_ps"))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(other_config["docker_ps"]);

                foreach (XmlNode entry in doc.GetElementsByTagName("entry"))
                {
                    //uuid
                    string id = "";
                    var propertyNode = entry.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "id");
                    if (propertyNode != null)
                        id = propertyNode.InnerText;

                    string name = "";
                    propertyNode = entry.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "names");
                    if (propertyNode != null)
                        name = propertyNode.InnerText;

                    string status = "";
                    propertyNode = entry.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "status");
                    if (propertyNode != null)
                        status = propertyNode.InnerText;

                    string container = "";
                    propertyNode = entry.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "container");
                    if (propertyNode != null)
                        container = propertyNode.InnerText;

                    string created = "";
                    propertyNode = entry.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "created");
                    if (propertyNode != null)
                        created = propertyNode.InnerText;

                    string image = "";
                    propertyNode = entry.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "image");
                    if (propertyNode != null)
                        image = propertyNode.InnerText;

                    string command = "";
                    propertyNode = entry.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "command");
                    if (propertyNode != null)
                        command = propertyNode.InnerText;

                    string ports = "";
                    propertyNode = entry.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "ports");
                    if (propertyNode != null)
                        ports = propertyNode.InnerText;

                    DockerContainer dockerContainer = new DockerContainer(vm, id, name, string.Empty, status, container, created, image, command, ports);
                    
                    // update existing container or add a new one
                    DockerContainer existingContainer = vm.Connection.Resolve(new XenRef<DockerContainer>(id));
                    if (existingContainer != null)
                    {
                        existingContainer.UpdateFrom(dockerContainer);
                        containers.Add(existingContainer);
                    }
                    else
                        containers.Add(dockerContainer);
                }
            }
            return containers;
        }

        public static ComparableList<DockerContainer> GetDockerVMs(IXenObject o)
        {
            var vm = o as VM;
            if (vm != null)
            {
                return new ComparableList<DockerContainer>(DockerContainers.GetContainersFromOtherConfig(vm));
            }
            return new ComparableList<DockerContainer>();
        }
    }
}
