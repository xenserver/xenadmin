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
using XenAdmin.Wizards.GenericPages;
using XenOvf;
using XenOvf.Definitions;

namespace XenAdmin.Wizards.ImportWizard
{
    internal class OvfNetworkResource : INetworkResource
    {
        private readonly RASD_Type rasd;

        public OvfNetworkResource(RASD_Type rasdType)
        {
            rasd = rasdType;
        }

        public string NetworkName
        {
            get
            {
                if (rasd.ElementName != null && !string.IsNullOrEmpty(rasd.ElementName.Value))
                    return rasd.ElementName.Value;
                
                if (rasd.Connection != null && rasd.Connection.Length > 0 && !string.IsNullOrEmpty(rasd.Connection[0].Value))
                    return rasd.Connection[0].Value;

                return Messages.NONE;
            }
        }

        public string MACAddress
        {
            get
            {
                return (rasd.Address == null || String.IsNullOrEmpty(rasd.Address.Value))
                     ? Messages.NEWVMWIZARD_NETWORKINGPAGE_AUTOGEN
                     : rasd.Address.Value;
            }
        }

        public string NetworkID
        {
            get { return rasd.InstanceID == null ? null : rasd.InstanceID.Value; }
        }
    }

    internal class OvfNetworkResourceContainer : NetworkResourceContainer
    {
        private readonly RASD_Type[] rasdArray;
        private int counter;

        public OvfNetworkResourceContainer(string sysId, EnvelopeType envelopeType)
        {
            rasdArray = OVF.FindRasdByType(envelopeType, sysId, 10);
        }

        public override INetworkResource Next()
        {
            INetworkResource res = new OvfNetworkResource(rasdArray[counter]);
            counter++;
            return res;
        }

        public override bool IsNext
        {
            get { return counter < rasdArray.Length; }
        }
    }
}
