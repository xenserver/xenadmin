﻿/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Controls.CustomDataGraph
{
    [Flags]
    public enum ArchiveInterval
    {
        None = 0,
        FiveSecond = 1,
        OneMinute = 2,
        OneHour = 4,
        OneDay = 8
    }

    public class ArchiveMaintainer
    {
        private const long TICKS_IN_ONE_SECOND = 10000000;
        private const long TICKS_IN_FIVE_SECONDS = 50000000;
        private const long TICKS_IN_ONE_MINUTE = 600000000;
        internal const long TICKS_IN_TEN_MINUTES = 6000000000;
        private const long TICKS_IN_ONE_HOUR = 36000000000;
        internal const long TICKS_IN_TWO_HOURS = 72000000000;
        private const long TICKS_IN_ONE_DAY = 864000000000;
        internal const long TICKS_IN_SEVEN_DAYS = 6048000000000;
        internal const long TICKS_IN_ONE_YEAR = 316224000000000;

        private const int FIVE_SECONDS_IN_TEN_MINUTES = 120;
        private const int MINUTES_IN_TWO_HOURS = 120;
        private const int HOURS_IN_ONE_WEEK = 168;
        private const int DAYS_IN_ONE_YEAR = 366;

        private static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan OneHour = TimeSpan.FromHours(1);
        private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        private const int SLEEP_TIME = 5000;

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        internal event Action ArchivesUpdated;

        internal readonly Dictionary<ArchiveInterval, DataArchive> Archives =
            new Dictionary<ArchiveInterval, DataArchive>();

        private CancellationTokenSource _cancellationTokenSource;
        private List<DataSet> _setsAdded;
        private List<Data_source> _dataSources = new List<Data_source>();

        private long _endTime;
        private bool _bailOut;
        private long _currentInterval;
        private long _stepSize;
        private long _currentTime;
        private int _valueCount;
        private string _lastNode = "";

        public IXenObject XenObject { get; }

        public DateTime LastFiveSecondCollection = DateTime.MinValue;
        public DateTime LastOneMinuteCollection = DateTime.MinValue;
        public DateTime LastOneHourCollection = DateTime.MinValue;
        public DateTime LastOneDayCollection = DateTime.MinValue;

        public bool FirstTime = true;
        public bool LoadingInitialData;

        private DateTime ServerNow => DateTime.UtcNow.Subtract(ClientServerOffset);

        public DateTime GraphNow => DateTime.Now - (ClientServerOffset + TimeSpan.FromSeconds(15));

        public TimeSpan ClientServerOffset => XenObject?.Connection.ServerTimeOffset ?? TimeSpan.Zero;

        public ArchiveMaintainer(IXenObject xenObject)
        {
            Archives.Add(ArchiveInterval.FiveSecond, new DataArchive(FIVE_SECONDS_IN_TEN_MINUTES + 4));
            Archives.Add(ArchiveInterval.OneMinute, new DataArchive(MINUTES_IN_TWO_HOURS));
            Archives.Add(ArchiveInterval.OneHour, new DataArchive(HOURS_IN_ONE_WEEK));
            Archives.Add(ArchiveInterval.OneDay, new DataArchive(DAYS_IN_ONE_YEAR));
            Archives.Add(ArchiveInterval.None, new DataArchive(0));

            XenObject = xenObject;
        }

        private void Update(object _)
        {
            var firstTime = true;
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var xenObject = XenObject;

                var serverWas =
                    ServerNow; // get time before updating so we don't miss any 5 second updates if getting the past data
                if (firstTime)
                {
                    // Restrict to at most 24 hours data if necessary
                    if (Helpers.FeatureForbidden(XenObject, XenAPI.Host.RestrictPerformanceGraphs))
                    {
                        Archives[ArchiveInterval.OneHour].MaxPoints = 24;
                        Archives[ArchiveInterval.OneDay].MaxPoints = 0;
                    }
                    else
                    {
                        Archives[ArchiveInterval.OneHour].MaxPoints = HOURS_IN_ONE_WEEK;
                        Archives[ArchiveInterval.OneDay].MaxPoints = DAYS_IN_ONE_YEAR;
                    }

                    _dataSources.Clear();

                    foreach (var a in Archives.Values)
                        a.ClearSets();

                    
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    LoadingInitialData = true;
                    ArchivesUpdated?.Invoke();

                    try
                    {
                        if (xenObject is Host h)
                            _dataSources = Host.get_data_sources(h.Connection.Session, h.opaque_ref);
                        else if (xenObject is VM vm && vm.power_state == vm_power_state.Running)
                            _dataSources = VM.get_data_sources(vm.Connection.Session, vm.opaque_ref);

                        Get(ArchiveInterval.None, RrdsUri, RRD_Full_InspectCurrentNode, xenObject, _cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        //Get handles its own exception;
                        //anything caught here is thrown by the get_data_sources operations
                        Log.Error($"Failed to retrieve data sources for '{xenObject.Name()}'", e);
                    }

                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    ArchivesUpdated?.Invoke();
                    LoadingInitialData = false;

                    LastFiveSecondCollection = serverWas;
                    LastOneMinuteCollection = serverWas;
                    LastOneHourCollection = serverWas;
                    LastOneDayCollection = serverWas;
                    firstTime = false;
                }

                if (serverWas - LastFiveSecondCollection > FiveSeconds)
                {
                    Get(ArchiveInterval.FiveSecond, UpdateUri, RRD_Update_InspectCurrentNode, xenObject, _cancellationTokenSource.Token);
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    LastFiveSecondCollection = serverWas;
                    Archives[ArchiveInterval.FiveSecond].Load(_setsAdded);
                }

                if (serverWas - LastOneMinuteCollection > OneMinute)
                {
                    Get(ArchiveInterval.OneMinute, UpdateUri, RRD_Update_InspectCurrentNode, xenObject, _cancellationTokenSource.Token);
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    LastOneMinuteCollection = serverWas;
                    Archives[ArchiveInterval.OneMinute].Load(_setsAdded);
                }

                if (serverWas - LastOneHourCollection > OneHour)
                {
                    Get(ArchiveInterval.OneHour, UpdateUri, RRD_Update_InspectCurrentNode, xenObject, _cancellationTokenSource.Token);
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    LastOneHourCollection = serverWas;
                    Archives[ArchiveInterval.OneHour].Load(_setsAdded);
                }

                if (serverWas - LastOneDayCollection > OneDay)
                {
                    Get(ArchiveInterval.OneDay, UpdateUri, RRD_Update_InspectCurrentNode, xenObject, _cancellationTokenSource.Token);
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    LastOneDayCollection = serverWas;
                    Archives[ArchiveInterval.OneDay].Load(_setsAdded);
                }
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    break;
                }
                ArchivesUpdated?.Invoke();
                Thread.Sleep(SLEEP_TIME);
            }
        }

        private void Get(ArchiveInterval interval, Func<ArchiveInterval, IXenObject, Uri> uriBuilder,
            Action<XmlReader, IXenObject> readerMethod, IXenObject xenObject, CancellationToken token)
        {
            try
            {
                var uri = uriBuilder(interval, xenObject);
                if (uri == null)
                    return;

                using (var stream = HTTPHelper.GET(uri, xenObject.Connection, true))
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        _setsAdded = new List<DataSet>();
                        while (reader.Read() && !token.IsCancellationRequested)
                        {
                            readerMethod(reader, xenObject);
                        }
                    }
                }
            }
            catch (WebException)
            {
            }
            catch (Exception e)
            {
                Log.Debug(
                    string.Format("ArchiveMaintainer: Get updates for {0}: {1} Failed.",
                        xenObject is Host ? "Host" : "VM",
                        xenObject != null ? xenObject.opaque_ref : Helper.NullOpaqueRef), e);
            }
        }

        #region Uri generators

        private Uri UpdateUri(ArchiveInterval interval, IXenObject xo)
        {
            var sessionRef = xo?.Connection?.Session?.opaque_ref;
            if (sessionRef == null)
                return null;

            var escapedRef = Uri.EscapeDataString(sessionRef);
            var startTime = TimeFromInterval(interval);
            var duration = ToSeconds(interval);

            if (xo is Host host)
            {
                return BuildUri(host, "rrd_updates",
                    $"session_id={escapedRef}&start={startTime}&cf=AVERAGE&interval={duration}&host=true");
            }

            if (xo is VM vm)
            {
                var vmHost = vm.Connection.Resolve(vm.resident_on) ?? Helpers.GetCoordinator(vm.Connection);
                return BuildUri(vmHost, "rrd_updates",
                    $"session_id={escapedRef}&start={startTime}&cf=AVERAGE&interval={duration}&vm_uuid={vm.uuid}");
            }

            return null;
        }

        private static Uri RrdsUri(ArchiveInterval interval, IXenObject xo)
        {
            var sessionRef = xo.Connection.Session?.opaque_ref;
            if (sessionRef == null)
                return null;

            var escapedRef = Uri.EscapeDataString(sessionRef);

            if (xo is Host host)
                return BuildUri(host, "host_rrds", $"session_id={escapedRef}");

            if (xo is VM vm)
            {
                var vmHost = vm.Connection.Resolve(vm.resident_on) ?? Helpers.GetCoordinator(vm.Connection);
                return BuildUri(vmHost, "vm_rrds", $"session_id={escapedRef}&uuid={vm.uuid}");
            }

            return null;
        }

        private static Uri BuildUri(Host host, string path, string query)
        {
            var builder = new UriBuilder
            {
                Scheme = host.Connection.UriScheme,
                Host = host.address,
                Port = host.Connection.Port,
                Path = path,
                Query = query
            };
            return builder.Uri;
        }

        #endregion

        #region Data fetcher methods

        private void RRD_Full_InspectCurrentNode(XmlReader reader, IXenObject xmo)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                _lastNode = reader.Name;
                if (_lastNode == "row")
                {
                    _currentTime += _currentInterval * _stepSize * TICKS_IN_ONE_SECOND;
                    _valueCount = 0;
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement)
            {
                _lastNode = reader.Name;
                if (_lastNode == "rra")
                {
                    if (_bailOut)
                    {
                        _bailOut = false;
                        return;
                    }

                    var i = GetArchiveIntervalFromFiveSecs(_currentInterval);
                    if (i != ArchiveInterval.None)
                        Archives[i].CopyLoad(_setsAdded, _dataSources);

                    foreach (var set in _setsAdded)
                        set.Points.Clear();
                    _bailOut = false;
                }
            }

            if (reader.NodeType != XmlNodeType.Text)
                return;

            if (_lastNode == "name")
            {
                var str = reader.ReadContentAsString();
                _setsAdded.Add(new DataSet(xmo, false, str, _dataSources));
            }
            else if (_lastNode == "step")
            {
                var str = reader.ReadContentAsString();
                _stepSize = long.Parse(str, CultureInfo.InvariantCulture);
            }
            else if (_lastNode == "lastupdate")
            {
                var str = reader.ReadContentAsString();
                _endTime = long.Parse(str, CultureInfo.InvariantCulture);
            }
            else if (_lastNode == "pdp_per_row")
            {
                var str = reader.ReadContentAsString();
                _currentInterval = long.Parse(str, CultureInfo.InvariantCulture);

                var modInterval = _endTime % (_stepSize * _currentInterval);
                long stepCount = _currentInterval == 1 ? FIVE_SECONDS_IN_TEN_MINUTES // 120 * 5 seconds in 10 minutes
                    : _currentInterval == 12 ? MINUTES_IN_TWO_HOURS // 120 minutes in 2 hours
                    : _currentInterval == 720 ? HOURS_IN_ONE_WEEK // 168 hours in a week
                    : DAYS_IN_ONE_YEAR; // 366 days in a year

                _currentTime =
                    new DateTime((((_endTime - modInterval) - (_stepSize * _currentInterval * stepCount)) *
                                  TimeSpan.TicksPerSecond) + Util.TicksBefore1970).ToLocalTime().Ticks;
            }
            else if (_lastNode == "cf")
            {
                var str = reader.ReadContentAsString();
                if (str != "AVERAGE")
                    _bailOut = true;
            }
            else if (_lastNode == "v")
            {
                if (_bailOut || _setsAdded.Count <= _valueCount)
                    return;

                var set = _setsAdded[_valueCount];
                var str = reader.ReadContentAsString();
                set.AddPoint(str, _currentTime, _setsAdded, _dataSources);
                _valueCount++;
            }
        }

        private void RRD_Update_InspectCurrentNode(XmlReader reader, IXenObject xo)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                _lastNode = reader.Name;
                if (_lastNode == "row")
                {
                    _valueCount = 0;
                }
            }

            if (reader.NodeType != XmlNodeType.Text) return;
            if (_lastNode == "entry")
            {
                var str = reader.ReadContentAsString();
                DataSet set = null;

                if (DataSet.ParseId(str, out var objType, out var objUuid, out var dataSourceName))
                {
                    if (objType == "host")
                    {
                        var host = xo.Connection.Cache.Hosts.FirstOrDefault(h => h.uuid == objUuid);
                        if (host != null)
                            set = new DataSet(host, (xo as Host)?.uuid != objUuid, dataSourceName, _dataSources);
                    }

                    if (objType == "vm")
                    {
                        var vm = xo.Connection.Cache.VMs.FirstOrDefault(v => v.uuid == objUuid);
                        if (vm != null)
                            set = new DataSet(vm, (xo as VM)?.uuid != objUuid, dataSourceName, _dataSources);
                    }
                }

                if (set == null)
                    set = new DataSet(null, true, str, _dataSources);

                _setsAdded.Add(set);
            }
            else if (_lastNode == "t")
            {
                var str = reader.ReadContentAsString();
                _currentTime = new DateTime((Convert.ToInt64(str) * TimeSpan.TicksPerSecond) + Util.TicksBefore1970)
                    .ToLocalTime().Ticks;
            }
            else if (_lastNode == "v")
            {
                if (_setsAdded.Count <= _valueCount) return;
                var set = _setsAdded[_valueCount];
                var str = reader.ReadContentAsString();
                set.AddPoint(str, _currentTime, _setsAdded, _dataSources);
                _valueCount++;
            }
        }

        #endregion

        #region Actions

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(Update);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Pause()
        {
           _cancellationTokenSource.Cancel();
        }

        #endregion

        #region Public static methods

        public static long ToTicks(ArchiveInterval interval)
        {
            switch (interval)
            {
                case ArchiveInterval.FiveSecond:
                    return TICKS_IN_FIVE_SECONDS;
                case ArchiveInterval.OneMinute:
                    return TICKS_IN_ONE_MINUTE;
                case ArchiveInterval.OneHour:
                    return TICKS_IN_ONE_HOUR;
                default:
                    return TICKS_IN_ONE_DAY;
            }
        }

        public static ArchiveInterval NextArchiveDown(ArchiveInterval current)
        {
            switch (current)
            {
                case ArchiveInterval.FiveSecond:
                    return ArchiveInterval.None;
                case ArchiveInterval.OneMinute:
                    return ArchiveInterval.FiveSecond;
                case ArchiveInterval.OneHour:
                    return ArchiveInterval.OneMinute;
                case ArchiveInterval.OneDay:
                    return ArchiveInterval.OneHour;
                default:
                    return ArchiveInterval.None;
            }
        }

        #endregion

        #region Helpers

        private static long ToSeconds(ArchiveInterval interval)
        {
            return ToTicks(interval) / TimeSpan.TicksPerSecond;
        }

        private long TimeFromInterval(ArchiveInterval interval)
        {
            switch (interval)
            {
                case ArchiveInterval.FiveSecond:
                    if (LastFiveSecondCollection != DateTime.MinValue)
                        return Util.TicksToSecondsSince1970(LastFiveSecondCollection.Ticks - TICKS_IN_FIVE_SECONDS);
                    break;
                case ArchiveInterval.OneMinute:
                    if (LastOneMinuteCollection != DateTime.MinValue)
                        return Util.TicksToSecondsSince1970(LastOneMinuteCollection.Ticks - TICKS_IN_ONE_MINUTE);
                    break;
                case ArchiveInterval.OneHour:
                    if (LastOneHourCollection != DateTime.MinValue)
                        return Util.TicksToSecondsSince1970(LastOneHourCollection.Ticks - TICKS_IN_ONE_HOUR);
                    break;
                case ArchiveInterval.OneDay:
                    if (LastOneDayCollection != DateTime.MinValue)
                        return Util.TicksToSecondsSince1970(LastOneDayCollection.Ticks - TICKS_IN_ONE_DAY);
                    break;
            }

            return 0;
        }

        private static ArchiveInterval GetArchiveIntervalFromFiveSecs(long v)
        {
            switch (v)
            {
                case 1:
                    return ArchiveInterval.FiveSecond;
                case 12:
                    return ArchiveInterval.OneMinute;
                case 720:
                    return ArchiveInterval.OneHour;
                case 17280:
                    return ArchiveInterval.OneDay;
                default:
                    return ArchiveInterval.None;
            }
        }

        #endregion

    }
}