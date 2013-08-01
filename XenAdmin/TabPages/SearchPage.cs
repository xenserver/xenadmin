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
using System.Windows.Forms;
using XenAdmin.Controls.XenSearch;
using XenAdmin.Core;
using XenAdmin.XenSearch;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Commands;


namespace XenAdmin.TabPages
{
    public partial class SearchPage : BaseTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool ignoreSearchUpdate;
        private List<IXenObject> xenObjects;

        public event EventHandler SearchChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPage"/> class.
        /// </summary>
        public SearchPage()
        {
            InitializeComponent();

            Searcher.SearchChanged += UI_SearchChanged;
            Searcher.SaveRequested += Searcher_SaveRequested;
            OutputPanel.QueryPanel.SearchChanged += UI_SearchChanged;

            if (!Application.RenderWithVisualStyles)
            {
                panel2.BackColor = Searcher.BackColor = SystemColors.Control;
                OutputPanel.BackColor = SystemColors.Control;
                tableLayoutPanel.BackColor = SystemColors.ControlDark;
            }
        }

        protected virtual void OnSearchChanged(EventArgs e)
        {
            EventHandler handler = SearchChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void Searcher_SaveRequested()
        {
            Search newSearch = Search;
            
            string saveName = newSearch.Name ?? String.Empty;
            List<Search> existingSearches = new List<Search>(Search.Searches);
            if (null != existingSearches.Find(search => search.Name == saveName))
            {
                // name already exists: choose a new name by appending an integer (CA-34780)
                for (int i = 2; ; ++i)
                {
                    string possName = string.Format("{0} ({1})", saveName, i);
                    if (null == existingSearches.Find(search => search.Name == possName))  // here's a good name
                    {
                        saveName = possName;
                        break;
                    }
                }
            }

            using (var dialog = new NameAndConnectionPrompt
            {
                PromptedName = saveName,
                HelpID = "SaveSearchDialog"
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK &&
                    dialog.Connection != null) // CA-40307
                {
                    newSearch.Name = dialog.PromptedName;
                    newSearch.Connection = dialog.Connection;

                    new SearchAction(newSearch, SearchAction.Operation.save).RunAsync();
                }
            }
        }

        private void UI_SearchChanged()
        {
            if (!ignoreSearchUpdate && !Program.Exiting)
            {
                OutputPanel.Search = Search;
                OnSearchChanged(EventArgs.Empty);
            }
        }

        public IXenObject XenObject
        {
            set
            {
                XenObjects = new IXenObject[] { value };
            }
        }

        public IEnumerable<IXenObject> XenObjects
        {
            set
            {
                Util.ThrowIfParameterNull(value, "value");

                xenObjects = new List<IXenObject>(value);

                if (xenObjects.Count == 0 && TreeSearch.DefaultTreeSearch != null)
                {
                    Search = TreeSearch.DefaultTreeSearch;
                }
                else
                {
                    Search = Search.SearchFor(value);
                }
            }
        }

        public Search Search
        {
            get
            {
                QueryScope scope = Searcher.QueryScope;
                QueryFilter filter = Searcher.QueryFilter;
                Query query = new Query(scope, filter);
                Grouping grouping = Searcher.Grouping;
                string name = (base.Text == Messages.CUSTOM_SEARCH ? null : base.Text);
                string uuid = null;
                List<KeyValuePair<String, int>> columns = OutputPanel.QueryPanel.ColumnsAndWidths;
                Sort[] sorting = OutputPanel.QueryPanel.Sorting;

                return new Search(query, grouping, Searcher.Expanded, name, uuid, columns, sorting);
            }

            set
            {
                Searcher.ToggleExpandedState(value.ShowSearch);

                ignoreSearchUpdate = true;
                try
                {
                    Searcher.Search = value;
                }
                finally
                {
                    ignoreSearchUpdate = false;
                }

                OutputPanel.Search = value;

                UpdateTitle(value);
            }
        }

        private void UpdateTitle(Search search)
        {
            base.Text = ((search == null || search.Name == null) ? Messages.CUSTOM_SEARCH : HelpersGUI.GetLocalizedSearchName(search));
        }

        public void BuildList()
        {
            if (!this.Visible)
                return;
            OutputPanel.BuildList();
        }
    
        private void SearchButton_Click(object sender, EventArgs e)
        {
            searchOptionsMenuStrip.Show(this, 
                new Point(SearchButton.Left + panel4.Left + tableLayoutPanel.Left + pageContainerPanel.Left, 
                    SearchButton.Bottom + panel4.Left + tableLayoutPanel.Left + pageContainerPanel.Top));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Searcher != null)
                Searcher.MaxHeight = Height / 2;
        }

        #region Search menu

        private void searchOptionsMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            AttachSavedSubmenu(applySavedToolStripMenuItem, true);
            AttachSavedSubmenu(deleteSavedToolStripMenuItem, false);
        }

        private void editSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Searcher.ToggleExpandedState(true);
            OnSearchChanged(EventArgs.Empty);
        }

        private void resetSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search search = Search.SearchFor(xenObjects);
            search.ShowSearch = Searcher.Visible;
            Search = search;
        }

        private void exportSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.AddExtension = true;
                dialog.Filter = string.Format("{0} (*.xensearch)|*.xensearch|{1} (*.*)|*.*",
                    Messages.XENSEARCH_SAVED_SEARCH, Messages.ALL_FILES);
                dialog.FilterIndex = 0;
                dialog.RestoreDirectory = true;
                dialog.DefaultExt = "xensearch";
                dialog.CheckPathExists = false;

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    log.InfoFormat("Exporting search to {0}", dialog.FileName);
                    Search.Save(dialog.FileName);
                    log.InfoFormat("Exported search to {0} successfully.", dialog.FileName);
                }
                catch
                {
                    log.ErrorFormat("Failed to export search to {0}", dialog.FileName);
                    throw;
                }
            }
        }

        private void importSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ImportSearchCommand(Program.MainWindow.CommandInterface).Execute();
        }

        private void applySavedSearch_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Search = item.Tag as Search;
        }

        private void deleteSavedSearch_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Search search = (Search)item.Tag;

            if (new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Information, String.Format(Messages.DELETE_SEARCH_PROMPT, search.Name), String.Format(Messages.DELETE_SEARCH, search.Name)),
                    ThreeButtonDialog.ButtonOK,
                    ThreeButtonDialog.ButtonCancel).ShowDialog(this) == DialogResult.OK)
            {
                new SearchAction(search, SearchAction.Operation.delete).RunAsync();
            }
        }

        // If applyMenu, include all searches; otherwise (delete menu), only include custom searches.
        // Also they use different click handlers.
        private ToolStripItem[] MakeSavedSubmenu(bool applyMenu)
        {
            List<ToolStripItem> ans = new List<ToolStripItem>();

            Search[] searches = Search.Searches;
            Array.Sort(searches);
            foreach (Search search in searches)
            {
                if (!applyMenu && search.DefaultSearch)
                    continue;
                Image icon = search.DefaultSearch ? Properties.Resources._000_defaultSpyglass_h32bit_16 : Properties.Resources._000_Search_h32bit_16;
                EventHandler onClickDelegate = applyMenu ? (EventHandler)applySavedSearch_Click : (EventHandler)deleteSavedSearch_Click;
                ToolStripMenuItem item = new ToolStripMenuItem(search.Name.EscapeAmpersands(), icon, onClickDelegate);
                item.Tag = search;
                ans.Add(item);
            }

            // If we have no items, make a greyed-out "(None)" item
            if (ans.Count == 0)
            {
                ToolStripMenuItem item = MainWindow.NewToolStripMenuItem(Messages.NONE_PARENS);
                item.Enabled = false;
                ans.Add(item);
            }

            return ans.ToArray();
        }

        // applyMenu: See comment on MakeSavedSubmenu()
        private void AttachSavedSubmenu(ToolStripDropDownItem parent, bool applyMenu)
        {
            parent.DropDownItems.Clear();
            parent.DropDownItems.AddRange(MakeSavedSubmenu(applyMenu));
        }

        #endregion

        public void PanelShown()
        {
            QueryPanel.PanelShown();
        }

        public void PanelHidden()
        {
            QueryPanel.PanelHidden();
        }

        internal void PanelProd()
        {
            QueryPanel.Prod();
        }
    }
}
