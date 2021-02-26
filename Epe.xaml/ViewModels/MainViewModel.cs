using BusinessEngine;
using BusinessEngine.IO;
using BusinessEngine.Operating;
using BusinessEngine.Sales;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace Epe.xaml.ViewModels
{
    public class MainViewModel : ViewBase, INotifyPropertyChanged
    {
        #region private Properties
        private int selectedACIndex, selectedProductIndex;
        private bool updatingACEnabled;
        private IProduct selectedProduct;
        #region ACINFO
        private AccountCompany selectedAC;
        #endregion

        #endregion
        #region Properties
        public int SelectedAccountCompanyIndex
        {
            get { return selectedACIndex; }
            set { selectedACIndex = value; NotifyPropertyChanged("SelectedAccountCompanyIndex"); }
        }
        public int SelectedProductIndex
        {
            get { return selectedProductIndex; }
            set { selectedProductIndex = value; NotifyPropertyChanged("SelectedProductIndex"); }
        }
        public bool UpdatingACEnabled
        {
            get { return updatingACEnabled; }
            set { updatingACEnabled = value; NotifyPropertyChanged("UpdatingACEnabled"); }
        }
        public Array WarningPointArray
        {
            get { return Enum.GetValues(typeof(Warning)); }
        }
        /// <summary>
        /// =로 셋하기 전에 무조건 clone으로 해라
        /// </summary>
        public AccountCompany SelectedAC
        {
            get { return selectedAC; }
            set { selectedAC = value; NotifyPropertyChanged("SelectedAC"); }
        }
        public IProduct SelectedProduct
        {
            get { return selectedProduct; }
            set { selectedProduct = value; NotifyPropertyChanged("SelectedProduct"); }
        }
        public Company Company { get; set; }
        #endregion
        #region private Commands
        private RelayCommand<object> deleteAccountCompany, showSalesOfSelectedAC, addSales, addProduct, addAC, saveAC;
        private RelayCommand<object> deleteProductCommand;
        #endregion
        #region Command Impls
        #region Accounting Company Command
        public ICommand SaveAccountCompanyInfo
        {
            get
            {
                return saveAC ?? (saveAC = new RelayCommand<object>(o => UpdateAccountCompany()));
            }
        }
        public ICommand DeleteAccountCompany
        {
            get
            {
                return deleteAccountCompany ?? (deleteAccountCompany = new RelayCommand<object>(o => RemoveAccountCompany(SelectedAccountCompanyIndex)));
            }
        }
        public ICommand AddAccountingCompany
        {
            get
            {
                return addAC ?? (addAC = new RelayCommand<object>(o => AddAC()));
            }
        }
        #endregion
        #region Sales Command
        public ICommand ShowSalesOfSelectedAC
        {
            get
            {
                return showSalesOfSelectedAC ?? (showSalesOfSelectedAC = new RelayCommand<object>(o => ShowSalesFromAC(SelectedAC.Name)));
            }
        }
        public ICommand AddSales
        {
            get
            {
                return addSales ?? (addSales = new RelayCommand<object>(o => AddSalesAndOpenSalesWindow()));
            }
        }
        #endregion
        #region Products Command
        public ICommand AddProduct
        {
            get
            {
                return addProduct ?? (addProduct = new RelayCommand<object>(o => AddProductAndOpenProductWindow()));
            }
        }
        public ICommand DeleteProductCommand
        {
            get
            {
                return deleteProductCommand ?? (deleteProductCommand = new RelayCommand<object>(o => RemoveProduct(SelectedProductIndex)));
            }
        }
        #endregion
        #endregion
        public DataSystem DataSys { get; set; }

        private static AccountCompany getCloneAC(AccountCompany ac) => new AccountCompany { Name = ac.Name, Note = ac.Note, WarningPoint = ac.WarningPoint };
        private static IProduct getCloneProduct(IProduct p) => new Product { Costs = p.Costs, Manufacturer = p.Manufacturer, Name = p.Name, Price = p.Price };
        
        public MainViewModel(string name)
        {
            PropertyChanged += AccountCompanyManageViewModel_PropertyChanged;
            DataSys = new DataSystem();
            Company = new Company(name);
            UpdatingACEnabled = false;            
        }
        public static async Task<MainViewModel> Build(string name)
        {
            var vm = new MainViewModel(name);

            var initTask = Task.Run(() => vm.DataSys.Initialize());
            await initTask;
            vm.DataSys.SetMyCompany(vm.Company);
            var getProductTask = Task.Run(() => vm.DataSys.GetProducts());
            var getACTask = Task.Run(() => vm.DataSys.GetAccountingCompanies());

            var acs = await getACTask;
            var products = await getProductTask;

            vm.Company.AccountCManage.AccountingCompanies = new ObservableCollection<AccountCompany>(acs);
            vm.Company.Finance.Book.Products = new ObservableCollection<IProduct>(products);
            if (products.Count > 0)
            {
                vm.SelectedProductIndex = 0;
                vm.SelectedProduct = getCloneProduct(vm.Company.Finance.Book.Products[vm.SelectedProductIndex]);
            }
            if (acs.Count > 0)
            {
                vm.SelectedAccountCompanyIndex = 0;
                //OnPropertychanged 내부 동작안함
                vm.SelectedAC = getCloneAC(vm.Company.AccountCManage.AccountingCompanies[vm.SelectedAccountCompanyIndex]);
                vm.UpdatingACEnabled = true;
            }

            return vm;
        }
        
        public void RemoveAccountCompany(int i)
        {
            if (i < 0 || i >= Company.AccountCManage.AccountingCompanies.Count) return;

            if (MessageBox.Show($"{Company.AccountCManage.AccountingCompanies[i].Name}을 정말 삭제할까요?", "삭제 경고", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DataSys.DeleteAccountingCompany(Company.AccountCManage.AccountingCompanies[i]);
                Company.AccountCManage.AccountingCompanies.RemoveAt(i);
                unselectAC();
            }
        }
        public void RemoveProduct(int i)
        {
            if (i < 0 || i >= Company.Finance.Book.Products.Count) return;

            if (MessageBox.Show($"{Company.Finance.Book.Products[i].Name}을 정말 삭제할까요?", "삭제 경고", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DataSys.DeleteProduct(SelectedProduct);
                Company.Finance.Book.Products.RemoveAt(i);
                unselectPd();
            }
        }
        public void ShowSalesFromAC(string acname)
        {
            
        }
        public void AddAC()
        {
            var acw = new AddCompanyWindow(true);
            acw.ShowDialog();
            if (acw.SelectedCompany == null) return;

            DataSys.AddAccountingCompany(acw.SelectedCompany);
            Company.AccountCManage.AddAccountingCompany(acw.SelectedCompany);

            if (Company.AccountCManage.AccountingCompanies.Count == 1)
            {
                SelectedAccountCompanyIndex = -1;
                SelectedAccountCompanyIndex = 1;
            }
        }
        public void AddSalesAndOpenSalesWindow()
        {
            var salesProfileWindow = new AddSalesWindow();
            salesProfileWindow.ShowDialog();

            //salesProfileWindow.SaleData;
        }
        public void AddProductAndOpenProductWindow()
        {
            var addProductWindow = new AddProductWindow(true, true);

            addProductWindow.ShowDialog();

            var product = addProductWindow.Product;
            if (product == null || product.Name == "" || product.Price < 0) return;

            DataSys.AddProduct(product.Name, product.Price, product.Manufacturer, product.Costs.ToArray());
            Company.Finance.Book.Products.Add(product);

            if (Company.Finance.Book.Products.Count == 1)
            {
                SelectedProductIndex = -1;
                SelectedProductIndex = 1;
            }

            MessageBox.Show($"상품이 정상적으로 추가 완료되었습니다.", $"{product.Name}({product.Price})");
        }
        public void UpdateAccountCompany()
        {
            if (SelectedAccountCompanyIndex < 0 || SelectedAccountCompanyIndex >= Company.AccountCManage.AccountingCompanies.Count) return;

            AccountCompany old = Company.AccountCManage.AccountingCompanies[SelectedAccountCompanyIndex], newData = SelectedAC;

            if (old.Name == newData.Name && old.Note == newData.Note && old.WarningPoint == newData.WarningPoint) return;
            
            DataSys.UpdateAccountingCompany(old, newData);
            Company.AccountCManage.AccountingCompanies[SelectedAccountCompanyIndex] = newData;

            MessageBox.Show("정보 변경 완료되었습니다");

            unselectAC();
        }
        private void onACSelectedChanged(int i)
        {
            var acinfo = Company.AccountCManage.AccountingCompanies[i];
            //binding 2개 겹치면 a=b, b=a 자동으로됨. 그래서 clone하는것
            SelectedAC = getCloneAC(acinfo);
            UpdatingACEnabled = true;
        }
        private void onProductChanged(int i)
        {
            var product = Company.Finance.Book.Products[i];
            SelectedProduct = getCloneProduct(product);
        }

        private void unselectAC()
        {
            SelectedAccountCompanyIndex = -1;
            SelectedAC = null;
            UpdatingACEnabled = false;
        }
        private void unselectPd()
        {
            SelectedProductIndex = -1;
            SelectedProduct = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void AccountCompanyManageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedAccountCompanyIndex":
                    if (SelectedAccountCompanyIndex >= 0 && SelectedAccountCompanyIndex < Company.AccountCManage.AccountingCompanies.Count)
                        onACSelectedChanged(SelectedAccountCompanyIndex);
                    break;
                case "SelectedProductIndex":
                    if (SelectedProductIndex >= 0 && SelectedProductIndex < Company.Finance.Book.Products.Count)
                        onProductChanged(SelectedProductIndex);
                    break;
            }
        }
        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
