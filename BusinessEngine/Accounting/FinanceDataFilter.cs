using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessEngine.Accounting
{
    public class FinanceDataFilter
    {
        public Dictionary<FinanceDataProperty, bool> FilterFlag = new Dictionary<FinanceDataProperty, bool>();
        public FinanceDataFilter(params FinanceDataProperty[] filterList)
        {
            var list = filterList.ToList();
            if (list.Exists((f) => f == FinanceDataProperty.All))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    FilterFlag.Add((FinanceDataProperty)i, true);
                }
                return;
            }
            var sortedList = list.OrderByDescending((f) => (int)f);

            for (int i = 0; i < sortedList.Count(); i++)
            {
                foreach (var f in sortedList)
                {
                    FilterFlag.Add((FinanceDataProperty)i, ((int)f == i) ? true : false);
                }
            }
        }
    }
}
