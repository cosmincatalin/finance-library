namespace CosminSanda.Finance.Records
{
    /// <summary>
    /// A financial instrument represents details about a publicly listed company
    /// </summary>
    public record FinancialInstrument
    {
        private string _ticker;

        /// <summary>
        /// The symbol of the company as can be found in Yahoo Finance
        /// </summary>
        public string Ticker
        {
            get => _ticker;
            set => _ticker = value.Trim().ToUpper();
        }
    }
}