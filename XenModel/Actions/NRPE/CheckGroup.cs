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

using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Actions.NRPE
{
    public class CheckGroup
    {
        private static readonly string DEFAULT_CHECK_WARNING_THRESHOLD = "80";
        private static readonly string DEFAULT_CHECK_CRITICAL_THRESHOLD = "90";

        private readonly decimal THRESHOLD_MINIMUM = 0.01M;
        private readonly decimal THRESHOLD_MAXIMUM = 100M;

        private string name;
        private string warningThresholdDefault;
        private string criticalThresholdDefault;
        private bool changed;

        protected DataGridViewRow checkThresholdRow;
        protected DataGridViewTextBoxCell nameCell;
        protected DataGridViewTextBoxCell warningThresholdCell;
        protected DataGridViewTextBoxCell criticalThresholdCell;

        public string Name { get => name; set => name = value; }
        public string WarningThresholdDefault { get => warningThresholdDefault; set => warningThresholdDefault = value; }
        public string CriticalThresholdDefault { get => criticalThresholdDefault; set => criticalThresholdDefault = value; }
        public bool Changed { get => changed; set => changed = value; }
        public DataGridViewRow CheckThresholdRow { get => checkThresholdRow; set => checkThresholdRow = value; }
        public DataGridViewTextBoxCell NameCell { get => nameCell; set => nameCell = value; }
        public DataGridViewTextBoxCell WarningThresholdCell { get => warningThresholdCell; set => warningThresholdCell = value; }
        public DataGridViewTextBoxCell CriticalThresholdCell { get => criticalThresholdCell; set => criticalThresholdCell = value; }

        public CheckGroup(string name, string labelName, string warningThresholdDefaultValue, string criticalThresholdDefaultValue)
        {
            InitCheckGroup(name, labelName, warningThresholdDefaultValue, criticalThresholdDefaultValue);
        }

        public CheckGroup(string name, string labelName)
        {
            InitCheckGroup(name, labelName, DEFAULT_CHECK_WARNING_THRESHOLD, DEFAULT_CHECK_CRITICAL_THRESHOLD);
        }

        private void InitCheckGroup(string name, string labelName, string warningThresholdDefaultValue, string criticalThresholdDefaultValue)
        {
            Name = name;
            nameCell = new DataGridViewTextBoxCell { Value = labelName };
            warningThresholdDefault = warningThresholdDefaultValue;
            criticalThresholdDefault = criticalThresholdDefaultValue;
            warningThresholdCell = new DataGridViewTextBoxCell { Value = warningThresholdDefaultValue };
            criticalThresholdCell = new DataGridViewTextBoxCell { Value = criticalThresholdDefaultValue };
            checkThresholdRow = new DataGridViewRow();
            checkThresholdRow.Cells.AddRange(nameCell, warningThresholdCell, criticalThresholdCell);
            checkThresholdRow.DefaultCellStyle.Format = "N2";
            checkThresholdRow.DefaultCellStyle.NullValue = 0;
            warningThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
            criticalThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
        }

        public void UpdateThreshold(string warningThreshold, string criticalThreshold)
        {
            warningThresholdCell.Value = warningThreshold;
            criticalThresholdCell.Value = criticalThreshold;
            warningThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            criticalThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        public virtual bool CheckValue()
        {
            warningThresholdCell.ErrorText = "";
            criticalThresholdCell.ErrorText = "";

            if (IsEmptyForPool())
            {
                return true;
            }

            if (CheckEachValue(warningThresholdCell) &&
                CheckEachValue(criticalThresholdCell) &&
                CompareWarningAndCritical() &&
                CheckModifyAllForPool())
            {
                return true;
            }
            return false;
        }

        private bool IsEmptyForPool()
        {
            return warningThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlDark))
                && criticalThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlDark));
        }

        protected virtual bool CheckEachValue(DataGridViewTextBoxCell cell)
        {
            string thresholdStr = cell.Value.ToString().Trim();
            if (thresholdStr.Equals(""))
            {
                cell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_SHOULD_NOT_BE_EMPTY);
                return false;
            }
            if (!decimal.TryParse(thresholdStr, out decimal threshold))
            {
                cell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_SHOULD_BE_NUMBER);
                return false;
            }
            if (threshold < THRESHOLD_MINIMUM || threshold > THRESHOLD_MAXIMUM)
            {
                cell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_RANGE_ERROR, THRESHOLD_MINIMUM, THRESHOLD_MAXIMUM);
                return false;
            }
            cell.ErrorText = "";
            return true;
        }

        protected virtual bool CompareWarningAndCritical()
        {
            decimal.TryParse(warningThresholdCell.Value.ToString().Trim(), out decimal warningDecimal);
            decimal.TryParse(criticalThresholdCell.Value.ToString().Trim(), out decimal criticalDecimal);
            if (warningDecimal < criticalDecimal)
            {
                warningThresholdCell.ErrorText = "";
                return true;
            }
            else
            {
                warningThresholdCell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_WARNING_SHOULD_LESS_THAN_CRITICAL);
                return false;
            }
        }

        private bool CheckModifyAllForPool()
        {
            if (warningThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlText))
                && criticalThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlDark)))
            {
                criticalThresholdCell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_SHOULD_NOT_BE_EMPTY);
                return false;
            }
            else if (warningThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlDark))
                && criticalThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlText)))
            {
                warningThresholdCell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_SHOULD_NOT_BE_EMPTY);
                return false;
            }
            return true;
        }
    }

    public class FreeCheckGroup : CheckGroup
    {
        private static readonly string DEFAULT_CHECK_WARNING_THRESHOLD = "20";
        private static readonly string DEFAULT_CHECK_CRITICAL_THRESHOLD = "10";

        public FreeCheckGroup(string name, string labelName)
            : base(name, labelName, DEFAULT_CHECK_WARNING_THRESHOLD, DEFAULT_CHECK_CRITICAL_THRESHOLD)
        {
        }

        protected override bool CompareWarningAndCritical()
        {
            decimal.TryParse(warningThresholdCell.Value.ToString().Trim(), out decimal warningDecimal);
            decimal.TryParse(criticalThresholdCell.Value.ToString().Trim(), out decimal criticalDecimal);
            if (warningDecimal > criticalDecimal)
            {
                warningThresholdCell.ErrorText = "";
                return true;
            }
            else
            {
                warningThresholdCell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_WARNING_SHOULD_BIGGER_THAN_CRITICAL);
                return false;
            }
        }

    }

    public class HostLoadCheckGroup : CheckGroup
    {
        private static readonly string DEFAULT_CHECK_WARNING_THRESHOLD = "3";
        private static readonly string DEFAULT_CHECK_CRITICAL_THRESHOLD = "4";

        public HostLoadCheckGroup(string name, string labelName)
            : base(name, labelName, DEFAULT_CHECK_WARNING_THRESHOLD, DEFAULT_CHECK_CRITICAL_THRESHOLD)
        {
        }
    }

    public class Dom0LoadCheckGroup : CheckGroup
    {
        private static readonly string DEFAULT_CHECK_WARNING_THRESHOLD = "2.7,2.6,2.5";
        private static readonly string DEFAULT_CHECK_CRITICAL_THRESHOLD = "3.2,3.1,3";

        public Dom0LoadCheckGroup(string name, string labelName)
            : base(name, labelName, DEFAULT_CHECK_WARNING_THRESHOLD, DEFAULT_CHECK_CRITICAL_THRESHOLD)
        {
        }

        protected override bool CompareWarningAndCritical()
        {
            string[] warningArray = warningThresholdCell.Value.ToString().Split(',');
            string[] criticalArray = criticalThresholdCell.Value.ToString().Split(',');
            for (int i = 0; i < 3; i++)
            {
                decimal.TryParse(warningArray[i].Trim(), out decimal warningDecimal);
                decimal.TryParse(criticalArray[i].Trim(), out decimal criticalDecimal);
                if (warningDecimal > criticalDecimal)
                {
                    warningThresholdCell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_WARNING_SHOULD_LESS_THAN_CRITICAL);
                    return false;
                }
            }
            warningThresholdCell.ErrorText = "";
            return true;
        }

        protected override bool CheckEachValue(DataGridViewTextBoxCell cell)
        {
            checkThresholdRow.DataGridView.ShowCellToolTips = true;
            cell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_SHOULD_BE_3_NUMBERS);
            string[] loadArray = cell.Value.ToString().Split(',');
            if (loadArray.Length != 3)
            {
                return false;
            }
            foreach (string load in loadArray)
            {
                bool isDecimal = decimal.TryParse(load, out _);
                if (!isDecimal)
                {
                    return false;
                }
            }
            cell.ErrorText = "";
            return true;
        }
    }
}
