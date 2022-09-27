using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CosminSanda.Finance.Records;

namespace CosminSanda.Finance.JsonConverters;

/// <summary>
/// A custom JSON converter that allows unmarshalling the Yahoo Finance earnings release schema to
/// a nested naturally incompatible domain schema
/// </summary>
public class EarningsReleaseConverter : JsonConverter<EarningsRelease>
{
    /// <summary>
    /// Deserialize a Yahoo Finance earnings release JSON record to a domain object
    /// </summary>
    /// <returns>An earnings release object with details about a specific earnings release portraying to a company</returns>
    /// <exception cref="JsonException"></exception>
    public override EarningsRelease Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        string ticker = null;
        string company = null;
        // ReSharper disable once TooWideLocalVariableScope
        DateOnly earningsDate;
        string earningsDateType = null;
        double? epsEstimate = null;
        double? epsActual = null;
        double? epsSurprise = null;

        while (reader.Read())
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return new EarningsRelease
                {
                    FinancialInstrument = new FinancialInstrument
                    {
                        Ticker = ticker,
                        CompanyName = company
                    },
                    EarningsDate = new EarningsDate
                    {
                        Date = earningsDate,
                        DateType = earningsDateType
                    },
                    IncomeStatement = new IncomeStatement
                    {
                        EpsEstimate = epsEstimate,
                        EpsActual = epsActual,
                        EpsSurprise = epsSurprise
                    }
                };
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

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
                    var date = DateTime.ParseExact(
                        reader.GetString()!,
                        "yyyy-MM-dd'T'HH:mm:ss'Z'",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal
                    );
                    earningsDate = DateOnly.FromDateTime(date);
                    break;
                case "startdatetimetype":
                    earningsDateType = reader.GetString()!.Trim().ToUpper();
                    break;
                case "epsestimate":
                    epsEstimate =
                        reader.TokenType == JsonTokenType.Null ? null : reader.GetDouble();
                    break;
                case "epsactual":
                    epsActual = reader.TokenType == JsonTokenType.Null ? null : reader.GetDouble();
                    break;
                case "epssurprisepct":
                    epsSurprise =
                        reader.TokenType == JsonTokenType.Null ? null : reader.GetDouble();
                    break;
            }
        }

        throw new JsonException();
    }

    /// <summary>
    /// Marshalling is a trivial but unnecessary feature
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void Write(
        Utf8JsonWriter writer,
        EarningsRelease value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}
