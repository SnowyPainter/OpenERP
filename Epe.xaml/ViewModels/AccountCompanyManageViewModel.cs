﻿using BusinessEngine;
using BusinessEngine.IO;
using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;

namespace Epe.xaml.ViewModels
{
    public class AccountCompanyManageViewModel : ViewBase, INotifyPropertyChanged
    {
        /*
         * 
         *  처리해야할 업무
         * 
         * 상품 보기 및 업데이트
         * 거래처 삭제할때 관련 상품 조회 (필수)
         * 
         * 디자인은 무조건 카드형식
         * 
         * 
         */


        #region private Properties
        private int selectedACIndex;
        private bool updatingACEnabled;
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
        public Company Company { get; set; }
        #endregion
        #region private Commands
        private RelayCommand<object> deleteAccountCompany, showSalesOfSelectedAC, addSales, addProduct, addAC, saveAC;
        #endregion
        #region Command Impls
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
            ds.Initialize();
            ds.SetMyCompany(Company);

            UpdatingACEnabled = false;

            var acs = ds.GetAccountingCompanies();
            for (int i = 0;i < acs.Count;i++)
            {
                Company.AccountCManage.AddAccountingCompany(acs[i]);
            }
            
            if(acs.Count > 0)
            {
                SelectedAccountCompanyIndex = 0;
                //OnPropertychanged 내부 동작안함
                SelectedAC = getCloneAC(Company.AccountCManage.AccountingCompanies[SelectedAccountCompanyIndex]);
                UpdatingACEnabled = true;
            }
        }
        private AccountCompany getCloneAC(AccountCompany ac) => new AccountCompany { Name = ac.Name, Note = ac.Note, WarningPoint = ac.WarningPoint };
        public void RemoveAccountCompany(int i)
        {
            if (i < 0 || i >= Company.AccountCManage.AccountingCompanies.Count) return;

            if (MessageBox.Show("정말 삭제하시겠습니까?", $"{SelectedAC.Name}", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;

            ds.DeleteAccountingCompany(Company.AccountCManage.AccountingCompanies[i]);
            Company.AccountCManage.AccountingCompanies.RemoveAt(i);
            unselectAC();
        }
        public void ShowSalesFromAC(string acname)
        {

        }
        public void AddAC()
        {
            var acw = new AddCompanyWindow(true);
            acw.ShowDialog();
            if (acw.SelectedCompany == null) return;

            ds.AddAccountingCompany(acw.SelectedCompany);
            Company.AccountCManage.AddAccountingCompany(acw.SelectedCompany);

            if (Company.AccountCManage.AccountingCompanies.Count == 1)
            {
                SelectedAC = getCloneAC(Company.AccountCManage.AccountingCompanies[0]);
                UpdatingACEnabled = true;
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

            ds.AddProduct(product.Name, product.Price, product.Manufacturer, product.Costs.ToArray());
            MessageBox.Show($"상품이 정상적으로 추가 완료되었습니다.", $"{product.Name}({product.Price})");
        }
        public void UpdateAccountCompany()
        {
            if (SelectedAccountCompanyIndex < 0 || SelectedAccountCompanyIndex >= Company.AccountCManage.AccountingCompanies.Count) return;

            AccountCompany old = Company.AccountCManage.AccountingCompanies[SelectedAccountCompanyIndex], newData = SelectedAC;

            if (old.Name == newData.Name && old.Note == newData.Note && old.WarningPoint == newData.WarningPoint) return;
            
            ds.UpdateAccountingCompany(old, newData);
            Company.AccountCManage.AccountingCompanies[SelectedAccountCompanyIndex] = newData;

            MessageBox.Show("정보 변경 완료되었습니다");

            unselectAC();
        }
        public void OnACSelectedChanged(int i)
        {
            var acinfo = Company.AccountCManage.AccountingCompanies[i];
            //binding 2개 겹치면 a=b, b=a 자동으로됨. 그래서 clone하는것
            SelectedAC = getCloneAC(acinfo);
            UpdatingACEnabled = true;
        }

        private void unselectAC()
        {
            SelectedAccountCompanyIndex = -1;
            SelectedAC = null;
            UpdatingACEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void AccountCompanyManageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedAccountCompanyIndex":
                    if (SelectedAccountCompanyIndex >= 0 && SelectedAccountCompanyIndex < Company.AccountCManage.AccountingCompanies.Count)
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
