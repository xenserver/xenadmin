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
using System.Security.Cryptography;
using System.Text;

namespace XenAdmin.Core
{
    /// <summary>
    /// Used to centralise the encryption routines used for master password
    /// & session lists
    /// </summary>
    public class EncryptionUtils
    {
        private static byte[] salt;

        /// <summary>
        /// Returns a secure hash of the given input string (usually the
        /// master password). We currently use SHA-1.
        /// </summary>
        /// <param name="password">The string to hash</param>
        /// <returns>The secure hash</returns>
        public static byte[] ComputeHash(String password)
        {
            //SHA1 hasher = SHA1.Create();
            MD5 hasher = MD5.Create();
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] bytes = ue.GetBytes(password);
            
            byte[] hash = hasher.ComputeHash(bytes);

            return hash;
        }

        public static string Protect(string data)
        {
            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            byte[] protectedBytes = ProtectedData.Protect(dataBytes, GetSalt(), DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        public static string Unprotect(string protectedstring)
        {
            byte[] protectedBytes = Convert.FromBase64String(protectedstring);
            byte[] dataBytes = ProtectedData.Unprotect(protectedBytes, GetSalt(), DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(dataBytes);
        }

        public static string ProtectForLocalMachine(string data)
        {
            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            byte[] protectedBytes = ProtectedData.Protect(dataBytes, GetSalt(), DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(protectedBytes);
        }

        public static string UnprotectForLocalMachine(string protectedstring)
        {
            byte[] protectedBytes = Convert.FromBase64String(protectedstring);
            byte[] dataBytes = ProtectedData.Unprotect(protectedBytes, GetSalt(), DataProtectionScope.LocalMachine);
            return Encoding.Unicode.GetString(dataBytes);
        }

        /// <summary>
        /// Encrypt a given string using the given key. The cipherText is Base64
        /// encoded and returned. The algorithjm currently used is "Rijndael"
        /// </summary>
        /// <param name="clearString">The string to encrypt.</param>
        /// <param name="keyBytes">The key for the encryption algorithm</param>
        /// <returns>The Base64 encoded cipher text</returns>
        public static String EncryptString(String clearString, byte[] keyBytes)
        {
            MemoryStream ms = new MemoryStream();

            //DES alg = DES.Create();
            //RC2 alg = RC2.Create();
            Rijndael alg = Rijndael.Create();

            alg.Key = keyBytes;
            alg.IV = GetSalt();

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearString);

            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.Close();

            byte[] cipherText = ms.ToArray();

            return Convert.ToBase64String(cipherText);
        }

        /// <summary>
        /// Decrypt some cipher text that was created by the encryptString method.
        /// </summary>
        /// <param name="cipherText64">The base64 encoded cipher text that was produced
        /// by encryptString</param>
        /// <param name="keyBytes">The key to use to decrypt</param>
        /// <returns>The decrypted text.</returns>
        public static String DecryptString(String cipherText64, byte[] keyBytes)
        {
            MemoryStream ms = new MemoryStream();

            byte[] cipherBytes = Convert.FromBase64String(cipherText64);
            Rijndael alg = Rijndael.Create();
            alg.Key = keyBytes;
            alg.IV = GetSalt();

            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();

            byte[] plainBytes = ms.ToArray();

            return Encoding.Unicode.GetString(plainBytes);
        }


        private static byte[] GetSalt()
        {
            // NOTE: This is what we did in Geneva - may want
            // to do something less lame in future...
            if (salt == null)
            {
                UnicodeEncoding ue = new UnicodeEncoding();
                salt = ue.GetBytes("XenRocks");
            }

            return salt;
        }
    }
}
