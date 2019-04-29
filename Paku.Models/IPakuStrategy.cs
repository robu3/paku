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
        IList<VirtualFileInfo> Eat(IList<VirtualFileInfo> files);
    }
}
