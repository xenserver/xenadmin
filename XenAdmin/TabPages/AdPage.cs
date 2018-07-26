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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Dialogs;
using System.Threading;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Actions;
using XenAdmin.Properties;
using XenCenterLib;

namespace XenAdmin.TabPages
{
    public partial class AdPage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The pool this settings page refers to (can be a poolOfOne). Only set on the GUI thread, and we also assert that it is only
        /// ever set once.
        /// </summary>
        private Pool pool;
        private Host master;
        private Thread _loggedInStatusUpdater;
        /// <summary>
        /// We keep a reference to this prompt to make repeated attempts to enable AD more user friendly (remembering the previously tried creds)
        /// </summary>
        private AdPasswordPrompt joinPrompt;

        private bool _updateInProgress;

        private IXenObject _xenObject;
        private readonly CollectionChangeEventHandler Pool_CollectionChangedWithInvoke;
        public IXenObject XenObject
        {
            set
            {
                Program.AssertOnEventThread();

                if (_xenObject != null)
                {
                    ClearHandles();
                }

                _xenObject = value;

                if (_xenObject == null)
                    return;

                pool = Helpers.GetPoolOfOne(_xenObject.Connection);
                if (pool == null)
                {
                    // Cache not populated
                    _xenObject.Connection.Cache.RegisterCollectionChanged<Pool>(Pool_CollectionChangedWithInvoke);
                    return;
                }

                pool.PropertyChanged += pool_PropertyChanged;
                pool.Connection.Session.PropertyChanged += Session_PropertyChanged;
                RefreshMaster();

                if (_loggedInStatusUpdater == null)
                {
                    // Fire off the the thread that will update the logged in status
                    _loggedInStatusUpdater = new Thread(updateLoggedInStatus);
                    _loggedInStatusUpdater.IsBackground = true;
                    _loggedInStatusUpdater.Start();
                }
            }
        }

        public AdPage()
        {
            InitializeComponent();
            Pool_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Pool_CollectionChanged);
            ColumnSubject.CellTemplate = new KeyValuePairCell();
            tTipLogoutButton.SetToolTip(Messages.AD_CANNOT_MODIFY_ROOT);
            tTipRemoveButton.SetToolTip(Messages.AD_CANNOT_MODIFY_ROOT);
            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;
            Text = Messages.ACTIVE_DIRECTORY_TAB_TITLE;
            joinPrompt = new AdPasswordPrompt(true, null);
        }

        /// <summary>
        /// This method is used when the cache was not populated by the time we set the XenObject. It sets the appropriate event handlers,
        /// references to the master and the pool, and populates the tab with the correct configuration. It de-registers
        /// itself when successful.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Pool_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            pool = Helpers.GetPoolOfOne(_xenObject.Connection);
            if (pool == null)
                return;

            _xenObject.Connection.Cache.DeregisterCollectionChanged<Pool>(Pool_CollectionChangedWithInvoke);
            pool.PropertyChanged += pool_PropertyChanged;
            pool.Connection.Session.PropertyChanged += Session_PropertyChanged;
            RefreshMaster();
        }

        /// <summary>
        /// Sets the references to the pool master and updates the tab with the correct configuration. Remember to de-reference events
        /// on the old master before running this method.
        /// </summary>
        private void RefreshMaster()
        {
            master = _xenObject.Connection.Resolve(pool.master);
            master.PropertyChanged += new PropertyChangedEventHandler(master_PropertyChanged);
            Program.BeginInvoke(this,checkAdType);
        }

        /// <summary>
        /// Clears all event handles that could be set in this control.
        /// </summary>
        private void ClearHandles()
        {
            pool.Connection.Cache.DeregisterBatchCollectionChanged<Subject>(SubjectCollectionChanged);
            pool.PropertyChanged -= pool_PropertyChanged;
            if (master != null)
                master.PropertyChanged -= master_PropertyChanged;

            if (pool.Connection.Session != null)
                pool.Connection.Session.PropertyChanged -= Session_PropertyChanged;
        }

        public override void PageHidden()
        {
            ClearHandles();
        }

        /// <summary>
        /// We keep track of the actions in currently running so we can disable the tab if we are in the middle of configuring AD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            //Program.AssertOnEventThread();
            Program.BeginInvoke(Program.MainWindow, () =>
                                                        {
                                                            if (e.Action == CollectionChangeAction.Add &&
                                                                (e.Element is EnableAdAction ||
                                                                 e.Element is DisableAdAction))
                                                            {
                                                                AsyncAction action = (AsyncAction) e.Element;
                                                                action.Completed += action_Completed;

                                                                if (_xenObject != null &&
                                                                    _xenObject.Connection == action.Connection)
                                                                    checkAdType();
                                                            }
                                                        });
        }


        void action_Completed(ActionBase sender)
        {
            AsyncAction action = (AsyncAction)sender;
            action.Completed -= action_Completed;

            if (_xenObject != null && _xenObject.Connection == action.Connection)
                Program.Invoke(this, checkAdType);
        }

        void Session_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.BeginInvoke(this, RepopulateListBox);
        }

        /// <summary>
        /// We need to update the configuration if the authentication method changes, and also various labels display the name of the
        /// master and should also be updated if that changes.
        /// </summary>
        /// <param name="sender1"></param>
        /// <param name="e"></param>
        void master_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "external_auth_type" || e.PropertyName == "name_label")
            {
                Program.Invoke(this, checkAdType);
            }
        }

        /// <summary>
        /// various labels display the name of the pool and should also be updated if that changes. Additionally if the pool master changes
        /// we need to update our event handles. There is a sanity check in the checkAdType() method in case this event is stuck in a queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label")
            {
                Program.Invoke(this, checkAdType);
            }
            else if (e.PropertyName == "master")
            {
                if (master != null)
                    master.PropertyChanged -= new PropertyChangedEventHandler(master_PropertyChanged);

                Program.Invoke(this, RefreshMaster);
            }
        }

        private void SetSubjectListEnable(bool enable)
        {
            Program.AssertOnEventThread();
            GridViewSubjectList.Enabled = enable;
            if (GridViewSubjectList.Enabled)
            {
                LabelGridViewDisabled.Visible = false;
                RepopulateListBox();

                foreach (AdSubjectRow r in GridViewSubjectList.Rows)
                {
                    if (r.IsLocalRootRow)
                    {
                        r.ToggleExpandedState();
                        break;
                    }
                }
            }
            else
            {
                foreach (AdSubjectRow row in GridViewSubjectList.Rows)
                {
                    if (row.IsLocalRootRow)
                        continue;
                    row.subject.PropertyChanged -= subject_PropertyChanged;
                }
                GridViewSubjectList.Rows.Clear();
                LabelGridViewDisabled.Visible = true;
            }
        }

        private void checkAdType()
        {
            Program.AssertOnEventThread();
            if (master == null)
            {
                log.WarnFormat("Could not resolve pool master for connection '{0}' in constructor; disabling.",
                    Helpers.GetName(pool.Connection).Ellipsise(100));
                OnMasterUnavailable();
                return;
            }
            // Sanity check in case the master change events are queued up
            if (pool.master.opaque_ref != master.opaque_ref)
            {
                master.PropertyChanged -= new PropertyChangedEventHandler(master_PropertyChanged);
                RefreshMaster();
            }
            AsyncAction a = HelpersGUI.FindActiveAdAction(master.Connection);
            if (a != null)
            {
                OnAdConfiguring();
            }
            else if (master.external_auth_type == Auth.AUTH_TYPE_NONE)
            {
                // AD is not yet configured.
                OnAdDisabled();
            }
            else
            {
                if (master.external_auth_type != Auth.AUTH_TYPE_AD)
                {
                    log.WarnFormat("Unrecognised value '{0}' for external_auth_type on pool master '{1}' for pool '{2}' in constructor; assuming AD enabled on pool.",
                        master.external_auth_type, Helpers.GetName(master).Ellipsise(100), Helpers.GetName(pool).Ellipsise(100));
                }
                // AD is already configured.
                OnAdEnabled();
            }
        }

        private void SubjectCollectionChanged(object sender, EventArgs e)
        {
            Program.BeginInvoke(this, RepopulateListBox);
        }

        private string Domain
        {
            get
            {
                Program.AssertOnEventThread();

                if (pool == null)
                    return "";
                // Resolve master
                Host master = Helpers.GetMaster(pool.Connection);
                if (master == null)
                {
                    log.WarnFormat("Could not resolve pool master for connection '{0}'; disabling.",
                        Helpers.GetName(pool.Connection).Ellipsise(50));
                    return Messages.UNKNOWN;
                }

                // Determine AD domain from master
                string domain = master.external_auth_service_name;
                return domain.Ellipsise(30);
            }
        }

        private void OnAdEnabled()
        {
            Program.AssertOnEventThread();

            flowLayoutPanel1.Enabled = true;
            SetSubjectListEnable(true);
            buttonJoinLeave.Text = Messages.AD_LEAVE_DOMAIN;
            buttonJoinLeave.Enabled = true;
            labelBlurb.Text = string.Format(pool.name_label.Length > 0 ? Messages.AD_CONFIGURED_BLURB : Messages.AD_CONFIGURED_BLURB_HOST, Helpers.GetName(pool).Ellipsise(70), Domain);
            pool.Connection.Cache.RegisterBatchCollectionChanged<Subject>(SubjectCollectionChanged);
        }

        private void OnAdDisabled()
        {
            Program.AssertOnEventThread();

            flowLayoutPanel1.Enabled = false;
            SetSubjectListEnable(false);
            buttonJoinLeave.Text = Messages.AD_JOIN_DOMAIN;
            buttonJoinLeave.Enabled = true;
            labelBlurb.Text = string.Format(pool.name_label.Length > 0 ? Messages.AD_NOT_CONFIGURED_BLURB : Messages.AD_NOT_CONFIGURED_BLURB_HOST,
                Helpers.GetName(pool).Ellipsise(70));
            pool.Connection.Cache.DeregisterBatchCollectionChanged<Subject>(SubjectCollectionChanged);
        }

        private void OnAdConfiguring()
        {
            Program.AssertOnEventThread();

            flowLayoutPanel1.Enabled = false;
            SetSubjectListEnable(false);
            buttonJoinLeave.Enabled = false;
            labelBlurb.Text = string.Format(pool.name_label.Length > 0 ? Messages.AD_CONFIGURING_BLURB : Messages.AD_CONFIGURING_BLURB_HOST,
                Helpers.GetName(pool).Ellipsise(70));
        }

        private void OnMasterUnavailable()
        {
            Program.AssertOnEventThread();

            flowLayoutPanel1.Enabled = false;
            SetSubjectListEnable(false);
            buttonJoinLeave.Enabled = false;
            labelBlurb.Text = Messages.AD_MASTER_UNAVAILABLE_BLURB;
        }

        private void RepopulateListBox()
        {
            Program.AssertOnEventThread();

            if (!GridViewSubjectList.Enabled)
                return;

            Dictionary<string, bool> selectedSubjectUuids = new Dictionary<string, bool>();
            Dictionary<string, bool> expandedSubjectUuids = new Dictionary<string, bool>();
            bool rootExpanded = false;
            string topSubject = "";
            if (GridViewSubjectList.FirstDisplayedScrollingRowIndex > 0)
            {
                AdSubjectRow topRow = GridViewSubjectList.Rows[GridViewSubjectList.FirstDisplayedScrollingRowIndex] as AdSubjectRow;
                if (topRow != null && topRow.subject != null)
                    topSubject = topRow.subject.uuid;
            }

            if (GridViewSubjectList.SelectedRows.Count > 0)
            {
                foreach (AdSubjectRow r in GridViewSubjectList.SelectedRows)
                {
                    if (r.subject != null)
                        selectedSubjectUuids.Add(r.subject.uuid, true);
                }
            }
            foreach (AdSubjectRow row in GridViewSubjectList.Rows)
            {
                if (row.Expanded)
                    if (row.subject == null)
                        rootExpanded = true;
                    else
                        expandedSubjectUuids.Add(row.subject.uuid, true);
            }

            try
            {
                _updateInProgress = true;
                GridViewSubjectList.SuspendLayout();

                foreach (AdSubjectRow row in GridViewSubjectList.Rows)
                {
                    if (row.IsLocalRootRow)
                        continue;
                    row.subject.PropertyChanged -= subject_PropertyChanged;
                }

                GridViewSubjectList.Rows.Clear();

                var rows = new List<DataGridViewRow> {new AdSubjectRow(null)}; //local root account

                foreach (Subject subject in pool.Connection.Cache.Subjects) //all other subjects in the pool
                {
                    subject.PropertyChanged += subject_PropertyChanged;
                    rows.Add(new AdSubjectRow(subject));
                }
                GridViewSubjectList.Rows.AddRange(rows.ToArray());

                GridViewSubjectList.Sort(GridViewSubjectList.SortedColumn ?? ColumnSubject,
                    GridViewSubjectList.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);

                // restore old selection, old expansion state and top row
                foreach (AdSubjectRow r in GridViewSubjectList.Rows)
                {
                    r.Selected = !r.IsLocalRootRow && selectedSubjectUuids.ContainsKey(r.subject.uuid);
                    r.Expanded = r.IsLocalRootRow ? rootExpanded : expandedSubjectUuids.ContainsKey(r.subject.uuid);

                    if (!r.IsLocalRootRow && topSubject == r.subject.uuid)
                        GridViewSubjectList.FirstDisplayedScrollingRowIndex = r.Index;
                }
                if (GridViewSubjectList.SelectedRows.Count == 0)
                    GridViewSubjectList.Rows[0].Selected = true;

                HelpersGUI.ResizeGridViewColumnToAllCells(ColumnSubject);
                HelpersGUI.ResizeGridViewColumnToAllCells(ColumnRoles);
                HelpersGUI.ResizeGridViewColumnToAllCells(ColumnStatus);
            }
            finally
            {
                GridViewSubjectList.ResumeLayout();
                _updateInProgress = false;
                EnableButtons();
            }
        }

        private void subject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "roles")
                return;

            var subject = sender as Subject;
            if (subject == null)
                return;

            Program.Invoke(this, () =>
            {
                if (!GridViewSubjectList.Enabled)
                    return;

                var found = (from DataGridViewRow row in GridViewSubjectList.Rows
                    let adRow = row as AdSubjectRow
                    where adRow != null && adRow.subject != null && adRow.subject.opaque_ref == subject.opaque_ref
                    select adRow).FirstOrDefault();

                try
                {
                    GridViewSubjectList.SuspendLayout();

                    if (found == null)
                        GridViewSubjectList.Rows.Add(new AdSubjectRow(subject));
                    else
                        found.RefreshCellContent(subject);
                }
                finally
                {
                    GridViewSubjectList.ResumeLayout();
                    EnableButtons();
                }
            });
        }


        private object statusUpdaterLock = new object();
        /// <summary>
        /// Background thread called periodically to update subjects logged in status.
        /// </summary>
        private void updateLoggedInStatus()
        {
            // This could get a bit spammy with a repeated exception, consider a back off or summary approach if it becomes an issue
            Program.AssertOffEventThread();
            Pool p;
            while (!Disposing && !IsDisposed)
            {
                try
                {

                    bool showing = false;
                    Program.Invoke(this, delegate
                    {
                        showing = Program.MainWindow.TheTabControl.SelectedTab == Program.MainWindow.TabPageAD;

                    });
                    if (showing)
                    {
                        // In case the value gets altered underneath us
                        p = pool;
                        String[] loggedInSids = p.Connection.Session.get_all_subject_identifiers();
                        // Want fast access time for when we take the lock and switch off the background thread
                        Dictionary<string, bool> loggedSids = new Dictionary<string, bool>();
                        foreach (string s in loggedInSids)
                            loggedSids.Add(s, true);
                        Program.Invoke(this, delegate
                        {
                            foreach (AdSubjectRow r in GridViewSubjectList.Rows)
                            {
                                // local root row
                                if (r.IsLocalRootRow)
                                    continue;

                                r.LoggedIn = loggedSids.ContainsKey(r.subject.subject_identifier);
                            }
                            EnableButtons();
                        });
                    }
                    lock (statusUpdaterLock)
                    {
                        Monitor.Wait(statusUpdaterLock, 5000);
                    }
                }
                catch (Exception e)
                {
                    showLoggedInStatusError();
                    log.Error(e);
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        private void showLoggedInStatusError()
        {
            Program.Invoke(this, delegate
            {
                foreach (AdSubjectRow r in GridViewSubjectList.Rows)
                {
                    if (r.IsLocalRootRow)
                        continue;

                    r.SetStatusLost();
                }
            });
        }

        #region Custom AD Row Class

        /// <summary>
        /// Used in the DataGridView on the ConfigureAdDialog. Stores information about the subject and the different text to show if the 
        /// row is expanded or not.
        /// </summary>
        private class AdSubjectRow : DataGridViewRow
        {
            private readonly DataGridViewImageCell _cellExpander = new DataGridViewImageCell();
            private readonly DataGridViewImageCell _cellGroupOrUser = new DataGridViewImageCell();
            private readonly KeyValuePairCell _cellSubjectInfo = new KeyValuePairCell();
            private readonly DataGridViewTextBoxCell _cellRoles = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _cellLoggedIn = new DataGridViewTextBoxCell();

            internal Subject subject { get; private set; }
            private bool expanded;
            private bool loggedIn;
            /// <summary>
            /// The row is created with unknown status until it's updated from outside the class
            /// </summary>
            private bool statusLost = true;

            public bool LoggedIn
            {
                get
                {
                    return loggedIn;
                }
                set
                {
                    loggedIn = value;
                    statusLost = false;
                    RefreshCellContent();
                }
            }

            internal bool IsLocalRootRow
            {
                get { return subject == null; }
            }

            /// <summary>
            /// The full list of the subject's roles
            /// </summary>
            private readonly string expandedRoles = string.Empty;
            /// <summary>
            /// Topmost of the subject's roles
            /// </summary>
            private readonly string contractedRoles = string.Empty;
            /// <summary>
            /// The detailed info from the subject's other_config along with their display name
            /// </summary>
            private readonly List<KeyValuePair<string, string>> expandedSubjectInfo = new List<KeyValuePair<String, String>>();
            /// <summary>
            /// The subject's display name
            /// </summary>
            private readonly List<KeyValuePair<string, string>> contractedSubjectInfo = new List<KeyValuePair<String, String>>();

            /// <summary>
            /// A DataGridViewRow that corresponds to a subject and shows their
            /// information in expanded and collapsed states
            /// </summary>
            /// <param name="subject">If null, if it is a root account (no subject)</param>
            internal AdSubjectRow(Subject subject)
            {
                if (subject == null) //root account
                {
                    expandedSubjectInfo.Add(new KeyValuePair<string, string>(Messages.AD_LOCAL_ROOT_ACCOUNT, ""));
                    expandedSubjectInfo.Add(new KeyValuePair<string, string>("", ""));
                    expandedSubjectInfo.Add(new KeyValuePair<string, string>(Messages.AD_ALWAYS_GRANTED_ACCESS, ""));
                    contractedSubjectInfo.Add(new KeyValuePair<string, string>(Messages.AD_LOCAL_ROOT_ACCOUNT, ""));
                }
                else
                {
                    this.subject = subject;
                    var roles = subject.Connection.ResolveAll(subject.roles);
                    roles.Sort();
                    roles.Reverse();
                    if (roles.Count > 0)
                        expandedRoles = roles.Select(r => r.FriendlyName()).Aggregate((acc, s) => acc + "\n" + s);

                    contractedRoles = roles.Count > 0
                        ? roles.Count > 1 ? roles[0].FriendlyName().AddEllipsis() : roles[0].FriendlyName()
                        : "";

                    contractedSubjectInfo.Add(new KeyValuePair<string, string>(subject.DisplayName ?? subject.SubjectName ?? "", ""));
                    expandedSubjectInfo = Subject.ExtractKvpInfo(subject);
                }

                Cells.AddRange(_cellExpander, _cellGroupOrUser, _cellSubjectInfo, _cellRoles, _cellLoggedIn);
                RefreshCellContent();
            }

            public void RefreshCellContent(Subject subj = null)
            {
                if (subj != null)
                    subject = subj;

                _cellExpander.Value = expanded ? Resources.expanded_triangle : Resources.contracted_triangle;
                _cellGroupOrUser.Value = IsLocalRootRow || !subject.IsGroup ? Resources._000_User_h32bit_16 : Resources._000_UserAndGroup_h32bit_32;
                _cellSubjectInfo.Value = expanded ? expandedSubjectInfo : contractedSubjectInfo;
                _cellRoles.Value = expanded ? expandedRoles : contractedRoles;
                _cellLoggedIn.Value = IsLocalRootRow || subject.IsGroup || statusLost
                    ? "-"
                    : loggedIn ? Messages.YES : Messages.NO;
            }

            public void ToggleExpandedState()
            {
                expanded = !expanded;
                RefreshCellContent();
            }

            public bool Expanded
            {
                get
                {
                    return expanded;
                }
                set
                {
                    if (expanded != value)
                    {
                        expanded = value;
                        RefreshCellContent();
                    }
                }
            }

            public void SetStatusLost()
            {
                if (statusLost)
                    return;

                statusLost = true;
                RefreshCellContent();
            }
        }

        #endregion

        private void buttonJoinLeave_Click(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            if (buttonJoinLeave.Text == Messages.AD_JOIN_DOMAIN)
            {
                // We're enabling AD            
                // Obtain domain, username and password

                joinPrompt.ShowDialog(this);
                // Blocking for a long time, check we haven't had the dialog disposed under us
                if (Disposing || IsDisposed)
                    return;

                if (joinPrompt.DialogResult == DialogResult.Cancel)
                {
                    joinPrompt.ClearPassword();
                    return;
                }

                EnableAdAction action = new EnableAdAction(pool, joinPrompt.Domain, joinPrompt.Username, joinPrompt.Password);
                if (pool.name_label.Length > 0)
                    action.Pool = pool;
                else
                    action.Host = Helpers.GetMaster(pool.Connection);
                action.RunAsync();
                joinPrompt.ClearPassword();
            }
            else
            {
                // We're disabling AD

                // Warn if the user will boot himself out by disabling AD
                Session session = pool.Connection.Session;
                if (session == null)
                    return;

                if (session.IsLocalSuperuser)
                {
                    // User is authenticated using local root account. Confirm anyway.
                    string msg = string.Format(Messages.AD_LEAVE_CONFIRM,
                                Helpers.GetName(pool).Ellipsise(50).EscapeAmpersands(), Domain);

                    DialogResult r;
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            null,
                            msg,
                            Messages.AD_FEATURE_NAME),
                        ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                    {
                        r = dlg.ShowDialog(this);
                    }

                    //CA-64818: DialogResult can be No if the No button has been hit
                    //or Cancel if the dialog has been closed from the control box
                    if (r != DialogResult.Yes)
                        return;
                }
                else
                {
                    // Warn user will be booted out.
                    string msg = string.Format(pool.name_label.Length > 0 ? Messages.AD_LEAVE_WARNING : Messages.AD_LEAVE_WARNING_HOST,
                                Helpers.GetName(pool).Ellipsise(50), Domain);

                    DialogResult r;
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning, msg, Messages.ACTIVE_DIRECTORY_TAB_TITLE),
                        ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                    {
                        r = dlg.ShowDialog(this);
                    }

                    //CA-64818: DialogResult can be No if the No button has been hit
                    //or Cancel if the dialog has been closed from the control box
                    if (r != DialogResult.Yes)
                        return;
                }


                Host master = Helpers.GetMaster(pool.Connection);
                if (master == null)
                {
                    // Really shouldn't happen unless we have been very slow with the cache
                    log.Error("Could not retrieve master when trying to look up domain..");
                    throw new Exception(Messages.CONNECTION_IO_EXCEPTION);
                }
                AdPasswordPrompt passPrompt = new AdPasswordPrompt(false, master.external_auth_service_name);
                DialogResult result = passPrompt.ShowDialog(Program.MainWindow);
                if (result == DialogResult.Cancel)
                    return;

                Dictionary<string, string> creds = new Dictionary<string, string>();
                if (result != DialogResult.Ignore)
                {
                    creds.Add(DisableAdAction.KEY_USER, passPrompt.Username);
                    creds.Add(DisableAdAction.KEY_PASSWORD, passPrompt.Password);
                }
                DisableAdAction action = new DisableAdAction(pool, creds);
                if (pool.name_label.Length > 0)
                    action.Pool = pool;
                else
                    action.Host = Helpers.GetMaster(pool.Connection);
                action.RunAsync();
            }
        }

        private void buttonResolve_Click(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            if (!buttonAdd.Enabled)
                return;

            using (var dlog = new ResolvingSubjectsDialog(pool))
                dlog.ShowDialog();
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            
            // Double check, this method is called from a context menu as well and the state could have changed under it
            if (!ButtonRemove.Enabled)
                return;

            List<Subject> subjectsToRemove = new List<Subject>();
            foreach (AdSubjectRow r in GridViewSubjectList.SelectedRows)
                subjectsToRemove.Add(r.subject);

            var removeMessage = subjectsToRemove.Count == 1
                ? string.Format(Messages.QUESTION_REMOVE_AD_USER_ONE, subjectsToRemove[0].DisplayName ?? subjectsToRemove[0].SubjectName)
                : string.Format(Messages.QUESTION_REMOVE_AD_USER_MANY, subjectsToRemove.Count);

            DialogResult questionDialog;
            using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(
                                    null,
                                    removeMessage,
                                    Messages.AD_FEATURE_NAME),
                                ThreeButtonDialog.ButtonYes,
                                new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
            {
                questionDialog = dlg.ShowDialog(this);
            }

            //CA-64818: DialogResult can be No if the No button has been hit
            //or Cancel if the dialog has been closed from the control box
            if (questionDialog != DialogResult.Yes)
                return;

            // Warn if user is revoking his currently-in-use credentials
            Session session = pool.Connection.Session;
            if (session != null && session.Subject != null)
            {
                foreach (Subject entry in subjectsToRemove)
                {
                    if (entry.opaque_ref == session.Subject)
                    {
                        string subjectName = entry.DisplayName ?? entry.SubjectName;
                        if (subjectName == null)
                        {
                            subjectName = entry.subject_identifier;
                        }
                        else
                        {
                            subjectName = subjectName.Ellipsise(256);
                        }
                        string msg = string.Format(entry.IsGroup ? Messages.AD_CONFIRM_SUICIDE_GROUP : Messages.AD_CONFIRM_SUICIDE,
                                    subjectName, Helpers.GetName(pool).Ellipsise(50));

                        DialogResult r;
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(
                                SystemIcons.Warning,
                                msg,
                                Messages.AD_FEATURE_NAME),
                            ThreeButtonDialog.ButtonYes,
                            new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                        {
                            r = dlg.ShowDialog(this);
                        }

                        //CA-64818: DialogResult can be No if the No button has been hit
                        //or Cancel if the dialog has been closed from the control box
                        if (r != DialogResult.Yes)
                            return;

                        break;
                    }
                }
            }

            var action = new AddRemoveSubjectsAction(pool, new List<string>(), subjectsToRemove);
            using (var dlog = new ActionProgressDialog(action, ProgressBarStyle.Continuous))
                dlog.ShowDialog();
        }

        private void GridViewSubjectList_SelectionChanged(object sender, EventArgs e)
        {
            if (!_updateInProgress)
                EnableButtons();
        }

        private void EnableButtons()
        {
            Program.AssertOnEventThread();
            if (GridViewSubjectList.SelectedRows.Count < 1)
                return;

            bool adminSelected = GridViewSubjectList.Rows[0].Selected;

            tTipChangeRole.SuppressTooltip = ButtonChangeRoles.Enabled = !adminSelected;
            tTipChangeRole.SetToolTip(Messages.AD_CANNOT_MODIFY_ROOT);

            tTipLogoutButton.SuppressTooltip = !adminSelected;
            ButtonLogout.Enabled = !adminSelected && AllSelectedRowsLoggedIn();

            tTipRemoveButton.SuppressTooltip = ButtonRemove.Enabled = !adminSelected;

            toolStripMenuItemChangeRoles.Enabled = ButtonChangeRoles.Enabled;
            toolStripMenuItemLogout.Enabled = ButtonLogout.Enabled;
            toolStripMenuItemRemove.Enabled = ButtonRemove.Enabled;
        }

        private void GridViewSubjectList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != ColumnExpand.Index)
                return;

            var row = GridViewSubjectList.Rows[e.RowIndex] as AdSubjectRow;
            if (row != null)
                row.ToggleExpandedState();
        }

        private void GridViewSubjectList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            var row = GridViewSubjectList.Rows[e.RowIndex] as AdSubjectRow;
            if (row != null)
                row.ToggleExpandedState();
        }

        private bool AllSelectedRowsLoggedIn()
        {
            foreach (AdSubjectRow r in GridViewSubjectList.SelectedRows)
            {
                if (!r.LoggedIn)
                    return false;
            }
            return true;
        }


        private void GridViewSubjectList_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            // First of all, the admin row is always displayed at the top.
            AdSubjectRow row1 = (AdSubjectRow)GridViewSubjectList.Rows[e.RowIndex1];
            AdSubjectRow row2 = (AdSubjectRow)GridViewSubjectList.Rows[e.RowIndex2];
            if (row1.IsLocalRootRow)
            {
                e.SortResult = GridViewSubjectList.SortOrder == SortOrder.Ascending ? -1 : 1;
            }
            else if (row2.IsLocalRootRow)
            {
                e.SortResult = GridViewSubjectList.SortOrder == SortOrder.Ascending ? 1 : -1;
            }
            else
            {
                // Next we look to see which column we are sorting on
                switch (GridViewSubjectList.SortedColumn.Index)
                {
                    case 0: // shouldn't happen (expander column)
                    case 1: // Group/individual picture column
                        e.SortResult = GroupCompare(row1.subject, row2.subject);
                        break;
                    case 2: // Name and detail column
                        e.SortResult = NameCompare(row1.subject, row2.subject);
                        break;
                    case 3: // Role Column
                        e.SortResult = RoleCompare(row1.subject, row2.subject);
                        break;
                    case 4: // Logged in column
                        e.SortResult = LoggedInCompare(row1, row2);
                        break;
                }
            }
            e.Handled = true;
        }

        private int RoleCompare(Subject s1, Subject s2)
        {
            List<Role> s1Roles = pool.Connection.ResolveAll(s1.roles);
            List<Role> s2Roles = pool.Connection.ResolveAll(s2.roles);
            s1Roles.Sort();
            s2Roles.Sort();
            // If one subject doesn't have any roles, but it below the one with roles
            if (s1Roles.Count < 1)
            {
                if (s2Roles.Count < 1)
                {
                    return 0;
                }
                return -1;
            }
            if (s2Roles.Count < 1)
                return 1;

            return s1Roles[0].CompareTo(s2Roles[0]);
        }

        private int NameCompare(Subject s1, Subject s2)
        {
            return StringUtility.NaturalCompare(s1.SubjectName, s2.SubjectName) * -1;
        }

        private int LoggedInCompare(AdSubjectRow s1, AdSubjectRow s2)
        {
            if (s1.LoggedIn)
            {
                if (s2.LoggedIn)
                    return 0;
                return 1;
            }
            else
            {
                if (s2.LoggedIn)
                    return -1;
                return 0;
            }
        }

        private int GroupCompare(Subject s1, Subject s2)
        {
            if (s1.IsGroup)
            {
                if (s2.IsGroup)
                    return 0;
                return 1;
            }
            else
            {
                if (s2.IsGroup)
                    return -1;
                return 0;
            }
        }

        private void ButtonChangeRoles_Click(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            if (Helpers.FeatureForbidden(pool.Connection, Host.RestrictRBAC))
            {
                // Show upsell dialog
                using (var dlg = new UpsellDialog(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_RBAC : Messages.UPSELL_BLURB_RBAC + Messages.UPSELL_BLURB_TRIAL,
                                                    InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL))
                    dlg.ShowDialog(this);
                return;

            }


            // Double check, this method is called from a context menu as well and the state could have changed under it
            if (!ButtonChangeRoles.Enabled)
                return;

            List<Subject> selectedSubjects = new List<Subject>();
            foreach (DataGridViewRow r in GridViewSubjectList.SelectedRows)
            {
                AdSubjectRow selectedRow = (AdSubjectRow)r;
                // Should not be here, you can't change the root man!
                if (selectedRow.IsLocalRootRow)
                    continue;
                selectedSubjects.Add(selectedRow.subject);
            }

            using (var dialog = new RoleSelectionDialog(selectedSubjects.ToArray(), pool))
                dialog.ShowDialog(this);
        }

        private void ButtonLogout_Click(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();
            // Double check, this method is called from a context menu as well and the state could have changed under it
            if (!ButtonLogout.Enabled)
                return;

            Session session = pool.Connection.Session;
            if (session == null)
                return;

            // First we check through the list to check what warning message we show
            List<Subject> subjectsToLogout = new List<Subject>();
            foreach (AdSubjectRow r in GridViewSubjectList.SelectedRows)
            {
                if (r.IsLocalRootRow || !r.LoggedIn)
                    continue;

                subjectsToLogout.Add(r.subject);
            }

            bool suicide = false;
            // Warn if user is logging themselves out
            if (session.Subject != null)//have already checked session not null
            {
                var warnMsg = string.Format(subjectsToLogout.Count > 1 ? Messages.AD_LOGOUT_SUICIDE_MANY : Messages.AD_LOGOUT_SUICIDE_ONE,
                                            Helpers.GetName(pool).Ellipsise(50));

                foreach (Subject entry in subjectsToLogout)
                {
                    if (entry.opaque_ref == session.Subject)
                    {
                        DialogResult r;
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(
                                SystemIcons.Warning,
                                warnMsg,
                                Messages.AD_FEATURE_NAME),
                            ThreeButtonDialog.ButtonYes,
                            new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                        {
                            r = dlg.ShowDialog(this);
                        }

                        //CA-64818: DialogResult can be No if the No button has been hit
                        //or Cancel if the dialog has been closed from the control box
                        if (r != DialogResult.Yes)
                            return;

                        suicide = true;
                        break;
                    }
                }
            }

            var logoutMessage = subjectsToLogout.Count == 1
                ? string.Format(Messages.QUESTION_LOGOUT_AD_USER_ONE, subjectsToLogout[0].DisplayName ?? subjectsToLogout[0].SubjectName)
                : string.Format(Messages.QUESTION_LOGOUT_AD_USER_MANY, subjectsToLogout.Count);

            if (!suicide)//CA-68645
            {
                DialogResult questionDialog;
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(
                        SystemIcons.Warning,
                        logoutMessage,
                        Messages.AD_FEATURE_NAME),
                    ThreeButtonDialog.ButtonYes,
                    new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)))
                {
                    questionDialog = dlg.ShowDialog(this);
                }

                //CA-64818: DialogResult can be No if the No button has been hit
                //or Cancel if the dialog has been closed from the control box
                if (questionDialog != DialogResult.Yes)
                    return;
            }

            // Then we go through the list and disconnect each user session, doing our own last if necessary
            foreach (AdSubjectRow r in GridViewSubjectList.SelectedRows)
            {
                // check they are not the root row and are logged in
                if (r.IsLocalRootRow || !r.LoggedIn)
                    continue;

                if (session.UserSid == r.subject.subject_identifier)
                {
                    continue;
                }
                DelegatedAsyncAction logoutAction = new DelegatedAsyncAction(pool.Connection, Messages.TERMINATING_SESSIONS, Messages.IN_PROGRESS, Messages.COMPLETED, delegate(Session s)
                    {
                        Session.logout_subject_identifier(s, r.subject.subject_identifier);
                    }, "session.logout_subject_identifier");
                logoutAction.RunAsync();
            }
            if (suicide)
            {
                DelegatedAsyncAction logoutAction = new DelegatedAsyncAction(pool.Connection, Messages.TERMINATING_SESSIONS, Messages.IN_PROGRESS, Messages.COMPLETED, delegate(Session s)
                    {
                        Session.logout_subject_identifier(s, session.UserSid);
                        pool.Connection.Logout();
                    }, "session.logout_subject_identifier");
                logoutAction.RunAsync();
            }
            else
            {
                // signal the background thread to update the logged in status
                lock (statusUpdaterLock)
                    Monitor.Pulse(statusUpdaterLock);
            }
        }
    }
}
