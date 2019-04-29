using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # ISelectionStrategy
    /// 
    /// Interface for all selection strategies: any piece of logic used to select files from a directory for processing.
    /// </summary>
    public interface ISelectionStrategy
    {
        IList<VirtualFileInfo> Select(DirectoryInfo dir);
    }
}
