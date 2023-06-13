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

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class PerfmonOptionsDefinitionAction : AsyncAction
    {
        private readonly Pool _pool;
        private readonly string _mailDestination;
        private readonly string _mailHub;
        private readonly string _mailLangCode;

        public PerfmonOptionsDefinitionAction(IXenConnection connection, string mailDestination, string mailHub, string mailLangCode, bool suppressHistory)
            : base(connection, Messages.ACTION_CHANGE_EMAIL_OPTIONS, suppressHistory)
        {
            _mailDestination = mailDestination;
            _mailHub = mailHub;
            _mailLangCode = mailLangCode;
            _pool = Helpers.GetPoolOfOne(connection);

            Description = string.Format(Messages.ACTION_CHANGING_EMAIL_OPTIONS_FOR, _pool);

            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_other_config", Pool.MAIL_DESTINATION_KEY_NAME);
            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_other_config", Pool.SMTP_MAILHUB_KEY_NAME);
            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_other_config", Pool.MAIL_LANGUAGE_KEY_NAME);

            if (_mailDestination != null && _mailHub != null)
            {
                ApiMethodsToRoleCheck.AddWithKey("pool.add_to_other_config", Pool.MAIL_DESTINATION_KEY_NAME);
                ApiMethodsToRoleCheck.AddWithKey("pool.add_to_other_config", Pool.SMTP_MAILHUB_KEY_NAME);
                
                if (_mailLangCode != null)
                    ApiMethodsToRoleCheck.AddWithKey("pool.add_to_other_config", Pool.MAIL_LANGUAGE_KEY_NAME);
            }
        }

        protected override void Run()
        {
            if (_pool == null)
                return;

            Pool.remove_from_other_config(Session, _pool.opaque_ref, Pool.MAIL_DESTINATION_KEY_NAME);

            if (_mailDestination != null)
                Pool.add_to_other_config(Session, _pool.opaque_ref, Pool.MAIL_DESTINATION_KEY_NAME, _mailDestination);

            Pool.remove_from_other_config(Session, _pool.opaque_ref, Pool.SMTP_MAILHUB_KEY_NAME);
            
            if (_mailHub != null)
                Pool.add_to_other_config(Session, _pool.opaque_ref, Pool.SMTP_MAILHUB_KEY_NAME, _mailHub);

            Pool.remove_from_other_config(Session, _pool.opaque_ref, Pool.MAIL_LANGUAGE_KEY_NAME);

            if (_mailLangCode != null)
                Pool.add_to_other_config(Session, _pool.opaque_ref, Pool.MAIL_LANGUAGE_KEY_NAME, _mailLangCode);
        }
    }
}
