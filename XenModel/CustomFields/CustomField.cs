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
using System.Globalization;

namespace XenAdmin.CustomFields
{
    public class CustomField
    {
        private readonly CustomFieldDefinition definition;
        private readonly Object value;

        public CustomField(CustomFieldDefinition definition, Object value)
        {
            this.definition = definition;
            this.value = value;
        }

        public CustomFieldDefinition Definition
        {
            get { return definition; }
        }

        public Object Value
        {
            get { return value; }
        }

        public override bool Equals(object obj)
        {
            CustomField other = obj as CustomField;

            return other != null && other.definition.Equals(definition) && other.Value.Equals(Value);
        }

        public override int GetHashCode()
        {
            return definition.GetHashCode();
        }

        public override string ToString()
        {
            return definition.ToString();
        }

        public string ValueAsInvariantString
        {
            get
            {
                if (value == null)
                    return null;

                if (definition.Type == CustomFieldDefinition.Types.Date)
                {
                    DateTime d = (DateTime)value;
                    return d.ToUniversalTime().ToString("u", CultureInfo.InvariantCulture); // CP-2036, CA-28123, CA-30587
                }

                return value.ToString();
            }
        }

        public object[] ToArray()
        {
            return new object[2] { definition.Name, value };
        }
    }
}