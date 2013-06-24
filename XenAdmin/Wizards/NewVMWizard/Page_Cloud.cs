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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using System.Diagnostics;
using System.Xml;

namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_Cloud : XenTabPage
    {
        public Page_Cloud()
        {
            InitializeComponent();
        }

        public override string PageTitle
        {
            get
            {
                return "Cloud Settings";
            }
        }

        public override string Text
        {
            get
            {
                return "Cloud Settings";
            }
        }

        public override bool EnableNext()
        {
            return comboBoxKernel.Items.Count > 0 &&
                   comboBoxRAMdisk.Items.Count > 0 &&
                   comboBoxKeyPair.Items.Count > 0 &&
                   checkedListBoxSecurityGroups.Items.Count > 0 &&
                   dataGridViewInstanceType.Rows.Count > 0;
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                           {
                               new KeyValuePair<string, string>("Instance Type", InstanceType),
                               new KeyValuePair<string, string>("Kernel",
                                                                Kernel != null ? Kernel.Name : "Default Kernel"),
                               new KeyValuePair<string, string>("RAM Disk",
                                                                RAMDisk != null
                                                                    ? RAMDisk.Name
                                                                    : "Default RAM Disk"),
                               new KeyValuePair<string, string>("Key Pair", KeyPair),
                               new KeyValuePair<string, string>("Security Groups", string.Join(", ", SecurityGroups))
                };
            }
        }

        public VM Kernel
        {
            get
            {
                if (comboBoxKernel.SelectedIndex == 0)
                {
                    //Resolve default kernel id
                    return null;
                }
                else
                    return (VM)comboBoxKernel.SelectedItem;
            }
        }

        public VM RAMDisk
        {
            get
            {
                if (comboBoxKernel.SelectedIndex == 0)
                {
                    return null;
                }
                return (VM)comboBoxRAMdisk.SelectedItem;
            }
        }

        public string InstanceType
        {
            get { return dataGridViewInstanceType.SelectedRows[0].Cells[0].Value.ToString(); }
        }

        public string KeyPair
        {
            get { return comboBoxKeyPair.SelectedItem.ToString(); }
        }

        public string[] SecurityGroups
        {
            get
            {
                string[] destination = new string[checkedListBoxSecurityGroups.CheckedItems.Count];

                checkedListBoxSecurityGroups.CheckedItems.CopyTo(destination, 0);

                return destination;
            }
        }
    }
}
