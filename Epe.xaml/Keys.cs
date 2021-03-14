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

        #region  Interact Text Key 0~19
        public static readonly int OkKey = 0;
        public static readonly int CancelKey = 1;
        public static readonly int WarningMessage = 3; //'경고 메세지' 그대로
        public static readonly int WriteDownUnderTextKey = 4; //for verity alert box
        public static readonly int SearchOtherKey = 5;
        public static readonly int CreateOneKey = 6;
        public static readonly int InterruptProcedureKey = 7;
        public static readonly int CloseWindowKey = 8;
        public static readonly int WarningDelete = 9;
        public static readonly int ProductNameKey = 10;
        public static readonly int CostNameKey = 11;
        public static readonly int PriceNameKey = 12;
        public static readonly int MFNameKey = 13;
        public static readonly int AddKey = 14;
        public static readonly int OtherCompany = 15;
        #endregion

        #region Sales Info Key 20~30

        public static readonly int ShowSalesMonthly1Key = 20; //ShowSalesMonthly1
        public static readonly int ShowSalesMonthly3Key = 21; //ShowSalesMonthly3
        public static readonly int ShowSalesMonthly6Key = 22; //ShowSalesMonthly6
        public static readonly int ShowSalesMonthly12Key = 23; //ShowSalesMonthly12
        public static readonly int SaleProductTextKey = 24; //SaleProductText
        public static readonly int SearchTextKey = 25; //SearchText
        public static readonly int ProductListNameHeaderKey = 26; //ProductListNameHeader;
        public static readonly int ProductListBuyerHeaderKey = 27; //ProductListBuyerHeader
        public static readonly int ProductListDRHeaderKey = 28; //ProductListDRHeader
        public static readonly int ProductListQTYHeaderKey = 29; //ProductListQTYHeader
        public static readonly int ProductListSDHeaderKey = 30; //ProductListSDHeader
        public static readonly int ProductListDpDHeaderKey = 31; //ProductListDpDHeader
        public static readonly int ShowAllSalesButtonKey = 32; //ShowAllSalesButton
        public static readonly int SalesListStateTextKey = 33; //SalesListStateText
        public static readonly int DeleteSaleButtonKey = 34; //DeleteSaleButton
        public static readonly int AddSaleButtonKey = 35; //AddSaleButton
        
        #endregion

        #region ACList Header Key 36~40
        
        public static readonly int AcListNameHeaderKey = 36; //ACListNameHeader
        public static readonly int AcListNoteHeaderKey = 37; //ACListNoteHeader
        
        #endregion

        
        #region AC Info Key 40~50
        public static readonly int AcInfoTitleKey = 40; //ACInfoTitle
        public static readonly int AcInfoNameKey = 41; //ACInfoName
        public static readonly int AcInfoNoteKey = 42; //ACInfoNote
        public static readonly int AcInfoWarningKey = 43; //ACInfoWarning
        public static readonly int ShowSaleListByACKey = 44; //ShowSaleListByACButton
        public static readonly int EditACKey = 45; //EditACButton
        public static readonly int DeleteACKey = 46; //DeleteACButton
        public static readonly int AddNewACTitleKey = 47; //AddNewACTitle
        public static readonly int AddNewACButtonKey = 48; //AddNewACText
        #endregion
        #region Product Manage Key 60~70
        public static readonly int ManageProductTitleKey = 60; //ManageProductTitle
        public static readonly int EditProductKey = 61; //EditProductButton
        //public static readonly int AddProductKey = 62; //AddProductButton
        //material icon - not language
        //public static readonly int DeleteProductKey = 63; //DeleteProductButton
        #endregion
        #region DB Manage Key 80~89
        public static readonly int DbManageTitleKey = 80; //DbManageTitle
        public static readonly int DbExportKey = 81; //DbExportButton
        public static readonly int DbImportKey = 82; //DbImpotButton
        public static readonly int DbExportWarningKey = 83; //DbExportWarningText
        public static readonly int DbImportWarningKey = 84; //DbImportWarningText
        public static readonly int DbSecurityWarningKey = 85; //DbSecurityWarningText
        #endregion

        #region AddProductWindow 90
        public static readonly int AddCostKey = 91;
        public static readonly int AddProductKey = 92;
        public static readonly int EditProductInfoKey = 93;
        public static readonly int FailedToAddKey = 94;
        public static readonly int PleaseCheckCheckKey = 95;
        public static readonly int PleaseCheckMFKey = 96;
        public static readonly int PleaseAddCostKey = 97;
        #endregion
        #region AddSalesWindow 100~110
        public static readonly int DataErrorInterruptProcedureKey = 100;
        public static readonly int ErrorInterruptProcedureKey = 101;
        public static readonly int ProductKey = 102; //ProductNameText
        public static readonly int BuyerKey = 103; //ProductBuyerText
        public static readonly int QtyKey = 104; //ProductQTYText
        public static readonly int SellDateKey = 105; //ProductSDText
        public static readonly int DepositDateKey = 106; //ProductDDText
        public static readonly int DiscountRateKey = 107; //ProductDRText
        public static readonly int RecommendInterruptUnknownMfKey = 108;
        public static readonly int InterruptByUknownError = 109;
        public static readonly int AddSaleKey = 110; //TitleText
        public static readonly int SetToNowKey = 111; //SetNowButton1 SetNowButton2
        #endregion
        #region AddCompanyWindow 120
        public static readonly int AddCompanyInfoKey = 120; //'회사정보추가'
        public static readonly int CompanyNameKey = 121; //'회사명'
        public static readonly int CompanyNoteKey = 122; //'노트'
        #endregion

        #region MainWindow - ViewModel 130~ 다 런타임 ViewModel.cs

        //{0}을 정말 삭제하시겠습니까?
        public static readonly int WarningDeleteSomethingKey = 130;
        //기재된 {0} 판매 {1} 개
        public static readonly int WrittenSalesMsgKey = 131;
        //상품명 {0}만 표시
        public static readonly int DisplayListOnlyProductKey = 132;
        //구매자 {0}만 표시
        public static readonly int DisplayListOnlyBuyerKey = 133;
        //지난 {0}개월만 표시
        public static readonly int DisplayLeastMonthlyTextKey = 134;
        //판매 기재에 실패했습니다.
        public static readonly int FailedToAddSaleKey = 135;
        //판매 기재 오류
        public static readonly int ErrorToAddSaleKey = 136;
        //상품이 정상적으로 추가되었습니다.
        public static readonly int PassToAddProductKey = 137;
        //상품 추가 {0}({1})
        public static readonly int AddProductInfoTextKey = 138;
        //정보가 변경되었습니다.
        public static readonly int AlertDataUpdatedKey = 139;
        //정보 변경
        public static readonly int AlertDataUpdatedTitleKey = 140;
        //데이터베이스를 변경하는 것 입니다. 그대로 진행하시겠습니까?
        public static readonly int AlertWarningExportTextKey = 141;
        //불러오기 및 종료
        public static readonly int ImportAndShutdownTextKey = 142;

        #endregion

        #endregion
    }
}
