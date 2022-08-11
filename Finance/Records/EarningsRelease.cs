namespace CosminSanda.Finance.Records;

/// <summary>
/// A slightly refined versioned of the JSON schema returned by Yahoo Finance when querying Earnings Releases
/// </summary>
public record EarningsRelease
{
    public FinancialInstrument FinancialInstrument { get; init; }

    public EarningsDate EarningsDate { get; init; }
}