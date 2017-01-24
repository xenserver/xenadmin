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
using System.ComponentModel;
using System.Collections.Generic;

namespace XenAdmin.Core
{
    public class ChangeableList<T> : List<T>
    {
        // NOTE: This class should really derive from Collection<T> since Collection<T> contains overrides for Add, Remove, Clear etc.
        // This class won't notify if cast to List<T> before Add, Remove etc. are called.

        public event CollectionChangeEventHandler CollectionChanged;

        public new void Add(T item)
        {
            System.Diagnostics.Debug.Assert(item != null, "Item cannot be null");
            base.Add(item);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            System.Diagnostics.Debug.Assert(collection != null, "Collection cannot be null");
            base.AddRange(collection);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, collection));
        }

        public new void Insert(int index, T item)
        {
            System.Diagnostics.Debug.Assert(item != null, "Item cannot be null");
            base.Insert(index, item);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
        }

        public new T this[int index]
        {
            get { return base[index]; }
            set
            {
                base[index] = value;
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, value));
            }
        }

        public new void Remove(T item)
        {
            System.Diagnostics.Debug.Assert(item != null, "Item cannot be null");
            base.Remove(item);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
        }

        public new int RemoveAll(Predicate<T> match)
        {
            var toRemove = FindAll(match);
            base.RemoveAll(match);
            toRemove.ForEach(item => OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item)));
            return toRemove.Count;
        }

        public new void RemoveAt(int index)
        {
            T item = this[index];
            base.RemoveAt(index);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
        }

        public new void Clear()
        {
            RemoveAll(t => true);
        }

        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            CollectionChangeEventHandler handler = CollectionChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
