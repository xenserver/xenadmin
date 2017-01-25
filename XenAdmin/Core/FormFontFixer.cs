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

/*
 *
 * Based loosely upon code copyrighted as below.
 *
 */

/*
 * 
Copyright (c) 2008, TeX HeX (http://www.texhex.info/)

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

   * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
   * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
   * Neither the name of the Xteq Systems (http://www.xteq.com/) nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

Original idea and code (3 lines :-) by Benjamin Hollis: http://brh.numbera.com/blog/index.php/2007/04/11/setting-the-correct-default-font-in-net-windows-forms-apps/

*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Core
{
    public static class FormFontFixer
    {
        [AttributeUsage(AttributeTargets.Class)]
        public class PreserveFonts : Attribute
        {
            public bool Preserve;
            public PreserveFonts(bool preserve)
            {
                Preserve = preserve;
            }
        }

        //This list contains the fonts we want to replace.
        private static readonly List<string> FontReplaceList
            = new List<string>(new[] {"Microsoft Sans Serif", "Tahoma", "Verdana", "Segoe UI", "Meiryo", "Meiryo UI", "MS UI Gothic", "Arial", "\u5b8b\u4f53"});

        /// <summary>
        /// May be null, implying that the fonts cannot be fixed.
        /// </summary>
        public static Font DefaultFont;

        static FormFontFixer()
        {
            //Basically the font name we want to use should be easy to choose by using the SystemFonts class. However, this class
            //is hard-coded (!!) and doesn't seem to work right. On XP, it will mostly return "Microsoft Sans Serif" except
            //for the DialogFont property (=Tahoma) but on Vista, this class will return "Tahoma" instead of "SegoeUI" for this property!

            //Therefore we will do the following: If we are running on a OS below XP, we will exit because the only font available
            //will be MS Sans Serif. On XP, we gonna use "Tahoma", and any other OS we will use the value of the MessageBoxFont
            //property because this seems to be set correctly on Vista an above.

            DefaultFont =
                Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Version.Major < 5 ?
                    null : // 95, 98, and NT can't be fixed.
                Environment.OSVersion.Version.Major < 6 ?
                    SystemFonts.DialogFont : // 2K (5.0), XP (5.1), 2K3 and XP Pro (5.2), using Tahoma by default
                    SystemFonts.MessageBoxFont; // Vista and above, using SegoeUI by default
        }

        public static void Fix(Form form)
        {
            if (DefaultFont == null)
                return;

            RegisterAndReplace(form);
        }

        private static void RegisterAndReplace(Control c)
        {
            if (ShouldPreserveFonts(c))
                return;

            if (c is ToolStrip)
            {
                ToolStrip t = (ToolStrip)c;
                Register(t);
                Replace(t);
                foreach (ToolStripItem d in t.Items)
                    Replace(d);
            }
            else
            {
                Register(c);
                Replace(c);
                foreach (Control d in c.Controls)
                    RegisterAndReplace(d);
            }
        }

        private static bool ShouldPreserveFonts(Control c)
        {
            Type t = c.GetType();
            object [] arr = t.GetCustomAttributes(typeof(PreserveFonts), true);
            return arr.Length > 0 && ((PreserveFonts)arr[0]).Preserve;
        }

        private static void Deregister(Control c)
        {
            c.ControlAdded -= c_ControlAdded;
            c.ControlRemoved -= c_ControlRemoved;

            foreach (Control d in c.Controls)
                Deregister(d);
        }

        private static void Deregister(ToolStrip c)
        {
            c.ItemAdded -= c_ItemAdded;
        }

        private static void Register(Control c)
        {
            if (!IsLeaf(c))
            {
                c.ControlAdded -= c_ControlAdded;
                c.ControlAdded += c_ControlAdded;

                c.ControlRemoved -= c_ControlRemoved;
                c.ControlRemoved += c_ControlRemoved;
            }
        }

        private static void Register(ToolStrip c)
        {
            c.ItemAdded -= c_ItemAdded;
            c.ItemAdded += c_ItemAdded;
        }

        static void c_ControlAdded(object sender, ControlEventArgs e)
        {
            RegisterAndReplace(e.Control);
        }

        static void c_ControlRemoved(object sender, ControlEventArgs e)
        {
            Deregister(e.Control);
        }

        static void c_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            Replace(e.Item);
        }

        private static bool IsLeaf(Control c)
        {
            return c is Button || c is Label || c is TextBox || c is ComboBox || c is ListBox;
        }

        private static void Replace(Control c)
        {
            c.Font = ReplacedFont(c.Font);
        }

        private static void Replace(ToolStripItem c)
        {
            c.Font = ReplacedFont(c.Font);
        }

        private static Font ReplacedFont(Font f)
        {
            //only replace fonts that use one the "system fonts" we have declared
            if (FontReplaceList.IndexOf(f.Name) > -1)
            {
                //Now check the size, when the size is 9 or below it's the default font size and we do not keep the size since
                //SegoeUI has a complete different spacing (and thus size) than MS SansS or Tahoma.

                //Also check if there are any styles applied on the font (e.g. Italic) which we need to apply to the new
                //font as well.

                bool UseDefaultSize = f.Size >= 8 && f.Size <= 9;
                bool UseDefaultStyle = !f.Italic && !f.Strikeout && !f.Underline && !f.Bold;

                //if everything is set to defaults, we can use our prepared font right away
                if (UseDefaultSize && UseDefaultStyle)
                {
                    return DefaultFont;
                }
                else
                {
                    //There are non default properties set so
                    //there is some work we need to do...
                    return new Font(DefaultFont.FontFamily, UseDefaultSize ? DefaultFont.SizeInPoints : f.SizeInPoints, f.Style);
                }
            }
            else
            {
                return f;
            }
        }
    }
}
