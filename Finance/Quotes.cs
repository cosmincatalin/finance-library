using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CosminSanda.Finance.Records;
using ServiceStack.Text;
using CsvHelperParty = CsvHelper;

namespace CosminSanda.Finance;

/// <summary>
/// Contains methods for retrieving daily quotes (prices) for selected companies.
/// </summary>
public static class Quotes
{
    /// <summary>
    /// Retrieve the Japanese candles for each day of a time interval for a specified company.
    /// </summary>
    /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance.</param>
    /// <param name="startDate">The day from when you start retrieving the quotes, inclusive.</param>
    /// <param name="endDate">The day until when you retrieve the quotes, inclusive</param>
    /// <returns>A list of quotes in ascending order</returns>
    public static async Task<List<Candle>> GetQuotes(
        string ticker,
        string startDate,
        string endDate
    )
    {
        var start = DateTime
            .ParseExact(
                startDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal
            )
            .ToUniversalTime();
        var end = DateTime
            .ParseExact(
                endDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal
            )
            .ToUniversalTime()
            .AddDays(1);
        var url =
            $"https://query1.finance.yahoo.com/v7/finance/download/{ticker}?period1={start.ToUnixTime()}&period2={end.ToUnixTime()}&interval=1d&events=history";

        await Console.Out.WriteLineAsync($"URL used: {url}");

        using var httpClient = new HttpClient();
        await using var responseStream = await httpClient.GetStreamAsync(url);

        using var responseStreamReader = new StreamReader(responseStream);
        using var csvReader = new CsvHelperParty.CsvReader(
            responseStreamReader,
            CultureInfo.InvariantCulture
        );
        var records = new List<Candle>();
        await csvReader.ReadAsync();
        csvReader.ReadHeader();
        while (await csvReader.ReadAsync())
        {
            var record = new Candle
            {
                FinancialInstrument = new FinancialInstrument { Ticker = ticker },
                Date = DateOnly.FromDateTime(
                    DateTime.ParseExact(
                        csvReader.GetField("Date"),
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal
                    )
                ),
                Open = csvReader.GetField<double>("Open"),
                High = csvReader.GetField<double>("High"),
                Low = csvReader.GetField<double>("Low"),
                Close = csvReader.GetField<double>("Adj Close"),
                Volume = csvReader.GetField<int>("Volume")
            };
            records.Add(record);
        }

        records.Sort((o1, o2) => o1.Date.CompareTo(o2.Date));
        return records;
    }

    /// <summary>
    /// Retrieves the quotes around an earnings call.
    /// Contains logic around the time relative to the market open.
    /// </summary>
    /// <param name="ticker">The symbol of the financial instrument as mastered by Yahoo Finance</param>
    /// <param name="earningsDate">An earnings call date</param>
    /// <param name="lookAround">the number of days before and after the earnings call</param>
    /// <returns></returns>
    public static async Task<List<Candle>> GetQuotesAround(
        string ticker,
        EarningsDate earningsDate,
        int lookAround = 2
    )
    {
        lookAround = Math.Max(lookAround, 1);
        var start = earningsDate.Date;
        var end = earningsDate.Date.AddDays(1);
        while (Util.IsHoliday(end))
            end = end.AddDays(1);
        if (earningsDate.DateType == "BMO") {
            start = earningsDate.Date.AddDays(-1);
            while (Util.IsHoliday(start))
                start = start.AddDays(-1);
            end = earningsDate.Date;
        }
        for (var i = 1; i < lookAround; i++)
        {
            start = start.AddDays(-1);
            while (Util.IsHoliday(start))
                start = start.AddDays(-1);

            end = end.AddDays(1);
            while (Util.IsHoliday(end))
                end = end.AddDays(1);
        }

        return await GetQuotes(ticker, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
    }
}
