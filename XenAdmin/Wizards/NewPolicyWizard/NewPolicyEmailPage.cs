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
using System.Drawing;
using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.SettingsPanels;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicyEmailPage : XenTabPage, IEditPage
    {
        readonly EmailAddressValidator emailAddressValidator = new EmailAddressValidator();

        public NewPolicyEmailPage()
        {
            InitializeComponent();
            textBoxPort.Text = 25.ToString();
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            textBoxEmailAddress.LostFocus += UpdatePageAndEnableOK_EventHandler;
            textBoxEmailAddress.TextChanged += UpdatePageAndEnableOKWhileInError_EventHandler;

            textBoxPort.TextChanged += UpdatePageAndEnableOK_EventHandler;
            textBoxSMTP.TextChanged += UpdatePageAndEnableOK_EventHandler;
        }

        private Pool _pool;
        public Pool Pool
        {
            get { return _pool; }
            set
            {
                _pool = value;
                if (value != null)
                {
                    var options = PerfmonOptionsDefinition.GetPerfmonOptionsDefinitions(_pool);
                    if (options != null)
                    {
                        textBoxSMTP.Text = PerfmonOptionsDefinition.GetSmtpServerAddress(options.MailHub);
                        textBoxPort.Text = PerfmonOptionsDefinition.GetSmtpPort(options.MailHub);
                        textBoxEmailAddress.Text = options.MailDestination;
                    }
                }
            }
        }


        public override string Text
        {
            get { return Messages.EMAIL_ALERTS; }
        }

        public string SubText
        {
            get { return EmailEnabled ? Messages.ENABLED : Messages.DISABLED; }
        }

        public override string HelpID
        {
            get { return "Emailalerts"; }
        }

        public Image Image
        {
            get { return Properties.Resources._000_Email_h32bit_16; }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.EMAIL_ALERTS_TITLE;
            }
        }

        public Dictionary<string, string> EmailSettings
        {
            get
            {
                var result = new Dictionary<string, string>();
                if (checkBox1.Checked)
                {
                    result.Add("email_address", textBoxEmailAddress.Text);
                    result.Add("smtp_server", textBoxSMTP.Text);
                    result.Add("smtp_port", textBoxPort.Text);
                }
                return result;
            }
        }

        public bool EmailEnabled
        {
            get { return checkBox1.Checked; }
        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            groupBox1.Enabled = checkBox1.Checked;
            OnPageUpdated();
            EnableOkButton();
        }

        private void RefreshTab(VMPP vmpp)
        {
            checkBox1.Checked = vmpp.is_alarm_enabled;
            if (vmpp.is_alarm_enabled)
            {
                textBoxSMTP.Text = vmpp.alarm_config_smtp_server;
                textBoxPort.Text = vmpp.alarm_config_smtp_port;
                textBoxEmailAddress.Text = vmpp.alarm_config_email_address;
            }
        }


        private void EnableOkButton()
        {
            if (PropertiesDialog != null)
                PropertiesDialog.okButton.Enabled = EnableNext();
        }

        public PropertiesDialog PropertiesDialog { private get; set; }

        public AsyncAction SaveSettings()
        {
            _clone.is_alarm_enabled = EmailEnabled;
            _clone.alarm_config = EmailSettings;
            return null;
        }

        private VMPP _clone;
        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _clone = (VMPP)clone;
            Pool = Helpers.GetPoolOfOne(_clone.Connection);
            RefreshTab(_clone);
        }

        /// <summary>
        /// Set the visibilty of the error warning message panel (picture and text)
        /// </summary>
        private bool ErrorWarningMessageIsVisible
        {
            set { errorPanel.Visible = value; }
            get { return errorPanel.Visible; }
        }

        public override bool EnableNext()
        {

            ErrorWarningMessageIsVisible = false;

            //Display the email error warning?
            

            if (checkBox1.Checked && !string.IsNullOrEmpty(textBoxEmailAddress.Text) &&
                !emailAddressValidator.IsValid(textBoxEmailAddress.Text))
            {
                ErrorWarningMessageIsVisible = true;
                return false;
            }

            //Check all required boxes have text
            if (checkBox1.Checked &&
                (string.IsNullOrEmpty(textBoxEmailAddress.Text) || string.IsNullOrEmpty(textBoxSMTP.Text) || string.IsNullOrEmpty(textBoxPort.Text)))
                return false;

            return true;
   
        }

        public bool ValidToSave
        {
            get { return EnableNext(); }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
            checkBox1.Checked = false;
        }

        public bool HasChanged
        {
            get
            {
                if (EmailEnabled != _clone.is_alarm_enabled)
                    return true;
                if (!Helper.AreEqual2(_clone.alarm_config, EmailSettings))
                    return true;
                return false;
            }
        }

        private void UpdatePageAndEnableOK_EventHandler(object sender, EventArgs e)
        {
            UpdatePageAndEnableOK();
        }

        private void UpdatePageAndEnableOKWhileInError_EventHandler(object sender, EventArgs e)
        {
            if (!ErrorWarningMessageIsVisible)
                return;

            UpdatePageAndEnableOK();
        }

        private void UpdatePageAndEnableOK()
        {
            OnPageUpdated();
            EnableOkButton();
        }
    }

    
}
