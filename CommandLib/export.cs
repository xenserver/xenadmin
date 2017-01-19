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
using System.IO;
using System.Xml;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

/* Thrown if we fail to verify a block (ie sha1) checksum */
public class BlockChecksumFailed : ApplicationException
{
    private string block;
    private string recomputed;
    private string original;

    public BlockChecksumFailed(string block, string recomputed, string original)
    {
        this.block = block;
        this.recomputed = recomputed;
        this.original = original;
    }

    public override string ToString()
    {
        return "Failed to verify the block checksum: block = " + block + "; recomputed = " + recomputed + "; original = " + original;
    }
}

public class Export
{
    public static bool verbose_debugging = false;
    public static void debug(string x)
    {
        if (verbose_debugging)
            Console.WriteLine(x);
    }

    private readonly SHA1 sha = new SHA1CryptoServiceProvider();

    private static char nibble(int x)
    {
        if (x < 10)
            return (char)((int)'0' + x);
        else
            return (char)((int)'a' - 10 + x);
    }

    private static string hex(byte x)
    {
        char low = nibble((int)x & 0xf);
        char high = nibble(((int)x >> 4) & 0xf);
        char[] chars = { high, low };
        return new String(chars);
    }

    private string checksum(byte[] data)
    {
        byte[] result = sha.ComputeHash(data);
        /* convert to a hex string for comparison */
        string x = "";
        for (int i = 0; i < result.Length; i++)
            x = x + hex(result[i]);
        return x;
    }

    private static Hashtable parse_checksum_table(string checksum_xml)
    {
        Hashtable table = new Hashtable();

        XmlDocument xmlDoc = new System.Xml.XmlDocument();
        xmlDoc.LoadXml(checksum_xml);
        XmlNodeList members = xmlDoc.GetElementsByTagName("member");
        string name;
        string value;
        foreach (XmlNode member in members)
        {
            name = ""; value = "";
            foreach (XmlNode child in member.ChildNodes)
            {
                XmlNode v = child.FirstChild;
                if (child.Name.Equals("name"))
                    name = v.Value;
                if (child.Name.Equals("value"))
                    value = v.Value;
            }
            debug(String.Format("adding {0} = {1}", name, value));
            table.Add(name, value);
        }
        return table;
    }

    private static void compare_tables(Hashtable recomputed, Hashtable original)
    {
        foreach (DictionaryEntry x in recomputed)
        {
            string ours = (string)x.Value;
            string theirs = (string)original[x.Key];
            if (!ours.Equals(theirs))
            {
                throw new BlockChecksumFailed((string)x.Key, ours, theirs);
            }
            else
            {
                debug(String.Format("{0} hash OK", (string)x.Key));
            }
        }
    }

    private static string string_of_byte_array(byte[] payload)
    {
        Decoder decoder = Encoding.UTF8.GetDecoder();
        char[] chars = new char[decoder.GetCharCount(payload, 0, (int)payload.Length)];
        decoder.GetChars(payload, 0, (int)payload.Length, chars, 0);
        return new string(chars);
    }

    public delegate void verifyCallback(uint read);
    public delegate bool cancellingCallback();

    /* 'input' is the source of the export data, if 'output' is not null then
       a perfect copy should be echoed there. */
    public void verify(Stream input, Stream output, cancellingCallback cancelling)
    {
        verify(input, output, cancelling, null);
    }

    public void verify(Stream input, Stream output, cancellingCallback cancelling, verifyCallback callback)
    {
        Hashtable recomputed_checksums = new Hashtable();
        Hashtable original_checksums = null;
        try
        {
            while (!cancelling())
            {
                Header x = null;
                byte[] one = IO.unmarshal_n(input, Header.length);
                if (callback != null) callback(Header.length);
                if (output != null) output.Write(one, 0, one.Length);

                if (Header.all_zeroes(one))
                {
                    byte[] two = IO.unmarshal_n(input, Header.length);
                    if (callback != null) callback(Header.length);
                    if (output != null) output.Write(two, 0, two.Length);
                    if (Header.all_zeroes(two))
                        throw new EndOfArchive();
                    x = new Header(two);
                }
                else
                {
                    x = new Header(one);
                }

                debug(x.ToString());
                byte[] payload = IO.unmarshal_n(input, x.file_size);
                if (callback != null) callback(x.file_size);
                if (output != null) output.Write(payload, 0, payload.Length);

                if (x.file_name.Equals("ova.xml"))
                {
                    debug("skipping ova.xml");
                }
                else if (x.file_name.EndsWith(".checksum"))
                {
                    string csum = string_of_byte_array(payload);
                    string base_name = x.file_name.Substring(0, x.file_name.Length - 9);
                    string ours = (string)recomputed_checksums[base_name];
                    if (!ours.Equals(csum))
                        throw new BlockChecksumFailed(base_name, ours, csum);
                    debug(String.Format("{0} hash OK", base_name));
                }
                else if (x.file_name.Equals("checksum.xml"))
                {
                    string xml = string_of_byte_array(payload);
                    original_checksums = parse_checksum_table(xml);
                }
                else
                {
                    string csum = checksum(payload);
                    debug(String.Format("   has checksum: {0}", csum));
                    recomputed_checksums.Add(x.file_name, csum);
                }
                byte[] padding = IO.unmarshal_n(input, x.paddingLength());
                if (callback != null) callback(x.paddingLength());
                if (output != null) output.Write(padding, 0, padding.Length);
            }

        }
        catch (EndOfArchive)
        {
            debug("EOF");
            if(original_checksums != null)
                compare_tables(recomputed_checksums, original_checksums);
        }
    }
}
