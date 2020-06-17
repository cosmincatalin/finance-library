using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CosminSanda.Finance.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using ServiceStack;
using ServiceStack.Text;

namespace CosminSanda.Finance

{
    public class EarningsCalendar
    {
        private const string Url = "https://finance.yahoo.com/calendar/earnings?symbol=";
        private const string Bookmark = "root.App.main = ";
        public const string CacheSubPath = ".cache/CosminSanda/Finance/Earnings";

        public static async Task<List<EarningsDate>> GetEarnings(string ticker)
        {
            var earnings = await LoadCachedEarnings(ticker);

            if (earnings.Count == 0 || InvalidateCache(earnings))
            {
                earnings = await RetrieveEarnings(ticker);
            }

            try
            {
                CacheEarnings(ticker, earnings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not cache earnings calendar to disk.");
                Console.WriteLine(ex.Message);
            }

            return earnings;
        }

        private static bool InvalidateCache(IEnumerable<EarningsDate> earnings)
        {
            var now = DateTime.UtcNow.AddDays(-5);
            return earnings.Any(earning => earning.Date < now && earning.EpsActual == null);
        }

        private static void CacheEarnings(string ticker, List<EarningsDate> earnings)
        {
            var savePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + CacheSubPath;
            Directory.CreateDirectory(savePath);
            JsConfig<DateTime>.SerializeFn = date => date.ToString("yyyy-MM-dd");

            using var csv = new StreamWriter(Path.Combine(savePath, $"{ticker}.csv"));
            CsvSerializer.SerializeToWriter(earnings, csv);
        }

        public static async Task<List<EarningsDate>> RetrieveEarnings(string ticker)
        {
            using var webConnector = new WebClient();
            await using var responseStream = await webConnector.OpenReadTaskAsync(Url + ticker);
            using var responseStreamReader = new StreamReader(responseStream ?? throw new NoDataException());
            var tempStorageString = await responseStreamReader.ReadToEndAsync();

            var rows = tempStorageString.Split("\n");
            
            var earnings = new List<EarningsDate>();
            foreach (var row in rows)
            {
                if (!row.StartsWith(Bookmark)) continue;
                var pageJsonData = row.Substring(Bookmark.Length, row.Length - 1 - Bookmark.Length);
                
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-ddThh:mm:ssZ" };
                
                var earningsJson = JObject.Parse(pageJsonData)["context"]?["dispatcher"]?["stores"]?["ScreenerResultsStore"]?["results"]?["rows"]?.Children().ToList();
                if (earningsJson != null)
                    foreach (var earningsDate in earningsJson)
                    {
                        var earning = JsonConvert.DeserializeObject<EarningsDate>(earningsDate.ToString(), dateTimeConverter);
                        earnings.Add(earning);
                    }
                break;
            }

            return earnings;
        }

        public static async Task<List<EarningsDate>> LoadCachedEarnings(string ticker)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + CacheSubPath;
            var filePath = Path.Combine(folderPath, $"{ticker}.csv");
            if (!File.Exists(filePath)) return new List<EarningsDate>();
            using var csv = new StreamReader(filePath);
            var content = await File.ReadAllTextAsync(filePath);
            return content.FromCsv<List<EarningsDate>>();
        }
        
    }
}