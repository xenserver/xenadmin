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

using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.XenSearch;

namespace XenAdmin.Controls.XenSearch
{
    public partial class SearchOutput : UserControl
    {
        public SearchOutput()
        {
            InitializeComponent();
        }

        public FolderNavigator FolderNavigator
        {
            get
            {
                return folderNavigator;
            }
        }

        public QueryPanel QueryPanel
        {
            get
            {
                return queryPanel;
            }
        }

        [Browsable(false)]
        public Search Search
        {
            set
            {
                string folder = (value == null ? null : value.FolderForNavigator);

                if (FolderNavigator != null)
                    FolderNavigator.Folder = folder;

                if (QueryPanel != null)
                    QueryPanel.Search = value;
            }
        }

        public void BuildList()
        {
            if (QueryPanel != null)
                QueryPanel.BuildList();
        }

        private void contextMenuStripColumns_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStripColumns.Items.Clear();
            contextMenuStripColumns.Items.AddRange(QueryPanel.GetChooseColumnsMenu().ToArray());
        }
    }
}