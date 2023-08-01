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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenCenterLib;


namespace XenAdmin.SettingsPanels
{
    public partial class PerfmonAlertOptionsPage : UserControl, IEditPage
    {
        // match anything with an @ sign in the middle
        private static readonly Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);

        private IXenObject _XenModelObject;

        private bool _origEmailNotificationCheckBox;
        private string _origEmailAddressTextBox;
        private string _origSmtpServerAddrTextBox;
        private string _origSmtpServerPortTextBox;
        private int _origMailLanguageIndex;

        private readonly ToolTip InvalidParamToolTip;

        public PerfmonAlertOptionsPage()
        {
            InitializeComponent();

            Text = Messages.EMAIL_OPTIONS;

            InvalidParamToolTip = new ToolTip
            {
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Warning,
                ToolTipTitle = Messages.INVALID_PARAMETER
            };

            MailLanguageComboBox.Items.Add(new ToStringWrapper<string>(Messages.MAIL_LANGUAGE_ENGLISH_CODE, Messages.MAIL_LANGUAGE_ENGLISH_NAME));
            MailLanguageComboBox.Items.Add(new ToStringWrapper<string>(Messages.MAIL_LANGUAGE_CHINESE_CODE, Messages.MAIL_LANGUAGE_CHINESE_NAME));
            MailLanguageComboBox.Items.Add(new ToStringWrapper<string>(Messages.MAIL_LANGUAGE_JAPANESE_CODE, Messages.MAIL_LANGUAGE_JAPANESE_NAME));
        }

        public string SubText => EmailNotificationCheckBox.Checked ? EmailAddressTextBox.Text : Messages.NONE_DEFINED;

        public Image Image => Images.StaticImages._000_Email_h32bit_16;

        private static bool IsValidEmail(string s)
        {
            return emailRegex.IsMatch(s);
        }

        private bool IsValidSmtpAddress()
        {
            return !SmtpServerAddrTextBox.Text.ToCharArray().Any(c => c >= 128) && SmtpServerAddrTextBox.Text.Trim().Length > 0;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _XenModelObject = clone;

            Populate();

            // Save original settings for change detection
            _origEmailNotificationCheckBox = EmailNotificationCheckBox.Checked;
            _origEmailAddressTextBox = EmailAddressTextBox.Text;
            _origSmtpServerAddrTextBox = SmtpServerAddrTextBox.Text;
            _origSmtpServerPortTextBox = SmtpServerPortTextBox.Text;
            _origMailLanguageIndex = MailLanguageComboBox.SelectedIndex;
        }

        private void Populate()
        {
            var pool = Helpers.GetPoolOfOne(_XenModelObject?.Connection);
            if (pool == null)
                return;

            pool.other_config.TryGetValue(Pool.MAIL_DESTINATION_KEY_NAME, out var mailDestination);
            pool.other_config.TryGetValue(Pool.SMTP_MAILHUB_KEY_NAME, out var mailHub);
            pool.other_config.TryGetValue(Pool.MAIL_LANGUAGE_KEY_NAME, out var mailLanguageCode);

            EmailNotificationCheckBox.Checked = !string.IsNullOrWhiteSpace(mailDestination) && !string.IsNullOrWhiteSpace(mailHub);

            if (!string.IsNullOrWhiteSpace(mailDestination))
                EmailAddressTextBox.Text = mailDestination.Trim();

            if (!string.IsNullOrWhiteSpace(mailHub))
            {
                string[] words = mailHub.Trim().Split(':');

                if (words.Length > 0)
                    SmtpServerAddrTextBox.Text = words[0];
                
                if (words.Length > 1)
                    SmtpServerPortTextBox.Text = words[1];
            }

            bool isLangSupported = Helpers.InvernessOrGreater(pool.Connection);

            MailLanguageLabel.Visible = MailLanguageComboBox.Visible = isLangSupported;

            int index = -1;
            
            if (isLangSupported && !string.IsNullOrWhiteSpace(mailLanguageCode))
                index = IndexOfLangItem(mailLanguageCode.Trim());

            if (index == -1)
                index = IndexOfLangItem(BrandManager.PerfAlertMailDefaultLanguage);

            if (index == -1)
                index = 0;

            MailLanguageComboBox.SelectedIndex = index;
        }

        private int IndexOfLangItem(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return -1;

            for (var index = 0; index < MailLanguageComboBox.Items.Count; index++)
            {
                var obj = MailLanguageComboBox.Items[index];
                if (obj is ToStringWrapper<string> wrapper && wrapper.item.ToLower() == code.ToLower())
                    return index;
            }

            return -1;
        }

        public bool HasChanged =>
            _origEmailNotificationCheckBox != EmailNotificationCheckBox.Checked ||
            _origEmailAddressTextBox != EmailAddressTextBox.Text ||
            _origSmtpServerAddrTextBox != SmtpServerAddrTextBox.Text ||
            _origSmtpServerPortTextBox != SmtpServerPortTextBox.Text ||
            _origMailLanguageIndex != MailLanguageComboBox.SelectedIndex;

        public void ShowLocalValidationMessages()
        {
            if (!IsValidEmail(EmailAddressTextBox.Text))
            {
                HelpersGUI.ShowBalloonMessage(EmailAddressTextBox, InvalidParamToolTip, Messages.INVALID_PARAMETER);
            }
            else if (!IsValidSmtpAddress())
            {
                HelpersGUI.ShowBalloonMessage(SmtpServerAddrTextBox, InvalidParamToolTip, Messages.INVALID_PARAMETER);
            }
            else if (!Util.IsValidPort(SmtpServerPortTextBox.Text))
            {
                HelpersGUI.ShowBalloonMessage(SmtpServerPortTextBox, InvalidParamToolTip, Messages.INVALID_PARAMETER);
            }
        }

        public void HideLocalValidationMessages()
        {
            if (EmailAddressTextBox != null)
            {
                InvalidParamToolTip.Hide(EmailAddressTextBox);
            }
            if (SmtpServerAddrTextBox != null)
            {
                InvalidParamToolTip.Hide(SmtpServerAddrTextBox);
            }
            if (SmtpServerPortTextBox != null)
            {
                InvalidParamToolTip.Hide(SmtpServerPortTextBox);
            }
        }

        public bool ValidToSave =>
            !EmailNotificationCheckBox.Checked ||
            IsValidEmail(EmailAddressTextBox.Text) && Util.IsValidPort(SmtpServerPortTextBox.Text) && IsValidSmtpAddress();

        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
        }

        public AsyncAction SaveSettings()
        {
            // a null value will clear the definitions
            string mailDestination = null;
            string mailHub = null;
            string mailLangCode = null;

            if (EmailNotificationCheckBox.Checked)
            {
                mailDestination = EmailAddressTextBox.Text;
                mailHub = SmtpServerAddrTextBox.Text + ":" + SmtpServerPortTextBox.Text;

                if (MailLanguageComboBox.Visible && MailLanguageComboBox.SelectedItem is ToStringWrapper<string> stringWrapper && !string.IsNullOrEmpty(stringWrapper.item))
                    mailLangCode = stringWrapper.item;
            }

            return new PerfmonOptionsDefinitionAction(_XenModelObject.Connection, mailDestination, mailHub, mailLangCode, true);
        }
    }
}
