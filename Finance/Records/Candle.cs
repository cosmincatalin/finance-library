using System;

namespace CosminSanda.Finance.Records;

/// <summary>
/// A Japanese Candle that indicates the price action of a time interval for a company
/// </summary>
public record Candle
{

    /// <summary>
    /// The company to which the quotes relate to
    /// </summary>
    public FinancialInstrument FinancialInstrument { get; init; }

    /// <summary>
    /// The day to which the quotes relate to
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// The open quote on the day
    /// </summary>
    public double Open { get; init; }

    /// <summary>
    /// The highest quote intraday
    /// </summary>
    public double High { get; init; }

    /// <summary>
    /// The lowest quote intraday
    /// </summary>
    public double Low { get; init; }

    /// <summary>
    /// The quote at market close
    /// </summary>
    public double Close { get; init; }

    /// <summary>
    /// The string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{FinancialInstrument.Ticker},{Date:yyyy-MM-dd},{Open:.##},{High:.##},{Low:.##},{Close:.##}";
    }

}