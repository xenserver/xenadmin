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
using XenAdmin.Commands;
using XenAdmin.Controls.CustomGridView;
using XenAdmin.Controls.XenSearch;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAPI;

namespace XenAdmin.XenSearch
{
    public class ColumnAccessors
    {
        private static Dictionary<ColumnNames, Column> columns = new Dictionary<ColumnNames, Column>();

        static ColumnAccessors()
        {
            columns[ColumnNames.name] = new NameColumn();
            columns[ColumnNames.cpu] = new BarGraphColumn(PropertyNames.cpuText, PropertyNames.cpuValue);
            columns[ColumnNames.memory] = new BarGraphColumn(PropertyNames.memoryText, PropertyNames.memoryValue, PropertyNames.memoryRank, true);
            columns[ColumnNames.disks] = new PropertyColumn(PropertyNames.diskText, true);
            columns[ColumnNames.network] = new PropertyColumn(PropertyNames.networkText, true);
            columns[ColumnNames.ha] = new PropertyColumn(PropertyNames.haText);
            columns[ColumnNames.ip] = new PropertyColumn(PropertyNames.ip_address);
            columns[ColumnNames.uptime] = new PropertyColumn(PropertyNames.uptime);
        }

        public static Column Get(ColumnNames c)
        {
            return columns[c];
        }
    }

    public abstract class Column
    {
        protected readonly PropertyNames property;
        private readonly bool checkTools;

        protected Column(PropertyNames property, bool checkTools)
        {
            this.property = property;
            this.checkTools = checkTools;
        }

        public PropertyNames SortBy { get { return property; } }
        public abstract GridItemBase GetGridItem(IXenObject o);

        protected static GridItemBase NewStringItem(object line)
        {
            return new GridStringItem(line, HorizontalAlignment.Center, VerticalAlignment.Middle, false, false, QueryPanel.TextBrush, Program.DefaultFont);
        }

        protected bool CheckVMTools(IXenObject o, out GridItemBase item)
        {
            item = null;

            if (!checkTools)
                return false;

            VM vm = o as VM;
            if (vm != null)
            {
                VM.VirtualisationStatus status = vm.virtualisation_status;
                if (vm.power_state != vm_power_state.Running ||
                    status.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED ) ||
                    status.HasFlag(VM.VirtualisationStatus.UNKNOWN))
                    return false;

                if (property == PropertyNames.memoryValue)
                {
                    if (InstallToolsCommand.CanExecute(vm))
                    {
                        item = new GridStringItem(GetVMToolsInstallMessage(vm),
                                                  HorizontalAlignment.Center,
                                                  VerticalAlignment.Middle,
                                                  false,
                                                  false,
                                                  QueryPanel.LinkBrush,
                                                  Program.DefaultFontUnderline,
                                                  QueryPanel.LinkBrush,
                                                  Program.DefaultFontUnderline,
                                                  3,
                                                  (sender, args) => new InstallToolsCommand(Program.MainWindow, vm).Execute(), null);
                    }
                    else
                    {
                        item = new GridStringItem(GetVMToolsInstallMessage(vm),
                                                  HorizontalAlignment.Center,
                                                  VerticalAlignment.Middle,
                                                  false,
                                                  false,
                                                  QueryPanel.TextBrush,
                                                  Program.DefaultFont,
                                                  3);
                    }
                }
                return true;
            }

            SR sr = o as SR;
            if (sr != null && sr.NeedsUpgrading)
            {
                if (property == PropertyNames.memoryValue)
                    item = new GridStringItem(Messages.UPGRADE_SR_WARNING,
                                              HorizontalAlignment.Center, VerticalAlignment.Middle, false, false,
                                              QueryPanel.LinkBrush, Program.DefaultFontUnderline, QueryPanel.LinkBrush, Program.DefaultFontUnderline, 3,
                                              (sender, args) => new UpgradeSRCommand(Program.MainWindow, sr).Execute(), null);

                return true;
            }

            Pool pool = o as Pool;
            if (pool != null && !pool.IsPoolFullyUpgraded)
            {
                if (property == PropertyNames.memoryValue)
                {
                    var master = pool.Connection.Resolve(pool.master);

                    item = new GridStringItem(string.Format(Messages.POOL_VERSIONS_LINK_TEXT, master.ProductVersionText),
                                  HorizontalAlignment.Center, VerticalAlignment.Middle, false, false,
                                  QueryPanel.LinkBrush, Program.DefaultFontUnderline, QueryPanel.LinkBrush,
                                  Program.DefaultFontUnderline, 3,
                                  (sender, args) => new RollingUpgradeCommand(Program.MainWindow).Execute(),
                                  null);
                }

                return true;
            }

            return false;
        }

        public string GetVMToolsInstallMessage(VM vm)
        {
            VM.VirtualisationStatus status = vm.GetVirtualisationStatus;

            if (vm.virtualisation_status.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED) && vm.virtualisation_status.HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED)
                || vm.virtualisation_status.HasFlag(VM.VirtualisationStatus.UNKNOWN))
                // calling function shouldn't send us here if tools are, or might be, present: used to assert here but it can sometimes happen (CA-51460)
                return "";

            if (vm.virtualisation_status.HasFlag(VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE))
            {
                VM_guest_metrics guestMetrics = vm.Connection.Resolve(vm.guest_metrics);
                if (guestMetrics != null
                    && guestMetrics.PV_drivers_version.ContainsKey("major")
                    && guestMetrics.PV_drivers_version.ContainsKey("minor"))
                {
                    return String.Format(Messages.PV_DRIVERS_OUT_OF_DATE, String.Format("{0}.{1}",
                        guestMetrics.PV_drivers_version["major"],
                        guestMetrics.PV_drivers_version["minor"]));
                }
                else
                    return Messages.PV_DRIVERS_OUT_OF_DATE_UNKNOWN_VERSION;
            }

            return vm.HasNewVirtualisationStates ? Messages.VIRTUALIZATION_STATE_VM_MANAGEMENT_AGENT_NOT_INSTALLED : Messages.PV_DRIVERS_NOT_INSTALLED;
        }
    }

    public class NameColumn : Column
    {
        public NameColumn()
            : base(PropertyNames.label, false)
        {
        }

        public override GridItemBase GetGridItem(IXenObject o)
        {
            if (o is Folder)
                return new GridStringItem(new PropertyWrapper(PropertyNames.label, o), HorizontalAlignment.Left,
                                          VerticalAlignment.Middle, true, true, QueryPanel.TextBrush, Program.DefaultFont,
                                          QueryPanel.TextBrush, Program.DefaultFontUnderline,
                                          delegate { Program.MainWindow.DoSearch(Search.SearchForFolder(o.opaque_ref)); }, null);

            return new GridVerticalArrayItem(
                new GridItemBase[] {
                                       new GridStringItem(new PropertyWrapper(PropertyNames.label, o), HorizontalAlignment.Left, 
                                                          VerticalAlignment.Middle, o != null, true, QueryPanel.TextBrush, Program.DefaultFont,
                                                          QueryPanel.TextBrush, Program.DefaultFontUnderline,
                                                          // Next argument used to be "if (ObjectInTreeView(o)) ? delegate() {...} : null".
                                                          // I moved the ObjectInTreeView inside the delegate because it's O(#nodes in tree)
                                                          // and so that we only have to check it at click time, not for each item put into
                                                          // the search results. (We no longer need to know at mouse-over time). See CA-29607.
                                                          delegate{ClickHandler(o); }, 
                                                          delegate(object gridItem, EventArgs e) {ContextMenuHandler((GridItemBase)gridItem, o);}),
                                       new GridStringItem(new PropertyWrapper(PropertyNames.description, o), HorizontalAlignment.Left, 
                                                          VerticalAlignment.Middle, false, false, QueryPanel.DarkGreyBrush, Program.DefaultFont)
                                   }, false);
        }

        private void ContextMenuHandler(GridItemBase item, IXenObject o)
        {
            List<SelectedItem> selection = new List<SelectedItem>();

            foreach (GridRow row in item.Row.GridView.RowsAndChildren)
            {
                if (row.Selected && row.Tag is IXenObject)
                {
                    selection.Add(new SelectedItem((IXenObject)row.Tag));
                }
            }

            if (selection.Count > 0)
            {
                ContextMenuStrip c = new ContextMenuStrip();
                c.Items.AddRange(Program.MainWindow.ContextMenuBuilder.Build(selection));
                c.Show(Form.MousePosition);
            }
        }

        private void ClickHandler(IXenObject o)
        {
            if (Program.MainWindow.SelectObject(o) && Program.MainWindow.TheTabControl.TabPages.Contains(Program.MainWindow.TabPageGeneral))
            {
                Program.MainWindow.SwitchToTab(MainWindow.Tab.Settings);
            }
        }
    }

    public class PropertyColumn : Column
    {
        public PropertyColumn(PropertyNames property)
            : this(property, false)
        {
        }

        public PropertyColumn(PropertyNames property, bool checkTools)
            : base(property, checkTools)
        {
        }

        public override GridItemBase GetGridItem(IXenObject o)
        {
            if (o is Folder)
                return NewStringItem(String.Empty);  // CA-28300.5

            GridItemBase item;
            if (CheckVMTools(o, out item))
                return item;

            return NewStringItem(new PropertyWrapper(property, o));
        }
    }

    public class BarGraphColumn : Column
    {
        private readonly PropertyNames textProperty;
        private readonly PropertyNames rankProperty;

        public BarGraphColumn(PropertyNames textProperty, PropertyNames intProperty)
            : this(textProperty, intProperty, false)
        {
        }

        public BarGraphColumn(PropertyNames textProperty, PropertyNames intProperty, bool checkTools)
            : this(textProperty, intProperty, intProperty, false)
        {
        }

        public BarGraphColumn(PropertyNames textProperty, PropertyNames valueProperty, PropertyNames rankProperty, bool checkTools)
            : base(valueProperty, checkTools)
        {
            this.textProperty = textProperty;
            this.rankProperty = rankProperty;
        }

        public override GridItemBase GetGridItem(IXenObject o)
        {
            if (o is Folder)
                return NewStringItem(String.Empty);  // CA-28300.5

            GridItemBase item;
            if (CheckVMTools(o, out item))
                return item;

            if (PropertyAccessors.Get(property)(o) == null)
                return NewStringItem(new PropertyWrapper(textProperty, o));

            return NewBarItem(
                new PropertyWrapper(textProperty, o),
                new ImageDelegate(delegate()
                {
                    int? i = (int?)PropertyAccessors.Get(rankProperty)(o);
                    if (i == null)
                        return null;

                    return HelpersGUI.GetProgressImage(i.Value);
                }));
        }

        private static GridItemBase NewBarItem(object line, ImageDelegate image)
        {
            return new GridVerticalArrayItem(
                new GridItemBase[] {
                                       new GridImageItem(line, image, HorizontalAlignment.Center, VerticalAlignment.Middle, false),
                                       new GridStringItem(line, HorizontalAlignment.Center, VerticalAlignment.Middle, false, false, QueryPanel.TextBrush, Program.DefaultFont)
                                   }, false);
        }
    }
}