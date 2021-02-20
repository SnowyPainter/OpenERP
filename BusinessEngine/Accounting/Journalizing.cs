using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace BusinessEngine.Accounting
{
    //광고, 접대, 회의, 교통비는 영업을 위한 것 입니다.
    public enum UsedFor
    {
        Entertaining, //접대비
        Meeting, //회의비
        Transportation, //교통비 
        Advert, //광고비
        ETC
    }
    /// <summary>
    /// Journalizing belongs to Manage.cs
    /// </summary>
    [Serializable]
    public class Journalizing:INotifyPropertyChanged
    {
        private DateTime when;
        private IJournalizeObject from, to;
        private UsedFor whatFor;
        private string description;
        private float amount;


        public DateTime When { 
            get { return when; } 
            set { when = value; NotifyPropertyChanged("When"); }
        }
        public IJournalizeObject From { 
            get { return from; } 
            set { from = value; NotifyPropertyChanged("From"); }
        }
        public IJournalizeObject To {
            get { return to; } 
            set { to = value; NotifyPropertyChanged("To"); }
        }
        public UsedFor For { 
            get { return whatFor; } 
            set { whatFor = value; NotifyPropertyChanged("For"); } 
        }
        public string Description {
            get { return description; } 
            set { description = value; NotifyPropertyChanged("Description"); }
        }
        public float Amount {
            get { return amount; }
            set { amount = value; NotifyPropertyChanged("Amount"); }
        }

        public Journalizing() { }
        public Journalizing(float amount, DateTime when, IJournalizeObject from, IJournalizeObject to, UsedFor whatFor, string descript="")
        {
            Amount = amount;
            When = when;
            From = from;
            To = to;
            For = whatFor;
            Description = descript;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
