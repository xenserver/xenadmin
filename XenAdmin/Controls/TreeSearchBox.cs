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
using XenAdmin.XenSearch;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Controls
{
    public partial class TreeSearchBox : UserControl
    {
        public enum Mode { ServerView, OrgView, SavedSearch }
        public Mode currentMode;
        private Search currentSearch;

        public TreeSearchBox()
        {
            InitializeComponent();
            Search.SearchesChanged += Search_SearchesChanged;
            PopulateMenu();
        }

        public bool OrganizationalMode
        {
            get { return currentMode == Mode.OrgView; }
        }

        public string searchText
        {
            get { return searchTextBox.Text; }
            set { searchTextBox.Text = value; }
        }

        // This is here to support the unit tests
        internal List<ToolStripItem> MenuItems
        {
            get { return comboButtonViews.Items; }
        }

        private void Search_SearchesChanged(object sender, EventArgs e)
        {
            PopulateMenu();
        }

        private void PopulateMenu()
        {
            comboButtonViews.ClearItems();

            AddMenuItem(Messages.CLUSTER_VIEW, Mode.ServerView);
            AddMenuItem(Messages.ORGANIZATIONAL_VIEW, Mode.OrgView);

            Search[] searches = Search.Searches;
            Array.Sort<Search>(searches);

            foreach (Search search in searches)
                AddSearchMenuItem(search);
        }

        private void AddMenuItem(string name, object tag, Image image, bool selected)
        {
            ToolStripMenuItem menuItem = MainWindow.NewToolStripMenuItem(name, image);
            menuItem.Tag = tag;
            comboButtonViews.AddItem(menuItem);
            if (selected)
                comboButtonViews.SelectedItem = menuItem;
        }

        private void AddMenuItem(string name, Mode mode)
        {
            AddMenuItem(name, mode, null, currentMode == mode);
        }

        private void AddSearchMenuItem(Search search)
        {
            AddMenuItem(search.Name.EscapeAmpersands(), search,
                search.DefaultSearch ? Properties.Resources._000_defaultSpyglass_h32bit_16 : Properties.Resources._000_Search_h32bit_16,
                currentMode == Mode.SavedSearch && currentSearch.UUID == search.UUID);
        }

        // When switching between org and cluster view we default back to XenCenter 
        // node to stop weirdness
        void JumpToRootNode()
        {
            if (Program.MainWindow == null)
                return;

            Program.MainWindow.FocusTreeView();
            Program.MainWindow.SelectObject(null);
        }

        private void comboButtonViews_SelectedItemChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = comboButtonViews.SelectedItem as ToolStripMenuItem;
            if (menuItem == null)
                return;
            bool isSearch = menuItem.Tag is Search;

            if (isSearch)
            {
                currentSearch = (Search)menuItem.Tag;
                currentMode = Mode.SavedSearch;
            }
            else
            {
                Mode mode = (Mode)menuItem.Tag;
                currentMode = mode;
            }

            searchTextBox.Text = "";
            OnSearchChanged();
            JumpToRootNode();
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            OnSearchChanged();
        }

        public event EventHandler SearchChanged;
        public virtual void OnSearchChanged()
        {
            if (SearchChanged != null)
                SearchChanged(this, new EventArgs());
        }

        public Search Search
        {
            get
            {
                Search search;
                switch (currentMode)
                {
                    case Mode.ServerView:
                        search = TreeSearch.DefaultTreeSearch ?? TreeSearch.SearchFor(null);
                        break;
                    case Mode.OrgView:
                        search = Search.SearchForEverything();
                        break;
                    default:  // Mode.SavedSearch
                        search = currentSearch;
                        break;
                }

                return search.AddFullTextFilter(searchTextBox.Text);
            }
        }

        private bool wasFocused;
        private int cursorLoc;

        public void SaveState()
        {
            wasFocused = searchTextBox.ContainsFocus;
            cursorLoc = searchTextBox.SelectionStart;
        }

        public void RestoreState()
        {
            if (!wasFocused || searchTextBox.ContainsFocus)
                return;

            searchTextBox.Select();
            searchTextBox.Select(cursorLoc, 0);
        }
    }
}
