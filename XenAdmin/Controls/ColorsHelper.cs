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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using XenAdmin.Dialogs;

namespace XenAdmin.Controls
{
    /// <summary>
    /// Utility class for viewing the members of System.Drawing.SystemColors and System.Windows.Forms.ProfessionalColors.
    /// 
    /// Try 'XenAdmin.Controls.ColorsHelper.Run()' in the Immediate Window.
    /// </summary>
    public partial class ColorsHelper : XenDialogBase
    {
        public ColorsHelper()
        {
            InitializeComponent();

            foreach (PropertyInfo pi in typeof(SystemColors).GetProperties())
            {
                if (pi.PropertyType == typeof(Color))
                {
                    Label label1 = new Label();
                    label1.Text = "SystemColors." + pi.Name;
                    label1.Dock = DockStyle.Fill;
                    tableLayoutPanel1.Controls.Add(label1);

                    Label label2 = new Label();
                    label2.Dock = DockStyle.Fill;
                    label2.BackColor = (Color)pi.GetValue(null, null);
                    tableLayoutPanel1.Controls.Add(label2);
                }
            }

            foreach (PropertyInfo pi in typeof(ProfessionalColors).GetProperties())
            {
                if (pi.PropertyType == typeof(Color))
                {
                    Label label1 = new Label();
                    label1.Text = "ProfessionalColors." + pi.Name;
                    label1.Dock = DockStyle.Fill;
                    tableLayoutPanel2.Controls.Add(label1);

                    Label label2 = new Label();
                    label2.Dock = DockStyle.Fill;
                    label2.BackColor = (Color)pi.GetValue(null, null);
                    tableLayoutPanel2.Controls.Add(label2);
                }
            }
        }

        static void Run()
        {
            Application.EnableVisualStyles();
            Application.Run(new ColorsHelper());
        }
    }
}