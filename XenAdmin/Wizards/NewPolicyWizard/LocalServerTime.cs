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
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class LocalServerTime : UserControl
    {
        public LocalServerTime()
        {
            InitializeComponent();
        }

        void action_CompletedTimeServer(ActionBase sender)
        {
            GetServerTimeAction action = (GetServerTimeAction)sender;
            Program.Invoke(Program.MainWindow, () =>
            {
                string serverLocalTimeString = action.Result;
                if (serverLocalTimeString != "")
                {
                    DateTime serverLocalTime = DateTime.Parse(serverLocalTimeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                    serverLocalTimeString = HelpersGUI.DateTimeToString(serverLocalTime, Messages.DATEFORMAT_WDMY_HM_LONG, true);
                }
                labelServerTime.Text = string.Format(Messages.SERVER_TIME, serverLocalTimeString);
            });
        }
        private Pool _pool;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Pool Pool
        {
            get
            {
                return _pool;
            }
            set
            {
                _pool = value;
                GetServerTime();
            }
        }

        public void GetServerTime()
        {
            Host master = Helpers.GetMaster(Pool);
            if (master != null)
            {
                GetServerTimeAction action = new GetServerTimeAction(master);
                action.Completed += action_CompletedTimeServer;
                action.RunAsync();
            }
        }
    }
}
