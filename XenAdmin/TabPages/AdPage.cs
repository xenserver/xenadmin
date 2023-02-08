/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;
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
        private Host coordinator;
        private IXenConnection _connection;

        private Thread _loggedInStatusUpdater;
        private bool _updateInProgress;
        private readonly object _statusUpdaterLock = new object();

        private string _storedDomain;
        private string _storedUsername;

        private readonly CollectionChangeEventHandler Pool_CollectionChangedWithInvoke;
        public IXenObject XenObject
        {
            set
            {
                Program.AssertOnEventThread();

                ClearHandles();

                _connection = value == null ? null : value.Connection;
                if (_connection == null)
                    return;

                pool = Helpers.GetPoolOfOne(_connection);
                if (pool == null) // Cache not populated
                    _connection.Cache.RegisterCollectionChanged<Pool>(Pool_CollectionChangedWithInvoke);
                else
                    pool.PropertyChanged += pool_PropertyChanged;

                if (_connection.Session != null)
                    _connection.Session.PropertyChanged += Session_PropertyChanged;
                checkAdType();

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
        }

        public override string HelpID => "TabPageAD";

        /// <summary>
        /// This method is used when the cache was not populated by the time we set the XenObject. It sets the appropriate event handlers,
        /// references to the coordinator and the pool, and populates the tab with the correct configuration. It de-registers
        /// itself when successful.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Pool_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            pool = Helpers.GetPoolOfOne(_connection);
            if (pool == null) // Cache not populated
                return;

            _connection.Cache.DeregisterCollectionChanged<Pool>(Pool_CollectionChangedWithInvoke);
            pool.PropertyChanged += pool_PropertyChanged;

            Program.Invoke(this, checkAdType);
        }

        private void RefreshCoordinator()
        {
            if (coordinator != null)
                coordinator.PropertyChanged -= coordinator_PropertyChanged;

            coordinator = Helpers.GetCoordinator(_connection);
            if (coordinator != null)
                coordinator.PropertyChanged += coordinator_PropertyChanged;
        }

        /// <summary>
        /// Clears all event handles that could be set in this control.
        /// </summary>
        private void ClearHandles()
        {
            if (pool != null)
                pool.PropertyChanged -= pool_PropertyChanged;

            if (coordinator != null)
                coordinator.PropertyChanged -= coordinator_PropertyChanged;

            if (_connection != null)
            {
                _connection.Cache.DeregisterBatchCollectionChanged<Subject>(SubjectCollectionChanged);
                if (_connection.Session != null)
                    _connection.Session.PropertyChanged -= Session_PropertyChanged;
            }
        }

        public override void PageHidden()
        {
            ClearHandles();
        }

        /// <summary>
        /// We keep track of the actions currently running so we can disable the tab if we are in the middle of configuring AD.
        /// </summary>
        void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add &&
                (e.Element is EnableAdAction || e.Element is DisableAdAction))
            {
                AsyncAction action = (AsyncAction)e.Element;
                action.Completed += action_Completed;

                if (_connection != null && _connection == action.Connection)
                    Program.Invoke(this, checkAdType);
            }
        }

        void action_Completed(ActionBase sender)
        {
            AsyncAction action = (AsyncAction)sender;
            action.Completed -= action_Completed;

            if (_connection != null && _connection == action.Connection)
                Program.Invoke(this, checkAdType);
        }

        void Session_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.BeginInvoke(this, RepopulateListBox);
        }

        /// <summary>
        /// We need to update the configuration if the authentication method changes, and also various labels display the name of the
        /// coordinator and should also be updated if that changes.
        /// </summary>
        /// <param name="sender1"></param>
        /// <param name="e"></param>
        void coordinator_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "external_auth_type" || e.PropertyName == "name_label")
                Program.Invoke(this, checkAdType);
        }

        /// <summary>
        /// Various labels display the name of the pool and should also be updated if that changes.
        /// Additionally if the pool coordinator changes we need to update our event handles.
        /// There is a sanity check in the checkAdType() method in case this event is stuck in a queue.
        /// </summary>
        void pool_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "master")
                Program.Invoke(this, checkAdType);
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
            //refresh the coordinator in case the cache is slow
            RefreshCoordinator();

            if (coordinator == null)
            {
                log.WarnFormat("Could not resolve pool coordinator for connection '{0}'; disabling.", Helpers.GetName(_connection));
                OnCoordinatorUnavailable();
                return;
            }

            var action = (from ActionBase act in ConnectionsManager.History
                          let async = act as AsyncAction
                          where async != null && !async.IsCompleted && !async.Cancelled && async.Connection == _connection
                                && (async is EnableAdAction || async is DisableAdAction)
                          select async).FirstOrDefault();

            if (action != null)
            {
                OnAdConfiguring();
            }
            else if (coordinator.external_auth_type == Auth.AUTH_TYPE_NONE) // AD is not yet configured
            {
                OnAdDisabled();
            }
            else // AD is already configured
            {
                if (coordinator.external_auth_type != Auth.AUTH_TYPE_AD)
                {
                    log.WarnFormat("Unrecognised value '{0}' for external_auth_type on pool coordinator '{1}' for pool '{2}'; assuming AD enabled on pool.",
                        coordinator.external_auth_type, Helpers.GetName(coordinator), Helpers.GetName(_connection));
                }

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

                Host coordinator = Helpers.GetCoordinator(_connection);
                if (coordinator == null)
                {
                    log.WarnFormat("Could not resolve pool coordinator for connection '{0}'; disabling.", Helpers.GetName(_connection));
                    return Messages.UNKNOWN;
                }

                return coordinator.external_auth_service_name;
            }
        }

        private void OnAdEnabled()
        {
            Program.AssertOnEventThread();

            flowLayoutPanel1.Enabled = true;
            SetSubjectListEnable(true);
            buttonJoinLeave.Text = Messages.AD_LEAVE_DOMAIN;
            buttonJoinLeave.Enabled = true;
            labelBlurb.Text = string.Format(Helpers.GetPool(_connection) != null ? Messages.AD_CONFIGURED_BLURB : Messages.AD_CONFIGURED_BLURB_HOST, Helpers.GetName(_connection).Ellipsise(70), Domain.Ellipsise(30));
            _connection.Cache.RegisterBatchCollectionChanged<Subject>(SubjectCollectionChanged);
        }

        private void OnAdDisabled()
        {
            Program.AssertOnEventThread();

            flowLayoutPanel1.Enabled = false;
            SetSubjectListEnable(false);
            buttonJoinLeave.Text = Messages.AD_JOIN_DOMAIN;
            buttonJoinLeave.Enabled = true;
            labelBlurb.Text = string.Format(Helpers.GetPool(_connection) != null ? Messages.AD_NOT_CONFIGURED_BLURB : Messages.AD_NOT_CONFIGURED_BLURB_HOST,
                Helpers.GetName(_connection).Ellipsise(70));
            _connection.Cache.DeregisterBatchCollectionChanged<Subject>(SubjectCollectionChanged);
        }

        private void OnAdConfiguring()
        {
            Program.AssertOnEventThread();

            flowLayoutPanel1.Enabled = false;
            SetSubjectListEnable(false);
            buttonJoinLeave.Enabled = false;
            labelBlurb.Text = string.Format(Helpers.GetPool(_connection) != null ? Messages.AD_CONFIGURING_BLURB : Messages.AD_CONFIGURING_BLURB_HOST,
                Helpers.GetName(_connection).Ellipsise(70));
        }

        private void OnCoordinatorUnavailable()
        {
            Program.AssertOnEventThread();

            flowLayoutPanel1.Enabled = false;
            SetSubjectListEnable(false);
            buttonJoinLeave.Enabled = false;
            labelBlurb.Text = Messages.AD_COORDINATOR_UNAVAILABLE_BLURB;
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

                var rows = new List<DataGridViewRow> { new AdSubjectRow(null) }; //local root account

                foreach (Subject subject in _connection.Cache.Subjects) //all other subjects in the pool
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
            if (e.PropertyName != "roles" && e.PropertyName != "other_config")
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


        /// <summary>
        /// Background thread called periodically to update subjects logged in status.
        /// </summary>
        private void updateLoggedInStatus()
        {
            // This could get a bit spammy with a repeated exception, consider a back off or summary approach if it becomes an issue
            Program.AssertOffEventThread();

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
                        String[] loggedInSids = _connection.Session.get_all_subject_identifiers();
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
                    lock (_statusUpdaterLock)
                    {
                        Monitor.Wait(_statusUpdaterLock, 5000);
                    }
                }
                catch (Exception e)
                {
                    showLoggedInStatusError();
                    log.Error(e);
                    Thread.Sleep(5000);
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

                _cellExpander.Value = expanded ? Images.StaticImages.expanded_triangle : Images.StaticImages.contracted_triangle;
                _cellGroupOrUser.Value = IsLocalRootRow || !subject.IsGroup ? Images.StaticImages._000_User_h32bit_16 : Images.StaticImages._000_UserAndGroup_h32bit_16;
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
            if (buttonJoinLeave.Text == Messages.AD_JOIN_DOMAIN)
            {
                // We're enabling AD            
                // Obtain domain, username and password; store the domain and username
                // so the user won't have to retype it for future join attempts

                using (var joinPrompt = new AdPasswordPrompt(true)
                { Domain = _storedDomain, Username = _storedUsername })
                {
                    var result = joinPrompt.ShowDialog(this);
                    _storedDomain = joinPrompt.Domain;
                    _storedUsername = joinPrompt.Username;

                    if (result == DialogResult.Cancel)
                        return;

                    new EnableAdAction(_connection, joinPrompt.Domain, joinPrompt.Username, joinPrompt.Password).RunAsync();
                }
            }
            else
            {
                // We're disabling AD

                // Warn if the user will boot himself out by disabling AD
                Session session = _connection.Session;
                if (session == null)
                    return;

                if (session.IsLocalSuperuser)
                {
                    // User is authenticated using local root account. Confirm anyway.
                    string msg = string.Format(Messages.AD_LEAVE_CONFIRM,
                                Helpers.GetName(_connection).Ellipsise(50).EscapeAmpersands(), Domain.Ellipsise(30));

                    DialogResult r;
                    using (var dlg = new NoIconDialog(msg,
                        ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true))
                    { WindowTitle = Messages.AD_FEATURE_NAME })
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
                    string msg = string.Format(Helpers.GetPool(_connection) != null ? Messages.AD_LEAVE_WARNING : Messages.AD_LEAVE_WARNING_HOST,
                                Helpers.GetName(_connection).Ellipsise(50), Domain.Ellipsise(30));

                    DialogResult r;
                    using (var dlg = new WarningDialog(msg,
                        ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true))
                    { WindowTitle = Messages.ACTIVE_DIRECTORY_TAB_TITLE })
                    {
                        r = dlg.ShowDialog(this);
                    }

                    //CA-64818: DialogResult can be No if the No button has been hit
                    //or Cancel if the dialog has been closed from the control box
                    if (r != DialogResult.Yes)
                        return;
                }

                Host coordinator = Helpers.GetCoordinator(_connection);
                if (coordinator == null)
                {
                    // Really shouldn't happen unless we have been very slow with the cache
                    log.Error("Could not retrieve coordinator when trying to look up domain..");
                    throw new Exception(Messages.CONNECTION_IO_EXCEPTION);
                }

                using (var passPrompt = new AdPasswordPrompt(false, coordinator.external_auth_service_name))
                {
                    var result = passPrompt.ShowDialog(this);
                    if (result == DialogResult.Cancel)
                        return;

                    var creds = new Dictionary<string, string>();
                    if (result != DialogResult.Ignore)
                    {
                        creds.Add(DisableAdAction.KEY_USER, passPrompt.Username);
                        creds.Add(DisableAdAction.KEY_PASS, passPrompt.Password);
                    }

                    new DisableAdAction(_connection, creds).RunAsync();
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (!buttonAdd.Enabled)
                return;

            using (var dlog = new ResolvingSubjectsDialog(_connection))
                dlog.ShowDialog(this);
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            // Double check, this method is called from a context menu as well and the state could have changed under it
            if (!ButtonRemove.Enabled)
                return;

            var subjectsToRemove = GridViewSubjectList.SelectedRows.Cast<AdSubjectRow>().Select(r => r.subject).ToList();
            if (subjectsToRemove.Count < 1)
                return;

            var removeMessage = subjectsToRemove.Count == 1
                ? string.Format(Messages.AD_REMOVE_USER_ONE, subjectsToRemove[0].DisplayName ?? subjectsToRemove[0].SubjectName)
                : string.Format(Messages.AD_REMOVE_USER_MANY, subjectsToRemove.Count);

            string adminMessage = null;
            var conn = subjectsToRemove.FirstOrDefault(s => s.Connection != null)?.Connection;

            if (conn != null && Helpers.StockholmOrGreater(conn) && !conn.Cache.Hosts.Any(Host.RestrictPoolSecretRotation))
            {
                var poolAdminsToRemove = (from Subject s in subjectsToRemove
                                          let roles = s.Connection.ResolveAll(s.roles)
                                          where roles.Any(r => r.name_label == Role.MR_ROLE_POOL_ADMIN)
                                          select s).ToList();

                if (subjectsToRemove.Count == poolAdminsToRemove.Count)
                    adminMessage = poolAdminsToRemove.Count == 1
                        ? Messages.QUESTION_ADMIN_EXIT_PROCEDURE_ONE
                        : Messages.QUESTION_ADMIN_EXIT_PROCEDURE_MANY;
                else if (poolAdminsToRemove.Count > 0)
                    adminMessage = poolAdminsToRemove.Count == 1
                        ? Messages.QUESTION_ADMIN_EXIT_PROCEDURE_ONE_OF_MANY
                        : string.Format(Messages.QUESTION_ADMIN_EXIT_PROCEDURE_SOME_OF_MANY, poolAdminsToRemove.Count);

                if (!string.IsNullOrEmpty(adminMessage))
                    removeMessage = string.Format("{0}\n\n{1} {2}", removeMessage, adminMessage, Messages.QUESTION_ADMIN_EXIT_PROCEDURE_ADVISORY);
            }

            using (var dlg = new WarningDialog(removeMessage,
                                ThreeButtonDialog.ButtonYes,
                                new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true))
            { WindowTitle = Messages.AD_FEATURE_NAME })
            {
                //CA-64818: DialogResult can be No if the No button has been hit
                //or Cancel if the dialog has been closed from the control box

                if (dlg.ShowDialog(this) != DialogResult.Yes)
                    return;
            }

            // Warn if user is revoking his currently-in-use credentials
            Session session = _connection.Session;
            if (session != null && session.SessionSubject != null)
            {
                foreach (Subject entry in subjectsToRemove)
                {
                    if (entry.opaque_ref == session.SessionSubject)
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
                        string msg = string.Format(entry.IsGroup ? Messages.AD_CONFIRM_LOGOUT_CURRENT_USER_GROUP : Messages.AD_CONFIRM_LOGOUT_CURRENT_USER,
                                    subjectName, Helpers.GetName(_connection).Ellipsise(50));

                        DialogResult r;
                        using (var dlg = new WarningDialog(msg,
                            ThreeButtonDialog.ButtonYes,
                            new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true))
                        { WindowTitle = Messages.AD_FEATURE_NAME })
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

            var action = new AddRemoveSubjectsAction(_connection, new List<string>(), subjectsToRemove);
            using (var dlog = new ActionProgressDialog(action, ProgressBarStyle.Continuous))
                dlog.ShowDialog(this);
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

            bool adminSelected = false;
            bool allSelectedLoggedIn = true;

            foreach (AdSubjectRow r in GridViewSubjectList.SelectedRows)
            {
                if (r.IsLocalRootRow)
                    adminSelected = true;

                if (!r.LoggedIn)
                    allSelectedLoggedIn = false;
            }

            tTipChangeRole.SuppressTooltip = ButtonChangeRoles.Enabled = !adminSelected;
            tTipChangeRole.SetToolTip(Messages.AD_CANNOT_MODIFY_ROOT);

            tTipLogoutButton.SuppressTooltip = !adminSelected;
            ButtonLogout.Enabled = !adminSelected && allSelectedLoggedIn;

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
            List<Role> s1Roles = _connection.ResolveAll(s1.roles);
            List<Role> s2Roles = _connection.ResolveAll(s2.roles);
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

        private static string GetFormattedSubjectList(List<Subject> selectedSubjects)
        {
            var listOfSubjects = string.Join(", ",
                selectedSubjects.Select(sub => sub.DisplayName ?? sub.SubjectName ?? sub.subject_identifier).ToList()
            );
            return listOfSubjects;
        }

        private void ButtonChangeRoles_Click(object sender, EventArgs e)
        {
            if (Helpers.FeatureForbidden(_connection, Host.RestrictRBAC))
            {
                UpsellDialog.ShowUpsellDialog(string.Format(Messages.UPSELL_BLURB_RBAC, BrandManager.ProductBrand), this);
                return;
            }

            // Double check, this method is called from a context menu as well and the state could have changed under it
            if (!ButtonChangeRoles.Enabled)
                return;

            var selectedRows = GridViewSubjectList.SelectedRows.Cast<AdSubjectRow>().ToList();
            var selectedSubjects = selectedRows.Where(r => !r.IsLocalRootRow).Select(r => r.subject).ToList();
            var loggedInSelectedSubjects = selectedRows.Where(r => !r.IsLocalRootRow && r.LoggedIn).Select(r => r.subject).ToList();

            using (var dialog = new RoleSelectionDialog(_connection, selectedSubjects))
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    var selectedRoles = dialog.SelectedRoles.OrderBy(x => x).ToList();
                    var subjectsToLogout = new List<Subject>();
                    var logSelfOut = false;

                    foreach (var subject in loggedInSelectedSubjects)
                    {
                        var subjectRoles = subject.roles.Select(_connection.Resolve).OrderBy(x => x).ToList();

                        if (!selectedRoles.SequenceEqual(subjectRoles))
                            subjectsToLogout.Add(subject);

                        if (subject.opaque_ref == _connection.Session.SessionSubject)
                            logSelfOut = true;
                    }

                    if (subjectsToLogout.Count > 0 && !ConfirmLogout(logSelfOut, true, subjectsToLogout))
                        return;

                    var actions = new List<AsyncAction>();
                    var successfulSubjects = new List<Subject>();

                    foreach (var subject in selectedSubjects)
                    {
                        var action = new AddRemoveRolesAction(_connection, subject, dialog.SelectedRoles);
                        action.Completed += actionBase =>
                        {
                            if (!actionBase.IsCancelled && actionBase.Exception == null)
                                successfulSubjects.Add(subject);
                        };
                        actions.Add(action);
                    }

                    var format = selectedSubjects.Count > 1 ? Messages.AD_ADDING_REMOVING_ROLES_ON_MULTIPLE : Messages.AD_ADDING_REMOVING_ROLES_ON;
                    var actionTitle = string.Format(format, GetFormattedSubjectList(selectedSubjects));

                    var updateRolesAction = new MultipleAction(_connection, actionTitle, Messages.IN_PROGRESS, Messages.COMPLETED, actions, true, true);
                    updateRolesAction.Completed += actionBase => Program.Invoke(this, () =>
                    {
                        if (actionBase.IsCancelled)
                            return;

                        var successfulSubjectsToLogOut = successfulSubjects.Where(subjectsToLogout.Contains).ToList();
                        if (successfulSubjectsToLogOut.Count <= 0)
                            return;

                        LogoutSubjects(_connection.Session, successfulSubjectsToLogOut);
                    });
                    updateRolesAction.RunAsync();
                }
        }

        private void ButtonLogout_Click(object sender, EventArgs e)
        {
            // Double check, this method is called from a context menu as well and the state could have changed under it
            if (!ButtonLogout.Enabled)
                return;

            var session = _connection.Session;
            if (session == null)
                return;

            var subjectsToLogout = new List<Subject>();
            var logSelfOut = false;

            foreach (AdSubjectRow r in GridViewSubjectList.SelectedRows)
            {
                if (r.IsLocalRootRow || !r.LoggedIn)
                    continue;

                subjectsToLogout.Add(r.subject);

                if (session.SessionSubject != null && r.subject.opaque_ref == session.SessionSubject)
                    logSelfOut = true;
            }

            if (subjectsToLogout.Count <= 0 || !ConfirmLogout(logSelfOut, false, subjectsToLogout))
                return;

            LogoutSubjects(session, subjectsToLogout);
        }

        private void LogoutSubjects(Session session, List<Subject> subjectsToLogout)
        {
            // We go through the list and disconnect each user session, doing our own last if necessary

            var currentSubject = subjectsToLogout.FirstOrDefault(subject => subject.subject_identifier == session.UserSid);
            var otherSubjects = subjectsToLogout.Where(subject => subject.subject_identifier != session.UserSid).ToList();

            if (otherSubjects.Count == 0 && currentSubject != null)
            {
               LogOutCurrentSubject(currentSubject);
                return;
            }

            var actions = otherSubjects.Select(NewLogOutSubjectAction).ToList();
            var format = otherSubjects.Count > 1 ? Messages.TERMINATING_USER_SESSION_MULTIPLE : Messages.TERMINATING_USER_SESSION;
            var actionTitle = string.Format(format, GetFormattedSubjectList(otherSubjects));

            var logoutAction = new MultipleAction(_connection, actionTitle, Messages.IN_PROGRESS, Messages.COMPLETED, actions, true, true);
            logoutAction.Completed += actionBase => Program.Invoke(this, () =>
            {
                if (!(actionBase is MultipleAction ma) || actionBase.IsCancelled)
                    return;

                if (currentSubject != null)
                {
                    //passing in elevated credentials avoids multiple prompts
                    LogOutCurrentSubject(currentSubject, new AsyncAction.SudoElevationResult(ma.sudoUsername, ma.sudoPassword, null));
                }
                else
                {
                    // signal the background thread to update the logged in status
                    lock (_statusUpdaterLock)
                        Monitor.Pulse(_statusUpdaterLock);
                }
            });
            logoutAction.RunAsync();
        }

        private void LogOutCurrentSubject(Subject currentSubject, AsyncAction.SudoElevationResult sudoElevation = null)
        {
            var action = NewLogOutSubjectAction(currentSubject);
            action.Completed += actionBase => Program.Invoke(this, () =>
            {
                if (actionBase.IsCancelled)
                    return;

                //Session.logout_subject_identifier logs out all sessions except the current one,
                //so if an elevated session was not needed, the current session will not have been
                //logged out, hence we need to disconnect explicitly.
                new DisconnectCommand(Program.MainWindow, _connection, false).Run();
            });
            action.RunAsync(sudoElevation);
        }

        private AsyncAction NewLogOutSubjectAction(Subject subject)
        {
            return new DelegatedAsyncAction(_connection,
                string.Format(Messages.TERMINATING_USER_SESSION, subject.DisplayName ?? subject.SubjectName),
                Messages.IN_PROGRESS, Messages.COMPLETED,
                s => Session.logout_subject_identifier(s, subject.subject_identifier),
                "session.logout_subject_identifier");
        }

        private bool ConfirmLogout(bool logSelfOut, bool isRoleChange, List<Subject> subjectsToLogout)
        {
            var logoutCurrentSubject = false;

            if (logSelfOut)
            {
                var format =
                    isRoleChange
                        ? subjectsToLogout.Count > 1
                            ? Messages.AD_LOGOUT_CURRENT_USER_MANY_ROLE_CHANGE
                            : Messages.AD_LOGOUT_CURRENT_USER_ONE_ROLE_CHANGE
                        : subjectsToLogout.Count > 1
                            ? Messages.AD_LOGOUT_CURRENT_USER_MANY
                            : Messages.AD_LOGOUT_CURRENT_USER_ONE;

                var warnMsg = string.Format(format, Helpers.GetName(_connection).Ellipsise(50));

                using (var dlg = new WarningDialog(warnMsg,
                        ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true))
                { WindowTitle = Messages.AD_FEATURE_NAME })
                {
                    //CA-64818: DialogResult can be No if the No button has been hit
                    //or Cancel if the dialog has been closed from the control box
                    if (dlg.ShowDialog(this) != DialogResult.Yes)
                        return false;

                    logoutCurrentSubject = true;
                }

                if (!DisconnectCommand.ConfirmCancelRunningActions(Program.MainWindow, this, _connection, true))
                    return false;
            }

            if (!logoutCurrentSubject) //CA-68645
            {
                var logoutMessage =
                    isRoleChange
                        ? subjectsToLogout.Count == 1
                            ? string.Format(Messages.AD_LOGOUT_USER_ONE_ROLE_CHANGE,
                                subjectsToLogout[0].DisplayName ?? subjectsToLogout[0].SubjectName)
                            : string.Format(Messages.AD_LOGOUT_USER_MANY_ROLE_CHANGE, subjectsToLogout.Count)
                        : subjectsToLogout.Count == 1
                            ? string.Format(Messages.AD_LOGOUT_USER_ONE,
                                subjectsToLogout[0].DisplayName ?? subjectsToLogout[0].SubjectName)
                            : string.Format(Messages.AD_LOGOUT_USER_MANY, subjectsToLogout.Count);

                using (var dlg = new WarningDialog(logoutMessage,
                        ThreeButtonDialog.ButtonYes,
                        new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, selected: true))
                { WindowTitle = Messages.AD_FEATURE_NAME })
                {
                    //CA-64818: DialogResult can be No if the No button has been hit
                    //or Cancel if the dialog has been closed from the control box
                    if (dlg.ShowDialog(this) != DialogResult.Yes)
                        return false;
                }
            }

            return true;
        }
    }
}
