﻿using BusinessEngine.Operating;
using System;
using System.Xml.Serialization;

namespace BusinessEngine
{
    public class Bond
    {
        public AccountingCompany Who;
        public DateTime When;
        public DateTime Paydate;
        public float Amount;
        public bool Abandonment = false;
    }
}
