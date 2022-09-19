using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CosminSanda.Finance.JsonConverters;
using CosminSanda.Finance.Records;
using Newtonsoft.Json.Linq;

namespace CosminSanda.Finance;

/// <summary>
/// A class used specifically for retrieving data from Yahoo Finance. This is a scraper utility and will not use a cache.
/// </summary>
internal static class Scraper
{
    private const string Url = "https://finance.yahoo.com/calendar/earnings";
    private const string Bookmark = "root.App.main = ";

    /// <summary>
    /// Try to retrieve from Yahoo Finance the earnings releases on a given day.
    ///
    /// This method will not use any cache and will try to reach the Yahoo Finance service
    /// </summary>
    /// <param name="day">The day for when you want to know the scheduled earning calls.</param>
    /// <returns>A list of generic earning releases objects</returns>
    public static async Task<IEnumerable<EarningsRelease>> RetrieveEarningsReleases(DateOnly day)
    {
        var query = $"day={day.ToString("yyyy-MM-dd")}";
        return await RetrieveEarningsData(query);
    }

    /// <summary>
    /// Try to retrieve from Yahoo Finance the earnings calls a company has reported in the past and also
    /// in the future. It is important to note that future dates are not set in stone and are likely to change.
    ///
    /// This method will not use any cache and will try to reach the Yahoo Finance service
    /// </summary>
    /// <param name="company">The financial instrument associated with the company</param>
    /// <returns>A list of generic earning releases objects</returns>
    public static async Task<IEnumerable<EarningsRelease>> RetrieveEarningsReleases(
        FinancialInstrument company
    )
    {
        var query = $"symbol={company.Ticker}";
        return await RetrieveEarningsData(query);
    }

    /// <summary>
    /// Retrieves the total number of earnings calls for a query
    /// </summary>
    /// <param name="query"></param>
    /// <returns>The total number of eranings calls</returns>
    private static async Task<int> GetNumberOfEarningCalls(string query)
    {
        using var httpClient = new HttpClient();

        var url = $"{Url}?{query}&offset=0&size=1";

        Console.Out.WriteLine($"URL used: {url}");

        await using var responseStream = await httpClient.GetStreamAsync(url);

        using var responseStreamReader = new StreamReader(responseStream);
        var htmlSource = await responseStreamReader.ReadToEndAsync();

        return htmlSource
            .Split("\n")
            .Where(o => o.StartsWith(Bookmark))
            .Take(1)
            .Select(o => o.Substring(Bookmark.Length, o.Length - 1 - Bookmark.Length))
            .Select(JObject.Parse)
            .Select(
                o =>
                    o.SelectToken("$.context.dispatcher.stores.ScreenerResultsStore.results.total")!
                        .Value<int>()
            )
            .First();
    }

    /// <summary>
    /// Get the actual list of earnings calls. This might require several requests.
    /// </summary>
    /// <param name="query">Either a date based query or a company based one</param>
    /// <returns>The total list of earnings releases</returns>
    private static async Task<IEnumerable<EarningsRelease>> RetrieveEarningsData(string query)
    {
        var total = await GetNumberOfEarningCalls(query);

        var offset = -100;
        const int size = 100;

        var earningsReleases = Enumerable.Empty<EarningsRelease>();

        using var httpClient = new HttpClient();

        while (offset + size < total)
        {
            offset += size;
            await using var responseStream = await httpClient.GetStreamAsync(
                $"{Url}?{query}&offset={offset}&size={size}"
            );
            using var responseStreamReader = new StreamReader(responseStream);

            var htmlSource = await responseStreamReader.ReadToEndAsync();

            earningsReleases = htmlSource
                .Split("\n")
                .Where(o => o.StartsWith(Bookmark))
                .Take(1)
                .Select(o => o.Substring(Bookmark.Length, o.Length - 1 - Bookmark.Length))
                .Select(JObject.Parse)
                .SelectMany(
                    o =>
                        o.SelectTokens(
                            "$.context.dispatcher.stores.ScreenerResultsStore.results.rows[*]"
                        )
                )
                .Select(o =>
                {
                    var options = new JsonSerializerOptions();
                    options.Converters.Add(new EarningsReleaseConverter());
                    return JsonSerializer.Deserialize<EarningsRelease>(o.ToString(), options);
                })
                .Concat(earningsReleases);
        }

        return earningsReleases;
    }
}
