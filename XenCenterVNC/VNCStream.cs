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
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using XenAdmin.Core;

namespace DotNetVnc
{
    public class VNCStream
    {
        private const int THUMBNAIL_SLEEP_TIME = 500;

        private const int RAW_ENCODING = 0;
        private const int COPY_RECTANGLE_ENCODING = 1;
        private const int RRE_ENCODING = 2;
        private const int CORRE_ENCODING = 4;
        private const int HEXTILE_ENCODING = 5;
        private const int CURSOR_PSEUDO_ENCODING = -239;
        private const int DESKTOP_SIZE_PSEUDO_ENCODING = -223;
        private const int XENCENTER_ENCODING = -254;

        private const int SET_PIXEL_FORMAT = 0;
        private const int SET_ENCODINGS = 2;
        private const int FRAMEBUFFER_UPDATE_REQUEST = 3;
        private const int KEY_EVENT = 4;
        private const int KEY_SCAN_EVENT = 254;
        private const int POINTER_EVENT = 5;
        private const int CLIENT_CUT_TEXT = 6;

        private const int RAW_SUBENCODING = 1;
        private const int BACKGROUND_SPECIFIED_SUBENCODING = 2;
        private const int FOREGROUND_SPECIFIED_SUBENCODING = 4;
        private const int ANY_SUBRECTS_SUBENCODING = 8;
        private const int SUBRECTS_COLORED_SUBENCODING = 16;

        private const int FRAME_BUFFER_UPDATE = 0;
        private const int BELL = 2;
        private const int SERVER_CUT_TEXT = 3;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Thread thread = null;

        /**
         * Current color properties
         */
        private int bitsPerPixel;
        private int bytesPerPixel;
        private int depth;
        private bool bigEndian;
        private bool trueColor;
        private int redMax;
        private int greenMax;
        private int blueMax;
        private int redMaxPlus1;
        private int greenMaxPlus1;
        private int blueMaxPlus1;
        private int redMaxOver2;
        private int greenMaxOver2;
        private int blueMaxOver2;
        private int redShift;
        private int greenShift;
        private int blueShift;
        private bool rgb565;

        /// <summary>
        /// This event will be fired when an error occurs.  The helper thread is guaranteed to be
        /// closing down at this point.
        /// </summary>
        public event XenAdmin.ExceptionEventHandler ErrorOccurred = null;

        public event EventHandler ConnectionSuccess = null;

        /**
         * The encodings used.  Note that these are ordered: preferred encoding first.
         */
        private static readonly int[] encodings = new int[] {
            //HEXTILE_ENCODING,
            CORRE_ENCODING,
            RRE_ENCODING,
	        COPY_RECTANGLE_ENCODING,
	        RAW_ENCODING,
	        CURSOR_PSEUDO_ENCODING,
	        DESKTOP_SIZE_PSEUDO_ENCODING,
            XENCENTER_ENCODING
	    };

        private readonly IVNCGraphicsClient client;

        private readonly MyStream stream;

        private readonly Object writeLock = new Object();
        private Object pauseMonitor = new Object();

        private volatile bool running = true;

        private int width;
        private int height;

        private bool incremental;

        private PixelFormat pixelFormat;
        private PixelFormat pixelFormatCursor;

        private byte[] data = new byte[1228800]; //640*480*32bpp
        private byte[] data_8bpp = null;

        private readonly long imageUpdateThreshold;

        /*
        public struct StatsEntry
        {
            public double time;
            public int size;
        }
        */

        public VNCStream(IVNCGraphicsClient client, Stream stream, bool startPaused)
        {
            this.client = client;
            this.stream = new MyStream(stream);
            this.paused = startPaused;

            long freq;
            if (!Win32.QueryPerformanceFrequency(out freq))
            {
                System.Diagnostics.Trace.Assert(false);
            }
            imageUpdateThreshold = freq / 3;
        }

        public void connect(char[] password)
        {
            System.Diagnostics.Trace.Assert(thread == null);

            thread = new Thread(this.run);
            thread.Name = String.Format("VNC connection to {0} - {1}", client.VmName, client.UUID);
            thread.IsBackground = true;
            thread.Start(password);
        }

        private ProtocolVersion getProtocolVersion()
        {
            byte[] buffer = new byte[12];
            this.stream.readFully(buffer, 0, 12);
            char[] chars = new char[12];
            Encoding.ASCII.GetDecoder().GetChars(buffer, 0, 12, chars, 0);
            String s = new String(chars);
            Regex regex = new Regex("RFB ([0-9]{3})\\.([0-9]{3})\n");
            Match match = regex.Match(s);
            if (!match.Success)
            {
                throw new VNCException("expected protocol version: " + s);
            }

            return new ProtocolVersion(Int32.Parse(match.Groups[1].Value),
                                       Int32.Parse(match.Groups[2].Value));
        }

        private void sendProtocolVersion()
        {
            lock (this.writeLock)
            {
                byte[] bytes = Encoding.ASCII.GetBytes("RFB 003.003\n");
                this.stream.Write(bytes, 0, bytes.Length);
                this.stream.Flush();
            }
        }

        private void readPixelFormat()
        {
            this.bitsPerPixel = this.stream.readCard8();
            this.depth = this.stream.readCard8();
            this.bigEndian = this.stream.readFlag();
            this.trueColor = this.stream.readFlag();
            this.redMax = this.stream.readCard16();
            this.greenMax = this.stream.readCard16();
            this.blueMax = this.stream.readCard16();
            this.redShift = this.stream.readCard8();
            this.greenShift = this.stream.readCard8();
            this.blueShift = this.stream.readCard8();
            this.stream.readPadding(3);
            Log.Debug("readPixelFormat " + this.bitsPerPixel +
                         " " + this.depth);
        }

        private void writePixelFormat()
        {
            Log.Debug("writePixelFormat " + this.bitsPerPixel +
                         " " + this.depth);
            this.stream.writeInt8(SET_PIXEL_FORMAT);
            this.stream.writePadding(3);

            this.stream.writeInt8(this.bitsPerPixel);
            this.stream.writeInt8(this.depth);
            this.stream.writeFlag(this.bigEndian);
            this.stream.writeFlag(this.trueColor);
            this.stream.writeInt16(this.redMax);
            this.stream.writeInt16(this.greenMax);
            this.stream.writeInt16(this.blueMax);
            this.stream.writeInt8(this.redShift);
            this.stream.writeInt8(this.greenShift);
            this.stream.writeInt8(this.blueShift);

            this.stream.writePadding(3);
        }

        private void force32bpp()
        {
            Log.Debug("force32bpp()");

            this.bitsPerPixel = 32;
            this.depth = 24;
            this.trueColor = true;
            this.redMax = 255;
            this.greenMax = 255;
            this.blueMax = 255;
            this.redShift = 16;
            this.greenShift = 8;
            this.blueShift = 0;

            // Note that we keep the endian value from the server.

            setupPixelFormat();

            lock (this.writeLock)
            {
                writePixelFormat();
            }
        }


        private void setupPixelFormat()
        {
            Log.Debug("setupPixelFormat(" + bitsPerPixel + ")");

            bytesPerPixel = bitsPerPixel >> 3;

            redMaxPlus1 = redMax + 1;
            greenMaxPlus1 = greenMax + 1;
            blueMaxPlus1 = blueMax + 1;

            redMaxOver2 = redMax >> 1;
            greenMaxOver2 = greenMax >> 1;
            blueMaxOver2 = blueMax >> 1;

            if (bitsPerPixel == 32 || bitsPerPixel == 8)
            {
                pixelFormat = PixelFormat.Format32bppRgb;
                pixelFormatCursor = PixelFormat.Format32bppArgb;
            }
            else if (bitsPerPixel == 16)
            {
                rgb565 = redShift == 11;
                pixelFormat =
                    rgb565 ?
                        PixelFormat.Format16bppRgb565 :
                        PixelFormat.Format16bppRgb555;
                pixelFormatCursor = PixelFormat.Format16bppArgb1555;
            }
            else
            {
                throw new IOException("unexpected bits per pixel: " + bitsPerPixel);
            }
        }

        private void writeSetEncodings()
        {
            Log.Debug("writeSetEncodings");
            this.stream.writeInt8(SET_ENCODINGS);
            this.stream.writePadding(1);
            this.stream.writeInt16(encodings.Length);
            for (int i = 0; i < encodings.Length; ++i)
            {
                this.stream.writeInt32(encodings[i]);
            }
        }

        private void writeFramebufferUpdateRequest(
            int x, int y, int width, int height, bool incremental
        )
        {
            this.stream.writeInt8(FRAMEBUFFER_UPDATE_REQUEST);
            this.stream.writeFlag(incremental);
            this.stream.writeInt16(x);
            this.stream.writeInt16(y);
            this.stream.writeInt16(width);
            this.stream.writeInt16(height);
        }

        private void handshake()
        {
            ProtocolVersion protocolVersion = getProtocolVersion();
            if (protocolVersion.major < 3)
            {
                throw new VNCException(
                    "don't know protocol version " + protocolVersion.major
                 );
            }
        }

        private void authenticationExchange(char[] password)
        {
            Log.Debug("authenticationExchange");

            int scheme = this.stream.readCard32();
            if (scheme == 0)
            {
                String reason = this.stream.readString();
                throw new VNCException("connection failed: " + reason);
            }
            else if (scheme == 1)
            {
                // no authentication needed
            }
            else if (scheme == 2)
            {
                PasswordAuthentication(password);
            }
            else
            {
                throw new VNCException(
                    "unexpected authentication scheme: " + scheme
                );
            }
        }

        private void PasswordAuthentication(char[] password)
        {
            byte[] keyBytes = new byte[8];
            for (int i = 0; (i < 8) && (i < password.Length); ++i)
            {
                keyBytes[i] = reverse((byte)password[i]);
            }

            DESCryptoServiceProvider des =
                new DESCryptoServiceProvider();

            des.Padding = PaddingMode.None;
            des.Mode = CipherMode.ECB;

            ICryptoTransform chiper =
                des.CreateEncryptor(keyBytes, null);

            byte[] challenge = new byte[16];
            this.stream.readFully(challenge, 0, 16);

            byte[] response = chiper.TransformFinalBlock(challenge, 0, 16);

            this.stream.Write(response, 0, 16);
            this.stream.Flush();

            int status = this.stream.readCard32();

            if (status == 0)
            {
                // ok
            }
            else if (status == 1 || status == 2)
            {
                throw new VNCAuthenticationException();
            }
            else
            {
                throw new VNCException("Bad Authentication Response");
            }
        }

        private static byte reverse(byte v)
        {
            byte r = 0;
            if ((v & 0x01) != 0) r |= 0x80;
            if ((v & 0x02) != 0) r |= 0x40;
            if ((v & 0x04) != 0) r |= 0x20;
            if ((v & 0x08) != 0) r |= 0x10;
            if ((v & 0x10) != 0) r |= 0x08;
            if ((v & 0x20) != 0) r |= 0x04;
            if ((v & 0x40) != 0) r |= 0x02;
            if ((v & 0x80) != 0) r |= 0x01;
            return r;
        }

        private void clientInitialization()
        {
            Log.Debug("clientInitialisation");
            lock (this.writeLock)
            {
                this.stream.writeFlag(true); // shared
                this.stream.Flush();
            }
        }

        private void serverInitialization()
        {
            Log.Debug("serverInitialisation");
            int width = this.stream.readCard16();
            int height = this.stream.readCard16();

            readPixelFormat();

            stream.readString(); /* The desktop name -- we don't care. */

            if (trueColor)
            {
                setupPixelFormat();
                lock (writeLock)
                {
                    writePixelFormat();
                }
            }
            else
            {
                force32bpp();
            }

            desktopSize(width, height);

            lock (this.writeLock)
            {
                writeSetEncodings();
            }
        }

        /**
         * Expects to be lock on writeLock.
         */
        private void writeKey(int command, bool down, int key)
        {
            this.stream.writeInt8(command); //Send Scancodes
            this.stream.writeFlag(down);
            this.stream.writePadding(2);
            this.stream.writeInt32(key);
        }

        public void keyScanEvent(bool down, int key)
        {
            lock (this.writeLock)
            {
                try
                {
                    writeKey(KEY_SCAN_EVENT, down, key);
                    this.stream.Flush();
                }
                catch (IOException e)
                {
                    Log.Warn(e, e);
                }
            }
        }

        public void keyCodeEvent(bool down, int key)
        {
            lock (this.writeLock)
            {
                try
                {
                    writeKey(KEY_EVENT, down, key);
                    this.stream.Flush();
                }
                catch (IOException e)
                {
                    Log.Warn(e, e);
                }
            }
        }

        public void pointerEvent(int buttonMask, int x, int y)
        {
            if (x < 0)
            {
                x = 0;
            }
            else if (x >= width)
            {
                x = width - 1;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y >= height)
            {
                y = height - 1;
            }

            lock (this.writeLock)
            {
                try
                {
                    pointerEvent_(buttonMask, x, y);
                    this.stream.Flush();
                }
                catch (IOException e)
                {
                    Log.Warn(e, e);
                }
            }
        }

        public void pointerWheelEvent(int x, int y, int r)
        {
            lock (this.writeLock)
            {
                try
                {
                    /*
                      The RFB protocol specifies a down-up pair for each
                      scroll of the wheel, on button 4 for scrolling up, and
                      button 5 for scrolling down.
                    */

                    int m;

                    if (r < 0)
                    {
                        r = -r;
                        m = 8;
                    }
                    else
                    {
                        m = 16;
                    }
                    for (int i = 0; i < r; i++)
                    {
                        pointerEvent_(m, x, y);
                        pointerEvent_(0, x, y);
                    }

                    this.stream.Flush();
                }
                catch (IOException e)
                {
                    Log.Warn(e, e);
                }
            }
        }


        private void pointerEvent_(int buttonMask, int x, int y)
        {
            this.stream.writeInt8(POINTER_EVENT);
            this.stream.writeInt8(buttonMask);
            this.stream.writeInt16(x);
            this.stream.writeInt16(y);
        }


        public void clientCutText(String text)
        {
            Log.Debug("cutEvent");

            lock (this.writeLock)
            {
                try
                {
                    this.stream.writeInt8(CLIENT_CUT_TEXT);
                    this.stream.writePadding(3);
                    this.stream.writeString(text);
                    this.stream.Flush();
                }
                catch (IOException e)
                {
                    Log.Warn(e, e);
                }
            }
        }

        /// <summary>
        /// Creates an image in place in this.data.  It expects the pixel data to already be in this.data.  
        /// </summary>
        /// <param name="start">the start of image data in this.data</param>
        /// <param name="length">the length of image data in this data</param>
        /// <param name="mask_length">length of cursor mask (after the image in this.data), as specified by
        /// the RFB protocol specification for the Cursor pseudo-encoding (1-bpp, packed). If 0,
        /// the mask is assumed to be totally opaque (as used by normal "raw" packets). Masks are not
        /// supported for 8-bpp images.</param>
        private void createImage(int width, int height, int x, int y, int start, int length, int mask_length, bool cursor)
        {
            if (width == 0 || height == 0)
                return;

            byte[] data_to_render;
            int stride;

            if (bitsPerPixel == 32)
            {
                stride = width * 4;
                data_to_render = data;

                System.Diagnostics.Debug.Assert(length == height * stride);

                if (cursor)
                {
                    // for mask
                    int j = 0; // bit within the current byte (k)
                    int k = start + length; //byte
                    int m = 0; // bit within the current row

                    for (int i = start; i < start + length; i += 4)
                    {
                        bool mask = (data[k] & (1 << (7 - j))) == 0;
                        data[i + 3] = (byte)(mask ? 0 : 0xff);

                        j++;
                        m++;

                        if (m == width)
                        {
                            j = 0;
                            m = 0;
                            k++;
                        }
                        else if (j > 7)
                        {
                            j = 0;
                            k++;
                        }
                    }
                }
            }
            else if (bitsPerPixel == 16)
            {
                // Bitmap requires that stride is a multiple of 4, so we 
                // will have to expand the data if width is odd.
                bool expand_data = width % 2 == 1;
                int stride_correction = expand_data ? 2 : 0;
                stride = width * 2 + stride_correction;
                data_to_render = expand_data ? new byte[stride * height] : data;

                System.Diagnostics.Debug.Assert(length == height * width * 2);

                if (cursor)
                {
                    int p = 0;      // Byte within the destination data_to_render.
                    // for mask
                    int j = 0; // bit within the current byte (k)
                    int k = start + length; //byte
                    int m = 0; // bit within the current row

                    for (int i = start; i < start + length; i += 2)
                    {
                        bool mask = (data[k] & (1 << (7 - j))) == 0;
                        byte mask_bit = (byte)(mask ? 0 : 0x80);

                        if (rgb565)
                        {
                            // Convert the 565 data into 1555.
                            data_to_render[p] = (byte)((data[i] & 0x1f) | ((data[i] & 0xe0) >> 1));
                            data_to_render[p + 1] = (byte)(((data[i + 1] & 0x7) >> 1) | (data[i + 1] & 0x78) | mask_bit);
                        }
                        else
                        {
                            // Add the mask bit -- everything else is OK because it's already 555.
                            data_to_render[p] = data[i];
                            data_to_render[p + 1] = (byte)(data[i + 1] | mask_bit);
                        }

                        j++;
                        m++;
                        p += 2;

                        if (m == width)
                        {
                            j = 0;
                            m = 0;
                            k++;
                            p += stride_correction;
                        }
                        else if (j > 7)
                        {
                            j = 0;
                            k++;
                        }
                    }
                }
                else if (expand_data)
                {
                    int w2 = width * 2;
                    int i = start;  // Byte within the source data.
                    int p = 0;      // Byte within the destination data_to_render.

                    for (int m = 0; m < height; m++)
                    {
                        Array.Copy(data, i, data_to_render, p, w2);
                        i += w2;
                        p += stride;
                    }
                }
            }
            else if (bitsPerPixel == 8)
            {
                stride = width * 4;
                data_to_render = data_8bpp;

                System.Diagnostics.Debug.Assert(length == width * height);

                // for mask
                int j = 0; // bit within the current byte (k)
                int k = start + length; //byte
                int m = 0; // bit within the current row

                for (int i = start, n = 0; i < start + length; i++, n += 4)
                {
                    data_8bpp[n + 2] = (byte)(((((data[i] >> redShift) & redMax) << 8) + redMaxOver2) / redMaxPlus1);
                    data_8bpp[n + 1] = (byte)(((((data[i] >> greenShift) & greenMax) << 8) + greenMaxOver2) / greenMaxPlus1);
                    data_8bpp[n] = (byte)(((((data[i] >> blueShift) & blueMax) << 8) + blueMaxOver2) / blueMaxPlus1);
                    if (cursor)
                    {
                        bool mask = (data[k] & (1 << (7 - j))) == 0;
                        data_8bpp[n + 3] = (byte)(mask ? 0 : 0xff);

                        j++;
                        m++;

                        if (m == width)
                        {
                            j = 0;
                            m = 0;
                            k++;
                        }
                        else if (j > 7)
                        {
                            j = 0;
                            k++;
                        }
                    }
                    else
                    {
                        data_8bpp[n + 3] = 0;
                    }
                }
            }
            else
            {
                throw new Exception("unexpected bits per pixel");
            }

            BitmapToClient(width, height, x, y, start, stride, cursor, data_to_render);
        }

        private void BitmapToClient(int width, int height, int x, int y, int start, int stride, bool cursor, byte [] img)
        {
            GCHandle handle = GCHandle.Alloc(img, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(img, start);
                using (Bitmap bitmap = new Bitmap(width, height, stride, cursor ? pixelFormatCursor : pixelFormat, pointer))
                {
                    if (cursor)
                    {
                        client.ClientSetCursor(bitmap, x, y, width, height);
                    }
                    else
                    {
                        client.ClientDrawImage(bitmap, x, y, width, height);
                    }
                }
            }
            catch (ArgumentException exn)
            {
                Log.Error(exn, exn);
            }
            finally
            {
                handle.Free();
            }
        }

        private void readRawEncoding(int x, int y, int width, int height)
        {
            readRawEncoding_(0, x, y, width, height, false);
        }

        /**
         * @param mask If true, read a mask after the raw data, as used by the
         * Cursor pseudo-encoding.
         * @param start The position in this.data to start using
         */
        private void readRawEncoding_(int start, int x, int y, int width, int height, bool cursor)
        {
            if (width < 0 || height < 0)
            {
                throw new VNCException("Invalid size: " + width + " x " +
                                       height);
            }

            int length = width * height * bytesPerPixel;
            if (data.Length < start + length)
                throw new VNCException("Server error: received rectangle bigger than desktop!");
            this.stream.readFully(this.data, start, length);

            int mask_length = 0;

            if (cursor)
            {
                // 1 bit mask.
                int scanline = (width + 7) >> 3;
                mask_length = scanline * height;
                System.Diagnostics.Trace.Assert(this.data.Length >= start + length + mask_length);
                this.stream.readFully(this.data, start + length, mask_length);
            }

            createImage(width, height, x, y, start, length, mask_length, cursor);
        }


        private void readCopyRectangleEncoding(int dx, int dy, int width, int height)
        {
            int x = this.stream.readCard16();
            int y = this.stream.readCard16();
            client.ClientCopyRectangle(x, y, width, height, dx, dy);
        }

        private Color readColor()
        {
            byte[] color = readColorBytes();
            return Color.FromArgb(color[2], color[1], color[0]);
        }

        private byte[] readColorBytes(byte[] color, int start)
        {
            uint pixel =
                bitsPerPixel == 32 ?
                    (uint)(color[start] |
                           color[start + 1] << 8 |
                           color[start + 2] << 16 |
                           color[start + 3] << 24) :
                bitsPerPixel == 16 ?
                    (uint)(color[start] |
                           color[start + 1] << 8) :
                    (uint)color[start];

            byte[] newColor = new byte[4];

            //ARGB Encoding
            newColor[3] = 0xFF;
            newColor[2] = (byte)(((((pixel >> redShift) & redMax) << 8) + redMaxOver2) / redMaxPlus1);
            newColor[1] = (byte)(((((pixel >> greenShift) & greenMax) << 8) + greenMaxOver2) / greenMaxPlus1);
            newColor[0] = (byte)(((((pixel >> blueShift) & blueMax) << 8) + blueMaxOver2) / blueMaxPlus1);

            return newColor;
        }

        private byte[] readColorBytes()
        {
            int n = bitsPerPixel >> 3;
            byte[] color = new byte[n];

            this.stream.readFully(color, 0, n);

            return readColorBytes(color, 0);
        }

        private void readRREEncoding(int x, int y, int width, int height)
        {
            int n = this.stream.readCard32();
            Color background = readColor();
            client.ClientFillRectangle(x, y, width, height, background);
            for (int i = 0; i < n; ++i)
            {
                Color foreground = readColor();
                int rx = this.stream.readCard16();
                int ry = this.stream.readCard16();
                int rw = this.stream.readCard16();
                int rh = this.stream.readCard16();
                client.ClientFillRectangle(x + rx, y + ry, rw, rh, foreground);
            }
        }

        private void readCoRREEncoding(int x, int y, int width, int height)
        {
            int n = this.stream.readCard32();
            Color background = readColor();
            client.ClientFillRectangle(x, y, width, height, background);
            for (int i = 0; i < n; ++i)
            {
                Color foreground = readColor();
                int rx = this.stream.readCard8();
                int ry = this.stream.readCard8();
                int rw = this.stream.readCard8();
                int rh = this.stream.readCard8();
                client.ClientFillRectangle(x + rx, y + ry, rw, rh, foreground);
            }
        }

        private void readFillRectangles(int rx, int ry, int n)
        {
            int pixelSize = (this.bitsPerPixel + 7) >> 3;
            int length = n * (pixelSize + 2);
            this.stream.readFully(data, 0, length);
            int index = 0;
            for (int i = 0; i < n; ++i)
            {
                Color foreground = readColor();
                int sxy = data[index++] & 0xff;
                int sx = sxy >> 4;
                int sy = sxy & 0xf;
                int swh = data[index++] & 0xff;
                int sw = (swh >> 4) + 1;
                int sh = (swh & 0xf) + 1;
                client.ClientFillRectangle(
                    rx + sx, ry + sy, sw, sh, foreground
                );
            }
        }

        private void readRectangles(int rx, int ry, int n, Color foreground)
        {
            for (int i = 0; i < n; ++i)
            {
                int sxy = this.stream.readCard8();
                int sx = sxy >> 4;
                int sy = sxy & 0xf;
                int swh = this.stream.readCard8();
                int sw = (swh >> 4) + 1;
                int sh = (swh & 0xf) + 1;
                client.ClientFillRectangle(
                    rx + sx, ry + sy, sw, sh, foreground
                );
            }
        }

        private void fillRectBytes(byte[] data, int stride, int x, int y, int width, int height, byte[] color)
        {
            // Fast path thin rectangles

            int p = (y * stride) + (x * 4);
            int skip;

            if (width == 1 && height == 1)
            {
                data[p + 0] = color[0];
                data[p + 1] = color[1];
                data[p + 2] = color[2];
                data[p + 3] = color[3];
            }
            else if (width == 1)
            {
                skip = stride - 4;

                for (int i = 0; i < height; i++)
                {
                    data[p + 0] = color[0];
                    data[p + 1] = color[1];
                    data[p + 2] = color[2];
                    data[p + 3] = color[3];

                    p += skip;
                }
            }
            else if (height == 1)
            {
                for (int i = 0; i < width; i++)
                {
                    data[p + 0] = color[0];
                    data[p + 1] = color[1];
                    data[p + 2] = color[2];
                    data[p + 3] = color[3];

                    p += 4;
                }
            }
            else
            {
                skip = stride - (width * 4);

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        data[p + 0] = color[0];
                        data[p + 1] = color[1];
                        data[p + 2] = color[2];
                        data[p + 3] = color[3];

                        p += 4;
                    }
                    p += skip;
                }
            }
        }

        private void readHextileEncoding(int x, int y, int width, int height)
        {
            /*
             * Basically, we have two ways of doing this.
             * 1. Draw the hextile to a byte buffer, convert to image
             *    and draw to client
             * 2. Draw the individual rectangles of the hex encoding
             *    directly to the client
             * 
             * I have found its faster to do 1) when size is > 64
             */
            //GraphicsUtils.startTime();
            if (width * height > 64)
            {
                byte[] background = { 0, 0, 0, 0xFF }; // Black
                byte[] foreground = { 0xFF, 0xFF, 0xFF, 0xFF }; // White

                byte[] buff = new byte[width * height * 4]; //assume 32 bpp
                int stride = width * 4;

                for (int sy = 0; sy < height; sy += 16)
                {
                    int sheight = Math.Min(16, height - sy);

                    for (int sx = 0; sx < width; sx += 16)
                    {
                        int swidth = Math.Min(16, width - sx);

                        int mask = this.stream.readCard8();

                        if ((mask & RAW_SUBENCODING) != 0)
                        {
                            int length = swidth * sheight * 4;
                            this.stream.readFully(this.data, 0, length);

                            int index = 0;
                            int skip = stride - (swidth * 4);
                            int p = (sy * stride) + (sx * 4);

                            for (int i = 0; i < sheight; i++)
                            {
                                for (int j = 0; j < swidth; j++)
                                {
                                    byte[] color = readColorBytes(this.data, index);

                                    index += 4; //assumed 32bpp here

                                    buff[p + 3] = color[3];
                                    buff[p + 2] = color[2];
                                    buff[p + 1] = color[1];
                                    buff[p + 0] = color[0];

                                    p += 4;
                                }

                                p += skip;
                            }
                        }
                        else
                        {
                            if ((mask & BACKGROUND_SPECIFIED_SUBENCODING) != 0)
                            {
                                background = readColorBytes();
                            }

                            fillRectBytes(buff, stride, sx, sy, swidth, sheight, background);

                            if ((mask & FOREGROUND_SPECIFIED_SUBENCODING) != 0)
                            {
                                foreground = readColorBytes();
                            }

                            if ((mask & ANY_SUBRECTS_SUBENCODING) != 0)
                            {
                                int n = this.stream.readCard8();
                                if ((mask & SUBRECTS_COLORED_SUBENCODING) != 0)
                                {
                                    int length = n * 6; //assume 32bpp
                                    this.stream.readFully(data, 0, length);
                                    int index = 0;
                                    for (int i = 0; i < n; ++i)
                                    {
                                        byte[] color = new byte[4];
                                        uint pixel = (uint)(data[index + 0] & 0xFF | data[index + 1] << 8
                                                     | data[index + 2] << 16 | data[index + 3] << 24);

                                        //ARGB Encoding
                                        color[3] = 0xFF;
                                        color[2] = (byte)((pixel >> this.redShift) & this.redMax);
                                        color[1] = (byte)((pixel >> this.greenShift) & this.greenMax);
                                        color[0] = (byte)((pixel >> this.blueShift) & this.blueMax);

                                        index += 4;
                                        int txy = data[index++] & 0xff;
                                        int tx = txy >> 4;
                                        int ty = txy & 0xf;
                                        int twh = data[index++] & 0xff;
                                        int tw = (twh >> 4) + 1;
                                        int th = (twh & 0xf) + 1;

                                        this.fillRectBytes(
                                            buff, stride, sx + tx, sy + ty, tw, th, color
                                        );
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < n; ++i)
                                    {
                                        int txy = this.stream.readCard8();
                                        int tx = txy >> 4;
                                        int ty = txy & 0xf;
                                        int twh = this.stream.readCard8();
                                        int tw = (twh >> 4) + 1;
                                        int th = (twh & 0xf) + 1;

                                        this.fillRectBytes(
                                            buff, stride, sx + tx, sy + ty, tw, th, foreground
                                        );
                                    }
                                }
                            }
                        }
                    }
                }

                // Now convert to image and write to screen
                GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                try
                {
                    IntPtr pointer = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);
                    using (Bitmap bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, pointer))
                    {
                        client.ClientDrawImage(bitmap, x, y, width, height);
                    }
                }
                finally
                {
                    handle.Free();
                }
            }
            else
            {
                Color foreground = Color.White;
                Color background = Color.Black;

                int xCount = (width + 15) >> 4;
                int yCount = (height + 15) >> 4;
                for (int yi = 0; yi < yCount; ++yi)
                {
                    int ry = y + (yi << 4);
                    int rh = (yi == (yCount - 1)) ? height & 0xf : 16;
                    if (rh == 0)
                    {
                        rh = 16;
                    }
                    for (int xi = 0; xi < xCount; ++xi)
                    {
                        int rx = x + (xi << 4);
                        int rw = (xi == (xCount - 1)) ? width & 0xf : 16;
                        if (rw == 0)
                        {
                            rw = 16;
                        }
                        int mask = this.stream.readCard8();
                        if ((mask & RAW_SUBENCODING) != 0)
                        {
                            readRawEncoding(rx, ry, rw, rh);
                        }
                        else
                        {
                            if ((mask & BACKGROUND_SPECIFIED_SUBENCODING) != 0)
                            {
                                background = readColor();
                            }
                            client.ClientFillRectangle(rx, ry, rw, rh, background);
                            if ((mask & FOREGROUND_SPECIFIED_SUBENCODING) != 0)
                            {
                                foreground = readColor();
                            }
                            if ((mask & ANY_SUBRECTS_SUBENCODING) != 0)
                            {
                                int n = this.stream.readCard8();
                                if ((mask & SUBRECTS_COLORED_SUBENCODING) != 0)
                                {
                                    readFillRectangles(rx, ry, n);
                                }
                                else
                                {
                                    readRectangles(rx, ry, n, foreground);
                                }
                            }
                        }
                    }
                }
            }
            /*double time = GraphicsUtils.endTime("hextile");
            int size = width * height;
            StatsEntry entry = new StatsEntry();
            entry.time = time;
            entry.size = size;

            this.client.stats.Add(entry);*/
        }

        private void readCursorPseudoEncoding(int x, int y, int width,
                                              int height)
        {
            readRawEncoding_(0, x, y, width, height, true);
        }

        private void readFrameBufferUpdate()
        {
            this.stream.readPadding(1);
            int n = this.stream.readCard16();
            Log.Debug("reading " + n + " rectangles");
            bool fb_updated = false;
            long start;
            long end;
            Win32.QueryPerformanceCounter(out start);
            for (int i = 0; i < n; ++i)
            {
                int x = this.stream.readCard16();
                int y = this.stream.readCard16();
                int width = this.stream.readCard16();
                int height = this.stream.readCard16();
                int encoding = this.stream.readCard32();
                Log.Debug("read " + x + " " + y + " " + width + " " +
                             height + " " + encoding);
                switch (encoding)
                {
                    case RAW_ENCODING:
                        readRawEncoding(x, y, width, height);
                        break;
                    case RRE_ENCODING:
                        readRREEncoding(x, y, width, height);
                        break;
                    case CORRE_ENCODING:
                        readCoRREEncoding(x, y, width, height);
                        break;
                    case COPY_RECTANGLE_ENCODING:
                        readCopyRectangleEncoding(x, y, width, height);
                        break;
                    case HEXTILE_ENCODING:
                        readHextileEncoding(x, y, width, height);
                        break;

                    case CURSOR_PSEUDO_ENCODING:
                        readCursorPseudoEncoding(x, y, width, height);
                        break;

                    case DESKTOP_SIZE_PSEUDO_ENCODING:
                        desktopSize(width, height);
                        // Since the desktop size has changed, we want a full buffer update next time
                        incremental = false;
                        break;

                    default:
                        throw new VNCException("unimplemented encoding: " + encoding);
                }

                Win32.QueryPerformanceCounter(out end);
                if (end - start > imageUpdateThreshold)
                {
                    client.ClientFrameBufferUpdate();
                    start = end;
                    fb_updated = true;
                }
                else
                {
                    fb_updated = false;
                }
            }

            if (!fb_updated)
                client.ClientFrameBufferUpdate();
        }


        private void desktopSize(int width, int height)
        {
            this.width = width;
            this.height = height;
            int neededBytes = width * height * bytesPerPixel;
            if (neededBytes > data.Length)
            {
                data = new byte[neededBytes];
            }
            if (bitsPerPixel == 8 && (data_8bpp == null || neededBytes * 4 > data_8bpp.Length))
            {
                data_8bpp = new byte[neededBytes * 4];
            }
            client.ClientDesktopSize(width, height);
        }

        private void readServerCutText()
        {
            this.stream.readPadding(3);
            String text = this.stream.readString();
            client.ClientCutText(text);
        }

        private void readServerMessage()
        {
            Log.Debug("readServerMessage");

            int type = this.stream.readCard8();

            switch (type)
            {
                case FRAME_BUFFER_UPDATE:
                    Log.Debug("Update");
                    //GraphicsUtils.startTime();
                    readFrameBufferUpdate();
                    //GraphicsUtils.endTime("readFrameBufferUpdate");
                    break;
                case BELL:
                    Log.Debug("Bell");
                    client.ClientBell();
                    break;
                case SERVER_CUT_TEXT:
                    Log.Debug("Cut text");
                    readServerCutText();
                    break;
                default:
                    throw new VNCException("unknown server message: " + type);
            }
        }

        public readonly Object updateMonitor = new Object(); 

        private void run(object o)
        {
            char[] password = (char[])o;

            try
            {
                handshake();
                sendProtocolVersion();
                authenticationExchange(password);
                clientInitialization();
                serverInitialization();

                if (ConnectionSuccess != null)
                    ConnectionSuccess(this, null);

                // Request a full framebuffer update the first time
                incremental = false;

                while (running)
                {
                    lock (this.writeLock)
                    {
                        writeFramebufferUpdateRequest(0, 0, width, height, incremental);
                        this.stream.Flush();
                    }

                    incremental = true;

                    readServerMessage();

                    lock (pauseMonitor)
                    {
                        lock(updateMonitor)
                            Monitor.PulseAll(updateMonitor);

                        if (paused)
                            Monitor.Wait(pauseMonitor);
                    }
                }
            }
            catch (Exception e)
            {
                if (running && this.ErrorOccurred != null)
                    ErrorOccurred(this, e);
            }
        }

        /// <summary>
        /// Nothrow guarantee.
        /// </summary>
        public void Close()
        {
            if (!running)
                return;

            running = false;
            try
            {
                stream.Close();
                lock (pauseMonitor)
                    Monitor.PulseAll(this.pauseMonitor);
                thread.Interrupt();
            }
            catch
            {
            }
        }

        private bool paused = true;

        public void Pause()
        {
            paused = true;
        }

        public void Unpause(bool fullupdate)
        {
            incremental = !fullupdate;
            paused = false;
            lock (this.pauseMonitor)
                Monitor.PulseAll(this.pauseMonitor);
        }

        public void Unpause()
        {
            Unpause(false);
        }
    }
}
