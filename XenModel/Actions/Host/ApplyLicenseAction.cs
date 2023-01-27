﻿/* Copyright (c) Cloud Software Group, Inc. 
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
using System.IO;
using XenAPI;

namespace XenAdmin.Actions
{
    public class ApplyLicenseAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly String Filepath;

        public ApplyLicenseAction(Host host, string filepath)
            : base(host.Connection, string.Format(Messages.APPLYLICENSE_TITLE, host.Name()), Messages.APPLYLICENSE_PREP)
        {
            this.Host = host;
            this.Filepath = filepath;
        }

        protected override void Run()
        {
            SafeToExit = false;
            if (Connection.IsConnected)
            {
                if (!File.Exists(Filepath))
                {
                    throw new Exception(Messages.LICENSE_FILE_DOES_NOT_EXIST);
                }
                if (new FileInfo(Filepath).Length > 1048576)
                {
                    throw new Exception(Messages.LICENSE_FILE_TOO_LARGE);
                }

                string encodedContent = Convert.ToBase64String(File.ReadAllBytes(Filepath));

                Description = string.Format(Messages.APPLYLICENSE_APPLYING, Filepath);
                log.DebugFormat("Applying license to server {0}", Host.Name());
                RelatedTask = Host.async_license_apply(Session, Host.opaque_ref, encodedContent);
                PollToCompletion();
                Description = Messages.APPLYLICENSE_APPLIED;
            }
        }
    }
}
