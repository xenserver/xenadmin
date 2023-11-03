/* Copyright (c) Cloud Software Group, Inc.
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

using System.Linq;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions.Updates
{
    public class RpuHotfix
    {
        private static readonly RpuHotfix StockholmHotfix = new RpuHotfix("RPU005", "38a7eeeb-31ec-4de3-934f-13929ad3e339");

        private RpuHotfix(string name, string uuid)
        {
            Name = name;
            Uuid = uuid;
        }

        public string Name { get; }
        public string Uuid { get; }

        public bool ShouldBeAppliedTo(Host host)
        {
            var updates = host.Connection.ResolveAll(host.updates);
            return !updates.Any(update => Uuid.ToLowerInvariant().Contains(update.uuid.ToLowerInvariant()));
        }

        public static bool Exists(Host host, out RpuHotfix hotfix)
        {
            if (Helpers.StockholmOrGreater(host) && !Helpers.CloudOrGreater(host))
            {
                hotfix = StockholmHotfix;
                return true;
            }

            hotfix = null;
            return false;
        }
    }
}
