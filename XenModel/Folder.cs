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
using System.Diagnostics;
using System.Text;

using XenAPI;

using XenAdmin.Core;

namespace XenAdmin.Model
{

    public class Folder : XenObject<Folder>, IComparable
    {
        public bool Grey = false;

        private readonly String _name_label;
        private readonly Folder parent;
        private List<IXenObject> xenObjects;

        public Folder(Folder parent, String name)
        {
            this.parent = parent;
            this._name_label = name;
            xenObjects = new List<IXenObject>();
        }

        public String name_label
        {
            get { return _name_label; }
        }

        public IXenObject[] XenObjects
        {
            get
            {
                lock (xenObjects)
                    return xenObjects.ToArray();
            }
        }

        public IXenObject[] RecursiveXenObjects
        {
            get
            {
                List<IXenObject> objects = new List<IXenObject>();
                lock (xenObjects)
                {
                    foreach (IXenObject o in xenObjects)
                    {
                        if (o is Folder)
                        {
                            Folder f = o as Folder;
                            objects.AddRange(f.RecursiveXenObjects);
                        }
                        else
                            objects.Add(o);
                    }
                }
                return objects.ToArray();
            }
        }

        public int XenObjectsCount
        {
            get
            {
                lock (xenObjects)
                    return xenObjects.Count;
            }
        }

        public Folder Parent
        {
            get { return parent; }
        }

        public void AddObject(IXenObject ixmo)
        {
            lock (xenObjects)
                if (!xenObjects.Contains(ixmo))
                    xenObjects.Add(ixmo);
        }

        public bool RemoveObject(IXenObject ixmo)
        {
            lock (xenObjects)
                return xenObjects.Remove(ixmo);
        }

        public override string ToString()
        {
            return _name_label;
        }

        public bool IsRootFolder
        {
            get
            {
                return (Parent == null);
            }
        }

        public override bool Equals(object obj)
        {
            Folder other = obj as Folder;

            return other != null &&
                FullPath().Equals(other.FullPath());
        }

        private string FullPath()
        {
            if (this.Path != "/")
                return string.Format("{0}/{1}", this.Path, _name_label);
            return string.Format("/{0}",name_label);
        }


        public override int GetHashCode()
        {
            return FullPath().GetHashCode();
        }

        #region IComparable Members


        public override int CompareTo(Folder other)
        {
            if (other == null)
                return 1;

            return StringUtility.NaturalCompare(_name_label, other._name_label);
        }

        #endregion

        public override void UpdateFrom(Folder update)
        {
        }

        public override string SaveChanges(Session session, string _serverOpaqueRef, Folder serverObject)
        {
            return String.Empty;
        }

        public bool IsChildOf(IXenObject target)
        {
            if (Parent == null)
                return false;

            if (Parent.opaque_ref == target.opaque_ref)
                return true;

            return Parent.IsChildOf(target);
        }

        public override string Name
        {
            get
            {
                return name_label;
            }
        }
    }
}
