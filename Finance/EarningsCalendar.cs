using System;
using System.Collections.Generic;
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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Given a financial instrument, get the list of past ER dates available on Yahoo Finance.
    /// </summary>
    /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance.</param>
    /// <returns>A list of calendar dates</returns>
    public static async Task<List<EarningsDate>> GetPastEarningsDates(string ticker)
    {
        throw new NotImplementedException();
    }

    // public static async Task<List<EarningsRelease>> GetPastEarnings(string ticker, int limit = int.MaxValue)
    // {
    //     var now = DateTime.UtcNow;
    //     var earnings = await GetPastEarningsDates(ticker);
    //     return earnings
    //         .Where(o => o.Date.CompareTo(now) < 0)
    //         .OrderByDescending(o => o.Date)
    //         .Take(limit)
    //         .ToList();
    // }
    
    /// <summary>
    /// A method to get the next estimated(or set) earnings release call.
    /// </summary>
    /// <param name="ticker"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static async Task<EarningsRelease> GetNextEarningsDate(string ticker)
    {
        throw new NotImplementedException();
    }

    // private static bool InvalidateCache(IEnumerable<EarningsRelease> earnings)
    // {
    //     var now = DateTime.UtcNow.AddDays(-5);
    //     return earnings.Any(earning => earning.Date < now && earning.EpsActual == null);
    // }
    
}