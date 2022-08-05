using System;
using Newtonsoft.Json;

namespace CosminSanda.Finance
{
    public class EarningsRelease
    {
        [JsonProperty("ticker")]
        public string Ticker { get; private set; }

        [JsonProperty("startdatetime")]
        public DateTime Date { get; set; }
        
        [JsonProperty("startdatetimetype")]
        public string DateType { get; set; }
        
        [JsonProperty("epsestimate")]
        public float? EpsEstimate { get; set; }
        
        [JsonProperty("epsactual")]
        public float? EpsActual { get; set; }

        public string EarningsMarker => DateType != "BMO" ? Date.ToString("yyyy-MM-dd 12:00:00") : Date.AddDays(-1).ToString("yyyy-MM-dd 12:00:00");

        public override string ToString()
        {
            return $"{Ticker},{Date:yyyy-MM-dd},{DateType.ToUpper()},{EpsEstimate},{EpsActual}";
        }

        public EarningsRelease WithTicker(string ticker)
        {
            Ticker = ticker;
            return this;
        }
        
    }
}