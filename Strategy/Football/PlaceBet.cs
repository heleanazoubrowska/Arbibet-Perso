using System;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;

namespace ArbibetProgram.Strategy
{
	public static partial class Football
	{
        public static BetResult placeBet(int prm_Index_Bookmaker, string bookmaker, string match, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //BetResult betIsPlaced = new BetResult();

            //NLogger.Log(EventLevel.Critical, $"MainClass place bet {bookmaker}");
            BetResult betIsPlaced = MainClass.bookmakers[prm_Index_Bookmaker].ActiveAccount.API.placeBet(match, winOddType, winOdd, winOddFav, hedgeBet);


            watch.Stop();
            NLogger.Log(EventLevel.Info, $"Place bet {bookmaker} in {watch.ElapsedMilliseconds} ms");

            return betIsPlaced;
        }

        public static BetResult confirmBet(int prm_Index_Bookmaker, string bookmaker)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            BetResult betIsPlaced = new BetResult();

            var prv_Bookmaker = MainClass.bookmakers[prm_Index_Bookmaker];

            NLogger.Log(EventLevel.Critical, $"MainClass Confirm bet {bookmaker}");
            betIsPlaced = prv_Bookmaker.ActiveAccount.API.confirmBet();
            watch.Stop();
            NLogger.Log(EventLevel.Info, $"Confirm bet {bookmaker} in " + watch.ElapsedMilliseconds + " ms");

            return betIsPlaced;
        }

        public static void backToFrame(int prm_Index_Bookmaker)
        {
            MainClass.bookmakers[prm_Index_Bookmaker].ActiveAccount.API.backToFrame();
        }

    }
}
