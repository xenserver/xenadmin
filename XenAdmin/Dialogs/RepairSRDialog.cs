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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using XenAPI;

using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Actions;
using XenAdmin.Network;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace XenAdmin.Dialogs
{
    public partial class RepairSRDialog : DialogWithProgress
    {
        private readonly ReadOnlyCollection<SR> _srList;
        private readonly List<AsyncAction> _repairActions = new List<AsyncAction>();
        private AsyncAction _repairAction;
        private Font BoldFont;
        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;
        private readonly CollectionChangeEventHandler PBD_CollectionChangedWithInvoke;

        public AsyncAction RepairAction
        {
            get
            {
                return _repairAction;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepairSRDialog"/> class.
        /// </summary>
        /// <param name="sr">The SR to be repaired.</param>
        public RepairSRDialog(SR sr)
            : this(new SR[] { sr })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepairSRDialog"/> class.
        /// </summary>
        /// <param name="srs">The SRs to be repaired.</param>
        public RepairSRDialog(IEnumerable<SR> srs)
        {
            Util.ThrowIfEnumerableParameterNullOrEmpty(srs, "srs");
            BoldFont = new Font(Font, FontStyle.Bold);
            List<SR> srList = new List<SR>(srs);
            srList.Sort();
            _srList = new ReadOnlyCollection<SR>(srList);
            InitializeComponent();

            if (_srList.Count == 1)
            {
                Text = string.Format(Messages.REPAIR_SR_DIALOG_TITLE_SINGLE, _srList[0].Name);
            }
            else
            {
                Text = Messages.REPAIR_SR_DIALOG_TITLE_MULTIPLE;
            }

            Shrink();

            Build();

            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Host_CollectionChanged);
            PBD_CollectionChangedWithInvoke=Program.ProgramInvokeHandler(PBD_CollectionChanged);
            foreach (SR sr in srList)
            {
                sr.PropertyChanged += Server_PropertyChanged;
                sr.Connection.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
                sr.Connection.Cache.RegisterCollectionChanged<PBD>(PBD_CollectionChangedWithInvoke);
            }
        }

        private void PBD_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.Invoke(this, Build);
        }

        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.Invoke(this, Build);
        }

        private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.Invoke(this, Build);
        }

        private void hostsTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void hostsTreeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true;
            RepairTreeNode node = (RepairTreeNode)e.Node;

            if (node.Host != null)
            {
                string status = Messages.REPAIR_SR_DIALOG_CONNECTION_MISSING;
                Color statusColor = Color.Red;
                Font statusFont = BoldFont;

                if (node.PBD != null)
                {
                    status = node.PBD.currently_attached ? Messages.CONNECTED : Messages.REPAIR_SR_DIALOG_UNPLUGGED;
                    statusColor = node.PBD.currently_attached ? Color.Green : Color.Red;
                    statusFont = node.PBD.currently_attached ? Font : BoldFont;
                }

                using (var brush = new SolidBrush(statusColor))
                    e.Graphics.DrawString(status, statusFont, brush, new Rectangle(e.Bounds.Right, e.Bounds.Top, Right - e.Bounds.Right, e.Bounds.Height));
            }
        }

        public void Build()
        {
            treeView.BeginUpdate();
            RepairButton.Enabled = false;
            bool firstTime = treeView.Nodes.Count == 0;
            bool anythingBroken = false;
            bool hostsAvailable = false;
            for (int i = 0; i < _srList.Count; i++)
            {
                // persist expanded-state of node, or default to true
                bool isExpanded = true;

                if (_srList[i].IsBroken() || !_srList[i].MultipathAOK)
                    anythingBroken = true;

                if (firstTime)
                {
                    treeView.Nodes.Add(new RepairTreeNode(_srList[i], null, null));
                }
                else
                {
                    isExpanded = treeView.Nodes[i].IsExpanded;
                }

                RepairTreeNode srNode = (RepairTreeNode)treeView.Nodes[i];
                srNode.Nodes.Clear();

                
                if (imageList.Images.ContainsKey(srNode.SR.opaque_ref))
                {
                    imageList.Images.RemoveByKey(srNode.SR.opaque_ref);
                }
                imageList.Images.Add(srNode.SR.opaque_ref, Images.GetImage16For(srNode.SR));

                srNode.ImageKey = srNode.SR.opaque_ref;

                List<Host> hosts = new List<Host>(srNode.SR.Connection.Cache.Hosts);
                hosts.Sort();
                foreach (Host host in hosts)
                {
                    if (host != null)
                    {
                        Host storageHost = srNode.SR.GetStorageHost();

                        if (srNode.SR.shared || (storageHost != null && host.opaque_ref == storageHost.opaque_ref))
                        {
                            PBD pdb = null;

                            foreach (PBD p in srNode.SR.Connection.ResolveAll<PBD>(srNode.SR.PBDs))
                            {
                                if (srNode.SR.Connection.Resolve<Host>(p.host) == host)
                                {
                                    pdb = p;
                                }
                            }

                            RepairTreeNode hostNode = new RepairTreeNode(srNode.SR, host, pdb);
                            srNode.Nodes.Add(hostNode);

                            if (imageList.Images.ContainsKey(host.opaque_ref))
                            {
                                imageList.Images.RemoveByKey(host.opaque_ref);
                            }
                            imageList.Images.Add(host.opaque_ref, Images.GetImage16For(host));

                            hostNode.ImageKey = host.opaque_ref;
                            hostsAvailable = true;
                        }
                    }
                }

                if (isExpanded)
                {
                    srNode.Expand();
                }
                else
                {
                    srNode.Collapse();
                }
            }
            // we only want to enable the repair button if
            // - there is a broken sr
            // - we have successfully found the right hosts to patch up pbds for
            // - we are not in the process of repairing already
            RepairButton.Enabled = anythingBroken && hostsAvailable && !ActionInProgress;


            treeView.EndUpdate();
        }

        private Boolean ActionInProgress
        {
            get
            {
                return (_repairAction != null && !_repairAction.IsCompleted);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RepairButton_Click(object sender, EventArgs e)
        {
            RepairButton.Enabled = false;
            CloseButton.Text = Messages.CLOSE;

            if (_srList.Count == 1)
            {
                _repairAction = new SrRepairAction(_srList[0].Connection, _srList[0], false);
            }
            else
            {
                List<AsyncAction> subActions = new List<AsyncAction>();

                foreach (SR sr in _srList)
                {
                    subActions.Add(new SrRepairAction(sr.Connection, sr, false));
                }

                _repairAction = new MultipleAction(null, string.Empty, string.Empty, string.Empty, subActions, true);
            }
            
            _repairAction.Changed += action_Changed;
            _repairAction.Completed += action_Completed;
            Grow(_repairAction.RunAsync);
        }

        private void action_Changed(ActionBase action)
        {
            Program.Invoke(this, () => UpdateProgressControls(action));
        }

        private void action_Completed(ActionBase sender)
        {
            if(_srList.Count > 0 && _srList.Any(s=>s !=null && !s.MultipathAOK))
            {
                SucceededWithWarning = true;
                SucceededWithWarningDescription = Messages.REPAIR_SR_WARNING_MULTIPATHS_DOWN;
            }

            Program.Invoke(this, () =>
                {
                    if (sender is MultipleAction)
                        Build();

                    FinalizeProgressControls(sender);
                });
        }

        private class RepairTreeNode : TreeNode
        {
            public readonly SR SR;
            public readonly Host Host;
            public readonly PBD PBD;

            public RepairTreeNode(SR sr, Host host, PBD pdb)
            {
                SR = sr;
                Host = host;
                PBD = pdb;

                if (host == null)
                {
                    Text = sr.Name;
                }
                else
                {
                    Text = host.Name;
                }
            }
        }
    }
}
