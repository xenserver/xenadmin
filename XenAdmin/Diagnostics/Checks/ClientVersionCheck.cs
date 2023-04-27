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

using System.Collections.Generic;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    public class ClientVersionCheck : Check
    {
        private readonly XenServerVersion _newServerVersion;

        public ClientVersionCheck(XenServerVersion newServerVersion)
        {
            _newServerVersion = newServerVersion;
        }
        
        protected override Problem RunCheck()
        {
            var requiredClientVersion = Updates.GetRequiredClientVersion(_newServerVersion);
            if (requiredClientVersion == null) 
                return null;
            if (_newServerVersion != null) 
                return new ClientVersionProblem(this, requiredClientVersion);
            else
                return new ClientVersionWarning(this, requiredClientVersion);
        }

        public override string Description => string.Format(Messages.XENCENTER_VERSION_CHECK_DESCRIPTION, BrandManager.BrandConsole);

        public override IList<IXenObject> XenObjects => null;
    }
}
