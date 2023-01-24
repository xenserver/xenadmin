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
using System.Linq;


namespace XenCenterLib
{
    /// <summary>
    /// Parses .ini files - content of the following type:
    ///   [section]
    ///   name1 = value1
    ///   name2 = value2
    /// </summary>
    public class IniDocument
    {
        public List<Section> Sections { get; } = new List<Section>();

        public IniDocument(string theString)
        {
            using (var sr = new StringReader(theString))
                Parse(sr);
        }

        public IniDocument(Stream stream)
        {
            using (var sr = new StreamReader(stream))
                Parse(sr);
        }

        private void Parse(TextReader textReader)
        {
            Sections.Clear();

            string line;
            while ((line = textReader.ReadLine()) != null)
            {
                line = line.Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                    Sections.Add(new Section(line.TrimStart('[').TrimEnd(']')));
                else if (Sections.Count > 0)
                {
                    var parts = line.Split(new[] {'='}, StringSplitOptions.None);
                    if (parts.Length != 2 || parts[0] == null)
                        continue;

                    var key = parts[0].Trim();
                    if (key == "")
                        continue;

                    var value = parts[1] == null ? "" : parts[1].Trim();
                    Sections[Sections.Count - 1].Entries.Add(new Entry(key, value));
                }
            }
        }

        public Section FindSection(string key)
        {
            return Sections.FirstOrDefault(e => e.Key == key);
        }

        public Entry FindEntry(string sectionName, string entryName)
        {
            Section section;
            if ((section = FindSection(sectionName)) != null)
                return section.FindEntry(entryName);

            return null;
        }

        #region Nested classes

        public class Section
        {
            public string Key { get; }

            public List<Entry> Entries { get; }

            public Section(string key, List<Entry> entries = null)
            {
                Key = key;
                Entries = entries ?? new List<Entry>();
            }

            public Entry FindEntry(string key)
            {
                return Entries.FirstOrDefault(e => e.Key == key);
            }
        }

        public class Entry
        {
            public string Key { get; }

            public string Value { get; }

            public Entry(string key, string value)
            {
                Key = key;
                Value = value;
            }
        }

        #endregion
    }
}
