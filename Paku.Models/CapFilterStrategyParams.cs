using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Paku.Models
{
    /// <summary>
    /// # CapFilterStrategyParams
    /// 
    /// Parameters to use for the `CapFilterStrategy`.
    /// </summary>
    public class CapFilterStrategyParams
    {
        public enum CapFilterFields
        {
            CreationTime,
            LastWriteTime,
            Name
        };
        
        // should match: "5 cdate desc"
        internal static readonly Regex ParametersRegex = new Regex(@"^(\d+)\s?(cdate|mdate|name)\s?(asc|desc)?$");

        internal static readonly Dictionary<string, CapFilterFields> CapFilterFieldsMap = new Dictionary<string, CapFilterFields>()
        {
            { "cdate", CapFilterFields.CreationTime },
            { "mdate", CapFilterFields.LastWriteTime },
            { "name", CapFilterFields.Name }
        };

        internal static readonly Dictionary<string, SortOrders> SortOrdersMap = new Dictionary<string, SortOrders>()
        {
            { "asc", SortOrders.Asc },
            { "desc", SortOrders.Desc },
            // default to ascending
            { "", SortOrders.Asc }
        };

        /// <summary>
        /// ## CapValue
        /// 
        /// Value to cap at.
        /// </summary>
        public int CapValue { get; set; }

        /// <summary>
        /// ## FilterField
        /// 
        /// The field used in determining the cap.
        /// </summary>
        public CapFilterFields FilterField { get; set; }

        /// <summary>
        /// ## SortOrder
        /// 
        /// The sort order used in determining the cap.
        /// </summary>
        public SortOrders SortOrder { get; set; }

        private void UpdateFromString(string str)
        {
            // force lowercase
            str = str.ToLower();
            Match match = ParametersRegex.Match(str);

            if (match.Success)
            {
                string strCapValue = match.Groups[1].Value;
                string strField = match.Groups[2].Value;
                string strSortOrder = match.Groups[3].Value;

                this.CapValue = Int32.Parse(strCapValue);
                this.FilterField = CapFilterFieldsMap[strField];
                this.SortOrder = SortOrdersMap[strSortOrder];
            }
            else
            {
                throw new ArgumentException("Input string is invalid.");
            }
        }

        public CapFilterStrategyParams()
        {
        }

        public CapFilterStrategyParams(string parms)
        {
            UpdateFromString(parms);
        }
    }
}
