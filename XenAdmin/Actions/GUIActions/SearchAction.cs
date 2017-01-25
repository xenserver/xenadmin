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
using XenAdmin.XenSearch;


namespace XenAdmin.Actions
{
    class SearchAction : AsyncAction
    {
        public enum Operation { save, delete };

        private readonly Operation operation;
        private readonly Search search;

        public SearchAction(Search search, Operation operation)
            : base(null, GetTitle(search, operation), GetDescription(search, operation))
        {
            this.operation = operation;
            this.search = search;
            Connection = search.Connection;
            Pool = XenAdmin.Core.Helpers.GetPoolOfOne(Connection);
        }

        private static String GetTitle(Search search, Operation operation)
        {
            switch(operation)
            {
                case Operation.save:
                    return String.Format(Messages.SAVE_SEARCH, search.Name);

                case Operation.delete:
                    return String.Format(Messages.DELETE_SEARCH, search.Name);
            }

            return "";
        }

        private static String GetDescription(Search search, Operation operation)
        {
            switch(operation)
            {
                case Operation.save:
                    return String.Format(Messages.SAVING_SEARCH, search.Name);

                case Operation.delete:
                    return String.Format(Messages.DELETING_SEARCH, search.Name);
            }

            return "";
        }

        protected override void Run()
        {
            switch(operation)
            {
                case Operation.save:
                    search.Save();
                    return;

                case Operation.delete:
                    search.Delete();
                    return;
            }
        }
    }
}
