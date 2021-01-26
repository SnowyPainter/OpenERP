using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Accounting
{
    public class Finance
    {
        //운용 자산
        //자본금
        //영억이익 등.. 계산
        public Company Owner { get; }


        public Finance(Company c)
        {
            Owner = c;
        }
    }
}
