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
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace DotNetVnc
{
    public class KeySet : IEquatable<KeySet>
    {
        List<Keys> keys;

        public IList<Keys> Keys
        {
            get
            {
                return keys.AsReadOnly();
            }
        }

        public KeySet()
        {
            this.keys = new List<Keys>();
        }

        public KeySet(List<Keys> keys)
        {
            this.keys = new List<Keys>();

            foreach (Keys key in keys)
            {
                if (!this.keys.Contains(key))
                {
                    this.keys.Add(key);
                }
            }

            this.keys.Sort();
        }

        public KeySet(params Keys[] keys)
        {
            this.keys = new List<Keys>();

            foreach (Keys key in keys)
            {
                if (!this.keys.Contains(key))
                {
                    this.keys.Add(key);
                }
            }

            this.keys.Sort();
        }

        public KeySet Add(Keys _key)
        {
            List<Keys> newKeys = new List<Keys>();
            
            foreach(Keys key in this.keys)
            {
                newKeys.Add(key);
            }

            if (!newKeys.Contains(_key))
                newKeys.Add(_key);

            return new KeySet(newKeys);
        }

        public KeySet Remove(Keys _key)
        {
            List<Keys> newKeys = new List<Keys>();

            foreach (Keys key in this.keys)
            {
                if (key != _key)
                {
                    newKeys.Add(key);
                }
            }

            return new KeySet(newKeys);
        }

        public bool Equals(KeySet keySet)
        {
            foreach (Keys key in keySet.Keys)
            {
                if (!keys.Contains(key))
                {
                    return false;
                }
            }

            foreach (Keys key in this.keys)
            {
                if (!keySet.Keys.Contains(key))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (this.keys.Count > 0)
            {
                return (int)this.keys[0];
            }
            else
            {
                return 0;
            }
        }

        public override String ToString()
        {
            String result = "";

            foreach (Keys key in this.keys)
            {
                result += key;
                result += " ";
            }

            return result;
        }
    }
}
