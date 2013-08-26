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
using System.Linq;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Commands;
using XenAdmin.XenSearch;
using XenAPI;


namespace XenAdmin.Controls.MainWindowControls
{
    public partial class NavigationPane : UserControl
    {
        public enum NavigationMode { Infrastructure, Objects, Organization, SavedSearch, Notifications }

        private NavigationMode currentMode;

        #region Events

        [Browsable(true)]
        public event Action TreeViewSelectionChanged;

        [Browsable(true)]
        public event Action TreeNodeBeforeSelected;

        [Browsable(true)]
        public event Action TreeNodeClicked;

        [Browsable(true)]
        public event Action TreeNodeRightClicked;

        [Browsable(true)]
        public event Action TreeViewRefreshed;

        [Browsable(true)]
        public event Action TreeViewRefreshSuspended;

        [Browsable(true)]
        public event Action TreeViewRefreshResumed;

        #endregion

        public NavigationPane()
        {
            InitializeComponent();

            AddNavigationItemPair(buttonInfraBig, buttonInfraSmall);
            AddNavigationItemPair(buttonObjectsBig, buttonObjectsSmall);
            AddNavigationItemPair(buttonTagsBig, buttonTagsSmall);
            AddNavigationItemPair(buttonSearchesBig, buttonSearchesSmall);
            AddNavigationItemPair(buttonNotifyBig, buttonNotifySmall);

            buttonInfraBig.SetTag(NavigationMode.Infrastructure);
            buttonObjectsBig.SetTag(NavigationMode.Objects);
            buttonTagsBig.SetTag(NavigationMode.Organization);
            buttonNotifyBig.SetTag(NavigationMode.Notifications);

            Search.SearchesChanged += PopulateSearchDropDown;
            PopulateSearchDropDown();

            buttonInfraBig.Checked = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            splitContainer1.Panel1MinSize = splitContainer1.ClientSize.Height - toolStripBig.MaximumSize.Height - splitContainer1.SplitterWidth;
        }

        #region Accessors

        private Search m_search;
        public Search Search
        {
            get
            {
                switch (currentMode)
                {
                    case NavigationMode.Notifications:
                    case NavigationMode.Infrastructure:
                        return TreeSearch.DefaultTreeSearch ?? TreeSearch.SearchFor(null);
                    case NavigationMode.Objects:
                        return Search.SearchForAllTypes();
                    case NavigationMode.Organization:
                        return Search.SearchForOrganization();
                    default:
                        return m_search;
                }
            }
        }

        public bool InSearchMode
        {
            set { navigationView.InSearchMode = value; }
        }

        internal SelectionBroadcaster SelectionManager
        {
            get { return navigationView.SelectionManager; }
        }

        #endregion

        public void XenConnectionCollectionChanged(CollectionChangeEventArgs e)
        {
            navigationView.XenConnectionCollectionChanged(e);
        }

        public bool SelectObject(IXenObject o)
        {
            return navigationView.SelectObject(o, false);
        }

        public void EditSelectedNode()
        {
           navigationView.EditSelectedNode();
        }

        public bool TryToSelectNewNode(Predicate<object> tagMatch, bool selectNode, bool expandNode, bool ensureNodeVisible)
        {
            return navigationView.TryToSelectNewNode(tagMatch, selectNode, expandNode, ensureNodeVisible);
        }

        public void RequestRefreshTreeView()
        {
            navigationView.RequestRefreshTreeView();
        }

        public void FocusTreeView()
        {
            navigationView.FocusTreeView();
        }

        public void SaveAndRestoreTreeViewFocus(MethodInvoker f)
        {
            navigationView.MajorChange(() => navigationView.SaveAndRestoreTreeViewFocus(f));
        }

        #region Private Methods

        private void AddNavigationItemPair(INavigationItem bigButton, INavigationItem smallButton)
        {
            bigButton.PairedItem = smallButton;
            smallButton.PairedItem = bigButton;
            bigButton.NavigationViewChanged += NavigationViewChanged;
            smallButton.NavigationViewChanged += NavigationViewChanged;
        }

        private void PopulateSearchDropDown()
        {
            Search[] searches = Search.Searches;
            Array.Sort(searches);

            var itemList = new List<ToolStripMenuItem>();

            foreach (Search search in searches)
            {
                var item = new ToolStripMenuItem
                               {
                                   Text = search.Name.EscapeAmpersands(),
                                   Tag = search,
                                   Image = search.DefaultSearch
                                               ? Properties.Resources._000_defaultSpyglass_h32bit_16
                                               : Properties.Resources._000_Search_h32bit_16,

                               };
                itemList.Add(item);
            }

            buttonSearchesBig.SetItemList(itemList.ToArray());
        }

        private void OnSearchChanged()
        {
            navigationView.CurrentSearch = Search;
            navigationView.NavigationMode = currentMode;
            navigationView.ResetSeachBox();
            navigationView.RequestRefreshTreeView();
            navigationView.FocusTreeView();
            navigationView.SelectObject(null, false); 
        }

        #endregion

        #region Control Event Handlers

        private void navigationView_TreeViewSelectionChanged()
        {
            if (TreeViewSelectionChanged != null)
                TreeViewSelectionChanged();
        }

        private void navigationView_TreeNodeBeforeSelected()
        {
            if (TreeNodeBeforeSelected != null)
                TreeNodeBeforeSelected();
        }

        private void navigationView_TreeNodeClicked()
        {
            if (TreeNodeClicked != null)
                TreeNodeClicked();
        }

        private void navigationView_TreeNodeRightClicked()
        {
            if (TreeNodeRightClicked != null)
                TreeNodeRightClicked();
        }

        private void navigationView_TreeViewRefreshed()
        {
            if (TreeViewRefreshed != null)
                TreeViewRefreshed();
        }

        private void navigationView_TreeViewRefreshResumed()
        {
            if (TreeViewRefreshResumed != null)
                TreeViewRefreshResumed();
        }

        private void navigationView_TreeViewRefreshSuspended()
        {
            if (TreeViewRefreshSuspended != null)
                TreeViewRefreshSuspended();
        }


        private void NavigationViewChanged(object obj)
        {
            var search = obj as Search;

            if (search == null)
            {
                currentMode = (NavigationMode)obj;
            }
            else
            {
                m_search = search;
                currentMode = NavigationMode.SavedSearch;
            }

            OnSearchChanged();
        }

        private void toolStripBig_LayoutCompleted(object sender, EventArgs e)
        {
            foreach (ToolStripItem item in toolStripBig.Items)
            {
                var navItem = item as INavigationItem;
                if (navItem == null)
                    continue;

                var pairedItem = navItem.PairedItem as ToolStripItem;
                if (pairedItem == null)
                    return;

                pairedItem.Visible = (item.Placement == ToolStripItemPlacement.None);
            }
        }

        #endregion
    }
}
