using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Paku.Models
{
    /// <summary>
    /// # VirtualFileInfo
    /// 
    /// A conceptual file entry that may or may not exist on disk.
    /// Think of it as a readonly wrapper for [FileInfo](https://docs.microsoft.com/en-us/dotnet/api/system.io.fileinfo?view=netstandard-2.0).
    /// </summary>
    public class VirtualFileInfo
    {
        public string FullName { get; private set; }
        public string Name { get; private set; }
        public string DirectoryName { get; private set; }
        public long Length { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        /// <summary>
        /// ## UpdateFullName
        /// 
        /// Update to the specified full file name, e.g., C:\foo\bar.txt; additionally, parse and
        /// update Name and DirectoryName to the correct values (bar.txt and C:\foo, respectively).
        /// </summary>
        /// <param name="fullName"></param>
        private void UpdateFullName(string fullName)
        {
            // these methods return empty string if the the information is missing
            // and null if the name is null or invalid (like a root directory)
            string name = Path.GetFileName(fullName);
            string dir = Path.GetDirectoryName(fullName);

            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(dir))
            {
                this.FullName = fullName;
                this.DirectoryName = dir;
                this.Name = name;
            }
            else
            {
                throw new ArgumentException("fullName does not appear to be a valid full file name.");
            }
        }

        public VirtualFileInfo(FileInfo fi)
        {
            this.FullName = fi.FullName;
            this.Name = fi.Name;
            this.DirectoryName = fi.DirectoryName;

            this.Length = fi.Length;
            this.CreationTime = fi.CreationTime;
            this.LastWriteTime = fi.LastWriteTime;
        }

        public VirtualFileInfo(string fullName)
        {
            UpdateFullName(fullName);
        }
    }
}
