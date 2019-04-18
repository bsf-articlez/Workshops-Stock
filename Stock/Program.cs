using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Stock
{
    static class MyExtension
    {
        private static char NULL_CHAR = '\0';

        public static IEnumerable<T> MyTake<T>(this IEnumerable<T> source, int count)
        {
            foreach (T s in source)
            {
                if (count-- == 0) yield break;
                yield return s;
            }
        }

        internal static ref char ElementAt(this char[] text, char searchChar)
        {
            int n = Array.IndexOf(text, searchChar);

            if (n >= 0) return ref text[n];
            //return ref NULL_CHAR;
            throw new Exception("not found");
        }
    }

    class Program
    {
        private const string pathStockA = "stock-a.csv";
        private const string pathStockB = "stock-b.csv";
        private const string stockA = "-a";
        private const string stockB = "-b";
        private const string summaryDaily = "-d";
        private const string summaryHourly = "-h";
        //private static Func<List<Stock>, List<SummaryStock>, List<SummaryStock>> exec;

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

                List<SummaryStock> summaryStocks = new List<SummaryStock>();
                await GetStock(filePath, summaryStocks);

                //if (args[1].Equals(summaryDaily))
                //{
                //    exec = DailySummaryStock;
                //}
                //else if (args[1].Equals(summaryHourly))
                //{
                //    exec = HourlySummaryStock;
                //}

                //List<SummaryStock> summaryStocks = new List<SummaryStock>();
                //exec(stocks, summaryStocks);

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
                    Console.WriteLine($"Date:{item.Date.ToShortDateString()} ==> Open:{item.Open}, Low:{item.Low}, Close:{item.Close}, Hight:{item.Hight}");
                }
                else if (args[1].Equals(summaryHourly))
                {
                    Console.WriteLine($"Date:{item.Date.ToShortDateString()} [{item.Hour}] ==> Open:{item.Open}, Low:{item.Low}, Close:{item.Close}, Hight:{item.Hight}");
                }
            }
        }

        private static async Task GetStock(string filePath, List<SummaryStock> summaryStocks)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("message", nameof(filePath));
            if (summaryStocks == null) throw new ArgumentNullException(nameof(summaryStocks));

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
                            SetStockData(strSplit, summaryStocks);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //private static List<SummaryStock> DailySummaryStock(List<Stock> stocks, List<SummaryStock> summaryStocks)
        //{
        //    if (stocks == null) throw new ArgumentNullException(nameof(stocks));

        //    var groupDate = stocks.GroupBy(x => x.Date.Date).Select(x => new { x.Key, StockItem = x }).ToList();
        //    SummaryStock summaryStock;
        //    foreach (var item in groupDate)
        //    {
        //        summaryStock = new SummaryStock
        //        {
        //            Date = item.Key.Date,
        //            Open = item.StockItem.OrderBy(x => x.Date).Select(x => x.Current).FirstOrDefault(),
        //            Close = item.StockItem.OrderByDescending(x => x.Date).Select(x => x.Current).FirstOrDefault(),
        //            Low = item.StockItem.Select(x => x.Current).Min(),
        //            Hight = item.StockItem.Select(x => x.Current).Max()
        //        };
        //        summaryStocks.Add(summaryStock);
        //    }
        //    return summaryStocks;
        //}

        //private static List<SummaryStock> HourlySummaryStock(List<Stock> stocks, List<SummaryStock> summaryStocks)
        //{
        //    if (stocks == null) throw new ArgumentNullException(nameof(stocks));

        //    var groupDate = stocks.GroupBy(x => x.Date.Date).Select(x => new { x.Key, StockItem = x }).ToList();
        //    SummaryStock summaryStock;
        //    foreach (var itemDate in groupDate)
        //    {
        //        var groupHour = itemDate.StockItem.GroupBy(x => x.Date.Hour).Select(x => new { x.Key, StockItem = x }).ToList();
        //        foreach (var itemHour in groupHour)
        //        {
        //            summaryStock = new SummaryStock
        //            {
        //                Date = itemDate.Key.Date,
        //                Hour = itemHour.Key,
        //                Open = itemHour.StockItem.OrderBy(x => x.Date).Select(x => x.Current).FirstOrDefault(),
        //                Close = itemHour.StockItem.OrderByDescending(x => x.Date).Select(x => x.Current).FirstOrDefault(),
        //                Low = itemHour.StockItem.Select(x => x.Current).Min(),
        //                Hight = itemHour.StockItem.Select(x => x.Current).Max()
        //            };
        //            summaryStocks.Add(summaryStock);
        //        }
        //    }
        //    return summaryStocks;
        //}

        private static void SetStockData(string[] strSplit, List<SummaryStock> summaryStocks)
        {
            SummaryStock summaryStock;
            if (strSplit[1].Trim() != "0")
            {
                summaryStock = new SummaryStock();
                if (summaryStocks.Count == 0)
                {
                    InitialNewStockRecord(strSplit, summaryStocks, summaryStock);
                }
                else
                {
                    for (int i = 0; i < summaryStocks.Count; i++)
                    {
                        i = summaryStocks.Count;
                        --i;
                        if (summaryStocks[i].Date.Date != DateTime.Parse(strSplit[0].Trim()).Date)
                        {
                            InitialNewStockRecord(strSplit, summaryStocks, summaryStock);
                        }
                        else
                        {
                            summaryStocks[i].Close = Double.Parse(strSplit[1].Trim());
                            if (summaryStocks[i].Low > Double.Parse(strSplit[1].Trim()))
                            {
                                summaryStocks[i].Low = Double.Parse(strSplit[1].Trim());
                            }
                            if (summaryStocks[i].Hight < Double.Parse(strSplit[1].Trim()))
                            {
                                summaryStocks[i].Hight = Double.Parse(strSplit[1].Trim());
                            }
                        }
                    }
                }
            }
        }

        private static void InitialNewStockRecord(string[] strSplit, List<SummaryStock> summaryStocks, SummaryStock summaryStock)
        {
            summaryStock.Date = DateTime.Parse(strSplit[0].Trim());
            summaryStock.Open = Double.Parse(strSplit[1].Trim());
            summaryStock.Close = Double.Parse(strSplit[1].Trim());
            summaryStock.Low = Double.Parse(strSplit[1].Trim());
            summaryStock.Hight = Double.Parse(strSplit[1].Trim());
            summaryStocks.Add(summaryStock);
        }
    }
}
