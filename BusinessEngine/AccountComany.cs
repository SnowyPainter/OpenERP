using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine
{
    /// <summary>
    /// 거래처입니다
    /// </summary>
    public class AccountComany:Company
    {
        public AccountComany(string name, BusinessSector sector) : base(name, sector) { }

        public Warning WarningPoint { get; private set; } = Warning.None;
        public void SetWarningPoint(Warning w) => WarningPoint = w;

    }
}