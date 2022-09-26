using System;
using System.Threading.Tasks;
using CosminSanda.Finance;
using NUnit.Framework;

namespace Tests;

public class QuotesTest
{
    private const string Ticker = "MSFT";

    [Test]
    public async Task FetchQuotes()
    {
        var ztoQuotes = await Quotes.GetQuotes(Ticker, "2020-01-01", "2020-01-05");
        Assert.Greater(ztoQuotes.Count, 0, $"There must be at least several quotes for {Ticker}.");
    }

    [Test]
    public async Task GetQuotesAround()
    {
        const int lookAround = 6;
        var earnings = await EarningsCalendar.GetPastEarningsDates(Ticker);
        var selectedEarning = earnings[0];
        if (earnings.Count > 2)
        {
            selectedEarning = earnings[earnings.Count - 1];
        }
        Console.Out.WriteLine(selectedEarning);
        var quotes = await Quotes.GetQuotesAround(Ticker, selectedEarning, lookAround);
        quotes.ForEach(Console.Out.WriteLine);
        Assert.AreEqual(
            lookAround * 2,
            quotes.Count,
            "there must be exactly double the quotes days as the lookAround."
        );
    }
}
