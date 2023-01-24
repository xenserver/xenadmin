/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    public class InstallCertificateAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _privateKeyFile;
        private readonly string _certificateFile;
        private readonly List<string> _chainFiles;
        private readonly Func<string, string> _dateConverter;
        private readonly string _hostRef;
        private readonly bool _isCoordinator;
        private readonly string _oldCertificateUuid;
        private volatile Session _session;

        public event Action<IXenConnection> RequestReconnection;

        public InstallCertificateAction(Host host, string privateKeyFile, string certificateFile,
            List<string> chainFiles, Func<string, string> dateConverter)
            : base(host.Connection, string.Format(Messages.INSTALL_SERVER_CERTIFICATE_TITLE, host.Name()),
                Messages.INSTALL_SERVER_CERTIFICATE_DESCRIPTION, true)
        {
            Host = host;
            _hostRef = host.opaque_ref;
            _isCoordinator = host.IsCoordinator();

            if (host.certificates != null && host.certificates.Count > 0)
            {
                var cert = host.Connection.Resolve(host.certificates[0]);
                if (cert != null)
                    _oldCertificateUuid = cert.uuid;
            }

            _privateKeyFile = privateKeyFile;
            _certificateFile = certificateFile;
            _chainFiles = chainFiles ?? new List<string>();
            _dateConverter = dateConverter;

            SetRbacPermissions();
        }

        public string KeyError { get; private set; }
        public string CertificateError { get; private set; }
        public string ChainError { get; private set; }

        private void SetRbacPermissions()
        {
            AddCommonAPIMethodsToRoleCheck();
            ApiMethodsToRoleCheck.Add("host.install_server_certificate");
        }

        private void CollectFileContents(out string privateKey, out string certificate, out string chain)
        {
            int fileCount = 2 + _chainFiles.Count;
            int i = 0;

            if (!File.Exists(_privateKeyFile))
            {
                KeyError = Messages.PATH_DOES_NOT_EXIST;
                throw new IOException();
            }

            try
            {
                privateKey = File.ReadAllText(_privateKeyFile);
            }
            catch
            {
                KeyError = Messages.CERTIFICATE_KEY_INVALID;
                throw;
            }

            PercentComplete = ++i * 30 / fileCount;

            if (Cancelling)
                throw new CancelledException();

            if (!File.Exists(_certificateFile))
            {
                CertificateError = Messages.PATH_DOES_NOT_EXIST;
                throw new IOException();
            }

            try
            {
                certificate = File.ReadAllText(_certificateFile);
            }
            catch
            {
                KeyError = Messages.CERTIFICATE_INVALID;
                throw;
            }

            PercentComplete = ++i * 30 / fileCount;

            if (Cancelling)
                throw new CancelledException();

            var chainFiles = new List<string>();

            foreach (var file in _chainFiles)
            {
                if (!File.Exists(file))
                {
                    ChainError = string.Format(Messages.PATH_DOES_NOT_EXIST_PLACEHOLDER, file);
                    throw new IOException();
                }

                try
                {
                    var content = File.ReadAllText(file);
                    chainFiles.Add(content);
                }
                catch
                {
                    ChainError = string.Format(Messages.CERTIFICATE_INVALID, file);
                    throw;
                }

                PercentComplete = ++i * 30 / fileCount;

                if (Cancelling)
                    throw new CancelledException();
            }

            chain = string.Join("\n", chainFiles.ToArray());
        }

        private void WaitForNewCertificate()
        {
            log.Info("Waiting for new certificate uuid...");

            var startTime = DateTime.Now;
            string newUuid = null;

            do
            {
                if (Cancelling)
                    throw new CancelledException();

                var resolvedHost = Host.get_record(_session, _hostRef);
                if (resolvedHost != null && resolvedHost.certificates != null && resolvedHost.certificates.Count > 0)
                {
                    var cert = Certificate.get_record(_session, resolvedHost.certificates[0]);
                    newUuid = cert.uuid;
                }

                Thread.Sleep(3000);

                var span = DateTime.Now - startTime;

                if ((int)span.TotalSeconds % 60 == 0)
                    log.InfoFormat("Been waiting for new certificate uuid for {0:0.0}sec...", span.TotalSeconds);
            } while (newUuid == _oldCertificateUuid);

            log.Info("New certificate uuid found");
            Description = Messages.CERTIFICATE_INSTALLATION_SUCCESS;
        }

        protected override void Run()
        {
            CollectFileContents(out string privateKey, out string certificate, out string chain);

            var host = Connection.Resolve(new XenRef<Host>(_hostRef));
            if (host == null || !host.IsLive())
                throw new Failure(Messages.HOST_UNREACHABLE);

            try
            {
                Connection.ConnectionStateChanged -= ConnectionStateChanged;
                Connection.ConnectionStateChanged += ConnectionStateChanged;
                Connection.ExpectDisruption = true;

                try
                {
                    _session = Session;
                    Host.install_server_certificate(_session, _hostRef, certificate, privateKey, chain);
                    PercentComplete = 50;
                    WaitForNewCertificate();
                }
                catch (WebException)
                {
                    ConnectionStateChanged(Connection);
                }
            }
            catch (Failure f)
            {
                if (f.ErrorDescription.Count > 0)
                    switch (f.ErrorDescription[0])
                    {
                        case "SERVER_CERTIFICATE_KEY_ALGORITHM_NOT_SUPPORTED":
                        case "SERVER_CERTIFICATE_KEY_INVALID":
                        case "SERVER_CERTIFICATE_KEY_MISMATCH":
                        case "SERVER_CERTIFICATE_KEY_RSA_LENGTH_NOT_SUPPORTED":
                        case "SERVER_CERTIFICATE_KEY_RSA_MULTI_NOT_SUPPORTED":
                            KeyError = f.Message;
                            break;
                        case "SERVER_CERTIFICATE_CHAIN_INVALID":
                            ChainError = f.Message;
                            break;
                        case "SERVER_CERTIFICATE_NOT_VALID_YET":
                            CertificateError = f.ErrorDescription.Count > 2 && _dateConverter != null
                                ? string.Format(FriendlyErrorNames.SERVER_CERTIFICATE_NOT_VALID_YET, _dateConverter(f.ErrorDescription[2]))
                                : f.Message;
                            break;
                        case "SERVER_CERTIFICATE_EXPIRED":
                            CertificateError = f.ErrorDescription.Count > 2 && _dateConverter != null
                                ? string.Format(FriendlyErrorNames.SERVER_CERTIFICATE_EXPIRED, _dateConverter(f.ErrorDescription[2]))
                                : f.Message;
                            break;
                        case "SERVER_CERTIFICATE_INVALID":
                        case "SERVER_CERTIFICATE_SIGNATURE_NOT_SUPPORTED":
                            CertificateError = f.Message;
                            break;
                    }

                throw;
            }
            finally
            {
                Connection.ConnectionStateChanged -= ConnectionStateChanged;
                PercentComplete = 100;
            }
        }

        protected override void Clean()
        {
            Connection.ExpectDisruption = false;
        }

        private void ConnectionStateChanged(IXenConnection conn)
        {
            if (_isCoordinator)
            {
                if (Cancelling)
                    throw new CancelledException();
                
                var startTime = DateTime.Now;

                do
                {
                    if (Cancelling)
                        throw new CancelledException();

                    try
                    {
                        _session = Connection.DuplicateSession();
                        log.InfoFormat("Reconnected after {0:0.0}sec...", (DateTime.Now - startTime).TotalSeconds);
                    }
                    catch (Exception e)
                    {
                        _session = null;
                        Thread.Sleep(5000);

                        var span = DateTime.Now - startTime;
                        if ((int)span.TotalSeconds % 60 == 0)
                            log.Info($"Been waiting for reconnection for {span.TotalSeconds:0.0}sec...", e);
                    }
                } while (_session == null);

                RequestReconnection?.Invoke(Connection);
            }

            PercentComplete = 60;
            WaitForNewCertificate();
        }
    }
}
