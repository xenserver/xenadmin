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
using System.Text;
using XenAdmin.Core;

namespace XenAdmin.Dialogs.Wlb
{
    public class ConfirmDeconfigureWLBDialog : XenAdmin.Dialogs.XenDialogBase
    {
        private System.Windows.Forms.PictureBox QuestionPictureBox;
        private System.Windows.Forms.Button YesButton;
        private System.Windows.Forms.Button NoButton;
        private System.Windows.Forms.Label labelMessage;

        public ConfirmDeconfigureWLBDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmDeconfigureWLBDialog));
            this.labelMessage = new System.Windows.Forms.Label();
            this.QuestionPictureBox = new System.Windows.Forms.PictureBox();
            this.YesButton = new System.Windows.Forms.Button();
            this.NoButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.QuestionPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            resources.ApplyResources(this.labelMessage, "labelMessage");
            this.labelMessage.Name = "labelMessage";
            // 
            // QuestionPictureBox
            // 
            resources.ApplyResources(this.QuestionPictureBox, "QuestionPictureBox");
            this.QuestionPictureBox.Name = "QuestionPictureBox";
            this.QuestionPictureBox.TabStop = false;
            // 
            // YesButton
            // 
            this.YesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            resources.ApplyResources(this.YesButton, "YesButton");
            this.YesButton.Name = "YesButton";
            this.YesButton.UseVisualStyleBackColor = true;
            // 
            // NoButton
            // 
            this.NoButton.DialogResult = System.Windows.Forms.DialogResult.No;
            resources.ApplyResources(this.NoButton, "NoButton");
            this.NoButton.Name = "NoButton";
            this.NoButton.UseVisualStyleBackColor = true;
            // 
            // ConfirmDeconfigureWLBDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.NoButton);
            this.Controls.Add(this.YesButton);
            this.Controls.Add(this.QuestionPictureBox);
            this.Controls.Add(this.labelMessage);
            this.HelpButton = false;
            this.Name = "ConfirmDeconfigureWLBDialog";
            ((System.ComponentModel.ISupportInitialize)(this.QuestionPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}
