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

        var results = await Scraper.RetrieveEarningsReleases(new FinancialInstrument{ Ticker = "MSFT"});
        await Persister.CacheEarnings(results);
    }
    
}