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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Controls.DataGridViewEx
{
    public class CollapsingPoolHostDataGridViewRow : DataGridViewExRow
    {
        protected class DataGridViewExNameCell : DataGridViewTextBoxCell
        {

        }

        protected DataGridViewImageCell _expansionCell;
        protected DataGridViewCheckBoxCell _poolCheckBoxCell;
        protected DataGridViewExNameCell _nameCell;
        protected bool _hasPool;

        protected CollapsingPoolHostDataGridViewRow()
        {}

        protected CollapsingPoolHostDataGridViewRow(Pool pool)
        {
            Tag = pool;
        }

        protected CollapsingPoolHostDataGridViewRow(Host host, bool hasPool)
        {
            _hasPool = hasPool;
            Tag = host;
        }

        protected virtual void SetStandaloneStatus(bool isStandalone)
        {}

        public virtual bool IsCheckable
        {
            get { return false; }
        }

        public int ExpansionCellIndex
        {
            get { return Cells.IndexOf(_expansionCell); }
        }

        public int PoolCheckBoxCellIndex
        {
            get { return Cells.IndexOf(_poolCheckBoxCell); }
        }

        public int NameCellIndex
        {
            get { return Cells.IndexOf(_nameCell); }
        }

        public bool IsAPoolRow
        {
            get { return Tag is Pool; }
        }

        public bool IsAHostRow
        {
            get { return Tag is Host; }
        }

        /// <summary>
        /// Get the underlying pool if a pool row otherwise returns null
        /// </summary>
        public Pool UnderlyingPool
        {
            get { return Tag as Pool; }
        }

        /// <summary>
        /// Get the underlying host if a host row otherwise returns null
        /// </summary>
        public Host UnderlyingHost
        {
            get { return Tag as Host; }
        }

        /// <summary>
        /// For a host row this means - are you a standalone host or not
        /// </summary>
        public bool HasPool
        {
            get { return _hasPool; }
        }

        public void SetCollapseIcon()
        {
            _expansionCell.Value = Resources.tree_minus;
        }

        public void SetExpandIcon()
        {
            _expansionCell.Value = Resources.tree_plus;
        }

        /// <summary>
        /// Convert row to a standalone host row
        /// </summary>
        /// <returns>success</returns>
        public void ToStandaloneHostRow()
        {
            SetStandaloneStatus(true);
        }

        /// <summary>
        /// Convert row non-standalone host
        /// </summary>
        /// <returns>success</returns>
        public void ToPooledHostRow()
        {
            SetStandaloneStatus(false);
        }
    }
}
