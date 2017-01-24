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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class VDISizeLocationPage : UserControl, IEditPage
    {
        private VDI vdi;
        private readonly ToolTip InvalidParamToolTip;
        private bool sizeChanged = false;

        public VDISizeLocationPage()
        {
            InitializeComponent();

            Text = Messages.SIZE_AND_LOCATION;

            InvalidParamToolTip = new ToolTip();
            InvalidParamToolTip.IsBalloon = true;
            InvalidParamToolTip.ToolTipIcon = ToolTipIcon.Warning;
            InvalidParamToolTip.ToolTipTitle = Messages.INVALID_PARAMETER;

            sizeNUD.TextChanged += sizeNUD_ValueChanged;
        }

        public String SubText
        {
            get
            {
                return String.Format(Messages.SIZE_LOCATION_SUB,
                    sizeNUD.Visible ? Util.DiskSizeString((long)(sizeNUD.Value * 1073741824),2) : Util.DiskSizeString(vdi.virtual_size,2),
                    vdi.Connection.Resolve<SR>(vdi.SR));
            }
        }

        public Image Image
        {
            get
            {
                return Properties.Resources._000_VirtualStorage_h32bit_16;
            }
        }

        public bool ValidToSave
        {
            get { return IsDiskSizeValid; }
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            if (!(clone is VDI))
                return;
            vdi = clone as VDI;

            Repopulate();
        }

        public void Repopulate()
        {
            if(vdi == null)
                return;
            SR sr = vdi.Connection.Resolve<SR>(vdi.SR);
            labelLocationValueRO.Text = string.Format("'{0}'", sr.NameWithoutHost);

            initial_alloc_value.Visible = incr_alloc_value.Visible 
                                        = initial_allocation_label.Visible 
                                        = incremental_allocation_label.Visible =  sr.IsThinProvisioned;
            
            if(sr.IsThinProvisioned && vdi.sm_config.ContainsKey("initial_allocation") && vdi.sm_config.ContainsKey("allocation_quantum"))
            {
                initial_alloc_value.Text = Util.MemorySizeStringSuitableUnits(Convert.ToDouble(vdi.sm_config["initial_allocation"]), true, Messages.VAL_MB);
                incr_alloc_value.Text = Util.MemorySizeStringSuitableUnits(Convert.ToDouble(vdi.sm_config["allocation_quantum"]), true, Messages.VAL_MB);
            }

            if (vdi.allowed_operations.Contains(vdi_operations.resize) ||
                vdi.allowed_operations.Contains(vdi_operations.resize_online))
            {
                sizeNUD.Value = ((ulong)vdi.virtual_size) / (decimal)Util.BINARY_GIGA;
                sizeNUD.Minimum = sizeNUD.Value;
                sizeNUD.Visible = true;
                panelShutDownHint.Visible = false;
                sizeValueROLabel.Text = "";
                sizeValueROLabel.Visible = false;
            }
            else
            {
                sizeNUD.Visible = false;
                sizeValueROLabel.Text = Util.DiskSizeString(vdi.virtual_size);
                sizeValueROLabel.Visible = true;

                sizeValue.Visible = false;
                panelShutDownHint.Visible = true;
            }

            

            AnyTextChanged(null, null);
        }

        public bool HasChanged
        {
            get
            {
                return sizeNUD.Visible && !CloseEnough(sizeNUD.Value, ((decimal) vdi.virtual_size) / Util.BINARY_GIGA);
            }
        }

        /// <summary>
        /// Make sure sizes are within 10MB
        /// </summary>
        /// <param name="value1">value in GB</param>
        /// <param name="value2">value in GB</param>
        /// <returns></returns>
        private bool CloseEnough(decimal value1, decimal value2)
        {
            return (value1 - value2) < (decimal) 0.01;
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
            sizeNUD.TextChanged -= sizeNUD_ValueChanged;
        }

        public AsyncAction SaveSettings()
        {
            decimal newSize = sizeNUD.Value;

            if (!sizeChanged || CloseEnough(newSize, ((decimal)vdi.virtual_size) / Util.BINARY_GIGA))
                return null;

            if (vdi.allowed_operations.Contains(vdi_operations.resize))
                return new DelegatedAsyncAction(
                    vdi.Connection,
                    Messages.ACTION_CHANGE_DISK_SIZE,
                    string.Format(Messages.ACTION_CHANGING_DISK_SIZE_FOR, vdi),
                    string.Format(Messages.ACTION_CHANGED_DISK_SIZE_FOR, vdi),
                    delegate(Session session) { VDI.resize(session, vdi.opaque_ref, (long)(newSize * Util.BINARY_GIGA)); },
                    true,
                    "vdi.resize"
                );
            else
                return new DelegatedAsyncAction(
                    vdi.Connection,
                    Messages.ACTION_CHANGE_DISK_SIZE,
                    string.Format(Messages.ACTION_CHANGING_DISK_SIZE_FOR, vdi),
                    string.Format(Messages.ACTION_CHANGED_DISK_SIZE_FOR, vdi),
                    delegate(Session session) { VDI.resize_online(session, vdi.opaque_ref, (long)(newSize * Util.BINARY_GIGA)); },
                    true,
                    "vdi.resize_online"
                );
        }

        private void AnyTextChanged(object sender, EventArgs e)
        {
        }

        private enum DiskSizeValidationResult { InvalidNumber, NotEnoughSpace, Valid }

        private void sizeNUD_ValueChanged(object sender, EventArgs e)
        {
            sizeChanged = true;

            DiskSizeValidationResult result = ValidateDiskSize();
            pictureBoxError.Visible = labelError.Visible = result != DiskSizeValidationResult.Valid;
            if (result == DiskSizeValidationResult.InvalidNumber)
                labelError.Text = Messages.INVALID_NUMBER;
            else if (result == DiskSizeValidationResult.NotEnoughSpace)
                labelError.Text = Messages.DISK_TOO_BIG;
        }

        private DiskSizeValidationResult ValidateDiskSize()
        {
            if (sizeNUD.Text.Trim() == string.Empty)
                return DiskSizeValidationResult.Valid;

            decimal diskSize;
            if (decimal.TryParse(sizeNUD.Text.Trim(), out diskSize))
            {
                long reqSize;
                try
                {
                    reqSize = (long)(Math.Round(diskSize) * Util.BINARY_GIGA);
                }
                catch (OverflowException)
                {
                    reqSize = diskSize < 0 ? long.MinValue : long.MaxValue;
                }
                long freeSpace = vdi.Connection.Resolve<SR>(vdi.SR).FreeSpace;
                return reqSize - vdi.virtual_size > freeSpace
                           ? DiskSizeValidationResult.NotEnoughSpace
                           : DiskSizeValidationResult.Valid;
            }
            return DiskSizeValidationResult.InvalidNumber;
        }

        private bool IsDiskSizeValid
        {
            get { return ValidateDiskSize() == DiskSizeValidationResult.Valid; }
        }
    }
}
