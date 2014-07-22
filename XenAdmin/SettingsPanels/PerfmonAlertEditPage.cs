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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Alerts;
using XenAdmin.Actions;
using XenAdmin.Controls;
using System.Diagnostics;


namespace XenAdmin.SettingsPanels
{
    public partial class PerfmonAlertEditPage : UserControl, IEditPage
    {
        private IXenObject _XenObject;

        private readonly ToolTip m_invalidParamToolTip;

        private readonly AlertGroup cpuAlert;
        private readonly AlertGroup netAlert;
        private readonly AlertGroup diskAlert;
        private readonly AlertGroup memoryAlert;
        private readonly AlertGroup srAlert;

        public PerfmonAlertEditPage()
        {
            InitializeComponent();
            Text = Messages.ALERTS;

            m_invalidParamToolTip = new ToolTip
                {
                    IsBalloon = true,
                    ToolTipIcon = ToolTipIcon.Warning,
                    ToolTipTitle = Messages.INVALID_PARAMETER
                };

            ////xapi trigger level is a fraction; gui shows percentage
            cpuAlert = new AlertGroup(CPUAlertCheckBox, CpuGroupBox,
                nudCPUUsagePercent, nudCPUDurationThreshold, nudAlertInterval,
                new[] { cpuMinutesLabel, cpuPercentLabel, CPUUsagePercentLabel, CPUDurationThresholdLabel })
                {
                    AlertEnablementChanged = SetAlertIntervalEnablement,
                    SubTextFormat = Messages.ALERT_CPUS_SUB_TEXT,
                    PerfmonDefinitionName = PerfmonDefinition.ALARM_TYPE_CPU,
                    XapiToGuiTriggerLevel = (num => num * 100),
                    XapiToGuiTriggerPeriod = (num => num / 60),
                    XapiToGuiAlertInterval = (num => num / 60),
                    GuiToXapiTriggerLevel = (num => num / 100),
                    GuiToXapiTriggerPeriod = (num => num * 60),
                    GuiToXapiAlertInterval = (num => num * 60),
                };

            //xapi trigger level is in B/s; gui shows KB/s
            netAlert = new AlertGroup(NetAlertCheckBox, NetGroupBox,
                nudNetUsagePercent, nudNetDurationThreshold, nudAlertInterval,
                new[] { NetMinutesLabel, NetPercentLabel, NetUsagePercentLabel, NetDurationThresholdLabel })
                {
                    AlertEnablementChanged = SetAlertIntervalEnablement,
                    SubTextFormat = Messages.ALERT_NET_SUB_TEXT,
                    PerfmonDefinitionName = PerfmonDefinition.ALARM_TYPE_NETWORK,
                    XapiToGuiTriggerLevel = (num => num / 1024),
                    XapiToGuiTriggerPeriod = (num => num / 60),
                    XapiToGuiAlertInterval = (num => num / 60),
                    GuiToXapiTriggerLevel = (num => num * 1024),
                    GuiToXapiTriggerPeriod = (num => num * 60),
                    GuiToXapiAlertInterval = (num => num * 60),
                };

            //xapi trigger level is in B/s; gui shows KB/s
            diskAlert = new AlertGroup(DiskAlertCheckBox, DiskGroupBox,
                nudDiskUsagePercent, nudDiskDurationThreshold, nudAlertInterval,
                new[] { DiskMinutesLabel, DiskPercentLabel, DiskUsagePercentLabel, DiskDurationThresholdLabel })
                {
                    AlertEnablementChanged = SetAlertIntervalEnablement,
                    SubTextFormat = Messages.ALERT_DISK_SUB_TEXT,
                    PerfmonDefinitionName = PerfmonDefinition.ALARM_TYPE_DISK,
                    XapiToGuiTriggerLevel = (num => num / 1024),
                    XapiToGuiTriggerPeriod = (num => num / 60),
                    XapiToGuiAlertInterval = (num => num / 60),
                    GuiToXapiTriggerLevel = (num => num * 1024),
                    GuiToXapiTriggerPeriod = (num => num * 60),
                    GuiToXapiAlertInterval = (num => num * 60),
                };

            //xapi trigger level is in kiB; gui shows MB
            memoryAlert = new AlertGroup(MemoryAlertCheckBox, MemoryGroupBox,
                nudMemoryUsage, nudMemoryDurationThreshold, nudAlertInterval,
                new[] { memoryMinutesLabel, memoryUsageLabel, memoryUnitsLabel, memoryDurationThresholdLabel })
                {
                    AlertEnablementChanged = SetAlertIntervalEnablement,
                    SubTextFormat = Messages.ALERT_MEMORY_SUB_TEXT,
                    PerfmonDefinitionName = PerfmonDefinition.ALARM_TYPE_MEMORY,
                    XapiToGuiTriggerLevel = (num => num / 1024),
                    XapiToGuiTriggerPeriod = (num => num / 60),
                    XapiToGuiAlertInterval = (num => num / 60),
                    GuiToXapiTriggerLevel = (num => num * 1024),
                    GuiToXapiTriggerPeriod = (num => num * 60),
                    GuiToXapiAlertInterval = (num => num * 60),
                };

            //xapi trigger level is in MiB/s; gui shows KB/s
            srAlert = new AlertGroup(SrAlertCheckBox, SrGroupBox,
                nudSrUsage, nudSrMinutes, nudAlertInterval,
                new[] { SrUsageLabel, srMinutesLabel, srUnitsLabel, SrDurationThresholdLabel })
                {
                    AlertEnablementChanged = SetAlertIntervalEnablement,
                    SubTextFormat = Messages.ALERT_SR_SUB_TEXT,
                    PerfmonDefinitionName = PerfmonDefinition.ALARM_TYPE_SR,
                    XapiToGuiTriggerLevel = (num => num * 1024),
                    XapiToGuiTriggerPeriod = (num => num / 60),
                    XapiToGuiAlertInterval = (num => num / 60),
                    GuiToXapiTriggerLevel = (num => num / 1024),
                    GuiToXapiTriggerPeriod = (num => num * 60),
                    GuiToXapiAlertInterval = (num => num * 60),
                };

            cpuAlert.ToggleAlertGroupEnablement();
            netAlert.ToggleAlertGroupEnablement();
            diskAlert.ToggleAlertGroupEnablement();
            memoryAlert.ToggleAlertGroupEnablement();
            srAlert.ToggleAlertGroupEnablement();
        }

        public string SubText
        {
            get
            {
                var subs = from AlertGroup g in new[] { cpuAlert, netAlert, diskAlert, memoryAlert, srAlert }
                           where !string.IsNullOrEmpty(g.SubText)
                           select g.SubText;

                string text = string.Join("; ", subs.ToArray());
                return string.IsNullOrEmpty(text) ? Messages.NONE_DEFINED : text;
            }
        }

        public Image Image
        {
            get { return Properties.Resources._000_Alert2_h32bit_16; }
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _XenObject = clone;
            bool isVm = _XenObject is VM;
            bool isHost = _XenObject is Host;
            bool isSr = _XenObject is SR;

            cpuAlert.Show(isVm || isHost);
            netAlert.Show(isVm || isHost);
            diskAlert.Show(isVm);
            memoryAlert.Show(isHost && Helpers.ClearwaterOrGreater(_XenObject.Connection));
            srAlert.Show(isSr && Helpers.ClearwaterOrGreater(_XenObject.Connection));

            if (isHost)
            {
                Host host = (Host)_XenObject;
                Host_metrics metrics = host.Connection.Resolve(host.metrics);
                if (metrics != null)
                    nudMemoryUsage.Maximum = metrics.memory_total / (1024 * 1024);
            }

            Repopulate();
        }

        private void Repopulate()
        {
            if (_XenObject == null)
                return;
            try
            {
                var perfmonDefinitions = PerfmonDefinition.GetPerfmonDefinitions(_XenObject);

                for (int i = 0; i < perfmonDefinitions.Length; i++)
                {
                    PerfmonDefinition perfmonDefinition = perfmonDefinitions[i];

                    if (perfmonDefinition.IsCPUUsage)
                        cpuAlert.Populate(perfmonDefinition);
                    else if (perfmonDefinition.IsNetworkUsage)
                        netAlert.Populate(perfmonDefinition);
                    else if (perfmonDefinition.IsDiskUsage)
                        diskAlert.Populate(perfmonDefinition);
                    else if (perfmonDefinition.IsMemoryUsage)
                        memoryAlert.Populate(perfmonDefinition);
                    else if (perfmonDefinition.IsSrUsage)
                        srAlert.Populate(perfmonDefinition);
                }
            }
            catch { }
        }

        public bool HasChanged
        {
            get
            {
                if (nudAlertInterval.HasChanged)
                    return true;

                if (_XenObject is SR)
                    return srAlert.HasChanged;

                if (_XenObject is VM)
                    return cpuAlert.HasChanged || netAlert.HasChanged || diskAlert.HasChanged;

                return cpuAlert.HasChanged || netAlert.HasChanged || memoryAlert.HasChanged;
            }
        }

        public void ShowLocalValidationMessages()
        {
            decimal val;
            if (decimal.TryParse(nudAlertInterval.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out val)
                 && val % 5 != 0)
            {
                HelpersGUI.ShowBalloonMessage(nudAlertInterval,
                    Messages.PERFORM_ALERT_EDIT_INTERVAL_WRONG_MULTIPLE,
                    m_invalidParamToolTip);
            }
        }

        public void Cleanup()
        {
            if (m_invalidParamToolTip != null)
                m_invalidParamToolTip.Dispose();
        }

        public bool ValidToSave
        {
            get
            {
                decimal val;

                if (!decimal.TryParse(nudAlertInterval.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                    return false;

                if (val % 5 != 0)
                    return false;

                return true;
            }
        }

        public AsyncAction SaveSettings()
        {
            List<PerfmonDefinition> perfmonDefinitions = new List<PerfmonDefinition>();

            if (cpuAlert.Enabled)
                perfmonDefinitions.Add(cpuAlert.AlertDefinition);

            if (netAlert.Enabled)
                perfmonDefinitions.Add(netAlert.AlertDefinition);

            if (_XenObject is VM && diskAlert.Enabled)
                perfmonDefinitions.Add(diskAlert.AlertDefinition);

            if (_XenObject is Host && memoryAlert.Enabled)
                perfmonDefinitions.Add(memoryAlert.AlertDefinition);

            if (_XenObject is SR && srAlert.Enabled)
                perfmonDefinitions.Add(srAlert.AlertDefinition);

            return new PerfmonDefinitionAction(_XenObject, perfmonDefinitions, true);
        }

        private void SetAlertIntervalEnablement()
        {
            bool enable = cpuAlert.Enabled || netAlert.Enabled || diskAlert.Enabled
                          || memoryAlert.Enabled || srAlert.Enabled;

            nudAlertInterval.Enabled = enable;
            AlertIntervalMinutesLabel.Enabled = enable;
            AlertIntervalLabel.Enabled = enable;
        }
    }

    public class AlertGroup
    {
        private readonly AlertCheckBox m_checkBox;
        private readonly DecentGroupBox m_groupBox;
        private readonly AlertNumericUpDown m_upDownTriggerLevel;
        private readonly AlertNumericUpDown m_upDownTriggerPeriod;
        private readonly AlertNumericUpDown m_upDownAlertInterval;
        private readonly IEnumerable<Label> m_labels;

        public AlertGroup(AlertCheckBox theCheckBox, DecentGroupBox theGroupBox,
            AlertNumericUpDown triggerLevelUpDown, AlertNumericUpDown triggerThresholdUpDown,
            AlertNumericUpDown alertIntervalUpDown, IEnumerable<Label> theLabels)
        {
            Debug.Assert(theCheckBox != null && theGroupBox != null
                         && triggerLevelUpDown != null && triggerThresholdUpDown != null
                         && alertIntervalUpDown != null && theLabels != null);

            m_checkBox = theCheckBox;
            m_groupBox = theGroupBox;
            m_upDownTriggerLevel = triggerLevelUpDown;
            m_upDownTriggerPeriod = triggerThresholdUpDown;
            m_upDownAlertInterval = alertIntervalUpDown;
            m_labels = theLabels;
            m_checkBox.CheckedChanged += m_theCheckBox_CheckedChanged;

            StoreOriginalSetting();
        }

        public Action AlertEnablementChanged;
        public string SubTextFormat;
        public string PerfmonDefinitionName;
        public Func<decimal, decimal> XapiToGuiTriggerLevel;
        public Func<decimal, decimal> GuiToXapiTriggerLevel;
        public Func<decimal, decimal> XapiToGuiTriggerPeriod;
        public Func<decimal, decimal> GuiToXapiTriggerPeriod;
        public Func<decimal, decimal> XapiToGuiAlertInterval;
        public Func<decimal, decimal> GuiToXapiAlertInterval;

        public PerfmonDefinition AlertDefinition
        {
            get
            {
                Debug.Assert(!string.IsNullOrEmpty(PerfmonDefinitionName) &&
                             GuiToXapiTriggerLevel != null && GuiToXapiTriggerPeriod != null
                             && GuiToXapiAlertInterval != null);

                return new PerfmonDefinition(PerfmonDefinitionName,
                    GuiToXapiTriggerLevel(m_upDownTriggerLevel.Value),
                    GuiToXapiTriggerPeriod(m_upDownTriggerPeriod.Value),
                    GuiToXapiAlertInterval(m_upDownAlertInterval.Value));
            }
        }

        public string SubText
        {
            get
            {
                return m_checkBox.Checked && !string.IsNullOrEmpty(this.SubTextFormat)
                    ? string.Format(this.SubTextFormat, this.m_upDownTriggerLevel.Value, this.m_upDownTriggerPeriod.Value)
                    : null;
            }
        }

        public bool Enabled
        {
            get { return m_checkBox.Checked; }
        }

        public bool HasChanged
        {
            get { return m_upDownTriggerLevel.HasChanged || m_upDownTriggerPeriod.HasChanged || m_checkBox.HasChanged; }
        }

        public void Populate(PerfmonDefinition perfmon)
        {
            Debug.Assert(XapiToGuiTriggerLevel != null && XapiToGuiTriggerPeriod != null && XapiToGuiAlertInterval != null);

            m_checkBox.Checked = true;
            m_upDownTriggerLevel.Value = XapiToGuiTriggerLevel(perfmon.AlarmTriggerLevel);
            m_upDownTriggerPeriod.Value = XapiToGuiTriggerPeriod(perfmon.AlarmTriggerPeriod);
            try
            {
                m_upDownAlertInterval.Value = XapiToGuiAlertInterval(perfmon.AlarmAutoInhibitPeriod);
            }
            catch
            {
                m_upDownAlertInterval.Value = 60;
            }

            StoreOriginalSetting();
        }

        public void Show(bool show)
        {
            m_groupBox.Visible = show;
        }

        public void ToggleAlertGroupEnablement()
        {
            m_upDownTriggerLevel.Enabled = m_checkBox.Checked;
            m_upDownTriggerPeriod.Enabled = m_checkBox.Checked;

            foreach (var label in m_labels)
                label.Enabled = m_checkBox.Checked;

            if (AlertEnablementChanged != null)
                AlertEnablementChanged.Invoke();
        }

        private void StoreOriginalSetting()
        {
            m_upDownTriggerLevel.StoreOriginalSetting();
            m_upDownTriggerPeriod.StoreOriginalSetting();
            m_upDownAlertInterval.StoreOriginalSetting();
            m_checkBox.StoreOriginalSetting();
        }

        private void m_theCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ToggleAlertGroupEnablement();
        }
    }

    public class AlertNumericUpDown : NumericUpDown
    {
        private decimal m_originalValue;

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (Text == "")
                Text = (Value = m_originalValue).ToString();
        }

        public bool HasChanged
        {
            get { return m_originalValue != Value; }
        }

        public void StoreOriginalSetting()
        {
            m_originalValue = Value;
        }
    }

    public class AlertCheckBox : CheckBox
    {
        private bool m_originalValue;

        public bool HasChanged
        {
            get { return m_originalValue != Checked; }
        }

        public void StoreOriginalSetting()
        {
            m_originalValue = Checked;
        }
    }
}
