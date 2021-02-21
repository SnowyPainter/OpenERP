using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace BusinessEngine.Sales
{
    public interface IProduct: INotifyPropertyChanged
    {
        string Name { get; }
        AccountCompany Manufacturer { get; } 
        float Price { get; } //정가
        ObservableCollection<IProduct> Costs { get; set; } //매출 원가
    }
}
