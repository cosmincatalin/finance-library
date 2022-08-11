using System;

namespace CosminSanda.Finance.Records;

/// <summary>
/// A Japanese Candle that indicates the price action of a time interval for a company
/// </summary>
public record Candle
{

    public FinancialInstrument FinancialInstrument { get; init; }

    public DateOnly Date { get; init; }

    public double Open { get; init; }

    public double High { get; init; }

    public double Low { get; init; }

    public double Close { get; init; }

    public override string ToString()
    {
        return $"{FinancialInstrument.Ticker},{Date:yyyy-MM-dd},{Open:.##},{High:.##},{Low:.##},{Close:.##}";
    }

}