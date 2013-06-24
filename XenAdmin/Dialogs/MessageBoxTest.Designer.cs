namespace XenAdmin.Dialogs
{
    partial class MessageBoxTest
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageBoxTest));
            this.FormatBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Parameter1Box = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Parameter3Box = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Parameter2Box = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.Button1Box = new System.Windows.Forms.TextBox();
            this.Button2Box = new System.Windows.Forms.TextBox();
            this.Button3Box = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FormatBox
            // 
            this.FormatBox.AcceptsReturn = true;
            resources.ApplyResources(this.FormatBox, "FormatBox");
            this.FormatBox.Name = "FormatBox";
            this.FormatBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // Parameter1Box
            // 
            resources.ApplyResources(this.Parameter1Box, "Parameter1Box");
            this.Parameter1Box.Name = "Parameter1Box";
            this.Parameter1Box.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // Parameter3Box
            // 
            resources.ApplyResources(this.Parameter3Box, "Parameter3Box");
            this.Parameter3Box.Name = "Parameter3Box";
            this.Parameter3Box.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // Parameter2Box
            // 
            resources.ApplyResources(this.Parameter2Box, "Parameter2Box");
            this.Parameter2Box.Name = "Parameter2Box";
            this.Parameter2Box.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // CloseButton
            // 
            resources.ApplyResources(this.CloseButton, "CloseButton");
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // Button1Box
            // 
            resources.ApplyResources(this.Button1Box, "Button1Box");
            this.Button1Box.Name = "Button1Box";
            this.Button1Box.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Button2Box
            // 
            resources.ApplyResources(this.Button2Box, "Button2Box");
            this.Button2Box.Name = "Button2Box";
            this.Button2Box.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Button3Box
            // 
            resources.ApplyResources(this.Button3Box, "Button3Box");
            this.Button3Box.Name = "Button3Box";
            this.Button3Box.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // MessageBoxTest
            // 
            this.AcceptButton = this.CloseButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.CloseButton;
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.Button3Box);
            this.Controls.Add(this.Button2Box);
            this.Controls.Add(this.Button1Box);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Parameter2Box);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Parameter3Box);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Parameter1Box);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FormatBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.HelpButton = false;
            this.Name = "MessageBoxTest";
            this.ShowInTaskbar = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FormatBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Parameter1Box;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Parameter3Box;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Parameter2Box;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox Button1Box;
        private System.Windows.Forms.TextBox Button2Box;
        private System.Windows.Forms.TextBox Button3Box;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}