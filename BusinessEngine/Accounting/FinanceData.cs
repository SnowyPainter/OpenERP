using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Accounting
{
    public class FinanceData:IFinanceData
    {
        public float Assets { get; set; }

        public List<Debt> Debts { get; set; }
        public List<Bond> Bonds { get; set; }

        public FinanceData() { }
    }
}
