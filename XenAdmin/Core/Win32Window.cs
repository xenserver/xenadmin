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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace XenAdmin.Core
{
    /// <summary>
    /// Encapsulates window functions that aren't in the framework.
    /// </summary>
    internal class Win32Window : IWin32Window
    {
        private readonly IntPtr _handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Win32Window"/> class.
        /// </summary>
        /// <param name="handle">The window handle.</param>
        public Win32Window(IntPtr handle)
        {
            _handle = handle;
        }

        /// <summary>
        /// Extract the window handle 
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return _handle;
            }
        }

        /// <summary>
        /// Posts WM_CLOSE.
        /// </summary>
        public void Close()
        {
            const int WM_CLOSE = 0x10;
            PostMessage(WM_CLOSE, 0, 0);
        }

        /// <summary>
        /// Return true if this window is null
        /// </summary>
        public bool IsNull
        {
            get
            {
                return _handle == IntPtr.Zero;
            }
        }

        /// <summary>
        /// The children of this window
        /// </summary>
        public ReadOnlyCollection<Win32Window> Children
        {
            get
            {
                List<Win32Window> windows = new List<Win32Window>();
                EnumChildWindows(_handle, delegate(IntPtr window, int i)
                {
                    windows.Add(new Win32Window(window));
                    return true;
                }, 0);
                return new ReadOnlyCollection<Win32Window>(windows);
            }
        }

        public ReadOnlyCollection<Win32Window> DirectChildren
        {
            get
            {
                List<Win32Window> windows = new List<Win32Window>();
                foreach (Win32Window w in Children)
                {
                    if (w.Parent._handle.ToInt32() == _handle.ToInt32())
                    {
                        windows.Add(w);
                    }
                }
                return new ReadOnlyCollection<Win32Window>(windows);
            }
        }

        /// <summary>
        /// Gets the first window that has the specified window text.
        /// </summary>
        /// <param name="text">The text that the window must have.</param>
        /// <returns>The first window that has the specified window text.</returns>
        public static Win32Window GetWindowWithText(string text)
        {
            return GetWindowWithText(text, w => true);
        }

        /// <summary>
        /// Gets the first window that has the specified window text and matches the specified Predicate.
        /// </summary>
        /// <param name="text">The text that the window must have.</param>
        /// <param name="match">The Predicate that the window must match.</param>
        /// <returns>The first window that has the specified window text and matches the specified Predicate.</returns>
        public static Win32Window GetWindowWithText(string text, Predicate<Win32Window> match)
        {
            Util.ThrowIfParameterNull(match, "match");
            Thread.Sleep(100);
            foreach (Win32Window window in TopLevelWindows)
            {
                if (window.Text == text && match(window))
                {
                    return window;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a value indicating whether a form of the specified type is currently visible in this process.
        /// </summary>
        public static bool ModalDialogIsVisible()
        {
            int currentProcess = Process.GetCurrentProcess().Id;
            IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
            bool output = false;

            EnumWindows(delegate(IntPtr handle, int i)
            {
                Win32Window w = new Win32Window(handle);
                if (w.ProcessId == currentProcess)
                {
                    Form f = Form.FromHandle(w.Handle) as Form;

                    if (f != null && w.Handle != mainWindowHandle && f.GetType().Namespace.StartsWith("XenAdmin"))
                    {
                        output = true;
                        return false;
                    }
                }
                return true;
            }, 0);
            
            return output;
        }

        /// <summary>
        /// All top level windows 
        /// </summary>
        public static ReadOnlyCollection<Win32Window> TopLevelWindows
        {
            get
            {
                List<Win32Window> windows = new List<Win32Window>();

                EnumWindows(delegate(IntPtr handle, int i)
                {
                    windows.Add(new Win32Window(handle));
                    return true;

                }, 0);
                return new ReadOnlyCollection<Win32Window>(windows);
            }
        }

        /// <summary>
        /// Return all windows of the current thread.
        /// </summary>
        public static ReadOnlyCollection<Win32Window> GetThreadWindows()
        {
            return GetThreadWindows(GetCurrentThreadId());
        }

        /// <summary>
        /// Return all windows of a given thread
        /// </summary>
        /// <param name="threadId">The thread id</param>
        public static ReadOnlyCollection<Win32Window> GetThreadWindows(int threadId)
        {
            List<Win32Window> windows = new List<Win32Window>();

            EnumThreadWindows(threadId, delegate(IntPtr window, int i)
            {
                windows.Add(new Win32Window(window));
                return true;
            }, 0);
            return new ReadOnlyCollection<Win32Window>(windows);
        }

        /// <summary>
        /// The deskop window
        /// </summary>
        public static Win32Window DesktopWindow
        {
            get
            {
                return new Win32Window(GetDesktopWindow());
            }
        }

        /// <summary>
        /// The current foreground window
        /// </summary>
        public static Win32Window ForegroundWindow
        {
            get
            {
                return new Win32Window(GetForegroundWindow());
            }
        }

        /// <summary>
        /// Bring a window to the top
        /// </summary>
        public void BringWindowToTop()
        {
            BringWindowToTop(_handle);
        }

        public void SetForegroundWindow()
        {
            SetForegroundWindow(_handle);
        }

        public void OpenIcon()
        {
            OpenIcon(_handle);
        }

        /// <summary>
        /// Find a child of this window
        /// </summary>
        /// <param name="className">Name of the class, or null</param>
        /// <param name="windowName">Name of the window, or null</param>
        /// <returns></returns>
        public Win32Window FindChild(string className, string windowName)
        {
            return new Win32Window(FindWindowEx(_handle, IntPtr.Zero, className, windowName));
        }

        /// <summary>
        /// Find a window by name or class
        /// </summary>
        /// <param name="className">Name of the class, or null</param>
        /// <param name="windowName">Name of the window, or null</param>
        /// <returns></returns>
        public static Win32Window FindWindow(string className, string windowName)
        {
            return new Win32Window(FindWindowEx(IntPtr.Zero, IntPtr.Zero, className, windowName));
        }

        /// <summary>
        /// Tests whether one window is a child of another
        /// </summary>
        /// <param name="parent">Parent window</param>
        /// <param name="window">Window to test</param>
        /// <returns></returns>
        public static bool IsChild(Win32Window parent, Win32Window window)
        {
            return IsChild(parent._handle, window._handle);
        }

        /// <summary>
        /// Send a windows message to this window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>
        public int SendMessage(int message, int wparam, int lparam)
        {
            return SendMessage(_handle, message, wparam, lparam);
        }

        /// <summary>
        /// Post a windows message to this window
        /// </summary>
        /// <param name="message"></param>
        /// <param name="wparam"></param>
        /// <param name="lparam"></param>
        /// <returns></returns>
        public int PostMessage(int message, int wparam, int lparam)
        {
            return PostMessage(_handle, message, wparam, lparam);
        }

        /// <summary>
        /// Get the parent of this window. Null if this is a top-level window
        /// </summary>
        public Win32Window Parent
        {
            get
            {
                return new Win32Window(GetParent(_handle));
            }
        }

        /// <summary>
        /// Get the last (topmost) active popup
        /// </summary>
        public Win32Window LastActivePopup
        {
            get
            {
                IntPtr popup = GetLastActivePopup(_handle);
                if (popup == _handle)
                    return new Win32Window(IntPtr.Zero);
                else
                    return new Win32Window(popup);
            }
        }

        /// <summary>
        /// The text in this window
        /// </summary>
        public string Text
        {
            get
            {
                int length = GetWindowTextLength(_handle);
                StringBuilder sb = new StringBuilder(length + 1);
                GetWindowText(_handle, sb, sb.Capacity);
                return sb.ToString();
            }
            set
            {
                SetWindowText(_handle, value);
            }
        }

        public string WindowClass
        {
            get
            {
                StringBuilder sb = new StringBuilder(1000);
                GetClassName(_handle, sb, sb.Capacity);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Get a long value for this window. See GetWindowLong()
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetWindowLong(int index)
        {
            return GetWindowLong(_handle, index);
        }

        /// <summary>
        /// Set a long value for this window. See SetWindowLong()
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int SetWindowLong(int index, int value)
        {
            return SetWindowLong(_handle, index, value);
        }

        /// <summary>
        /// The id of the thread that owns this window
        /// </summary>
        public int ThreadId
        {
            get
            {
                return GetWindowThreadProcessId(_handle, IntPtr.Zero);
            }
        }

        /// <summary>
        /// The id of the process that owns this window
        /// </summary>
        public int ProcessId
        {
            get
            {
                int processId = 0;
                GetWindowThreadProcessId(_handle, ref processId);
                return processId;
            }
        }

        /// <summary>
        /// Whether the window is minimized
        /// </summary>
        public bool Minimized
        {
            get
            {
                return IsIconic(_handle);
            }
            set
            {
                const int SC_MINIMIZE = 0xF020;
                const int WM_SYSCOMMAND = 0x0112;
                PostMessage(WM_SYSCOMMAND, SC_MINIMIZE, 0);
            }
        }

        /// <summary>
        /// Whether the window is maximized
        /// </summary>
        public bool Maximized
        {
            get
            {
                return IsZoomed(_handle);
            }
        }

        public bool MoveWindow(int X, int Y, int width, int height, bool repaint)
        {
            return MoveWindow(_handle, X, Y, width, height, repaint);
        }

        public Win32Window NextWindow
        {
            get
            {
                IntPtr h = GetNextWindow(_handle, 2);
                if (h == IntPtr.Zero)
                    return null;
                return new Win32Window(h);
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are pending message in the message queue.
        /// </summary>
        /// <value><c>true</c> if the are messages pending; otherwise, <c>false</c>.</value>
        public bool MessagesPending
        {
            get
            {
                NativeMessage msg = new NativeMessage();
                return PeekMessage(out msg, _handle, 0, 0, 0);
            }
        }

        /// <summary>
        /// Turn this window into a tool window, so it doesn't show up in the Alt-tab list...
        /// </summary>
        /// 
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        public void MakeToolWindow()
        {
            int windowStyle = GetWindowLong(GWL_EXSTYLE);
            SetWindowLong(GWL_EXSTYLE, windowStyle | WS_EX_TOOLWINDOW);
        }

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr window);

        [DllImport("user32.dll")]
        private static extern bool CloseWindow(IntPtr window);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr window);

        [DllImport("user32.dll")]
        private static extern bool OpenIcon(IntPtr window);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string windowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindowWin32(string className, string windowName);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr window, int message, int wparam, int lparam);

        [DllImport("user32.dll")]
        private static extern int PostMessage(IntPtr window, int message, int wparam, int lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr window);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetLastActivePopup(IntPtr window);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr window, [In][Out] StringBuilder text, int copyCount);

        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr window, string text);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr window);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr window, int index);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr window, [In][Out] StringBuilder text, int copyCount);

        private delegate bool EnumWindowsProc(IntPtr window, int i);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, int i);

        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int threadId, EnumWindowsProc callback, int i);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc callback, int i);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr window, ref int processId);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr window, IntPtr ptr);

        [DllImport("user32.dll")]
        private static extern bool IsChild(IntPtr parent, IntPtr window);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr window);

        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr window);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int width, int height, bool repaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindow", SetLastError = true)]
        private static extern IntPtr GetNextWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int wFlag);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern int GetCurrentThreadId();

        /// <summary>
        /// The PeekMessage function dispatches incoming sent messages, checks the thread message queue for a posted message, 
        /// and retrieves the message (if any exist).
        /// </summary>
        /// <param name="message">Pointer to an MSG structure that receives message information.</param>
        /// <param name="handle">Handle to the window whose messages are to be retrieved. The window must belong to the current thread.</param>
        /// <param name="filterMin">Specifies the value of the first message in the range of messages to be examined.</param>
        /// <param name="filterMax">Specifies the value of the last message in the range of messages to be examined.</param>
        /// <param name="removeMsg">Specifies how messages are handled.</param>
        /// <returns>If a message is available, the return value is nonzero. If no messages are available, the return value is zero.</returns>
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool PeekMessage(out NativeMessage message, IntPtr handle, uint filterMin, uint filterMax, uint removeMsg);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

    }
}
