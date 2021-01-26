using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BusinessEngine.Accounting
{
    //광고, 접대, 회의, 교통비는 영업을 위한 것 입니다.
    public enum JournalizingKinds
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
    public class Journalizing
    {
        public DateTime When { get; }
        [XmlIgnore]
        public Company FromCompany { get; }
        public IJournalizeObject To { get; }
        public JournalizingKinds For { get; set; }
        public string Description { get; set; }
        public float Amount { get;   }

        public Journalizing(float amount, DateTime when, Company c, IJournalizeObject to, JournalizingKinds whatFor, string descript="")
        {
            Amount = amount;
            When = when;
            FromCompany = c;
            To = to;
            For = whatFor;
            Description = descript;
        }
    }
}
