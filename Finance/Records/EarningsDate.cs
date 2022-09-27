using System;

namespace CosminSanda.Finance.Records;

/// <summary>
/// An Earnings Release date is like a regular date, but can have a certain special meaning
/// based on if the call will be before or after market close.
/// </summary>
public record EarningsDate
{
    /// <summary>
    /// The calendaristic day when the earnings release call takes place
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// The date type is usually either Before Market Open(BMO) or After Market Close (AMC), but for past ERs
    /// it is set to TAS/TNS which means unspecified.
    /// </summary>
    public string DateType { get; init; }

    /// <summary>
    /// This is intended to be used in UIs to understand the split between when the market was last open before
    /// ER call, and when it was first open after the ER call.
    /// </summary>
    public string EarningsMarker =>
        DateType == "BMO"
            ? Date.AddDays(-1).ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd 12:00:00")
            : Date.ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd 12:00:00");
}
