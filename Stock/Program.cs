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
        private const string pathStockA = "../../../stock-a.csv";
        private const string pathStockB = "../../../stock-b.csv";
        private const string stockA = "-a";
        private const string stockB = "-b";
        private const string summaryDaily = "-d";
        private const string summaryHourly = "-h";
        private static Func<List<Stock>, List<SummaryStock>, List<SummaryStock>> exec;

        static async Task Main(string[] args)
        {
            if (args.Length == 2)
            {
                string filePath = string.Empty;
                if (args[0].Equals(stockA))
                {
                    filePath = pathStockA;
                }
                else if (args[0].Equals(stockB))
                {
                    filePath = pathStockB;
                }

                List<Stock> stocks = new List<Stock>();
                await GetStock(filePath, stocks);

                if (args[1].Equals(summaryDaily))
                {
                    exec = DailySummaryStock;
                }
                else if (args[1].Equals(summaryHourly))
                {
                    exec = HourlySummaryStock;
                }

                List<SummaryStock> summaryStocks = new List<SummaryStock>();
                exec(stocks, summaryStocks);

                PrintSummaryStock(args, summaryStocks);
            }
        }

        private static void PrintSummaryStock(string[] args, List<SummaryStock> summaryStocks)
        {
            if (summaryStocks == null) throw new ArgumentNullException(nameof(summaryStocks));

            foreach (var item in summaryStocks)
            {
                if (args[1].Equals(summaryDaily))
                {
                    Console.WriteLine($"Date:{item.Date.ToShortDateString()} ==> Start:{item.Start}, Min:{item.Min}, End:{item.End}, Max:{item.Max}");
                }
                else if (args[1].Equals(summaryHourly))
                {
                    Console.WriteLine($"Date:{item.Date.ToShortDateString()} [{item.Hour}] ==> Start:{item.Start}, Min:{item.Min}, End:{item.End}, Max:{item.Max}");
                }
            }
        }

        private static async Task GetStock(string filePath, List<Stock> stocks)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("message", nameof(filePath));
            if (stocks == null) throw new ArgumentNullException(nameof(stocks));

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

        private static List<SummaryStock> DailySummaryStock(List<Stock> stocks, List<SummaryStock> summaryStocks)
        {
            if (stocks == null) throw new ArgumentNullException(nameof(stocks));

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

        private static List<SummaryStock> HourlySummaryStock(List<Stock> stocks, List<SummaryStock> summaryStocks)
        {
            if (stocks == null) throw new ArgumentNullException(nameof(stocks));

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
            if (!Double.Parse(strSplit[1].Trim()).Equals(0))
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
