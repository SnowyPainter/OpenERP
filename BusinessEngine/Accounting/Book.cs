using BusinessEngine.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessEngine.Accounting
{
    /// <summary>
    /// 관리회계에서 분개와 판매를 기록합니다.
    /// </summary>
    public class Book
    {
        public List<Journalizing> Journals { get; private set; }

        public List<Sale> Sales { get; private set; }

        public Book() { Sales = new List<Sale>(); }

        public void Sold(Sale sold)
        {
            Sales.Add(sold);
        }

        public void Insert(Journalizing j)
        {
            Journals.Add(j);
        }
        public void SortJournals()
        {
            Journals = Journals.OrderBy((j) => j.When).ToList();
        }
        
    }
}
