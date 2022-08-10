using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CosminSanda.Finance.Exceptions;
using CosminSanda.Finance.Records;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var query = $"day={day.ToString("yyyy-MM-dd")}";
            var earnings = await RetrieveEarningsData(query);
            return earnings
                .Select(o => o.SelectToken("$.ticker"))
                .Select(o => new FinancialInstrument { Ticker = o.Value<string>() });
        }

        /// <summary>
        /// Try to retrieve from Yahoo Finance the dates when a company has reported earnings in th epast and also
        /// the dates when it is going to report earnings in the future. It is important to note that future dates
        /// are not set in stone and are likely to change.
        /// </summary>
        /// <param name="company">The financial instrument associated with the company</param>
        /// <returns>A list of calendaristic days</returns>
        public static async Task<IEnumerable<EarningsDate>> RetrieveEarningsDates(FinancialInstrument company)
        {
            var query = $"symbol={company.Ticker}";
            var earnings = await RetrieveEarningsData(query);
            return earnings
                .Select(o =>
                    new EarningsDate {
                        Date = DateOnly.FromDateTime(DateTime.ParseExact(o.SelectToken("$.startdatetime").Value<string>(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)),
                        DateType = o.SelectToken("$.startdatetimetype").Value<string>()
                    }
                );
        }

        private static async Task<IEnumerable<JToken>> RetrieveEarningsData(string query)
        {
            using var httpClient = new HttpClient();
            
            await using var responseStream = await httpClient.GetStreamAsync($"{Url}?{query}");
            
            using var responseStreamReader = new StreamReader(responseStream);
            var htmlSource = await responseStreamReader.ReadToEndAsync();

/*
 * {
  "ticker": "MSFT",
  "companyshortname": "Microsoft Corporation",
  "startdatetime": "2022-07-26T16:09:00Z",
  "startdatetimetype": "TAS",
  "epsestimate": 2.29,
  "epsactual": 2.23,
  "epssurprisepct": -2.75,
  "timeZoneShortName": "EDT",
  "gmtOffsetMilliSeconds": -14400000,
  "quoteType": "EQUITY"
}
 */
            
            htmlSource
                .Split("\n")
                .Where(o => o.StartsWith(Bookmark))
                .Take(1)
                .Select(o => o.Substring(Bookmark.Length, o.Length - 1 - Bookmark.Length))
                .Select(JObject.Parse)
                .SelectMany(o => o.SelectTokens("$.context.dispatcher.stores.ScreenerResultsStore.results.rows[*]"))
                .Select(o => {
                    var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
                    options.Converters.Add(new FinancialInstrumentConverter());
                    options.Converters.Add(new EarningsDateConverter());
                    options.Converters.Add(new EarningsReleaseConverter());
                    return JsonSerializer.Deserialize<CosminSanda.Finance.Records.EarningsRelease>(o.ToString(), options);
                })
                .ToList()
                .ForEach(Console.WriteLine);
            
            return htmlSource
                .Split("\n")
                .Where(o => o.StartsWith(Bookmark))
                .Take(1)
                .Select(o => o.Substring(Bookmark.Length, o.Length - 1 - Bookmark.Length))
                .Select(JObject.Parse)
                .SelectMany(o => o.SelectTokens("$.context.dispatcher.stores.ScreenerResultsStore.results.rows[*]"));
        }
    }
}