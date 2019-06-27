using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # AllFilterStrategy
    /// 
    /// Returns all files passed in (the "non-filter" filter).
    /// </summary>
    [CommandAlias("all")]
    [Description("This filter includes all files.")]
    public class AllFilterStrategy : IFilterStrategy
    {
        public IList<VirtualFileInfo> Filter(IList<VirtualFileInfo> files, string parameters)
        {
            return files;
        }
    }
}
