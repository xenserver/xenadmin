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
using System.ComponentModel;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    /// <summary>
    /// Bespoke ComboBoxCell which allows you to provide IEnableableComboBoxItems to it.
    /// If the item is displayed it will be painted grey and will be non-clickable as it 
    /// is a DataGridViewCell hosting a EnableableComboBox
    /// </summary>
    public class DataGridViewEnableableComboBoxCell : DataGridViewComboBoxCell
    {

        public override string ValueMember
        {
            get { return "IEnableableComboBoxItem"; }
        }

        public override string DisplayMember
        {
            get { return "ToStringProperty"; }
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            //Transfer cell value to editing control (EnableableComboBox)
            EnableableComboBoxEditingControl ctl = DataGridView.EditingControl as EnableableComboBoxEditingControl;
            if(ctl != null)
            {
                ctl.SelectedItem = Value ?? DefaultNewRowValue;
            }
        }

        /// <summary>
        /// Type of class that proxies the hosted control
        /// </summary>
        public override Type EditType
        {
            get { return typeof(EnableableComboBoxEditingControl); }
        }

        /// <summary>
        /// Type of object stored by the control behind the selection
        /// </summary>
        public override Type ValueType
        {
            get { return typeof(IEnableableComboBoxItem); }
        }

        public override object DefaultNewRowValue
        {
            get { return null; }
        }

        /// <summary>
        /// Type to display on the UI
        /// </summary>
        public override Type FormattedValueType
        {
            get { return typeof(string); }
        }

        /// <summary>
        /// Parse the data being sent back to the cell from the EnableableComboBox to update the cell
        /// </summary>
        /// <param name="formattedValue"></param>
        /// <param name="cellStyle"></param>
        /// <param name="formattedValueTypeConverter"></param>
        /// <param name="valueTypeConverter"></param>
        /// <returns></returns>
        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle, 
                                                   TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            return formattedValue as IEnableableComboBoxItem;
        }
    }

    /// <summary>
    /// A column to host the bespoke DataGridViewEnableableComboBoxCell in the DataGridView
    /// </summary>
    public class EnableableComboBoxColumn : DataGridViewColumn
    {
        public EnableableComboBoxColumn()
            : base(new DataGridViewEnableableComboBoxCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(DataGridViewEnableableComboBoxCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewEnableableComboBoxCell");
                }
                base.CellTemplate = value;
            }
        }
    }

}
