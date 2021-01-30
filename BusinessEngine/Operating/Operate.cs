using BusinessEngine.Accounting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Operating
{
    public class Operate
    {
        public Company Target { get; }
        public FinanceManage Finance { get; private set; } = null;
        public Operate(Company com)
        {
            Target = com;
        }
        public void SetFinance(FinanceManage f)
        {
            Finance = f;
        }
    }
}
