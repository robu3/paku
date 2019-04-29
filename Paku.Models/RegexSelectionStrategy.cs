using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Paku.Models
{
    public class RegexSelectionStrategy : ISelectionStrategy
    {
        /// <summary>
        /// ## SelectionRegex
        /// 
        /// The regular expression used to select files.
        /// </summary>
        public Regex SelectionRegex { get; set; }

        /// <summary>
        /// ## Select
        /// 
        /// Selects files that `SelectionRegex` is able to match.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IList<VirtualFileInfo> Select(DirectoryInfo dir)
        {
            List<VirtualFileInfo> results = new List<VirtualFileInfo>();

            foreach (FileInfo fi in dir.EnumerateFiles())
            {
                if (SelectionRegex.IsMatch(fi.Name))
                {
                    results.Add(new VirtualFileInfo(fi));
                }
            }

            return results;
        }

        public RegexSelectionStrategy(string regex)
        {
            this.SelectionRegex = new Regex(regex);
        }
    }
}
