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
        public string hostname = ""; // no default hostname
        public string username = "root";
        public string password = "";
        public int port = 443;
        public int block_size = 65536;
        public bool nossl = false;
        public bool debug = false;
    }
    
    public class thinCLIProtocol
    {
        public Config conf;
        public string magic_string = "XenSource thin CLI protocol";
	    public int major = 0;
	    public int minor = 1;
        public bool dropOut;
        public List<string> EnteredParamValues;

        public thinCLIProtocol(Config conf)
        {
            this.conf = conf;
            dropOut = false;
            EnteredParamValues = new List<string>();
        }
        
    }        

    public class Transport
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
        public static Stream Connect(TcpClient client, thinCLIProtocol tCLIprotocol, int port)
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
                if (tCLIprotocol.conf.debug)
                    throw;
                Logger.Error("Authentication failed - closing the connection.");
                client.Close();
                return null;
            }
            catch
            {
                if (tCLIprotocol.conf.debug)
                    throw;
                Logger.Error("Exception during SSL auth - closing the connection.");
                client.Close();
                return null;
            }
        }
    }

    public class HTTP
    {
	    public static string readLine(Stream stream){
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

	    public static void writeLine(Stream stream, string line){
            byte[] message = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", line));
		    stream.Write(message, 0, message.Length);
		    stream.Flush();
	    }

	    public static int getResultCode(string line){
		    string[] bits = line.Split(new char[] {' '});
		    if (bits.Length < 2) return 0;
		    return Int32.Parse(bits[1]);
	    }

        public static Stream doRPC(TcpClient client, string method, Uri uri, thinCLIProtocol tCLIprotocol, params string[] headers)
        {
            Stream http = Transport.Connect(client, tCLIprotocol, uri.Port);

            var startLine = string.Format("{0} {1} HTTP/1.0", method, uri.PathAndQuery);
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
                    tCLIprotocol.conf.hostname = redirect.Host;
                    http.Close();
                    http.Dispose();
                    return doRPC(client, method, redirect, tCLIprotocol, headers);

                default:
                    Logger.Error(string.Format("Received error code {0} from the server doing an HTTP {1}", code, method));
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

    public class Types
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

    public class Messages
    {
        public enum tag
        {
            Print = 0, Load = 1, HttpGet = 12, HttpPut = 13, Prompt = 3, Exit = 4,
            Error = 14, OK = 5, Failed = 6, Chunk = 7, End = 8, Command = 9, Response = 10,
            Blob = 11, Debug = 15, PrintStderr = 16
        }

        public static tag unmarshal_tag(Stream stream)
        {
            int x = Types.unmarshal_int(stream);
            return (tag)x;
        }

        public static void marshal_tag(Stream stream, tag tag)
        {
            Types.marshal_int(stream, (int)tag);
        }
        
        public static void marshal_response(Stream stream, tag t)
        {
            Types.marshal_int(stream, 4 + 4);
            marshal_tag(stream, tag.Response);
            marshal_tag(stream, t);
        }
        
        public static void load(Stream stream, string filename, thinCLIProtocol tCLIprotocol)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    FileInfo fi = new FileInfo(filename);
                    // Immediately report our success in opening the file
                    marshal_response(stream, tag.OK);

                    // The server doesn't like multiple chunks but this is fine for
                    // Zurich/Geneva imports
                    Types.marshal_int(stream, 4 + 4 + 4);
                    marshal_tag(stream, tag.Blob);
                    marshal_tag(stream, tag.Chunk);
                    Types.marshal_int32(stream, (uint)fi.Length);

                    byte[] block = new byte[tCLIprotocol.conf.block_size];
                    while (true)
                    {
                        int n = fs.Read(block, 0, block.Length);
                        if (n == 0)
                        {
                            Types.marshal_int(stream, 4 + 4);
                            marshal_tag(stream, tag.Blob);
                            marshal_tag(stream, tag.End);
                            break;
                        }
                        stream.Write(block, 0, n);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                marshal_response(stream, tag.Failed);
            }
            catch (FileNotFoundException)
            {
                marshal_response(stream, tag.Failed);
            }
        }

        public static void http_put(Stream stream, string filename, Uri uri, thinCLIProtocol tCLIprotocol)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (TcpClient client = new TcpClient(uri.Host, uri.Port))
                    using (Stream http = HTTP.doRPC(client, "PUT", uri, tCLIprotocol, 
                        string.Format("Content-Length: {0}", fs.Length)))
                    {
                        if (http == null)
                        {
                            marshal_response(stream, tag.Failed);
                            return;
                        }
                        byte[] block = new byte[tCLIprotocol.conf.block_size];
                        while (true)
                        {
                            int n = fs.Read(block, 0, block.Length);
                            if (n == 0) break;
                            http.Write(block, 0, n);
                        }
                        marshal_response(stream, tag.OK);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Error("File not found");
                marshal_response(stream, tag.Failed);
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Received exception: {0}", e.Message));
                marshal_response(stream, tag.Failed);
            }
        }

        public static void http_get(Stream stream, string filename, Uri uri, thinCLIProtocol tCLIprotocol)
        {
            try
            {
                if (File.Exists(filename))
                    throw new Exception(string.Format("The file '{0}' already exists", filename));

                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    using (TcpClient client = new TcpClient(uri.Host, uri.Port))
                    using (Stream http = HTTP.doRPC(client, "GET", uri, tCLIprotocol))
                    {
                        if (http == null)
                        {
                            Logger.Error("Server rejected request");
                            marshal_response(stream, tag.Failed);
                            return;
                        }
                        byte[] block = new byte[tCLIprotocol.conf.block_size];
                        while (true)
                        {
                            int n = http.Read(block, 0, block.Length);
                            if (n == 0) break;
                            fs.Write(block, 0, n);
                        }
                        marshal_response(stream, tag.OK);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Received exception: " + e.Message);
                Logger.Error("Unable to write output file: " + filename);
                marshal_response(stream, tag.Failed);
            }
        }

        public static void version_handshake(Stream stream, thinCLIProtocol tCLIprotocol)
        {
            /* Check for the initial magic string */
            byte[] magic = Types.unmarshal_n(stream, (uint)tCLIprotocol.magic_string.Length);
            for (int i = 0; i < tCLIprotocol.magic_string.Length; i++)
            {
                if (magic[i] != tCLIprotocol.magic_string[i])
                {
                    Logger.Error("Failed to find a server on " + tCLIprotocol.conf.hostname + ":" + tCLIprotocol.conf.port);
                    Logger.Usage();
                    Environment.Exit(1);
                }
            }
            /* Read the remote version numbers */
            int remote_major = Types.unmarshal_int(stream);
            int remote_minor = Types.unmarshal_int(stream);
            Logger.Debug("Remote host has version " + remote_major + "." + remote_minor, tCLIprotocol);
            Logger.Debug("Client has version " + tCLIprotocol.major + "." + tCLIprotocol.minor, tCLIprotocol);
            if (tCLIprotocol.major != remote_major)
            {
                Logger.Error("Protocol version mismatch talking to server on " + tCLIprotocol.conf.hostname + ":" + tCLIprotocol.conf.port);
                Logger.Usage();
                Environment.Exit(1);
            }
            /* Tell the server our version numbers */
            for (int i = 0; i < tCLIprotocol.magic_string.Length; i++)
            {
                stream.WriteByte((byte)tCLIprotocol.magic_string[i]);
            }
            Types.marshal_int(stream, tCLIprotocol.major);
            Types.marshal_int(stream, tCLIprotocol.minor);
        }

        public static void performCommand(string Body, thinCLIProtocol tCLIprotocol)
        {
            string body = Body;
            body += "username=" + tCLIprotocol.conf.username + "\n";
            body += "password=" + tCLIprotocol.conf.password + "\n";
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
                client = new TcpClient(tCLIprotocol.conf.hostname, tCLIprotocol.conf.port);
                stream = Transport.Connect(client, tCLIprotocol, tCLIprotocol.conf.port);

                if (stream == null)
                {
                    // The SSL functions already tell us what happened
                    Environment.Exit(1);
                    return;
                }

                byte[] message = Encoding.UTF8.GetBytes(tosend);
                stream.Write(message, 0, message.Length);
                stream.Flush();
                version_handshake(stream, tCLIprotocol);
                interpreter(stream, tCLIprotocol);
            }
            catch (SocketException)
            {
                Logger.Error("Connection to " + tCLIprotocol.conf.hostname + ":" + tCLIprotocol.conf.port + " refused.");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                if (tCLIprotocol.conf.debug)
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

        public static void CheckPermitFiles(String filename, thinCLIProtocol tCLIprotocol, bool includeCurrentDir = false)
        {
            string fullpath = "";

            try
            {
                fullpath = Path.GetFullPath(filename);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to retrieve full path of file '{0}', '{1}'", filename, ex.Message));
            }

            if (includeCurrentDir)
                tCLIprotocol.EnteredParamValues.Add(Directory.GetCurrentDirectory());

            foreach (string value in tCLIprotocol.EnteredParamValues)
            {
                try
                {
                    String valueFullPath = Path.GetFullPath(value);

                    if (fullpath.StartsWith(valueFullPath))
                    {
                        Logger.Debug("Passed permit files check", tCLIprotocol);
                        return;
                    }
                }
                catch
                {
                    // ignored
                }
            }

            throw new Exception(string.Format("The file with name '{0}' is not present at the command line.", filename));
        }

        public static void interpreter(Stream stream, thinCLIProtocol tCLIprotocol)
        {
            string filename = "";
            string path = "";
            string msg = "";
            while (!tCLIprotocol.dropOut)
            {
                Types.unmarshal_int32(stream); // total message length (unused here)	
                Messages.tag t = Messages.unmarshal_tag(stream);
                switch (t)
                {
                    case Messages.tag.Command:
                        t = Messages.unmarshal_tag(stream);
                        switch (t)
                        {
                            case Messages.tag.Print:
                                msg = Types.unmarshal_string(stream);
                                Logger.Debug("Read: Print: " + msg, tCLIprotocol);
                                Logger.Info(msg);
                                break;
                            case Messages.tag.PrintStderr:
                                msg = Types.unmarshal_string(stream);
                                Logger.Debug("Read: PrintStderr: " + msg, tCLIprotocol);
                                Logger.Info(msg); 
                                break;
                            case Messages.tag.Debug:
                                msg = Types.unmarshal_string(stream);
                                Logger.Debug("Read: Debug: " + msg, tCLIprotocol);
                                Logger.Info(msg);
                                break;
                            case Messages.tag.Exit:
                                int code = Types.unmarshal_int(stream);
                                Logger.Debug("Read: Command Exit " + code, tCLIprotocol);
                                Environment.Exit(code);
                                break;
                            case Messages.tag.Error:
                                Logger.Debug("Read: Command Error", tCLIprotocol);
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
                            case Messages.tag.Prompt:
                                Logger.Debug("Read: Command Prompt", tCLIprotocol);
                                string response = Console.ReadLine();
                                Logger.Info("Read "+response);
                                /* NB, 4+4+4 here for the blob, chunk and string length, put in by the marshal_string
                                 function. A franken-marshal. */
                                Types.marshal_int(stream, 4 + 4 + 4); // length
                                Messages.marshal_tag(stream, Messages.tag.Blob);
                                Messages.marshal_tag(stream, Messages.tag.Chunk);
                                Types.marshal_string(stream, response);
                                Types.marshal_int(stream, 4 + 4); // length
                                Messages.marshal_tag(stream, Messages.tag.Blob);
                                Messages.marshal_tag(stream, Messages.tag.End);
                                break;
                            case Messages.tag.Load:
                                filename = Types.unmarshal_string(stream);
                                CheckPermitFiles(filename, tCLIprotocol);
                                Logger.Debug("Read: Load " + filename, tCLIprotocol);
                                Messages.load(stream, filename, tCLIprotocol);
                                break;
                            case Messages.tag.HttpPut:
                                filename = Types.unmarshal_string(stream);
                                CheckPermitFiles(filename, tCLIprotocol);
                                path = Types.unmarshal_string(stream);
                                Uri uri = ParseUri(path, tCLIprotocol);
                                Logger.Debug("Read: HttpPut " + filename + " -> " + uri, tCLIprotocol);
                                Messages.http_put(stream, filename, uri, tCLIprotocol);
                                break;
                            case Messages.tag.HttpGet:
                                filename = Types.unmarshal_string(stream);
                                CheckPermitFiles(filename, tCLIprotocol, true);
                                path = Types.unmarshal_string(stream);
                                uri = ParseUri(path, tCLIprotocol);
                                Logger.Debug("Read: HttpGet " + filename + " -> " + uri, tCLIprotocol);
                                Messages.http_get(stream, filename, uri, tCLIprotocol);
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

        private static Uri ParseUri(string path, thinCLIProtocol tcli)
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
                uriBuilder.Host = tcli.conf.hostname;
                uriBuilder.Port = tcli.conf.port;
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
