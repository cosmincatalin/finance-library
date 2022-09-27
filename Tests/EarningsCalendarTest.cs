using System;
using System.Threading.Tasks;
using CosminSanda.Finance;
using NUnit.Framework;

namespace Tests;

public class EarningsCalendarTest
{
    [Test]
    public async Task FetchEarnings()
    {
        var ztoEarnings = await EarningsCalendar.GetPastEarningsDates("MSFT");
        Assert.Greater(
            ztoEarnings.Count,
            110,
            $"There must be more than 110 earnings calls for the Microsoft Corp."
        );
    }

    [Test]
    public async Task GetNextEarningsDate()
    {
        var date = await EarningsCalendar.GetNextEarningsDate("MSFT");
        Assert.NotNull(date, "The date should not be null");
    }
}
