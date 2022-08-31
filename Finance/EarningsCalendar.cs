using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosminSanda.Finance.Records;

namespace CosminSanda.Finance;

/// <summary>
/// The main class that should be utilized to get information about earnings releases
/// </summary>
public static class EarningsCalendar
{

    /// <summary>
    /// Get a list of companies that report earnings on a specific day.
    /// </summary>
    /// <param name="day">The day for which you want to know the companies reporting earnings</param>
    /// <returns>A list of financial instruments</returns>
    public static async Task<List<FinancialInstrument>> GetCompaniesReporting(DateTime day)
    {
        var date = DateOnly.FromDateTime(day);

        var earnings = await Scraper.RetrieveEarningsReleases(date);
        return earnings
            .Select(o => o.FinancialInstrument)
            .ToList();
    }

    /// <summary>
    /// Given a financial instrument, get the list of past ER dates available on Yahoo Finance.
    /// </summary>
    /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance.</param>
    /// <returns>A list of calendar dates</returns>
    public static async Task<List<EarningsDate>> GetPastEarningsDates(string ticker)
    {
        var financialInstrument = new FinancialInstrument{ Ticker = ticker};
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var earnings = await Scraper.RetrieveEarningsReleases(financialInstrument);
        return earnings
            .Where(o => o.EarningsDate.Date < DateOnly.FromDateTime(DateTime.Now))
            .Select(o => o.EarningsDate)
            .OrderBy(o => o.Date)
            .ToList();
    }

    /// <summary>
    /// A method to get the next estimated(or set) earnings release call.
    /// </summary>
    /// <param name="ticker"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static async Task<EarningsDate> GetNextEarningsDate(string ticker)
    {
        var financialInstrument = new FinancialInstrument{ Ticker = ticker};
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var earnings = await Scraper.RetrieveEarningsReleases(financialInstrument);
        return earnings
            .Where(o => o.EarningsDate.Date >= DateOnly.FromDateTime(DateTime.Now))
            .Select(o => o.EarningsDate)
            .OrderBy(o => o.Date)
            .Take(1)
            .ToList()
            .First();
    }

}