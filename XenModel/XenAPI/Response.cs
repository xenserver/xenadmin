/* Copyright (c) Cloud Software Group, Inc.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 * 
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;

using CookComputing.XmlRpc;

namespace XenAPI
{
    public static class ResponseTypes
    {
        public const String SUCCESS = "Success";
        public const String FAILURE = "Failure";
    }

    public struct Response<ValType>
    {
        public string Status;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public ValType Value;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string[] ErrorDescription;

        public Response(ValType Value)
        {
            this.Status = ResponseTypes.SUCCESS;
            this.Value = Value;
            this.ErrorDescription = new String[] { };
        }

        public Response(bool Failure, String[] error)
        {
            this.Status = Failure ? ResponseTypes.FAILURE : ResponseTypes.SUCCESS;
            this.Value = default(ValType);
            this.ErrorDescription = error;
        }

        internal ValType parse()
        {
            if (ResponseTypes.SUCCESS.Equals(Status))
            {
                System.Diagnostics.Trace.Assert(Value != null, "Value must not be null");
                return Value;
            }
            else
            {
                if (ErrorDescription == null)
                {
                    List<string> error = new List<string>();
                    error.Add(Failure.INTERNAL_ERROR);
                    error.Add("Null ErrorDescription in response");
                    throw new Failure(error);
                }
                else
                {
                    throw new Failure(new List<string>(ErrorDescription));
                }
            }
        }
    }
}
