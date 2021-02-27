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

        public VerityBox(string body, string caption, string verity)
        {
            InitializeComponent();
            this.DataContext = new VerityBoxViewModel(caption, verity, body);
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
