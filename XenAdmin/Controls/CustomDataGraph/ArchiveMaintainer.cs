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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Controls.CustomDataGraph
{
    [Flags]
    public enum ArchiveInterval { None = 0, FiveSecond = 1, OneMinute = 2, OneHour = 4, OneDay = 8 }

    public class ArchiveMaintainer
    {
        private const long TicksInOneSecond = 10000000;
        private const long TicksInFiveSeconds = 50000000;
        internal const long TicksInTenSeconds = 100000000;
        private const long TicksInOneMinute = 600000000;
        internal const long TicksInTenMinutes = 6000000000;
        private const long TicksInOneHour = 36000000000;
        internal const long TicksInTwoHours = 72000000000;
        private const long TicksInOneDay = 864000000000;
        internal const long TicksInSevenDays = 6048000000000;
        internal const long TicksInOneYear = 316224000000000;

        private const int FiveSecondsInTenMinutes = 120;
        private const int MinutesInTwoHours = 120;
        private const int HoursInOneWeek = 168;
        private const int DaysInOneYear = 366;

        private static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan OneHour = TimeSpan.FromHours(1);
        private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        private const int SleepTime = 5000;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Fired (on a background thread) when new performance data are received from the server
        /// </summary>
        internal event Action ArchivesUpdated;

        internal readonly Dictionary<ArchiveInterval, DataArchive> Archives = new Dictionary<ArchiveInterval, DataArchive>();

        /// <summary>
        /// for pausing the retrieval of updates
        /// call Monitor.PulseAll(UpdateMonitor) on resume
        /// </summary>
        private readonly object UpdateMonitor = new object();
        /// <summary>
        /// for waiting between updates
        /// the Monitor has a timeout too so we either wait for 'SleepTime' or a pulseall on WaitUpdates
        /// </summary>
        private readonly object WaitUpdates = new object();

        private Thread UpdaterThread;

        /// <summary>
        ///  if true UpdaterThread will keep looping
        /// </summary>
        private bool RunThread;
        /// <summary>
        /// Whether the thread is started or not
        /// </summary>
        private bool ThreadRunning;

        /// <summary>
        /// collection for holding updates whil
        /// </summary>
        private List<DataSet> SetsAdded;

        private IXenObject _xenObject;

        /// <summary>
        /// Gui Thread
        /// </summary>
        public IXenObject XenObject
        {
            private get { return _xenObject; }
            set
            {
                Program.AssertOnEventThread();

                string oldref = _xenObject == null ? "" : _xenObject.opaque_ref;
                _xenObject = value;
                string newref = _xenObject == null ? "" : _xenObject.opaque_ref;
                FirstTime = FirstTime || newref != oldref;
            }
        }

        public DateTime LastFiveSecondCollection = DateTime.MinValue;
        public DateTime LastOneMinuteCollection = DateTime.MinValue;
        public DateTime LastOneHourCollection = DateTime.MinValue;
        public DateTime LastOneDayCollection = DateTime.MinValue;

        public bool FirstTime = true;

        public bool LoadingInitialData = false;

        public TimeSpan ClientServerOffset
        {
            get
            {
                IXenObject m = XenObject;
                return m == null ? TimeSpan.Zero : m.Connection.ServerTimeOffset;
            }
        }

        public ArchiveMaintainer()
        {
            Archives.Add(ArchiveInterval.FiveSecond, new DataArchive(FiveSecondsInTenMinutes + 4));
            Archives.Add(ArchiveInterval.OneMinute, new DataArchive(MinutesInTwoHours));
            Archives.Add(ArchiveInterval.OneHour, new DataArchive(HoursInOneWeek));
            Archives.Add(ArchiveInterval.OneDay, new DataArchive(DaysInOneYear));
            Archives.Add(ArchiveInterval.None, new DataArchive(0));

            UpdaterThread = new Thread(Update) {Name = "Archive Maintainer", IsBackground = true};
        }

        /// <summary>
        /// Call me, async update graph data set
        /// UpdaterThread Thread
        /// </summary>
        private void Update()
        {
            while (RunThread)
            {
                IXenObject xenObject = XenObject;

                DateTime ServerWas = ServerNow(); // get time before updating so we don't miss any 5 second updates if getting the past data

                if (FirstTime)
                {
                    // Restrict to at most 24 hours data if necessary
                    if (Helpers.FeatureForbidden(_xenObject, XenAPI.Host.RestrictPerformanceGraphs))
                    {
                        Archives[ArchiveInterval.OneHour].MaxPoints = 24;
                        Archives[ArchiveInterval.OneDay].MaxPoints = 0;
                    }
                    else
                    {
                        Archives[ArchiveInterval.OneHour].MaxPoints = HoursInOneWeek;
                        Archives[ArchiveInterval.OneDay].MaxPoints = DaysInOneYear;
                    }

                    foreach (DataArchive a in Archives.Values)
                        a.ClearSets();

                    LoadingInitialData = true;
                    OnArchivesUpdated();
                    Get(ArchiveInterval.None, RrdsUri, RRD_Full_InspectCurrentNode, xenObject);
                    LoadingInitialData = false;
                    OnArchivesUpdated();

                    LastFiveSecondCollection = ServerWas;
                    LastOneMinuteCollection = ServerWas;
                    LastOneHourCollection = ServerWas;
                    LastOneDayCollection = ServerWas;
                    FirstTime = false;
                }

                if (ServerWas - LastFiveSecondCollection > FiveSeconds)
                {
                    Get(ArchiveInterval.FiveSecond, UpdateUri, RRD_Update_InspectCurrentNode, xenObject);
                    LastFiveSecondCollection = ServerWas;
                    Archives[ArchiveInterval.FiveSecond].Load(SetsAdded);
                }
                if (ServerWas - LastOneMinuteCollection > OneMinute)
                {
                    Get(ArchiveInterval.OneMinute, UpdateUri, RRD_Update_InspectCurrentNode, xenObject);
                    LastOneMinuteCollection = ServerWas;
                    Archives[ArchiveInterval.OneMinute].Load(SetsAdded);
                }
                if (ServerWas - LastOneHourCollection > OneHour)
                {
                    Get(ArchiveInterval.OneHour, UpdateUri, RRD_Update_InspectCurrentNode, xenObject);
                    LastOneHourCollection = ServerWas;
                    Archives[ArchiveInterval.OneHour].Load(SetsAdded);
                }
                if (ServerWas - LastOneDayCollection > OneDay)
                {
                    Get(ArchiveInterval.OneDay, UpdateUri, RRD_Update_InspectCurrentNode, xenObject);
                    LastOneDayCollection = ServerWas;
                    Archives[ArchiveInterval.OneDay].Load(SetsAdded);
                }

                lock (WaitUpdates)
                {
                    Monitor.Wait(WaitUpdates, SleepTime);
                }
                lock (UpdateMonitor)
                {
                    if (!ThreadRunning)
                        Monitor.Wait(UpdateMonitor);
                }
            }
        }

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        public DateTime ServerNow()
        {
            return DateTime.UtcNow.Subtract(ClientServerOffset);
        }

        private void Get(ArchiveInterval interval, Func<ArchiveInterval, IXenObject, Uri> uriBuilder,
            Action<XmlReader, IXenObject>Reader, IXenObject xenObject)
        {
            try
            {
                var uri = uriBuilder(interval, xenObject);
                if (uri == null)
                    return;

                using (Stream httpstream = HTTPHelper.GET(uri, xenObject.Connection, true))
                {
                    using (XmlReader reader = XmlReader.Create(httpstream))
                    {
                        SetsAdded = new List<DataSet>();
                        while (reader.Read())
                        {
                            Reader(reader, xenObject);
                        }
                    }
                }
            }
            catch (WebException)
            {
            }
            catch (Exception e)
            {
                log.Debug(string.Format("ArchiveMaintainer: Get updates for {0}: {1} Failed.", xenObject is Host ? "Host" : "VM", xenObject != null ? xenObject.opaque_ref : Helper.NullOpaqueRef), e);
            }
        }

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
                var vmHost = vm.Connection.Resolve(vm.resident_on) ?? Helpers.GetMaster(vm.Connection);
                BuildUri(vmHost, "rrd_updates",
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
                var vmHost = vm.Connection.Resolve(vm.resident_on) ?? Helpers.GetMaster(vm.Connection);
                return BuildUri(vmHost, "vm_rrds", $"session_id={escapedRef}&uuid={vm.uuid}");
            }

            return null;
        }

        private static Uri BuildUri(Host host, string path, string query)
        {
            UriBuilder builder = new UriBuilder
            {
                Scheme = host.Connection.UriScheme,
                Host = host.address,
                Port = host.Connection.Port,
                Path = path,
                Query = query
            };
            return builder.Uri;
        }

        public static long ToTicks(ArchiveInterval interval)
        {
            switch (interval)
            {
                case ArchiveInterval.FiveSecond:
                    return TicksInFiveSeconds;
                case ArchiveInterval.OneMinute:
                    return TicksInOneMinute;
                case ArchiveInterval.OneHour:
                    return TicksInOneHour;
                default:
                    return TicksInOneDay;
            }
        }

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
                        return Util.TicksToSecondsSince1970(LastFiveSecondCollection.Ticks - TicksInFiveSeconds);
                    break;
                case ArchiveInterval.OneMinute:
                    if (LastOneMinuteCollection != DateTime.MinValue)
                        return Util.TicksToSecondsSince1970(LastOneMinuteCollection.Ticks - TicksInOneMinute);
                    break;
                case ArchiveInterval.OneHour:
                    if (LastOneHourCollection != DateTime.MinValue)
                        return Util.TicksToSecondsSince1970(LastOneHourCollection.Ticks - TicksInOneHour);
                    break;
                case ArchiveInterval.OneDay:
                    if (LastOneDayCollection != DateTime.MinValue)
                        return Util.TicksToSecondsSince1970(LastOneDayCollection.Ticks - TicksInOneDay);
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


        private long EndTime = 0;
        private bool BailOut = false;
        private long CurrentInterval = 0;
        private long StepSize = 0;

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        private void RRD_Full_InspectCurrentNode(XmlReader reader, IXenObject xmo)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                LastNode = reader.Name;
                if (LastNode == "row")
                {
                    CurrentTime += CurrentInterval * StepSize * TicksInOneSecond;
                    ValueCount = 0;
                }
            }

            if (reader.NodeType == XmlNodeType.EndElement)
            {
                LastNode = reader.Name;
                if (LastNode == "rra")
                {
                    if (BailOut)
                    {
                        BailOut = false;
                        return;
                    }

                    ArchiveInterval i = GetArchiveIntervalFromFiveSecs(CurrentInterval);
                    if (i != ArchiveInterval.None)
                        Archives[i].CopyLoad(SetsAdded);

                    foreach (DataSet set in SetsAdded)
                        set.Points.Clear();
                    BailOut = false;
                }
            }

            if (reader.NodeType != XmlNodeType.Text)
                return;

            if (LastNode == "name")
            {
                string str = reader.ReadContentAsString();
                SetsAdded.Add(DataSet.Create(xmo, false, str));
            }
            else if (LastNode == "step")
            {
                string str = reader.ReadContentAsString();
                StepSize = long.Parse(str, CultureInfo.InvariantCulture);
            }
            else if (LastNode == "lastupdate")
            {
                string str = reader.ReadContentAsString();
                EndTime = long.Parse(str, CultureInfo.InvariantCulture);
            }
            else if (LastNode == "pdp_per_row")
            {
                string str = reader.ReadContentAsString();
                CurrentInterval = long.Parse(str, CultureInfo.InvariantCulture);

                long modInterval = EndTime % (StepSize * CurrentInterval);
                long stepCount = CurrentInterval == 1 ? FiveSecondsInTenMinutes // 120 * 5 seconds in 10 minutes
                               : CurrentInterval == 12 ? MinutesInTwoHours   // 120 minutes in 2 hours
                               : CurrentInterval == 720 ? HoursInOneWeek     // 168 hours in a week
                               : DaysInOneYear;                              // 366 days in a year

                CurrentTime = new DateTime((((EndTime - modInterval) - (StepSize * CurrentInterval * stepCount)) * TimeSpan.TicksPerSecond) + Util.TicksBefore1970).ToLocalTime().Ticks;
            }
            else if (LastNode == "cf")
            {
                string str = reader.ReadContentAsString();
                if (str != "AVERAGE")
                    BailOut = true;
            }
            else if (LastNode == "v")
            {
                if (BailOut || SetsAdded.Count <= ValueCount)
                    return;

                DataSet set = SetsAdded[ValueCount];
                string str = reader.ReadContentAsString();
                set.AddPoint(str, CurrentTime, SetsAdded);
                ValueCount++;
            }
        }

        private long CurrentTime = 0;
        private int ValueCount;

        private string LastNode = "";

        /// <summary>
        /// UpdaterThread Thread
        /// </summary>
        private void RRD_Update_InspectCurrentNode(XmlReader reader, IXenObject xo)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                LastNode = reader.Name;
                if (LastNode == "row")
                {
                    ValueCount = 0;
                }
            }
            if (reader.NodeType != XmlNodeType.Text) return;
            if (LastNode == "entry")
            {
                string str = reader.ReadContentAsString();
                DataSet set = null;

                if (DataSet.ParseId(str, out string objType, out string objUuid, out string dataSourceName))
                {
                    if (objType == "host")
                    {
                        Host host = xo.Connection.Cache.Hosts.FirstOrDefault(h => h.uuid == objUuid);
                        if (host != null)
                            set = DataSet.Create(host, (xo as Host)?.uuid != objUuid, dataSourceName);
                    }

                    if (objType == "vm")
                    {
                        VM vm = xo.Connection.Cache.VMs.FirstOrDefault(v => v.uuid == objUuid);
                        if (vm != null)
                            set = DataSet.Create(vm, (xo as VM)?.uuid != objUuid, dataSourceName);
                    }
                }

                if (set == null)
                    set = DataSet.Create(null, true, str);

                SetsAdded.Add(set);
            }
            else if (LastNode == "t")
            {
                string str = reader.ReadContentAsString();
                CurrentTime = new DateTime((Convert.ToInt64(str) * TimeSpan.TicksPerSecond) + Util.TicksBefore1970).ToLocalTime().Ticks;
            }
            else if (LastNode == "v")
            {
                if (SetsAdded.Count <= ValueCount) return;
                DataSet set = SetsAdded[ValueCount];
                string str = reader.ReadContentAsString();
                set.AddPoint(str, CurrentTime, SetsAdded);
                ValueCount++;
            }
        }

        /// <summary>
        /// run this to start or resume getting updates
        /// </summary>
        public void Start()
        {
            if (ThreadRunning)
                return;  // if we are already running dont start twice!
            ThreadRunning = true;
            RunThread = true; // keep looping
            if ((UpdaterThread.ThreadState & ThreadState.Unstarted) > 0)
                UpdaterThread.Start(); // if we have never been started
            else
            {
                lock (UpdateMonitor)
                    Monitor.PulseAll(UpdateMonitor);
                lock (WaitUpdates)
                    Monitor.PulseAll(WaitUpdates);
            }
        }

        /// <summary>
        /// for clean-up on exit only
        /// </summary>
        public void Stop()
        {
            ThreadRunning = false;
            RunThread = false; // exit loop
            // make sure we clear all Monitor.Waits so we can exit
            lock (WaitUpdates)
                Monitor.PulseAll(WaitUpdates);
            lock (UpdateMonitor)
                Monitor.PulseAll(UpdateMonitor);
        }

        /// <summary>
        /// for stoping getting updates when switching away from perfomance panel
        /// </summary>
        public void Pause()
        {
            ThreadRunning = false; // stop updating
            lock (WaitUpdates) // clear the first Monitor.Wait so we pause the thread instantly.
                Monitor.PulseAll(WaitUpdates);
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

        public DateTime GraphNow
        {
            get
            {
                return DateTime.Now - (ClientServerOffset + TimeSpan.FromSeconds(15));
            }
        }

        private void OnArchivesUpdated()
        {
            if (ArchivesUpdated != null)
                ArchivesUpdated();
        }
    }
}
