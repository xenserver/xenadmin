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
using System.IO;

namespace XenAdmin.Core
{
	public class PathValidator
	{
		private static readonly char[] m_invalidFileCharList = Path.GetInvalidFileNameChars();
		private static readonly string[] m_deviceNames = {
		                                                 	"CON", "PRN", "AUX", "NUL",
		                                                 	"COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
		                                                 	"LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
		                                                 };

		public static bool IsFileNameValid(string filename)
		{
			if (filename.IndexOfAny(m_invalidFileCharList) > -1)
				return false;

			foreach (var name in m_deviceNames)
			{
				if (name == filename.ToUpper())
					return false;
			}

			return true;
		}

		public static bool IsPathValid(string path)
		{
			if (string.IsNullOrEmpty(path))
				return false;

			try
			{
				if (Path.IsPathRooted(path))
				{
					path = path[0] == '\\' && path.Length == 1
					       	? path.Substring(1)
					       	: path.Substring(2);
				}
			}
			catch(ArgumentException)
			{
				//path contains a character from Path.GetInvalidPathChars()
				return false;
			}

			string[] parts = path.Split(new[] {'\\'});

			if (parts.Length > 0)
			{
				foreach (var part in  parts)
				{
					if (part.IndexOfAny(m_invalidFileCharList) > -1)
						return false;

					foreach (var name in m_deviceNames)
					{
						if (name == part.ToUpper())
							return false;
					}
				}
			}

			return true;
		}
	}
}
