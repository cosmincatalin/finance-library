using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosminSanda.Finance.Records;

namespace CosminSanda.Finance;

/// <summary>
/// A static class that provides methods for retrieving information around earnings calls.
/// </summary>
public static class EarningsCalendar
{
    /// <summary>
    /// Get a list of companies that report earnings on a specific day.
    /// </summary>
    /// <example>
    /// In this example we get a list of companies that report on 2022-09-28
    /// <code language="c#">
    /// using CosminSanda.Finance;
    ///
    /// var companies = await EarningsCalendar
    ///     .GetCompaniesReporting(new DateTime(year: 2022, month: 9, day: 28));
    /// companies.ForEach(Console.WriteLine);
    /// </code>
    /// </example>
    /// <param name="day">The day for which you want to know the companies reporting earnings</param>
    /// <returns>A list of companies</returns>
    public static async Task<List<FinancialInstrument>> GetCompaniesReporting(DateTime day)
    {
        var date = DateOnly.FromDateTime(day);

        var earnings = await Scraper.RetrieveEarningsReleases(date);
        return earnings.Select(o => o.FinancialInstrument).ToList();
    }

    /// <summary>
    /// Given a financial instrument, get the list of past earnings releases dates available on Yahoo Finance.
    /// </summary>
    /// <example>
    /// In this example we get past earnings calls dates for the Microsoft Corporation.
    /// <code language="c#">
    /// using CosminSanda.Finance;
    ///
    /// var earnings = await EarningsCalendar.GetPastEarningsDates("msft");
    /// earnings.ForEach(Console.WriteLine);
    /// </code>
    /// </example>
    /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance. The case is not important.</param>
    /// <returns>A list of calendar dates</returns>
    public static async Task<List<EarningsDate>> GetPastEarningsDates(string ticker)
    {
        var financialInstrument = new FinancialInstrument { Ticker = ticker };
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var earnings = await Scraper.RetrieveEarningsReleases(financialInstrument);
        return earnings
            .Where(o => o.EarningsDate.Date < today)
            .Select(o => o.EarningsDate)
            .OrderBy(o => o.Date)
            .ToList();
    }

    /// <summary>
    /// A method to get the next estimated(or set) earnings release call.
    /// </summary>
    /// <example>
    /// <code language="c#">
    /// using CosminSanda.Finance;
    ///
    /// var earning = await EarningsCalendar.GetNextEarningsDate("msft");
    /// Console.WriteLine(earning);
    /// </code>
    /// </example>
    /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance. The case is not important.</param>
    /// <returns>An earnings date in the future</returns>
    public static async Task<EarningsDate> GetNextEarningsDate(string ticker)
    {
        var financialInstrument = new FinancialInstrument { Ticker = ticker };
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var earnings = await Scraper.RetrieveEarningsReleases(financialInstrument);
        return earnings
            .Where(o => o.EarningsDate.Date >= today)
            .Select(o => o.EarningsDate)
            .OrderBy(o => o.Date)
            .Take(1)
            .First();
    }
}
