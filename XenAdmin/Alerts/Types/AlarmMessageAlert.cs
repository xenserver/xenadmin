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
using XenAPI;
using XenAdmin.TabPages;
using XenAdmin.Dialogs;
using System.Xml;
using System.Text.RegularExpressions;
using XenAdmin.Core;
using System.Globalization;


namespace XenAdmin.Alerts
{
    public class AlarmMessageAlert : MessageAlert
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private AlarmType AlarmType;
        private double CurrentValue;
        private double TriggerLevel;
        private int TriggerPeriod;
        private SR sr;

        public AlarmMessageAlert(Message m)
            : base(m)
        {
            ParseAlarmMessage(m);
        }

        private void ParseAlarmMessage(Message m)
        {
            /*
             * message.body will look similar to this:
             * value: 1234
             * config:
             * <variable>
             *  <name value="network_usage"/>
             *  <alarm_trigger_level value="1"/>
             *  <alarm_trigger_period value="60"/>
             * </variable>
            */
            List<string> lines = new List<string>(m.body.Split('\n'));
            if (lines.Count < 2)
                return;
            string value = lines[0].Replace("value: ", "");
            double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out CurrentValue);

            string variableName = "";

            try
            {
                string xml = string.Join("", lines.GetRange(1, lines.Count - 1).ToArray()).Replace("config:", "");
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNodeList name = doc.GetElementsByTagName("name");
                if (name.Count > 0)
                    variableName = name[0].Attributes["value"].Value;
                // if this doesn't get set the alarm will appear as "unrecognised"
                //in the dialog and will show the message body instead

                XmlNodeList level = doc.GetElementsByTagName("alarm_trigger_level");
                if (level.Count > 0)
                    double.TryParse(level[0].Attributes["value"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out TriggerLevel);

                XmlNodeList period = doc.GetElementsByTagName("alarm_trigger_period");
                if (period.Count > 0)
                    int.TryParse(period[0].Attributes["value"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out TriggerPeriod);
            }
            catch (Exception e)
            {
                log.Debug("Error Parsing Message Description", e);
            }

            switch (variableName)
            {
                case PerfmonDefinition.ALARM_TYPE_CPU:
                    AlarmType = AlarmType.Cpu;
                    break;
                case PerfmonDefinition.ALARM_TYPE_NETWORK:
                    AlarmType = AlarmType.Net;
                    break;
                case PerfmonDefinition.ALARM_TYPE_DISK:
                    AlarmType = AlarmType.Disk;
                    break;
                case PerfmonDefinition.ALARM_TYPE_FILESYSTEM:
                    AlarmType = AlarmType.FileSystem;
                    break;
                case PerfmonDefinition.ALARM_TYPE_MEMORY_FREE:
                    AlarmType = AlarmType.Memory;
                    break;
                case PerfmonDefinition.ALARM_TYPE_MEMORY_DOM0_USAGE:
                    AlarmType = AlarmType.Dom0MemoryDemand;
                    break;
                case PerfmonDefinition.ALARM_TYPE_LOG_FILESYSTEM:
                    AlarmType = AlarmType.LogFileSystem;
                    break;
                case PerfmonDefinition.ALARM_TYPE_SR_PHYSICAL_UTILISATION:
                    AlarmType = AlarmType.SrPhysicalUtilisation;
                    break;
                default:
                    {
                        var match = PerfmonDefinition.SrRegex.Match(variableName);
                        if (match.Success)
                        {
                            AlarmType = AlarmType.Storage;
                            sr = GetStorage(match.Groups[1].Value);
                        }
                        else
                            AlarmType = AlarmType.None;
                    }
                    break;
            }
        }

        public override AlertPriority Priority
        {
            get
            {
                switch (AlarmType)
                {
                    case AlarmType.FileSystem:
                        return AlertPriority.Priority2;
                    case AlarmType.LogFileSystem:
                        return AlertPriority.Priority3;
                    default:
                        return base.Priority;
                }
            }
        }
        
        public override string Description
        {
            get
            {
                switch (AlarmType)
                {
                    case AlarmType.Cpu:
                        return string.Format(Messages.ALERT_ALARM_CPU_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             Util.PercentageString(CurrentValue),
                                             Util.TimeString(TriggerPeriod),
                                             Util.PercentageString(TriggerLevel));
                    case AlarmType.Net:
                        return string.Format(Messages.ALERT_ALARM_NETWORK_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             Util.DataRateString(CurrentValue),
                                             Util.TimeString(TriggerPeriod),
                                             Util.DataRateString(TriggerLevel));
                    case AlarmType.Disk:
                        return string.Format(Messages.ALERT_ALARM_DISK_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             Util.DataRateString(CurrentValue),
                                             Util.TimeString(TriggerPeriod),
                                             Util.DataRateString(TriggerLevel));
                    case AlarmType.FileSystem:
                        return string.Format(Messages.ALERT_ALARM_FILESYSTEM_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             Util.PercentageString(CurrentValue));
                    case AlarmType.Memory:
                        return string.Format(Messages.ALERT_ALARM_MEMORY_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             Util.MemorySizeStringSuitableUnits(CurrentValue * Util.BINARY_KILO, false),//xapi unit is in kib
                                             Util.TimeString(TriggerPeriod),
                                             Util.MemorySizeStringSuitableUnits(TriggerLevel * Util.BINARY_KILO, false));
                    case AlarmType.Dom0MemoryDemand:
                        return string.Format(Messages.ALERT_ALARM_DOM0_MEMORY_DEMAND_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             Util.PercentageString(CurrentValue),
                                             Util.PercentageString(TriggerLevel));
                    case AlarmType.Storage:
                        return string.Format(Messages.ALERT_ALARM_STORAGE_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             sr == null ? "" : sr.Name,
                                             Util.DataRateString(CurrentValue * Util.BINARY_MEGA), //xapi unit is in Mib
                                             Util.TimeString(TriggerPeriod),
                                             Util.DataRateString(TriggerLevel * Util.BINARY_MEGA));
                    case AlarmType.LogFileSystem:
                        return string.Format(Messages.ALERT_ALARM_LOG_FILESYSTEM_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             Util.PercentageString(CurrentValue));
                    case AlarmType.SrPhysicalUtilisation:
                        return string.Format(Messages.ALERT_ALARM_SR_PHYSICAL_UTILISATION_DESCRIPTION,
                                             Helpers.GetNameAndObject(XenObject),
                                             Util.PercentageString(CurrentValue),
                                             Util.PercentageString(TriggerLevel));
                    default:
                        return base.Description;
                }
            }
        }

        public override string Title
        {
            get
            {
                switch (AlarmType)
                {
                    case AlarmType.Cpu:
                        return Messages.ALERT_ALARM_CPU;
                    case AlarmType.Net:
                        return Messages.ALERT_ALARM_NETWORK;
                    case AlarmType.Disk:
                        return Messages.ALERT_ALARM_DISK;
                    case AlarmType.FileSystem:
                        return Messages.ALERT_ALARM_FILESYSTEM;
                    case AlarmType.Memory:
                        return Messages.ALERT_ALARM_MEMORY;
                    case AlarmType.Storage:
                        return Messages.ALERT_ALARM_STORAGE;
                    case AlarmType.Dom0MemoryDemand:
                        return Messages.ALERT_ALARM_DOM0_MEMORY;
                    case AlarmType.LogFileSystem:
                        return Messages.ALERT_ALARM_LOG_FILESYSTEM;
                    case AlarmType.SrPhysicalUtilisation:
                        return Messages.ALERT_ALARM_SR_PHYSICAL_UTILISATION;
                    default:
                        return base.Title;
                }
            }
        }

        public override Action FixLinkAction
        {
            get
            {
                return () =>
                    {
                        IXenObject xenObject = null;

                        if (XenObject is Host)
                        {
                            //sr is only set when it's AlarmType.Storage 
                            xenObject = sr ?? XenObject;
                        }
                        else if (XenObject is VM)
                        {
                            VM vm = (VM)XenObject;
                            xenObject = vm.IsControlDomainZero ? XenObject.Connection.Resolve(vm.resident_on) : XenObject;
                        }

                        if (xenObject == null)
                            return;

                        using (var dialog = new PropertiesDialog(xenObject) { TopMost = true })
                        {
                            dialog.SelectPerfmonAlertEditPage();
                            dialog.ShowDialog(Program.MainWindow);
                        }
                    };
            }
        }

        public override string FixLinkText
        {
            get
            {
                return Messages.ALERT_ALARM_ACTION;
            }
        }

        public override string HelpID
        {
            get
            {
                return string.Format("{0}UsageMessageAlert", AlarmType);
            }
        }

        public override string HelpLinkText
        {
            get
            {
                return Messages.ALERT_GENERIC_HELP;
            }
        }

        private SR GetStorage(string srUuid)
        {
            var connection = XenObject.Connection;

            for (int i = 0; i < connection.Cache.PBDs.Length; i++)
            {
                var sr = connection.Resolve(connection.Cache.PBDs[i].SR);
                if (sr != null && sr.uuid.StartsWith(srUuid))
                    return sr;
            }
            return null;
        }
    }

    public enum AlarmType { None, Cpu, Net, Disk, FileSystem, Memory, Storage, Dom0MemoryDemand, LogFileSystem, SrPhysicalUtilisation }
}
