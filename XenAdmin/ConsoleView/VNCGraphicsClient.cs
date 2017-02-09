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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DotNetVnc;
using XenAdmin.Core;
using System.Linq;

namespace XenAdmin.ConsoleView
{
    public class VNCGraphicsClient : UserControl, IVNCGraphicsClient, IRemoteConsole
    {
        public const int BORDER_PADDING = 4;
        public const int BORDER_WIDTH = 1;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event XenAdmin.ExceptionEventHandler ErrorOccurred = null;
        public event EventHandler ConnectionSuccess = null;
        private VNCStream vncStream = null;

        /// <summary>
        /// connected implies that vncStream is non-null and ready to be used.
        /// </summary>
        private volatile bool connected = false;
        public bool Connected
        {
            get { return connected; }
        }

        /// <summary>
        /// terminated means that we have been told to disconnect.  connected starts off false, becomes
        /// true when a connection is made, and then becomes false again when connection goes away for
        /// whatever reason.  In contrast, terminated may become true even before a connection has been
        /// made.
        /// </summary>
        private volatile bool terminated = false;
        public bool Terminated
        {
            get { return terminated; }
        }

        private CustomCursor RemoteCursor = null;
        private CustomCursor LocalCursor = new CustomCursor(XenAdmin.Properties.Resources.vnc_local_cursor, 2, 2);

        /// <summary>
        /// This field is locked before any drawing is done through backGraphics or frontGraphics.
        /// It is set in the constructor below, and then there is a delicate handover during desktop
        /// resize to keep this safe.
        /// </summary>
        private Bitmap backBuffer = null;

        /// <summary>
        /// The contents of the backbuffer are interesting if the backbuffer has been drawn to,
        /// and we will present them to the user even after disconnect (if they are not interesting
        /// we just show a blank rectangle instead).
        /// </summary>
        private volatile bool backBufferInteresting = false;

        /// <summary>
        /// A Graphics object onto backBuffer.  Access to this field must always be locked under backBuffer.
        /// This field will be set to null in Dispose.
        /// </summary>
        private Graphics backGraphics = null;

        /// <summary>
        /// Graphics on actual screen.  Access to this field must always be locked under backBuffer.
        /// This field will be set to null in Dispose.
        /// </summary>
        private Graphics frontGraphics = null;

        private Object mouseEventLock = new Object();

        private Rectangle damage = Rectangle.Empty;

        private float scale;
        private float dx;
        private float dy;
        private float oldDx = 0;
        private float oldDy = 0;
        private int Bump;
        private bool scaling = false;
        private bool sendScanCodes = true;
        private bool useSource = false;

        public bool UseSource
        {
            set
            {
                useSource = value;
            }
        }

        public bool talkingToVNCTerm
        {
            get
            {
                return !sendScanCodes && useSource;
            }
        }

        public string UUID
        {
            get { return sourceVM.uuid; }
        }

        private XenAPI.VM sourceVM;
        public XenAPI.VM SourceVM
        {
            get { return sourceVM; }
            set { sourceVM = value; }
        }

        public string VmName
        {
            get { return sourceVM.name_label; }
        }

        public object Console;

        public event EventHandler DesktopResized = null;

        public VNCGraphicsClient(ContainerControl parent)
        {
            Program.AssertOnEventThread();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                        | ControlStyles.UserPaint
                        | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Opaque, false);

            frontGraphics = CreateGraphics();
            setupGraphicsOptions(frontGraphics);
            backBuffer = new Bitmap(640, 480, frontGraphics);
            backGraphics = Graphics.FromImage(backBuffer);
            setupGraphicsOptions(backGraphics);
            using (SolidBrush backBrush = new SolidBrush(BackColor))
            {
                backGraphics.FillRectangle(backBrush, 0, 0, 640, 480);
            }
            DesktopSize = new Size(640, 480);

#pragma warning disable 0219
            IntPtr _ = Handle;
#pragma warning restore 0219

            Clip.ClipboardChanged += ClipboardChanged;
            parent.Controls.Add(this);
        }

        private static void setupGraphicsOptions(Graphics g)
        {
            g.CompositingQuality = CompositingQuality.AssumeLinear;
            g.InterpolationMode = InterpolationMode.Default;
            g.SmoothingMode = SmoothingMode.Default;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!disposing)
                    return;

                Clip.ClipboardChanged -= ClipboardChanged;

                Disconnect();

                lock (backBuffer)
                {
                    frontGraphics.Dispose();
                    backGraphics.Dispose();
                    backBuffer.Dispose();
                    backGraphics = null;
                    frontGraphics = null;
                }

                if (RemoteCursor != null)
                {
                    RemoteCursor.Dispose();
                    RemoteCursor = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        internal void connect(Stream stream, char[] password)
        {
            Program.AssertOnEventThread();

            this.vncStream = new VNCStream(this, stream, helperIsPaused);
            this.vncStream.ErrorOccurred += this.OnError;
            this.vncStream.ConnectionSuccess += this.vncStream_ConnectionSuccess;
            this.connected = true;
            this.vncStream.connect(password);
        }

        private void vncStream_ConnectionSuccess(object sender, EventArgs e)
        {
            Program.AssertOffEventThread();

            // Set the remote clipboard based on the current contents.
            Program.Invoke(this, (EventHandler)ClipboardChanged, null, null);

            if (ConnectionSuccess != null)
                Program.Invoke(this, ConnectionSuccess, sender, e);
        }

        private void OnError(object sender, Exception e)
        {
            Program.AssertOffEventThread();
            System.Diagnostics.Debug.Assert(sender == vncStream); // Please see to CA-236844 if this assertion fails

            if (sender != vncStream)
                return;
            
            this.connected = false;
            if (ErrorOccurred != null)
                ErrorOccurred(this, e);
        }

        /// <summary>
        /// Nothrow guarantee.
        /// </summary>
        public void Disconnect()
        {
            connected = false;
            terminated = true;
            if (vncStream != null)
            {
                vncStream.ErrorOccurred -= OnError;
                vncStream.ConnectionSuccess -= vncStream_ConnectionSuccess;
            } 
            VNCStream s = vncStream;
            vncStream = null;
            if (s != null)
            {
                s.Close();
            }
        }

        private bool RedirectingClipboard()
        {
#if VNCControl
            return true;
#else
            return XenAdmin.Properties.Settings.Default.ClipboardAndPrinterRedirection;
#endif
        }

        private static bool handlingChange = false;
        private bool updateClipboardOnFocus = false;
        private void ClipboardChanged(object sender, EventArgs args)
        {
            Program.AssertOnEventThread();

            if (!RedirectingClipboard())
                return;

            try
            {
                if (!handlingChange && connected)
                {
                    if (Focused)
                        setConsoleClipboard();
                    else
                        updateClipboardOnFocus = true;
                }
            }
            catch (System.IO.IOException)
            {
                // The server's gone away -- that's fine.
            }
            catch (Exception exn)
            {
                Log.Warn(exn, exn);
                // Nothing more we can do with this.
            }
        }

        private void setConsoleClipboard()
        {
            try
            {
                handlingChange = true;
                string text = Clip.ClipboardText;
                if (talkingToVNCTerm)
                    text = text.Replace("\r\n", "\n");
                vncStream.clientCutText(text);
                updateClipboardOnFocus = false;
            }
            finally
            {
                handlingChange = false;
            }
        }

        /// <summary>
        /// Records damage to the screen.
        /// </summary>
        private void Damage(int x, int y, int width, int height)
        {
            Program.AssertOffEventThread();

            Rectangle r = new Rectangle(x, y, width, height);

            if (scaling)
            {
                r.Inflate(Bump, Bump); // Fix for scaling issues
            }

            if (this.damage.IsEmpty)
            {
                this.damage = r;
            }
            else
            {
                this.damage = Rectangle.Union(this.damage, r);
            }
        }

        private void checkAssertion(bool assertion, String message, params Object[] args)
        {
            if (!assertion)
            {
                Log.Error("Bad VNC server message: " + String.Format(message, args));
            }
        }

        #region Functions called by vncStream to update display

        public void ClientDrawImage(Bitmap image, int x, int y, int width, int height)
        {
            Program.AssertOffEventThread();

            //GraphicsUtils.startTime();
            Damage(x, y, width, height);
            lock (backBuffer)
            {
                try
                {
                    if (backGraphics != null)
                        backGraphics.DrawImageUnscaled(image, x, y);
                }
                catch (Exception e)
                {
                    // We seem to be very occasionally getting wierd exception from this.  These are probably due to
                    // bad server messages, so we can just log and ignore them

                    Log.Error("Error drawing image from server", e);

                    try
                    {
                        checkAssertion(image.Width == width, "Width {0} != {1}", image.Width, width);
                        checkAssertion(image.Height == height, "Height {0} != {1}", image.Height, height);
                        checkAssertion(x < DesktopSize.Width, "x {0} >= {1}", x, DesktopSize.Width);
                        checkAssertion(y < DesktopSize.Height, "y {0} >= {1}", y, DesktopSize.Height);
                        checkAssertion(x + width <= DesktopSize.Width, "x + width {0} + {1} > {2}", x, width, DesktopSize.Width);
                        checkAssertion(y + height <= DesktopSize.Height, "y + height {0} + {1} > {2}", y, height, DesktopSize.Height);
                    }
                    catch
                    {
                    }

                    return;
                }
            }
            //GraphicsUtils.endTime("drawImage");
        }

        #endregion

        #region Custom Cursor handling

        // Taken from 
        // http://www.codeproject.com/cs/miscctrl/DragDropTreeview.asp?df=100&forumid=84437&exp=0&select=1838138#xx1838138xx

        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref ICONINFO iconinfo);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        class CustomCursor
        {
            private Cursor cursor = null;
            private IntPtr handle = IntPtr.Zero;

            internal CustomCursor(Bitmap bitmap, int x, int y)
            {
                ICONINFO iconInfo = new ICONINFO();
                iconInfo.fIcon = false;
                iconInfo.xHotspot = x;
                iconInfo.yHotspot = y;
                iconInfo.hbmMask = bitmap.GetHbitmap();
                iconInfo.hbmColor = bitmap.GetHbitmap();

                handle = CreateIconIndirect(ref iconInfo);
                cursor = new Cursor(handle);
            }

            ~CustomCursor()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
                try
                {
                    if (handle != IntPtr.Zero)
                        DestroyIcon(handle);
                }
                catch
                {
                }
                finally
                {
                    handle = IntPtr.Zero;
                }
            }

            public Cursor Cursor
            {
                get { return cursor; }
            }
        }

        public void ClientSetCursor(Bitmap image, int x, int y, int width, int height)
        {
            Program.AssertOffEventThread();

            if (RemoteCursor != null)
                RemoteCursor.Dispose();
            RemoteCursor = new CustomCursor(image, x, y);

            Program.Invoke(this, delegate()
            {
                if (cursorOver)
                    this.Cursor = RemoteCursor.Cursor;
            });
        }

        #endregion

        public void ClientCopyRectangle(int x, int y, int width, int height, int dx, int dy)
        {
            Program.AssertOffEventThread();

            //GraphicsUtils.startTime();
            Damage(dx, dy, width, height);
            lock (backBuffer)
            {
                if (backGraphics != null)
                    GraphicsUtils.copyRect(backBuffer, x, y, width, height, backGraphics, dx, dy);
            }
            //GraphicsUtils.endTime("copyRectangle");
        }

        public void ClientFillRectangle(int x, int y, int width, int height, Color color)
        {
            Program.AssertOffEventThread();

            //GraphicsUtils.startTime();
            Damage(x, y, width, height);
            lock (backBuffer)
            {
                if (backGraphics != null)
                {
                    using (SolidBrush backBrush = new SolidBrush(color))
                    {
                        backGraphics.FillRectangle(backBrush, x, y, width, height);
                    }
                }
            }
            //GraphicsUtils.endTime("FillRect");
        }

        public void ClientFrameBufferUpdate()
        {
            Program.AssertOffEventThread();

            //GraphicsUtils.startTime();
            lock (backBuffer)
            {
                try
                {
                    if (!damage.IsEmpty)
                    {
                        if (frontGraphics != null)
                            frontGraphics.DrawImage(backBuffer, damage, damage, GraphicsUnit.Pixel);
                        damage = Rectangle.Empty;
                    }

                    backBufferInteresting = true;
                }
                catch
                {
                }
            }

            /*
             * If theres a pending mouse event, send it.
             * Also reset the mouse event counters
             */
            lock (this.mouseEventLock)
            {
                if (this.pending != null)
                {
                    mouseEvent(this.pendingState, this.pending.X, this.pending.Y);
                    this.pending = null;
                }

                this.mouseMoved = 0;
                this.mouseNotMoved = 0;
            }
            //GraphicsUtils.endTime("franeBufferUpdate");
        }

        public void ClientBell()
        {
        }

        private String clipboardStash = "";

        public void ClientCutText(String text)
        {
            Program.AssertOffEventThread();

            if (talkingToVNCTerm)
            {
                // Lets translate from unix line endings to windows ones...
                clipboardStash = toWindowsLineEndings(text);
            }
            else
            {
                if (!RedirectingClipboard())
                    return;
                Program.Invoke(this, delegate()
                {
                    if (Clipboard.ContainsText() && Clipboard.GetText() == text)
                        return;
                    Clip.SetClipboardText(text);
                });
            }
        }

        // This is a cut-and-paste of Helpers.toWindowsLineEndings.  This avoids
        // the standalone VNC control depending upon Helpers.
        private static string toWindowsLineEndings(string input)
        {
            return Regex.Replace(input, "\r?\n", "\r\n");
        }

        public void ClientDesktopSize(int width, int height)
        {
            Program.AssertOffEventThread();

            // Cannot do an invoke with a locked back buffer, as it may event thread
            // (onPaint) tried to lock back buffer as well - therefore deadlock.

            Program.Invoke(this, delegate()
            {
                Bitmap old_back_buffer;
                lock (backBuffer)
                {
                    if (width <= 0)
                        width = 1;
                    if (height <= 0)
                        height = 1;

                    if (width == DesktopSize.Width &&
                        height == DesktopSize.Height)
                    {
                        return;
                    }

                    old_back_buffer = backBuffer;

                    backGraphics.Dispose();
                    frontGraphics.Dispose();

                    frontGraphics = CreateGraphics();
                    setupGraphicsOptions(frontGraphics);

                    Bitmap new_back_buffer = new Bitmap(width, height, frontGraphics);
                    backGraphics = Graphics.FromImage(new_back_buffer);
                    setupGraphicsOptions(backGraphics);

                    using (SolidBrush backBrush = new SolidBrush(BackColor))
                    {
                        backGraphics.FillRectangle(backBrush, 0, 0, width, height);
                    }

                    DesktopSize = new Size(width, height);

                    // Now that backGraphics is valid, we can switch backBuffer.  We're relying on backBuffer
                    // as our lock.
                    backBuffer = new_back_buffer;
                }

                lock (backBuffer)
                {
                    SetupScaling();
                }

                Invalidate();
                Update();

                old_back_buffer.Dispose();

                OnDesktopResized();
            });
        }

        private void OnDesktopResized()
        {
            Program.AssertOnEventThread();

            if (DesktopResized != null)
                DesktopResized(this, null);
        }

        /**
         * Overridden Events
         */
        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            if (!this.scaling && se.OldValue != se.NewValue)
            {
                if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                {
                    dx = (-1 * se.NewValue) + (displayBorder ? BORDER_PADDING : 0);
                    lock (backBuffer)
                    {
                        if (frontGraphics != null)
                        {
                            frontGraphics.ResetTransform();
                            frontGraphics.TranslateTransform(dx, dy);
                        }
                    }
                }

                if (se.ScrollOrientation == ScrollOrientation.VerticalScroll)
                {
                    dy = (-1 * se.NewValue) + (displayBorder ? BORDER_PADDING : 0);
                    lock (backBuffer)
                    {
                        if (frontGraphics != null)
                        {
                            frontGraphics.ResetTransform();
                            frontGraphics.TranslateTransform(dx, dy);
                        }
                    }
                }

                this.Refresh();
            }
        }

        private void MyPaintBackground(Graphics g, int x, int y, int width, int height)
        {
            Rectangle r = new Rectangle(x, y, width, height);

            // Hack pulled from Control.cs:PaintTransparentBackground
            if (Application.RenderWithVisualStyles)
            {
                ButtonRenderer.DrawParentBackground(g, r, this);
            }
            else
            {
                using (Brush backBrush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(backBrush, r);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Program.AssertOnEventThread();

            //base.OnPaint(e);
            if (this.connected || backBufferInteresting)
            {
                /*
                 * Draw the background by working out the surrounding bars
                 * as opposed to just one big black rect, to reduce flicker.
                 */

                // Draw two vertical bars at either end

                int w = (int)dx;//Math.Max(displayBorder ? BORDER_PADDING : 0,(int)Math.Ceiling((this.Size.Width - (this.DesktopSize.Width * this.scale)) / 2));

                Graphics g = e.Graphics;

                //GraphicsUtils.startTime();
                lock (backBuffer)
                {
                    if (frontGraphics != null)
                        frontGraphics.DrawImageUnscaled(backBuffer, 0, 0);
                }
                //GraphicsUtils.endTime("OnPaint");

                if (w > 0)
                    MyPaintBackground(g, 0, 0, w + 1, ClientSize.Height);

                w = (int)(ClientSize.Width - dx - (DesktopSize.Width * scale)) + 1;

                if (w > 0)
                    MyPaintBackground(g, ClientSize.Width - w, 0, w, ClientSize.Height);

                // Draw two horizontal bars at top and bottom

                int h = (int)dy;

                if (h > 0)
                    MyPaintBackground(g, 0, 0, Size.Width, h + 1);

                h = (int)(ClientSize.Height - dy - (DesktopSize.Height * scale)) + 1;

                if (h > 0)
                    MyPaintBackground(g, 0, ClientSize.Height - h, Size.Width, h);

                /*
                 * Draw the focus rect
                 */
                /*if (Focused)
                {
                    DrawFocusRect(e);
                }*/
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnResize(e);

            /*
             * Need to recreate the front graphics or we
             * get weird clipping problems.
             */
            lock (backBuffer)
            {
                if (frontGraphics != null)
                    frontGraphics.Dispose();
                frontGraphics = CreateGraphics();
                setupGraphicsOptions(frontGraphics);
                SetupScaling();

                // We don't need to redraw, as the window will repaint us
            }
        }

        private int currentMouseState = 0;
        /*
         * We're going to track how many moves we have between screen updates 
         * to stop the mouse running away with long updates.
         * Unfortunately we sometimes seem to send moves and get no update
         * back, so we cap the number of 'dropped' moves.
         */
        private volatile int mouseMoved = 0;
        private volatile int mouseNotMoved = 0;

        private const int MOUSE_EVENTS_BEFORE_UPDATE = 2;
        private const int MOUSE_EVENTS_DROPPED = 5; // should this be proportional to bandwidth?

        private MouseEventArgs pending = null;
        private int pendingState = 0;
        private bool cursorOver = false;

        private int last_state = 0;

        private void mouseEvent(int state, int x, int y)
        {
            DoIfConnected(delegate()
            {
                if (talkingToVNCTerm && last_state == 4 && state == 0)
                {
                    ShowPopupMenu(x, y);
                }
                else if (this.scaling)
                {
                    this.vncStream.pointerEvent(state, (int)((x - dx) / this.scale), (int)((y - dy) / this.scale));
                }
                else
                {
                    this.vncStream.pointerEvent(state, x - (int)dx, y - (int)dy);
                }

                last_state = state;
            });
        }

        private void ShowPopupMenu(int x, int y)
        {
            ToolStripDropDownMenu popupMenu = new ToolStripDropDownMenu();

            ToolStripMenuItem copyItem = new ToolStripMenuItem(Messages.COPY);
#if !VNCControl
            copyItem.Image = XenAdmin.Properties.Resources.copy_16;
#endif
            copyItem.Click += copyItem_Click;
            popupMenu.Items.Add(copyItem);
            if (sourceVM != null && sourceVM.power_state == XenAPI.vm_power_state.Running)
            {
                ToolStripMenuItem pasteItem = new ToolStripMenuItem(Messages.PASTE);
#if !VNCControl
                pasteItem.Image = XenAdmin.Properties.Resources.paste_16;
#endif
                pasteItem.Click += pasteItem_Click;

                popupMenu.Items.Add(pasteItem);
            }
            popupMenu.Show(this, x, y);
        }

        void copyItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(talkingToVNCTerm);
            Program.AssertOnEventThread();
            if (clipboardStash != "")
            {
                Clip.SetClipboardText(clipboardStash);
            }
        }

        void pasteItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Assert(talkingToVNCTerm);
            Program.AssertOnEventThread();

            if (Clipboard.ContainsText())
            {
                vncStream.clientCutText(Clipboard.GetText().Replace("\r\n", "\n"));
                mouseEvent(2, 0, 0);
                mouseEvent(0, 0, 0);
            }
        }

        public void DisableMenuShortcuts()
        {
#if !VNCControl
            Program.MainWindow.MenuShortcuts = false;
#endif
        }

        public void EnableMenuShortcuts()
        {
#if !VNCControl
            Program.MainWindow.MenuShortcuts = true;
#endif
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnGotFocus(e);

            DisableMenuShortcuts();

            if (sendScanCodes)
            {
                InterceptKeys.grabKeys(this.keyScan, false);
            }

            if (updateClipboardOnFocus)
                setConsoleClipboard();

            /*Invalidate();
            Update();*/
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Program.AssertOnEventThread();
            base.OnLostFocus(e);

            EnableMenuShortcuts();

            InterceptKeys.releaseKeys();

            cursorOver = false;

            Cursor = Cursors.Default;

            //Release any held keys
            DoIfConnected(delegate()
            {
                foreach (Keys key in pressedKeys)
                {
                    // This won't release composite key events atm.
                    int sym = KeyMap.translateKey(ConsoleKeyHandler.GetSimpleKey(key));
                    if (sym > 0)
                        this.vncStream.keyCodeEvent(false, sym);
                }

                foreach (int key in pressedScans)
                {
                    this.vncStream.keyScanEvent(false, key);
                }
            });

            this.pressedKeys = new Set<Keys>();
            this.pressedScans = new Set<int>();

            /*Invalidate();
            Update();*/
        }

        /* We don't want this functionality anymore (see CA-39469 Console window loses focus when alt-tab-ing)
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
 
            // If the user alt-tabs to a different window, make sure when they come back that
            // focus is NOT given to the VNC control, since this is confusing.
            // Low-level intervention required: listen for the 'gain focus event'.
            // See http://msdn2.microsoft.com/en-us/library/ms646282.aspx and
            // http://www.pinvoke.net/default.aspx/Enums/WindowsMessages.html
            if (m.Msg == XenAdmin.Core.Win32.WM_SETFOCUS)
            {
                // m.WParam gives us the handle of the window that had focus immediately before
                // this control was granted focus. If it was from a window outside the app,
                // then it is set to zero (or so it seems
                if (m.WParam.ToInt64() == 0L)
                {
                    // Divert focus to a parent control
                    if (this.Parent.Parent != null)
                    {
                        // Always executed, fingers crossed
                        this.Parent.Parent.Focus();
                    }
                }
            }
        }*/

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Only hide the mouse when over the actual screen,
            // some parts of this control will be window blinds

            if (Focused && Connected)
            {
                int top = (int)dy;
                int left = (int)dx;
                int bottom = (int)((DesktopSize.Height * scale) + top);
                int right = (int)((DesktopSize.Width * scale) + left);

                if (e.X > left && e.X < right
                 && e.Y > top && e.Y < bottom)
                {
                    cursorOver = true;

                    if (RemoteCursor == null)
                        Cursor = LocalCursor.Cursor;
                    else
                        Cursor = RemoteCursor.Cursor;

                    lock (this.mouseEventLock)
                    {
                        if (this.mouseMoved < MOUSE_EVENTS_BEFORE_UPDATE)
                        {
                            this.mouseMoved++;

                            mouseEvent(currentMouseState, e.X, e.Y);
                        }
                        else if (this.mouseNotMoved > MOUSE_EVENTS_DROPPED)
                        {
                            this.mouseMoved = 0;
                            this.mouseNotMoved = 0;
                        }
                        else
                        {
                            this.mouseNotMoved++;

                            this.pendingState = currentMouseState;
                            this.pending = e;
                        }
                    }
                }
                else
                {
                    cursorOver = false;

                    Cursor = Cursors.Default;
                }
            }
        }

        /*
         * We're not going to buffer any clicks etc.
         * Don't forget to clear any pending moves.
         */
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //added this line to track mouse clicks for automatic switch to RDP
            //if there are any issues with focu start looking here
            base.OnMouseDown(e);
            this.Focus();

            this.pending = null;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    currentMouseState |= 1;
                    break;
                case MouseButtons.Right:
                    currentMouseState |= 4;
                    break;
                case MouseButtons.Middle:
                    currentMouseState |= 2;
                    break;
            }
            mouseEvent(currentMouseState, e.X, e.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.pending = null;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    currentMouseState &= ~1;
                    break;
                case MouseButtons.Right:
                    currentMouseState &= ~4;
                    break;
                case MouseButtons.Middle:
                    currentMouseState &= ~2;
                    break;
            }
            mouseEvent(currentMouseState, e.X, e.Y);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (Focused)
            {
                DoIfConnected(delegate()
                {
                    this.vncStream.pointerWheelEvent((int)((e.X - dx) / this.scale), (int)((e.Y - dy) / this.scale),
                                                     e.Delta * -SystemInformation.MouseWheelScrollLines / 120);
                });

            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Cursor = Cursors.Default;
            cursorOver = false;
            base.OnMouseLeave(e);
            this.currentMouseState = 0;
            this.mouseMoved = 0;
        }

        private Set<Keys> pressedKeys = new Set<Keys>();
        private Set<int> pressedScans = new Set<int>();

        private bool modifierKeyPressedAlone = false;

        protected override bool ProcessTabKey(bool forward)
        {
            /*if (sendScanCodes)
                return true;

            DoIfConnected((MethodInvoker)delegate()
            {
                this.Keysym(true, Keys.Tab);
                this.Keysym(false, Keys.Tab);
            });*/

            return true;
        }

        private void DoIfConnected(MethodInvoker methodInvoker)
        {
            if (connected)
            {
                try
                {
                    methodInvoker();
                }
                catch (System.IO.IOException)
                {
                    // The server's gone away -- that's fine.
                }
                catch (Exception exn)
                {
                    Log.Warn(exn, exn);
                    // Nothing more we can do with this.
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            bool down = ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN));

            //System.Console.WriteLine(keyData + " " + down);

            Keys key = keyData;

            if ((key & Keys.Control) == Keys.Control)
                key = key & ~Keys.Control;

            if ((key & Keys.Alt) == Keys.Alt)
                key = key & ~Keys.Alt;

            if ((key & Keys.Shift) == Keys.Shift)
                key = key & ~Keys.Shift;

            // use TranslateKeyMessage to identify if Left or Right modifier keys have been pressed/released
            Keys extKey = ConsoleKeyHandler.TranslateKeyMessage(msg);

            return Keysym(down, key, extKey);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            //added this line to track key presses for automatic switch to RDP
            //if there are any issues with focu start looking here
            base.OnKeyDown(e);
            //e.Handled = Keysym(true, e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // we cannot identify if Left or Right modifier keys have been pressed
            e.Handled = Keysym(false, e.KeyCode, e.KeyCode);
        }

        private bool altGrReleaseSent;

        private void SendAltGrKeysym(bool pressed)
        {
            Keysym_(pressed, Keys.ControlKey);
            Keysym_(pressed, Keys.Menu);
        }

        private void HandleAltGrKeysym(bool pressed, Keys key)
        {
            if (pressed)
            {
                if (pressedKeys.Count > 2 && key != Keys.ControlKey && key != Keys.Menu)
                {
                    bool isAltGrPressed = pressedKeys.Where(
                        extKey => ConsoleKeyHandler.GetSimpleKey(extKey) == Keys.ControlKey ||
                                  ConsoleKeyHandler.GetSimpleKey(extKey) == Keys.Menu).ToList().Count == 2;

                    if (isAltGrPressed &&
                        (KeyMap.IsMapped(key) && altGrReleaseSent || !KeyMap.IsMapped(key) && !altGrReleaseSent))
                    {
                        SendAltGrKeysym(altGrReleaseSent);
                        altGrReleaseSent = !altGrReleaseSent;
                    }
                }
            }
            else
            {
                if (key == Keys.ControlKey || key == Keys.Menu)
                    altGrReleaseSent = false;
            }
        }

        private bool Keysym(bool pressed, Keys key, Keys extendedKey)
        {
            if (sendScanCodes)
                return true;

            if (KeyHandler.handleExtras<Keys>(pressed, pressedKeys, KeyHandler.ExtraKeys, extendedKey, KeyHandler.ModifierKeys, ref modifierKeyPressedAlone))
            {
                if (!pressed && modifierKeyPressedAlone)
                {
                    // send key up anyway
                    modifierKeyPressedAlone = false;
                    return Keysym_(pressed, key);
                }
                this.Parent.Focus();
                return true;
            }

            HandleAltGrKeysym(pressed, key);

            // on keyup, try to remove extended keys (i.e. LControlKey, LControlKey, RShiftKey, LShiftKey, RMenu, LMenu)
            // we need to do this here, because we cannot otherwise distinguish between Left and Right modifier keys on KeyUp
            if (!pressed)
            {
                List<Keys> extendedKeys = ConsoleKeyHandler.GetExtendedKeys(key);
                foreach (var k in extendedKeys)
                {
                    pressedKeys.Remove(k);
                }
            }

            return Keysym_(pressed, key);
        }

        private bool Keysym_(bool pressed, Keys key)
        {
            int keysym = KeyMap.translateKey(key);

            if (keysym > 0)
            {
                DoIfConnected(delegate()
                {
                    this.vncStream.keyCodeEvent(pressed, keysym);
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        public void keyScan(bool pressed, int scanCode)
        {
            if (KeyHandler.handleExtras<int>(pressed, pressedScans, KeyHandler.ExtraScans, scanCode, KeyHandler.ModifierScans, ref modifierKeyPressedAlone))
            {
                if (!pressed && modifierKeyPressedAlone)
                {
                    // send key up anyway
                    modifierKeyPressedAlone = false;
                    keyScan_(pressed, scanCode);
                    return;
                }
                this.Focus();
                return;
            }

            keyScan_(pressed, scanCode);
        }

        private void keyScan_(bool pressed, int scanCode)
        {
            DoIfConnected(delegate()
            {
                this.vncStream.keyScanEvent(pressed, scanCode);
            });
        }

        public void Clear()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(Object o)
            {
                ClientFillRectangle(0, 0, DesktopSize.Width, DesktopSize.Height, Color.Black);
            }), null);

        }

        #region IRemoteConsole implementation
        
        public ConsoleKeyHandler KeyHandler { get; set; }

        public Control ConsoleControl
        {
            get { return this; }
        }

        public void Activate()
        {
            this.Select();
        }

        public void DisconnectAndDispose()
        {
            Disconnect();
            Dispose();
        }

        private bool helperIsPaused = true;

        public void Pause()
        {
            helperIsPaused = true;
            if (this.vncStream != null)
                this.vncStream.Pause();
        }

        public void Unpause()
        {
            helperIsPaused = false;
            if (this.vncStream != null)
                this.vncStream.Unpause();
        }

        public void SendCAD()
        {
            if (sendScanCodes)
            {
                keyScan_(true, ConsoleKeyHandler.CTRL_SCAN);
                keyScan_(true, ConsoleKeyHandler.ALT_SCAN);
                keyScan_(true, ConsoleKeyHandler.DEL_SCAN);

                keyScan_(false, ConsoleKeyHandler.CTRL_SCAN);
                keyScan_(false, ConsoleKeyHandler.ALT_SCAN);
                keyScan_(false, ConsoleKeyHandler.DEL_SCAN);
            }
            else
            {
                Keysym_(true, Keys.ControlKey);
                Keysym_(true, Keys.Menu);
                Keysym_(true, Keys.Delete);

                Keysym_(false, Keys.ControlKey);
                Keysym_(false, Keys.Menu);
                Keysym_(false, Keys.Delete);
            }
        }

        public Image Snapshot()
        {
            while (vncStream == null)
                Thread.Sleep(100);

            vncStream.Unpause(true); //request full update

            lock (vncStream.updateMonitor)
            {
                Monitor.Wait(vncStream.updateMonitor, 1000);
            }

            lock (backBuffer)
            {
                //if (!backBufferInteresting)
                //    return null;

                Image image = new Bitmap(backBuffer);
                //Graphics graphics = Graphics.FromImage(image);
                //setupGraphicsOptions(graphics);

                //graphics.DrawImage(backBuffer, 0, 0, x, y);
                //graphics.Dispose();

                return image;
            }
        }    

        public bool SendScanCodes
        {
            set
            {
                Program.AssertOnEventThread();
                SetSendScanCodes(value);
            }
        }

        private void SetSendScanCodes(bool newSendScanCodes)
        {
            Program.AssertOnEventThread();
            if (sendScanCodes == newSendScanCodes)
                return;

            Log.InfoFormat("VNCGraphicsClient.SetSendScanCodes newSendScanCodes={0}", newSendScanCodes);

            if (!newSendScanCodes)
            {
                InterceptKeys.releaseKeys();
            }
            else if (Focused)
            {
                InterceptKeys.grabKeys(this.keyScan, false);
            }

            sendScanCodes = newSendScanCodes;
        }

        public bool Scaling
        {
            get
            {
                Program.AssertOnEventThread();
                return this.scaling;
            }
            set
            {
                Program.AssertOnEventThread();

                if (scaling == value)
                    return;

                // it going to scaling, quickly save the old dx, dy values
                if (value)
                {
                    this.oldDx = dx;
                    this.oldDy = dy;
                }

                lock (this.backBuffer)
                {
                    this.scaling = value;
                    this.SetupScaling();
                }
                this.Invalidate();
            }
        }
        /// <summary>
        /// Invoke holding backBuffer lock
        /// </summary>
        private void SetupScaling()
        {
            Program.AssertOnEventThread();

            if (frontGraphics == null)
                return;

            if (this.scaling)
            {
                this.AutoScroll = false;

                float xScale = this.Size.Width /
                    (float)(displayBorder ? this.DesktopSize.Width + BORDER_PADDING + BORDER_PADDING : this.DesktopSize.Width);
                float yScale = this.Size.Height /
                    (float)(displayBorder ? this.DesktopSize.Height + BORDER_PADDING + BORDER_PADDING : this.DesktopSize.Height);

                scale = (xScale > yScale) ? yScale : xScale;
                scale = (scale > 0.01) ? scale : (float)0.01;

                Bump = (int)Math.Ceiling(1 / scale);

                // Now do the offset

                dx = (this.Size.Width - (this.DesktopSize.Width * scale)) / 2;
                dy = (this.Size.Height - (this.DesktopSize.Height * scale)) / 2;

                Matrix transform = new Matrix();
                transform.Translate(dx, dy);
                transform.Scale(scale, scale);

                this.frontGraphics.Transform = transform;
            }
            else
            {
                this.scale = 1;
                this.Bump = 0;

                if (this.connected)
                {
                    this.AutoScrollMinSize = new Size(
                        displayBorder ? DesktopSize.Width + BORDER_PADDING + BORDER_PADDING : DesktopSize.Width,
                        displayBorder ? DesktopSize.Height + BORDER_PADDING + BORDER_PADDING : DesktopSize.Height);
                }
                else
                {
                    this.AutoScrollMinSize = new Size(0, 0);
                }

                // The change of AutoScrollMinSize can trigger a resize event, which in turn can trigger
                // scaling to be turned off.  If this happens, restart this calculation altogether.
                if (scaling)
                {
                    SetupScaling();
                    return;
                }

                this.AutoScroll = true;

                if (this.Size.Height >= (displayBorder ? this.DesktopSize.Height + BORDER_PADDING + BORDER_PADDING : DesktopSize.Height))
                {
                    this.dy = (this.Size.Height - this.DesktopSize.Height) / 2;
                }
                else
                {
                    if (displayBorder)
                    {
                        this.dy = BORDER_PADDING;
                        this.AutoScrollPosition = new Point(BORDER_PADDING, (int)this.oldDy);
                    }
                    else
                    {
                        this.dy = 0;
                        this.AutoScrollPosition = new Point(0, (int)this.oldDy);
                    }
                }

                if (this.Size.Width >= (displayBorder ? this.DesktopSize.Width + BORDER_PADDING + BORDER_PADDING : DesktopSize.Width))
                {
                    this.dx = (this.Size.Width - this.DesktopSize.Width) / 2;
                }
                else
                {
                    if (displayBorder)
                    {
                        this.dx = BORDER_PADDING;
                        this.AutoScrollPosition = new Point((int)this.oldDx, BORDER_PADDING);
                    }
                    else
                    {
                        this.dx = 0;
                        this.AutoScrollPosition = new Point((int)this.oldDx, 0);
                    }
                }

                Matrix transform = new Matrix();
                transform.Translate(dx, dy);

                this.frontGraphics.Transform = transform;
            }
        }

        private bool displayBorder = true;

        /// <summary>
        /// Whether or not to display the blue rectangle around the control when it has focus.
        /// </summary>
        public bool DisplayBorder
        {
            set
            {
                displayBorder = value;
                lock (backBuffer)
                {
                    SetupScaling();
                }
                this.Invalidate();
                this.Update();
            }
        }

        public Size DesktopSize { get; set; }

        public Rectangle ConsoleBounds
        {
            get
            {
                return scaling ? new Rectangle((int)dx, (int)dy, Size.Width - (2 * (int)dx), Size.Height - (2 * (int)dy))
                    : new Rectangle((int)dx, (int)dy, DesktopSize.Width, DesktopSize.Height);
            }
        }

        #endregion
    }
}
