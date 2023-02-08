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
using XenAdmin.CustomFields;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class SaveCustomFieldsAction : AsyncAction
    {
        private readonly IXenObject xenObject;
        private readonly List<CustomField> customFields;

        public SaveCustomFieldsAction(IXenObject xenObject, List<CustomField> customFields, bool suppressHistory)
            : base(xenObject.Connection, Messages.ACTION_SAVE_CUSTOM_FIELDS, string.Format(Messages.ACTION_SAVING_CUSTOM_FIELDS_FOR, xenObject), suppressHistory)
        {
            this.xenObject = xenObject;
            this.customFields = customFields;

            var name = xenObject.GetType().Name;

            foreach (CustomField customField in customFields)
            {
                string key = CustomFieldsManager.GetCustomFieldKey(customField.Definition);
                string value = customField.ValueAsInvariantString;

                ApiMethodsToRoleCheck.AddWithKey($"{name}.remove_from_other_config", key);

                if (!string.IsNullOrEmpty(value))
                    ApiMethodsToRoleCheck.AddWithKey($"{name}.add_to_other_config", key);
            }
        }

        protected override void Run()
        {
            foreach (CustomField customField in customFields)
            {
                string key = CustomFieldsManager.GetCustomFieldKey(customField.Definition);
                string value = customField.ValueAsInvariantString;

                if (string.IsNullOrEmpty(value))
                    Helpers.RemoveFromOtherConfig(Session, xenObject, key);
                else
                    Helpers.SetOtherConfig(Session, xenObject, key, value);
            }
        }
    }
}
