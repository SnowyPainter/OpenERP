using BusinessEngine.IO;
using BusinessEngine.Operating;
using BusinessEngine.Sales;
using Epe.xaml.Message;
using Epe.xaml.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        private DataSystem ds;

        public AddSalesWindow()
        {
            InitializeComponent();

            ds = new DataSystem();
            ds.Initialize();

            TitleBar.DataContext = new TitleBarViewModel();
        }

        private async void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var getProductTask = Task.Run(() => ds.GetProducts());
            ACComboBox.ItemsSource = ds.GetAccountingCompanies();
            ProductCombobox.ItemsSource = await getProductTask;
        }

        private void SetNowButton2_Click(object sender, RoutedEventArgs e)
        {
            ExpectedDepositDate.SelectedDate = DateTime.Now;
        }

        private void SetNowButton1_Click(object sender, RoutedEventArgs e)
        {
            SellDate.SelectedDate = DateTime.Now;
        }
        private void showErrorOfDataDialog(string dataName, out WarningBox box)
        {
            (box = new WarningBox($"{dataName} 정보에 오류가 있습니다. 절차를 중단하시겠습니까?", "판매 기재 중단")).ShowDialog();
        }
        private void showErrorMsgDialog(string message, out WarningBox box)
        {
            (box = new WarningBox(message, "판매 기재 중단")).ShowDialog();
        }
        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sell = SellDate.SelectedDate;
                var deposit = ExpectedDepositDate.SelectedDate;
                var product = ProductCombobox.SelectedItem as Product;
                var buyer = ACComboBox.SelectedItem as AccountCompany;
                var qty = int.Parse(SellQTY.Text);
                var discountRate = int.Parse(DiscountRate.Text);

                WarningBox box = null;
                if (product == null)
                    showErrorOfDataDialog("상품", out box);
                else if (buyer == null)
                    showErrorOfDataDialog("바이어", out box);
                else if (qty <= 0)
                    showErrorOfDataDialog("판매 갯수", out box);
                else if (!sell.HasValue)
                    showErrorOfDataDialog("판매일", out box);
                else if (!deposit.HasValue)
                    showErrorOfDataDialog("입금 예정일", out box);
                else if (discountRate < 0)
                    showErrorOfDataDialog("할인률", out box);
                else if (product != null && product.Manufacturer == null)
                    showErrorMsgDialog("선택하신 상품의 제조사가 확인 불명입니다. 절차를 중단하시는 것을 권장드립니다.", out box);


                if (box != null && !box.Ok) //어딘가에서 에러가 나왔다.
                    return;
                else if (box != null && box.Ok)
                { //절차 중단 
                    SaleData = null;
                    this.Close();
                }
                else
                {
                    SaleData = new Sale(deposit.GetValueOrDefault(DateTime.Now), sell.GetValueOrDefault(DateTime.Now), product, buyer, discountRate, qty);

                    this.Close();
                }
            }
            catch
            {
                AlertBox box = new AlertBox("알 수 없는 오류에 의해 판매 절차를 중단합니다.", "경고 메세지");
                box.ShowDialog();
                return;
            }
        }

        private void OnlyNumber_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
