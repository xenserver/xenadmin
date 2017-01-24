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
using System.Text;
using XenAdmin.Core;
using XenAPI;
using System.Collections;
using XenAdmin.Network;

namespace XenAdmin.ServerDBs.FakeAPI
{
    internal class fakeSR : fakeXenObject
    {
        public fakeSR(DbProxy proxy)
            : base("sr", proxy)
        {

        }

        public Response<string> get_uuid(string session, string opaque_ref)
        {
            return new Response<string>((string)proxy.db.GetValue("sr", opaque_ref, "uuid"));
        }

        public Response<string> probe(string session, string host_ref, Hashtable device_config, string type, Hashtable sm_config)
        {
            switch (type)
            {
                case "lvmohba":
                    return new Response<string>(
@"<?xml version=""1.0"" ?> 
<BlockDevice>
  <Vendor>Vendor</Vendor>
  <Size>500GB</Size>
  <Serial>Serial</Serial>
  <Path>Path</Path>
  <SCSIID>1234-abcd-5678</SCSIID>
  <Adapter>1</Adapter>
  <Channel>2</Channel>
  <ID>3</ID>
  <LUN>4</LUN>
</BlockDevice>
");
                default:
                    return new Response<string>(@"<?xml version=""1.0"" ?>
<SRlist>
  <SR>
    <UUID>5fc2cbc4-8ae9-37aa-ab7e-a19fa24f3e91</UUID>
    <Size>197984256</Size>
    <Aggregate>SMAPI</Aggregate>
  </SR>
</SRlist>");
            }
        }

        public Response<string> assert_can_host_ha_statefile(string session, string opaque_ref)
        {
            return new Response<string>("");
        }

        public Response<string> create(string session, string host_ref, Hashtable device_config, string size, string label, string description, string type, string content, bool shared, Hashtable sm_config)
        {
            // For SrScanAction: fake data so we can step through the New SR Wizard
            Response<string> response = new Response<string>();
            if (label == Helpers.GuiTempObjectPrefix)  // fake create to find list of aggregates on an SR
            {
                response.Status = ResponseTypes.FAILURE;
                if (type == "netapp")
                    response.ErrorDescription = new string[] { "SR_BACKEND_FAILURE_123", "", "",
@"<?xml version=""1.0"" ?> 
<Aggrlist>
  <Aggr>
    <Name>SMAPI</Name> 
    <Size>609550192640</Size> 
    <Disks>6</Disks> 
    <RAIDType>raid4</RAIDType> 
    <asis_dedup>True</asis_dedup> 
  </Aggr>
</Aggrlist>" };
                else
                    // I haven't managed to find out what a real Dell EqualLogic response looks like,
                    // but this is good enough to make the New SR Wizard work.
                    response.ErrorDescription = new string[] { "SR_BACKEND_FAILURE_163", "", "",
@"<?xml version=""1.0"" ?> 
<Aggrlist>
  <StoragePool>
    <Name>SMAPI</Name>
    <Default>true</Default>
    <Members>2</Members>
    <Volumes>2</Volumes>
    <Capacity>609550192640</Capacity>
    <FreeSpace>532573377854</FreeSpace> 
  </StoragePool>
</Aggrlist>"};
            }

            return response;
        }
    }
}
