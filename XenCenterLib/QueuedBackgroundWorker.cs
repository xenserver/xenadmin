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

using System.Collections.Generic;
using System.ComponentModel;

namespace XenCenterLib
{
    /// <summary>
    /// This is thread-safe
    /// </summary>
    public class QueuedBackgroundWorker:BackgroundWorker
    {
        private Queue<BackgroundWorker> Queue = new Queue<BackgroundWorker>();

        private object _lock = new object();

        public delegate object DoWorkQueuedEventHandler(object sender, object argument);

        public void RunWorkerAsync(DoWorkQueuedEventHandler doWork, RunWorkerCompletedEventHandler workerCompleted)
        {
            BackgroundWorker bw = GetBackgroundWorker(doWork, workerCompleted);

            Queue.Enqueue(bw);

            lock (_lock)
            {
                if (Queue.Count == 1)
                {
                    ((BackgroundWorker)this.Queue.Peek()).RunWorkerAsync();
                }
            }
        }

        private BackgroundWorker GetBackgroundWorker(DoWorkQueuedEventHandler doWork, RunWorkerCompletedEventHandler workerCompleted)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = false;
            bw.WorkerSupportsCancellation = false;

            bw.DoWork += (sender, args) =>
                             {
                                 if (doWork != null)
                                 {
                                     args.Result=doWork(this, new DoWorkEventArgs(args.Argument));
                                 }
                             };

            bw.RunWorkerCompleted += (sender, args) =>
                                         {
                                             if (workerCompleted != null)
                                             {
                                                 object result = args.Error == null ? args.Result : null;
                                                 workerCompleted(this,
                                                                 new RunWorkerCompletedEventArgs(result, args.Error,
                                                                                                 args.Cancelled));
                                             }
                                             Queue.Dequeue();
                                             lock (_lock)
                                             {
                                                 if (Queue.Count > 0)
                                                 {
                                                     ((BackgroundWorker)this.Queue.Peek()).RunWorkerAsync();
                                                 }
                                             }
                                         };
            return bw;
        }


    
    }
}