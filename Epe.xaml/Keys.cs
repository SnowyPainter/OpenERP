using System;
using System.Collections.Generic;
using System.Text;

namespace Epe.xaml
{
    public static class Keys
    {
        #region text keys
        //윈도우 타이틀 - public, static = 0번대
        //거래처 정보 = 10번대
        //상품 정보 + Grid = 20~30번대
        //거래처 관리 = 40~50번대
        //상품관리 = 60~70번대
        //db관리 = 80번대
        //AddProductWindow = 90번대
        //AddSalesWindow = 100~110번대
        //AddCompanyWindow = 120번대

        #region  Interact Text Key 0~9
        public static readonly int OkKey = 0;
        public static readonly int CancelKey = 1;
        public static readonly int WarningMessage = 3; //'경고 메세지' 그대로
        public static readonly int WriteDownUnderTextKey = 4; //for verity alert box
        public static readonly int SearchOtherKey = 5;
        public static readonly int CreateOneKey = 6;
        public static readonly int InterruptProcedureKey = 7;
        public static readonly int CloseWindowKey = 8;
        #endregion
        #region ACList Header Key 10~19
        private const int AcListNameHeaderKey = 10; //ACListNameHeader
        private const int AcListNoteHeaderKey = 11; //ACListNoteHeader
        #endregion
        #region Sales Info Key 20~30
        private const int showSalesMonthly1Key = 20; //ShowSalesMonthly1
        private const int showSalesMonthly3Key = 21; //ShowSalesMonthly3
        private const int showSalesMonthly6Key = 22; //ShowSalesMonthly6
        private const int showSalesMonthly12Key = 23; //ShowSalesMonthly12
        private const int saleProductTextKey = 24; //ShowProductText
        private const int searchTextKey = 25; //SearchText
        private const int productListNameHeaderKey = 26; //ProductListNameHeader;
        private const int productListBuyerHeaderKey = 27; //ProductListBuyerHeader
        private const int productListDRHeaderKey = 28; //ProductListDRHeader
        private const int productListQTYHeaderKey = 29; //ProductListQTYHeader
        private const int productListSDHeaderKey = 30; //ProductListSDHeader
        private const int productListDpDHeaderKey = 31; //ProductListDpDHeader
        private const int showAllSalesButtonKey = 32; //ShowAllSalesButton
        private const int salesListStateTextKey = 33; //SalesListStateText
        private const int deleteSaleButtonKey = 34; //DeleteSaleButton
        private const int addSaleButtonKey = 35; //AddSaleButton
        #endregion
        #region AC Info Key 40~50
        private const int acInfoTitleKey = 40; //ACInfoTitle
        private const int acInfoNameKey = 41; //ACInfoName
        private const int acInfoNoteKey = 42; //ACInfoNote
        private const int acInfoWarningKey = 43; //ACInfoWarning
        private const int showSaleListByACKey = 44; //ShowSaleListByACButton
        private const int editACKey = 45; //EditACButton
        private const int deleteACKey = 46; //DeleteACButton
        private const int addNewACTitleKey = 47; //AddNewACTitle
        private const int addNewACButtonKey = 48; //AddNewACText
        #endregion
        #region Product Manage Key 60~70
        private const int manageProductTitleKey = 60; //ManageProductTitle
        private const int editProductKey = 61; //EditProductButton
        private const int addProductKey = 62; //AddProductButton
        private const int deleteProductKey = 63; //DeleteProductButton
        #endregion
        #region DB Manage Key 80~89
        private const int dbManageTitleKey = 80; //DbManageTitle
        private const int dbExportKey = 81; //DbExportButton
        private const int dbImportKey = 82; //DbImpotButton
        private const int dbExportWarningKey = 83; //DbExportWarningText
        private const int dbImportWarningKey = 84; //DbImportWarningText
        private const int dbSecurityWarningKey = 85; //DbSecurityWarningText
        #endregion
        #region AddProductWindow 90
        public static readonly int AddCostKey = 81;
        public static readonly int AddProductKey = 82;
        public static readonly int EditProductInfoKey = 83;
        public static readonly int FailedToAddKey = 84;
        public static readonly int PleaseCheckCheckKey = 85;
        public static readonly int PleaseCheckMFKey = 86;
        public static readonly int PleaseAddCostKey = 87;
        public static readonly int UpdateProductInfoKey = 88;
        #endregion
        #region AddSalesWindow 100~110
        public static readonly int DataErrorInterruptProcedureKey = 100;
        public static readonly int ErrorInterruptProcedureKey = 101;
        public static readonly int ProductKey = 102;
        public static readonly int BuyerKey = 103;
        public static readonly int QtyKey = 104;
        public static readonly int SellDateKey = 105;
        public static readonly int DepositDateKey = 106;
        public static readonly int DiscountRateKey = 107;
        public static readonly int RecommendInterruptUnknownMfKey = 108;
        public static readonly int InterruptByUknownError = 109;
        #endregion
        #region AddCompanyWindow 120
        public static readonly int AddCompanyInfoKey = 120; //'회사정보추가'
        public static readonly int CompanyNameKey = 121; //'회사명'
        public static readonly int CompanyNoteKey = 122; //'노트'
        #endregion
        #endregion
    }
}
