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
using XenAPI;

namespace XenAdmin.Diagnostics.Hotfixing
{
    internal class MultipleHotfix : Hotfix
    {
        public List<Hotfix> ComponentHotfixes { get; set; }

        /// <summary>
        /// Concatenated string of sorted UUIDs separated with a ";"
        /// Use String.Contains() to look up if the individual UUID is present
        /// </summary>
        public override string UUID
        {
            get
            {
                List<string> uuids = ComponentHotfixes.Select(hotfix => hotfix.UUID).ToList();
                return String.Join(";", uuids.ToArray());
            }
            set { throw new NotImplementedException("Multiple hotfix string does not implement a setter"); }
        }

        /// <summary>
        /// Concatenated string of sorted file names separated with a ";"
        /// Use String.Contains() to look up if the individual file name is present
        /// </summary>
        public override string Filename
        {
            get
            {
                List<string> filenames = ComponentHotfixes.Select(h => h.Filename).ToList();
                return String.Join(";", filenames.ToArray());
            }
            set { throw new NotImplementedException("Multiple hotfix string does not implement a setter"); }
        }

        public sealed override void Apply(Host host, Session session)
        {
            foreach (Hotfix hotfix in ComponentHotfixes)
            {
                if(hotfix.ShouldBeAppliedTo(host))
                    hotfix.Apply(host, session);
            }
        }

        public override bool ShouldBeAppliedTo(Host host)
        {
            return ComponentHotfixes.Any(hf => hf.ShouldBeAppliedTo(host));
        }
    }
}
