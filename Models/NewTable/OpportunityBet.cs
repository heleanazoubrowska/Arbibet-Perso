using Newtonsoft.Json;
using System.Collections.Generic;
namespace ArbibetProgram.Models
{
    public class OpportunityBet
    {

        [JsonProperty("arb_uniq_id")]
        public string ArbUniqId { get; set; }

        [JsonProperty("account_name_1")]
        public string AccountName1 { get; set; }

        [JsonProperty("bookmaker_name")]
        public string BoomakerName { get; set; }

        [JsonProperty("team_bet")]
        public string TeamBet { get; set; }

        [JsonProperty("odd_type")]
        public int OddType { get; set; }

        [JsonProperty("odd_initial")]
        public string OddInitial { get; set; }

        [JsonProperty("bet_amount")]
        public decimal BetAmount { get; set; }

        [JsonProperty("bet_status")]
        public int BetStatus { get; set; }


        public OpportunityBet()
        {

        }
    }
}
