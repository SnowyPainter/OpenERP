using BusinessEngine.Accounting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine
{
    public class Individual:IJournalizeObject
    {
        public string Name { get; set; }
        public float Funds { get; set; }
    }
}
