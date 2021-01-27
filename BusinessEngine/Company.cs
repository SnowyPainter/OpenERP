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

        private Finance finance;
        private Manage manage;

        public Company(string name, BusinessSector model)
        {
            Name = name;
            Sector = model;

            manage = new Manage(this);
            finance = new Finance(this);
            
        }
        private string getNameEnum<T>(T enumElement)
        {
            return Enum.GetName(typeof(T), enumElement);
        }
        /// <summary>
        /// 년 매출/이익 보고서를 파일에 저장합니다.
        /// </summary>
        /// <param name="path">마크다운파일입니다.</param>
        /// <param name="comparePastYear">비교할 과거 년도 입니다. 1이면, -1년입니다.</param>
        public void SaveYearlyProfitReport(int year, string path, int comparePastYear = 1,string title="매출 보고서")
        {
            var js = manage.GetJournal(new DateFilter().SetYear(year));
            var sales = manage.CalculateSalesYearly(year);
            var salesProfit = manage.GetCalculatedSalesProfit();
            var operProfit = manage.CalculateOperatingProfitYearly(year, salesProfit);
            var salesDiff = sales - manage.CalculateSalesYearly(year - comparePastYear);
            var sprofitDiff = salesProfit - manage.GetCalculatedSalesProfit();
            var operDiff = operProfit - manage.CalculateOperatingProfitYearly(year, manage.GetCalculatedSalesProfit());
            using (StreamWriter writer = new StreamWriter(path, true)) //// true to append data to the file
            {
                writer.Write($"# {Name} {year}년 {title}  ");
                writer.Write($"### {DateTime.Now}에 작성되었습니다.  ");
                writer.Write($"본 보고서에서 다룰 항목은 아래와 같습니다.  ");
                writer.Write("## 매출 | 매출이익 | 영업이익 | 자산 | 분개  ");
                writer.Write("====  ");
                writer.Write($"* 매출  ");
                writer.Write($"{sales} 원");
                writer.Write($"{yearlyCompareMessage(year, comparePastYear, salesDiff)}");
                writer.Write("====  ");
                writer.Write($"* 매출 이익  ");
                writer.Write($"{salesProfit} 원");
                writer.Write($"{yearlyCompareMessage(year, comparePastYear, sprofitDiff)}");
                writer.Write("====  ");
                writer.Write($"* 영업 이익  ");
                writer.Write($"{operProfit} 원");
                writer.Write($"{yearlyCompareMessage(year, comparePastYear, operDiff)}");
                writer.Write("====  ");
                writer.Write($"* 자산  ");
                writer.Write($"{manage.GetAssets()} 원");
                writer.Write("====  ");
                writer.Write($"* 최다 분개 내용  ");
                var jos = manage.GetLoggedJournalObject(js);
                var jo = getMostLoggedJournalObject(jos);
                writer.Write($"{jo.Name} : {jos[jo]} 회");
                writer.Write("====  ");
                writer.Write($"* 분개 내역  ");
                for (int i = 0;i < js.Count;i++)
                {
                    var j = js[i];
                    writer.Write($"{i + 0}  : {getNameEnum(j.For)} {j.Amount} 원  ");
                    writer.Write($"사유 : {j.Description}  ");
                    writer.Write($"사용처 : {j.To.Name}");
                    writer.Write("----  ");
                }
                writer.Write("====  ");

            }
        }
        /// <summary>
        /// 월 매출/이익 보고서를 작성합니다.
        /// </summary>
        /// <param name="year">년도</param>
        /// <param name="month">월</param>
        /// <param name="comparePastMonth">비교할 전 월. 1이면 1개월 전입니다.</param>
        /// <param name="path">파일을 저장할 위치(md)</param>
        /// <param name="title">보고서의 이름</param>
        public void SaveMonthlyProfitReport(int year, Month month, int comparePastMonth, string path, string title="매출 보고서")
        {
            var numMonth = (int)month;
            var pastYear = year;
            Month pastMonth;
            if (numMonth - 1 <= 0) {
                pastYear -= 1;
                pastMonth = (Month)(12 - Math.Abs(comparePastMonth) % 12);
            }
            else
            {
                pastYear = year;
                pastMonth = (Month)(numMonth - comparePastMonth);
            }

            var js = manage.GetJournal(new DateFilter().SetYear(year).SetMonth(numMonth));
            var sales = manage.CalculateSalesMonthly(year, month);
            var salesProfit = manage.GetCalculatedSalesProfit();
            var operProfit = manage.CalculateOperatingProfitMonthly(year, month, salesProfit);

            var salesDiff = sales - manage.CalculateSalesMonthly(pastYear, pastMonth);
            var sprofitDiff = salesProfit - manage.GetCalculatedSalesProfit();
            var operDiff = operProfit - manage.CalculateOperatingProfitMonthly(pastYear, pastMonth, manage.GetCalculatedSalesProfit());
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.Write($"# {Name} {year}년 {numMonth}월 {title}  ");
                writer.Write($"### {DateTime.Now}에 작성되었습니다.  ");
                writer.Write($"본 보고서에서 다룰 항목은 아래와 같습니다.  ");
                writer.Write("## 매출 | 매출이익 | 영업이익 | 자산 | 분개  ");
                writer.Write("====  ");
                writer.Write($"* 매출  ");
                writer.Write($"{sales} 원");
                writer.Write($"{monthlyCompareMessage(pastYear, pastMonth, salesDiff)}");
                writer.Write("====  ");
                writer.Write($"* 매출 이익  ");
                writer.Write($"{salesProfit} 원");
                writer.Write($"{monthlyCompareMessage(pastYear, pastMonth, sprofitDiff)}");
                writer.Write("====  ");
                writer.Write($"* 영업 이익  ");
                writer.Write($"{operProfit} 원");
                writer.Write($"{monthlyCompareMessage(pastYear, pastMonth, operDiff)}");
                writer.Write("====  ");
                writer.Write($"* 자산  ");
                writer.Write($"{manage.GetAssets()} 원  ");
                writer.Write("====  ");
                writer.Write($"* 최다 분개 내용  ");
                var jos = manage.GetLoggedJournalObject(js);
                var jo = getMostLoggedJournalObject(jos);
                writer.Write($"{jo.Name} : {jos[jo]} 회  ");
                writer.Write("====  ");
                writer.Write($"* 분개 내역  ");
                for (int i = 0; i < js.Count; i++)
                {
                    var j = js[i];
                    writer.Write($"{i + 0}  : {getNameEnum(j.For)} {j.Amount} 원  ");
                    writer.Write($"사유 : {j.Description}  ");
                    writer.Write($"사용처 : {j.To.Name}  ");
                    writer.Write("----  ");
                }
                writer.Write("====  ");

            }
        }

        private IJournalizeObject getMostLoggedJournalObject(Dictionary<IJournalizeObject, int> jos)
        {
            int max = 0;
            IJournalizeObject most = null;
            foreach(var kv in jos)
            {
                if (max < kv.Value) {
                    max = kv.Value;
                    most = kv.Key;
                }
            }
            return most;
        }
        /// <summary>
        /// 지난 {year - comparePastYear}년과 비교했을때 {dif} 원, 매출 {(dif > 0 ? "증가" : (dif < 0 ? "감소" : "유지"))} 되었습니다.  
        /// </summary>
        /// <returns></returns>
        private string yearlyCompareMessage(int year, int past, float diff)
        {
            return $"지난 {year - past}년과 비교했을때 {diff} 원, {(diff > 0 ? "증가" : (diff < 0 ? "감소" : "유지"))} 되었습니다.  ";
        }
        private string monthlyCompareMessage(int pastYear, Month pastMonth, float diff)
        {
            return $"지난 {pastYear}년 {(int)pastMonth}월과 비교했을때 {diff} 원, {(diff > 0 ? "증가" : (diff < 0 ? "감소" : "유지"))} 되었습니다.  ";
        }
    }
}
