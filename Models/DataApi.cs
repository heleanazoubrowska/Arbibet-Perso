using Newtonsoft.Json;
using System.Collections.Generic;
namespace ArbibetProgram.Models
{
    public class DataApi
    {
        [JsonProperty("list_account_info")]
        public List<AccountInfoApi> ListAccountInfoApi { get; set; }

        [JsonProperty("list_opportunities")]
        public List<OpportunitiesApi> ListOpportunities { get; set; }

        [JsonProperty("list_bets")]
        public List<BetsApi> ListBets { get; set; }

        public DataApi()
        {
            ListAccountInfoApi = new List<AccountInfoApi>();
            ListOpportunities = new List<OpportunitiesApi>();
            ListBets = new List<BetsApi>();
        }
    }

    public class AccountInfoApi
    {
        [JsonProperty("account_name")]
        public string Accountname { get; set; }

        [JsonProperty("account_balance")]
        public decimal AccountBalance { get; set; }
    }

    public class OpportunitiesApi
    {
        [JsonProperty("arb_bookmaker_1")]
        public string ArbBookmaker1 { get; set; }

        [JsonProperty("arb_bookmaker_2")]
        public string ArbBookmaker2 { get; set; }

        [JsonProperty("arb_duration")]
        public int ArbDuration { get; set; }

        [JsonProperty("arb_game")]
        public string ArbGame { get; set; }

        [JsonProperty("arb_type")]
        public string ArbType { get; set; }

        [JsonProperty("arb_gui")]
        public string ArbGui { get; set; }

        [JsonProperty("arb_potential_profit_team_fav")]
        public decimal ArbPotentialProfitTeamFav { get; set; }

        [JsonProperty("arb_potential_profit_team_no_fav")]
        public decimal ArbPotentialProfitTeamNoFav { get; set; }

        [JsonProperty("arb_match")]
        public MatchApi ArbMatch { get; set; }

        [JsonProperty("odd_bookmaker_1")]
        public OddApi OddBookmaker1 { get; set; }

        [JsonProperty("odd_bookmaker_2")]
        public OddApi OddBookmaker2 { get; set; }

        [JsonProperty("error_status_1")]
        public string ErrorStatus1 { get; set; }

        [JsonProperty("error_status_2")]
        public string ErrorStatus2 { get; set; }

        public OpportunitiesApi()
        {

        }
    }

    public class MatchApi
    {
        [JsonProperty("match_name")]
        public string MatchName { get; set; }

        [JsonProperty("match_gui")]
        public string MatchGui { get; set; }

        [JsonProperty("match_team_fav")]
        public string MatchTeamFav { get; set; }

        [JsonProperty("match_team_no_fav")]
        public string MatchTeamNoFav { get; set; }

        [JsonProperty("match_league")]
        public string MatchLeague { get; set; }

        [JsonProperty("match_date")]
        public string MatchDate { get; set; }
    }

    public class OddApi
    {
        [JsonProperty("odd_fav")]
        public decimal OddFav { get; set; }

        [JsonProperty("odd_no_fav")]
        public decimal OddNoFav { get; set; }
    }



    public class BetsApi
    {
        [JsonProperty("bet_1")]
        public BetApi Bet1 { get; set; }

        [JsonProperty("bet_2")]
        public BetApi Bet2 { get; set; }

        [JsonProperty("arbitrage_gui")]
        public string ArbitrageGui { get; set; }

        [JsonProperty("match_time")]
        public string MatchTime { get; set; }

    }

    public class BetApi
    {
        [JsonProperty("bet_account")]
        public string BetAccount { get; set; }

        [JsonProperty("bet_bookmaker")]
        public string BetBoomaker { get; set; }

        [JsonProperty("bet_match")]
        public string BetMatch { get; set; }

        [JsonProperty("bet_team")]
        public string BetTeam { get; set; }

        [JsonProperty("bet_type_odd")]
        public string BetTypeOdd { get; set; }

        [JsonProperty("bet_odd")]
        public decimal BetOdd { get; set; }

        [JsonProperty("bet_price")]
        public decimal BetPrice { get; set; }

        [JsonProperty("bet_status")]
        public int BetStatus { get; set; }

        [JsonProperty("bet_is_live")]
        public int BetIsLive { get; set; }
    }

    public class ResultApi
    {
        [JsonProperty("accounts")]
        public List<AccountData> AccountsApi { get; set; }
    }

}
