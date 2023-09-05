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
        public abstract void Add(Stream fileToAdd, string fileName, DateTime modificationTime, Action cancellingDelegate);

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
            var longPathToArchive = StringUtility.ToLongWindowsPath(pathToArchive);
            if (!Directory.Exists(longPathToArchive))
                throw new FileNotFoundException("The path " + pathToArchive + " does not exist");

            // We look recursively and do not use Directory.GetDirectories and Directory.GetFiles with
            // AllDirectories options because in .NET 4.8 they do not enumerate all elements if they
            // have paths longer than 260 characters (248 for directories).
            // If moving to .NET Core, please consider reverting this.
            PopulateArchive(pathToArchive, pathToArchive, cancellingDelegate, progressDelegate);
        }

        /// <summary>
        /// Populate the archive by recursively calling the overriden <see cref="Add"/> and <see cref="AddDirectory"/>.
        /// </summary>
        /// <param name="pathToArchive">The path to the root of the folder we're archiving</param>
        /// <param name="pathToCurrentDirectory">Keeps track of the current directory we're archiving. In the first recursive call it should be the same as <see cref="pathToArchive"/></param>
        /// <param name="cancellingDelegate">Action cal led for cancelling</param>
        /// <param name="progressDelegate">Action for reporting progress. Method will populate its parameter with the current progress of the recursive operation</param>
        /// <param name="totalProgressIncrease">Total progress that needs to be added for archiving this directory. In the first recursive call it should be 100. If the folder we're adding should add 18 percentage points to the total progress, set this value to 18.</param>
        /// <param name="progressOffset">Offset to the progress. This is added to <see cref="totalProgressIncrease"/> when setting the progress for this directory. If this folder should add 18 percentage points to the total progress, but it's for a folder past the 50% mark of the total progress (i.e.: completing this folder should set the total to 68), set this value to 50.</param>
        /// <exception cref="FileNotFoundException">A directory could not be found.</exception>
        private void PopulateArchive(string pathToArchive, string pathToCurrentDirectory, Action cancellingDelegate = null, Action<int> progressDelegate = null, int totalProgressIncrease = 100, int progressOffset = 0)
        {
            cancellingDelegate?.Invoke();

            var longPathToArchive = StringUtility.ToLongWindowsPath(pathToArchive);

            if (!Directory.Exists(longPathToArchive))
                throw new FileNotFoundException("The path " + pathToArchive + " does not exist");

            AddFiles(longPathToArchive, pathToCurrentDirectory, cancellingDelegate);

            var directories = Directory.GetDirectories(pathToCurrentDirectory);

            if (directories.Length == 0)
            {
                progressDelegate?.Invoke(totalProgressIncrease + progressOffset);
                return;
            }

            var progressIncrement = totalProgressIncrease / directories.Length;
            
            for (var j = 0; j < directories.Length; j++)
            {
                var subdirectoryPath = directories[j];
                
                AddDirectory(CleanRelativePathName(pathToArchive, subdirectoryPath), Directory.GetCreationTime(subdirectoryPath));

                // Add the remainder to the offset of the last subdirectory
                var remainder = j == directories.Length - 1 ? totalProgressIncrease % directories.Length : 0;
                var newProgressOffset = j * progressIncrement + progressOffset + remainder;

                PopulateArchive(pathToArchive, subdirectoryPath, cancellingDelegate, progressDelegate,progressIncrement, newProgressOffset);
            }
        }

        private void AddFiles(string pathToArchive, string currentDirectory, Action cancellingDelegate)
        {
            var files = Directory.GetFiles(StringUtility.ToLongWindowsPath(currentDirectory));
            foreach (var filePath in files)
            {
                cancellingDelegate?.Invoke();

                using (var fs = File.OpenRead(StringUtility.ToLongWindowsPath(filePath)))
                {
                    Add(fs, CleanRelativePathName(pathToArchive, filePath), File.GetCreationTime(StringUtility.ToLongWindowsPath(filePath)), cancellingDelegate);
                }
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
