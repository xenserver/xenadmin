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
using System.Drawing;
using XenAdmin.Core;
using XenAPI;
using System.Text.RegularExpressions;


namespace XenAdmin.Controls.CustomDataGraph
{
    public class Palette
    {
        public static readonly Pen AxisPen = SystemPens.ControlText;
        public static readonly Pen GridPen = new Pen(Color.FromArgb(64,SystemColors.GrayText));
        public static readonly Font LabelFont = Program.DefaultFont;
        public static readonly Brush GraphShadow = Brushes.LightBlue;
        public static readonly Brush LabelBrush = SystemBrushes.ControlText ?? Brushes.Black;
        public static readonly Brush PaperBrush = SystemBrushes.Window;
        public static readonly SolidBrush ShadowRangeBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0));

        public static object PaletteLock = new object();

        public const float PEN_THICKNESS_THIN = 1f;
        public const float PEN_THICKNESS_NORMAL = 1.5f;
        public const float PEN_THICKNESS_THICK = 2f;
        private const int GraphShadowAlpha = 128;

        private const string OtherConfigDataSource = "DataSource";
        private const string OtherConfigGraphLayout = "GraphLayout";
        private const string OtherConfigGraphName = "GraphName";

        public static Regex GraphNameKey = new Regex(string.Format("^XenCenter.{0}.[0-9]+.(host|vm).[a-zA-Z0-9_\\-]+$", OtherConfigGraphName));
        public static Regex LayoutKey = new Regex(string.Format("^XenCenter.{0}.[0-9]+.(host|vm).[a-zA-Z0-9_\\-]+$", OtherConfigGraphLayout));
        public static Regex OtherConfigUUIDRegex = new Regex(string.Format("^XenCenter.{0}.(host|vm).[a-zA-Z0-9_\\-]+", OtherConfigDataSource));

        private static readonly Color[] DefaultColors = new [] {
                Color.FromArgb(0,102,255),      // blue
                Color.FromArgb(51,153,0),       // green
                Color.FromArgb(255,153,0),      // orange
                Color.FromArgb(102,0,153),      // purple
                Color.FromArgb(153,0,51),       // red(ish)
                Color.FromArgb(153,94,0),       // brown
                Color.FromArgb(255,255,0),      // yellow
                Color.FromArgb(0,255,180),      // light-green
                Color.FromArgb(0,153,130),      // green(ish)
                Color.FromArgb(191,164,166),    // gray(ish)
            };


        private static Dictionary<string, Color> DefaultDatasetColours = new Dictionary<string, Color>();
        private static Dictionary<string, Color> CustomDatasetColours = new Dictionary<string, Color>();

        public static Color GetColour(string uuid)
        {
            if (CustomDatasetColours.ContainsKey(uuid))
                return CustomDatasetColours[uuid];

            if (!DefaultDatasetColours.ContainsKey(uuid))
                DefaultDatasetColours[uuid] = DefaultColors[DefaultDatasetColours.Count % DefaultColors.Length];

            return DefaultDatasetColours[uuid];
        }

        /// <summary>
        /// Will return null if FillAreaUnderGraphs is false.
        /// Use under Palette.PaletteLock to ensure colour doesn't get changed
        /// under you. Also use within a using(){} block to ensure disposal.
        /// </summary>
        public static Brush CreateBrush(string uuid)
        {
            if (!Properties.Settings.Default.FillAreaUnderGraphs)
                return null;

            Color c = GetColour(uuid);
            return new SolidBrush(Color.FromArgb(GraphShadowAlpha, c));
        }

        /// <summary>
        /// Use under Palette.PaletteLock to ensure colour doesn't get changed
        /// under you. Also use within a using(){} block to ensure disposal.
        /// </summary>
        public static Pen CreatePen(string uuid, float thickness)
        {
            Color c = GetColour(uuid);
            return new Pen(c, thickness);
        }

        public static bool HasCustomColour(string uuid)
        {
            return CustomDatasetColours.ContainsKey(uuid);
        }

        public static void SetCustomColor(string uuid, Color c)
        {
            //if the dataset has the default colour, remove it
            if (DefaultDatasetColours.ContainsKey(uuid))
                DefaultDatasetColours.Remove(uuid);

            //no need to check if the key exists; if it doesn't it will be set
            CustomDatasetColours[uuid] = c;
        }

        internal static void LoadSetColor(DataSet set)
        {
            if (set.XenObject == null)
                return;

            Pool pool = Helpers.GetPoolOfOne(set.XenObject.Connection);

            if(pool == null)
                return;

            string key = GetColorKey(set.TypeString, set.XenObject);

            Dictionary<string, string> gui_config = Helpers.GetGuiConfig(pool);

            if(gui_config == null || !gui_config.ContainsKey(key))
                return;

            int argb;
            if (!int.TryParse(gui_config[key], out argb))
                return;

            SetCustomColor(set.Uuid, Color.FromArgb(argb));
        }

        public static string GetColorKey(string ds_name, IXenObject xo)
        {
            if (xo is Host)
            {
                Host host = (Host)xo;
                return string.Format("XenCenter.{0}.host.{1}.{2}", OtherConfigDataSource, host.uuid, ds_name);
            }
            else if (xo is VM)
            {
                VM vm = (VM)xo;
                return string.Format("XenCenter.{0}.vm.{1}.{2}", OtherConfigDataSource, vm.uuid, ds_name);
            }
            return "";
        }

        public static string GetLayoutKey(int index, IXenObject xmo)
        {
            return GetGuiConfigKey(OtherConfigGraphLayout, index, xmo);
        }

        public static string GetGraphNameKey(int index, IXenObject xmo)
        {
            return GetGuiConfigKey(OtherConfigGraphName, index, xmo);
        }

        private static string GetGuiConfigKey(string keytype, int index, IXenObject xmo)
        {
            if (xmo is Host)
            {
                Host host = (Host)xmo;
                return string.Format("XenCenter.{0}.{1}.host.{2}", keytype, index, host.uuid);
            }
            if (xmo is VM)
            {
                VM vm = (VM)xmo;
                return string.Format("XenCenter.{0}.{1}.vm.{2}", keytype, index, vm.uuid);
            }
            return "";
        }

        public static string GetUuid(string ds_name, IXenObject xmo)
        {
            if (xmo is Host)
            {
                Host host = (Host)xmo;
                return string.Format("host:{0}:{1}", host.uuid, ds_name);
            }
            if (xmo is VM)
            {
                VM vm = (VM)xmo;
                return string.Format("vm:{0}:{1}", vm.uuid, ds_name);
            }
            return "";
        }
    }
}
