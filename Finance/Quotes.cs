using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CosminSanda.Finance.Exceptions;
using ServiceStack;
using ServiceStack.Text;

namespace CosminSanda.Finance
{
    public static class Quotes
    {

        public static async Task<List<Candle>> GetQuotes(string ticker, string startDate, string endDate)
        {
            var start = DateTime.ParseExact($"{startDate}Z", "yyyy-MM-ddK", null).ToUniversalTime().AddHours(18);
            var end = DateTime.ParseExact($"{endDate}Z", "yyyy-MM-ddK", null).ToUniversalTime().AddHours(18);
            var url = $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={start.ToUnixTime()}&period2={end.ToUnixTime()}&interval=1d&events=history";

            using var webConnector = new WebClient();
            await using var responseStream = await webConnector.OpenReadTaskAsync(url);
            using var responseStreamReader = new StreamReader(responseStream ?? throw new NoDataException());
            var tempStorageString = await responseStreamReader.ReadToEndAsync();
            return tempStorageString.FromCsv<List<Candle>>().Select(q => q.WithTicker(ticker)).ToList();
        }

        public static async Task<List<Candle>> GetQuotesAround(string ticker, EarningsRelease earningsRelease, int lookAround = 1)
        {
            lookAround = Math.Max(lookAround, 1);
            var start = earningsRelease.Date.AddHours(-earningsRelease.Date.Hour).AddMinutes(-earningsRelease.Date.Minute).AddSeconds(-earningsRelease.Date.Second).AddMilliseconds(-earningsRelease.Date.Millisecond);
            var end = start.AddDays(1);
            if (earningsRelease.DateType != "BMO") lookAround -= 1;
            for (var i = 0; i < lookAround; i++)
            {
                start = start.AddDays(-1);
                while (Util.IsHoliday(start)) start = start.AddDays(-1);
                
                end = end.AddDays(1);
                while (Util.IsHoliday(end)) end = end.AddDays(1);
            }

            return await GetQuotes(ticker, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
        }
    }
}