using System;
using System.Collections.Generic;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # IFilterStrategy
    /// 
    /// Interface for strategies used to filter a list of files.
    /// </summary>
    public interface IFilterStrategy
    {
        /// <summary>
        /// ## Filter
        /// 
        /// Filters the specified file list.
        /// </summary>
        /// <param name="files">List of files to filter.</param>
        /// <param name="parameters">Parameters string.</param>
        /// <returns>The files filtered out (to be kept).</returns>
        IList<VirtualFileInfo> Filter(IList<VirtualFileInfo> files, string parameters);
    }
}
