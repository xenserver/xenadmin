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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.CustomFields;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class CustomFieldsDisplayPage : UserControl, IEditPage
    {
        /// <summary>
        /// This is not ideal, but we need something in the last row of the tableLayoutPanel
        /// that fills all the way to the bottom of the page; if we leave the controls in the
        /// last row, they align vertically in its middle because they don't have vertical
        /// anchoring (because they have different heights and it's a pain to get the vertical
        /// alignment right by fiddling with the margins)
        /// </summary>
        private readonly Panel _panelDummy;
        private IXenObject _xenObject;
        private readonly List<CustomFieldRow> _fieldRows = new List<CustomFieldRow>();

        public CustomFieldsDisplayPage()
        {
            InitializeComponent();
            _panelDummy = new Panel { Size = new Size(0, 0), Margin = new Padding(0) };
            Text = Messages.CUSTOM_FIELDS;
        }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            var customFields = new List<CustomField>();

            foreach (var row in _fieldRows)
            {
                object currentValue = CustomFieldsManager.GetCustomFieldValue(_xenObject, row.CustomFieldDefinition);
                object newValue = row.GetValue();

                if (currentValue == null && newValue == null)
                    continue;

                customFields.Add(new CustomField(row.CustomFieldDefinition, newValue));
            }

            return new SaveCustomFieldsAction(_xenObject, customFields, true);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            CustomFieldsManager.CustomFieldsChanged -= CustomFields_CustomFieldsChanged;
            _xenObject = clone;

            if (_xenObject != null)
            {
                CustomFieldsManager.CustomFieldsChanged += CustomFields_CustomFieldsChanged;
                Rebuild(true);
            }
        }

        public bool ValidToSave => true;

        public void ShowLocalValidationMessages()
        {
        }

        public void HideLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged
        {
            get
            {
                foreach (var row in _fieldRows)
                {
                    object currentValue = CustomFieldsManager.GetCustomFieldValue(_xenObject, row.CustomFieldDefinition);
                    object newValue = row.GetValue();

                    if (currentValue == null && newValue == null)
                        continue;

                    if (currentValue == null || newValue == null)
                        return true;

                    if (newValue.Equals(currentValue))
                        continue;

                    return true;
                }

                return false;
            }
        }

        #endregion

        #region IVerticalTab Members

        public string SubText
        {
            get
            {
                List<string> fields = new List<string>();

                foreach (var row in _fieldRows)
                {
                    object newValue = row.GetValue();

                    if (newValue == null || newValue.ToString() == string.Empty)
                        continue;

                    fields.Add(row.CustomFieldDefinition.Name + Messages.GENERAL_PAGE_KVP_SEPARATOR + newValue);
                }

                if (fields.Count == 0)
                    return Messages.NONE;

                return string.Join(", ", fields);
            }
        }

        public Image Image => Images.StaticImages._000_Fields_h32bit_16;

        #endregion

        private void CustomFields_CustomFieldsChanged()
        {
            Rebuild(false);
        }

        private void Rebuild(bool resetValues)
        {
            var customFieldDefinitions = CustomFieldsManager.GetCustomFields(_xenObject.Connection).ToArray();

            tableLayoutPanel.SuspendLayout();
            tableLayoutPanel.Controls.Remove(_panelDummy);

            // Add new custom fields
            foreach (var definition in customFieldDefinitions)
            {
                object value = CustomFieldsManager.GetCustomFieldValue(_xenObject, definition);
                var row = _fieldRows.FirstOrDefault(r => r.CustomFieldDefinition.Equals(definition));

                if (row == null)
                {
                    row = new CustomFieldRow(definition, value);
                    row.DeleteCustomFieldClicked += DeleteCustomFieldClicked_Click;

                    _fieldRows.Add(row);
                    tableLayoutPanel.Controls.AddRange(row.Controls);

                    if (1 < row.Controls.Length && row.Controls.Length < tableLayoutPanel.ColumnCount)
                        tableLayoutPanel.SetColumnSpan(row.Controls[1], 2); //this is the textbox
                }
                else if (resetValues)
                {
                    row.SetValue(value);
                }
            }

            tableLayoutPanel.Controls.Add(_panelDummy);

            var extraRows = _fieldRows.Where(r => !customFieldDefinitions.Contains(r.CustomFieldDefinition)).ToList();

            foreach (var row in extraRows)
            {
                row.DeleteCustomFieldClicked -= DeleteCustomFieldClicked_Click;
                _fieldRows.Remove(row);

                foreach (var control in row.Controls)
                {
                    tableLayoutPanel.Controls.Remove(control);
                    control.Dispose();
                }
            }

            tableLayoutPanel.ResumeLayout();
        }

        private void buttonNewCustomField_Click(object sender, EventArgs e)
        {
            using (var dialog = new NewCustomFieldDialog(_xenObject.Connection))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    new AddCustomFieldAction(_xenObject.Connection, dialog.Definition).RunAsync();
            }
        }

        private void DeleteCustomFieldClicked_Click(CustomFieldRow row)
        {
            string name = row.CustomFieldDefinition.Name.Ellipsise(50);

            if (!Program.RunInAutomatedTestMode)
            {
                using (var dialog = new WarningDialog(string.Format(Messages.MESSAGEBOX_DELETE_CUSTOM_FIELD, name),
                           ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo))
                {
                    if (dialog.ShowDialog(Program.MainWindow) != DialogResult.Yes)
                        return;
                }
            }

            if (_xenObject.Connection != null && _xenObject.Connection.IsConnected)
            {
                new RemoveCustomFieldAction(_xenObject.Connection, row.CustomFieldDefinition).RunAsync();
            }
            else if (!Program.RunInAutomatedTestMode)
            {
                using (var dlg = new ErrorDialog(Messages.DISCONNECTED_BEFORE_ACTION_STARTED))
                    dlg.ShowDialog(this);
            }
        }


        private class CustomFieldRow
        {
            private readonly TextBox _textBox;
            private readonly DateTimePicker _datePicker;
            private readonly DateTimePicker _timePicker;

            public event Action<CustomFieldRow> DeleteCustomFieldClicked;

            public CustomFieldRow(CustomFieldDefinition definition, object value)
            {
                CustomFieldDefinition = definition;

                var controls = new List<Control>();

                var label = CreateNewLabel(definition);
                controls.Add(label);

                switch (definition.Type)
                {
                    case CustomFieldDefinition.Types.String:
                        _textBox = CreateNewTextBox(value as string);
                        controls.Add(_textBox);
                        break;

                    case CustomFieldDefinition.Types.Date:
                        _datePicker = CreateNewDatePicker(value as DateTime?);
                        _timePicker = CreateNewTimePicker(value as DateTime?);
                        controls.Add(_datePicker);
                        controls.Add(_timePicker);
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(definition));
                }

                var deleteButton = CreateNewDeleteButton();
                controls.Add(deleteButton);
                Controls = controls.ToArray();

                UpdateDateTImePickerState();
            }

            public CustomFieldDefinition CustomFieldDefinition { get; }

            public Control[] Controls { get; }

            public object GetValue()
            {
                if (_textBox != null && !string.IsNullOrEmpty(_textBox.Text))
                    return _textBox.Text;

                if (_datePicker != null && _timePicker != null && _datePicker.Checked)
                {
                    DateTime date = _datePicker.Value;
                    DateTime time = _timePicker.Value;
                    return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                }

                return null;
            }

            public void SetValue(object value)
            {
                if (_textBox != null)
                {
                    _textBox.Text = value as string;
                    return;
                }

                if (_datePicker != null && _timePicker != null)
                {
                    if (value != null)
                    {
                        _datePicker.Checked = true;
                        _datePicker.Value = (DateTime)value;
                        _timePicker.Value = (DateTime)value;
                    }
                    else
                    {
                        _datePicker.Checked = false;
                    }
                }
            }

            private Label CreateNewLabel(CustomFieldDefinition customFieldDefinition)
            {
                return new Label
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    Text = customFieldDefinition.Name.EscapeAmpersands().Ellipsise(25),
                    Font = Program.DefaultFont,
                    AutoSize = true,
                    AutoEllipsis = false
                };
            }

            private TextBox CreateNewTextBox(string text)
            {
                return new TextBox
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    Text = text ?? string.Empty
                };
            }

            private DateTimePicker CreateNewDatePicker(DateTime? value)
            {
                var date = new DateTimePicker
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    MinDate = DateTime.MinValue,
                    MaxDate = DateTime.MaxValue,
                    ShowCheckBox = true,
                    Format = DateTimePickerFormat.Long,
                    Checked = value.HasValue
                };

                if (value.HasValue)
                    date.Value = (DateTime)value;

                date.ValueChanged += Date_ValueChanged;
                return date;
            }

            private DateTimePicker CreateNewTimePicker(DateTime? value)
            {
                var time = new DateTimePicker
                {
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    MinDate = DateTime.MinValue,
                    MaxDate = DateTime.MaxValue,
                    Format = DateTimePickerFormat.Time,
                    ShowUpDown = true,
                    Enabled = value.HasValue
                };

                if (value.HasValue)
                    time.Value = (DateTime)value;

                return time;
            }

            private Button CreateNewDeleteButton()
            {
                var buttonDelete = new Button
                {
                    Anchor = AnchorStyles.Left,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Image = Images.StaticImages._000_Abort_h32bit_16,
                    Size = new Size(22, 22),
                    Text = string.Empty,
                };
                buttonDelete.FlatAppearance.BorderSize = 0;
                buttonDelete.Click += buttonDelete_Click;
                return buttonDelete;
            }

            private void UpdateDateTImePickerState()
            {
                if (_datePicker != null && _timePicker != null)
                    _timePicker.Enabled = _datePicker.Checked;
            }

            private void Date_ValueChanged(object sender, EventArgs e)
            {
                UpdateDateTImePickerState();
            }

            private void buttonDelete_Click(object sender, EventArgs e)
            {
                DeleteCustomFieldClicked?.Invoke(this);
            }
        }
    }
}
