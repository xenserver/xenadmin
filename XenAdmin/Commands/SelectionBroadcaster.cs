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
using System.Windows.Forms;

namespace XenAdmin.Commands
{
    /// <summary>
    /// A class used to broad-cast MainWindow's current selection to <see cref="CommandToolStripMenuItem"/>s
    /// and <see cref="CommandToolStripButton"/>s.
    /// </summary>
    internal abstract class SelectionBroadcaster : IDisposable
    {
        /// <summary>
        /// Raises the <see cref="SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            EventHandler handler = SelectionChanged;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        /// <summary>
        /// Binds this object to any <see cref="CommandToolStripMenuItem"/>s and <see cref="CommandToolStripButton"/>s
        /// that are found in the specified <see cref="ToolStripItemCollection"/>.
        /// </summary>
        public void BindTo(ToolStripItemCollection toolStripItems, IMainWindow mainWindow)
        {
            Util.ThrowIfParameterNull(toolStripItems, "toolStripItems");
            foreach (ToolStripItem item in toolStripItems)
            {
                ICommandControl commandControl = item as ICommandControl;

                if (commandControl != null)
                    BindTo(commandControl, mainWindow);

                ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;

                if (dropDownItem != null)
                {
                    BindTo(dropDownItem.DropDownItems, mainWindow);
                }
            }
        }

        /// <summary>
        /// Bind this object to a single control
        /// </summary>
        public void BindTo(ICommandControl control, IMainWindow mainWindow)
        {
            ((ICommand)control.Command).SetMainWindow(mainWindow);
            control.SelectionBroadcaster = this;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SelectionChanged = null;
            }
        }

        /// <summary>
        /// Gets the current selection which will be used by listening <see cref="CommandToolStripMenuItem"/>s and 
        /// <see cref="CommandToolStripButton"/>s.
        /// </summary>
        public abstract SelectedItemCollection Selection { get;}

        public abstract void RefreshSelection();

        /// <summary>
        /// Occurs when the selection changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        public abstract void SaveAndClearSelection();
        public abstract void RestoreSavedSelection();

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
