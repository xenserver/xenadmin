/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
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


namespace XenAPI
{
    public enum vdi_operations
    {
        scan, clone, copy, resize, resize_online, snapshot, mirror, destroy, forget, update, force_unlock, generate_config, blocked, unknown
    }

    public static class vdi_operations_helper
    {
        public static string ToString(vdi_operations x)
        {
            switch (x)
            {
                case vdi_operations.scan:
                    return "scan";
                case vdi_operations.clone:
                    return "clone";
                case vdi_operations.copy:
                    return "copy";
                case vdi_operations.resize:
                    return "resize";
                case vdi_operations.resize_online:
                    return "resize_online";
                case vdi_operations.snapshot:
                    return "snapshot";
                case vdi_operations.mirror:
                    return "mirror";
                case vdi_operations.destroy:
                    return "destroy";
                case vdi_operations.forget:
                    return "forget";
                case vdi_operations.update:
                    return "update";
                case vdi_operations.force_unlock:
                    return "force_unlock";
                case vdi_operations.generate_config:
                    return "generate_config";
                case vdi_operations.blocked:
                    return "blocked";
                default:
                    return "unknown";
            }
        }
    }
}
