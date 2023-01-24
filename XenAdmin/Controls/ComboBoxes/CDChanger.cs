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
using System.ComponentModel;
using XenAPI;
using XenAdmin.Actions;
using XenCenterLib;

namespace XenAdmin.Controls
{
    public class CDChanger : ISODropDownBox
    {
        private VBD cdrom;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VBD Drive
        {
            get => cdrom;
            set
            {
                if (cdrom != null)
                    cdrom.PropertyChanged -= cdrom_PropertyChanged;

                cdrom = value;

                if (cdrom != null)
                    cdrom.PropertyChanged += cdrom_PropertyChanged;

                SelectedCD = cdrom == null || cdrom.empty || cdrom.VDI == null
                    ? null
                    : connection.Resolve(cdrom.VDI);
            }
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            // let the base class take care of skipping the SR headings (CA-40779)
            base.OnSelectionChangeCommitted(e); 

            ToStringWrapper<VDI> vdiNameWrapper = SelectedItem as ToStringWrapper<VDI>;
            if (vdiNameWrapper == null)
                return;

            // don't change the cdrom if we go from <empty> to <empty>
            if (vdiNameWrapper.item == null && cdrom != null && Helper.IsNullOrEmptyOpaqueRef(cdrom.VDI.opaque_ref))
                return;

            // don't change the cdrom if we leave the same one in
            if (vdiNameWrapper.item != null && cdrom != null && cdrom.VDI.opaque_ref == vdiNameWrapper.item.opaque_ref)
                return;

            if (cdrom == null)
                return;

            ChangeCD(vdiNameWrapper.item);
        }

        public void ChangeCD(VDI vdi)
        {
            changing = true;
            Enabled = false;

            ChangeVMISOAction action = new ChangeVMISOAction(connection, vm, vdi, cdrom);

            action.Completed += delegate
            {
                Program.Invoke(this, () =>
                {
                    changing = false;
                    SelectedCD = cdrom == null || cdrom.empty || cdrom.VDI == null
                        ? null
                        : connection.Resolve(cdrom.VDI);
                    Enabled = true;
                });
            };

            action.RunAsync();
        }

        internal override void DeregisterEvents()
        {
            if (cdrom != null)
                cdrom.PropertyChanged -= cdrom_PropertyChanged;

            base.DeregisterEvents();
        }

        private void cdrom_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "empty" || e.PropertyName == "vdi") && !changing)
            {
                SelectCD(SelectedCD);
            }
        }
    }
}
