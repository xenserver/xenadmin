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
using XenAPI;
using XenAdmin.Controls;
using XenCenterLib;
using System.Collections;
using System.Linq;
using XenAdmin.Core;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_Template : XenTabPage
    {
        private bool templatesLoaded;
        private VM m_selectedTemplate;

        public Page_Template()
        {
            InitializeComponent();
        }

        private void searchTextBox1_TextChanged(object sender, EventArgs e)
        {
            RefreshRows();
        }

        #region XentabPage overrides

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (templatesLoaded)
                return;

            PopulateTemplatesBox();
            templatesLoaded = true;

            if (m_selectedTemplate == null)
                return;

            foreach (TemplatesGridViewItem template in TemplatesGridView.Rows)
            {
                if (template.Template.opaque_ref == m_selectedTemplate.opaque_ref)
                {
                    template.Selected = true;
                    TemplatesGridView.CurrentCell = template.Cells[0];
                    break;
                }
            }
        }

        public override string Text => Messages.NEWVMWIZARD_TEMPLATEPAGE_NAME;

        public override string PageTitle => Messages.NEWVMWIZARD_TEMPLATEPAGE_TITLE;

        public override string HelpID => "Template";

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> summ = new List<KeyValuePair<string, string>>();
                summ.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_TEMPLATEPAGE_TEMPLATE, SelectedTemplate.name_label));
                return summ;
            }
        }

        public override bool EnableNext()
        {
            return SelectedTemplate != null && TemplatesGridView.SelectedRows.Count > 0 && TemplatesGridView.SelectedRows[0].Visible;
        }

        #endregion

        public bool CopyBiosStrings => checkBoxCopyBiosStrings.Checked;

        public VM SelectedTemplate
        {
            get
            {
                if (TemplatesGridView.SelectedRows.Count == 0)
                    return null;

                return ((TemplatesGridViewItem)TemplatesGridView.SelectedRows[0]).Template;
            }
            set
            {
                if (value == null)
                    return;

                m_selectedTemplate = value;
            }
        }

        private void PopulateTemplatesBox()
        {
            AddRows();
            RefreshRows();
        }

        private void AddRows()
        {
            List<TemplatesGridViewItem> selectedRows = new List<TemplatesGridViewItem>();

            foreach (TemplatesGridViewItem item in TemplatesGridView.SelectedRows)
                selectedRows.Add(item);

            TemplatesGridView.SuspendLayout();
            TemplatesGridView.Rows.Clear();

            foreach (VM vm in Connection.Cache.VMs)
            {
                if (!vm.is_a_template || !vm.Show(Properties.Settings.Default.ShowHiddenVMs))
                    continue;

                if (vm.Connection.Cache.Hosts.Any(Host.RestrictVtpm) &&
                    vm.platform.TryGetValue("vtpm", out var result) && result.ToLower() == "true")
                    continue;

                TemplatesGridView.Rows.Add(new TemplatesGridViewItem(vm));
            }

            foreach (TemplatesGridViewItem temp in TemplatesGridView.Rows)
            {
                if (selectedRows.Contains(temp))
                    temp.Selected = true;
            }

            TemplatesGridView.ResumeLayout();
            RowsChanged();

            TemplatesGridView.Sort(new Sorter());
        }

        private void RefreshRows()
        {
            var rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in TemplatesGridView.Rows)
                rows.Add(row);

            TemplatesGridView.Rows.Clear();

            foreach (TemplatesGridViewItem item in rows)
                item.Visible = searchTextBox1.Matches(item.Template.Name());

            TemplatesGridView.Rows.AddRange(rows.ToArray());
            RowsChanged();
            if (TemplatesGridView.Rows.Count > 0)
            {
                foreach (TemplatesGridViewItem item in TemplatesGridView.Rows)
                {
                    if (item.Visible)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
        }

        private void RowsChanged()
        {
            if (TemplatesGridView.SelectedRows.Count == 0 || !TemplatesGridView.SelectedRows[0].Visible)
            {
                DescriptionBox.Visible = false;
                checkBoxCopyBiosStrings.Enabled = false;
                checkBoxCopyBiosStrings.Checked = false;
            }
            else
            {
                TemplatesGridViewItem item = (TemplatesGridViewItem)TemplatesGridView.SelectedRows[0];

                if (item.Template.DescriptionType() == VM.VmDescriptionType.None)
                    DescriptionBox.Visible = false;
                else
                {
                    var description = item.Template.Description();
                    DescriptionLabel.Text = string.IsNullOrEmpty(description) ? Messages.TEMPLATE_NO_DESCRIPTION : description;
                    DescriptionBox.Visible = true;
                }

                checkBoxCopyBiosStrings.Enabled = item.Template.DefaultTemplate() && item.Template.IsHVM();
            }

            OnPageUpdated();
        }

        private void TemplatesGridView_SelectionChanged(object sender, EventArgs e)
        {
            bool checkBoxPreviouslyEnabled = checkBoxCopyBiosStrings.Enabled;
            RowsChanged();

            if (TemplatesGridView.SelectedRows.Count > 0)
            {
                TemplatesGridViewItem item = (TemplatesGridViewItem)TemplatesGridView.SelectedRows[0];
                if (!checkBoxPreviouslyEnabled || !checkBoxCopyBiosStrings.Enabled)
                {
                    checkBoxCopyBiosStrings.Checked = item.Template.BiosStringsCopied();
                }
            }
        }

        private class Sorter : IComparer
        {
            public int Compare(object x, object y)
            {
                var xItem = x as TemplatesGridViewItem;
                var yItem = y as TemplatesGridViewItem;

                if (xItem == null && yItem == null)
                    return 0;
                if (xItem == null)
                    return -1;
                if (yItem == null)
                    return 1;

                int result = xItem.SortOrder - yItem.SortOrder;
                if (result != 0)
                    return result;
                
                result = StringUtility.NaturalCompare(xItem.Template.Name(), yItem.Template.Name());
                if (result != 0)
                    return result;
                
                return xItem.Template.opaque_ref.CompareTo(yItem.Template.opaque_ref);
            }
        }

        public class TemplatesGridViewItem : DataGridViewRow, IEquatable<TemplatesGridViewItem>
        {
            public readonly VM Template;
            public readonly int SortOrder;

            public TemplatesGridViewItem(VM template)
            {
                Template = template;
                var typ = template.TemplateType();
                SortOrder = (int)typ;
                if (template.IsHidden())
                    SortOrder += (int)VM.VmTemplateType.Count;

                var imageCell = new DataGridViewImageCell(false) {ValueType = typeof(Image), Value = typ.ToBitmap()};
                var typeCell = new DataGridViewTextBoxCell {Value = typ.ToDisplayString()};
                var nameCell = new DataGridViewTextBoxCell {Value = template.Name()};

                Cells.AddRange(imageCell, nameCell, typeCell);
            }

            public bool Equals(TemplatesGridViewItem other)
            {
                return Template.Equals(other?.Template);
            }
        }
    }

    internal static class VmTemplateTypeExtensions
    {
        public static string ToDisplayString(this VM.VmTemplateType templateType)
        {
            switch (templateType)
            {
                case VM.VmTemplateType.Windows:
                case VM.VmTemplateType.LegacyWindows:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_WINDOWS;
                case VM.VmTemplateType.Centos:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_CENTOS;
                case VM.VmTemplateType.Debian:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_DEBIAN;
                case VM.VmTemplateType.Gooroom:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_GOOROOM;
                case VM.VmTemplateType.Rocky:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_ROCKY;
                case VM.VmTemplateType.Linx:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_LINX; 
                case VM.VmTemplateType.Oracle:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_ORACLE;
                case VM.VmTemplateType.RedHat:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_REDHAT;
                case VM.VmTemplateType.SciLinux:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_SCILINUX;
                case VM.VmTemplateType.Suse:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_SUSE;
                case VM.VmTemplateType.Ubuntu:
                    return Messages.NEW_VM_WIZARD_TEMPLATEPAGE_UBUNTU;
                case VM.VmTemplateType.YinheKylin:
                    return Messages.NEW_VM_WIZARD_TEMPLATEPAGE_YINHEKYLIN;    
                case VM.VmTemplateType.NeoKylin:
                    return Messages.NEW_VM_WIZARD_TEMPLATEPAGE_NEOKYLIN;
                case VM.VmTemplateType.Asianux:
                    return Messages.NEW_VM_WIZARD_TEMPLATEPAGE_ASIANUX;
                case VM.VmTemplateType.Turbo:
                    return Messages.NEW_VM_WIZARD_TEMPLATEPAGE_TURBO;        
                case VM.VmTemplateType.Citrix:
                    return BrandManager.CompanyNameShort;
                case VM.VmTemplateType.Solaris:
                case VM.VmTemplateType.Misc:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_MISC;
                case VM.VmTemplateType.CoreOS:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_COREOS;
                case VM.VmTemplateType.Snapshot:
                case VM.VmTemplateType.SnapshotFromVmpp:
                default:
                    return Messages.NEWVMWIZARD_TEMPLATEPAGE_SNAPSHOTS;
            }
        }

        public static Bitmap ToBitmap(this VM.VmTemplateType templateType)
        {
            switch (templateType)
            {
                case VM.VmTemplateType.Custom:
                    return Images.StaticImages._000_UserTemplate_h32bit_16;
                case VM.VmTemplateType.Windows:
                case VM.VmTemplateType.LegacyWindows:
                    return Images.StaticImages.windows_h32bit_16;
                case VM.VmTemplateType.Centos:
                    return Images.StaticImages.centos_16x;
                case VM.VmTemplateType.Debian:
                    return Images.StaticImages.debian_16x;
                case VM.VmTemplateType.Gooroom:
                    return Images.StaticImages.gooroom_16x;
                case VM.VmTemplateType.Rocky:
                    return Images.StaticImages.rocky_16x;
                case VM.VmTemplateType.Linx:
                    return Images.StaticImages.linx_16x;     
                case VM.VmTemplateType.Oracle:
                    return Images.StaticImages.oracle_16x;
                case VM.VmTemplateType.RedHat:
                    return Images.StaticImages.redhat_16x;
                case VM.VmTemplateType.SciLinux:
                    return Images.StaticImages.scilinux_16x;
                case VM.VmTemplateType.Suse:
                    return Images.StaticImages.suse_16x;
                case VM.VmTemplateType.Ubuntu:
                    return Images.StaticImages.ubuntu_16x;
                case VM.VmTemplateType.YinheKylin:
                    return Images.StaticImages.yinhekylin_16x;
                case VM.VmTemplateType.NeoKylin:
                    return Images.StaticImages.neokylin_16x;
                case VM.VmTemplateType.Asianux:
                    return Images.StaticImages.asianux_16x;   
                case VM.VmTemplateType.Turbo:
                    return Images.StaticImages.turbo_16x;                      
                case VM.VmTemplateType.Citrix:
                    return Images.StaticImages.Logo;
                case VM.VmTemplateType.Solaris:
                case VM.VmTemplateType.Misc:
                    return Images.StaticImages._000_VMTemplate_h32bit_16;
                case VM.VmTemplateType.CoreOS:
                    return Images.StaticImages.coreos_globe_icon;
                case VM.VmTemplateType.Snapshot:
                case VM.VmTemplateType.SnapshotFromVmpp:
                default:
                    return Images.StaticImages._000_VMSession_h32bit_16;
                // Also modify 'case PropertyNames.os_name' in method GetImagesFor(PropertyNames p) in XenModel/XenSearch/Common.cs
            }
        }
    }
}
