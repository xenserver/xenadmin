﻿/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Windows.Forms;

namespace XenAdmin.Controls.DataGridViewEx
{
    public class DataGridViewExRow : DataGridViewRow, IComparable<DataGridViewExRow>
    {
        private bool _enabled = true;

        public DataGridViewExRow()
        {
            UpdateDefaultCellStyle();
        }

        public virtual bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                UpdateDefaultCellStyle();
            }
        }

        public void UpdateDefaultCellStyle()
        {
            if (DataGridView is DataGridViewEx dgv)
                DefaultCellStyle = dgv.GetRowCellStyle(_enabled, DataGridView?.Enabled ?? true);
        }

        public int CompareTo(DataGridViewExRow other)
        {
            if (Enabled != other.Enabled)
                return Enabled ? -1 : 1;

            return GetHashCode().CompareTo(other.GetHashCode());
        }
    }
}
