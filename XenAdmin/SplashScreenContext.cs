/* Copyright (c) Cloud Software Group Holdings, Inc.
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
using System.Configuration;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Dialogs;

namespace XenAdmin
{
    internal class SplashScreenContext : ApplicationContext
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SplashScreen _splashScreen;
        private MainWindow _mainWindow;
        private readonly string[] _args;

        public SplashScreenContext(params string[] args)
        {
            _args = args;
            _splashScreen = new SplashScreen();
            _splashScreen.ShowMainWindowRequested += _splashScreen_ShowMainWindowRequested;
            StartLoadingSettings();
            _splashScreen.Show();
        }

        private void StartLoadingSettings()
        {
            ThreadPool.QueueUserWorkItem(obj =>
            {
                try
                {
                    Settings.Load();
                    Settings.ReconfigureProxyAuthenticationSettings();
                    Settings.ConfigureExternalSshClientSettings();
                    Settings.Log();

                    _splashScreen.AllowToClose = true;
                }
                catch (ConfigurationErrorsException ex)
                {
                    _log.Error("Could not load settings.", ex);
                    CloseSplash();

                    var msg = string.Format("{0}\n\n{1}", Messages.MESSAGEBOX_LOAD_CORRUPTED_TITLE,
                        string.Format(Messages.MESSAGEBOX_LOAD_CORRUPTED, Settings.GetUserConfigPath()));
                    using (var dlg = new ErrorDialog(msg)
                    {
                        StartPosition = FormStartPosition.CenterScreen,
                        ShowInTaskbar = true
                    })
                    {
                        dlg.ShowDialog();
                    }

                    Environment.Exit(1);
                }
            });
        }

        private void _splashScreen_ShowMainWindowRequested()
        {
            if (_mainWindow == null)
            {
                Program.MainWindow = _mainWindow = new MainWindow(_args);
                _mainWindow.CloseSplashRequested += _mainWindow_CloseSplashRequested;
                _mainWindow.FormClosed += mainWindow_FormClosed;
                _mainWindow.Show();
            }
        }

        private void _mainWindow_CloseSplashRequested()
        {
            CloseSplash();
        }

        private void mainWindow_FormClosed(object s, FormClosedEventArgs args)
        {
            ExitThread();
        }

        public void CloseSplash()
        {
            Program.Invoke(_splashScreen, _splashScreen.Close);
        }
    }
}
