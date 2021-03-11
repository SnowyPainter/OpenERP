using BusinessEngine;
using BusinessEngine.IO;
using BusinessEngine.Operating;
using BusinessEngine.Sales;
using Epe.xaml.Message;
using Epe.xaml.ViewModels;
using LPS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace Epe.xaml
{
    /// <summary>
    /// AddProductWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddProductWindow : Window,INotifyPropertyChanged
    {
        private int selectedItem = -1, selectedCostItem = -1;
        private AccountingCompany selectedAC;
        private DataSystem ds;
        private bool windowForCostProduct = false, forUpdating=false;

        public IProduct Product;

        readonly string SearchingText;
        readonly string CreationText;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IProduct> Costs { get; set; } = new ObservableCollection<IProduct>();
        public ObservableCollection<IProduct> ProductList { get; set; } = new ObservableCollection<IProduct>();

        public ObservableCollection<AccountingCompany> ACList { get; set; } = new ObservableCollection<AccountingCompany>();
        public AccountingCompany SelectedAC
        {
            get { return selectedAC; }
            set { selectedAC = value; NotifyPropertyChanged("SelectedAC"); }
        }

        public int SelectedCostItem
        {
            get { return selectedItem; }
            set { selectedItem = value; NotifyPropertyChanged("SelectedItem"); }
        }

        //찾아보기 메뉴에서..
        public int SelectedProduct
        {
            get { return selectedCostItem; }
            set { selectedCostItem = value; NotifyPropertyChanged("SelectedProduct"); }
        }

        readonly string defaultSearchText = "찾아보기";
        readonly string defaultCreateText = "생성하기";
        readonly string defaultAddCost = "원재료 추가", defaultAddProduct = "상품 추가";
        readonly string defaultUpdatingTitle = "상품 정보 수정";
        readonly string defaultAddError = "추가 오류";
        readonly string defaultAddCostError = "원재료를 추가해주세요.";
        readonly string defaultErrorCheck = "체크 확인 부탁드립니다.";
        readonly string defaultErrorMf = "제조사 확인 부탁드립니다.";
        readonly string defaultCloseText = "창을 닫으시겠습니까?";
        readonly string defaultInterruptText = "{0} 절차를 중단하시겠습니까?";
        private void lpDefaultOr(int key, ref string text)
        {
            if (!MainWindow.LangPack.ContainsKey(key) || MainWindow.LangPack[key].Length == 0)
                return;
            text = MainWindow.LangPack[key];
        }
        public AddProductWindow(bool notCostItem=true, bool hideSearchBtn=false, bool updating = false)
        {
            ds = new DataSystem();
            InitializeComponent();
            ds.Initialize();

            AddCostButton.IsEnabled = notCostItem;
            windowForCostProduct = !notCostItem;
            forUpdating = updating;

            if(MainWindow.LangPack != null)
            {
                lpDefaultOr(Keys.SearchOtherKey, ref defaultSearchText);
                lpDefaultOr(Keys.CreateOneKey, ref defaultCreateText);
                lpDefaultOr(Keys.AddCostKey, ref defaultAddCost);
                lpDefaultOr(Keys.AddProductKey, ref defaultAddProduct);
                lpDefaultOr(Keys.UpdateProductInfoKey, ref defaultUpdatingTitle);
                lpDefaultOr(Keys.FailedToAddKey, ref defaultAddError);
                lpDefaultOr(Keys.PleaseCheckCheckKey, ref defaultErrorCheck);
                lpDefaultOr(Keys.PleaseAddCostKey, ref defaultAddError);
                lpDefaultOr(Keys.PleaseCheckMFKey, ref defaultErrorMf);
                lpDefaultOr(Keys.CloseWindowKey, ref defaultCloseText);
                lpDefaultOr(Keys.InterruptProcedureKey, ref defaultInterruptText);
            }

            SearchingText = defaultSearchText;
            CreationText = defaultCreateText;

            var title = windowForCostProduct ? defaultAddCost : defaultAddProduct;
            if (updating) title = defaultUpdatingTitle;        
            
            TitleBar.DataContext = new TitleBarViewModel(title);
            this.DataContext = this;

            
            ProductListView.Visibility = Visibility.Hidden;

            if (hideSearchBtn)
                SelectProductButton.Visibility = Visibility.Hidden;

            if (windowForCostProduct) {
                ProductList = new ObservableCollection<IProduct>(ds.GetCostProducts());
            }
            else
                ProductList = new ObservableCollection<IProduct>(ds.GetProducts());

            ACList = new ObservableCollection<AccountingCompany>(ds.GetAccountingCompanies());

            ACComboBox.IsEnabled = true;

        }
        public void SetDefaultValues(IProduct product)
        {
            ProductName.Text = product.Name;

            if (ds.CheckMyCompany(product.Manufacturer.Note))
            {
                OtherCompany.IsChecked = false;
                SelectedAC = null;
            }
            else
                SelectedAC = product.Manufacturer;
            Price.Text = product.Price.ToString();
            Costs = product.Costs;
        }
        private bool isCreationGrid()
        {
            return SelectProductButton.Content.ToString() == SearchingText;
        }
        private void SelectProducts_Click(object sender, RoutedEventArgs e)
        {
            if (isCreationGrid())
            {
                // 찾아보기로 전환
                ProductListView.Visibility = Visibility.Visible;
                CreationGrid.Visibility = Visibility.Hidden;
                SelectProductButton.Content = CreationText;
            }
            else
            {
                ProductListView.Visibility = Visibility.Hidden;
                CreationGrid.Visibility = Visibility.Visible;
                SelectProductButton.Content = SearchingText;
                Product = null;
            }
        }
        private AlertBox getErrorAlert(string text)
        {
            return new AlertBox(text, defaultAddError);
        }
        private void AddProductDB_Click(object sender, RoutedEventArgs e)
        {
            int price = 0;
            AlertBox alert;

            if (isCreationGrid())
            {
                if (ProductName.Text != "" && int.TryParse(Price.Text, out price) && price >= 0)
                { 
                    var ischecked = OtherCompany.IsChecked;
                    if(ischecked == null)
                    {
                        alert = getErrorAlert(defaultErrorCheck);
                        alert.ShowDialog();
                        return;
                    }
                    else if(ischecked == true && SelectedAC == null)
                    {
                        alert = getErrorAlert(defaultErrorMf);
                        alert.ShowDialog();
                        return;
                    }
                    else
                    {
                        Product = new Product
                        {
                            Price = price,
                            Name = ProductName.Text,
                            Manufacturer = ischecked == true ? SelectedAC : ds.MyCompany,
                        };
                        if (!windowForCostProduct)
                            Product.Costs = Costs;
                    }

                }
                else// if((OtherCompany.IsChecked == true && SelectedAC != null) || OtherCompany.IsChecked == false)
                {
                    WarningBox box = new WarningBox(
                        defaultInterruptText.Fill((windowForCostProduct ? defaultAddCost : (forUpdating ? defaultUpdatingTitle : defaultAddProduct))), defaultCloseText);
                    box.ShowDialog();
                    if (box.Ok == false)
                        return;
                }
                this.Close();
            }
            else if (ProductListView.SelectedItem != null)
            {
                Product = ProductListView.SelectedItem as IProduct;

                this.Close();
            }
            else
            {
                Product = null;
                this.Close();
            }
        }
         
        private void TextBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Costs.RemoveAt(SelectedCostItem);
        }

        private void AddCostButton_Click(object sender, RoutedEventArgs e)
        {
            //원재료 추가
            var a = new AddProductWindow(false);
            a.ShowDialog();
            if (a.Product != null && a.windowForCostProduct)
            {
                if (a.isCreationGrid())
                    addCostToDB(a.Product);
                Costs.Add(a.Product);
            }
            else if (a.windowForCostProduct)
                getErrorAlert(defaultAddCostError).ShowDialog();
        }
        private void addCostToDB(IProduct product)
        {
            if (product == null) return;

            ds.AddCostProduct(product.Name, product.Price, product.Manufacturer);
        }

        private void ManufacturerIsOtherCompany_Checked(object sender, RoutedEventArgs e)
        {
            ACComboBox.IsEnabled = true;
        }

        private void ManufacturerIsOtherCompany_Unchecked(object sender, RoutedEventArgs e)
        {
            ACComboBox.IsEnabled = false;
        }

        private void OnlyNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
