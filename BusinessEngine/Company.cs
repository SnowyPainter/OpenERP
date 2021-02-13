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
    /// <summary>
    /// Operatable 상속자의 예시 형태
    /// </summary>
    public class Company:Operatable
    {
        public override string Name { get; }
        public Company(string name)
        {
            Name = name;
        }
    }
}
