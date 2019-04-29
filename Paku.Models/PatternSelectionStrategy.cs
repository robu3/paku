using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Paku.Models
{
    /// <summary>
    /// # PatternSelectionStrategy
    /// 
    /// Selects files using a standard terminal matching pattern, e.g., "*.txt" or "*.*"
    /// </summary>
    public class PatternSelectionStrategy : ISelectionStrategy
    {
        /// <summary>
        /// ## SelectionPattern
        /// 
        /// The pattern (e.g., "*.txt") used to select files.
        /// </summary>
        public string SelectionPattern { get; set; }

        /// <summary>
        /// ## Select
        /// 
        /// Selects files that match the `SelectionPattern`.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IList<VirtualFileInfo> Select(DirectoryInfo dir)
        {
            List<VirtualFileInfo> results = new List<VirtualFileInfo>();

            foreach (FileInfo fi in dir.EnumerateFiles(SelectionPattern))
            {
                results.Add(new VirtualFileInfo(fi));
            }

            return results;
        }

        public PatternSelectionStrategy(string pattern)
        {
            this.SelectionPattern = pattern;
        }
    }
}
