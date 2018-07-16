using System;
using System.Collections.Generic;
using System.Text;

namespace Stock
{
    public class Stock
    {
        public DateTime Date { get; set; }
        public double Current { get; set; }
        public double Delta { get; set; }
        public double Bids { get; set; }
        public double Offers { get; set; }
    }
}
