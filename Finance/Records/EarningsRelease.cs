using CosminSanda.Finance.Json;
using Newtonsoft.Json;

namespace CosminSanda.Finance.Records;

/// <summary>
/// A slightly refined versioned of the JSON schema returned by Yahoo Finance when querying Earnings Releases
/// </summary>
public record EarningsRelease
{
    [JsonConverter(typeof(FinancialInstrumentJsonConverter))]
    public FinancialInstrument Ticker { get; init; }
}