using System;
using CosminSanda.Finance.Records;
using Newtonsoft.Json;

namespace CosminSanda.Finance.Json;

/// <summary>
/// Used for serializing/deserializing objects of type <see cref="FinancialInstrument"/>
/// </summary>
public class FinancialInstrumentJsonConverter: JsonConverter
{
    public override void WriteJson(JsonWriter writer, FinancialInstrument value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override FinancialInstrument ReadJson(JsonReader reader, Type objectType, FinancialInstrument existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        return new FinancialInstrument { Ticker = reader.ReadAsString()};        
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }
}
