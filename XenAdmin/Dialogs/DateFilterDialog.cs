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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XenAdmin.Dialogs
{
    public partial class DateFilterDialog : XenDialogBase
    {
        /* 24 hours ago in the choice list literally means Now - 24 hours. All the rest are taken to mean the beginning/end of the day
         * x days ago for the start and end date entry respectively.
         * 
         * This makes having the same option selected for both the start and end date a valid choice for the 'x days ago' entries. It
         * will return all the alerts that happened over the whole day.
         * 
         * Combo Choices:
         * 
         * Now/All time
         * 24 hours ago
         * 3 days ago
         * 7 days ago
         * 30 days ago
         * Custom
         */

        private bool startDateSet = false;
        public bool StartDateSet { get { return startDateSet; } }

        private DateTime startDate = DateTime.MinValue;
        public DateTime StartDate { get { return startDate; } }

        private bool endDateSet = false;
        public bool EndDateSet { get { return endDateSet; } }

        private DateTime endDate = DateTime.MaxValue;
        public DateTime EndDate { get { return endDate; } }

        private const int NO_FILTER_INDEX = 0;
        private const int TWENTY_FOUR_HOURS_INDEX = 1;
        private const int THREE_DAYS_INDEX = 2;
        private const int SEVEN_DAYS_INDEX = 3;
        private const int THIRTY_DAYS_INDEX = 4;
        private const int CUSTOM_INDEX = 5;

        // Setting these as our min max values allows them to be displayed in the date pickers without looking too odd
        private readonly DateTime END_DATE_NOT_SET_VALUE = new DateTime(3000,1,1);
        private readonly DateTime START_DATE_NOT_SET_VALUE = new DateTime(2000,1,1);

        public DateFilterDialog()
        {
            InitializeComponent();
            ComboStartDate.SelectedIndex = NO_FILTER_INDEX;
            ComboEndDate.SelectedIndex = NO_FILTER_INDEX;
        }

        private void startDateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboStartDate.SelectedIndex == CUSTOM_INDEX)
            {
                DatePickerStartTime.Enabled = DatePickerStartDate.Enabled = true;
                DatePickerStartTime.Value = DateTime.Now;
                DatePickerStartDate.Value = DateTime.Now;
            }
            else
            {
                DatePickerStartTime.Enabled = DatePickerStartDate.Enabled = false;
            }
             
            validateInterval();
        }

        private void endDateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboEndDate.SelectedIndex == CUSTOM_INDEX)
            {
                DatePickerEndTime.Enabled = DatePickerEndDate.Enabled = true;
                DatePickerEndTime.Value = DateTime.Now;
                DatePickerEndDate.Value = DateTime.Now;
            }
            else
            {
                DatePickerEndTime.Enabled = DatePickerEndDate.Enabled = false;
            }
            validateInterval();
        }

        private void RefreshPickersAndCombo(object sender, EventArgs e)
        {
            DatePickerEndTime.Enabled = DatePickerEndDate.Enabled = ComboEndDate.SelectedIndex == CUSTOM_INDEX;
            DatePickerStartTime.Enabled = DatePickerStartDate.Enabled = ComboStartDate.SelectedIndex == CUSTOM_INDEX;
            validateInterval();
        }

        private void DatePickersChanged(object sender, EventArgs e)
        {
            validateInterval();
        }

        private void validateInterval()
        {
            // We dont set the actual endDate/startDate variables until the user clicks the apply button
            // But we do validate the current choice so we can display warnings if it is bogus

            DateTime parsedEndDate = parseEndDate();
            DateTime parsedStartDate = parseStartDate();

            bool valid = parsedStartDate < parsedEndDate;
            tableLayoutPanelWarning.Visible = !valid;
            buttonApplyFilter.Enabled = valid;

            // If a preset choice update the datepickers to reflect the choice
            if (ComboStartDate.SelectedIndex != CUSTOM_INDEX)
            {
                DatePickerStartDate.Value = parsedStartDate;
                DatePickerStartTime.Value = parsedStartDate;
            }
            if (ComboEndDate.SelectedIndex != CUSTOM_INDEX)
            {
                DatePickerEndDate.Value = parsedEndDate;
                DatePickerEndTime.Value = parsedEndDate;
            }
        }
     
        private void buttonApplyFilter_Click(object sender, EventArgs e)
        {
            startDate = parseStartDate();
            endDate = parseEndDate();
            startDateSet = ComboStartDate.SelectedIndex != NO_FILTER_INDEX;
            endDateSet = ComboEndDate.SelectedIndex != NO_FILTER_INDEX;
            DialogResult = DialogResult.OK;
            Close();
        }

        private DateTime parseEndDate()
        {

            DateTime d = END_DATE_NOT_SET_VALUE;
            switch (ComboEndDate.SelectedIndex)
            {
                case NO_FILTER_INDEX: 
                    break;
                case TWENTY_FOUR_HOURS_INDEX: 
                    d = DateTime.Now.AddDays(-1); 
                    break;
                case THREE_DAYS_INDEX: 
                    d = EndOfDay(DateTime.Now.AddDays(-3)); 
                    break;
                case SEVEN_DAYS_INDEX:
                    d = EndOfDay(DateTime.Now.AddDays(-7)); 
                    break;
                case THIRTY_DAYS_INDEX: 
                    d = EndOfDay(DateTime.Now.AddDays(-30)); 
                    break;
                case CUSTOM_INDEX: 
                    d = new DateTime(
                                DatePickerEndDate.Value.Year,
                                DatePickerEndDate.Value.Month,
                                DatePickerEndDate.Value.Day,
                                DatePickerEndTime.Value.Hour,
                                DatePickerEndTime.Value.Minute,
                                DatePickerEndTime.Value.Second); 
                    break;
            }
            return d;
        }

        private DateTime parseStartDate()
        {
            DateTime d = START_DATE_NOT_SET_VALUE;
            switch (ComboStartDate.SelectedIndex)
            {
                case NO_FILTER_INDEX:
                    break;
                case TWENTY_FOUR_HOURS_INDEX: 
                    d = DateTime.Now.AddDays(-1); 
                    break;
                case THREE_DAYS_INDEX: 
                    d = BegginningOfDay(DateTime.Now.AddDays(-3)); 
                    break;
                case SEVEN_DAYS_INDEX: 
                    d = BegginningOfDay(DateTime.Now.AddDays(-7)); 
                    break;
                case THIRTY_DAYS_INDEX: 
                    d = BegginningOfDay(DateTime.Now.AddDays(-30)); 
                    break;
                case CUSTOM_INDEX: 
                    d = new DateTime(
                                DatePickerStartDate.Value.Year,
                                DatePickerStartDate.Value.Month,
                                DatePickerStartDate.Value.Day,
                                DatePickerStartTime.Value.Hour,
                                DatePickerStartTime.Value.Minute,
                                DatePickerStartTime.Value.Second); 
                    break;
            }
            return d;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private DateTime BegginningOfDay(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        private DateTime EndOfDay(DateTime dt)
        {
            return (new DateTime(dt.Year, dt.Month, dt.Day)).AddDays(1);
        }


#region external methods
        // These methods are available to allow other controls to adjust the filters stored in the dialog to various presets.
        
        public void SetNone()
        {
            endDateSet = false;
            endDate = END_DATE_NOT_SET_VALUE;
            ComboEndDate.SelectedIndex = NO_FILTER_INDEX;

            startDateSet = false;
            startDate = START_DATE_NOT_SET_VALUE;
            ComboStartDate.SelectedIndex = NO_FILTER_INDEX;
        }

        public void Set24Hours()
        {
            endDateSet = false;
            endDate = END_DATE_NOT_SET_VALUE;
            ComboEndDate.SelectedIndex = NO_FILTER_INDEX;

            startDateSet = true;
            startDate = DateTime.Now.AddDays(-1);
            ComboStartDate.SelectedIndex = TWENTY_FOUR_HOURS_INDEX;
            
        }

        /// <summary>
        /// This enum reflects the different pre set options that are available in this dialog
        /// </summary>
        public enum DaysInPastOptions { THREE_DAYS, SEVEN_DAYS, THIRTY_DAYS};
        public void SetDays(DaysInPastOptions DaysInPast)
        {
            endDateSet = false;
            endDate = END_DATE_NOT_SET_VALUE;
            ComboEndDate.SelectedIndex = NO_FILTER_INDEX;

            startDateSet = true;
            switch (DaysInPast)
            {
                case DaysInPastOptions.THREE_DAYS: 
                    ComboStartDate.SelectedIndex = THREE_DAYS_INDEX;
                    startDate = BegginningOfDay(DateTime.Now.AddDays(-3));
                    break;
                case DaysInPastOptions.SEVEN_DAYS:
                    ComboStartDate.SelectedIndex = SEVEN_DAYS_INDEX;
                    startDate = BegginningOfDay(DateTime.Now.AddDays(-7));
                    break;
                case DaysInPastOptions.THIRTY_DAYS:
                    ComboStartDate.SelectedIndex = THIRTY_DAYS_INDEX;
                    startDate = BegginningOfDay(DateTime.Now.AddDays(-30));
                    break;
            }
        }

#endregion
    }
}