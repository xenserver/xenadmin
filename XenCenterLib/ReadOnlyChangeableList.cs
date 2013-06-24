/* Copyright (c) Citrix Systems Inc. 
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
using System.ComponentModel;

namespace XenAdmin.Core
{
    public class ReadOnlyChangeableList<T> : IList<T>
    {
        private readonly List<T> _list = new List<T>();

        public event CollectionChangeEventHandler CollectionChanged;

        protected virtual void InsertItem(int index, T item)
        {
            _list.Insert(index, item);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
        }

        protected virtual void RemoveItem(int index)
        {
            T item = _list[index];
            _list.RemoveAt(index);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
        }

        protected virtual void SetItem(int index, T item)
        {
            T oldItem = _list[index];

            if (!ReferenceEquals(item, oldItem))
            {
                _list[index] = item;
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, oldItem));
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
            }
        }

        protected virtual void Clear()
        {
            RemoveAll(item => true);
        }

        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            CollectionChangeEventHandler handler = CollectionChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual int RemoveAll(Predicate<T> match)
        {
            int output = 0;
            for (int i = Count - 1; i >= 0; i--)
            {
                if(match(this[i]))
                {
                    RemoveItem(i);
                    output++;
                }
            }
            return output;
        }

        public T Find(Predicate<T> match)
        {
            return _list.Find(match);
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
        }

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_list).GetEnumerator();
        }

        #endregion

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        T IList<T>.this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { throw new NotSupportedException(); }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
