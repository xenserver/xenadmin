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
    public partial class MessageBoxTest : XenDialogBase
    {
        private readonly System.Threading.Timer timer;
        private readonly List<TextBox> argBoxes = new List<TextBox>();

        private ThreeButtonDialog tbd = null;

        public MessageBoxTest()
        {
            timer = new System.Threading.Timer(refreshTick, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            InitializeComponent();

            argBoxes.Add(Parameter1Box);
            argBoxes.Add(Parameter2Box);
            argBoxes.Add(Parameter3Box);

            Button1Box.Text = "OK";
            Button2Box.Text = "Cancel";

            textBox1_TextChanged(null, null);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            timer.Change(500, System.Threading.Timeout.Infinite);

            Parameter1Box.Enabled = FormatBox.Text.Contains("{0}");
            Parameter2Box.Enabled = FormatBox.Text.Contains("{1}");
            Parameter3Box.Enabled = FormatBox.Text.Contains("{2}");
        }

        private void refreshTick(object state)
        {
            timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            Program.Invoke(this, refreshTick_);
        }

        private void refreshTick_()
        {
            Point loc = tbd == null ? new Point(100, 100) : tbd.Location;

            List<string> args = new List<string>();
            foreach (TextBox tb in argBoxes)
            {
                if (tb.Enabled)
                    args.Add(tb.Text);
            }

            string msg;
            try
            {
                msg = string.Format(FormatBox.Text, args.ToArray());
            }
            catch (FormatException)
            {
                msg = FormatBox.Text;
            }

            bool two = Button3Box.Text == "";
            ThreeButtonDialog d;
            if (two)
            {
                d = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Information, msg),
                        new ThreeButtonDialog.TBDButton(Button1Box.Text, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT),
                        new ThreeButtonDialog.TBDButton(Button2Box.Text, DialogResult.OK, ThreeButtonDialog.ButtonType.CANCEL));
            }
            else
            {
                d = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Information, msg),
                        new ThreeButtonDialog.TBDButton(Button1Box.Text, DialogResult.OK, ThreeButtonDialog.ButtonType.NONE),
                        new ThreeButtonDialog.TBDButton(Button2Box.Text, DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT),
                        new ThreeButtonDialog.TBDButton(Button3Box.Text, DialogResult.OK, ThreeButtonDialog.ButtonType.CANCEL));
            }
            d.Show();
            d.Location = loc;
            Focus();
            BringToFront();
            
            if (tbd != null)
                tbd.Close();
            tbd = d;
        }
    }
}