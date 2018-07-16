using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace App01
{
    class Program
    {
        // WTF
        static async Task Main(string[] args)
        {
            const string filePath = "../../../Program.cs";
            List<string> vs = new List<string>();
            vs.Add(filePath);
            var t1 = PrintAsync(vs);
            var t2 = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($"Line: {i}");
                }
            });

            try
            {
                await Task.WhenAll(new[] { t1, t2 });
            }
            catch (InvalidCommentException ex)
            {
                //ex.LineNumber;
                Console.WriteLine(ex.Message);
            }
            
        }

        private static async Task PrintAsync<T>(List<T> filePath)
        {
            using (var reader = new StreamReader(filePath[0].ToString()))
            {
                string s = string.Empty;
                int lineNumber = 0;
                while ((s = await reader.ReadLineAsync()) != null)
                {
                    ++lineNumber;
                    if (s.Trim().Equals("// WTF"))
                    {
                        var ex = new InvalidCommentException(lineNumber);
                        throw ex;
                    }
                    Console.WriteLine(s);
                }
            }
        }
    }
}
