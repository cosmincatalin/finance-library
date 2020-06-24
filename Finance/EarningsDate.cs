using System;
using Newtonsoft.Json;

namespace CosminSanda.Finance
{
    public class EarningsDate
    {
        public string Ticker { get; private set; }

        [JsonProperty("startdatetime")]
        public DateTime Date { get; set; }
        
        [JsonProperty("startdatetimetype")]
        public string DateType { get; set; }
        
        [JsonProperty("epsestimate")]
        public float? EpsEstimate { get; set; }
        
        [JsonProperty("epsactual")]
        public float? EpsActual { get; set; }

        public override string ToString()
        {
            return $"{Ticker},{Date:yyyy-MM-dd},{DateType.ToUpper()},{EpsEstimate},{EpsActual}";
        }

        public EarningsDate WithTicker(string ticker)
        {
            Ticker = ticker;
            return this;
        }
        
    }
}