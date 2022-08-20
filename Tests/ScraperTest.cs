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
        Persister.CreateSchema();
        var results = await Scraper.RetrieveCompaniesReporting(new DateOnly(2022, 8, 9));
        var count = results.Count();
        Console.WriteLine(count);
    }
    
}