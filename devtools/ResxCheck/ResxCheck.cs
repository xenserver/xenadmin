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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;


namespace ResxCheck
{
    static class ResxCheck
    {
        /// <summary>
        /// Produces a list of unused resources in Messages and FriendlyNames.
        /// </summary>
        /// <param name="removeUnused">If true, will actually purge unused messages from the Messages.resx file</param>
        public static void FindUnusedMessages(string rootDir, bool removeUnused)
        {
            Assembly assembly = Assembly.LoadFrom(Path.Combine(rootDir, @"XenModel\bin\Debug\XenModel.dll"));

            int totalMessages = 0, totalFriendlyErrorNames = 0;
            var resources = new List<string>();

            Type messagesType = assembly.GetType("XenAdmin.Messages");
            Type friendlyNamesType = assembly.GetType("XenAdmin.FriendlyNames");

            foreach (PropertyInfo property in messagesType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                resources.Add("Messages." + property.Name.Trim());
                totalMessages++;
            }
            foreach (PropertyInfo property in friendlyNamesType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic))
            {
                resources.Add("FriendlyNames." + property.Name.Trim());
                totalFriendlyErrorNames++;
            }

            // Build file list for project
            List<FileInfo> files = new List<FileInfo>();
            RecursiveGetCsFiles(new DirectoryInfo(rootDir), files);
            files.RemoveAll(f => f.Name.StartsWith("Messages.") || f.Name.StartsWith("FriendlyNames."));
            Console.WriteLine(string.Format("Looking in {0} files", files.Count));

            // Now remove resources from the list if they appear in source files
            foreach (FileInfo fileinfo in files)
            {
                string[] lines = File.ReadAllLines(fileinfo.FullName);
                foreach (string line in lines)
                {
                    string curLine = line;
                    resources.RemoveAll(resource => curLine.Contains(resource));
                }
            }

            int messages = 0, friendlyErrorNames = 0;
            foreach (string unused in resources)
            {
                if (unused.StartsWith("Messages."))
                {
                    messages++;
                }
                else if (unused.StartsWith("FriendlyNames."))
                {
                    friendlyErrorNames++;
                }
                Console.WriteLine(unused);
            }
            Console.WriteLine(string.Format("Messages.resx: {0}/{1} are unused", messages, totalMessages));
            Console.WriteLine(string.Format("FriendlyNames.resx: {0}/{1} are unused", friendlyErrorNames, totalFriendlyErrorNames));

            // Remove unused messages from Messages.rex. Note that this method is extremely
            // crude and depends on the exact format of the XML.
            if (removeUnused)
            {
                Console.WriteLine("Removing unused messages from Messages.resx");

                List<string> unusedFromMessages = new List<string>();
                foreach (string line in resources)
                {
                    if (line.StartsWith("Messages."))
                    {
                        unusedFromMessages.Add(line.Substring(9));
                    }
                }

                string path = Path.Combine(rootDir, "Messages.resx");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(path));

                List<XmlNode> nodesToRemove = new List<XmlNode>();
                foreach (XmlNode node in doc.GetElementsByTagName("data"))
                {
                    if (unusedFromMessages.Contains(node.Attributes["name"].Value))
                    {
                        nodesToRemove.Add(node);
                    }
                }

                foreach (XmlNode node in nodesToRemove)
                {
                    doc.ChildNodes[1].RemoveChild(node);
                }

                doc.Save(path);
            }
        }

        private static void RecursiveGetCsFiles(DirectoryInfo dir, List<FileInfo> files)
        {
            files.AddRange(dir.GetFiles("*.cs"));
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                RecursiveGetCsFiles(subdir, files);
            }
        }

        private static void RecursiveGetResxFiles(DirectoryInfo dir, List<FileInfo> files)
        {
            files.AddRange(dir.GetFiles("*.resx"));
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                if (subdir.Name == "i18n")
                    continue;
                RecursiveGetResxFiles(subdir, files);
            }
        }

        private static void FindNodesInJaButNotEn(string rootDir)
        {
            // Find all english resxs
            List<FileInfo> enResxFiles = new List<FileInfo>();
            RecursiveGetResxFiles(new DirectoryInfo(rootDir), enResxFiles);

            foreach (FileInfo enResxFile in enResxFiles)
            {
                string enResxPath = enResxFile.FullName;
                XmlDocument enXml = new XmlDocument();
                enXml.LoadXml(File.ReadAllText(enResxPath));

                string jaFilename = enResxPath.Substring(rootDir.Length);
                jaFilename = jaFilename.Insert(jaFilename.Length - 5, ".ja");
                string jaResxPath = rootDir + "\\i18n\\ja" + jaFilename;
                XmlDocument jaXml = new XmlDocument();
                if (!File.Exists(jaResxPath))
                {
                    continue;
                }
                jaXml.LoadXml(File.ReadAllText(jaResxPath));

                XmlNodeList enDataNodes = enXml.GetElementsByTagName("data");
                XmlNodeList jaDataNodes = jaXml.GetElementsByTagName("data");

                List<XmlNode> jaDataNodeList = new List<XmlNode>();
                foreach (XmlNode jaNode in jaDataNodes)
                {
                    jaDataNodeList.Add(jaNode);
                }

                List<XmlNode> inJaButNotEn = jaDataNodeList.FindAll((Predicate<XmlNode>)delegate(XmlNode jaNode)
                {
                    string jaDataName = jaNode.Attributes["name"].Value;
                    foreach (XmlNode enNode in enDataNodes)
                    {
                        if (enNode.Attributes["name"].Value == jaDataName)
                        {
                            return enNode.InnerXml != jaNode.InnerXml;
                        }
                    }
                    return true;
                });

                foreach (XmlNode node in inJaButNotEn)
                {
                    System.Console.WriteLine(string.Format("'{0}' is in '{1}' but not in '{2}'",
                        node.Attributes["name"].Value,
                        jaResxPath,
                        enResxFile.Name));
                }
            }
        }

        private static readonly string[] i18nYes = new string[] { "Text", "ToolTipText", "HeaderText", "AccessibleDescription", "ToolTip", "Filter" };
        private static readonly string[] i18nNo = new string[] { "ZOrder", "Size", "Location", "Anchor", "Type", "MinimumSize", "ClientSize",
            "Font", "TabIndex", "Parent", "LayoutSettings", "Margin", "Padding", "ColumnCount", "Dock", "AutoSize", "Name", "ImeMode",
            "IntegralHeight", "Visible", "InitialImage", "AutoScaleDimensions", "FlowDirection", "RowCount", "ImageAlign", "WrapContents",
            "Enabled", "TextAlign", "StartPosition", "SizeMode", "Multiline", "ScrollBars", "ItemHeight", "CellBorderStyle", "AutoSizeMode",
            "Image", "AutoCompleteCustomSource", "AutoCompleteCustomSource1", "AutoCompleteCustomSource2", "BulletIndent", "Width",
            "MinimumWidth", "AutoScroll", "ImageSize", "MaxLength", "BackgroundImageLayout", "ImageTransparentColor", "ImageIndex",
            "SplitterDistance", "MaximumSize", "ThousandsSeparator", "RightToLeft", "TextImageRelation", "ContentAlignment",
            "SelectedImageIndex", "HorizontalScrollbar", "CheckAlign", "RightToLeftLayout", "ShowShortcutKeys", "ShortcutKeys",
            "ShortcutKeyDisplayString", "Localizable", "Icon", "Menu", "AutoScrollMinSize", "Items", "ScrollAlwaysVisible",
            "Items1", "Items2", "Items3", "MaxDropDownItems" };
        private static bool IsI18nableProperty(string filename, string name)
        {
            foreach (string property in i18nYes)
            {
                if (name.EndsWith("." + property))
                {
                    // Keep these tags
                    return true;
                }
            }

            foreach (string property in i18nNo)
            {
                if (name.EndsWith("." + property))
                {
                    // Reject these tags
                    return false;
                }
            }

            // We haven't seen these tags before - keep them but issue a notification
            Console.WriteLine(filename + ": " + name);
            return true;
        }

        private static bool ExcludeResx(string filePath)
        {
            return filePath.EndsWith(@"\Properties\Resources.resx") ||
                filePath.EndsWith(@"\Help\HelpManager.resx") ||
                filePath.EndsWith(@"\DotNetVnc\KeyMap.resx");
        }
        
        /// <summary>
        /// Try from the immediate window e.g.
        /// XenAdmin.ResxCheck.TrimJaResxs(@"C:\Documents and Settings\hwarrington\xenadmin-unstable.hg\XenAdmin")
        /// </summary>
        /// <param name="rootDir"></param>
        private static void TrimJaResxs(string rootDir)
        {
            // Find all english resxs
            List<FileInfo> enResxFiles = new List<FileInfo>();
            RecursiveGetResxFiles(new DirectoryInfo(rootDir), enResxFiles);

            List<string> names = new List<string>();

            foreach (FileInfo enResxFile in enResxFiles)
            {
                if (ExcludeResx(enResxFile.FullName))
                {
                    continue;
                }

                // Load the en resx
                string enResxPath = enResxFile.FullName;
                XmlDocument enXml = new XmlDocument();
                enXml.LoadXml(File.ReadAllText(enResxPath));
                XmlNodeList enDataNodes = enXml.GetElementsByTagName("data");

                // Find the ja resx
                string jaFilename = enResxPath.Substring(rootDir.Length);
                jaFilename = jaFilename.Insert(jaFilename.Length - 5, ".ja");
                string jaResxPath = rootDir + "\\i18n\\ja" + jaFilename;
                
                if (!File.Exists(jaResxPath))
                {
                    // There is no ja resx file corresponding to the en resx. We need to check there are no i18nable tags
                    // in the en resx.
                    bool i18nRequired = false;
                    foreach (XmlNode enNode in enDataNodes)
                    {
                        if (IsI18nableProperty(enResxFile.Name, enNode.Attributes["name"].Value))
                        {
                            Console.WriteLine(string.Format("{0} is missing. Tag {1} needs i18n. Copying en resx across.", jaFilename, enNode.Attributes["name"].Value));
                            Directory.CreateDirectory(Path.GetDirectoryName(jaResxPath));
                            File.Copy(enResxPath, jaResxPath);
                            i18nRequired = true;
                            break;
                        }
                    }
                    if (!i18nRequired)
                    {
                        continue;
                    }
                }

                // Load the ja resx
                XmlDocument jaXml = new XmlDocument();
                jaXml.LoadXml(File.ReadAllText(jaResxPath));
                XmlNodeList jaDataNodes = jaXml.GetElementsByTagName("data");
                // Take a copy of the jaDataNodes
                List<XmlNode> jaDataNodeList = new List<XmlNode>();
                foreach (XmlNode node in jaDataNodes)
                {
                    jaDataNodeList.Add(node);
                }

                // Go through all the ja nodes, keeping only the ones where their values differ from the en original
                // Don't bother to do this for the messages files.
                if (enResxFile.Name != "Messages.resx" && enResxFile.Name != "FriendlyNames.resx" &&
                    enResxFile.Name != "FriendlyNames.resx")
                {
                    foreach (XmlNode jaNode in jaDataNodeList)
                    {
                        string jaDataName = jaNode.Attributes["name"].Value;

                        if (!IsI18nableProperty(jaFilename, jaDataName))
                        {
                            // Delete node
                            jaXml.GetElementsByTagName("root")[0].RemoveChild(jaNode);
                            continue;
                        }

                        foreach (XmlNode enNode in enDataNodes)
                        {
                            if (enNode.Attributes["name"].Value == jaDataName)
                            {
                                if (enNode.InnerXml == jaNode.InnerXml)
                                {
                                    // If node unchanged, delete it
                                    jaXml.GetElementsByTagName("root")[0].RemoveChild(jaNode);
                                    break;
                                }
                            }
                        }
                    }
                }

                // Now add any nodes that are in en but not ja (as long as they are of the i18nable types).
                foreach (XmlNode enNode in enDataNodes)
                {
                    bool needToAdd = true;

                    foreach (XmlNode jaNode in jaDataNodes)
                    {
                        if (enNode.Attributes["name"].Value == jaNode.Attributes["name"].Value)
                        {
                            needToAdd = false;
                            break;
                        }
                    }

                    if (needToAdd && IsI18nableProperty(enResxFile.Name, enNode.Attributes["name"].Value))
                    {
                        XmlNode n = jaXml.GetElementsByTagName("root")[0].AppendChild(jaXml.ImportNode(enNode, true));
                        foreach (XmlNode child in n.ChildNodes)
                        {
                            if (child is XmlWhitespace)
                                continue;
                            if (child is XmlSignificantWhitespace)
                                continue;
                            else
                                child.InnerText += " (ja)";
                        }
                    }
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.CloseOutput = true;
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(jaResxPath, settings);
                jaXml.WriteContentTo(writer);
                writer.Flush();
                writer.Close();
            }

            Console.WriteLine("Done");
        }

        /// <summary>
        /// Checks I haven't screwed stuff up while changing the autogen resx stuff.
        /// </summary>
        public static void CheckNotBorked()
        {
            XmlDocument origXml = new XmlDocument();
            origXml.LoadXml(File.ReadAllText(@"C:\Documents and Settings\hwarrington\xenadmin-unstable.hg\XenAdmin\XenAPI\FriendlyNames.resx"));

            XmlDocument newXml = new XmlDocument();
            newXml.LoadXml(File.ReadAllText(@"Q:\local\scratch-2\hwarrington\build.hg\myrepos\api.hg\ocaml\idl\csharp_backend\autogen-gui\FriendlyNames.resx"));

            CheckAIncludesB(origXml, newXml);
            CheckAIncludesB(newXml, origXml);
        }

        private static XmlNode FindByName(XmlDocument doc, string name)
        {
            foreach (XmlNode node in doc.GetElementsByTagName("data"))
            {
                if (name == node.Attributes["name"].Value)
                    return node;
            }
            return null;
        }

        private static void CheckAIncludesB(XmlDocument origXml, XmlDocument newXml)
        {
            XmlNodeList origDataNodes = origXml.GetElementsByTagName("data");
   
            foreach (XmlNode oldNode in origDataNodes)
            {
                string name = oldNode.Attributes["name"].Value;
                string oldValue = oldNode.InnerXml;

                XmlNode newNode = FindByName(newXml, name);
                if (newNode == null)
                {
                    throw new Exception(String.Format("Node with name {0} exists in old but not new!", name));
                }
                string newValue = newNode.InnerXml;
                if (newValue != oldValue)
                {
                    throw new Exception(String.Format("Node with name {0} has value {1} in old but {2} in new!", name, oldValue, newValue));
                }
            }
        }
    }
}
