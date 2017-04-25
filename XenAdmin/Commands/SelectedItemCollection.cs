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
using System.Collections.ObjectModel;

using XenAdmin.Controls;

using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;

namespace XenAdmin.Commands
{
    /// <summary>
    /// A read-only collection of <see cref="SelectedItem"/>s.
    /// </summary>
    public class SelectedItemCollection : ReadOnlyCollection<SelectedItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItemCollection"/> class.
        /// The collection has no contents.
        /// </summary>
        public SelectedItemCollection()
            : base(new List<SelectedItem>())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItemCollection"/> class with one item
        /// </summary>
        /// <param name="item">The itemsthat will populate the collection.</param>
        public SelectedItemCollection(SelectedItem item)
            : base(new List<SelectedItem> {item})
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItemCollection"/> class.
        /// </summary>
        /// <param name="items">The items that will populate the collection.</param>
        public SelectedItemCollection(IEnumerable<SelectedItem> items)
            : base(new List<SelectedItem>(items))
        {
        }

        /// <summary>
        /// Returns a list of GroupingTags and IXenObjects that
        /// this collection represents.
        /// </summary>
        /// <returns></returns>
        public List<object> AsObjects()
        {
            return new List<SelectedItem>(this).ConvertAll(s => s.Value);
        }

        /// <summary>
        /// Gets a list of GroupingTags that this collection represents.
        /// </summary>
        public List<GroupingTag> AsGroupingTags()
        {
            List<GroupingTag> output = new List<GroupingTag>();
            foreach (SelectedItem item in this)
            {
                output.Add(item.GroupingTag);
            }
            return output;
        }

        /// <summary>
        /// Gets a list of IXenObjects that this collection represents.
        /// </summary>
        public List<IXenObject> AsXenObjects()
        {
            return AsXenObjects<IXenObject>();
        }

        /// <summary>
        /// Gets a list of IXenObjects that this collection represents.
        /// </summary>
        public List<T> AsXenObjects<T>() where T : IXenObject
        {
            List<T> output = new List<T>();
            foreach (SelectedItem item in this)
            {
                if (item.XenObject is T)
                    output.Add((T)item.XenObject);
            }
            return output;
        }

        /// <summary>
        /// Gets a list of IXenObjects that this collection represents.
        /// </summary>
        /// <param name="filter">A Predicate for filtering the list.</param>
        public List<T> AsXenObjects<T>(Predicate<T> filter) where T : IXenObject
        {
            List<T> output = new List<T>();
            foreach (SelectedItem item in this)
            {
                if (item.XenObject is T)
                {
                    T obj = (T)item.XenObject;

                    if (filter(obj))
                        output.Add(obj);
                }
            }
            return output;
        }

        /// <summary>
        /// Determines whether this collection contains only one item of the specified type. T
        /// can either be IXenObject or GroupingTag.
        /// </summary>
        /// <param name="predicate">A Predicate that must be satisfied.</param>
        public bool ContainsOneItemOfType<T>(Predicate<T> predicate) where T : class
        {
            if (ContainsOneItemOfType<T>())
            {
                object item = (this[0].XenObject as T) ?? (this[0].GroupingTag as T);
                return item != null && predicate((T)item);
            }
            return false;
        }

        /// <summary>
        /// Determines whether this collection contains only one item of the specified type. T
        /// can either be IXenObject or GroupingTag.
        /// </summary>
        public bool ContainsOneItemOfType<T>() where T : class
        {
            return Count == 1 && ItemIs<T>(0);
        }

        /// <summary>
        /// Determines whether the item at the specified index is of the specified type. T
        /// can either be IXenObject or GroupingTag.
        /// </summary>
        private bool ItemIs<T>(int index) where T : class
        {
            return Count >= index && (this[index].XenObject is T || this[index].GroupingTag is T);
        }

        /// <summary>
        /// Determines whether all items of the collection are of the specified type. T
        /// can either be IXenObject or GroupingTag.
        /// </summary>
        public bool AllItemsAre<T>() where T : class
        {
            if (Count == 0)
            {
                return false;
            }

            for (int i = 0; i < Count; i++)
            {
                if (!ItemIs<T>(i))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether all items of the collection are of the specified type. T
        /// can either be IXenObject or GroupingTag.
        /// </summary>
        /// <param name="predicate">A predicate that must be satisfied.</param>
        public bool AllItemsAre<T>(Predicate<T> predicate) where T : class
        {
            if (Count == 0)
            {
                return false;
            }

            for (int i = 0; i < Count; i++)
            {
                object item = (this[i].XenObject as T) ?? (this[i].GroupingTag as T);

                if (!ItemIs<T>(i) || !predicate((T)item))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Determines if at least one item in the collection satisfies the specified condition. T
        /// is a type the implements IXenObject.
        /// </summary>
        /// <param name="doThis">The condition to be satisfied.</param>
        public bool AtLeastOneXenObjectCan<T>(Predicate<T> doThis) where T : IXenObject
        {
            Util.ThrowIfParameterNull(doThis, "doThis");

            foreach (SelectedItem item in this)
            {
                if (doThis((T)item.XenObject))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the first item whether it be either a IXenObject or a GroupingTag. Returns null if the collection is empty.
        /// </summary>
        public object First
        {
            get
            {
                return Count == 0 ? null : this[0].Value;
            }
        }

        /// <summary>
        /// Gets the first item and casts it to an IXenObject. Can return null.
        /// </summary>
        public IXenObject FirstAsXenObject
        {
            get
            {
                return First as IXenObject;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the first item is a T.
        /// </summary>
        public bool FirstIs<T>() where T : IXenObject
        {
            return First is T;
        }

        /// <summary>
        /// Gets a value indicating whether the first item is a live host.
        /// </summary>
        public bool FirstIsLiveHost
        {
            get
            {
                return FirstIs<Host>() && ((Host)this[0].XenObject).IsLive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the first item is a real VM (not a template).
        /// </summary>
        public bool FirstIsRealVM
        {
            get
            {
                return FirstIs<VM>() && !((VM)this[0].XenObject).is_a_template;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the first item is a template.
        /// </summary>
        public bool FirstIsTemplate
        {
            get
            {
                return FirstIs<VM>() && ((VM)this[0].XenObject).is_a_template && !((VM)this[0].XenObject).is_a_snapshot;
            }
        }

        /// <summary>
        /// Gets the connection of first item. If the collection is empty then null is returned.
        /// </summary>
        public IXenConnection GetConnectionOfFirstItem()
        {
            if (Count > 0)
            {
                return this[0].Connection;
            }
            return null;
        }

        /// <summary>
        /// Gets the common host ancestor for the selection. If the selected items have different host ancestors then null is returned.
        /// </summary>
        public Host HostAncestor
        {
            get
            {
                Host hostAncestor = null;
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].HostAncestor == null)
                    {
                        return null;
                    }
                    else if (i > 0 && this[i].HostAncestor != hostAncestor)
                    {
                        return null;
                    }

                    hostAncestor = this[i].HostAncestor;
                }
                return hostAncestor;
            }
        }

        /// <summary>
        /// Gets the common pool ancestor for the selection. If the selected items have different pool ancestors then null is returned.
        /// </summary>
        public Pool PoolAncestor
        {
            get
            {
                Pool poolAncestor = null;
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].PoolAncestor == null)
                    {
                        return null;
                    }
                    else if (i > 0 && this[i].PoolAncestor != poolAncestor)
                    {
                        return null;
                    }

                    poolAncestor = this[i].PoolAncestor;
                }
                return poolAncestor;
            }
        }

        /// <summary>
        /// Gets the common group ancestor for the selection. If the selected
        /// items have different group ancestors then null is returned.
        /// </summary>
        public GroupingTag GroupAncestor
        {
            get
            {
                GroupingTag groupAncestor = null;
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].GroupAncestor == null)
                        return null;
                    
                    if (i > 0 && this[i].GroupAncestor != groupAncestor)
                        return null;

                    groupAncestor = this[i].GroupAncestor;
                }
                return groupAncestor;
            }
        }

        /// <summary>
        /// Gets the common host ancestor for the selection without using the individual objects' HostAncestor.
        /// </summary>
        public Host HostAncestorFromConnection
        {
            get
            {
                Host hostAncestor = Helpers.GetHostAncestor(First as IXenObject);
                
                foreach (var item in AsXenObjects())
                {
                    if (hostAncestor != Helpers.GetHostAncestor(item))
                        return null;
                }

                return hostAncestor;
            }
        }

        /// <summary>
        /// Gets the common pool ancestor for the selection without using the individual objects' PoolAncestor.
        /// </summary>
        public Pool PooAncestorFromConnection
        {
            get
            {
                var connection = GetConnectionOfAllItems();
                return connection == null ? null : Helpers.GetPoolOfOne(connection);
            }
        }

        /// <summary>
        /// Gets the selected items connection. If the selection is cross-pool then null is returned.
        /// </summary>
        public IXenConnection GetConnectionOfAllItems()
        {
            IXenConnection connection = GetConnectionOfFirstItem();

            foreach (var item in this)
            {
                if (connection != item.Connection)
                    return null;
            }

            return connection;
        }

        public VirtualTreeNode RootNode
        {
            get
            {
                return Items.Count > 0 ? Items[0].RootNode : null;
            }
        }
    }
}
