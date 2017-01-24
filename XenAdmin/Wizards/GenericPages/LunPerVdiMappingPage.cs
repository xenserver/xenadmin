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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions.VMActions;
using XenAdmin.Controls;
using XenAPI;

namespace XenAdmin.Wizards.GenericPages
{
    public abstract partial class LunPerVdiMappingPage : XenTabPage
    {
        protected LunPerVdiMappingPage()
        {
            InitializeComponent();
            lunPerVdiPicker.OnSelectionChanged += picker_SelectionChanged;
        }

        /// <summary>
        /// Data has been set in the picker i.e. user requires a mapping from this page
        /// </summary>
        public bool MapLunsToVdisRequired
        {
            get { return PickerData.Count > 0; }
        }

        /// <summary>
        /// Is all the data in the picker mappable
        /// </summary>
        public bool IsAnyPickerDataMappable
        {
            get { return PickerData.Any(i => i.IsValidForMapping); }
        }

        private void picker_SelectionChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        protected abstract void PopulatePicker();

        protected void AddDataToPicker(List<LunPerVdiPickerItem> itemsToAdd)
        {
            lunPerVdiPicker.AddRange(itemsToAdd);
        }

        protected List<LunPerVdiPickerItem> PickerData
        {
            get { return lunPerVdiPicker.MappedItems; }
        }

        public void ClearPickerData()
        {
            lunPerVdiPicker.Clear();
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if(direction == PageLoadedDirection.Back)
            {
                ClearPickerData();
            }

            OnPageUpdated();
            base.PageLeave(direction, ref cancel);
        }

        public bool ValidSelectionMade
        {
            get { return lunPerVdiPicker.SelectionIsValid; }
        }

        public override bool EnableNext()
        {
            return ValidSelectionMade;
        }

        public override string Text
        {
            get { return Messages.LUNPERVDI_PAGE_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.LUNPERVDI_PAGE_TITLE; }
        }

        protected string VdiColumnTitle
        {
            set { lunPerVdiPicker.VdiColumnTitle = value; }
        }

    }
}
