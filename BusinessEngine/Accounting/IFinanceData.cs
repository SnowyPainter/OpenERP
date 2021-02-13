using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Accounting
{
    public interface IFinanceData
    {
        float Assets { get; set; }

        List<Debt> Debts { get; set; }
        List<Bond> Bonds { get; set; }
    }
}
