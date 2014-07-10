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
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Diagnostics.Hotfixing
{
    internal sealed class HotfixFactory
    {
        public enum HotfixableServerVersion
        {
            MNR,
            Cowley,
            Boston,
            SanibelToClearwater
        }

        private readonly Hotfix mnrHotfix = new SingleHotfix
                                       {
                                           Filename = "XS56E008.xsupdate",
                                           UUID = "e2cb047b-66ed-4fa0-882a-67ff1726f4b9"
                                       };

        private readonly Hotfix cowleyHotfix = new SingleHotfix
                                       {
                                           Filename = "XS56EFP1002.xsupdate",
                                           UUID = "ca0ca2c6-cc96-4e4b-946b-39ebe6652fd6"
                                       };

        private readonly Hotfix bostonHotfix = new MultipleHotfix()
                                        {
                                            ComponentHotfixes = new List<Hotfix>
                                                     {
                                                          new SingleHotfix
                                                         {
                                                             Filename = "XS60E001.xsupdate",
                                                             UUID = "95ac709c-e408-423f-8d22-84b8134a149e"
                                                         },
                                                          new SingleHotfix
                                                         {
                                                             Filename = "XS62E006.xsupdate",
                                                             UUID = "b412a910-0453-42ed-bae0-982cc48b00d6"
                                                         }
                                                     }
                                        };

        private readonly Hotfix sanibelToClearwaterHotfix = new SingleHotfix
                                                         {
                                                             Filename = "XS62E006.xsupdate",
                                                             UUID = "b412a910-0453-42ed-bae0-982cc48b00d6"
                                                         };

        public Hotfix Hotfix(Host host)
        {
            if (Helpers.SanibelOrGreater(host) && !Helpers.CreedenceOrGreater(host))
                return Hotfix(HotfixableServerVersion.SanibelToClearwater);
            if (Helpers.BostonOrGreater(host) && !Helpers.SanibelOrGreater(host))
                return Hotfix(HotfixableServerVersion.Boston);
            if (Helpers.CowleyOrGreater(host) && !Helpers.BostonOrGreater(host))
                return Hotfix(HotfixableServerVersion.Cowley);
            if (Helpers.MidnightRideOrGreater(host) && !Helpers.CowleyOrGreater(host))
                return Hotfix(HotfixableServerVersion.MNR);

            return null;
        }

        public Hotfix Hotfix(HotfixableServerVersion version)
        {
            if (version == HotfixableServerVersion.SanibelToClearwater)
                return sanibelToClearwaterHotfix;
            if (version == HotfixableServerVersion.Boston)
                return bostonHotfix;
            if (version == HotfixableServerVersion.Cowley)
                return cowleyHotfix;
            if (version == HotfixableServerVersion.MNR)
                return mnrHotfix;

            throw new ArgumentException("A version was provided for which there is no hotfix filename");
        }

        public bool IsHotfixRequired(Host host)
        {
            return Hotfix(host) != null;
        }
    }
}
