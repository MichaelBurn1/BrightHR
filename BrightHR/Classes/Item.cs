using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightHR.Classes
{
    public class Item
    {
        public string SKU { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public List<ItemOffer> Offers = new List<ItemOffer>();
    }
}
