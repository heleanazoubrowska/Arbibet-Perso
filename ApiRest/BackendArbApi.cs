using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using ArbibetProgram.Models;
using System.Net;
using System.Linq;
using ArbibetProgram.Functions;

namespace ArbibetProgram.ApiRest
{

    public class Payload
    {
        [JsonProperty("arb_bookmaker_1")]
        public string ArbBookmaker1 { get; set; }

        [JsonProperty("arb_bookmaker_2")]
        public string ArbBookmaker2 { get; set; }

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
    }

    public class MatchApi
    {
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

    public class PayloadBets
    {
        [JsonProperty("bet_1")]
        public BetApi Bet1 { get; set; }

        [JsonProperty("bet_2")]
        public BetApi Bet2 { get; set; }
    }

    public class BetApi
    {
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
    }

    public class ResultBets
    {
        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("bookmaker_name")]
        public string BookmakerName { get; set; }

        [JsonProperty("bet_match")]
        public string BetMatch { get; set; }

        [JsonProperty("bet_found")]
        public string BetFound { get; set; }

        [JsonProperty("bet_waiting")]
        public string BetWaiting { get; set; }

        [JsonProperty("bet_reject")]
        public string BetRejected { get; set; }

        [JsonProperty("bet_confirmed")]
        public string BetConfirmed { get; set; }
    }

    public class BackendArbApi
    {
        // sn: added definition
        static readonly HttpClient client = new HttpClient()
        {
            BaseAddress = new Uri("https://bet.arbicap.com/api/"),
            MaxResponseContentBufferSize = 1000000
        };

        public BackendArbApi()
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        // sn: changed Task<dynamic> to Task<string>
        public static async Task<string> sendRequestAsync(string url, string stringPayload)
        {
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            NLogger.Log(EventLevel.Debug,httpContent.ToString());

            // ---------------------------------------------------------------------
            #region sn: replaced new httpclient for each call with shared httpclient
            // ---------------------------------------------------------------------
            /*
			string fullUrl = "https://bet.arbicap.com/api/" + url;
			using (var httpClient = new HttpClient())
			{
				// Do the actual request and await the response
				var httpResponse = await httpClient.PostAsync(fullUrl, httpContent);
				// If the response contains content we want to read it!
				if (httpResponse.Content != null)
				{
					var responseContent = await httpResponse.Content.ReadAsStringAsync();
					//NLogger.Log(EventLevel.Debug,responseContent);
					return responseContent;

					// From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net
				}
				else
				{
					return "error";
				}
			}
            */
            #endregion sn: replaced new httpclient for each call with shared httpclient
            // ---------------------------------------------------------------------

            string prv_RetVal = "";

            try
			{
                // Send request asynchronously, continue when complete
                using HttpResponseMessage prv_Result = await client.PostAsync(url, httpContent);
                {
                    if (prv_Result.Content != null)
                    {
                        prv_RetVal = await prv_Result.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        prv_RetVal = "error";
                    }
                }
            }
			catch (Exception prv_Exception)
			{
                prv_RetVal = prv_Exception.Message;
			}
            return prv_RetVal;
        }

        // sn: made sendRequestGet asynchronous
        private static async Task<string> sendRequestGet(string url)
        {
            // ---------------------------------------------------------------------
            #region sn: replaced new httpclient for each call with shared httpclient
            // ---------------------------------------------------------------------
            /*
            string fullUrl = "https://bet.arbicap.com/api/" + url;
            using (var client = new WebClient())
            {

                return client.DownloadString(fullUrl);
            }
            */
            #endregion sn: replaced new httpclient for each call with shared httpclient
            // ---------------------------------------------------------------------

            string prv_RetVal = "";
			try
			{
                // Send request asynchronously, continue when complete
                using HttpResponseMessage prv_Result = await client.GetAsync(url);
                if (prv_Result.Content != null)
                {
                    prv_RetVal = await prv_Result.Content.ReadAsStringAsync();
                }
            }
            catch (Exception prv_Exception)
            {
                prv_RetVal = prv_Exception.Message;
            }
            return prv_RetVal;
        }

        // sn: made sendBet asynchronous
        public static async void sendBet(
            string bookmaker1,
            string bookmaker2,
            string match,
            string team1,
            string team2,
            string typeOdd,
            decimal odd1,
            decimal odd2,
            decimal price,
            int status1,
            int status2
            )
        {
            var payload = new PayloadBets
            {
                Bet1 = new BetApi
                {
                    BetBoomaker = bookmaker1,
                    BetMatch = match,
                    BetTeam = team1,
                    BetTypeOdd = typeOdd,
                    BetOdd = odd1,
                    BetPrice = price,
                    BetStatus = status1

                },

                Bet2 = new BetApi
                {
                    BetBoomaker = bookmaker2,
                    BetMatch = match,
                    BetTeam = team2,
                    BetTypeOdd = typeOdd,
                    BetOdd = odd2,
                    BetPrice = price,
                    BetStatus = status2

                },
            };

            var stringPayload = JsonConvert.SerializeObject(payload);

            await sendRequestAsync("Insert_Bet", stringPayload);
        }

        // sn: made sendArb asynchronous
        public static async void sendArb(
            string arbBookmaker1,
            string arbBookmaker2,
            string arbGame,
            Match bookmaker1,
            Match bookmaker2,
            Odd odd1,
            Odd odd2,
            string arbGui,
            decimal arbProfitTeamFav,
            decimal arbProfitTeamNoFav,
            string matchId
            )
        {
            var payload = new Payload
            {
                ArbBookmaker1 = arbBookmaker1,
                ArbBookmaker2 = arbBookmaker2,
                ArbGame = arbGame,
                ArbType = odd1.oddType,
                ArbGui = arbGui,
                ArbPotentialProfitTeamFav = arbProfitTeamFav,
                ArbPotentialProfitTeamNoFav = arbProfitTeamNoFav,
                ArbMatch = new MatchApi
                {
                    MatchGui = matchId,
                    MatchTeamFav = bookmaker1.TeamFav.TeamName,
                    MatchTeamNoFav = bookmaker1.TeamNoFav.TeamName,
                    MatchLeague = bookmaker1.Matchleague,
                    MatchDate = "no date"

                },
                OddBookmaker1 = new OddApi
                {
                    OddFav = odd1.oddFav,
                    OddNoFav = odd1.oddNoFav
                },
                OddBookmaker2 = new OddApi
                {
                    OddFav = odd2.oddFav,
                    OddNoFav = odd2.oddNoFav
                }
            };

            var stringPayload = JsonConvert.SerializeObject(payload);

            await sendRequestAsync("Insert_Arbitrage", stringPayload);
        }

        // sn: added .Result to wait for sendRequestGet result
        // sn: let getLastBets handle the conversion to List
        public static List<ResultBets> getLastBets()
        {
            string prv_Json = sendRequestGet("Get_Bets_Match").Result;
            var prv_BetList = JsonConvert.DeserializeObject<IEnumerable<ResultBets>>(prv_Json).ToList();
            return prv_BetList;
        }

        // sn: added .Result to wait for sendRequestGet result
        // sn: let getAccounts handle the conversion to List
        public static List<AccountData> getAccounts(string prm_DemoMode)
        {
            string prv_Json = sendRequestGet($"Get_Accounts/{prm_DemoMode}").Result;
            //NLogger.Log(EventLevel.Debug,prv_Json);

            var prv_AccountList = JsonConvert.DeserializeObject<IEnumerable<AccountData>>(prv_Json).ToList();
            NLogger.Log(EventLevel.Info, JsonConvert.SerializeObject(prv_AccountList, Formatting.Indented));
            
            return prv_AccountList;
        }


        public static async Task<string> checkApi()
        {
            var stringPayload = JsonConvert.SerializeObject(MainClass.DataApi);

            string result = await sendRequestAsync("Check_Api", stringPayload);
            return result;
        }

        public static async Task<string> sendOpportunity(BetsApi betsApi)
        {

            Console.WriteLine(JsonConvert.SerializeObject(betsApi, Formatting.Indented));
            var stringPayload = JsonConvert.SerializeObject(betsApi);

            string result = await sendRequestAsync("Insert_Opportunity", stringPayload);
            return result;
        }

        public static async Task<string> sendBetStatus(BetStatus betStatus)
        {

            Console.WriteLine(JsonConvert.SerializeObject(betStatus, Formatting.Indented));
            var stringPayload = JsonConvert.SerializeObject(betStatus);

            string result = await sendRequestAsync("Update_Bet_Status", stringPayload);
            return result;
        }

        public static async Task<string> sendReport()
        {
            var stringPayload = JsonConvert.SerializeObject(MainClass.ReportApi);
            string result = await sendRequestAsync("Send_Report", stringPayload);
            return result;
        }

    }
}
