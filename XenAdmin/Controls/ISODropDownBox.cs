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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;

using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using System.Runtime.InteropServices;


namespace XenAdmin.Controls
{
    public partial class ISODropDownBox : LongStringComboBox
    {
        public VM vm;
        protected VBD cdrom;
        protected bool refreshOnClose = false;
        protected bool changing = false;
        private IXenConnection _connection;
        protected bool physicalOnly = false;
        protected bool isoOnly = false;
        protected bool empty = false;
        private bool _skipDown = false;
        private VDI selectedCD;

        public VDI SelectedCD
        {
            get { return SelectedItem == null ? null : (SelectedItem as ToStringWrapper<VDI>).item; }
            set { selectedCD = value; }
        }
        public bool noTools = false;

        public ISODropDownBox()
        {
            SR_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(SR_CollectionChanged);
            InitializeComponent();
        }

        public void RefreshSRs_()
        {
            BeginUpdate();
            try
            {
                Items.Clear();
                RefreshSRs();
            }
            finally
            {
                EndUpdate();
            }
        }

        private static string srToString(SR sr)
        {
            return sr.Name;
        }

        public virtual void RefreshSRs()
        {
            Program.AssertOnEventThread();

            if (Empty)
                Items.Add(new ToStringWrapper<VDI>(null, Messages.EMPTY)); //Create a special VDIWrapper for the empty dropdown item

            if (connection == null)
                return;

            List<ToStringWrapper<SR>> items = new List<ToStringWrapper<SR>>();
            foreach (SR sr in connection.Cache.SRs)
            {
                if (sr.content_type != SR.Content_Type_ISO)
                    continue;

                if (physicalOnly && !sr.Physical)
                    continue;

                if (isoOnly && (sr.Physical || (noTools && sr.IsToolsSR)))
                    continue;

                if (vm == null && sr.IsBroken())
                    continue;

                if (vm != null)
                {                    
                    if (vm.power_state == vm_power_state.Halted)
                    {
                        Host storageHost = vm.GetStorageHost(true);
                        // The storage host is the host that the VM is bound to because the VM is using local storage on that host.
                        // It will be null if there is no such host (i.e. the VM is not restricted host-wise by storage). 
                        if (storageHost != null && !sr.CanBeSeenFrom(storageHost))
                        {
                            // The storage host was not null, and this SR can't be seen from that host: don't show the SR.
                            continue;
                        }
                    }
                    else
                    {
                        // If VM is running, only show SRs on its current host
                        Host runningOn = vm.Connection.Resolve(vm.resident_on);
                        if (!sr.CanBeSeenFrom(runningOn))
                        {
                            continue;
                        }
                    }
                }

                items.Add(new ToStringWrapper<SR>(sr, srToString));
            }

            if (items.Count > 0)
            {
                items.Sort();
                foreach (ToStringWrapper<SR> srWrapper in items)
                {
                    AddSR(srWrapper);
                }
            }
        }

        public virtual void SelectCD()
        {
            if (selectedCD == null)
            {
                if (Items.Count > 0)
                    this.SelectedIndex = 0;
                else
                    this.SelectedIndex = -1;

                return;
            }

            foreach (Object o in Items)
            {
                ToStringWrapper<VDI> vdiNameWrapper = o as ToStringWrapper<VDI>;

                if (vdiNameWrapper == null)
                    continue;

                XenAPI.VDI iso = vdiNameWrapper.item;
                if (iso == null || !iso.Show(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                    continue;

                if (iso == selectedCD)
                {
                    this.SelectedItem = o;
                    break;
                }
            }
        }

        // CA-12115
        public bool HasSelectableItems
        {
            get
            {
                foreach (Object o in Items)
                {
                    if (!(o is ToStringWrapper<SR>))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        // TODO: this means only physical. refactor to mean display physical
        public bool Physical
        {
            get
            {
                return physicalOnly;
            }
            set
            {
                physicalOnly = value;
            }
        }

        // TODO: this means only iso. refactor to mean display iso
        public bool ISO
        {
            get
            {
                return isoOnly;
            }
            set
            {
                isoOnly = value;
            }
        }

        public bool Empty
        {
            get
            {
                return empty;
            }
            set
            {
                empty = value;
            }
        }

        protected void AddSR(ToStringWrapper<SR> srWrapper)
        {
            Items.Add(srWrapper);

            List<ToStringWrapper<VDI>> items = new List<ToStringWrapper<VDI>>();
            if (srWrapper.item.Physical)
            {
                List<ToStringWrapper<VDI>> vdis = new List<ToStringWrapper<VDI>>();
                foreach (VDI vdi in connection.ResolveAll<VDI>(srWrapper.item.VDIs))
                {
                    ToStringWrapper<VDI> vdiWrapper = new ToStringWrapper<VDI>(vdi, vdi.Name);
                    vdis.Add(vdiWrapper);
                }
                vdis.Sort(new Comparison<ToStringWrapper<VDI>>(delegate(ToStringWrapper<VDI> object1, ToStringWrapper<VDI> object2)
                {
                    return Core.StringUtility.NaturalCompare(object1.item.Name, object2.item.Name);
                }));

                Host host = srWrapper.item.GetStorageHost();
                if (host != null)
                {
                    for (int i = 0; i < vdis.Count; i++)
                    {
                        items.Add(new ToStringWrapper<VDI>(vdis[i].item, "    " + string.Format(Messages.ISOCOMBOBOX_CD_DRIVE, i, host.Name)));
                    }
                }
            }
            else
            {
                if (srWrapper.item.IsToolsSR)
                {
                    foreach (VDI vdi in connection.ResolveAll<VDI>(srWrapper.item.VDIs))
                    {
                        if(Actions.InstallPVToolsAction.ISONameOld.Equals(vdi.name_label) ||
                            Actions.InstallPVToolsAction.ISONameNew.Equals(vdi.name_label))
                            items.Add(new ToStringWrapper<VDI>(vdi, "    " + vdi.Name));
                    }
                }
                else
                {
                    foreach (VDI vdi in connection.ResolveAll<VDI>(srWrapper.item.VDIs))
                    {
                        items.Add(new ToStringWrapper<VDI>(vdi, "    " + vdi.Name));
                    }
                    items.Sort(new Comparison<ToStringWrapper<VDI>>(delegate(ToStringWrapper<VDI> object1, ToStringWrapper<VDI> object2)
                    {
                        return Core.StringUtility.NaturalCompare(object1.item.Name, object2.item.Name);
                    }));
                }
            }

            foreach (ToStringWrapper<VDI> vdiWrapper in items)
            {
                Items.Add(vdiWrapper);
            }
        }

        public int ISOCount
        {
            get
            {
                int i = 0;
                foreach (object o in Items)
                {
                    if (o is ToStringWrapper<VDI>)
                        i++;
                }
                return i;
            }
        }

        public IXenConnection connection
        {
            set
            {
                if (connection != null)
                {
                    DeregisterEvents();
                }
                _connection = value;
                if (connection != null)
                {
                    RegisterEvents();
                    refreshAll();
                }
            }
            get
            {
                if (vm != null)
                {
                    return vm.Connection;
                }
                else
                {
                    return _connection;
                }
            }
        }

        protected virtual void DeregisterEvents()
        {
            if (connection == null)
                return;

            // deregister collection listener
            connection.Cache.DeregisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);
            // Remove SR listeners
            foreach (SR sr in connection.Cache.SRs)
            {
                sr.PropertyChanged -= sr_PropertyChanged;
                foreach (PBD pbd in connection.Cache.PBDs)
                {
                    pbd.PropertyChanged -= pbd_PropertyChanged;
                }
            }
        }

        protected void RegisterEvents()
        {
            if (connection == null)
                return;
            
            // register collection listener
            connection.Cache.RegisterCollectionChanged<SR>(SR_CollectionChangedWithInvoke);

            // Add SR listeners
            foreach (SR sr in connection.Cache.SRs)
            {
                sr.PropertyChanged -= sr_PropertyChanged;
                sr.PropertyChanged += sr_PropertyChanged;
                foreach (PBD pbd in connection.Cache.PBDs)
                {
                    pbd.PropertyChanged -= pbd_PropertyChanged;
                    pbd.PropertyChanged += pbd_PropertyChanged;
                }
            }
        }

        private readonly CollectionChangeEventHandler SR_CollectionChangedWithInvoke = null;
        protected void SR_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            if (vm == null || e.Action == CollectionChangeAction.Refresh)
                return; 

            foreach (SR sr in connection.Cache.SRs)
            {
                sr.PropertyChanged -= sr_PropertyChanged;
                sr.PropertyChanged += sr_PropertyChanged;
            }

            Program.Invoke(this, refreshAll);
        }

        public virtual void refreshAll()
        {
            if (!DroppedDown)
            {
                RefreshSRs_();
                SelectCD();
                refreshOnClose = false;
            }
            else
            {
                refreshOnClose = true;
            }
        }

        private void sr_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VDIs" || e.PropertyName == "PBDs")
            {
                refreshAll();
            }
        }

        private void pbd_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "currently_attached")
            {
                refreshAll();
            }
        }

        protected void cdrom_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "empty" || e.PropertyName == "vdi") && !changing)
            {
                SelectCD();
            }
        }

        protected void vm_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VBDs" || e.PropertyName == "resident_on" || e.PropertyName == "affinity")
            {
                refreshAll();
            }
        }
        
        // if we have selected an SR then we automatically select the first ISO on that SR
        private void this_SelectedIndexChanged(object sender, EventArgs e)
        {
            skipSRs();
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            skipSRs();
            base.OnSelectionChangeCommitted(e);
            if (SelectedItem != null)
                selectedCD = (SelectedItem as ToStringWrapper<VDI>).item;
        }

        private void skipSRs()
        {
            int i = SelectedIndex;

            if (!HasSelectableItems || i == -1)
            {
                SelectedIndex = -1;
                return;
            }

            // Find the next selectable item in the appropriate (up/down) direction
            while (true)
            {
                if (i == 0)
                {
                    _skipDown = true;
                }
                else if (i == Items.Count - 1)
                {
                    _skipDown = false;
                }
                if (Items[i] is ToStringWrapper<SR>)
                {
                    i += _skipDown ? 1 : -1;
                }
                else
                {
                    _skipDown = true;
                    break;
                }
            }
            SelectedIndex = i;
        }

        // CA-12115
        // The up and down arrow keys cause the combobox to change selection to the next
        // or previous in the list. The page up and page down keys change the selection
        // by one page at a time as defined by the size of the dropdown list. We need to
        // make sure that when we arrow up or down that the item selection jumps over the
        // bold SR read-only group separator.  We will change the skip direction here.
        // Also the left and right arrows should navigate through list in the same way 
        // as the up and down arrows (CA-40779)
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.Left)
            {
                _skipDown = false;
            }
            else
            {
                _skipDown = true;
            }
            base.OnKeyDown(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {
                Object o = Items[e.Index];

                if (o is ToStringWrapper<SR>)
                {
                    e.DrawBackground();
                    Drawing.DrawText(e.Graphics, o.ToString(), Program.DefaultFontBold, e.Bounds, SystemColors.ControlText, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                else
                {
                    if ((e.State & DrawItemState.Selected) != 0)
                    {
                        e.DrawBackground();
                        Drawing.DrawText(e.Graphics, o.ToString(), Program.DefaultFont, e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    }
                    else if((e.State & DrawItemState.Disabled) != 0)
                    {
                        e.DrawBackground();
                        Drawing.DrawText(e.Graphics, o.ToString(), Program.DefaultFont, e.Bounds, SystemColors.GrayText, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    }
                    else
                    {
                        e.DrawBackground();
                        Drawing.DrawText(e.Graphics, o.ToString(), Program.DefaultFont, e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    }
                    e.DrawFocusRectangle();
                }
            }

            base.OnDrawItem(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            skipSRs();

            base.OnDropDownClosed(e);

            if (refreshOnClose)
                refreshAll();
        }

        // we need to prevent the mouse click occuring when the user clicks on certain objects
        // the combo box creates another window for the dropdown list so we cannot use it's wndproc
        // when this other window is created at run time we get told the window handle (LParam on WM_PARENTNOTIFY)
        // so we then replace this window's wndproc with our own (ReplacementWndProc)

        private IntPtr oldWndProc = IntPtr.Zero;
        private Win32.WndProcDelegate newWndProc;
        private IntPtr DropDownHandle = IntPtr.Zero;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case Win32.WM_PARENTNOTIFY:
                    DropDownHandle = m.LParam;
                    oldWndProc = Win32.GetWindowLong(DropDownHandle, Win32.GWL_WNDPROC);
                    newWndProc = new Win32.WndProcDelegate(ReplacementWndProc);
                    Win32.SetWindowLong(DropDownHandle, Win32.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));
                    break;
            }

            base.WndProc(ref m);
        }

        private IntPtr ReplacementWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (uint)Win32.WM_LBUTTONDOWN || msg == (uint)Win32.WM_LBUTTONDBLCLK)
            {
                Win32.POINT loc = new Win32.POINT();
                loc.X = MousePosition.X;
                loc.Y = MousePosition.Y;
                Win32.ScreenToClient(DropDownHandle, ref loc);
                Win32.RECT dropdown_rect = new Win32.RECT();
                Win32.GetClientRect(DropDownHandle, out dropdown_rect);
                if (dropdown_rect.Left <= loc.X && loc.X < dropdown_rect.Right && dropdown_rect.Top <= loc.Y && loc.Y < dropdown_rect.Bottom)
                {
                    int index = (int)Win32.SendMessage(DropDownHandle, Win32.LB_ITEMFROMPOINT, IntPtr.Zero, (IntPtr)(loc.X + (loc.Y << 16)));
                    if (index >> 16 == 0)
                    {
                        Object o = Items[index];
                        if (o is ToStringWrapper<SR>)
                            return IntPtr.Zero;
                    }
                }
            }
            return Win32.CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
        }
    }
}
