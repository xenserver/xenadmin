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
using System.Windows.Forms;
using System.Linq;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Actions;
using System.Drawing;
using System.Globalization;


namespace XenAdmin.Dialogs
{
    public partial class ConvertToThinSRDialog : XenDialogBase
    {
        private readonly SR TheSR;

        public ConvertToThinSRDialog(IXenConnection connection, SR sr)
            :base(connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (sr == null) throw new ArgumentNullException("sr");

            InitializeComponent();
            InitDialog(connection);
         
   			this.connection = connection;
            TheSR = sr;
            thinProvisioningParameters1.SR = TheSR;

            this.Text = string.Format(Messages.ACTION_SR_CONVERT_TO_THIN, TheSR.Name);
        }

        private void InitDialog(IXenConnection connection)
        {
            this.Owner = Program.MainWindow;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            new XenAdmin.Actions.SrAction(SrActionKind.ConvertToThin, TheSR, NewSMConfig).RunAsync();
            
            DialogResult = DialogResult.OK;
            Close();
        }

        public Dictionary<string, string> NewSMConfig
        {
            get
            {
                return thinProvisioningParameters1.SMConfig;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        internal override string HelpName
        {
            get
            {
                return "ConvertToThinSRDialog";
            }
        }
    }
}
