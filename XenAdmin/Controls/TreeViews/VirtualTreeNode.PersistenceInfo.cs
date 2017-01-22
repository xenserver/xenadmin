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
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace XenAdmin.Controls
{
    partial class VirtualTreeNode
    {
        [DebuggerDisplay("{Tag}")]
        public class PersistenceInfo : IEquatable<PersistenceInfo>
        {
            public ReadOnlyCollection<object> Path { get; private set; }
            public ReadOnlyCollection<object> PathToMaximalSubTree { get; private set; }
            public VirtualTreeNode Node { get; private set; }
            public object Tag { get; private set; }

            public PersistenceInfo(VirtualTreeNode node)
            {
                Util.ThrowIfParameterNull(node, "node");

                Node = node;
                Path = new ReadOnlyCollection<object>(GetPath(node));
                
                var maxSubTree = MaximalSubTree(node, node.Tag);
                PathToMaximalSubTree = new ReadOnlyCollection<object>(maxSubTree == null ? new List<object>() : GetPath(maxSubTree));
                
                Tag = node.Tag;
            }

            private static List<object> GetPath(VirtualTreeNode node)
            {
                var path = new List<object>();

                if (node.Tag != null && node.Parent != null)
                    path.Add(node.Tag);

                path.AddRange(new List<VirtualTreeNode>(node.Ancestors).FindAll(o => o.Parent != null).ConvertAll(n => n.Tag).FindAll(o => o != null));
                path.Reverse();
                return path;
            }

            /// <summary>
            /// Find the maximal sub-tree under which the object appears only once
            /// </summary>
            /// <param name="me"></param>
            /// <param name="o"></param>
            /// <returns></returns>
            private VirtualTreeNode MaximalSubTree(VirtualTreeNode me, Object o)
            {
                if (me.Parent == null)
                    return me;

                foreach (VirtualTreeNode sibling in me.Parent.Nodes)
                {
                    if (sibling == me)
                        continue;

                    if (ObjectExistsUnder(o, sibling))
                        return me;
                }

                return MaximalSubTree(me.Parent, o);
            }

            private bool ObjectExistsUnder(Object o, VirtualTreeNode parent)
            {
                foreach (VirtualTreeNode child in parent.Nodes)
                {
                    if (child.Tag == o)
                        return true;

                    if (ObjectExistsUnder(o, child))
                        return true;
                }

                return false;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as PersistenceInfo);
            }

            public override int GetHashCode()
            {
                if (Tag == null)
                {
                    return string.Empty.GetHashCode();
                }
                return Tag.GetHashCode();
            }

            #region IEquatable<SelectionInfo> Members

            public bool Equals(PersistenceInfo other)
            {
                if (other != null)
                {
                    if (Tag == null && other.Tag == null)
                    {
                        return true;
                    }
                    else if (Tag != null && other.Tag != null && Tag.Equals(other.Tag))
                    {
                        return true;
                    }
                }
                return false;
            }

            #endregion
        }
    }
}
