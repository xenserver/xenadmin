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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using XenAdmin.Controls;
using XenAdmin.Wizards.GenericPages;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;


namespace XenAdmin.Wizards.ImportWizard
{
    class ImportSelectHostPage : SelectMultipleVMDestinationPage
    {
        public EnvelopeType SelectedOvfEnvelope { private get; set; }
        private List<Xen_ConfigurationSettingData_Type> vgpuSettings = new List<Xen_ConfigurationSettingData_Type>();

        #region XenTabPage overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle { get { return Messages.IMPORT_SELECT_HOST_PAGE_TITLE; } }

		/// <summary>
		/// Gets the page's label in the (left hand side) wizard progress panel
		/// </summary>
		public override string Text { get { return Messages.NEWSR_LOCATION; } }

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (direction == PageLoadedDirection.Forward)
            {
                vgpuSettings.Clear();
                ShowWarning(null);

                if (SelectedOvfEnvelope == null || VmMappings.Count < 1)
                    return;

                vgpuSettings = FindVgpuSettings(SelectedOvfEnvelope);
            }
        }

        #endregion

        protected override string InstructionText { get { return Messages.IMPORT_WIZARD_DESTINATION_INSTRUCTIONS; } }

        protected override string TargetServerText { get { return Messages.IMPORT_WIZARD_DESTINATION_DESTINATION; } }

        protected override string TargetServerSelectionIntroText { get { return Messages.IMPORT_WIZARD_DESTINATION_TABLE_INTRO; } }

        protected override void OnChosenItemChanged()
        {
            if (vgpuSettings.Count == 0 || ChosenItem ==null || CheckRightGpuExists())
            {
                ShowWarning(null);
                return;
            }

            ShowWarning(VmMappings.Count == 1
                            ? Messages.IMPORT_VM_WITH_VGPU_WARNING_ONE
                            : Messages.IMPORT_VM_WITH_VGPU_WARNING_MANY);
        }

        public override DelayLoadingOptionComboBoxItem CreateDelayLoadingOptionComboBoxItem(IXenObject xenItem)
        {
            return new ImportDelayLoadingOptionComboBoxItem(xenItem);
        }

        private List<Xen_ConfigurationSettingData_Type> FindVgpuSettings(EnvelopeType envelopeType)
        {
            var list = new List<Xen_ConfigurationSettingData_Type>();

            foreach (VirtualSystem_Type vsType in ((VirtualSystemCollection_Type)envelopeType.Item).Content)
            {
                VirtualHardwareSection_Type vhs = OVF.FindVirtualHardwareSectionByAffinity(envelopeType, vsType.id, "xen");
                var data = vhs.VirtualSystemOtherConfigurationData;
                if (data != null)
                    list.AddRange(vhs.VirtualSystemOtherConfigurationData.Where(s => s.Name == "vgpu"));
            }

            return list;
        }

        private bool CheckRightGpuExists()
        {
            foreach (var vgpuSetting in vgpuSettings)
            {
                Match m = XenOvfTransport.Import.VGPU_REGEX.Match(vgpuSetting.Value.Value);
                if (!m.Success)
                    continue;

                var types = m.Groups[1].Value.Split(';');

                var gpuGroup = ChosenItem.Connection.Cache.GPU_groups.FirstOrDefault(g =>
                    g.GPU_types.Length == types.Length &&
                    g.GPU_types.Intersect(types).Count() == types.Length);

                if (gpuGroup == null)
                    return false;

                string vendorName = m.Groups[2].Value;
                string modelName = m.Groups[3].Value;

                var vgpuType = ChosenItem.Connection.Cache.VGPU_types.FirstOrDefault(v =>
                    v.vendor_name == vendorName && v.model_name == modelName);

                if (vgpuType == null)
                    return false;
            }

            return true;
        }
    }
}
