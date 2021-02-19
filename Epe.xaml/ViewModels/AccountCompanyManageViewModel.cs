using BusinessEngine;
using BusinessEngine.IO;
using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace Epe.xaml.ViewModels
{
    public class AccountCompanyManageViewModel : ViewBase, INotifyPropertyChanged
    {
        #region private Properties
        private int selectedACIndex;

        #region ACINFO
        private AccountComany selectedAC; 
        #endregion
        
        #endregion
        #region Properties
        public int SelectedAccountCompanyIndex
        {
            get { return selectedACIndex; }
            set { selectedACIndex = value; NotifyPropertyChanged("SelectedAccountCompanyIndex"); }
        }
        public AccountComany SelectedAC
        {
            get { return selectedAC; }
            set { selectedAC = value; NotifyPropertyChanged("SelectedAC"); }
        }
        public Company Company { get; set; }
        #endregion
        #region private Commands
        private RelayCommand<object> deleteAccountCompany, showSalesOfSelectedAC, addSales, addProduct, addAC;
        #endregion
        #region Command Impls
        public ICommand DeleteAccountCompany
        {
            get
            {
                return deleteAccountCompany ?? (deleteAccountCompany = new RelayCommand<object>(o=>RemoveAccountCompany(SelectedAccountCompanyIndex)));
            }
        }
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
        public ICommand AddProduct
        {
            get
            {
                return addProduct ?? (addProduct = new RelayCommand<object>(o => AddProductAndOpenProductWindow()));
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

        private DataSystem ds;

        public AccountCompanyManageViewModel(string name, string manage)
        {
            PropertyChanged += AccountCompanyManageViewModel_PropertyChanged;

            Company = new Company(name);
            ds = new DataSystem();
            for(int i = 0;i < 10;i++)
            {
                var com = new AccountComany($"회사 {i}");
                com.Note = $"note {i}.";
                Company.AccountCManage.AddAccountingCompany(com);
            }
            SelectedAccountCompanyIndex = 0;

            ds.Initialize();
        }

        public void RemoveAccountCompany(int i)
        {
            /*
             * 필수 항목 : 리스트에서 삭제, 인덱스 disable, 선택항목 삭제
             */
            if (i < 0 || i >= Company.AccountCManage.AccountingCompanies.Count) return;
            Company.AccountCManage.AccountingCompanies.RemoveAt(i);
            SelectedAccountCompanyIndex = -1;
            SelectedAC = null;
        }
        public void ShowSalesFromAC(string acname)
        {

        }
        public void AddAC()
        {
            var acw = new AddCompanyWindow();
            acw.ShowDialog();

            //acw.SelectedCompany;
        }
        public void AddSalesAndOpenSalesWindow()
        {
            var salesProfileWindow = new AddSalesWindow();
            salesProfileWindow.ShowDialog();

            //salesProfileWindow.SaleData;
        }
        private void AddProductAndOpenProductWindow()
        {
            var addProductWindow = new AddProductWindow();

            addProductWindow.ShowDialog();

            //addProductWindow.Product;
        }
        public void OnACSelectedChanged(int i)
        {
            var acinfo = Company.AccountCManage.AccountingCompanies[i];
            SelectedAC = acinfo;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void AccountCompanyManageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedAccountCompanyIndex":
                    if(SelectedAccountCompanyIndex >= 0 && SelectedAccountCompanyIndex < Company.AccountCManage.AccountingCompanies.Count)
                        OnACSelectedChanged(SelectedAccountCompanyIndex);
                    break;
            }
        }
        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
