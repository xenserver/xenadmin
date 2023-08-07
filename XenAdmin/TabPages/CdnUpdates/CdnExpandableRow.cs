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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.TabPages.CdnUpdates
{
    internal abstract class CdnExpandableRow : DataGridViewRow
    {
        private readonly CdnExpandableTextAndImageCell _nameCell = new CdnExpandableTextAndImageCell();
        private readonly DataGridViewTextBoxCell _lastSyncCell = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell _lastUpdateCell = new DataGridViewTextBoxCell();

        protected CdnExpandableRow()
        {
            Cells.AddRange(_nameCell, _lastSyncCell, _lastUpdateCell);
            MinimumHeight = 22;
        }

        protected void SetValues(string name, Image image, string lastSync = null, string lastUpdate = null)
        {
            _nameCell.Value = name;
            _nameCell.Image = image;

            if (lastSync != null)
                _lastSyncCell.Value = lastSync;

            if (lastUpdate != null)
                _lastUpdateCell.Value = lastUpdate;
        }

        public virtual List<CdnExpandableRow> ChildRows => new List<CdnExpandableRow>();

        public CdnExpandableRow ParentRow { get; set; }

        public int Level;

        public bool IsExpanded { get; set; }

        public List<string> Export()
        {
            var details = new List<string>
            {
                _nameCell.Value as string,
                string.Empty
            };
            details.AddRange(ChildRows.SelectMany(r => r.Export()));
            return details;
        }
    }


    internal class PoolUpdateInfoRow : CdnExpandableRow
    {
        public PoolUpdateInfoRow(IXenConnection connection, CdnPoolUpdateInfo poolUpdateInfo)
        {
            Pool = Helpers.GetPoolOfOne(connection);

            var lastSyncTime = Messages.INDETERMINABLE;

            if (Helpers.XapiEqualOrGreater_23_18_0(connection))
            {
                lastSyncTime = Messages.NEVER;

                if (Pool.last_update_sync > Util.GetUnixMinDateTime())
                {
                    lastSyncTime = HelpersGUI.DateTimeToString(Pool.last_update_sync.ToLocalTime(), Messages.DATEFORMAT_DMY_HMS, true);
                }
            }

            SetValues(Helpers.GetName(connection), Images.GetImage16For(Images.GetIconFor(connection)), lastSyncTime);

            if (poolUpdateInfo == null)
            {
                ChildRows = connection.Cache.Hosts
                    .Select(h => new HostUpdateInfoRow(connection, h, null, null))
                    .Cast<CdnExpandableRow>().ToList();
            }
            else
            {
                ChildRows = poolUpdateInfo.HostsWithUpdates
                    .Select(h => new HostUpdateInfoRow(connection, connection.Resolve(new XenRef<Host>(h.HostOpaqueRef)), poolUpdateInfo, h))
                    .Cast<CdnExpandableRow>().ToList();
            }
        }

        public Pool Pool { get; }

        public override List<CdnExpandableRow> ChildRows { get; } = new List<CdnExpandableRow>();
    }


    internal class HostUpdateInfoRow : CdnExpandableRow
    {
        private readonly List<CdnExpandableRow> _childRows = new List<CdnExpandableRow>();

        public HostUpdateInfoRow(IXenConnection connection, Host host, CdnPoolUpdateInfo poolUpdateInfo, CdnHostUpdateInfo hostUpdateInfo)
        {
            Connection = connection;
            Host = host;

            string lastSyncTime = null;
            string lastUpdateTime = Messages.NEVER;

            if (Helpers.GetPool(Connection) == null) //standalone host
            {
                lastSyncTime = Messages.INDETERMINABLE;

                if (Helpers.XapiEqualOrGreater_23_18_0(Connection))
                {
                    lastSyncTime = Messages.NEVER;

                    var pool = Helpers.GetPoolOfOne(Connection);
                    
                    if (pool != null && pool.last_update_sync > Util.GetUnixMinDateTime())
                    {
                        lastSyncTime = HelpersGUI.DateTimeToString(pool.last_update_sync.ToLocalTime(), Messages.DATEFORMAT_DMY_HMS, true);
                    }
                }
            }

            if (Helpers.XapiEqualOrGreater_22_20_0(Host))
            {
                var unixMinDateTime = Util.GetUnixMinDateTime();
                var softwareVersionDate = unixMinDateTime;

                if (Host.software_version.ContainsKey("date"))
                {
                    if (!Util.TryParseIso8601DateTime(Host.software_version["date"], out softwareVersionDate))
                        Util.TryParseNonIso8601DateTime(Host.software_version["date"], out softwareVersionDate);
                }

                if (Host.last_software_update > softwareVersionDate && Host.last_software_update > unixMinDateTime)
                {
                    lastUpdateTime = HelpersGUI.DateTimeToString(Host.last_software_update.ToLocalTime(), Messages.DATEFORMAT_DMY_HMS, true);
                }
            }

            SetValues(Host.Name(), Images.GetImage16For(Images.GetIconFor(Host)), lastSyncTime, lastUpdateTime);

            if (poolUpdateInfo != null && hostUpdateInfo != null)
            {
                if (hostUpdateInfo.RecommendedGuidance.Length > 0)
                {
                    _childRows.Add(new PostUpdateActionRow(hostUpdateInfo.RecommendedGuidance));

                    if (hostUpdateInfo.LivePatches.Length > 0 && !hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.RebootHost))
                        _childRows.Add(new LivePatchActionRow());
                }

                var categories = hostUpdateInfo.GetUpdateCategories(poolUpdateInfo);

                _childRows.AddRange(categories.Select(c => new UpdateCategoryRow(c.Item1, c.Item2) as CdnExpandableRow));

                if (hostUpdateInfo.Rpms.Length > 0)
                    _childRows.Add(new RpmCategoryRow(hostUpdateInfo.Rpms));
            }
        }

        public IXenConnection Connection { get; }

        public Host Host { get; }

        public override List<CdnExpandableRow> ChildRows => _childRows;
    }


    internal class UpdateCategoryRow : CdnExpandableRow
    {
        public UpdateCategoryRow(CdnUpdateType updateType, List<CdnUpdate> updates)
        {
            SetValues(updateType.GetCategoryTitle(updates.Count), updateType.GetImageOf());
            ChildRows = updates.Select(u => new UpdateRow(u)).Cast<CdnExpandableRow>().ToList();
        }

        public override List<CdnExpandableRow> ChildRows { get; } = new List<CdnExpandableRow>();
    }


    internal class RpmCategoryRow : CdnExpandableRow
    {
        public RpmCategoryRow(params string[] rpms)
        {
            SetValues(string.Format(Messages.HOTFIX_RPMS_TO_INSTALL, rpms.Length),
                Images.StaticImages._000_Patch_h32bit_16);

            ChildRows = new List<CdnExpandableRow> { new RpmsRow(rpms) };
        }

        public override List<CdnExpandableRow> ChildRows { get; } = new List<CdnExpandableRow>();
    }


    internal class UpdateRow : CdnExpandableRow
    {
        public UpdateRow(CdnUpdate update)
        {
            SetValues(update.Summary, null);

            var details = update.CollateDetails();

            if (!string.IsNullOrEmpty(details))
                ChildRows = new List<CdnExpandableRow> { new UpdateDetailRow(details) };
        }

        public override List<CdnExpandableRow> ChildRows { get; } = new List<CdnExpandableRow>();
    }


    internal class UpdateDetailRow : CdnExpandableRow
    {
        public UpdateDetailRow(string detail)
        {
            SetValues(detail, null);
        }
    }

    internal class PostUpdateActionRow : CdnExpandableRow
    {
        public PostUpdateActionRow(CdnGuidance[] guidance)
        {
            var text = string.Format(Messages.HOTFIX_POST_UPDATE_ACTIONS, string.Join(Environment.NewLine, guidance.Select(Cdn.FriendlyInstruction)));
            SetValues(text, Images.StaticImages.rightArrowLong_Blue_16);
        }
    }

    internal class LivePatchActionRow : CdnExpandableRow
    {
        public LivePatchActionRow()
        {
            SetValues(Messages.HOTFIX_POST_UPDATE_LIVEPATCH_ACTIONS, Images.StaticImages.livepatch_16);
        }
    }


    internal class RpmsRow : CdnExpandableRow
    {
        public RpmsRow(string[] rpms)
        {
            SetValues(string.Join(Environment.NewLine, rpms), null);
        }
    }
}
