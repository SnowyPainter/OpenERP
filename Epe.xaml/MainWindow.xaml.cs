using BusinessEngine;
using BusinessEngine.IO;
using BusinessEngine.Operating;
using Epe.xaml.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Epe.xaml
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        /* 혹시 모르니까 기본 한글 단어 모음
        readonly string defaultAcListNameHeader = "이름";
        readonly string defaultAcListNoteHeaderKey = "노트";

        readonly string defaultShowSalesMonthly1Key = "1개월";
        readonly string defaultShowSalesMonthly3Key = "3개월";
        readonly string defaultShowSalesMonthly6Key = "6개월";
        readonly string defaultShowSalesMonthly12Key = "12개월";
        readonly string defaultSaleProductText = "판매 상품";
        readonly string defaultSearchTextKey = "검색";

        readonly string defaultProductListNameHeader = "상품명";
        readonly string defaultProductListBuyerHeader = "바이어";
        readonly string defaultProductListDRHeader = "할인률";
        readonly string defaultProductListQTYHeader = "갯수";
        readonly string defaultProductListSDHeader = "판매일";
        readonly string defaultProductListDpDHeader = "입금 예정일";
        readonly string defaultShowAllSalesButton = "전체 보기";
        readonly string defaultSalesListStateText = "리스트 상태";
        readonly string defaultDeleteSaleButton = "선택 삭제";
        readonly string defaultAddSaleButton = "판매 기재";

        readonly string defaultACInfoTitle = "거래처 정보";
        readonly string defaultACInfoName = "거래처";
        readonly string defaultACInfoNote = "노트";
        readonly string defaultACInfoWarning = "주의";
        readonly string defaultShowSaleListByACButton = "판매내역 보기";
        readonly string defaultEditACButton = "수정";
        readonly string defaultDeleteACButton = "삭제";
        readonly string defaultAddNewACTitle = "새 거래처";
        readonly string defaultAddNewACText = "추가";

        readonly string defaultManageProductTitle = "상품 관리";
        readonly string defaultEditProductButton = "수정";
        readonly string defaultAddProductButton = "상품 추가";

        readonly string defaultDbManageTitle = "Database 관리";
        readonly string defaultDbExportButton = "내보내기";
        readonly string defaultDbImportButton = "불러오기";
        readonly string defaultDbExportWarningText = "현재 정보들을 모두 복제하여 다른 이름으로 저장합니다.";
        readonly string defaultDbImportWarningText = "다른 데이터베이스를 가져와 사용합니다.";
        readonly string defaultDbSecurityWarningText = "데이터가 노출, 훼손되지 않도록 주의를 기울여야합니다.";
        */

        public string ProductListManufacturerText { get; set; } = "제조사";
        public string ProductListPriceText { get; set; } = "정가";
        public string ProductListCostHeader { get; set; } = "원가";
        public string ProductListManufacturerHeader { get; set; } = "제조사";
        public string ProductListCostNameHeader { get; set; } = "제품명";
        public string ProductListPriceHeader { get; set; } = "정가";

        private List<string> readStringsFromFile(string file)
        {
            if (file == null) return null;
            if (file.Length == 0) return null;
            List<string> list = new List<string>();
            using (StreamReader streamReader = new StreamReader(file, Encoding.UTF8))
            {
                string item;
                while ((item = streamReader.ReadLine()) != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }
        private string getLpDefaultOr(int key, string defaultText)
        {
            if (!MainViewModel.LangPack.ContainsKey(key) || MainViewModel.LangPack[key].Length == 0)
                return defaultText;
            return MainViewModel.LangPack[key];
        }
        public MainWindow()
        {
            InitializeComponent();

            var lpfile = INI.Read(DataSystem.PROGRAM_SECTION, DataSystem.LP, DataSystem.INISaveFile);
            var lpsplit = INI.Read(DataSystem.PROGRAM_SECTION, DataSystem.LpSplit, DataSystem.INISaveFile);
            if (lpfile == null || lpfile.Length == 0 || lpsplit == null || lpsplit.Length == 0)
            {
                var defaultLpfilePath = "./ui-languages/default-ko.lp";
                var defaultLpSplitCode = ";";
                INI.Write(DataSystem.PROGRAM_SECTION, DataSystem.LP, defaultLpfilePath, DataSystem.INISaveFile);
                INI.Write(DataSystem.PROGRAM_SECTION, DataSystem.LpSplit, defaultLpSplitCode, DataSystem.INISaveFile);
                lpfile = defaultLpfilePath;
                lpsplit = defaultLpSplitCode;
            }
            var lpFileContents = readStringsFromFile(lpfile);
            MainViewModel.LangPack = LPS.Loading.Load(lpFileContents, lpsplit);
        }

        private async void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var title = "Company";
            TitleBar.DataContext = new TitleBarViewModel(title);
            AccountCompanyManageGrid.DataContext = await MainViewModel.Build(title);
            if (MainViewModel.LangPack != null)
            {
                ACListNameHeader.Header = getLpDefaultOr(Keys.AcListNameHeaderKey, ACListNameHeader.Header.ToString());
                ACListNoteHeader.Header = getLpDefaultOr(Keys.AcListNoteHeaderKey, ACListNoteHeader.Header.ToString());

                ShowSalesMonthly1.Content = getLpDefaultOr(Keys.ShowSalesMonthly1Key, ShowSalesMonthly1.Content.ToString());
                ShowSalesMonthly3.Content = getLpDefaultOr(Keys.ShowSalesMonthly3Key, ShowSalesMonthly3.Content.ToString());
                ShowSalesMonthly6.Content = getLpDefaultOr(Keys.ShowSalesMonthly6Key, ShowSalesMonthly6.Content.ToString());
                ShowSalesMonthly12.Content = getLpDefaultOr(Keys.ShowSalesMonthly12Key, ShowSalesMonthly12.Content.ToString());
                SaleProductText.Text = getLpDefaultOr(Keys.SaleProductTextKey, SaleProductText.Text);
                SearchText.Content = getLpDefaultOr(Keys.SearchTextKey, SearchText.Content.ToString());

                ProductListNameHeader.Header = getLpDefaultOr(Keys.ProductListNameHeaderKey, ProductListNameHeader.Header.ToString());
                ProductListBuyerHeader.Header = getLpDefaultOr(Keys.ProductListBuyerHeaderKey, ProductListBuyerHeader.Header.ToString());
                ProductListDRHeader.Header = getLpDefaultOr(Keys.ProductListDRHeaderKey, ProductListDRHeader.Header.ToString());
                ProductListQTYHeader.Header = getLpDefaultOr(Keys.ProductListQTYHeaderKey, ProductListQTYHeader.Header.ToString());
                ProductListSDHeader.Header = getLpDefaultOr(Keys.ProductListSDHeaderKey, ProductListSDHeader.Header.ToString());
                ProductListDpDHeader.Header = getLpDefaultOr(Keys.ProductListDpDHeaderKey, ProductListDpDHeader.Header.ToString());
                
                ProductListManufacturerText = getLpDefaultOr(Keys.MFNameKey, ProductListManufacturerText);
                ProductListPriceText = getLpDefaultOr(Keys.PriceNameKey, ProductListPriceText);
                ProductListCostHeader = getLpDefaultOr(Keys.CostNameKey, ProductListCostHeader);
                ProductListManufacturerHeader = getLpDefaultOr(Keys.MFNameKey, ProductListManufacturerHeader);
                ProductListCostNameHeader = getLpDefaultOr(Keys.ProductKey, ProductListCostNameHeader);
                ProductListPriceHeader = getLpDefaultOr(Keys.PriceNameKey, ProductListPriceHeader);
                
                ShowAllSalesButton.Content = getLpDefaultOr(Keys.ShowAllSalesButtonKey, ShowAllSalesButton.Content.ToString());
                SalesListStateText.Text = getLpDefaultOr(Keys.SalesListStateTextKey, SalesListStateText.Text);
                DeleteSaleButton.Content = getLpDefaultOr(Keys.DeleteSaleButtonKey, DeleteSaleButton.Content.ToString());
                AddSaleButton.Content = getLpDefaultOr(Keys.AddSaleButtonKey, AddSaleButton.Content.ToString());
                
                ACInfoTitle.Text = getLpDefaultOr(Keys.AcInfoTitleKey, ACInfoTitle.Text);
                ACInfoName.Text = getLpDefaultOr(Keys.AcInfoNameKey, ACInfoName.Text);
                ACInfoNote.Text = getLpDefaultOr(Keys.AcInfoNoteKey, ACInfoNote.Text);
                ACInfoWarning.Text = getLpDefaultOr(Keys.AcInfoWarningKey, ACInfoWarning.Text);
                ShowSaleListByACButton.Content = getLpDefaultOr(Keys.ShowSaleListByACKey, ShowSaleListByACButton.Content.ToString());
                EditACButton.Content = getLpDefaultOr(Keys.EditACKey, EditACButton.Content.ToString());
                DeleteACButton.Content = getLpDefaultOr(Keys.DeleteACKey, DeleteACButton.Content.ToString());
                AddNewACTitle.Text = getLpDefaultOr(Keys.AddNewACTitleKey, AddNewACTitle.Text);
                AddNewACText.Content = getLpDefaultOr(Keys.AddNewACButtonKey, AddNewACText.Content.ToString());
                AddProductButton.Content = getLpDefaultOr(Keys.AddProductKey, AddProductButton.Content.ToString());

                ManageProductTitle.Text = getLpDefaultOr(Keys.ManageProductTitleKey, ManageProductTitle.Text);
                EditProductButton.Content = getLpDefaultOr(Keys.EditProductKey, EditProductButton.Content.ToString());
                
                DbManageTitle.Text = getLpDefaultOr(Keys.DbManageTitleKey, DbManageTitle.Text);
                DbExportButton.Content = getLpDefaultOr(Keys.DbExportKey, DbExportButton.Content.ToString());
                DbImpotButton.Content = getLpDefaultOr(Keys.DbImportKey, DbImpotButton.Content.ToString());
                DbExportWarningText.Text = getLpDefaultOr(Keys.DbExportWarningKey, DbExportWarningText.Text);
                DbImportWarningText.Text = getLpDefaultOr(Keys.DbImportWarningKey, DbImportWarningText.Text);
                DbSecurityWarningText.Text = getLpDefaultOr(Keys.DbSecurityWarningKey, DbSecurityWarningText.Text);

            }
        }
    }
}
