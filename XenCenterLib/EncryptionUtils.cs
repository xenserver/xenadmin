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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace XenCenterLib
{
    public static class EncryptionUtils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int SALT_LENGTH = 16;

        public enum HashMethod
        {
            Md5,
            Sha256
        }

        private static string StringOf(this HashMethod method)
        {
            switch (method)
            {
                case HashMethod.Md5:
                    return "MD5";
                case HashMethod.Sha256:
                    return "SHA256";
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        /// <summary>
        /// Returns a secure hash of the given input string.
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <param name="method">The hash algorithm implementation to use.</param>
        /// <returns>The secure hash</returns>
        public static byte[] ComputeHash(string input, HashMethod method = HashMethod.Sha256)
        {
            if (input == null)
                return null;

            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] bytes = ue.GetBytes(input);

            using (var hasher = HashAlgorithm.Create(method.StringOf()))
                return hasher?.ComputeHash(bytes);
        }

        public static string Protect(string data)
        {
            return ProtectForScope(data, DataProtectionScope.CurrentUser);
        }

        public static string Unprotect(string protectedString)
        {
            return UnprotectForScope(protectedString, DataProtectionScope.CurrentUser);
        }

        public static string ProtectForLocalMachine(string data)
        {
            return ProtectForScope(data, DataProtectionScope.LocalMachine);
        }

        public static string UnprotectForLocalMachine(string protectedString)
        {
            return UnprotectForScope(protectedString, DataProtectionScope.LocalMachine);
        }

        private static string ProtectForScope(string data, DataProtectionScope scope)
        {
            byte[] saltBytes = GetSalt();
            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            byte[] protectedBytes = ProtectedData.Protect(dataBytes, saltBytes, scope);
            return $"{Convert.ToBase64String(protectedBytes)},{Convert.ToBase64String(saltBytes)}";
        }

        private static string UnprotectForScope(string protectedString, DataProtectionScope scope)
        {
            var parts = protectedString.Split(new[] {','}, 2);
            byte[] saltBytes = parts.Length == 2
                ? Convert.FromBase64String(parts[1])
                : new UnicodeEncoding().GetBytes("XenRocks"); //backwards compatibility
            byte[] protectedBytes = Convert.FromBase64String(parts[0]);
            byte[] dataBytes = ProtectedData.Unprotect(protectedBytes, saltBytes, scope);
            return Encoding.Unicode.GetString(dataBytes);
        }

        /// <summary>
        /// Encrypt a given string using the given key.
        /// </summary>
        /// <param name="clearString">The string to encrypt.</param>
        /// <param name="keyBytes">The key for the encryption algorithm</param>
        /// <returns>The Base64 encoded cipher text</returns>
        public static string EncryptString(string clearString, byte[] keyBytes)
        {
            byte[] saltBytes = GetSalt();

            using (var alg = new AesManaged
                   {
                       Key = keyBytes,
                       IV = saltBytes,
                       Padding = PaddingMode.PKCS7,//default value
                       Mode = CipherMode.CBC//default value
                   })
            {
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] clearBytes = Encoding.Unicode.GetBytes(clearString);
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.FlushFinalBlock(); //important for getting the padding right
                    return $"{Convert.ToBase64String(ms.ToArray())},{Convert.ToBase64String(saltBytes)}";
                }
            }
        }

        /// <summary>
        /// Decrypt some cipher text that was created by the encryptString method.
        /// </summary>
        /// <param name="cipherText64">The base64 encoded cipher text that was produced
        /// by encryptString</param>
        /// <param name="key">The key for the decryption algorithm</param>
        /// <returns>The decrypted text.</returns>
        public static string DecryptString(string cipherText64, string key)
        {
            var parts = cipherText64.Split(new[] {','}, 2);
            byte[] saltBytes = parts.Length == 2
                ? Convert.FromBase64String(parts[1])
                : new UnicodeEncoding().GetBytes("XenRocks"); //backwards compatibility
            byte[] cipherBytes = Convert.FromBase64String(parts[0]);

            try
            {
                using (var alg = new AesManaged
                       {
                           IV = saltBytes,
                           Key = ComputeHash(key),
                           Padding = PaddingMode.PKCS7,//default value
                           Mode = CipherMode.CBC//default value
                       })
                    return DecryptString(cipherBytes, alg);
            }
            catch (Exception e)
            {
                log.Warn("Failed to decrypt. Trying legacy mode.", e);

                using (var alg = Rijndael.Create())
                {
                    alg.IV = saltBytes;
                    alg.Key = ComputeHash(key, HashMethod.Md5);
                    return DecryptString(cipherBytes, alg); //backwards compatibility
                }
            }
        }

        private static string DecryptString(byte[] cipherBytes, SymmetricAlgorithm alg)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock(); //important for getting the padding right
                    return Encoding.Unicode.GetString(ms.ToArray());
                }
            }
        }

        private static byte[] GetSalt()
        {
            using (var rngCsProvider = new RNGCryptoServiceProvider())
            {
                var saltBytes = new byte[SALT_LENGTH];
                rngCsProvider.GetBytes(saltBytes);
                return saltBytes;
            }
        }
    }
}
