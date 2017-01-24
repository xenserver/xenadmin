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
using System.Threading;

// Copied from http://www.boyet.com/Articles/LockfreeQueue.html
// Contacted author for licences conditions 10/03/08, none
// state on blog
// IEnumerable added by Tom Wilkie (Citrix)

namespace XenAdmin.Core
{
    internal class SingleLinkNode<T>
    {
        // Note; the Next member cannot be a property since it participates in
        // many CAS operations
        public SingleLinkNode<T> Next;
        public T Item;
    }

    public class LockFreeQueue<T> : IEnumerable<T>
    {
        SingleLinkNode<T> head;
        SingleLinkNode<T> tail;

        public LockFreeQueue()
        {
            head = new SingleLinkNode<T>();
            tail = head;
        }

        private static bool CAS<S>(ref S location, S comparand, S newValue) where S : class
        {
            return
                (object)comparand ==
                (object)Interlocked.CompareExchange<S>(ref location, newValue, comparand);
        }

        public void Enqueue(T item)
        {
            SingleLinkNode<T> oldTail = null;
            SingleLinkNode<T> oldTailNext;

            SingleLinkNode<T> newNode = new SingleLinkNode<T>();
            newNode.Item = item;

            bool newNodeWasAdded = false;
            while (!newNodeWasAdded)
            {
                oldTail = tail;
                oldTailNext = oldTail.Next;

                if (tail == oldTail)
                {
                    if (oldTailNext == null)
                        newNodeWasAdded = CAS<SingleLinkNode<T>>(ref tail.Next, null, newNode);
                    else
                        CAS<SingleLinkNode<T>>(ref tail, oldTail, oldTailNext);
                }
            }

            CAS<SingleLinkNode<T>>(ref tail, oldTail, newNode);
        }

        public bool Dequeue(out T item)
        {
            item = default(T);
            SingleLinkNode<T> oldHead = null;

            bool haveAdvancedHead = false;
            while (!haveAdvancedHead)
            {

                oldHead = head;
                SingleLinkNode<T> oldTail = tail;
                SingleLinkNode<T> oldHeadNext = oldHead.Next;

                if (oldHead == head)
                {
                    if (oldHead == oldTail)
                    {
                        if (oldHeadNext == null)
                        {
                            return false;
                        }
                        CAS<SingleLinkNode<T>>(ref tail, oldTail, oldHeadNext);
                    }

                    else
                    {
                        item = oldHeadNext.Item;
                        haveAdvancedHead =
                          CAS<SingleLinkNode<T>>(ref head, oldHead, oldHeadNext);
                    }
                }
            }
            return true;
        }

        public T Dequeue()
        {
            T result;
            Dequeue(out result);
            return result;
        }

        public bool NotEmpty
        {
            get
            {
                return head.Next != null;
            }
        }

        #region IEnumerable<T> Members

        private class LockFreeQueueEnumerator<U> : IEnumerator<U>
        {
            private readonly LockFreeQueue<U> queue;
            private SingleLinkNode<U> current;

            public LockFreeQueueEnumerator(LockFreeQueue<U> queue)
            {
                this.queue = queue;
                this.current = queue.head;
            }

            #region IEnumerator<T> Members

            public U Current
            {
                get { return current.Item; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                if (current.Next == null)
                    return false;

                current = current.Next;

                return true;
            }

            public void Reset()
            {
                current = queue.head;
            }

            #endregion
        }
        
        /// <summary>
        /// This function is not lock-free, and must only be used if no-one if reading from the list.
        /// This is fine for our use, as we only need to enumerate a queue when we first populate the cache.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new LockFreeQueueEnumerator<T>(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}