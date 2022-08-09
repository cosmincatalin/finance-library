using System;
using System.Linq;
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
        var results = await Scraper.RetrieveEarningsDates(new FinancialInstrument{ Ticker = "MSFT"});
        var count = results.Count();
        Console.WriteLine(count);
    }
    
}