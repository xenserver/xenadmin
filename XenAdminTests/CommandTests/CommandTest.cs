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
using XenAdmin.Commands;
using XenAdmin;
using NUnit.Framework;

using XenAdmin.Controls.MainWindowControls;

using XenAPI;
using XenAdmin.Controls;


namespace XenAdminTests.CommandTests
{

    public struct CommandTestsDatabase
    {
        public const string George = "state1.xml";
        public const string MidnightRide = "state4.xml";
        public const string SingleHost = "single-host-krakout.xml";
        public const string Boston = "boston-db.xml";
    }

    public abstract class CommandTest : MainWindowTester
    {
        internal Command Command { get; private set; }

        internal abstract Command CreateCommand();

        protected virtual NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Infrastructure; }
        }

        internal IEnumerable<SelectedItemCollection> RunTest()
        {
            PutInNavigationMode(NativeMode);
            return RunTest(GetSelections());
        }

        internal IEnumerable<IXenObject> RunTest(Func<IXenObject> xenObject)
        {
            foreach (SelectedItemCollection selection in RunTest(GetSelections(xenObject)))
            {
                yield return selection.FirstAsXenObject;
            }
        }

        internal IEnumerable<IXenObject> RunTest(IEnumerable<IXenObject> xenObjects)
        {
            foreach (SelectedItemCollection selection in RunTest(GetSelections(xenObjects)))
            {
                yield return selection.FirstAsXenObject;
            }
        }

        internal IEnumerable<SelectedItemCollection> RunTest( IEnumerable<SelectedItemCollection> selections)
        {           
            Command = CreateCommand();

            ((ICommand)Command).SetMainWindow(Program.MainWindow);

            bool noneExecuted = true;

            foreach (SelectedItemCollection selection in selections)
            {
                ((ICommand)Command).SetSelection(selection);

                if (MW(() => Command.CanExecute()))
                {
                    noneExecuted = false;
                    SelectNodes(selection);

                    yield return selection;
                }
            }

            Assert.IsFalse(noneExecuted, "No tests were executed");
        }

        private IEnumerable<SelectedItemCollection> GetSelections(IEnumerable<IXenObject> xenObjects)
        {
            foreach (IXenObject x in xenObjects)
            {
                yield return new SelectedItemCollection(new SelectedItem(x));
            }
        }

        private IEnumerable<SelectedItemCollection> GetSelections(Func<IXenObject> x)
        {
            yield return new SelectedItemCollection(new SelectedItem(x()));
        }

        private IEnumerable<SelectedItemCollection> GetSelections()
        {
            List<object> output = new List<object>();
            var rootNode = GetAllTreeNodes().Find(n => n.Parent == null);
            foreach (VirtualTreeNode n in GetAllTreeNodes())
            {
                if (!output.Contains(n.Tag))
                {
                    output.Add(n.Tag);

                    GroupingTag gt = n.Tag as GroupingTag;
                    if (gt != null)
                    {
                        yield return new SelectedItemCollection(new SelectedItem(gt, rootNode));
                    }
                    else
                    {
                        yield return new SelectedItemCollection(new SelectedItem(n.Tag as IXenObject));
                    }
                }
            }
        }

        private void SelectNodes(SelectedItemCollection selection)
        {
            List<object> objects = selection.AsObjects();
            List<VirtualTreeNode> nodes = new List<VirtualTreeNode>(objects.ConvertAll(o => FindInTree(o)));

            if (!nodes.Contains(null))
            {
                Assert.IsTrue(SelectInTree(objects.ToArray()), "Couldn't select node for selection.");
            }
            else
            {
                PutInNavigationMode(NativeMode);

                nodes = new List<VirtualTreeNode>(objects.ConvertAll(o => FindInTree(o)));

                Assert.IsFalse(nodes.Contains(null), "Couldn't find nodes for selection.");
                Assert.IsTrue(SelectInTree(objects.ToArray()), "Couldn't select node for selection.");
            }
        }
    }
}

