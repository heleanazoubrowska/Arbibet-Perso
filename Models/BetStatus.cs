using Newtonsoft.Json;
using System.Collections.Generic;
namespace ArbibetProgram.Models
{
    public class BetStatus
    {

        [JsonProperty("arbitrage_gui")]
        public string AribtrageGui { get; set; }

        [JsonProperty("bet_status")]
        public string BetNewStatus { get; set; }

        [JsonProperty("bookmaker_name")]
        public string BookmakerName { get; set; }

        [JsonProperty("actual_odd")]
        public decimal ActualOdd { get; set; }

        public BetStatus()
        {
        }
    }

    public class CasinoBetStatus
    {
        public string NewStatus { get; set; }

        public decimal ActualOdd { get; set; }
    }
}
