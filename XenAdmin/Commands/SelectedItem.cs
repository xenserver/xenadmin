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

using XenAdmin.Controls;

using XenAPI;
using XenAdmin.Network;
using XenAdmin.Core;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Represents an item selected in the main treeview or the search results. This can either be a IXenObject or a grouping tag.
    /// </summary>
    public class SelectedItem
    {
        private readonly IXenObject _xenObject;
        private readonly Host _hostAncestor;
        private readonly Pool _poolAncestor;
        private readonly IXenConnection _connection;
        private readonly GroupingTag _groupingTag;
        private readonly GroupingTag _groupAncestor;
        private readonly VirtualTreeNode _rootNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItem"/> class.
        /// </summary>
        /// <param name="groupingTag">The grouping tag that is selected.</param>
        public SelectedItem(GroupingTag groupingTag, VirtualTreeNode rootNode)
        {
            _groupingTag = groupingTag;
            _rootNode = rootNode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItem"/> class.
        /// </summary>
        /// <param name="xenObject">The xen object that is selected.</param>
        /// <param name="connection">The connection of the xen object.</param>
        /// <param name="hostAncestor">The host ancestor of the xen object in the tree.</param>
        /// <param name="poolAncestor">The pool ancestor of the xen object in the tree.</param>
        public SelectedItem(IXenObject xenObject, IXenConnection connection, Host hostAncestor, Pool poolAncestor)
        {
            _xenObject = xenObject;
            _hostAncestor = hostAncestor;
            _poolAncestor = poolAncestor;
            _connection = connection;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItem"/> class.
        /// </summary>
        /// <param name="xenObject">The xen object that is selected.</param>
        /// <param name="connection">The connection of the xen object.</param>
        /// <param name="hostAncestor">The host ancestor of the xen object in the tree.</param>
        /// <param name="poolAncestor">The pool ancestor of the xen object in the tree.</param>
        /// <param name="groupAncestor">In Objects view this is the type under which
        /// the object is grouped.</param>
        public SelectedItem(IXenObject xenObject, IXenConnection connection, Host hostAncestor,
            Pool poolAncestor, GroupingTag groupAncestor, VirtualTreeNode rootNode)
            : this(xenObject, connection, hostAncestor, poolAncestor)
        {
            _groupAncestor = groupAncestor;
            _rootNode = rootNode;
        }

        public VirtualTreeNode RootNode
        {
            get { return _rootNode; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItem"/> class.
        /// </summary>
        /// <param name="xenObject">The xen object that is selected.</param>
        public SelectedItem(IXenObject xenObject)
        {
            _xenObject = xenObject;

            if (_xenObject == null)
            {
                return;
            }
            
            _connection = xenObject.Connection;

            if (_connection == null)
            {
                return;
            }
            else if (xenObject is Host)
            {
                Host host = (Host)xenObject;
                _hostAncestor = host;
                _poolAncestor = Helpers.GetPool(_connection);
            }
            else if (xenObject is Pool)
            {
                Pool pool = (Pool)xenObject;
                _poolAncestor = pool;
            }
            else if (xenObject is SR)
            {
                SR sr = (SR)xenObject;
                _hostAncestor = sr.Home;
                _poolAncestor = Helpers.GetPool(_connection);
            }
            else if (xenObject is VM)
            {
                VM vm = (VM)xenObject;
                _hostAncestor = vm.Home();
                _poolAncestor = Helpers.GetPool(_connection);
            }
			else if (xenObject is VM_appliance)
			{
				_poolAncestor = Helpers.GetPool(_connection);
			}
        }

        public IXenObject XenObject
        {
            get
            {
                return _xenObject;
            }
        }

        public Host HostAncestor
        {
            get
            {
                return _hostAncestor;
            }
        }

        public Pool PoolAncestor
        {
            get
            {
                return _poolAncestor;
            }
        }

        public IXenConnection Connection
        {
            get
            {
                return _connection;
            }
        }


        public GroupingTag GroupingTag
        {
            get
            {
                return _groupingTag;
            }
        }

        public GroupingTag GroupAncestor
        {
            get { return _groupAncestor; }
        }

        /// <summary>
        /// Gets the value of the XenObject or Grouping tag that this object represents.
        /// </summary>
        public object Value
        {
            get
            {
                if (XenObject != null)
                {
                    return XenObject;
                }
                return GroupingTag;
            }
        }
    }
}
