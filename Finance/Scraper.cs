using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CosminSanda.Finance.Exceptions;
using CosminSanda.Finance.Records;
using Newtonsoft.Json.Linq;

namespace CosminSanda.Finance
{
    /// <summary>
    /// A class used specifically for retrieving data from Yahoo Finance
    /// </summary>
    public static class Scraper
    {
        
        private const string Url = "https://finance.yahoo.com/calendar/earnings";
        private const string Bookmark = "root.App.main = ";
        
        /// <summary>
        /// Try to retrieve from Yahoo Finance the companies reporting on a given day.
        /// </summary>
        /// <param name="day">The day for when you want to know the companies reporting.</param>
        /// <returns>A list of financial instruments</returns>
        /// <exception cref="NoDataException"></exception>
        public static async Task<IEnumerable<FinancialInstrument>> RetrieveCompaniesReporting(DateOnly day)
        {
            using var httpClient = new HttpClient();
            var queryParameters = $"?day={day.ToString("yyyy-MM-dd")}";
            
            await using var responseStream = await httpClient.GetStreamAsync(Url + queryParameters);
            
            using var responseStreamReader = new StreamReader(responseStream);
            var htmlSource = await responseStreamReader.ReadToEndAsync();

            return htmlSource
                .Split("\n")
                .Where(o => o.StartsWith(Bookmark))
                .Take(1)
                .Select(o => o.Substring(Bookmark.Length, o.Length - 1 - Bookmark.Length))
                .Select(JObject.Parse)
                .SelectMany(o =>
                    o.SelectTokens("$.context.dispatcher.stores.ScreenerResultsStore.results.rows[*].ticker"))
                .Select(o => new FinancialInstrument { Ticker = o.Value<string>() });

        }
    }
}