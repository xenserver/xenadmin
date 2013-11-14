/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin;

namespace XenAPI
{
    partial class VGPU_type : IComparable<VGPU_type>, IEquatable<VGPU_type>
    {
        public override string Name
        {
            get
            {
                if (max_heads == 0)
                    return Messages.VGPU_PASSTHRU_TOSTRING;

                return string.Format(Messages.VGPU_TOSTRING, model_name, Capacity);
            }

        }

        public override string Description
        {
            get
            {
                if (max_heads == 0)
                    return Messages.VGPU_PASSTHRU_TOSTRING;

                var videoRam = framebuffer_size != 0 ? Util.SuperiorSizeString(framebuffer_size, 0): string.Empty;
                return string.Format(Messages.VGPU_DESCRIPTION, model_name, Capacity, videoRam, max_heads); // videoRam TO BE REPLACED with max_resolution 
            }

        }
        
        #region IEquatable<VGPU_type> Members

        /// <summary>
        /// Indicates whether the current object is equal to the specified object.
        /// This calls the implementation from XenObject.
        /// This implementation is required for ToStringWrapper.
        /// </summary>
        public bool Equals(VGPU_type other)
        {
            return base.Equals(other);
        }

        #endregion
    
        /// <summary>
        /// vGPUs per GPU
        /// </summary>
        public long Capacity
        {
            get
            {
                long capacity = 0;
                if (supported_on_PGPUs != null && supported_on_PGPUs.Count > 0)
                {
                    PGPU pgpu = Connection.Resolve(supported_on_PGPUs[0]);
                    if (pgpu.supported_VGPU_max_capacities != null)
                    {
                        capacity = pgpu.supported_VGPU_max_capacities[new XenRef<VGPU_type>(this)];
                    }
                }
                return capacity;
            }
        }
        
    }
}
