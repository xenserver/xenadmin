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
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace XenAdmin.Core
{
    internal static class ExtensionMethods
    {
        private const int DEFAULT_STRING_INDENTATION = 2;
        /// <summary>
        /// Internationalization of True/False
        /// </summary>
        public static string ToStringI18n(this bool value)
        {
            return value ? Messages.TRUE : Messages.FALSE;
        }

        /// <summary>
        /// Turns a bool to internationalized Yes/No (on occasion it's user friendlier than True/False)
        /// </summary>
        public static string ToYesNoStringI18n(this bool value)
        {
            return value ? Messages.YES : Messages.NO;
        }

        /// <summary>
        /// This has the same bahvoiur as the standard ellipsise extension but this uses graphics
        /// objects to scale the text. This performs a binary chop on the string to get the correct length
        /// </summary>
        /// <param name="text"></param>
        /// <param name="rectangle"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static string Ellipsise(this string text, Rectangle rectangle, Font font)
        {
            int width = TextRenderer.MeasureText(text, font).Width;
            if (width <= rectangle.Width)
                return text;

            int widthel = TextRenderer.MeasureText(Messages.ELLIPSIS, font).Width;
            if (widthel > rectangle.Width)
                return ".";

            // Binary chop to set the string to the right size
            int a = 0;
            int b = text.Length;
            int c;
            for (c = (a + b) / 2; c > a; c = (a + b) / 2)
            {
                string sr = text.Ellipsise(c);
                int srWidth = TextRenderer.MeasureText(sr, font).Width;
                if (srWidth > rectangle.Width)
                    b = c;
                else
                    a = c;
            }
            return text.Ellipsise(c);
        }

        /// <summary>
        /// Append the input value after it's been prepended with the specified amount of spaces.
        /// If the value spans multiple lines, each line will be indented.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to which the modified value will be appended.</param>
        /// <param name="value">The value to prepend with spaces and then append.</param>
        /// <param name="indent">The amount of spaces to prepend to each line in the input value.</param>
        /// <returns>The input <see cref="StringBuilder"/> after the operation has been completed.</returns>
        public static StringBuilder AppendIndented(this StringBuilder builder, string value, int indent = DEFAULT_STRING_INDENTATION)
        {
            return builder.Append(PrependIndentation(value, indent));
        }

        /// <summary>
        /// Add a new line to the input <see cref="StringBuilder"/>, with options to format the input value before it's appended.
        /// timestamps and indentation will be ignored if the input value is an null or whitespace
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to which the modified value will be appended.</param>
        /// <param name="value">The value to format before appending.</param>
        /// <param name="timestamp">The timestamp to prepend to each line. If null, no timestamp will be added.</param>
        /// <param name="showTimestamp">Override for the timestamp. If set to false, no timestamp will be shown even if the value is null.</param>
        /// <param name="indent">true if each line should be prepended with indentation. Uses the default indentation defined in <see cref="ExtensionMethods"/>: <see cref="DEFAULT_STRING_INDENTATION"/></param>
        /// <param name="addExtraLine">true to append an extra line.</param>
        /// <returns>The input <see cref="StringBuilder"/> after the operation has been completed.</returns>
        public static StringBuilder AppendFormattedLine(this StringBuilder builder, string value, DateTime? timestamp, bool showTimestamp = true, bool indent = false, bool addExtraLine = false)
        {
            var formattedValue = value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (indent)
                {
                    formattedValue = PrependIndentation(formattedValue);
                }
                if (timestamp != null && showTimestamp)
                {
                    formattedValue = PrependTimestamps(formattedValue, (DateTime) timestamp);
                }
            }

            builder.AppendLine(formattedValue);

            if (addExtraLine)
            {
                builder.AppendLine();
            }

            return builder;
        }

        /// <summary>
        /// Prepend every line in the input value with the specified indentation level.
        /// </summary>
        /// <param name="value">The value to which indentation will be applied</param>
        /// <param name="indent">The level of indentation, i.e. the number of spaces to prepend to every line in the value.</param>
        /// <returns>The input value with prepended indentation./</returns>
        private static string PrependIndentation(string value, int indent = DEFAULT_STRING_INDENTATION)
        {
            var indentString = new string(' ', indent);
            var newValue = value.Replace(Environment.NewLine, $"{Environment.NewLine}{indentString}");
            return $"{indentString}{newValue}";
        }

        /// <summary>
        /// Prepend every line in the input value with a formatted string of <see cref="DateTime.Now"/>.
        /// </summary>
        /// <param name="value">The input value</param>
        /// <param name="timestamp">The timestamp to show</param>
        /// <param name="localize">true to format the string with the user's locale</param>
        /// <returns>The input value with prepended timestamps/</returns>
        public static string PrependTimestamps(string value, DateTime timestamp, bool localize = true)
        {
            var timestampString = HelpersGUI.DateTimeToString(timestamp, Messages.DATEFORMAT_DM_HM, localize);
            // normalise all line endings before splitting
            var lines = value.Replace(Environment.NewLine, "\n").Split('\n');
            return string.Join(Environment.NewLine, lines.Select(line => $"{timestampString} | {line}"));
        }
    }
}
