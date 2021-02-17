using BusinessEngine.Accounting;
using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace BusinessEngine
{
    /// <summary>
    /// MVVM 패턴을 위한 Operatable 상속자
    /// </summary>
    public class Company:Operatable
    {
        /// <summary>
        /// 관리/재무 회계 관리
        /// </summary>
        public FinanceManage Finance
        {
            get { return finance; }
            set
            {
                finance = value;
            }
        }
        /// <summary>
        /// 보고서 작성 도우미
        /// </summary>
        public ReportManage Reporter
        {
            get { return reporter; }
            set
            {
                reporter = value;
            }
        }
        /// <summary>
        /// 거래처 관리
        /// </summary>
        public AccountCompanyManage AccountCManage {
            get { return accountComsManage; }
            set
            {
                accountComsManage = value;
            }
        }
        
        public Company(string name) : base(name) 
        {
            Finance = new FinanceManage();
            Reporter = new ReportManage();
            AccountCManage = new AccountCompanyManage();
        }
    }
}
