using BusinessEngine;
using BusinessEngine.IO;
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
        private DataSystem ds;
        private bool forCostItem = false;

        public IProduct Product;

        readonly string SearchingText = "찾아보기";
        readonly string CreationText = "생성하기";

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IProduct> Costs { get; set; } = new ObservableCollection<IProduct>();
        public ObservableCollection<IProduct> ProductList { get; set; } = new ObservableCollection<IProduct>();

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

            AddCostButton.IsEnabled = notCostItem;
            forCostItem = !notCostItem;
            ProductListView.Visibility = Visibility.Hidden;
            var title = forCostItem ? "원재료 추가" : "상품 추가";
            TitleBar.DataContext = new TitleBarViewModel(title);
            ds.Initialize();

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
                
                if(forCostItem)
                {
                    //list from cost table
                }
                else
                {
                    //products table
                }

            }
            else
            {
                ProductListView.Visibility = Visibility.Hidden;
                CreationGrid.Visibility = Visibility.Visible;
                SelectProductButton.Content = SearchingText;
                Product = null;
            }
        }
        private List<IProduct> getProducts()
        {
            return new List<IProduct>();
        }
        private void addCostToDB(IProduct product)
        {
            if (product == null) return;

            Debug.WriteLine("원품목 추가");
            Debug.WriteLine($"상품명: {product.Name}");
            Debug.WriteLine($"정가 : {product.Price}");
        }
        private void AddProductDB_Click(object sender, RoutedEventArgs e)
        {
            int price = -1;

            if (ProductName.Text != "" 
                && int.TryParse(Price.Text, out price) && price >= 0
                && isCreationGrid())
            {
                Product = new Product
                {
                    Price = price,
                    Name = ProductName.Text
                }; 
                Product.Costs = Costs;
                
                if(forCostItem)
                {
                    //원재료 항목에서 Ok버튼 누를 경우, 그 이전 Window에서 처리하기 때문에 넘김
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
            var a = new AddProductWindow(false);
            a.ShowDialog();
            addCostToDB(a.Product);
            Costs.Add(a.Product);
        }

        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
