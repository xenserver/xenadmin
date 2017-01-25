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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public class LicenseManagerLauncher
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool LicenseSummaryVisible { get; set; }
        public IWin32Window Parent{private get; set;}
        private DateTime LastCloseTime { get; set; }
        private LicenseManager licenseManagerDialog { get; set; }
        private readonly object licenseLock = new object();

        protected virtual DateTime ReferenceTime
        {
            get { return DateTime.Now; }
        }

        protected virtual TimeSpan TimeSinceLastClose
        {
            get { return ReferenceTime - LastCloseTime; }
        }

        protected virtual bool ModalDialogVisible
        {
            get { return Win32Window.ModalDialogIsVisible(); }
        }

        protected virtual DialogResult LaunchDialog(IEnumerable<IXenObject> allObjects, IEnumerable<IXenObject> selectedObjects)
        {
            if (licenseManagerDialog == null)
                return DialogResult.None;

            return licenseManagerDialog.ShowDialog(Parent, allObjects.ToList(), selectedObjects.ToList());
        }

        protected virtual void RefreshDialog(IEnumerable<IXenObject> allObjects, IEnumerable<IXenObject> selectedObjects)
        {
            if(licenseManagerDialog == null)
                return;

            licenseManagerDialog.RefreshView(allObjects.ToList(), selectedObjects.ToList());
        }

        public LicenseManagerLauncher(IWin32Window parent)
        {
            Parent = parent;
            LicenseSummaryVisible = false;
        }

        public bool LicenceDialogIsShowing
        {
            get { return licenseManagerDialog != null; }
        }

        private void LoadManagerDialog()
        {
            licenseManagerDialog = new LicenseManager(new LicenseManagerController());
        }

        public void LaunchIfRequired(bool nag, ChangeableList<IXenConnection> connections)
        {
            LaunchIfRequired(nag, connections, Enumerable.Empty<IXenObject>());
        }

        public void LaunchIfRequired(bool nag, ChangeableList<IXenConnection> connections, SelectedItemCollection selectedObjects)
        {
            if (selectedObjects != null && selectedObjects.AllItemsAre<IXenObject>(x => x is Pool || x is Host))
            {
                List<IXenObject> itemsSelected = selectedObjects.AsXenObjects<Pool>().ConvertAll(p => p as IXenObject);
                itemsSelected.AddRange(selectedObjects.AsXenObjects<Host>().Where(h => Helpers.GetPool(h.Connection) == null).ToList().ConvertAll(h => h as IXenObject));
                itemsSelected.AddRange(selectedObjects.AsXenObjects<Host>().Where(h => Helpers.GetPool(h.Connection) != null).ToList().ConvertAll(h => Helpers.GetPool(h.Connection)).ConvertAll(p => p as IXenObject).Distinct());
                if(itemsSelected.All(xo => Helpers.ClearwaterOrGreater(xo.Connection)) || 
                   itemsSelected.All(xo => !Helpers.ClearwaterOrGreater(xo.Connection)))
                {
                    LaunchIfRequired(nag, connections, itemsSelected);
                }
                else
                    LaunchIfRequired(nag, connections, itemsSelected.Where(xo => Helpers.ClearwaterOrGreater(xo.Connection)));
            }
            else
                LaunchIfRequired(nag, connections);
        }

        private void LaunchIfRequired(bool nag, IEnumerable<IXenConnection> connections, IEnumerable<IXenObject> selectedObjects)
        {
            List<IXenObject> allObjects = new List<IXenObject>();
            foreach (IXenConnection conn in connections)
            {
                if(conn == null)
                    continue;

                if(!conn.IsConnected)
                    continue;

                Pool pool = Helpers.GetPool(conn);
                if (pool == null)
                    allObjects.AddRange(conn.Cache.Hosts);
                else
                    allObjects.Add(pool);
            }

            LaunchIfRequired(nag, allObjects, selectedObjects);
        }

        private void LaunchIfRequired(bool nag, IEnumerable<IXenObject> allObjects, IEnumerable<IXenObject> selectedObjects)
        {
            lock (licenseLock)
            {
                if (!LicenseSummaryVisible)
                {
                    LoadManagerDialog();
                    if (nag && TimeSinceLastClose < TimeSpan.FromSeconds(10))
                    {
                        // this nag was less than 10 seconds since we closed this dialog. Don't re-show.
                        return;
                    }

                    if (nag && ModalDialogVisible)
                    {
                        // if the add-server dialog is visible, then don't nag with the license-manager as it
                        // will appear above it.
                        return;
                    }
                    LicenseSummaryVisible = true;
                    log.Info("License summary not showing. Show it now.");

                    try
                    {
                        LaunchDialog(allObjects, selectedObjects);
                    }
                    finally
                    {
                        LicenseSummaryVisible = false;
                        LastCloseTime = ReferenceTime;
                        if(licenseManagerDialog != null)
                        {
                            licenseManagerDialog.Dispose();
                            licenseManagerDialog = null;
                        }
                        
                    }
                }
                else
                {
                    RefreshDialog(allObjects, selectedObjects);
                }
            }
        }

    }
}
