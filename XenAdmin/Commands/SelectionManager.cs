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
using System.Text;
using XenAPI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace XenAdmin.Commands
{
    internal class SelectionManager : SelectionBroadcaster
    {
        private SelectedItemCollection _selection = new SelectedItemCollection();
        private SelectedItemCollection savedSelection = null;
        private bool saved = false;

        /// <summary>
        /// Sets the main selection for XenCenter.
        /// </summary>
        /// <param name="selection">The selection.</param>
        public void SetSelection(IEnumerable<SelectedItem> selection)
        {
            Program.AssertOnEventThread();
            Util.ThrowIfParameterNull(selection, "selection");

            int count = 0;
            foreach (SelectedItem item in selection)
            {
                if (item == null)
                    throw new ArgumentException("Null SelectedItem found.", "selection");
                count++;
            }

            if (saved &&  // We have a saved selection: update it instead (CA-147401)
                count != 0)   // although if the new selection is empty, we're just refreshing the view via RefreshSelection(): don't save that
                savedSelection = new SelectedItemCollection(selection);
            else
            {
                _selection = new SelectedItemCollection(selection);
                OnSelectionChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the main selection for XenCenter.
        /// </summary>
        /// <param name="item">The selection.</param>
        public void SetSelection(SelectedItem item)
        {
            SetSelection(new SelectedItem[] { item });
        }

        /// <summary>
        /// Gets the current selection which will be used by listening <see cref="CommandToolStripMenuItem"/>s and
        /// <see cref="CommandToolStripButton"/>s.
        /// </summary>
        public override SelectedItemCollection Selection
        {
            get { return _selection; }
        }

        public override void RefreshSelection()
        {
            SetSelection(Selection);
        }

        public override void SaveAndClearSelection()
        {
            Program.AssertOnEventThread();
            savedSelection = _selection;
            SetSelection(new SelectedItemCollection());
            saved = true;
        }
        
        public override void RestoreSavedSelection() 
        {
            Program.AssertOnEventThread();
            if (saved)
            {
                saved = false;
                SetSelection(savedSelection);
                savedSelection = null;
            }
        }
    }
}
