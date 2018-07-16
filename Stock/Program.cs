using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Stock
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string filePath_stack_a = "../../../stock-a.csv";
            const string filePath_stack_b = "../../../stock-b.csv";
            await PrintAsync(filePath_stack_a);
        }

        private static async Task PrintAsync(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                int lineNumber = 0;
                string[] strSplit;
                string s = string.Empty;
                string tatolS = string.Empty;
                List<Stock> stocks = new List<Stock>();
                Stock stock;
                try
                {
                    while ((s = await reader.ReadLineAsync()) != null)
                    {
                        ++lineNumber;
                        stock = new Stock();
                        strSplit = s.Split(',');
                        if (!lineNumber.Equals(1))
                        {
                            if (!strSplit[1].Trim().Equals("0"))
                            {
                                stock.Date = DateTime.Parse(strSplit[0].Trim());
                                stock.Current = Convert.ToDouble(strSplit[1].Trim());
                                stock.Delta = Convert.ToDouble(strSplit[2].Trim());
                                stock.Bids = Convert.ToDouble(strSplit[3].Trim());
                                stock.Offers = Convert.ToDouble(strSplit[4].Trim());
                                stocks.Add(stock);
                            }
                        }
                    }
                    var dateTimes = stocks.GroupBy(x => x.Date.Date).Select(x => new { x.Key, StockItem = x }).ToList();
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}
