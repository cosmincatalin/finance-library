using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CosminSanda.Finance.Records;

namespace CosminSanda.Finance;
public class FinancialInstrumentConverter : JsonConverter<FinancialInstrument>
{
    public override FinancialInstrument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new FinancialInstrument{ Ticker = reader.GetString()};
    }

    public override void Write(Utf8JsonWriter writer, FinancialInstrument value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}