using Epe.xaml.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Epe.xaml.Message
{
    /// <summary>
    /// WarningBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WarningBox : Window
    {
        private string getLpDefaultOr(int key, string defaultText)
        {
            if (!MainWindow.LangPack.ContainsKey(key) || MainWindow.LangPack[key].Length == 0)
                return defaultText;
            return MainWindow.LangPack[key];
        }
        public bool Ok = false;
        public WarningBox(string body, string caption)
        {
            InitializeComponent();
            this.DataContext = new TextBoxViewModel(caption, body);

            if(MainWindow.LangPack != null)
            {
                OkButton.Content = getLpDefaultOr(Keys.OkKey, OkButton.Content.ToString());
                CancelButton.Content = getLpDefaultOr(Keys.CancelKey, CancelButton.Content.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Ok = true;
            this.Close();
        }
    }
}
