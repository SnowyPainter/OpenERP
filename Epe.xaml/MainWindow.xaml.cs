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
        public MainWindow()
        {
            InitializeComponent();

        }

        private async void ThisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var title = "민우회사";
            TitleBar.DataContext = new TitleBarViewModel(title);
            AccountCompanyManageGrid.DataContext = await MainViewModel.Build(title);
        }
    }
}
