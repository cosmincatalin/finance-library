namespace CosminSanda.Finance.Records;

/**
 * Financial information released during an earnings release calls 
 */
public record IncomeStatement
{
    
    /// <summary>
    /// This is actually not released by the company, but it is a consensus estimate of
    /// financial analysts ahead of the ER call
    /// </summary>
    public double? EpsEstimate { get; init; }
    
    /// <summary>
    /// The actual Earnings Per Share reported during the ER call 
    /// </summary>
    public double? EpsActual { get; init; }
    
    /// <summary>
    /// The difference between the estimated EPS and the actual EPS in percentage points
    /// </summary>
    public double? EpsSurprise { get; init; }
}