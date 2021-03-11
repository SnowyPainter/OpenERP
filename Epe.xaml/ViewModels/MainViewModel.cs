using BusinessEngine;
using BusinessEngine.IO;
using BusinessEngine.Operating;
using BusinessEngine.Sales;
using Epe.xaml.Message;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Epe.xaml.ViewModels
{
    public class MainViewModel : ViewBase, INotifyPropertyChanged
    {
        #region private Properties
        private int selectedACIndex, selectedProductIndex, selectedSaleIndex;
        private bool updatingACEnabled;
        private string currentSaleDisplayStateString;
        private ObservableCollection<Sale> salesForDisplay;
        private IProduct selectedProduct;
        #region ACINFO
        private AccountingCompany selectedAC;
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
        public int SelectedSaleIndex
        {
            get { return selectedSaleIndex; }
            set { selectedSaleIndex = value; NotifyPropertyChanged("SelectedSaleIndex"); }
        }
        public string CurrentSaleDisplayStateString
        {
            get { return currentSaleDisplayStateString; }
            set { currentSaleDisplayStateString = value; NotifyPropertyChanged("CurrentSaleDisplayStateString"); }
        }

        public ObservableCollection<Sale> SalesForDisplay
        {
            get { return salesForDisplay; }
            set { salesForDisplay = value; NotifyPropertyChanged("SalesForDisplay"); }
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
        public AccountingCompany SelectedAC
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
        private RelayCommand<object> deleteAccountCompany, showSalesOfSelectedAC, showAllSales,addSales, addProduct, addAC, saveAC;
        private RelayCommand<object> deleteProductCommand, deleteSelectedSale;
        private RelayCommand<object> exportDBCommand;
        private RelayCommand<Window> importDBCommand;
        private RelayCommand<string> showSalesMonthly, showSalesByProductName;
        private RelayCommand<object> updateProduct;
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
        public ICommand ShowAllSales
        {
            get
            {
                return showAllSales ?? (showAllSales = new RelayCommand<object>(o => ShowSaleListAll()));
            }
        }
        public ICommand ShowSalesByProductName
        {
            get { return showSalesByProductName ?? (showSalesByProductName = new RelayCommand<string>(search => ShowSaleListByProductName(search))); }
        }
        public ICommand ShowSalesOfSelectedAC
        {
            get
            {
                return showSalesOfSelectedAC ?? (showSalesOfSelectedAC = new RelayCommand<object>(o => ShowSaleListFromAC(SelectedAC)));
            }
        }
        public ICommand ShowSalesMonthly
        {
            get
            {
                return showSalesMonthly ?? (showSalesMonthly = new RelayCommand<string>(monthFromNow => ShowSaleListMonthly(int.Parse(monthFromNow))));
            }
        }
        public ICommand AddSales
        {
            get
            {
                return addSales ?? (addSales = new RelayCommand<object>(o => AddSalesAndOpenSalesWindow()));
            }
        }
        public ICommand DeleteSelectedSale
        {
            get
            {
                return deleteSelectedSale ?? (deleteSelectedSale = new RelayCommand<object>(o
                    => RemoveSelectedSale(SelectedSaleIndex)));
            }
        }
        #endregion
        #region Products Command
        public ICommand UpdateProduct
        {
            get
            {
                return updateProduct ?? (updateProduct = new RelayCommand<object>(o => OpenUpdateWindow()));
            }
        }
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
        #region import/export
        public ICommand ExportDBCommand
        {
            get
            {
                return exportDBCommand ?? (exportDBCommand = new RelayCommand<object>(o => ExportDB()));
            }
        }
        public ICommand ImportDBCommand
        {
            get
            {
                return importDBCommand ?? (importDBCommand = new RelayCommand<Window>(w => ImportDB(w)));
            }
        }
        #endregion
        #endregion
        public DataSystem DataSys { get; set; }
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

            vm.DataSys.Initialize();
            vm.DataSys.SetMyCompany(vm.Company);
            
            var getProductTask = Task.Run(() => vm.DataSys.GetProducts());
            var getACTask = Task.Run(() => vm.DataSys.GetAccountingCompanies());
            var getSalesTask = Task.Run(() => vm.DataSys.GetSales());

            var acs = await getACTask;
            var products = await getProductTask;
            var sales = await getSalesTask;

            vm.Company.AccountCManage.AccountingCompanies = new ObservableCollection<AccountingCompany>(acs);
            vm.Company.Finance.Book.Products = new ObservableCollection<IProduct>(products);
            vm.Company.Finance.Book.Sales = new ObservableCollection<Sale>(sales);
            vm.SalesForDisplay = new ObservableCollection<Sale>(sales); //표시용 판매 기재 목록
            vm.CurrentSaleDisplayStateString = "전체 보기";
            if (products.Count > 0)
            {
                vm.SelectedProductIndex = 0;
                vm.SelectedProduct = vm.Company.Finance.Book.Products[vm.SelectedProductIndex].Clone();
            }
            if (acs.Count > 0)
            {
                vm.SelectedAccountCompanyIndex = 0;
                //OnPropertychanged 내부 동작안함
                vm.SelectedAC = vm.Company.AccountCManage.AccountingCompanies[vm.SelectedAccountCompanyIndex].Clone();
                vm.UpdatingACEnabled = true;
            }
            if(sales.Count > 0)
            {
                vm.SelectedSaleIndex = 0;
                //SelectedSaleIndex는 항상 ItemSource인 SalesForDisplay의 Index이다.
                //주의가 필요하다.
            }

            return vm;
        }
        
        public void ExportDB()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Database (*.db)|*.db";
            if (dialog.ShowDialog() == true)
            {
                File.Copy(DataSys.GetDBPath(), dialog.FileName);
            }
        }
        public void ImportDB(Window w)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Database (*.db)|*.db";
            if (dialog.ShowDialog() == true)
            {
                WarningBox box = new WarningBox("데이터베이스를 변경하는 것 입니다. 그대로 진행하시겠습니까?"
                    +Environment.NewLine+"프로그램의 재시작이 필수적입니다.", "불러오기 및 종료");
                box.ShowDialog();

                if(box.Ok)
                {
                    DataSys.SetDBPath(dialog.FileName);

                    w.Close();
                }
            }
        }

        public void RemoveAccountCompany(int i)
        {
            if (i < 0 || i >= Company.AccountCManage.AccountingCompanies.Count) return;

            var name = Company.AccountCManage.AccountingCompanies[i].Name;
            VerityBox box = new VerityBox($"{name}을 정말 삭제할까요?", "삭제 경고", name);
            box.ShowDialog();
            if (box.Ok)
            {
                DataSys.DeleteAccountingCompany(Company.AccountCManage.AccountingCompanies[i]);
                Company.AccountCManage.AccountingCompanies.RemoveAt(i);
                unselectAC();
            }
        }
        public void RemoveProduct(int i)
        {
            if (i < 0 || i >= Company.Finance.Book.Products.Count) return;

            var name = Company.Finance.Book.Products[i].Name;
            VerityBox box = new VerityBox($"{name}을 정말 삭제할까요?", "삭제 경고", name);
            box.ShowDialog();
            if (box.Ok)
            {
                DataSys.DeleteProduct(SelectedProduct);
                Company.Finance.Book.Products.RemoveAt(i);
                unselectPd();
            }
        }
        public void RemoveSelectedSale(int saleIndex)
        {
            if (saleIndex >= SalesForDisplay.Count || saleIndex < 0)
                return;
            var sale = SalesForDisplay[saleIndex];
            var name = $"기재된 판매 {sale.Product.Name} {sale.Qty}개";
            VerityBox box = new VerityBox($"{name} 정말 삭제할까요?", "삭제 경고", name);
            box.ShowDialog();
            if (box.Ok)
            {
                Company.Finance.Book.Sales.Remove(sale);
                DataSys.DeleteSale(sale);
                //삭제할때, 전체보기로 Refresh
                ShowSaleListAll();
            }
        }
        public void ShowSaleListByProductName(string productName)
        {
            if (productName == "") return;

            SalesForDisplay = new ObservableCollection<Sale>(Company.Finance.GetSaleByProductName(productName));
            CurrentSaleDisplayStateString = $"상품명 {productName} 표시";
        }
        public void ShowSaleListMonthly(int monthFromNow)
        {
            if (monthFromNow <= 0) return;

            SalesForDisplay = new ObservableCollection<Sale>(Company.Finance.GetSalesMonthly(monthFromNow));
            CurrentSaleDisplayStateString = $"지난 {monthFromNow}개월만 표시";
        }
        public void ShowSaleListFromAC(AccountingCompany ac)
        {
            if (ac == null) return;

            SalesForDisplay = new ObservableCollection<Sale>(Company.Finance.GetSalesByBuyer(ac));
            CurrentSaleDisplayStateString = $"구매자 {ac.Name}만 표시";
        }
        public void ShowSaleListAll()
        {
            SalesForDisplay = Company.Finance.Book.Sales;
            CurrentSaleDisplayStateString = "전체 표시";
        }
        
        public void AddSalesAndOpenSalesWindow()
        {
            var salesProfileWindow = new AddSalesWindow();
            salesProfileWindow.ShowDialog();
            if(salesProfileWindow.SaleData == null)
            {
                AlertBox box = new AlertBox("판매 사실 기재에 실패했습니다.", "판매 기재 오류");
                box.ShowDialog();
            }
            else
            {
                var data = salesProfileWindow.SaleData;
                DataSys.AddSale(
                    data.ExpectedDepositDate,
                    data.Date,
                    data.To,
                    data.Product,
                    data.DiscountRate,
                    data.Qty
                );
                Company.Finance.Book.Sold(data);

                //새로 추가할때, 전체보기로 Refresh
                ShowSaleListAll();
            }
        }
        
        public void AddProductAndOpenProductWindow()
        {
            var addProductWindow = new AddProductWindow(true, true);

            addProductWindow.ShowDialog();

            var product = addProductWindow.Product;
            if (product == null || product.Name == "" || product.Price < 0) return;
            if (DataSys.CheckMyCompany(product.Manufacturer.Note) && product.Manufacturer.Name == null)
            {
                for(int i = 0;i < product.Costs.Count;i++)
                {
                    var c = product.Costs[i];
                    if (DataSys.CheckMyCompany(c.Manufacturer.Note) && c.Manufacturer.Name == null)
                        product.Costs[i].Manufacturer = DataSys.MyCompany;
                }
                product.Manufacturer = DataSys.MyCompany;
            }

            DataSys.AddProduct(product.Name, product.Price, product.Manufacturer, product.Costs.ToArray());
            Company.Finance.Book.Products.Add(product);

            if (Company.Finance.Book.Products.Count == 1)
            {
                SelectedProductIndex = -1;
                SelectedProductIndex = 1;
            }
            var alert = new AlertBox("상품이 정상적으로 추가되었습니다.", $"상품 추가 {product.Name}({product.Price})");
            alert.ShowDialog();
        }
        public void UpdateAccountCompany()
        {
            if (SelectedAccountCompanyIndex < 0 || SelectedAccountCompanyIndex >= Company.AccountCManage.AccountingCompanies.Count) return;

            AccountingCompany old = Company.AccountCManage.AccountingCompanies[SelectedAccountCompanyIndex], newData = SelectedAC;

            if (old.Name == newData.Name && old.Note == newData.Note && old.WarningPoint == newData.WarningPoint) return;
            
            DataSys.UpdateAccountingCompany(old, newData);
            Company.AccountCManage.AccountingCompanies[SelectedAccountCompanyIndex] = newData;

            var alert = new AlertBox("정보가 변경되었습니다.", "정보 변경");
            alert.ShowDialog();

            unselectAC();
        }
        public void OpenUpdateWindow()
        {
            if (SelectedProduct == null) return;

            var window = new AddProductWindow(true, true, true);
            window.SetDefaultValues(SelectedProduct);
            window.ShowDialog();
            var updatedProduct = window.Product;

            DataSys.UpdateProduct(SelectedProduct, updatedProduct);
            Company.Finance.Book.Products[SelectedProductIndex] = updatedProduct;
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
                SelectedAccountCompanyIndex = -1; //calling on property -changed-
                SelectedAccountCompanyIndex = 1;
            }
        }
        private void onACSelectedChanged(int i)
        {
            var acinfo = Company.AccountCManage.AccountingCompanies[i];
            //binding 2개 겹치면 a=b, b=a 자동으로됨. 그래서 clone하는것
            SelectedAC = acinfo.Clone();
            UpdatingACEnabled = true;
        }
        private void onProductChanged(int i)
        {
            var product = Company.Finance.Book.Products[i];
            SelectedProduct = product.Clone();
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
