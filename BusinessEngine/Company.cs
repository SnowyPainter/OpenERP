using BusinessEngine.Accounting;
using BusinessEngine.Operating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace BusinessEngine
{
    public class Company:Operatable
    {
        public override string Name { get; }
        public Company(string name)
        {
            Name = name;
        }
    }
}
