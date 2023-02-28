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
    /// Adds a new custom field definition in the pool.
    /// </summary>
    public class AddCustomFieldAction : AsyncAction
    {
        private readonly CustomFieldDefinition _definition;

        public AddCustomFieldAction(IXenConnection connection, CustomFieldDefinition definition)
            : base(connection, string.Format(Messages.ADD_CUSTOM_FIELD, definition.Name),
                string.Format(Messages.ADDING_CUSTOM_FIELD, definition.Name), false)
        {
            _definition = definition;

            ApiMethodsToRoleCheck.AddWithKey("pool.add_to_gui_config", CustomFieldsManager.CUSTOM_FIELD_BASE_KEY);
            ApiMethodsToRoleCheck.AddWithKey("pool.remove_from_gui_config", CustomFieldsManager.CUSTOM_FIELD_BASE_KEY);
        }

        protected override void Run()
        {
            CustomFieldsManager.AddCustomField(Session, Connection, _definition);
            Description = string.Format(Messages.ADDED_CUSTOM_FIELD, _definition.Name);
        }
    }
}
