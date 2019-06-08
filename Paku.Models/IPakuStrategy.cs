using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # IPakuStrategy
    /// 
    /// Interface for any パクパク ("pakupaku") strategy: a bit of code that "eats" files by deleting and/or archiving them.
    /// </summary>
    public interface IPakuStrategy
    {
        /// <summary>
        /// ## Eat
        /// 
        /// Eat the specified files by deleting, compressing, etc.
        /// </summary>
        /// <param name="files"></param>
        /// <returns>Result indicating success/failure, files removed, and files created.</returns>
        PakuResult Eat(DirectoryInfo dir, IList<VirtualFileInfo> files, string parameters);
    }
}
