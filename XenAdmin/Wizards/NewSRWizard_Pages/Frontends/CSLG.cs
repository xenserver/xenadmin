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
using System.Collections.ObjectModel;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Network.StorageLink;
using XenAdmin.StorageLinkAPI;
using XenAPI;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using XenAdmin.Properties;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class CSLG : XenTabPage
    {
        #region Private fields
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IStorageLinkObject _storageLinkObject;
        private ReadOnlyCollection<CslgSystemStorage> _storages;
        private SR _srToReattach;
        private bool _disasterRecoveryTask;
        private int _storageSystemComboLastSelectedIndex = -1;

        private const string ADAPTER_ID = "adapterid";
        private const string STORAGE_SYSTEM_ID = "storageSystemId";
        #endregion

        public CSLG()
        {
            InitializeComponent();
        }

        #region Accessors

        public StorageLinkAdapterBoston SelectedStorageAdapter { get { return comboBoxStorageSystem.SelectedItem as StorageLinkAdapterBoston; } }

        public CslgSystemStorage SelectedStorageSystem { get { return comboBoxStorageSystem.SelectedItem as CslgSystemStorage; } }

        public ReadOnlyCollection<CslgStoragePool> StoragePools { get; private set; }

        public bool NetAppSelected
        {
            get
            {
                var item = comboBoxStorageSystem.SelectedItem as string;
                return item != null && item == Messages.CSLG_NETAPP_DIRECT;
            }
        }

        public bool DellSelected
        {
            get
            {
                var item = comboBoxStorageSystem.SelectedItem as string;
                return item != null && item == Messages.CSLG_DELL_DIRECT;
            }
        }

        public Dictionary<string, string> DeviceConfigParts
        {
            get
            {
                var dconf = new Dictionary<string, string>();

                if (Helpers.BostonOrGreater(Connection))
                {
                    var adapter = comboBoxStorageSystem.SelectedItem as StorageLinkAdapterBoston;
                    if (adapter != null)
                        dconf[ADAPTER_ID] = adapter.Id;
                }
                else
                {
                    var system = comboBoxStorageSystem.SelectedItem as CslgSystemStorage;
                    if (system != null)
                        dconf[STORAGE_SYSTEM_ID] = system.StorageSystemId;

                    StorageLinkCredentials credentials = GetStorageLinkCredentials(Connection);
                    if (credentials != null)
                    {
                        dconf["target"] = credentials.Host;
                        dconf["username"] = credentials.Username;
                        dconf["password"] = credentials.Password;
                    }
                }

                return dconf;
            }
        }

        public SrWizardType SrWizardType
        {
            set
            {
                _srToReattach = value.SrToReattach;
                _disasterRecoveryTask = value.DisasterRecoveryTask;
            }
        }

        #endregion

        private static List<StorageLinkCredentials> GetAllValidStorageLinkCreds()
        {
            var credsList = new List<StorageLinkCredentials>();

            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy.FindAll(c => c.IsConnected && Helpers.MidnightRideOrGreater(c) && !Helpers.FeatureForbidden(c, Host.RestrictStorageChoices)))
            {
                var p = Helpers.GetPoolOfOne(c);
                credsList.Add(p.GetStorageLinkCredentials());
                credsList.AddRange(Array.ConvertAll(p.Connection.Cache.PBDs, pbd => pbd.GetStorageLinkCredentials()));
            }
            credsList.RemoveAll(cc => cc == null || !cc.IsValid);
            return credsList;
        }

        private StorageLinkCredentials GetStorageLinkCredentials(IXenConnection connection)
        {
            if (Helpers.BostonOrGreater(connection))
                return null;

            if (_storageLinkObject != null)
            {
                return GetAllValidStorageLinkCreds().Find(c =>
                    c.Host == _storageLinkObject.StorageLinkConnection.Host &&
                    c.Username == _storageLinkObject.StorageLinkConnection.Username &&
                    c.Password == _storageLinkObject.StorageLinkConnection.Password);
            }
            else
            {
                // just do a check that local creds have been correctly moved the server pool object.
                Settings.CslgCredentials localCreds = Settings.GetCslgCredentials(connection);
                Debug.Assert(localCreds == null || string.IsNullOrEmpty(localCreds.Host));

                Pool pool = Helpers.GetPoolOfOne(connection);

                if (pool != null)
                {
                    StorageLinkCredentials creds = pool.GetStorageLinkCredentials();

                    if (creds != null && creds.IsValid)
                    {
                        return creds;
                    }
                    else
                    {
                        // if there aren't any creds then try importing from another pool. The user will probably only
                        // have one set of CSLG creds and they just haven't set the creds to this pool yet. Do it for them.

                        var credsList = GetAllValidStorageLinkCreds();

                        if (credsList.Count > 0 && !Helpers.BostonOrGreater(Connection))
                        {
                            var action = new SetCslgCredentialsToPoolAction(pool.Connection, credsList[0].Host, credsList[0].Username, credsList[0].Password);
                            new ActionProgressDialog(action, ProgressBarStyle.Marquee).ShowDialog(this);
                            return pool.GetStorageLinkCredentials();
                        }
                    }
                }
                return null;
            }
        }

        public void SetStorageLinkObject(IStorageLinkObject storageLinkObject)
        {
            _storageLinkObject = storageLinkObject;
        }

        /// <summary>
        /// Performs a scan of the CSLG host specified in the textboxes. 
        /// </summary>
        /// <returns>True, if the scan succeeded, otherwise False.</returns>
        public bool PerformStorageSystemScan()
        {
            var items = new List<object>();
            StorageLinkCredentials credentials = null;
            SrCslgStorageSystemScanAction scanAction = null;

            if (_storageLinkObject != null || (Connection.IsConnected && !Helpers.FeatureForbidden(Connection, Host.RestrictStorageChoices) && Helpers.MidnightRideOrGreater(Connection)))
            {
                if (_srToReattach == null || _srToReattach.type == "cslg")
                {
                    credentials = GetStorageLinkCredentials(Connection);

                    if (credentials != null && credentials.IsValid)
                    {
                        scanAction = new SrCslgStorageSystemScanAction(Program.MainWindow, Connection,
                                                                       Program.StorageLinkConnections.GetCopy(),
                                                                       credentials.Host, credentials.Username,
                                                                       credentials.PasswordSecret);
                    }
                    else if (Helpers.BostonOrGreater(Connection))
                    {

                        var action = new SrCslgAdaptersScanAction(Connection);
                        var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee);
                        // never show the error message if it fails.
                        action.Completed += (s, e) =>
                        {
                            if (!action.Succeeded)
                            {
                                Program.Invoke(dialog, dialog.Close);
                            }
                        };

                        dialog.ShowDialog(this);
                        if (action.Succeeded)
                        {
                            var adapters = action.GetAdapters();
                            items.AddRange(Util.PopulateList<object>(adapters));
                            items.Sort((x, y) => x.ToString().CompareTo(y.ToString()));
                        }
                        else
                            return false;
                    }
                    if (scanAction != null)
                    {
                        var dialog = new ActionProgressDialog(scanAction, ProgressBarStyle.Marquee);

                        // never show the error message if it fails.
                        scanAction.Completed += (s, e) =>
                                                    {
                                                        if (!scanAction.Succeeded)
                                                        {
                                                            Program.Invoke(dialog, dialog.Close);
                                                        }
                                                    };

                        dialog.ShowDialog(this);

                        if (scanAction.Succeeded)
                        {
                            _storages = scanAction.CslgSystemStorages;
                            items.AddRange(Util.PopulateList<object>(_storages));
                            items.Sort((x, y) => x.ToString().CompareTo(y.ToString()));
                        }
                    }
                }
            }

            bool bostonHasDell = false;
            bool bostonHasNetapp = false;
            if (Helpers.BostonOrGreater(Connection) && items != null)
            {
                bostonHasDell = (items.Find(item => ((StorageLinkAdapterBoston)item).Id == "DELL_EQUALLOGIC") != null);
                bostonHasNetapp = (items.Find(item => ((StorageLinkAdapterBoston)item).Id == "NETAPP") != null);
            }

            comboBoxStorageSystem.Items.Clear();

            if (_storageLinkObject != null)
            {
                // the wizard was launched with a storagelink-server, storage-system or storage-pool selected.
                if (scanAction != null && scanAction.Succeeded)
                {
                    comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(string.Format(Messages.CSLG_STORAGELINK_SERVER, credentials.Host), true));
                    comboBoxStorageSystem.Items.AddRange(items.ToArray());
                    comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(Messages.ADD_HOST, false));

                    // if a specific storage-system was selected when the wizard was launched then select that storage-system
                    // in the combo-box here.
                    var system = _storageLinkObject as StorageLinkSystem;

                    if (system == null)
                    {
                        // if a specific storage-pool was selected when the wizard was launched then select the storage-system
                        // of that storage-pool here.
                        var storagePool = _storageLinkObject as StorageLinkPool;
                        system = storagePool == null ? null : storagePool.StorageLinkSystem;
                    }

                    if (system != null)
                    {
                        comboBoxStorageSystem.SelectedItem = items.Find(o => ((CslgSystemStorage)o).StorageSystemId == system.opaque_ref);
                    }
                }
            }
            else if (_srToReattach != null)
            {
                if (_srToReattach.type == "equal")
                {
                    // a direct-connect Equallogic is being reattached. Only add this item.
                    comboBoxStorageSystem.Items.Add(Messages.CSLG_DELL_DIRECT);
                }
                else if (_srToReattach.type == "netapp")
                {
                    // a direct-connect NetApp is being reattached. Only add this item.
                    comboBoxStorageSystem.Items.Add(Messages.CSLG_NETAPP_DIRECT);
                }
                else if (credentials != null)
                {
                    // credentials can be null if we don't have a license which supports CSLG.

                    // re-attaching StorageLink SR
                    Debug.Assert(_srToReattach.type == "cslg");
                    comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(string.Format(Messages.CSLG_STORAGELINK_SERVER, credentials.Host), true));
                    comboBoxStorageSystem.Items.AddRange(items.ToArray());
                }
            }
            else
            {
                // a pool or host was selected in the mainwindow tree when the wizard was launched.
                bool canAdd = scanAction != null && scanAction.Succeeded && scanAction.StorageLinkConnection != null;
                bool showHeaders = scanAction != null && scanAction.Succeeded && (items.Count > 0 || scanAction.StorageLinkConnection != null);

                if (showHeaders || Helpers.BostonOrGreater(Connection))
                {
                    if (!bostonHasDell || !bostonHasNetapp)
                        comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(Messages.CSLG_DIRECT_CONNECTION, true));
                }

                if (!bostonHasDell)
                    comboBoxStorageSystem.Items.Add(Messages.CSLG_DELL_DIRECT);
                if (!bostonHasNetapp)
                    comboBoxStorageSystem.Items.Add(Messages.CSLG_NETAPP_DIRECT);

                if (showHeaders)
                {
                    comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(string.Format(Messages.CSLG_STORAGELINK_SERVER, credentials.Host), true));
                    comboBoxStorageSystem.Items.AddRange(items.ToArray());
                }

                if (canAdd)
                {
                    comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(Messages.ADD_HOST, false));
                }
            }

            if (Helpers.BostonOrGreater(Connection) && items != null && items.Count > 0)
            {
                if (!bostonHasDell || !bostonHasNetapp)
                    comboBoxStorageSystem.Items.Add(new NonSelectableComboBoxItem(Messages.CSLG_STORAGELINK_ADAPTERS, true));
                comboBoxStorageSystem.Items.AddRange(items.ToArray());
            }

            if (comboBoxStorageSystem.SelectedIndex < 0 && comboBoxStorageSystem.Items.Count > 0)
            {
                // select the first selectable item if nothing's already been selected.
                comboBoxStorageSystem.SelectedItem = Util.PopulateList<object>(comboBoxStorageSystem.Items).Find(s => !(s is NonSelectableComboBoxItem));
                if (_srToReattach != null && _srToReattach.type == "cslg" &&
                              Helpers.BostonOrGreater(Connection))
                {
                    comboBoxStorageSystem.SelectedItem =
                        Util.PopulateList<object>(comboBoxStorageSystem.Items).Find(s =>
                        {
                            var bostonadapter = s as StorageLinkAdapterBoston;
                            if (bostonadapter != null)
                            {
                                // sm_config["md_svid"] looks like "DELL__EQUALLOGIC__{GUID}"
                                // bostonadapter.Id looks like "DELL_EQUALLOGIC"
                                // Additionally, EMC__CLARIION is always SMI-S (CA-72968)
                                if (_srToReattach.sm_config.ContainsKey("md_svid"))
                                {
                                    var svid = _srToReattach.sm_config["md_svid"];
                                    if (svid.Replace("__", "_").StartsWith(bostonadapter.Id))
                                        return true;
                                    if (bostonadapter.Id == "SMIS_STORAGE_SYSTEM" && svid.StartsWith("EMC__CLARIION"))
                                        return true;
                                    return false;
                                }
                            }
                            return false;
                        });
                }
            }
            return true;
        }

        private bool PerformStoragePoolScan()
        {
            StorageLinkCredentials credentials = GetStorageLinkCredentials(Connection);

            var scanAction = new SrCslgStoragePoolScanAction(Connection, Program.StorageLinkConnections.GetCopy(), credentials.Host,
                                                             credentials.Username, credentials.PasswordSecret, SelectedStorageSystem.StorageSystemId);

            ActionProgressDialog dialog = new ActionProgressDialog(scanAction, ProgressBarStyle.Marquee);
            dialog.ShowDialog(this);

            if (scanAction.Succeeded)
                StoragePools = scanAction.CslgStoragePools;

            return scanAction.Succeeded;
        }

        private void AddStorageSystem()
        {
            StorageLinkConnection slCon = Program.StorageLinkConnections.GetCopy().Find(s => s.Host == GetStorageLinkCredentials(Connection).Host);
            
            if (slCon == null)
                return;

            var systemsBefore = new List<StorageLinkSystem>(slCon.Cache.StorageSystems);
            AddStorageLinkSystemCommand command = new AddStorageLinkSystemCommand(Program.MainWindow.CommandInterface, slCon.Cache.Server, Parent);

                command.Completed += (s, ee) =>
                    {
                        if (ee.Success)
                        {
                            var systemsAfter = new List<StorageLinkSystem>(slCon.Cache.StorageSystems);

                            ThreadPool.QueueUserWorkItem(o =>
                                {
                                    Program.Invoke(Program.MainWindow, () =>
                                        {
                                            PerformStorageSystemScan();

                                            if (systemsAfter.Count > systemsBefore.Count)
                                            {
                                                // the new item should be selected.
                                                comboBoxStorageSystem.SelectedItem = systemsAfter.Find(ss => !systemsBefore.Contains(ss));
                                                comboBoxStorageSystem.DroppedDown = true;
                                            }
                                        });
                                });
                        }
                    };

            command.Execute();
        }

        #region XenTabPage overrides

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward && SelectedStorageSystem != null)
                cancel = !PerformStoragePoolScan();

            base.PageLeave(direction, ref cancel);
        }

        public override void PopulatePage()
        {
            linkLabelGotoStorageLinkProperties.Visible = Helpers.MidnightRideOrGreater(Connection) && _storageLinkObject == null;
            labelStorageLinkPropertiesLinkBlurb.Visible = linkLabelGotoStorageLinkProperties.Visible;

            labelAdapter.Visible = Helpers.BostonOrGreater(Connection);
            labelSystem.Visible = flowLayoutPanel1.Visible = !Helpers.BostonOrGreater(Connection);

            if (Helpers.BostonOrGreater(Connection))
                labelStorageSystem.Text = Messages.CSLG_STORAGEADAPTER;
        }

        public override string Text
        {
            get { return Helpers.BostonOrGreater(Connection) ? Messages.STORAGE_ADAPTER : Messages.STORAGE_SYSTEM; }
        }

        public override string PageTitle
        {
            get { return Helpers.BostonOrGreater(Connection) ? Messages.NEWSR_CSLG_ADAPTER_PAGE_TITLE : Messages.NEWSR_CSLG_PAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return Helpers.BostonOrGreater(Connection) ? "SL_System" : "SL_System_PreBoston"; }
        }

        public override bool EnableNext()
        {
            return comboBoxStorageSystem.SelectedItem != null &&
                   !(comboBoxStorageSystem.SelectedItem is NonSelectableComboBoxItem);
        }

        public override bool EnablePrevious()
        {
            if (_disasterRecoveryTask && _srToReattach == null)
                return false;

            return true;
        }

        #endregion

        #region NonSelectableComboBoxItem class

        private class NonSelectableComboBoxItem
        {
            public string Text { get; private set; }
            public bool Bold { get; private set; }

            public NonSelectableComboBoxItem(string text, bool bold)
            {
                Text = text;
                Bold = bold;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        #endregion

        #region Event handlers

        private void comboBoxStorageSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxStorageSystem.SelectedItem is NonSelectableComboBoxItem)
            {
                if (comboBoxStorageSystem.SelectedItem.ToString() == Messages.ADD_HOST)
                {
                    Program.BeginInvoke(Program.MainWindow, AddStorageSystem);
                    comboBoxStorageSystem.SelectedIndex = -1;
                }
                else if (comboBoxStorageSystem.SelectedIndex < comboBoxStorageSystem.Items.Count - 1 &&
                    (_storageSystemComboLastSelectedIndex < comboBoxStorageSystem.SelectedIndex || comboBoxStorageSystem.SelectedIndex == 0))
                {
                    _storageSystemComboLastSelectedIndex = comboBoxStorageSystem.SelectedIndex;
                    comboBoxStorageSystem.SelectedIndex++;
                }
                else
                {
                    _storageSystemComboLastSelectedIndex = comboBoxStorageSystem.SelectedIndex;
                    comboBoxStorageSystem.SelectedIndex--;
                }
            }
            else
            {
                _storageSystemComboLastSelectedIndex = comboBoxStorageSystem.SelectedIndex;
            }

            OnPageUpdated();

            if (comboBoxStorageSystem.SelectedIndex >= 0)
            {
                if (_storageLinkObject != null)
                {
                    // The user has changed which SL object to use using the storage-system combo-box so update that here.

                    if (_storageLinkObject is StorageLinkServer || _storageLinkObject is StorageLinkSystem)
                    {
                        _storageLinkObject = ((CslgSystemStorage)comboBoxStorageSystem.SelectedItem).StorageLinkSystem;
                    }
                    else
                    {
                        StorageLinkPool storageLinkPool = (StorageLinkPool)_storageLinkObject;

                        if (storageLinkPool.StorageLinkSystem != ((CslgSystemStorage)comboBoxStorageSystem.SelectedItem).StorageLinkSystem)
                        {
                            _storageLinkObject = ((CslgSystemStorage)comboBoxStorageSystem.SelectedItem).StorageLinkSystem;
                        }
                    }
                }
            }
        }

        private void comboBoxStorageSystem_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (e.Index > -1)
            {
                var item = comboBox.Items[e.Index];
                var storageSystem = item as CslgSystemStorage;
                var nonSelectable = item as NonSelectableComboBoxItem;
                bool edit = (e.State & DrawItemState.ComboBoxEdit) != 0;
                const int imageWidth = 16;
                int indent = edit ? 0 : (imageWidth / 2);

                e.DrawBackground();

                var textRect = new Rectangle(e.Bounds.X + indent + imageWidth, e.Bounds.Y, e.Bounds.Width - imageWidth - indent, e.Bounds.Height);

                if (storageSystem != null)
                {
                    e.Graphics.DrawImageUnscaled(Resources.sl_system_16, new Point(e.Bounds.X + indent, e.Bounds.Y + 1));
                    Drawing.DrawText(e.Graphics, item.ToString(), e.Font, textRect, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                else if (item.ToString() == Messages.ADD_HOST)
                {
                    e.Graphics.DrawImageUnscaled(Resources.sl_add_storage_system_small_16, new Point(e.Bounds.X + indent, e.Bounds.Y + 1));
                    Drawing.DrawText(e.Graphics, item.ToString(), e.Font, textRect, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                else if (nonSelectable != null)
                {
                    // bold headings
                    Drawing.DrawText(e.Graphics, item.ToString(), new Font(e.Font, FontStyle.Bold), e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                else
                {
                    // dell and netapp direction connection.
                    textRect = new Rectangle(e.Bounds.X + indent, e.Bounds.Y, e.Bounds.Width - indent, e.Bounds.Height);
                    Drawing.DrawText(e.Graphics, item.ToString(), e.Font, textRect, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }

                if ((e.State & DrawItemState.NoFocusRect) == 0)
                {
                    e.DrawFocusRectangle();
                }
            }
        }

        private void linkLabelGotoStorageLinkProperties_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IXenObject o = Helpers.GetPool(Connection);

            if (o == null)
            {
                var hosts = Connection.Cache.Hosts;
                if (hosts.Length > 0)
                {
                    o = hosts[0];
                }
            }

            if (o != null)
            {
                var dialog = new PropertiesDialog(o);
                dialog.Load += (s, ee) => dialog.SelectPage(dialog.StorageLinkPage);

                dialog.FormClosing += (s, ee) =>
                {
                    if (dialog.DialogResult == DialogResult.Yes && ee.Action != null)
                    {
                        ee.StartAction = false;
                        ee.Action.Completed += (ss, eee) =>
                        {
                            if (ee.Action.Succeeded)
                            {
                                Program.Invoke(Program.MainWindow, () =>
                                {
                                    int comboBoxItemCount = comboBoxStorageSystem.Items.Count;
                                    PerformStorageSystemScan();
                                    comboBoxStorageSystem.DroppedDown = comboBoxStorageSystem.Items.Count > comboBoxItemCount;
                                });
                            }
                        };

                        new ActionProgressDialog(ee.Action, ProgressBarStyle.Marquee).ShowDialog(this);
                    }
                };

                dialog.Show(this);
            }
        }

        #endregion
    }
}
