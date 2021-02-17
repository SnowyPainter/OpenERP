﻿using BusinessEngine.Sales;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BusinessEngine.Accounting
{
    /// <summary>
    /// 관리회계에서 분개와 판매를 기록합니다.
    /// </summary>
    [Serializable]
    public class Book
    {
        public ObservableCollection<Journalizing> Journals { get; set; }

        public ObservableCollection<Sale> Sales { get; set; }

        public Book() { Sales = new ObservableCollection<Sale>(); }

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
            Journals = new ObservableCollection<Journalizing>(Journals.OrderBy((j) => j.When));
        }
        
    }
}
