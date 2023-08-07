/* Copyright (c) Cloud Software Group, Inc. 
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
using System.IO;

namespace XenCenterLib.Archive
{
    public abstract class ArchiveWriter : IDisposable
    {
        public abstract void Add(Stream filetoAdd, string fileName, DateTime modificationTime, Action cancellingDelegate);

        public virtual void SetBaseStream(Stream outputStream)
        {
            throw new NotImplementedException();
        }

        public abstract void AddDirectory(string directoryName, DateTime modificationTime);

        /// <summary>
        /// Disposal hook
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing){ }

        public void CreateArchive(string pathToArchive, Action cancellingDelegate = null, Action<int> progressDelegate = null)
        {
            if (!Directory.Exists(StringUtility.ToLongWindowsPath(pathToArchive)))
                throw new FileNotFoundException("The path " + pathToArchive + " does not exist");

            // We look recursively and do not use Directory.GetDirectories and Directory.GetFiles with
            // AllDirectories options because in .NET 4.8 they do not enumerate all elements if they
            // have paths longer than 260 characters (240 for directories).
            // If moving to .NET Core, please consider reverting this .
            AddFiles(StringUtility.ToLongWindowsPath(pathToArchive), cancellingDelegate);
            var directories = Directory.GetDirectories(StringUtility.ToLongWindowsPath(pathToArchive));
            for (var j = 0; j < directories.Length; j++)
            {
                string dirPath = directories[j];
                cancellingDelegate?.Invoke();
                AddDirectory(CleanRelativePathName(pathToArchive, dirPath), Directory.GetCreationTime(dirPath));
                AddFiles(dirPath, cancellingDelegate);
                TraverseDirectories(dirPath, pathToArchive, cancellingDelegate);
                progressDelegate?.Invoke( (int)100.0 * j / directories.Length);
            }
        }

        private void AddFiles(string pathToArchive, Action cancellingDelegate)
        {
            var files = Directory.GetFiles(pathToArchive);
            foreach (var filePath in files)
            {
                cancellingDelegate?.Invoke();

                using (var fs = File.OpenRead(StringUtility.ToLongWindowsPath(filePath)))
                {
                    Add(fs, CleanRelativePathName(pathToArchive, filePath), File.GetCreationTime(StringUtility.ToLongWindowsPath(filePath)), cancellingDelegate);
                }
            }
        }

        private void TraverseDirectories(string currentDirectory, string pathToArchive, Action cancellingDelegate )
        {
            var subdirectories = Directory.GetDirectories(StringUtility.ToLongWindowsPath(currentDirectory));

            foreach (var subdirectory in subdirectories)
            {
                var longPathSubdirectory = StringUtility.ToLongWindowsPath(subdirectory);
                cancellingDelegate?.Invoke();
                AddDirectory(CleanRelativePathName(pathToArchive, subdirectory), Directory.GetCreationTime(longPathSubdirectory));
                AddFiles(longPathSubdirectory, cancellingDelegate);
                TraverseDirectories(longPathSubdirectory, pathToArchive, cancellingDelegate);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);   
        }

        private string CleanRelativePathName(string rootPath, string pathName)
        {
            return pathName
                .Replace(@"\\?\", string.Empty)
                .Replace(rootPath, string.Empty)
                .Replace('\\', '/')
                .TrimStart('/');
        }
    }
}
