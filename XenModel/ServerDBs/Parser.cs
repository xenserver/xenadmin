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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Citrix.XenCenter;

using XenAdmin.Core;
using System.Text;

namespace XenAdmin.ServerDBs
{
    /// <summary>
    /// A class which provides various XAPI parsing methods.
    /// </summary>
    public class Parser
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Parses the specified value to the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to which the value should be parsed to.</param>
        /// <param name="value">The value to be parsed.</param>
        /// <returns></returns>
        public static Object Parse(Type type, String value)
        {
            Util.ThrowIfParameterNull(type, "type");
            Util.ThrowIfParameterNull(value, "value");

            try
            {
                if (type == typeof(String[]))
                {
                    return ParseSXPList(value);
                }
                if (type == typeof(String))
                {
                    return DeEscapeWhiteSpace(value);
                }
                if (type == typeof(bool))
                {
                    return Boolean.Parse(value);
                }
                if (type == typeof(DateTime))
                {
                    return TimeUtil.ParseISO8601DateTime(value);
                }
                if (type == typeof(double))
                {
                    return Double.Parse(value);
                }
                if (type == typeof(long))
                {
                    return long.Parse(value);
                }
                return ParseSXPDict(value);
            }
            catch (Exception e)
            {
                log.Debug(e, e);
                return null;
            }
        }

        /// <summary>
        /// Xapi has some additional whitespace escaping rules which we need to catch
        /// two spaces become %_
        /// '\n' becomes %n
        /// '\r' becomes %r
        /// '\t' becomes %t
        /// '%' becomes '%%'
        /// </summary>
        /// <param name="value"></param>
        /// <returns>DeEscapedString</returns>
        private static string DeEscapeWhiteSpace(string value)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '%')
                {
                    i++;
                    // skip to next char as long as it's not the end, and interpret
                    if (i < value.Length)
                    {
                        switch (value[i])
                        {
                            case '_': sb.Append("  "); break;
                            case 'n': sb.Append("\n"); break;
                            case 'r': sb.Append("\r"); break;
                            case 't': sb.Append("\t"); break;
                            case '%': sb.Append("%"); break;
                        }
                    }
                }
                else
                    sb.Append(value[i]);

            }

            return sb.ToString();
        }

        /// <summary>
        /// Unparses the value for the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to be unparsed from.</param>
        /// <param name="value">The value to be unparsed.</param>
        /// <returns></returns>
        public static string Unparse(Type type, object value)
        {
            Util.ThrowIfParameterNull(type, "type");
            Util.ThrowIfParameterNull(value, "value");

            try
            {
                if (type == typeof(string[]))
                {
                    return ToSXPList((string[])value);
                }
                if (type == typeof(DateTime))
                {
                    return TimeUtil.ToISO8601DateTime((DateTime)value);
                }
                if (type == typeof(string) || type == typeof(bool) || type == typeof(double))
                {
                    return value.ToString();
                }
                return ToSXPDict((Hashtable)value);
            }
            catch (Exception e)
            {
                log.Debug(e, e);
                return null;
            }
        }

        public static Hashtable ParseSXPDict(String p)
        {
            Hashtable result = new Hashtable();

            IEnumerator<String> enumerator = Tokenize(p).GetEnumerator();

            if (!enumerator.MoveNext())
            {
                return result;
            }

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == ")")
                {
                    break;
                }

                enumerator.MoveNext();
                String key = enumerator.Current;
                enumerator.MoveNext();
                String value = enumerator.Current;
                enumerator.MoveNext();

                result.Add(key, value);
            }

            return result;
        }

        public static string ToSXPDict(IDictionary dict)
        {
            string ans = "(";
            bool first = true;
            foreach (object k in dict.Keys)
            {
                if (!first)
                {
                    ans += " ";
                }
                ans += "('" + EscapeString((string)k) + "' '" + EscapeString((string)dict[k]) + "')";
                first = false;
            }
            ans += ")";
            return ans;
        }

        public static String[] ParseSXPList(String p)
        {
            List<String> result = new List<String>();

            foreach (String token in Tokenize(p))
            {
                if (token == "(" || token == ")")
                {
                    continue;
                }

                result.Add(token);
            }

            return result.ToArray();
        }

        public static string ToSXPList(IEnumerable<string> vals)
        {
            string ans = "(";
            bool first = true;
            foreach (string val in vals)
            {
                if (!first)
                {
                    ans += " ";
                }
                ans += "'" + EscapeString(val) + "'";
                first = false;
            }
            ans += ")";
            return ans;
        }

        private static string EscapeString(string s)
        {
            return
                s.Replace("\"", "\\\"")
                 .Replace("\'", "\\\'");
        }

        public static IEnumerable<String> Tokenize(String str)
        {
            bool inStr = false;
            int j = 0;

            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '(':
                        if (!inStr)
                        {
                            yield return "(";
                        }
                        break;

                    case ')':
                        if (!inStr)
                        {
                            yield return ")";
                        }
                        break;

                    case '\'':
                    case '"':
                        if (!inStr)
                        {
                            inStr = true;
                            j = i;
                        }
                        else if (str[i - 1] != '\\')
                        {
                            inStr = false;
                            yield return str.Substring(j + 1, i - j - 1).Replace("\\\"", "\"").Replace("\\\'", "\'");
                        }
                        break;
                }
            }
        }
    }
}
