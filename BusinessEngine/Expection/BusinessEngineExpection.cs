using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Expection
{
    public class BusinessEngineExpection:System.Exception
    {
        public BusinessEngineExpection(string message) : base(message) { }
    }
}
