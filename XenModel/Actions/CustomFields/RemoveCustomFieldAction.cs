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

using XenAdmin.CustomFields;
using XenAdmin.Network;

namespace XenAdmin.Actions
{
    /// <summary>
    /// Removes a custom field definition from the pool.
    /// </summary>
    public class RemoveCustomFieldAction : AsyncAction
    {
        private readonly CustomFieldDefinition _definition;

        public RemoveCustomFieldAction(IXenConnection connection, CustomFieldDefinition definition)
            : base(connection, string.Format(Messages.DELETE_CUSTOM_FIELD, definition.Name),
                string.Format(Messages.DELETING_CUSTOM_FIELD, definition.Name), false)
        {
            _definition = definition;

            string key = CustomFieldsManager.GetCustomFieldKey(definition);

            ApiMethodsToRoleCheck.AddWithKey("pool.add_to_gui_config", CustomFieldsManager.CUSTOM_FIELD_BASE_KEY);
            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_gui_config", CustomFieldsManager.CUSTOM_FIELD_BASE_KEY);
            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_other_config", key);
            ApiMethodsToRoleCheck.AddWithKey("host.remove_from_other_config", key);
            ApiMethodsToRoleCheck.AddWithKey("VM.remove_from_other_config", key);
            ApiMethodsToRoleCheck.AddWithKey("SR.remove_from_other_config", key);
            ApiMethodsToRoleCheck.AddWithKey("VDI.remove_from_other_config", key);
            ApiMethodsToRoleCheck.AddWithKey("Network.remove_from_other_config", key);
        }

        protected override void Run()
        {
            CustomFieldsManager.RemoveCustomField(Session, Connection, _definition);
            Description = string.Format(Messages.DELETED_CUSTOM_FIELD, _definition.Name);
        }
    }
}
