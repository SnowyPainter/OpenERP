using BusinessEngine.Accounting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace BusinessEngine
{
    public enum BusinessSector
    {
        Wholesale,
        Retail,
        It,
        Freelancer,
        Bank,
    }

    public class Company:IJournalizeObject
    {
        public string Name { get; }
        public BusinessSector Sector { get; }

        //관리회계 -> 재무회계 -> 세무회계가 기초다.
        private FinanceManage finance;
        

        public Company(string name, BusinessSector model)
        {
            Name = name;
            Sector = model;

            finance = new FinanceManage(this);
        }
    }
}
