/* Copyright (c) Citrix Systems Inc. 
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

// ============================================================================
// Description:   Utilitiy functions built on top of libxen for use in all
//                providers.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace XenOvf.Utilities
{
    [Flags]
    public enum EnumLogLevel { Off = 0, Audit = 1, Error = 2, Warning = 4, Info = 8, Trace = 16, Debug = 32 };
    [Flags]
    public enum EnumLogType { Off = 0, Audit = 1, Console = 2, File = 4, Event = 8, Exception = 16 };
    public sealed class Log
    {
        private static FileStream fs;
        private static string logpath = null;
        private static string logfile = null;
        private static object lockfile = new object();
        private static object lockevent = new object();
        private static object lockexcept = new object();
        private static EnumLogLevel defaultLogLevel = (EnumLogLevel.Audit | EnumLogLevel.Error | EnumLogLevel.Warning);
        private static EnumLogType defaultLogType = (EnumLogType.Audit | EnumLogType.Console | EnumLogType.File);
        private static bool isconfigured = false;
        private static EnumLogLevel curLevel = defaultLogLevel;
        private static EnumLogType logType = defaultLogType;

        private Log() { }

        [SecurityPermission(SecurityAction.LinkDemand)]
        public static string GetLogPath()
        {
            string path = null;
            if (Properties.Settings.Default.LogPath.ToLower().StartsWith("installlocation"))
            {
                path = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            }
            else if (Properties.Settings.Default.LogPath.ToLower().StartsWith("applicationdata"))
            {
                path = Path.Combine(System.Environment.GetEnvironmentVariable("APPDATA"), Properties.Settings.Default.LogSubPath);
            }
            else if (Properties.Settings.Default.LogPath.ToLower().StartsWith("programdata"))
            {
                path = Path.Combine(System.Environment.GetEnvironmentVariable("ProgramData"), Properties.Settings.Default.LogSubPath);
            }
            else
            {
                path = Properties.Settings.Default.LogPath;
            }
            return path;
        }
        
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static string GetLogFileName()
        {
            return logfile;
        }
        
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void Trace(string format, params object[] args)
        {
            Write(EnumLogLevel.Trace, format, args);
        }
        
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void Debug(string format, params object[] args)
        {
            Write(EnumLogLevel.Debug, format, args);
        }

        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void Info(string format, params object[] args)
        {
            Write(EnumLogLevel.Info, format, args);
        }

        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void Warning(string format, params object[] args)
        {
            Write(EnumLogLevel.Warning, format, args);
        }

        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void Error(string format, params object[] args)
        {
            Write(EnumLogLevel.Error, format, args);
        }

        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void Audit(string format, params object[] args)
        {
            Write(EnumLogLevel.Audit, format, args);
        }
 
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void Bytes(byte[] bytes)
        {
            EnumLogLevel curLevel = (EnumLogLevel)Enum.Parse(typeof(EnumLogLevel), Properties.Settings.Default.LogLevel, true);
            EnumLogType logType = (EnumLogType)Enum.Parse(typeof(EnumLogType), Properties.Settings.Default.LogType, true);

            #region LOG TO CONSOLE
            if (logType >= EnumLogType.Console)
            {
                Console.WriteLine(Encoding.ASCII.GetString(bytes));
            }
            #endregion

            #region LOG TO FILE
            if (logType >= EnumLogType.File)
            {
                lock (lockfile)
                {
                    if (fs == null)
                    {
                        try
                        {
                            fs = new FileStream(logfile, FileMode.Create, FileAccess.Write, FileShare.Read);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Cannot open Repository, try running as Administrator or change location of Log", ex);
                        }
                    }
                    if (fs != null)
                    {
                        try
                        {
                            StreamWriter sw = new StreamWriter(fs);
                            sw.WriteLine(Encoding.ASCII.GetString(bytes));
                            sw.Flush();
                        }
                        catch
                        {
                            // do nothing
                        }
                        finally
                        {
                            // do nothing
                        }
                    }
                }
            }
            #endregion

        }

        [SecurityPermission(SecurityAction.LinkDemand)]
        private static void Write(EnumLogLevel level, string format, params object[] args)
        {
            string forwardstatus = null;
            string resetloglevel = null;
            string resetlogtype = null;
            if (Properties.Settings.Default.LogForwarding.Length > 0 &&
                !Properties.Settings.Default.LogForwarding.ToLower().StartsWith("no") &&
                !Properties.Settings.Default.LogForwarding.ToLower().StartsWith("off"))
            {
                try
                {
                    Forward(Properties.Settings.Default.LogForwarding, level, format, args);
                    return;
                }
                catch
                {
                    forwardstatus = "FWD: ";
                }
            }

            if (logpath == null || logfile == null)
            {
                logpath = GetLogPath();
                logfile = Path.Combine(logpath, Properties.Settings.Default.LogFile);
            }

            #region CONFIGURE TYPE AND LEVEL
            if (!isconfigured)
            {
                if (Properties.Settings.Default.LogLevel.Length > 0)
                {
                    try
                    {
                        curLevel = (EnumLogLevel)Enum.Parse(typeof(EnumLogLevel), Properties.Settings.Default.LogLevel, true);
                    }
                    catch
                    {
                        curLevel = defaultLogLevel;
                        resetloglevel = "--- Failure in determining configured log level, reset to: Audit, Error, Warning";
                    }

                }

                if (Properties.Settings.Default.LogType.Length > 0)
                {
                    try
                    {
                        logType = (EnumLogType)Enum.Parse(typeof(EnumLogType), Properties.Settings.Default.LogType, true);
                    }
                    catch
                    {
                        logType = defaultLogType;
                        resetlogtype = "--- Failure in determining configure log type, reset to: Audit, Console, File";
                    }
                }

                if ((curLevel & EnumLogLevel.Audit) != EnumLogLevel.Audit)
                {
                    curLevel |= EnumLogLevel.Audit;
                }

                if ((logType & EnumLogType.Audit) != EnumLogType.Audit)
                {
                    logType |= EnumLogType.Audit;
                }
                isconfigured = true;
            }
            #endregion

            string data = string.Format(format, args);
            string message = null;
            if (level == EnumLogLevel.Audit)
            {
                message = string.Format("{0}:{1}.{2}.{3}.{4}.{5}.{6},{7}:[{8}] {9}",
                        level,
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        DateTime.Now.Day,
                        DateTime.Now.Hour,
                        DateTime.Now.Minute,
                        DateTime.Now.Second,
                        DateTime.Now.Millisecond,
                        System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                        data
                        );
                
            }
            else
            {
                if (Properties.Settings.Default.LogFormat.ToLower().Equals("plain"))
                {
                    message = data;
                }
                else
                {
                    message = string.Format("{0}:{1}.{2}.{3}.{4}.{5}.{6},{7}: {8}",
                            level,
                            DateTime.Now.Year,
                            DateTime.Now.Month,
                            DateTime.Now.Day,
                            DateTime.Now.Hour,
                            DateTime.Now.Minute,
                            DateTime.Now.Second,
                            DateTime.Now.Millisecond,
                            data
                            );
                }
            }

            if (forwardstatus != null)
            {
                message = forwardstatus + message;
            }

            if (curLevel >= level && curLevel != EnumLogLevel.Off && logType != EnumLogType.Off)
            {

                #region LOG TO CONSOLE
                if ((logType & EnumLogType.Console) == EnumLogType.Console)
                {
                    if (resetloglevel != null)
                    {
                        Console.WriteLine(resetloglevel);
                    }
                    if (resetlogtype != null)
                    {
                        Console.WriteLine(resetlogtype);
                    }
                    Console.WriteLine(message);
                }
                #endregion

                #region LOG TO FILE
                if ((logType & EnumLogType.File) == EnumLogType.File)
                {
                    lock (lockfile)
                    {
                        if (fs == null)
                        {
                            try
                            {
                                fs = new FileStream(logfile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Cannot open log file, try running as Administrator or change location of Log", ex);
                            }
                        }
                        if (fs != null)
                        {
                            try
                            {
                                StreamWriter sw = new StreamWriter(fs);
                                if (resetloglevel != null) { sw.WriteLine(resetloglevel); }
                                if (resetlogtype != null) { sw.WriteLine(resetlogtype); }
                                sw.WriteLine(message);
                                sw.Flush();
                                sw.Close();
                            }
                            catch
                            {
                                // do nothing
                            }
                            finally
                            {
                                if (fs != null) fs.Close();
                                fs = null;
                            }
                        }
                    }
                }
                #endregion

                #region LOG TO EVENT
                if ((logType & EnumLogType.Event) == EnumLogType.Event)
                {
                    lock (lockevent)
                    {
                        if (!EventLog.SourceExists(Properties.Settings.Default.LogSource))
                        {
                            EventLog.CreateEventSource(Properties.Settings.Default.LogSource, "Application");
                        }
                        EventLogEntryType logtype = EventLogEntryType.Information;
                        switch(level)
                        {
                            case EnumLogLevel.Audit: { logtype = EventLogEntryType.SuccessAudit;  break; }
                            case EnumLogLevel.Debug: { break; }
                            case EnumLogLevel.Error: { logtype = EventLogEntryType.Error;  break; }
                            case EnumLogLevel.Info: { logtype = EventLogEntryType.Information;  break; }
                            case EnumLogLevel.Warning: { break; }
                            case EnumLogLevel.Trace: { break; }
                            case EnumLogLevel.Off: { break; }
                            default: { break; }
                        }
                        EventLog.WriteEntry(Properties.Settings.Default.LogSource, message, logtype);
                    }
                }
                #endregion

                #region LOG TO EXCEPTION
                if ((logType & EnumLogType.Exception) == EnumLogType.Exception)
                {
                    lock (lockexcept)
                    {
                        throw new Exception(message);
                    }
                }
                #endregion

                #region LOG IN DebugView
                if ((curLevel & EnumLogLevel.Debug) == EnumLogLevel.Debug)
                {
                    if (resetloglevel != null) { System.Diagnostics.Debug.WriteLine(resetloglevel); }
                    if (resetlogtype != null) { System.Diagnostics.Debug.WriteLine(resetlogtype); }
                    System.Diagnostics.Debug.WriteLine(message);
                }
                #endregion

                #region LOG IN TraceView
                if ((curLevel & EnumLogLevel.Trace) == EnumLogLevel.Trace)
                {
                    if (resetloglevel != null) { System.Diagnostics.Debug.WriteLine(resetloglevel); }
                    if (resetlogtype != null) { System.Diagnostics.Debug.WriteLine(resetlogtype); }
                    System.Diagnostics.Trace.WriteLine(message);
                }
                #endregion
            }

            resetloglevel = null;
            resetlogtype = null;
        }

        private static void Forward(string AssemblyClassName, EnumLogLevel level, string format, params object[] args)
        {

            string[] _assemblyinfo = AssemblyClassName.Split(new char[] { ',' });

            try
            {
                Assembly logassembly = null;
                try
                {
                    logassembly = Assembly.GetAssembly(Type.GetType(_assemblyinfo[1]));
                }
                catch { }
                if (logassembly == null)
                {
                    string asspath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _assemblyinfo[0]);
                    logassembly = Assembly.LoadFrom(asspath);
                }
                Type LogClassType = logassembly.GetType(_assemblyinfo[1]);
                MethodInfo mi = LogClassType.GetMethod(level.ToString());
                List<object> newargs = new List<object>();
                newargs.Add(format);
                newargs.Add(args);

                mi.Invoke(null, newargs.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
