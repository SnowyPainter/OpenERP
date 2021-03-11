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
    /// VerityBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VerityBox : Window
    {
        public bool Ok = false;
        private string getLpDefaultOr(int key, string defaultText)
        {
            if (!MainWindow.LangPack.ContainsKey(key) || MainWindow.LangPack[key].Length == 0)
                return defaultText;
            return MainWindow.LangPack[key];
        }
        public VerityBox(string body, string caption, string verity)
        {
            InitializeComponent();
            this.DataContext = new VerityBoxViewModel(caption, verity, body);

            if (MainWindow.LangPack != null)
            {
                OkButton.Content = getLpDefaultOr(Keys.OkKey, OkButton.Content.ToString());
                CancelButton.Content = getLpDefaultOr(Keys.CancelKey, CancelButton.Content.ToString());
                AnnouncingMsg.Text = getLpDefaultOr(Keys.WriteDownUnderTextKey, AnnouncingMsg.Text);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Vt.Text == InputBox.Text)
            {
                Ok = true;
                this.Close();
            }
        }
    }
}
