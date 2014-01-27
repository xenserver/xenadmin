/* Copyright (c) Citrix Systems Inc. 
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

using XenAdmin.Core;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using XenAdmin.Commands;


namespace XenAdmin.TabPages
{
    public partial class HomePage : DoubleBufferedPanel
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private Label[] LearnLabels
        {
            get { return new Label[] { titleLearn, line1Learn, line2Learn }; }
        }

        private Label[] AddLabels
        {
            get { return new Label[] { titleAdd, line1Add }; }
        }

        private Label[] GetLabels
        {
            get { return new Label[] { titleUpgrade, line1Upgrade }; }
        }

        private Label[] TryLabels
        {
            get { return new Label[] { titleTry, line1Try, line2Try }; }
        }

        private void panelLearn_Click(object sender, EventArgs e)
        {
            Program.MainWindow.ShowHelpTOC();
        }

        private void panelAdd_Click(object sender, EventArgs e)
        {
            new AddHostCommand(Program.MainWindow).Execute();
        }

        private void panelGet_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.UPSELL_LEARNMOREURL_GENERAL);
        }

        private void panelTry_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.XENDESKTOP_URL);
        }

        private void labelNetwork_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.COMMUNITY_URL);
        }

        private void labelSupport_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.SUPPORT_URL);
        }

        private void labelPartners_Click(object sender, EventArgs e)
        {
            Program.OpenURL(InvisibleMessages.PARTNEROFFERS_URL);
        }

        private void Underline(bool b, params Label[] lbls)
        {
            foreach (Label lbl in lbls)
            {
                lbl.Font = b ?
                    new Font(lbl.Font, lbl.Font.Style | FontStyle.Underline) :
                    new Font(lbl.Font, lbl.Font.Style & ~FontStyle.Underline);
            }
        }

        private void Colour(Color c, params Label[] lbls)
        {
            foreach (Label lbl in lbls)
                lbl.ForeColor = c;
        }

        private void panelLearn_MouseEnter(object sender, EventArgs e)
        {
            Underline(true, LearnLabels);
        }

        private void panelLearn_MouseLeave(object sender, EventArgs e)
        {
            Underline(false, LearnLabels);
        }

        private void panelLearn_MouseDown(object sender, MouseEventArgs e)
        {
            Colour(Color.Red, LearnLabels);
        }

        private void panelLearn_MouseUp(object sender, MouseEventArgs e)
        {
            Colour(Color.Black, LearnLabels);
        }

        private void panelAdd_MouseEnter(object sender, EventArgs e)
        {
            Underline(true, AddLabels);
        }

        private void panelAdd_MouseLeave(object sender, EventArgs e)
        {
            Underline(false, AddLabels);
        }

        private void panelAdd_MouseDown(object sender, MouseEventArgs e)
        {
            Colour(Color.Red, AddLabels);
        }

        private void panelAdd_MouseUp(object sender, MouseEventArgs e)
        {
            Colour(Color.Black, AddLabels);
        }

        private void panelGet_MouseEnter(object sender, EventArgs e)
        {
            Underline(true, GetLabels);
        }

        private void panelGet_MouseLeave(object sender, EventArgs e)
        {
            Underline(false, GetLabels);
        }

        private void panelGet_MouseDown(object sender, MouseEventArgs e)
        {
            Colour(Color.Red, GetLabels);
        }

        private void panelGet_MouseUp(object sender, MouseEventArgs e)
        {
            Colour(Color.Black, GetLabels);
        }

        private void panelTry_MouseEnter(object sender, EventArgs e)
        {
            Underline(true, TryLabels);
        }

        private void panelTry_MouseLeave(object sender, EventArgs e)
        {
            Underline(false, TryLabels);
        }

        private void panelTry_MouseDown(object sender, MouseEventArgs e)
        {
            Colour(Color.Red, TryLabels);
        }

        private void panelTry_MouseUp(object sender, MouseEventArgs e)
        {
            Colour(Color.Black, TryLabels);
        }

        private void labelNetwork_MouseEnter(object sender, EventArgs e)
        {
            Underline(true, labelNetwork);
        }

        private void labelNetwork_MouseLeave(object sender, EventArgs e)
        {
            Underline(false, labelNetwork);
        }

        private void labelNetwork_MouseDown(object sender, MouseEventArgs e)
        {
            Colour(Color.Red, labelNetwork);
        }

        private void labelNetwork_MouseUp(object sender, MouseEventArgs e)
        {
            Colour(Color.Black, labelNetwork);
        }

        private void labelSupport_MouseEnter(object sender, EventArgs e)
        {
            Underline(true, labelSupport);
        }

        private void labelSupport_MouseLeave(object sender, EventArgs e)
        {
            Underline(false, labelSupport);
        }

        private void labelSupport_MouseDown(object sender, MouseEventArgs e)
        {
            Colour(Color.Red, labelSupport);
        }

        private void labelSupport_MouseUp(object sender, MouseEventArgs e)
        {
            Colour(Color.Black, labelSupport);
        }

        private void labelPartners_MouseEnter(object sender, EventArgs e)
        {
            Underline(true, labelPartners);
        }

        private void labelPartners_MouseLeave(object sender, EventArgs e)
        {
            Underline(false, labelPartners);
        }

        private void labelPartners_MouseDown(object sender, MouseEventArgs e)
        {
            Colour(Color.Red, labelPartners);
        }

        private void labelPartners_MouseUp(object sender, MouseEventArgs e)
        {
            Colour(Color.Black, labelPartners);
        }

        private void HomePage_SizeChanged(object sender, EventArgs e)
        {
            // Implement own centring, because built-in anchoring loses both edges if the window gets too narrow.
            if (this.Width <= mainPanel.Width)
                mainPanel.Left = 0;
            else
                mainPanel.Left = (this.Width - mainPanel.Width) / 2;
        }

    }
}
