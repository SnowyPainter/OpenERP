using BusinessEngine;
using BusinessEngine.IO;
using Epe.xaml.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// AddCompanyWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddCompanyWindow : Window
    {
        readonly string SearchingText = "찾아보기";
        readonly string CreationText = "생성하기";

        public Company SelectedCompany;

        public AddCompanyWindow()
        {
            InitializeComponent();
            TitleBar.DataContext = new TitleBarViewModel();
            CompanyListView.Visibility = Visibility.Hidden;
        }
        private bool isCreationGrid()
        {
            return SelectCompanyButton.Content.ToString() == SearchingText;
        }
        private void SelecteCompany_Click(object sender, RoutedEventArgs e)
        {
            if(isCreationGrid())
            {
                // 찾아보기로 전환
                CompanyListView.Visibility = Visibility.Visible;
                CreationGrid.Visibility = Visibility.Hidden;
                SelectCompanyButton.Content = CreationText;

            }
            else
            {
                CompanyListView.Visibility = Visibility.Hidden;
                CreationGrid.Visibility = Visibility.Visible;
                SelectCompanyButton.Content = SearchingText;
                SelectedCompany = null;
            }
        }
        private void AddCompanyXML_Click(object sender, RoutedEventArgs e)
        {
            if(CompanyName.Text != "" && isCreationGrid())
            {
                SelectedCompany = new Company(CompanyName.Text);

                this.Close();
            }
            else if(CompanyListView.SelectedItem != null)
            {
                SelectedCompany = CompanyListView.SelectedItem as Company;

                this.Close();
            }
            else
            {
                SelectedCompany = null;

                this.Close();
            }
        }
    }
}
