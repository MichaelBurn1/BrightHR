using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightHR.Classes
{
    public class ItemOffer
    {
        public enum OfferType_Enum
        {
            MultiBuy = 1
        }

        public OfferType_Enum OfferType { get; set; }
        public int MultibuyAmount { get; set; }
        public decimal MultibuyPrice { get; set; }
    }
}
