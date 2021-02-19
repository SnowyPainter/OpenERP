using BusinessEngine.Sales;
using Epe.xaml.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace Epe.xaml
{
    /// <summary>
    /// AddSalesWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddSalesWindow : Window
    {
        public Sale SaleData = null;

        public AddSalesWindow()
        {
            InitializeComponent();
            TitleBar.DataContext = new TitleBarViewModel();
        }

        private void SetNowButton2_Click(object sender, RoutedEventArgs e)
        {
            ExpectedDepositDate.SelectedDate = DateTime.Now;
        }

        private void SetNowButton1_Click(object sender, RoutedEventArgs e)
        {
            SellDate.SelectedDate = DateTime.Now;
        }
        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
