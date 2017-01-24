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
using System.Threading;
using System.Collections.Generic;

public class ProduceConsumerQueue
{
    readonly object _locker = new object();
    Thread[] _workers;
    Queue<Action> _itemQ = new Queue<Action>();

    public ProduceConsumerQueue(int workerCount)
    {
        _workers = new Thread[workerCount];
        for (int i = 0; i < workerCount; i++)
            (_workers[i] = new Thread(Consume) { IsBackground = true }).Start();
    }

    public void StopWorkers(bool waitForWorkers)
    {
        foreach (Thread worker in _workers)
            EnqueueItem(null);

        if (waitForWorkers)
            foreach (Thread worker in _workers)
                worker.Join();
    }

    public void EnqueueItem(Action item)
    {
        lock (_locker)
        {
            _itemQ.Enqueue(item);
            //Check the blocking condition
            Monitor.Pulse(_locker);
        }
    }

    void Consume()
    {
        while (true)
        {
            Action item;
            lock (_locker)
            {
                while (_itemQ.Count == 0)
                    Monitor.Wait(_locker);

                item = _itemQ.Dequeue();
            }
            if (item == null)
                return;
            item();
        }
    }
}