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

using System.ComponentModel;
using NUnit.Framework;
using XenAdmin.Controls.DataGridViewEx;

namespace XenAdminTests.UnitTests.Dialogs.DataGridViewEx
{
    [TestFixture, NUnit.Framework.Category(TestCategories.Unit)]
    public class CollapsingPoolHostDataGridViewRowSorterTests
    {
        private class TestRow : PoolHostDataGridViewOneCheckboxRow
        {
        }

        private class TestSorter : CollapsingPoolHostDataGridViewRowSorter
        {
            private readonly int sortValueToReturn;

            public TestSorter(ListSortDirection direction, int sortValueToReturn) : base(direction)
            {
                this.sortValueToReturn = sortValueToReturn;
            }

            public TestSorter(int sortValueToReturn) : base()
            {
                this.sortValueToReturn = sortValueToReturn;
            }

            protected override int PerformSort()
            {
                return sortValueToReturn;
            }
        }

        [Test]
        public void TestDirectionConstructorAscending()
        {
            TestSorter sorter = new TestSorter(ListSortDirection.Ascending, 3);
            Assert.AreEqual(3, sorter.Compare(new TestRow(), new TestRow()));
        }

        [Test]
        public void TestDirectionConstructorDescending()
        {
            TestSorter sorter = new TestSorter(ListSortDirection.Descending, 3);
            Assert.AreEqual(-3, sorter.Compare(new TestRow(), new TestRow()));
        }

        [Test]
        public void TestDefaultConstructorUsesAscending()
        {
            TestSorter sorter = new TestSorter(3);
            Assert.AreEqual(3, sorter.Compare(new TestRow(), new TestRow()));
        }

        [Test]
        public void TestNullsInComparisonReturnOne()
        {
            TestSorter sorter = new TestSorter(3);
            Assert.AreEqual(3, sorter.Compare(new TestRow(), new TestRow()));
            Assert.AreEqual(1, sorter.Compare(null, new TestRow()));
            Assert.AreEqual(1, sorter.Compare(new TestRow(), null));
            Assert.AreEqual(1, sorter.Compare(null, null));
        }
    }
}