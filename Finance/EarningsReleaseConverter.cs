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

        string ticker = null;
        string company = null;
        DateOnly earningsDate;
        string earningsDateType = null;

        while (reader.Read())
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new Records.EarningsRelease {
                    FinancialInstrument = new FinancialInstrument
                    {
                        Ticker  = ticker,
                        CompanyName = company
                    },
                    EarningsDate = new EarningsDate
                    {
                        Date = earningsDate,
                        DateType = earningsDateType
                    }
                };
            }

            if (reader.TokenType != JsonTokenType.PropertyName) continue;
            
            var propertyName = reader.GetString();
            reader.Read();
            switch (propertyName)
            {
                case "ticker":
                    ticker = reader.GetString()!.Trim().ToUpper();
                    break;
                case "companyshortname":
                    company = reader.GetString()!.Trim();
                    break;
                case "startdatetime":
                    var date = DateTime.ParseExact(reader.GetString()!, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
                    earningsDate = DateOnly.FromDateTime(date);
                    break;
                case "startdatetimetype":
                    earningsDateType = reader.GetString()!.Trim().ToUpper();
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