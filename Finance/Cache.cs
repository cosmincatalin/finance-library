using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Text;

namespace CosminSanda.Finance
{
    /// <summary>
    /// Handles caching to local storage of information retrieved from remote sources
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Tries to use the local storage to return a list of dates when ERs took place or will
        /// take place. Returns an empty list when nothing is found locally. The item will be filtered
        /// so that any duplicates are removed.
        /// </summary>
        /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance.</param>
        /// <returns>A list of calendar dates</returns>
        public static async Task<List<EarningsDate>> GetCachedEarnings(string ticker)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CosminSanda", "Finance", $"{ticker.Trim().ToLower()}.csv");

            if (!File.Exists(filePath)) return new List<EarningsDate>();

            var content = await File.ReadAllTextAsync(filePath);
            return content.FromCsv<List<EarningsDate>>();
        }

        /// <summary>
        /// Saves a list of ER dates associated with a ticker to local storage as a .csv file.
        /// If the file already exists, data will be overwritten.
        /// TODO: Instead of overwriting, append, but make sure updates are applied for existing dates
        /// </summary>
        /// <param name="ticker">The financial instrument's ticker as used on Yahoo Finance.</param>
        /// <param name="earnings">The list of ER dates to be persisted to local storage</param>
        public static async Task CacheEarnings(string ticker, List<EarningsDate> earnings)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CosminSanda", "Finance", $"{ticker.Trim().ToLower()}.csv");
            
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CosminSanda", "Finance"));

            JsConfig<DateTime>.SerializeFn = date => date.ToString("yyyy-MM-dd");

            if (File.Exists(filePath)) CsvConfig<EarningsDate>.OmitHeaders = true;

            await using var csv = new StreamWriter(filePath, append: false);
            CsvSerializer.SerializeToWriter(earnings, csv);
        }
    }
}