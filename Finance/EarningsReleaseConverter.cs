using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CosminSanda.Finance.Records;

namespace CosminSanda.Finance;

/// <summary>
/// A custom JSON converter that allows unmarshalling the Yahoo Finance earnings release schema to
/// a nested naturally incompatible domain schema
/// </summary>
public class EarningsReleaseConverter : JsonConverter<CosminSanda.Finance.Records.EarningsRelease>
{
    /// <summary>
    /// Deserialize a Yahoo Finance earnings release JSON record to a domain object 
    /// </summary>
    /// <returns>An earnings release object with details about a specific earnings release portraying to a company</returns>
    /// <exception cref="JsonException"></exception>
    public override CosminSanda.Finance.Records.EarningsRelease Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        reader.Read();
        
        if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

        DateOnly erEarningsDateDate;
        string erEarningsDateDateType = null;
        
        while (reader.Read())
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                Console.WriteLine(erEarningsDateDate);
                return new Records.EarningsRelease {
                    EarningsDate = new EarningsDate {Date = erEarningsDateDate, DateType = erEarningsDateDateType}
                };
            }

            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            
            var propertyName = reader.GetString();
            reader.Read();
            switch (propertyName)
            {
                case "startdatetime":
                    var date = DateTime.ParseExact(reader.GetString()!, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
                    erEarningsDateDate = DateOnly.FromDateTime(date);
                    break;
                case "startdatetimetype":
                    erEarningsDateDateType = reader.GetString()!.Trim().ToUpper();
                    break;
            }
        }
        
        throw new JsonException();
    }

    /// <summary>
    /// Marshalling is a trivial but unnecessary feature 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void Write(Utf8JsonWriter writer, CosminSanda.Finance.Records.EarningsRelease value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}