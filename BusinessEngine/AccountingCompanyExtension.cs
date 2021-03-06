using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine
{
    public static class AccountingCompanyExtension
    {
        public static AccountingCompany Clone(this AccountingCompany ac) => new AccountingCompany { Name = ac.Name, Note = ac.Note, WarningPoint = ac.WarningPoint };
    }
}
