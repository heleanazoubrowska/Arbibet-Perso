using System;
namespace ArbibetProgram.Models
{
    public class BetResult
    {

        public int BetStatus { get; set; }
        public string BetMessage { get; set; }
        public bool BetIsPlaced { get; set; }
        public bool BetIsConfirmed { get; set; }
        public decimal BetMaxBet { get; set; }
        public string BetHdpSens { get; set; }
        public int BetErrorStatus { get; set; }
        public string BetTeamFav { get; set; }
        public string BetTeamNoFav { get; set; }

        public BetResult()
        {
        }
    }
}
