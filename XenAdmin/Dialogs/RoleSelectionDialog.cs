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
using System.Linq;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAdmin.Dialogs
{
    public partial class RoleSelectionDialog : XenDialogBase
    {
        private List<Subject> subjects;

        private Dictionary<Role, List<Subject>> _subjectsPerRole;

        public RoleSelectionDialog()
        {
            InitializeComponent();
        }

        public RoleSelectionDialog(IXenConnection connection, List<Subject> subjects)
            : base(connection)
        {
            InitializeComponent();

            this.subjects = subjects;

            var allAreGroups = subjects.Count > 0 && subjects.All(s => s.IsGroup);
            var allAreUsers = subjects.Count > 0 && subjects.All(s => !s.IsGroup);

            pictureBoxSubjectType.Image = allAreUsers
                ? Images.StaticImages._000_User_h32bit_32
                : Images.StaticImages._000_UserAndGroup_h32bit_32;

            if (subjects.Count == 1)
            {
                Subject subject = subjects[0];
                string sName = (subject.DisplayName ?? subject.SubjectName ?? Messages.UNKNOWN_AD_USER).Ellipsise(30);
                labelBlurb.Text = string.Format(allAreGroups ? Messages.AD_SELECT_ROLE_GROUP : Messages.AD_SELECT_ROLE_USER, sName);
            }
            else
            {
                if (allAreGroups)
                    labelBlurb.Text = Messages.AD_SELECT_ROLE_GROUP_MANY;
                else if (allAreUsers)
                    labelBlurb.Text = Messages.AD_SELECT_ROLE_USER_MANY;
                else
                    labelBlurb.Text = Messages.AD_SELECT_ROLE_MIXED;
            }

            // Get the list of roles off the server and arrange them into rank
            var serverRoles = connection.Cache.Roles
                .Where(r => (!Helpers.CloudOrGreater(connection) || !Helpers.XapiEqualOrGreater_22_5_0(connection) || !r.is_internal) && r.subroles.Count > 0)
                .OrderBy(r => r).Reverse().ToList();
            _subjectsPerRole = new Dictionary<Role, List<Subject>>();

            foreach (Role role in serverRoles)
            {
                var subjectsWithRole = subjects.Where(s => s.roles.Contains(new XenRef<Role>(role.opaque_ref))).ToList();
                _subjectsPerRole[role] = subjectsWithRole;

                var row = new RoleDataGridViewRow(role);

                if (subjectsWithRole.Count == subjects.Count)
                    row.IsChecked = true;
                else if (subjectsWithRole.Count == 0)
                    row.IsChecked = false;

                gridRoles.Rows.Add(row);
            }

            UpdateRoleDescription();
            tableLayoutPanel2.Visible = false;
            buttonSave.Enabled = false;
        }

        public List<Role> SelectedRoles =>
            gridRoles.Rows.Cast<RoleDataGridViewRow>().Where(r => r.IsChecked).Select(r => r.Role).ToList();

        private void UpdateRoleDescription()
        {
            if (gridRoles.SelectedRows.Count == 1 && gridRoles.SelectedRows[0] is RoleDataGridViewRow row)
                labelDescription.Text = row.Role.FriendlyDescription();
        }

        private void UpdateButtonsAndInfo()
        {
            foreach (DataGridViewRow r in gridRoles.Rows)
            {
                if (r is RoleDataGridViewRow row)
                {
                    if (row.IsChecked && _subjectsPerRole[row.Role].Count < subjects.Count)
                    {
                        buttonSave.Enabled =  true;
                        tableLayoutPanel2.Visible = false;
                        return;
                    }

                    if (!row.IsChecked && _subjectsPerRole[row.Role].Count > 0)
                    {
                        buttonSave.Enabled =  true;
                        tableLayoutPanel2.Visible = row.Role.name_label == Role.MR_ROLE_POOL_ADMIN &&
                                                    connection != null && Helpers.StockholmOrGreater(connection) &&
                                                    !connection.Cache.Hosts.Any(Host.RestrictPoolSecretRotation);
                        return;
                    }
                }
            }

            tableLayoutPanel2.Visible = false;
            buttonSave.Enabled = false;
        }

        private void gridRoles_SelectionChanged(object sender, EventArgs e)
        {
            UpdateRoleDescription();
        }

        private void gridRoles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //handle mouse clicks only on the check box column because the user should be able
            //to select rows by clicking but without checking them so as to browse the descriptions

            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex >= gridRoles.RowCount)
                return;

            CheckRowByIndex(e.RowIndex);
        }

        private void gridRoles_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Space || gridRoles.SelectedRows.Count != 1) 
                return;

            CheckRowByIndex(gridRoles.SelectedRows[0].Index);
        }

        private void CheckRowByIndex(int index)
        {
            foreach (DataGridViewRow r in gridRoles.Rows)
            {
                if (r is RoleDataGridViewRow row)
                    row.IsChecked = index == r.Index;
            }

            UpdateButtonsAndInfo();
        }


        private class RoleDataGridViewRow : DataGridViewRow
        {
            private readonly DataGridViewCheckBoxCell _c1 = new DataGridViewCheckBoxCell {ThreeState = true};
            private readonly DataGridViewTextBoxCell _c2 = new DataGridViewTextBoxCell();

            public RoleDataGridViewRow(Role role)
            {
                Role = role;
                _c1.Value = CheckState.Indeterminate;
                _c2.Value = role.FriendlyName();
                Cells.AddRange(_c1, _c2);
            }

            public Role Role { get; }

            public bool IsChecked
            {
                get => (CheckState)_c1.Value == CheckState.Checked;
                set => _c1.Value = value ? CheckState.Checked : CheckState.Unchecked;
            }
        }
    }
}