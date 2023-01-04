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
using System.IO;
using System.IO.Compression;


namespace XenCenterLib.Archive
{
    public class ZipArchiveIterator : ArchiveIterator
    {
        private ZipArchive _zipArchive;
        private IEnumerator<ZipArchiveEntry> _enumerator;
        private ZipArchiveEntry _currentEntry;
        private int _currentPosition = -1;
        private bool _disposed;

        /// <summary>
        /// index of current file, total file count
        /// </summary>
        public event Action<int, int> CurrentFileExtracted;

        /// <summary>
        /// Parameterless constructor needed by tests
        /// </summary>
        public ZipArchiveIterator()
        {
        }

        public ZipArchiveIterator(Stream inputStream)
        {
            _zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read);
            _enumerator = _zipArchive.Entries.GetEnumerator();
        }

        public override void SetBaseStream(Stream inputStream)
        {
            _zipArchive = new ZipArchive(inputStream, ZipArchiveMode.Read);
            _enumerator = _zipArchive.Entries.GetEnumerator();
            _disposed = false;
            _currentPosition = -1;
        }

        ~ZipArchiveIterator()
        {
            Dispose();
        }

        public override bool HasNext()
        {
            if (_enumerator != null && _enumerator.MoveNext())
            {
                _currentPosition++;
                _currentEntry = _enumerator.Current;
                return true;
            }
            return false;
        }

        public override string CurrentFileName()
        {
            return _currentEntry == null ? string.Empty : _currentEntry.FullName;
        }

        public override long CurrentFileSize()
        {
            return _currentEntry?.Length ?? 0;
        }

        public override DateTime CurrentFileModificationTime()
        {
            return _currentEntry == null ? new DateTime() : _currentEntry.LastWriteTime.DateTime;
        }

        public override bool IsDirectory()
        {
            if (_currentEntry.ExternalAttributes != 0)
            {
                var attr = (FileAttributes)(_currentEntry.ExternalAttributes & 0xff);
                return attr.HasFlag(FileAttributes.Directory);
            }

            var name = CurrentFileName();
            return !string.IsNullOrEmpty(name) &&
                   (name[name.Length - 1] == '/' || name[name.Length - 1] == '\\');
        }

        public override void ExtractCurrentFile(Stream extractedFileContents, Action cancellingDelegate)
        {
            if (IsDirectory())
                return;

            using (var entryStream = _currentEntry.Open())
                StreamUtilities.BufferedStreamCopy(entryStream, extractedFileContents);

            CurrentFileExtracted?.Invoke(_currentPosition, _zipArchive.Entries.Count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _zipArchive.Dispose();
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
