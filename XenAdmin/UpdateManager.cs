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
using System.Diagnostics;

namespace XenAdmin
{
    /// <summary>
    /// A class to throttle the updates to a particular class/component.
    /// 
    /// The purpose of the class is best described by example:
    /// It is used by <see cref="MainWindow"/> to control the refreshes to the TreeView.
    /// The refresh of the tree is slow and it is called all over the place. 
    /// This class ensures there is always a small break between refreshes 
    /// - the length of the break is calculated by <see cref="DelayCalculator"/>.
    /// 
    /// This class also allows consumers to specify a maximum time between updates.
    /// </summary>
    internal partial class UpdateManager : IDisposable
    {
        /// <summary>
        /// Used to ensure the maximum time between updates. (ms)
        /// </summary>
        private readonly int _longInterval;

        private bool _disposed;
        private Timer _shortIntervalTimer;
        private Timer _longIntervalTimer;
        private readonly DelayCalculator _delayCalculator = new DelayCalculator();
        private readonly object _lock = new object();

        /// <summary>
        /// A Boolean indicating whether an update is currently taking place. 
        /// The _lock variable should be used when reading and writing to this variable.
        /// </summary>
        private bool _doingUpdate;

        /// <summary>
        /// A Boolean indicating whether an update request was received while the current update is taking place.
        /// The _lock variable should be used when reading and writing to this variable.
        /// </summary>
        private bool _anotherUpdateRequestOccurredDuringUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManager"/> class.
        /// </summary>
        /// <param name="longInterval">The maximum time between updates (ms). Specify 0 for no maximum value.</param>
        public UpdateManager(int longInterval)
        {
            _longInterval = longInterval;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateManager"/> class. There is no maximum time between updates specified.
        /// </summary>
        public UpdateManager()
            : this(0)
        {
        }

        /// <summary>
        /// Requests an update. The <see cref="Update"/> event occurs when an update should take place.
        /// </summary>
        public void RequestUpdate()
        {
            _delayCalculator.RegisterLatestUpdateRequest();
            RequestUpdate(null);
        }

        private void RequestUpdate(object state)
        {
            if (TryBeginUpdate())
            {
                // ensure update occurs on a new thread. This frees the calling thread up to continue.

                ThreadPool.QueueUserWorkItem(DoUpdate);
            }
        }

        private void DoUpdate(object state)
        {
            try
            {
                // stop watch to time how long an update takes.
                Stopwatch sw = Stopwatch.StartNew();

                OnUpdate(EventArgs.Empty);

                _delayCalculator.RegisterLatestUpdate(sw.ElapsedMilliseconds);
            }
            finally
            {
                // allow another request only after _delayCalculator.GetDelay() has elapsed.
                _shortIntervalTimer = new Timer(EndUpdate, null, _delayCalculator.GetDelay(), Timeout.Infinite);
            }
        }

        /// <summary>
        /// Tries to begin an Update.
        /// </summary>
        /// <returns>Returns false if an update is already underway.</returns>
        private bool TryBeginUpdate()
        {
            lock (_lock)
            {
                if (!_doingUpdate && !_disposed)
                {
                    // now do an update so reset this variable.
                    _anotherUpdateRequestOccurredDuringUpdate = false;

                    DisposeTimer(_longIntervalTimer);

                    _doingUpdate = true;
                    return true;
                }

                // an update is taking place and another request has taken place.
                _anotherUpdateRequestOccurredDuringUpdate = true;
                return false;
            }
        }

        private void EndUpdate(object state)
        {
            lock (_lock)
            {
                _doingUpdate = false;
                DisposeTimer(_shortIntervalTimer);

                if (!_disposed)
                {
                    if (_longInterval > 0)
                    {
                        _longIntervalTimer = new Timer(RequestUpdate, null, _longInterval, _longInterval);
                    }

                    if (_anotherUpdateRequestOccurredDuringUpdate)
                    {
                        // another update was requested during this update
                        RequestUpdate();
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Update"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUpdate(EventArgs e)
        {
            EventHandler handler = Update;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    if (!_disposed)
                    {
                        _disposed = true;

                        DisposeTimer(_longIntervalTimer);
                        DisposeTimer(_shortIntervalTimer);
                    }
                }
                GC.SuppressFinalize(this);
            }
        }

        private static void DisposeTimer(Timer timer)
        {
            if (timer != null)
            {
                timer.Dispose();
            }
        }

        /// <summary>
        /// Occurs when an update should take place.
        /// </summary>
        public event EventHandler Update;

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
