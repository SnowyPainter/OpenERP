using BusinessEngine;
using BusinessEngine.IO;
using BusinessEngine.Operating;
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
        readonly string defaultSearchText = "찾아보기";
        readonly string defaultCreateText = "생성하기";

        DataSystem ds;

        public AccountingCompany SelectedCompany;
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
        public AddCompanyWindow(bool hideSearchBtn=false)
        {
            InitializeComponent();
            TitleBar.DataContext = new TitleBarViewModel();
            this.DataContext = this;

            ds = new DataSystem();
            ds.Initialize();

            if(MainViewModel.LangPack != null)
            {
                lpDefaultOr(Keys.SearchOtherKey, ref defaultSearchText);
                lpDefaultOr(Keys.CreateOneKey, ref defaultCreateText);

                SelectCompanyButton.Content = defaultSearchText;
                OkButton.Content = getLpDefaultOr(Keys.OkKey, OkButton.Content.ToString());
                TitleText.Text = getLpDefaultOr(Keys.AddCompanyInfoKey, TitleText.Text);
                CompanyNameHeader.Text = getLpDefaultOr(Keys.CompanyNameKey, CompanyNameHeader.Text);
                CompanyListNameHeader.Header = getLpDefaultOr(Keys.CompanyNameKey, CompanyListNameHeader.Header.ToString());
                MaterialDesignThemes.Wpf.HintAssist.SetHint(CompanyName, getLpDefaultOr(Keys.CompanyNameKey, CompanyNameHeader.Text));
                MaterialDesignThemes.Wpf.HintAssist.SetHint(CompanyNote, getLpDefaultOr(Keys.CompanyNoteKey, CompanyNoteHeader.Text));
            }

            if (hideSearchBtn)
                SelectCompanyButton.Visibility = Visibility.Hidden;
            else
            {
                CompanyListView.ItemsSource = ds.GetAccountingCompanies();
            }
            CompanyListView.Visibility = Visibility.Hidden;
        }
        private bool isCreationGrid()
        {
            return SelectCompanyButton.Content.ToString() == defaultSearchText;
        }
        private void SelecteCompany_Click(object sender, RoutedEventArgs e)
        {
            if(isCreationGrid())
            {
                // 찾아보기로 전환
                CompanyListView.Visibility = Visibility.Visible;
                CreationGrid.Visibility = Visibility.Hidden;
                SelectCompanyButton.Content = defaultCreateText;
            }
            else
            {
                CompanyListView.Visibility = Visibility.Hidden;
                CreationGrid.Visibility = Visibility.Visible;
                SelectCompanyButton.Content = defaultSearchText;
                SelectedCompany = null;
            }
        }
        private void AddCompanyXML_Click(object sender, RoutedEventArgs e)
        {
            if(isCreationGrid() && CompanyName.Text != "")
            {
                SelectedCompany = new AccountingCompany(CompanyName.Text);
                SelectedCompany.Note = CompanyNote.Text;
                SelectedCompany.WarningPoint = Warning.None;

                this.Close();
            }
            else if(CompanyListView.SelectedItem != null)
            {
                SelectedCompany = CompanyListView.SelectedItem as AccountingCompany;
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
