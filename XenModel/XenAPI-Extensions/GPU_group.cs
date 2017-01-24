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
using System.Linq;
using XenAdmin;

namespace XenAPI
{
    partial class GPU_group : IComparable<GPU_group>, IEquatable<GPU_group>
    {
        public override string Name
        {
            get
            {
                string name = name_label;
                if (name.StartsWith("Group of "))
                    name = name.Substring(9);
                return name;
            }
        }

        public override string ToString()
        {
            return PGPUs.Count == 1 
                ? String.Format(Messages.GPU_GROUP_NAME_AND_NO_OF_GPUS_ONE, Name)
                : String.Format(Messages.GPU_GROUP_NAME_AND_NO_OF_GPUS, Name, PGPUs.Count);
        }

        public bool HasVGpu
        {
            get
            {
                return Connection.ResolveAll(PGPUs).Any(pgpu => pgpu.HasVGpu);
            }
        }


        #region IEquatable<GPU_group> Members

        /// <summary>
        /// Indicates whether the current object is equal to the specified object.
        /// This calls the implementation from XenObject.
        /// This implementation is required for ToStringWrapper.
        /// </summary>
        public bool Equals(GPU_group other)
        {
            return base.Equals(other);
        }

        #endregion
    }
}
