using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Paku.Models
{
    /// <summary>
    /// # AgeFilterStrategyParams
    /// 
    /// Parameters for the age-based filter.
    /// </summary>
    public class AgeFilterStrategyParams
    {
        // should match: "cdate>8h" or "mdate <= 30m"
        internal static readonly Regex ParametersRegex = new Regex(@"^(cdate|mdate)\s?([><=]=*)\s?(\d+)([smhd])$");

        internal static readonly Dictionary<string, FileDateProperties> FileDatesMap = new Dictionary<string, FileDateProperties>()
        {
            { "cdate", FileDateProperties.CreateDate },
            { "mdate", FileDateProperties.ModifiedDate }
        };

        internal static readonly Dictionary<string, Operators> OperatorsMap = new Dictionary<string, Operators>()
        {
            { ">", Operators.GreaterThan },
            { ">=", Operators.GreaterThanEquals },
            { "<", Operators.LessThan },
            { "<=", Operators.LessThanEquals },
            { "=", Operators.Equals }
        };

        internal static readonly Dictionary<string, TimeUnits> TimeUnitsMap = new Dictionary<string, TimeUnits>()
        {
            { "s", TimeUnits.Second },
            { "m", TimeUnits.Minute },
            { "h", TimeUnits.Hour },
            { "d", TimeUnits.Day }
        };

        public FileDateProperties FileDate { get; set; }
        public Operators Operator { get; set; }
        public TimeUnits TimeUnit { get; set; }
        public int UnitValue { get; set; }

        private void UpdateFromString(string str)
        {
            // force lowercase
            str = str.ToLower();
            Match match = ParametersRegex.Match(str);

            if (match.Success)
            {
                string strFileDate = match.Groups[1].Value;
                string strOperator = match.Groups[2].Value;
                string strUnitValue = match.Groups[3].Value;
                string strTimeUnit = match.Groups[4].Value;

                this.FileDate = FileDatesMap[strFileDate];
                this.Operator = OperatorsMap[strOperator];
                this.UnitValue = Int32.Parse(strUnitValue);
                this.TimeUnit = TimeUnitsMap[strTimeUnit];
            }
            else
            {
                throw new ArgumentException("Input string is invalid.");
            }
        }

        public AgeFilterStrategyParams()
        {
        }

        public AgeFilterStrategyParams(string parms)
        {
            UpdateFromString(parms);
        }
    }
}
