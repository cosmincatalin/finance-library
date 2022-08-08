using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CosminSanda.Finance.Exceptions;
using CosminSanda.Finance.Records;
using Newtonsoft.Json.Linq;

namespace CosminSanda.Finance
{
    public class Scraper
    {
        
        private const string Url = "https://finance.yahoo.com/calendar/earnings";
        private const string Bookmark = "root.App.main = ";
        
        public static async Task<List<FinancialInstrument>> RetrieveCompaniesReporting(DateTime day)
        {
            using var webConnector = new WebClient();
            var queryParameters = $"?day={day.ToString("yyyy-MM-dd")}";
            
            await using var responseStream = await webConnector.OpenReadTaskAsync(Url + queryParameters);
            
            using var responseStreamReader = new StreamReader(responseStream ?? throw new NoDataException());
            var tempStorageString = await responseStreamReader.ReadToEndAsync();
            
            var rows = tempStorageString.Split("\n");

            foreach (var row in rows)
            {
                if (!row.StartsWith(Bookmark)) continue;
                var pageJsonData = row.Substring(Bookmark.Length, row.Length - 1 - Bookmark.Length);

                return JObject
                    .Parse(pageJsonData)["context"]?["dispatcher"]?["stores"]?["ScreenerResultsStore"]?["results"]?["rows"]?
                    .Children()
                    .Select(o => new FinancialInstrument(){ Ticker = o.ToObject<EarningsRelease>()?.Ticker})
                    .ToList();

            }

            return new List<FinancialInstrument>();

        }
    }
}