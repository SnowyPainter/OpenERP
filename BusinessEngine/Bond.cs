using System;
using System.Xml.Serialization;

namespace BusinessEngine
{
    public class Bond
    {
        [XmlIgnore]
        public Company Who;
        public DateTime When;
        public DateTime Paydate;
        public float Amount;
        public bool Abandonment = false;
    }
}
