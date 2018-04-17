﻿namespace XenAdmin.SettingsPanels
{
    partial class ClusteringEditPage
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClusteringEditPage));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.CheckBoxEnableClustering = new System.Windows.Forms.CheckBox();
            this.labelNetwork = new System.Windows.Forms.Label();
            this.tableLayoutInfo = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxInfo1 = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelHostCountWarning = new System.Windows.Forms.Label();
            this.pictureBoxInfo2 = new System.Windows.Forms.PictureBox();
            this.labelTitle = new XenAdmin.Controls.Common.AutoHeightLabel();
            this.comboBoxNetwork = new XenAdmin.Controls.NetworkComboBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo2)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.labelTitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.CheckBoxEnableClustering, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelNetwork, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxNetwork, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutInfo, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // CheckBoxEnableClustering
            // 
            resources.ApplyResources(this.CheckBoxEnableClustering, "CheckBoxEnableClustering");
            this.tableLayoutPanel1.SetColumnSpan(this.CheckBoxEnableClustering, 2);
            this.CheckBoxEnableClustering.Name = "CheckBoxEnableClustering";
            this.CheckBoxEnableClustering.UseVisualStyleBackColor = true;
            // 
            // labelNetwork
            // 
            resources.ApplyResources(this.labelNetwork, "labelNetwork");
            this.labelNetwork.Name = "labelNetwork";
            // 
            // tableLayoutInfo
            // 
            resources.ApplyResources(this.tableLayoutInfo, "tableLayoutInfo");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutInfo, 2);
            this.tableLayoutInfo.Controls.Add(this.pictureBoxInfo2, 0, 2);
            this.tableLayoutInfo.Controls.Add(this.pictureBoxInfo1, 0, 0);
            this.tableLayoutInfo.Controls.Add(this.labelWarning, 1, 0);
            this.tableLayoutInfo.Controls.Add(this.labelInfo, 1, 2);
            this.tableLayoutInfo.Controls.Add(this.labelHostCountWarning, 1, 3);
            this.tableLayoutInfo.Name = "tableLayoutInfo";
            // 
            // pictureBoxInfo1
            // 
            resources.ApplyResources(this.pictureBoxInfo1, "pictureBoxInfo1");
            this.pictureBoxInfo1.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxInfo1.Name = "pictureBoxInfo1";
            this.pictureBoxInfo1.TabStop = false;
            // 
            // labelWarning
            // 
            resources.ApplyResources(this.labelWarning, "labelWarning");
            this.labelWarning.Name = "labelWarning";
            // 
            // labelInfo
            // 
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
            // 
            // labelHostCountWarning
            // 
            resources.ApplyResources(this.labelHostCountWarning, "labelHostCountWarning");
            this.labelHostCountWarning.Name = "labelHostCountWarning";
            // 
            // pictureBoxInfo2
            // 
            resources.ApplyResources(this.pictureBoxInfo2, "pictureBoxInfo2");
            this.pictureBoxInfo2.Image = global::XenAdmin.Properties.Resources._000_Info3_h32bit_16;
            this.pictureBoxInfo2.Name = "pictureBoxInfo2";
            this.pictureBoxInfo2.TabStop = false;
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.tableLayoutPanel1.SetColumnSpan(this.labelTitle, 2);
            this.labelTitle.Name = "labelTitle";
            // 
            // comboBoxNetwork
            // 
            this.comboBoxNetwork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNetwork.FormattingEnabled = true;
            this.comboBoxNetwork.IncludeOnlyEnabledNetworksInComboBox = false;
            this.comboBoxNetwork.IncludeOnlyNetworksWithIPAddresses = false;
            this.comboBoxNetwork.IncludePoolNameInComboBox = false;
            resources.ApplyResources(this.comboBoxNetwork, "comboBoxNetwork");
            this.comboBoxNetwork.Name = "comboBoxNetwork";
            // 
            // ClusteringEditPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ClusteringEditPage";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutInfo.ResumeLayout(false);
            this.tableLayoutInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxInfo2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Controls.Common.AutoHeightLabel labelTitle;
        private System.Windows.Forms.CheckBox CheckBoxEnableClustering;
        private System.Windows.Forms.Label labelNetwork;
        private System.Windows.Forms.PictureBox pictureBoxInfo1;
        private System.Windows.Forms.Label labelWarning;
        private Controls.NetworkComboBox comboBoxNetwork;
        private System.Windows.Forms.TableLayoutPanel tableLayoutInfo;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelHostCountWarning;
        private System.Windows.Forms.PictureBox pictureBoxInfo2;

    }
}
