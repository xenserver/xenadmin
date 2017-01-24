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
using System.Drawing.Imaging;
using System.Text;
using System.Drawing;
using XenAPI;
using System.Drawing.Drawing2D;
using XenAdmin.Core;
using XenAdmin.XenSearch;


namespace XenAdmin.Controls.CustomDataGraph
{
    public class DataEvent : DataPoint, IEquatable<DataEvent>, IComparable<DataEvent>
    {
        public bool Selected;
        public Message Message;
        public IXenObject xo;

        public DataEvent(long x, long y, Message message)
            : base(x, y)
        {
            Message = message;
            xo = Helpers.XenObjectFromMessage(message);
        }

        public void DrawToBuffer(Graphics g, Point p, Rectangle rectangle, int stripheight)
        {
            Rectangle r = new Rectangle(p.X - (stripheight / 2), rectangle.Top, stripheight, stripheight);
            Image image = TypeImage;

            if (Selected)
                g.DrawImage(image, r);
            else
                g.DrawImage(image, r, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, Drawing.AlphaAttributes);
        }

        public Image TypeImage
        {
            get
            {
                return Images.GetImage16For(Message.Type);
            }
        }

        public override string ToString()
        {
            return HelpersGUI.DateTimeToString(Message.TimestampLocal, Messages.DATEFORMAT_DMY_HM, true);
        }

        public bool Equals(DataEvent other)
        {
            return Message.uuid == other.Message.uuid;
        }

        #region IComparable<DataEvent> Members

        public int CompareTo(DataEvent other)
        {
            if (Message.timestamp != other.Message.timestamp)
                return -Message.timestamp.CompareTo(other.Message.timestamp);

            return StringUtility.NaturalCompare(ToString(),other.ToString());
        }

        #endregion
    }
}
