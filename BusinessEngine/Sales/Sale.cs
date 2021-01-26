using BusinessEngine.Accounting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Sales
{
    /// <summary>
    /// ExpctedDepositDate 입금예정일 혹은, 입금일입니다. 바로 채권회수를 했다면 판매일과 날짜가 동일해야합니다.
    /// 분개에 기록되었을때, Sale과 다르면 안됩니다.
    /// </summary>
    public class Sale
    {
        public int DiscountRate, Qty;
        public IJournalizeObject Seller;
        public DateTime Date;
        public DateTime ExpectedDepositDate;
        public IProduct Product;

        public Sale(DateTime expectDepDate, DateTime sellDate, IProduct product, IJournalizeObject seller, int discountRate, int qty)
        {
            DiscountRate = discountRate;
            Qty = qty;
            Seller = seller;
            ExpectedDepositDate = expectDepDate;
            Product = product;
        }
    }
}
