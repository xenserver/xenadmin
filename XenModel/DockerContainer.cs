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
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Model
{
    public class DockerContainer : XenObject<DockerContainer>, IComparable
    {
        public DockerContainer()
        {
        }

        public DockerContainer(VM parent, string uuid, string name, string description, string status, string container,
            string created, string image)
            : this()
        {
            this.parent = parent;
            this.Connection = parent.Connection;
            this.uuid = uuid;
            this.name_label = name;
            this.name_description = description;
            this.status = status;
            this.container = container;
            this.created = created;
            this.image = image;
        }

        public VM Parent
        {
            get { return parent; }
        }
        private readonly VM parent;

        /// <summary>
        /// Unique identifier/object reference
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    Changed = true;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid;

        /// <summary>
        /// a human-readable name
        /// </summary>
        public virtual string name_label
        {
            get { return _name_label; }
            set
            {
                if (!Helper.AreEqual(value, _name_label))
                {
                    _name_label = value;
                    Changed = true;
                    NotifyPropertyChanged("name_label");
                }
            }
        }
        private string _name_label;

        /// <summary>
        /// a notes field containing human-readable description
        /// </summary>
        public virtual string name_description
        {
            get { return _name_description; }
            set
            {
                if (!Helper.AreEqual(value, _name_description))
                {
                    _name_description = value;
                    Changed = true;
                    NotifyPropertyChanged("name_description");
                }
            }
        }
        private string _name_description;

        /// <summary>
        /// a notes field containing status description
        /// </summary>
        public virtual string status
        {
            get { return _status; }
            set
            {
                if (!Helper.AreEqual(value, _status))
                {
                    _status = value;
                    Changed = true;
                    NotifyPropertyChanged("status");
                }
            }
        }
        private string _status;

        /// <summary>
        /// a notes field containing container (id)
        /// </summary>
        public virtual string container
        {
            get { return _container; }
            set
            {
                if (!Helper.AreEqual(value, _container))
                {
                    _container = value;
                    Changed = true;
                    NotifyPropertyChanged("container");
                }
            }
        }
        private string _container;
        
        /// <summary>
        /// a notes field containing creation time
        /// </summary>
        public virtual string created
        {
            get { return _created; }
            set
            {
                if (!Helper.AreEqual(value, _created))
                {
                    _created = value;
                    Changed = true;
                    NotifyPropertyChanged("created");
                }
            }
        }
        private string _created;
        
        /// <summary>
        /// a notes field containing image information
        /// </summary>
        public virtual string image
        {
            get { return _image; }
            set
            {
                if (!Helper.AreEqual(value, _image))
                {
                    _image = value;
                    Changed = true;
                    NotifyPropertyChanged("image");
                }
            }
        }
        private string _image;

        public override string ToString()
        {
            return _name_label;
        }

        public override bool Equals(object obj)
        {
            DockerContainer other = obj as DockerContainer;

            return other != null &&
                name_label.Equals(other.name_label);
        }


        public override int GetHashCode()
        {
            return name_label.GetHashCode();
        }

        #region IComparable Members


        public override int CompareTo(DockerContainer other)
        {
            if (other == null)
                return 1;

            return StringUtility.NaturalCompare(_name_label, other._name_label);
        }

        #endregion

        public override void UpdateFrom(DockerContainer update)
        {
        }

        public override string SaveChanges(Session session, string _serverOpaqueRef, DockerContainer serverObject)
        {
            return String.Empty;
        }

        public override string Name
        {
            get
            {
                return name_label;
            }
        }

        public vm_power_state power_state
        {
            get {
                return status.Contains("Paused") 
                    ? vm_power_state.Paused
                    : status.StartsWith("Up") ? vm_power_state.Running : vm_power_state.Halted;
            }
        }
    }
}
