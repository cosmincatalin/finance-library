using System;
using System.Collections.Generic;
using System.IO;
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
        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CosminSanda", "Finance"));

        await using var connection = new SqliteConnection($"Data Source={filePath}");
        connection.Open();
    }

    /// <summary>
    /// Creates the database and associated schema if not already present
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void CreateSchema()
    {
        if (_schemaReady) return;
        
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CosminSanda", "Finance", "cache.sqlite");
        Console.WriteLine(filePath);
        using var connection = new SqliteConnection($"Filename={filePath}");
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
        _schemaReady = true;
    }
}