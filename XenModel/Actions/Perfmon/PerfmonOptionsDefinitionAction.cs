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

using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class PerfmonOptionsDefinitionAction : AsyncAction
    {
        private readonly Pool pool;
        private readonly PerfmonOptionsDefinition perfmonOptions;

        public PerfmonOptionsDefinitionAction(IXenConnection connection, PerfmonOptionsDefinition perfmonOptions, bool suppressHistory)
            : base(connection, Messages.ACTION_CHANGE_EMAIL_OPTIONS, suppressHistory)
        {
            this.perfmonOptions = perfmonOptions;
            pool = Helpers.GetPoolOfOne(connection);
            Description = string.Format(Messages.ACTION_CHANGING_EMAIL_OPTIONS_FOR, pool);

            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_other_config", PerfmonOptionsDefinition.MAIL_DESTINATION_KEY_NAME);
            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_other_config", PerfmonOptionsDefinition.SMTP_MAILHUB_KEY_NAME);
            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_other_config", PerfmonOptionsDefinition.MAIL_LANGUAGE_KEY_NAME);

            if (perfmonOptions != null)
            {
                ApiMethodsToRoleCheck.AddWithKey("pool.add_to_other_config", PerfmonOptionsDefinition.MAIL_DESTINATION_KEY_NAME);
                ApiMethodsToRoleCheck.AddWithKey("pool.add_to_other_config", PerfmonOptionsDefinition.SMTP_MAILHUB_KEY_NAME);
                
                if (perfmonOptions.MailLanguageCode != null)
                    ApiMethodsToRoleCheck.AddWithKey("pool.add_to_other_config", PerfmonOptionsDefinition.MAIL_LANGUAGE_KEY_NAME);
            }
        }

        protected override void Run()
        {
            if (pool == null)
                return;

            Pool.remove_from_other_config(Session, pool.opaque_ref, PerfmonOptionsDefinition.MAIL_DESTINATION_KEY_NAME);

            if (perfmonOptions != null)
                Pool.add_to_other_config(Session, pool.opaque_ref, PerfmonOptionsDefinition.MAIL_DESTINATION_KEY_NAME, perfmonOptions.MailDestination);

            Pool.remove_from_other_config(Session, pool.opaque_ref, PerfmonOptionsDefinition.SMTP_MAILHUB_KEY_NAME);
            
            if (perfmonOptions != null)
                Pool.add_to_other_config(Session, pool.opaque_ref, PerfmonOptionsDefinition.SMTP_MAILHUB_KEY_NAME, perfmonOptions.MailHub);

            Pool.remove_from_other_config(Session, pool.opaque_ref, PerfmonOptionsDefinition.MAIL_LANGUAGE_KEY_NAME);

            if (perfmonOptions?.MailLanguageCode != null)
                Pool.add_to_other_config(Session, pool.opaque_ref, PerfmonOptionsDefinition.MAIL_LANGUAGE_KEY_NAME, perfmonOptions.MailLanguageCode);
        }
    }
}
