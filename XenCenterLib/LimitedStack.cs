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
using System.Text;
using System.Collections;

namespace XenAdmin.Core
{
    public class LimitedStack<T> : IEnumerable<T> where T : class
    {
        private T[] contents;
        private int index;
        private int capacity;

        public LimitedStack(int capacity)
        {
            this.capacity = capacity;
            this.contents = new T[capacity];
            this.index = 0;
        }

        public void Push(T t)
        {
            System.Diagnostics.Trace.Assert(t != null, "Cannot push null item onto stack");

            index++;
            index %= capacity;

            contents[index] = t;
        }

        public T Pop()
        {
            if (contents[index] == null)
                return null;

            T candidate = contents[index];
            contents[index] = null;

            index--;
            index += capacity; //ensure its always > 0;
            index %= capacity;

            return candidate;
        }

        public T Peek()
        {
            return contents[index];
        }

        public void Clear()
        {
            index = 0;

            for(int i = 0; i< capacity; i++)
                contents[i] = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<T>
        {
            private readonly LimitedStack<T> stack;
            private readonly int firstIndex;
            private int index = -1;

            public Enumerator(LimitedStack<T> stack)
            {
                this.stack = stack;
                this.firstIndex = stack.index;
            }

            public T Current
            {
                get { return stack.contents[index]; }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (index == -1)
                {
                    index = firstIndex;

                    return true;
                }
                else
                {
                    index--;
                    index += stack.capacity;
                    index %= stack.capacity;

                    return index != firstIndex && Current != null;
                }
            }

            public void Reset()
            {
                index = firstIndex;
            }
        }
    }
}
