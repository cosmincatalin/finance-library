using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            var start = DateTime.ParseExact($"{startDate}Z", "yyyy-MM-ddK", null).ToUniversalTime();
            var end = DateTime.ParseExact($"{endDate}Z", "yyyy-MM-ddK", null).ToUniversalTime();
            var url = $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={start.ToUnixTime()}&period2={end.ToUnixTime()}&interval=1d&events=history";

            using var webConnector = new WebClient();
            await using var responseStream = await webConnector.OpenReadTaskAsync(url);
            using var responseStreamReader = new StreamReader(responseStream ?? throw new NoDataException());
            var tempStorageString = await responseStreamReader.ReadToEndAsync();
            return tempStorageString.FromCsv<List<Candle>>();
        }

        public static async Task<List<Candle>> GetQuotesAround(string ticker, EarningsDate earningsDate, int lookAround = 1)
        {
            lookAround = Math.Max(lookAround, 1);
            var start = earningsDate.Date;
            var end = earningsDate.Date.AddDays(1);
            if (earningsDate.DateType != "BMO") lookAround -= 1;
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