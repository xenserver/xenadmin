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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace XenAdmin.ServerDBs
{
    public partial class Db
    {
        public abstract class DbDictionary<TValue> : Dictionary<string, TValue>
        {
            public event EventHandler Changed;

            public DbDictionary()
                : base(StringComparer.InvariantCultureIgnoreCase)
            {

            }

            public new ReadOnlyCollection<string> Keys
            {
                get
                {
                    return new ReadOnlyCollection<string>(new List<string>(base.Keys));
                }
            }

            public new void Add(string key, TValue value)
            {
                base.Add(key, value);
                OnChanged(EventArgs.Empty);
            }

            public new bool Remove(string key)
            {
                bool ret = base.Remove(key);

                if (ret)
                {
                    OnChanged(EventArgs.Empty);
                }
                
                return ret;
            }

            public new TValue this[string key]
            {
                get
                {
                    return base[key];
                }
                set
                {
                    base[key] = value;
                    OnChanged(EventArgs.Empty);
                }
            }

            protected virtual void OnChanged(EventArgs e)
            {
                EventHandler handler = Changed;

                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        public class PropDictionary : DbDictionary<Prop>
        {
            private readonly Row _row;

            public PropDictionary(Row row)
            {
                Util.ThrowIfParameterNull(row, "row");
                _row = row;
            }

            public void Add(string name, string value)
            {
                if (TypeCache.GetFieldType(_row.Table.Name, name) == null)
                    return;

                base.Add(name, new Prop(_row, name, value));
            }
        }

        public class RowDictionary : DbDictionary<Row>
        {
            private readonly Table _table;

            public RowDictionary(Table table)
            {
                Util.ThrowIfParameterNull(table, "table");
                _table = table;
            }

            public Row Add(string opaqueRef)
            {
                Util.ThrowIfStringParameterNullOrEmpty(opaqueRef, "opaqueRef");

                Row row = new Row(_table);
                base.Add(opaqueRef, row);
                return row;
            }
        }

        public class TableDictionary : DbDictionary<Table>
        {
            private readonly Db _db;

            public TableDictionary(Db db)
            {
                Util.ThrowIfParameterNull(db, "db");
                _db = db;
            }

            public Table Add(string name)
            {
                Util.ThrowIfStringParameterNullOrEmpty(name, "name");

                Table table = new Table(name, _db);
                base.Add(name, table);
                return table;
            }
        }
    }
}
