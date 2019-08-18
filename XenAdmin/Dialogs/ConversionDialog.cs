﻿/* Copyright (c) Citrix Systems Inc. 
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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.XCM;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Wizards.ConversionWizard;
using XenAPI;
using XenCenterLib;


namespace XenAdmin.Dialogs
{
    public partial class ConversionDialog : XenDialogBase
    {
        #region Private fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int HEARTBEAT = 10; //seconds
        private ConversionClient _conversionClient;
        private Conversion[] _currentConversionList = { };
        private readonly object _conversionLock = new object();
        private volatile bool _updating;
        private volatile bool _updateRequired;
        private VM _conversionVm;

        private static readonly string[] DetailHeaders =
        {
            Messages.CONVERSION_DETAIL_CONVERSION_ID,
            Messages.CONVERSION_DETAIL_TARGET_SR,
            Messages.CONVERSION_DETAIL_NETWORK_READ,
            Messages.CONVERSION_DETAIL_DISK_WRITE,
            Messages.CONVERSION_DETAIL_START_TIME,
            Messages.CONVERSION_DETAIL_FINISH_TIME,
            Messages.CONVERSION_DETAIL_DURATION,
            Messages.CONVERSION_DETAIL_STATUS,
            Messages.CONVERSION_DETAIL_ERROR
        };

        #endregion

        public ConversionDialog(IXenConnection conn)
            : base(conn)
        {
            InitializeComponent();

            toolStripSplitButtonRefresh.DefaultItem = toolStripMenuItemRefreshAll;
            toolStripSplitButtonRefresh.Text = toolStripMenuItemRefreshAll.Text;
            toolStripSplitButtonLogs.DefaultItem = menuItemFetchAllLogs;
            toolStripSplitButtonLogs.Text = menuItemFetchAllLogs.Text;

            statusLabel.Image = null;
            statusLabel.Text = string.Empty;
            statusLinkLabel.Reset();
            SetFilterLabel();
            UpdateButtons();
        }

        private Conversion[] CurrentConversionList
        {
            get
            {
                lock (_conversionLock)
                    return _currentConversionList;
            }
            set
            {
                lock (_conversionLock)
                    _currentConversionList = value ?? new Conversion[] { };
            }
        }

        private bool FilterIsOn => toolStripDdbFilterStatus.FilterIsOn;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ConnectToVpx();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timerVpx.Stop();
            if (_conversionVm != null)
                _conversionVm.PropertyChanged -= _conversionVm_PropertyChanged;
            base.OnFormClosing(e);
        }

        private void ConnectToVpx()
        {
            statusLabel.Image = Images.StaticImages.ajax_loader;
            statusLabel.Text = Messages.CONVERSION_INITIALIZING_VPX;
            statusLinkLabel.Reset();

            ThreadPool.QueueUserWorkItem(obj =>
            {
                var master = Helpers.GetMaster(connection);

                string serviceIp = null;
                try
                {
                    serviceIp = Host.call_plugin(connection.Session, master.opaque_ref, "conversion", "main", null);
                }
                catch (Exception e)
                {
                    log.Error("Cannot communicate with the conversion plugin.", e);
                }

                Program.Invoke(this, () =>
                {
                    if (string.IsNullOrEmpty(serviceIp))
                    {
                        statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                        statusLabel.Text = Messages.CONVERSION_INITIALIZING_VPX_FAILURE;
                        statusLinkLabel.Reset(Messages.CONVERSION_TRY_AGAIN, ConnectToVpx);
                        return;
                    }

                    var useSsl = Properties.Settings.Default.ConversionClientUseSsl;
                    _conversionClient = new ConversionClient(connection, serviceIp, useSsl);
                    RegisterVM(serviceIp);
                    CheckVersionCompatibility();
                });
            });
        }

        private void RegisterVM(string ipAddress)
        {
            // if we're reconnecting the conversion VM, we need to clear the old one
            if (_conversionVm != null)
            {
                _conversionVm.PropertyChanged -= _conversionVm_PropertyChanged;
                _conversionVm = null;
            }

            var vifs = connection.Cache.VIFs;

            foreach (var vif in vifs)
            {
                if (!vif.IPAddresses().Contains(ipAddress))
                    continue;

                var vm = connection.Resolve(vif.VM);
                if (!vm.IsConversionVM())
                    continue;

                _conversionVm = vm;
                _conversionVm.PropertyChanged += _conversionVm_PropertyChanged;
                break;
            }
        }

        private void _conversionVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "power_state" || _conversionVm == null)
                return;

            if (_conversionVm.power_state != vm_power_state.Running)
            {
                timerVpx.Stop();
                CurrentConversionList = null;
                Program.Invoke(this, BuildConversionList);
                ConnectToVpx();
            }
        }

        private void CheckVersionCompatibility()
        {
            statusLabel.Text = Messages.CONVERSION_VERSION_CHECK;

            ThreadPool.QueueUserWorkItem(obj =>
            {
                string version = null;
                try
                {
                    version = _conversionClient.GetVpxVersion();
                }
                catch (Exception e)
                {
                    log.Error("Cannot retrieve XCM VPX version.", e);

                    Program.Invoke(this, () =>
                    {
                        statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                        statusLabel.Text = Messages.CONVERSION_VERSION_CHECK_FAILURE;
                        statusLinkLabel.Reset();
                    });

                    return;
                }

                Program.Invoke(this, () =>
                {
                    if (!Version.TryParse(version, out _) ||
                        ConversionClient.SupportedVersions.All(v => ConversionClient.CompareVersions(v, version) != 0))
                    {
                        statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                        statusLabel.Text = Messages.CONVERSION_VERSION_INCOMPATIBILITY;
                        statusLinkLabel.Reset(Messages.MORE_INFO, () =>
                        {
                            using (var dlog = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(null, Messages.CONVERSION_VERSION_INCOMPATIBILITY_INFO)))
                            {
                                dlog.ShowDialog(this);
                            }
                        });
                        return;
                    }

                    statusLabel.Image = Images.StaticImages.ConversionManager;
                    statusLabel.Text = Messages.CONVERSION_CONNECTING_VPX_SUCCESS;
                    statusLinkLabel.Reset();

                    timerVpx.Start();
                    FetchConversionHistory();
                });
            });
        }

        private void FetchConversionHistory()
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                try
                {
                    CurrentConversionList = _conversionClient.GetConversionHistory();
                }
                catch (Exception e)
                {
                    log.Error("Cannot fetch conversion history.", e);
                    CurrentConversionList = null;

                    Program.Invoke(this, () =>
                    {
                        statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                        statusLabel.Text = string.Format(Messages.CONVERSION_CONNECTING_VPX_INTERRUPTION, HEARTBEAT);
                        statusLinkLabel.Reset();
                    });
                }

                if (_updating)
                {
                    _updateRequired = true;
                    return;
                }

                Program.Invoke(this, BuildConversionList);
            });
        }

        private void FetchConversionDetails(Conversion conversion)
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                Conversion refreshedConversion;
                try
                {
                    refreshedConversion = _conversionClient.GetConversionDetails(conversion);
                }
                catch (Exception e)
                {
                    log.Error($"Cannot fetch details for conversion {conversion.Id}", e);

                    Program.Invoke(this, () =>
                    {
                        statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                        statusLabel.Text = Messages.CONVERSION_DETAIL_GET_FAILURE;
                        statusLinkLabel.Reset();
                    });
                    return;
                }

                if (_updating)
                {
                    _updateRequired = true;
                    return;
                }

                Program.Invoke(this, () =>
                {
                    UpdateConversionRow(refreshedConversion);
                    UpdateButtons();
                });
            });
        }

        private void CreateConversion(ConversionConfig config)
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                Conversion conv;
                try
                {
                    conv = _conversionClient.CreateConversion(config);
                }
                catch (Exception e)
                {
                    log.Error("Failed to create new conversion", e);

                    Program.Invoke(this, () =>
                    {
                        statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                        statusLabel.Text = Messages.CONVERSION_CREATE_FAILURE;
                        statusLinkLabel.Reset();
                    });
                    return;
                }

                FetchConversionDetails(conv);
            });
        }


        private void BuildConversionList()
        {
            try
            {
                _updating = true;
                dataGridViewConversions.SuspendLayout();

                var selectedConversionId = dataGridViewConversions.SelectedRows.Count == 1 && dataGridViewConversions.SelectedRows[0] is ConversionRow r
                    ? r.Conversion.Id
                    : null;

                dataGridViewConversions.Rows.Clear();
                var rows = CurrentConversionList.Select(c => new ConversionRow(c)).ToList();

                if (dataGridViewConversions.SortedColumn != null)
                {
                    rows.Sort((r1, r2) => CompareConversionRows(dataGridViewConversions.SortedColumn.Index, r1, r2));

                    if (dataGridViewConversions.SortOrder == SortOrder.Descending)
                        rows.Reverse();
                }

                dataGridViewConversions.Rows.AddRange(rows.Cast<DataGridViewRow>().ToArray());

                foreach (ConversionRow row in dataGridViewConversions.Rows)
                {
                    if (row.Conversion.Id == selectedConversionId)
                    {
                        row.Selected = true;
                        break;
                    }
                }

                if (dataGridViewConversions.SelectedRows.Count == 0 && dataGridViewConversions.Rows.Count > 0)
                    dataGridViewConversions.Rows[0].Selected = true;
            }
            finally
            {
                dataGridViewConversions.ResumeLayout();
                _updating = false;

                if (_updateRequired)
                {
                    _updateRequired = false;
                    BuildConversionList();
                }

                UpdateButtons();
            }
        }

        private void UpdateConversionRow(Conversion conversion)
        {
            foreach (ConversionRow row in dataGridViewConversions.Rows)
            {
                if (row.Conversion.Id == conversion.Id)
                {
                    if (toolStripDdbFilterStatus.HideByStatus(conversion))
                    {
                        dataGridViewConversions.Rows.Remove(row);
                        return;
                    }

                    row.RefreshRow(conversion);
                    if (row.Selected)
                        BuildDetailsView(row.Conversion);
                }
            }
        }

        private void BuildDetailsView(Conversion conversion)
        {
            try
            {
                dataGridViewDetails.SuspendLayout();

                var details = GetDetailValues(conversion);

                for (int i = 0; i < DetailHeaders.Length; i++)
                        AddOrUpdateDetailRow($"{DetailHeaders[i]}:", details[i]);
            }
            finally
            {
                dataGridViewDetails.ResumeLayout();
            }
        }

        private void AddOrUpdateDetailRow(string key, string val)
        {
            foreach (DataGridViewRow row in dataGridViewDetails.Rows)
            {
                if (row.Cells.Count > 1 && row.Cells[0].Value as string == key)
                {
                    row.Cells[1].Value = val;
                    return;
                }
            }

            dataGridViewDetails.Rows.Add(new ConversionDetailRow(key, val));
        }

        private void ClearDetailsView()
        {
            try
            {
                dataGridViewDetails.SuspendLayout();
                dataGridViewDetails.Rows.Clear();
            }
            finally
            {
                dataGridViewDetails.ResumeLayout();
            }
        }

        private string[] GetDetailValues(Conversion conversion)
        {
            var startTime = conversion.StartTime.ToLocalTime();
            var finishTime = conversion.CompletedTime >= conversion.StartTime ? conversion.CompletedTime.ToLocalTime() : DateTime.Now;

            var startTimeString = Messages.HYPHEN;
            var finishTimeString = Messages.HYPHEN;

            Program.Invoke(this, () =>
            {
                startTimeString = HelpersGUI.DateTimeToString(startTime, Messages.DATEFORMAT_DMY_HM, true);

                if (conversion.CompletedTime >= conversion.StartTime)
                    finishTimeString = HelpersGUI.DateTimeToString(finishTime, Messages.DATEFORMAT_DMY_HM, true);
            });

            return new[]
            {
                conversion.Id,
                conversion.SRName,
                Util.DiskSizeString(conversion.CompressedBytesRead),
                Util.DiskSizeString(conversion.UncompressedBytesWritten),
                startTimeString,
                finishTimeString,
                (finishTime - startTime).ToString(@"h\:mm\:ss"),
                conversion.StatusDetail,
                string.IsNullOrWhiteSpace(conversion.Error) ? Messages.HYPHEN: conversion.Error
            };
        }

        private void FetchLogs(Conversion? conversion = null)
        {
            string fileName;
            using (SaveFileDialog dialog = new SaveFileDialog
            {
                AddExtension = true,
                Filter = string.Format("{0} (*.txt)|*.txt|{1} (*.*)|*.*", Messages.TXT_DESCRIPTION, Messages.ALL_FILES),
                FilterIndex = 0,
                Title = Messages.CONVERSION_LOG_SAVE_TITLE,
                RestoreDirectory = true,
                DefaultExt = "txt",
                CheckPathExists = false,
                OverwritePrompt = true
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                fileName = dialog.FileName;
            }

            new DelegatedAsyncAction(null,
                string.Format(Messages.CONVERSION_LOG_SAVE, fileName),
                string.Format(Messages.CONVERSION_LOG_SAVING, fileName),
                string.Format(Messages.CONVERSION_LOG_SAVED, fileName),
                obj =>
                {
                    var logString = conversion.HasValue
                        ? _conversionClient.GetConversionLog(conversion.Value)
                        : _conversionClient.GetVpxLogs();

                    using (StreamWriter stream = new StreamWriter(fileName, false, Encoding.UTF8))
                        stream.Write(logString);
                }).RunAsync();
        }

        private void UpdateButtons()
        {
            toolStripButtonNew.Enabled = _conversionClient != null;

            ConversionRow oneRow = null;
            if (dataGridViewConversions.SelectedRows.Count == 1)
                oneRow = dataGridViewConversions.SelectedRows[0] as ConversionRow;

            toolStripButtonCancel.Enabled = oneRow != null && oneRow.Conversion.CanCancel;
            toolStripButtonRetry.Enabled = oneRow != null && oneRow.Conversion.CanRetry;

            toolStripMenuItemRefreshSelected.Enabled = oneRow != null;
            toolStripMenuItemRefreshAll.Enabled = dataGridViewConversions.Rows.Count > 0;
            toolStripSplitButtonRefresh.Enabled = toolStripMenuItemRefreshAll.Enabled || toolStripMenuItemRefreshSelected.Enabled;

            menuItemFetchSelectedLog.Enabled = oneRow != null;
            menuItemFetchAllLogs.Enabled = _conversionClient != null;
            toolStripSplitButtonLogs.Enabled = menuItemFetchSelectedLog.Enabled || menuItemFetchAllLogs.Enabled;

            toolStripButtonClear.Enabled = toolStripButtonExport.Enabled = dataGridViewConversions.Rows.Count > 0;
        }

        private void SetFilterLabel()
        {
            toolStripLabelFiltersOnOff.Text = FilterIsOn ? Messages.FILTERS_ON : Messages.FILTERS_OFF;
        }

        private int CompareConversionRows(int sortingColumnIndex, ConversionRow row1, ConversionRow row2)
        {
            var conv1 = row1.Conversion;
            var conv2 = row2.Conversion;

            if (sortingColumnIndex == ColumnVm.Index)
                return Conversion.CompareOnVm(conv1, conv2);

            if (sortingColumnIndex == ColumnSourceServer.Index)
                return Conversion.CompareOnServer(conv1, conv2);

            if (sortingColumnIndex == ColumnStartTime.Index)
                return Conversion.CompareOnStartTime(conv1, conv2);

            if (sortingColumnIndex == ColumnFinishTime.Index)
                return Conversion.CompareOnCompletedTime(conv1, conv2);

            if (sortingColumnIndex == ColumnStatus.Index)
                return Conversion.CompareOnStatus(conv1, conv2);

            return Conversion.CompareOnId(conv1, conv2);
        }


        #region Event handlers

        private void timerVpx_Tick(object sender, EventArgs e)
        {
            //the timer ticks every second, but a request is sent only every as many seconds as specified by the HEARTBEAT
            if (DateTime.Now.Second % HEARTBEAT == 0)
                FetchConversionHistory();

            if (dataGridViewConversions.SelectedRows.Count == 1 && dataGridViewConversions.SelectedRows[0] is ConversionRow row && !row.Conversion.IsCompleted)
                BuildDetailsView(row.Conversion);
        }

        private void dataGridViewConversions_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();

            if (dataGridViewConversions.SelectedRows.Count == 1 && dataGridViewConversions.SelectedRows[0] is ConversionRow row)
                BuildDetailsView(row.Conversion);
            else
                ClearDetailsView();
        }

        private void dataGridViewConversions_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            var row1 = (ConversionRow)dataGridViewConversions.Rows[e.RowIndex1];
            var row2 = (ConversionRow)dataGridViewConversions.Rows[e.RowIndex2];
            e.SortResult = CompareConversionRows(e.Column.Index, row1, row2);
            e.Handled = true;
        }

        private void ConversionWizard_FormClosed(object sender, FormClosedEventArgs e)
        {
            var wizard = sender as ConversionWizard;
            if (wizard == null)
                return;

            wizard.FormClosed -= ConversionWizard_FormClosed;

            if (wizard.ConversionConfigs == null)
                return;

            foreach (var config in wizard.ConversionConfigs)
                CreateConversion(config);
        }

        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            var wizard = new ConversionWizard(_conversionClient);
            wizard.FormClosed += ConversionWizard_FormClosed;
            Program.MainWindow.ShowPerConnectionWizard(connection, wizard, this);
        }

        private void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            if (dataGridViewConversions.SelectedRows.Count == 1 && dataGridViewConversions.SelectedRows[0] is ConversionRow row)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.CONVERSION_CANCEL_CONFIRM),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
                {
                    if (dlog.ShowDialog() == DialogResult.No)
                        return;
                }

                ThreadPool.QueueUserWorkItem(obj =>
                {
                    try
                    {
                        _conversionClient.CancelConversion(row.Conversion);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Cannot cancel conversion {row.Conversion.Id}.", ex);

                        Program.Invoke(this, () =>
                        {
                            statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                            statusLabel.Text = Messages.CONVERSION_CANCEL_FAILURE;
                            statusLinkLabel.Reset();
                        });
                    }
                });
            }
        }

        private void toolStripButtonRetry_Click(object sender, EventArgs e)
        {
            if (dataGridViewConversions.SelectedRows.Count == 1 && dataGridViewConversions.SelectedRows[0] is ConversionRow row)
            {
                ThreadPool.QueueUserWorkItem(obj =>
                {
                    try
                    {
                        _conversionClient.RetryConversion(row.Conversion);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Cannot retry conversion {row.Conversion.Id}.", ex);

                        Program.Invoke(this, () =>
                        {
                            statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                            statusLabel.Text = Messages.CONVERSION_RETRY_FAILURE;
                            statusLinkLabel.Reset();
                        });
                    }
                });
            }
        }

        private void toolStripButtonClear_Click(object sender, EventArgs e)
        {
            using (var dlog = new ThreeButtonDialog(
                new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.CONVERSION_CLEAR_HISTORY_CONFIRM),
                ThreeButtonDialog.ButtonYes,
                ThreeButtonDialog.ButtonNo))
            {
                if (dlog.ShowDialog() == DialogResult.No)
                    return;
            }

            toolStripButtonClear.Enabled = false;

            ThreadPool.QueueUserWorkItem(obj =>
            {
                try
                {
                    _conversionClient.ClearConversionHistory();
                }
                catch (Exception ex)
                {
                    log.Error("Cannot clear conversion history.", ex);

                    Program.Invoke(this, () =>
                    {
                        statusLabel.Image = Images.StaticImages._000_error_h32bit_16;
                        statusLabel.Text = Messages.CONVERSION_CLEAR_HISTORY_FAILURE;
                        statusLinkLabel.Reset();
                        toolStripButtonClear.Enabled = true;
                    });
                }
            });
        }

        private void toolStripDdbFilterStatus_FilterChanged()
        {
            SetFilterLabel();
            BuildConversionList();
        }

        private void toolStripButtonExport_Click(object sender, EventArgs e)
        {
            bool exportAll = true;

            if (FilterIsOn)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.CONVERSION_EXPORT_ALL_OR_FILTERED),
                    new ThreeButtonDialog.TBDButton(Messages.EXPORT_ALL_BUTTON, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.EXPORT_FILTERED_BUTTON, DialogResult.No, ThreeButtonDialog.ButtonType.NONE),
                    ThreeButtonDialog.ButtonCancel))
                {
                    var result = dlog.ShowDialog(this);

                    if (result == DialogResult.No)
                        exportAll = false;
                    else if (result == DialogResult.Cancel)
                        return;
                }
            }

            string fileName;
            using (SaveFileDialog dialog = new SaveFileDialog
            {
                AddExtension = true,
                Filter = string.Format("{0} (*.csv)|*.csv|{1} (*.txt)|*.txt|{2} (*.*)|*.*",
                    Messages.CSV_DESCRIPTION, Messages.TXT_DESCRIPTION, Messages.ALL_FILES),
                FilterIndex = 0,
                Title = Messages.EXPORT_ALL,
                RestoreDirectory = true,
                DefaultExt = "csv",
                CheckPathExists = false,
                OverwritePrompt = true
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                fileName = dialog.FileName;
            }

            new DelegatedAsyncAction(null,
                string.Format(Messages.CONVERSION_EXPORT, fileName),
                string.Format(Messages.CONVERSION_EXPORTING, fileName),
                string.Format(Messages.CONVERSION_EXPORTED, fileName),
                s =>
                {
                    using (StreamWriter stream = new StreamWriter(fileName, false, Encoding.UTF8))
                    {
                        stream.WriteLine(string.Join(",", DetailHeaders.Select(v => $"\"{v}\"")));

                        if (exportAll)
                        {
                            var exportable = new List<Conversion>(CurrentConversionList);
                            foreach (var conv in exportable)
                                stream.WriteLine(string.Join(",", GetDetailValues(conv).Select(v => $"\"{v}\"")));
                        }
                        else
                        {
                            foreach (ConversionRow row in dataGridViewConversions.Rows)
                            {
                                if (row != null)
                                    stream.WriteLine(string.Join(",", GetDetailValues(row.Conversion).Select(v => $"\"{v}\"")));
                            }
                        }
                    }
                }).RunAsync();
        }

        private void toolStripSplitButtonRefresh_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripSplitButtonRefresh.DefaultItem = e.ClickedItem;
            toolStripSplitButtonRefresh.Text = toolStripSplitButtonRefresh.DefaultItem.Text;
        }

        private void toolStripMenuItemRefreshSelected_Click(object sender, EventArgs e)
        {
            if (dataGridViewConversions.SelectedRows.Count == 1 && dataGridViewConversions.SelectedRows[0] is ConversionRow row)
                FetchConversionDetails(row.Conversion);
        }

        private void toolStripMenuItemRefreshAll_Click(object sender, EventArgs e)
        {
            FetchConversionHistory();
        }

        private void toolStripSplitButtonLogs_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripSplitButtonLogs.DefaultItem = e.ClickedItem;
            toolStripSplitButtonLogs.Text = toolStripSplitButtonLogs.DefaultItem.Text;
        }

        private void menuItemFetchSelectedLog_Click(object sender, EventArgs e)
        {
            if (dataGridViewConversions.SelectedRows.Count == 1 && dataGridViewConversions.SelectedRows[0] is ConversionRow row)
                FetchLogs(row.Conversion);
        }

        private void menuItemFetchAllLogs_Click(object sender, EventArgs e)
        {
            FetchLogs();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion


        #region Nested items

        private class ConversionRow : DataGridViewRow
        {
            private readonly DataGridViewTextBoxCell cellSourceVm = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellsourceServer = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellStartTime = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellFinishTime = new DataGridViewTextBoxCell();
            private readonly DataGridViewImageCell cellStatus = new DataGridViewImageCell();

            public Conversion Conversion { get; private set; }

            public ConversionRow(Conversion conversion)
            {
                Cells.AddRange(cellSourceVm, cellsourceServer, cellStartTime, cellFinishTime, cellStatus);
                RefreshRow(conversion);
            }

            public void RefreshRow(Conversion conversion)
            {
                Conversion = conversion;
                cellSourceVm.Value = conversion.Configuration.SourceVmName;
                cellsourceServer.Value = conversion.Configuration.SourceServer.Hostname;

                cellStartTime.Value = HelpersGUI.DateTimeToString(conversion.StartTime.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true);

                cellFinishTime.Value = conversion.CompletedTime >= conversion.StartTime
                    ? HelpersGUI.DateTimeToString(conversion.CompletedTime.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true)
                    : Messages.HYPHEN;

                if (conversion.Status == (int)ConversionStatus.Completed)
                    cellStatus.Value = Images.StaticImages._000_Tick_h32bit_16;
                else if (conversion.Status == (int)ConversionStatus.Aborted)
                    cellStatus.Value = Images.StaticImages._000_error_h32bit_16;
                else if (conversion.Status == (int)ConversionStatus.UserAborted)
                    cellStatus.Value = Images.StaticImages.cancelled_action_16;
                else if (conversion.Status == (int)ConversionStatus.Incomplete)
                    cellStatus.Value = Images.StaticImages._075_WarningRound_h32bit_16;
                else if (conversion.Status == (int)ConversionStatus.Running)
                    cellStatus.Value = Images.GetImageForPercentage(conversion.PercentComplete);
                else
                    cellStatus.Value = Images.GetImageForPercentage(0);
            }
        }

        private class ConversionDetailRow : DataGridViewRow
        {
            private readonly DataGridViewTextBoxCell cellKey = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellValue = new DataGridViewTextBoxCell();

            private readonly Font boldFont = new Font(Program.DefaultFont, FontStyle.Bold);

            public ConversionDetailRow(string key, string val)
            {
                cellKey.Style.Font = boldFont;
                Cells.AddRange(cellKey, cellValue);
                cellKey.Value = key;
                cellValue.Value = val;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    boldFont.Dispose();

                base.Dispose(disposing);
            }
        }

        private class ActionableLinkLabel : ToolStripStatusLabel
        {
            private Action _linkAction;

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                _linkAction?.Invoke();
            }

            public void Reset(string text = "", Action linkAction = null)
            {
                Text = text;
                _linkAction = linkAction;
            }
        }

        #endregion
    }
}
