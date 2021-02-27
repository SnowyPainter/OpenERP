using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Epe.xaml.ViewModels
{
    public class BoxViewModel:ViewBase, INotifyPropertyChanged
    {
        public string Title { get; }
        private RelayCommand<Window> dragMoveCommand, closeCommand;

        public bool Ok;

        public ICommand DragMoveCommand
        {
            get
            {
                return dragMoveCommand ?? (dragMoveCommand = new RelayCommand<Window>(w => dragMove(w)));
            }
        }
        public ICommand OkCloseCommand
        {
            get
            {
                return closeCommand ?? (closeCommand = new RelayCommand<Window>(w => okClose(w)));
            }
        }
        public ICommand CancelCloseCommand
        {
            get
            {
                return closeCommand ?? (closeCommand = new RelayCommand<Window>(w => cancelClose(w)));
            }
        }

        public BoxViewModel(string title)
        {
            Ok = false;
            Title = title;
        }

        private void ok()
        {
            Ok = true;
        }
        private void cancel()
        {
            Ok = false;
        }
        private void okClose(Window window)
        {
            ok();
            window.Close();
        }
        private void cancelClose(Window window)
        {
            cancel();
            window.Close();
        }
        private void dragMove(Window window)
        {
            window.DragMove();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class TextBoxViewModel:BoxViewModel
    {
        private string text;

        public string Text { 
            get { return text; }
            set { text = value; NotifyPropertyChanged("Text"); }
        }
        public TextBoxViewModel(string title, string text):base(title)
        {
            Text = text;
        }
    }
    public class VerityBoxViewModel:TextBoxViewModel
    {
        private string inputText;
        private RelayCommand<Window> verityCommand; 

        public string VerityText { get; }
        public ICommand VerityCommand
        {
            get
            {
                return verityCommand ?? (verityCommand = new RelayCommand<Window>(w => verity(w)));
            }
        }
        public string InputText
        {
            get { return inputText; }
            set { inputText = value; NotifyPropertyChanged("InputText"); }
        }
        
        public VerityBoxViewModel(string title, string verity, string body):base(title, body)
        {
            VerityText = verity;
        }

        private void verity(Window w)
        {
            if(VerityText == InputText)
            {
                w.Close();
            }
        }

    }
}
