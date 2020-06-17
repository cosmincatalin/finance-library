using System;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;

namespace CosminSanda.Finance
{
    public class Candle
    {
        private float _close;
        
        public string Ticker { get; set; }

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
            return $"{Date:yyyy-MM-dd},{Open:.##},{High:.##},{Low:.##},{Close:.##}";
        }
    }

}