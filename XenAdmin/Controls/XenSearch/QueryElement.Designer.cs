using System;

namespace XenAdmin.Controls.XenSearch
{
    partial class QueryElement
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryElement));
            this.textBox = new System.Windows.Forms.TextBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.unitsLabel = new System.Windows.Forms.Label();
            this.ComboButton = new XenAdmin.Controls.XenSearch.DropDownComboButton();
            this.matchTypeComboButton = new XenAdmin.Controls.XenSearch.DropDownComboButton();
            this.queryTypeComboButton = new XenAdmin.Controls.XenSearch.DropDownComboButton();
            this.resourceSelectButton = new XenAdmin.Controls.XenSearch.ResourceSelectButton();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox
            // 
            resources.ApplyResources(this.textBox, "textBox");
            this.textBox.Name = "textBox";
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // removeButton
            // 
            resources.ApplyResources(this.removeButton, "removeButton");
            this.removeButton.Image = global::XenAdmin.Properties.Resources.minus;
            this.removeButton.Name = "removeButton";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // dateTimePicker
            // 
            resources.ApplyResources(this.dateTimePicker, "dateTimePicker");
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.ValueChanged += new System.EventHandler(this.dateTimePicker_ValueChanged);
            // 
            // numericUpDown
            // 
            resources.ApplyResources(this.numericUpDown, "numericUpDown");
            this.numericUpDown.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.numericUpDown.KeyUp += new System.Windows.Forms.KeyEventHandler(this.numericUpDown_KeyUp);
            // 
            // unitsLabel
            // 
            resources.ApplyResources(this.unitsLabel, "unitsLabel");
            this.unitsLabel.Name = "unitsLabel";
            // 
            // ComboButton
            // 
            resources.ApplyResources(this.ComboButton, "ComboButton");
            this.ComboButton.Name = "ComboButton";
            this.ComboButton.SelectedItem = null;
            this.ComboButton.UseVisualStyleBackColor = true;
            this.ComboButton.SelectedItemChanged += new System.EventHandler(this.ComboButton_SelectedIndexChanged);
            // 
            // matchTypeComboButton
            // 
            resources.ApplyResources(this.matchTypeComboButton, "matchTypeComboButton");
            this.matchTypeComboButton.Name = "matchTypeComboButton";
            this.matchTypeComboButton.SelectedItem = null;
            this.matchTypeComboButton.UseVisualStyleBackColor = true;
            this.matchTypeComboButton.SelectedItemChanged += new System.EventHandler(this.matchTypeComboButton_SelectedItemChanged);
            // 
            // queryTypeComboButton
            // 
            resources.ApplyResources(this.queryTypeComboButton, "queryTypeComboButton");
            this.queryTypeComboButton.Name = "queryTypeComboButton";
            this.queryTypeComboButton.SelectedItem = null;
            this.queryTypeComboButton.UseVisualStyleBackColor = true;
            this.queryTypeComboButton.SelectedItemChanged += new System.EventHandler(this.queryTypeComboButton_SelectedItemChanged);
            this.queryTypeComboButton.ItemSelected += new System.EventHandler(this.queryTypeComboButton_ItemSelected);
            this.queryTypeComboButton.BeforeItemSelected += new System.EventHandler(this.queryTypeComboButton_BeforeItemSelected);
            // 
            // resourceSelectButton
            // 
            resources.ApplyResources(this.resourceSelectButton, "resourceSelectButton");
            this.resourceSelectButton.Name = "resourceSelectButton";
            this.resourceSelectButton.SelectedItem = null;
            this.resourceSelectButton.UseMnemonic = false;
            this.resourceSelectButton.UseVisualStyleBackColor = true;
            this.resourceSelectButton.SelectedItemChanged += new System.EventHandler(this.resourceSelectButton_SelectedIndexChanged);
            // 
            // QueryElement
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.unitsLabel);
            this.Controls.Add(this.numericUpDown);
            this.Controls.Add(this.ComboButton);
            this.Controls.Add(this.matchTypeComboButton);
            this.Controls.Add(this.queryTypeComboButton);
            this.Controls.Add(this.dateTimePicker);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.resourceSelectButton);
            this.Name = "QueryElement";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private DropDownComboButton ComboButton;
        internal DropDownComboButton queryTypeComboButton;
        internal DropDownComboButton matchTypeComboButton;
        private System.Windows.Forms.NumericUpDown numericUpDown;
        private System.Windows.Forms.Label unitsLabel;
        private ResourceSelectButton resourceSelectButton;
    }
}