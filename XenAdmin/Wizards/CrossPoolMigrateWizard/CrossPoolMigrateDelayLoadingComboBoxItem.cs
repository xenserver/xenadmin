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
using System.Threading;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{

    public class CrossPoolMigrateDelayLoadingComboBoxItem : DelayLoadingOptionComboBoxItem
    {
        private readonly Host preSelectedHost;
        private readonly List<VM> preSelectedVMs;
        private  Queue<ReasoningFilter> filters;
        private bool allowSameTargetPool;

        /// <summary>
        /// Instantiate a delay loading combo box item for cross pool migrate
        /// </summary>
        /// <param name="itemAddedToComboBox">Item being added to the list</param>
        /// <param name="preSelectedHost">Host that was preselected by user prior to loading this item</param>
        /// <param name="preSelectedVMs">VMs selected prior to loading this item</param>
        public CrossPoolMigrateDelayLoadingComboBoxItem(IXenObject itemAddedToComboBox, Host preSelectedHost, List<VM> preSelectedVMs, bool allowSameTargetPool)
            : base(itemAddedToComboBox)
        {
            this.preSelectedHost = preSelectedHost;
            this.preSelectedVMs = preSelectedVMs;
            this.allowSameTargetPool = allowSameTargetPool;
        }

        /// <summary>
        /// FIFO of filters
        /// </summary>
        /// <returns></returns>
        private void BuildFilterList()
        {
            filters = new Queue<ReasoningFilter>();
            filters.Enqueue(new CrossPoolMigrateVersionFilter(Item));
            filters.Enqueue(new ResidentHostIsSameAsSelectionFilter(Item, preSelectedVMs));
            filters.Enqueue(new CrossPoolMigrateCanMigrateFilter(Item, preSelectedVMs, allowSameTargetPool));
            filters.Enqueue(new WlbEnabledFilter(Item, preSelectedVMs));
        }

        protected override string FetchFailureReason()
        {
            BuildFilterList();

            foreach (ReasoningFilter filter in filters)
            {
                if (filter.FailureFound)
                {
                    return filter.Reason;
                }  
            }

            return String.Empty;
        }
    }
}
