using System;
using System.Collections.Generic;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # IPakuStrategy
    /// 
    /// Interface for any "pakupaku" strategy: a bit of code that "eats" files by deleting and/or archiving them.
    /// </summary>
    public interface IPakuStrategy
    {
        /// <summary>
        /// ## Eat
        /// 
        /// Eat the specified files by deleting, compressing, etc.
        /// </summary>
        /// <param name="files"></param>
        /// <returns>Any files generated and/or remaining after consumption.</returns>
        PakuResult Eat(IList<VirtualFileInfo> files);
    }
}
