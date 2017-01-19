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

namespace XenAdmin.Controls.DataGridViewEx
{
    /// <summary>
    /// Class that performs a default sort of the CollapsingPoolHostDataGridViewRows
    /// Order being pools, hosts and if equal, use the classes comaprison method
    /// </summary>
    public sealed class CollapsingPoolHostDataGridViewRowDefaultSorter : CollapsingPoolHostDataGridViewRowSorter
    {
        public CollapsingPoolHostDataGridViewRowDefaultSorter() { }

        public CollapsingPoolHostDataGridViewRowDefaultSorter(ListSortDirection direction) : base(direction) { }

        protected override int PerformSort()
        {
            return ComparePoolsThenHosts();
        }

        private int ComparePoolsThenHosts()
        {
            if (Lhs.IsAPoolRow && Rhs.IsAHostRow)
                return -1;

            if (Lhs.IsAHostRow && Rhs.IsAPoolRow)
                return 1;

            if (Lhs.IsAPoolRow && Rhs.IsAPoolRow)
            {
                return ComparePools();
            }

            if (Lhs.IsAHostRow && Rhs.IsAHostRow)
            {
                return CompareHosts();
            }

            return 0;
        }

        private int ComparePools()
        {
            return Lhs.UnderlyingPool.CompareTo(Rhs.UnderlyingPool);
        }

        private int CompareHosts()
        {
            return Lhs.UnderlyingHost.CompareTo(Rhs.UnderlyingHost);
        }

    }
}
