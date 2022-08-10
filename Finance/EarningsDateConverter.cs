using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using CosminSanda.Finance.Records;

namespace CosminSanda.Finance;

public class EarningsDateConverter : JsonConverter<EarningsDate>
{
    public override EarningsDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new EarningsDate{ DateType = "BMO"};
    }

    public override void Write(Utf8JsonWriter writer, EarningsDate value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}