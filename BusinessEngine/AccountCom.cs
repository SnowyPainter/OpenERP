using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine
{
    public class AccountCom:Company
    {
        public AccountCom(string name, BusinessSector sector) : base(name, sector) { }

        public Warning WarningPoint { get; private set; } = Warning.None;
        public void SetWarningPoint(Warning w) => WarningPoint = w;

    }
}