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
using XenAdmin.CustomFields;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin
{
    /// <summary>
    /// Wrap a XenObject and the custom field identifier.
    /// </summary>
    public class CustomFieldWrapper : IComparable<CustomFieldWrapper>, IEquatable<CustomFieldWrapper>
    {
        private readonly IXenObject o;
        private readonly CustomFieldDefinition customFieldDefinition;

        public CustomFieldWrapper(IXenObject o, CustomFieldDefinition customFieldDefinition)
        {
            this.o = o;
            this.customFieldDefinition = customFieldDefinition;
        }

        /// <summary>
        /// Format the value of a custom field for a XenObject for display to the user (localised if necessary).
        /// </summary>
        public override string ToString()
        {
            // Must be run on the event thread, otherwise dates won't be formatted correctly (CA-46983).
            Program.AssertOnEventThread();
            object value = CustomFieldsManager.GetCustomFieldValue(o, customFieldDefinition);
            return (value == null)
                       ? ""
                       : (customFieldDefinition.Type == CustomFieldDefinition.Types.Date)
                             ? HelpersGUI.DateTimeToString((DateTime)value, Messages.DATEFORMAT_DMY_HM, true)
                             : value.ToString();
        }

        public bool Equals(CustomFieldWrapper other)
        {
            return customFieldDefinition.Equals(other.customFieldDefinition) && o.opaque_ref.Equals(other.o.opaque_ref);
        }

        public int CompareTo(CustomFieldWrapper other)
        {
            return StringUtility.NaturalCompare(ToString(), other.ToString());
        }
    }
}