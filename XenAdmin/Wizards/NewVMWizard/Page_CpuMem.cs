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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Controls.Ballooning;
using XenAdmin.Core;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_CpuMem : XenTabPage
    {
        private VM Template;
        int memoryMode = 1;  // number of spinners to show: 1 for just "memory", 2 for "minimum" and "maximum", 3 for "static max" too
        double memoryRatio = 0.0;  // the permitted ratio of dynamic_min / static_max
        bool initialising = true;

        public Page_CpuMem()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_CPUMEMPAGE_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_CPUMEMPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "CPU&Memory"; }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (SelectedTemplate ==  Template)
                return;

            initialising = true;

            Template = SelectedTemplate;
            if (Template.has_ballooning && Helpers.MidnightRideOrGreater(Template.Connection) && !Helpers.FeatureForbidden(Template, Host.RestrictDMC))
                memoryMode = (Template.memory_dynamic_max == Template.memory_static_max ? 2 : 3);
            else
                memoryMode = 1;

            memoryRatio = VMMemoryControlsEdit.GetMemoryRatio(Template);

            VcpuSpinner.Minimum = 1;
            VcpuSpinner.Maximum = (decimal)(Template.MaxVCPUsAllowed);
            VcpuSpinner.Value = (decimal)(Template.VCPUs_at_startup);

            FreeSpinnerLimits();

            if (memoryMode == 1)
            {
                spinnerDynMin.Initialize(Messages.MEMORY_COLON, null, Template.memory_static_max);
                spinnerDynMax.Visible = spinnerStatMax.Visible = false;
            }
            else
            {
                spinnerDynMax.Visible = true;
                spinnerDynMin.Initialize(Messages.DYNAMIC_MIN_COLON, null, Template.memory_dynamic_min);
                FreeSpinnerLimits();  // same as CA-33831
                spinnerDynMax.Initialize(Messages.DYNAMIC_MAX_COLON, null, Template.memory_dynamic_max);
                if (memoryMode == 3)
                {
                    FreeSpinnerLimits();
                    spinnerStatMax.Initialize(Messages.STATIC_MAX_COLON, null, Template.memory_static_max);
                }
                else
                    spinnerStatMax.Visible = false;
            }

            SetSpinnerLimits();

            VcpuSpinner.Select();
            ValuesUpdated();

            initialising = false;
        }

        public VM SelectedTemplate { private get; set; }

        // Return the larger of the template's MaxMemAllowed and the template's static max,
        // to avoid crashes in the spinners (CA-40041).
        private long MaxMemAllowed
        {
            get
            {
                long msm = Template.memory_static_max;
                long mma = Template.MaxMemAllowed;
                return (msm > mma ? msm : mma);
            }
        }

        private void FreeSpinnerLimits()
        {
            long maxMemAllowed = MaxMemAllowed;
            spinnerDynMin.SetRange(0, maxMemAllowed);
            spinnerDynMax.SetRange(0, maxMemAllowed);
            spinnerStatMax.SetRange(0, maxMemAllowed);
        }

        private void SetSpinnerLimits()
        {
            long maxMemAllowed = MaxMemAllowed;
            long min = Template.memory_static_min;
            if (memoryMode == 1)
            {
                spinnerDynMin.SetRange(min, maxMemAllowed);
                return;
            }
            long min2 = (long)((double)SelectedMemoryStaticMax * memoryRatio);
            if (min < min2)
                min = min2;
            long max = SelectedMemoryDynamicMax;
            if (max < min)
                max = min;
            spinnerDynMin.SetRange(min, max);
            spinnerDynMax.SetRange(SelectedMemoryDynamicMin,
                memoryMode == 2 ? maxMemAllowed : SelectedMemoryStaticMax);
            spinnerStatMax.SetRange(SelectedMemoryDynamicMax, maxMemAllowed);
        }

        public void DisableMemoryControls()
        {
            spinnerDynMin.Enabled = false;
            spinnerDynMax.Enabled = false;
            spinnerStatMax.Enabled = false;
        }

        public long SelectedMemoryDynamicMin
        {
            get
            {
                return spinnerDynMin.Value;
            }
        }

        public long SelectedMemoryDynamicMax
        {
            get
            {
                return memoryMode == 1 ? spinnerDynMin.Value : spinnerDynMax.Value;
            }
        }

        public long SelectedMemoryStaticMax
        {
            get
            {
                return
                    memoryMode == 1 ? spinnerDynMin.Value :
                    memoryMode == 2 ? spinnerDynMax.Value :
                    spinnerStatMax.Value;
            }
        }

        public long SelectedVcpus
        {
            get
            {
                return (long)VcpuSpinner.Value;
            }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();
                sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_VCPUS, SelectedVcpus.ToString()));
                if (memoryMode == 1)
                    sum.Add(new KeyValuePair<string, string>(Messages.MEMORY, Util.MemorySizeString(SelectedMemoryStaticMax)));
                else
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.DYNAMIC_MIN, Util.MemorySizeString(SelectedMemoryDynamicMin)));
                    sum.Add(new KeyValuePair<string, string>(Messages.DYNAMIC_MAX, Util.MemorySizeString(SelectedMemoryDynamicMax)));
                    if (memoryMode == 3)
                        sum.Add(new KeyValuePair<string, string>(Messages.STATIC_MAX, Util.MemorySizeString(SelectedMemoryStaticMax)));
                }
                return sum;
            }
        }

        private void ValuesUpdated()
        {
            CheckForError();
            OnPageUpdated();
        }

        private void CheckForError()
        {
            long max_mem_total = 0;
            long max_mem_free = 0;
            long max_vcpus = 0;
            Host max_mem_total_host = null;
            Host max_mem_free_host = null;
            Host max_vcpus_host = null;

            foreach (Host host in Connection.Cache.Hosts)
            {
                long host_cpus = 0;

                foreach (Host_cpu cpu in Connection.Cache.Host_cpus)
                {
                    if (cpu.host.opaque_ref.Equals(host.opaque_ref))
                        host_cpus++;
                }

                Host_metrics metrics = Connection.Resolve(host.metrics);

                if (host_cpus > max_vcpus)
                {
                    max_vcpus = host_cpus;
                    max_vcpus_host = host;
                }

                if (metrics != null && metrics.memory_total > max_mem_total)
                {
                    max_mem_total = metrics.memory_total;
                    max_mem_total_host = host;
                }

                // The available memory of a server is taken to be the current memory_free,
                // plus however much we can squeeze down the existing VMs. This assumes
                // that the overhead won't increase when we create the new VM, so it
                // has false negatives.
                long memory_free = host.memory_available_calc;

                if (metrics != null && memory_free > max_mem_free)
                {
                    max_mem_free = memory_free;
                    max_mem_free_host = host;
                }
            }

            if (max_mem_total_host != null && SelectedMemoryDynamicMin > max_mem_total)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYWARN1, Helpers.GetName(max_mem_total_host), Util.MemorySizeString(max_mem_total));
            }
            else if (max_mem_free_host != null && SelectedMemoryDynamicMin > max_mem_free)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYWARN2, Helpers.GetName(max_mem_free_host), Util.MemorySizeString(max_mem_free));
            }
            else if (max_vcpus_host != null && SelectedVcpus > max_vcpus)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_VCPUSWARN, Helpers.GetName(max_vcpus_host), max_vcpus);
            }
            else
            {
                ErrorPanel.Visible = false;
            }
        }

        private void vCPU_ValueChanged(object sender, EventArgs e)
        {
            ValuesUpdated();
        }

        private void memory_ValueChanged(object sender, EventArgs e)
        {
            if (initialising)
                return;

            SetSpinnerLimits();
            ValuesUpdated();
        }

        private void VcpuSpinner_Leave(object sender, EventArgs e)
        {
            if (sender is NumericUpDown)
                ((Control)sender).Text = ((NumericUpDown)sender).Value.ToString();
        }
    }
}
