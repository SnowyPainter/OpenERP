using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Sales
{
    public interface IProduct
    {
        Company Manufacturer { get; }
        float Price { get; } //정가
        List<IProduct> Costs { get; } //매출 원가


        float SetCost(IProduct c);
    }
}
