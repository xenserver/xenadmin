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
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Threading;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using System.Drawing;


namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class LVMoISCSI : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// If an SR already exists on this LUN, this will point to the SR info which indicates that 
        /// the SR should be reattached.
        /// 
        /// If this is null, indicates the LUN should be formatted into a new SR.
        /// </summary>
        private SR.SRInfo _srToIntroduce;

        private ISCSIPopulateLunsAction IscsiPopulateLunsAction;
        private ISCSIPopulateIQNsAction IscsiPopulateIqnsAction;

        private readonly Dictionary<String, ISCSIInfo> LunMap = new Dictionary<String, ISCSIInfo>();

        private readonly ToolTip TargetIqnToolTip = new ToolTip();

        private const string TARGET = "target";
        private const string PORT = "port";
        private const string TARGETIQN = "targetIQN";
        private const string LUNSERIAL = "LUNSerial";
        private const string SCSIID = "SCSIid";
        private const string CHAPUSER = "chapuser";
        private const string CHAPPASSWORD = "chappassword";

        public LVMoISCSI()
        {
            InitializeComponent();
        }

        #region XentabPage overrides

        public override string PageTitle { get { return Messages.NEWSR_PATH_ISCSI; } }

        public override string Text { get { return Messages.NEWSR_LOCATION; } }

        public override string HelpID { get { return "Location_ISCSI"; } }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            HelpersGUI.PerformIQNCheck();

            if (direction == PageLoadedDirection.Forward)
                HelpersGUI.FocusFirstControl(Controls);
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                return;

            // For Miami hosts we need to ensure an SR.probe()
            // has been performed, and that the user has made a decision. Show the iSCSI choices dialog until
            // they click something other than 'Cancel'. For earlier host versions, warn that data loss may
            // occur.

            Host master = Helpers.GetMaster(Connection);
            if (master == null)
            {
                cancel = true;
                return;
            }

            Dictionary<String, String> dconf = DeviceConfig;
            if (dconf == null)
            {
                cancel = true;
                return;
            }

            // Start probe
            SrProbeAction IscsiProbeAction = new SrProbeAction(Connection, master, SR.SRTypes.lvmoiscsi, dconf);
            ActionProgressDialog dialog = new ActionProgressDialog(IscsiProbeAction, ProgressBarStyle.Marquee);
            dialog.ShowCancel = true;
            dialog.ShowDialog(this);

            // Probe has been performed. Now ask the user if they want to Reattach/Format/Cancel.
            // Will return false on cancel
            cancel = !ExamineIscsiProbeResults(IscsiProbeAction);
            
            base.PageLeave(direction, ref cancel);
        }

        public override bool EnableNext()
        {
            UInt16 i;
            bool portValid = UInt16.TryParse(textBoxIscsiPort.Text, out i);

            return !String.IsNullOrEmpty(getIscsiHost())
                && portValid
                && !(IscsiUseChapCheckBox.Checked && String.IsNullOrEmpty(IScsiChapUserTextBox.Text))
                && comboBoxIscsiLuns.SelectedIndex > -1
                && !LunInUse();
        }

        public override bool EnablePrevious()
        {
            if (SrWizardType.DisasterRecoveryTask && SrWizardType.SrToReattach == null)
                return false;

            return true;
        }

        public override void PopulatePage()
        {
            lunInUseLabel.Text = "";
            labelIscsiInvalidHost.Visible = false;

            // Enable IQN scanning
            comboBoxIscsiIqns.Visible = true;
            
            // IQN's can be very long, so we will show the value as a mouse over tooltip.
            // Initialize the tooltip here.
            TargetIqnToolTip.Active = true;
            TargetIqnToolTip.AutomaticDelay = 0;
            TargetIqnToolTip.AutoPopDelay = 50000;
            TargetIqnToolTip.InitialDelay = 50;
            TargetIqnToolTip.ReshowDelay = 50;
            TargetIqnToolTip.ShowAlways = true;
        }

        #endregion

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

            // Cause wizards next etc to update
            OnPageUpdated();
        }

        private void textBoxIscsiHost_TextChanged(object sender, EventArgs e)
        {
            labelIscsiInvalidHost.Visible = false;
            IScsiParams_TextChanged(null, null);
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

        private bool LunInUse()
        {
            SR sr = UniquenessCheck();

            // LUN is not in use iff sr != null

            if (sr == null)
            {
                lunInUseLabel.Text = "";
                return false;
            }

            Pool pool = Helpers.GetPool(sr.Connection);
            if (pool != null)
            {
                lunInUseLabel.Text = String.Format(Messages.NEWSR_LUN_IN_USE_ON_POOL,
                    sr.Name, pool.Name);
                return true;
            }

            Host master = Helpers.GetMaster(sr.Connection);
            if (master != null)
            {
                lunInUseLabel.Text = String.Format(Messages.NEWSR_LUN_IN_USE_ON_SERVER,
                     sr.Name, master.Name);
                return true;
            }

            lunInUseLabel.Text = Messages.NEWSR_LUN_IN_USE;
            return true;
        }

        private void comboBoxIscsiLuns_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
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

        /// <summary>
        /// Check the current config of the iSCSI sr in the wizard is unique across
        /// all active connections.
        /// </summary>
        /// <returns>SR that uses this config if not unique, null if unique</returns>
        private SR UniquenessCheck()
        {
            // Check currently selected lun is unique amongst other connected hosts.
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
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
            else
            {
                return Util.DEFAULT_ISCSI_PORT;
            }
        }

        private String getIscsiIQN()
        {
            ToStringWrapper<IScsiIqnInfo> wrapper = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (wrapper == null)
                return "";
            else
                return wrapper.item.TargetIQN;
        }

        private void IScsiTargetIqnComboBox_SelectedIndexChanged(object sender, EventArgs e)
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

        private String getIscsiLUN()
        {
            return comboBoxIscsiLuns.Text;
        }

        private void IscsiUseChapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxChap.Enabled = IscsiUseChapCheckBox.Checked;
            foreach (Control c in groupBoxChap.Controls)
            {
                c.Enabled = IscsiUseChapCheckBox.Checked;
            }
            lunInUseLabel.Text = "";
            ChapSettings_Changed(null, null);
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
                IscsiPopulateIqnsAction = new ISCSIPopulateIQNsAction(Connection,
                    getIscsiHost(), getIscsiPort(), IScsiChapUserTextBox.Text, IScsiChapSecretTextBox.Text);
            }
            else
            {
                IscsiPopulateIqnsAction = new ISCSIPopulateIQNsAction(Connection,
                    getIscsiHost(), getIscsiPort(), null, null);
            }

            ActionProgressDialog dialog = new ActionProgressDialog(IscsiPopulateIqnsAction, ProgressBarStyle.Marquee);
            //CA-80178: need to register Completed after the action has been added to the dialog, otherwise
            //the handler will be called before the dialog is closed and the focus won't be set on the right control.
            IscsiPopulateIqnsAction.Completed += IscsiPopulateIqnsAction_Completed;
            dialog.ShowCancel = true;
            dialog.ShowDialog(this);
        }

        private void ClearLunMapAndCombo()
        {
            // Clear LUNs as they may no longer be valid
            comboBoxIscsiLuns.Items.Clear();
            comboBoxIscsiLuns.Text = "";
            comboBoxIscsiLuns.Enabled = false;
            LunMap.Clear();
            lunInUseLabel.Text = "";

            UpdateButtons();
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
                    comboBoxIscsiIqns.Focus();
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
                IscsiPopulateLunsAction = new Actions.ISCSIPopulateLunsAction(Connection,
                    getIscsiHost(), getIscsiPort(), getIscsiIQN(), IScsiChapUserTextBox.Text, IScsiChapSecretTextBox.Text);
            }
            else
            {
                IscsiPopulateLunsAction = new Actions.ISCSIPopulateLunsAction(Connection,
                    getIscsiHost(), getIscsiPort(), getIscsiIQN(), null, null);
            }

            ActionProgressDialog dialog =  new ActionProgressDialog(IscsiPopulateLunsAction, ProgressBarStyle.Marquee);
            //CA-80178: need to register Completed after the action has been added to the dialog, otherwise
            //the handler will be called before the dialog is closed and the focus won't be set on the right control.
            IscsiPopulateLunsAction.Completed += IscsiPopulateLunsAction_Completed;
            dialog.ShowCancel = true;
            dialog.ShowDialog(this);
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
                }
                comboBoxIscsiLuns.SelectedItem = comboBoxIscsiLuns.Items[0];
                comboBoxIscsiLuns.Enabled = true;
                comboBoxIscsiLuns.Focus();
            }

            UpdateButtons();
        }

        /// <summary>
        /// Called with the results of an iSCSI SR.probe(), either immediately after the scan, or after the
        /// user has performed a scan, clicked 'cancel' on a dialog, and then clicked 'next' again (this
        /// avoids duplicate probing if none of the settings have changed).
        /// </summary>
        /// <returns>
        /// Whether to continue or not - wheter to format or not is stored in 
        /// iScsiFormatLUN.
        /// </returns>
        private bool ExamineIscsiProbeResults(SrProbeAction action)
        {
            _srToIntroduce = null;

            if (!action.Succeeded)
            {
                Exception exn = action.Exception;
                log.Warn(exn, exn);
                Failure failure = exn as Failure;
                if (failure != null && failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140")
                {
                    labelIscsiInvalidHost.Visible = true;
                    textBoxIscsiHost.Focus();
                }

                return false;
            }

            try
            {
                List<SR.SRInfo> SRs = SR.ParseSRListXML(action.Result);

                if (!String.IsNullOrEmpty(SrWizardType.UUID))
                {
                    // Check LUN contains correct SR
                    if (SRs.Count == 1 && SRs[0].UUID == SrWizardType.UUID)
                    {
                        _srToIntroduce = SRs[0];
                        return true;
                    }

                    lunInUseLabel.Text = String.Format(Messages.INCORRECT_LUN_FOR_SR, SrWizardType.SrName); 

                    return false;
                }
                else if (SRs.Count == 0)
                {
                    // No existing SRs were found on this LUN. If allowed to create new SR, ask the user if they want to proceed and format.
                    if (!SrWizardType.AllowToCreateNewSr)
                    {
                        new ThreeButtonDialog(
                           new ThreeButtonDialog.Details(SystemIcons.Error, Messages.NEWSR_LUN_HAS_NO_SRS, Messages.XENCENTER)).ShowDialog(this);

                        return false;
                    }
                    DialogResult result = Program.RunInAutomatedTestMode ? DialogResult.Yes :
                        new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.NEWSR_ISCSI_FORMAT_WARNING, this.Text),
                            ThreeButtonDialog.ButtonYes,
                            new ThreeButtonDialog.TBDButton(Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)).ShowDialog(this);

                    return result == DialogResult.Yes;
                }
                else
                {
                    // There should be 0 or 1 SRs on the LUN
                    System.Diagnostics.Trace.Assert(SRs.Count == 1);

                    // CA-17230
                    // Check this isn't a detached SR
                    SR.SRInfo info = SRs[0];
                    SR sr = SrWizardHelpers.SrInUse(info.UUID);
                    if (sr != null)
                    {
                        ThreeButtonDialog d = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(null, string.Format(Messages.DETACHED_ISCI_DETECTED, Helpers.GetName(sr.Connection))),
                            new ThreeButtonDialog.TBDButton(Messages.ATTACH_SR, DialogResult.OK),
                            ThreeButtonDialog.ButtonCancel);

                        DialogResult res = d.ShowDialog(Program.MainWindow);
                        if (res == DialogResult.Cancel)
                            return false;

                        _srToIntroduce = info;
                        return true;
                    }

                    // An SR exists on this LUN. Ask the user if they want to attach it, format it and
                    // create a new SR, or cancel.
                    DialogResult result = Program.RunInAutomatedTestMode ? DialogResult.Yes :
                        new IscsiChoicesDialog(Connection, info).ShowDialog(this);
                    
                    switch (result)
                    {
                        case DialogResult.Yes:
                            // Reattach
                            _srToIntroduce = SRs[0];
                            return true;

                        case DialogResult.No:
                            // Format - SrToIntroduce is already null
                            return true;

                        default:
                            return false;
                    }
                }
            }
            catch
            {
                // We really want to prevent the user getting to the next step if there is any kind of
                // exception here, since clicking 'finish' might destroy data: require another probe.
                return false;
            }
        }

        #region Accessors

        public SrWizardType SrWizardType { private get; set; }

        public string UUID { get { return _srToIntroduce == null ? null : _srToIntroduce.UUID; } }
        
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

        public string SrDescription
        {
            get
            {
                ToStringWrapper<IScsiIqnInfo> iqn = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
                return iqn == null ? null : string.Format(Messages.NEWSR_ISCSI_DESCRIPTION, iqn.item.IpAddress, getIscsiIQN(), getIscsiLUN());
            }
        }

        #endregion
    }
}
