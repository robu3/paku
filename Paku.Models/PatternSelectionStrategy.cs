using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Paku.Models
{
    /// <summary>
    /// # PatternSelectionStrategy
    /// 
    /// Selects files using a standard terminal glob matching pattern, e.g., "*.txt" or "*.*"
    /// </summary>
    [CommandAlias("pattern")]
    [Description("Uses a standard glob matching pattern (ex: *.txt) to select files.")]
    public class PatternSelectionStrategy : ISelectionStrategy
    {
        /// <summary>
        /// ## Select
        /// 
        /// Selects files that match the specified selection pattern (e.g., "*.txt").
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IList<VirtualFileInfo> Select(DirectoryInfo dir, string selectionPattern)
        {
            List<VirtualFileInfo> results = new List<VirtualFileInfo>();

            foreach (FileInfo fi in dir.EnumerateFiles(selectionPattern))
            {
                results.Add(new VirtualFileInfo(fi));
            }

            return results;
        }

        public PatternSelectionStrategy()
        {
        }
    }
}
