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
using System.Collections;
using System.ComponentModel;

namespace XenAdmin.Controls.DataGridViewEx
{
    /// <summary>
    /// A specific generic base class implementation to do a stable sort of CollapsingPoolHostDataGridViewRow derived types
    /// stable sorting using the default sorter
    /// 
    /// T is the sort of Row used which is a decendent of CollapsingPoolHostDataGridViewRow 
    /// </summary>
    /// <note>Base class takes care of the bi-direction behaviour</note>
    public abstract class CollapsingPoolHostDataGridViewRowStableSorter<T> : CollapsingPoolHostDataGridViewRowSorter where T : PoolHostDataGridViewOneCheckboxRow
    {
        private IComparer stableSorter = new CollapsingPoolHostDataGridViewRowDefaultSorter();

        protected CollapsingPoolHostDataGridViewRowStableSorter(){}

        /// <summary>
        /// Use this IComparer to sort any values where the implementations sorter 
        /// comes back with a zero i.e. the objects are the same
        /// </summary>
        public IComparer StableSorter
        {
            set { stableSorter = value;  }
        }

        protected CollapsingPoolHostDataGridViewRowStableSorter(ListSortDirection direction) : base(direction) { }

        protected override int PerformSort()
        {
            T rowLhs = Lhs as T;
            T rowRhs = Rhs as T;

            if (rowLhs == null || rowRhs == null)
                return 1;

            int comparision = SortRowByColumnDetails(rowLhs, rowRhs);

            if (comparision == 0)
            {
                return stableSorter.Compare(rowLhs, rowRhs);
            }

            return comparision;
        }

        /// <summary>
        /// Implement this to sort column details, if the comparison from this
        /// method returns zero, then the StableSorter will refine the comparison
        /// </summary>
        /// <param name="rowLhs">Object to compare</param>
        /// <param name="rowRhs">Object to compare</param>
        /// <returns>-1, +1 or 0</returns>
        protected abstract int SortRowByColumnDetails(T rowLhs,
                                                 T rowRhs);
    }
}
