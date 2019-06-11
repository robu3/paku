using System;
using System.Collections.Generic;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # PakuResult
    /// 
    /// The result of a "paku" operation.
    /// </summary>
    public class PakuResult
    {
        /// <summary>
        /// ## Success
        /// 
        /// True if the operation was successful (no errors encountered).
        /// </summary>
        public bool Success
        {
            get
            {
                return Error == null;
            }
        }

        /// <summary>
        /// ## Error
        /// 
        /// Any exception that was encountered during processing.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// ## RemovedFiles
        /// 
        /// All files that were successfully removed.
        /// </summary>
        public IList<VirtualFileInfo> RemovedFiles { get; set; }

        /// <summary>
        /// ## CreatedFiles
        /// 
        /// Any files that were successfully created, e.g., zip archives.
        /// </summary>
        public IList<VirtualFileInfo> CreatedFiles { get; set; }

        public PakuResult()
        {
            this.RemovedFiles = new List<VirtualFileInfo>();
            this.CreatedFiles = new List<VirtualFileInfo>();
        }
    }
}
