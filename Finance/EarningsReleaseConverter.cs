using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CosminSanda.Finance;

// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-6-0#support-polymorphic-deserialization
public class EarningsReleaseConverter : JsonConverter<CosminSanda.Finance.Records.EarningsRelease>
{
    public override CosminSanda.Finance.Records.EarningsRelease Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        var propertyName = reader.GetString();
        if (propertyName != "startdatetimetype")
        {
            throw new JsonException();
        }

        return new CosminSanda.Finance.Records.EarningsRelease{ Ticker = new Records.FinancialInstrument{ Ticker = reader.GetString()} };
    }

    public override void Write(Utf8JsonWriter writer, CosminSanda.Finance.Records.EarningsRelease value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}