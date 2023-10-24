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

namespace XenAdmin.SettingsPanels
{
    public partial class NRPEEditPage
    {
        public class CheckGroup
        {
            private const decimal THRESHOLD_MINIMUM = 0.01M;
            private const decimal THRESHOLD_MAXIMUM = 100M;

            public string Name { get; }

            public DataGridViewRow CheckThresholdRow { get; }

            public DataGridViewTextBoxCell NameCell { get; }

            public DataGridViewTextBoxCell WarningThresholdCell { get; }

            public DataGridViewTextBoxCell CriticalThresholdCell { get; }

            public CheckGroup(string name, string labelName)
            {
                Name = name;
                NameCell = new DataGridViewTextBoxCell { Value = labelName };
                WarningThresholdCell = new DataGridViewTextBoxCell { Value = "" };
                CriticalThresholdCell = new DataGridViewTextBoxCell { Value = "" };
                CheckThresholdRow = new DataGridViewRow();
                CheckThresholdRow.Cells.AddRange(NameCell, WarningThresholdCell, CriticalThresholdCell);
                CheckThresholdRow.DefaultCellStyle.Format = "N2";
                CheckThresholdRow.DefaultCellStyle.NullValue = 0;
                WarningThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
                CriticalThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
            }

            public void UpdateThreshold(string warningThreshold, string criticalThreshold)
            {
                WarningThresholdCell.Value = warningThreshold;
                CriticalThresholdCell.Value = criticalThreshold;
                WarningThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                CriticalThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            }

            public virtual bool CheckValue()
            {
                WarningThresholdCell.ErrorText = "";
                CriticalThresholdCell.ErrorText = "";

                return CheckEachValue(WarningThresholdCell) &&
                       CheckEachValue(CriticalThresholdCell) &&
                       CompareWarningAndCritical();
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
                    cell.ErrorText = string.Format(Messages.NRPE_THRESHOLD_RANGE_ERROR, THRESHOLD_MINIMUM,
                        THRESHOLD_MAXIMUM);
                    return false;
                }

                cell.ErrorText = "";
                return true;
            }

            protected virtual bool CompareWarningAndCritical()
            {
                decimal.TryParse(WarningThresholdCell.Value.ToString().Trim(), out decimal warningDecimal);
                decimal.TryParse(CriticalThresholdCell.Value.ToString().Trim(), out decimal criticalDecimal);
                if (warningDecimal < criticalDecimal)
                {
                    WarningThresholdCell.ErrorText = "";
                    return true;
                }
                else
                {
                    WarningThresholdCell.ErrorText =
                        string.Format(Messages.NRPE_THRESHOLD_WARNING_SHOULD_LESS_THAN_CRITICAL);
                    return false;
                }
            }
        }

        public class FreeCheckGroup : CheckGroup
        {
            public FreeCheckGroup(string name, string labelName)
                : base(name, labelName)
            {
            }

            protected override bool CompareWarningAndCritical()
            {
                decimal.TryParse(WarningThresholdCell.Value.ToString().Trim(), out decimal warningDecimal);
                decimal.TryParse(CriticalThresholdCell.Value.ToString().Trim(), out decimal criticalDecimal);
                if (warningDecimal > criticalDecimal)
                {
                    WarningThresholdCell.ErrorText = "";
                    return true;
                }
                else
                {
                    WarningThresholdCell.ErrorText =
                        string.Format(Messages.NRPE_THRESHOLD_WARNING_SHOULD_BIGGER_THAN_CRITICAL);
                    return false;
                }
            }

        }

        public class HostLoadCheckGroup : CheckGroup
        {
            public HostLoadCheckGroup(string name, string labelName)
                : base(name, labelName)
            {
            }
        }

        public class Dom0LoadCheckGroup : CheckGroup
        {
            public Dom0LoadCheckGroup(string name, string labelName)
                : base(name, labelName)
            {
            }

            protected override bool CompareWarningAndCritical()
            {
                string[] warningArray = WarningThresholdCell.Value.ToString().Split(',');
                string[] criticalArray = CriticalThresholdCell.Value.ToString().Split(',');
                for (int i = 0; i < 3; i++)
                {
                    decimal.TryParse(warningArray[i].Trim(), out decimal warningDecimal);
                    decimal.TryParse(criticalArray[i].Trim(), out decimal criticalDecimal);
                    if (warningDecimal > criticalDecimal)
                    {
                        WarningThresholdCell.ErrorText =
                            string.Format(Messages.NRPE_THRESHOLD_WARNING_SHOULD_LESS_THAN_CRITICAL);
                        return false;
                    }
                }

                WarningThresholdCell.ErrorText = "";
                return true;
            }

            protected override bool CheckEachValue(DataGridViewTextBoxCell cell)
            {
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
}
