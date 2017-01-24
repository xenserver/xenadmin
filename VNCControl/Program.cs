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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace XenAdmin
{
    public static class Program
    {
        public static UserControl MainWindow = null;

        private static log4net.ILog log = null;
        public static volatile bool Exiting = false;

        static Program()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Assembly.GetCallingAssembly().Location + ".config"));
            log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        internal static void AssertOffEventThread()
        {
            if (Program.MainWindow.Visible && !Program.MainWindow.InvokeRequired)
            {
                FatalError();
            }
        }

        internal static void AssertOnEventThread()
        {
            if (Program.MainWindow != null && Program.MainWindow.Visible && Program.MainWindow.InvokeRequired)
            {
                FatalError();
            }
        }

        private static void FatalError()
        {
            string msg = String.Format(Messages.MESSAGEBOX_PROGRAM_UNEXPECTED,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), GetLogFile_());
            log.Fatal(msg + "\n" + Environment.StackTrace);
            MessageBox.Show(msg, Messages.XENCENTER);
        }

        public static string GetLogFile_()
        {
            foreach (log4net.Appender.IAppender appender in log.Logger.Repository.GetAppenders())
            {
                if (appender is log4net.Appender.FileAppender)
                {
                    // Assume that the first FileAppender is the primary log file.
                    return ((log4net.Appender.FileAppender)appender).File;
                }
            }
            return null;
        }

        /// <summary>
        /// Safely invoke the given delegate on the given control.
        /// </summary>
        public static void Invoke(Control c, MethodInvoker f)
        {
            try
            {
                if (!IsExiting(c))
                {
                    if (c.InvokeRequired)
                    {
                        c.Invoke(f);
                    }
                    else
                    {
                        f();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                if (!IsExiting(c)) throw;
            }
            catch (InvalidAsynchronousStateException)
            {
                if (!IsExiting(c)) throw;
            }
        }

        private static bool IsExiting(Control c)
        {
            return Exiting || c.Disposing || c.IsDisposed || !c.IsHandleCreated;
        }

        /// <summary>
        /// Safely invoke the given delegate on the given control.
        /// </summary>
        /// <returns>The result of the function call, or null if c is being disposed.</returns>
        public static object Invoke(Control c, Delegate f, params object[] p)
        {
            try
            {
                if (!IsExiting(c))
                {
                    return c.InvokeRequired ? c.Invoke(f, p) : f.DynamicInvoke(p);
                }
            }
            catch (ObjectDisposedException)
            {
                if (!IsExiting(c)) throw;
            }
            catch (InvalidAsynchronousStateException)
            {
                if (!IsExiting(c)) throw;
            }
            return null;
        }
    }
}
