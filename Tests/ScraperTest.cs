using System;
using System.Threading.Tasks;
using CosminSanda.Finance;
using CosminSanda.Finance.Records;
using NUnit.Framework;

namespace Tests;

public class ScraperTest
{

    [Test]
    public async Task Runner()
    {

        var results = await Scraper.RetrieveEarningsReleases(new FinancialInstrument{ Ticker = "AMRS"});
        Persister.CacheEarnings(results);
        foreach (var er in Persister.GetCachedEarnings(new FinancialInstrument{ Ticker = "AMRS"}))
        {
            Console.Write("{0} ", er);
        }
    }

}