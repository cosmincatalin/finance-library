using System;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;

namespace CosminSanda.Finance
{
    public class Candle
    {
        
        public string Ticker { get; private set; }
        
        private float _close;

        public DateTime Date { get; set; }

        public float Open { get; set; }

        public float High { get; set; }

        public float Low { get; set; }

        public float Close
        {
            get => AdjustedClose ?? _close;
            set => _close = value;
        }
        
        public float? AdjustedClose { get; set; }
        
        public override string ToString()
        {
            return $"{Ticker},{Date:yyyy-MM-dd},{Open:.##},{High:.##},{Low:.##},{Close:.##}";
        }
        
        public Candle WithTicker(string ticker)
        {
            Ticker = ticker;
            return this;
        }
    }

}