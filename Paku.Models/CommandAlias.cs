using System;
using System.Collections.Generic;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # CommandAlias
    /// 
    /// An alias (or aliases) used for this class in a command line interface.
    /// </summary>
    public class CommandAlias : Attribute
    {
        /// <summary>
        /// ## Aliases
        /// 
        /// The list of associated aliases.
        /// </summary>
        public List<string> Aliases { get; set; }

        public CommandAlias(params string[] aliases)
        {
            Aliases = new List<string>(aliases);
        }
    }
}
