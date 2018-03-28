﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAdmin.Properties;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class DisplayOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DisplayOptionsPage()
        {
            InitializeComponent();

            build();
        }

        private void build()
        {
            GraphAreasRadioButton.Checked = Properties.Settings.Default.FillAreaUnderGraphs;
            GraphLinesRadioButton.Checked = !Properties.Settings.Default.FillAreaUnderGraphs;

            showHostOnlyOptionForSearchRadioButton.Checked = Properties.Settings.Default.ShowJustHostInSearch;
            showWholePoolOptionForSearchRadioButton.Checked = !Properties.Settings.Default.ShowJustHostInSearch;
        }

        public static void Log()
        {
            log.Info("=== FillAreaUnderGraphs: " + Properties.Settings.Default.FillAreaUnderGraphs.ToString());
            log.Info("=== ShowHostOnlyInSearch: " + Properties.Settings.Default.ShowJustHostInSearch.ToString());
        }

        #region IOptionsPage Members

        public void Save()
        {
            if (GraphAreasRadioButton.Checked != Properties.Settings.Default.FillAreaUnderGraphs)
                Properties.Settings.Default.FillAreaUnderGraphs = GraphAreasRadioButton.Checked;
 
            if (showHostOnlyOptionForSearchRadioButton.Checked != Properties.Settings.Default.ShowJustHostInSearch)
                Properties.Settings.Default.ShowJustHostInSearch = showHostOnlyOptionForSearchRadioButton.Checked;
 
        }

        #endregion

        #region IVerticalTab Members

        public override string Text
        {
            get { return Messages.DISPLAY; }
        }

        public string SubText
        {
            get { return Messages.DISPLAY_DETAILS; }
        }

        public Image Image
        {
            get { return Resources._001_PerformanceGraph_h32bit_16; }
        }

        #endregion
    }
}
