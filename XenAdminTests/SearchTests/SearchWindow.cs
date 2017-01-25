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
using System.Collections.ObjectModel;
using XenAdmin.Controls.CustomGridView;
using XenAdmin.Controls.XenSearch;
using XenAdmin.TabPages;
using System.Reflection;
using XenAdmin;

namespace XenAdminTests.SearchTests
{
    internal static class SearchWindow
    {
        public static RowCollection GetVisibleResults()
        {
            SearchPage searchPage = Program.MainWindow.SearchPage;
            SearchOutput searchOutput = (SearchOutput)searchPage.GetType().GetField("OutputPanel", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(searchPage);

            return new RowCollection(GetVisibleResults(searchOutput.QueryPanel.Rows));
        }

        private static List<Row> GetVisibleResults(IEnumerable<GridRow> rows)
        {
            List<Row> output = new List<Row>();
            foreach (GridRow gridRow in rows)
            {
                string[] path = gridRow.Path.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                output.Add(new Row(path, GetVisibleResults(gridRow.Rows)));
            }
            return output;
        }

        public class Row
        {
            public readonly ReadOnlyCollection<string> Path;
            public readonly RowCollection SubRows;

            public Row(IEnumerable<string> path, IEnumerable<Row> subRows)
            {
                Path = new ReadOnlyCollection<string>(new List<string>(path));
                SubRows = new RowCollection(new List<Row>(subRows));
            }
        }

        public class RowCollection : ReadOnlyCollection<Row>
        {
            public RowCollection(IList<Row> list)
                : base(list)
            {
            }

            public bool Contains(string pathPart)
            {
                foreach (Row row in this)
                {
                    foreach (string pp in row.Path)
                    {
                        if (pp == pathPart)
                        {
                            return true;
                        }
                    }
                    if (row.SubRows.Contains(pathPart))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
