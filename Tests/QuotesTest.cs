using System;
using System.Threading.Tasks;
using CosminSanda.Finance;
using NUnit.Framework;

namespace Tests
{
    public class QuotesTest
    {
        
        private const string Ticker = "ZTO";
        
        [Test]
        public async Task FetchQuotesAndCache()
        {
            var ztoQuotes = await Quotes.GetQuotes(Ticker, "2020-01-01", "2020-01-05");
            Assert.Greater(ztoQuotes.Count, 0, $"There must be at least several quotes for {Ticker}.");
        }

        [Test]
        public async Task GetQuotesAround()
        {
            var fictiveZtoEarningsDate = DateTime.UtcNow.Date.AddDays(-180);
            while (Util.IsHoliday(fictiveZtoEarningsDate)) fictiveZtoEarningsDate = fictiveZtoEarningsDate.AddDays(1);
            
            var fictiveZtoEarnings = new EarningsDate()
            {
                Date = fictiveZtoEarningsDate,
                DateType = "AMC"
            };

            var ztoQuotes = await Quotes.GetQuotesAround(Ticker, fictiveZtoEarnings, 9);
            Assert.AreEqual(0, ztoQuotes.Count % 2, "The total number of quotes needs to be even.");
        }
    }
}