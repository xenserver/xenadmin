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
using System.Text;
using System.Net;

namespace XenAPI
{
    public partial class HTTP_actions
    {
        private static void Get(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, string remotePath, IWebProxy proxy, string localPath, params object[] args)
        {
            HTTP.Get(dataCopiedDelegate, cancellingDelegate, HTTP.BuildUri(hostname, remotePath, args), proxy, localPath, timeout_ms);
        }

        private static void Put(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, string remotePath, IWebProxy proxy, string localPath, params object[] args)
        {
            HTTP.Put(progressDelegate, cancellingDelegate, HTTP.BuildUri(hostname, remotePath, args), proxy, localPath, timeout_ms);
        }

        public static void get_services(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/services", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }

        public static void put_import(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, bool restore, bool force, string sr_id)
        {
            Put(progressDelegate, cancellingDelegate, timeout_ms, hostname, "/import", proxy, path,
                "task_id", task_id, "session_id", session_id, "restore", restore, "force", force, "sr_id", sr_id);
        }

        public static void put_import_metadata(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, bool restore, bool force)
        {
            Put(progressDelegate, cancellingDelegate, timeout_ms, hostname, "/import_metadata", proxy, path,
                "task_id", task_id, "session_id", session_id, "restore", restore, "force", force);
        }

        public static void put_import_raw_vdi(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string vdi)
        {
            Put(progressDelegate, cancellingDelegate, timeout_ms, hostname, "/import_raw_vdi", proxy, path,
                "task_id", task_id, "session_id", session_id, "vdi", vdi);
        }

        public static void get_export(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string uuid)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/export", proxy, path,
                "task_id", task_id, "session_id", session_id, "uuid", uuid);
        }

        public static void get_export_metadata(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string uuid)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/export_metadata", proxy, path,
                "task_id", task_id, "session_id", session_id, "uuid", uuid);
        }

        public static void get_export_raw_vdi(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string vdi)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/export_raw_vdi", proxy, path,
                "task_id", task_id, "session_id", session_id, "vdi", vdi);
        }

        public static void get_host_backup(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/host_backup", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }

        public static void put_host_restore(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Put(progressDelegate, cancellingDelegate, timeout_ms, hostname, "/host_restore", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }

        public static void get_host_logs_download(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/host_logs_download", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }

        public static void put_pool_patch_upload(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Put(progressDelegate, cancellingDelegate, timeout_ms, hostname, "/pool_patch_upload", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }

        public static void get_pool_patch_download(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string uuid)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/pool_patch_download", proxy, path,
                "task_id", task_id, "session_id", session_id, "uuid", uuid);
        }

        public static void put_oem_patch_stream(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Put(progressDelegate, cancellingDelegate, timeout_ms, hostname, "/oem_patch_stream", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }

        public static void get_vncsnapshot(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string uuid)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/vncsnapshot", proxy, path,
                "task_id", task_id, "session_id", session_id, "uuid", uuid);
        }

        public static void get_pool_xml_db_sync(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/pool/xmldbdump", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }

        public static void get_system_status(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string entries, string output)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/system-status", proxy, path,
                "task_id", task_id, "session_id", session_id, "entries", entries, "output", output);
        }

        public static void vm_rrd(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string uuid)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/vm_rrd", proxy, path,
                "task_id", task_id, "session_id", session_id, "uuid", uuid);
        }

        public static void host_rrd(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, bool json)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/host_rrd", proxy, path,
                "task_id", task_id, "session_id", session_id, "json", json);
        }

        public static void sr_rrd(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string uuid)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/sr_rrd", proxy, path,
                "task_id", task_id, "session_id", session_id, "uuid", uuid);
        }

        public static void rrd_updates(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, long start, string cf, long interval, bool host, string uuid, bool json)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/rrd_updates", proxy, path,
                "task_id", task_id, "session_id", session_id, "start", start, "cf", cf, "interval", interval, "host", host, "uuid", uuid, "json", json);
        }

        public static void put_blob(HTTP.UpdateProgressDelegate progressDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string reff)
        {
            Put(progressDelegate, cancellingDelegate, timeout_ms, hostname, "/blob", proxy, path,
                "task_id", task_id, "session_id", session_id, "ref", reff);
        }

        public static void get_wlb_report(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id, string report, params string[] args /* alternate names & values */)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/wlb_report", proxy, path,
                "task_id", task_id, "session_id", session_id, "report", report, args);
        }

        public static void get_wlb_diagnostics(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/wlb_diagnostics", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }

        public static void get_audit_log(HTTP.DataCopiedDelegate dataCopiedDelegate, HTTP.FuncBool cancellingDelegate, int timeout_ms,
            string hostname, IWebProxy proxy, string path, string task_id, string session_id)
        {
            Get(dataCopiedDelegate, cancellingDelegate, timeout_ms, hostname, "/audit_log", proxy, path,
                "task_id", task_id, "session_id", session_id);
        }
    }
}
