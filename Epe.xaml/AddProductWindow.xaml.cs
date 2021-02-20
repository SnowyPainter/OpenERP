using BusinessEngine;
using BusinessEngine.IO;
using BusinessEngine.Operating;
using BusinessEngine.Sales;
using Epe.xaml.ViewModels;
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
        private AccountComany selectedAC;
        private DataSystem ds;
        private bool windowForCostProduct = false;

        public IProduct Product;

        readonly string SearchingText = "찾아보기";
        readonly string CreationText = "생성하기";

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IProduct> Costs { get; set; } = new ObservableCollection<IProduct>();
        public ObservableCollection<IProduct> ProductList { get; set; } = new ObservableCollection<IProduct>();

        public ObservableCollection<AccountComany> ACList { get; set; } = new ObservableCollection<AccountComany>();
        public AccountComany SelectedAC
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
        public AddProductWindow(bool notCostItem=true)
        {
            ds = new DataSystem();
            InitializeComponent();
            ds.Initialize();
            this.DataContext = this;

            AddCostButton.IsEnabled = notCostItem;
            windowForCostProduct = !notCostItem;
            ProductListView.Visibility = Visibility.Hidden;
            var title = windowForCostProduct ? "원재료 추가" : "상품 추가";
            TitleBar.DataContext = new TitleBarViewModel(title);

            if(windowForCostProduct) {
                ProductList = new ObservableCollection<IProduct>(ds.GetCostProducts());
            }
            else
                ProductList = new ObservableCollection<IProduct>(ds.GetProducts());
            ACList = new ObservableCollection<AccountComany>(ds.GetAccountingCompanies());
            
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
        private void AddProductDB_Click(object sender, RoutedEventArgs e)
        {
            int price = 0;

            if (isCreationGrid())
            {
                if (ProductName.Text != "" && int.TryParse(Price.Text, out price) && price >= 0 && SelectedAC != null)
                {
                    Product = new Product
                    {
                        Price = price,
                        Name = ProductName.Text,
                        Manufacturer = SelectedAC,
                    };
                    if (!windowForCostProduct)
                        Product.Costs = Costs;

                    if (windowForCostProduct)
                    {
                        //원재료 항목에서 Ok버튼 누를 경우, 그 이전 Window에서 처리하기 때문에 넘김
                    }

                }
                else
                {
                    //입력확인
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
                if(a.isCreationGrid())
                    addCostToDB(a.Product);
                Costs.Add(a.Product);
            }
            else if(a.windowForCostProduct)
                MessageBox.Show("원재료를 추가해주세요.");
        }
        private void addCostToDB(IProduct product)
        {
            if (product == null) return;

            ds.AddCostProduct(product.Name, product.Price, product.Manufacturer);
        }
        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
