using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Sales
{
    public interface IProduct
    {
        Company Manufacturer { get; }
        float Price { get; } //정가
        List<IProduct> Costs { get; } //매출 원가
        float GetAllCosts();
        float SetCost(IProduct p);

        
        /*
        public float GetAllCosts()
        {
            float costs = 0;
            Costs.ForEach((c) =>
            {
                costs += c.Price;
            });
            return costs;
        }
        
        */

        /*
         public float SetCost(p) {
            Costs.Add(p);
         }

         */
    }
}
