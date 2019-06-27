using System;
using System.Collections.Generic;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # PgpPakuStrategyParams
    /// </summary>
    public class PgpPakuStrategyParams
    {
        /// <summary>
        /// ## PgpPublicKeyPath
        /// 
        /// Path to the public key file.
        /// </summary>
        public string PgpPublicKeyPath { get; set; }

        /// <summary>
        /// ## OutputFilePrefix
        /// 
        /// Prefix added to generated PGP file.
        /// </summary>
        public string OutputFilePrefix { get; set; }

        private void UpdateFromString(string parameters)
        {
            // parameters string should be in the form: public_key.asc|prefix (optional)
            string[] parts = parameters.Split('|');

            if (parts.Length == 0 || parts.Length > 2)
            {
                throw new ArgumentException("Input string is invalid.");
            }

            PgpPublicKeyPath = parts[0].Trim();

            if (parts.Length == 2)
            {
                OutputFilePrefix = parts[1].Trim();
            }
        }

        public PgpPakuStrategyParams(string parameters) : base()
        {
            UpdateFromString(parameters);
        }

        public PgpPakuStrategyParams()
        {
            OutputFilePrefix = "paku";
        }
    }
}
