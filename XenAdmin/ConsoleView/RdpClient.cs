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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DotNetVnc;
using XenAdmin.Core;
using XenAdmin.RDP;

namespace XenAdmin.ConsoleView
{
    class RdpClient: IRemoteConsole
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Size size;

        private bool allowDisplayUpdate;

        private readonly ContainerControl parent;

        /// <summary>
        /// http://msdn2.microsoft.com/en-us/library/aa383022(VS.85).aspx
        /// </summary>
        private MsRdpClient9 rdpClient = null;

        public event EventHandler OnDisconnected = null;

        internal RdpClient(ContainerControl parent, Size size, EventHandler resizeHandler)
        {
            this.parent = parent;
            this.size = size;
            try
            {
                rdpClient = new MsRdpClient9();
                RDPConfigure(size);

                //add event handler for when RDP display is resized
                rdpClient.OnRemoteDesktopSizeChange += rdpClient_OnRemoteDesktopSizeChange;
                rdpClient.Resize += resizeHandler;
                // CA-96135: Try adding rdpControl to parent.Controls list; this will throw exception when
                // MsRdpClient9 control cannot be created (there is no appropriate version of dll present)
                parent.Controls.Add(rdpClient);
                allowDisplayUpdate = true;
            }
            catch (Exception ex)
            {
                if (parent.Controls.Contains(rdpClient))
                    parent.Controls.Remove(rdpClient);
                rdpClient.Dispose();
                rdpClient = null;
                Log.Error("Adding rdpControl to parent.Controls list caused an exception.", ex);
            }
        }

        private void RDPConfigure(Size currentConsoleSize)
        {
            rdpClient.BeginInit();
            rdpLocationOffset = new Point(3, 3); //small offset to accomodate focus border
            rdpClient.Dock = DockStyle.None;
            rdpClient.Anchor = AnchorStyles.None;
            rdpClient.Size = currentConsoleSize;
            RDPAddOnDisconnected();
            rdpClient.Enter += RdpEnter;
            rdpClient.Leave += rdpClient_Leave;
            rdpClient.GotFocus += rdpClient_GotFocus;
            rdpClient.EndInit();
        }


        public Point rdpLocationOffset
        {
            set 
            {
                if (rdpClient == null)
                    return;

                rdpClient.Location = value; 
            }
        }

        private void RDPAddOnDisconnected()
        {
            if (rdpClient == null)
                return;

            rdpClient.OnDisconnected += rdpClient_OnDisconnected;
        }

        private void RDPSetSettings()
        {
            if (rdpClient == null)
                return;
            
            rdpClient.SecuredSettings2.KeyboardHookMode = Properties.Settings.Default.WindowsShortcuts ? 1 : 0;
            rdpClient.SecuredSettings2.AudioRedirectionMode = Properties.Settings.Default.ReceiveSoundFromRDP ? 0 : 1;
            rdpClient.AdvancedSettings3.DisableRdpdr = Properties.Settings.Default.ClipboardAndPrinterRedirection ? 0 : 1;
            rdpClient.AdvancedSettings7.ConnectToAdministerServer = Properties.Settings.Default.ConnectToServerConsole;
           
            //CA-103910 - enable NLA
            rdpClient.AdvancedSettings5.AuthenticationLevel = 2;
            rdpClient.AdvancedSettings7.EnableCredSspSupport = true;
        }

        public void RDPConnect(string rdpIP, int w, int h)
        {
            if (rdpClient == null)
                return;

            Log.DebugFormat("Connecting RDPClient9 using server '{0}', width '{1}' and height '{2}'", rdpIP, w, h);

            rdpClient.Server = rdpIP;
            rdpClient.DesktopWidth = w;
            rdpClient.DesktopHeight = h;
            rdpClient.Connect();
        }

        public void UpdateDisplay(int width, int height, Point locationOffset)
        {
            if (rdpClient == null)
                return;

            if (Connected && allowDisplayUpdate)
            {
                try
                {
                    Log.DebugFormat("Updating display settings using width '{0}' and height '{1}'", width, height);
                    rdpClient.UpdateSessionDisplaySettings((uint)width, (uint)height, (uint)width, (uint)height, 1, 1, 1);
                    rdpClient.Size = new Size(width, height);
                    rdpLocationOffset = locationOffset;
                    parent.AutoScroll = false;
                }
                catch
                {
                    allowDisplayUpdate = false;
                    parent.AutoScroll = true;
                    parent.AutoScrollMinSize = rdpClient.Size;
                }
            }
        }

        private bool Connected
        {
            get { return rdpClient == null ? false : rdpClient.Connected == 1; }
        }

        private int DesktopHeight
        {
            get { return rdpClient == null ? 0 : rdpClient.DesktopHeight; }
        }

        private int DesktopWidth
        {
            get { return rdpClient == null ? 0 : rdpClient.DesktopWidth; }
        }

        private static readonly List<System.Windows.Forms.Timer> RdpCleanupTimers = new List<System.Windows.Forms.Timer>();
        void rdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Program.AssertOnEventThread();

            if (OnDisconnected != null)
                OnDisconnected(this, null);

        }

        //refresh to draw focus border in correct position after display is updated
        void rdpClient_OnRemoteDesktopSizeChange(object sender, AxMSTSCLib.IMsTscAxEvents_OnRemoteDesktopSizeChangeEvent e)
        {
            Program.AssertOnEventThread();

            if (rdpClient == null || parent == null)
                return;

            rdpClient.Size = DesktopSize;
            parent.Refresh();
        }

        public void Connect(string rdpIP)
        {
            try
            {
                RDPSetSettings();
            }
            catch (Exception ex)
            {
                if (parent.Controls.Contains(rdpClient))
                    parent.Controls.Remove(rdpClient);
                rdpClient.Dispose();
                rdpClient = null;
                Log.Error("Setting the RDP client properties caused an exception.", ex);
            } 
            RDPConnect(rdpIP, size.Width, size.Height);
        }

        public void Disconnect()
        {
            try
            {
                if (Connected)
                {
                    if (rdpClient == null)
                        return;
                    rdpClient.Disconnect();
                }  
            }
            catch(InvalidComObjectException ex)
            {
                Log.ErrorFormat("Disconnecting RdpClient caused an exception: {0}, {1}", ex.Message, ex.StackTrace);
            }
            catch(AccessViolationException ex)
            {
                //We seem (often unpredictably) to get a read/write into protected memory while disposing the client eg: CA-91482
                Log.ErrorFormat("Disconnecting RdpClient caused an AccessViolationException: {0}, {1}", ex.Message, ex.StackTrace);
            }
            catch(NullReferenceException ex)
            {
                //Sometimes rdpClient.Disconnect() crashes with NullReferenceException eg: CA-94062
                Log.ErrorFormat("Disconnecting RdpClient caused a NullReferenceException: {0}, {1}", ex.Message, ex.StackTrace);
            }
        }

        internal Set<int> pressedScans = new Set<int>();
        private bool modifierKeyPressedAlone = false;

        private void handleRDPKey(bool pressed, int scancode, int keysym)
        {
            bool containsFocus = parent.ParentForm != null && parent.ParentForm.ContainsFocus;

            if (rdpClient == null || !containsFocus)
                return;

            if (KeyHandler.handleExtras<int>(pressed, pressedScans, KeyHandler.ExtraScans, scancode, KeyHandler.ModifierScans, ref modifierKeyPressedAlone))
                parent.Focus();
        }

        void rdpClient_Leave(object sender, EventArgs e)
        {
            Program.MainWindow.MenuShortcuts = true;
            InterceptKeys.releaseKeys();
            pressedScans = new Set<int>();
        }

        void RdpEnter(object sender, EventArgs e)
        {
            Activate();
        }

        void rdpClient_GotFocus(object sender, EventArgs e)
        {
            Activate();
        }

        #region IRemoteConsole implementation

        public ConsoleKeyHandler KeyHandler
        {
            get;
            set;
        }

        public Control ConsoleControl
        {
            get { return rdpClient; }
        }

        public void Activate()
        {
            Program.MainWindow.MenuShortcuts = false;
            if (rdpClient != null)
            {
                if (!rdpClient.Focused)
                    rdpClient.Select();

                InterceptKeys.releaseKeys();
                InterceptKeys.grabKeys(new InterceptKeys.KeyEvent(handleRDPKey), true);
            }
        }

        public void DisconnectAndDispose()
        {
            try
            {
                Disconnect();
            }
            catch
            {
            }

            try
            {
                Dispose();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        private bool disposed;
        public void Dispose(bool disposing)
        {
            if(!disposed)
            {
                if(disposing)
                {
                    if (rdpClient != null)
                    {
                        // We need to dispose the rdp control. However, doing it immediately (in the control's own
                        // OnDisconnected event) will cause a horrible crash. Instead, start a timer that will
                        // call the dispose method on the GUI thread at the next available opportunity. CA-12902
                        Timer t = new Timer();
                        t.Tick += delegate
                                      {
                                          try
                                          {
                                              Log.Debug("RdpClient Dispose(): rdpControl.Dispose() in delegate");
                                              rdpClient.Dispose();
                                          }
                                          catch (Exception)
                                          {
                                              // We often get NullReferenceException here
                                          }
                                          t.Stop();
                                          RdpCleanupTimers.Remove(t);
                                          Log.Debug("RdpClient Dispose(): Timer stopped and removed in delegate");
                                      };
                        t.Interval = 1;
                        RdpCleanupTimers.Add(t);
                        Log.DebugFormat("RdpClient Dispose(): Start timer (timers count {0})", RdpCleanupTimers.Count);
                        t.Start();
                    }
                    else
                        Log.Debug("RdpClient Dispose(): rdpControl == null");
                }
                rdpClient = null;
                Log.Debug("RdpClient Dispose(): disposed = true");
                disposed = true;
            }
        }

        public void Pause()
        {
        }

        public void Unpause()
        {
        }

        public void SendCAD()
        {
        }

        public Image Snapshot()
        {
            return null;
        }

        public bool SendScanCodes
        {
            set { }
        }

        public bool Scaling
        {
            get;
            set;
        }

        public bool DisplayBorder
        {
            set { }
        }
        
        public Size DesktopSize
        {
            get { return rdpClient != null ? new Size(DesktopWidth, DesktopHeight) /*rdpControl.Size*/ : Size.Empty; }
            set { }
        }

        public Rectangle ConsoleBounds
        {
            get
            {
                return rdpClient != null ? new Rectangle(rdpClient.Location.X, rdpClient.Location.Y, DesktopWidth, DesktopHeight) : Rectangle.Empty;
            }
        }

        #endregion
    }
}
