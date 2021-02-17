using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BusinessEngine.Sales
{
    public interface IProduct:INotifyPropertyChanged
    {
        string Name { get; }
        Company Manufacturer { get; } 
        float Price { get; } //정가
        List<IProduct> Costs { get; } //매출 원가
        float SetCost(IProduct c);
    }
}
