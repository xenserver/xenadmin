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

using System.Collections.Generic;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.DR
{
    public class GetMetadataVDIsAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<VDI> _vdis = new List<VDI>();
        private List<string> _restrictSrList;

        public GetMetadataVDIsAction(IXenConnection connection, List<string> restrictSrList)
            : base(connection, Messages.ACTION_GET_METADATA_VDIS_TITLE, null, true)
        {
            _restrictSrList = restrictSrList;

            Pool = Helpers.GetPoolOfOne(connection);
        }

        public List<VDI> VDIs
        {
            get
            {
                return _vdis;
            }
        }

        protected override void Run()
        {
            List<SR> srs = new List<SR>(Connection.Cache.SRs);
            if (srs.Count == 0)
                return;
            double increment = 100.0 / srs.Count;

            foreach (SR sr in srs)
            {
                if (!sr.shared || sr.IsToolsSR || sr.GetSRType(true) == SR.SRTypes.iso)
                    continue;

                if (_restrictSrList != null && !_restrictSrList.Contains(sr.uuid))
                    continue;

                Description = string.Format(Messages.ACTION_GET_METADATA_VDIS_STATUS, sr.Name);
                log.DebugFormat("Looking for metadata VDIs on SR {0}.", sr.Name);

                List<VDI> vdis = sr.Connection.ResolveAll(sr.VDIs);

                foreach (VDI vdi in vdis)
                {
                    if (vdi.type != vdi_type.metadata)
                        continue;

                    /*if (vdi.metadata_of_pool.opaque_ref == Pool.opaque_ref)
                    {
                        continue;
                    }*/

                    if (!vdi.metadata_latest) 
                        continue;
                    
                    _vdis.Add(vdi);
                    log.DebugFormat("Metadata VDI {0} found on SR {1}.", vdi.Name, sr.Name);
                }
                PercentComplete = (int) (PercentComplete + increment);
            }
            Description = string.Format(Messages.ACTION_GET_METADATA_VDIS_DONE, _vdis.Count);
        }
    }
}
