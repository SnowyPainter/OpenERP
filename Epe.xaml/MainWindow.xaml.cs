using BusinessEngine;
using BusinessEngine.Operating;
using Epe.xaml.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Null인지 체크 확실히 하고, 런타임에는 수정할때 가급적 조심할것(파일 변경 후 수정, 낭비 없게)
        /// </summary>
        public static Dictionary<int, string> LangPack;

        public MainWindow()
        {
            InitializeComponent();
            //Static Text들은 무조건 MainWindow.xaml.cs에서 LPS 이용하여 쓰도록 한다.

            LPS.Loading.KeyPosition = 0;
            LPS.Loading.TextPosition = 2;
            LangPack = LPS.Loading.Load("./ui-language/default-ko.lp", ";");
        }
        private async void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var title = "민우회사";
            TitleBar.DataContext = new TitleBarViewModel(title);
            AccountCompanyManageGrid.DataContext = await MainViewModel.Build(title);
        }
    }
}
