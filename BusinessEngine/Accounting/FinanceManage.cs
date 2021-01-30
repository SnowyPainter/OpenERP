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
    public class FinanceManage
    {
        public Company Owner { get; }

        /// <summary>
        /// 예상 가능 운전자금
        /// </summary>
        public float ReserveAssets { get; private set; }
        public float Assets { get; private set; }
        public List<AccountCom> WarnCompany;

        private List<Debt> debts;
        private List<Bond> bonds;
        private Book accountingBook;

        private float operatingProfit;
        private float salesProfit;
        private float sales;

        public FinanceManage(Company c)
        {
            Owner = c;
            debts = new List<Debt>();
            bonds = new List<Bond>();
            accountingBook = new Book();
            WarnCompany = new List<AccountCom>();
        }        
        public FinanceManage(List<Debt> debts, List<Bond> bonds, Book book, List<AccountCom> accounts)
        {
            this.debts = debts;
            this.bonds = bonds;
            this.accountingBook = book;
            this.WarnCompany = accounts;
        } 
        /// <summary>
        /// 채권회수가 어려워 보이는 회사를 반환합니다.
        /// </summary>
        /// <param name="bonds"></param>
        /// <returns></returns>
        public List<Bond> GetBondsFromWarnCompany(List<Bond> bonds)
        {
            List<Bond> dangerBond = new List<Bond>();
            
            bonds.ForEach((b) => {
                WarnCompany.ForEach((c) => {
                    if (b.Who.Name == c.Name)
                        dangerBond.Add(b);
                });
            });
            return dangerBond;
        }

        public void RemoveWarningCompany(AccountCom com)
        {
            if (WarnCompany.Contains(com))
                WarnCompany.Remove(com);
        }
        public void AddWarningCompany(AccountCom com)
        {
            if(!WarnCompany.Contains(com))
                WarnCompany.Add(com);
        }

        /// <summary>
        /// ReserveMonths에 대하여 운전 자금을 예상합니다.
        /// 영업비용, 부채를 제외하고 남은 영업이익과 채권 회수금을 포함합니다.
        /// </summary>
        /// <param name="reserveMonths">지금으로부터 ~개월 후</param>
        /// <returns></returns>
        public float OrganizeOperatingCapital(int reserveMonths = 3)
        {
            var now = DateTime.Now;
            var debts = WarningDebts(reserveMonths).Select((d) => d.Amount).Sum();
            float opers = 0;
            if (now.Month + reserveMonths > 12)
            { 
                var opersDf = new DateFilter();
                opers += GetJournal(opersDf.SetYear(now.Year + 1 + reserveMonths/12).SetMonthTerm(1, 12 - Math.Abs(now.Month - reserveMonths) % 12)).Select((j) => j.Amount).Sum();
                opers += GetJournal(opersDf.SetYear(now.Year).SetMonthTerm(now.Month, 12)).Select((j) => j.Amount).Sum();
            }
            else
            {
                var opersDf = new DateFilter();
                opers += GetJournal(opersDf.SetYear(now.Year).SetMonthTerm(now.Month, now.Month + reserveMonths)).Select((j) => j.Amount).Sum();
            }
            var bonds = BondsSoonCash(reserveMonths).Select((b)=>b.Amount).Sum();
            var profit = accountingBook.Sales.Where((s) => s.ExpectedDepositDate.Year == now.Year && //매출총이익
                s.ExpectedDepositDate.Month >= now.Month - reserveMonths)
                .Select((s)=>(s.Product.Price*s.DiscountRate - s.Product.GetAllCostsPrice())*s.Qty).Sum();

            return bonds + profit - (debts + opers);
        }

        /// <summary>
        /// 몇달 내에 채무 이행해야 하는 채무 목록입니다.
        /// 채권자가 파산할 경우도 있으나, 일단 목록에 대해 예비 운전 자금에서 제합니다.
        /// </summary>
        /// <param name="allowMonths"></param>
        /// <returns></returns>
        public IEnumerable<Debt> WarningDebts(int allowMonths = 2)
        {
            var now = DateTime.Now;
            return debts.Where((d) => now.Year >= d.Paydate.Year && now.Month >= d.Paydate.Month
                || now.Year == d.Paydate.Year && now.Month >= d.Paydate.Month - allowMonths);
        }

        /// <summary>
        /// 몇달 내에 채권 회수 할 수 있는 채권 목록입니다.
        /// 실제로 회수 가능할지는 알수 없음으로, 이 목록을 기반으로 운전 자금을 편성하면 안됩니다.
        /// 목록에 대해서 예비 운전 자금으로 분류합니다.
        /// </summary>
        /// <param name="allowMonths"></param>
        /// <returns>채권 목록</returns>
        public IEnumerable<Bond> BondsSoonCash(int allowMonths = 2)
        {
            var now = DateTime.Now;
            return bonds.Where((b) => now.Year >= b.Paydate.Year && now.Month >= b.Paydate.Month
                || now.Year == b.Paydate.Year && now.Month >= b.Paydate.Month - allowMonths);
        }

        /// <summary>
        /// 채무 리스트를 XML로 변환합니다.
        /// </summary>
        /// <returns></returns>
        public string ToDebtsXml()
        {
            return IO.XMLWriter.Serialize(debts);
        }
        /// <summary>
        /// 채권 리스트를 XML로 변환합니다.
        /// </summary>
        /// <returns></returns>
        public string ToBondsXml()
        {
            return IO.XMLWriter.Serialize(bonds);
        }
        /// <summary>
        /// 분개가 정리된 회계장부를 XML로 변환합니다.
        /// </summary>
        /// <returns></returns>
        public string ToBookXml()
        {
            return IO.XMLWriter.Serialize(accountingBook);
        }


        /// <summary>
        /// CalculateSales에서 계산한 매출총이익 결과를 반환합니다.
        /// 만약 다른 날짜의 매출총이익을 계산하고 싶다면 CalculateSales를 먼저 해준후 호출하세요.
        /// </summary>
        /// <returns>매출총이익</returns>
        public float GetCalculatedSalesProfit()
        {
            return salesProfit;
        }
        public float CalculateSalesYearly(int soldyear)
        {
            sales = 0;
            salesProfit = 0;
            accountingBook.Sales.ForEach((s) =>
            {
                if (s.Date.Year == soldyear)
                {
                    float price = s.Product.Price * s.DiscountRate;
                    salesProfit += (price - s.Product.GetAllCostsPrice()) * s.Qty;
                    sales += price * s.Qty;
                }
            });
            //매출총이익(salesProfit) 계산 완료
            //매출(sales) 계산 완료
            return sales;
        }
        /// <summary>
        /// 매출을 계산합니다.
        /// </summary>
        /// <param name="soldyear">판매년도</param>
        /// <param name="month">판매월</param>
        /// <returns>매출</returns>
        public float CalculateSalesMonthly(int soldyear, Month month)
        {
            sales = 0;
            salesProfit = 0;
            accountingBook.Sales.ForEach((s) =>
            {
                if(s.Date.Year == soldyear && s.Date.Month == (int)month)
                {
                    float price = s.Product.Price * s.DiscountRate;
                    salesProfit += (price - s.Product.GetAllCostsPrice()) * s.Qty;
                    sales += price * s.Qty;
                }
            });
            return sales;
        }
        /// <summary>
        /// 영업이익을 계산합니다.
        /// </summary>
        /// <param name="soldyear">참고하여 계산할 분개들의 년도와 같습니다.</param>
        /// <param name="month">참고하여 계산할 분개들의 월과 같습니다.</param>
        /// <param name="salesprofit">기본적으로 0입니다. 아닐경우 이것을 기준으로 영업이익을 계산합니다.</param>
        /// <returns>영업 이익</returns>
        public float CalculateOperatingProfitYearly(int soldyear, float salesprofit = 0.0f)
        {
            float operatingCost = 0;

            if (salesprofit == 0.0f)
                CalculateSalesYearly(soldyear);

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
        public float CalculateOperatingProfitMonthly(int soldyear, Month month, float salesprofit=0.0f)
        {
            float operatingCost = 0;

            if (salesprofit == 0.0f)
                CalculateSalesMonthly(soldyear, month);

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
        /// 회계장부에 일시불로 분개합니다.
        /// </summary>
        /// <param name="moneyAmount">출입금</param>
        /// <param name="debtor">돈의 목적지</param>
        /// <param name="whyType">무슨 종류의 돈</param>
        /// <param name="why">돈의 목적</param>
        public void InsertJournalFullPayment(DateTime date, float moneyAmount, IJournalizeObject debtor, JournalizingKinds whyType, string why)
        {
            accountingBook.Insert(new Journalizing(moneyAmount, date, Owner, debtor, whyType, why));
        }
        /// <summary>
        /// 감가상각을 대비하여 년단위로 분개합니다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="moneyAmount"></param>
        /// <param name="debtor"></param>
        /// <param name="whyType"></param>
        /// <param name="why"></param>
        /// <param name="years">1년에 한번 지불합니다.</param>
        public void InsertJournalYearInstallment(DateTime date, float moneyAmount, IJournalizeObject debtor, JournalizingKinds whyType, string why, int years)
        {
            for(int i = 0;i < years;i++)
            {
                date = date.AddYears(i);
                InsertJournalFullPayment(date, moneyAmount / (float)years, debtor, whyType, why);
            }
        }
        /// <summary>
        /// 감가상각을 대비하여 월단위로 분개합니다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="moneyAmount"></param>
        /// <param name="debtor"></param>
        /// <param name="whyType"></param>
        /// <param name="why"></param>
        /// <param name="months"></param>
        public void InsertJournalMonthlyInstallment(DateTime date, float moneyAmount, IJournalizeObject debtor, JournalizingKinds whyType, string why, int months)
        {
            for (int i = 0; i < months; i++)
            {
                date = date.AddMonths(i);
                InsertJournalFullPayment(date, moneyAmount / (float)months, debtor, whyType, why);
            }
        }
        
        /// <summary>
        /// 분개에 기록된 수를 계산합니다.
        /// </summary>
        /// <returns></returns>
        public Dictionary<IJournalizeObject, int> GetLoggedJournalObject(List<Journalizing> js)
        {
            Dictionary<IJournalizeObject, int> graph = new Dictionary<IJournalizeObject, int>();
            js.ForEach((j) =>
            {
                if (graph.ContainsKey(j.To))
                    graph[j.To]++;
                else
                    graph.Add(j.To, 1);
            });
            return graph;
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
            Assets = assets;
            return assets;
        }
    }
}
