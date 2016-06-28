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
    public enum storage_operations
    {
        scan, destroy, forget, plug, unplug, update, vdi_create, vdi_introduce, vdi_destroy, vdi_resize, vdi_clone, vdi_snapshot, vdi_mirror, pbd_create, pbd_destroy, unknown
    }

    public static class storage_operations_helper
    {
        public static string ToString(storage_operations x)
        {
            switch (x)
            {
                case storage_operations.scan:
                    return "scan";
                case storage_operations.destroy:
                    return "destroy";
                case storage_operations.forget:
                    return "forget";
                case storage_operations.plug:
                    return "plug";
                case storage_operations.unplug:
                    return "unplug";
                case storage_operations.update:
                    return "update";
                case storage_operations.vdi_create:
                    return "vdi_create";
                case storage_operations.vdi_introduce:
                    return "vdi_introduce";
                case storage_operations.vdi_destroy:
                    return "vdi_destroy";
                case storage_operations.vdi_resize:
                    return "vdi_resize";
                case storage_operations.vdi_clone:
                    return "vdi_clone";
                case storage_operations.vdi_snapshot:
                    return "vdi_snapshot";
                case storage_operations.vdi_mirror:
                    return "vdi_mirror";
                case storage_operations.pbd_create:
                    return "pbd_create";
                case storage_operations.pbd_destroy:
                    return "pbd_destroy";
                default:
                    return "unknown";
            }
        }
    }
}
