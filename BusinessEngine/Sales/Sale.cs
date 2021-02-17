using BusinessEngine.Accounting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BusinessEngine.Sales
{
    /// <summary>
    /// ExpctedDepositDate 입금예정일 혹은, 입금일입니다. 바로 채권회수를 했다면 판매일과 날짜가 동일해야합니다.
    /// 분개에 기록되었을때, Sale과 다르면 안됩니다.
    /// </summary>
    [Serializable]
    public class Sale:INotifyPropertyChanged
    {
        private int dr;
        private int qty;
        private IJournalizeObject seller;
        private DateTime date, expectDeposit;
        private IProduct product;

        public int DiscountRate
        {
            get { return dr; }
            set { dr = value; NotifyPropertyChanged("DiscountRate"); }
        }
        public int Qty
        {
            get { return qty; }
            set { qty = value; NotifyPropertyChanged("Qty"); }
        }
        public IJournalizeObject Seller
        {
            get { return seller; }
            set { seller = value; NotifyPropertyChanged("Seller"); }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; NotifyPropertyChanged("Date"); }
        }
        public DateTime ExpectedDepositDate
        {
            get { return expectDeposit; }
            set { expectDeposit = value; NotifyPropertyChanged("ExpectedDepositDate"); }
        }
        public IProduct Product
        {
            get { return product; }
            set { product = value; NotifyPropertyChanged("Product"); }
        }

        public Sale() { }
        public Sale(DateTime expectDepDate, DateTime sellDate, IProduct product, IJournalizeObject seller, int discountRate, int qty)
        {
            DiscountRate = discountRate;
            Qty = qty;
            Seller = seller;
            ExpectedDepositDate = expectDepDate;
            Product = product;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
