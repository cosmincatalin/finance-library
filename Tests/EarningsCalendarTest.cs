using System;
using System.IO;
using System.Threading.Tasks;
using CosminSanda.Finance;
using NUnit.Framework;

namespace Tests
{
    public class EarningsCalendarTest
    {
        private const string Ticker = "ZTO";

        [OneTimeSetUp]
        public void SetUp()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + EarningsCalendar.CacheSubPath;
            var filePath = Path.Combine(folderPath, $"{Ticker}.csv");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

        }

        [Test, Order(1)]
        public async Task FetchEarningsAndCache()
        {
            var ztoEarnings = await EarningsCalendar.GetPastEarnings(Ticker);
            Assert.Greater(ztoEarnings.Count, 0, $"There must be at least several earning dates for {Ticker}.");
        }

        [Test, Order(2)]
        public async Task LoadCachedEarnings()
        {
            var earnings = await EarningsCalendar.LoadCachedEarnings(Ticker);
            Assert.Greater(earnings.Count, 0, $"There must be at least several earning dates for {Ticker}.");
        }
        
        [Test, Order(3)]
        public async Task LoadUnknownCachedEarnings()
        {
            var earnings = await EarningsCalendar.LoadCachedEarnings("UNKNOWN");
            Assert.AreEqual(0, earnings.Count, "There must be 0 earnings fro UNKNOWN");
        }
        
        [Test, Order(4)]
        public async Task GetNextEarningsDate()
        {
            var date = await EarningsCalendar.GetNextEarningsDate(Ticker);
            Assert.NotNull(date, "The date should not be null");
        }

        [Test, Order(0)]
        public async Task FetchEarningsAndCacheByDay()
        {
            DateTime day = new DateTime(2022, 7, 6);
            var earnings = await EarningsCalendar.GetEarnings(day);
            Assert.Greater(earnings.Count, 0, $"There must be at least several earning dates for {Ticker}.");
        }
    }
}