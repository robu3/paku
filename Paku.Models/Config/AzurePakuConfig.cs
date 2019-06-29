using System;
using System.Collections.Generic;
using System.Text;

namespace Paku.Models.Config
{
    /// <summary>
    /// # AzurePakuConfig
    /// 
    /// Configuration settings for the `AzureBlobPakuStrategy`.
    /// </summary>
    public class AzurePakuConfig
    {
        /// <summary>
        /// ## ConnectionString
        /// 
        /// Azure storage account connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// ## Container
        /// 
        /// Name of the container to upload to.
        /// The container will be created if it does not already exist.
        /// </summary>
        public string Container { get; set; }
    }
}
