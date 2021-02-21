using BusinessEngine.Accounting;
using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BusinessEngine.Operating
{
    /// <summary>
    /// 거래처입니다.
    /// 채무자,채권자도 포함
    /// </summary>
    [Serializable]
    public class AccountCompany:IJournalizeObject, INotifyPropertyChanged
    {
        protected string name;
        protected string note;
        protected Warning warning;

        public string Name {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }
        public string Note { 
            get { return note; }
            set { note = value; NotifyPropertyChanged("Note"); }
        }
        public Warning WarningPoint {
            get { return warning; }
            set { warning = value; NotifyPropertyChanged("WarningPoint"); }
        }

        public AccountCompany() { }
        public AccountCompany(string name) { Name = name; }

        
        public void SetWarningPoint(Warning w) => WarningPoint = w;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}