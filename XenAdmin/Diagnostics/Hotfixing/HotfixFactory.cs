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

using System;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Diagnostics.Hotfixing
{
    internal sealed class HotfixFactory
    {
        public enum HotfixableServerVersion
        {
            Clearwater,
            Creedence,
            Dundee,
            Ely
        }

        private readonly Hotfix clearwaterHotfix = new SingleHotfix
                                                         {
                                                             Filename = "RPU001",
                                                             UUID = "591d0209-531e-4ed8-9ed2-98df2a1a445c"
                                                         };

        private readonly Hotfix creedenceHotfix = new SingleHotfix
                                                         {
                                                             Filename = "RPU002",
                                                             UUID = "3f92b111-0a90-4ec6-b85a-737f241a3fc1 "
                                                         };

        private readonly Hotfix dundeeHotfix = new SingleHotfix
        {
            Filename = "RPU003",
            UUID = "474a0f28-0d33-4c9b-9e20-52baaea8ce5e"
        };

        private readonly Hotfix elyHotfix = new SingleHotfix
        {
            Filename = "RPU004",
            UUID = "b8d21409-adb7-44a5-a7ff-a78ed97162a5"
        };

        public Hotfix Hotfix(Host host)
        {
            if (Helpers.ElyOrGreater(host) && !Helpers.FalconOrGreater(host))
                return Hotfix(HotfixableServerVersion.Ely);
            if (Helpers.DundeeOrGreater(host) && !Helpers.ElyOrGreater(host))
                return Hotfix(HotfixableServerVersion.Dundee);
            if (Helpers.CreedenceOrGreater(host) && !Helpers.DundeeOrGreater(host))
                return Hotfix(HotfixableServerVersion.Creedence);
            if (!Helpers.CreedenceOrGreater(host))
                return Hotfix(HotfixableServerVersion.Clearwater);

            return null;
        }

        public Hotfix Hotfix(HotfixableServerVersion version)
        {
            if (version == HotfixableServerVersion.Ely)
                return elyHotfix;
            if (version == HotfixableServerVersion.Dundee)
                return dundeeHotfix; 
            if (version == HotfixableServerVersion.Creedence)
                return creedenceHotfix; 
            if (version == HotfixableServerVersion.Clearwater)
                return clearwaterHotfix;

            throw new ArgumentException("A version was provided for which there is no hotfix filename");
        }

        public bool IsHotfixRequired(Host host)
        {
            return Hotfix(host) != null;
        }
    }
}
