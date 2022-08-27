using System;
using System.Linq;
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
        results.ToList().ForEach(Console.WriteLine);
    }

}