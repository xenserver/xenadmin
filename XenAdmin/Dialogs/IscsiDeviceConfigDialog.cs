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
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class IscsiDeviceConfigDialog : XenDialogBase
    {
        private ISCSIPopulateLunsAction IscsiPopulateLunsAction;
        private ISCSIPopulateIQNsAction IscsiPopulateIqnsAction;
        private bool LunInUse = false;

        private readonly Dictionary<String, ISCSIInfo> LunMap = new Dictionary<String, ISCSIInfo>();

        private readonly ToolTip TargetIqnToolTip = new ToolTip();
        private readonly ToolTip TargetLunToolTip = new ToolTip();

        public IscsiDeviceConfigDialog()
        {
            InitializeComponent();
        }

        public IscsiDeviceConfigDialog(IXenConnection connection)
            : base(connection)
        {
            InitializeComponent();
            labelIscsiInvalidHost.Visible = false;

            // IQN's can be very long, so we will show the value as a mouse over tooltip.
            // Initialize the tooltip here.
            TargetIqnToolTip.Active = true;
            TargetIqnToolTip.AutomaticDelay = 0;
            TargetIqnToolTip.AutoPopDelay = 50000;
            TargetIqnToolTip.InitialDelay = 50;
            TargetIqnToolTip.ReshowDelay = 50;
            TargetIqnToolTip.ShowAlways = true;


            // Initialize LUN's tooltip here.
            TargetLunToolTip.Active = true;
            TargetLunToolTip.AutomaticDelay = 0;
            TargetLunToolTip.AutoPopDelay = 50000;
            TargetLunToolTip.InitialDelay = 50;
            TargetLunToolTip.ReshowDelay = 50;
            TargetLunToolTip.ShowAlways = true;
        }

        private void textBoxIscsiHost_TextChanged(object sender, EventArgs e)
        {
            labelIscsiInvalidHost.Visible = false;
            IScsiParams_TextChanged(null, null);
        }
        
        private void IscsiUseChapCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            groupBoxChap.Enabled = IscsiUseChapCheckBox.Checked;
            foreach (Control c in groupBoxChap.Controls)
            {
                c.Enabled = IscsiUseChapCheckBox.Checked;
            }
            lunInUseLabel.Text = "";
            ChapSettings_Changed(null, null);
        }

        /// <summary>
        /// Called when any of the iSCSI filer params change: resets the IQNs/LUNs.
        /// Must be called on the event thread.
        /// </summary>
        private void IScsiParams_TextChanged(object sender, EventArgs e)
        {
            Program.AssertOnEventThread();

            // User has changed filer hostname/username/password - clear IQN/LUN boxes
            comboBoxIscsiIqns.Items.Clear();
            comboBoxIscsiIqns.Enabled = false;

            // Cancel pending IQN/LUN scans
            if (IscsiPopulateIqnsAction != null)
            {
                IscsiPopulateIqnsAction.Cancel();
            }

            ChapSettings_Changed(null, null);
        }

        private void ChapSettings_Changed(object sender, EventArgs e)
        {
            comboBoxIscsiLuns.Items.Clear();
            comboBoxIscsiLuns.Text = "";
            comboBoxIscsiLuns.Enabled = false;

            if (IscsiPopulateLunsAction != null)
            {
                IscsiPopulateLunsAction.Cancel();
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            UInt16 i;
            bool portValid = UInt16.TryParse(textBoxIscsiPort.Text, out i);

            buttonIscsiPopulateIQNs.Enabled =
                !String.IsNullOrEmpty(getIscsiHost())
                && portValid;

            buttonIscsiPopulateLUNs.Enabled =
                !String.IsNullOrEmpty(getIscsiHost())
                && !String.IsNullOrEmpty(getIscsiIQN())
                && portValid;

            buttonOk.Enabled = !String.IsNullOrEmpty(getIscsiLUN()) &&  !LunInUse;
        }

        private String getIscsiHost()
        {
            // If the user has selected an IQN, use the host from that IQN (due to multi-homing,
            // this may differ from the host they first entered). Otherwise use the host
            // they first entered,
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (wrapper != null)
                return wrapper.item.IpAddress;
            return textBoxIscsiHost.Text.Trim();
        }

        private UInt16 getIscsiPort()
        {
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (wrapper != null)
                return wrapper.item.Port;

            // No combobox item was selected
            UInt16 port;
            if (UInt16.TryParse(textBoxIscsiPort.Text, out port))
            {
                return port;
            }
            return Util.DEFAULT_ISCSI_PORT;
        }

        private String getIscsiIQN()
        {
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (wrapper == null)
                return "";
            return wrapper.item.TargetIQN;
        }

        private String getIscsiLUN()
        {
            return comboBoxIscsiLuns.Text;
        }

        private void comboBoxIscsiIqns_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            // Keep the IScsiTargetIqnComboBox tooltip in sync with the selected item
            if (wrapper != null)
            {
                TargetIqnToolTip.SetToolTip(comboBoxIscsiIqns, wrapper.ToString());
            }
            else
            {
                TargetIqnToolTip.SetToolTip(comboBoxIscsiIqns, "");
            }
            // Clear the LUN map and ComboBox because the user has changed the IQN.  The user
            // must re-discover the LUNs for the new IQN.
            ClearLunMapAndCombo();
        }

        private void comboBoxIscsiLuns_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedLun = comboBoxIscsiLuns.SelectedItem as string;
            // Keep the tooltip in sync with the selected item
            if (selectedLun != null)
            {
                TargetLunToolTip.SetToolTip(comboBoxIscsiLuns, selectedLun);
            }
            else
            {
                TargetLunToolTip.SetToolTip(comboBoxIscsiLuns, "");
            }

            try
            {
                SR sr = UniquenessCheck(ConnectionsManager.XenConnectionsCopy);

                // LUN is not in use if sr != null
                LunInUse = sr != null;

                if (sr == null)
                {
                    lunInUseLabel.Text = "";
                    return;
                }

                Pool pool = Helpers.GetPool(sr.Connection);
                if (pool != null)
                {
                    lunInUseLabel.Text = String.Format(Messages.NEWSR_LUN_IN_USE_ON_POOL,
                        sr.Name, pool.Name);
                    return;
                }

                Host master = Helpers.GetMaster(sr.Connection);
                if (master != null)
                {
                    lunInUseLabel.Text = String.Format(Messages.NEWSR_LUN_IN_USE_ON_SERVER,
                         sr.Name, master.Name);
                    return;
                }

                lunInUseLabel.Text = Messages.NEWSR_LUN_IN_USE;
            }
            finally
            {
                UpdateButtons();
            }
        }

        private void ClearLunMapAndCombo()
        {
            // Clear LUNs as they may no longer be valid
            comboBoxIscsiLuns.Items.Clear();
            comboBoxIscsiLuns.Text = "";
            comboBoxIscsiLuns.Enabled = false;
            LunMap.Clear();
            LunInUse = false;
            lunInUseLabel.Text = "";

            UpdateButtons();
        }

        /// <summary>
        /// Check the current config of the iSCSI sr in the wizard is unique across
        /// all active connections.
        /// </summary>
        /// <returns>SR that uses this config if not unique, null if unique</returns>
        private SR UniquenessCheck(List<IXenConnection> connections)
        {
            // Check currently selected lun is unique amongst other connected hosts.
            foreach (IXenConnection connection in connections)
            {
                foreach (SR sr in connection.Cache.SRs)
                {
                    if (sr.GetSRType(false) != SR.SRTypes.lvmoiscsi)
                        continue;

                    if (sr.PBDs.Count < 1)
                        continue;

                    PBD pbd = connection.Resolve(sr.PBDs[0]);

                    if (pbd == null)
                        continue;

                    if (UniquenessCheckMiami(connection, pbd))
                        return sr;
                }
            }

            return null;
        }

        /// <summary>
        /// Check currently LUN against miami host
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="pbd"></param>
        /// <returns></returns>
        private bool UniquenessCheckMiami(IXenConnection connection, PBD pbd)
        {
            if (!pbd.device_config.ContainsKey(SCSIID))
                return false;

            String scsiID = pbd.device_config[SCSIID];
            String myLUN = getIscsiLUN();

            if (!LunMap.ContainsKey(myLUN))
                return false;

            ISCSIInfo info = LunMap[myLUN];

            return info.ScsiID == scsiID;
        }

        private const String TARGET = "target";
        private const String PORT = "port";
        private const String TARGETIQN = "targetIQN";
        private const String LUNSERIAL = "LUNSerial";
        private const String SCSIID = "SCSIid";
        private const String LUNID = "LUNid";
        private const String CHAPUSER = "chapuser";
        private const String CHAPPASSWORD = "chappassword";

        public Dictionary<String, String> DeviceConfig
        {
            get
            {
                Dictionary<String, String> dconf = new Dictionary<String, String>();
                ToStringWrapper<IScsiIqnInfo> iqn = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
                if (iqn == null)
                    return null;

                // Reset target IP address to home address specified in IQN scan.
                // Allows multi-homing - see CA-11607
                dconf[TARGET] = iqn.item.IpAddress;
                dconf[PORT] = iqn.item.Port.ToString();
                dconf[TARGETIQN] = getIscsiIQN();

                if (!LunMap.ContainsKey(getIscsiLUN()))
                    return null;

                ISCSIInfo info = LunMap[getIscsiLUN()];
                if (info.LunID == -1)
                {
                    dconf[LUNSERIAL] = info.Serial;
                }
                else
                {
                    dconf[SCSIID] = info.ScsiID;
                }

                if (IscsiUseChapCheckBox.Checked)
                {
                    dconf[CHAPUSER] = IScsiChapUserTextBox.Text;
                    dconf[CHAPPASSWORD] = IScsiChapSecretTextBox.Text;
                }

                return dconf;
            }
        }

        private void buttonIscsiPopulateIQNs_Click(object sender, EventArgs e)
        {
            buttonIscsiPopulateIQNs.Enabled = false;
            // For this button to be enabled, we must be Miami or newer
            comboBoxIscsiIqns.Items.Clear();
            // Clear LUNs as they may no longer be valid
            ClearLunMapAndCombo();
            // Disable the Discover LUNs button because we have no IQNs
            buttonIscsiPopulateLUNs.Enabled = false;
            // Cancel any LUN scan in progress, as it is no longer meaningful
            if (IscsiPopulateLunsAction != null)
            {
                IscsiPopulateLunsAction.Cancel();
            }

            if (IscsiUseChapCheckBox.Checked)
            {
                IscsiPopulateIqnsAction = new ISCSIPopulateIQNsAction(connection,
                    getIscsiHost(), getIscsiPort(), IScsiChapUserTextBox.Text, IScsiChapSecretTextBox.Text);
            }
            else
            {
                IscsiPopulateIqnsAction = new ISCSIPopulateIQNsAction(connection,
                    getIscsiHost(), getIscsiPort(), null, null);
            }

            IscsiPopulateIqnsAction.Completed += IscsiPopulateIqnsAction_Completed;
            Dialogs.ActionProgressDialog dialog = new Dialogs.ActionProgressDialog(
                IscsiPopulateIqnsAction, ProgressBarStyle.Marquee);
            dialog.ShowCancel = true;
            dialog.ShowDialog(this);
        }

        private void IscsiPopulateIqnsAction_Completed(ActionBase sender)
        {
            Program.Invoke(this, (System.Threading.WaitCallback)IscsiPopulateIqnsAction_Completed_, sender);
        }

        private void IscsiPopulateIqnsAction_Completed_(object o)
        {
            Program.AssertOnEventThread();
            ISCSIPopulateIQNsAction action = (ISCSIPopulateIQNsAction)o;

            if (action.Succeeded)
            {
                if (action.IQNs.Length == 0)
                {
                    // Do nothing: ActionProgressDialog will show Messages.NEWSR_NO_IQNS_FOUND
                }
                else
                {
                    int width = comboBoxIscsiIqns.Width;
                    foreach (Actions.IScsiIqnInfo iqnInfo in action.IQNs)
                    {
                        if (!String.IsNullOrEmpty(iqnInfo.TargetIQN))
                        {
                            String toString = String.Format("{0} ({1}:{2})", iqnInfo.TargetIQN, iqnInfo.IpAddress, iqnInfo.Port);
                            comboBoxIscsiIqns.Items.Add(new ToStringWrapper<IScsiIqnInfo>(iqnInfo, toString));
                            width = Math.Max(width, Drawing.MeasureText(toString, comboBoxIscsiIqns.Font).Width);
                        }
                    }
                    // Set the combo box dropdown width to accommodate the widest item (within reason)
                    comboBoxIscsiIqns.DropDownWidth = Math.Min(width, Int16.MaxValue);

                    if (comboBoxIscsiIqns.Items.Count > 0)
                    {
                        comboBoxIscsiIqns.SelectedItem = comboBoxIscsiIqns.Items[0];
                        comboBoxIscsiIqns.Enabled = true;
                        buttonIscsiPopulateLUNs.Enabled = true;
                    }
                }
            }
            else
            {
                Failure failure = action.Exception as Failure;
                if (failure != null && failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140")
                {
                    labelIscsiInvalidHost.Visible = true;
                    textBoxIscsiHost.Focus();
                }
            }
            buttonIscsiPopulateIQNs.Enabled = true;
        }

        private void buttonIscsiPopulateLUNs_Click(object sender, EventArgs e)
        {
            buttonIscsiPopulateLUNs.Enabled = false;
            comboBoxIscsiLuns.Items.Clear();
            LunMap.Clear();

            if (IscsiUseChapCheckBox.Checked)
            {
                IscsiPopulateLunsAction = new Actions.ISCSIPopulateLunsAction(connection,
                    getIscsiHost(), getIscsiPort(), getIscsiIQN(), IScsiChapUserTextBox.Text, IScsiChapSecretTextBox.Text);
            }
            else
            {
                IscsiPopulateLunsAction = new Actions.ISCSIPopulateLunsAction(connection,
                    getIscsiHost(), getIscsiPort(), getIscsiIQN(), null, null);
            }

            IscsiPopulateLunsAction.Completed += IscsiPopulateLunsAction_Completed;
            using (var dialog = new ActionProgressDialog(IscsiPopulateLunsAction, ProgressBarStyle.Marquee))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }
        }

        private void IscsiPopulateLunsAction_Completed(ActionBase sender)
        {
            Program.Invoke(this, (WaitCallback)IscsiPopulateLunsAction_Completed_, sender);
        }

        private void IscsiPopulateLunsAction_Completed_(object o)
        {
            Program.AssertOnEventThread();
            ISCSIPopulateLunsAction action = (ISCSIPopulateLunsAction)o;

            buttonIscsiPopulateLUNs.Enabled = true;

            if (!action.Succeeded)
            {
                Failure failure = action.Exception as Failure;
                if (failure != null && failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140")
                {
                    labelIscsiInvalidHost.Visible = true;
                    textBoxIscsiHost.Focus();
                }
                return;
            }

            if (action.LUNs.Length == 0)
            {
                // Do nothing: ActionProgressDialog will show Messages.NEWSR_NO_LUNS_FOUND
            }
            else
            {
                int width = comboBoxIscsiLuns.Width;

                foreach (Actions.ISCSIInfo i in action.LUNs)
                {
                    String label = "LUN";
                    if (i.LunID != -1)
                        label += String.Format(" {0}", i.LunID);
                    if (i.Serial != "")
                        label += String.Format(": {0}", i.Serial);
                    if (i.Size >= 0)
                        label += String.Format(": {0}", Util.DiskSizeString(i.Size));
                    if (i.Vendor != "")
                        label += String.Format(" ({0})", i.Vendor);
                    comboBoxIscsiLuns.Items.Add(label);
                    LunMap.Add(label, i);

                    width = Math.Max(width, Drawing.MeasureText(label, comboBoxIscsiLuns.Font).Width);
                }
                comboBoxIscsiLuns.SelectedItem = comboBoxIscsiLuns.Items[0];
                comboBoxIscsiLuns.Enabled = true;

                // Set the combo box dropdown width to accommodate the widest item (within reason)
                comboBoxIscsiLuns.DropDownWidth = Math.Min(width, Int16.MaxValue);
            }

            UpdateButtons();
        }
    }
}
