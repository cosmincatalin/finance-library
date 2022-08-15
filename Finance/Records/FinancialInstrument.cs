namespace CosminSanda.Finance.Records;

/// <summary>
/// A financial instrument represents details about a publicly listed company
/// </summary>
public record FinancialInstrument
{
    private readonly string _ticker;

    /// <summary>
    /// The symbol of the company as can be found in Yahoo Finance
    /// </summary>
    public string Ticker
    {
        get => _ticker;
        init => _ticker = value.Trim().ToUpper();
    }
        
    /// <summary>
    /// The recognizable company name associated with the financial instrument
    /// </summary>
    public string CompanyName { get; init; }
}