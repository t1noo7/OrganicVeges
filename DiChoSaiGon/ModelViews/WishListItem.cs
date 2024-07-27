using DiChoSaiGon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiChoSaiGon.ModelViews
{
    public class WishListItem
    {
        public Product product { get; set; }
        public int amount { get; set; }
        public double TotalMoney => 1 * product.Price.Value;
    }
}
