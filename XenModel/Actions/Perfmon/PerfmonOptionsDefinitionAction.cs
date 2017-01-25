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
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class PerfmonOptionsDefinitionAction : PureAsyncAction
    {
        private readonly Pool pool;
        private readonly PerfmonOptionsDefinition perfmonOptions;

        public PerfmonOptionsDefinitionAction(IXenConnection connection, PerfmonOptionsDefinition perfmonOptions, bool suppressHistory)
            : base(connection, Messages.ACTION_CHANGE_EMAIL_OPTIONS, suppressHistory)
        {
            this.perfmonOptions = perfmonOptions;
            pool = Helpers.GetPoolOfOne(connection);
            this.Description = string.Format(Messages.ACTION_CHANGING_EMAIL_OPTIONS_FOR, pool);
        }

        protected override void Run()
        {
            if (pool == null)
                return;

            if (perfmonOptions == null)
            {
                Helpers.RemoveFromOtherConfig(Session, pool, PerfmonOptionsDefinition.MAIL_DESTINATION_KEY_NAME);
                Helpers.RemoveFromOtherConfig(Session, pool, PerfmonOptionsDefinition.SMTP_MAILHUB_KEY_NAME);
            }
            else
            {
                Helpers.SetOtherConfig(Session, pool, PerfmonOptionsDefinition.MAIL_DESTINATION_KEY_NAME, perfmonOptions.MailDestination);
                Helpers.SetOtherConfig(Session, pool, PerfmonOptionsDefinition.SMTP_MAILHUB_KEY_NAME, perfmonOptions.MailHub);
            }
        }
    }
}
