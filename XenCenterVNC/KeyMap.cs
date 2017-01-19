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
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Resources;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

using XenAdmin.Core;

namespace DotNetVnc
{
    /// <summary>
    /// Translates C# Keys (i.e. virtual key codes) into X11 keysyms.
    /// </summary>
    public class KeyMap
    {
        private static Dictionary<Keys, int> map = new Dictionary<Keys, int>();

        static KeyMap()
        {
            ResourceManager resources = new ResourceManager("DotNetVnc.KeyMap", typeof(KeyMap).Assembly);
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                int sym = parse_keysym(resources.GetString(Enum.GetName(typeof(Keys), key)));
                if (sym != -1)
                    map[key] = sym;
            }
        }

        private static int parse_keysym(string s)
        {
            if (s != null)
            {
                NumberStyles style;

                if (s.StartsWith("0x") || s.StartsWith("0X"))
                {
                    s = s.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                else
                {
                    style = NumberStyles.Integer;
                }

                int keysym;
                if (int.TryParse(s, style, null, out keysym))
                {
                    return keysym;
                }
            }
            return -1;
        }

        public static int translateKey(Keys key)
        {
            return IsMapped(key) ? map[key] : UnicodeOfKey(key);
        }

        public static bool IsMapped(Keys key)
        {
            return map.ContainsKey(key);
        }

        private static IntPtr keyboard_state = Marshal.AllocHGlobal(256);
        private static StringBuilder char_buffer = new StringBuilder(Win32.TO_UNICODE_BUFFER_SIZE);

        private static int UnicodeOfKey(Keys key)
        {
            try
            {
                Win32.GetKeyboardState(keyboard_state);
                int n = Win32.ToUnicode((uint)key, 0, keyboard_state, char_buffer, Win32.TO_UNICODE_BUFFER_SIZE, 0);

                if (n == 1)
                {
                    int k = char_buffer[0];

                    if (k < 0x20)
                    {
                        return k + 0x60;
                    }
                    else
                    {
                        return k;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
        }
    }

    public unsafe class InterceptKeys
    {
        private static bool bubble = false;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int FLAG_EXTENDED = 0x01;

        private delegate int LowLevelKeyboardProc(
            int nCode, int wParam, KBDLLHOOKSTRUCT* lParam);

        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public delegate void KeyEvent(bool down, int scancode);
        private static KeyEvent keyEvent = null;

#pragma warning disable 0649
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
#pragma warning restore 0649

        public static void grabKeys(KeyEvent keyEvent, bool bubble)
        {
            InterceptKeys.bubble = bubble;

            if (InterceptKeys.keyEvent == null)
            {
                InterceptKeys.keyEvent = keyEvent;
                _hookID = SetHook(_proc);
            }
        }

        public static void releaseKeys()
        {
            if (InterceptKeys.keyEvent != null)
            {
                InterceptKeys.keyEvent = null;
                UnhookWindowsHookEx(_hookID);
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    Win32.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private const int NUM_LOCK_SCAN = 197;

        private static int HookCallback(int nCode, int wParam, KBDLLHOOKSTRUCT* lParam)
        {
             if (nCode < 0)
            {
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
            else
            {
                KBDLLHOOKSTRUCT kbStruct = *lParam;

                bool extended = (kbStruct.flags & FLAG_EXTENDED) == 0;
                bool down = (wParam == WM_KEYDOWN) || (wParam == WM_SYSKEYDOWN);
                int scanCode = kbStruct.scanCode;

                switch (scanCode)
                {
                    case 54:
                        break;
                    default:
                        scanCode += (extended ? 0 : 128);
                        break;
                }

                if (InterceptKeys.keyEvent != null)
                {
                    InterceptKeys.keyEvent(down, scanCode);
                }

                if (bubble || scanCode == NUM_LOCK_SCAN)
                {
                    return CallNextHookEx(_hookID, nCode, wParam, lParam);
                }
                else
                {
                    return 1; // Prevent the message being passed on.
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode,
            int wParam, KBDLLHOOKSTRUCT * lParam);

    }
}
