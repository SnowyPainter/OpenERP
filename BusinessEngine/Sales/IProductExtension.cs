using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BusinessEngine.Sales
{
    public static class IProductExtension
    {
        
        public static float GetAllCostsPrice(this IProduct product)
        {
            float costs = 0;
            product.Costs.ForEach((c) =>
            {
                costs += c.Price;
            });
            return costs;
        }
        
    }
}
