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
        public ObservableCollection<AccountCompany> A { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var title = "업체명";
            TitleBar.DataContext = new TitleBarViewModel(title);
            AccountCompanyManageGrid.DataContext = new AccountCompanyManageViewModel("My Company", FileSave.AccountCompaniesXML);

        }
    }
}
