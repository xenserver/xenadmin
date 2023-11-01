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
using System.Linq;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Commands;
using XenAdmin.Core;


namespace XenAdmin.Controls.Ballooning
{
    public partial class VMMemoryControlsBasic : VMMemoryControlsEdit
    {
        public event Action InstallTools;

        public VMMemoryControlsBasic()
        {
            InitializeComponent();
            linkInstallTools.Text = string.Format(linkInstallTools.Text, BrandManager.VmTools);
        }
        
        protected override void Populate()
        {
            if (vms == null || vms.Count == 0)
                return;

            pictureBoxDynMin.Visible = hasBallooning;
            pictureBoxDynMax.Visible = hasBallooning;

            // Calculate the maximum legal value of dynamic minimum
            CalcMaxDynMin();

            vmShinyBar.Populate(vms, true);

            bool licenseRestriction = Helpers.FeatureForbidden(vm0.Connection, Host.RestrictDMC);

            // Radio buttons and "DMC Unavailable" warning
            if (hasBallooning && !licenseRestriction)
            {
                if (vm0.memory_dynamic_min == vm0.memory_static_max)
                    radioFixed.Checked = true;
                else
                    radioDynamic.Checked = true;
                iconDMCUnavailable.Visible = labelDMCUnavailable.Visible = linkInstallTools.Visible = false;
            }
            else
            {
                radioFixed.Checked = true;
                radioDynamic.Enabled = false;
                groupBoxOn.Enabled = false;

                if (licenseRestriction)
                {
                    labelDMCUnavailable.Text = Messages.DMC_UNAVAILABLE_LICENSE_RESTRICTION;
                    linkInstallTools.Visible = false;
                }
                else if (vms.Count > 1)
                {
                    // If all the Virtualisation Statuses are the same, report that reason.
                    // Otherwise give a generic message.
                    VM.VirtualizationStatus vs0 = vm0.GetVirtualizationStatus(out _);
                    bool identical = true;
                    foreach (VM vm in vms)
                    {
                        if (vm.GetVirtualizationStatus(out _) != vs0)
                        {
                            identical = false;
                            break;
                        }
                    }
                    if (identical)
                    {
                        var status = vm0.GetVirtualizationStatus(out _);
                        if (status.HasFlag(VM.VirtualizationStatus.IoDriversInstalled))
                            labelDMCUnavailable.Text = Messages.DMC_UNAVAILABLE_NOTSUPPORTED_PLURAL;
                        else if (!status.HasFlag(VM.VirtualizationStatus.IoDriversInstalled))
                            labelDMCUnavailable.Text = vm0.HasNewVirtualizationStates()
                                ? Messages.DMC_UNAVAILABLE_NO_IO_NO_MGMNT_PLURAL
                                : string.Format(Messages.DMC_UNAVAILABLE_NOTOOLS_PLURAL, BrandManager.VmTools);
                        else if (status.HasFlag(VM.VirtualizationStatus.PvDriversOutOfDate))
                            labelDMCUnavailable.Text = string.Format(Messages.DMC_UNAVAILABLE_OLDTOOLS_PLURAL, BrandManager.VmTools);
                        else
                            labelDMCUnavailable.Text = Messages.DMC_UNAVAILABLE_VMS;
                    }
                    else
                        labelDMCUnavailable.Text = Messages.DMC_UNAVAILABLE_VMS;

                    linkInstallTools.Visible = vms.All(InstallToolsCommand.CanRun);
                }
                else if (vm0.is_a_template)
                {
                    labelDMCUnavailable.Text = Messages.DMC_UNAVAILABLE_TEMPLATE;
                    linkInstallTools.Visible = false;
                }
                else
                {
                    var status = vm0.GetVirtualizationStatus(out _);

                    if (status.HasFlag(VM.VirtualizationStatus.IoDriversInstalled))
                            labelDMCUnavailable.Text = Messages.DMC_UNAVAILABLE_NOTSUPPORTED;
                    else if (!status.HasFlag(VM.VirtualizationStatus.IoDriversInstalled))
                        labelDMCUnavailable.Text = vm0.HasNewVirtualizationStates()
                            ? Messages.DMC_UNAVAILABLE_NO_IO_NO_MGMNT
                            : string.Format(Messages.DMC_UNAVAILABLE_NOTOOLS, BrandManager.VmTools);
                    else if (status.HasFlag(VM.VirtualizationStatus.PvDriversOutOfDate))
                            labelDMCUnavailable.Text = string.Format(Messages.DMC_UNAVAILABLE_OLDTOOLS, BrandManager.VmTools);
                    else
                        labelDMCUnavailable.Text = Messages.DMC_UNAVAILABLE_VM;
                    
                    linkInstallTools.Visible = InstallToolsCommand.CanRun(vm0);
                }
            }

            if (linkInstallTools.Visible)
                linkInstallTools.Text = vms.All(v => Helpers.StockholmOrGreater(v.Connection))
                    ? string.Format(Messages.INSTALLTOOLS_READ_MORE, BrandManager.VmTools)
                    : string.Format(Messages.INSTALL_XENSERVER_TOOLS, BrandManager.VmTools);

            // Spinners
            FreeSpinnerRanges();
            memorySpinnerDynMin.Initialize(vm0.memory_dynamic_min, vm0.memory_static_max);
            memorySpinnerDynMax.Initialize(vm0.memory_dynamic_max, vm0.memory_static_max);
            memorySpinnerFixed.Initialize(vm0.memory_static_max, vm0.memory_static_max);
            SetIncrements();
            SetSpinnerRanges();
        }

        protected override double dynamic_min
        {
            get
            {
                System.Diagnostics.Trace.Assert(hasBallooning);
                return radioDynamic.Checked ? memorySpinnerDynMin.Value : memorySpinnerFixed.Value;
            }
        }

        protected override double dynamic_max
        {
            get
            {
                System.Diagnostics.Trace.Assert(hasBallooning);
                return radioDynamic.Checked ? memorySpinnerDynMax.Value : memorySpinnerFixed.Value;
            }
        }

        protected override double static_max => (radioDynamic.Checked ? memorySpinnerDynMax.Value : memorySpinnerFixed.Value);

        private void SetIncrements()
        {
            vmShinyBar.Increment = memorySpinnerDynMin.Increment = memorySpinnerDynMax.Increment = memorySpinnerFixed.Increment = CalcIncrement(static_max, memorySpinnerDynMax.Units);
        }

        private void DynamicSpinners_ValueChanged(object sender, EventArgs e)
        {
            radioDynamic.Checked = true;
            if (sender == memorySpinnerDynMax)
            {
                // Force supported envelope
                FreeSpinnerRanges();
                long min = (long)(static_max * GetMemoryRatio());
                if (memorySpinnerDynMin.Value < min)
                    memorySpinnerDynMin.Initialize(min, RoundingBehaviour.Up);
            }
            SetIncrements();
            SetSpinnerRanges();
            vmShinyBar.ChangeSettings(vm0.memory_static_min, dynamic_min, dynamic_max, static_max);
            vmShinyBar.Refresh();
        }

        private void FixedSpinner_ValueChanged(object sender, EventArgs e)
        {
            radioFixed.Checked = true;
            SetIncrements();
        }

        private void SetSpinnerRanges()
        {
            // Set the limit for the fixed spinner
            double maxFixed = ((maxDynMin >= 0 && maxDynMin <= MemorySpinnerMax) ? maxDynMin : MemorySpinnerMax);
            memorySpinnerFixed.SetRange(vm0.memory_static_min >= Util.BINARY_MEGA ? vm0.memory_static_min : Util.BINARY_MEGA, maxFixed);

            if (!hasBallooning)
                return;

            // Calculate limits for the dynamic spinners
            double maxDM = DynMinSpinnerMax;
            double minDM = DynMinSpinnerMin;
            double maxSM = StatMaxSpinnerMax;

            // Set the limits
            memorySpinnerDynMin.SetRange(minDM, maxDM);
            memorySpinnerDynMax.SetRange(dynamic_min >= Util.BINARY_MEGA ? dynamic_min : Util.BINARY_MEGA, maxSM);
            vmShinyBar.SetRanges(minDM, maxDM, dynamic_min, maxSM, memorySpinnerDynMax.Units);
        }

        private void FreeSpinnerRanges()
        {
            memorySpinnerDynMin.SetRange(0, MemorySpinnerMax);
            memorySpinnerDynMax.SetRange(0, MemorySpinnerMax);
            memorySpinnerFixed.SetRange(0, MemorySpinnerMax);
        }

        private void vmShinyBar_SliderDragged(object sender, EventArgs e)
        {
            memorySpinnerDynMin.Initialize(vmShinyBar.Dynamic_min, RoundingBehaviour.None);
            memorySpinnerDynMax.Initialize(vmShinyBar.Dynamic_max, RoundingBehaviour.None);
            memorySpinnerDynMin.Refresh();
            memorySpinnerDynMax.Refresh();
        }

        private void InstallTools_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (vms.All(v => Helpers.StockholmOrGreater(v.Connection)))
            {
                Help.HelpManager.Launch("InstallToolsWarningDialog");
                return;
            }

            var cmd = new InstallToolsCommand(Program.MainWindow, vms);
            cmd.InstallTools += _ => InstallTools?.Invoke();
            cmd.Run();
        }
    }
}
