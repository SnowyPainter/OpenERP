using BusinessEngine.Accounting;
using System;
using System.Collections.Generic;
using System.Text;

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

        private Finance finance;
        private Manage manageAccounting;

        public Company(string name, BusinessSector model)
        {
            Name = name;
            Sector = model;

            manageAccounting = new Manage(this);
            finance = new Finance(this);
            
        }
    }
}
