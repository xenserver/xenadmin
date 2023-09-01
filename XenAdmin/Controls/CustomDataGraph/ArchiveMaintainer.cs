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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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

    public class ArchiveMaintainer : IDisposable
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public IXenObject XenObject { get; }

        public DateTime LastFiveSecondCollection = DateTime.MinValue;
        public DateTime LastOneMinuteCollection = DateTime.MinValue;
        public DateTime LastOneHourCollection = DateTime.MinValue;
        public DateTime LastOneDayCollection = DateTime.MinValue;

        public DateTime GraphNow => DateTime.Now - (ClientServerOffset + TimeSpan.FromSeconds(15));
        public TimeSpan ClientServerOffset => XenObject?.Connection.ServerTimeOffset ?? TimeSpan.Zero;
        public bool LoadingInitialData;

        internal const long TICKS_IN_ONE_SECOND = 10000000;
        internal const long TICKS_IN_FIVE_SECONDS = 50000000;
        internal const long TICKS_IN_ONE_MINUTE = 600000000;
        internal const long TICKS_IN_TEN_MINUTES = 6000000000;
        internal const long TICKS_IN_ONE_HOUR = 36000000000;
        internal const long TICKS_IN_TWO_HOURS = 72000000000;
        internal const long TICKS_IN_ONE_DAY = 864000000000;
        internal const long TICKS_IN_SEVEN_DAYS = 6048000000000;
        internal const long TICKS_IN_ONE_YEAR = 316224000000000;

        internal const int FIVE_SECONDS_IN_TEN_MINUTES = 120;
        internal const int MINUTES_IN_TWO_HOURS = 120;
        internal const int HOURS_IN_ONE_WEEK = 168;
        internal const int DAYS_IN_ONE_YEAR = 366;

        internal event Action ArchivesUpdated;

        internal readonly Dictionary<ArchiveInterval, DataArchive> Archives =
            new Dictionary<ArchiveInterval, DataArchive>();

        private const int SLEEP_TIME = 5000;

        private volatile bool _requestedCancellation;
        private volatile bool _running;

        private readonly object _runningLock = new object();
        private bool RequestedCancellation
        {
            get
            {
                lock (_runningLock)
                {
                    return _requestedCancellation;
                }
            }
            set
            {
                lock (_runningLock)
                {
                    _requestedCancellation = value;
                }
            }
        }

        private bool Running
        {
            get
            {
                lock (_runningLock)
                {
                    return _running;
                }
            }
            set
            {
                lock (_runningLock)
                {
                    _running = value;
                }
            }
        }


        private List<DataSet> _setsAdded;
        private List<Data_source> _dataSources = new List<Data_source>();
        private DateTime ServerNow => DateTime.UtcNow.Subtract(ClientServerOffset);
        private long _endTime;
        private bool _bailOut;
        private long _currentInterval;
        private long _stepSize;
        private long _currentTime;
        private int _valueCount;
        private string _lastNode = string.Empty;

        public ArchiveMaintainer(IXenObject xenObject)
        {
            Archives.Add(ArchiveInterval.FiveSecond, new DataArchive(FIVE_SECONDS_IN_TEN_MINUTES + 4));
            Archives.Add(ArchiveInterval.OneMinute, new DataArchive(MINUTES_IN_TWO_HOURS));
            Archives.Add(ArchiveInterval.OneHour, new DataArchive(HOURS_IN_ONE_WEEK));
            Archives.Add(ArchiveInterval.OneDay, new DataArchive(DAYS_IN_ONE_YEAR));
            Archives.Add(ArchiveInterval.None, new DataArchive(0));

            XenObject = xenObject;
        }

        private void StartUpdateLoop(object _)
        {
            var serverWas = ServerNow;
            InitialLoad(serverWas);

            while (!RequestedCancellation)
            {
                if (ServerNow - LastFiveSecondCollection > TimeSpan.FromSeconds(5))
                {
                    Get(ArchiveInterval.FiveSecond, UpdateUri, RRD_Update_InspectCurrentNode, XenObject);
                    if (RequestedCancellation)
                    {
                        break;
                    }

                    LastFiveSecondCollection = ServerNow;
                    Archives[ArchiveInterval.FiveSecond].Load(_setsAdded);
                }

                if (ServerNow - LastOneMinuteCollection > TimeSpan.FromMinutes(1))
                {
                    Get(ArchiveInterval.OneMinute, UpdateUri, RRD_Update_InspectCurrentNode, XenObject);
                    if (RequestedCancellation)
                    {
                        break;
                    }

                    LastOneMinuteCollection = ServerNow;
                    Archives[ArchiveInterval.OneMinute].Load(_setsAdded);
                }

                if (ServerNow - LastOneHourCollection > TimeSpan.FromHours(1))
                {
                    Get(ArchiveInterval.OneHour, UpdateUri, RRD_Update_InspectCurrentNode, XenObject);
                    if (RequestedCancellation)
                    {
                        break;
                    }

                    LastOneHourCollection = ServerNow;
                    Archives[ArchiveInterval.OneHour].Load(_setsAdded);
                }

                if (ServerNow - LastOneDayCollection > TimeSpan.FromDays(1))
                {
                    Get(ArchiveInterval.OneDay, UpdateUri, RRD_Update_InspectCurrentNode, XenObject);
                    if (RequestedCancellation)
                    {
                        break;
                    }

                    LastOneDayCollection = ServerNow;
                    Archives[ArchiveInterval.OneDay].Load(_setsAdded);
                }
                if (RequestedCancellation)
                {
                    break;
                }
                ArchivesUpdated?.Invoke();
                Thread.Sleep(SLEEP_TIME);
                if (RequestedCancellation)
                {
                    break;
                }
            }
        }

        private void InitialLoad(DateTime initialServerTime)
        {
            // Restrict to at most 24 hours data if necessary
            if (Helpers.FeatureForbidden(XenObject, Host.RestrictPerformanceGraphs))
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

            if (RequestedCancellation)
            {
                return;
            }

            LoadingInitialData = true;
            ArchivesUpdated?.Invoke();

            try
            {
                switch (XenObject)
                {
                    case Host h:
                        _dataSources = Host.get_data_sources(h.Connection.Session, h.opaque_ref);
                        break;
                    case VM vm when vm.power_state == vm_power_state.Running:
                        _dataSources = VM.get_data_sources(vm.Connection.Session, vm.opaque_ref);
                        break;
                }

                if (RequestedCancellation)
                {
                    return;
                }

                Get(ArchiveInterval.None, RrdsUri, RRD_Full_InspectCurrentNode, XenObject);
            }
            catch (Exception e)
            {
                //Get handles its own exception; Anything caught here is thrown by the get_data_sources operations
                Log.Error($"Failed to retrieve data sources for '{XenObject.Name()}'", e);
            }

            if (RequestedCancellation)
            {
                return;
            }

            ArchivesUpdated?.Invoke();
            LoadingInitialData = false;

            LastFiveSecondCollection = initialServerTime;
            LastOneMinuteCollection = initialServerTime;
            LastOneHourCollection = initialServerTime;
            LastOneDayCollection = initialServerTime;
        }

        private void Get(ArchiveInterval interval, Func<ArchiveInterval, IXenObject, Uri> uriBuilder,
            Action<XmlReader, IXenObject> readerMethod, IXenObject xenObject)
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
                        while (reader.Read() && !RequestedCancellation)
                        {
                            readerMethod(reader, xenObject);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Warn(
                    $"Get updates for {(xenObject is Host ? "Host" : "VM")}: {(xenObject != null ? xenObject.opaque_ref : Helper.NullOpaqueRef)} Failed.",
                    e);
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

            switch (xo)
            {
                case Host host:
                    return BuildUri(host, "rrd_updates",
                        $"session_id={escapedRef}&start={startTime}&cf=AVERAGE&interval={duration}&host=true");
                case VM vm:
                {
                    var vmHost = vm.Connection.Resolve(vm.resident_on) ?? Helpers.GetCoordinator(vm.Connection);
                    return BuildUri(vmHost, "rrd_updates",
                        $"session_id={escapedRef}&start={startTime}&cf=AVERAGE&interval={duration}&vm_uuid={vm.uuid}");
                }
                default:
                    const string issue =
                        "ArchiveMaintainer.UpdateUri was given an invalid XenObject. Only Hosts and VMs are supported.";
                    Log.Warn(issue);
                    Debug.Assert(false, issue);
                    return null;
            }
        }

        private static Uri RrdsUri(ArchiveInterval interval, IXenObject xo)
        {
            var sessionRef = xo.Connection.Session?.opaque_ref;
            if (sessionRef == null)
                return null;

            var escapedRef = Uri.EscapeDataString(sessionRef);

            switch (xo)
            {
                case Host host:
                    return BuildUri(host, "host_rrds", $"session_id={escapedRef}");
                case VM vm:
                {
                    var vmHost = vm.Connection.Resolve(vm.resident_on) ?? Helpers.GetCoordinator(vm.Connection);
                    return BuildUri(vmHost, "vm_rrds", $"session_id={escapedRef}&uuid={vm.uuid}");
                }
                default:
                    const string issue =
                        "ArchiveMaintainer.UpdateUri was given an invalid XenObject. Only Hosts and VMs are supported.";
                    Log.Warn(issue);
                    Debug.Assert(false, issue);
                    return null;
            }
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
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                {
                    _lastNode = reader.Name;
                    if (_lastNode == "row")
                    {
                        _currentTime += _currentInterval * _stepSize * TICKS_IN_ONE_SECOND;
                        _valueCount = 0;
                    }

                    break;
                }
                case XmlNodeType.EndElement:
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

                    break;
                }
            }

            if (reader.NodeType != XmlNodeType.Text)
                return;

            switch (_lastNode)
            {
                case "name":
                {
                    var str = reader.ReadContentAsString();
                    _setsAdded.Add(new DataSet(xmo, false, str, _dataSources));
                    break;
                }
                case "step":
                {
                    var str = reader.ReadContentAsString();
                    _stepSize = long.Parse(str, CultureInfo.InvariantCulture);
                    break;
                }
                case "lastupdate":
                {
                    var str = reader.ReadContentAsString();
                    _endTime = long.Parse(str, CultureInfo.InvariantCulture);
                    break;
                }
                case "pdp_per_row":
                {
                    var str = reader.ReadContentAsString();
                    _currentInterval = long.Parse(str, CultureInfo.InvariantCulture);

                    var modInterval = _endTime % (_stepSize * _currentInterval);
                    long stepCount = _currentInterval == 1
                        ? FIVE_SECONDS_IN_TEN_MINUTES // 120 * 5 seconds in 10 minutes
                        : _currentInterval == 12
                            ? MINUTES_IN_TWO_HOURS // 120 minutes in 2 hours
                            : _currentInterval == 720
                                ? HOURS_IN_ONE_WEEK // 168 hours in a week
                                : DAYS_IN_ONE_YEAR; // 366 days in a year

                    _currentTime =
                        new DateTime((((_endTime - modInterval) - (_stepSize * _currentInterval * stepCount)) *
                                      TimeSpan.TicksPerSecond) + Util.TicksBefore1970).ToLocalTime().Ticks;
                    break;
                }
                case "cf":
                {
                    var str = reader.ReadContentAsString();
                    if (str != "AVERAGE")
                        _bailOut = true;
                    break;
                }
                case "v" when _bailOut || _setsAdded.Count <= _valueCount:
                    return;
                case "v":
                {
                    var set = _setsAdded[_valueCount];
                    var str = reader.ReadContentAsString();
                    set.AddPoint(str, _currentTime, _setsAdded, _dataSources);
                    _valueCount++;
                    break;
                }
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
            Debug.Assert(!Running, "ArchiveMaintainer is not meant to have more than one worker thread. Ensure you are not calling Start multiple times");
            // someone already tried to dispose this archive maintainer
            if (RequestedCancellation || Running)
            {
                return;
            }

            Running = ThreadPool.QueueUserWorkItem(StartUpdateLoop);
        }

        public void Dispose()
        {
            RequestedCancellation = true;
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