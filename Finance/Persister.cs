using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CosminSanda.Finance.Records;
using Microsoft.Data.Sqlite;
using ServiceStack;

namespace CosminSanda.Finance;

/// <summary>
/// Handles caching to local storage of information retrieved from remote sources
/// </summary>
public static class Persister
{
    private static bool _schemaReady;
    
    /// <summary>
    /// Tries to use the local storage to return a list of dates when ERs took place or will
    /// take place. Returns an empty list when nothing is found locally. The item will be filtered
    /// so that any duplicates are removed.
    /// </summary>
    /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance.</param>
    /// <returns>A list of calendar dates</returns>
    public static async Task<List<EarningsRelease>> GetCachedEarnings(string ticker)
    {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CosminSanda", "Finance", $"{ticker.Trim().ToLower()}.csv");

        if (!File.Exists(filePath)) return new List<EarningsRelease>();

        var content = await File.ReadAllTextAsync(filePath);
        return content.FromCsv<List<EarningsRelease>>();
    }

    /// <summary>
    /// Saves a list of Earnings Releases to local storage as a SQLite table.
    /// If the database and table already exist, records will be added and updated so that the past does not have gaps
    /// as much as possible
    /// </summary>
    /// <param name="earnings">The list of ER objects to be persisted to local storage</param>
    public static async Task CacheEarnings(IEnumerable<EarningsRelease> earnings)
    {
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CosminSanda", "Finance", "cache.sqlite");
        await using var connection = new SqliteConnection($"Filename={filePath}");
        CreateSchema(connection);
        connection.Open();

        await using var transaction = connection.BeginTransaction();
        
        var clean = connection.CreateCommand();
        clean.CommandText = "DELETE FROM earnings_releases WHERE DATE() >= `date` AND ticker = $ticker";
        
        var parameter = clean.CreateParameter();
        parameter.ParameterName = "$ticker";
        clean.Parameters.Add(parameter);

        var earningsReleases = earnings.ToList();

        earningsReleases
            .Select(earningsRelease => earningsRelease.FinancialInstrument.Ticker)
            .Distinct()
            .ToList()
            .ForEach(ticker =>
            {
                parameter.Value = ticker;
                clean.ExecuteNonQuery();
            });
        
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO earnings_releases 
            VALUES ($ticker, $company_name, $date, $date_type, $eps_actual, $eps_estimate, $eps_surprise)
            ON CONFLICT(ticker, `date`) DO
            UPDATE SET company_name=excluded.company_name, date_type=excluded.date_type, eps_actual=excluded.eps_actual,
                eps_estimate=excluded.eps_estimate, eps_surprise=excluded.eps_surprise;
        ";
        
        var tickerParameter = command.CreateParameter();
        tickerParameter.ParameterName = "$ticker";
        command.Parameters.Add(tickerParameter);
        
        var companyNameParameter = command.CreateParameter();
        companyNameParameter.ParameterName = "$company_name";
        command.Parameters.Add(companyNameParameter);
        
        var dateParameter = command.CreateParameter();
        dateParameter.ParameterName = "$date";
        command.Parameters.Add(dateParameter);
        
        var dateTypeParameter = command.CreateParameter();
        dateTypeParameter.ParameterName = "$date_type";
        command.Parameters.Add(dateTypeParameter);
        
        var epsActualParameter = command.CreateParameter();
        epsActualParameter.ParameterName = "$eps_actual";
        command.Parameters.Add(epsActualParameter);

        var epsEstimateParameter = command.CreateParameter();
        epsEstimateParameter.ParameterName = "$eps_estimate";
        command.Parameters.Add(epsEstimateParameter);
        
        var epsSurpriseParameter = command.CreateParameter();
        epsSurpriseParameter.ParameterName = "$eps_surprise";
        command.Parameters.Add(epsSurpriseParameter);
        
        earningsReleases
            .ToList()
            .ForEach(earningsRelease =>
            {
                tickerParameter.Value = earningsRelease.FinancialInstrument.Ticker;
                companyNameParameter.Value = earningsRelease.FinancialInstrument.CompanyName;
                dateParameter.Value = earningsRelease.EarningsDate.Date.ToString("yyyy-MM-dd");
                dateTypeParameter.Value = earningsRelease.EarningsDate.DateType;
                epsActualParameter.Value = (object)earningsRelease.IncomeStatement.EpsActual ?? DBNull.Value;
                epsEstimateParameter.Value = (object)earningsRelease.IncomeStatement.EpsEstimate ?? DBNull.Value;
                epsSurpriseParameter.Value = (object)earningsRelease.IncomeStatement.EpsSurprise ?? DBNull.Value;
                
                command.ExecuteNonQuery();
            });

        transaction.Commit();
    }

    /// <summary>
    /// Creates the database and associated schema if not already present
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    private static void CreateSchema(SqliteConnection connection)
    {
        if (_schemaReady) return;
        
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CosminSanda", "Finance", "cache.sqlite");
        Console.WriteLine(filePath);
        connection.Open();

        var command = connection.CreateCommand();

        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS earnings_releases
            (
                ticker       TEXT NOT NULL,
                company_name TEXT,
                `date`       TEXT NOT NULL,
                date_type    TEXT,
                eps_actual   FLOAT,
                eps_estimate FLOAT,
                eps_surprise FLOAT,
                CONSTRAINT earnings_releases_pk
                    PRIMARY KEY (ticker, `date`)
            );
        ";

        command.ExecuteNonQuery();
        connection.Close();
        _schemaReady = true;
    }
}