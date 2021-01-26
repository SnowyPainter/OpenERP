using BusinessEngine.Sales;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace BusinessEngine.Accounting
{
    /// <summary>
    /// 기록들을 모아 매출과 각종 이익으로 분화해서 저장합니다.
    /// </summary>
    public class Manage
    {
        public Company Owner { get; }
        private List<Debt> debts;
        private List<Bond> bonds;
        private Book accountingBook;

        private float operatingProfit;
        private float salesProfit;
        private float sales;

        public Manage(Company c)
        {
            Owner = c;
            debts = new List<Debt>();
            bonds = new List<Bond>();
            accountingBook = new Book();
        }
        public Manage(List<Debt> debts, List<Bond> bonds, Book book)
        {
            this.debts = debts;
            this.bonds = bonds;
            this.accountingBook = book;
        }
        /// <summary>
        /// 매출을 계산합니다.
        /// </summary>
        /// <param name="soldyear">판매년도</param>
        /// <param name="month">판매월</param>
        /// <returns>매출</returns>
        public float CalculateSales(int soldyear, Month month)
        {
            sales = 0;
            salesProfit = 0;
            accountingBook.Sales.ForEach((s) =>
            {
                if(s.Date.Year == soldyear && s.Date.Month == (int)month)
                {
                    float price = s.Product.Price * s.DiscountRate;
                    salesProfit += (price - s.Product.GetAllCosts()) * s.Qty;
                    sales += price * s.Qty;
                }
            });
            //매출총이익(salesProfit) 계산 완료
            //매출(sales) 계산 완료
            return sales;
        }
        /// <summary>
        /// 영업이익을 계산합니다.
        /// </summary>
        /// <param name="soldyear">참고하여 계산할 분개들의 년도와 같습니다.</param>
        /// <param name="month">참고하여 계산할 분개들의 월과 같습니다.</param>
        /// <param name="salesprofit">기본적으로 0입니다. 아닐경우 이것을 기준으로 영업이익을 계산합니다.</param>
        /// <returns>영업 이익입니다.</returns>
        public float CalculateOperatingProfit(int soldyear, Month month, float salesprofit=0.0f)
        {
            float operatingCost = 0;

            if (salesprofit == 0.0f)
                CalculateSales(soldyear, month);

            accountingBook.Journals.ForEach((j) =>
            {
                switch (j.For)
                {
                    case JournalizingKinds.ETC:
                        break;
                    //
                    //case ... 
                    //
                    case JournalizingKinds.Advert:
                    case JournalizingKinds.Entertaining:
                    case JournalizingKinds.Meeting:
                    case JournalizingKinds.Transportation:
                        operatingCost += j.Amount;
                        break;
                } 
            });
            operatingProfit = salesProfit - operatingCost;
            return operatingProfit;
        }

        /// <summary>
        /// 판매한 상품에 대해 회계장부에 기록합니다.
        /// </summary>
        /// <param name="expectDepDate">판매 대금 입금일</param>
        /// <param name="sellDate">판매일</param>
        /// <param name="product">상품</param>
        /// <param name="seller">판매자</param>
        /// <param name="discountRate">할인율 1=100%</param>
        /// <param name="qty">판매 갯수</param>
        public void AddSale(DateTime expectDepDate, DateTime sellDate, IProduct product, IJournalizeObject seller, int discountRate, int qty)
        {
            accountingBook.Sold(new Sale(expectDepDate, sellDate, product, seller, discountRate, qty));
        }
        /// <summary>
        /// 회계장부에 분개합니다.
        /// </summary>
        /// <param name="moneyAmount">출입금</param>
        /// <param name="debtor">돈의 목적지</param>
        /// <param name="whyType">무슨 종류의 돈</param>
        /// <param name="why">돈의 목적</param>
        public void InsertJournal(float moneyAmount, IJournalizeObject debtor, JournalizingKinds whyType, string why)
        {
            accountingBook.Insert(new Journalizing(moneyAmount, DateTime.Now, Owner, debtor, whyType, why));
        }
        public void InsertJournal(DateTime date, float moneyAmount, IJournalizeObject debtor, JournalizingKinds whyType, string why)
        {
            accountingBook.Insert(new Journalizing(moneyAmount, date, Owner, debtor, whyType, why));
        }
        public List<Journalizing> GetJournalByCompany(List<Journalizing> js, string company) => js.Where(j => j.FromCompany.Name == company).ToList();
        public List<Journalizing> GetJournalBySector(List<Journalizing> js, BusinessSector s) => js.Where(j => j.FromCompany.Sector == s).ToList();
        public List<Journalizing> GetJournal(DateFilter filter)
        {
            List<Journalizing> js = new List<Journalizing>();
            accountingBook.Journals.ForEach((j) =>
            {
                var date = new int[] { j.When.Day, j.When.Month, j.When.Year };
                var filterArr = filter.ToArray();
                int i = 0;
                bool ok = false;
                while (i < date.Length) {
                    if (date[i] >= filterArr[i][0] && date[i] <= filterArr[i][1])
                        ok = true;
                    else
                        ok = false;
                    i++;
                }

                if (ok)
                    js.Add(j);

            });

            return js;
        }

        public float GetAssets()
        {
            float assets = 0;
            debts.ForEach((d) => assets += d.Amount);
            bonds.ForEach((b) => { if (!b.Abandonment) assets += b.Amount;  });

            return assets;
        }
    }
}
