using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using System.Reflection;

namespace Paku.Models
{
    [CommandAlias("cap")]
    [Description("Caps the file list to the specified number, returning all items beyond the cap. Available fields to check are: mdate, cdate, and name. Ex: '5 cdate desc' to cap to the five most recent files.")]
    public class CapFilterStategy : IFilterStrategy
    {
        public IList<VirtualFileInfo> Filter(IList<VirtualFileInfo> files, string parameters)
        {
            CapFilterStrategyParams parms = new CapFilterStrategyParams(parameters);
            PropertyInfo propertyInfo = typeof(VirtualFileInfo).GetProperty(parms.FilterField.ToString());

            // sort the files based on the field and sort order
            IOrderedEnumerable<VirtualFileInfo> sorted;

            if (parms.SortOrder == SortOrders.Asc)
            {
                sorted = files.OrderBy(f => propertyInfo.GetValue(f));
            }
            else
            {
                sorted = files.OrderByDescending(f => propertyInfo.GetValue(f));
            }

            // return all files beyond the cap
            return sorted.Skip(parms.CapValue).ToList();
        }
    }
}
