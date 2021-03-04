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
using System.Text;

namespace CommandLib
{
    /// <summary>
    /// Thrown if we fail to verify a tar header checksum
    /// </summary>
    public class HeaderChecksumFailed : ApplicationException
    {
        private uint expected;
        private uint received;

        public HeaderChecksumFailed(uint expected, uint received)
        {
            this.expected = expected;
            this.received = received;
        }

        public override string ToString()
        {
            string expected = Convert.ToString(this.expected);
            string received = Convert.ToString(this.received);

            return "Failed to verify the tar header checksum: received = " + received + "; expected = " + expected;
        }
    }

    /// <summary>
    /// Thrown when we find the end of archive marker (two zero blocks)
    /// </summary>
    class EndOfArchive : ApplicationException
    {
        public EndOfArchive()
        {
        }

        public override string ToString()
        {
            return "End of tar archive";
        }
    }

    public class Header
    {
        public string file_name;
        public int file_mode;
        public int user_id;
        public int group_id;
        public uint file_size;
        public uint mod_time;
        public bool link;
        public int link_name;

        /* Length of a header block */
        public static uint length = 512;

        /* http://en.wikipedia.org/w/index.php?title=Tar_%28file_format%29&oldid=83554041 */
        private static int file_name_off = 0;
        private static int file_name_len = 100;
        private static int file_mode_off = 100;
        private static int file_mode_len = 8;
        private static int user_id_off = 108;
        private static int user_id_len = 8;
        private static int group_id_off = 116;
        private static int group_id_len = 8;
        private static int file_size_off = 124;
        private static int file_size_len = 12;
        private static int mod_time_off = 136;
        private static int mod_time_len = 12;
        private static int chksum_off = 148;
        private static int chksum_len = 8;
        private static int link_off = 156;
        private static int link_len = 1;
        private static int link_name_off = 156;
        private static int link_name_len = 100;

        /// <summary>
        /// True if a buffer contains all zeroes
        /// </summary>
        public static bool all_zeroes(byte[] buffer)
        {
            bool zeroes = true;
            for (int i = 0; i < buffer.Length && zeroes; i++)
            {
                if (buffer[i] != 0) zeroes = false;
            }

            return zeroes;
        }

        /// <summary>
        /// Return a sub-array of bytes
        /// </summary>
        private byte[] slice(byte[] input, int offset, int length)
        {
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = input[offset + i];
            }

            return result;
        }

        /// <summary>
        /// Remove NULLs and spaces from the end of a string
        /// </summary>
        private string trim_trailing_stuff(string x)
        {
            char[] trimmed = {'\0', ' '};
            return x.TrimEnd(trimmed);
        }

        /// <summary>
        /// Convert the byte array into a string (assume UTF8)
        /// </summary>
        private string unmarshal_string(byte[] buffer)
        {
            Decoder decoder = Encoding.UTF8.GetDecoder();
            char[] chars = new char[decoder.GetCharCount(buffer, 0, (int)buffer.Length)];
            decoder.GetChars(buffer, 0, (int)buffer.Length, chars, 0);
            return trim_trailing_stuff(new string(chars));
        }

        /// <summary>
        /// Unmarshal an octal string into an int32
        /// </summary>
        private uint unmarshal_int32(byte[] buffer)
        {
            string octal = "0" + unmarshal_string(buffer);
            return System.Convert.ToUInt32(octal, 8);
        }

        /// <summary>
        /// Unmarshal an octal string into an int 
        /// </summary>
        private int unmarshal_int(byte[] buffer)
        {
            string octal = "0" + unmarshal_string(buffer);
            return System.Convert.ToInt32(octal, 8);
        }

        /// <summary>
        /// Recompute the (weak) header checksum
        /// </summary>
        private uint compute_checksum(byte[] buffer)
        {
            uint total = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                /* treat the checksum digits as ' ' */
                if ((i >= chksum_off) && (i < (chksum_off + chksum_len)))
                {
                    total += 32; /* ' ' */
                }
                else
                {
                    total += buffer[i];
                }
            }

            return total;
        }

        /// <summary>
        /// Compute the required length of padding data to follow the data payload
        /// </summary>
        public uint paddingLength()
        {
            /* round up to the next whole number of blocks */
            uint next_block_length = (file_size + length - 1) / length * length;
            return next_block_length - file_size;
        }

        /// <summary>
        /// pretty-print a header
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0}/{1} {2:000000000000} {3:000000000000} {4}",
                user_id, group_id, file_size, mod_time, file_name);
        }

        /// <summary>
        /// Unmarshal a header from a buffer, throw an exception if the checksum doesn't validate
        /// </summary>
        public Header(byte[] buffer)
        {
            file_name = unmarshal_string(slice(buffer, file_name_off, file_name_len));
            file_mode = unmarshal_int(slice(buffer, file_mode_off, file_mode_len));
            user_id = unmarshal_int(slice(buffer, user_id_off, user_id_len));
            group_id = unmarshal_int(slice(buffer, group_id_off, group_id_len));
            file_size = unmarshal_int32(slice(buffer, file_size_off, file_size_len));
            mod_time = unmarshal_int32(slice(buffer, mod_time_off, mod_time_len));
            link = unmarshal_string(slice(buffer, link_off, link_len)) == "1";
            link_name = unmarshal_int(slice(buffer, link_name_off, link_name_len));

            uint chksum = unmarshal_int32(slice(buffer, chksum_off, chksum_len));
            uint recomputed = compute_checksum(buffer);
            if (chksum != recomputed)
                throw new HeaderChecksumFailed(recomputed, chksum);

        }

        /// <summary>
        /// Read a tar header from a stream
        /// </summary>
        public static Header fromStream(Stream input)
        {
            byte[] one = IO.unmarshal_n(input, length);
            if (all_zeroes(one))
            {
                byte[] two = IO.unmarshal_n(input, length);
                if (all_zeroes(two))
                    throw new EndOfArchive();
                return new Header(two);
            }

            return new Header(one);
        }
    }

    public class Archive
    {
        public static void list(Stream stream)
        {
            try
            {
                while (true)
                {
                    Header x = Header.fromStream(stream);
                    Console.WriteLine(x);
                    IO.skip(stream, x.file_size);
                    IO.skip(stream, x.paddingLength());
                }

            }
            catch (EndOfArchive)
            {
                Console.WriteLine("EOF");
            }
        }
    }
}
