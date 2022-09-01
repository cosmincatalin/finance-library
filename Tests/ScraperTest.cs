using System;
using System.Threading.Tasks;
using CosminSanda.Finance;
using NUnit.Framework;

namespace Tests;

public class ScraperTest
{
    [Test]
    public async Task Runner()
    {
        var results = await EarningsCalendar.GetPastEarningsDates(ticker: "MSFT");
        results.ForEach(Console.WriteLine);
        var quotes = await Quotes.GetQuotes("MSFT", "2022-08-01", "2022-08-05");
        quotes.ForEach(Console.WriteLine);
    }
}
