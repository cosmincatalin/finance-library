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

namespace CosminSanda.Finance

{
    /// <summary>
    /// 
    /// </summary>
    public static class EarningsCalendar
    {
        private const string Url = "https://finance.yahoo.com/calendar/earnings";
        private const string Bookmark = "root.App.main = ";

        /// <summary>
        /// Get a list
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static async Task<List<EarningsDate>> GetEarnings(DateTime day)
        {
            var formattedDate = day.ToString("yyyy-MM-dd");

            var earnings = await Cache.GetCachedEarnings(formattedDate);

            if (earnings.Count == 0 || InvalidateCache(earnings))
            {
                earnings = await RetrieveEarnings(new List<(string query, string value)>() { ("day", formattedDate) });
            }

            try
            {
                await Cache.CacheEarnings(formattedDate, earnings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not cache earnings calendar to disk.");
                Console.WriteLine(ex.Message);
            }
            
            return earnings
                .Where(o => o.Date < DateTime.Today)
                .OrderByDescending(o => o.Date)
                .ToList();
        }

        /// <summary>
        /// Given a financial instrument, get the list of past ER dates available on Yahoo Finance.
        /// </summary>
        /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance.</param>
        /// <returns>A list of calendar dates</returns>
        public static async Task<List<EarningsDate>> GetEarnings(string ticker)
        {
            var earnings = await Cache.GetCachedEarnings(ticker);

            if (earnings.Count == 0 || InvalidateCache(earnings))
            {
                earnings = await RetrieveEarnings(new List<(string query, string value)>() {("symbol", ticker)});
            }

            try
            {
                await Cache.CacheEarnings(ticker, earnings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not cache earnings calendar to disk.");
                Console.WriteLine(ex.Message);
            }

            var now = DateTime.UtcNow;
            return earnings.OrderByDescending(o => o.Date).ToList();
        }

        public static async Task<List<EarningsDate>> GetPastEarnings(string ticker, int limit = int.MaxValue)
        {
            var now = DateTime.UtcNow;
            var earnings = await GetEarnings(ticker);
            return earnings
                .Where(o => o.Date.CompareTo(now) < 0)
                .OrderByDescending(o => o.Date)
                .Take(limit)
                .ToList();
        } 

        private static bool InvalidateCache(IEnumerable<EarningsDate> earnings)
        {
            var now = DateTime.UtcNow.AddDays(-5);
            return earnings.Any(earning => earning.Date < now && earning.EpsActual == null);
        }

        public static async Task<List<EarningsDate>> RetrieveEarnings(List<(string query, string value)> queryValues)
        {
            using var webConnector = new WebClient();
            var queryParameters = "";
            foreach(var queryValue in queryValues)
            {
                queryParameters += $"?{queryValue.query}={queryValue.value}";
            }
            await using var responseStream = await webConnector.OpenReadTaskAsync(Url + queryParameters);
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
                        
                        if (queryValues.Count(q => q.query == "ticker") > 0)
                        {
                            var tickerQuery = queryValues.FirstOrDefault(q => q.query == "ticker");
                            earnings.Add(earning?.WithTicker(tickerQuery.value));
                        }
                        else
                        {
                            earnings.Add(earning);
                        }
                    }
                break;
            }

            return earnings;
        }

        public static async Task<EarningsDate> GetNextEarningsDate(string ticker)
        {
            var now = DateTime.UtcNow;
            var earnings = await GetEarnings(ticker);
            return earnings
                .Where(o => o.Date.CompareTo(now) >= 0)
                .OrderBy(o => o.Date)
                .First();
        }
    }
}