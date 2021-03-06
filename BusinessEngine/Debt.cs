using BusinessEngine.Operating;
using System;
using System.Xml.Serialization;

namespace BusinessEngine
{
    public class Debt
    {
        public AccountingCompany Creditor;
        public DateTime When;
        public DateTime Paydate;
        public float Amount;
        public string Why;

        public Debt(AccountingCompany creditor, DateTime when, DateTime paydate, float amount, string why)
        {
            Creditor = creditor;
            When = when;
            Paydate = paydate;
            Amount = amount;
            Why = why;
        }
    }
}
