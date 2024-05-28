using Newtonsoft.Json;
using System.Collections.Generic;
namespace ArbibetProgram.Models
{


    public class ReportApi
    {

        [JsonProperty("list_report")]
        public List<Report> ListReport { get; set; }

        public ReportApi()
        {
            ListReport = new List<Report>();
        }
    }

    public class Report
    {

        [JsonProperty("report_account")]
        public string RepportAccount { get; set; }

        [JsonProperty("report_bookmaker")]
        public string RepportBookmaker { get; set; }

        [JsonProperty("report_date")]
        public string ReportDate { get; set; }

        [JsonProperty("report_total")]
        public decimal ReportTotal { get; set; }

        [JsonProperty("report_bets_list")]
        public List<ReportBetList> ReportBetList { get; set; }

        public Report()
        {
            ReportBetList = new List<ReportBetList>();
        }
    }

    public class ReportBetList
    {

        [JsonProperty("report_bet_match")]
        public string ReportBetMatch { get; set; }

        [JsonProperty("report_bet_team")]
        public string ReportBetTeam { get; set; }

        [JsonProperty("report_bet_odd_type")]
        public string ReportBetOddType { get; set; }

        [JsonProperty("report_bet_odd_bet")]
        public string ReportBetOddBet { get; set; }

        [JsonProperty("report_bet_win_lose")]
        public string ReportBetWinLose { get; set; }

        [JsonProperty("report_bet_win_lose_price")]
        public decimal ReportBetWinLosePrice { get; set; }

        public ReportBetList()
        {

        }
    }

}
