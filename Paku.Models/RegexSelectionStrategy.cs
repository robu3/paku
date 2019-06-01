using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Paku.Models
{
    /// <summary>
    /// # RegexSelectionStrategy
    /// 
    /// Selects files using a regular expression.
    /// </summary>
    [CommandAlias("regex")]
    [Description("Uses a regular expression to select files.")]
    public class RegexSelectionStrategy : ISelectionStrategy
    {
        /// <summary>
        /// ## Select
        /// 
        /// Selects files that the specified regular expression is able to match.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IList<VirtualFileInfo> Select(DirectoryInfo dir, string regexString)
        {
            List<VirtualFileInfo> results = new List<VirtualFileInfo>();
            Regex selectionRegex = new Regex(regexString);

            foreach (FileInfo fi in dir.EnumerateFiles())
            {
                if (selectionRegex.IsMatch(fi.Name))
                {
                    results.Add(new VirtualFileInfo(fi));
                }
            }

            return results;
        }

        public RegexSelectionStrategy()
        {
        }
    }
}
