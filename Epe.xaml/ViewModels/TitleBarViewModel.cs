using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Epe.xaml.ViewModels
{
    public class TitleBarViewModel:ViewBase
    {
        private RelayCommand<Window> titleBar_MouseDown, minimizeButton_Click, closeButton_Click;
        public string Title { get; set; }
        public ICommand TitleBar_MouseDown
        {
            get
            {
                return titleBar_MouseDown ?? (titleBar_MouseDown = new RelayCommand<Window>(w => DragMove(w)));
            }
        }
        public ICommand MinimizeButton_Click
        {
            get
            {
                return minimizeButton_Click ?? (minimizeButton_Click = new RelayCommand<Window>(w => Minimize(w)));
            }
        }
        public ICommand CloseButton_Click
        {
            get
            {
                return closeButton_Click ?? (closeButton_Click = new RelayCommand<Window>(w => Close(w)));
            }
        }

        public TitleBarViewModel(string title="")
        {
            Title = title;
        }

        public void DragMove(Window w)
        {
            try
            {
                w.DragMove();
            }
            catch { }
        }
        public void Minimize(Window w)
        {
            w.WindowState = WindowState.Minimized;
        }
        public void Close(Window w)
        {
            w.Close();
        }
    }
}
