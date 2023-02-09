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
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ThinCLI.Properties;


namespace ThinCLI
{
    public class Config
    {
        public string Hostname { get; set; } // no default hostname
        public string Username { get; set; }
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 443;
        public int BlockSize { get; set; } = 65536;
        public bool Debug { get; set; }
        public bool NoWarnNewCertificates { get; set; }
        public bool NoWarnCertificates { get; set; }
        public List<string> EnteredParamValues { get; } = new List<string>();
    }

    public class Transport
    {
        private readonly Config _conf;
        private readonly object _certificateValidationLock = new object();

        public Transport(Config conf)
        {
            _conf = conf;
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        private bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            lock (_certificateValidationLock)
            {
                string trustEvaluation = Messages.CERTIFICATE_NOT_TRUSTED;
                try
                {
                    var cert2 = new X509Certificate2(certificate);
                    if (new X509Chain(true).Build(cert2) || cert2.Verify())
                        trustEvaluation = Messages.CERTIFICATE_TRUSTED;
                }
                catch (CryptographicException)
                {
                    Logger.Warn("Invalid server certificate.");
                }

                var knownCertificates = Settings.GetKnownServers();

                string fingerprint = GetReadableFingerPrint(certificate.GetCertHashString());

                bool acceptCertificate;

                if (knownCertificates.ContainsKey(_conf.Hostname))
                {
                    var oldFingerPrint = knownCertificates[_conf.Hostname];
                    if (oldFingerPrint == fingerprint)
                        return true;

                    acceptCertificate = _conf.NoWarnCertificates || GetUserConsent(
                        string.Format(Messages.CERTIFICATE_CHANGED, fingerprint, oldFingerPrint, trustEvaluation));
                }
                else
                {
                    acceptCertificate = _conf.NoWarnNewCertificates || GetUserConsent(
                        string.Format(Messages.CERTIFICATE_FOUND, fingerprint, trustEvaluation));
                }

                if (acceptCertificate)
                {
                    knownCertificates[_conf.Hostname] = fingerprint;
                    Settings.SetKnownServers(knownCertificates);
                    Settings.TrySaveSettings(_conf);
                }

                return acceptCertificate;
            }
        }

        private static string GetReadableFingerPrint(string fingerprint)
        {
            var readable = new List<char>();

            for (int i = 0; i < fingerprint.Length; i++)
            {
                readable.Add(fingerprint[i]);

                if (i % 2 != 0 && i != fingerprint.Length - 1)
                    readable.Add(':');
            }

            return new string(readable.ToArray());
        }

        private bool GetUserConsent(string prompt)
        {
            Console.WriteLine(prompt);

            do
            {
                var key = Console.ReadKey(true);
                switch (key.KeyChar)
                {
                    case 'y':
                    case 'Y':
                        return true;
                    case 'n':
                    case 'N':
                        return false;
                }
            } while (true);
        }

        /// <summary>
        /// Create an SSL stream that will close the client's stream.
        /// </summary>
        public Stream Connect(TcpClient client, int port)
        {
            if (port != 443)
                return client.GetStream();

            try
            {
                var sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate, null);
                sslStream.AuthenticateAsClient("", null, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, true);
                return sslStream;
            }
            catch (AuthenticationException ae)
            {
                throw new ThinCliProtocolException(ae.Message);
            }
        }
    }

    public static class Http
    {
        private static string ReadLine(Stream stream)
        {
            StringBuilder messageData = new StringBuilder();
            do
            {
                int i = stream.ReadByte();
                if (i == -1)
                {
                    throw new EndOfStreamException();
                }
                else
                {
                    char b = (char)i;
                    messageData.Append(b);
                    if (b == '\n') break;
                }
            } while (true);

            return messageData.ToString();
        }

        private static void WriteLine(Stream stream, string line)
        {
            byte[] message = Encoding.UTF8.GetBytes($"{line}\r\n");
            stream.Write(message, 0, message.Length);
            stream.Flush();
        }

        private static int GetResultCode(string line)
        {
            string[] bits = line.Split(' ');
            if (bits.Length < 2) return 0;
            return Int32.Parse(bits[1]);
        }

        public static Stream DoRpc(TcpClient client, string method, Uri uri, Config conf, params string[] headers)
        {
            var transport = new Transport(conf);
            Stream http = transport.Connect(client, uri.Port);

            var startLine = $"{method} {uri.PathAndQuery} HTTP/1.0";
            WriteLine(http, startLine);
            foreach (string h in headers)
                WriteLine(http, h);
            WriteLine(http, "");

            var response = ReadLine(http);
            int code = GetResultCode(response);

            switch (code)
            {
                case 200:
                    break;

                case 302:
                    string url = "";
                    while (true)
                    {
                        response = ReadLine(http);
                        if (response.StartsWith("Location: "))
                            url = response.Substring(10);
                        if (response.Equals("\r\n") || response.Equals(""))
                            break;
                    }

                    Uri redirect = new Uri(url.Trim());
                    conf.Hostname = redirect.Host;
                    http.Close();
                    http.Dispose();
                    return DoRpc(client, method, redirect, conf, headers);

                default:
                    Logger.Error($"Received error code {code} from the server doing an HTTP {method}");
                    http.Close();
                    http.Dispose();
                    return null;
            }

            while (true)
            {
                response = ReadLine(http);
                if (response.Equals("\r\n") || response.Equals(""))
                    break;
            }

            // Stream should be positioned after the headers
            return http;
        }
    }

    public static class Types
    {
        public static uint UnMarshalUint(Stream stream)
        {
            uint a = (uint)stream.ReadByte();
            uint b = (uint)stream.ReadByte();
            uint c = (uint)stream.ReadByte();
            uint d = (uint)stream.ReadByte();
            return (a << 0) | (b << 8) | (c << 16) | (d << 24);
        }

        public static void MarshalUint(Stream stream, uint x)
        {
            uint mask = 0xff;
            stream.WriteByte((byte)((x >> 0) & mask));
            stream.WriteByte((byte)((x >> 8) & mask));
            stream.WriteByte((byte)((x >> 16) & mask));
            stream.WriteByte((byte)((x >> 24) & mask));
        }

        public static int UnMarshalInt(Stream stream)
        {
            return (int)UnMarshalUint(stream);
        }

        public static void MarshalInt(Stream stream, int x)
        {
            MarshalUint(stream, (uint)x);
        }

        public static byte[] UnMarshalN(Stream stream, uint n)
        {
            byte[] buffer = new byte[n];
            int toRead = (int)n;
            int offset = 0;

            while (toRead > 0)
            {
                int nRead = stream.Read(buffer, offset, toRead);
                offset = nRead;
                toRead -= nRead;
            }

            return buffer;
        }

        public static string UnMarshalString(Stream stream)
        {
            uint length = UnMarshalUint(stream);
            byte[] buffer = UnMarshalN(stream, length);
            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, (int)length)];
            decoder.GetChars(buffer, 0, (int)length, chars, 0);
            return new string(chars);
        }

        public static void MarshalString(Stream stream, string x)
        {
            MarshalInt(stream, x.Length);
            char[] c = x.ToCharArray();
            Encoder encoder = Encoding.UTF8.GetEncoder();
            byte[] bytes = new byte[encoder.GetByteCount(c, 0, c.Length, true)];
            encoder.GetBytes(c, 0, c.Length, bytes, 0, true);
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    public static class Marshalling
    {
        /// <summary>
        /// Unique prefix string used to ensure we're talking to a thin CLI server
        /// </summary>
        private const string THIN_CLI_SERVER_PREFIX = "XenSource thin CLI protocol";

        private const int CLI_PROTOCOL_MAJOR = 0;
        private const int CLI_PROTOCOL_MINOR = 2;

        private enum Tag
        {
            Print = 0,
            Load = 1,
            HttpGet = 12,
            HttpPut = 13,
            Prompt = 3,
            Exit = 4,
            Error = 14,
            Ok = 5,
            Failed = 6,
            Chunk = 7,
            End = 8,
            Command = 9,
            Response = 10,
            Blob = 11,
            Debug = 15,
            PrintStderr = 16
        }

        private static Tag UnMarshalTag(Stream stream)
        {
            int x = Types.UnMarshalInt(stream);
            return (Tag)x;
        }

        private static void MarshalTag(Stream stream, Tag tag)
        {
            Types.MarshalInt(stream, (int)tag);
        }

        private static void MarshalResponse(Stream stream, Tag t)
        {
            Types.MarshalInt(stream, 4 + 4);
            MarshalTag(stream, Tag.Response);
            MarshalTag(stream, t);
        }

        private static void Load(Stream stream, string filename, Config conf)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    FileInfo fi = new FileInfo(filename);
                    // Immediately report our success in opening the file
                    MarshalResponse(stream, Tag.Ok);

                    // The server doesn't like multiple chunks but this is fine for
                    // Zurich/Geneva imports
                    Types.MarshalInt(stream, 4 + 4 + 4);
                    MarshalTag(stream, Tag.Blob);
                    MarshalTag(stream, Tag.Chunk);
                    Types.MarshalUint(stream, (uint)fi.Length);

                    byte[] block = new byte[conf.BlockSize];
                    while (true)
                    {
                        int n = fs.Read(block, 0, block.Length);
                        if (n == 0)
                        {
                            Types.MarshalInt(stream, 4 + 4);
                            MarshalTag(stream, Tag.Blob);
                            MarshalTag(stream, Tag.End);
                            break;
                        }

                        stream.Write(block, 0, n);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                MarshalResponse(stream, Tag.Failed);
            }
            catch (FileNotFoundException)
            {
                MarshalResponse(stream, Tag.Failed);
            }
        }

        private static void HttpPut(Stream stream, string filename, Uri uri, Config conf)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (TcpClient client = new TcpClient(uri.Host, uri.Port))
                    using (Stream http = Http.DoRpc(client, "PUT", uri, conf, $"Content-Length: {fs.Length}"))
                    {
                        if (http == null)
                        {
                            MarshalResponse(stream, Tag.Failed);
                            return;
                        }

                        byte[] block = new byte[conf.BlockSize];
                        while (true)
                        {
                            int n = fs.Read(block, 0, block.Length);
                            if (n == 0) break;
                            http.Write(block, 0, n);
                        }

                        MarshalResponse(stream, Tag.Ok);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Error("File not found");
                MarshalResponse(stream, Tag.Failed);
            }
            catch (Exception e)
            {
                Logger.Error($"Received exception: {e.Message}");
                Logger.Debug(e, conf);
                MarshalResponse(stream, Tag.Failed);
            }
        }

        private static void HttpGet(Stream stream, string filename, Uri uri, Config conf)
        {
            try
            {
                if (File.Exists(filename))
                    throw new ThinCliProtocolException($"The file '{filename}' already exists");

                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    using (TcpClient client = new TcpClient(uri.Host, uri.Port))
                    using (Stream http = Http.DoRpc(client, "GET", uri, conf))
                    {
                        if (http == null)
                        {
                            Logger.Error("Server rejected request");
                            MarshalResponse(stream, Tag.Failed);
                            return;
                        }

                        byte[] block = new byte[conf.BlockSize];
                        while (true)
                        {
                            int n = http.Read(block, 0, block.Length);
                            if (n == 0) break;
                            fs.Write(block, 0, n);
                        }

                        MarshalResponse(stream, Tag.Ok);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Received exception: " + e.Message);
                Logger.Error("Unable to write output file: " + filename);
                MarshalResponse(stream, Tag.Failed);
            }
        }

        private static void VersionHandshake(Stream stream, Config conf)
        {
            // Check for the initial prefix string
            byte[] magic = Types.UnMarshalN(stream, (uint)THIN_CLI_SERVER_PREFIX.Length);

            for (int i = 0; i < THIN_CLI_SERVER_PREFIX.Length; i++)
            {
                if (magic[i] != THIN_CLI_SERVER_PREFIX[i])
                    throw new ThinCliProtocolException($"Failed to find a server on {conf.Hostname}:{conf.Port}");
            }

            // Read the remote version numbers
            int remoteMajor = Types.UnMarshalInt(stream);
            int remoteMinor = Types.UnMarshalInt(stream);

            Logger.Debug($"Remote thin CLI protocol has version {remoteMajor}.{remoteMinor}", conf);
            Logger.Debug($"Client expects version {CLI_PROTOCOL_MAJOR}.{CLI_PROTOCOL_MINOR}", conf);

            if (CLI_PROTOCOL_MAJOR != remoteMajor || CLI_PROTOCOL_MINOR != remoteMinor)
                throw new ThinCliProtocolException($"Protocol version mismatch talking to server on {conf.Hostname}:{conf.Port}");

            // Tell the server our version numbers
            foreach (var t in THIN_CLI_SERVER_PREFIX)
                stream.WriteByte((byte)t);

            Types.MarshalInt(stream, CLI_PROTOCOL_MAJOR);
            Types.MarshalInt(stream, CLI_PROTOCOL_MINOR);
        }

        public static void PerformCommand(string command, Config conf)
        {
            try
            {
                var transport = new Transport(conf);
                using (var client = new TcpClient(conf.Hostname, conf.Port))
                using (var stream = transport.Connect(client, conf.Port))
                {
                    if (stream == null)
                        throw new ThinCliProtocolException($"Connection to {conf.Hostname}:{conf.Port} failed.");

                    byte[] message = Encoding.UTF8.GetBytes(command);
                    stream.Write(message, 0, message.Length);
                    stream.Flush();
                    VersionHandshake(stream, conf);
                    Interpreter(stream, conf);
                }
            }
            catch (SocketException)
            {
                throw new ThinCliProtocolException($"Connection to {conf.Hostname}:{conf.Port} refused.");
            }
        }

        private static void CheckPermitFiles(string filename, Config conf, bool includeCurrentDir = false)
        {
            string fullPath;

            try
            {
                fullPath = Path.GetFullPath(filename);
            }
            catch (Exception ex)
            {
                throw new ThinCliProtocolException($"Failed to retrieve full path of file '{filename}', '{ex.Message}'");
            }

            if (includeCurrentDir)
                conf.EnteredParamValues.Add(Directory.GetCurrentDirectory());

            foreach (string value in conf.EnteredParamValues)
            {
                try
                {
                    var valueFullPath = Path.GetFullPath(value);

                    if (fullPath.StartsWith(valueFullPath))
                    {
                        Logger.Debug("Passed permit files check", conf);
                        return;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            throw new ThinCliProtocolException($"The file with name '{filename}' is not present at the command line.");
        }

        private static void Interpreter(Stream stream, Config conf)
        {
            string filename;
            string path;
            string msg;

            while (true)
            {
                Types.UnMarshalUint(stream); // total message length (unused here)	
                Tag t = UnMarshalTag(stream);
                switch (t)
                {
                    case Tag.Command:
                        t = UnMarshalTag(stream);
                        switch (t)
                        {
                            case Tag.Print:
                                msg = Types.UnMarshalString(stream);
                                Logger.Debug("Read: Print: ", conf);
                                Logger.Info(msg);
                                break;
                            case Tag.PrintStderr:
                                msg = Types.UnMarshalString(stream);
                                Logger.Debug("Read: PrintStderr: ", conf);
                                Logger.Info(msg);
                                break;
                            case Tag.Debug:
                                msg = Types.UnMarshalString(stream);
                                Logger.Debug("Read: Debug: ", conf);
                                Logger.Info(msg);
                                break;
                            case Tag.Exit:
                                int code = Types.UnMarshalInt(stream);
                                Logger.Debug("Read: Command Exit " + code, conf);
                                if (code == 0)
                                    return; //exit
                                throw new ThinCliProtocolException($"Command Exit {code}", code);
                            case Tag.Error:
                                Logger.Debug("Read: Command Error", conf);
                                string errCode = Types.UnMarshalString(stream);
                                Logger.Info("Error code: " + errCode);
                                var paramList = new List<string>();
                                int length = Types.UnMarshalInt(stream);
                                for (int i = 0; i < length; i++)
                                {
                                    string param = Types.UnMarshalString(stream);
                                    paramList.Add(param);
                                }

                                Logger.Info("Error params: " + string.Join(", ", paramList));
                                break;
                            case Tag.Prompt:
                                Logger.Debug("Read: Command Prompt", conf);
                                string response = Console.ReadLine();
                                Logger.Info("Read " + response);
                                /* NB, 4+4+4 here for the blob, chunk and string length, put in by the MarshalString
                                 function. A franken-marshal. */
                                Types.MarshalInt(stream, 4 + 4 + 4); // length
                                MarshalTag(stream, Tag.Blob);
                                MarshalTag(stream, Tag.Chunk);
                                Types.MarshalString(stream, response);
                                Types.MarshalInt(stream, 4 + 4); // length
                                MarshalTag(stream, Tag.Blob);
                                MarshalTag(stream, Tag.End);
                                break;
                            case Tag.Load:
                                filename = Types.UnMarshalString(stream);
                                CheckPermitFiles(filename, conf);
                                Logger.Debug("Read: Load " + filename, conf);
                                Load(stream, filename, conf);
                                break;
                            case Tag.HttpPut:
                                filename = Types.UnMarshalString(stream);
                                CheckPermitFiles(filename, conf);
                                path = Types.UnMarshalString(stream);
                                Uri uri = ParseUri(path, conf);
                                Logger.Debug("Read: HttpPut " + filename + " -> " + uri, conf);
                                HttpPut(stream, filename, uri, conf);
                                break;
                            case Tag.HttpGet:
                                filename = Types.UnMarshalString(stream);
                                CheckPermitFiles(filename, conf, true);
                                path = Types.UnMarshalString(stream);
                                uri = ParseUri(path, conf);
                                Logger.Debug("Read: HttpGet " + filename + " -> " + uri, conf);
                                HttpGet(stream, filename, uri, conf);
                                break;
                            default:
                                throw new ThinCliProtocolException("Protocol failure: Reading Command: unexpected tag: " + t);
                        }

                        break;
                    default:
                        throw new ThinCliProtocolException("Protocol failure: Reading Message: unexpected tag: " + t);
                }
            }
        }

        private static Uri ParseUri(string path, Config conf)
        {
            // The server sometimes sends us a relative path (e.g. for VM import)
            // and sometimes an absolute URI (https://host/path). Construct the absolute URI
            // based on what we're given. The same hack exists in the server code...
            // See CA-10942.
            if (path.StartsWith("/"))
            {
                string[] bits = path.Split('?');
                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "https";
                uriBuilder.Host = conf.Hostname;
                uriBuilder.Port = conf.Port;
                uriBuilder.Path = bits[0];
                if (bits.Length > 1)
                    uriBuilder.Query = bits[1];
                return uriBuilder.Uri;
            }

            return new Uri(path);
        }
    }
}
