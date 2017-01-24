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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Network;
using XenAPI;
using System.Threading;
using XenAdmin.Core;
using XenAdmin.Properties;
using System.Linq;

namespace XenAdmin.Wizards.GenericPages
{
    public partial class RBACWarningPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<IXenConnection, List<WizardPermissionCheck>> checksPerConnectionDict;
        private Thread bgThread;
        private bool refreshPage = false;
        private bool finished = false;
        public bool blockProgress = false;

        public RBACWarningPage(string description) 
            : this()
        {
            labelDescription.Text = description;
        }

        public RBACWarningPage()
        {
            InitializeComponent();
            checksPerConnectionDict = new Dictionary<IXenConnection, List<WizardPermissionCheck>>();
        }

        public override string HelpID { get { return "RBAC"; } }

        public override string Text { get { return Messages.RBAC_WARNING_PAGE_TEXT_TITLE; } }
        
        public override string PageTitle { get { return Messages.RBAC_WARNING_PAGE_TEXT_TITLE; } }

        public void AddPermissionChecks(IXenConnection connection, params WizardPermissionCheck[] permissionChecks)
        {
            if (!checksPerConnectionDict.ContainsKey(connection))
            {
                checksPerConnectionDict.Add(connection, new List<WizardPermissionCheck>());
            }

            checksPerConnectionDict[connection].AddRange(permissionChecks);
        }

		public void ClearPermissionChecks()
		{
            DeregisterConnectionEvents();
            checksPerConnectionDict.Clear();
		}

        public override void PageLoaded(PageLoadedDirection direction)
        {
            RegisterConnectionEvents();
            RefreshPage();
            base.PageLoaded(direction);
        }

        void Connection_ConnectionResult(object sender, ConnectionResultEventArgs e)
        {
            if (e.Connected)
                Program.Invoke(this, RefreshPage);
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            DeregisterConnectionEvents();
            base.PageLeave(direction, ref cancel);
        }

        public override void PageCancelled()
        {
            DeregisterConnectionEvents();
            base.PageCancelled();
        }

        public void RefreshPage()
        {
            if (bgThread == null || !bgThread.IsAlive)
            {
                bgThread = new Thread(RetrieveRBACWarnings);
                bgThread.IsBackground = true;
                bgThread.Start();
            }
            else
            {
                refreshPage = true;
            }
        }

        public override bool EnableNext()
        {
            return !blockProgress && finished;
        }

        private void RegisterConnectionEvents()
        {
            foreach (var connectionChecks in checksPerConnectionDict)
            {
                IXenConnection connection = connectionChecks.Key;
                connection.ConnectionResult += Connection_ConnectionResult;
            }
        }

        private void DeregisterConnectionEvents()
        {
            foreach (var connectionChecks in checksPerConnectionDict)
            {
                IXenConnection connection = connectionChecks.Key;
                connection.ConnectionResult -= Connection_ConnectionResult;
            }
        }

        private PermissionCheckResult RunPermissionChecks(IXenConnection connection,
            List<WizardPermissionCheck> permissionChecks, out List<WizardPermissionCheck> errors,
            out List<WizardPermissionCheck> warnings)
        {
            PermissionCheckResult checkResult = PermissionCheckResult.OK;

            errors = new List<WizardPermissionCheck>();
            warnings = new List<WizardPermissionCheck>();
            foreach (WizardPermissionCheck wpc in permissionChecks)
            {
                List<Role> rolesAbleToComplete = Role.ValidRoleList(wpc.ApiCallsToCheck, connection);
                List<Role> subjectRoles = connection.Session.Roles;

                if (subjectRoles.Find(rolesAbleToComplete.Contains) != null)
                    continue;

                log.DebugFormat("Failed RBAC check: {0}", wpc.WarningMessage);
                if (wpc.Blocking)
                {
                    errors.Add(wpc);
                    checkResult = PermissionCheckResult.Failed;
                }
                else
                {
                    warnings.Add(wpc);
                    if (checkResult == PermissionCheckResult.OK)
                        checkResult = PermissionCheckResult.Warning;
                }
            }

            return checkResult;
        }

        private void RetrieveRBACWarnings()
        {
            SetUpdating();
            foreach (var connectionChecks in checksPerConnectionDict)
            {
                IXenConnection connection = connectionChecks.Key;
                PermissionCheckHeaderRow headerRow = AddHeaderRow(connection);
                PermissionCheckResult checkResult = PermissionCheckResult.OK;

                if (connection.Session.IsLocalSuperuser || connectionChecks.Value.Count == 0)
                {
                    SetNoWarnings();
                }
                else
                {
                    List<WizardPermissionCheck> errors;
                    List<WizardPermissionCheck> warnings;
                    checkResult = RunPermissionChecks(connection, connectionChecks.Value, out errors, out warnings);
                    switch (checkResult)
                    {
                        case PermissionCheckResult.OK:
                            SetNoWarnings();
                            break;
                        case PermissionCheckResult.Warning:
                            AddWarnings(connection, warnings);
                            break;
                        case PermissionCheckResult.Failed:
                            AddErrors(connection, errors);
                            break;
                    }
                }
                UpdateHeaderRow(headerRow, checkResult);
            }
            FinishedUpdating();
        }

        private void AddErrors(IXenConnection connection, List<WizardPermissionCheck> errors)
        {
            Program.AssertOffEventThread();

            if (connection.Session.IsLocalSuperuser)
            {
                // We should not be here.
                log.Warn("A local root account is being blocked access");
            }

            List<Role> roleList = connection.Session.Roles;
            roleList.Sort();

            foreach (WizardPermissionCheck wizardPermissionCheck in errors)
            {
                // the string is a format string that needs to take the current role (we use the subject they were authorised under which could be a group or user)
                string description = String.Format(wizardPermissionCheck.WarningMessage, roleList[0].FriendlyName);
                AddDetailsRow(description, PermissionCheckResult.Failed);
            }

            Program.Invoke(this, () => blockProgress = true);
        }

        private void FinishedUpdating()
        {
            if (refreshPage)
            {
                // If we have received a request to refresh while updating then we run the logic again
                refreshPage = false;
                RetrieveRBACWarnings();
                return;
            }
            finished = true;
            Program.Invoke(this, delegate
            {
                labelClickNext.Visible = !blockProgress;
                OnPageUpdated();
            });
        }

        private void AddWarnings(IXenConnection connection, List<WizardPermissionCheck> warnings)
        {
            List<Role> roleList = connection.Session.Roles;
            roleList.Sort();

            foreach (WizardPermissionCheck wizardPermissionCheck in warnings)
            {
                if (wizardPermissionCheck.WarningAction != null)
                    wizardPermissionCheck.WarningAction();

                // the string is a format string that needs to take the current role (we use the subject they were authorised under which could be a group or user)
                string description = String.Format(wizardPermissionCheck.WarningMessage, roleList[0].FriendlyName);
                AddDetailsRow(description, PermissionCheckResult.Warning);
            }
        }

        private void SetNoWarnings()
        {
            AddDetailsRow(Messages.RBAC_NO_WIZARD_LIMITS, PermissionCheckResult.OK);
        }

        private void SetUpdating()
        {
            Program.AssertOffEventThread();

            Program.Invoke(this, delegate
            {
                blockProgress = false;
                finished = false;
                OnPageUpdated();
                labelClickNext.Visible = false;
                dataGridViewEx1.Rows.Clear();
            });
        }

        private PermissionCheckHeaderRow AddHeaderRow(IXenConnection connection)
        {
            Program.AssertOffEventThread();

            string text = string.Format(Messages.RBAC_WARNING_PAGE_HEADER_ROW_DESC, connection.Session.UserFriendlyName,
                                        connection.Session.FriendlyRoleDescription, connection.FriendlyName);
            PermissionCheckHeaderRow headerRow = new PermissionCheckHeaderRow(text);
            Program.Invoke(this, delegate
                                     {
                                         headerRow.SetPermissionCheckInProgress(true);
                                         dataGridViewEx1.Rows.Add(headerRow);
                                     });
            return headerRow;
        }

        private void UpdateHeaderRow(PermissionCheckHeaderRow headerRow, PermissionCheckResult checkResult)
        {
            Program.AssertOffEventThread();
            Program.Invoke(this, delegate
                                     {
                                         headerRow.SetPermissionCheckInProgress(false);
                                         headerRow.UpdateDescription(checkResult);
                                     });
        }

        private void AddDetailsRow(string desc, PermissionCheckResult checkResult)
        {
            Program.AssertOffEventThread();
            Program.Invoke(this, delegate
                                     {
                                         PermissionCheckDetailsRow detailsRow =
                                             new PermissionCheckDetailsRow(desc, checkResult);
                                         dataGridViewEx1.Rows.Add(detailsRow);
                                     });
        }

        internal enum PermissionCheckResult { OK, Warning, Failed }

        private abstract class PermissionCheckGridRow : DataGridViewRow
        {
            protected DataGridViewImageCell iconCell = new DataGridViewImageCell();
            protected DataGridViewTextBoxCell descriptionCell = new DataGridViewTextBoxCell();
            protected PermissionCheckGridRow()
            {
                Cells.Add(iconCell);
                Cells.Add(descriptionCell);
            }
        }

        private class PermissionCheckHeaderRow : PermissionCheckGridRow
        {
            private string description;

            public PermissionCheckHeaderRow(string desc)
            {
                iconCell.Value = new Bitmap(1, 1);
                description = desc;
                descriptionCell.Value = desc;
                descriptionCell.Style.Font = new Font(Program.DefaultFont, FontStyle.Bold);
            }

            public void UpdateDescription(PermissionCheckResult permissionCheckResult)
            {
                string result = permissionCheckResult == PermissionCheckResult.OK
                                    ? Messages.GENERAL_STATE_OK
                                    : permissionCheckResult == PermissionCheckResult.Warning
                                          ? Messages.WARNING
                                          : Messages.FAILED;

                descriptionCell.Value = string.Format("{0} {1}", description, result);
            }

            public void SetPermissionCheckInProgress(bool value)
            {
                iconCell.Value = value ? XenAdmin.Properties.Resources.ajax_loader : new Bitmap(1, 1);
            }
        }

        private class PermissionCheckDetailsRow : PermissionCheckGridRow
        {
            public PermissionCheckDetailsRow(string desc, PermissionCheckResult checkResult)
            {
                iconCell.Value = checkResult == PermissionCheckResult.OK
                                     ? Resources._000_Tick_h32bit_16
                                     : checkResult == PermissionCheckResult.Warning
                                           ? Resources._000_Alert2_h32bit_16
                                           : Resources._000_Abort_h32bit_16;
                descriptionCell.Value = desc;
            }
        }

        public delegate void PermissionCheckActionDelegate();
        public class WizardPermissionCheck
        {
            public RbacMethodList ApiCallsToCheck;
            /// <summary>
            /// This is the warning message will be displayed. It should be a format string which takes one input, the users current role, which will be supplied when the page is displayed.
            /// </summary>
            public string WarningMessage;
            /// <summary>
            /// If true, this warning will be the only one that displays, and will use the cross icon.
            /// </summary>
            public bool Blocking = false;
            public PermissionCheckActionDelegate WarningAction;
            
            public WizardPermissionCheck(string warningMessage)
            {
                this.WarningMessage = warningMessage;
                ApiCallsToCheck = new RbacMethodList();
            }

            public void AddApiCheck(string s)
            {
                ApiCallsToCheck.Add(s);
            }

            public void AddApiCheck(RbacMethod method)
            {
                ApiCallsToCheck.Add(method);
            }

			public void AddApiCheckRange(RbacMethodList methodList)
			{
				foreach (var method in methodList)
					ApiCallsToCheck.Add(method);
			}

            public void AddWarningAction(PermissionCheckActionDelegate d)
            {
                WarningAction = d;
            }
        }
    }
}
