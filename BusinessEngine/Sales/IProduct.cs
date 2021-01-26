using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEngine.Sales
{
    public interface IProduct
    {
        float Price { get; } //정가
        List<IProduct> Costs { get; } //매출 원가
        float GetAllCosts();
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
    }
}
