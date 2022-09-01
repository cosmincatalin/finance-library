using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CosminSanda.Finance.Records;
using ServiceStack;
using ServiceStack.Text;

namespace CosminSanda.Finance;

public static class Quotes
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance.</param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static async Task<List<Candle>> GetQuotes(
        string ticker,
        string startDate,
        string endDate
    )
    {
        var start = DateTime
            .ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            .ToUniversalTime();
        var end = DateTime
            .ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            .ToUniversalTime()
            .AddDays(1);
        var url =
            $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={start.ToUnixTime()}&period2={end.ToUnixTime()}&interval=1d&events=history";

        using var httpClient = new HttpClient();
        await using var responseStream = await httpClient.GetStreamAsync(url);

        using var responseStreamReader = new StreamReader(responseStream);
        var tempStorageString = await responseStreamReader.ReadToEndAsync();
        return tempStorageString.FromCsv<List<Candle>>();
    }

    // public static async Task<List<Candle>> GetQuotesAround(
    //     string ticker,
    //     EarningsRelease earningsRelease,
    //     int lookAround = 1
    // )
    // {
    //     lookAround = Math.Max(lookAround, 1);
    //     var start = earningsRelease.Date
    //         .AddHours(-earningsRelease.Date.Hour)
    //         .AddMinutes(-earningsRelease.Date.Minute)
    //         .AddSeconds(-earningsRelease.Date.Second)
    //         .AddMilliseconds(-earningsRelease.Date.Millisecond);
    //     var end = start.AddDays(1);
    //     if (earningsRelease.DateType != "BMO")
    //         lookAround -= 1;
    //     for (var i = 0; i < lookAround; i++)
    //     {
    //         start = start.AddDays(-1);
    //         while (Util.IsHoliday(start))
    //             start = start.AddDays(-1);
    //
    //         end = end.AddDays(1);
    //         while (Util.IsHoliday(end))
    //             end = end.AddDays(1);
    //     }
    //
    //     return await GetQuotes(ticker, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
    // }
}
