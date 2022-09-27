namespace CosminSanda.Finance.Records;

/// <summary>
/// A slightly refined versioned of the JSON schema returned by Yahoo Finance when querying Earnings Releases
/// </summary>
public record EarningsRelease
{
    /// <summary>
    /// Details related to the company and its listing on the stock exchange according to Yahoo Finance
    /// </summary>
    public FinancialInstrument FinancialInstrument { get; init; }

    /// <summary>
    /// The earnings call schedule
    /// </summary>
    public EarningsDate EarningsDate { get; init; }

    /// <summary>
    /// Financial income details released during an earnings call
    /// </summary>
    public IncomeStatement IncomeStatement { get; init; }
}
