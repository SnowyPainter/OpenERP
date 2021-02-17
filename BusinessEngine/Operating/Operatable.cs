using BusinessEngine.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessEngine.Operating
{
    public class Operatable: IJournalizeObject
    {
        public string Name { get; }
        /// <summary>
        /// 관리/재무 회계 관리
        /// </summary>
        protected FinanceManage finance;
        /// <summary>
        /// 보고서 작성 도우미
        /// </summary>
        protected ReportManage reporter;
        /// <summary>
        /// 거래처 관리
        /// </summary>
        protected AccountCompanyManage accountComsManage;

        public Operatable()
        {
            finance = new FinanceManage();
            reporter = new ReportManage();
            accountComsManage = new AccountCompanyManage();
        }
        public Operatable(string name):base() { Name = name; }

        /// <summary>
        /// 필터에 맞는 데이터만 찾아 재무회계 데이터에 접근할수있습니다.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFinanceData GetData(FinanceDataFilter filter = null)
        {
            return finance.GetData(filter);
        }


    }
}
