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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;

namespace XenAdmin.TabPages.CdnUpdates
{
    internal static class CdnExtensions
    {
        internal static string InitialOf(this CdnUpdateType updateType)
        {
            switch (updateType)
            {
                case CdnUpdateType.SecurityFix:
                    return "S";
                case CdnUpdateType.Bugfix:
                    return "B";
                case CdnUpdateType.Improvement:
                    return "I";
                case CdnUpdateType.NewFeature:
                    return "N";
                case CdnUpdateType.PreviewFeature:
                    return "P";
                case CdnUpdateType.Foundational:
                    return "F";
                default:
                    return "?";
            }
        }

        internal static Color ColorOf(this CdnUpdateType updateType)
        {
            switch (updateType)
            {
                case CdnUpdateType.SecurityFix:
                    return Color.Red;
                case CdnUpdateType.Bugfix:
                    return Color.DarkOrange;
                case CdnUpdateType.Improvement:
                    return Color.Gold;
                case CdnUpdateType.NewFeature:
                    return Color.Green;
                case CdnUpdateType.PreviewFeature:
                    return Color.DodgerBlue;
                case CdnUpdateType.Foundational:
                    return Color.Tan;
                default:
                    return Color.DarkGray;
            }
        }

        internal static string CollateDetails(this CdnUpdate update)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(update.Description) && update.Description.ToLower() != "none")
                sb.AppendLine(update.Description).AppendLine();

            if (!string.IsNullOrWhiteSpace(update.SpecialInfo) && update.SpecialInfo.ToLower() != "none")
                sb.AppendLine(update.SpecialInfo).AppendLine();

            if (!string.IsNullOrWhiteSpace(update.Url) && update.Url.ToLower() != "none")
                sb.AppendLine(update.Url).AppendLine();

            if (update.LivePatches?.Length > 0)
                sb.AppendLine(string.Format(Messages.CDN_LIVEPATCHES_IN_UPDATE,
                    string.Join(", ", update.LivePatches.Select(lp => lp.Component).Distinct()))).AppendLine();

            return sb.ToString().Trim();
        }

        internal static List<(CdnUpdateType, List<CdnUpdate>)> GetUpdateCategories(this CdnHostUpdateInfo hostUpdateInfo, CdnPoolUpdateInfo poolUpdateInfo)
        {
            return (from string id in hostUpdateInfo.UpdateIDs
                let update = poolUpdateInfo.Updates.FirstOrDefault(u => u.Id == id)
                group update by update.Type
                into category
                orderby category.Key
                where category.Any()
                select (category.Key, category.ToList())).ToList();
        }

        internal static string GetCategoryTitle(this CdnUpdateType updateType, int numberOfUpdates)
        {
            switch (updateType)
            {
                case CdnUpdateType.Bugfix:
                    return numberOfUpdates == 1
                        ? Messages.HOTFIX_TYPE_BUG_FIX_ONE
                        : string.Format(Messages.HOTFIX_TYPE_BUG_FIX_MANY, numberOfUpdates);
                case CdnUpdateType.SecurityFix:
                    return numberOfUpdates == 1
                        ? Messages.HOTFIX_TYPE_SECURITY_FIX_ONE
                        : string.Format(Messages.HOTFIX_TYPE_SECURITY_FIX_MANY, numberOfUpdates);
                case CdnUpdateType.NewFeature:
                    return numberOfUpdates == 1
                        ? Messages.HOTFIX_TYPE_NEW_FEATURE_ONE
                        : string.Format(Messages.HOTFIX_TYPE_NEW_FEATURE_MANY, numberOfUpdates);
                case CdnUpdateType.PreviewFeature:
                    return numberOfUpdates == 1
                        ? Messages.HOTFIX_TYPE_PREVIEW_FEATURE_ONE
                        : string.Format(Messages.HOTFIX_TYPE_PREVIEW_FEATURE_MANY, numberOfUpdates);
                case CdnUpdateType.Improvement:
                    return numberOfUpdates == 1
                        ? Messages.HOTFIX_TYPE_IMPROVEMENT_ONE
                        : string.Format(Messages.HOTFIX_TYPE_IMPROVEMENT_MANY, numberOfUpdates);
                case CdnUpdateType.Foundational:
                    return numberOfUpdates == 1
                        ? Messages.HOTFIX_TYPE_IMPROVEMENT_ONE
                        : string.Format(Messages.HOTFIX_TYPE_IMPROVEMENT_MANY, numberOfUpdates);
                default:
                    return Messages.UNKNOWN;
            }
        }

        internal static Image GetImageOf(this CdnUpdateType updateType)
        {
            switch (updateType)
            {
                case CdnUpdateType.Bugfix:
                    return CdnStaticImages.BugFix;
                case CdnUpdateType.SecurityFix:
                    return CdnStaticImages.SecurityFix;
                case CdnUpdateType.NewFeature:
                    return CdnStaticImages.NewFeature;
                case CdnUpdateType.PreviewFeature:
                    return CdnStaticImages.PreviewFeature;
                case CdnUpdateType.Improvement:
                    return CdnStaticImages.Improvement;
                case CdnUpdateType.Foundational:
                    return CdnStaticImages.Foundational;
                default:
                    return CdnStaticImages.Unknown;

            }
        }
    }

    internal static class CdnStaticImages
    {
        public static readonly Image SecurityFix = GetBitmap(CdnUpdateType.SecurityFix);
        public static readonly Image BugFix = GetBitmap(CdnUpdateType.Bugfix);
        public static readonly Image Improvement = GetBitmap(CdnUpdateType.Improvement);
        public static readonly Image NewFeature = GetBitmap(CdnUpdateType.NewFeature);
        public static readonly Image PreviewFeature = GetBitmap(CdnUpdateType.PreviewFeature);
        public static readonly Image Foundational = GetBitmap(CdnUpdateType.Foundational);
        public static readonly Image Unknown = Images.StaticImages.alert6_16;

        private static Bitmap GetBitmap(CdnUpdateType updateType)
        {
            string letter = updateType.InitialOf();
            Color color = updateType.ColorOf();

            var bmp = new Bitmap(16, 16);
            var rect = new Rectangle(0, 0, 15, 15);

            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var brush = new SolidBrush(color))
                    g.FillEllipse(brush, rect);

                using (var pen = new Pen(ControlPaint.Dark(color), 1))
                    g.DrawEllipse(pen, rect);

                using (var brush = new SolidBrush(Color.WhiteSmoke))
                using (var font = new Font(FontFamily.GenericSansSerif, 7.5F, FontStyle.Bold))
                using (var format = new StringFormat())
                {
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    rect.Inflate(1, 1);
                    g.DrawString(letter, font, brush, rect, format);
                }
            }

            return bmp;
        }
    }
}
