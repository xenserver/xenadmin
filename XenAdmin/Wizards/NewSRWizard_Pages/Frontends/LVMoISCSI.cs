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
using System.Collections.Generic;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using System.Linq;
using XenAdmin.Dialogs.WarningDialogs;
using XenCenterLib;


namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class LVMoISCSI : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Non-null value indicates an SR already existing on this LUN that should be reattached. 
        /// Null indicates the LUN should be formatted into a new SR.
        /// </summary>
        private SR.SRInfo _srToIntroduce;

        private ISCSIPopulateLunsAction _populateLunsAction;
        private ISCSIPopulateIQNsAction _populateIqnsAction;

        private readonly Dictionary<string, ISCSIInfo> LunMap = new Dictionary<string, ISCSIInfo>();

        /// <summary>
        /// Tooltip to show IQN value (as it can be very long)
        /// </summary>
        private readonly ToolTip toolTipTargetIqn = new ToolTip
        {
            Active = true,
            AutomaticDelay = 0,
            AutoPopDelay = 50000,
            InitialDelay = 50,
            ReshowDelay = 50,
            ShowAlways = true
        };

        private bool _errorExists;
        private bool _buttonNextEnabled;

        private const string PROVIDER = "provider";
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
            SrType = SR.SRTypes.lvmoiscsi;
            HideErrors();
        }

        #region XentabPage overrides

        public override string PageTitle => Messages.NEWSR_PATH_ISCSI;

        public override string Text => Messages.NEWSR_LOCATION;

        public override string HelpID => "Location_ISCSI";

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            HelpersGUI.PerformIQNCheck();

            if (direction == PageLoadedDirection.Forward)
            {
                textBoxIscsiHost.Focus();
                ResetAll();
            }
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                return;

            HideErrors();

            Host coordinator = Helpers.GetCoordinator(Connection);
            if (coordinator == null)
            {
                cancel = true;
                return;
            }

            var currentSrType = SrType;

            if (!RunProbe(coordinator, currentSrType, out var srs)) // start first probe
            {
                cancel = true;
                return;
            }

            var performSecondProbe = Helpers.KolkataOrGreater(Connection) && !Helpers.FeatureForbidden(Connection, Host.CorosyncDisabled);

            if (performSecondProbe && srs.Count == 0)
            {
                currentSrType = SrType == SR.SRTypes.gfs2 ? SR.SRTypes.lvmoiscsi : SR.SRTypes.gfs2;

                if (!RunProbe(coordinator, currentSrType, out srs)) // start second probe
                {
                    cancel = true;
                    return;
                }
            }

            // Probe has been performed. Now ask the user if they want to Reattach/Format/Cancel.
            // Will return false on cancel
            cancel = !ExamineIscsiProbeResults(currentSrType, srs);
        }

        public override void PageCancelled(ref bool cancel)
        {
            _populateIqnsAction?.Cancel();
            _populateLunsAction?.Cancel();
        }

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
        }

        public override bool EnablePrevious()
        {
            if (SrWizardType.DisasterRecoveryTask && SrWizardType.SrToReattach == null)
                return false;

            return _populateIqnsAction == null && _populateLunsAction == null;
        }

        public override void PopulatePage()
        {
            HideErrors();
        }

        #endregion

        private bool RunProbe(Host coordinator, SR.SRTypes srType, out List<SR.SRInfo> srs)
        {
            srs = new List<SR.SRInfo>();

            var dconf = GetDeviceConfig(srType);
            if (dconf == null)
                return false;

            var action = new SrProbeAction(Connection, coordinator, srType, dconf);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee) {ShowCancel = true})
                dialog.ShowDialog(this);

            srs = action.SRs ?? new List<SR.SRInfo>();

            if (action.Succeeded)
                return true;

            HandleFailure(action);
            return false;
        }

        private void UpdateButtons()
        {
            bool portValid = ushort.TryParse(textBoxIscsiPort.Text, out _);
            var iscsiHost = getIscsiHost();
            bool validChap = !checkBoxUseChap.Checked || !string.IsNullOrEmpty(textBoxChapUser.Text);

            buttonScanTargetHost.Enabled = !string.IsNullOrEmpty(iscsiHost) && portValid && validChap &&
                                           _populateIqnsAction == null && _populateLunsAction == null;

            _buttonNextEnabled = !string.IsNullOrEmpty(iscsiHost) && portValid && validChap &&
                                 !string.IsNullOrEmpty(getIscsiLUN()) && !_errorExists;

            OnPageUpdated();
        }

        private string getIscsiHost()
        {
            // If the user has selected an IQN, use the host from that IQN (due to multi-homing, 
            // this may differ from the host they first entered). Otherwise use the host
            // they first entered
            
            if (comboBoxIscsiIqns.SelectedItem is ToStringWrapper<IScsiIqnInfo> wrapper)
                return wrapper.item.IpAddress;
            return textBoxIscsiHost.Text.Trim();
        }

        private ushort getIscsiPort()
        {
            if (comboBoxIscsiIqns.SelectedItem is ToStringWrapper<IScsiIqnInfo> wrapper)
                return wrapper.item.Port;

            if (ushort.TryParse(textBoxIscsiPort.Text, out var port))
                return port;
            
            return Util.DEFAULT_ISCSI_PORT;
        }

        private string getIscsiIQN()
        {
            if (comboBoxIscsiIqns.SelectedItem is ToStringWrapper<IScsiIqnInfo> wrapper)
                return wrapper.item.TargetIQN;
           
            return "";
        }

        private string getIscsiLUN()
        {
            var text = comboBoxIscsiLuns.SelectedItem as string;
            return text == null || text == Messages.SELECT_TARGET_LUN ? "" : text;
        }

        private void ShowError(PictureBox errorIcon, Label errorLabel, string errorMessage)
        {
            _errorExists = true;
            errorLabel.Text = errorMessage;
            errorIcon.Visible = true;
            errorLabel.Visible = true;
        }

        private void HideErrors()
        {
            _errorExists = false;
            errorIconAtHostOrIP.Visible = false;
            errorIconAtCHAPPassword.Visible = false;
            errorIconAtTargetLUN.Visible = false;
            errorIconBottom.Visible = false;
            errorLabelAtHostOrIP.Visible = false;
            errorLabelAtCHAPPassword.Visible = false;
            errorLabelAtTargetLUN.Visible = false;
            errorLabelBottom.Visible = false;
        }

        private void ResetIqns()
        {
            spinnerIconAtTargetIqn.StopSpinning();
            try
            {
                comboBoxIscsiIqns.SelectedIndexChanged -= comboBoxIscsiIqns_SelectedIndexChanged;
                comboBoxIscsiIqns.Items.Clear();
            }
            finally
            {
                comboBoxIscsiIqns.SelectedIndexChanged += comboBoxIscsiIqns_SelectedIndexChanged;
            }

            comboBoxIscsiIqns.Enabled = false;
            labelTargetIqn.Enabled = false;
            toolTipTargetIqn.SetToolTip(comboBoxIscsiIqns, null);
        }

        private void ResetLuns()
        {
            spinnerIconAtTargetLun.StopSpinning();
            try
            {
                comboBoxIscsiLuns.SelectedIndexChanged -= comboBoxIscsiLuns_SelectedIndexChanged;
                comboBoxIscsiLuns.Items.Clear();
            }
            finally
            {
                comboBoxIscsiLuns.SelectedIndexChanged += comboBoxIscsiLuns_SelectedIndexChanged;
            }
            
            comboBoxIscsiLuns.Enabled = false;
            labelTargetLun.Enabled = false;
            LunMap.Clear();
        }

        private void ResetAll()
        {
            spinnerIconAtScanTargetHostButton.StopSpinning();
            iSCSITargetGroupBox.Enabled = false;

            HideErrors();
            ResetIqns();
            ResetLuns();
            UpdateButtons();
        }

        private void DisableAllInputControls()
        {
            textBoxIscsiHost.Enabled = false;
            textBoxIscsiPort.Enabled = false;
            checkBoxUseChap.Enabled = false;
            textBoxChapUser.Enabled = false;
            textBoxChapPassword.Enabled = false;
            iSCSITargetGroupBox.Enabled = false;
        }

        private void EnableInputControls()
        {
            textBoxIscsiHost.Enabled = true;
            textBoxIscsiPort.Enabled = true;
            checkBoxUseChap.Enabled = true;
            EnableChapControls();
            iSCSITargetGroupBox.Enabled = true;
        }

        private void EnableChapControls()
        {
            bool enabled = checkBoxUseChap.Checked;
            textBoxChapUser.Enabled = enabled;
            textBoxChapPassword.Enabled = enabled;
            labelCHAPuser.Enabled = enabled;
            IScsiChapSecretLabel.Enabled = enabled;
        }

        private void HandleFailure(ActionBase action)
        {
            if (action == null || !(action.Exception is Failure failure) || failure.ErrorDescription.Count < 1)
                return;

            if (failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_68")
            {
                ShowError(errorIconAtCHAPPassword, errorLabelAtCHAPPassword, Messages.LOGGING_IN_TO_THE_ISCSI_TARGET_FAILED);
                textBoxChapUser.Focus();
            }
            else if (failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140")
            {
                ShowError(errorIconAtHostOrIP, errorLabelAtHostOrIP, Messages.INVALID_HOST);
                textBoxIscsiHost.Focus();
            }
            else if (failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_141")
            {
                ShowError(errorIconAtHostOrIP, errorLabelAtHostOrIP, Messages.SR_UNABLE_TO_CONNECT_TO_SCSI_TARGET);
                textBoxIscsiHost.Focus();
            }
            else
                ShowError(errorIconBottom, errorLabelBottom, failure.Message);
        }

        #region Event handlers
        
        private void textBoxIscsiHost_TextChanged(object sender, EventArgs e)
        {
            ResetAll();
        }

        private void checkBoxUseChap_CheckedChanged(object sender, EventArgs e)
        {
            EnableChapControls();
            ResetAll();
        }

        private void textBoxChapPassword_TextChanged(object sender, EventArgs e)
        {
            ResetAll();
        }

        private void textBoxChapUser_TextChanged(object sender, EventArgs e)
        {
            ResetAll();
        }

        private void buttonScanTargetHost_Click(object sender, EventArgs e)
        {
            HideErrors();
            ResetIqns();
            ResetLuns();

            DisableAllInputControls();
            spinnerIconAtScanTargetHostButton.StartSpinning();

            _populateIqnsAction = SrType == SR.SRTypes.gfs2
                ? new Gfs2PopulateIQNsAction(Connection, getIscsiHost(), getIscsiPort(), ChapUser, ChapPassword)
                : new ISCSIPopulateIQNsAction(Connection, getIscsiHost(), getIscsiPort(), ChapUser, ChapPassword);
            
            _populateIqnsAction.Completed += PopulateIqnsAction_Completed;
            _populateIqnsAction.RunAsync();
            UpdateButtons();
        }

        private void comboBoxIscsiIqns_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideErrors();
            ResetLuns();
            
            if (comboBoxIscsiIqns.SelectedItem is ToStringWrapper<IScsiIqnInfo> wrapper)
            {
                toolTipTargetIqn.SetToolTip(comboBoxIscsiIqns, wrapper.ToString());
                DisableAllInputControls();
                spinnerIconAtTargetIqn.StartSpinning();

                _populateLunsAction = SrType == SR.SRTypes.gfs2
                    ? new Gfs2PopulateLunsAction(Connection, getIscsiHost(), getIscsiPort(), getIscsiIQN(), ChapUser, ChapPassword)
                    : new ISCSIPopulateLunsAction(Connection, getIscsiHost(), getIscsiPort(), getIscsiIQN(), ChapUser, ChapPassword);
            
                _populateLunsAction.Completed += PopulateLunsAction_Completed;
                _populateLunsAction.RunAsync();
            }
            else
            {
                toolTipTargetIqn.SetToolTip(comboBoxIscsiIqns, Messages.SELECT_TARGET_IQN);
            }

            UpdateButtons();
        }

        private void comboBoxIscsiLuns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxIscsiLuns.SelectedItem as string == Messages.SELECT_TARGET_LUN)
            {
                spinnerIconAtTargetLun.StopSpinning();
                HideErrors();
            }
            else
            {
                spinnerIconAtTargetLun.ShowSuccessImage();
                var isLunInUse = SrUsingLunExists(out var sr);//in this or other connected pools

                if (isLunInUse)
                    ShowError(errorIconAtTargetLUN, errorLabelAtTargetLUN, LVMoIsciWarningDialog.GetSrInUseMessage(sr));
                else
                    HideErrors();
            }

            UpdateButtons();
        }


        private void PopulateIqnsAction_Completed(ActionBase sender)
        {
            Program.Invoke(this, () => PopulateIqnsAction_Completed_(sender as ISCSIPopulateIQNsAction));
        }

        private void PopulateIqnsAction_Completed_(ISCSIPopulateIQNsAction action)
        {
            if (action == null)
                return;

            Program.AssertOnEventThread();

            _populateIqnsAction = null;
            EnableInputControls();

            if (!action.Succeeded)
            {
                spinnerIconAtScanTargetHostButton.StopSpinning();
                HandleFailure(action);
                UpdateButtons();
                return;
            }

            // If no IQNs are found do nothing; the ActionProgressDialog will have shown Messages.NEWSR_NO_IQNS_FOUND
            var validIqns = action.IQNs.Where(info => !string.IsNullOrEmpty(info.TargetIQN)).ToList();
            if (validIqns.Count == 0)
            {
                UpdateButtons();
                return;
            }
            
            int width = comboBoxIscsiIqns.Width;

            comboBoxIscsiIqns.Items.Add(Messages.SELECT_TARGET_IQN);

            foreach (IScsiIqnInfo iqnInfo in validIqns)
            {
                var toString = string.Format("{0} ({1}:{2})", iqnInfo.TargetIQN, iqnInfo.IpAddress, iqnInfo.Port);
                comboBoxIscsiIqns.Items.Add(new ToStringWrapper<IScsiIqnInfo>(iqnInfo, toString));
                width = Math.Max(width, Drawing.MeasureText(toString, comboBoxIscsiIqns.Font).Width);
            }

            // Set the combo box dropdown width to accommodate the widest item (within reason)
            comboBoxIscsiIqns.DropDownWidth = Math.Min(width, Int16.MaxValue);

            comboBoxIscsiIqns.SelectedItem = comboBoxIscsiIqns.Items.Count == 2
                ? comboBoxIscsiIqns.Items[1]
                : Messages.SELECT_TARGET_IQN;

            labelTargetIqn.Enabled = true;
            comboBoxIscsiIqns.Enabled = true;
            comboBoxIscsiIqns.Focus();
            spinnerIconAtScanTargetHostButton.ShowSuccessImage();
            UpdateButtons();
        }

        private void PopulateLunsAction_Completed(ActionBase sender)
        {
            Program.Invoke(this, () => PopulateLunsAction_Completed_(sender as ISCSIPopulateLunsAction));
        }

        private void PopulateLunsAction_Completed_(ISCSIPopulateLunsAction action)
        {
            if (action == null)
                return;

            Program.AssertOnEventThread();

            _populateLunsAction = null;
            EnableInputControls();

            if (!action.Succeeded)
            {
                spinnerIconAtTargetIqn.StopSpinning();
                HandleFailure(action);
                UpdateButtons();
                return;
            }

            // If no IQNs are found do nothing; the ActionProgressDialog will have shown Messages.NEWSR_NO_LUNS_FOUND
            if (action.LUNs.Length == 0)
            {
                UpdateButtons();
                return;
            }

            comboBoxIscsiLuns.Items.Add(Messages.SELECT_TARGET_LUN);

            foreach (ISCSIInfo i in action.LUNs)
            {
                string label = "LUN";
                if (i.LunID != -1)
                    label += string.Format(" {0}", i.LunID);
                if (i.Serial != "")
                    label += string.Format(": {0}", i.Serial);
                if (i.Size >= 0)
                    label += string.Format(": {0}", Util.DiskSizeString(i.Size));
                if (i.Vendor != "")
                    label += string.Format(" ({0})", i.Vendor);
                comboBoxIscsiLuns.Items.Add(label);
                LunMap.Add(label, i);
            }

            //if there is only one choice, select that one by default
            comboBoxIscsiLuns.SelectedItem = comboBoxIscsiLuns.Items.Count == 2
                ? comboBoxIscsiLuns.Items[1]
                : Messages.SELECT_TARGET_LUN;

            labelTargetLun.Enabled = true;
            comboBoxIscsiLuns.Enabled = true;
            comboBoxIscsiLuns.Focus();
            spinnerIconAtTargetIqn.ShowSuccessImage();
            UpdateButtons();
        }

        #endregion

        private bool SrUsingLunExists(out SR theSr)
        {
            theSr = null;

            if (string.IsNullOrEmpty(getIscsiLUN()))
                return false;

            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (SR sr in connection.Cache.SRs)
                {
                    if (sr.GetSRType(false) != SR.SRTypes.lvmoiscsi && sr.GetSRType(false) != SR.SRTypes.gfs2)
                        continue;

                    if (sr.PBDs.Count < 1)
                        continue;

                    PBD pbd = connection.Resolve(sr.PBDs[0]);

                    if (pbd == null)
                        continue;

                    if (pbd.device_config.TryGetValue(SCSIID, out var scsiId) &&
                        LunMap.TryGetValue(getIscsiLUN(), out var info) &&
                        info.ScsiID == scsiId)
                    {
                        theSr = sr;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ExamineIscsiProbeResults(SR.SRTypes currentSrType, List<SR.SRInfo> srs)
        {
            _srToIntroduce = null;

            if (srs == null)
                return false;

            // There should be 0 or 1 SRs on the LUN
            System.Diagnostics.Debug.Assert(srs.Count == 0 || srs.Count == 1);

            try
            {
                if (!string.IsNullOrEmpty(SrWizardType.UUID))
                {
                    // Check LUN contains correct SR
                    if (srs.Count == 1 && srs[0].UUID == SrWizardType.UUID)
                    {
                        _srToIntroduce = srs[0];
                        SrType = currentSrType; // the type of the existing SR
                        return true;
                    }

                    ShowError(errorIconAtTargetLUN, errorLabelAtTargetLUN, string.Format(Messages.INCORRECT_LUN_FOR_SR, SrWizardType.SrName));
                    return false;
                }

                if (srs.Count == 0)
                {
                    if (!SrWizardType.AllowToCreateNewSr)
                    {
                        using (var dlg = new ErrorDialog(Messages.NEWSR_LUN_HAS_NO_SRS))
                            dlg.ShowDialog(this);

                        return false;
                    }

                    if (Program.RunInAutomatedTestMode)
                        return true;

                    // SR creation is allowed; ask the user if they want to proceed and format.
                    using (var dlog = new LVMoIsciWarningDialog(Connection, null, currentSrType, SrType))
                    {
                        dlog.ShowDialog(this);
                        return dlog.SelectedOption == LVMoHBAWarningDialog.UserSelectedOption.Format;
                    }
                }

                if (Program.RunInAutomatedTestMode)
                    return true;

                // offer to attach it, or format it to create a new SR, or cancel
                SR.SRInfo srInfo = srs[0];

                using (var dlog = new LVMoIsciWarningDialog(Connection, srInfo, currentSrType, SrType))
                {
                    dlog.ShowDialog(this);
                        
                    switch (dlog.SelectedOption)
                    {
                        case LVMoHBAWarningDialog.UserSelectedOption.Reattach:
                            _srToIntroduce = srInfo;
                            SrType = currentSrType; // the type of the existing SR
                            return true;
                        case LVMoHBAWarningDialog.UserSelectedOption.Format:
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

        private Dictionary<string, string> GetDeviceConfig(SR.SRTypes srType)
        {
            var iqn = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
            if (iqn == null)
                return null;

            var selectedLan = getIscsiLUN();
            if (!LunMap.TryGetValue(selectedLan, out ISCSIInfo info))
                return null;

            var dconf = new Dictionary<string, string>();

            if (srType == SR.SRTypes.gfs2)
            {
                if (_srToIntroduce != null && _srToIntroduce.Configuration != null)
                    dconf = _srToIntroduce.Configuration;

                dconf[PROVIDER] = "iscsi";
                dconf[TARGET] = iqn.item.IpAddress;
                dconf[PORT] = iqn.item.Port.ToString();
                dconf[TARGETIQN] = getIscsiIQN();
                dconf[SCSIID] = LunMap[selectedLan].ScsiID;

                if (checkBoxUseChap.Checked)
                {
                    dconf[CHAPUSER] = textBoxChapUser.Text;
                    dconf[CHAPPASSWORD] = textBoxChapPassword.Text;
                }

                return dconf;
            }

            // Reset target IP address to home address specified in IQN scan.
            // Allows multi-homing - see CA-11607
            dconf[TARGET] = iqn.item.IpAddress;
            dconf[PORT] = iqn.item.Port.ToString();
            dconf[TARGETIQN] = getIscsiIQN();

            if (info.LunID == -1)
                dconf[LUNSERIAL] = info.Serial;
            else
                dconf[SCSIID] = info.ScsiID;

            if (checkBoxUseChap.Checked)
            {
                dconf[CHAPUSER] = textBoxChapUser.Text;
                dconf[CHAPPASSWORD] = textBoxChapPassword.Text;
            }

            return dconf;
        }

        #region Accessors

        private string ChapUser => checkBoxUseChap.Checked ? textBoxChapUser.Text : null;
        private string ChapPassword => checkBoxUseChap.Checked ? textBoxChapPassword.Text : null;

        public SrWizardType SrWizardType { private get; set; }

        public string UUID => _srToIntroduce?.UUID;

        public Dictionary<string, string> DeviceConfig => GetDeviceConfig(SrType);

        public string SrDescription
        {
            get
            {
                ToStringWrapper<IScsiIqnInfo> iqn = comboBoxIscsiIqns.SelectedItem as ToStringWrapper<IScsiIqnInfo>;
                return iqn == null ? null : string.Format(Messages.NEWSR_ISCSI_DESCRIPTION, iqn.item.IpAddress, getIscsiIQN(), getIscsiLUN());
            }
        }

        public SR.SRTypes SrType { get; set; }

        #endregion
    }
}
