using System;
using System.Xml.Serialization;

namespace BusinessEngine
{
    public class Debt
    {
        [XmlIgnore]
        public Company Who;
        public DateTime When;
        public DateTime Payment;
        public float Amount;
    }
}
