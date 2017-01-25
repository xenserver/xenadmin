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
    public enum vdi_type
    {
        system, user, ephemeral, suspend, crashdump, ha_statefile, metadata, redo_log, rrd, pvs_cache, unknown
    }

    public static class vdi_type_helper
    {
        public static string ToString(vdi_type x)
        {
            switch (x)
            {
                case vdi_type.system:
                    return "system";
                case vdi_type.user:
                    return "user";
                case vdi_type.ephemeral:
                    return "ephemeral";
                case vdi_type.suspend:
                    return "suspend";
                case vdi_type.crashdump:
                    return "crashdump";
                case vdi_type.ha_statefile:
                    return "ha_statefile";
                case vdi_type.metadata:
                    return "metadata";
                case vdi_type.redo_log:
                    return "redo_log";
                case vdi_type.rrd:
                    return "rrd";
                case vdi_type.pvs_cache:
                    return "pvs_cache";
                default:
                    return "unknown";
            }
        }
    }
}
