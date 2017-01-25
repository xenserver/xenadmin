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
using System.Linq;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Controls;
using System.ComponentModel;
using XenAdmin.Actions;
using XenAdmin.Model;


namespace XenAdmin.SettingsPanels
{
    public partial class GeneralEditPage : UserControl, IEditPage
    {
        private IXenObject xenObjectOrig, xenObjectCopy;

        private bool _ValidToSave = true;
        private bool saveDescription;

        private readonly ToolTip InvalidParamToolTip;

        public bool ValidToSave
        {
            get { return _ValidToSave; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ObjectName
        {
            set
            {
                this.txtName.Text = value;
            }
            get
            {
                return this.txtName.Text.Trim();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ObjectDescription
        {
            get
            {
                return saveDescription
                           ? txtDescription.Text
                           : txtDescrReadOnly.Text;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ServerIQN
        {
            set
            {
                this.txtIQN.Text = value;
            }
            get
            {
                return this.txtIQN.Text.Trim();
            }
        }

        private TagsEditor tagsEditor;
        private FolderEditor folderEditor;

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            if (clone == null)
                return;
            this.xenObjectCopy = clone;
            this.xenObjectOrig = orig;

            if (tagsEditor != null)
                tagsEditor.Dispose();
            tagsEditor = new TagsEditor(GetTagsFromXenObjectCopy(), tagsPanel);
            tagsEditor.Location = new Point(5, 5);
            tagsEditor.Size = new Size(tagsPanel.Width - 10, tagsPanel.Height - 10);
            tagsEditor.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            if (folderEditor != null)
                folderEditor.Dispose();
            folderEditor = new FolderEditor(clone.Path);
            folderEditor.Parent = folderPanel;
            folderEditor.Location = new Point(0, 2);
            folderEditor.Size = new Size(folderPanel.Width - 6, folderPanel.Height - 4);
            folderEditor.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            Repopulate();
        }

        public GeneralEditPage()
        {
            InitializeComponent();

            Text = Messages.NAME_DESCRIPTION_TAGS;

            InvalidParamToolTip = new ToolTip();
            InvalidParamToolTip.IsBalloon = true;
            InvalidParamToolTip.ToolTipIcon = ToolTipIcon.Warning;
            InvalidParamToolTip.ToolTipTitle = Messages.INVALID_PARAMETER;
        }

        internal void SelectName()
        {
            txtName.Select();
        }

        internal void SelectDescription()
        {
            txtDescription.Select();
        }

        internal void SelectIqn()
        {
            txtIQN.Select();
        }

        public Image Image
        {
            get
            {
                return Properties.Resources.edit_16;
            }
        }

        private void Repopulate()
        {
            ObjectName = Helpers.GetName(xenObjectCopy);

            VM vm = xenObjectCopy as VM;
            if (vm != null && vm.DescriptionType == VM.VmDescriptionType.None)
            {
                lblDescription.Visible = false;
                txtDescription.Visible = false;
                lblDescrReadOnly.Visible = false;
                txtDescrReadOnly.Visible = false;
            }
            else if (vm != null && vm.DescriptionType == VM.VmDescriptionType.ReadOnly)
            {
                lblDescription.Visible = false;
                txtDescription.Visible = false;
                lblDescrReadOnly.Visible = true;
                txtDescrReadOnly.Visible = true;
                txtDescrReadOnly.Text = xenObjectCopy.Description;
            }
            else
            {
                lblDescription.Visible = true;
                txtDescription.Visible = true;
                txtDescription.Text = xenObjectCopy.Description;
                lblDescrReadOnly.Visible = false;
                txtDescrReadOnly.Visible = false;
                saveDescription = true;
            }

            if (xenObjectCopy is Host)
            {
                Host host = xenObjectCopy as Host;
                ServerIQN = host.iscsi_iqn;
                txtIQN.Visible = true;
                lblIQN.Visible = true;
                labelIqnHint.Visible = true;
            }

            if (xenObjectCopy is VMPP)
            {
                lblFolder.Visible = labelTags.Visible = folderPanel.Visible = tagsPanel.Visible = false;
                labelTitle.Text = Messages.GENERAL_PAGE_VMPP_SETTINGS;
            }

            if (xenObjectCopy is VMSS)
            {
                lblFolder.Visible = labelTags.Visible = folderPanel.Visible = tagsPanel.Visible = false;
                labelTitle.Text = Messages.GENERAL_PAGE_VMSS_SETTINGS;
            }

            if (xenObjectCopy is VM_appliance)
            {
                lblFolder.Visible = labelTags.Visible = folderPanel.Visible = tagsPanel.Visible = false;
                labelTitle.Text = Messages.GENERAL_PAGE_VM_APPLIANCE_SETTINGS;
            }

            AnyTextChanged(null, null);
        }

        private List<string> GetTagsFromXenObjectCopy()
        {
            var tags = Tags.GetTags(xenObjectCopy);
            if (tags != null)
                return new List<string>(tags);
            else
            {
                return new List<string>();
            }
        }

        private bool FolderChanged
        {
            get
            {
                return folderEditor.Path != xenObjectCopy.Path;
            }
        }

        private bool TagsChanged
        {
            get
            {
                List<string> oldTags = GetTagsFromXenObjectCopy();
                List<string> newTags = tagsEditor.Tags;
                oldTags.Sort();
                newTags.Sort();
                return (!Helper.AreEqual(oldTags, newTags));
            }
        }

        public bool HasChanged
        {
            get
            {
                if (Helpers.GetName(xenObjectCopy) != this.ObjectName ||
                    xenObjectCopy.Description != this.ObjectDescription)
                    return true;

                if (FolderChanged)
                    return true;

                if (xenObjectCopy is Host)
                {
                    Host host = xenObjectCopy as Host;
                    if (host.iscsi_iqn != ServerIQN)
                        return true;
                }

                if (TagsChanged)
                    return true;

                return false;
            }
        }

        public void ShowLocalValidationMessages()
        {
            if (txtName.Text.Trim() == "")
            {
                // Show invalid target host local validation error message.
                HelpersGUI.ShowBalloonMessage(txtName, Messages.GENERAL_EDIT_INVALID_NAME, InvalidParamToolTip);
            }
            else if (xenObjectCopy is Host)
            {
                Host host = xenObjectCopy as Host;

                if (Helpers.ValidateIscsiIQN(ServerIQN) || ServerIQN == host.iscsi_iqn)
                    return;

                // Allow invalid IQN only if previously set from CLI
                HelpersGUI.ShowBalloonMessage(txtIQN, Messages.GENERAL_EDIT_INVALID_IQN, InvalidParamToolTip);
            }
        }

        public void Cleanup()
        {
            if (InvalidParamToolTip != null)
                InvalidParamToolTip.Dispose();
            if (tagsEditor != null)
                tagsEditor.Dispose();
            if (folderEditor != null)
                folderEditor.Dispose();
        }

        public AsyncAction SaveSettings()
        {
            Program.AssertOnEventThread();

            if (ObjectName != "" && ObjectName != Helpers.GetName(xenObjectCopy))
                xenObjectCopy.Set("name_label", ObjectName);
            if (ObjectDescription != xenObjectCopy.Description)
                xenObjectCopy.Set("name_description", ObjectDescription);

            if (xenObjectCopy is Host)
            {
                Host host = xenObjectCopy as Host;
                host.iscsi_iqn = ServerIQN;
            }

            if (FolderChanged || TagsChanged)
                return new GeneralEditPageAction(xenObjectOrig, xenObjectCopy, folderEditor.Path, tagsEditor.Tags, true);
            else
                return null;
        }

        private void AnyTextChanged(object sender, EventArgs e)
        {
            _ValidToSave = txtName.Text.Trim().Length > 0;

            if (xenObjectCopy is Host)
            {
                Host host = xenObjectCopy as Host;

                // Allow invalid IQN only if it was set by CLI
                if (ServerIQN != host.iscsi_iqn)
                {
                    _ValidToSave &= Helpers.ValidateIscsiIQN(txtIQN.Text.Trim());
                }
            }
        }

        public String SubText
        {
            get
            {
                return txtName.Text;
            }
        }

    }
}
