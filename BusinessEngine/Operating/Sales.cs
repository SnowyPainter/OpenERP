﻿using BusinessEngine.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Operating
{
    public enum Warning
    {
        /// <summary>
        /// 불량한 공급을 합니다.
        /// </summary>
        PoorSupply,
        /// <summary>
        /// 비탄력적인 시장의 공급을 합니다.
        /// </summary>
        InelasticSupply,
        /// <summary>
        /// 문제 없습니다.
        /// </summary>
        None,
    }

    public class Sales:Operate
    {
        public List<AccountCom> AccountingCompanies;

        public Sales(Company com) : base(com) { }
        
        public void AddAccountingCompany(AccountCom com, Warning warn = Warning.None)
        {
            if(warn != Warning.None)
                com.SetWarningPoint(warn);
            AccountingCompanies.Add(com);
        }

        /// <summary>
        /// 원재료가 위험회사에 의존되는 경우를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public List<IProduct> GetProductFrom(Warning warnPoint, List<Sale> sales)
        {
            List<IProduct> prdts = new List<IProduct>();
            sales.ForEach((s) => AccountingCompanies.ForEach((c) => s.Product.Costs.ForEach((p) => {
                if (p.Manufacturer.Name == c.Name && c.WarningPoint == warnPoint) prdts.Add(p);
            })));
            return prdts;
        }
    }
}
