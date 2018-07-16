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
        private static Func<List<Stock>, List<SummaryStock>, List<SummaryStock>> exec;
        static async Task Main(string[] args)
        {
            const string filePath_stack_a = "../../../stock-a.csv";
            const string filePath_stack_b = "../../../stock-b.csv";
            string filePath = string.Empty;
            List<Stock> stocks = new List<Stock>();
            List<SummaryStock> summaryStocks = new List<SummaryStock>();
            
            if (args.Length > 0)
            {
                if (args[0].Equals("-a"))
                {
                    filePath = filePath_stack_a;
                }
                else if (args[0].Equals("-b"))
                {
                    filePath = filePath_stack_b;
                }

                await GetStock(filePath, stocks);

                if (args[1].Equals("-d"))
                {
                    exec = DailySumaryStock;
                }
                else if (args[1].Equals("-h"))
                {
                    exec = HourlySumaryStock;
                }

                exec(stocks, summaryStocks);

                foreach (var item in summaryStocks)
                {
                    if (args[1].Equals("-d"))
                    {
                        Console.WriteLine($"Date:{item.Date.ToShortDateString()} ==> Start:{item.Start}, Min:{item.Min}, End:{item.End}, Max:{item.Max}");
                    }
                    else if (args[1].Equals("-h"))
                    {
                        Console.WriteLine($"Date:{item.Date.ToShortDateString()} [{item.Hour}] ==> Start:{item.Start}, Min:{item.Min}, End:{item.End}, Max:{item.Max}");
                    }
                }
            }
        }

        private static async Task GetStock(string filePath, List<Stock> stocks)
        {
            using (var reader = new StreamReader(filePath))
            {
                int lineNumber = 0;
                string[] strSplit;
                string s = string.Empty;
                try
                {
                    while ((s = await reader.ReadLineAsync()) != null)
                    {
                        ++lineNumber;
                        strSplit = s.Split(',');
                        if (!lineNumber.Equals(1))
                        {
                            SetStockData(strSplit, stocks, new Stock());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static List<SummaryStock> DailySumaryStock(List<Stock> stocks, List<SummaryStock> summaryStocks)
        {
            var groupDate = stocks.GroupBy(x => x.Date.Date).Select(x => new { x.Key, StockItem = x }).ToList();
            SummaryStock summaryStock;
            foreach (var item in groupDate)
            {
                summaryStock = new SummaryStock
                {
                    Date = item.Key.Date,
                    Start = item.StockItem.OrderBy(x => x.Date).Select(x => x.Current).FirstOrDefault(),
                    End = item.StockItem.OrderByDescending(x => x.Date).Select(x => x.Current).FirstOrDefault(),
                    Min = item.StockItem.Select(x => x.Current).Min(),
                    Max = item.StockItem.Select(x => x.Current).Max()
                };
                summaryStocks.Add(summaryStock);
            }
            return summaryStocks;
        }

        private static List<SummaryStock> HourlySumaryStock(List<Stock> stocks, List<SummaryStock> summaryStocks)
        {
            var groupDate = stocks.GroupBy(x => x.Date.Date).Select(x => new { x.Key, StockItem = x }).ToList();
            SummaryStock summaryStock;
            foreach (var itemDate in groupDate)
            {
                var groupHour = itemDate.StockItem.GroupBy(x => x.Date.Hour).Select(x => new { x.Key, StockItem = x }).ToList();
                foreach (var itemHour in groupHour)
                {
                    summaryStock = new SummaryStock
                    {
                        Date = itemDate.Key.Date,
                        Hour = itemHour.Key,
                        Start = itemHour.StockItem.OrderBy(x => x.Date).Select(x => x.Current).FirstOrDefault(),
                        End = itemHour.StockItem.OrderByDescending(x => x.Date).Select(x => x.Current).FirstOrDefault(),
                        Min = itemHour.StockItem.Select(x => x.Current).Min(),
                        Max = itemHour.StockItem.Select(x => x.Current).Max()
                    };
                    summaryStocks.Add(summaryStock);
                }
            }
            return summaryStocks;
        }

        private static void SetStockData(string[] strSplit, List<Stock> stocks, Stock stock)
        {
            if (!strSplit[1].Trim().Equals("0"))
            {
                stock.Date = DateTime.Parse(strSplit[0].Trim());
                stock.Current = Double.Parse(strSplit[1].Trim());
                stock.Delta = Double.Parse(strSplit[2].Trim());
                stock.Bids = Double.Parse(strSplit[3].Trim());
                stock.Offers = Double.Parse(strSplit[4].Trim());
                stocks.Add(stock);
            }
        }
    }
}
