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

using System.Collections;
using System.ComponentModel;

namespace XenAdmin.Controls.DataGridViewEx
{
    /// <summary>
    /// Abstract base class that provides a bidirectional comparator for sorting
    /// for CollapsingPoolHostDataGridViewRows
    /// </summary>
    public abstract class CollapsingPoolHostDataGridViewRowSorter : IComparer
    {
        private readonly ListSortDirection direction;
        private CollapsingPoolHostDataGridViewRow lhs;
        private CollapsingPoolHostDataGridViewRow rhs;

        protected CollapsingPoolHostDataGridViewRow Lhs
        {
            get { return lhs; }
        }

        protected CollapsingPoolHostDataGridViewRow Rhs
        {
            get { return rhs; }
        }

        protected CollapsingPoolHostDataGridViewRowSorter()
        {
            direction = ListSortDirection.Ascending;
        }

        protected CollapsingPoolHostDataGridViewRowSorter(ListSortDirection direction)
        {
            this.direction = direction;
        }

        /// <summary>
        /// Interface member correcting the sort for the direction required
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>0,1 or -1 depending on the comparison</returns>
        public int Compare(object x, object y)
        {
            if (x == null || y == null)
                return 1;

            lhs = x as CollapsingPoolHostDataGridViewRow;
            rhs = y as CollapsingPoolHostDataGridViewRow;

            if (lhs == null || rhs == null)
                return 1;

            if (direction == ListSortDirection.Ascending)
                return PerformSort();
            else
                return -1 * PerformSort();
        }

        /// <summary>
        /// Method to perform the sort on the two CollapsingPoolHostDataGridViewRows
        /// </summary>
        /// <returns></returns>
        protected abstract int PerformSort();
    }
}
