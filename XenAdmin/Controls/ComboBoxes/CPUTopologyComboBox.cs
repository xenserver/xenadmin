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
using System.Windows.Forms;
using XenAPI;

namespace XenAdmin.Controls
{
    public partial class CPUTopologyComboBox : LongStringComboBox
    {
        public CPUTopologyComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            Sorted = false;
        }
        
        public long CoresPerSocket { get; private set; }
        private long origNoOfVCPUs;
        private long origCoresPerSocket;
        private long maxNoOfCoresPerSocket;

        /// <summary>
        /// Populate the combo box options, based on maxVCPUs.
        /// </summary>
        /// <param name="noOfVCPUs">Current value VCPUs_at_startup</param>
        /// <param name="maxVCPUs">Current value of VCPUs_max</param>
        /// <param name="coresPerSocket">Current value of platform:cores-per-socket</param>
        /// <param name="maxCoresPerSocket">Number of cores per socket on the underlying server</param>
        public void Populate(long noOfVCPUs, long maxVCPUs, long coresPerSocket, long maxCoresPerSocket)
        {
            origNoOfVCPUs = noOfVCPUs;
            origCoresPerSocket = CoresPerSocket = coresPerSocket;
            maxNoOfCoresPerSocket = maxCoresPerSocket;
            Update(maxVCPUs);
        }

        /// <summary>
        /// Repopulate the combobox options, based on number of vCPUs and update the selected item
        /// </summary>
        /// <param name="noOfVCPUs"></param>
        public void Update(long noOfVCPUs)
        {
            BeginUpdate();
            try
            {
                Items.Clear();
                // Build the list of topologies, based on number of vCPUs
                var topologies = GetTopologies(noOfVCPUs, maxNoOfCoresPerSocket);
                Items.AddRange(topologies.ToArray());

                // if the original value is not in the list (because is invalid) and the noOfVCPU hasn't changed, 
                // then add it to the combo box as an invalid configuration
                if (origNoOfVCPUs == noOfVCPUs &&
                    !topologies.Any(topologyTuple => topologyTuple.Cores == origCoresPerSocket))
                {
                    Items.Add(new TopologyTuple(origCoresPerSocket));
                }

                // try to re-select the current value
                foreach (var item in Items)
                {
                    if (item as TopologyTuple != null && (item as TopologyTuple ).Cores.Equals(CoresPerSocket))
                    {
                        SelectedItem = item;
                        break;
                    }
                }
            }
            finally
            {
                if (SelectedIndex < 0 && Items.Count > 0)  // if nothing selected, select first item
                    SelectedIndex = 0;
                EndUpdate();
            }
        }

        /// <summary>
        /// Build the list of topologies, based on number of vCPUs.
        /// This list will contain only valid values, i.e. cores per socket is a factor of number of vCPUs
        /// and does not exceed the number of cores per socket on the underlying server.
        /// </summary>
        /// <param name="noOfVCPUs">Number of vCPUs</param>
        /// <param name="maxNoOfCoresPerSocket">Maximum number of cores per socket</param>
        /// <returns>A list of topologies</returns>
        private static List<TopologyTuple> GetTopologies(long noOfVCPUs, long maxNoOfCoresPerSocket)
        {
            var result = new List<TopologyTuple>();
            var maxCoresPerSocket = maxNoOfCoresPerSocket > 0 ? Math.Min(noOfVCPUs, maxNoOfCoresPerSocket) : noOfVCPUs;
            for (var cores = 1; cores <= maxCoresPerSocket; cores++)
            {
                if (noOfVCPUs % cores == 0 && noOfVCPUs / cores <= VM.MAX_SOCKETS)
                    result.Add(new TopologyTuple(noOfVCPUs / cores, cores));
            }
            return result;
        }

        public bool IsValidVCPU(long noOfVCPUs)
        {
            return GetTopologies(noOfVCPUs, maxNoOfCoresPerSocket).Count != 0;
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            var selectedTopology = SelectedItem as TopologyTuple;
            if (selectedTopology != null)
                CoresPerSocket = selectedTopology.Cores;
            base.OnSelectedIndexChanged(e);
        }
    }

    public class TopologyTuple
    {
        public readonly long Sockets;
        public readonly long Cores;

        /// <summary>
        /// Constructor used for valid configurations
        /// </summary>
        /// <param name="sockets"></param>
        /// <param name="cores"></param>
        public TopologyTuple(long sockets, long cores)
        {
            this.Sockets = sockets;
            this.Cores = cores;
        }

        /// <summary>
        /// Constructor used for invalid cores per socket value
        /// </summary>
        /// <param name="cores"></param>
        public TopologyTuple(long cores)
        {
            this.Sockets = 0;
            this.Cores = cores;
        }

        public override string ToString()
        {
            return VM.GetTopology(Sockets, Cores);
        }
    }
}
