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

    public class SummaryStock
    {
        public DateTime Date { get; set; }
        public double Start { get; set; }
        public double Min { get; set; }
        public double End { get; set; }
        public double Max { get; set; }
    }
}
