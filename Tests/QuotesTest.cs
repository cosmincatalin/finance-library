using System;
using System.Threading.Tasks;
using CosminSanda.Finance;
using NUnit.Framework;

namespace Tests;

public class QuotesTest
{
        
    private const string Ticker = "ZTO";
        
    [Test]
    public async Task FetchQuotesAndCache()
    {
        var ztoQuotes = await Quotes.GetQuotes(Ticker, "2020-01-01", "2020-01-05");
        Assert.Greater(ztoQuotes.Count, 0, $"There must be at least several quotes for {Ticker}.");
    }

    [Test]
    public async Task GetQuotesAround()
    {
        var lookAround = 6;
        // var earnings = await EarningsCalendar.GetPastEarnings(Ticker, 3);
        // var quotes = await Quotes.GetQuotesAround(Ticker, earnings[0], lookAround);
        // Assert.AreEqual(lookAround * 2, quotes.Count , "there must be exactly double the quotes days as the lookAround.");
    }
}