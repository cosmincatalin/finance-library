using System;

namespace CosminSanda.Finance.Records
{
    /// <summary>
    /// An Earnings Release date is like a regular date, but can have a certain special meaning
    /// based on if the call will be before or after market close.
    /// </summary>
    public record EarningsDate
    {
        public DateOnly Date { get; init; }

        public string DateType { get; init; }
        
        /// <summary>
        /// This is intended to be used in UIs to understand the split between when the market was last open before
        /// ER call, and when it was first open after the ER call.
        /// </summary>
        public string EarningsMarker => DateType.Trim().ToUpper() != "BMO" ? Date.ToString("yyyy-MM-dd 12:00:00") : Date.AddDays(-1).ToString("yyyy-MM-dd 12:00:00");
    }
}