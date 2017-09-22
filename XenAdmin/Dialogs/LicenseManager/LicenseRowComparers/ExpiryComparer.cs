﻿/* Copyright (c) Citrix Systems, Inc. 
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

using System.Collections.Generic;
using XenAdmin.Controls.CheckableDataGridView;

namespace XenAdmin.Dialogs.LicenseManagerLicenseRowComparers
{
    public class ExpiryComparer : IComparer<CheckableDataGridViewRow>
    {
        public int Compare(CheckableDataGridViewRow x, CheckableDataGridViewRow y)
        {
            LicenseDataGridViewRow lx = x as LicenseDataGridViewRow;
            LicenseDataGridViewRow ly = y as LicenseDataGridViewRow;

            if (lx == null && ly != null)
                return 1;

            if (lx != null && ly == null)
                return -1;

            if (lx == null && ly == null)
                return 0;

            int result = lx.CurrentLicenseState.CompareTo(ly.CurrentLicenseState);
            if (result == 0)
            {
                if (lx.CurrentLicenseState == LicenseStatus.HostState.RegularGrace || lx.CurrentLicenseState == LicenseStatus.HostState.UpgradeGrace)
                    result = lx.LicenseExpires.GetValueOrDefault().CompareTo(ly.LicenseExpires.GetValueOrDefault());
                
                if (result == 0)
                    result = lx.XenObject.Name().CompareTo(ly.XenObject.Name());
            }
            return result;
        }
    }
}
