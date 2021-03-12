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
    /// AlertBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlertBox : Window
    {
        private string getLpDefaultOr(int key, string defaultText)
        {
            if (!MainViewModel.LangPack.ContainsKey(key) || MainViewModel.LangPack[key].Length == 0)
                return defaultText;
            return MainViewModel.LangPack[key];
        }
        public AlertBox(string body, string caption)
        {
            InitializeComponent();

            this.DataContext = new TextBoxViewModel(caption, body);
            if (MainViewModel.LangPack != null)
            {
                OkButton.Content = getLpDefaultOr(Keys.OkKey, OkButton.Content.ToString());
            }
        }

    }
}
