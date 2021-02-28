using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BusinessEngine.Sales
{
    public abstract class IProduct: INotifyPropertyChanged
    {
        private string name;
        private AccountCompany mft;
        private float price;

        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        public AccountCompany Manufacturer
        {
            get { return mft; }
            set { mft = value; NotifyPropertyChanged("Manufacturer"); }
        }

        public float Price
        {
            get { return price; }
            set { price = value; NotifyPropertyChanged("Price"); }
        }

        public ObservableCollection<IProduct> Costs { get; set; } = new ObservableCollection<IProduct>(); //매출 원가
        public bool Equals(IProduct product)
        {
            return (product.Name == this.Name
                && product.Manufacturer.Equals(this.Manufacturer)
                && product.Price == this.Price);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
