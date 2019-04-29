using System;
using System.Collections.Generic;
using System.Text;

namespace Paku.Models
{
    /// <summary>
    /// # AgeFilterStrategy
    /// 
    /// A file age-based filter (created or last modified).
    /// </summary>
    public class AgeFilterStrategy : IFilterStrategy
    {
        public IList<VirtualFileInfo> Filter(IList<VirtualFileInfo> files, string parameters)
        {
            List<VirtualFileInfo> results = new List<VirtualFileInfo>();
            AgeFilterStrategyParams parms = new AgeFilterStrategyParams(parameters);

            foreach (VirtualFileInfo fi in files)
            {
                // calculate the time unit difference
                TimeSpan dateDiff = DateTime.Now - (parms.FileDate == FileDateProperties.CreateDate ? fi.CreationTime : fi.LastWriteTime);

                // edge case: if future-dated file (why would this happen?), ignore
                if (dateDiff.TotalMilliseconds < 0)
                {
                    continue;
                }

                // convert all values to seconds (smallest unit) for accuracy & simplicity
                double threshold = parms.UnitValue;

                switch (parms.TimeUnit)
                {
                    case TimeUnits.Minute:
                        threshold *= 60;
                        break;
                    case TimeUnits.Hour:
                        threshold *= 3600;
                        break;
                    case TimeUnits.Day:
                        threshold *= 86400;
                        break;
                }

                // compare with operators and value passed in
                if (dateDiff.TotalSeconds == 0 && (parms.Operator == Operators.Equals || parms.Operator == Operators.GreaterThanEquals || parms.Operator == Operators.LessThanEquals))
                {
                    results.Add(fi);
                }
                else if (dateDiff.TotalSeconds > threshold && (parms.Operator == Operators.GreaterThan || parms.Operator == Operators.GreaterThanEquals))
                {
                    results.Add(fi);
                }
                else if (dateDiff.TotalSeconds < threshold && (parms.Operator == Operators.LessThan || parms.Operator == Operators.LessThanEquals))
                {
                    results.Add(fi);
                }
            }

            return results;
        }

        public AgeFilterStrategy()
        {
        }
    }
}
