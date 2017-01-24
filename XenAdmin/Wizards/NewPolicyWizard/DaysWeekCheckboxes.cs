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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class DaysWeekCheckboxes : UserControl
    {
        public DaysWeekCheckboxes()
        {
            InitializeComponent();
            checkBoxMonday.CheckedChanged += new EventHandler(checkBoxMonday_CheckedChanged);
            checkBoxTuesday.CheckedChanged += new EventHandler(checkBoxMonday_CheckedChanged);
            checkBoxWednesday.CheckedChanged += new EventHandler(checkBoxMonday_CheckedChanged);
            checkBoxThursday.CheckedChanged += new EventHandler(checkBoxMonday_CheckedChanged);
            checkBoxFriday.CheckedChanged += new EventHandler(checkBoxMonday_CheckedChanged);
            checkBoxSaturday.CheckedChanged += new EventHandler(checkBoxMonday_CheckedChanged);
            checkBoxSunday.CheckedChanged += new EventHandler(checkBoxMonday_CheckedChanged);
        }

        void checkBoxMonday_CheckedChanged(object sender, EventArgs e)
        {
            EventHandler handler = CheckBoxChanged;
            if (handler != null) handler(sender, e);
        }


        public event EventHandler CheckBoxChanged;

        public enum DaysMode { ENGLISH, L10N_SHORT, L10N_LONG };
        private string DaysToString(DaysMode mode)
        {
            var sb = new StringBuilder();
            if (checkBoxMonday.Checked)
                sb.Append((mode == DaysMode.ENGLISH ? "Monday" : mode == DaysMode.L10N_LONG ? Messages.MONDAY_LONG : Messages.MONDAY_SHORT) + ",");
            if (checkBoxTuesday.Checked)
                sb.Append((mode == DaysMode.ENGLISH ? "Tuesday" : mode == DaysMode.L10N_LONG ? Messages.TUESDAY_LONG : Messages.TUESDAY_SHORT) + ",");
            if (checkBoxWednesday.Checked)
                sb.Append((mode == DaysMode.ENGLISH ? "Wednesday" : mode == DaysMode.L10N_LONG ? Messages.WEDNESDAY_LONG : Messages.WEDNESDAY_SHORT) + ",");
            if (checkBoxThursday.Checked)
                sb.Append((mode == DaysMode.ENGLISH ? "Thursday" : mode == DaysMode.L10N_LONG ? Messages.THURSDAY_LONG : Messages.THURSDAY_SHORT) + ",");
            if (checkBoxFriday.Checked)
                sb.Append((mode == DaysMode.ENGLISH ? "Friday" : mode == DaysMode.L10N_LONG ? Messages.FRIDAY_LONG : Messages.FRIDAY_SHORT) + ",");
            if (checkBoxSaturday.Checked)
                sb.Append((mode == DaysMode.ENGLISH ? "Saturday" : mode == DaysMode.L10N_LONG ? Messages.SATURDAY_LONG : Messages.SATURDAY_SHORT) + ",");
            if (checkBoxSunday.Checked)
                sb.Append((mode == DaysMode.ENGLISH ? "Sunday" : mode == DaysMode.L10N_LONG ? Messages.SUNDAY_LONG : Messages.SUNDAY_SHORT) + ",");
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public string Days
        {
            get
            {
                return DaysToString(DaysMode.ENGLISH);
            }
            set
            {
                if (value != null)
                {
                    checkBoxTuesday.Checked = value.IndexOf("tuesday", StringComparison.InvariantCultureIgnoreCase) >= 0;
                    checkBoxWednesday.Checked = value.IndexOf("wednesday", StringComparison.InvariantCultureIgnoreCase) >= 0;
                    checkBoxThursday.Checked = value.IndexOf("thursday", StringComparison.InvariantCultureIgnoreCase) >= 0;
                    checkBoxFriday.Checked = value.IndexOf("friday", StringComparison.InvariantCultureIgnoreCase) >= 0;
                    checkBoxSaturday.Checked = value.IndexOf("saturday", StringComparison.InvariantCultureIgnoreCase) >= 0;
                    checkBoxSunday.Checked = value.IndexOf("sunday", StringComparison.InvariantCultureIgnoreCase) >= 0;
                    checkBoxMonday.Checked = value.IndexOf("monday", StringComparison.InvariantCultureIgnoreCase) >= 0;
                }
            }
        }

        // Localise a comma-separated days string. I don't think this is the right way to do it. Really
        // the data structures should keep the days as an array of bools or list of ints or something and
        // only translate on input and output, but that would require too much rewriting now. (CA-51612)
        public static string L10NDays(string days, DaysMode mode)
        {
            DaysWeekCheckboxes dwc = new DaysWeekCheckboxes();
            dwc.Days = days;
            return dwc.DaysToString(mode);
        }

        public void DisableUnSelected()
        {
            checkBoxTuesday.Enabled = checkBoxTuesday.Checked;
            checkBoxWednesday.Enabled = checkBoxWednesday.Checked;
            checkBoxThursday.Enabled = checkBoxThursday.Checked;
            checkBoxFriday.Enabled = checkBoxFriday.Checked;
            checkBoxSaturday.Enabled = checkBoxSaturday.Checked;
            checkBoxSunday.Enabled = checkBoxSunday.Checked;
            checkBoxMonday.Enabled = checkBoxMonday.Checked;
        }

        public void EnableAll()
        {
            checkBoxTuesday.Enabled =
            checkBoxWednesday.Enabled = 
            checkBoxThursday.Enabled = 
            checkBoxFriday.Enabled = 
            checkBoxSaturday.Enabled = 
            checkBoxSunday.Enabled = 
            checkBoxMonday.Enabled = true;
        }

    }
}
