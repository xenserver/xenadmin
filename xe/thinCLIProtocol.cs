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
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO;
using System.Net.Sockets;
using System.Security.Authentication;


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
        public List<string> EnteredParamValues { get; } = new List<string>();
    }

    public static class Transport
    {
        // The following method is invoked by the RemoteCertificateValidationDelegate.
        private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            // Do allow this client to communicate with unauthenticated servers.
            return true;
        }

        /// <summary>
        /// Create an SSL stream that will close the client's stream.
        /// </summary>
        public static Stream Connect(TcpClient client, Config conf, int port)
        {
            if (port != 443)
                return client.GetStream();

            try
            {
                var sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate, null);

                sslStream.AuthenticateAsClient("", null, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, true);

                return sslStream;
            }
            catch (AuthenticationException)
            {
                if (conf.Debug)
                    throw;
                Logger.Error("Authentication failed - closing the connection.");
                client.Close();
                return null;
            }
            catch
            {
                if (conf.Debug)
                    throw;
                Logger.Error("Exception during SSL auth - closing the connection.");
                client.Close();
                return null;
            }
        }
    }

    public static class HTTP
    {
        private static string readLine(Stream stream){
		    StringBuilder messageData = new StringBuilder();
		    do {
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

        private static void writeLine(Stream stream, string line){
            byte[] message = Encoding.UTF8.GetBytes($"{line}\r\n");
		    stream.Write(message, 0, message.Length);
		    stream.Flush();
	    }

        private static int getResultCode(string line){
		    string[] bits = line.Split(' ');
		    if (bits.Length < 2) return 0;
		    return Int32.Parse(bits[1]);
	    }

        public static Stream DoRPC(TcpClient client, string method, Uri uri, Config conf, params string[] headers)
        {
            Stream http = Transport.Connect(client, conf, uri.Port);

            var startLine = $"{method} {uri.PathAndQuery} HTTP/1.0";
            writeLine(http, startLine);
            foreach (string h in headers)
                writeLine(http, h);
            writeLine(http, "");

            var response = readLine(http);
            int code = getResultCode(response);

            switch (code)
            {
                case 200:
                    break;

                case 302:
                    string url = "";
                    while (true)
                    {
                        response = readLine(http);
                        if (response.StartsWith("Location: "))
                            url = response.Substring(10);
                        if (response.Equals("\r\n") || response.Equals(""))
                            break;
                    }

                    Uri redirect = new Uri(url.Trim());
                    conf.Hostname = redirect.Host;
                    http.Close();
                    http.Dispose();
                    return DoRPC(client, method, redirect, conf, headers);

                default:
                    Logger.Error($"Received error code {code} from the server doing an HTTP {method}");
                    http.Close();
                    http.Dispose();
                    return null;
            }

            while (true)
            {
                response = readLine(http);
                if (response.Equals("\r\n") || response.Equals(""))
                    break;
            }
            // Stream should be positioned after the headers
            return http;
        }
    }

    public static class Types
    {
	    public static uint unmarshal_int32(Stream stream){
		    uint a = (uint)stream.ReadByte();
		    uint b = (uint)stream.ReadByte();
		    uint c = (uint)stream.ReadByte();
		    uint d = (uint)stream.ReadByte();
		    //Console.WriteLine("a = " + a + " b = " + b + " c = " + c + " d = " + d);
		    return (a << 0) | (b << 8) | (c << 16) | (d << 24);
	    }

        public static void marshal_int32(Stream stream, uint x){
		    uint mask = 0xff;
		    stream.WriteByte((byte) ((x >> 0) & mask));
		    stream.WriteByte((byte) ((x >> 8) & mask));
		    stream.WriteByte((byte) ((x >> 16) & mask));
		    stream.WriteByte((byte) ((x >> 24) & mask));
	    }

        public static int unmarshal_int(Stream stream){
		    return (int)unmarshal_int32(stream);
	    }

        public static void marshal_int(Stream stream, int x){
		    marshal_int32(stream, (uint)x);
	    }

        public static byte[] unmarshal_n(Stream stream, uint n){
		    byte[] buffer = new byte[n];
		    int toread = (int)n;
		    int offset = 0;
		    while (toread > 0){
			    int nread = stream.Read(buffer, offset, toread);
			    offset= nread; toread -= nread;
		    }
		    return buffer;
	    }

        public static string unmarshal_string(Stream stream){
		    uint length = unmarshal_int32(stream);
		    byte[] buffer = unmarshal_n(stream, length);
		    Decoder decoder = Encoding.UTF8.GetDecoder();
		    char[] chars = new char[decoder.GetCharCount(buffer, 0, (int)length)];
		    decoder.GetChars(buffer, 0, (int)length, chars, 0);
		    return new string(chars);
	    }

        public static void marshal_string(Stream stream, string x){
		    marshal_int(stream, x.Length);
		    char[] c = x.ToCharArray();
		    Encoder encoder = Encoding.UTF8.GetEncoder();
		    byte[] bytes = new byte[encoder.GetByteCount(c, 0, c.Length, true)];
		    encoder.GetBytes(c, 0, c.Length, bytes, 0, true);
		    stream.Write(bytes, 0, bytes.Length);
	    }
    }

    public static class Messages
    {
        private const string MAGIC_STRING = "XenSource thin CLI protocol";
        private const int CLI_PROTOCOL_MAJOR = 0;
        private const int CLI_PROTOCOL_MINOR = 2;

        private enum Tag
        {
            Print = 0, Load = 1, HttpGet = 12, HttpPut = 13, Prompt = 3, Exit = 4,
            Error = 14, OK = 5, Failed = 6, Chunk = 7, End = 8, Command = 9, Response = 10,
            Blob = 11, Debug = 15, PrintStderr = 16
        }

        private static Tag UnmarshalTag(Stream stream)
        {
            int x = Types.unmarshal_int(stream);
            return (Tag)x;
        }

        private static void MarshalTag(Stream stream, Tag tag)
        {
            Types.marshal_int(stream, (int)tag);
        }
        
        private static void MarshalResponse(Stream stream, Tag t)
        {
            Types.marshal_int(stream, 4 + 4);
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
                    MarshalResponse(stream, Tag.OK);

                    // The server doesn't like multiple chunks but this is fine for
                    // Zurich/Geneva imports
                    Types.marshal_int(stream, 4 + 4 + 4);
                    MarshalTag(stream, Tag.Blob);
                    MarshalTag(stream, Tag.Chunk);
                    Types.marshal_int32(stream, (uint)fi.Length);

                    byte[] block = new byte[conf.BlockSize];
                    while (true)
                    {
                        int n = fs.Read(block, 0, block.Length);
                        if (n == 0)
                        {
                            Types.marshal_int(stream, 4 + 4);
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
                    using (Stream http = HTTP.DoRPC(client, "PUT", uri, conf, $"Content-Length: {fs.Length}"))
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
                        MarshalResponse(stream, Tag.OK);
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
                MarshalResponse(stream, Tag.Failed);
            }
        }

        private static void HttpGet(Stream stream, string filename, Uri uri, Config conf)
        {
            try
            {
                if (File.Exists(filename))
                    throw new Exception($"The file '{filename}' already exists");

                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    using (TcpClient client = new TcpClient(uri.Host, uri.Port))
                    using (Stream http = HTTP.DoRPC(client, "GET", uri, conf))
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
                        MarshalResponse(stream, Tag.OK);
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
            // Check for the initial magic string
            byte[] magic = Types.unmarshal_n(stream, (uint)MAGIC_STRING.Length);

            for (int i = 0; i < MAGIC_STRING.Length; i++)
            {
                if (magic[i] != MAGIC_STRING[i])
                {
                    Logger.Error($"Failed to find a server on {conf.Hostname}:{conf.Port}");
                    Logger.Usage();
                    Environment.Exit(1);
                }
            }

            // Read the remote version numbers
            int remoteMajor = Types.unmarshal_int(stream);
            int remoteMinor = Types.unmarshal_int(stream);

            Logger.Debug($"Remote thin CLI protocol has version {remoteMajor}.{remoteMinor}", conf);
            Logger.Debug($"Client expects version {CLI_PROTOCOL_MAJOR}.{CLI_PROTOCOL_MINOR}", conf);

            if (CLI_PROTOCOL_MAJOR != remoteMajor || CLI_PROTOCOL_MINOR != remoteMinor)
            {
                Logger.Error($"Protocol version mismatch talking to server on {conf.Hostname}:{conf.Port}");
                Logger.Usage();
                Environment.Exit(1);
            }

            // Tell the server our version numbers
            foreach (var t in MAGIC_STRING)
                stream.WriteByte((byte)t);

            Types.marshal_int(stream, CLI_PROTOCOL_MAJOR);
            Types.marshal_int(stream, CLI_PROTOCOL_MINOR);
        }

        public static void PerformCommand(string Body, Config conf)
        {
            string body = Body;
            body += "username=" + conf.Username + "\n";
            body += "password=" + conf.Password + "\n";
            if (body.Length != 0)
            {
                body = body.Substring(0, body.Length - 1); // strip last "\n"
            }

            string header = "POST /cli HTTP/1.0\r\n";
            string content_length = "content-length: " + Encoding.UTF8.GetBytes(body).Length + "\r\n";
            string tosend = header + content_length + "\r\n" + body;

            TcpClient client = null;
            Stream stream = null;

            try
            {
                client = new TcpClient(conf.Hostname, conf.Port);
                stream = Transport.Connect(client, conf, conf.Port);

                if (stream == null)
                {
                    // The SSL functions already tell us what happened
                    Environment.Exit(1);
                    return;
                }

                byte[] message = Encoding.UTF8.GetBytes(tosend);
                stream.Write(message, 0, message.Length);
                stream.Flush();
                VersionHandshake(stream, conf);
                Interpreter(stream, conf);
            }
            catch (SocketException)
            {
                Logger.Error($"Connection to {conf.Hostname}:{conf.Port} refused.");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                if (conf.Debug)
                    throw;
                Logger.Error("Caught exception: " + e.Message);
                Environment.Exit(1);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }

                if (client != null)
                {
                    client.Close();
                    client.Dispose();
                }
            }
        }

        private static void CheckPermitFiles(String filename, Config conf, bool includeCurrentDir = false)
        {
            string fullpath;

            try
            {
                fullpath = Path.GetFullPath(filename);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve full path of file '{filename}', '{ex.Message}'");
            }

            if (includeCurrentDir)
                conf.EnteredParamValues.Add(Directory.GetCurrentDirectory());

            foreach (string value in conf.EnteredParamValues)
            {
                try
                {
                    String valueFullPath = Path.GetFullPath(value);

                    if (fullpath.StartsWith(valueFullPath))
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

            throw new Exception($"The file with name '{filename}' is not present at the command line.");
        }

        private static void Interpreter(Stream stream, Config conf)
        {
            string filename;
            string path;
            string msg;

            while (true)
            {
                Types.unmarshal_int32(stream); // total message length (unused here)	
                Tag t = UnmarshalTag(stream);
                switch (t)
                {
                    case Tag.Command:
                        t = UnmarshalTag(stream);
                        switch (t)
                        {
                            case Tag.Print:
                                msg = Types.unmarshal_string(stream);
                                Logger.Debug("Read: Print: " + msg, conf);
                                Logger.Info(msg);
                                break;
                            case Tag.PrintStderr:
                                msg = Types.unmarshal_string(stream);
                                Logger.Debug("Read: PrintStderr: " + msg, conf);
                                Logger.Info(msg); 
                                break;
                            case Tag.Debug:
                                msg = Types.unmarshal_string(stream);
                                Logger.Debug("Read: Debug: " + msg, conf);
                                Logger.Info(msg);
                                break;
                            case Tag.Exit:
                                int code = Types.unmarshal_int(stream);
                                Logger.Debug("Read: Command Exit " + code, conf);
                                Environment.Exit(code);
                                break;
                            case Tag.Error:
                                Logger.Debug("Read: Command Error", conf);
                                string err_code = Types.unmarshal_string(stream);
                                Logger.Info("Error code: " + err_code);
                                var paramList = new List<string>();
                                int length = Types.unmarshal_int(stream);
                                for (int i = 0; i < length; i++)
                                {
                                    string param = Types.unmarshal_string(stream);
                                    paramList.Add(param);
                                }
                                Logger.Info("Error params: " + string.Join(", ", paramList));
                                break;
                            case Tag.Prompt:
                                Logger.Debug("Read: Command Prompt", conf);
                                string response = Console.ReadLine();
                                Logger.Info("Read "+response);
                                /* NB, 4+4+4 here for the blob, chunk and string length, put in by the marshal_string
                                 function. A franken-marshal. */
                                Types.marshal_int(stream, 4 + 4 + 4); // length
                                MarshalTag(stream, Tag.Blob);
                                MarshalTag(stream, Tag.Chunk);
                                Types.marshal_string(stream, response);
                                Types.marshal_int(stream, 4 + 4); // length
                                MarshalTag(stream, Tag.Blob);
                                MarshalTag(stream, Tag.End);
                                break;
                            case Tag.Load:
                                filename = Types.unmarshal_string(stream);
                                CheckPermitFiles(filename, conf);
                                Logger.Debug("Read: Load " + filename, conf);
                                Load(stream, filename, conf);
                                break;
                            case Tag.HttpPut:
                                filename = Types.unmarshal_string(stream);
                                CheckPermitFiles(filename, conf);
                                path = Types.unmarshal_string(stream);
                                Uri uri = ParseUri(path, conf);
                                Logger.Debug("Read: HttpPut " + filename + " -> " + uri, conf);
                                HttpPut(stream, filename, uri, conf);
                                break;
                            case Tag.HttpGet:
                                filename = Types.unmarshal_string(stream);
                                CheckPermitFiles(filename, conf, true);
                                path = Types.unmarshal_string(stream);
                                uri = ParseUri(path, conf);
                                Logger.Debug("Read: HttpGet " + filename + " -> " + uri, conf);
                                HttpGet(stream, filename, uri, conf);
                                break;
                            default:
                                Logger.Error("Protocol failure: Reading Command: unexpected tag: " + t);
                                Environment.Exit(1);
                                break;
                        }
                        break;
                    default:
                        Logger.Error("Protocol failure: Reading Message: unexpected tag: " + t);
                        Environment.Exit(1);
                        break;
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
            else
            {
                return new Uri(path);
            }
        }
    }
}
