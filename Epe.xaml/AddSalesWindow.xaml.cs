using BusinessEngine.IO;
using BusinessEngine.Operating;
using BusinessEngine.Sales;
using Epe.xaml.Message;
using Epe.xaml.ViewModels;
using System;
using System.Collections.Generic;
using LPS;
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

        readonly string defaultDataErrorIntterruptProcedure = "{0} 정보에 오류가 있습니다. 절차를 중단하시겠습니까?";
        readonly string defaultErrorInterruptProcedure = "판매 기재 중단";
        readonly string defaultInterruptByUknownError = "알 수 없는 오류에 의해 중단되었습니다.";
        readonly string defaultWarningMsg = "경고 메세지";
        readonly string defaultRecommendInterruptUnknownMf = "선택하신 상품의 제조사가 확인 불명입니다. 절차를 중단하시는 것을 권장드립니다.";

        readonly string defaultProduct = "상품";
        readonly string defaultBuyer = "바이어";
        readonly string defaultQty = "갯수";
        readonly string defaultSellDate = "판매일";
        readonly string defaultDepositDate = "입금 예정일";
        readonly string defaultDr = "할인율(%)";

        private void lpDefaultOr(int key, ref string text)
        {
            if (!MainViewModel.LangPack.ContainsKey(key) || MainViewModel.LangPack[key].Length == 0)
                return;
            text = MainViewModel.LangPack[key];
        }
        private string getLpDefaultOr(int key, string defaultText)
        {
            if (!MainViewModel.LangPack.ContainsKey(key) || MainViewModel.LangPack[key].Length == 0)
                return defaultText;
            return MainViewModel.LangPack[key];
        }
        private DataSystem ds;

        public AddSalesWindow()
        {
            InitializeComponent();

            ds = new DataSystem();
            ds.Initialize();

            if(MainViewModel.LangPack != null)
            {
                lpDefaultOr(Keys.DataErrorInterruptProcedureKey, ref defaultDataErrorIntterruptProcedure);
                lpDefaultOr(Keys.ErrorInterruptProcedureKey, ref defaultErrorInterruptProcedure);
                lpDefaultOr(Keys.RecommendInterruptUnknownMfKey, ref defaultRecommendInterruptUnknownMf);
                lpDefaultOr(Keys.InterruptByUknownError, ref defaultInterruptByUknownError);
                lpDefaultOr(Keys.WarningMessage, ref defaultWarningMsg);

                lpDefaultOr(Keys.ProductKey, ref defaultProduct);
                lpDefaultOr(Keys.BuyerKey, ref defaultBuyer);
                lpDefaultOr(Keys.QtyKey, ref defaultQty);
                lpDefaultOr(Keys.SellDateKey, ref defaultSellDate);
                lpDefaultOr(Keys.DepositDateKey, ref defaultDepositDate);
                lpDefaultOr(Keys.DiscountRateKey, ref defaultDr);

                TitleText.Text = getLpDefaultOr(Keys.AddSaleKey, TitleText.Text);
                SetNowButton1.Content = getLpDefaultOr(Keys.SetToNowKey, SetNowButton1.Content.ToString());
                SetNowButton2.Content = getLpDefaultOr(Keys.SetToNowKey, SetNowButton2.Content.ToString());
                OkButton.Content = getLpDefaultOr(Keys.OkKey, OkButton.Content.ToString());
            }

            ProductNameText.Text = defaultProduct;
            ProductBuyerText.Text = defaultBuyer;
            ProductQTYText.Text = defaultQty;
            ProductSDText.Text = defaultSellDate;
            ProductDDText.Text = defaultDepositDate;
            ProductDRText.Text = defaultDr;

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
            (box = new WarningBox(defaultDataErrorIntterruptProcedure.Fill(dataName), defaultErrorInterruptProcedure)).ShowDialog();
        }
        private void showErrorMsgDialog(string message, out WarningBox box)
        {
            //ErrorInterruptProcedureKey
            (box = new WarningBox(message, defaultErrorInterruptProcedure)).ShowDialog();
        }
        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sell = SellDate.SelectedDate;
                var deposit = ExpectedDepositDate.SelectedDate;
                var product = ProductCombobox.SelectedItem as Product;
                var buyer = ACComboBox.SelectedItem as AccountingCompany;
                var qty = int.Parse(SellQTY.Text);
                var discountRate = int.Parse(DiscountRate.Text);

                WarningBox box = null;
                if (product == null)
                    showErrorOfDataDialog(defaultProduct, out box);
                else if (buyer == null)
                    showErrorOfDataDialog(defaultBuyer, out box);
                else if (qty <= 0)
                    showErrorOfDataDialog(defaultQty, out box);
                else if (!sell.HasValue)
                    showErrorOfDataDialog(defaultSellDate, out box);
                else if (!deposit.HasValue)
                    showErrorOfDataDialog(defaultDepositDate, out box);
                else if (discountRate < 0)
                    showErrorOfDataDialog(defaultDr, out box);
                else if (product != null && product.Manufacturer == null) //RecommendInterruptUnknownMfKey
                    showErrorMsgDialog(defaultRecommendInterruptUnknownMf, out box);


                if (box != null && !box.Ok) //어딘가에서 에러가 나왔다. 절대 발생안함.
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
                //InterruptByUknownError
                AlertBox box = new AlertBox(defaultInterruptByUknownError, defaultWarningMsg);
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
